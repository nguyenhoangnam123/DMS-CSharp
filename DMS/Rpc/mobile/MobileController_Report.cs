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
                List<long> KpiGeneralContentIds = await DataContext.KpiGeneralContent.Where(x => x.KpiGeneralId == KpiGeneralId && x.StatusId == StatusEnum.ACTIVE.Id && KpiCriticalGeneralListIds.Contains(x.KpiCriteriaGeneralId)).Select(x => x.Id).ToListAsync();
                // lấy ra toàn bộ kpiGeneralContentKpiPeriodMappings bằng filter theo kpiGeneralContent và theo kì
                List<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappingDAOs = await DataContext.KpiGeneralContentKpiPeriodMapping.Where(x => KpiGeneralContentIds.Contains(x.KpiGeneralContentId) && x.KpiPeriodId == KpiPeriodId).ToListAsync();
                // lấy ra toàn bộ storeChecking để tính số liệu thực hiện bằng filter SaleEmployeeId
                List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.Where(x => x.SaleEmployeeId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.CheckInAt >= FirstDayOfMonth && x.CheckOutAt <= LastDayOfMonth).ToListAsync();
                int NumberOfStoreVisit = StoreCheckingDAOs.Count; // NUMBER_OF_STORE_VISIT
                int StoreVisited = StoreCheckingDAOs.GroupBy(x => x.StoreId).Count(); // STORE_VISITED
                List<decimal> IndirectSalesOrderTotals = await DataContext.IndirectSalesOrder.Where(x => x.CreatorId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.RequestStateId == RequestStateEnum.APPROVED.Id && x.CreatedAt >= FirstDayOfMonth && x.CreatedAt <= LastDayOfMonth).Select(x => x.Total).ToListAsync();
                decimal TotalIndirectSalesAmount = IndirectSalesOrderTotals.Sum(); // TOTAL_INDIRECT_SALES_AMOUNT
                List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => x.AppUserId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.CreatedAt >= FirstDayOfMonth && x.CreatedAt <= LastDayOfMonth).ToListAsync();
                var NewStoreCreated = StoreDAOs.Count(); // NEW_STORE_CREATED
                // loops mappings và lấy ra giá trị kế hoạch
                foreach (KpiGeneralContentKpiPeriodMappingDAO KpiGeneralContentKpiPeriodMapping in KpiGeneralContentKpiPeriodMappingDAOs)
                {
                    var KpiGeneral = new Mobile_EmployeeKpiGeneralReportDTO();
                    KpiGeneral.KpiCriticalGeneralName = KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneral.Name;
                    KpiGeneral.PlannedValue = KpiGeneralContentKpiPeriodMapping.Value ?? 0; // nếu số ké hoạch = 0 thì thực hiện = 0
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                    {
                        KpiGeneral.CurrentValue = KpiGeneral.PlannedValue > 0 ? TotalIndirectSalesAmount : 0;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                    {
                        KpiGeneral.CurrentValue = KpiGeneral.PlannedValue > 0 ? NewStoreCreated: 0;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                    {
                        KpiGeneral.CurrentValue = KpiGeneral.PlannedValue > 0 ?  NumberOfStoreVisit : 0;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                    {
                        KpiGeneral.CurrentValue = KpiGeneral.PlannedValue > 0 ?  StoreVisited : 0;
                    }
                    KpiGeneral.Percentage = CalculatePercentage(KpiGeneral.PlannedValue, KpiGeneral.CurrentValue); // tính ra phần trăm thực hiện
                    KpiGenerals.Add(KpiGeneral);
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

            long KpiItemId = await DataContext.KpiItem.Where(x => x.EmployeeId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.StatusId == StatusEnum.ACTIVE.Id && x.KpiPeriodId == KpiPeriodId && x.KpiYearId == CurrentKpiYearId).Select(p => p.Id).FirstOrDefaultAsync();
            if (KpiItemId > 0)
            {
                List<long> KpiItemContentIds = await DataContext.KpiItemContent.Where(x => x.KpiItemId == KpiItemId).Select(p => p.Id).ToListAsync();
                List<long> KpiCriticalGeneralListIds = new List<long>
                {
                    KpiCriteriaItemEnum.INDIRECT_REVENUE.Id, // Doanh thu sản phẩm theo đơn hàng gián tiếp
                    KpiCriteriaItemEnum.INDIRECT_STORE.Id // số đại lý theo đơn gián tiếp
                };
                if (KpiItemContentIds.Count > 0)
                {
                    List<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMappingDAOs = await DataContext.KpiItemContentKpiCriteriaItemMapping.Where(x => KpiItemContentIds.Contains(x.KpiItemContentId) && KpiCriticalGeneralListIds.Contains(x.KpiCriteriaItemId)).ToListAsync();
                    var KpiItems = new List<Mobile_EmployeeKpiItem>();
                    if (KpiItemContentKpiCriteriaItemMappingDAOs.Count > 0)
                    {
                        List<long> ItemIds = KpiItemContentKpiCriteriaItemMappingDAOs.Select(x => x.KpiItemContent.ItemId).ToList(); // lẩy ra list itemId theo chỉ tiêu
                        List<long> IndirectSalesOrderIds = await DataContext.IndirectSalesOrderContent.Where(x => ItemIds.Contains(x.ItemId)).Select(p => p.IndirectSalesOrderId).ToListAsync(); // lấy ra list id của đơn hàng theo list itemId
                        // lấy ra các đơn hàng do user tạo + trong tháng + được ghi nhận + có id thuộc list id trên
                        List<IndirectSalesOrderDAO> IndirectSalesDAOs = await DataContext.IndirectSalesOrder.Where(x => x.CreatorId == Mobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.RequestStateId == RequestStateEnum.APPROVED.Id && x.CreatedAt >= FirstDayOfMonth && x.CreatedAt <= LastDayOfMonth && IndirectSalesOrderIds.Contains(x.Id)).ToListAsync();
                        decimal IndirectRevenue = IndirectSalesDAOs.Select(i => i.Total).Sum();
                        int IndirectStore = IndirectSalesDAOs.GroupBy(i => i.BuyerStoreId).Count();
                        foreach (KpiItemContentKpiCriteriaItemMappingDAO KpiItemContentKpiCriteriaItemMapping in KpiItemContentKpiCriteriaItemMappingDAOs)
                        {
                            var KpiItem = new Mobile_EmployeeKpiItem();
                            KpiItem.ItemId = KpiItemContentKpiCriteriaItemMapping.KpiItemContent.ItemId;
                            KpiItem.KpiCriteriaItemName = KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItem.Name;
                            KpiItem.PlannedValue = KpiItemContentKpiCriteriaItemMapping.Value ?? 0; // nếu số ké hoạch = 0 thì thực hiện = 0
                            if (KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                            {
                                KpiItem.CurrentValue = KpiItem.PlannedValue > 0 ? IndirectRevenue : 0;
                            }
                            if (KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                            {
                                KpiItem.CurrentValue = KpiItem.PlannedValue > 0 ?  IndirectStore : 0;
                            }
                            KpiItem.Percentage = CalculatePercentage(KpiItem.PlannedValue, KpiItem.CurrentValue); // tính ra phần trăm thực hiện
                            KpiItems.Add(KpiItem);
                        }

                        var ItemDAOs = await DataContext.Item.Where(x => ItemIds.Contains(x.Id)).ToListAsync();
                        foreach (var Item in ItemDAOs)
                        {
                            var kpiItemDTO = new Mobile_EmployeeKpiItemReportDTO();
                            kpiItemDTO.ItemName = Item.Name;
                            kpiItemDTO.CurrentKpiItems = KpiItems.Where(x => x.ItemId == Item.Id).ToList();
                            KpiItemDTOs.Add(kpiItemDTO);
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

