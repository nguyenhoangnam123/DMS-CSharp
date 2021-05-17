using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MAppUser;
using DMS.Services.MBrand;
using DMS.Services.MCategory;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiProductGrouping;
using DMS.Services.MKpiProductGroupingContent;
using DMS.Services.MKpiProductGroupingCriteria;
using DMS.Services.MKpiProductGroupingType;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_product_grouping
{
    public partial class KpiProductGroupingController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IKpiProductGroupingTypeService KpiProductGroupingTypeService;
        private IKpiProductGroupingCriteriaService KpiProductGroupingCriteriaService;
        private IStatusService StatusService;
        private IKpiProductGroupingService KpiProductGroupingService;
        private IKpiProductGroupingContentService KpiProductGroupingContentService;
        private IItemService ItemService;
        private IKpiYearService KpiYearService;
        private IOrganizationService OrganizationService;
        private ICategoryService CategoryService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private IBrandService BrandService;
        private ICurrentContext CurrentContext;
        private DataContext DataContext;
        public KpiProductGroupingController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IKpiProductGroupingTypeService KpiProductGroupingTypeService,
            IKpiProductGroupingCriteriaService KpiProductGroupingCriteriaService,
            IStatusService StatusService,
            IKpiProductGroupingService KpiProductGroupingService,
            IKpiProductGroupingContentService KpiProductGroupingContentService,
            IItemService ItemService,
            IKpiYearService KpiYearService,
            IOrganizationService OrganizationService,
            ICategoryService CategoryService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            IBrandService BrandService,
            ICurrentContext CurrentContext,
            DataContext DataContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiProductGroupingTypeService = KpiProductGroupingTypeService;
            this.KpiProductGroupingCriteriaService = KpiProductGroupingCriteriaService;
            this.StatusService = StatusService;
            this.KpiProductGroupingService = KpiProductGroupingService;
            this.KpiProductGroupingContentService = KpiProductGroupingContentService;
            this.ItemService = ItemService;
            this.KpiYearService = KpiYearService;
            this.OrganizationService = OrganizationService;
            this.CategoryService = CategoryService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.BrandService = BrandService;
            this.CurrentContext = CurrentContext;
            this.DataContext = DataContext;
        }

        [Route(KpiProductGroupingRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO);
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            int count = await KpiProductGroupingService.Count(KpiProductGroupingFilter);
            return count;
        }

        [Route(KpiProductGroupingRoute.List), HttpPost]
        public async Task<List<KpiProductGrouping_KpiProductGroupingDTO>> List([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO);
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);
            List<KpiProductGrouping_KpiProductGroupingDTO> KpiProductGrouping_KpiProductGroupingDTOs = KpiProductGroupings
                .Select(c => new KpiProductGrouping_KpiProductGroupingDTO(c)).ToList();
            return KpiProductGrouping_KpiProductGroupingDTOs;
        }

        [Route(KpiProductGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Get([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = await KpiProductGroupingService.Get(KpiProductGrouping_KpiProductGroupingDTO.Id);
            return new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
        }

        [Route(KpiProductGroupingRoute.GetDraft), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> GetDraft()
        {
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            List<KpiProductGroupingCriteria> KpiProductGroupingCriterias = await KpiProductGroupingCriteriaService.List(new KpiProductGroupingCriteriaFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiProductGroupingCriteriaSelect.ALL,
            });
            var KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO();
            (KpiProductGrouping_KpiProductGroupingDTO.CurrentMonth, KpiProductGrouping_KpiProductGroupingDTO.CurrentQuarter, KpiProductGrouping_KpiProductGroupingDTO.CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            KpiProductGrouping_KpiProductGroupingDTO.KpiYearId = KpiYearId;
            KpiProductGrouping_KpiProductGroupingDTO.KpiYear = KpiProductGrouping_KpiProductGroupingDTO.KpiYear = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId)
                .Select(x => new KpiProductGrouping_KpiYearDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .FirstOrDefault();
            KpiProductGrouping_KpiProductGroupingDTO.KpiPeriodId = KpiPeriodId; // trả về năm
            KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod = Enums.KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Id == KpiPeriodId)
                .Select(x => new KpiProductGrouping_KpiPeriodDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .FirstOrDefault(); //  trả về kỳ
            KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingCriterias = KpiProductGroupingCriterias.Select(x => new KpiProductGrouping_KpiProductGroupingCriteriaDTO(x)).ToList(); // trả về các chỉ tiêu
            KpiProductGrouping_KpiProductGroupingDTO.Status = new KpiProductGrouping_StatusDTO
            {
                Code = Enums.StatusEnum.ACTIVE.Code,
                Id = Enums.StatusEnum.ACTIVE.Id,
                Name = Enums.StatusEnum.ACTIVE.Name
            }; // trả về trạng thái
            KpiProductGrouping_KpiProductGroupingDTO.StatusId = Enums.StatusEnum.ACTIVE.Id;
            return KpiProductGrouping_KpiProductGroupingDTO;
        }

        [Route(KpiProductGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Create([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO);
            KpiProductGrouping = await KpiProductGroupingService.Create(KpiProductGrouping);
            KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return KpiProductGrouping_KpiProductGroupingDTO;
            else
                return BadRequest(KpiProductGrouping_KpiProductGroupingDTO);
        }

        [Route(KpiProductGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Update([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO);
            KpiProductGrouping = await KpiProductGroupingService.Update(KpiProductGrouping);
            KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return KpiProductGrouping_KpiProductGroupingDTO;
            else
                return BadRequest(KpiProductGrouping_KpiProductGroupingDTO);
        }

        [Route(KpiProductGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Delete([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO);
            KpiProductGrouping = await KpiProductGroupingService.Delete(KpiProductGrouping);
            KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return KpiProductGrouping_KpiProductGroupingDTO;
            else
                return BadRequest(KpiProductGrouping_KpiProductGroupingDTO);
        }

        [Route(KpiProductGroupingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            KpiProductGroupingFilter.Id = new IdFilter { In = Ids };
            KpiProductGroupingFilter.Selects = KpiProductGroupingSelect.Id;
            KpiProductGroupingFilter.Skip = 0;
            KpiProductGroupingFilter.Take = int.MaxValue;

            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);
            KpiProductGroupings = await KpiProductGroupingService.BulkDelete(KpiProductGroupings);
            if (KpiProductGroupings.Any(x => !x.IsValidated))
                return BadRequest(KpiProductGroupings.Where(x => !x.IsValidated));
            return true;
        }

        [Route(KpiProductGroupingRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
            AppUserFilter EmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Employees = await AppUserService.List(EmployeeFilter);
            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL
            };
            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            KpiProductGroupingTypeFilter KpiProductGroupingTypeFilter = new KpiProductGroupingTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiProductGroupingTypeSelect.ALL
            };
            List<KpiProductGroupingType> KpiProductGroupingTypes = await KpiProductGroupingTypeService.List(KpiProductGroupingTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiProductGrouping> KpiProductGroupings = new List<KpiProductGrouping>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(KpiProductGroupings);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int OrganizationIdColumn = 1 + StartColumn;
                int KpiYearIdColumn = 2 + StartColumn;
                int KpiPeriodIdColumn = 3 + StartColumn;
                int KpiProductGroupingTypeIdColumn = 4 + StartColumn;
                int StatusIdColumn = 5 + StartColumn;
                int EmployeeIdColumn = 6 + StartColumn;
                int CreatorIdColumn = 7 + StartColumn;
                int RowIdColumn = 11 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string KpiYearIdValue = worksheet.Cells[i + StartRow, KpiYearIdColumn].Value?.ToString();
                    string KpiPeriodIdValue = worksheet.Cells[i + StartRow, KpiPeriodIdColumn].Value?.ToString();
                    string KpiProductGroupingTypeIdValue = worksheet.Cells[i + StartRow, KpiProductGroupingTypeIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string EmployeeIdValue = worksheet.Cells[i + StartRow, EmployeeIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();

                    KpiProductGrouping KpiProductGrouping = new KpiProductGrouping();
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    KpiProductGrouping.CreatorId = Creator == null ? 0 : Creator.Id;
                    KpiProductGrouping.Creator = Creator;
                    AppUser Employee = Employees.Where(x => x.Id.ToString() == EmployeeIdValue).FirstOrDefault();
                    KpiProductGrouping.EmployeeId = Employee == null ? 0 : Employee.Id;
                    KpiProductGrouping.Employee = Employee;
                    KpiPeriod KpiPeriod = KpiPeriods.Where(x => x.Id.ToString() == KpiPeriodIdValue).FirstOrDefault();
                    KpiProductGrouping.KpiPeriodId = KpiPeriod == null ? 0 : KpiPeriod.Id;
                    KpiProductGrouping.KpiPeriod = KpiPeriod;
                    KpiProductGroupingType KpiProductGroupingType = KpiProductGroupingTypes.Where(x => x.Id.ToString() == KpiProductGroupingTypeIdValue).FirstOrDefault();
                    KpiProductGrouping.KpiProductGroupingTypeId = KpiProductGroupingType == null ? 0 : KpiProductGroupingType.Id;
                    KpiProductGrouping.KpiProductGroupingType = KpiProductGroupingType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    KpiProductGrouping.StatusId = Status == null ? 0 : Status.Id;
                    KpiProductGrouping.Status = Status;

                    KpiProductGroupings.Add(KpiProductGrouping);
                }
            }
            KpiProductGroupings = await KpiProductGroupingService.Import(KpiProductGroupings);
            if (KpiProductGroupings.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < KpiProductGroupings.Count; i++)
                {
                    KpiProductGrouping KpiProductGrouping = KpiProductGroupings[i];
                    if (!KpiProductGrouping.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.Id)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.Id)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.OrganizationId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.OrganizationId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.KpiYearId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.KpiYearId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.KpiPeriodId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.KpiPeriodId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.KpiProductGroupingTypeId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.KpiProductGroupingTypeId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.StatusId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.StatusId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.EmployeeId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.EmployeeId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.CreatorId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.CreatorId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.RowId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.RowId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(KpiProductGroupingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            #region validate dữ liệu filter bắt buộc có 
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (KpiProductGrouping_KpiProductGroupingFilterDTO.KpiYearId.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn năm Kpi" });

            if (KpiProductGrouping_KpiProductGroupingFilterDTO.KpiPeriodId.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn kỳ Kpi" });

            #endregion

            #region dữ liệu kì, năm kpi
            long KpiYearId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiYearId.Equal.Value;
            var KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId).FirstOrDefault();

            long KpiPeriodId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiPeriodId.Equal.Value;
            var KpiPeriod = KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Id == KpiPeriodId).FirstOrDefault();
            #endregion

            #region lấy ra dữ liệu kpiProductGrouping từ filter
            KpiProductGrouping_KpiProductGroupingFilterDTO.Skip = 0;
            KpiProductGrouping_KpiProductGroupingFilterDTO.Take = int.MaxValue;
            KpiProductGrouping_KpiProductGroupingFilterDTO.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<KpiProductGrouping_KpiProductGroupingDTO> KpiProductGrouping_KpiProductGroupingDTOs = await List(KpiProductGrouping_KpiProductGroupingFilterDTO);
            List<long> KpiProductGroupingIds = KpiProductGrouping_KpiProductGroupingDTOs.Select(x => x.Id)
                .Distinct()
                .ToList();
            List<long> AppUserIds = KpiProductGrouping_KpiProductGroupingDTOs.Select(x => x.EmployeeId)
                .Distinct()
                .ToList();
            List<long> OrganizationIds = KpiProductGrouping_KpiProductGroupingDTOs
                .Select(x => x.OrganizationId)
                .Distinct()
                .ToList();
            List<KpiProductGroupingContent> KpiProductGroupingContents = await KpiProductGroupingContentService.List(new KpiProductGroupingContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiProductGroupingContentSelect.ALL,
                KpiProductGroupingId = new IdFilter { In = KpiProductGroupingIds }
            });
            List<KpiProductGroupingContentCriteriaMapping> kpiProductGroupingContentCriteriaMappings = KpiProductGroupingContents
                .SelectMany(x => x.KpiProductGroupingContentCriteriaMappings)
                .ToList(); // lay ra mapping content voi chi tieu
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter{ 
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Name,
                Id = new IdFilter { In = OrganizationIds }
            });
            List<AppUser> AppUsers = await AppUserService.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName,
                Id = new IdFilter { In = AppUserIds }
            });
            #endregion

            #region tổ hợp dữ liệu
            List<KpiProductGrouping_ExportDTO> KpiProductGrouping_ExportDTOs = new List<KpiProductGrouping_ExportDTO>();
            foreach (var Organization in Organizations)
            {
                KpiProductGrouping_ExportDTO kpiProductGrouping_ExportDTO = new KpiProductGrouping_ExportDTO();
                kpiProductGrouping_ExportDTO.OrganizationName = Organization.Name;
                kpiProductGrouping_ExportDTO.Employees = new List<KpiProductGrouping_AppUserExportDTO>();
                KpiProductGrouping_ExportDTOs.Add(kpiProductGrouping_ExportDTO);

                List<KpiProductGrouping_KpiProductGroupingDTO> SubKpiProductGroupings = KpiProductGrouping_KpiProductGroupingDTOs
                    .Where(x => x.OrganizationId == Organization.Id)
                    .ToList();
                List<long> SubAppUserIds = SubKpiProductGroupings
                    .Select(x => x.EmployeeId)
                    .ToList();
                List<AppUser> SubAppUsers = AppUsers
                    .Where(x => SubAppUserIds.Contains(x.Id))
                    .Distinct()
                    .ToList();
                foreach (var SubAppUser in SubAppUsers)
                {
                    KpiProductGrouping_AppUserExportDTO KpiProductGrouping_AppUserExportDTO = new KpiProductGrouping_AppUserExportDTO();
                    KpiProductGrouping_AppUserExportDTO.Username = SubAppUser.Username;
                    KpiProductGrouping_AppUserExportDTO.DisplayName = SubAppUser.DisplayName;
                    KpiProductGrouping_AppUserExportDTO.ProductGroupings = new List<KpiProductGrouping_ProductGroupingExportDTO>();
                    kpiProductGrouping_ExportDTO.Employees.Add(KpiProductGrouping_AppUserExportDTO);

                    var SubKpiProductGrouping = SubKpiProductGroupings
                        .Where(x => x.EmployeeId == SubAppUser.Id)
                        .FirstOrDefault();
                    if (SubKpiProductGrouping != null)
                    {
                        KpiProductGrouping_AppUserExportDTO.KpiProductGroupingTypeName = SubKpiProductGrouping.KpiProductGroupingType.Name;
                        List<KpiProductGroupingContent> SubContents =  KpiProductGroupingContents
                            .Where(x => x.KpiProductGroupingId == SubKpiProductGrouping.Id)
                            .ToList();
                        List<ProductGrouping> SubProductGroupings = SubContents
                            .Select(x => x.ProductGrouping)
                            .Distinct()
                            .ToList();
                        List<KpiProductGroupingContentItemMapping> SubItemMappings = SubContents
                            .SelectMany(x => x.KpiProductGroupingContentItemMappings)
                            .ToList();
                        foreach (var ProductGrouping in SubProductGroupings)
                        {
                            KpiProductGrouping_ProductGroupingExportDTO KpiProductGrouping_ProductGroupingExportDTO = new KpiProductGrouping_ProductGroupingExportDTO();
                            KpiProductGrouping_ProductGroupingExportDTO.Code = ProductGrouping.Code;
                            KpiProductGrouping_ProductGroupingExportDTO.Name = ProductGrouping.Name;
                            KpiProductGrouping_ProductGroupingExportDTO.ItemCount = SubItemMappings.Where(x => x.KpiProductGroupingContent.ProductGroupingId == ProductGrouping.Id)
                                .Select(x => x.ItemId)
                                .Distinct()
                                .Count(); // dem so item trong productGrouping
                            KpiProductGrouping_ProductGroupingExportDTO.Items = new List<KpiProductGrouping_ExportItemDTO>();
                            KpiProductGrouping_AppUserExportDTO.ProductGroupings.Add(KpiProductGrouping_ProductGroupingExportDTO);

                            List<KpiProductGroupingContent> LeafContents = SubContents
                                .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                                .ToList();
                            List<KpiProductGroupingContentItemMapping> LeafSubItemMappings = LeafContents
                                .SelectMany(x => x.KpiProductGroupingContentItemMappings)
                                .Distinct()
                                .ToList();
                            foreach (var ItemMapping in LeafSubItemMappings)
                            {
                                KpiProductGrouping_ExportItemDTO KpiProductGrouping_ExportItemDTO = new KpiProductGrouping_ExportItemDTO();
                                KpiProductGrouping_ExportItemDTO.Code = ItemMapping.Item.Code;
                                KpiProductGrouping_ExportItemDTO.Name = ItemMapping.Item.Name;
                                KpiProductGrouping_ProductGroupingExportDTO.Items.Add(KpiProductGrouping_ExportItemDTO);

                                List<KpiProductGroupingContentCriteriaMapping> SubCriteriaMappings = kpiProductGroupingContentCriteriaMappings
                                    .Where(x => x.KpiProductGroupingContentId == ItemMapping.KpiProductGroupingContentId)
                                    .ToList();
                                if(SubCriteriaMappings != null && SubCriteriaMappings.Any())
                                {
                                    foreach(var CriteriaMapping in SubCriteriaMappings)
                                    {
                                        if(CriteriaMapping.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_REVENUE.Id)
                                        {
                                            KpiProductGrouping_ExportItemDTO.IndirectRevenue = CriteriaMapping.Value;
                                        }
                                        if (CriteriaMapping.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_STORE.Id)
                                        {
                                            KpiProductGrouping_ExportItemDTO.IndirectStoreCounter = CriteriaMapping.Value;
                                        }
                                    } // them chi tieu kpi cho item
                                }
                            } // them item
                        } // them productGrouping
                    }

                } // thêm Employee
            } // thêm orgName
            KpiProductGrouping_ExportDTOs = KpiProductGrouping_ExportDTOs.Where(x => x.Employees.Count > 0).ToList(); // chỉ lấy org nào có dữ liệu
            #endregion

            #region ghi dữ liệu vào file
            string path = "Templates/Kpi_Product_Grouping_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiProductGroupings = KpiProductGrouping_ExportDTOs; // đổ dữ liệu vào sheet chính
            Data.KpiPeriod = KpiPeriod.Name;
            Data.KpiYear = KpiYear.Name;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            #endregion

            return File(output.ToArray(), "application/octet-stream", "KpiProductGroupings.xlsx");
        }

        [Route(KpiProductGroupingRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            #region dữ liệu sheet chính
            var appUser = await AppUserService.Get(CurrentContext.UserId);
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiProductGrouping_ExportTemplateDTO> KpiProductGrouping_ExportTemplateDTOs = new List<KpiProductGrouping_ExportTemplateDTO>();
            foreach (var AppUser in AppUsers)
            {
                KpiProductGrouping_ExportTemplateDTO KpiProductGrouping_ExportTemplateDTO = new KpiProductGrouping_ExportTemplateDTO();
                KpiProductGrouping_ExportTemplateDTO.Username = AppUser.Username;
                KpiProductGrouping_ExportTemplateDTO.DisplayName = AppUser.DisplayName;
                KpiProductGrouping_ExportTemplateDTOs.Add(KpiProductGrouping_ExportTemplateDTO);
            }
            #endregion


            #region dữ liệu đổ vào các sheet khác
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.Code | ProductGroupingSelect.Name,
                OrderBy = ProductGroupingOrder.Id,
                OrderType = OrderType.ASC
            });
            var query = from i in DataContext.Item
                        join prd in DataContext.Product on i.ProductId equals prd.Id
                        join prdg in DataContext.ProductProductGroupingMapping on prd.Id equals prdg.ProductId
                        join prg in DataContext.ProductGrouping on prdg.ProductGroupingId equals prg.Id
                        select new KpiProductGrouping_ItemExportDTO
                        {
                            ItemCode = i.Code,
                            ItemName = i.Name,
                            IsNew = prd.IsNew,
                            ProductGroupingCode = prg.Code,
                            ProductGroupingName = prg.Name,
                        };

            List<KpiProductGrouping_ItemExportDTO> Items = await query.ToListAsync();
            List<KpiProductGrouping_ItemExportDTO> NewItems = Items.Where(x => x.IsNew).ToList();
            #endregion

            #region ghi dữ liệu vào file
            string path = "Templates/Kpi_Product_Grouping.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiProductGroupings = KpiProductGrouping_ExportTemplateDTOs; // đổ dữ liệu vào sheet chính
            Data.ProductGroupings = ProductGroupings; // đổ dữ liệu vào tab nhóm sản phẩm
            Data.Items = Items; // đổ dữ liệu vào tab sản phẩm
            Data.NewItems = NewItems; // đổ dữ liệu vào tab sản phẩm mới
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            #endregion

            return File(output.ToArray(), "application/octet-stream", "Template_Kpi_ProductGrouping.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiProductGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiProductGroupingService.Count(KpiProductGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiProductGrouping ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            KpiProductGrouping KpiProductGrouping = new KpiProductGrouping();
            KpiProductGrouping.Id = KpiProductGrouping_KpiProductGroupingDTO.Id;
            KpiProductGrouping.OrganizationId = KpiProductGrouping_KpiProductGroupingDTO.OrganizationId;
            KpiProductGrouping.KpiYearId = KpiProductGrouping_KpiProductGroupingDTO.KpiYearId;
            KpiProductGrouping.KpiPeriodId = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriodId;
            KpiProductGrouping.KpiProductGroupingTypeId = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingTypeId;
            KpiProductGrouping.StatusId = KpiProductGrouping_KpiProductGroupingDTO.StatusId;
            KpiProductGrouping.EmployeeId = KpiProductGrouping_KpiProductGroupingDTO.EmployeeId;
            KpiProductGrouping.CreatorId = KpiProductGrouping_KpiProductGroupingDTO.CreatorId;
            KpiProductGrouping.RowId = KpiProductGrouping_KpiProductGroupingDTO.RowId;
            KpiProductGrouping.Creator = KpiProductGrouping_KpiProductGroupingDTO.Creator == null ? null : new AppUser
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.Creator.Id,
                Username = KpiProductGrouping_KpiProductGroupingDTO.Creator.Username,
                DisplayName = KpiProductGrouping_KpiProductGroupingDTO.Creator.DisplayName,
                Address = KpiProductGrouping_KpiProductGroupingDTO.Creator.Address,
                Email = KpiProductGrouping_KpiProductGroupingDTO.Creator.Email,
                Phone = KpiProductGrouping_KpiProductGroupingDTO.Creator.Phone,
            };
            KpiProductGrouping.Employee = KpiProductGrouping_KpiProductGroupingDTO.Employee == null ? null : new AppUser
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.Employee.Id,
                Username = KpiProductGrouping_KpiProductGroupingDTO.Employee.Username,
                DisplayName = KpiProductGrouping_KpiProductGroupingDTO.Employee.DisplayName,
                Address = KpiProductGrouping_KpiProductGroupingDTO.Employee.Address,
                Email = KpiProductGrouping_KpiProductGroupingDTO.Employee.Email,
                Phone = KpiProductGrouping_KpiProductGroupingDTO.Employee.Phone,
            };
            KpiProductGrouping.KpiPeriod = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod == null ? null : new KpiPeriod
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod.Id,
                Code = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod.Code,
                Name = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod.Name,
            };
            KpiProductGrouping.KpiProductGroupingType = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType == null ? null : new KpiProductGroupingType
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType.Id,
                Code = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType.Code,
                Name = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType.Name,
            };
            KpiProductGrouping.Status = KpiProductGrouping_KpiProductGroupingDTO.Status == null ? null : new Status
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.Status.Id,
                Code = KpiProductGrouping_KpiProductGroupingDTO.Status.Code,
                Name = KpiProductGrouping_KpiProductGroupingDTO.Status.Name,
            };
            KpiProductGrouping.Employees = KpiProductGrouping_KpiProductGroupingDTO.Employees?.Select(x => new AppUser
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Username = x.Username,
                Phone = x.Phone,
                Email = x.Email,
            }).ToList();
            KpiProductGrouping.KpiProductGroupingContents = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingContents?
                .Select(x => new KpiProductGroupingContent
                {
                    Id = x.Id,
                    ProductGroupingId = x.ProductGroupingId,
                    ProductGrouping = x.ProductGrouping == null ? null : new ProductGrouping
                    {
                        Id = x.ProductGrouping.Id,
                        Code = x.ProductGrouping.Code,
                        Name = x.ProductGrouping.Name,
                        ParentId = x.ProductGrouping.ParentId,
                        Path = x.ProductGrouping.Path,
                    },
                    KpiProductGroupingContentCriteriaMappings = x.KpiProductGroupingContentCriteriaMappings.Select(p => new KpiProductGroupingContentCriteriaMapping
                    {
                        KpiProductGroupingCriteriaId = p.Key,
                        Value = p.Value,
                    }).ToList(), // map chi tieu voi gia tri
                    KpiProductGroupingContentItemMappings = x.KpiProductGroupingContentItemMappings.Select(p => new KpiProductGroupingContentItemMapping
                    {
                        ItemId = p.ItemId
                    }).ToList(), // map content voi itemId
                }).ToList();

            KpiProductGrouping.BaseLanguage = CurrentContext.Language;
            return KpiProductGrouping;
        }

        private KpiProductGroupingFilter ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter.Selects = KpiProductGroupingSelect.ALL;
            KpiProductGroupingFilter.Skip = KpiProductGrouping_KpiProductGroupingFilterDTO.Skip;
            KpiProductGroupingFilter.Take = KpiProductGrouping_KpiProductGroupingFilterDTO.Take;
            KpiProductGroupingFilter.OrderBy = KpiProductGrouping_KpiProductGroupingFilterDTO.OrderBy;
            KpiProductGroupingFilter.OrderType = KpiProductGrouping_KpiProductGroupingFilterDTO.OrderType;

            KpiProductGroupingFilter.Id = KpiProductGrouping_KpiProductGroupingFilterDTO.Id;
            KpiProductGroupingFilter.OrganizationId = KpiProductGrouping_KpiProductGroupingFilterDTO.OrganizationId;
            KpiProductGroupingFilter.KpiYearId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiYearId;
            KpiProductGroupingFilter.KpiPeriodId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiPeriodId;
            KpiProductGroupingFilter.KpiProductGroupingTypeId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiProductGroupingTypeId;
            KpiProductGroupingFilter.StatusId = KpiProductGrouping_KpiProductGroupingFilterDTO.StatusId;
            KpiProductGroupingFilter.EmployeeId = KpiProductGrouping_KpiProductGroupingFilterDTO.EmployeeId;
            KpiProductGroupingFilter.CreatorId = KpiProductGrouping_KpiProductGroupingFilterDTO.CreatorId;
            KpiProductGroupingFilter.RowId = KpiProductGrouping_KpiProductGroupingFilterDTO.RowId;
            KpiProductGroupingFilter.CreatedAt = KpiProductGrouping_KpiProductGroupingFilterDTO.CreatedAt;
            KpiProductGroupingFilter.UpdatedAt = KpiProductGrouping_KpiProductGroupingFilterDTO.UpdatedAt;
            return KpiProductGroupingFilter;
        }

        private Tuple<GenericEnum, GenericEnum, GenericEnum> ConvertDateTime(DateTime date)
        {
            GenericEnum monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum quarterName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum yearName = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == StaticParams.DateTimeNow.Year).FirstOrDefault();
            switch (date.Month)
            {
                case 1:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 2:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH02;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 3:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH03;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 4:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH04;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 5:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH05;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 6:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH06;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 7:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH07;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 8:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH08;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 9:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH09;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 10:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH10;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 11:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH11;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 12:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH12;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
            }
            return Tuple.Create(monthName, quarterName, yearName);
        }
    }
}

