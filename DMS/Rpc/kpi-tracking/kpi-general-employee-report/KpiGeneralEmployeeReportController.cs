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

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IKpiPeriodService KpiPeriodService;
        public KpiGeneralEmployeeReportController(DataContext DataContext, 
            IOrganizationService OrganizationService, 
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IKpiPeriodService KpiPeriodService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
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
                throw new BindException(ModelState); // to do kpi year and period

            DateTime StartDate, EndDate;
            long SaleEmployeeId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.SaleEmployeeId.Equal ?? 1;
            long? KpiPeriodId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiPeriodId?.Equal;
            long? KpiYearId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiYearId?.Equal;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where (SaleEmployeeId == c.EmployeeId
                                      && (KpiYearId == null || c.KpiYearId == KpiYearId)
                                      && (KpiPeriodId == null || a.KpiPeriodId == KpiPeriodId))
                               select new KpiGeneralEmployeeReport_SaleEmployeeDetailDTO
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaGeneralId = b.KpiCriteriaGeneralId,
                                   KpiPeriodId = a.KpiPeriodId,
                                   Value = a.Value.Value,
                               };
            return await query_detail.Distinct().CountAsync();
        }

        [Route(KpiGeneralEmployeeReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiGeneralEmployeeReport_SaleEmployeeDTO>>> List([FromBody] KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.SaleEmployeeId == null) return BadRequest("Chưa chọn nhân viên");

            DateTime StartDate, EndDate;
            long SaleEmployeeId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.SaleEmployeeId.Equal ?? 1;
            long? KpiPeriodId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiPeriodId?.Equal;
            long? KpiYearId = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.KpiYearId?.Equal;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where (c.EmployeeId == SaleEmployeeId
                                      && (KpiYearId == null || c.KpiYearId == KpiYearId)
                                      && (KpiPeriodId == null || a.KpiPeriodId == KpiPeriodId))
                               select new KpiGeneralEmployeeReport_SaleEmployeeDetailDTO
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaGeneralId = b.KpiCriteriaGeneralId,
                                   KpiPeriodId = a.KpiPeriodId,
                                   Value = a.Value.Value,
                               };
            List<KpiGeneralEmployeeReport_SaleEmployeeDetailDTO> KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs = await query_detail
                .Distinct()
                .OrderBy(x => x.SaleEmployeeId)
                .Skip(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.Skip)
                .Take(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO.Take)
                .ToListAsync();

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
                        Quantity = c.Quantity
                    }).ToList(),
                })
                .ToListAsync();

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => x.SaleEmployeeId == SaleEmployeeId && x.CheckOutAt.HasValue && x.CheckOutAt.Value >= StartDate && x.CheckOutAt.Value <= EndDate)
                .Select(x => new StoreCheckingDAO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt
                })
                .ToListAsync();


            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => x.CreatorId == SaleEmployeeId && x.CreatedAt >= StartDate)
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


            List<KpiGeneralEmployeeReport_SaleEmployeeDTO> kpiGeneralEmployeeReport_SaleEmployeeDTOs = new List<KpiGeneralEmployeeReport_SaleEmployeeDTO>();   
            foreach (var SaleEmployeeDetailDTO in KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs)
            {
                KpiGeneralEmployeeReport_SaleEmployeeDTO SaleEmployeeDTO = new KpiGeneralEmployeeReport_SaleEmployeeDTO();

                SaleEmployeeDTO.SaleEmployeeId = SaleEmployeeId;
                SaleEmployeeDTO.KpiPeriodId = SaleEmployeeDetailDTO.KpiPeriodId;
                foreach (var KpiPeriod in KpiPeriodEnum.KpiPeriodEnumList)
                {
                    if (KpiPeriod.Id == SaleEmployeeDTO.KpiPeriodId) SaleEmployeeDTO.KpiPeriodName = KpiPeriod.Name;
                }
                // TOTALINDIRECTORDERS
                SaleEmployeeDTO.TotalIndirectOrdersPLanned = KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTORDERS.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectOrders = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                SaleEmployeeDTO.TotalIndirectOrdersRatio = SaleEmployeeDTO.TotalIndirectOrdersPLanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectOrders / SaleEmployeeDTO.TotalIndirectOrdersPLanned, 2);


                // TOTALINDIRECTOUTPUT
                SaleEmployeeDTO.TotalIndirectOutputPlanned = KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTOUTPUT.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectOutput = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(iso => iso.IndirectSalesOrderContents)
                    .Select(z => z.Quantity)
                    .DefaultIfEmpty(0).Sum();
                SaleEmployeeDTO.TotalIndirectOutputRatio = SaleEmployeeDTO.TotalIndirectOutputPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOutputPlanned, 2);

                // TOTALINDIRECTSALESAMOUNT
                SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTSALESAMOUNT.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectSalesAmount = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Sum(iso => iso.Total);
                SaleEmployeeDTO.TotalIndirectSalesAmountRatio = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectSalesAmount / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned, 2);

                // SKUINDIRECTORDER
                SaleEmployeeDTO.SkuIndirectOrderPlanned = KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.SKUINDIRECTORDER.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.TotalIndirectOrders == 0 ? 0 : SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOrders;
                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.SkuIndirectOrderPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.SkuIndirectOrder / SaleEmployeeDTO.SkuIndirectOrderPlanned, 2);

                // STORESVISITED
                SaleEmployeeDTO.StoresVisitedPLanned= KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs
                       .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.STORESVISITED.Id)
                       .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.StoresVisited = StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                SaleEmployeeDTO.StoresVisitedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.StoresVisited / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);

                // NEWSTORECREATED
                SaleEmployeeDTO.NewStoreCreatedPlanned = KpiGeneralEmployeeReport_SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.NEWSTORECREATED.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.NewStoreCreated = StoreScoutingDAOs
                    .Where(sc => sc.CreatorId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(sc => sc.Stores)
                    .Select(z => z.StoreScoutingId.HasValue)
                    .Count();
                SaleEmployeeDTO.NewStoreCreatedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.NewStoreCreated / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);

                //
                kpiGeneralEmployeeReport_SaleEmployeeDTOs.Add(SaleEmployeeDTO);
            };

            
            return kpiGeneralEmployeeReport_SaleEmployeeDTOs;
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
