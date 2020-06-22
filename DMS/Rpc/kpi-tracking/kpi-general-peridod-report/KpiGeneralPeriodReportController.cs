using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
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
        public KpiGeneralPeriodReportController(DataContext DataContext, IOrganizationService OrganizationService, IAppUserService AppUserService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
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

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiGeneralPeriodReport_OrganizationDTO> KpiGeneralPeriodReport_OrganizationDTOs = Organizations
                .Select(x => new KpiGeneralPeriodReport_OrganizationDTO(x)).ToList();
            return KpiGeneralPeriodReport_OrganizationDTOs;
        }

        [Route(KpiGeneralPeriodReportRoute.FilterListKpiPeriod), HttpPost]
        public List<GenericEnum> FilterListKpiPeriod()
        {
            return KpiPeriodEnum.KpiPeriodEnumList;
        }

        [Route(KpiGeneralPeriodReportRoute.FilterListKpiYear), HttpPost]
        public List<GenericEnum> FilterListKpiYear()
        {
            return KpiYearEnum.KpiYearEnumList;
        }

        [Route(KpiGeneralPeriodReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState); // to do kpi year and period
            OrganizationDAO OrganizationDAO = null;
            long? SaleEmployeeId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.SaleEmployeeId?.Equal;
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select ap.Id;
            return await query.Distinct().CountAsync();
        }

        [Route(KpiGeneralPeriodReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO>>> List([FromBody] KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId == null) return BadRequest("Chưa chọn kì KPI");
            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId == null) return BadRequest("Chưa chọn năm KPI");

            OrganizationDAO OrganizationDAO = null;
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.SaleEmployeeId?.Equal;
            long KpiPeriodId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
            long KpiYearId = KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            if (KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }

            // list toan bo nhan vien sau khi chay qua filter
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select new KpiGeneralPeriodReport_SaleEmployeeDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                            OrganizationId = ap.OrganizationId.Value
                        };
            List<KpiGeneralPeriodReport_SaleEmployeeDTO> SaleEmployeeDTOs = await query.Distinct()
                .OrderBy(q => q.DisplayName)
                .Skip(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Skip)
                .Take(KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO.Take)
                .ToListAsync();
            
            //get organization
            List<long> OrganizationIds = SaleEmployeeDTOs.Where(x => x.OrganizationId.HasValue).Select(x => x.OrganizationId.Value).ToList();
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Name,
                Id = new IdFilter { In = OrganizationIds }
            });

            List<long> SaleEmployeeIds = SaleEmployeeDTOs.Select(x => x.SaleEmployeeId).ToList();
            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where (SaleEmployeeIds.Contains(c.EmployeeId) &&
                                      c.KpiYearId == KpiYearId &&
                                      a.KpiPeriodId == KpiPeriodId)
                               select new KpiGeneralPeriodReport_SaleEmployeeDetailDTO
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaGeneralId = b.KpiCriteriaGeneralId,
                                   Value = a.Value.Value,
                               };
            List<KpiGeneralPeriodReport_SaleEmployeeDetailDTO> SaleEmployeeDetailDTOs = await query_detail.Distinct().ToListAsync();

            

            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) && x.OrderDate >= StartDate && x.OrderDate <= EndDate)
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
                .ToListAsync(); // to do 

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) && x.CheckOutAt.HasValue && x.CheckOutAt.Value >= StartDate && x.CheckOutAt.Value <= EndDate)
                .Select(x => new StoreCheckingDAO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt
                })
                .ToListAsync();


            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => SaleEmployeeIds.Contains(x.CreatorId) && x.CreatedAt >= StartDate)
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

            foreach (var SaleEmployeeDTO in SaleEmployeeDTOs)
            {
                // TOTALINDIRECTORDERS
                SaleEmployeeDTO.TotalIndirectOrdersPLanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTORDERS.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectOrders = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                SaleEmployeeDTO.TotalIndirectOrdersRatio = SaleEmployeeDTO.TotalIndirectOrdersPLanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectOrders / SaleEmployeeDTO.TotalIndirectOrdersPLanned, 2);


                // TOTALINDIRECTOUTPUT
                SaleEmployeeDTO.TotalIndirectOutputPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTOUTPUT.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectOutput = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(iso => iso.IndirectSalesOrderContents)
                    .Select(z => z.Quantity)
                    .DefaultIfEmpty(0).Sum();
                SaleEmployeeDTO.TotalIndirectOutputRatio = SaleEmployeeDTO.TotalIndirectOutputPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOutputPlanned, 2);

                // TOTALINDIRECTSALESAMOUNT
                SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTSALESAMOUNT.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectSalesAmount = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Sum(iso => iso.Total);
                SaleEmployeeDTO.TotalIndirectSalesAmountRatio = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectSalesAmount / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned, 2);

                // SKUINDIRECTORDER
                SaleEmployeeDTO.SkuIndirectOrderPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.SKUINDIRECTORDER.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.TotalIndirectOrders == 0 ? 0 : SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOrders;
                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.SkuIndirectOrderPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.SkuIndirectOrder / SaleEmployeeDTO.SkuIndirectOrderPlanned, 2);

                // STORESVISITED
                SaleEmployeeDTO.StoresVisitedPLanned= SaleEmployeeDetailDTOs
                       .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.STORESVISITED.Id)
                       .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.StoresVisited = StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                SaleEmployeeDTO.StoresVisitedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.StoresVisited / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);

                // NEWSTORECREATED
                SaleEmployeeDTO.NewStoreCreatedPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.NEWSTORECREATED.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.NewStoreCreated = StoreScoutingDAOs
                    .Where(sc => sc.CreatorId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(sc => sc.Stores)
                    .Select(z => z.StoreScoutingId.HasValue)
                    .Count();
                SaleEmployeeDTO.NewStoreCreatedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.NewStoreCreated / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);
            };

            List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO> kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs = new List<KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO>();
            foreach (Organization Organization in Organizations)
            {
                KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO = new KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO()
                {
                    OrganizationName = Organization.Name,
                    SaleEmployees = SaleEmployeeDTOs.Where(x => x.OrganizationName.Equals(Organization.Name)).ToList()
                };
                if (KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees.Count() > 0) kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs.Add(KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO);
            }
            return kpiGeneralPeriodReport_KpiGeneralPeriodReportDTOs;
        }

        private Tuple<DateTime, DateTime> DateTimeConvert(long KpiPeriodId, long KpiYearId)
        {
            DateTime startDate =  StaticParams.DateTimeNow;
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
