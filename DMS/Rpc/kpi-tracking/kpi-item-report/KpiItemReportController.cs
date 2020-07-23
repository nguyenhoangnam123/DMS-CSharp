﻿using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using Hangfire.Annotations;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IKpiPeriodService KpiPeriodService;
        private IItemService ItemService;
        private ICurrentContext CurrentContext;
        public KpiItemReportController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IKpiPeriodService KpiPeriodService,
            IItemService ItemService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.ItemService = ItemService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiItemReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiItemReport_AppUserDTO>> FilterListAppUser([FromBody] KpiItemReport_AppUserFilterDTO KpiItemReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiItemReport_AppUserFilterDTO.Id;
            AppUserFilter.OrganizationId = KpiItemReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiItemReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItemReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItemReport_AppUserDTO> KpiItemReport_AppUserDTOs = AppUsers
                .Select(x => new KpiItemReport_AppUserDTO(x)).ToList();
            return KpiItemReport_AppUserDTOs;
        }

        [Route(KpiItemReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiItemReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiItemReport_OrganizationFilterDTO KpiItemReport_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);


            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiItemReport_OrganizationDTO> KpiItemReport_OrganizationDTOs = Organizations
                .Select(x => new KpiItemReport_OrganizationDTO(x)).ToList();
            return KpiItemReport_OrganizationDTOs;
        }

        [Route(KpiItemReportRoute.FilterListItem), HttpPost]
        public async Task<List<KpiItemReport_ItemDTO>> FilterListItem([FromBody] KpiItemReport_ItemFilterDTO KpiItemReport_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = int.MaxValue;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItemReport_ItemDTO> KpiItemReport_ItemDTOs = Items
                .Select(x => new KpiItemReport_ItemDTO(x)).ToList();
            return KpiItemReport_ItemDTOs;
        }

        [Route(KpiItemReportRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiItemReport_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiItemReport_KpiPeriodFilterDTO KpiItemReport_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiItemReport_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiItemReport_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiItemReport_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiItemReport_KpiPeriodDTO> KpiItemReport_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiItemReport_KpiPeriodDTO(x)).ToList();
            return KpiItemReport_KpiPeriodDTOs;
        }

        [Route(KpiItemReportRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiItemReport_KpiYearDTO>> FilterListKpiYear([FromBody] KpiItemReport_KpiYearFilterDTO KpiItemReport_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiItemReport_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiItemReport_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiItemReport_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiItemReport_KpiYearDTO> KpiItemReport_KpiYearDTOs = KpiYears
                .Select(x => new KpiItemReport_KpiYearDTO(x)).ToList();
            return KpiItemReport_KpiYearDTOs;
        }

        [Route(KpiItemReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiItemReport_KpiItemReportFilterDTO KpiItemReport_KpiItemReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState); // to do kpi year and period
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiItemReport_KpiItemReportFilterDTO.AppUserId?.Equal;
            long? ItemId = KpiItemReport_KpiItemReportFilterDTO.ItemId?.Equal;
            if (KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal.HasValue == false ||
                KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal.HasValue == false)
                return 0;
            long? KpiPeriodId = KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal.Value;
            long? KpiYearId = KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal.Value;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId.Value, KpiYearId.Value);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiItemReport_KpiItemReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            var query = from ki in DataContext.KpiItem
                        join kic in DataContext.KpiItemContent on ki.Id equals kic.KpiItemId
                        join i in DataContext.Item on kic.ItemId equals i.Id
                        where OrganizationIds.Contains(ki.OrganizationId)
                        && AppUserIds.Contains(ki.EmployeeId)
                        && (SaleEmployeeId == null || ki.Id == SaleEmployeeId.Value)
                        && (ItemId == null || i.Id == ItemId.Value)
                        && (ki.KpiYearId == KpiYearId)
                        select new
                        {
                            SaleEmployeeId = ki.EmployeeId,
                            ItemId = i.Id,
                        };
            return await query.Distinct().CountAsync();
        }

        [Route(KpiItemReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiItemReport_KpiItemReportDTO>>> List([FromBody] KpiItemReport_KpiItemReportFilterDTO KpiItemReport_KpiItemReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal.HasValue == false) return BadRequest("Chưa chọn kì KPI");
            if (KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal.HasValue == false) return BadRequest("Chưa chọn năm KPI");

            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiItemReport_KpiItemReportFilterDTO.AppUserId?.Equal;
            long? ItemId = KpiItemReport_KpiItemReportFilterDTO.ItemId?.Equal;
            long? KpiPeriodId = KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal.Value;
            long? KpiYearId = KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal.Value;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId.Value, KpiYearId.Value);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiItemReport_KpiItemReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            var query = from ki in DataContext.KpiItem
                        join au in DataContext.AppUser on ki.EmployeeId equals au.Id
                        join kic in DataContext.KpiItemContent on ki.Id equals kic.KpiItemId
                        join i in DataContext.Item on kic.ItemId equals i.Id
                        where OrganizationIds.Contains(ki.OrganizationId)
                        && AppUserIds.Contains(au.Id)
                        && (SaleEmployeeId == null || ki.Id == SaleEmployeeId.Value)
                        && (ItemId == null || i.Id == ItemId.Value)
                        && (ki.KpiYearId == KpiYearId)
                        select new
                        {
                            SaleEmployeeId = au.Id,
                            Username = au.Username,
                            DisplayName = au.DisplayName,
                            ItemId = i.Id,
                            ItemCode = i.Code,
                            ItemName = i.Name,
                        };

            var ItemContents = await query.Distinct()
                .OrderBy(q => q.DisplayName)
                .Skip(KpiItemReport_KpiItemReportFilterDTO.Skip)
                .Take(KpiItemReport_KpiItemReportFilterDTO.Take)
                .ToListAsync();

            List<long> SaleEmployeeIds = ItemContents.Select(x => x.SaleEmployeeId).Distinct().ToList();


            List<KpiItemReport_KpiItemReportDTO> KpiItemReport_KpiItemReportDTOs = new List<KpiItemReport_KpiItemReportDTO>();
            foreach (var EmployeeId in SaleEmployeeIds)
            {
                KpiItemReport_KpiItemReportDTO KpiItemReport_KpiItemReportDTO = new KpiItemReport_KpiItemReportDTO()
                {
                    SaleEmployeeId = EmployeeId,
                    DisplayName = ItemContents.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => x.DisplayName).FirstOrDefault(),
                    Username = ItemContents.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => x.Username).FirstOrDefault(),
                    ItemContents = ItemContents.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => new KpiItemReport_KpiItemContentDTO
                    {
                        ItemId = x.ItemId,
                        SaleEmployeeId = EmployeeId,
                        ItemName = x.ItemName,
                        ItemCode = x.ItemCode,
                    })
                    .Where(x => x.SaleEmployeeId == EmployeeId)
                    .ToList()
                };
                KpiItemReport_KpiItemReportDTOs.Add(KpiItemReport_KpiItemReportDTO);
            }

            // lay du lieu bang mapping
            var query_detail = from km in DataContext.KpiItemContentKpiCriteriaItemMapping
                               join kc in DataContext.KpiItemContent on km.KpiItemContentId equals kc.Id
                               join k in DataContext.KpiItem on kc.KpiItemId equals k.Id
                               join i in DataContext.Item on kc.ItemId equals i.Id
                               where (SaleEmployeeIds.Contains(k.EmployeeId) &&
                                      k.KpiYearId == KpiYearId &&
                                      k.KpiPeriodId == KpiPeriodId &&
                                      (ItemId == null || i.Id == ItemId))
                               select new
                               {
                                   SaleEmployeeId = k.EmployeeId,
                                   KpiCriteriaItemId = km.KpiCriteriaItemId,
                                   Value = km.Value,
                                   ItemId = i.Id,
                               };

            List<KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTO>
                KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs = (await query_detail.Distinct()
                .ToListAsync())
                .Select(x => new KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    KpiCriteriaItemId = x.KpiCriteriaItemId,
                    Value = x.Value,
                    ItemId = x.ItemId,
                })
                .ToList();

            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) &&
                x.OrderDate >= StartDate && x.OrderDate <= EndDate)
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Total = x.Total,
                    SaleEmployeeId = x.SaleEmployeeId,
                    OrderDate = x.OrderDate,
                    BuyerStoreId = x.BuyerStoreId,
                    IndirectSalesOrderContents = x.IndirectSalesOrderContents.Select(c => new IndirectSalesOrderContentDAO
                    {
                        IndirectSalesOrderId = x.Id,
                        Quantity = c.Quantity,
                        ItemId = c.ItemId,
                        Amount = c.Amount,
                        GeneralDiscountAmount = c.GeneralDiscountAmount,
                    }).ToList(),
                })
                .ToListAsync();

            foreach (var Employee in KpiItemReport_KpiItemReportDTOs)
            {
                foreach (var ItemContent in Employee.ItemContents)
                {
                    //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                    var IndirectSalesOrders = IndirectSalesOrderDAOs
                            .Where(x => x.SaleEmployeeId == Employee.SaleEmployeeId)
                            .ToList();

                    #region Sản lượng theo đơn hàng gián tiếp
                    //kế hoạch
                    ItemContent.IndirectQuantityPlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(x => x.SaleEmployeeId == ItemContent.SaleEmployeeId &&
                            x.ItemId == ItemContent.ItemId &&
                            x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    //thực hiện
                    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                    {
                        foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                        {
                            if (content.ItemId == ItemContent.ItemId)
                                ItemContent.IndirectQuantity += content.RequestedQuantity;
                        }
                    }
                    //tỉ lệ
                    ItemContent.IndirectQuantityRatio = ItemContent.IndirectQuantityPlanned == 0 ?
                        0.00m : Math.Round((ItemContent.IndirectQuantity / ItemContent.IndirectQuantityPlanned) * 100, 2);
                    #endregion

                    #region Doanh thu theo đơn hàng gián tiếp
                    //kế hoạch
                    ItemContent.IndirectRevenuePlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(x => x.SaleEmployeeId == ItemContent.SaleEmployeeId &&
                            x.ItemId == ItemContent.ItemId &&
                            x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                            .Select(x => x.Value).FirstOrDefault();

                    //thực hiện
                    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                    {
                        foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                        {
                            if (content.ItemId == ItemContent.ItemId)
                            {
                                ItemContent.IndirectRevenue += content.Amount;
                                ItemContent.IndirectRevenue -= content.GeneralDiscountAmount ?? 0;
                            }
                        }
                    }
                    //tỉ lệ
                    ItemContent.IndirectRevenueRatio = ItemContent.IndirectRevenuePlanned == 0 ?
                        0.00m : Math.Round((ItemContent.IndirectRevenue / ItemContent.IndirectRevenuePlanned) * 100, 2);
                    #endregion

                    #region Số đơn hàng gián tiếp
                    //kế hoạch
                    ItemContent.IndirectAmountPlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(x => x.SaleEmployeeId == ItemContent.SaleEmployeeId &&
                            x.ItemId == ItemContent.ItemId &&
                            x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_AMOUNT.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    //thực hiện
                    ItemContent.IndirectSalesOrderIds = new HashSet<long>();
                    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                    {
                        foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                        {
                            if (content.ItemId == ItemContent.ItemId)
                                ItemContent.IndirectSalesOrderIds.Add(content.IndirectSalesOrderId);
                        }
                    }
                    //tỉ lệ
                    ItemContent.IndirectAmountRatio = ItemContent.IndirectAmountPlanned == 0 ?
                        0.00m : Math.Round((ItemContent.IndirectAmount / ItemContent.IndirectAmountPlanned) * 100, 2);
                    #endregion

                    #region Đại lý theo đơn hàng gián tiếp
                    //kế hoạch
                    ItemContent.IndirectStorePlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(x => x.SaleEmployeeId == ItemContent.SaleEmployeeId &&
                            x.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    //thực hiện
                    ItemContent.StoreIds = new HashSet<long>();
                    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                    {
                        foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                        {
                            if (content.ItemId == ItemContent.ItemId)
                                ItemContent.StoreIds.Add(IndirectSalesOrder.BuyerStoreId);
                        }
                    }
                    //tỉ lệ
                    ItemContent.IndirectStoreRatio = ItemContent.IndirectStorePlanned == 0 ?
                        0.00m : Math.Round((ItemContent.IndirectStore / ItemContent.IndirectStorePlanned) * 100, 2);
                    #endregion
                }
            };
            KpiItemReport_KpiItemReportDTOs = KpiItemReport_KpiItemReportDTOs.Where(x => x.ItemContents.Any()).ToList();
            return KpiItemReport_KpiItemReportDTOs;
        }

        private Tuple<DateTime, DateTime> DateTimeConvert(long KpiPeriodId, long KpiYearId)
        {
            DateTime startDate = StaticParams.DateTimeNow;
            DateTime endDate = StaticParams.DateTimeNow;
            if (KpiPeriodId <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
            {
                startDate = new DateTime((int)KpiYearId, (int)(KpiPeriodId % 100), 1);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
            }
            else
            {
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER01.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 1, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER02.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 4, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER03.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 7, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER04.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 10, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_YEAR01.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 1, 1);
                    endDate = startDate.AddYears(1).AddSeconds(-1);
                }
            }

            return Tuple.Create(startDate, endDate);
        }
    }
}
