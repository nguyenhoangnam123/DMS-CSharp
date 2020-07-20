using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
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

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IKpiPeriodService KpiPeriodService;
        private IItemService ItemService;
        private ICurrentContext CurrentContext;
        public KpiItemReportController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IKpiPeriodService KpiPeriodService,
            IItemService ItemService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.ItemService = ItemService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiItemReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiItemReport_AppUserDTO>> FilterListAppUser([FromBody] KpiItemReport_AppUserFilterDTO KpiItemReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiItemReport_AppUserFilterDTO.Id;
            AppUserFilter.OrganizationId = KpiItemReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiItemReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItemReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItemReport_AppUserDTO> KpiItemReport_AppUserDTOs = AppUsers
                .Select(x => new KpiItemReport_AppUserDTO(x)).ToList();
            return KpiItemReport_AppUserDTOs;
        }

        [Route(KpiItemReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiItemReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiItemReport_OrganizationFilterDTO KpiItemReport_OrganizationFilterDTO)
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
            List<KpiItemReport_OrganizationDTO> KpiItemReport_OrganizationDTOs = Organizations
                .Select(x => new KpiItemReport_OrganizationDTO(x)).ToList();
            return KpiItemReport_OrganizationDTOs;
        }

        [Route(KpiItemReportRoute.FilterListItem), HttpPost]
        public async Task<List<KpiItemReport_ItemDTO>> FilterListItem([FromBody] KpiItemReport_ItemFilterDTO KpiItemReport_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = int.MaxValue;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItemReport_ItemDTO> KpiItemReport_ItemDTOs = Items
                .Select(x => new KpiItemReport_ItemDTO(x)).ToList();
            return KpiItemReport_ItemDTOs;
        }

        [Route(KpiItemReportRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiItemReport_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiItemReport_KpiPeriodFilterDTO KpiItemReport_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiItemReport_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiItemReport_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiItemReport_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiItemReport_KpiPeriodDTO> KpiItemReport_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiItemReport_KpiPeriodDTO(x)).ToList();
            return KpiItemReport_KpiPeriodDTOs;
        }

        [Route(KpiItemReportRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiItemReport_KpiYearDTO>> FilterListKpiYear([FromBody] KpiItemReport_KpiYearFilterDTO KpiItemReport_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiItemReport_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiItemReport_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiItemReport_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiItemReport_KpiYearDTO> KpiItemReport_KpiYearDTOs = KpiYears
                .Select(x => new KpiItemReport_KpiYearDTO(x)).ToList();
            return KpiItemReport_KpiYearDTOs;
        }

        [Route(KpiItemReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiItemReport_KpiItemReportFilterDTO KpiItemReport_KpiItemReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState); // to do kpi year and period
            OrganizationDAO OrganizationDAO = null;
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiItemReport_KpiItemReportFilterDTO.AppUserId?.Equal;
            long? ItemId = KpiItemReport_KpiItemReportFilterDTO.ItemId?.Equal;
            long KpiPeriodId = KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
            long KpiYearId = KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            if (KpiItemReport_KpiItemReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiItemReport_KpiItemReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }

            // list toan bo nhan vien sau khi chay qua filter
            var query = from ki in DataContext.KpiItem
                        join ap in DataContext.AppUser on ki.EmployeeId equals ap.Id
                        join o in DataContext.Organization on ki.OrganizationId equals o.Id
                        join kic in DataContext.KpiItemContent on ki.Id equals kic.KpiItemId
                        join i in DataContext.Item on kic.ItemId equals i.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path))
                        && (SaleEmployeeId == null || ki.Id == SaleEmployeeId.Value)
                        && (ItemId == null || i.Id == ItemId.Value)
                        && (ki.KpiYearId == KpiYearId)
                        select new KpiItemReport_SaleEmployeeItemDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                            OrganizationId = ap.OrganizationId.Value,
                            ItemId = i.Id,
                            ItemName = i.Name,
                        };
            return await query.Distinct().CountAsync();
        }

        [Route(KpiItemReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiItemReport_KpiItemReportDTO>>> List([FromBody] KpiItemReport_KpiItemReportFilterDTO KpiItemReport_KpiItemReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId == null) return BadRequest("Chưa chọn kì KPI");
            if (KpiItemReport_KpiItemReportFilterDTO.KpiYearId == null) return BadRequest("Chưa chọn năm KPI");

            OrganizationDAO OrganizationDAO = null;
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiItemReport_KpiItemReportFilterDTO.AppUserId?.Equal;
            long? ItemId = KpiItemReport_KpiItemReportFilterDTO.ItemId?.Equal;
            long KpiPeriodId = KpiItemReport_KpiItemReportFilterDTO.KpiPeriodId?.Equal ?? KpiPeriodEnum.PERIOD_MONTH01.Id;
            long KpiYearId = KpiItemReport_KpiItemReportFilterDTO.KpiYearId?.Equal ?? KpiYearEnum.YEAR_2020.Id;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);

            if (KpiItemReport_KpiItemReportFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == KpiItemReport_KpiItemReportFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }

            // list toan bo nhan vien sau khi chay qua filter
            var query = from ki in DataContext.KpiItem
                        join ap in DataContext.AppUser on ki.EmployeeId equals ap.Id
                        join o in DataContext.Organization on ki.OrganizationId equals o.Id
                        join kic in DataContext.KpiItemContent on ki.Id equals kic.KpiItemId
                        join i in DataContext.Item on kic.ItemId equals i.Id
                        where (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path))
                        && (SaleEmployeeId == null || ki.Id == SaleEmployeeId.Value)
                        && (ItemId == null || i.Id == ItemId.Value)
                        && (ki.KpiYearId == KpiYearId)
                        select new KpiItemReport_SaleEmployeeItemDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                            OrganizationId = ap.OrganizationId.Value,
                            ItemId = i.Id,
                            ItemName = i.Name,
                        };

            List<KpiItemReport_SaleEmployeeItemDTO> KpiItemReport_SaleEmployeeItemDTOs = await query.Distinct()
                .OrderBy(q => q.DisplayName)
                .Skip(KpiItemReport_KpiItemReportFilterDTO.Skip)
                .Take(KpiItemReport_KpiItemReportFilterDTO.Take)
                .ToListAsync();

            //get organization
            List<long> OrganizationIds = KpiItemReport_SaleEmployeeItemDTOs.Select(x => x.OrganizationId).Distinct().ToList(); // to do
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Name,
                Id = new IdFilter { In = OrganizationIds }
            });

            List<long> SaleEmployeeIds = KpiItemReport_SaleEmployeeItemDTOs.Select(x => x.SaleEmployeeId).Distinct().ToList();


            List<KpiItemReport_SaleEmployeeDTO> KpiItemReport_SaleEmployeeDTOs = new List<KpiItemReport_SaleEmployeeDTO>();
            foreach (var EmployeeId in SaleEmployeeIds)
            {
                KpiItemReport_SaleEmployeeDTO KpiItemReport_SaleEmployeeDTO = new KpiItemReport_SaleEmployeeDTO()
                {
                    SaleEmployeeId = EmployeeId,
                    DisplayName = KpiItemReport_SaleEmployeeItemDTOs.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => x.DisplayName).FirstOrDefault(),
                    OrganizationName = KpiItemReport_SaleEmployeeItemDTOs.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => x.OrganizationName).FirstOrDefault(),
                    OrganizationId = KpiItemReport_SaleEmployeeItemDTOs.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => x.OrganizationId).FirstOrDefault(),
                    Username = KpiItemReport_SaleEmployeeItemDTOs.Where(x => x.SaleEmployeeId == EmployeeId).Select(x => x.Username).FirstOrDefault(),
                    KpiItemReport_SaleEmployeeItemDTOs = KpiItemReport_SaleEmployeeItemDTOs.Select(x => new KpiItemReport_SaleEmployeeItemDTO
                    {
                        ItemId = x.ItemId,
                        SaleEmployeeId = EmployeeId,
                        ItemName = x.ItemName,
                        ItemCode = x.ItemCode,
                        DisplayName = x.DisplayName,
                        Username = x.Username,
                        OrganizationName = x.OrganizationName,
                        OrganizationId = x.OrganizationId
                    })
                    .Where(x => x.SaleEmployeeId == EmployeeId)
                    .ToList()
                };
                KpiItemReport_SaleEmployeeDTOs.Add(KpiItemReport_SaleEmployeeDTO);
            }

            // lay du lieu bang mapping
            var query_detail = from a in DataContext.KpiItemContentKpiCriteriaItemMapping
                               join b in DataContext.KpiItemContent on a.KpiItemContentId equals b.Id
                               join c in DataContext.KpiItem on b.KpiItemId equals c.Id
                               join d in DataContext.Item on b.ItemId equals d.Id
                               where (SaleEmployeeIds.Contains(c.EmployeeId) &&
                                      c.KpiYearId == KpiYearId &&
                                      c.KpiPeriodId == KpiPeriodId &&
                                      (ItemId == null || d.Id == ItemId))
                               select new KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTO
                               {
                                   SaleEmployeeId = c.EmployeeId,
                                   KpiCriteriaItemId = a.KpiCriteriaItemId,
                                   Value = a.Value,
                                   ItemId = d.Id,
                               };

            List<KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTO> KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs = await query_detail.Distinct().ToListAsync();

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
                        Quantity = c.Quantity,
                        ItemId = c.ItemId,
                    }).ToList(),
                })
                .ToListAsync(); 

            foreach (var SaleEmployeeDTO in KpiItemReport_SaleEmployeeDTOs)
            {
                foreach (var SaleEmployeeItemDTO in SaleEmployeeDTO.KpiItemReport_SaleEmployeeItemDTOs)
                {
                    // INDIRECT_ORDERS_OF_KEY_ITEM - Sản lượng theo đơn gián tiếp
                    SaleEmployeeItemDTO.IndirectOutputOfkeyItemPlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(sed => sed.SaleEmployeeId == SaleEmployeeItemDTO.SaleEmployeeId && sed.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_ORDERS_OF_KEY_ITEM.Id)
                            .Select(sed => sed.Value).FirstOrDefault();
                    SaleEmployeeItemDTO.IndirectOutputOfKeyItem = IndirectSalesOrderDAOs
                        .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                        .SelectMany(iso => iso.IndirectSalesOrderContents)
                        .Where(z => z.ItemId == ItemId)
                        .Select(z => z.Quantity)
                        .DefaultIfEmpty(0).Sum();
                    SaleEmployeeItemDTO.IndirectOutputOfkeyItemRatio = SaleEmployeeItemDTO.IndirectOutputOfkeyItemPlanned == 0 ? 0 : Math.Round(SaleEmployeeItemDTO.IndirectOutputOfKeyItem / SaleEmployeeItemDTO.IndirectOutputOfkeyItemPlanned, 2);

                    // INDIRECT_SALES_OF_KEY_ITEM - Doanh số theo đơn gián tiếp
                    SaleEmployeeItemDTO.IndirectSalesOfKeyItemPlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(sed => sed.SaleEmployeeId == SaleEmployeeItemDTO.SaleEmployeeId && sed.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_SALES_OF_KEY_ITEM.Id)
                            .Select(sed => sed.Value).FirstOrDefault();

                    List<long> IndirectSalesOrderIds = IndirectSalesOrderDAOs
                        .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                        .SelectMany(iso => iso.IndirectSalesOrderContents)
                        .Where(z => z.ItemId == ItemId)
                        .Select(z => z.IndirectSalesOrderId)
                        .ToList();  // list Id
                    SaleEmployeeItemDTO.IndirectSalesOfKeyItem = IndirectSalesOrderDAOs
                        .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && IndirectSalesOrderIds.Contains(iso.Id))
                        .Select(iso => iso.Total)
                        .DefaultIfEmpty(0).Sum();
                    SaleEmployeeItemDTO.IndirectSalesOfKeyItemRatio = SaleEmployeeItemDTO.IndirectSalesOfKeyItemPlanned == 0 ? 0 : Math.Round(SaleEmployeeItemDTO.IndirectSalesOfKeyItem / SaleEmployeeItemDTO.IndirectSalesOfKeyItemPlanned, 2);

                    // INDIRECT_ORDERS_OF_KEY_ITEM - Đơn hàng gián tiếp
                    SaleEmployeeItemDTO.IndirectOrdersOfKeyItemPlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(sed => sed.SaleEmployeeId == SaleEmployeeItemDTO.SaleEmployeeId && sed.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_ORDERS_OF_KEY_ITEM.Id)
                            .Select(sed => sed.Value).FirstOrDefault();
                    SaleEmployeeItemDTO.IndirectOrdersOfKeyItem = IndirectSalesOrderDAOs
                        .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId)
                        .SelectMany(iso => iso.IndirectSalesOrderContents)
                        .Where(z => z.ItemId == ItemId)
                        .Count();
                    SaleEmployeeItemDTO.IndirectOrdersOfKeyItemRatio = SaleEmployeeItemDTO.IndirectOrdersOfKeyItemPlanned == 0 ? 0 : Math.Round(SaleEmployeeItemDTO.IndirectOrdersOfKeyItem / SaleEmployeeItemDTO.IndirectOrdersOfKeyItemPlanned, 2);

                    // INDIRECT_STORES_OF_KEY_ITEM - khách hàng theo đơn gián tiếp
                    SaleEmployeeItemDTO.IndirectStoresOfKeyItemPlanned = KpiItemReport_KpiItemContentKpiCriteriaItemMappingDTOs
                            .Where(sed => sed.SaleEmployeeId == SaleEmployeeItemDTO.SaleEmployeeId && sed.KpiCriteriaItemId == KpiCriteriaItemEnum.INDIRECT_STORES_OF_KEY_ITEM.Id)
                            .Select(sed => sed.Value).FirstOrDefault();
                    SaleEmployeeItemDTO.IndirectStoresOfKeyItem = IndirectSalesOrderDAOs
                        .Where(iso => iso.SaleEmployeeId == SaleEmployeeDTO.SaleEmployeeId && IndirectSalesOrderIds.Contains(iso.Id))
                        .Count();
                    SaleEmployeeItemDTO.IndirectStoresOfKeyItem = SaleEmployeeItemDTO.IndirectStoresOfKeyItemPlanned == 0 ? 0 : Math.Round(SaleEmployeeItemDTO.IndirectStoresOfKeyItem / SaleEmployeeItemDTO.IndirectStoresOfKeyItemPlanned, 2);

                }
            };

            List<KpiItemReport_KpiItemReportDTO> KpiItemReport_KpiItemReportDTOs = new List<KpiItemReport_KpiItemReportDTO>();
            foreach (Organization Organization in Organizations)
            {
                KpiItemReport_KpiItemReportDTO KpiItemReport_KpiItemReportDTO = new KpiItemReport_KpiItemReportDTO()
                {
                    OrganizationName = Organization.Name,
                    SaleEmployees = KpiItemReport_SaleEmployeeDTOs.Where(x => x.OrganizationName.Equals(Organization.Name)).ToList()
                };
                if (KpiItemReport_KpiItemReportDTO.SaleEmployees.Count() > 0) KpiItemReport_KpiItemReportDTOs.Add(KpiItemReport_KpiItemReportDTO);
            }
            return KpiItemReport_KpiItemReportDTOs;
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
