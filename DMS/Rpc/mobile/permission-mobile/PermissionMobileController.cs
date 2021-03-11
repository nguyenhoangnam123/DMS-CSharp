using DMS.Common;
using DMS.Enums;
using DMS.Helpers;
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
        public async Task<List<PermissionMobile_EmployeeKpiGeneralReportDTO>> ListCurrentKpiGeneral([FromBody] PermissionMobile_EmployeeKpiGeneralReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiGenerals = new List<PermissionMobile_EmployeeKpiGeneralReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }
            // lấy ra số KpiGeneral theo kế hoạch
            // lấy ra KpiGeneral bằng filter theo AppUserId, trạng thái = true, kpiYearId
            long KpiGeneralId = await DataContext.KpiGeneral.Where(x => 
                    AppUserIds.Contains(x.EmployeeId) && 
                    x.StatusId == StatusEnum.ACTIVE.Id && 
                    x.KpiYearId == CurrentYear.Id
                ).Select(p => p.Id).FirstOrDefaultAsync();
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
                        AppUserIds.Contains(x.SaleEmployeeId) &&
                        x.CheckInAt >= Start &&
                        x.CheckOutAt <= End)
                    .ToListAsync();
                int NumberOfStoreVisit =
                    StoreCheckingDAOs.Count; // NUMBER_OF_STORE_VISIT
                int StoreVisited = StoreCheckingDAOs.GroupBy(x => x.StoreId).Count(); // STORE_VISITED

                List<decimal> IndirectSalesOrderTotals = await DataContext.IndirectSalesOrder
                    .Where(x =>
                        AppUserIds.Contains(x.SaleEmployeeId) &&
                        x.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        x.CreatedAt >= Start &&
                        x.CreatedAt <= End)
                    .Select(x => x.Total).ToListAsync();


                decimal TotalIndirectSalesAmount = IndirectSalesOrderTotals.Sum(); // TOTAL_INDIRECT_SALES_AMOUNT
                List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x =>
                        x.AppUserId.HasValue &&
                        AppUserIds.Contains(x.AppUserId.Value) &&
                        x.CreatedAt >= Start && 
                        x.CreatedAt <= End)
                    .ToListAsync();
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
        public async Task<List<PermissionMobile_EmployeeKpiItemReportDTO>> ListCurrentKpiItem([FromBody] PermissionMobile_EmployeeKpiItemReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiItemDTOs = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);
            
            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }

            long KpiItemId = await DataContext.KpiItem.Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.KpiItemTypeId == KpiItemTypeEnum.ALL_PRODUCT.Id)
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

                        List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                            .Where(x =>
                                ItemIds.Contains(x.ItemId) &&
                                AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) &&
                                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id &&
                                x.IndirectSalesOrder.OrderDate >= Start &&
                                x.IndirectSalesOrder.OrderDate <= End
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

        [Route(PermissionMobileRoute.ListCurrentKpiNewItem), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiItemReportDTO>> ListCurrentKpiNewItem([FromBody] PermissionMobile_EmployeeKpiItemReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiItemDTOs = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }

            long KpiItemId = await DataContext.KpiItem.Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.KpiItemTypeId == KpiItemTypeEnum.NEW_PRODUCT.Id)
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

                        List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                            .Where(x =>
                                ItemIds.Contains(x.ItemId) &&
                                AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) &&
                                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id &&
                                x.IndirectSalesOrder.OrderDate >= Start &&
                                x.IndirectSalesOrder.OrderDate <= End
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

        private Tuple<GenericEnum, GenericEnum, GenericEnum> ConvertDateTime(DateTime date)
        {
            GenericEnum monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum quarterName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum yearName = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == StaticParams.DateTimeNow.Year).FirstOrDefault();
            switch (date.Month)
            {
                case 1:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 2:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH02;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 3:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH03;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 4:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH04;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 5:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH05;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 6:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH06;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 7:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH07;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 8:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH08;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 9:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH09;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 10:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH10;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 11:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH11;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 12:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH12;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
            }
            return Tuple.Create(monthName, quarterName, yearName);
        }

        private decimal CalculatePercentage(decimal PlannedValue, decimal CurrentValue)
        {
            if (PlannedValue > 0) return CurrentValue / PlannedValue * 100;
            return 0;
        } // trả về phần trăm thực hiện kế hoạch
    }
}
