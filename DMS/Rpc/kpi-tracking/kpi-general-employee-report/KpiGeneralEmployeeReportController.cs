using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MKpiGeneral;
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
                return BadRequest("Chưa chọn nhân viên");

            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.AppUserId.Equal.Value;
            long? KpiPeriodId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiPeriodId?.Equal;
            long? KpiYearId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiYearId?.Equal;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            var KpiGeneralId = await DataContext.KpiGeneral
                .Where(x => x.EmployeeId == SaleEmployeeId.Value &&
                x.StatusId == StatusEnum.ACTIVE.Id &&
                x.DeletedAt == null)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            var KpiGeneral = await KpiGeneralService.Get(KpiGeneralId);

            if (KpiGeneral == null)
                return new List<KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO>();
            var KpiPeriodIds = KpiGeneral.KpiGeneralContents
                .SelectMany(x => x.KpiGeneralContentKpiPeriodMappings)
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
            foreach (var KpiPeriod in KpiPeriods)
            {
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO
                    = new KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO();
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiPeriodId = KpiPeriod.Id;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiPeriodName = KpiPeriod.Name;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiYearId = KpiGeneral.KpiYearId;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiYearName = KpiGeneral.KpiYear.Name;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SaleEmployeeId = SaleEmployeeId.Value;
                KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs.Add(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO);
            }
            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => x.SaleEmployeeId == SaleEmployeeId && x.OrderDate >= StartDate && x.OrderDate <= EndDate)
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Total = x.Total,
                    SaleEmployeeId = x.SaleEmployeeId,
                    OrderDate = x.OrderDate,
                    IndirectSalesOrderContents = x.IndirectSalesOrderContents.Select(c => new IndirectSalesOrderContentDAO
                    {
                        IndirectSalesOrderId = x.Id,
                        Quantity = c.Quantity,
                        ItemId = c.ItemId,
                    }).ToList(),
                })
                .ToListAsync();

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => x.SaleEmployeeId == SaleEmployeeId && 
                x.CheckOutAt.HasValue && x.CheckOutAt.Value >= StartDate && x.CheckOutAt.Value <= EndDate)
                .Select(x => new StoreCheckingDAO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt
                })
                .ToListAsync();

            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => x.CreatorId == SaleEmployeeId && x.CreatedAt >= StartDate && x.CreatedAt <= EndDate)
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

            foreach (var Period in KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs)
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

                //lấy tất cả lượt checking trong kì đang xét
                var StoreCheckings = StoreCheckingDAOs
                    .Where(x => x.SaleEmployeeId == Period.SaleEmployeeId &&
                    x.CheckOutAt.HasValue && x.CheckOutAt.Value >= Start && x.CheckOutAt.Value <= End)
                    .ToList();

                #region Số đơn hàng gián tiếp
                //kế hoạch
                Period.TotalIndirectOrdersPLanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value == null ? 0 : x.Value.Value)
                        .FirstOrDefault();
                //thực hiện
                Period.TotalIndirectOrders = IndirectSalesOrders.Count();
                //tỉ lệ
                Period.TotalIndirectOrdersRatio = Period.TotalIndirectOrdersPLanned == 0 ?
                    0.00m : Math.Round((Period.TotalIndirectOrders / Period.TotalIndirectOrdersPLanned) * 100, 2);
                #endregion

                #region Tổng sản lượng theo đơn gián tiếp
                //kế hoạch
                Period.TotalIndirectQuantityPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id)
                        .Select(x => x.Value == null ? 0 : x.Value.Value)
                        .FirstOrDefault();
                //thực hiện
                foreach (var IndirectSalesOrder in IndirectSalesOrders)
                {
                    foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                    {
                        Period.TotalIndirectQuantity += content.RequestedQuantity;
                    }
                }
                //tỉ lệ
                Period.TotalIndirectQuantityRatio = Period.TotalIndirectQuantityPlanned == 0 ?
                    0.00m : Math.Round((Period.TotalIndirectQuantity / Period.TotalIndirectQuantityPlanned) * 100, 2);
                #endregion

                #region Doanh thu theo đơn hàng gián tiếp
                //kế hoạch
                Period.TotalIndirectSalesAmountPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                        .Select(x => x.Value == null ? 0 : x.Value.Value)
                        .FirstOrDefault();
                //thực hiện
                Period.TotalIndirectSalesAmount = IndirectSalesOrders.Sum(x => x.Total);
                //tỉ lệ
                Period.TotalIndirectSalesAmountRatio = Period.TotalIndirectSalesAmountPlanned == 0 ?
                    0.00m : Math.Round((Period.TotalIndirectSalesAmount / Period.TotalIndirectSalesAmountPlanned) * 100, 2);
                #endregion

                #region SKU/Đơn hàng gián tiếp
                //kế hoạch
                Period.SkuIndirectOrderPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id)
                        .Select(x => x.Value == null ? 0 : x.Value.Value)
                        .FirstOrDefault();
                //thực hiện
                Period.SKUItems = new HashSet<long>();
                foreach (var IndirectSalesOrder in IndirectSalesOrders)
                {
                    foreach (var content in IndirectSalesOrder.IndirectSalesOrderContents)
                    {
                        Period.SKUItems.Add(content.ItemId);
                    }
                }
                Period.SkuIndirectOrder = Period.TotalIndirectOrders == 0 ?
                    0 : Math.Round(Period.SKUItems.Count() / Period.TotalIndirectOrders, 2);
                //tỉ lệ
                Period.SkuIndirectOrderRatio = Period.SkuIndirectOrderPlanned == 0 ?
                    0.00m : Math.Round((Period.SkuIndirectOrder / Period.SkuIndirectOrderPlanned) * 100, 2);
                #endregion

                #region Số cửa hàng viếng thăm
                //kế hoạch
                Period.StoresVisitedPLanned = KpiGeneralContentKpiPeriodMappings
                       .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                       x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                       .Select(x => x.Value == null ? 0 : x.Value.Value)
                       .FirstOrDefault();
                //thực hiện
                Period.StoreIds = new HashSet<long>();
                foreach (var StoreChecking in StoreCheckings)
                {
                    Period.StoreIds.Add(StoreChecking.StoreId);
                }
                //tỉ lệ
                Period.StoresVisitedRatio = Period.StoresVisitedPLanned == 0 ?
                    0.00m : Math.Round((Period.StoresVisited / Period.StoresVisitedPLanned) * 100, 2);
                #endregion

                #region Số cửa hàng tạo mới
                //kế hoạch
                Period.NewStoreCreatedPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                        .Select(x => x.Value == null ? 0 : x.Value.Value)
                        .FirstOrDefault();
                //thực hiện
                Period.NewStoreCreated = StoreScoutingDAOs
                    .Where(sc => sc.CreatorId == Period.SaleEmployeeId &&
                    sc.CreatedAt >= Start && sc.CreatedAt <= End)
                    .SelectMany(sc => sc.Stores)
                    .Where(x => x.StoreScoutingId.HasValue)
                    .Select(z => z.StoreScoutingId.Value)
                    .Count();
                //tỉ lệ
                Period.NewStoreCreatedRatio = Period.NewStoreCreatedPlanned == 0 ?
                    0.00m : Math.Round((Period.NewStoreCreated / Period.NewStoreCreatedPlanned) * 100, 2);
                #endregion

                #region Số lần viếng thăm cửa hàng
                //kế hoạch
                Period.NumberOfStoreVisitsPlanned = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.KpiPeriodId == Period.KpiPeriodId &&
                        x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                        .Select(x => x.Value == null ? 0 : x.Value.Value)
                        .FirstOrDefault();
                //thực hiện
                Period.NumberOfStoreVisits = StoreCheckings.Count();
                //tỉ lệ
                Period.NumberOfStoreVisitsRatio = Period.NumberOfStoreVisitsPlanned == 0 ?
                    0.00m : Math.Round((Period.NumberOfStoreVisits / Period.NumberOfStoreVisitsPlanned) * 100, 2);
                #endregion
            };

            return KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTOs;
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
