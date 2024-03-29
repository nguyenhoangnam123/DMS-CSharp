﻿using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MKpiGeneral;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using Hangfire.Annotations;
using DMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NGS.Templater;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IKpiGeneralService KpiGeneralService;
        private IKpiPeriodService KpiPeriodService;
        private ICurrentContext CurrentContext;
        public KpiGeneralEmployeeReportController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IKpiGeneralService KpiGeneralService,
            IKpiYearService KpiYearService,
            IKpiPeriodService KpiPeriodService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.KpiGeneralService = KpiGeneralService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiGeneralEmployeeReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiGeneralEmployeeReport_AppUserDTO>> FilterListAppUser([FromBody] KpiGeneralEmployeeReport_AppUserFilterDTO KpiGeneralEmployeeReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiGeneralEmployeeReport_AppUserFilterDTO.Id;
            AppUserFilter.OrganizationId = KpiGeneralEmployeeReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiGeneralEmployeeReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneralEmployeeReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);


            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiGeneralEmployeeReport_AppUserDTO> KpiGeneralEmployeeReport_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneralEmployeeReport_AppUserDTO(x)).ToList();
            return KpiGeneralEmployeeReport_AppUserDTOs;
        }

        [Route(KpiGeneralEmployeeReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiGeneralEmployeeReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiGeneralEmployeeReport_OrganizationFilterDTO KpiGeneralEmployeeReport_OrganizationFilterDTO)
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
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiGeneralEmployeeReport_OrganizationDTO> KpiGeneralEmployeeReport_OrganizationDTOs = Organizations
                .Select(x => new KpiGeneralEmployeeReport_OrganizationDTO(x)).ToList();
            return KpiGeneralEmployeeReport_OrganizationDTOs;
        }
        [Route(KpiGeneralEmployeeReportRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiGeneralEmployeeReport_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiGeneralEmployeeReport_KpiPeriodFilterDTO KpiGeneralEmployeeReport_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiGeneralEmployeeReport_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiGeneralEmployeeReport_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiGeneralEmployeeReport_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiGeneralEmployeeReport_KpiPeriodDTO> KpiGeneralEmployeeReport_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiGeneralEmployeeReport_KpiPeriodDTO(x)).ToList();
            return KpiGeneralEmployeeReport_KpiPeriodDTOs;
        }

        [Route(KpiGeneralEmployeeReportRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiGeneralEmployeeReport_KpiYearDTO>> FilterListKpiYear([FromBody] KpiGeneralEmployeeReport_KpiYearFilterDTO KpiGeneralEmployeeReport_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiGeneralEmployeeReport_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiGeneralEmployeeReport_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiGeneralEmployeeReport_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiGeneralEmployeeReport_KpiYearDTO> KpiGeneralEmployeeReport_KpiYearDTOs = KpiYears
                .Select(x => new KpiGeneralEmployeeReport_KpiYearDTO(x)).ToList();
            return KpiGeneralEmployeeReport_KpiYearDTOs;
        }

        [Route(KpiGeneralEmployeeReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.AppUserId.Equal;
            if (SaleEmployeeId == null)
                return 0;
            long? KpiPeriodId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiPeriodId?.Equal;
            long? KpiYearId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiYearId?.Equal;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where OrganizationIds.Contains(c.OrganizationId) &&
                               c.EmployeeId == SaleEmployeeId.Value &&
                               (KpiYearId == null || c.KpiYearId == KpiYearId) &&
                               (KpiPeriodId == null || a.KpiPeriodId == KpiPeriodId) &&
                               c.StatusId == StatusEnum.ACTIVE.Id &&
                               c.DeletedAt == null
                               select new
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiYearId = c.KpiYearId,
                                   KpiPeriodId = a.KpiPeriodId,
                               };
            return await query_detail.Distinct().CountAsync();
        }

        [Route(KpiGeneralEmployeeReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO>>> List([FromBody] KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.AppUserId == null)
                return BadRequest(new { message = "Chưa chọn nhân viên" });

            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.AppUserId.Equal.Value;
            long? KpiPeriodId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiPeriodId?.Equal;
            long? KpiYearId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiYearId?.Equal;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            var KpiGeneralId = await DataContext.KpiGeneral
                .Where(x => x.EmployeeId == SaleEmployeeId.Value &&
                (KpiYearId.HasValue == false || x.KpiYearId == KpiYearId.Value) &&
                x.StatusId == StatusEnum.ACTIVE.Id &&
                (KpiYearId == null || x.KpiYearId == KpiYearId) &&
                x.DeletedAt == null)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            var KpiGeneral = await KpiGeneralService.Get(KpiGeneralId);

            if (KpiGeneral == null)
                return new List<KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO>();
            var KpiPeriodIds = KpiGeneral.KpiGeneralContents
                .SelectMany(x => x.KpiGeneralContentKpiPeriodMappings)
                .Where(x => KpiPeriodId.HasValue == false || x.KpiPeriodId == KpiPeriodId.Value)
                .Select(x => x.KpiPeriodId)
                .Distinct()
                .Skip(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.Skip)
                .Take(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.Take)
                .ToList();
            var KpiPeriods = await KpiPeriodService.List(new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL,
                Id = new IdFilter { In = KpiPeriodIds },
                OrderBy = KpiPeriodOrder.Id,
                OrderType = OrderType.ASC
            });

            var KpiGeneralContentKpiPeriodMappings = KpiGeneral.KpiGeneralContents
                .SelectMany(x => x.KpiGeneralContentKpiPeriodMappings)
                .Where(x => KpiPeriodIds.Contains(x.KpiPeriodId))
                .ToList();
            List<KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO>
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs = new List<KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO>();
            Parallel.ForEach(KpiPeriods, KpiPeriod =>
            {
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO
                    = new KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO();
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiPeriodId = KpiPeriod.Id;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiPeriodName = KpiPeriod.Name;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiYearId = KpiGeneral.KpiYearId;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiYearName = KpiGeneral.KpiYear.Name;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SaleEmployeeId = SaleEmployeeId.Value;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs.Add(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO);
            });
            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Total = x.Total,
                    SaleEmployeeId = x.SaleEmployeeId,
                    OrderDate = x.OrderDate,
                    BuyerStoreId = x.BuyerStoreId,
                    BuyerStore = x.BuyerStore == null ? null : new StoreDAO
                    {
                        StoreType = x.BuyerStore.StoreType == null ? null : new StoreTypeDAO
                        {
                            Code = x.BuyerStore.StoreType.Code
                        }
                    },
                    IndirectSalesOrderContents = x.IndirectSalesOrderContents.Select(c => new IndirectSalesOrderContentDAO
                    {
                        IndirectSalesOrderId = x.Id,
                        RequestedQuantity = c.RequestedQuantity,
                        ItemId = c.ItemId,
                    }).ToList(),
                })
                .ToListAsync();

            //var DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
            //    .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
            //    x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
            //    x.RequestStateId == RequestStateEnum.APPROVED.Id)
            //    .Select(x => new DirectSalesOrderDAO
            //    {
            //        Id = x.Id,
            //        Total = x.Total,
            //        SaleEmployeeId = x.SaleEmployeeId,
            //        OrderDate = x.OrderDate,
            //        DirectSalesOrderContents = x.DirectSalesOrderContents.Select(c => new DirectSalesOrderContentDAO
            //        {
            //            DirectSalesOrderId = x.Id,
            //            RequestedQuantity = c.RequestedQuantity,
            //            ItemId = c.ItemId,
            //        }).ToList(),
            //        DirectSalesOrderPromotions = x.DirectSalesOrderPromotions.Select(x => new DirectSalesOrderPromotionDAO
            //        {
            //            RequestedQuantity = x.RequestedQuantity,
            //            ItemId = x.ItemId
            //        }).ToList()
            //    })
            //    .ToListAsync();

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                x.CheckOutAt.HasValue && x.CheckOutAt.Value >= StartDate && x.CheckOutAt.Value <= EndDate)
                .Select(x => new StoreCheckingDAO
                {
                    StoreId = x.StoreId,
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt
                })
                .ToListAsync();

            var StoreDAOs = await DataContext.Store
                    .Where(x => x.CreatorId == SaleEmployeeId &&
                    x.CreatedAt >= StartDate && x.CreatedAt <= EndDate)
                    .Select(x => new StoreDAO
                    {
                        CreatorId = x.CreatorId,
                        Id = x.Id,
                        CreatedAt = x.CreatedAt,
                        StoreType = x.StoreType == null ? null : new StoreTypeDAO
                        {
                            Code = x.StoreType.Code
                        }
                    })
                    .ToListAsync();

            var ProblemDAOs = await DataContext.Problem
                 .Where(x => x.CreatorId == SaleEmployeeId &&
                x.NoteAt >= StartDate && x.NoteAt <= EndDate)
                 .ToListAsync();
            var StoreImages = await DataContext.StoreImage
                .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                x.ShootingAt >= StartDate && x.ShootingAt <= EndDate)
                .ToListAsync();

            Parallel.ForEach(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs, Period =>
            {
                foreach (var KpiPeriod in KpiPeriodEnum.KpiPeriodEnumList)
                {
                    if (KpiPeriod.Id == Period.KpiPeriodId)
                        Period.KpiPeriodName = KpiPeriod.Name;
                }
                DateTime Start, End;
                (Start, End) = DateTimeConvert(Period.KpiPeriodId, Period.KpiYearId);

                //lấy tất cả đơn hàng được đặt trong kì đang xét
                var IndirectSalesOrders = IndirectSalesOrderDAOs
                    .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                    x.OrderDate >= Start && x.OrderDate <= End)
                    .ToList();

                //var DirectSalesOrders = DirectSalesOrderDAOs
                //    .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                //    x.OrderDate >= Start && x.OrderDate <= End)
                //    .ToList();

                //lấy tất cả lượt checking trong kì đang xét
                var StoreCheckings = StoreCheckingDAOs
                    .Where(x => x.SaleEmployeeId == Period.SaleEmployeeId &&
                    x.CheckOutAt.HasValue && x.CheckOutAt.Value >= Start && x.CheckOutAt.Value <= End)
                    .ToList();

                #region các chỉ tiêu tạm ẩn
                //#region Số đơn hàng gián tiếp
                ////kế hoạch
                //Period.TotalIndirectOrdersPLanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                //if (Period.TotalIndirectOrdersPLanned.HasValue)
                //{
                //    //thực hiện
                //    Period.TotalIndirectOrders = Period.TotalIndirectOrdersPLanned == null ? null : (decimal?)IndirectSalesOrders.Count();
                //    //tỉ lệ
                //    Period.TotalIndirectOrdersRatio = Period.TotalIndirectOrdersPLanned == null || Period.TotalIndirectOrders == null || Period.TotalIndirectOrdersPLanned == 0 ? null :
                //        (decimal?)
                //        Math.Round((Period.TotalIndirectOrders.Value / Period.TotalIndirectOrdersPLanned.Value) * 100, 2);
                //}
                //#endregion

                //#region Tổng sản lượng theo đơn gián tiếp
                ////kế hoạch
                //Period.TotalIndirectQuantityPlanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                //if (Period.TotalIndirectQuantityPlanned.HasValue)
                //{
                //    //thực hiện
                //    Period.TotalIndirectQuantity = 0;
                //    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                //    {
                //        foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                //        {
                //            Period.TotalIndirectQuantity += content.RequestedQuantity;
                //        }
                //    }
                //    //tỉ lệ
                //    Period.TotalIndirectQuantityRatio = Period.TotalIndirectQuantityPlanned == null || Period.TotalIndirectQuantity == null || Period.TotalIndirectQuantityPlanned == 0 ? null :
                //        (decimal?)
                //        Math.Round((Period.TotalIndirectQuantity.Value / Period.TotalIndirectQuantityPlanned.Value) * 100, 2);
                //}
                //#endregion

                //#region SKU/Đơn hàng gián tiếp
                ////kế hoạch
                //Period.SkuIndirectOrderPlanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                //if (Period.SkuIndirectOrderPlanned.HasValue)
                //{
                //    //thực hiện
                //    Period.SKUIndirectItems = new List<long>();
                //    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                //    {
                //        var itemIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).Distinct().ToList();
                //        Period.SKUIndirectItems.AddRange(itemIds);
                //    }
                //    Period.SkuIndirectOrder = IndirectSalesOrders.Count() == 0 || Period.SKUIndirectItems == null ? 0 :
                //        Math.Round((decimal)Period.SKUIndirectItems.Count() / IndirectSalesOrders.Count(), 2);
                //    //tỉ lệ
                //    Period.SkuIndirectOrderRatio = Period.SkuIndirectOrderPlanned == null || Period.SkuIndirectOrder == null || Period.SkuIndirectOrderPlanned == 0 ? 0 :
                //        Math.Round((Period.SkuIndirectOrder.Value / Period.SkuIndirectOrderPlanned.Value) * 100, 2);
                //}

                //#endregion

                //#region Số đơn hàng trực tiếp
                ////kế hoạch
                //Period.TotalDirectOrdersPLanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                //if (Period.TotalDirectOrdersPLanned.HasValue)
                //{
                //    //thực hiện
                //    Period.TotalDirectOrders = Period.TotalDirectOrdersPLanned == null ? null : (decimal?)DirectSalesOrders.Count();
                //    //tỉ lệ
                //    Period.TotalDirectOrdersRatio = Period.TotalDirectOrdersPLanned == null || Period.TotalDirectOrders == null || Period.TotalDirectOrdersPLanned == 0 ? null :
                //        (decimal?)
                //        Math.Round((Period.TotalDirectOrders.Value / Period.TotalDirectOrdersPLanned.Value) * 100, 2);
                //}
                //#endregion

                //#region Tổng sản lượng theo đơn trực tiếp
                ////kế hoạch
                //Period.TotalDirectQuantityPlanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                //if (Period.TotalDirectQuantityPlanned.HasValue)
                //{
                //    //thực hiện
                //    Period.TotalDirectQuantity = 0;
                //    foreach (var DirectSalesOrder in DirectSalesOrders)
                //    {
                //        foreach (var content in DirectSalesOrder.DirectSalesOrderContents)
                //        {
                //            Period.TotalDirectQuantity += content.RequestedQuantity;
                //        }
                //    }
                //    //tỉ lệ
                //    Period.TotalDirectQuantityRatio = Period.TotalDirectQuantityPlanned == null || Period.TotalDirectQuantity == null || Period.TotalDirectQuantityPlanned == 0 ? null :
                //        (decimal?)
                //        Math.Round((Period.TotalDirectQuantity.Value / Period.TotalDirectQuantityPlanned.Value) * 100, 2);
                //}
                //#endregion

                //#region Doanh thu theo đơn hàng trực tiếp
                ////kế hoạch
                //Period.TotalDirectSalesAmountPlanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                //if (Period.TotalDirectSalesAmountPlanned.HasValue)
                //{
                //    //thực hiện
                //    Period.TotalDirectSalesAmount = Period.TotalDirectSalesAmountPlanned == null ? null : (decimal?)DirectSalesOrders.Sum(x => x.Total);
                //    //tỉ lệ
                //    Period.TotalDirectSalesAmountRatio = Period.TotalDirectSalesAmountPlanned == null || Period.TotalDirectSalesAmount == null || Period.TotalDirectSalesAmountPlanned == 0 ? null :
                //        (decimal?)
                //        Math.Round((Period.TotalDirectSalesAmount.Value / Period.TotalDirectSalesAmountPlanned.Value) * 100, 2);
                //}
                //#endregion

                //#region SKU/Đơn hàng trực tiếp
                ////kế hoạch
                //Period.SkuDirectOrderPlanned = KpiGeneralContentKpiPeriodMappings
                //        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                //        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id)
                //        .Select(x => x.Value)
                //        .FirstOrDefault();
                ////thực hiện
                //if (Period.SkuDirectOrderPlanned.HasValue)
                //{
                //    Period.SKUDirectItems = new List<long>();
                //    foreach (var DirectSalesOrder in DirectSalesOrders)
                //    {
                //        var itemIds = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).Distinct().ToList();
                //        Period.SKUDirectItems.AddRange(itemIds);
                //    }
                //    Period.SkuDirectOrder = DirectSalesOrders.Count() == 0 || Period.SKUDirectItems == null ? 0 :
                //        Math.Round((decimal)Period.SKUDirectItems.Count() / DirectSalesOrders.Count(), 2);
                //    //tỉ lệ
                //    Period.SkuDirectOrderRatio = Period.SkuDirectOrderPlanned == null || Period.SkuDirectOrder == null || Period.SkuDirectOrderPlanned == 0 ? 0 :
                //        Math.Round((Period.SkuDirectOrder.Value / Period.SkuDirectOrderPlanned.Value) * 100, 2);
                //}
                //#endregion
                #endregion

                #region Tổng doanh thu đơn hàng
                //kế hoạch
                Period.TotalIndirectSalesAmountPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.TotalIndirectSalesAmountPlanned.HasValue)
                {
                    //thực hiện
                    Period.TotalIndirectSalesAmount = Period.TotalIndirectSalesAmountPlanned == null ? null : (decimal?)IndirectSalesOrders.Sum(x => x.Total);
                    //tỉ lệ
                    Period.TotalIndirectSalesAmountRatio = Period.TotalIndirectSalesAmountPlanned == null || Period.TotalIndirectSalesAmount == null || Period.TotalIndirectSalesAmountPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.TotalIndirectSalesAmount.Value / Period.TotalIndirectSalesAmountPlanned.Value) * 100, 2);
                }
                #endregion

                #region Số đại lý ghé thăm
                //kế hoạch
                Period.StoresVisitedPLanned = KpiGeneralContentKpiPeriodMappings
                       .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                       x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                       .Select(x => x.Value)
                       .FirstOrDefault();
                //thực hiện
                if (Period.StoresVisitedPLanned.HasValue)
                {
                    Period.StoreIds = new HashSet<long>();
                    foreach (var StoreChecking in StoreCheckings)
                    {
                        Period.StoreIds.Add(StoreChecking.StoreId);
                    }
                    if (Period.StoresVisitedPLanned == 0)
                        Period.StoreIds = null;
                    //tỉ lệ
                    Period.StoresVisitedRatio = Period.StoresVisitedPLanned == null || Period.StoresVisited == null || Period.StoresVisitedPLanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.StoresVisited.Value / Period.StoresVisitedPLanned.Value) * 100, 2);
                }
                #endregion

                #region Tổng số đại lý mở mới
                //kế hoạch
                Period.NewStoreCreatedPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.NewStoreCreatedPlanned.HasValue)
                {
                    //thực hiện
                    Period.NewStoreCreated = Period.NewStoreCreatedPlanned == null ? null :
                        (decimal?)
                        StoreDAOs
                        .Where(sc => sc.CreatorId == Period.SaleEmployeeId &&
                        sc.CreatedAt >= Start && sc.CreatedAt <= End)
                        .Count();
                    //tỉ lệ
                    Period.NewStoreCreatedRatio = Period.NewStoreCreatedPlanned == null || Period.NewStoreCreated == null || Period.NewStoreCreatedPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.NewStoreCreated.Value / Period.NewStoreCreatedPlanned.Value) * 100, 2);
                }
                #endregion

                #region Tổng số lượt ghé thăm
                //kế hoạch
                Period.NumberOfStoreVisitsPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.NumberOfStoreVisitsPlanned.HasValue)
                {
                    //thực hiện
                    Period.NumberOfStoreVisits = Period.NumberOfStoreVisitsPlanned == null ? null :
                        (decimal?)
                        StoreCheckings.Count();
                    //tỉ lệ
                    Period.NumberOfStoreVisitsRatio = Period.NumberOfStoreVisitsPlanned == null || Period.NumberOfStoreVisits == null || Period.NumberOfStoreVisitsPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.NumberOfStoreVisits.Value / Period.NumberOfStoreVisitsPlanned.Value) * 100, 2);
                }
                #endregion

                #region Doanh thu C2 Trọng điểm
                //kế hoạch
                Period.RevenueC2TDPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.REVENUE_C2_TD.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.RevenueC2TDPlanned.HasValue)
                {
                    //thực hiện
                    Period.RevenueC2TD = Period.RevenueC2TDPlanned == null ?
                    null : (decimal?)IndirectSalesOrders.Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2TD).Sum(x => x.Total);
                    //tỉ lệ
                    Period.RevenueC2TDRatio = Period.RevenueC2TDPlanned == null || Period.RevenueC2TD == null || Period.RevenueC2TDPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.RevenueC2TD.Value / Period.RevenueC2TDPlanned.Value) * 100, 2);
                }
                #endregion

                #region Doanh thu C2 Siêu lớn
                //kế hoạch
                Period.RevenueC2SLPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.REVENUE_C2_SL.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.RevenueC2SLPlanned.HasValue)
                {
                    //thực hiện
                    Period.RevenueC2SL = Period.RevenueC2SLPlanned == null ?
                    null : (decimal?)IndirectSalesOrders.Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2SL).Sum(x => x.Total);
                    //tỉ lệ
                    Period.RevenueC2SLRatio = Period.RevenueC2SLPlanned == null || Period.RevenueC2SL == null || Period.RevenueC2SLPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.RevenueC2SL.Value / Period.RevenueC2SLPlanned.Value) * 100, 2);
                }
                #endregion

                #region Doanh thu C2
                //kế hoạch
                Period.RevenueC2Planned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.REVENUE_C2.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.RevenueC2Planned.HasValue)
                {
                    //thực hiện
                    Period.RevenueC2 = Period.RevenueC2Planned == null ?
                    null : (decimal?)IndirectSalesOrders.Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2).Sum(x => x.Total);
                    //tỉ lệ
                    Period.RevenueC2Ratio = Period.RevenueC2Planned == null || Period.RevenueC2 == null || Period.RevenueC2Planned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.RevenueC2.Value / Period.RevenueC2Planned.Value) * 100, 2);
                }
                #endregion

                #region Số đại lý trọng điểm mở mới
                //kế hoạch
                Period.NewStoreC2CreatedPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_C2_CREATED.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.NewStoreC2CreatedPlanned.HasValue)
                {
                    //thực hiện
                    Period.NewStoreC2Created = Period.NewStoreC2CreatedPlanned == null ? null :
                        (decimal?)
                        StoreDAOs
                        .Where(sc => sc.CreatorId == Period.SaleEmployeeId &&
                        sc.CreatedAt >= Start && sc.CreatedAt <= End)
                        .Where(x => x.StoreType.Code == StaticParams.C2TD)
                        .Count();
                    //tỉ lệ
                    Period.NewStoreC2CreatedRatio = Period.NewStoreC2CreatedPlanned == null || Period.NewStoreC2Created == null || Period.NewStoreC2CreatedPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.NewStoreC2Created.Value / Period.NewStoreC2CreatedPlanned.Value) * 100, 2);
                }
                #endregion

                #region Số thông tin phản ảnh
                //kế hoạch
                Period.TotalProblemPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_PROBLEM.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.TotalProblemPlanned.HasValue)
                {
                    //thực hiện
                    Period.TotalProblem = Period.TotalProblemPlanned == null ?
                    null : (decimal?)ProblemDAOs.Where(x => x.CreatorId == Period.SaleEmployeeId &&
                    Start <= x.NoteAt && x.NoteAt <= End)
                    .Count();
                    //tỉ lệ
                    Period.TotalProblemRatio = Period.TotalProblemPlanned == null || Period.TotalProblem == null || Period.TotalProblemPlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.TotalProblem.Value / Period.TotalProblemPlanned.Value) * 100, 2);
                }
                #endregion

                #region Số hình ảnh chụp
                //kế hoạch
                Period.TotalImagePlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_IMAGE.Id)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                if (Period.TotalImagePlanned.HasValue)
                {
                    //thực hiện
                    Period.TotalImage = Period.TotalImagePlanned == null ? 
                    null : (decimal?)StoreImages.Where(x => x.SaleEmployeeId == Period.SaleEmployeeId &&
                    Start <= x.ShootingAt && x.ShootingAt <= End)
                    .Count();
                    //tỉ lệ
                    Period.TotalImageRatio = Period.TotalImagePlanned == null || Period.TotalImage == null || Period.TotalImagePlanned == 0 ? null :
                        (decimal?)
                        Math.Round((Period.TotalImage.Value / Period.TotalImagePlanned.Value) * 100, 2);
                }
                #endregion
            });

            return KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs.OrderBy(x => x.KpiPeriodId).ThenBy(x => x.KpiYearId).ToList();
        }

        [Route(KpiGeneralEmployeeReportRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.AppUserId == null)
                return BadRequest(new { message = "Chưa chọn nhân viên" });

            KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.Skip = 0;
            KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.Take = int.MaxValue;
            List<KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO> KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs = (await List(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO)).Value;
            var SaleEmployee = await AppUserService.Get(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.AppUserId.Equal.Value);
            long stt = 1;
            foreach (KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO in KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs)
            {
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.STT = stt;
                stt++;
            }

            List<KpiGeneralEmployeeReport_ExportDTO> KpiGeneralEmployeeReport_ExportDTOs = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs?.Select(x => new KpiGeneralEmployeeReport_ExportDTO(x)).ToList();
            string path = "Templates/KpiGeneral_Employee_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Username = SaleEmployee.Username;
            Data.DisplayName = SaleEmployee.DisplayName;
            Data.KpiGeneralEmployeeReports = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "KpiGeneralEmployeeReport.xlsx");
        }

        private Tuple<DateTime, DateTime> DateTimeConvert(long? KpiPeriodId, long? KpiYearId)
        {
            DateTime startDate = StaticParams.DateTimeNow;
            DateTime endDate = StaticParams.DateTimeNow;
            if (KpiYearId == null) KpiYearId = startDate.Year;
            if (KpiPeriodId == null)
            {
                startDate = new DateTime(2019, 1, 1);
                endDate = new DateTime(2040, 12, 12);
            }
            else
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
