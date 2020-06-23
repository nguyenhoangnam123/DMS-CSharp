//using Common;
//using DMS.Entities;
//using DMS.Enums;
//using DMS.Models;
//using DMS.Services.MAppUser;
//using DMS.Services.MOrganization;
//using Hangfire.Annotations;
//using Helpers;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.EntityFrameworkCore;
//using RestSharp.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DMS.Rpc.kpi_tracking.kpi_item_report
//{
//    public class KpiItemReportController : RpcController
//    {
//        private DataContext DataContext;
//        private IOrganizationService OrganizationService;
//        private IAppUserService AppUserService;
//        public KpiItemReportController(DataContext DataContext, IOrganizationService OrganizationService, IAppUserService AppUserService)
//        {
//            this.DataContext = DataContext;
//            this.OrganizationService = OrganizationService;
//            this.AppUserService = AppUserService;
//        }

//        [Route(KpiItemReportRoute.FilterListAppUser), HttpPost]
//        public async Task<List<KpiItemReport_AppUserDTO>> FilterListAppUser([FromBody] KpiItemReport_AppUserFilterDTO KpiItemReport_AppUserFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            AppUserFilter AppUserFilter = new AppUserFilter();
//            AppUserFilter.Skip = 0;
//            AppUserFilter.Take = 20;
//            AppUserFilter.OrderBy = AppUserOrder.Id;
//            AppUserFilter.OrderType = OrderType.ASC;
//            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
//            AppUserFilter.Id = KpiItemReport_AppUserFilterDTO.Id;
//            AppUserFilter.OrganizationId = KpiItemReport_AppUserFilterDTO.OrganizationId;
//            AppUserFilter.Username = KpiItemReport_AppUserFilterDTO.Username;
//            AppUserFilter.DisplayName = KpiItemReport_AppUserFilterDTO.DisplayName;
//            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

//            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
//            List<KpiItemReport_AppUserDTO> KpiItemReport_AppUserDTOs = AppUsers
//                .Select(x => new KpiItemReport_AppUserDTO(x)).ToList();
//            return KpiItemReport_AppUserDTOs;
//        }

//        [Route(KpiItemReportRoute.FilterListOrganization), HttpPost]
//        public async Task<List<KpiItemReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiItemReport_OrganizationFilterDTO KpiItemReport_OrganizationFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            OrganizationFilter OrganizationFilter = new OrganizationFilter();
//            OrganizationFilter.Skip = 0;
//            OrganizationFilter.Take = int.MaxValue;
//            OrganizationFilter.OrderBy = OrganizationOrder.Id;
//            OrganizationFilter.OrderType = OrderType.ASC;
//            OrganizationFilter.Selects = OrganizationSelect.ALL;
//            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

//            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
//            List<KpiItemReport_OrganizationDTO> KpiItemReport_OrganizationDTOs = Organizations
//                .Select(x => new KpiItemReport_OrganizationDTO(x)).ToList();
//            return KpiItemReport_OrganizationDTOs;
//        }

//        [Route(KpiItemReportRoute.FilterListItem), HttpPost] // to do
//        public async Task<List<KpiItemReport_OrganizationDTO>> FilterListItem([FromBody] KpiItemReport_ItemFilterDTO KpiItemReport_ItemFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            ItemFilter ItemFilter = new ItemFilter();
//            ItemFilter.Skip = 0;
//            ItemFilter.Take = int.MaxValue;
//            ItemFilter.OrderBy = ItemOrder.Id;
//            ItemFilter.OrderType = OrderType.ASC;
//            ItemFilter.Selects = ItemSelect.ALL;
//            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

//            List<Organization> Organizations = await ItemService.List(ItemFilter);
//            List<KpiItemReport_OrganizationDTO> KpiItemReport_OrganizationDTOs = Organizations
//                .Select(x => new KpiItemReport_OrganizationDTO(x)).ToList();
//            return KpiItemReport_OrganizationDTOs;
//        }

//        [Route(KpiItemReportRoute.FilterListKpiPeriod), HttpPost]
//        public List<GenericEnum> FilterListKpiPeriod()
//        {
//            return KpiPeriodEnum.KpiPeriodEnumList;
//        }

//        [Route(KpiItemReportRoute.FilterListKpiYear), HttpPost]
//        public List<GenericEnum> FilterListKpiYear()
//        {
//            return KpiYearEnum.KpiYearEnumList;
//        }

//        [Route(KpiItemReportRoute.Count), HttpPost]
//        public async Task<int> Count([FromBody] KpiItemReport_KpiItemReportFilterDTO KpiItemReport_KpiItemReportFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState); // to do kpi year and period
//            OrganizationDAO OrganizationDAO = null;
//            long? SaleEmployeeId = KpiItemReport_KpiItemReportFilterDTO.SaleEmployeeId?.Equal;
//            if (KpiItemReport_KpiItemReportFilterDTO.OrganizationId?.Equal != null)
//            {
//                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiItemReport_KpiItemReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
//            }
//            var query = from ap in DataContext.AppUser
//                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
//                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
//                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
//                        select ap.Id;
//            return await query.Distinct().CountAsync();
//        }

