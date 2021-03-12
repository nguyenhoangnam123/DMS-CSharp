using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
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
                        where OrganizationIds.Contains(k.OrganizationId) &&
                        AppUserIds.Contains(k.EmployeeId) &&
                        (SaleEmployeeId == null || k.EmployeeId == SaleEmployeeId.Value) &&
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
                        where OrganizationIds.Contains(k.OrganizationId) &&
                        AppUserIds.Contains(k.EmployeeId) &&
                        (SaleEmployeeId == null || k.EmployeeId == SaleEmployeeId.Value) &&
                        k.KpiYearId == KpiYearId &&
                        km.KpiPeriodId == KpiPeriodId &&
                        km.Value.HasValue &&
                        k.StatusId == StatusEnum.ACTIVE.Id &&
                        k.DeletedAt == null
                        select new
                        {
                            EmployeeId = k.EmployeeId,
                            OrganizationId = k.OrganizationId
                        };
            var Ids = await query
                .Distinct()
                .OrderBy(x => x.OrganizationId)
                .Skip(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Skip)
                .Take(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Take)
                .ToListAsync();
            AppUserIds = Ids.Select(x => x.EmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(au => AppUserIds.Contains(au.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .Select(x => new AppUserDAO
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    OrganizationId = x.OrganizationId
                })
                .ToListAsync();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO> KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs = new List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO>();
            Parallel.ForEach(Organizations, Organization =>
            {
                KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO = new KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<KpiGeneralPeriodReport_SaleEmployeeDTO>()
                };
                KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees = Ids.Where(x => x.OrganizationId == Organization.Id).Select(x => new KpiGeneralPeriodReport_SaleEmployeeDTO
                {
                    SaleEmployeeId = x.EmployeeId
                }).ToList();
                KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs.Add(KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO);

                foreach (var SaleEmployee in KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees)
                {
                    var Employee = AppUserDAOs.Where(x => x.Id == SaleEmployee.SaleEmployeeId).FirstOrDefault();
                    if (Employee != null)
                    {
                        SaleEmployee.Username = Employee.Username;
                        SaleEmployee.DisplayName = Employee.DisplayName;
                    }
                }
            }); // nhóm saleEmp theo Organization, lấy ra UserName và DisplayName
            // list toan bo mapping value and criteria
            var query_detail = from kcm in DataContext.KpiGeneralContentKpiPeriodMapping
                               join kc in DataContext.KpiGeneralContent on kcm.KpiGeneralContentId equals kc.Id
                               join k in DataContext.KpiGeneral on kc.KpiGeneralId equals k.Id
                               where (AppUserIds.Contains(k.EmployeeId) &&
                               OrganizationIds.Contains(k.OrganizationId) &&
                               k.KpiYearId == KpiYearId &&
                               kcm.KpiPeriodId == KpiPeriodId &&
                               k.StatusId == StatusEnum.ACTIVE.Id &&
                               k.DeletedAt == null)
                               select new
                               {
                                   EmployeeId = k.EmployeeId,
                                   KpiCriteriaGeneralId = kc.KpiCriteriaGeneralId,
                                   Value = kcm.Value,
                               };
            List<KpiGeneralPeriodReport_SaleEmployeeDetailDTO> KpiGeneralPeriodReport_SaleEmployeeDetailDTOs = (await query_detail
                .Distinct()
                .ToListAsync())
                .Select(x => new KpiGeneralPeriodReport_SaleEmployeeDetailDTO
                {
                    SaleEmployeeId = x.EmployeeId,
                    KpiCriteriaGeneralId = x.KpiCriteriaGeneralId,
                    Value = x.Value,
                }).ToList();

            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
                x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Total = x.Total,
                    SaleEmployeeId = x.SaleEmployeeId,
                    OrderDate = x.OrderDate,
                    BuyerStore = x.BuyerStore == null ? null : new StoreDAO
                    {
                        StoreType = x.BuyerStore.StoreType == null ? null : new StoreTypeDAO
                        {
                            Code = x.BuyerStore.StoreType.Code
                        }
                    },
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

            //var DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
            //   .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
            //   x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
            //   x.RequestStateId == RequestStateEnum.APPROVED.Id)
            //   .Select(x => new DirectSalesOrderDAO
            //   {
            //       Id = x.Id,
            //       Total = x.Total,
            //       SaleEmployeeId = x.SaleEmployeeId,
            //       OrderDate = x.OrderDate,
            //       BuyerStoreId = x.BuyerStoreId,
            //       BuyerStore = x.BuyerStore == null ? null : new StoreDAO
            //       {
            //           StoreType = x.BuyerStore.StoreType == null ? null : new StoreTypeDAO
            //           {
            //               Code = x.BuyerStore.StoreType.Code
            //           }
            //       },
            //       DirectSalesOrderContents = x.DirectSalesOrderContents.Select(c => new DirectSalesOrderContentDAO
            //       {
            //           RequestedQuantity = c.RequestedQuantity,
            //           ItemId = c.ItemId
            //       }).ToList(),
            //       DirectSalesOrderPromotions = x.DirectSalesOrderPromotions.Select(x => new DirectSalesOrderPromotionDAO
            //       {
            //           RequestedQuantity = x.RequestedQuantity,
            //           ItemId = x.ItemId
            //       }).ToList()
            //   })
            //   .ToListAsync();

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
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

            var ProblemDAOs = await DataContext.Problem
                 .Where(x => AppUserIds.Contains(x.CreatorId) &&
                x.NoteAt >= StartDate && x.NoteAt <= EndDate)
                 .Select(x => new ProblemDAO
                 {
                     Id = x.Id
                 }).ToListAsync();
            var StoreImages = await DataContext.StoreImage
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId.Value) &&
                x.ShootingAt >= StartDate && x.ShootingAt <= EndDate)
                .ToListAsync();

            List<StoreDAO> Stores = await DataContext.Store.
                Where(x => AppUserIds.Contains(x.CreatorId) &&
                x.CreatedAt >= StartDate && x.CreatedAt <= EndDate)
                .Select(x => new StoreDAO
                {
                    Id = x.Id,
                    StoreScoutingId = x.StoreScoutingId,
                    StoreType = x.StoreType == null ? null : new StoreTypeDAO
                    {
                        Code = x.StoreType.Code
                    }
                }).ToListAsync();

            Parallel.ForEach(KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs, KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO =>
            {
                foreach (var SaleEmployeeDTO in KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees)
                {
                    SaleEmployeeDTO.OrganizationName = KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.OrganizationName;
                    //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                    var IndirectSalesOrders = IndirectSalesOrderDAOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                        .ToList();
                    //var DirectSalesOrders = DirectSalesOrderDAOs
                    //    .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    //    .ToList();

                    #region các chỉ tiêu tạm ẩn
                    //#region Số đơn hàng gián tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.TotalIndirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.TotalIndirectOrdersPLanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.TotalIndirectOrders = SaleEmployeeDTO.TotalIndirectOrdersPLanned == null ? null : (decimal?)IndirectSalesOrders.Count();
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.TotalIndirectOrdersRatio = (SaleEmployeeDTO.TotalIndirectOrdersPLanned == null || SaleEmployeeDTO.TotalIndirectOrders == null || SaleEmployeeDTO.TotalIndirectOrdersPLanned.Value == 0)
                    //        ? null
                    //        : (decimal?)Math.Round(SaleEmployeeDTO.TotalIndirectOrders.Value / SaleEmployeeDTO.TotalIndirectOrdersPLanned.Value * 100, 2);
                    //}
                    //#endregion

                    //#region Tổng sản lượng theo đơn gián tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.TotalIndirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.TotalIndirectQuantityPlanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.TotalIndirectQuantity = SaleEmployeeDTO.TotalIndirectQuantityPlanned == null ? null : (decimal?)IndirectSalesOrders
                    //        .SelectMany(c => c.IndirectSalesOrderContents)
                    //        .Select(q => q.RequestedQuantity)
                    //        .DefaultIfEmpty(0).Sum();
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.TotalIndirectQuantityRatio = SaleEmployeeDTO.TotalIndirectQuantityPlanned == null || SaleEmployeeDTO.TotalIndirectQuantity == null || SaleEmployeeDTO.TotalIndirectQuantityPlanned.Value == 0
                    //        ? null
                    //        : (decimal?)Math.Round(SaleEmployeeDTO.TotalIndirectQuantity.Value / SaleEmployeeDTO.TotalIndirectQuantityPlanned.Value * 100, 2);
                    //}
                    //#endregion

                    //#region SKU/Đơn hàng gián tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.SkuIndirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.SkuIndirectOrderPlanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.SKUIndirectItems = new List<long>();
                    //    foreach (var IndirectSalesOrder in IndirectSalesOrders)
                    //    {
                    //        var itemIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).Distinct().ToList();
                    //        SaleEmployeeDTO.SKUIndirectItems.AddRange(itemIds);
                    //    }

                    //    SaleEmployeeDTO.SkuIndirectOrder = IndirectSalesOrders.Count() == 0 || SaleEmployeeDTO.SKUIndirectItems == null
                    //        ? null
                    //        : (decimal?)Math.Round((decimal)SaleEmployeeDTO.SKUIndirectItems.Count() / IndirectSalesOrders.Count(), 2);
                    //    if (SaleEmployeeDTO.SkuIndirectOrderPlanned != null && SaleEmployeeDTO.SkuIndirectOrder == null)
                    //        SaleEmployeeDTO.SkuIndirectOrder = 0;
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.SkuIndirectOrderRatio = SaleEmployeeDTO.SkuIndirectOrderPlanned == null || SaleEmployeeDTO.SkuIndirectOrder == null || SaleEmployeeDTO.SkuIndirectOrderPlanned.Value == 0
                    //        ? null
                    //        : (decimal?)Math.Round((SaleEmployeeDTO.SkuIndirectOrder.Value / SaleEmployeeDTO.SkuIndirectOrderPlanned.Value) * 100, 2);
                    //}
                    //#endregion

                    //#region Số đơn hàng trực tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.TotalDirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.TotalDirectOrdersPLanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.TotalDirectOrders = SaleEmployeeDTO.TotalDirectOrdersPLanned == null ? null : (decimal?)DirectSalesOrders.Count();
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.TotalDirectOrdersRatio = (SaleEmployeeDTO.TotalDirectOrdersPLanned == null || SaleEmployeeDTO.TotalDirectOrders == null || SaleEmployeeDTO.TotalDirectOrdersPLanned.Value == 0)
                    //        ? null
                    //        : (decimal?)Math.Round(SaleEmployeeDTO.TotalDirectOrders.Value / SaleEmployeeDTO.TotalDirectOrdersPLanned.Value * 100, 2);
                    //}
                    //#endregion

                    //#region Tổng sản lượng theo đơn trực tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.TotalDirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.TotalDirectQuantityPlanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.TotalDirectQuantity = SaleEmployeeDTO.TotalDirectQuantityPlanned == null ? null : (decimal?)DirectSalesOrders
                    //        .SelectMany(c => c.DirectSalesOrderContents)
                    //        .Select(q => q.RequestedQuantity)
                    //        .DefaultIfEmpty(0).Sum();
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.TotalDirectQuantityRatio = SaleEmployeeDTO.TotalDirectQuantityPlanned == null || SaleEmployeeDTO.TotalDirectQuantity == null || SaleEmployeeDTO.TotalDirectQuantityPlanned.Value == 0
                    //        ? null
                    //        : (decimal?)Math.Round(SaleEmployeeDTO.TotalDirectQuantity.Value / SaleEmployeeDTO.TotalDirectQuantityPlanned.Value * 100, 2);
                    //}
                    //#endregion

                    //#region Doanh thu theo đơn hàng trực tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.TotalDirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.TotalDirectSalesAmountPlanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.TotalDirectSalesAmount = SaleEmployeeDTO.TotalDirectSalesAmountPlanned == null ? null : (decimal?)DirectSalesOrders.Sum(iso => iso.Total);
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.TotalDirectSalesAmountRatio = SaleEmployeeDTO.TotalDirectSalesAmountPlanned == null || SaleEmployeeDTO.TotalDirectSalesAmount == null || SaleEmployeeDTO.TotalDirectSalesAmountPlanned.Value == 0
                    //        ? null
                    //        : (decimal?)Math.Round((SaleEmployeeDTO.TotalDirectSalesAmount.Value / SaleEmployeeDTO.TotalDirectSalesAmountPlanned.Value) * 100, 2);
                    //}

                    //#endregion

                    //#region SKU/Đơn hàng trực tiếp
                    ////kế hoạch
                    //SaleEmployeeDTO.SkuDirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                    //        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                    //        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id)
                    //        .Select(x => x.Value).FirstOrDefault();
                    //if (SaleEmployeeDTO.SkuDirectOrderPlanned.HasValue)
                    //{
                    //    //thực hiện
                    //    SaleEmployeeDTO.SKUDirectItems = new List<long>();
                    //    foreach (var DirectSalesOrder in DirectSalesOrders)
                    //    {
                    //        var itemIds = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).Distinct().ToList();
                    //        SaleEmployeeDTO.SKUDirectItems.AddRange(itemIds);
                    //    }

                    //    SaleEmployeeDTO.SkuDirectOrder = DirectSalesOrders.Count() == 0 || SaleEmployeeDTO.SKUDirectItems == null
                    //        ? null
                    //        : (decimal?)Math.Round((decimal)SaleEmployeeDTO.SKUDirectItems.Count() / DirectSalesOrders.Count(), 2);
                    //    if (SaleEmployeeDTO.SkuDirectOrderPlanned != null && SaleEmployeeDTO.SkuDirectOrder == null)
                    //        SaleEmployeeDTO.SkuDirectOrder = 0;
                    //    //tỉ lệ
                    //    SaleEmployeeDTO.SkuDirectOrderRatio = SaleEmployeeDTO.SkuDirectOrderPlanned == null || SaleEmployeeDTO.SkuDirectOrder == null || SaleEmployeeDTO.SkuDirectOrderPlanned.Value == 0
                    //        ? null
                    //        : (decimal?)Math.Round((SaleEmployeeDTO.SkuDirectOrder.Value / SaleEmployeeDTO.SkuDirectOrderPlanned.Value) * 100, 2);
                    //}
                    //#endregion
                    #endregion

                    #region Tổng doanh thu đơn hàng
                    //kế hoạch
                    SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.TotalIndirectSalesAmountPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.TotalIndirectSalesAmount = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == null ? null : (decimal?)IndirectSalesOrders.Sum(iso => iso.Total);
                        //tỉ lệ
                        SaleEmployeeDTO.TotalIndirectSalesAmountRatio = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == null || SaleEmployeeDTO.TotalIndirectSalesAmount == null || SaleEmployeeDTO.TotalIndirectSalesAmountPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.TotalIndirectSalesAmount.Value / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned.Value) * 100, 2);
                    }
                    #endregion

                    #region Số đại lý ghé thăm
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
                        SaleEmployeeDTO.StoresVisitedRatio = SaleEmployeeDTO.StoresVisitedPLanned == null || SaleEmployeeDTO.StoresVisited == null || SaleEmployeeDTO.StoresVisitedPLanned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.StoresVisited.Value / SaleEmployeeDTO.StoresVisitedPLanned.Value) * 100, 2);
                    }
                    #endregion

                    #region Tổng số đại lý mở mới
                    //kế hoạch
                    SaleEmployeeDTO.NewStoreCreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId
                             && x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.NewStoreCreatedPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.NewStoreCreated = SaleEmployeeDTO.NewStoreCreatedPlanned == null || SaleEmployeeDTO.NewStoreCreatedPlanned == 0
                            ? null
                            : (decimal?)Stores
                            .Where(st => st.CreatorId == SaleEmployeeDTO.SaleEmployeeId &&
                                    (st.StoreStatusId == Enums.StoreStatusEnum.DRAFT.Id || st.StoreStatusId == Enums.StoreStatusEnum.OFFICIAL.Id)
                                    && st.DeletedAt == null)
                            .Count(); // lấy ra tất cả cửa hàng có người tạo là saleEmployee, trạng thái là dự thảo hoặc chính thức
                        //tỉ lệ
                        SaleEmployeeDTO.NewStoreCreatedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == null || SaleEmployeeDTO.NewStoreCreated == null || SaleEmployeeDTO.NewStoreCreatedPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round(SaleEmployeeDTO.NewStoreCreated.Value / SaleEmployeeDTO.NewStoreCreatedPlanned.Value * 100, 2);
                    }
                    #endregion

                    #region Tổng số lượt ghé thăm
                    //kế hoạch
                    SaleEmployeeDTO.NumberOfStoreVisitsPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.NumberOfStoreVisitsPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.NumberOfStoreVisits = SaleEmployeeDTO.NumberOfStoreVisitsPlanned == null || SaleEmployeeDTO.NumberOfStoreVisitsPlanned.Value == 0
                            ? null
                            : (decimal?)StoreCheckingDAOs
                            .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                            .Count();
                        //tỉ lệ
                        SaleEmployeeDTO.NumberOfStoreVisitsRatio = SaleEmployeeDTO.NumberOfStoreVisitsPlanned == null || SaleEmployeeDTO.NumberOfStoreVisits == null || SaleEmployeeDTO.NumberOfStoreVisitsPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round(SaleEmployeeDTO.NumberOfStoreVisits.Value / SaleEmployeeDTO.NumberOfStoreVisitsPlanned.Value * 100, 2);
                    }
                    #endregion

                    #region Doanh thu C2 Trọng điểm
                    //kế hoạch
                    SaleEmployeeDTO.RevenueC2TDPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.REVENUE_C2_TD.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.RevenueC2TDPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.RevenueC2TD = SaleEmployeeDTO.RevenueC2TDPlanned == null ?
                        null : (decimal?)IndirectSalesOrders.Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2TD).Sum(iso => iso.Total);
                        //tỉ lệ
                        SaleEmployeeDTO.RevenueC2TDRatio = SaleEmployeeDTO.RevenueC2TDPlanned == null || SaleEmployeeDTO.RevenueC2TD == null || SaleEmployeeDTO.RevenueC2TDPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.RevenueC2TD.Value / SaleEmployeeDTO.RevenueC2TDPlanned.Value) * 100, 2);
                    }
                    #endregion

                    #region Doanh thu C2 Siêu lớn
                    //kế hoạch
                    SaleEmployeeDTO.RevenueC2SLPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.REVENUE_C2_SL.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.RevenueC2SLPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.RevenueC2SL = SaleEmployeeDTO.RevenueC2SLPlanned == null ?
                        null : (decimal?)IndirectSalesOrders.Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2SL).Sum(iso => iso.Total);
                        //tỉ lệ
                        SaleEmployeeDTO.RevenueC2SLRatio = SaleEmployeeDTO.RevenueC2SLPlanned == null || SaleEmployeeDTO.RevenueC2SL == null || SaleEmployeeDTO.RevenueC2SLPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.RevenueC2SL.Value / SaleEmployeeDTO.RevenueC2SLPlanned.Value) * 100, 2);
                    }
                    #endregion

                    #region Doanh thu C2
                    //kế hoạch
                    SaleEmployeeDTO.RevenueC2Planned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.REVENUE_C2.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.RevenueC2Planned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.RevenueC2 = SaleEmployeeDTO.RevenueC2Planned == null ?
                        null : (decimal?)IndirectSalesOrders.Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2).Sum(iso => iso.Total);
                        //tỉ lệ
                        SaleEmployeeDTO.RevenueC2Ratio = SaleEmployeeDTO.RevenueC2Planned == null || SaleEmployeeDTO.RevenueC2 == null || SaleEmployeeDTO.RevenueC2Planned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.RevenueC2.Value / SaleEmployeeDTO.RevenueC2Planned.Value) * 100, 2);
                    }
                    #endregion

                    #region Số đại lý trọng điểm mở mới
                    //kế hoạch
                    SaleEmployeeDTO.NewStoreC2CreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId
                             && x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_C2_CREATED.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.NewStoreC2CreatedPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.NewStoreC2Created = SaleEmployeeDTO.NewStoreC2CreatedPlanned == null || SaleEmployeeDTO.NewStoreC2CreatedPlanned == 0
                            ? null
                            : (decimal?)Stores
                            .Where(st => st.CreatorId == SaleEmployeeDTO.SaleEmployeeId &&
                            st.StoreType.Code == StaticParams.C2TD &&
                            (st.StoreStatusId == Enums.StoreStatusEnum.DRAFT.Id || st.StoreStatusId == Enums.StoreStatusEnum.OFFICIAL.Id)
                            && st.DeletedAt == null)
                            .Count(); // lấy ra tất cả cửa hàng có người tạo là saleEmployee, trạng thái là dự thảo hoặc chính thức
                        //tỉ lệ
                        SaleEmployeeDTO.NewStoreC2CreatedRatio = SaleEmployeeDTO.NewStoreC2CreatedPlanned == null || SaleEmployeeDTO.NewStoreC2Created == null || SaleEmployeeDTO.NewStoreC2CreatedPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round(SaleEmployeeDTO.NewStoreC2Created.Value / SaleEmployeeDTO.NewStoreC2CreatedPlanned.Value * 100, 2);
                    }
                    #endregion

                    #region Số thông tin phản ánh
                    //kế hoạch
                    SaleEmployeeDTO.TotalProblemPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_PROBLEM.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.TotalProblemPlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.TotalProblem = SaleEmployeeDTO.TotalProblemPlanned == null ? 
                        null : (decimal?)ProblemDAOs.Where(x => x.CreatorId == SaleEmployeeDTO.SaleEmployeeId).Count();
                        //tỉ lệ
                        SaleEmployeeDTO.TotalProblemRatio = SaleEmployeeDTO.TotalProblemPlanned == null || SaleEmployeeDTO.TotalProblem == null || SaleEmployeeDTO.TotalProblemPlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.TotalProblem.Value / SaleEmployeeDTO.TotalProblemPlanned.Value) * 100, 2);
                    }
                    #endregion

                    #region Số hình ảnh chụp
                    //kế hoạch
                    SaleEmployeeDTO.TotalImagePlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                            .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                             x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_IMAGE.Id)
                            .Select(x => x.Value).FirstOrDefault();
                    if (SaleEmployeeDTO.TotalImagePlanned.HasValue)
                    {
                        //thực hiện
                        SaleEmployeeDTO.TotalImage = SaleEmployeeDTO.TotalImagePlanned == null ? 
                        null : (decimal?)StoreImages.Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId).Count();
                        //tỉ lệ
                        SaleEmployeeDTO.TotalImageRatio = SaleEmployeeDTO.TotalImagePlanned == null || SaleEmployeeDTO.TotalImage == null || SaleEmployeeDTO.TotalImagePlanned.Value == 0
                            ? null
                            : (decimal?)Math.Round((SaleEmployeeDTO.TotalImage.Value / SaleEmployeeDTO.TotalImagePlanned.Value) * 100, 2);
                    }
                    #endregion
                }
            });

            return KpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs;
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
