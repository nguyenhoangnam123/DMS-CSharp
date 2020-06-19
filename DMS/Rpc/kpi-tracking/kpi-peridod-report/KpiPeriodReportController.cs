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

namespace DMS.Rpc.kpi_tracking.kpi_period_report
{
    public class KpiPeriodReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        public KpiPeriodReportController(DataContext DataContext, IOrganizationService OrganizationService, IAppUserService AppUserService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
        }

        [Route(KpiPeriodReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiPeriodReport_AppUserDTO>> FilterListAppUser([FromBody] KpiPeriodReport_AppUserFilterDTO KpiPeriodReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiPeriodReport_AppUserFilterDTO.Id;
            AppUserFilter.Id = KpiPeriodReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiPeriodReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiPeriodReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiPeriodReport_AppUserDTO> KpiPeriodReport_AppUserDTOs = AppUsers
                .Select(x => new KpiPeriodReport_AppUserDTO(x)).ToList();
            return KpiPeriodReport_AppUserDTOs;
        }

        [Route(KpiPeriodReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiPeriodReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiPeriodReport_OrganizationFilterDTO KpiPeriodReport_OrganizationFilterDTO)
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
            List<KpiPeriodReport_OrganizationDTO> KpiPeriodReport_OrganizationDTOs = Organizations
                .Select(x => new KpiPeriodReport_OrganizationDTO(x)).ToList();
            return KpiPeriodReport_OrganizationDTOs;
        }

        [Route(KpiPeriodReportRoute.FilterListKpiPeriod), HttpPost]
        public List<GenericEnum> FilterListKpiPeriod()
        {
            return KpiPeriodEnum.KpiPeriodEnumList;
        }

        [Route(KpiPeriodReportRoute.FilterListKpiYear), HttpPost]
        public List<GenericEnum> FilterListKpiYear()
        {
            return KpiYearEnum.KpiYearEnumList;
        }

        [Route(KpiPeriodReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiPeriodReport_KpiPeriodReportFilterDTO KpiPeriodReport_KpiPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState); // to do kpi year and period
            OrganizationDAO OrganizationDAO = null;
            long? SaleEmployeeId = KpiPeriodReport_KpiPeriodReportFilterDTO.SaleEmployeeId?.Equal;
            if (KpiPeriodReport_KpiPeriodReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiPeriodReport_KpiPeriodReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select ap.Id;
            return await query.Distinct().CountAsync();
        }

        [Route(KpiPeriodReportRoute.List), HttpPost]
        public async Task<List<KpiPeriodReport_KpiPeriodReportDTO>> List([FromBody] KpiPeriodReport_KpiPeriodReportFilterDTO KpiPeriodReport_KpiPeriodReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            OrganizationDAO OrganizationDAO = null;
            long? SaleEmployeeId = KpiPeriodReport_KpiPeriodReportFilterDTO.SaleEmployeeId?.Equal;
            long? KpiPeriodId = KpiPeriodReport_KpiPeriodReportFilterDTO.KpiPeriodId?.Equal;
            long? KpiYearId = KpiPeriodReport_KpiPeriodReportFilterDTO.KpiYearId?.Equal;
            if (KpiPeriodReport_KpiPeriodReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiPeriodReport_KpiPeriodReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }

            // list toan bo nhan vien sau khi chay qua filter
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select new KpiPeriodReport_SaleEmployeeDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                        };
            List<KpiPeriodReport_SaleEmployeeDTO> SaleEmployeeDTOs = await query.Distinct().ToListAsync();

            List<long> SaleEmployeeIds = SaleEmployeeDTOs.Select(x => x.SaleEmployeeId).ToList();
            var query_detail = from a in DataContext.KpiGeneralContentKpiPeriodMapping
                               join b in DataContext.KpiGeneralContent on a.KpiGeneralContentId equals b.Id
                               join c in DataContext.KpiGeneral on b.KpiGeneralId equals c.Id
                               where (SaleEmployeeIds.Contains(c.EmployeeId) &&
                                      c.KpiYearId == KpiYearId &&
                                      a.KpiPeriodId == KpiPeriodId)
                               select new KpiPeriodReport_SaleEmployeeDetailDTO
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaGeneralId = b.KpiCriteriaGeneralId,
                                   Value = a.Value.Value,
                               };
            List<KpiPeriodReport_SaleEmployeeDetailDTO> SaleEmployeeDetailDTOs = await query_detail.Distinct().ToListAsync();

            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId))
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
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId))
                .Select(x => new StoreCheckingDAO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt
                })
                .ToListAsync();

            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => SaleEmployeeIds.Contains(x.CreatorId))
                .Select(x => new StoreScoutingDAO
                {
                    CreatorId = x.CreatorId,
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
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
                SaleEmployeeDTO.NewStoreCreated = StoreCheckingDAOs
                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                    .Count();
                SaleEmployeeDTO.NewStoreCreatedRatio = Math.Round(SaleEmployeeDTO.NewStoreCreated / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);
            };

            List<KpiPeriodReport_KpiPeriodReportDTO> KpiPeriodReport_KpiPeriodReportDTOs = new List<KpiPeriodReport_KpiPeriodReportDTO>();

            #region delete
            //List < KpiPeriodReport_SaleEmployeeDTO > KpiPeriodReport_SaleEmployeeDTOs = await query.Distinct().OrderBy(q => q.DisplayName)
            //    .Skip(KpiPeriodReport_KpiPeriodReportFilterDTO.Skip)
            //    .Take(KpiPeriodReport_KpiPeriodReportFilterDTO.Take)
            //    .ToListAsync(); // to do

            //List<KpiGeneralDAO> KpiGeneralDAOs = await DataContext.KpiGeneral.ToListAsync();
            //List<KpiGeneralContentDAO> KpiGeneralContentDAOs = await DataContext.KpiGeneralContent.ToListAsync();
            //List<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappingDAOs = await DataContext.KpiGeneralContentKpiPeriodMapping.ToListAsync();
            //foreach (var KpiPeriodReport_SaleEmployeeDTO in KpiPeriodReport_SaleEmployeeDTOs)
            //{
            //    // get KPI General
            //    var KpiGeneralDTO = KpiGeneralDAOs.Where(x => x.EmployeeId == KpiPeriodReport_SaleEmployeeDTO.SaleEmployeeId && x.KpiYearId == KpiYearId).FirstOrDefault();
            //    //Get KPI General Content (6 for each employee)
            //    var KpiGeneralContentDTOS = KpiGeneralContentDAOs.Where(x => x.KpiGeneralId == KpiGeneralDTO.Id);
            //    //get value for each employee
            //    foreach (var KpiGeneralContentDTO in KpiGeneralContentDTOS)
            //    {
            //        var KpiGeneralContentKpiPeriodMappingDTO = KpiGeneralContentKpiPeriodMappingDAOs.Where(x => x.KpiGeneralContentId == KpiGeneralContentDTO.Id && x.KpiPeriodId == KpiPeriodId).FirstOrDefault();

            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTORDERS.Id)
            //            KpiPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersPLanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTOUTPUT.Id)
            //            KpiPeriodReport_SaleEmployeeDTO.TotalIndirectOutputPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTSALESAMOUNT.Id)
            //            KpiPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.SKUINDIRECTORDER.Id)
            //            KpiPeriodReport_SaleEmployeeDTO.SkuIndirectOrderPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.STORESVISITED.Id)
            //            KpiPeriodReport_SaleEmployeeDTO.StoresVisitedPLanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //        if (KpiGeneralContentDTO.KpiCriteriaGeneralId == GeneralCriteriaEnum.NEWSTORECREATED.Id)
            //            KpiPeriodReport_SaleEmployeeDTO.NewStoreCreatedPlanned = KpiGeneralContentKpiPeriodMappingDTO.Value.Value;
            //    }
            //}
            #endregion



            return null;
        }

        //private Tuple<DateTime, DateTime> DateTimeConvert(long KpiPeriod, long KpiYear)
        //{
        //    DateTime startDate, endDate;
        //    Tuple<DateTime, DateTime> result = new Tuple<DateTime, DateTime>();
        //    return Tuple.Create(startDate, endDate);
        //}
    }
}
