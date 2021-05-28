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
using System.Text;
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
            for (int i = 0; i < KpiProductGroupings.Count; i++)
            {
                KpiProductGroupings[i] = Utils.Clone(KpiProductGroupings[i]);
                KpiProductGroupings[i].Organization = KpiProductGroupings[i].Employee.Organization;
                KpiProductGroupings[i].OrganizationId = KpiProductGroupings[i].Employee.OrganizationId;
            } // BA yêu cầu trả về đơn vị tổ chức của Employee, không phải của Kpi
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
            KpiProductGrouping_KpiProductGroupingDTO NewKpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return NewKpiProductGroupingDTO;
            NewKpiProductGroupingDTO.Employees = KpiProductGrouping_KpiProductGroupingDTO.Employees; // map lai Employees neu xay ra loi
            return BadRequest(NewKpiProductGroupingDTO);
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

            #region validate file
            StringBuilder errorContent = new StringBuilder();
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
            {
                errorContent.AppendLine("Định dạng file không hợp lệ");
                return BadRequest(errorContent.ToString());
            }

            GenericEnum KpiYear;
            GenericEnum KpiPeriod;
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["KPI nhom san pham"];
                if (worksheet == null)
                {
                    errorContent.AppendLine("File không đúng biểu mẫu import");
                    return BadRequest(errorContent.ToString());
                }

                string KpiPeriodValue = worksheet.Cells[2, 4].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(KpiPeriodValue))
                    KpiPeriod = KpiPeriodEnum.KpiPeriodEnumList.Where(x => x.Name == KpiPeriodValue).FirstOrDefault();
                else
                {
                    errorContent.AppendLine("Chưa chọn kỳ Kpi hoặc kỳ Kpi không hợp lệ");
                    return BadRequest(errorContent.ToString());
                }

                string KpiYearValue = worksheet.Cells[2, 6].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(KpiYearValue))
                    KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Name == KpiYearValue.Trim()).FirstOrDefault();
                else
                {
                    errorContent.AppendLine("Chưa chọn năm Kpi hoặc năm Kpi không hợp lệ");
                    return BadRequest(errorContent.ToString());
                }
            }

            HashSet<long> KpiPeriodIds = new HashSet<long>();
            long CurrentMonth = 100 + StaticParams.DateTimeNow.Month;
            long CurrentQuater = 0;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH01.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH03.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER01.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH04.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH06.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER02.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH07.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH09.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER03.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH10.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER04.Id;
            if (KpiYear.Id >= StaticParams.DateTimeNow.Year)
            {
                KpiPeriodIds.Add(KpiYear.Id);
            }
            foreach (var kpiPeriod in KpiPeriodEnum.KpiPeriodEnumList)
            {
                if (CurrentMonth <= kpiPeriod.Id && kpiPeriod.Id <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
                    KpiPeriodIds.Add(kpiPeriod.Id);
                if (CurrentQuater <= kpiPeriod.Id && kpiPeriod.Id <= Enums.KpiPeriodEnum.PERIOD_QUATER04.Id)
                    KpiPeriodIds.Add(kpiPeriod.Id);
            }

            if (!KpiPeriodIds.Contains(KpiPeriod.Id))
            {
                errorContent.AppendLine("Không thể nhập Kpi cho các kỳ trong quá khứ");
                return BadRequest(errorContent.ToString());
            }
            #endregion

            #region filter dữ liệu
            AppUserFilter EmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization,
                Id = new IdFilter { }
            };

            EmployeeFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            List<AppUser> Employees = await AppUserService.List(EmployeeFilter);
            var AppUserIds = Employees.Select(x => x.Id).ToList();

            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(new KpiProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                EmployeeId = new IdFilter { In = AppUserIds },
                KpiYearId = new IdFilter { Equal = KpiYear.Id },
                KpiPeriodId = new IdFilter { Equal = KpiPeriod.Id },
                Selects = KpiProductGroupingSelect.ALL
            });
            List<long> KpiProductGroupingIds = KpiProductGroupings.Select(x => x.Id).ToList();
            List<KpiProductGroupingContent> KpiProductGroupingContents = await KpiProductGroupingContentService.List(new KpiProductGroupingContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiProductGroupingContentSelect.ALL,
                KpiProductGroupingId = new IdFilter { In = KpiProductGroupingIds }
            });
            Parallel.ForEach(KpiProductGroupings, KpiProductGrouping =>
            {
                KpiProductGrouping.KpiProductGroupingContents = KpiProductGroupingContents.Where(x => x.KpiProductGroupingId == KpiProductGrouping.Id).ToList();
            });
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code | ProductGroupingSelect.Name,
            }); // lay ra tat ca productGrouping de validate

            var item_query = from i in DataContext.Item
                             join pr in DataContext.Product on i.ProductId equals pr.Id
                             join prgm in DataContext.ProductProductGroupingMapping on pr.Id equals prgm.ProductId
                             join prg in DataContext.ProductGrouping on prgm.ProductGroupingId equals prg.Id
                             select new KpiProductGrouping_ItemImportDTO
                             {
                                 Id = i.Id,
                                 Code = i.Code,
                                 Name = i.Name,
                                 IsNew = pr.IsNew,
                                 ProductGroupingId = prg.Id,
                             };
            List<KpiProductGrouping_ItemImportDTO> Items = await item_query.ToListAsync();
            #endregion

            #region lấy ra dữ liệu
            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            List<KpiProductGrouping_ImportDTO> KpiProductGrouping_ImportDTOs = new List<KpiProductGrouping_ImportDTO>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["KPI nhom san pham"];

                int StartColumn = 1; // cot bat dau tinh du lieu
                int StartRow = 5; // dong bat dau tinh du lieu
                int UsernameColumn = 0 + StartColumn;
                int KpiProductGroupingTypeColumnValue = 2 + StartColumn;
                int ProductGroupingCodeColumn = 3 + StartColumn;
                int ItemCodeColumn = 4 + StartColumn;
                int RevenueColumn = 5 + StartColumn;
                int StoreColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    string KpiProductGroupingTypeValue = worksheet.Cells[i, KpiProductGroupingTypeColumnValue].Value?.ToString();
                    string ProductGroupingCodeValue = worksheet.Cells[i, ProductGroupingCodeColumn].Value?.ToString();
                    string ItemCodeValue = worksheet.Cells[i, ItemCodeColumn].Value?.ToString();
                    string RevenueValue = worksheet.Cells[i, RevenueColumn].Value?.ToString();
                    string StoreValue = worksheet.Cells[i, StoreColumn].Value?.ToString();

                    KpiProductGrouping_ImportDTO KpiProductGrouping_ImportDTO = new KpiProductGrouping_ImportDTO();

                    #region validate nhan vien
                    if (UsernameValue != null && UsernameValue.ToLower() == "END".ToLower())
                        break;
                    else if (!string.IsNullOrWhiteSpace(UsernameValue) && string.IsNullOrWhiteSpace(ItemCodeValue))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(UsernameValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập mã nhân viên");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(UsernameValue) && i == worksheet.Dimension.End.Row)
                        break;

                    AppUser Employee = Employees.Where(x => x.Username.ToLower() == UsernameValue.ToLower()).FirstOrDefault();
                    if (Employee == null)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Nhân viên không tồn tại");
                        continue;
                    }
                    else
                    {
                        KpiProductGrouping_ImportDTO.EmployeeId = Employee.Id;
                        KpiProductGrouping_ImportDTO.OrganizationId = Employee.OrganizationId;
                    }
                    #endregion

                    #region validate loai kpi
                    GenericEnum KpiProductGroupingType;
                    if (!string.IsNullOrWhiteSpace(KpiProductGroupingTypeValue))
                    {
                        KpiProductGroupingType = KpiProductGroupingTypeEnum.KpiProductGroupingTypeEnumList.Where(x => x.Name == KpiProductGroupingTypeValue.Trim()).FirstOrDefault();
                        if (KpiProductGroupingType == null)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Loại KPI sản phẩm không tồn tại");
                            continue;
                        }
                        else
                        {
                            KpiProductGrouping_ImportDTO.KpiProductGroupingTypeId = KpiProductGroupingType.Id;
                        }
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa chọn loại KPI sản phẩm");
                        continue;
                    }
                    #endregion

                    #region validate nhom san pham va item
                    ProductGrouping ProductGrouping;
                    if (!string.IsNullOrWhiteSpace(ProductGroupingCodeValue))
                    {
                        ProductGrouping = ProductGroupings.Where(x => x.Code == ProductGroupingCodeValue.Trim()).FirstOrDefault();
                        if (ProductGrouping == null)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Nhóm sản phẩm không tồn tại");
                            continue;
                        }
                        KpiProductGrouping_ImportDTO.ProductGroupingId = ProductGrouping.Id;
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa chọn nhóm sản phẩm");
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(ItemCodeValue))
                    {
                        List<string> ItemCodes = ItemCodeValue.Trim().Split(";").ToList();
                        ItemCodes = ItemCodes
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToList(); // bỏ các khoảng trắng hoặc null trong list item code
                        List<KpiProductGrouping_ItemImportDTO> LineItems = new List<KpiProductGrouping_ItemImportDTO>();
                        foreach (var ItemCode in ItemCodes)
                        {
                            KpiProductGrouping_ItemImportDTO Item = Items.Where(x => x.Code.ToLower() == ItemCode.ToLower().Trim()).FirstOrDefault();
                            if (Item == null)
                            {
                                errorContent.AppendLine($"Lỗi dòng thứ {i}: Sản phẩm không tồn tại");
                                continue;
                            }
                            else if (KpiProductGroupingType.Id == KpiProductGroupingTypeEnum.NEW_PRODUCT_GROUPING.Id && !Item.IsNew)
                            {
                                errorContent.AppendLine($"Lỗi dòng thứ {i}: {Item.Code} - {Item.Name} Không phải sản phẩm mới");
                                continue;
                            }
                            //else if (ProductGrouping.Id != Item.ProductGroupingId)
                            //{
                            //    errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Sản phẩm không thuộc nhóm sản phẩm");
                            //    continue;
                            //} // ko cần validate Item trong group vì có thể chọn Item khác Group
                            else
                            {
                                LineItems.Add(Item);
                            }
                        }
                        KpiProductGrouping_ImportDTO.Items = LineItems; // lay ra tat ca cac item hop le
                    }
                    if (string.IsNullOrWhiteSpace(RevenueValue))
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập chỉ tiêu doanh thu");
                        continue;
                    }
                    else
                    {
                        decimal Revenue;
                        if (!decimal.TryParse(RevenueValue, out Revenue))
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Chỉ tiêu doanh thu không hợp lệ");
                            continue;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(StoreValue))
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập chỉ tiêu số cửa hàng");
                        continue;
                    }
                    else
                    {
                        long StoreCount;
                        if (!long.TryParse(StoreValue, out StoreCount))
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Chỉ tiêu số cửa hàng không hợp lệ");
                            continue;
                        }
                    }
                    #endregion

                    KpiProductGrouping_ImportDTO.Stt = i;
                    KpiProductGrouping_ImportDTO.Username = UsernameValue;
                    KpiProductGrouping_ImportDTO.ProductGroupingCode = ProductGroupingCodeValue;
                    KpiProductGrouping_ImportDTO.IndirectRevenue = worksheet.Cells[i, RevenueColumn].Value?.ToString();
                    KpiProductGrouping_ImportDTO.IndirectStoreCounter = worksheet.Cells[i, StoreColumn].Value?.ToString();
                    KpiProductGrouping_ImportDTO.KpiPeriodId = KpiPeriod.Id;
                    KpiProductGrouping_ImportDTO.KpiYearId = KpiYear.Id;
                    KpiProductGrouping_ImportDTOs.Add(KpiProductGrouping_ImportDTO);
                }
            }

            Dictionary<long, StringBuilder> Errors = new Dictionary<long, StringBuilder>();
            Dictionary<long, int> OldKpiCountDict = new Dictionary<long, int>();
            foreach (KpiProductGrouping_ImportDTO ImportDTO in KpiProductGrouping_ImportDTOs)
            {
                if (ImportDTO.HasValue == false)
                {
                    Errors[ImportDTO.Stt].Append($"Lỗi dòng thứ {ImportDTO.Stt}: Chưa nhập chỉ tiêu");
                    continue;
                }
                KpiProductGrouping KpiProductGroupingInDB = KpiProductGroupings.Where(x => x.EmployeeId == ImportDTO.EmployeeId &&
                    x.KpiPeriodId == ImportDTO.KpiPeriodId &&
                    x.KpiYearId == ImportDTO.KpiYearId &&
                    x.KpiProductGroupingTypeId == ImportDTO.KpiProductGroupingTypeId)
                    .FirstOrDefault(); // tim trong 
                if (KpiProductGroupingInDB == null)
                {
                    KpiProductGroupingInDB = new KpiProductGrouping();
                    KpiProductGroupingInDB.EmployeeId = ImportDTO.EmployeeId;
                    KpiProductGroupingInDB.OrganizationId = ImportDTO.OrganizationId;
                    KpiProductGroupingInDB.KpiPeriodId = ImportDTO.KpiPeriodId;
                    KpiProductGroupingInDB.KpiYearId = ImportDTO.KpiYearId;
                    KpiProductGroupingInDB.KpiProductGroupingTypeId = ImportDTO.KpiProductGroupingTypeId;
                    KpiProductGroupingInDB.RowId = Guid.NewGuid();
                    KpiProductGroupingInDB.KpiProductGroupingContents = new List<KpiProductGroupingContent>();
                    KpiProductGroupingInDB.KpiProductGroupingContents.Add(new KpiProductGroupingContent
                    {
                        ProductGroupingId = ImportDTO.ProductGroupingId,
                        RowId = Guid.NewGuid(),
                        KpiProductGroupingContentCriteriaMappings = KpiProductGroupingCriteriaEnum.KpiProductGroupingCriteriaEnumList.Select(x => new KpiProductGroupingContentCriteriaMapping
                        {
                            KpiProductGroupingCriteriaId = x.Id,
                        }).ToList(),
                        KpiProductGroupingContentItemMappings = ImportDTO.Items.Select(x => new KpiProductGroupingContentItemMapping
                        {
                            ItemId = x.Id
                        }).ToList()
                    });
                    KpiProductGroupings.Add(KpiProductGroupingInDB);
                } // neu them moi Kpi
                else
                {
                    int OldKpiCount = default(int);
                    if (!OldKpiCountDict.TryGetValue(KpiProductGroupingInDB.Id, out OldKpiCount))
                    {
                        OldKpiCountDict.Add(KpiProductGroupingInDB.Id, 1);
                        KpiProductGroupingInDB.KpiProductGroupingContents = new List<KpiProductGroupingContent> {new KpiProductGroupingContent
                        {
                            ProductGroupingId = ImportDTO.ProductGroupingId,
                            RowId = Guid.NewGuid(),
                            KpiProductGroupingContentCriteriaMappings = KpiProductGroupingCriteriaEnum.KpiProductGroupingCriteriaEnumList.Select(x => new KpiProductGroupingContentCriteriaMapping
                            {
                                KpiProductGroupingCriteriaId = x.Id,
                            }).ToList(),
                            KpiProductGroupingContentItemMappings = ImportDTO.Items.Select(x => new KpiProductGroupingContentItemMapping
                            {
                                ItemId = x.Id
                            }).ToList()
                        }};
                    } // nếu là lần đầu tiên update kpi thì xóa hết content của kpi đó và thêm mới một kpi content tương ứng
                    else
                    {
                        var KpiProductGroupingContent = KpiProductGroupingInDB.KpiProductGroupingContents
                            .Where(x => x.KpiProductGroupingId == ImportDTO.ProductGroupingId)
                            .FirstOrDefault();
                        if (KpiProductGroupingContent == null)
                        {
                            KpiProductGroupingInDB.KpiProductGroupingContents.Add(new KpiProductGroupingContent
                            {
                                ProductGroupingId = ImportDTO.ProductGroupingId,
                                RowId = Guid.NewGuid(),
                                KpiProductGroupingContentCriteriaMappings = KpiProductGroupingCriteriaEnum.KpiProductGroupingCriteriaEnumList.Select(x => new KpiProductGroupingContentCriteriaMapping
                                {
                                    KpiProductGroupingCriteriaId = x.Id,
                                }).ToList(),
                                KpiProductGroupingContentItemMappings = ImportDTO.Items.Select(x => new KpiProductGroupingContentItemMapping
                                {
                                    ItemId = x.Id
                                }).ToList()
                            });
                        }
                    }

                }
                var Content = KpiProductGroupingInDB.KpiProductGroupingContents
                            .Where(x => x.ProductGroupingId == ImportDTO.ProductGroupingId)
                            .FirstOrDefault();
                if (Content != null)
                {
                    foreach (var KpiProductGroupingContentCriteriaMapping in Content.KpiProductGroupingContentCriteriaMappings)
                    {
                        if (long.TryParse(ImportDTO.IndirectRevenue, out long IndirectRevenue)
                            && KpiProductGroupingContentCriteriaMapping.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_REVENUE.Id)
                        {
                            KpiProductGroupingContentCriteriaMapping.Value = IndirectRevenue;
                        }
                        else if (long.TryParse(ImportDTO.IndirectStoreCounter, out long IndirectStoreCounter)
                            && KpiProductGroupingContentCriteriaMapping.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_STORE.Id)
                        {
                            KpiProductGroupingContentCriteriaMapping.Value = IndirectStoreCounter;
                        }
                    } // update list mapping content voi chi tieu
                }

                KpiProductGroupingInDB.CreatorId = AppUser.Id;
                KpiProductGroupingInDB.StatusId = StatusEnum.ACTIVE.Id;
            } // thiet lap Kpi, KpiContent tu DTOs

            #endregion
            if (errorContent.Length > 0)
                return BadRequest(errorContent.ToString()); // tra ve loi khi lay du lieu
            string error = string.Join("\n", Errors.Where(x => !string.IsNullOrWhiteSpace(x.Value.ToString())).Select(x => x.Value.ToString()).ToList());
            if (!string.IsNullOrWhiteSpace(error))
                return BadRequest(error);
            KpiProductGroupings = await KpiProductGroupingService.Import(KpiProductGroupings);
            List<KpiProductGrouping_KpiProductGroupingDTO> KpiProductGrouping_KpiProductGroupingDTOs = KpiProductGroupings
                .Select(x => new KpiProductGrouping_KpiProductGroupingDTO(x))
                .ToList();
            return Ok(KpiProductGroupings);
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
            KpiProductGrouping_KpiProductGroupingFilterDTO.KpiPeriodId = new IdFilter { Equal = KpiPeriodId };
            KpiProductGrouping_KpiProductGroupingFilterDTO.KpiYearId = new IdFilter { Equal = KpiYearId };

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
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
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
            long stt = 1;
            foreach (var Organization in Organizations)
            {
                KpiProductGrouping_ExportDTO kpiProductGrouping_ExportDTO = new KpiProductGrouping_ExportDTO();
                kpiProductGrouping_ExportDTO.OrganizationName = Organization.Name;
                kpiProductGrouping_ExportDTO.Kpis = new List<KpiProductGrouping_KpiProductGroupingExportDTO>();
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
                    List<KpiProductGrouping_KpiProductGroupingDTO> AppUserKpis = SubKpiProductGroupings
                        .Where(x => x.EmployeeId == SubAppUser.Id)
                        .ToList(); // lay tat cac kpi cua User

                    foreach (KpiProductGrouping_KpiProductGroupingDTO AppUserKpi in AppUserKpis)
                    {
                        KpiProductGrouping_KpiProductGroupingExportDTO KpiProductGrouping_KpiProductGroupingExportDTO = new KpiProductGrouping_KpiProductGroupingExportDTO();
                        KpiProductGrouping_KpiProductGroupingExportDTO.STT = stt;
                        stt++;
                        KpiProductGrouping_KpiProductGroupingExportDTO.UserName = SubAppUser.Username;
                        KpiProductGrouping_KpiProductGroupingExportDTO.DisplayName = SubAppUser.DisplayName;
                        KpiProductGrouping_KpiProductGroupingExportDTO.KpiProductGroupingTypeName = AppUserKpi.KpiProductGroupingType.Name;
                        KpiProductGrouping_KpiProductGroupingExportDTO.ProductGroupings = new List<KpiProductGrouping_ProductGroupingExportDTO>();
                        kpiProductGrouping_ExportDTO.Kpis.Add(KpiProductGrouping_KpiProductGroupingExportDTO);

                        List<KpiProductGroupingContent> SubContents = KpiProductGroupingContents
                            .Where(x => x.KpiProductGroupingId == AppUserKpi.Id)
                            .ToList();
                        foreach (KpiProductGroupingContent KpiProductGroupingContent in SubContents)
                        {
                            KpiProductGrouping_ProductGroupingExportDTO KpiProductGrouping_ProductGroupingExportDTO = new KpiProductGrouping_ProductGroupingExportDTO();
                            KpiProductGrouping_ProductGroupingExportDTO.Code = KpiProductGroupingContent.ProductGrouping.Code;
                            KpiProductGrouping_ProductGroupingExportDTO.Name = KpiProductGroupingContent.ProductGrouping.Name;
                            KpiProductGrouping_ProductGroupingExportDTO.ItemCount = KpiProductGroupingContent.KpiProductGroupingContentItemMappings.Count;
                            List<KpiProductGroupingContentCriteriaMapping> SubCriteriaMappings = KpiProductGroupingContent.KpiProductGroupingContentCriteriaMappings;
                            foreach (var CriteriaMapping in SubCriteriaMappings)
                            {
                                if (CriteriaMapping.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_REVENUE.Id)
                                {
                                    KpiProductGrouping_ProductGroupingExportDTO.IndirectRevenue = CriteriaMapping.Value;
                                }
                                if (CriteriaMapping.KpiProductGroupingCriteriaId == KpiProductGroupingCriteriaEnum.INDIRECT_STORE.Id)
                                {
                                    KpiProductGrouping_ProductGroupingExportDTO.IndirectStoreCounter = CriteriaMapping.Value;
                                }
                            } // them chi tieu kpi cho item

                            KpiProductGrouping_ProductGroupingExportDTO.Items = new List<KpiProductGrouping_ExportItemDTO>();
                            KpiProductGrouping_KpiProductGroupingExportDTO.ProductGroupings.Add(KpiProductGrouping_ProductGroupingExportDTO);

                            foreach (var SubItemMapping in KpiProductGroupingContent.KpiProductGroupingContentItemMappings)
                            {
                                KpiProductGrouping_ExportItemDTO kpiProductGrouping_ExportItemDTO = new KpiProductGrouping_ExportItemDTO();
                                kpiProductGrouping_ExportItemDTO.Code = SubItemMapping.Item.Code;
                                kpiProductGrouping_ExportItemDTO.Name = SubItemMapping.Item.Name;
                                KpiProductGrouping_ProductGroupingExportDTO.Items.Add(kpiProductGrouping_ExportItemDTO);
                            }
                        }
                    }
                } // thêm Employee
            } // thêm orgName
            KpiProductGrouping_ExportDTOs = KpiProductGrouping_ExportDTOs.Where(x => x.Kpis.Count > 0).ToList(); // chỉ lấy org nào có dữ liệu
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
            Data.ProductGroupings = ProductGroupings.Select(x => new {
                Code = Utils.ReplaceHexadecimalSymbols(x.Code),
                Name = Utils.ReplaceHexadecimalSymbols(x.Name),
            }); // đổ dữ liệu vào tab nhóm sản phẩm
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

