using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Hangfire.Annotations;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiPeriodGeneralReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        public KpiPeriodGeneralReportController(DataContext DataContext, IOrganizationService OrganizationService, IAppUserService AppUserService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
        }

        [Route(KpiPeriodGeneralReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiPeriodGeneralReport_AppUserDTO>> FilterListAppUser([FromBody] KpiPeriodGeneralReport_AppUserFilterDTO KpiPeriodGeneralReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiPeriodGeneralReport_AppUserFilterDTO.Id;
            AppUserFilter.Id = KpiPeriodGeneralReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiPeriodGeneralReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiPeriodGeneralReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiPeriodGeneralReport_AppUserDTO> KpiPeriodGeneralReport_AppUserDTOs = AppUsers
                .Select(x => new KpiPeriodGeneralReport_AppUserDTO(x)).ToList();
            return KpiPeriodGeneralReport_AppUserDTOs;
        }

        [Route(KpiPeriodGeneralReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiPeriodGeneralReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiPeriodGeneralReport_OrganizationFilterDTO KpiPeriodGeneralReport_OrganizationFilterDTO)
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
            List<KpiPeriodGeneralReport_OrganizationDTO> KpiPeriodGeneralReport_OrganizationDTOs = Organizations
                .Select(x => new KpiPeriodGeneralReport_OrganizationDTO(x)).ToList();
            return KpiPeriodGeneralReport_OrganizationDTOs;
        }

        [Route(KpiPeriodGeneralReportRoute.FilterListKpiPeriod), HttpPost]
        public List<GenericEnum> FilterListKpiPeriod()
        {
            return KpiPeriodEnum.KpiPeriodEnumList;
        }

        [Route(KpiPeriodGeneralReportRoute.FilterListKpiYear), HttpPost]
        public List<GenericEnum> FilterListKpiYear()
        {
            return KpiYearEnum.KpiYearEnumList;
        }

        [Route(KpiPeriodGeneralReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState); // to do kpi year and period
            OrganizationDAO OrganizationDAO = null;
            long? SaleEmployeeId = KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.SaleEmployeeId?.Equal;
            if (KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select ap.Id;
            return await query.Distinct().CountAsync();
        }

        [Route(KpiPeriodGeneralReportRoute.List), HttpPost]
        public async Task<List<KpiPeriodGeneralReport_KpiPeriodGeneralReportDTO>> List([FromBody] KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            OrganizationDAO OrganizationDAO = null;
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.SaleEmployeeId?.Equal;
            long KpiPeriodId = KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
            long KpiYearId = KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            if (KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }

            // list toan bo nhan vien sau khi chay qua filter
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select new KpiPeriodGeneralReport_SaleEmployeeDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                        };
            List<KpiPeriodGeneralReport_SaleEmployeeDTO> SaleEmployeeDTOs = await query.Distinct().ToListAsync();

            List<long> SaleEmployeeIds = SaleEmployeeDTOs.Select(x => x.SaleEmployeeId).ToList();
            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where (SaleEmployeeIds.Contains(c.EmployeeId) &&
                                      c.KpiYearId == KpiYearId &&
                                      a.KpiPeriodId == KpiPeriodId)
                               select new KpiPeriodGeneralReport_SaleEmployeeDetailDTO
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaGeneralId = b.KpiCriteriaGeneralId,
                                   Value = a.Value.Value,
                               };
            List<KpiPeriodGeneralReport_SaleEmployeeDetailDTO> SaleEmployeeDetailDTOs = await query_detail.Distinct().ToListAsync();

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
                .ToListAsync(); // to do x.OrderDate

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

            //var query_store = from a in DataContext.Store
            //                  join b in DataContext.StoreScouting on a.StoreScoutingId equals b.Id
            //                  where (SaleEmployeeIds.Contains(b.CreatorId))
            //                  select new StoreScoutingDAO

            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => SaleEmployeeIds.Contains(x.CreatorId))
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
                SaleEmployeeDTO.TotalIndirectOrdersRatio = Math.Round(SaleEmployeeDTO.TotalIndirectOrders / SaleEmployeeDTO.TotalIndirectOrdersPLanned, 2);


                // TOTALINDIRECTOUTPUT
                SaleEmployeeDTO.TotalIndirectOutputPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTOUTPUT.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectOutput = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(iso => iso.IndirectSalesOrderContents)
                    .Select(z => z.Quantity)
                    .DefaultIfEmpty(0).Sum();
                SaleEmployeeDTO.TotalIndirectOutputRatio = Math.Round(SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOutputPlanned, 2);

                // TOTALINDIRECTSALESAMOUNT
                SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTSALESAMOUNT.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.TotalIndirectSalesAmount = IndirectSalesOrderDAOs
                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Sum(iso => iso.Total);
                SaleEmployeeDTO.TotalIndirectSalesAmountRatio = Math.Round(SaleEmployeeDTO.TotalIndirectSalesAmount / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned, 2);

                // SKUINDIRECTORDER
                SaleEmployeeDTO.SkuIndirectOrderPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.SKUINDIRECTORDER.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOrders;
                SaleEmployeeDTO.SkuIndirectOrder = Math.Round(SaleEmployeeDTO.SkuIndirectOrder / SaleEmployeeDTO.SkuIndirectOrderPlanned, 2);

                // STORESVISITED
                SaleEmployeeDTO.StoresVisitedPLanned= SaleEmployeeDetailDTOs
                       .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.STORESVISITED.Id)
                       .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.StoresVisited = StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                SaleEmployeeDTO.StoresVisitedRatio = Math.Round(SaleEmployeeDTO.StoresVisited / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);

                // NEWSTORECREATED
                SaleEmployeeDTO.NewStoreCreatedPlanned = SaleEmployeeDetailDTOs
                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.NEWSTORECREATED.Id)
                        .Select(sed => sed.Value).FirstOrDefault();
                SaleEmployeeDTO.NewStoreCreated = StoreScoutingDAOs
                    .Where(sc => sc.CreatorId == SaleEmployeeDTO.SaleEmployeeId)
                    .SelectMany(sc => sc.Stores)
                    .Select(z => z.StoreScoutingId.HasValue)
                    .Count();
                SaleEmployeeDTO.NewStoreCreatedRatio = Math.Round(SaleEmployeeDTO.NewStoreCreated / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);
            };

            List<KpiPeriodGeneralReport_KpiPeriodGeneralReportDTO> KpiPeriodGeneralReport_KpiPeriodGeneralReportDTOs = new List<KpiPeriodGeneralReport_KpiPeriodGeneralReportDTO>();

            #region delete
            //List < KpiPeriodGeneralReport_SaleEmployeeDTO > KpiPeriodGeneralReport_SaleEmployeeDTOs = await query.Distinct().OrderBy(q => q.DisplayName)
            //    .Skip(KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.Skip)
            //    .Take(KpiPeriodGeneralReport_KpiPeriodGeneralReportFilterDTO.Take)
            //    .ToListAsync(); // to do

            //List<KpiGeneralDAO> KpiGeneralDAOs = await DataContext.KpiGeneral.ToListAsync();
            //List<KpiGeneralContentDAO> KpiGeneralContentDAOs = await DataContext.KpiGeneralContent.ToListAsync();
            //List<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappingDAOs = await DataContext.KpiGeneralContentKpiPeriodMapping.ToListAsync();
            //foreach (var KpiPeriodGeneralReport_SaleEmployeeDTO in KpiPeriodGeneralReport_SaleEmployeeDTOs)
            //{
            //    // get KPI General
            //    var KpiGeneralDTO = KpiGeneralDAOs.Where(x => x.EmployeeId == KpiPeriodGeneralReport_SaleEmployeeDTO.SaleEmployeeId && x.KpiYearId == KpiYearId).FirstOrDefault();
            //    //Get KPI General Content (6 for each employee)
            //    var KpiGeneralContentDTOS = KpiGeneralContentDAOs.Where(x => x.KpiGeneralId == KpiGeneralDTO.Id);
            //    //get value for each employee
            //    foreach (var KpiGeneralContentDTO in KpiGeneralContentDTOS)
            //    {
            //        var KpiGeneralContentKpiPeriodMappingDTO = KpiGeneralContentKpiPeriodMappingDAOs.Where(x => x.KpiGeneralContentId == KpiGeneralContentDTO.Id && x.KpiPeriodId == KpiPeriodId).FirstOrDefault();

            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTORDERS.Id)
            //            KpiPeriodGeneralReport_SaleEmployeeDTO.TotalIndirectOrdersPLanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTOUTPUT.Id)
            //            KpiPeriodGeneralReport_SaleEmployeeDTO.TotalIndirectOutputPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTSALESAMOUNT.Id)
            //            KpiPeriodGeneralReport_SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.SKUINDIRECTORDER.Id)
            //            KpiPeriodGeneralReport_SaleEmployeeDTO.SkuIndirectOrderPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.STORESVISITED.Id)
            //            KpiPeriodGeneralReport_SaleEmployeeDTO.StoresVisitedPLanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.NEWSTORECREATED.Id)
            //            KpiPeriodGeneralReport_SaleEmployeeDTO.NewStoreCreatedPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //    }
            //}
            #endregion

            return null;
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
