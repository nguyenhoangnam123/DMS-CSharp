using DMS.Common;
using DMS.Models;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MKpiProductGroupingType;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DMS.Enums;
using DMS.Services.MProductGrouping;
using DMS.Helpers;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace DMS.Rpc.kpi_tracking.kpi_product_grouping_report
{
    public class KpiProductGroupingReportController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IKpiPeriodService KpiPeriodService;
        private IItemService ItemService;
        private IKpiProductGroupingTypeService KpiProductGroupingTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public KpiProductGroupingReportController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IKpiPeriodService KpiPeriodService,
            IItemService ItemService,
            IKpiProductGroupingTypeService KpiProductGroupingTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.ItemService = ItemService;
            this.KpiProductGroupingTypeService = KpiProductGroupingTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(KpiProductGroupingReportRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiProductGroupingReport_AppUserDTO>> FilterListAppUser([FromBody] KpiProductGroupingReport_AppUserFilterDTO KpiProductGroupingReport_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = KpiProductGroupingReport_AppUserFilterDTO.Id;
            AppUserFilter.OrganizationId = KpiProductGroupingReport_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = KpiProductGroupingReport_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiProductGroupingReport_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiProductGroupingReport_AppUserDTO> KpiProductGroupingReport_AppUserDTOs = AppUsers
                .Select(x => new KpiProductGroupingReport_AppUserDTO(x)).ToList();
            return KpiProductGroupingReport_AppUserDTOs;
        }

        [Route(KpiProductGroupingReportRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiProductGroupingReport_OrganizationDTO>> FilterListOrganization([FromBody] KpiProductGroupingReport_OrganizationFilterDTO KpiProductGroupingReport_OrganizationFilterDTO)
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
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);


            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiProductGroupingReport_OrganizationDTO> KpiProductGroupingReport_OrganizationDTOs = Organizations
                .Select(x => new KpiProductGroupingReport_OrganizationDTO(x)).ToList();
            return KpiProductGroupingReport_OrganizationDTOs;
        }

        [Route(KpiProductGroupingReportRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiProductGroupingReport_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiProductGroupingReport_KpiPeriodFilterDTO KpiProductGroupingReport_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiProductGroupingReport_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiProductGroupingReport_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiProductGroupingReport_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiProductGroupingReport_KpiPeriodDTO> KpiProductGroupingReport_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiProductGroupingReport_KpiPeriodDTO(x)).ToList();
            return KpiProductGroupingReport_KpiPeriodDTOs;
        }

        [Route(KpiProductGroupingReportRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiProductGroupingReport_KpiYearDTO>> FilterListKpiYear([FromBody] KpiProductGroupingReport_KpiYearFilterDTO KpiProductGroupingReport_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiProductGroupingReport_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiProductGroupingReport_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiProductGroupingReport_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiProductGroupingReport_KpiYearDTO> KpiProductGroupingReport_KpiYearDTOs = KpiYears
                .Select(x => new KpiProductGroupingReport_KpiYearDTO(x)).ToList();
            return KpiProductGroupingReport_KpiYearDTOs;
        }

        [Route(KpiProductGroupingReportRoute.FilterListKpiProductGroupingType), HttpPost]
        public async Task<List<KpiProductGroupingReport_KpiProductGroupingTypeDTO>> FilterListKpiProductGroupingType([FromBody] KpiProductGroupingReport_KpiProductGroupingTypeFilterDTO KpiProductGroupingReport_KpiProductGroupingTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingTypeFilter KpiProductGroupingTypeFilter = new KpiProductGroupingTypeFilter();
            KpiProductGroupingTypeFilter.Skip = 0;
            KpiProductGroupingTypeFilter.Take = 20;
            KpiProductGroupingTypeFilter.OrderBy = KpiProductGroupingTypeOrder.Id;
            KpiProductGroupingTypeFilter.OrderType = OrderType.ASC;
            KpiProductGroupingTypeFilter.Selects = KpiProductGroupingTypeSelect.ALL;
            KpiProductGroupingTypeFilter.Id = KpiProductGroupingReport_KpiProductGroupingTypeFilterDTO.Id;
            KpiProductGroupingTypeFilter.Code = KpiProductGroupingReport_KpiProductGroupingTypeFilterDTO.Code;
            KpiProductGroupingTypeFilter.Name = KpiProductGroupingReport_KpiProductGroupingTypeFilterDTO.Name;

            List<KpiProductGroupingType> KpiProductGroupingTypes = await KpiProductGroupingTypeService.List(KpiProductGroupingTypeFilter);
            List<KpiProductGroupingReport_KpiProductGroupingTypeDTO> KpiProductGroupingReport_KpiProductGroupingTypeDTOs = KpiProductGroupingTypes
                .Select(x => new KpiProductGroupingReport_KpiProductGroupingTypeDTO(x)).ToList();
            return KpiProductGroupingReport_KpiProductGroupingTypeDTOs;
        }


        [Route(KpiProductGroupingReportRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<KpiProductGroupingReport_ProductGroupingDTO>> FilterListProductGrouping([FromBody] KpiProductGroupingReport_ProductGroupingFilterDTO KpiProductGroupingReport_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            ProductGroupingFilter.Id = KpiProductGroupingReport_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = KpiProductGroupingReport_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = KpiProductGroupingReport_ProductGroupingFilterDTO.Name;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<KpiProductGroupingReport_ProductGroupingDTO> KpiProductGroupingReport_ProductGroupingDTOs = ProductGroupings
                .Select(x => new KpiProductGroupingReport_ProductGroupingDTO(x)).ToList();
            return KpiProductGroupingReport_ProductGroupingDTOs;
        }
        #endregion

        [Route(KpiProductGroupingReportRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiProductGroupingReport_KpiProductGroupingReportFilterDTO KpiProductGroupingReport_KpiProductGroupingReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            #region tính thời gian bắt đầu, kết thúc, lấy ra Id nhân viên và orgUnit từ filter
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.AppUserId?.Equal;
            long? ProductGroupingId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.ProductGroupingId?.Equal;
            if (KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiPeriodId?.Equal.HasValue == false ||
                KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiYearId?.Equal.HasValue == false)
                return 0;
            long? KpiPeriodId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiPeriodId?.Equal.Value;
            long? KpiYearId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiYearId?.Equal.Value;
            long? KpiProductGroupingTypeId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiProductGroupingTypeId?.Equal.Value;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId.Value, KpiYearId.Value);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);
            #endregion

            var query = from kpg in DataContext.KpiProductGrouping
                        join kpgc in DataContext.KpiProductGroupingContent on kpg.Id equals kpgc.KpiProductGroupingId
                        join pg in DataContext.ProductGrouping on kpgc.ProductGroupingId equals pg.Id
                        where OrganizationIds.Contains(kpg.OrganizationId) &&
                        AppUserIds.Contains(kpg.EmployeeId) &&
                        (SaleEmployeeId == null || kpg.Id == SaleEmployeeId.Value) &&
                        (ProductGroupingId == null || pg.Id == ProductGroupingId.Value) &&
                        (kpg.KpiPeriodId == KpiPeriodId.Value) &&
                        (kpg.KpiYearId == KpiYearId) &&
                        (kpg.KpiProductGroupingTypeId == KpiProductGroupingTypeId.Value) &&
                        kpg.DeletedAt == null &&
                        kpg.StatusId == StatusEnum.ACTIVE.Id
                        select new
                        {
                            OrganizationId = kpg.OrganizationId,
                            SaleEmployeeId = kpg.EmployeeId,
                        }; // grouping kpi nhóm sản phẩm theo Organization và ProductGrouping
            return await query.Distinct().CountAsync();
        }

        [Route(KpiProductGroupingReportRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiProductGroupingReport_KpiProductGroupingReportDTO>>> List([FromBody] KpiProductGroupingReport_KpiProductGroupingReportFilterDTO KpiProductGroupingReport_KpiProductGroupingReportFilterDTO)
        {
            #region validate dữ liệu filter bắt buộc có 
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiPeriodId?.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn kì KPI" });
            if (KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiYearId?.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn năm KPI" });
            if (KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiProductGroupingTypeId?.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn loại KPI" });
            #endregion

            #region tính thời gian bắt đầu, kết thúc, lấy ra Id nhân viên và orgUnit từ filter
            DateTime StartDate, EndDate;
            long? SaleEmployeeId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.AppUserId?.Equal;
            long? ProductGroupingId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.ProductGroupingId?.Equal;
            long? KpiPeriodId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiPeriodId?.Equal.Value;
            long? KpiYearId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiYearId?.Equal.Value;
            long? KpiProductGroupingTypeId = KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.KpiProductGroupingTypeId?.Equal.Value;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId.Value, KpiYearId.Value);

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);
            #endregion

            #region lấy dữ liệu báo cáo
            var query = from kpg in DataContext.KpiProductGrouping
                        join au in DataContext.AppUser on kpg.EmployeeId equals au.Id
                        join kpgc in DataContext.KpiProductGroupingContent on kpg.Id equals kpgc.KpiProductGroupingId
                        join pg in DataContext.ProductGrouping on kpgc.ProductGroupingId equals pg.Id
                        where OrganizationIds.Contains(kpg.OrganizationId) &&
                        AppUserIds.Contains(kpg.EmployeeId) &&
                        (SaleEmployeeId == null || kpg.Id == SaleEmployeeId.Value) &&
                        (ProductGroupingId == null || pg.Id == ProductGroupingId.Value) &&
                        (kpg.KpiPeriodId == KpiPeriodId.Value) &&
                        (kpg.KpiYearId == KpiYearId) &&
                        (kpg.KpiProductGroupingTypeId == KpiProductGroupingTypeId.Value) &&
                        kpg.DeletedAt == null &&
                        kpg.StatusId == StatusEnum.ACTIVE.Id
                        select new
                        {
                            KpiProductGroupingId = kpg.Id,
                            OrganizationId = kpg.OrganizationId,
                            SaleEmployeeId = kpg.EmployeeId,
                            Username = au.Username,
                            DisplayName = au.DisplayName,
                            ProductGroupingId = pg.Id,
                            ProductGroupingCode = pg.Code,
                            ProductGroupingName = pg.Name,
                        }; // grouping kpi nhóm sản phẩm theo Organization và ProductGrouping

            var datas = await query.Distinct()
                .OrderBy(x => x.OrganizationId)
                .ThenBy(x => x.DisplayName)
                .Skip(KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.Skip)
                .Take(KpiProductGroupingReport_KpiProductGroupingReportFilterDTO.Take)
                .ToListAsync(); // lấy ra toàn bộ dữ liệu theo filter
            var KpiProductGroupingIds = datas
                .Select(x => x.KpiProductGroupingId)
                .Distinct().ToList();
            OrganizationIds = datas
                .Select(x => x.OrganizationId)
                .Distinct().ToList();
            AppUserIds = datas
                .Select(x => x.SaleEmployeeId)
                .Distinct().ToList();
            var OrganizationDAOs = await DataContext.Organization.AsNoTracking()
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync(); // lấy ra toàn bộ Org trong danh sách phân trang
            var AppUserDAOs = await DataContext.AppUser.AsNoTracking()
                .Where(x => AppUserIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new AppUserDAO
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                }).ToListAsync(); // lấy ra toàn bộ Nhân viên trong danh sách phân trang


            var query_content = from km in DataContext.KpiProductGroupingContentCriteriaMapping
                                join kc in DataContext.KpiProductGroupingContent on km.KpiProductGroupingContentId equals kc.Id
                                join k in DataContext.KpiProductGrouping on kc.KpiProductGroupingId equals k.Id
                                join pg in DataContext.ProductGrouping on kc.ProductGroupingId equals pg.Id
                                where KpiProductGroupingIds.Contains(k.Id)
                                select new
                                {
                                    SaleEmployeeId = k.EmployeeId,
                                    ProductGroupingId = kc.ProductGroupingId,
                                    KpiProductGroupingContentId = kc.Id,
                                    KpiProductGroupingCriteriaId = km.KpiProductGroupingCriteriaId,
                                    Value = km.Value,
                                };

            List<KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTO> KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTOs =
                await query_content.Distinct().Select(x => new KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    ProductGroupingId = x.ProductGroupingId,
                    KpiProductGroupingContentId = x.KpiProductGroupingContentId,
                    KpiProductGroupingCriteriaId = x.KpiProductGroupingCriteriaId,
                    Value = x.Value,
                }).ToListAsync();

            List<long> KpiProductGroupingContentIds = KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTOs
                .Select(x => x.KpiProductGroupingContentId)
                .Distinct()
                .ToList();
            List<long> ProductGroupingIds = KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTOs
                .Select(x => x.ProductGroupingId)
                .Distinct().ToList();
            List<ProductGroupingDAO> ProductGroupingDAOs = await DataContext.ProductGrouping.AsNoTracking()
                .Where(x => ProductGroupingIds.Contains(x.Id))
                .Select(x => new ProductGroupingDAO { 
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                })
                .ToListAsync();
            List<KpiProductGroupingContentItemMappingDAO> KpiProductGroupingContentItemMappingDAOs = await DataContext.KpiProductGroupingContentItemMapping.AsNoTracking()
                .Where(x => KpiProductGroupingContentIds.Contains(x.KpiProductGroupingContentId))
                .Select(x => new KpiProductGroupingContentItemMappingDAO
                {
                    ItemId = x.ItemId,
                    KpiProductGroupingContentId = x.KpiProductGroupingContentId,
                })
                .ToListAsync(); // lay ra toan bo itemId duoc map voi contentIds
            List<long> ItemIds = KpiProductGroupingContentItemMappingDAOs
                .Select(x => x.ItemId)
                .Distinct()
                .ToList();

            var indirect_sales_order_query = from transaction in DataContext.IndirectSalesOrderTransaction
                                             join ind in DataContext.IndirectSalesOrder on transaction.IndirectSalesOrderId equals ind.Id
                                             where AppUserIds.Contains(transaction.SalesEmployeeId) &&
                                             (transaction.OrderDate >= StartDate) &&
                                             (transaction.OrderDate <= EndDate) &&
                                             (ind.RequestStateId == RequestStateEnum.APPROVED.Id) &&
                                             (ItemIds.Contains(transaction.ItemId))
                                             select transaction;
            List<IndirectSalesOrderTransactionDAO> IndirectTransactionDAOs = await indirect_sales_order_query
            .Distinct()
            .Select(x => new IndirectSalesOrderTransactionDAO
            {
                Id = x.Id,
                SalesEmployeeId = x.SalesEmployeeId,
                OrderDate = x.OrderDate,
                BuyerStoreId = x.BuyerStoreId,
                ItemId = x.ItemId,
                Revenue = x.Revenue,
            }).ToListAsync();
            #endregion

            #region tổng hợp dữ liệu báo cáo
            List<KpiProductGroupingReport_KpiProductGroupingReportDTO> KpiProductGroupingReport_KpiProductGroupingReportDTOs = new List<KpiProductGroupingReport_KpiProductGroupingReportDTO>();
            foreach (var Organization in OrganizationDAOs)
            {
                KpiProductGroupingReport_KpiProductGroupingReportDTO KpiProductGroupingReport_KpiProductGroupingReportDTO = new KpiProductGroupingReport_KpiProductGroupingReportDTO
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<KpiProductGroupingReport_KpiSaleEmployeetDTO>(),
                };
                KpiProductGroupingReport_KpiProductGroupingReportDTO.SaleEmployees = datas.Where(x => x.OrganizationId == Organization.Id).Select(x => new KpiProductGroupingReport_KpiSaleEmployeetDTO
                {
                    Id = x.SaleEmployeeId,
                    UserName = x.Username,
                    DisplayName = x.DisplayName,
                    OrganizationId = x.OrganizationId,
                }).ToList();
                KpiProductGroupingReport_KpiProductGroupingReportDTOs.Add(KpiProductGroupingReport_KpiProductGroupingReportDTO);
            }

            foreach (var Organization in KpiProductGroupingReport_KpiProductGroupingReportDTOs)
            { 
                foreach(var Employee in Organization.SaleEmployees)
                {

                    Employee.Contents = new List<KpiProductGroupingReport_KpiProductGroupingContentDTO>();
                    List<KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTO> ContentCriteriaMappings = KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTOs
                        .Where(x => x.SaleEmployeeId == Employee.Id)
                        .ToList();
                    List<long> SubProductGroupingIds = ContentCriteriaMappings.Select(x => x.ProductGroupingId)
                        .Distinct()
                        .ToList();
                    List<long> SubkpiProductGroupingContentIds = ContentCriteriaMappings.Select(x => x.KpiProductGroupingContentId)
                        .Distinct()
                        .ToList();
                    List<ProductGroupingDAO> SubProductGroupings = ProductGroupingDAOs
                        .Where(x => SubProductGroupingIds.Contains(x.Id))
                        .ToList();
                    foreach(var ProductGrouping in SubProductGroupings)
                    {
                        KpiProductGroupingReport_KpiProductGroupingContentDTO Content = new KpiProductGroupingReport_KpiProductGroupingContentDTO();
                        Content.ProductGroupingId = ProductGrouping.Id;
                        Content.ProductGroupingCode = ProductGrouping.Code;
                        Content.ProductGroupingName = ProductGrouping.Name;

                        Content.IndirectRevenuePlanned = ContentCriteriaMappings
                            .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                            .Where(x => x.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_REVENUE.Id)
                            .Where(x => x.SaleEmployeeId == Employee.Id)
                            .Select(x => x.Value)
                            .FirstOrDefault(); // lấy ra giá trị kế hoạch theo nhóm sản phẩm, user và loại kpi
                        if(Content.IndirectRevenuePlanned.HasValue)
                        {
                            List<KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTO> SubContentCriteriaMappings = ContentCriteriaMappings
                                .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                                .ToList();
                            List<long> SubContentIds = SubContentCriteriaMappings.Select(x => x.KpiProductGroupingContentId)
                                .Distinct()
                                .ToList();
                            List<long> SubItemIds = KpiProductGroupingContentItemMappingDAOs.Where(x => SubContentIds.Contains(x.KpiProductGroupingContentId))
                                .Select(x => x.ItemId)
                                .Distinct()
                                .ToList();
                            List<IndirectSalesOrderTransactionDAO> SubIndirectTransactions = IndirectTransactionDAOs
                                .Where(x => x.SalesEmployeeId == Employee.Id)
                                .Where(x => ItemIds.Contains(x.ItemId))
                                .ToList(); // lay transaction don gian tiep theo SalesEmployee va Item
                            Content.IndirectRevenue = SubIndirectTransactions
                                .Where(x => x.Revenue.HasValue)
                                .Sum(x => x.Revenue);
                            Content.IndirectRevenue = Math.Round(Content.IndirectRevenue.Value, 0);
                            Content.IndirectRevenueRatio = Content.IndirectRevenuePlanned == 0 || Content.IndirectRevenuePlanned == null ?
                                null : (decimal?)
                                Math.Round((Content.IndirectRevenue.Value / Content.IndirectRevenuePlanned.Value) * 100, 2);
                        }
                        Employee.Contents.Add(Content); // thêm content
                    }
                }
            }
            #endregion

            return KpiProductGroupingReport_KpiProductGroupingReportDTOs;
        }

        [Route(KpiProductGroupingReportRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiProductGroupingReport_KpiProductGroupingReportFilterDTO KpiProductGroupingReport_KpiProductGroupingReportFilterDTO)
        {
            return null;
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
