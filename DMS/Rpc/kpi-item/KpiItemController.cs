using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MKpiCriteriaItem;
using DMS.Services.MKpiCriteriaTotal;
using DMS.Services.MKpiItem;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_item
{
    public class KpiItemController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IKpiYearService KpiYearService;
        private IOrganizationService OrganizationService;
        private IKpiCriteriaItemService KpiCriteriaItemService;
        private IKpiCriteriaTotalService KpiCriteriaTotalService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IKpiItemService KpiItemService;
        private ISupplierService SupplierService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public KpiItemController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IKpiYearService KpiYearService,
            IOrganizationService OrganizationService,
            IKpiCriteriaItemService KpiCriteriaItemService,
            IKpiCriteriaTotalService KpiCriteriaTotalService,
            IStatusService StatusService,
            IItemService ItemService,
            IKpiItemService KpiItemService,
            ISupplierService SupplierService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiYearService = KpiYearService;
            this.OrganizationService = OrganizationService;
            this.KpiCriteriaItemService = KpiCriteriaItemService;
            this.KpiCriteriaTotalService = KpiCriteriaTotalService;
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.KpiItemService = KpiItemService;
            this.SupplierService = SupplierService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiItemRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiItemFilter KpiItemFilter = ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO);
            KpiItemFilter = KpiItemService.ToFilter(KpiItemFilter);
            int count = await KpiItemService.Count(KpiItemFilter);
            return count;
        }

        [Route(KpiItemRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiItem_KpiItemDTO>>> List([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiItemFilter KpiItemFilter = ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO);
            KpiItemFilter = KpiItemService.ToFilter(KpiItemFilter);
            List<KpiItem> KpiItems = await KpiItemService.List(KpiItemFilter);
            List<KpiItem_KpiItemDTO> KpiItem_KpiItemDTOs = KpiItems
                .Select(c => new KpiItem_KpiItemDTO(c)).ToList();
            return KpiItem_KpiItemDTOs;
        }

        [Route(KpiItemRoute.Get), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Get([FromBody]KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = await KpiItemService.Get(KpiItem_KpiItemDTO.Id);
            return new KpiItem_KpiItemDTO(KpiItem);
        }

        [Route(KpiItemRoute.GetDraft), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> GetDraft()
        {
            List<KpiCriteriaTotal> KpiCriteriaTotals = await KpiCriteriaTotalService.List(new KpiCriteriaTotalFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaTotalSelect.ALL,
            });
            List<KpiCriteriaItem> KpiCriteriaItems = await KpiCriteriaItemService.List(new KpiCriteriaItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaItemSelect.ALL,
            });
            var KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO();
            KpiItem_KpiItemDTO.KpiCriteriaTotals = KpiCriteriaTotals.Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            KpiItem_KpiItemDTO.KpiItemKpiCriteriaTotalMappings = KpiCriteriaTotals.ToDictionary(x => x.Id, y => 0L);
            KpiItem_KpiItemDTO.KpiCriteriaItems = KpiCriteriaItems.Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            KpiItem_KpiItemDTO.Status = new KpiItem_StatusDTO
            {
                Code = Enums.StatusEnum.ACTIVE.Code,
                Id = Enums.StatusEnum.ACTIVE.Id,
                Name = Enums.StatusEnum.ACTIVE.Name
            };
            KpiItem_KpiItemDTO.StatusId = Enums.StatusEnum.ACTIVE.Id;
            return KpiItem_KpiItemDTO;
        }

        [Route(KpiItemRoute.Create), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Create([FromBody] KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = ConvertDTOToEntity(KpiItem_KpiItemDTO);
            KpiItem = await KpiItemService.Create(KpiItem);
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO(KpiItem);
            if (KpiItem.IsValidated)
                return KpiItem_KpiItemDTO;
            else
                return BadRequest(KpiItem_KpiItemDTO);
        }

        [Route(KpiItemRoute.Update), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Update([FromBody] KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = ConvertDTOToEntity(KpiItem_KpiItemDTO);
            KpiItem = await KpiItemService.Update(KpiItem);
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO(KpiItem);
            if (KpiItem.IsValidated)
                return KpiItem_KpiItemDTO;
            else
                return BadRequest(KpiItem_KpiItemDTO);
        }

        [Route(KpiItemRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiItem_KpiItemDTO>> Delete([FromBody] KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiItem_KpiItemDTO.Id))
                return Forbid();

            KpiItem KpiItem = ConvertDTOToEntity(KpiItem_KpiItemDTO);
            KpiItem = await KpiItemService.Delete(KpiItem);
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO(KpiItem);
            if (KpiItem.IsValidated)
                return KpiItem_KpiItemDTO;
            else
                return BadRequest(KpiItem_KpiItemDTO);
        }

        [Route(KpiItemRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiItemFilter KpiItemFilter = new KpiItemFilter();
            KpiItemFilter = KpiItemService.ToFilter(KpiItemFilter);
            KpiItemFilter.Id = new IdFilter { In = Ids };
            KpiItemFilter.Selects = KpiItemSelect.Id;
            KpiItemFilter.Skip = 0;
            KpiItemFilter.Take = int.MaxValue;

            List<KpiItem> KpiItems = await KpiItemService.List(KpiItemFilter);
            KpiItems = await KpiItemService.BulkDelete(KpiItems);
            return true;
        }

        [Route(KpiItemRoute.Import), HttpPost]
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
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiItem> KpiItems = new List<KpiItem>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(KpiItems);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int OrganizationIdColumn = 1 + StartColumn;
                int KpiPeriodIdColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                int EmployeeIdColumn = 4 + StartColumn;
                int CreatorIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string KpiPeriodIdValue = worksheet.Cells[i + StartRow, KpiPeriodIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string EmployeeIdValue = worksheet.Cells[i + StartRow, EmployeeIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();

                    KpiItem KpiItem = new KpiItem();
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    KpiItem.CreatorId = Creator == null ? 0 : Creator.Id;
                    KpiItem.Creator = Creator;
                    AppUser Employee = Employees.Where(x => x.Id.ToString() == EmployeeIdValue).FirstOrDefault();
                    KpiItem.EmployeeId = Employee == null ? 0 : Employee.Id;
                    KpiItem.Employee = Employee;
                    KpiPeriod KpiPeriod = KpiPeriods.Where(x => x.Id.ToString() == KpiPeriodIdValue).FirstOrDefault();
                    KpiItem.KpiPeriodId = KpiPeriod == null ? 0 : KpiPeriod.Id;
                    KpiItem.KpiPeriod = KpiPeriod;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    KpiItem.StatusId = Status == null ? 0 : Status.Id;
                    KpiItem.Status = Status;

                    KpiItems.Add(KpiItem);
                }
            }
            KpiItems = await KpiItemService.Import(KpiItems);
            if (KpiItems.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < KpiItems.Count; i++)
                {
                    KpiItem KpiItem = KpiItems[i];
                    if (!KpiItem.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (KpiItem.Errors.ContainsKey(nameof(KpiItem.Id)))
                            Error += KpiItem.Errors[nameof(KpiItem.Id)];
                        if (KpiItem.Errors.ContainsKey(nameof(KpiItem.OrganizationId)))
                            Error += KpiItem.Errors[nameof(KpiItem.OrganizationId)];
                        if (KpiItem.Errors.ContainsKey(nameof(KpiItem.KpiPeriodId)))
                            Error += KpiItem.Errors[nameof(KpiItem.KpiPeriodId)];
                        if (KpiItem.Errors.ContainsKey(nameof(KpiItem.StatusId)))
                            Error += KpiItem.Errors[nameof(KpiItem.StatusId)];
                        if (KpiItem.Errors.ContainsKey(nameof(KpiItem.EmployeeId)))
                            Error += KpiItem.Errors[nameof(KpiItem.EmployeeId)];
                        if (KpiItem.Errors.ContainsKey(nameof(KpiItem.CreatorId)))
                            Error += KpiItem.Errors[nameof(KpiItem.CreatorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(KpiItemRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiItem
                var KpiItemFilter = ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO);
                KpiItemFilter.Skip = 0;
                KpiItemFilter.Take = int.MaxValue;
                KpiItemFilter = KpiItemService.ToFilter(KpiItemFilter);
                List<KpiItem> KpiItems = await KpiItemService.List(KpiItemFilter);

                var KpiItemHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "OrganizationId",
                        "KpiPeriodId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                    }
                };
                List<object[]> KpiItemData = new List<object[]>();
                for (int i = 0; i < KpiItems.Count; i++)
                {
                    var KpiItem = KpiItems[i];
                    KpiItemData.Add(new Object[]
                    {
                        KpiItem.Id,
                        KpiItem.OrganizationId,
                        KpiItem.KpiPeriodId,
                        KpiItem.StatusId,
                        KpiItem.EmployeeId,
                        KpiItem.CreatorId,
                    });
                }
                excel.GenerateWorksheet("KpiItem", KpiItemHeaders, KpiItemData);
                #endregion

                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Username",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "Avatar",
                        "ProvinceId",
                        "SexId",
                        "Birthday",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.ProvinceId,
                        AppUser.SexId,
                        AppUser.Birthday,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region KpiPeriod
                var KpiPeriodFilter = new KpiPeriodFilter();
                KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
                KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
                KpiPeriodFilter.OrderType = OrderType.ASC;
                KpiPeriodFilter.Skip = 0;
                KpiPeriodFilter.Take = int.MaxValue;
                List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);

                var KpiPeriodHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiPeriodData = new List<object[]>();
                for (int i = 0; i < KpiPeriods.Count; i++)
                {
                    var KpiPeriod = KpiPeriods[i];
                    KpiPeriodData.Add(new Object[]
                    {
                        KpiPeriod.Id,
                        KpiPeriod.Code,
                        KpiPeriod.Name,
                    });
                }
                excel.GenerateWorksheet("KpiPeriod", KpiPeriodHeaders, KpiPeriodData);
                #endregion
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion

                #region Item
                var ItemFilter = new ItemFilter();
                ItemFilter.Selects = ItemSelect.ALL;
                ItemFilter.OrderBy = ItemOrder.Id;
                ItemFilter.OrderType = OrderType.ASC;
                ItemFilter.Skip = 0;
                ItemFilter.Take = int.MaxValue;
                List<Item> Items = await ItemService.List(ItemFilter);

                var ItemHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "ProductId",
                        "Code",
                        "Name",
                        "ScanCode",
                        "SalePrice",
                        "RetailPrice",
                        "StatusId",
                    }
                };
                List<object[]> ItemData = new List<object[]>();
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    ItemData.Add(new Object[]
                    {
                        Item.Id,
                        Item.ProductId,
                        Item.Code,
                        Item.Name,
                        Item.ScanCode,
                        Item.SalePrice,
                        Item.RetailPrice,
                        Item.StatusId,
                    });
                }
                excel.GenerateWorksheet("Item", ItemHeaders, ItemData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiItem.xlsx");
        }

        [Route(KpiItemRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiItem
                var KpiItemHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "OrganizationId",
                        "KpiPeriodId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                    }
                };
                List<object[]> KpiItemData = new List<object[]>();
                excel.GenerateWorksheet("KpiItem", KpiItemHeaders, KpiItemData);
                #endregion

                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Username",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "Avatar",
                        "ProvinceId",
                        "SexId",
                        "Birthday",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.ProvinceId,
                        AppUser.SexId,
                        AppUser.Birthday,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region KpiPeriod
                var KpiPeriodFilter = new KpiPeriodFilter();
                KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
                KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
                KpiPeriodFilter.OrderType = OrderType.ASC;
                KpiPeriodFilter.Skip = 0;
                KpiPeriodFilter.Take = int.MaxValue;
                List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);

                var KpiPeriodHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiPeriodData = new List<object[]>();
                for (int i = 0; i < KpiPeriods.Count; i++)
                {
                    var KpiPeriod = KpiPeriods[i];
                    KpiPeriodData.Add(new Object[]
                    {
                        KpiPeriod.Id,
                        KpiPeriod.Code,
                        KpiPeriod.Name,
                    });
                }
                excel.GenerateWorksheet("KpiPeriod", KpiPeriodHeaders, KpiPeriodData);
                #endregion
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                #region Item
                var ItemFilter = new ItemFilter();
                ItemFilter.Selects = ItemSelect.ALL;
                ItemFilter.OrderBy = ItemOrder.Id;
                ItemFilter.OrderType = OrderType.ASC;
                ItemFilter.Skip = 0;
                ItemFilter.Take = int.MaxValue;
                List<Item> Items = await ItemService.List(ItemFilter);

                var ItemHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "ProductId",
                        "Code",
                        "Name",
                        "ScanCode",
                        "SalePrice",
                        "RetailPrice",
                        "StatusId",
                    }
                };
                List<object[]> ItemData = new List<object[]>();
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    ItemData.Add(new Object[]
                    {
                        Item.Id,
                        Item.ProductId,
                        Item.Code,
                        Item.Name,
                        Item.ScanCode,
                        Item.SalePrice,
                        Item.RetailPrice,
                        Item.StatusId,
                    });
                }
                excel.GenerateWorksheet("Item", ItemHeaders, ItemData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiItem.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiItemFilter KpiItemFilter = new KpiItemFilter();
            KpiItemFilter = KpiItemService.ToFilter(KpiItemFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiItemFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiItemService.Count(KpiItemFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiItem ConvertDTOToEntity(KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            KpiItem KpiItem = new KpiItem();
            KpiItem.Id = KpiItem_KpiItemDTO.Id;
            KpiItem.OrganizationId = KpiItem_KpiItemDTO.OrganizationId;
            KpiItem.KpiYearId = KpiItem_KpiItemDTO.KpiYearId;
            KpiItem.KpiPeriodId = KpiItem_KpiItemDTO.KpiPeriodId;
            KpiItem.StatusId = KpiItem_KpiItemDTO.StatusId;
            KpiItem.EmployeeId = KpiItem_KpiItemDTO.EmployeeId;
            KpiItem.CreatorId = KpiItem_KpiItemDTO.CreatorId;
            KpiItem.Creator = KpiItem_KpiItemDTO.Creator == null ? null : new AppUser
            {
                Id = KpiItem_KpiItemDTO.Creator.Id,
                Username = KpiItem_KpiItemDTO.Creator.Username,
                DisplayName = KpiItem_KpiItemDTO.Creator.DisplayName,
                Address = KpiItem_KpiItemDTO.Creator.Address,
                Email = KpiItem_KpiItemDTO.Creator.Email,
                Phone = KpiItem_KpiItemDTO.Creator.Phone,
            };
            KpiItem.Employee = KpiItem_KpiItemDTO.Employee == null ? null : new AppUser
            {
                Id = KpiItem_KpiItemDTO.Employee.Id,
                Username = KpiItem_KpiItemDTO.Employee.Username,
                DisplayName = KpiItem_KpiItemDTO.Employee.DisplayName,
                Address = KpiItem_KpiItemDTO.Employee.Address,
                Email = KpiItem_KpiItemDTO.Employee.Email,
                Phone = KpiItem_KpiItemDTO.Employee.Phone,
            };
            KpiItem.KpiYear = KpiItem_KpiItemDTO.KpiYear == null ? null : new KpiYear
            {
                Id = KpiItem_KpiItemDTO.KpiYear.Id,
                Code = KpiItem_KpiItemDTO.KpiYear.Code,
                Name = KpiItem_KpiItemDTO.KpiYear.Name,
            };
            KpiItem.KpiPeriod = KpiItem_KpiItemDTO.KpiPeriod == null ? null : new KpiPeriod
            {
                Id = KpiItem_KpiItemDTO.KpiPeriod.Id,
                Code = KpiItem_KpiItemDTO.KpiPeriod.Code,
                Name = KpiItem_KpiItemDTO.KpiPeriod.Name,
            };
            KpiItem.Organization = KpiItem_KpiItemDTO.Organization == null ? null : new Organization
            {
                Id = KpiItem_KpiItemDTO.Organization.Id,
                Code = KpiItem_KpiItemDTO.Organization.Code,
                Name = KpiItem_KpiItemDTO.Organization.Name,
                ParentId = KpiItem_KpiItemDTO.Organization.ParentId,
            };
            KpiItem.Status = KpiItem_KpiItemDTO.Status == null ? null : new Status
            {
                Id = KpiItem_KpiItemDTO.Status.Id,
                Code = KpiItem_KpiItemDTO.Status.Code,
                Name = KpiItem_KpiItemDTO.Status.Name,
            };
            KpiItem.EmployeeIds = KpiItem_KpiItemDTO.EmployeeIds; // to do
            KpiItem.KpiItemContents = KpiItem_KpiItemDTO.KpiItemContents?
                .Select(x => new KpiItemContent
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                    },
                    KpiItemContentKpiCriteriaItemMappings = x.KpiItemContentKpiCriteriaItemMappings.Select(p => new KpiItemContentKpiCriteriaItemMapping
                    {
                        KpiCriteriaItemId = p.Key,
                        Value = p.Value,
                    }).ToList(),
                }).ToList();
            KpiItem.KpiItemKpiCriteriaTotalMappings = KpiItem_KpiItemDTO.KpiItemKpiCriteriaTotalMappings?.Select(p => new KpiItemKpiCriteriaTotalMapping
            {
                KpiCriteriaTotalId = p.Key,
                Value = p.Value
            }).ToList();
            KpiItem.BaseLanguage = CurrentContext.Language;
            return KpiItem;
        }

        private KpiItemFilter ConvertFilterDTOToFilterEntity(KpiItem_KpiItemFilterDTO KpiItem_KpiItemFilterDTO)
        {
            KpiItemFilter KpiItemFilter = new KpiItemFilter();
            KpiItemFilter.Selects = KpiItemSelect.ALL;
            KpiItemFilter.Skip = KpiItem_KpiItemFilterDTO.Skip;
            KpiItemFilter.Take = KpiItem_KpiItemFilterDTO.Take;
            KpiItemFilter.OrderBy = KpiItem_KpiItemFilterDTO.OrderBy;
            KpiItemFilter.OrderType = KpiItem_KpiItemFilterDTO.OrderType;

            KpiItemFilter.Id = KpiItem_KpiItemFilterDTO.Id;
            KpiItemFilter.OrganizationId = KpiItem_KpiItemFilterDTO.OrganizationId;
            KpiItemFilter.KpiYearId = KpiItem_KpiItemFilterDTO.KpiYearId;
            KpiItemFilter.KpiPeriodId = KpiItem_KpiItemFilterDTO.KpiPeriodId;
            KpiItemFilter.StatusId = KpiItem_KpiItemFilterDTO.StatusId;
            KpiItemFilter.EmployeeId = KpiItem_KpiItemFilterDTO.EmployeeId;
            KpiItemFilter.CreatorId = KpiItem_KpiItemFilterDTO.CreatorId;
            KpiItemFilter.CreatedAt = KpiItem_KpiItemFilterDTO.CreatedAt;
            KpiItemFilter.UpdatedAt = KpiItem_KpiItemFilterDTO.UpdatedAt;
            return KpiItemFilter;
        }

        [Route(KpiItemRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> FilterListAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiItem_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }
        [Route(KpiItemRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiItem_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiItem_KpiPeriodFilterDTO KpiItem_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiItem_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiItem_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiItem_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiItem_KpiPeriodDTO> KpiItem_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiItem_KpiPeriodDTO(x)).ToList();
            return KpiItem_KpiPeriodDTOs;
        }

        [Route(KpiItemRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiItem_KpiYearDTO>> FilterListKpiYear([FromBody] KpiItem_KpiYearFilterDTO KpiItem_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiItem_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiItem_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiItem_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiItem_KpiYearDTO> KpiItem_KpiYearDTOs = KpiYears
                .Select(x => new KpiItem_KpiYearDTO(x)).ToList();
            return KpiItem_KpiYearDTOs;
        }

        [Route(KpiItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiItem_OrganizationDTO>> FilterListOrganization([FromBody] KpiItem_OrganizationFilterDTO KpiItem_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiItem_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = KpiItem_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = KpiItem_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = KpiItem_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = KpiItem_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = KpiItem_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = KpiItem_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = KpiItem_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiItem_OrganizationDTO> KpiItem_OrganizationDTOs = Organizations
                .Select(x => new KpiItem_OrganizationDTO(x)).ToList();
            return KpiItem_OrganizationDTOs;
        }
        [Route(KpiItemRoute.FilterListStatus), HttpPost]
        public async Task<List<KpiItem_StatusDTO>> FilterListStatus()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiItem_StatusDTO> KpiItem_StatusDTOs = Statuses
                .Select(x => new KpiItem_StatusDTO(x)).ToList();
            return KpiItem_StatusDTOs;
        }

        [Route(KpiItemRoute.FilterListKpiCriteriaItem), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaItemDTO>> FilterListKpiCriteriaItem()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaItemFilter KpiCriteriaItemFilter = new KpiCriteriaItemFilter();
            KpiCriteriaItemFilter.Skip = 0;
            KpiCriteriaItemFilter.Take = int.MaxValue;
            KpiCriteriaItemFilter.Take = 20;
            KpiCriteriaItemFilter.OrderBy = KpiCriteriaItemOrder.Id;
            KpiCriteriaItemFilter.OrderType = OrderType.ASC;
            KpiCriteriaItemFilter.Selects = KpiCriteriaItemSelect.ALL;

            List<KpiCriteriaItem> KpiCriteriaItemes = await KpiCriteriaItemService.List(KpiCriteriaItemFilter);
            List<KpiItem_KpiCriteriaItemDTO> KpiItem_KpiCriteriaItemDTOs = KpiCriteriaItemes
                .Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            return KpiItem_KpiCriteriaItemDTOs;
        }

        [Route(KpiItemRoute.FilterListKpiCriteriaTotal), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaTotalDTO>> FilterListKpiCriteriaTotal()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaTotalFilter KpiCriteriaTotalFilter = new KpiCriteriaTotalFilter();
            KpiCriteriaTotalFilter.Skip = 0;
            KpiCriteriaTotalFilter.Take = int.MaxValue;
            KpiCriteriaTotalFilter.Take = 20;
            KpiCriteriaTotalFilter.OrderBy = KpiCriteriaTotalOrder.Id;
            KpiCriteriaTotalFilter.OrderType = OrderType.ASC;
            KpiCriteriaTotalFilter.Selects = KpiCriteriaTotalSelect.ALL;

            List<KpiCriteriaTotal> KpiCriteriaTotales = await KpiCriteriaTotalService.List(KpiCriteriaTotalFilter);
            List<KpiItem_KpiCriteriaTotalDTO> KpiItem_KpiCriteriaTotalDTOs = KpiCriteriaTotales
                .Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            return KpiItem_KpiCriteriaTotalDTOs;
        }

        [Route(KpiItemRoute.FilterListItem), HttpPost]
        public async Task<List<KpiItem_ItemDTO>> FilterListItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = KpiItem_ItemFilterDTO.Id;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = KpiItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItem_ItemDTO> KpiItem_ItemDTOs = Items
                .Select(x => new KpiItem_ItemDTO(x)).ToList();
            return KpiItem_ItemDTOs;
        }

        [Route(KpiItemRoute.FilterListSupplier), HttpPost]
        public async Task<List<KpiItem_SupplierDTO>> FilterListSupplier([FromBody] KpiItem_SupplierFilterDTO KpiItem_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = KpiItem_SupplierFilterDTO.Id;
            SupplierFilter.Code = KpiItem_SupplierFilterDTO.Code;
            SupplierFilter.Name = KpiItem_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = KpiItem_SupplierFilterDTO.TaxCode;

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<KpiItem_SupplierDTO> KpiItem_SupplierDTOs = Suppliers
                .Select(x => new KpiItem_SupplierDTO(x)).ToList();
            return KpiItem_SupplierDTOs;
        }
        [Route(KpiItemRoute.FilterListProductType), HttpPost]
        public async Task<List<KpiItem_ProductTypeDTO>> FilterListProductType([FromBody] KpiItem_ProductTypeFilterDTO KpiItem_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = KpiItem_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = KpiItem_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = KpiItem_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = KpiItem_ProductTypeFilterDTO.Description;

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<KpiItem_ProductTypeDTO> KpiItem_ProductTypeDTOs = ProductTypes
                .Select(x => new KpiItem_ProductTypeDTO(x)).ToList();
            return KpiItem_ProductTypeDTOs;
        }

        [Route(KpiItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<KpiItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] KpiItem_ProductGroupingFilterDTO KpiItem_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<KpiItem_ProductGroupingDTO> KpiItem_ProductGroupingDTOs = ProductGroupings
                .Select(x => new KpiItem_ProductGroupingDTO(x)).ToList();
            return KpiItem_ProductGroupingDTOs;
        }

        [Route(KpiItemRoute.SingleListAppUser), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> SingleListAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiItem_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }
        [Route(KpiItemRoute.SingleListKpiPeriod), HttpPost]
        public async Task<List<KpiItem_KpiPeriodDTO>> SingleListKpiPeriod([FromBody] KpiItem_KpiPeriodFilterDTO KpiItem_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = KpiItem_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = KpiItem_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = KpiItem_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiItem_KpiPeriodDTO> KpiItem_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiItem_KpiPeriodDTO(x)).ToList();
            return KpiItem_KpiPeriodDTOs;
        }
        [Route(KpiItemRoute.SingleListKpiYear), HttpPost]
        public async Task<List<KpiItem_KpiYearDTO>> SingleListKpiYear([FromBody] KpiItem_KpiYearFilterDTO KpiItem_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;
            KpiYearFilter.Id = KpiItem_KpiYearFilterDTO.Id;
            KpiYearFilter.Code = KpiItem_KpiYearFilterDTO.Code;
            KpiYearFilter.Name = KpiItem_KpiYearFilterDTO.Name;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiItem_KpiYearDTO> KpiItem_KpiYearDTOs = KpiYears
                .Select(x => new KpiItem_KpiYearDTO(x)).ToList();
            return KpiItem_KpiYearDTOs;
        }
        [Route(KpiItemRoute.SingleListOrganization), HttpPost]
        public async Task<List<KpiItem_OrganizationDTO>> SingleListOrganization([FromBody] KpiItem_OrganizationFilterDTO KpiItem_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiItem_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = KpiItem_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = KpiItem_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = KpiItem_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = KpiItem_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = KpiItem_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = KpiItem_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = KpiItem_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiItem_OrganizationDTO> KpiItem_OrganizationDTOs = Organizations
                .Select(x => new KpiItem_OrganizationDTO(x)).ToList();
            return KpiItem_OrganizationDTOs;
        }
        [Route(KpiItemRoute.SingleListStatus), HttpPost]
        public async Task<List<KpiItem_StatusDTO>> SingleListStatus()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiItem_StatusDTO> KpiItem_StatusDTOs = Statuses
                .Select(x => new KpiItem_StatusDTO(x)).ToList();
            return KpiItem_StatusDTOs;
        }

        [Route(KpiItemRoute.SingleListKpiCriteriaItem), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaItemDTO>> SingleListKpiCriteriaItem()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaItemFilter KpiCriteriaItemFilter = new KpiCriteriaItemFilter();
            KpiCriteriaItemFilter.Skip = 0;
            KpiCriteriaItemFilter.Take = int.MaxValue;
            KpiCriteriaItemFilter.Take = 20;
            KpiCriteriaItemFilter.OrderBy = KpiCriteriaItemOrder.Id;
            KpiCriteriaItemFilter.OrderType = OrderType.ASC;
            KpiCriteriaItemFilter.Selects = KpiCriteriaItemSelect.ALL;

            List<KpiCriteriaItem> KpiCriteriaItemes = await KpiCriteriaItemService.List(KpiCriteriaItemFilter);
            List<KpiItem_KpiCriteriaItemDTO> KpiItem_KpiCriteriaItemDTOs = KpiCriteriaItemes
                .Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            return KpiItem_KpiCriteriaItemDTOs;
        }

        [Route(KpiItemRoute.SingleListKpiCriteriaTotal), HttpPost]
        public async Task<List<KpiItem_KpiCriteriaTotalDTO>> SingleListKpiCriteriaTotal()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaTotalFilter KpiCriteriaTotalFilter = new KpiCriteriaTotalFilter();
            KpiCriteriaTotalFilter.Skip = 0;
            KpiCriteriaTotalFilter.Take = int.MaxValue;
            KpiCriteriaTotalFilter.Take = 20;
            KpiCriteriaTotalFilter.OrderBy = KpiCriteriaTotalOrder.Id;
            KpiCriteriaTotalFilter.OrderType = OrderType.ASC;
            KpiCriteriaTotalFilter.Selects = KpiCriteriaTotalSelect.ALL;

            List<KpiCriteriaTotal> KpiCriteriaTotales = await KpiCriteriaTotalService.List(KpiCriteriaTotalFilter);
            List<KpiItem_KpiCriteriaTotalDTO> KpiItem_KpiCriteriaTotalDTOs = KpiCriteriaTotales
                .Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            return KpiItem_KpiCriteriaTotalDTOs;
        }

        [Route(KpiItemRoute.SingleListItem), HttpPost]
        public async Task<List<KpiItem_ItemDTO>> SingleListItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = KpiItem_ItemFilterDTO.Id;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = KpiItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItem_ItemDTO> KpiItem_ItemDTOs = Items
                .Select(x => new KpiItem_ItemDTO(x)).ToList();
            return KpiItem_ItemDTOs;
        }

        [Route(KpiItemRoute.CountAppUser), HttpPost]
        public async Task<long> CountAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiItem_AppUserFilterDTO.Phone;
            AppUserFilter.OrganizationId = KpiItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            return await KpiItemService.CountAppUser(AppUserFilter, KpiItem_AppUserFilterDTO.KpiPeriodId);
        }
        [Route(KpiItemRoute.ListAppUser), HttpPost]
        public async Task<List<KpiItem_AppUserDTO>> ListAppUser([FromBody] KpiItem_AppUserFilterDTO KpiItem_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = KpiItem_AppUserFilterDTO.Skip;
            AppUserFilter.Take = KpiItem_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = KpiItem_AppUserFilterDTO.Email;
            AppUserFilter.OrganizationId = KpiItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Phone = KpiItem_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await KpiItemService.ListAppUser(AppUserFilter, KpiItem_AppUserFilterDTO.KpiPeriodId);
            List<KpiItem_AppUserDTO> KpiItem_AppUserDTOs = AppUsers
                .Select(x => new KpiItem_AppUserDTO(x)).ToList();
            return KpiItem_AppUserDTOs;
        }

        [Route(KpiItemRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ProductGroupingId = KpiItem_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = KpiItem_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = KpiItem_ItemFilterDTO.SupplierId;

            return await ItemService.Count(ItemFilter);
        }
        [Route(KpiItemRoute.ListItem), HttpPost]
        public async Task<List<KpiItem_ItemDTO>> ListItem([FromBody] KpiItem_ItemFilterDTO KpiItem_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = KpiItem_ItemFilterDTO.Skip;
            ItemFilter.Take = KpiItem_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Code = KpiItem_ItemFilterDTO.Code;
            ItemFilter.Name = KpiItem_ItemFilterDTO.Name;
            ItemFilter.ProductGroupingId = KpiItem_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = KpiItem_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = KpiItem_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = KpiItem_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = KpiItem_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = KpiItem_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = KpiItem_ItemFilterDTO.SupplierId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<KpiItem_ItemDTO> KpiItem_ItemDTOs = Items
                .Select(x => new KpiItem_ItemDTO(x)).ToList();
            return KpiItem_ItemDTOs;
        }
    }
}

