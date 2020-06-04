using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MKpiItem;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItem;
using DMS.Services.MKpiCriteriaItem;
using DMS.Services.MKpiCriteriaTotal;

namespace DMS.Rpc.kpi_item
{
    public class KpiItemController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IOrganizationService OrganizationService;
        private IKpiCriteriaItemService KpiCriteriaItemService;
        private IKpiCriteriaTotalService KpiCriteriaTotalService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IKpiItemService KpiItemService;
        private ICurrentContext CurrentContext;
        public KpiItemController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IOrganizationService OrganizationService,
            IKpiCriteriaItemService KpiCriteriaItemService,
            IKpiCriteriaTotalService KpiCriteriaTotalService,
            IStatusService StatusService,
            IItemService ItemService,
            IKpiItemService KpiItemService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.OrganizationService = OrganizationService;
            this.KpiCriteriaItemService = KpiCriteriaItemService;
            this.KpiCriteriaTotalService = KpiCriteriaTotalService;
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.KpiItemService = KpiItemService;
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
        public async Task<ActionResult<KpiItem_KpiItemDTO>> GetDraft([FromBody]KpiItem_KpiItemDTO KpiItem_KpiItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

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
            KpiItem_KpiItemDTO = new KpiItem_KpiItemDTO();
            KpiItem_KpiItemDTO.KpiCriteriaItems = KpiCriteriaItems.Select(x => new KpiItem_KpiCriteriaItemDTO(x)).ToList();
            KpiItem_KpiItemDTO.KpiCriteriaTotals = KpiCriteriaTotals.Select(x => new KpiItem_KpiCriteriaTotalDTO(x)).ToList();
            KpiItem_KpiItemDTO.KpiItemKpiCriteriaTotalMappings = KpiCriteriaTotals.ToDictionary(x => x.Id, y => 0L);
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
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                    },
                    KpiItemContentKpiCriteriaItemMappings = x.KpiItemContentKpiCriteriaItemMappings.Select(p => new KpiItemContentKpiCriteriaItemMapping
                    {
                        KpiCriteriaItemId = p.Key,
                        Value = p.Value,
                    }).ToList(),
                }).ToList();
            KpiItem.KpiItemKpiCriteriaTotalMappings = KpiItem_KpiItemDTO.KpiItemKpiCriteriaTotalMappings.Select(p => new KpiItemKpiCriteriaTotalMapping
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

    }
}

