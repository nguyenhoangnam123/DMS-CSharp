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
using DMS.Services.MItemSpecificKpi;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItemSpecificKpiContent;
using DMS.Services.MItem;
using DMS.Services.MItemSpecificCriteria;
using DMS.Services.MTotalItemSpecificCriteria;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MSupplier;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpiController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IItemSpecificKpiContentService ItemSpecificKpiContentService;
        private IItemService ItemService;
        private IItemSpecificCriteriaService ItemSpecificCriteriaService;
        private ITotalItemSpecificCriteriaService TotalItemSpecificCriteriaService;
        private IItemSpecificKpiService ItemSpecificKpiService;
        private IProductGroupingService ProductGroupingService;
        private IProductTypeService ProductTypeService;
        private ISupplierService SupplierService;
        private ICurrentContext CurrentContext;
        public ItemSpecificKpiController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IItemSpecificKpiContentService ItemSpecificKpiContentService,
            IItemService ItemService,
            IItemSpecificCriteriaService ItemSpecificCriteriaService,
            ITotalItemSpecificCriteriaService TotalItemSpecificCriteriaService,
            IItemSpecificKpiService ItemSpecificKpiService,
            IProductGroupingService ProductGroupingService,
            IProductTypeService ProductTypeService,
            ISupplierService SupplierService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.ItemSpecificKpiContentService = ItemSpecificKpiContentService;
            this.ItemService = ItemService;
            this.ItemSpecificCriteriaService = ItemSpecificCriteriaService;
            this.TotalItemSpecificCriteriaService = TotalItemSpecificCriteriaService;
            this.ItemSpecificKpiService = ItemSpecificKpiService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductTypeService = ProductTypeService;
            this.SupplierService = SupplierService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ItemSpecificKpiRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ItemSpecificKpi_ItemSpecificKpiFilterDTO ItemSpecificKpi_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = ConvertFilterDTOToFilterEntity(ItemSpecificKpi_ItemSpecificKpiFilterDTO);
            ItemSpecificKpiFilter = ItemSpecificKpiService.ToFilter(ItemSpecificKpiFilter);
            int count = await ItemSpecificKpiService.Count(ItemSpecificKpiFilter);
            return count;
        }

        [Route(ItemSpecificKpiRoute.List), HttpPost]
        public async Task<ActionResult<List<ItemSpecificKpi_ItemSpecificKpiDTO>>> List([FromBody] ItemSpecificKpi_ItemSpecificKpiFilterDTO ItemSpecificKpi_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = ConvertFilterDTOToFilterEntity(ItemSpecificKpi_ItemSpecificKpiFilterDTO);
            ItemSpecificKpiFilter = ItemSpecificKpiService.ToFilter(ItemSpecificKpiFilter);
            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<ItemSpecificKpi_ItemSpecificKpiDTO> ItemSpecificKpi_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(c => new ItemSpecificKpi_ItemSpecificKpiDTO(c)).ToList();
            return ItemSpecificKpi_ItemSpecificKpiDTOs;
        }

        [Route(ItemSpecificKpiRoute.Get), HttpPost]
        public async Task<ActionResult<ItemSpecificKpi_ItemSpecificKpiDTO>> Get([FromBody]ItemSpecificKpi_ItemSpecificKpiDTO ItemSpecificKpi_ItemSpecificKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificKpi_ItemSpecificKpiDTO.Id))
                return Forbid();

            ItemSpecificKpi ItemSpecificKpi = await ItemSpecificKpiService.Get(ItemSpecificKpi_ItemSpecificKpiDTO.Id);
            return new ItemSpecificKpi_ItemSpecificKpiDTO(ItemSpecificKpi);
        }

        [Route(ItemSpecificKpiRoute.Create), HttpPost]
        public async Task<ActionResult<ItemSpecificKpi_ItemSpecificKpiDTO>> Create([FromBody] ItemSpecificKpi_ItemSpecificKpiDTO ItemSpecificKpi_ItemSpecificKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ItemSpecificKpi_ItemSpecificKpiDTO.Id))
                return Forbid();

            ItemSpecificKpi ItemSpecificKpi = ConvertDTOToEntity(ItemSpecificKpi_ItemSpecificKpiDTO);
            ItemSpecificKpi = await ItemSpecificKpiService.Create(ItemSpecificKpi);
            ItemSpecificKpi_ItemSpecificKpiDTO = new ItemSpecificKpi_ItemSpecificKpiDTO(ItemSpecificKpi);
            if (ItemSpecificKpi.IsValidated)
                return ItemSpecificKpi_ItemSpecificKpiDTO;
            else
                return BadRequest(ItemSpecificKpi_ItemSpecificKpiDTO);
        }

        [Route(ItemSpecificKpiRoute.CreateDraft), HttpPost]
        public async Task<ActionResult<ItemSpecificKpi_ItemSpecificKpiDTO>> CreateDraft([FromBody] ItemSpecificKpi_ItemSpecificKpiDTO ItemSpecificKpi_ItemSpecificKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificKpi_ItemSpecificKpiDTO.Id))
                return Forbid();

            List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(new ItemSpecificCriteriaFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSpecificCriteriaSelect.ALL
            });

            List<TotalItemSpecificCriteria> TotalItemSpecificCriteria = await TotalItemSpecificCriteriaService.List(new TotalItemSpecificCriteriaFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TotalItemSpecificCriteriaSelect.ALL
            });

            ItemSpecificKpi_ItemSpecificKpiDTO = new ItemSpecificKpi_ItemSpecificKpiDTO()
            {
                ItemSpecificKpiContents = new List<ItemSpecificKpi_ItemSpecificKpiContentDTO>(),
                ItemSpecificKpiTotalItemSpecificCriteriaMappings = new List<ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO>()
            };

            foreach (var ItemSpecificCriteria in ItemSpecificCriterias)
            {
                ItemSpecificKpi_ItemSpecificKpiDTO.ItemSpecificKpiContents.Add(new ItemSpecificKpi_ItemSpecificKpiContentDTO
                {
                    ItemSpecificCriteriaId = ItemSpecificCriteria.Id,
                    ItemSpecificCriteria = new ItemSpecificKpi_ItemSpecificCriteriaDTO(ItemSpecificCriteria)
                });
            }

            foreach (var TotalItemSpecificCriteri in TotalItemSpecificCriteria)
            {
                ItemSpecificKpi_ItemSpecificKpiDTO.ItemSpecificKpiTotalItemSpecificCriteriaMappings.Add(new ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO
                {
                    TotalItemSpecificCriteriaId = TotalItemSpecificCriteri.Id,
                    TotalItemSpecificCriteria = new ItemSpecificKpi_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteri)
                });
            }
            return ItemSpecificKpi_ItemSpecificKpiDTO;
        }

        [Route(ItemSpecificKpiRoute.Update), HttpPost]
        public async Task<ActionResult<ItemSpecificKpi_ItemSpecificKpiDTO>> Update([FromBody] ItemSpecificKpi_ItemSpecificKpiDTO ItemSpecificKpi_ItemSpecificKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ItemSpecificKpi_ItemSpecificKpiDTO.Id))
                return Forbid();

            ItemSpecificKpi ItemSpecificKpi = ConvertDTOToEntity(ItemSpecificKpi_ItemSpecificKpiDTO);
            ItemSpecificKpi = await ItemSpecificKpiService.Update(ItemSpecificKpi);
            ItemSpecificKpi_ItemSpecificKpiDTO = new ItemSpecificKpi_ItemSpecificKpiDTO(ItemSpecificKpi);
            if (ItemSpecificKpi.IsValidated)
                return ItemSpecificKpi_ItemSpecificKpiDTO;
            else
                return BadRequest(ItemSpecificKpi_ItemSpecificKpiDTO);
        }

        [Route(ItemSpecificKpiRoute.Delete), HttpPost]
        public async Task<ActionResult<ItemSpecificKpi_ItemSpecificKpiDTO>> Delete([FromBody] ItemSpecificKpi_ItemSpecificKpiDTO ItemSpecificKpi_ItemSpecificKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificKpi_ItemSpecificKpiDTO.Id))
                return Forbid();

            ItemSpecificKpi ItemSpecificKpi = ConvertDTOToEntity(ItemSpecificKpi_ItemSpecificKpiDTO);
            ItemSpecificKpi = await ItemSpecificKpiService.Delete(ItemSpecificKpi);
            ItemSpecificKpi_ItemSpecificKpiDTO = new ItemSpecificKpi_ItemSpecificKpiDTO(ItemSpecificKpi);
            if (ItemSpecificKpi.IsValidated)
                return ItemSpecificKpi_ItemSpecificKpiDTO;
            else
                return BadRequest(ItemSpecificKpi_ItemSpecificKpiDTO);
        }
        
        [Route(ItemSpecificKpiRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter = ItemSpecificKpiService.ToFilter(ItemSpecificKpiFilter);
            ItemSpecificKpiFilter.Id = new IdFilter { In = Ids };
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.Id;
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = int.MaxValue;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            ItemSpecificKpis = await ItemSpecificKpiService.BulkDelete(ItemSpecificKpis);
            return true;
        }
        
        [Route(ItemSpecificKpiRoute.Import), HttpPost]
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
            List<ItemSpecificKpi> ItemSpecificKpis = new List<ItemSpecificKpi>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ItemSpecificKpis);
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
                    
                    ItemSpecificKpi ItemSpecificKpi = new ItemSpecificKpi();
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    ItemSpecificKpi.CreatorId = Creator == null ? 0 : Creator.Id;
                    ItemSpecificKpi.Creator = Creator;
                    AppUser Employee = Employees.Where(x => x.Id.ToString() == EmployeeIdValue).FirstOrDefault();
                    ItemSpecificKpi.EmployeeId = Employee == null ? 0 : Employee.Id;
                    ItemSpecificKpi.Employee = Employee;
                    KpiPeriod KpiPeriod = KpiPeriods.Where(x => x.Id.ToString() == KpiPeriodIdValue).FirstOrDefault();
                    ItemSpecificKpi.KpiPeriodId = KpiPeriod == null ? 0 : KpiPeriod.Id;
                    ItemSpecificKpi.KpiPeriod = KpiPeriod;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    ItemSpecificKpi.StatusId = Status == null ? 0 : Status.Id;
                    ItemSpecificKpi.Status = Status;
                    
                    ItemSpecificKpis.Add(ItemSpecificKpi);
                }
            }
            ItemSpecificKpis = await ItemSpecificKpiService.Import(ItemSpecificKpis);
            if (ItemSpecificKpis.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ItemSpecificKpis.Count; i++)
                {
                    ItemSpecificKpi ItemSpecificKpi = ItemSpecificKpis[i];
                    if (!ItemSpecificKpi.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ItemSpecificKpi.Errors.ContainsKey(nameof(ItemSpecificKpi.Id)))
                            Error += ItemSpecificKpi.Errors[nameof(ItemSpecificKpi.Id)];
                        if (ItemSpecificKpi.Errors.ContainsKey(nameof(ItemSpecificKpi.OrganizationId)))
                            Error += ItemSpecificKpi.Errors[nameof(ItemSpecificKpi.OrganizationId)];
                        if (ItemSpecificKpi.Errors.ContainsKey(nameof(ItemSpecificKpi.KpiPeriodId)))
                            Error += ItemSpecificKpi.Errors[nameof(ItemSpecificKpi.KpiPeriodId)];
                        if (ItemSpecificKpi.Errors.ContainsKey(nameof(ItemSpecificKpi.StatusId)))
                            Error += ItemSpecificKpi.Errors[nameof(ItemSpecificKpi.StatusId)];
                        if (ItemSpecificKpi.Errors.ContainsKey(nameof(ItemSpecificKpi.EmployeeId)))
                            Error += ItemSpecificKpi.Errors[nameof(ItemSpecificKpi.EmployeeId)];
                        if (ItemSpecificKpi.Errors.ContainsKey(nameof(ItemSpecificKpi.CreatorId)))
                            Error += ItemSpecificKpi.Errors[nameof(ItemSpecificKpi.CreatorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ItemSpecificKpiRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] ItemSpecificKpi_ItemSpecificKpiFilterDTO ItemSpecificKpi_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ItemSpecificKpi
                var ItemSpecificKpiFilter = ConvertFilterDTOToFilterEntity(ItemSpecificKpi_ItemSpecificKpiFilterDTO);
                ItemSpecificKpiFilter.Skip = 0;
                ItemSpecificKpiFilter.Take = int.MaxValue;
                ItemSpecificKpiFilter = ItemSpecificKpiService.ToFilter(ItemSpecificKpiFilter);
                List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);

                var ItemSpecificKpiHeaders = new List<string[]>()
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
                List<object[]> ItemSpecificKpiData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpis.Count; i++)
                {
                    var ItemSpecificKpi = ItemSpecificKpis[i];
                    ItemSpecificKpiData.Add(new Object[]
                    {
                        ItemSpecificKpi.Id,
                        ItemSpecificKpi.OrganizationId,
                        ItemSpecificKpi.KpiPeriodId,
                        ItemSpecificKpi.StatusId,
                        ItemSpecificKpi.EmployeeId,
                        ItemSpecificKpi.CreatorId,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpi", ItemSpecificKpiHeaders, ItemSpecificKpiData);
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
                #region ItemSpecificKpiContent
                var ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
                ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
                ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContentOrder.Id;
                ItemSpecificKpiContentFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiContentFilter.Skip = 0;
                ItemSpecificKpiContentFilter.Take = int.MaxValue;
                List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);

                var ItemSpecificKpiContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ItemSpecificKpiId",
                        "ItemSpecificCriteriaId",
                        "ItemId",
                        "Value",
                    }
                };
                List<object[]> ItemSpecificKpiContentData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpiContents.Count; i++)
                {
                    var ItemSpecificKpiContent = ItemSpecificKpiContents[i];
                    ItemSpecificKpiContentData.Add(new Object[]
                    {
                        ItemSpecificKpiContent.Id,
                        ItemSpecificKpiContent.ItemSpecificKpiId,
                        ItemSpecificKpiContent.ItemSpecificCriteriaId,
                        ItemSpecificKpiContent.ItemId,
                        ItemSpecificKpiContent.Value,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpiContent", ItemSpecificKpiContentHeaders, ItemSpecificKpiContentData);
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
                #region ItemSpecificCriteria
                var ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
                ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
                ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteriaOrder.Id;
                ItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
                ItemSpecificCriteriaFilter.Skip = 0;
                ItemSpecificCriteriaFilter.Take = int.MaxValue;
                List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);

                var ItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < ItemSpecificCriterias.Count; i++)
                {
                    var ItemSpecificCriteria = ItemSpecificCriterias[i];
                    ItemSpecificCriteriaData.Add(new Object[]
                    {
                        ItemSpecificCriteria.Id,
                        ItemSpecificCriteria.Code,
                        ItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificCriteria", ItemSpecificCriteriaHeaders, ItemSpecificCriteriaData);
                #endregion
                #region TotalItemSpecificCriteria
                var TotalItemSpecificCriteriaFilter = new TotalItemSpecificCriteriaFilter();
                TotalItemSpecificCriteriaFilter.Selects = TotalItemSpecificCriteriaSelect.ALL;
                TotalItemSpecificCriteriaFilter.OrderBy = TotalItemSpecificCriteriaOrder.Id;
                TotalItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
                TotalItemSpecificCriteriaFilter.Skip = 0;
                TotalItemSpecificCriteriaFilter.Take = int.MaxValue;
                List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.List(TotalItemSpecificCriteriaFilter);

                var TotalItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> TotalItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < TotalItemSpecificCriterias.Count; i++)
                {
                    var TotalItemSpecificCriteria = TotalItemSpecificCriterias[i];
                    TotalItemSpecificCriteriaData.Add(new Object[]
                    {
                        TotalItemSpecificCriteria.Id,
                        TotalItemSpecificCriteria.Code,
                        TotalItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("TotalItemSpecificCriteria", TotalItemSpecificCriteriaHeaders, TotalItemSpecificCriteriaData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ItemSpecificKpi.xlsx");
        }

        [Route(ItemSpecificKpiRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] ItemSpecificKpi_ItemSpecificKpiFilterDTO ItemSpecificKpi_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ItemSpecificKpi
                var ItemSpecificKpiHeaders = new List<string[]>()
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
                List<object[]> ItemSpecificKpiData = new List<object[]>();
                excel.GenerateWorksheet("ItemSpecificKpi", ItemSpecificKpiHeaders, ItemSpecificKpiData);
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
                #region ItemSpecificKpiContent
                var ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
                ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
                ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContentOrder.Id;
                ItemSpecificKpiContentFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiContentFilter.Skip = 0;
                ItemSpecificKpiContentFilter.Take = int.MaxValue;
                List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);

                var ItemSpecificKpiContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ItemSpecificKpiId",
                        "ItemSpecificCriteriaId",
                        "ItemId",
                        "Value",
                    }
                };
                List<object[]> ItemSpecificKpiContentData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpiContents.Count; i++)
                {
                    var ItemSpecificKpiContent = ItemSpecificKpiContents[i];
                    ItemSpecificKpiContentData.Add(new Object[]
                    {
                        ItemSpecificKpiContent.Id,
                        ItemSpecificKpiContent.ItemSpecificKpiId,
                        ItemSpecificKpiContent.ItemSpecificCriteriaId,
                        ItemSpecificKpiContent.ItemId,
                        ItemSpecificKpiContent.Value,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpiContent", ItemSpecificKpiContentHeaders, ItemSpecificKpiContentData);
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
                #region ItemSpecificCriteria
                var ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
                ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
                ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteriaOrder.Id;
                ItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
                ItemSpecificCriteriaFilter.Skip = 0;
                ItemSpecificCriteriaFilter.Take = int.MaxValue;
                List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);

                var ItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < ItemSpecificCriterias.Count; i++)
                {
                    var ItemSpecificCriteria = ItemSpecificCriterias[i];
                    ItemSpecificCriteriaData.Add(new Object[]
                    {
                        ItemSpecificCriteria.Id,
                        ItemSpecificCriteria.Code,
                        ItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificCriteria", ItemSpecificCriteriaHeaders, ItemSpecificCriteriaData);
                #endregion
                #region TotalItemSpecificCriteria
                var TotalItemSpecificCriteriaFilter = new TotalItemSpecificCriteriaFilter();
                TotalItemSpecificCriteriaFilter.Selects = TotalItemSpecificCriteriaSelect.ALL;
                TotalItemSpecificCriteriaFilter.OrderBy = TotalItemSpecificCriteriaOrder.Id;
                TotalItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
                TotalItemSpecificCriteriaFilter.Skip = 0;
                TotalItemSpecificCriteriaFilter.Take = int.MaxValue;
                List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.List(TotalItemSpecificCriteriaFilter);

                var TotalItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> TotalItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < TotalItemSpecificCriterias.Count; i++)
                {
                    var TotalItemSpecificCriteria = TotalItemSpecificCriterias[i];
                    TotalItemSpecificCriteriaData.Add(new Object[]
                    {
                        TotalItemSpecificCriteria.Id,
                        TotalItemSpecificCriteria.Code,
                        TotalItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("TotalItemSpecificCriteria", TotalItemSpecificCriteriaHeaders, TotalItemSpecificCriteriaData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ItemSpecificKpi.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter = ItemSpecificKpiService.ToFilter(ItemSpecificKpiFilter);
            if (Id == 0)
            {

            }
            else
            {
                ItemSpecificKpiFilter.Id = new IdFilter { Equal = Id };
                int count = await ItemSpecificKpiService.Count(ItemSpecificKpiFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ItemSpecificKpi ConvertDTOToEntity(ItemSpecificKpi_ItemSpecificKpiDTO ItemSpecificKpi_ItemSpecificKpiDTO)
        {
            ItemSpecificKpi ItemSpecificKpi = new ItemSpecificKpi();
            ItemSpecificKpi.Id = ItemSpecificKpi_ItemSpecificKpiDTO.Id;
            ItemSpecificKpi.OrganizationId = ItemSpecificKpi_ItemSpecificKpiDTO.OrganizationId;
            ItemSpecificKpi.KpiPeriodId = ItemSpecificKpi_ItemSpecificKpiDTO.KpiPeriodId;
            ItemSpecificKpi.StatusId = ItemSpecificKpi_ItemSpecificKpiDTO.StatusId;
            ItemSpecificKpi.EmployeeId = ItemSpecificKpi_ItemSpecificKpiDTO.EmployeeId;
            ItemSpecificKpi.CreatorId = ItemSpecificKpi_ItemSpecificKpiDTO.CreatorId;
            ItemSpecificKpi.Creator = ItemSpecificKpi_ItemSpecificKpiDTO.Creator == null ? null : new AppUser
            {
                Id = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Id,
                Username = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Username,
                DisplayName = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.DisplayName,
                Address = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Address,
                Email = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Email,
                Phone = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Phone,
                PositionId = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.PositionId,
                Department = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Department,
                OrganizationId = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.OrganizationId,
                StatusId = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.StatusId,
                Avatar = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Avatar,
                ProvinceId = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.ProvinceId,
                SexId = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.SexId,
                Birthday = ItemSpecificKpi_ItemSpecificKpiDTO.Creator.Birthday,
            };
            ItemSpecificKpi.Employee = ItemSpecificKpi_ItemSpecificKpiDTO.Employee == null ? null : new AppUser
            {
                Id = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Id,
                Username = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Username,
                DisplayName = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.DisplayName,
                Address = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Address,
                Email = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Email,
                Phone = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Phone,
                PositionId = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.PositionId,
                Department = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Department,
                OrganizationId = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.OrganizationId,
                StatusId = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.StatusId,
                Avatar = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Avatar,
                ProvinceId = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.ProvinceId,
                SexId = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.SexId,
                Birthday = ItemSpecificKpi_ItemSpecificKpiDTO.Employee.Birthday,
            };
            ItemSpecificKpi.KpiPeriod = ItemSpecificKpi_ItemSpecificKpiDTO.KpiPeriod == null ? null : new KpiPeriod
            {
                Id = ItemSpecificKpi_ItemSpecificKpiDTO.KpiPeriod.Id,
                Code = ItemSpecificKpi_ItemSpecificKpiDTO.KpiPeriod.Code,
                Name = ItemSpecificKpi_ItemSpecificKpiDTO.KpiPeriod.Name,
            };
            ItemSpecificKpi.Organization = ItemSpecificKpi_ItemSpecificKpiDTO.Organization == null ? null : new Organization
            {
                Id = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Id,
                Code = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Code,
                Name = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Name,
                ParentId = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.ParentId,
                Path = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Path,
                Level = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Level,
                StatusId = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.StatusId,
                Phone = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Phone,
                Email = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Email,
                Address = ItemSpecificKpi_ItemSpecificKpiDTO.Organization.Address,
            };
            ItemSpecificKpi.Status = ItemSpecificKpi_ItemSpecificKpiDTO.Status == null ? null : new Status
            {
                Id = ItemSpecificKpi_ItemSpecificKpiDTO.Status.Id,
                Code = ItemSpecificKpi_ItemSpecificKpiDTO.Status.Code,
                Name = ItemSpecificKpi_ItemSpecificKpiDTO.Status.Name,
            };
            ItemSpecificKpi.EmployeeIds = ItemSpecificKpi_ItemSpecificKpiDTO.EmployeeIds;
            ItemSpecificKpi.ItemSpecificKpiContents = ItemSpecificKpi_ItemSpecificKpiDTO.ItemSpecificKpiContents?
                .Select(x => new ItemSpecificKpiContent
                {
                    Id = x.Id,
                    ItemSpecificCriteriaId = x.ItemSpecificCriteriaId,
                    ItemId = x.ItemId,
                    Value = x.Value,
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
                    ItemSpecificCriteria = x.ItemSpecificCriteria == null ? null : new ItemSpecificCriteria
                    {
                        Id = x.ItemSpecificCriteria.Id,
                        Code = x.ItemSpecificCriteria.Code,
                        Name = x.ItemSpecificCriteria.Name,
                    },
                }).ToList();
            ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings = ItemSpecificKpi_ItemSpecificKpiDTO.ItemSpecificKpiTotalItemSpecificCriteriaMappings?
                .Select(x => new ItemSpecificKpiTotalItemSpecificCriteriaMapping
                {
                    TotalItemSpecificCriteriaId = x.TotalItemSpecificCriteriaId,
                    Value = x.Value,
                    TotalItemSpecificCriteria = x.TotalItemSpecificCriteria == null ? null : new TotalItemSpecificCriteria
                    {
                        Id = x.TotalItemSpecificCriteria.Id,
                        Code = x.TotalItemSpecificCriteria.Code,
                        Name = x.TotalItemSpecificCriteria.Name,
                    },
                }).ToList();
            ItemSpecificKpi.BaseLanguage = CurrentContext.Language;
            return ItemSpecificKpi;
        }

        private ItemSpecificKpiFilter ConvertFilterDTOToFilterEntity(ItemSpecificKpi_ItemSpecificKpiFilterDTO ItemSpecificKpi_ItemSpecificKpiFilterDTO)
        {
            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Skip = ItemSpecificKpi_ItemSpecificKpiFilterDTO.Skip;
            ItemSpecificKpiFilter.Take = ItemSpecificKpi_ItemSpecificKpiFilterDTO.Take;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpi_ItemSpecificKpiFilterDTO.OrderBy;
            ItemSpecificKpiFilter.OrderType = ItemSpecificKpi_ItemSpecificKpiFilterDTO.OrderType;

            ItemSpecificKpiFilter.Id = ItemSpecificKpi_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = ItemSpecificKpi_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = ItemSpecificKpi_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = ItemSpecificKpi_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = ItemSpecificKpi_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = ItemSpecificKpi_ItemSpecificKpiFilterDTO.CreatorId;
            ItemSpecificKpiFilter.CreatedAt = ItemSpecificKpi_ItemSpecificKpiFilterDTO.CreatedAt;
            ItemSpecificKpiFilter.UpdatedAt = ItemSpecificKpi_ItemSpecificKpiFilterDTO.UpdatedAt;
            return ItemSpecificKpiFilter;
        }

        [Route(ItemSpecificKpiRoute.FilterListAppUser), HttpPost]
        public async Task<List<ItemSpecificKpi_AppUserDTO>> FilterListAppUser([FromBody] ItemSpecificKpi_AppUserFilterDTO ItemSpecificKpi_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ItemSpecificKpi_AppUserFilterDTO.Id;
            AppUserFilter.Username = ItemSpecificKpi_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ItemSpecificKpi_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ItemSpecificKpi_AppUserFilterDTO.Address;
            AppUserFilter.Email = ItemSpecificKpi_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ItemSpecificKpi_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = ItemSpecificKpi_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = ItemSpecificKpi_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = ItemSpecificKpi_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = ItemSpecificKpi_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = ItemSpecificKpi_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = ItemSpecificKpi_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = ItemSpecificKpi_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ItemSpecificKpi_AppUserDTO> ItemSpecificKpi_AppUserDTOs = AppUsers
                .Select(x => new ItemSpecificKpi_AppUserDTO(x)).ToList();
            return ItemSpecificKpi_AppUserDTOs;
        }
        [Route(ItemSpecificKpiRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<ItemSpecificKpi_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] ItemSpecificKpi_KpiPeriodFilterDTO ItemSpecificKpi_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = ItemSpecificKpi_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = ItemSpecificKpi_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = ItemSpecificKpi_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<ItemSpecificKpi_KpiPeriodDTO> ItemSpecificKpi_KpiPeriodDTOs = KpiPeriods
                .Select(x => new ItemSpecificKpi_KpiPeriodDTO(x)).ToList();
            return ItemSpecificKpi_KpiPeriodDTOs;
        }
        [Route(ItemSpecificKpiRoute.FilterListOrganization), HttpPost]
        public async Task<List<ItemSpecificKpi_OrganizationDTO>> FilterListOrganization([FromBody] ItemSpecificKpi_OrganizationFilterDTO ItemSpecificKpi_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ItemSpecificKpi_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ItemSpecificKpi_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ItemSpecificKpi_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ItemSpecificKpi_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ItemSpecificKpi_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ItemSpecificKpi_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = ItemSpecificKpi_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = ItemSpecificKpi_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = ItemSpecificKpi_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = ItemSpecificKpi_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ItemSpecificKpi_OrganizationDTO> ItemSpecificKpi_OrganizationDTOs = Organizations
                .Select(x => new ItemSpecificKpi_OrganizationDTO(x)).ToList();
            return ItemSpecificKpi_OrganizationDTOs;
        }
        [Route(ItemSpecificKpiRoute.FilterListStatus), HttpPost]
        public async Task<List<ItemSpecificKpi_StatusDTO>> FilterListStatus([FromBody] ItemSpecificKpi_StatusFilterDTO ItemSpecificKpi_StatusFilterDTO)
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
            List<ItemSpecificKpi_StatusDTO> ItemSpecificKpi_StatusDTOs = Statuses
                .Select(x => new ItemSpecificKpi_StatusDTO(x)).ToList();
            return ItemSpecificKpi_StatusDTOs;
        }
        [Route(ItemSpecificKpiRoute.SingleListAppUser), HttpPost]
        public async Task<List<ItemSpecificKpi_AppUserDTO>> SingleListAppUser([FromBody] ItemSpecificKpi_AppUserFilterDTO ItemSpecificKpi_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ItemSpecificKpi_AppUserFilterDTO.Id;
            AppUserFilter.Username = ItemSpecificKpi_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ItemSpecificKpi_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ItemSpecificKpi_AppUserFilterDTO.Address;
            AppUserFilter.Email = ItemSpecificKpi_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ItemSpecificKpi_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = ItemSpecificKpi_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = ItemSpecificKpi_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = ItemSpecificKpi_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            AppUserFilter.ProvinceId = ItemSpecificKpi_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = ItemSpecificKpi_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = ItemSpecificKpi_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ItemSpecificKpi_AppUserDTO> ItemSpecificKpi_AppUserDTOs = AppUsers
                .Select(x => new ItemSpecificKpi_AppUserDTO(x)).ToList();
            return ItemSpecificKpi_AppUserDTOs;
        }
        [Route(ItemSpecificKpiRoute.SingleListKpiPeriod), HttpPost]
        public async Task<List<ItemSpecificKpi_KpiPeriodDTO>> SingleListKpiPeriod([FromBody] ItemSpecificKpi_KpiPeriodFilterDTO ItemSpecificKpi_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = ItemSpecificKpi_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = ItemSpecificKpi_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = ItemSpecificKpi_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<ItemSpecificKpi_KpiPeriodDTO> ItemSpecificKpi_KpiPeriodDTOs = KpiPeriods
                .Select(x => new ItemSpecificKpi_KpiPeriodDTO(x)).ToList();
            return ItemSpecificKpi_KpiPeriodDTOs;
        }
        [Route(ItemSpecificKpiRoute.SingleListOrganization), HttpPost]
        public async Task<List<ItemSpecificKpi_OrganizationDTO>> SingleListOrganization([FromBody] ItemSpecificKpi_OrganizationFilterDTO ItemSpecificKpi_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ItemSpecificKpi_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ItemSpecificKpi_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ItemSpecificKpi_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ItemSpecificKpi_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ItemSpecificKpi_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ItemSpecificKpi_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = ItemSpecificKpi_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = ItemSpecificKpi_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = ItemSpecificKpi_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ItemSpecificKpi_OrganizationDTO> ItemSpecificKpi_OrganizationDTOs = Organizations
                .Select(x => new ItemSpecificKpi_OrganizationDTO(x)).ToList();
            return ItemSpecificKpi_OrganizationDTOs;
        }

        [Route(ItemSpecificKpiRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<ItemSpecificKpi_ProductGroupingDTO>> SingleListProductGrouping([FromBody] ItemSpecificKpi_ProductGroupingFilterDTO ItemSpecificKpi_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ItemSpecificKpiGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ItemSpecificKpi_ProductGroupingDTO> ItemSpecificKpi_ProductGroupingDTOs = ItemSpecificKpiGroupings
                .Select(x => new ItemSpecificKpi_ProductGroupingDTO(x)).ToList();
            return ItemSpecificKpi_ProductGroupingDTOs;
        }
        [Route(ItemSpecificKpiRoute.SingleListProductType), HttpPost]
        public async Task<List<ItemSpecificKpi_ProductTypeDTO>> SingleListProductType([FromBody] ItemSpecificKpi_ProductTypeFilterDTO ItemSpecificKpi_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = ItemSpecificKpi_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = ItemSpecificKpi_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = ItemSpecificKpi_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = ItemSpecificKpi_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<ItemSpecificKpi_ProductTypeDTO> ItemSpecificKpi_ProductTypeDTOs = ProductTypes
                .Select(x => new ItemSpecificKpi_ProductTypeDTO(x)).ToList();
            return ItemSpecificKpi_ProductTypeDTOs;
        }

        [Route(ItemSpecificKpiRoute.SingleListSupplier), HttpPost]
        public async Task<List<ItemSpecificKpi_SupplierDTO>> SingleListSupplier([FromBody] ItemSpecificKpi_SupplierFilterDTO Product_SupplierFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = Product_SupplierFilterDTO.Id;
            SupplierFilter.Code = Product_SupplierFilterDTO.Code;
            SupplierFilter.Name = Product_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Product_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<ItemSpecificKpi_SupplierDTO> ItemSpecificKpi_SupplierDTOs = Suppliers
                .Select(x => new ItemSpecificKpi_SupplierDTO(x)).ToList();
            return ItemSpecificKpi_SupplierDTOs;
        }

        [Route(ItemSpecificKpiRoute.SingleListStatus), HttpPost]
        public async Task<List<ItemSpecificKpi_StatusDTO>> SingleListStatus([FromBody] ItemSpecificKpi_StatusFilterDTO ItemSpecificKpi_StatusFilterDTO)
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
            List<ItemSpecificKpi_StatusDTO> ItemSpecificKpi_StatusDTOs = Statuses
                .Select(x => new ItemSpecificKpi_StatusDTO(x)).ToList();
            return ItemSpecificKpi_StatusDTOs;
        }

        [Route(ItemSpecificKpiRoute.CountAppUser), HttpPost]
        public async Task<long> CountAppUser([FromBody] ItemSpecificKpi_AppUserFilterDTO ItemSpecificKpi_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = ItemSpecificKpi_AppUserFilterDTO.Id;
            AppUserFilter.Username = ItemSpecificKpi_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ItemSpecificKpi_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = ItemSpecificKpi_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ItemSpecificKpi_AppUserFilterDTO.Phone;
            AppUserFilter.OrganizationId = ItemSpecificKpi_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            return await AppUserService.Count(AppUserFilter);
        }
        [Route(ItemSpecificKpiRoute.ListAppUser), HttpPost]
        public async Task<List<ItemSpecificKpi_AppUserDTO>> ListAppUser([FromBody] ItemSpecificKpi_AppUserFilterDTO ItemSpecificKpi_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = ItemSpecificKpi_AppUserFilterDTO.Skip;
            AppUserFilter.Take = ItemSpecificKpi_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ItemSpecificKpi_AppUserFilterDTO.Id;
            AppUserFilter.Username = ItemSpecificKpi_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ItemSpecificKpi_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = ItemSpecificKpi_AppUserFilterDTO.Email;
            AppUserFilter.OrganizationId = ItemSpecificKpi_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Phone = ItemSpecificKpi_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ItemSpecificKpi_AppUserDTO> ItemSpecificKpi_AppUserDTOs = AppUsers
                .Select(x => new ItemSpecificKpi_AppUserDTO(x)).ToList();
            return ItemSpecificKpi_AppUserDTOs;
        }

        [Route(ItemSpecificKpiRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] ItemSpecificKpi_ItemFilterDTO ItemSpecificKpi_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = ItemSpecificKpi_ItemFilterDTO.Id;
            ItemFilter.Code = ItemSpecificKpi_ItemFilterDTO.Code;
            ItemFilter.Name = ItemSpecificKpi_ItemFilterDTO.Name;
            ItemFilter.ProductGroupingId = ItemSpecificKpi_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = ItemSpecificKpi_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = ItemSpecificKpi_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = ItemSpecificKpi_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = ItemSpecificKpi_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = ItemSpecificKpi_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = ItemSpecificKpi_ItemFilterDTO.SupplierId;
            ItemFilter.IsNew = ItemSpecificKpi_ItemFilterDTO.IsNew;
            return await ItemService.Count(ItemFilter);
        }

        [Route(ItemSpecificKpiRoute.ListItem), HttpPost]
        public async Task<List<ItemSpecificKpi_ItemDTO>> ListItem([FromBody] ItemSpecificKpi_ItemFilterDTO ItemSpecificKpi_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = ItemSpecificKpi_ItemFilterDTO.Skip;
            ItemFilter.Take = ItemSpecificKpi_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ItemSpecificKpi_ItemFilterDTO.Id;
            ItemFilter.Code = ItemSpecificKpi_ItemFilterDTO.Code;
            ItemFilter.Name = ItemSpecificKpi_ItemFilterDTO.Name;
            ItemFilter.ProductGroupingId = ItemSpecificKpi_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = ItemSpecificKpi_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = ItemSpecificKpi_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = ItemSpecificKpi_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = ItemSpecificKpi_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = ItemSpecificKpi_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = ItemSpecificKpi_ItemFilterDTO.SupplierId;
            ItemFilter.IsNew = ItemSpecificKpi_ItemFilterDTO.IsNew;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ItemSpecificKpi_ItemDTO> ItemSpecificKpi_ItemDTOs = Items
                .Select(x => new ItemSpecificKpi_ItemDTO(x)).ToList();
            return ItemSpecificKpi_ItemDTOs;
        }
    }
}

