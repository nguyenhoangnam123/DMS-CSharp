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
using DMS.Services.MPriceList;
using DMS.Services.MOrganization;
using DMS.Services.MPriceListType;
using DMS.Services.MStatus;
using DMS.Services.MItem;
using DMS.Services.MStore;
using DMS.Services.MStoreType;

namespace DMS.Rpc.price_list
{
    public class PriceListRoute : Root
    {
        public const string Master = Module + "/price-list/price-list-master";
        public const string Detail = Module + "/price-list/price-list-detail";
        private const string Default = Rpc + Module + "/price-list";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";
        
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        
        public const string FilterListPriceListType = Default + "/filter-list-price-list-type";
        
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string FilterListItem = Default + "/filter-list-item";
        
        public const string FilterListStore = Default + "/filter-list-store";
        
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        

        
        public const string SingleListOrganization = Default + "/single-list-organization";
        
        public const string SingleListPriceListType = Default + "/single-list-price-list-type";
        
        public const string SingleListStatus = Default + "/single-list-status";
        
        public const string SingleListItem = Default + "/single-list-item";
        
        public const string SingleListStore = Default + "/single-list-store";
        
        public const string SingleListStoreType = Default + "/single-list-store-type";
        
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountStoreType = Default + "/count-store-type";
        public const string ListStoreType = Default + "/list-store-type";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(PriceListFilter.Id), FieldType.ID },
            { nameof(PriceListFilter.Code), FieldType.STRING },
            { nameof(PriceListFilter.Name), FieldType.STRING },
            { nameof(PriceListFilter.StatusId), FieldType.ID },
            { nameof(PriceListFilter.OrganizationId), FieldType.ID },
            { nameof(PriceListFilter.PriceListTypeId), FieldType.ID },
        };
    }

    public class PriceListController : RpcController
    {
        private IOrganizationService OrganizationService;
        private IPriceListTypeService PriceListTypeService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IPriceListService PriceListService;
        private ICurrentContext CurrentContext;
        public PriceListController(
            IOrganizationService OrganizationService,
            IPriceListTypeService PriceListTypeService,
            IStatusService StatusService,
            IItemService ItemService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IPriceListService PriceListService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.PriceListTypeService = PriceListTypeService;
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.PriceListService = PriceListService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PriceListRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = PriceListService.ToFilter(PriceListFilter);
            int count = await PriceListService.Count(PriceListFilter);
            return count;
        }

        [Route(PriceListRoute.List), HttpPost]
        public async Task<ActionResult<List<PriceList_PriceListDTO>>> List([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = PriceListService.ToFilter(PriceListFilter);
            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            List<PriceList_PriceListDTO> PriceList_PriceListDTOs = PriceLists
                .Select(c => new PriceList_PriceListDTO(c)).ToList();
            return PriceList_PriceListDTOs;
        }

        [Route(PriceListRoute.Get), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Get([FromBody]PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = await PriceListService.Get(PriceList_PriceListDTO.Id);
            return new PriceList_PriceListDTO(PriceList);
        }

        [Route(PriceListRoute.Create), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Create([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Create(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.Update), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Update([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Update(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }

        [Route(PriceListRoute.Delete), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Delete([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceList_PriceListDTO.Id))
                return Forbid();

            PriceList PriceList = ConvertDTOToEntity(PriceList_PriceListDTO);
            PriceList = await PriceListService.Delete(PriceList);
            PriceList_PriceListDTO = new PriceList_PriceListDTO(PriceList);
            if (PriceList.IsValidated)
                return PriceList_PriceListDTO;
            else
                return BadRequest(PriceList_PriceListDTO);
        }
        
        [Route(PriceListRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = PriceListService.ToFilter(PriceListFilter);
            PriceListFilter.Id = new IdFilter { In = Ids };
            PriceListFilter.Selects = PriceListSelect.Id;
            PriceListFilter.Skip = 0;
            PriceListFilter.Take = int.MaxValue;

            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            PriceLists = await PriceListService.BulkDelete(PriceLists);
            return true;
        }
        
        [Route(PriceListRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            PriceListTypeFilter PriceListTypeFilter = new PriceListTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListTypeSelect.ALL
            };
            List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<PriceList> PriceLists = new List<PriceList>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(PriceLists);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                int OrganizationIdColumn = 4 + StartColumn;
                int PriceListTypeIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string PriceListTypeIdValue = worksheet.Cells[i + StartRow, PriceListTypeIdColumn].Value?.ToString();
                    
                    PriceList PriceList = new PriceList();
                    PriceList.Code = CodeValue;
                    PriceList.Name = NameValue;
                    PriceListType PriceListType = PriceListTypes.Where(x => x.Id.ToString() == PriceListTypeIdValue).FirstOrDefault();
                    PriceList.PriceListTypeId = PriceListType == null ? 0 : PriceListType.Id;
                    PriceList.PriceListType = PriceListType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    PriceList.StatusId = Status == null ? 0 : Status.Id;
                    PriceList.Status = Status;
                    
                    PriceLists.Add(PriceList);
                }
            }
            PriceLists = await PriceListService.Import(PriceLists);
            if (PriceLists.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < PriceLists.Count; i++)
                {
                    PriceList PriceList = PriceLists[i];
                    if (!PriceList.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.Id)))
                            Error += PriceList.Errors[nameof(PriceList.Id)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.Code)))
                            Error += PriceList.Errors[nameof(PriceList.Code)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.Name)))
                            Error += PriceList.Errors[nameof(PriceList.Name)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.StatusId)))
                            Error += PriceList.Errors[nameof(PriceList.StatusId)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.OrganizationId)))
                            Error += PriceList.Errors[nameof(PriceList.OrganizationId)];
                        if (PriceList.Errors.ContainsKey(nameof(PriceList.PriceListTypeId)))
                            Error += PriceList.Errors[nameof(PriceList.PriceListTypeId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(PriceListRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region PriceList
                var PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
                PriceListFilter.Skip = 0;
                PriceListFilter.Take = int.MaxValue;
                PriceListFilter = PriceListService.ToFilter(PriceListFilter);
                List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);

                var PriceListHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "OrganizationId",
                        "PriceListTypeId",
                    }
                };
                List<object[]> PriceListData = new List<object[]>();
                for (int i = 0; i < PriceLists.Count; i++)
                {
                    var PriceList = PriceLists[i];
                    PriceListData.Add(new Object[]
                    {
                        PriceList.Id,
                        PriceList.Code,
                        PriceList.Name,
                        PriceList.StatusId,
                        PriceList.OrganizationId,
                        PriceList.PriceListTypeId,
                    });
                }
                excel.GenerateWorksheet("PriceList", PriceListHeaders, PriceListData);
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
                #region PriceListType
                var PriceListTypeFilter = new PriceListTypeFilter();
                PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;
                PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
                PriceListTypeFilter.OrderType = OrderType.ASC;
                PriceListTypeFilter.Skip = 0;
                PriceListTypeFilter.Take = int.MaxValue;
                List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);

                var PriceListTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PriceListTypeData = new List<object[]>();
                for (int i = 0; i < PriceListTypes.Count; i++)
                {
                    var PriceListType = PriceListTypes[i];
                    PriceListTypeData.Add(new Object[]
                    {
                        PriceListType.Id,
                        PriceListType.Code,
                        PriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("PriceListType", PriceListTypeHeaders, PriceListTypeData);
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
                #region Store
                var StoreFilter = new StoreFilter();
                StoreFilter.Selects = StoreSelect.ALL;
                StoreFilter.OrderBy = StoreOrder.Id;
                StoreFilter.OrderType = OrderType.ASC;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                List<Store> Stores = await StoreService.List(StoreFilter);

                var StoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentStoreId",
                        "OrganizationId",
                        "StoreTypeId",
                        "StoreGroupingId",
                        "ResellerId",
                        "Telephone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Address",
                        "DeliveryAddress",
                        "Latitude",
                        "Longitude",
                        "DeliveryLatitude",
                        "DeliveryLongitude",
                        "OwnerName",
                        "OwnerPhone",
                        "OwnerEmail",
                        "StatusId",
                        "WorkflowDefinitionId",
                        "RequestStateId",
                    }
                };
                List<object[]> StoreData = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    StoreData.Add(new Object[]
                    {
                        Store.Id,
                        Store.Code,
                        Store.Name,
                        Store.ParentStoreId,
                        Store.OrganizationId,
                        Store.StoreTypeId,
                        Store.StoreGroupingId,
                        Store.ResellerId,
                        Store.Telephone,
                        Store.ProvinceId,
                        Store.DistrictId,
                        Store.WardId,
                        Store.Address,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.StatusId,
                        Store.WorkflowDefinitionId,
                        Store.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreType
                var StoreTypeFilter = new StoreTypeFilter();
                StoreTypeFilter.Selects = StoreTypeSelect.ALL;
                StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
                StoreTypeFilter.OrderType = OrderType.ASC;
                StoreTypeFilter.Skip = 0;
                StoreTypeFilter.Take = int.MaxValue;
                List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);

                var StoreTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                    }
                };
                List<object[]> StoreTypeData = new List<object[]>();
                for (int i = 0; i < StoreTypes.Count; i++)
                {
                    var StoreType = StoreTypes[i];
                    StoreTypeData.Add(new Object[]
                    {
                        StoreType.Id,
                        StoreType.Code,
                        StoreType.Name,
                        StoreType.StatusId,
                    });
                }
                excel.GenerateWorksheet("StoreType", StoreTypeHeaders, StoreTypeData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "PriceList.xlsx");
        }

        [Route(PriceListRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region PriceList
                var PriceListHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "OrganizationId",
                        "PriceListTypeId",
                    }
                };
                List<object[]> PriceListData = new List<object[]>();
                excel.GenerateWorksheet("PriceList", PriceListHeaders, PriceListData);
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
                #region PriceListType
                var PriceListTypeFilter = new PriceListTypeFilter();
                PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;
                PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
                PriceListTypeFilter.OrderType = OrderType.ASC;
                PriceListTypeFilter.Skip = 0;
                PriceListTypeFilter.Take = int.MaxValue;
                List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);

                var PriceListTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> PriceListTypeData = new List<object[]>();
                for (int i = 0; i < PriceListTypes.Count; i++)
                {
                    var PriceListType = PriceListTypes[i];
                    PriceListTypeData.Add(new Object[]
                    {
                        PriceListType.Id,
                        PriceListType.Code,
                        PriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("PriceListType", PriceListTypeHeaders, PriceListTypeData);
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
                #region Store
                var StoreFilter = new StoreFilter();
                StoreFilter.Selects = StoreSelect.ALL;
                StoreFilter.OrderBy = StoreOrder.Id;
                StoreFilter.OrderType = OrderType.ASC;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                List<Store> Stores = await StoreService.List(StoreFilter);

                var StoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentStoreId",
                        "OrganizationId",
                        "StoreTypeId",
                        "StoreGroupingId",
                        "ResellerId",
                        "Telephone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Address",
                        "DeliveryAddress",
                        "Latitude",
                        "Longitude",
                        "DeliveryLatitude",
                        "DeliveryLongitude",
                        "OwnerName",
                        "OwnerPhone",
                        "OwnerEmail",
                        "StatusId",
                        "WorkflowDefinitionId",
                        "RequestStateId",
                    }
                };
                List<object[]> StoreData = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    StoreData.Add(new Object[]
                    {
                        Store.Id,
                        Store.Code,
                        Store.Name,
                        Store.ParentStoreId,
                        Store.OrganizationId,
                        Store.StoreTypeId,
                        Store.StoreGroupingId,
                        Store.ResellerId,
                        Store.Telephone,
                        Store.ProvinceId,
                        Store.DistrictId,
                        Store.WardId,
                        Store.Address,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.StatusId,
                        Store.WorkflowDefinitionId,
                        Store.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region StoreType
                var StoreTypeFilter = new StoreTypeFilter();
                StoreTypeFilter.Selects = StoreTypeSelect.ALL;
                StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
                StoreTypeFilter.OrderType = OrderType.ASC;
                StoreTypeFilter.Skip = 0;
                StoreTypeFilter.Take = int.MaxValue;
                List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);

                var StoreTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                    }
                };
                List<object[]> StoreTypeData = new List<object[]>();
                for (int i = 0; i < StoreTypes.Count; i++)
                {
                    var StoreType = StoreTypes[i];
                    StoreTypeData.Add(new Object[]
                    {
                        StoreType.Id,
                        StoreType.Code,
                        StoreType.Name,
                        StoreType.StatusId,
                    });
                }
                excel.GenerateWorksheet("StoreType", StoreTypeHeaders, StoreTypeData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "PriceList.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = PriceListService.ToFilter(PriceListFilter);
            if (Id == 0)
            {

            }
            else
            {
                PriceListFilter.Id = new IdFilter { Equal = Id };
                int count = await PriceListService.Count(PriceListFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private PriceList ConvertDTOToEntity(PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            PriceList PriceList = new PriceList();
            PriceList.Id = PriceList_PriceListDTO.Id;
            PriceList.Code = PriceList_PriceListDTO.Code;
            PriceList.Name = PriceList_PriceListDTO.Name;
            PriceList.StatusId = PriceList_PriceListDTO.StatusId;
            PriceList.OrganizationId = PriceList_PriceListDTO.OrganizationId;
            PriceList.PriceListTypeId = PriceList_PriceListDTO.PriceListTypeId;
            PriceList.Organization = PriceList_PriceListDTO.Organization == null ? null : new Organization
            {
                Id = PriceList_PriceListDTO.Organization.Id,
                Code = PriceList_PriceListDTO.Organization.Code,
                Name = PriceList_PriceListDTO.Organization.Name,
                ParentId = PriceList_PriceListDTO.Organization.ParentId,
                Path = PriceList_PriceListDTO.Organization.Path,
                Level = PriceList_PriceListDTO.Organization.Level,
                StatusId = PriceList_PriceListDTO.Organization.StatusId,
                Phone = PriceList_PriceListDTO.Organization.Phone,
                Email = PriceList_PriceListDTO.Organization.Email,
                Address = PriceList_PriceListDTO.Organization.Address,
            };
            PriceList.PriceListType = PriceList_PriceListDTO.PriceListType == null ? null : new PriceListType
            {
                Id = PriceList_PriceListDTO.PriceListType.Id,
                Code = PriceList_PriceListDTO.PriceListType.Code,
                Name = PriceList_PriceListDTO.PriceListType.Name,
            };
            PriceList.Status = PriceList_PriceListDTO.Status == null ? null : new Status
            {
                Id = PriceList_PriceListDTO.Status.Id,
                Code = PriceList_PriceListDTO.Status.Code,
                Name = PriceList_PriceListDTO.Status.Name,
            };
            PriceList.PriceListItemMappings = PriceList_PriceListDTO.PriceListItemMappings?
                .Select(x => new PriceListItemMapping
                {
                    ItemId = x.ItemId,
                    Price = x.Price,
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
                }).ToList();
            PriceList.PriceListStoreMappings = PriceList_PriceListDTO.PriceListStoreMappings?
                .Select(x => new PriceListStoreMapping
                {
                    StoreId = x.StoreId,
                    Store = x.Store == null ? null : new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        ParentStoreId = x.Store.ParentStoreId,
                        OrganizationId = x.Store.OrganizationId,
                        StoreTypeId = x.Store.StoreTypeId,
                        StoreGroupingId = x.Store.StoreGroupingId,
                        ResellerId = x.Store.ResellerId,
                        Telephone = x.Store.Telephone,
                        ProvinceId = x.Store.ProvinceId,
                        DistrictId = x.Store.DistrictId,
                        WardId = x.Store.WardId,
                        Address = x.Store.Address,
                        DeliveryAddress = x.Store.DeliveryAddress,
                        Latitude = x.Store.Latitude,
                        Longitude = x.Store.Longitude,
                        DeliveryLatitude = x.Store.DeliveryLatitude,
                        DeliveryLongitude = x.Store.DeliveryLongitude,
                        OwnerName = x.Store.OwnerName,
                        OwnerPhone = x.Store.OwnerPhone,
                        OwnerEmail = x.Store.OwnerEmail,
                        StatusId = x.Store.StatusId,
                        WorkflowDefinitionId = x.Store.WorkflowDefinitionId,
                        RequestStateId = x.Store.RequestStateId,
                    },
                }).ToList();
            PriceList.PriceListStoreTypeMappings = PriceList_PriceListDTO.PriceListStoreTypeMappings?
                .Select(x => new PriceListStoreTypeMapping
                {
                    StoreTypeId = x.StoreTypeId,
                    StoreType = x.StoreType == null ? null : new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                }).ToList();
            PriceList.BaseLanguage = CurrentContext.Language;
            return PriceList;
        }

        private PriceListFilter ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter.Selects = PriceListSelect.ALL;
            PriceListFilter.Skip = PriceList_PriceListFilterDTO.Skip;
            PriceListFilter.Take = PriceList_PriceListFilterDTO.Take;
            PriceListFilter.OrderBy = PriceList_PriceListFilterDTO.OrderBy;
            PriceListFilter.OrderType = PriceList_PriceListFilterDTO.OrderType;

            PriceListFilter.Id = PriceList_PriceListFilterDTO.Id;
            PriceListFilter.Code = PriceList_PriceListFilterDTO.Code;
            PriceListFilter.Name = PriceList_PriceListFilterDTO.Name;
            PriceListFilter.StatusId = PriceList_PriceListFilterDTO.StatusId;
            PriceListFilter.OrganizationId = PriceList_PriceListFilterDTO.OrganizationId;
            PriceListFilter.PriceListTypeId = PriceList_PriceListFilterDTO.PriceListTypeId;
            PriceListFilter.CreatedAt = PriceList_PriceListFilterDTO.CreatedAt;
            PriceListFilter.UpdatedAt = PriceList_PriceListFilterDTO.UpdatedAt;
            return PriceListFilter;
        }

        [Route(PriceListRoute.FilterListOrganization), HttpPost]
        public async Task<List<PriceList_OrganizationDTO>> FilterListOrganization([FromBody] PriceList_OrganizationFilterDTO PriceList_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = PriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = PriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = PriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = PriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = PriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = PriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = PriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = PriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = PriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = PriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<PriceList_OrganizationDTO> PriceList_OrganizationDTOs = Organizations
                .Select(x => new PriceList_OrganizationDTO(x)).ToList();
            return PriceList_OrganizationDTOs;
        }
        [Route(PriceListRoute.FilterListPriceListType), HttpPost]
        public async Task<List<PriceList_PriceListTypeDTO>> FilterListPriceListType([FromBody] PriceList_PriceListTypeFilterDTO PriceList_PriceListTypeFilterDTO)
        {
            PriceListTypeFilter PriceListTypeFilter = new PriceListTypeFilter();
            PriceListTypeFilter.Skip = 0;
            PriceListTypeFilter.Take = int.MaxValue;
            PriceListTypeFilter.Take = 20;
            PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
            PriceListTypeFilter.OrderType = OrderType.ASC;
            PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;

            List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);
            List<PriceList_PriceListTypeDTO> PriceList_PriceListTypeDTOs = PriceListTypes
                .Select(x => new PriceList_PriceListTypeDTO(x)).ToList();
            return PriceList_PriceListTypeDTOs;
        }
        [Route(PriceListRoute.FilterListStatus), HttpPost]
        public async Task<List<PriceList_StatusDTO>> FilterListStatus([FromBody] PriceList_StatusFilterDTO PriceList_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<PriceList_StatusDTO> PriceList_StatusDTOs = Statuses
                .Select(x => new PriceList_StatusDTO(x)).ToList();
            return PriceList_StatusDTOs;
        }
        [Route(PriceListRoute.FilterListItem), HttpPost]
        public async Task<List<PriceList_ItemDTO>> FilterListItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = PriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<PriceList_ItemDTO> PriceList_ItemDTOs = Items
                .Select(x => new PriceList_ItemDTO(x)).ToList();
            return PriceList_ItemDTOs;
        }
        [Route(PriceListRoute.FilterListStore), HttpPost]
        public async Task<List<PriceList_StoreDTO>> FilterListStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = PriceList_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<PriceList_StoreDTO> PriceList_StoreDTOs = Stores
                .Select(x => new PriceList_StoreDTO(x)).ToList();
            return PriceList_StoreDTOs;
        }
        [Route(PriceListRoute.FilterListStoreType), HttpPost]
        public async Task<List<PriceList_StoreTypeDTO>> FilterListStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = PriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<PriceList_StoreTypeDTO> PriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new PriceList_StoreTypeDTO(x)).ToList();
            return PriceList_StoreTypeDTOs;
        }

        [Route(PriceListRoute.SingleListOrganization), HttpPost]
        public async Task<List<PriceList_OrganizationDTO>> SingleListOrganization([FromBody] PriceList_OrganizationFilterDTO PriceList_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = PriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = PriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = PriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = PriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = PriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = PriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = PriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = PriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = PriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = PriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<PriceList_OrganizationDTO> PriceList_OrganizationDTOs = Organizations
                .Select(x => new PriceList_OrganizationDTO(x)).ToList();
            return PriceList_OrganizationDTOs;
        }
        [Route(PriceListRoute.SingleListPriceListType), HttpPost]
        public async Task<List<PriceList_PriceListTypeDTO>> SingleListPriceListType([FromBody] PriceList_PriceListTypeFilterDTO PriceList_PriceListTypeFilterDTO)
        {
            PriceListTypeFilter PriceListTypeFilter = new PriceListTypeFilter();
            PriceListTypeFilter.Skip = 0;
            PriceListTypeFilter.Take = int.MaxValue;
            PriceListTypeFilter.Take = 20;
            PriceListTypeFilter.OrderBy = PriceListTypeOrder.Id;
            PriceListTypeFilter.OrderType = OrderType.ASC;
            PriceListTypeFilter.Selects = PriceListTypeSelect.ALL;

            List<PriceListType> PriceListTypes = await PriceListTypeService.List(PriceListTypeFilter);
            List<PriceList_PriceListTypeDTO> PriceList_PriceListTypeDTOs = PriceListTypes
                .Select(x => new PriceList_PriceListTypeDTO(x)).ToList();
            return PriceList_PriceListTypeDTOs;
        }
        [Route(PriceListRoute.SingleListStatus), HttpPost]
        public async Task<List<PriceList_StatusDTO>> SingleListStatus([FromBody] PriceList_StatusFilterDTO PriceList_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<PriceList_StatusDTO> PriceList_StatusDTOs = Statuses
                .Select(x => new PriceList_StatusDTO(x)).ToList();
            return PriceList_StatusDTOs;
        }
        [Route(PriceListRoute.SingleListItem), HttpPost]
        public async Task<List<PriceList_ItemDTO>> SingleListItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = PriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<PriceList_ItemDTO> PriceList_ItemDTOs = Items
                .Select(x => new PriceList_ItemDTO(x)).ToList();
            return PriceList_ItemDTOs;
        }
        [Route(PriceListRoute.SingleListStore), HttpPost]
        public async Task<List<PriceList_StoreDTO>> SingleListStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = PriceList_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<PriceList_StoreDTO> PriceList_StoreDTOs = Stores
                .Select(x => new PriceList_StoreDTO(x)).ToList();
            return PriceList_StoreDTOs;
        }
        [Route(PriceListRoute.SingleListStoreType), HttpPost]
        public async Task<List<PriceList_StoreTypeDTO>> SingleListStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = PriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<PriceList_StoreTypeDTO> PriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new PriceList_StoreTypeDTO(x)).ToList();
            return PriceList_StoreTypeDTOs;
        }

        [Route(PriceListRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = PriceList_ItemFilterDTO.StatusId;

            return await ItemService.Count(ItemFilter);
        }

        [Route(PriceListRoute.ListItem), HttpPost]
        public async Task<List<PriceList_ItemDTO>> ListItem([FromBody] PriceList_ItemFilterDTO PriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = PriceList_ItemFilterDTO.Skip;
            ItemFilter.Take = PriceList_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = PriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = PriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = PriceList_ItemFilterDTO.Code;
            ItemFilter.Name = PriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = PriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = PriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = PriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = PriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<PriceList_ItemDTO> PriceList_ItemDTOs = Items
                .Select(x => new PriceList_ItemDTO(x)).ToList();
            return PriceList_ItemDTOs;
        }
        [Route(PriceListRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = PriceList_StoreFilterDTO.StatusId;

            return await StoreService.Count(StoreFilter);
        }

        [Route(PriceListRoute.ListStore), HttpPost]
        public async Task<List<PriceList_StoreDTO>> ListStore([FromBody] PriceList_StoreFilterDTO PriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = PriceList_StoreFilterDTO.Skip;
            StoreFilter.Take = PriceList_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = PriceList_StoreFilterDTO.Id;
            StoreFilter.Code = PriceList_StoreFilterDTO.Code;
            StoreFilter.Name = PriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = PriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = PriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = PriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = PriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = PriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = PriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = PriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = PriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = PriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = PriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = PriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = PriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = PriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = PriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = PriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = PriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = PriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = PriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = PriceList_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<PriceList_StoreDTO> PriceList_StoreDTOs = Stores
                .Select(x => new PriceList_StoreDTO(x)).ToList();
            return PriceList_StoreDTOs;
        }
        [Route(PriceListRoute.CountStoreType), HttpPost]
        public async Task<long> CountStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = PriceList_StoreTypeFilterDTO.StatusId;

            return await StoreTypeService.Count(StoreTypeFilter);
        }

        [Route(PriceListRoute.ListStoreType), HttpPost]
        public async Task<List<PriceList_StoreTypeDTO>> ListStoreType([FromBody] PriceList_StoreTypeFilterDTO PriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = PriceList_StoreTypeFilterDTO.Skip;
            StoreTypeFilter.Take = PriceList_StoreTypeFilterDTO.Take;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = PriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = PriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = PriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = PriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<PriceList_StoreTypeDTO> PriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new PriceList_StoreTypeDTO(x)).ToList();
            return PriceList_StoreTypeDTOs;
        }
    }
}

