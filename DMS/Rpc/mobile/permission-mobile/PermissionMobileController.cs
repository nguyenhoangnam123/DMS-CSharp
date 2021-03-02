using DMS.Common;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public partial class PermissionMobileController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        private DataContext DataContext;
        public PermissionMobileController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext, 
            DataContext DataContext)
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
            this.DataContext = DataContext;
        }
        [Route(PermissionMobileRoute.ListCurrentKpiGeneral), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiGeneralReportDTO>> GetCuGetCurrentKpiGeneralrentMonthKpi([FromBody] PermissionMobile_EmployeeKpiGeneralReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiGenerals = new List<PermissionMobile_EmployeeKpiGeneralReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            DateTime Now = LocalStartDay(CurrentContext);
            int CurrentMonth = Now.Month;
            int CurrentYear = Now.Year;
            DateTime FirstDayOfMonth = GetFirstDayOfMonth(CurrentYear, CurrentMonth, CurrentContext.TimeZone); // lấy ra ngày đầu của tháng
            DateTime LastDayOfMonth = GetLastDayOfMonth(CurrentYear, CurrentMonth, CurrentContext.TimeZone); ; // lấy ra ngày cuối của tháng
            int KpiPeriodId = CurrentMonth + 100;
            // lấy ra kpiYear, lấy ra kpiPeriod theo tháng hiện tại
            long CurrentKpiYearId = await DataContext.KpiYear.Where(x => x.Id == CurrentYear).Select(o => o.Id).FirstOrDefaultAsync();

            // lấy ra số KpiGeneral theo kế hoạch
            // lấy ra KpiGeneral bằng filter theo AppUserId, trạng thái = true, kpiYearId
            long KpiGeneralId = await DataContext.KpiGeneral.Where(x => x.EmployeeId == PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.StatusId == StatusEnum.ACTIVE.Id && x.KpiYearId == CurrentKpiYearId).Select(p => p.Id).FirstOrDefaultAsync();
            if (KpiGeneralId > 0)
            {
                // lấy ra toàn bộ KpiGeneralContent bằng filter KpiGeneral, KpiCriteriaGeneralId in [], trạng thái = true,
                List<long> KpiCriticalPermissionListIds = new List<long>
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
                        KpiCriticalPermissionListIds.Contains(x.KpiCriteriaGeneralId))
                    .ToListAsync();
                List<long> KpiGeneralContentIds = KpiGeneralContentDAOs.Select(x => x.Id).ToList();
                // lấy ra toàn bộ KpiGeneralContentKpiPeriodMappings bằng filter theo KpiGeneralContent và theo kì
                List<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappingDAOs = await DataContext.KpiGeneralContentKpiPeriodMapping
                    .Where(x =>
                        KpiGeneralContentIds.Contains(x.KpiGeneralContentId) &&
                        x.KpiPeriodId == KpiPeriodId)
                    .ToListAsync();
                // lấy ra toàn bộ storeChecking để tính số liệu thực hiện bằng filter SaleEmployeeId
                List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                    .Where(x =>
                        x.SaleEmployeeId == PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                        x.CheckInAt >= FirstDayOfMonth &&
                        x.CheckOutAt <= LastDayOfMonth)
                    .ToListAsync();
                int NumberOfStoreVisit =
                    StoreCheckingDAOs.Count; // NUMBER_OF_STORE_VISIT
                int StoreVisited = StoreCheckingDAOs.GroupBy(x => x.StoreId).Count(); // STORE_VISITED

                List<decimal> IndirectSalesOrderTotals = await DataContext.IndirectSalesOrder
                    .Where(x =>
                        x.SaleEmployeeId == PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                        x.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        x.CreatedAt >= FirstDayOfMonth &&
                        x.CreatedAt <= LastDayOfMonth)
                    .Select(x => x.Total).ToListAsync();


                decimal TotalIndirectSalesAmount = IndirectSalesOrderTotals.Sum(); // TOTAL_INDIRECT_SALES_AMOUNT
                List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => x.AppUserId == PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal && x.CreatedAt >= FirstDayOfMonth && x.CreatedAt <= LastDayOfMonth).ToListAsync();
                var NewStoreCreated = StoreDAOs.Count(); // NEW_STORE_CREATED

                List<KpiCriteriaGeneralDAO> KpiCriteriaGeneralDAOs = await DataContext.KpiCriteriaGeneral.ToListAsync();
                // loops mappings và lấy ra giá trị kế hoạch
                foreach (KpiGeneralContentKpiPeriodMappingDAO KpiGeneralContentKpiPeriodMapping in KpiGeneralContentKpiPeriodMappingDAOs)
                {
                    PermissionMobile_EmployeeKpiGeneralReportDTO PermissionMobile_EmployeeKpiGeneralReportDTO = new PermissionMobile_EmployeeKpiGeneralReportDTO();
                    long KpiCriteriaGeneralId = KpiGeneralContentDAOs
                        .Where(x => x.Id == KpiGeneralContentKpiPeriodMapping.KpiGeneralContentId)
                        .Select(x => x.KpiCriteriaGeneralId).FirstOrDefault();

                    KpiCriteriaGeneralDAO KpiCriteriaGeneralDAO = KpiCriteriaGeneralDAOs.Where(x => x.Id == KpiCriteriaGeneralId).FirstOrDefault();
                    PermissionMobile_EmployeeKpiGeneralReportDTO.KpiCriteriaGeneralName = KpiCriteriaGeneralDAO.Name;
                    PermissionMobile_EmployeeKpiGeneralReportDTO.PlannedValue = KpiGeneralContentKpiPeriodMapping.Value ?? 0;

                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = TotalIndirectSalesAmount;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = NewStoreCreated;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreVisited;
                    }
                    if (KpiGeneralContentKpiPeriodMapping.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = NumberOfStoreVisit;
                    }
                    PermissionMobile_EmployeeKpiGeneralReportDTO.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiGeneralReportDTO.PlannedValue, PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue); // tính ra phần trăm thực hiện
                    KpiGenerals.Add(PermissionMobile_EmployeeKpiGeneralReportDTO);
                }
            } // nếu có kpi chung tương ứng với nhân viên + trạng thái + năm kpi
            return KpiGenerals;
        }

        [Route(PermissionMobileRoute.ListCurrentKpiItem), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiItemReportDTO>> GetCurrentKpiItem([FromBody] PermissionMobile_EmployeeKpiItemReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiItemDTOs = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
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
                    x.EmployeeId == PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentKpiYearId)
                .Select(p => p.Id).FirstOrDefaultAsync();

            if (KpiItemId > 0)
            {
                List<KpiItemContentDAO> KpiItemContentDAOs = await DataContext.KpiItemContent.Where(x => x.KpiItemId == KpiItemId).ToListAsync();
                List<long> KpiItemContentIds = KpiItemContentDAOs.Select(x => x.Id).ToList();
                List<long> KpiCriticalPermissionListIds = new List<long>
                {
                    KpiCriteriaItemEnum.INDIRECT_REVENUE.Id, // Doanh thu sản phẩm theo đơn hàng gián tiếp
                    KpiCriteriaItemEnum.INDIRECT_STORE.Id // số đại lý theo đơn gián tiếp
                };
                if (KpiItemContentIds.Count > 0)
                {
                    List<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMappingDAOs = await DataContext.KpiItemContentKpiCriteriaItemMapping
                        .Where(x => KpiItemContentIds.Contains(x.KpiItemContentId) && KpiCriticalPermissionListIds.Contains(x.KpiCriteriaItemId))
                        .Include(x => x.KpiItemContent)
                        .ToListAsync();
                    List<long> KpiCriteriaItemIds = KpiItemContentKpiCriteriaItemMappingDAOs.Select(x => x.KpiCriteriaItemId).Distinct().ToList();
                    List<KpiCriteriaItemDAO> KpiCriteriaItemDAOs = await DataContext.KpiCriteriaItem.Where(x => KpiCriteriaItemIds.Contains(x.Id)).ToListAsync();
                    List<PermissionMobile_EmployeeKpiItem> PermissionMobile_EmployeeKpiItems = new List<PermissionMobile_EmployeeKpiItem>();
                    if (KpiItemContentKpiCriteriaItemMappingDAOs.Count > 0)
                    {
                        List<long> ItemIds = KpiItemContentDAOs.Select(x => x.ItemId).ToList(); // lẩy ra list itemId theo chỉ tiêu
                        List<ItemDAO> ItemDAOs = await DataContext.Item.Where(x => ItemIds.Contains(x.Id)).ToListAsync(); // lẩy ra list items 
                                                                                                                          // lấy ra các đơn hàng do user tạo + trong tháng + được ghi nhận + có id thuộc list id trên


                        List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                            .Where(x =>
                                ItemIds.Contains(x.ItemId) &&
                                x.IndirectSalesOrder.SaleEmployeeId == PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal &&
                                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id &&
                                x.IndirectSalesOrder.CreatedAt >= FirstDayOfMonth &&
                                x.IndirectSalesOrder.CreatedAt <= LastDayOfMonth
                            ).Include(x => x.IndirectSalesOrder).ToListAsync();

                        foreach (KpiItemContentKpiCriteriaItemMappingDAO KpiItemContentKpiCriteriaItemMappingDAO in KpiItemContentKpiCriteriaItemMappingDAOs)
                        {
                            PermissionMobile_EmployeeKpiItem PermissionMobile_EmployeeKpiItem = new PermissionMobile_EmployeeKpiItem();
                            KpiCriteriaItemDAO KpiCriteriaItemDAO = KpiCriteriaItemDAOs.Where(x => x.Id == KpiItemContentKpiCriteriaItemMappingDAO.KpiCriteriaItemId).FirstOrDefault();
                            PermissionMobile_EmployeeKpiItem.KpiCriteriaItemName = KpiCriteriaItemDAO.Name;
                            PermissionMobile_EmployeeKpiItem.ItemId = KpiItemContentDAOs.Where(x => x.Id == KpiItemContentKpiCriteriaItemMappingDAO.KpiItemContentId).Select(p => p.ItemId).FirstOrDefault(); // lấy ra itemId từ filter ContentDAO
                            PermissionMobile_EmployeeKpiItem.PlannedValue = KpiItemContentKpiCriteriaItemMappingDAO.Value ?? 0;
                            if (KpiItemContentKpiCriteriaItemMappingDAO.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                            {
                                PermissionMobile_EmployeeKpiItem.CurrentValue = IndirectSalesOrderContentDAOs
                                    .Where(x => x.ItemId == KpiItemContentKpiCriteriaItemMappingDAO.KpiItemContent.ItemId)
                                    .Select(x => x.Amount - x.GeneralDiscountAmount.Value).Sum();
                            }
                            if (KpiItemContentKpiCriteriaItemMappingDAO.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                            {
                                PermissionMobile_EmployeeKpiItem.CurrentValue = IndirectSalesOrderContentDAOs
                                    .Where(x => x.ItemId == KpiItemContentKpiCriteriaItemMappingDAO.KpiItemContent.ItemId)
                                    .Select(x => x.IndirectSalesOrder.BuyerStoreId).Distinct().Count();
                            }
                            PermissionMobile_EmployeeKpiItem.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiItem.PlannedValue, PermissionMobile_EmployeeKpiItem.CurrentValue); // tính ra phần trăm thực hiện
                            PermissionMobile_EmployeeKpiItems.Add(PermissionMobile_EmployeeKpiItem);
                        }
                        foreach (var Item in ItemDAOs)
                        {
                            PermissionMobile_EmployeeKpiItemReportDTO PermissionMobile_EmployeeKpiItemReportDTO = new PermissionMobile_EmployeeKpiItemReportDTO();
                            PermissionMobile_EmployeeKpiItemReportDTO.ItemName = Item.Name;
                            PermissionMobile_EmployeeKpiItemReportDTO.CurrentKpiItems = PermissionMobile_EmployeeKpiItems.Where(x => x.ItemId == Item.Id).ToList();
                            KpiItemDTOs.Add(PermissionMobile_EmployeeKpiItemReportDTO);
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