//        [Route(KpiItemReportRoute.List), HttpPost]
//        public async Task<ActionResult<List<KpiItemReport_KpiItemReportDTO>>> List([FromBody] KpiItemReport_KpiItemReportFilterDTO KpiItemReport_KpiItemReportFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);
//            if (KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId == null) return BadRequest("Chưa chọn kì KPI");
//            if (KpiItemReport_KpiItemReportFilterDTO.KpiYearId == null) return BadRequest("Chưa chọn năm KPI");

//            OrganizationDAO OrganizationDAO = null;
//            DateTime StartDate, EndDate;
//            long? SaleEmployeeId = KpiItemReport_KpiItemReportFilterDTO.SaleEmployeeId?.Equal;
//            long? ItemId = KpiItemReport_KpiItemReportFilterDTO.ItemId?.Equal;
//            long KpiPeriodId = KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
//            long KpiYearId = KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;
//            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

//            if (KpiItemReport_KpiItemReportFilterDTO.OrganizationId?.Equal != null)
//            {
//                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiItemReport_KpiItemReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
//            }

//            // list toan bo nhan vien sau khi chay qua filter
//            var query = from ap in DataContext.AppUser
//                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
//                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
//                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
//                        select new KpiItemReport_SaleEmployeeDTO
//                        {
//                            SaleEmployeeId = ap.Id,
//                            Username = ap.Username,
//                            DisplayName = ap.DisplayName,
//                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
//                            OrganizationId = ap.OrganizationId.Value
//                        };
//            List<KpiItemReport_SaleEmployeeDTO> SaleEmployeeDTOs = await query.Distinct()
//                .OrderBy(q => q.DisplayName)
//                .Skip(KpiItemReport_KpiItemReportFilterDTO.Skip)
//                .Take(KpiItemReport_KpiItemReportFilterDTO.Take)
//                .ToListAsync();

//            //get organization
//            List<long> OrganizationIds = SaleEmployeeDTOs.Where(x => x.OrganizationId.HasValue).Select(x => x.OrganizationId.Value).ToList();
//            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
//            {
//                Skip = 0,
//                Take = int.MaxValue,
//                Selects = OrganizationSelect.Id | OrganizationSelect.Name,
//                Id = new IdFilter { In = OrganizationIds }
//            });

//            List<long> SaleEmployeeIds = SaleEmployeeDTOs.Select(x => x.SaleEmployeeId).ToList();
//            var query_detail = from a in DataContext.KpiItemContentKpiCriteriaItemMapping
//                               join b in DataContext.KpiItemContent on a.KpiItemContentId equals b.Id
//                               join c in DataContext.KpiItem on b.KpiItemId equals c.Id
//                               join d in DataContext.Item on b.ItemId equals d.Id
//                               where (SaleEmployeeIds.Contains(c.EmployeeId) &&
//                                      c.KpiYearId == KpiYearId &&
//                                      c.KpiPeriodId == KpiPeriodId &&
//                                      (ItemId == null || d.Id == ItemId)) // to do
//                               select new KpiItemReport_SaleEmployeeItemDTO
//                               {
//                                   SaleEmployeeId = c.EmployeeId,
//                                   KpiCriteriaItemId = a.KpiCriteriaItemId,
//                                   Value = a.Value,
//                                   ItemId = c.Id,
//                               };
//            List<KpiItemReport_SaleEmployeeItemDTO> KpiItemReport_ItemDTOs = await query_detail.Distinct().ToListAsync();





//            foreach (var SaleEmployeeDTO in SaleEmployeeDTOs)
//            {
//                // TOTALINDIRECTORDERS
//                SaleEmployeeDTO.TotalIndirectOrdersPLanned = SaleEmployeeDetailDTOs
//                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTORDERS.Id)
//                        .Select(sed => sed.Value).FirstOrDefault();
//                SaleEmployeeDTO.TotalIndirectOrders = IndirectSalesOrderDAOs
//                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
//                    .Count();
//                SaleEmployeeDTO.TotalIndirectOrdersRatio = SaleEmployeeDTO.TotalIndirectOrdersPLanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectOrders / SaleEmployeeDTO.TotalIndirectOrdersPLanned, 2);


//                // TOTALINDIRECTOUTPUT
//                SaleEmployeeDTO.TotalIndirectOutputPlanned = SaleEmployeeDetailDTOs
//                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTOUTPUT.Id)
//                        .Select(sed => sed.Value).FirstOrDefault();
//                SaleEmployeeDTO.TotalIndirectOutput = IndirectSalesOrderDAOs
//                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
//                    .SelectMany(iso => iso.IndirectSalesOrderContents)
//                    .Select(z => z.Quantity)
//                    .DefaultIfEmpty(0).Sum();
//                SaleEmployeeDTO.TotalIndirectOutputRatio = SaleEmployeeDTO.TotalIndirectOutputPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOutputPlanned, 2);

