using DMS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MProblem;
using DMS.Services.MProduct;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MSurvey;
using DMS.Services.MTaxType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.Services.MBanner;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DMS.Services.MStoreScouting;
using DMS.Services.MProvince;
using DMS.Services.MDistrict;
using DMS.Services.MWard;
using DMS.Services.MBrand;
using DMS.Services.MSupplier;
using DMS.Services.MProductGrouping;
using System.Text;
using DMS.Services.MNotification;
using DMS.Services.MProblemType;
using DMS.Services.MColor;
using DMS.Models;
using GeoCoordinatePortable;
using DMS.Services.MStoreScoutingType;
using DMS.Services.MStoreStatus;
using DMS.Services.MRewardHistory;
using DMS.Services.MLuckyNumber;
using DMS.Helpers;
using System.Dynamic;
using System.Net.Mime;
using GleamTech.DocumentUltimate;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace DMS.Rpc.mobile
{
    public partial class MobileController : SimpleController
    {

        [Route(MobileRoute.ListCurrentKpiGeneral), HttpPost]
        public async Task<List<Mobile_EmployeeKpiGeneralReportDTO>> GetCuGetCurrentKpiGeneralrentMonthKpi([FromBody] Mobile_EmployeeKpiGeneralReportFilterDTO Mobile_EmployeeKpiFilterDTO)
        {
            var KpiGenerals = new List<Mobile_EmployeeKpiGeneralReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            DateTime Now = LocalStartDay(CurrentContext);
            int CurrentMonth = Now.Month;
            int CurrentYear = Now.Year;
            DateTime FirstDayOfMonth = GetFirstDayOfMonth(CurrentYear, CurrentMonth, CurrentContext.TimeZone); // lấy ra ngày đầu của tháng
            DateTime LastDayOfMonth = GetLastDayOfMonth(CurrentYear, CurrentMonth, CurrentContext.TimeZone); ; // lấy ra ngày cuối của tháng
            int KpiPeriodId = CurrentMonth + 100;
            // lấy ra kpiYear, lấy ra kpiPeriod theo tháng hiện tại
            long CurrentKpiYearId = await DataContext.KpiYear.Where(x => x.Id == CurrentYear).Select(o => o.Id).FirstOrDefaultAsync();

            // lấy ra số kpiGeneral theo kế hoạch
            // lấy ra kpiGeneral bằng filter theo AppUserId, trạng thái = true, kpiYearId
            long KpiGeneralId = await DataContext.KpiGeneral.Where(x => x.EmployeeId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.StatusId == StatusEnum.ACTIVE.Id && x.KpiYearId == CurrentKpiYearId).Select(p => p.Id).FirstOrDefaultAsync();
            if (KpiGeneralId > 0)
            {
                // lấy ra toàn bộ kpiGeneralContent bằng filter kpiGeneral, KPICriteriaGeneralId in [], trạng thái = true,
                List<long> KpiCriticalGeneralListIds = new List<long>
                {
                    KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id, // Doanh thu đơn hàng gián tiếp
                    KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id, // Số đại lý tạo mới
                    KpiCriteriaGeneralEnum.STORE_VISITED.Id, // Số đại lý viếng thăm
                    KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id, // Số lần viếng thăm đại lý

                };
                List<KpiGeneralContentDAO> KpiGeneralContentDAOs = await DataContext.KpiGeneralContent
                    .Where(x =>
                        x.KpiGeneralId == KpiGeneralId &&
                        x.StatusId == StatusEnum.ACTIVE.Id &&
                        KpiCriticalGeneralListIds.Contains(x.KpiCriteriaGeneralId))
                    .ToListAsync();
                List<long> KpiGeneralContentIds = KpiGeneralContentDAOs.Select(x => x.Id).ToList();
                // lấy ra toàn bộ kpiGeneralContentKpiPeriodMappings bằng filter theo kpiGeneralContent và theo kì
                List<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappingDAOs = await DataContext.KpiGeneralContentKpiPeriodMapping
                    .Where(x =>
                        KpiGeneralContentIds.Contains(x.KpiGeneralContentId) &&
                        x.KpiPeriodId == KpiPeriodId)
                    .ToListAsync();
                // lấy ra toàn bộ storeChecking để tính số liệu thực hiện bằng filter SaleEmployeeId
                List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                    .Where(x =>
                        x.SaleEmployeeId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                        x.CheckInAt >= FirstDayOfMonth &&
                        x.CheckOutAt <= LastDayOfMonth)
                    .ToListAsync();
                int NumberOfStoreVisit =
                    StoreCheckingDAOs.Count; // NUMBER_OF_STORE_VISIT
                int StoreVisited = StoreCheckingDAOs.GroupBy(x => x.StoreId).Count(); // STORE_VISITED
                List<decimal> IndirectSalesOrderTotals = await DataContext.IndirectSalesOrder
                    .Where(x =>
                        x.CreatorId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                        x.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        x.CreatedAt >= FirstDayOfMonth &&
                        x.CreatedAt <= LastDayOfMonth)
                    .Select(x => x.Total).ToListAsync();

                decimal TotalIndirectSalesAmount = IndirectSalesOrderTotals.Sum(); // TOTAL_INDIRECT_SALES_AMOUNT
                List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => x.AppUserId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.CreatedAt >= FirstDayOfMonth && x.CreatedAt <= LastDayOfMonth).ToListAsync();
                var NewStoreCreated = StoreDAOs.Count(); // NEW_STORE_CREATED

                List<KpiCriteriaGeneralDAO> KpiCriteriaGeneralDAOs = await DataContext.KpiCriteriaGeneral.ToListAsync();
                // loops mappings và lấy ra giá trị kế hoạch
                foreach (KpiGeneralContentKpiPeriodMappingDAO KpiGeneralContentKpiPeriodMapping in KpiGeneralContentKpiPeriodMappingDAOs)
                {
                    Mobile_EmployeeKpiGeneralReportDTO Mobile_EmployeeKpiGeneralReportDTO = new Mobile_EmployeeKpiGeneralReportDTO();
                    long KpiCriteriaGeneralId = KpiGeneralContentDAOs
                        .Where(x => x.Id == KpiGeneralContentKpiPeriodMapping.KpiGeneralContentId)
                        .Select(x => x.KpiCriteriaGeneralId).FirstOrDefault();

                    KpiCriteriaGeneralDAO KpiCriteriaGeneralDAO = KpiCriteriaGeneralDAOs.Where(x => x.Id == KpiCriteriaGeneralId).FirstOrDefault();
                    Mobile_EmployeeKpiGeneralReportDTO.KpiCriticalGeneralName = KpiCriteriaGeneralDAO.Name;
                    Mobile_EmployeeKpiGeneralReportDTO.PlannedValue = KpiGeneralContentKpiPeriodMapping.Value ?? 0;

                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                    {
                        Mobile_EmployeeKpiGeneralReportDTO.CurrentValue = TotalIndirectSalesAmount;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                    {
                        Mobile_EmployeeKpiGeneralReportDTO.CurrentValue = NewStoreCreated;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                    {
                        Mobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreVisited;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                    {
                        Mobile_EmployeeKpiGeneralReportDTO.CurrentValue = NumberOfStoreVisit;
                    }
                    Mobile_EmployeeKpiGeneralReportDTO.Percentage = CalculatePercentage(Mobile_EmployeeKpiGeneralReportDTO.PlannedValue, Mobile_EmployeeKpiGeneralReportDTO.CurrentValue); // tính ra phần trăm thực hiện
                    KpiGenerals.Add(Mobile_EmployeeKpiGeneralReportDTO);
                }
            } // nếu có kpi chung tương ứng với nhân viên + trạng thái + năm kpi
            return KpiGenerals;
        }

        [Route(MobileRoute.ListCurrentKpiItem), HttpPost]
        public async Task<List<Mobile_EmployeeKpiItemReportDTO>> GetCurrentKpiItem([FromBody] Mobile_EmployeeKpiItemReportFilterDTO Mobile_EmployeeKpiFilterDTO)
        {
            var KpiItemDTOs = new List<Mobile_EmployeeKpiItemReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            DateTime Now = LocalStartDay(CurrentContext);
            int CurrentMonth = Now.Month;
            int CurrentYear = Now.Year;
            DateTime FirstDayOfMonth = GetFirstDayOfMonth(CurrentYear, CurrentMonth, CurrentContext.TimeZone); // lấy ra ngày đầu của tháng
            DateTime LastDayOfMonth = GetLastDayOfMonth(CurrentYear, CurrentMonth, CurrentContext.TimeZone); ; // lấy ra ngày cuối của tháng
            int KpiPeriodId = CurrentMonth + 100;
            // lấy ra kpiYear, lấy ra kpiPeriod theo tháng hiện tại
            long CurrentKpiYearId = await DataContext.KpiYear.Where(x => x.Id == CurrentYear).Select(o => o.Id).FirstOrDefaultAsync();

            long KpiItemId = await DataContext.KpiItem.Where(x =>
                    x.EmployeeId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentKpiYearId)
                .Select(p => p.Id).FirstOrDefaultAsync();

            if (KpiItemId > 0)
            {
                List<KpiItemContentDAO> KpiItemContentDAOs = await DataContext.KpiItemContent.Where(x => x.KpiItemId == KpiItemId).ToListAsync();
                List<long> KpiItemContentIds = KpiItemContentDAOs.Select(x => x.Id).ToList();
                List<long> KpiCriticalGeneralListIds = new List<long>
                {
                    KpiCriteriaItemEnum.INDIRECT_REVENUE.Id, // Doanh thu sản phẩm theo đơn hàng gián tiếp
                    KpiCriteriaItemEnum.INDIRECT_STORE.Id // số đại lý theo đơn gián tiếp
                };
                if (KpiItemContentIds.Count > 0)
                {
                    List<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMappingDAOs = await DataContext.KpiItemContentKpiCriteriaItemMapping
                        .Where(x => KpiItemContentIds.Contains(x.KpiItemContentId) && KpiCriticalGeneralListIds.Contains(x.KpiCriteriaItemId))
                        .ToListAsync();
                    List<long> KpiCriteriaItemIds = KpiItemContentKpiCriteriaItemMappingDAOs.Select(x => x.KpiCriteriaItemId).Distinct().ToList();
                    List<KpiCriteriaItemDAO> KpiCriteriaItemDAOs = await DataContext.KpiCriteriaItem.Where(x => KpiCriteriaItemIds.Contains(x.Id)).ToListAsync();
                    List<Mobile_EmployeeKpiItem> Mobile_EmployeeKpiItems = new List<Mobile_EmployeeKpiItem>();
                    if (KpiItemContentKpiCriteriaItemMappingDAOs.Count > 0)
                    {
                        List<long> ItemIds = KpiItemContentDAOs.Select(x => x.ItemId).ToList(); // lẩy ra list itemId theo chỉ tiêu
                        List<ItemDAO> ItemDAOs = await DataContext.Item.Where(x => ItemIds.Contains(x.Id)).ToListAsync(); // lẩy ra list items 
                        // lấy ra các đơn hàng do user tạo + trong tháng + được ghi nhận + có id thuộc list id trên
                        List<IndirectSalesOrderDAO> IndirectSalesDAOs = await (from indirect in DataContext.IndirectSalesOrder
                                                                               join content in DataContext.IndirectSalesOrderContent on indirect.Id equals content.IndirectSalesOrderId
                                                                               where ItemIds.Contains(content.ItemId) &&
                                                                                    indirect.SaleEmployeeId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                                                                                    indirect.RequestStateId == RequestStateEnum.APPROVED.Id &&
                                                                                    indirect.CreatedAt >= FirstDayOfMonth &&
                                                                                    indirect.CreatedAt <= LastDayOfMonth
                                                                               select indirect).ToListAsync();

                        decimal IndirectRevenue = IndirectSalesDAOs.Select(i => i.Total).Sum();
                        int IndirectStore = IndirectSalesDAOs.GroupBy(i => i.BuyerStoreId).Count();
                        foreach (KpiItemContentKpiCriteriaItemMappingDAO KpiItemContentKpiCriteriaItemMapping in KpiItemContentKpiCriteriaItemMappingDAOs)
                        {
                            Mobile_EmployeeKpiItem Mobile_EmployeeKpiItem = new Mobile_EmployeeKpiItem();
                            KpiCriteriaItemDAO KpiCriteriaItemDAO = KpiCriteriaItemDAOs.Where(x => x.Id == KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId).FirstOrDefault();
                            Mobile_EmployeeKpiItem.KpiCriteriaItemName = KpiCriteriaItemDAO.Name;
                            Mobile_EmployeeKpiItem.ItemId = KpiItemContentDAOs.Where(x => x.Id == KpiItemContentKpiCriteriaItemMapping.KpiItemContentId).Select(p => p.ItemId).FirstOrDefault(); // lấy ra itemId từ filter ContentDAO
                            Mobile_EmployeeKpiItem.PlannedValue = KpiItemContentKpiCriteriaItemMapping.Value ?? 0;
                            if (KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                            {
                                Mobile_EmployeeKpiItem.CurrentValue = IndirectRevenue;
                            }
                            if (KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                            {
                                Mobile_EmployeeKpiItem.CurrentValue = IndirectStore;
                            }
                            Mobile_EmployeeKpiItem.Percentage = CalculatePercentage(Mobile_EmployeeKpiItem.PlannedValue, Mobile_EmployeeKpiItem.CurrentValue); // tính ra phần trăm thực hiện
                            Mobile_EmployeeKpiItems.Add(Mobile_EmployeeKpiItem);
                        }
                        foreach (var Item in ItemDAOs)
                        {
                            Mobile_EmployeeKpiItemReportDTO Mobile_EmployeeKpiItemReportDTO = new Mobile_EmployeeKpiItemReportDTO();
                            Mobile_EmployeeKpiItemReportDTO.ItemName = Item.Name;
                            Mobile_EmployeeKpiItemReportDTO.CurrentKpiItems = Mobile_EmployeeKpiItems.Where(x => x.ItemId == Item.Id).ToList();
                            KpiItemDTOs.Add(Mobile_EmployeeKpiItemReportDTO);
                        } // group các chỉ tiêu kpi theo item
                    }
                } // nếu có contents
            } // nếu có kpi sản phẩm tương ứng với nhân viên + trạng thái + năm kpi

            return KpiItemDTOs;
        }

        private DateTime GetFirstDayOfMonth(int Year, int Month, int TimeZone)
        {
            return new DateTime(Year, Month, 1).AddHours(TimeZone).Date.AddHours(0 - TimeZone);
        }

        private DateTime GetLastDayOfMonth(int Year, int Month, int TimeZone)
        {
            DateTime FirstDayOfMonth = new DateTime(Year, Month, 1).AddHours(TimeZone).Date.AddHours(0 - TimeZone);
            return FirstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(TimeZone).Date.AddHours(0 - TimeZone).AddDays(1).AddSeconds(-1);
        }

        private decimal CalculatePercentage(decimal PlannedValue, decimal CurrentValue)
        {
            if (PlannedValue > 0) return CurrentValue / PlannedValue * 100;
            return 0;
        } // trả về phần trăm thực hiện kế hoạch
    }
}

