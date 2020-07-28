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
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
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
            long KpiPeriodId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
            long KpiYearId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            var query = from k in DataContext.KpiGeneral
                        join au in DataContext.AppUser on k.EmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where OrganizationIds.Contains(o.Id) &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                        k.KpiYearId == KpiYearId &&
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
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId == null) return BadRequest("Chưa chọn kì KPI");
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId == null) return BadRequest("Chưa chọn năm KPI");

            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.AppUserId?.Equal;
            long KpiPeriodId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
            long KpiYearId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            // list toan bo nhan vien trong organization do va cac con ma co kpi general
            var query = from k in DataContext.KpiGeneral
                        join au in DataContext.AppUser on k.EmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where OrganizationIds.Contains(o.Id) &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                        k.KpiYearId == KpiYearId &&
                        k.StatusId == StatusEnum.ACTIVE.Id &&
                        k.DeletedAt == null
                        select new
                        {
                            SaleEmployeeId = au.Id,
                            Username = au.Username,
                            DisplayName = au.DisplayName,
                            OrganizationName = au.Organization == null ? null : au.Organization.Name,
                            OrganizationId = au.OrganizationId.Value
                        };
            List<KpiGeneralPeriodReport_SaleEmployeeDTO> KpiGeneralPeriodReport_SaleEmployeeDTOs = (await query
                .Distinct()
                .OrderBy(x => x.DisplayName)
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
            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where (SaleEmployeeIds.Contains(c.EmployeeId) &&
                               OrganizationIds.Contains(c.OrganizationId) &&
                               c.KpiYearId == KpiYearId &&
                               a.KpiPeriodId == KpiPeriodId &&
                               c.StatusId == StatusEnum.ACTIVE.Id &&
                               c.DeletedAt == null)
                               select new
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaGeneralId = b.KpiCriteriaGeneralId,
                                   Value = a.Value.Value,
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
                x.OrderDate >= StartDate && x.OrderDate <= EndDate)
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
                })
                .ToListAsync(); // to do 

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
                #region Số đơn hàng gián tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalIndirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalIndirectOrders = IndirectSalesOrders.Count();
                //tỉ lệ
                SaleEmployeeDTO.TotalIndirectOrdersRatio = SaleEmployeeDTO.TotalIndirectOrdersPLanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.TotalIndirectOrders / SaleEmployeeDTO.TotalIndirectOrdersPLanned) * 100, 2);
                #endregion

                #region Tổng sản lượng theo đơn gián tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalIndirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalIndirectQuantity = IndirectSalesOrders
                    .SelectMany(c => c.IndirectSalesOrderContents)
                    .Select(q => q.RequestedQuantity)
                    .DefaultIfEmpty(0).Sum();
                //tỉ lệ
                SaleEmployeeDTO.TotalIndirectQuantityRatio = SaleEmployeeDTO.TotalIndirectQuantityPlanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.TotalIndirectQuantity / SaleEmployeeDTO.TotalIndirectQuantityPlanned) * 100, 2);
                #endregion

                #region Doanh thu theo đơn hàng gián tiếp
                //kế hoạch
                SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.TotalIndirectSalesAmount = IndirectSalesOrders.Sum(iso => iso.Total);
                //tỉ lệ
                SaleEmployeeDTO.TotalIndirectSalesAmountRatio = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.TotalIndirectSalesAmount / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned) * 100, 2);
                #endregion

                #region SKU/Đơn hàng gián tiếp
                //kế hoạch
                SaleEmployeeDTO.SkuIndirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                var IndirectSalesOrderContents = IndirectSalesOrders
                    .SelectMany(x => x.IndirectSalesOrderContents)
                    .ToList();
                SaleEmployeeDTO.SKUItems = new HashSet<long>();
                foreach (var content in IndirectSalesOrderContents)
                {
                    SaleEmployeeDTO.SKUItems.Add(content.ItemId);
                }
                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.TotalIndirectOrders == 0 ?
                    0.00m : Math.Round(SaleEmployeeDTO.SKUItems.Count() / SaleEmployeeDTO.TotalIndirectOrders, 2);
                //tỉ lệ
                SaleEmployeeDTO.SkuIndirectOrderRatio = SaleEmployeeDTO.SkuIndirectOrderPlanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.SkuIndirectOrder / SaleEmployeeDTO.SkuIndirectOrderPlanned) * 100, 2);
                #endregion

                #region Số cửa hàng viếng thăm
                //kế hoạch
                SaleEmployeeDTO.StoresVisitedPLanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                       .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                        x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                       .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                var StoreIds = StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Select(x => x.StoreId)
                    .ToList();
                SaleEmployeeDTO.StoreIds = new HashSet<long>();
                foreach (var StoreId in StoreIds)
                {
                    SaleEmployeeDTO.StoreIds.Add(StoreId);
                }
                //tỉ lệ
                SaleEmployeeDTO.StoresVisitedRatio = SaleEmployeeDTO.StoresVisitedPLanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.StoresVisited / SaleEmployeeDTO.StoresVisitedPLanned) * 100, 2);
                #endregion

                #region Số cửa hàng tạo mới
                //kế hoạch
                SaleEmployeeDTO.NewStoreCreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId
                         && x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.NewStoreCreated = StoreScoutingDAOs
                    .Where(sc => sc.CreatorId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(sc => sc.Stores)
                    .Where(x => x.StoreScoutingId.HasValue)
                    .Select(z => z.StoreScoutingId.Value)
                    .Count();
                //tỉ lệ
                SaleEmployeeDTO.NewStoreCreatedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.NewStoreCreated / SaleEmployeeDTO.NewStoreCreatedPlanned) * 100, 2);
                #endregion

                #region Số lần viếng thăm cửa hàng
                //kế hoạch
                SaleEmployeeDTO.NumberOfStoreVisitsPlanned = KpiGeneralPeriodReport_SaleEmployeeDetailDTOs
                        .Where(x => x.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId &&
                         x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                        .Select(x => x.Value).FirstOrDefault();
                //thực hiện
                SaleEmployeeDTO.NumberOfStoreVisits = StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                //tỉ lệ
                SaleEmployeeDTO.NumberOfStoreVisitsRatio = SaleEmployeeDTO.NumberOfStoreVisitsPlanned == 0 ?
                    0.00m : Math.Round((SaleEmployeeDTO.NumberOfStoreVisits / SaleEmployeeDTO.NumberOfStoreVisitsPlanned) * 100, 2);
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
