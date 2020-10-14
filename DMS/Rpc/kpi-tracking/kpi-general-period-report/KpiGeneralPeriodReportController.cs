using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using Hangfire.Annotations;
using Helpers;
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

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiGeneralPeriodReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IKpiPeriodService KpiPeriodService;
        private ICurrentContext CurrentContext;
        public KpiGeneralPeriodReportController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IKpiPeriodService KpiPeriodService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiGeneralPeriodReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiGeneralPeriodReport_AppUserDTO>> FilterListAppUser([FromBody] KpiGeneralPeriodReport_AppUserFilterDTO KpiGeneralPeriodReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiGeneralPeriodReport_AppUserFilterDTO.Id;
            AppUserFilter.OrganizationId = KpiGeneralPeriodReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiGeneralPeriodReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneralPeriodReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiGeneralPeriodReport_AppUserDTO> KpiGeneralPeriodReport_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneralPeriodReport_AppUserDTO(x)).ToList();
            return KpiGeneralPeriodReport_AppUserDTOs;
        }

        [Route(KpiGeneralPeriodReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiGeneralPeriodReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiGeneralPeriodReport_OrganizationFilterDTO KpiGeneralPeriodReport_OrganizationFilterDTO)
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
            List<KpiGeneralPeriodReport_OrganizationDTO> KpiGeneralPeriodReport_OrganizationDTOs = Organizations
                .Select(x => new KpiGeneralPeriodReport_OrganizationDTO(x)).ToList();
            return KpiGeneralPeriodReport_OrganizationDTOs;
        }
        [Route(KpiGeneralPeriodReportRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiGeneralPeriodReport_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiGeneralPeriodReport_KpiPeriodFilterDTO KpiGeneralPeriodReport_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiGeneralPeriodReport_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiGeneralPeriodReport_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiGeneralPeriodReport_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiGeneralPeriodReport_KpiPeriodDTO> KpiGeneralPeriodReport_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiGeneralPeriodReport_KpiPeriodDTO(x)).ToList();
            return KpiGeneralPeriodReport_KpiPeriodDTOs;
        }

        [Route(KpiGeneralPeriodReportRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiGeneralPeriodReport_KpiYearDTO>> FilterListKpiYear([FromBody] KpiGeneralPeriodReport_KpiYearFilterDTO KpiGeneralPeriodReport_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiGeneralPeriodReport_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiGeneralPeriodReport_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiGeneralPeriodReport_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiGeneralPeriodReport_KpiYearDTO> KpiGeneralPeriodReport_KpiYearDTOs = KpiYears
                .Select(x => new KpiGeneralPeriodReport_KpiYearDTO(x)).ToList();
            return KpiGeneralPeriodReport_KpiYearDTOs;
        }

        [Route(KpiGeneralPeriodReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState); // to do kpi year and period
            long? SaleEmployeeId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.AppUserId?.Equal;
            long KpiPeriodId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId?.Equal ?? 100 + StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Month;
            long KpiYearId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId?.Equal ?? StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            var query = from k in DataContext.KpiGeneral
                        join kc in DataContext.KpiGeneralContent on k.Id equals kc.KpiGeneralId
                        join km in DataContext.KpiGeneralContentKpiPeriodMapping on kc.Id equals km.KpiGeneralContentId
                        join au in DataContext.AppUser on k.EmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where OrganizationIds.Contains(o.Id) &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                        k.KpiYearId == KpiYearId &&
                        km.KpiPeriodId == KpiPeriodId &&
                        km.Value.HasValue &&
                        k.StatusId == StatusEnum.ACTIVE.Id &&
                        k.DeletedAt == null
                        select k.Id;
            return await query.Distinct().CountAsync();
        }

        [Route(KpiGeneralPeriodReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO>>> List([FromBody] KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId == null)
                return BadRequest(new { message = "Chưa chọn kì KPI" });
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId == null)
                return BadRequest(new { message = "Chưa chọn năm KPI" });

            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.AppUserId?.Equal;
            long KpiPeriodId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId?.Equal ?? 100 + StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Month;
            long KpiYearId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId?.Equal ?? StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            // list toan bo nhan vien trong organization do va cac con ma co kpi general
            var query = from k in DataContext.KpiGeneral
                        join kc in DataContext.KpiGeneralContent on k.Id equals kc.KpiGeneralId
                        join km in DataContext.KpiGeneralContentKpiPeriodMapping on kc.Id equals km.KpiGeneralContentId
                        join au in DataContext.AppUser on k.EmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where OrganizationIds.Contains(o.Id) &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                        k.KpiYearId == KpiYearId &&
                        km.KpiPeriodId == KpiPeriodId &&
                        km.Value.HasValue &&
                        k.StatusId == StatusEnum.ACTIVE.Id &&
                        k.DeletedAt == null
                        select new
                        {
                            SaleEmployeeId = au.Id,
                            Username = au.Username,
                            DisplayName = au.DisplayName,
                            OrganizationName = au.Organization == null ? null : au.Organization.Name,
                            OrganizationId = au.OrganizationId
                        };
            List<KpiGeneralPeriodReport_SaleEmployeeDTO> KpiGeneralPeriodReport_SaleEmployeeDTOs = (await query
                .Distinct()
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.DisplayName)
                .Skip(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Skip)
                .Take(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Take)
                .ToListAsync())
                .Select(x => new KpiGeneralPeriodReport_SaleEmployeeDTO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    OrganizationName = x.OrganizationName,
                    OrganizationId = x.OrganizationId,
                }).ToList();

            //build khung theo org
            var OrganizationNames = KpiGeneralPeriodReport_SaleEmployeeDTOs.Where(x => x.OrganizationId.HasValue).Select(x => x.OrganizationName).Distinct().ToList();
            List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO> kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs = new List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO>();
            foreach (string OrganizationName in OrganizationNames)
            {
                KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO = new KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO()
                {
                    OrganizationName = OrganizationName,
                };
                kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs.Add(KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO);
            }

            List<long> SaleEmployeeIds = KpiGeneralPeriodReport_SaleEmployeeDTOs.Select(x => x.SaleEmployeeId).ToList();

            // list toan bo mapping value and criteria
            var query_detail = from kcm in DataContext.KpiGeneralContentKpiPeriodMapping
                               join kc in DataContext.KpiGeneralContent on kcm.KpiGeneralContentId equals kc.Id
                               join k in DataContext.KpiGeneral on kc.KpiGeneralId equals k.Id
                               where (SaleEmployeeIds.Contains(k.EmployeeId) &&
                               OrganizationIds.Contains(k.OrganizationId) &&
                               k.KpiYearId == KpiYearId &&
                               kcm.KpiPeriodId == KpiPeriodId &&
                               k.StatusId == StatusEnum.ACTIVE.Id &&
                               k.DeletedAt == null)
                               select new
                               {
                                   SaleEmployeeId = k.EmployeeId,
                                   KpiCriteriaGeneralId = kc.KpiCriteriaGeneralId,
                                   Value = kcm.Value,
                               };
            List<KpiGeneralPeriodReport_SaleEmployeeDetailDTO> KpiGeneralPeriodReport_SaleEmployeeDetailDTOs = (await query_detail
                .Distinct()
                .ToListAsync())
                .Select(x => new KpiGeneralPeriodReport_SaleEmployeeDetailDTO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    KpiCriteriaGeneralId = x.KpiCriteriaGeneralId,
                    Value = x.Value,
                }).ToList();

            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) &&
                x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Total = x.Total,
                    SaleEmployeeId = x.SaleEmployeeId,
                    OrderDate = x.OrderDate,
                    IndirectSalesOrderContents = x.IndirectSalesOrderContents.Select(c => new IndirectSalesOrderContentDAO
                    {
                        RequestedQuantity = c.RequestedQuantity,
                        ItemId = c.ItemId
                    }).ToList(),
                    IndirectSalesOrderPromotions = x.IndirectSalesOrderPromotions.Select(x => new IndirectSalesOrderPromotionDAO
                    {
                        RequestedQuantity = x.RequestedQuantity,
                        ItemId = x.ItemId
                    }).ToList()
                })
                .ToListAsync();

            var DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
               .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) &&
               x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
               x.RequestStateId == RequestStateEnum.APPROVED.Id)
               .Select(x => new DirectSalesOrderDAO
               {
                   Id = x.Id,
                   Total = x.Total,
                   SaleEmployeeId = x.SaleEmployeeId,
                   OrderDate = x.OrderDate,
                   DirectSalesOrderContents = x.DirectSalesOrderContents.Select(c => new DirectSalesOrderContentDAO
                   {
                       RequestedQuantity = c.RequestedQuantity,
                       ItemId = c.ItemId
                   }).ToList(),
                   DirectSalesOrderPromotions = x.DirectSalesOrderPromotions.Select(x => new DirectSalesOrderPromotionDAO
                   {
                       RequestedQuantity = x.RequestedQuantity,
                       ItemId = x.ItemId
                   }).ToList()
               })
               .ToListAsync();

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) &&
                x.CheckOutAt.HasValue && x.CheckOutAt.Value >= StartDate && x.CheckOutAt.Value <= EndDate)
                .Select(x => new StoreCheckingDAO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt,
                    StoreId = x.StoreId
                })
                .ToListAsync();


            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => SaleEmployeeIds.Contains(x.CreatorId) &&
                x.CreatedAt >= StartDate && x.CreatedAt <= EndDate)
                .Select(x => new StoreScoutingDAO
                {
                    CreatorId = x.CreatorId,
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Stores = x.Stores.Select(c => new StoreDAO
                    {
                        StoreScoutingId = c.StoreScoutingId
                    }).ToList()
                })
                .ToListAsync();

            foreach (var SaleEmployeeDTO in KpiGeneralPeriodReport_SaleEmployeeDTOs)
            {
                //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                var IndirectSalesOrders = IndirectSalesOrderDAOs
                    .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .ToList();
                var DirectSalesOrders = DirectSalesOrderDAOs
                    .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .ToList();
                #region Số đơn hàng gián tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalIndirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalIndirectOrders = SaleEmployeeDTO.TotalIndirectOrdersPLanned == null ? null : (decimal?)IndirectSalesOrders.Count();
                //tỉ lệ
                SaleEmployeeDTO.TotalIndirectOrdersRatio = (SaleEmployeeDTO.TotalIndirectOrdersPLanned == null || SaleEmployeeDTO.TotalIndirectOrdersPLanned.Value == 0)
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.TotalIndirectOrders.Value / SaleEmployeeDTO.TotalIndirectOrdersPLanned.Value * 100, 2);
                #endregion

                #region Tổng sản lượng theo đơn gián tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalIndirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalIndirectQuantity = SaleEmployeeDTO.TotalIndirectQuantityPlanned == null ? null : (decimal?)IndirectSalesOrders
                    .SelectMany(c => c.IndirectSalesOrderContents)
                    .Select(q => q.RequestedQuantity)
                    .DefaultIfEmpty(0).Sum();
                //tỉ lệ
                SaleEmployeeDTO.TotalIndirectQuantityRatio = SaleEmployeeDTO.TotalIndirectQuantityPlanned == null || SaleEmployeeDTO.TotalIndirectQuantityPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.TotalIndirectQuantity.Value / SaleEmployeeDTO.TotalIndirectQuantityPlanned.Value * 100, 2);
                #endregion

                #region Doanh thu theo đơn hàng gián tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalIndirectSalesAmount = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == null ? null : (decimal?)IndirectSalesOrders.Sum(iso => iso.Total);
                //tỉ lệ
                SaleEmployeeDTO.TotalIndirectSalesAmountRatio = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == null || SaleEmployeeDTO.TotalIndirectSalesAmountPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round((SaleEmployeeDTO.TotalIndirectSalesAmount.Value / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned.Value) * 100, 2);
                #endregion

                #region SKU/Đơn hàng gián tiếp
                //kế hoạch
                SaleEmployeeDTO.SkuIndirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.SKUIndirectItems = new List<long>();
                foreach (var IndirectSalesOrder in IndirectSalesOrders)
                {
                    var itemIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).Distinct().ToList();
                    SaleEmployeeDTO.SKUIndirectItems.AddRange(itemIds);
                }

                SaleEmployeeDTO.SkuIndirectOrder = IndirectSalesOrders.Count() == 0
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.SKUIndirectItems.Count() / SaleEmployeeDTO.TotalIndirectOrders.Value, 2);
                if (SaleEmployeeDTO.SkuIndirectOrderPlanned != null && SaleEmployeeDTO.SkuIndirectOrder == null)
                    SaleEmployeeDTO.SkuIndirectOrder = 0;
                //tỉ lệ
                SaleEmployeeDTO.SkuIndirectOrderRatio = SaleEmployeeDTO.SkuIndirectOrderPlanned == null || SaleEmployeeDTO.SkuIndirectOrderPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round((SaleEmployeeDTO.SkuIndirectOrder.Value / SaleEmployeeDTO.SkuIndirectOrderPlanned.Value) * 100, 2);
                #endregion

                #region Số đơn hàng trực tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalDirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalDirectOrders = SaleEmployeeDTO.TotalDirectOrdersPLanned == null ? null : (decimal?)DirectSalesOrders.Count();
                //tỉ lệ
                SaleEmployeeDTO.TotalDirectOrdersRatio = (SaleEmployeeDTO.TotalDirectOrdersPLanned == null || SaleEmployeeDTO.TotalDirectOrdersPLanned.Value == 0)
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.TotalDirectOrders.Value / SaleEmployeeDTO.TotalDirectOrdersPLanned.Value * 100, 2);
                #endregion

                #region Tổng sản lượng theo đơn trực tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalDirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalDirectQuantity = SaleEmployeeDTO.TotalDirectQuantityPlanned == null ? null : (decimal?)DirectSalesOrders
                    .SelectMany(c => c.DirectSalesOrderContents)
                    .Select(q => q.RequestedQuantity)
                    .DefaultIfEmpty(0).Sum();
                //tỉ lệ
                SaleEmployeeDTO.TotalDirectQuantityRatio = SaleEmployeeDTO.TotalDirectQuantityPlanned == null || SaleEmployeeDTO.TotalDirectQuantityPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.TotalDirectQuantity.Value / SaleEmployeeDTO.TotalDirectQuantityPlanned.Value * 100, 2);
                #endregion

                #region Doanh thu theo đơn hàng trực tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalDirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalDirectSalesAmount = SaleEmployeeDTO.TotalDirectSalesAmountPlanned == null ? null : (decimal?)DirectSalesOrders.Sum(iso => iso.Total);
                //tỉ lệ
                SaleEmployeeDTO.TotalDirectSalesAmountRatio = SaleEmployeeDTO.TotalDirectSalesAmountPlanned == null || SaleEmployeeDTO.TotalDirectSalesAmountPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round((SaleEmployeeDTO.TotalDirectSalesAmount.Value / SaleEmployeeDTO.TotalDirectSalesAmountPlanned.Value) * 100, 2);
                #endregion

                #region SKU/Đơn hàng trực tiếp
                //kế hoạch
                SaleEmployeeDTO.SkuDirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.SKUDirectItems = new List<long>();
                foreach (var DirectSalesOrder in DirectSalesOrders)
                {
                    var itemIds = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).Distinct().ToList();
                    SaleEmployeeDTO.SKUDirectItems.AddRange(itemIds);
                }

                SaleEmployeeDTO.SkuDirectOrder = DirectSalesOrders.Count() == 0
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.SKUDirectItems.Count() / SaleEmployeeDTO.TotalDirectOrders.Value, 2);
                if (SaleEmployeeDTO.SkuDirectOrderPlanned != null && SaleEmployeeDTO.SkuDirectOrder == null)
                    SaleEmployeeDTO.SkuDirectOrder = 0;
                //tỉ lệ
                SaleEmployeeDTO.SkuDirectOrderRatio = SaleEmployeeDTO.SkuDirectOrderPlanned == null || SaleEmployeeDTO.SkuDirectOrderPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round((SaleEmployeeDTO.SkuDirectOrder.Value / SaleEmployeeDTO.SkuDirectOrderPlanned.Value) * 100, 2);
                #endregion

                #region Số cửa hàng viếng thăm
                //kế hoạch
                SaleEmployeeDTO.StoresVisitedPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                       .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                       .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                if (SaleEmployeeDTO.StoresVisitedPLanned.HasValue)
                {
                    var StoreIds = StoreCheckingDAOs
                        .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                        .Select(x => x.StoreId)
                        .ToList();
                    SaleEmployeeDTO.StoreIds = new HashSet<long>();
                    foreach (var StoreId in StoreIds)
                    {
                        SaleEmployeeDTO.StoreIds.Add(StoreId);
                    }
                    if (SaleEmployeeDTO.StoresVisitedPLanned == 0)
                        SaleEmployeeDTO.StoreIds = new HashSet<long>();
                    //tỉ lệ
                    SaleEmployeeDTO.StoresVisitedRatio = SaleEmployeeDTO.StoresVisitedPLanned == null || SaleEmployeeDTO.StoresVisitedPLanned.Value == 0
                        ? null
                        : (decimal?)Math.Round((SaleEmployeeDTO.StoresVisited.Value / SaleEmployeeDTO.StoresVisitedPLanned.Value) * 100, 2);
                }
                #endregion

                #region Số cửa hàng tạo mới
                //kế hoạch
                SaleEmployeeDTO.NewStoreCreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId
                         && x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.NewStoreCreated = SaleEmployeeDTO.NewStoreCreatedPlanned == null || SaleEmployeeDTO.NewStoreCreatedPlanned == 0
                    ? null
                    : (decimal?)StoreScoutingDAOs
                    .Where(sc => sc.CreatorId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(sc => sc.Stores)
                    .Where(x => x.StoreScoutingId.HasValue)
                    .Select(z => z.StoreScoutingId.Value)
                    .Count();
                //tỉ lệ
                SaleEmployeeDTO.NewStoreCreatedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == null || SaleEmployeeDTO.NewStoreCreatedPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.NewStoreCreated.Value / SaleEmployeeDTO.NewStoreCreatedPlanned.Value * 100, 2);
                #endregion

                #region Số lần viếng thăm cửa hàng
                //kế hoạch
                SaleEmployeeDTO.NumberOfStoreVisitsPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.NumberOfStoreVisits = SaleEmployeeDTO.NumberOfStoreVisitsPlanned == null || SaleEmployeeDTO.NumberOfStoreVisitsPlanned.Value == 0
                    ? null
                    : (decimal?)StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                //tỉ lệ
                SaleEmployeeDTO.NumberOfStoreVisitsRatio = SaleEmployeeDTO.NumberOfStoreVisitsPlanned == null || SaleEmployeeDTO.NumberOfStoreVisitsPlanned.Value == 0
                    ? null
                    : (decimal?)Math.Round(SaleEmployeeDTO.NumberOfStoreVisits.Value / SaleEmployeeDTO.NumberOfStoreVisitsPlanned.Value * 100, 2);
                #endregion
            };

            foreach (var kpiGeneralPeriodReport_KpiGeneralPeriodReportDTO in kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs)
            {
                kpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees = KpiGeneralPeriodReport_SaleEmployeeDTOs
                    .Where(x => x.OrganizationName.Equals(kpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.OrganizationName))
                    .ToList();
            }
            return kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs.Where(x => x.SaleEmployees.Any()).ToList();
        }

        [Route(KpiGeneralPeriodReportRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId == null)
                return BadRequest(new { message = "Chưa chọn kì KPI" });
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId == null)
                return BadRequest(new { message = "Chưa chọn năm KPI" });

            var KpiPeriod = KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Id == KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId.Equal.Value).FirstOrDefault();
            var KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId.Equal.Value).FirstOrDefault();

            KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Skip = 0;
            KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Take = int.MaxValue;
            List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO> KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs = (await List(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO)).Value;

            long stt = 1;
            foreach (KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO in KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs)
            {
                foreach (var SaleEmployee in KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees)
                {
                    SaleEmployee.STT = stt;
                    stt++;
                }
            }

            List<KpiGeneralPeriodReport_ExportDTO> KpiGeneralPeriodReport_ExportDTOs = KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs?.Select(x => new KpiGeneralPeriodReport_ExportDTO(x)).ToList();
            string path = "Templates/KpiGeneral_Period_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiPeriod = KpiPeriod.Name;
            Data.KpiYear = KpiYear.Name;
            Data.KpiGeneralPeriodReports = KpiGeneralPeriodReport_ExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "KpiGeneralPeriodReport.xlsx");
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