//                // TOTALINDIRECTSALESAMOUNT
//                SaleEmployeeDTO.TotalIndirectSalesAmountPlanned = SaleEmployeeDetailDTOs
//                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.TOTALINDIRECTSALESAMOUNT.Id)
//                        .Select(sed => sed.Value).FirstOrDefault();
//                SaleEmployeeDTO.TotalIndirectSalesAmount = IndirectSalesOrderDAOs
//                    .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
//                    .Sum(iso => iso.Total);
//                SaleEmployeeDTO.TotalIndirectSalesAmountRatio = SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.TotalIndirectSalesAmount / SaleEmployeeDTO.TotalIndirectSalesAmountPlanned, 2);

//                // SKUINDIRECTORDER
//                SaleEmployeeDTO.SkuIndirectOrderPlanned = SaleEmployeeDetailDTOs
//                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.SKUINDIRECTORDER.Id)
//                        .Select(sed => sed.Value).FirstOrDefault();
//                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.TotalIndirectOrders == 0 ? 0 : SaleEmployeeDTO.TotalIndirectOutput / SaleEmployeeDTO.TotalIndirectOrders;
//                SaleEmployeeDTO.SkuIndirectOrder = SaleEmployeeDTO.SkuIndirectOrderPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.SkuIndirectOrder / SaleEmployeeDTO.SkuIndirectOrderPlanned, 2);

//                // STORESVISITED
//                SaleEmployeeDTO.StoresVisitedPLanned = SaleEmployeeDetailDTOs
//                       .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.STORESVISITED.Id)
//                       .Select(sed => sed.Value).FirstOrDefault();
//                SaleEmployeeDTO.StoresVisited = StoreCheckingDAOs
//                    .Where(sc => sc.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
//                    .Count();
//                SaleEmployeeDTO.StoresVisitedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.StoresVisited / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);

//                // NEWSTORECREATED
//                SaleEmployeeDTO.NewStoreCreatedPlanned = SaleEmployeeDetailDTOs
//                        .Where(sed => sed.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && sed.KpiCriteriaGeneralId == GeneralCriteriaEnum.NEWSTORECREATED.Id)
//                        .Select(sed => sed.Value).FirstOrDefault();
//                SaleEmployeeDTO.NewStoreCreated = StoreScoutingDAOs
//                    .Where(sc => sc.CreatorId == SaleEmployeeDTO.SaleEmployeeId)
//                    .SelectMany(sc => sc.Stores)
//                    .Select(z => z.StoreScoutingId.HasValue)
//                    .Count();
//                SaleEmployeeDTO.NewStoreCreatedRatio = SaleEmployeeDTO.NewStoreCreatedPlanned == 0 ? 0 : Math.Round(SaleEmployeeDTO.NewStoreCreated / SaleEmployeeDTO.NewStoreCreatedPlanned, 2);
//            };

//            List<KpiItemReport_KpiItemReportDTO> KpiItemReport_KpiItemReportDTOs = new List<KpiItemReport_KpiItemReportDTO>();
//            foreach (Organization Organization in Organizations)
//            {
//                KpiItemReport_KpiItemReportDTO KpiItemReport_KpiItemReportDTO = new KpiItemReport_KpiItemReportDTO()
//                {
//                    OrganizationName = Organization.Name,
//                    SaleEmployees = SaleEmployeeDTOs.Where(x => x.OrganizationName.Equals(Organization.Name)).ToList()
//                };
//                if (KpiItemReport_KpiItemReportDTO.SaleEmployees.Count() > 0) KpiItemReport_KpiItemReportDTOs.Add(KpiItemReport_KpiItemReportDTO);
//            }
//            return KpiItemReport_KpiItemReportDTOs;
//        }

//        private Tuple<DateTime, DateTime> DateTimeConvert(long KpiPeriodId, long KpiYearId)
//        {
//            DateTime startDate = StaticParams.DateTimeNow;
//            DateTime endDate = StaticParams.DateTimeNow;
//            if (KpiPeriodId <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
//            {
//                startDate = new DateTime((int)KpiYearId, (int)(KpiPeriodId % 100), 1);
//                endDate = startDate.AddMonths(1).AddSeconds(-1);
//            }
//            else
//            {
//                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER01.Id)
//                {
//                    startDate = new DateTime((int)KpiYearId, 1, 1);
//                    endDate = startDate.AddMonths(3).AddSeconds(-1);
//                }
//                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER02.Id)
//                {
//                    startDate = new DateTime((int)KpiYearId, 4, 1);
//                    endDate = startDate.AddMonths(3).AddSeconds(-1);
//                }
//                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER03.Id)
//                {
//                    startDate = new DateTime((int)KpiYearId, 7, 1);
//                    endDate = startDate.AddMonths(3).AddSeconds(-1);
//                }
//                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER04.Id)
//                {
//                    startDate = new DateTime((int)KpiYearId, 10, 1);
//                    endDate = startDate.AddMonths(3).AddSeconds(-1);
//                }
//                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_YEAR01.Id)
//                {
//                    startDate = new DateTime((int)KpiYearId, 1, 1);
//                    endDate = startDate.AddYears(1).AddSeconds(-1);
//                }
//            }

//            return Tuple.Create(startDate, endDate);
//        }
//    }
//}
