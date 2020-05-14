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
using DMS.Services.MDirectPriceList;
using DMS.Services.MDirectPriceListType;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItem;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStore;
using DMS.Services.MStoreType;
using DMS.Enums;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceListRoute : Root
    {
        public const string Master = Module + "/direct-price-list/direct-price-list-master";
        public const string Detail = Module + "/direct-price-list/direct-price-list-detail";
        private const string Default = Rpc + Module + "/direct-price-list";
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
        
        public const string FilterListDirectPriceListType = Default + "/filter-list-direct-price-list-type";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        
        public const string SingleListDirectPriceListType = Default + "/single-list-direct-price-list-type";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string CountStoreGrouping = Default + "/count-store-grouping";
        public const string ListStoreGrouping = Default + "/list-store-grouping";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountStoreType = Default + "/count-store-type";
        public const string ListStoreType = Default + "/list-store-type";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(DirectPriceListFilter.Code), FieldType.STRING },
            { nameof(DirectPriceListFilter.Name), FieldType.STRING },
            { nameof(DirectPriceListFilter.StartDate), FieldType.DATE },
            { nameof(DirectPriceListFilter.EndDate), FieldType.DATE },
            { nameof(DirectPriceListFilter.StatusId), FieldType.ID },
            { nameof(DirectPriceListFilter.OrganizationId), FieldType.ID },
            { nameof(DirectPriceListFilter.DirectPriceListTypeId), FieldType.ID },
        };
    }

    public class DirectPriceListController : RpcController
    {
        private IDirectPriceListTypeService DirectPriceListTypeService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IDirectPriceListService DirectPriceListService;
        private ICurrentContext CurrentContext;
        public DirectPriceListController(
            IDirectPriceListTypeService DirectPriceListTypeService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IItemService ItemService,
            IStoreGroupingService StoreGroupingService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IDirectPriceListService DirectPriceListService,
            ICurrentContext CurrentContext
        )
        {
            this.DirectPriceListTypeService = DirectPriceListTypeService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.DirectPriceListService = DirectPriceListService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DirectPriceListRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] DirectPriceList_DirectPriceListFilterDTO DirectPriceList_DirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectPriceListFilter DirectPriceListFilter = ConvertFilterDTOToFilterEntity(DirectPriceList_DirectPriceListFilterDTO);
            DirectPriceListFilter = DirectPriceListService.ToFilter(DirectPriceListFilter);
            int count = await DirectPriceListService.Count(DirectPriceListFilter);
            return count;
        }

        [Route(DirectPriceListRoute.List), HttpPost]
        public async Task<ActionResult<List<DirectPriceList_DirectPriceListDTO>>> List([FromBody] DirectPriceList_DirectPriceListFilterDTO DirectPriceList_DirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectPriceListFilter DirectPriceListFilter = ConvertFilterDTOToFilterEntity(DirectPriceList_DirectPriceListFilterDTO);
            DirectPriceListFilter = DirectPriceListService.ToFilter(DirectPriceListFilter);
            List<DirectPriceList> DirectPriceLists = await DirectPriceListService.List(DirectPriceListFilter);
            List<DirectPriceList_DirectPriceListDTO> DirectPriceList_DirectPriceListDTOs = DirectPriceLists
                .Select(c => new DirectPriceList_DirectPriceListDTO(c)).ToList();
            return DirectPriceList_DirectPriceListDTOs;
        }

        [Route(DirectPriceListRoute.Get), HttpPost]
        public async Task<ActionResult<DirectPriceList_DirectPriceListDTO>> Get([FromBody]DirectPriceList_DirectPriceListDTO DirectPriceList_DirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectPriceList_DirectPriceListDTO.Id))
                return Forbid();

            DirectPriceList DirectPriceList = await DirectPriceListService.Get(DirectPriceList_DirectPriceListDTO.Id);
            return new DirectPriceList_DirectPriceListDTO(DirectPriceList);
        }

        [Route(DirectPriceListRoute.Create), HttpPost]
        public async Task<ActionResult<DirectPriceList_DirectPriceListDTO>> Create([FromBody] DirectPriceList_DirectPriceListDTO DirectPriceList_DirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectPriceList_DirectPriceListDTO.Id))
                return Forbid();

            DirectPriceList DirectPriceList = ConvertDTOToEntity(DirectPriceList_DirectPriceListDTO);
            DirectPriceList = await DirectPriceListService.Create(DirectPriceList);
            DirectPriceList_DirectPriceListDTO = new DirectPriceList_DirectPriceListDTO(DirectPriceList);
            if (DirectPriceList.IsValidated)
                return DirectPriceList_DirectPriceListDTO;
            else
                return BadRequest(DirectPriceList_DirectPriceListDTO);
        }

        [Route(DirectPriceListRoute.Update), HttpPost]
        public async Task<ActionResult<DirectPriceList_DirectPriceListDTO>> Update([FromBody] DirectPriceList_DirectPriceListDTO DirectPriceList_DirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectPriceList_DirectPriceListDTO.Id))
                return Forbid();

            DirectPriceList DirectPriceList = ConvertDTOToEntity(DirectPriceList_DirectPriceListDTO);
            DirectPriceList = await DirectPriceListService.Update(DirectPriceList);
            DirectPriceList_DirectPriceListDTO = new DirectPriceList_DirectPriceListDTO(DirectPriceList);
            if (DirectPriceList.IsValidated)
                return DirectPriceList_DirectPriceListDTO;
            else
                return BadRequest(DirectPriceList_DirectPriceListDTO);
        }

        [Route(DirectPriceListRoute.Delete), HttpPost]
        public async Task<ActionResult<DirectPriceList_DirectPriceListDTO>> Delete([FromBody] DirectPriceList_DirectPriceListDTO DirectPriceList_DirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectPriceList_DirectPriceListDTO.Id))
                return Forbid();

            DirectPriceList DirectPriceList = ConvertDTOToEntity(DirectPriceList_DirectPriceListDTO);
            DirectPriceList = await DirectPriceListService.Delete(DirectPriceList);
            DirectPriceList_DirectPriceListDTO = new DirectPriceList_DirectPriceListDTO(DirectPriceList);
            if (DirectPriceList.IsValidated)
                return DirectPriceList_DirectPriceListDTO;
            else
                return BadRequest(DirectPriceList_DirectPriceListDTO);
        }
        
        [Route(DirectPriceListRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectPriceListFilter DirectPriceListFilter = new DirectPriceListFilter();
            DirectPriceListFilter = DirectPriceListService.ToFilter(DirectPriceListFilter);
            DirectPriceListFilter.Id = new IdFilter { In = Ids };
            DirectPriceListFilter.Selects = DirectPriceListSelect.Id;
            DirectPriceListFilter.Skip = 0;
            DirectPriceListFilter.Take = int.MaxValue;

            List<DirectPriceList> DirectPriceLists = await DirectPriceListService.List(DirectPriceListFilter);
            DirectPriceLists = await DirectPriceListService.BulkDelete(DirectPriceLists);
            return true;
        }
        
        [Route(DirectPriceListRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DirectPriceListTypeFilter DirectPriceListTypeFilter = new DirectPriceListTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DirectPriceListTypeSelect.ALL
            };
            List<DirectPriceListType> DirectPriceListTypes = await DirectPriceListTypeService.List(DirectPriceListTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<DirectPriceList> DirectPriceLists = new List<DirectPriceList>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(DirectPriceLists);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                int OrganizationIdColumn = 4 + StartColumn;
                int DirectPriceListTypeIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string DirectPriceListTypeIdValue = worksheet.Cells[i + StartRow, DirectPriceListTypeIdColumn].Value?.ToString();
                    
                    DirectPriceList DirectPriceList = new DirectPriceList();
                    DirectPriceList.Code = CodeValue;
                    DirectPriceList.Name = NameValue;
                    DirectPriceListType DirectPriceListType = DirectPriceListTypes.Where(x => x.Id.ToString() == DirectPriceListTypeIdValue).FirstOrDefault();
                    DirectPriceList.DirectPriceListTypeId = DirectPriceListType == null ? 0 : DirectPriceListType.Id;
                    DirectPriceList.DirectPriceListType = DirectPriceListType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    DirectPriceList.StatusId = Status == null ? 0 : Status.Id;
                    DirectPriceList.Status = Status;
                    
                    DirectPriceLists.Add(DirectPriceList);
                }
            }
            DirectPriceLists = await DirectPriceListService.Import(DirectPriceLists);
            if (DirectPriceLists.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < DirectPriceLists.Count; i++)
                {
                    DirectPriceList DirectPriceList = DirectPriceLists[i];
                    if (!DirectPriceList.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (DirectPriceList.Errors.ContainsKey(nameof(DirectPriceList.Id)))
                            Error += DirectPriceList.Errors[nameof(DirectPriceList.Id)];
                        if (DirectPriceList.Errors.ContainsKey(nameof(DirectPriceList.Code)))
                            Error += DirectPriceList.Errors[nameof(DirectPriceList.Code)];
                        if (DirectPriceList.Errors.ContainsKey(nameof(DirectPriceList.Name)))
                            Error += DirectPriceList.Errors[nameof(DirectPriceList.Name)];
                        if (DirectPriceList.Errors.ContainsKey(nameof(DirectPriceList.StatusId)))
                            Error += DirectPriceList.Errors[nameof(DirectPriceList.StatusId)];
                        if (DirectPriceList.Errors.ContainsKey(nameof(DirectPriceList.OrganizationId)))
                            Error += DirectPriceList.Errors[nameof(DirectPriceList.OrganizationId)];
                        if (DirectPriceList.Errors.ContainsKey(nameof(DirectPriceList.DirectPriceListTypeId)))
                            Error += DirectPriceList.Errors[nameof(DirectPriceList.DirectPriceListTypeId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(DirectPriceListRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] DirectPriceList_DirectPriceListFilterDTO DirectPriceList_DirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectPriceList
                var DirectPriceListFilter = ConvertFilterDTOToFilterEntity(DirectPriceList_DirectPriceListFilterDTO);
                DirectPriceListFilter.Skip = 0;
                DirectPriceListFilter.Take = int.MaxValue;
                DirectPriceListFilter = DirectPriceListService.ToFilter(DirectPriceListFilter);
                List<DirectPriceList> DirectPriceLists = await DirectPriceListService.List(DirectPriceListFilter);

                var DirectPriceListHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "OrganizationId",
                        "DirectPriceListTypeId",
                    }
                };
                List<object[]> DirectPriceListData = new List<object[]>();
                for (int i = 0; i < DirectPriceLists.Count; i++)
                {
                    var DirectPriceList = DirectPriceLists[i];
                    DirectPriceListData.Add(new Object[]
                    {
                        DirectPriceList.Id,
                        DirectPriceList.Code,
                        DirectPriceList.Name,
                        DirectPriceList.StatusId,
                        DirectPriceList.OrganizationId,
                        DirectPriceList.DirectPriceListTypeId,
                    });
                }
                excel.GenerateWorksheet("DirectPriceList", DirectPriceListHeaders, DirectPriceListData);
                #endregion
                
                #region DirectPriceListType
                var DirectPriceListTypeFilter = new DirectPriceListTypeFilter();
                DirectPriceListTypeFilter.Selects = DirectPriceListTypeSelect.ALL;
                DirectPriceListTypeFilter.OrderBy = DirectPriceListTypeOrder.Id;
                DirectPriceListTypeFilter.OrderType = OrderType.ASC;
                DirectPriceListTypeFilter.Skip = 0;
                DirectPriceListTypeFilter.Take = int.MaxValue;
                List<DirectPriceListType> DirectPriceListTypes = await DirectPriceListTypeService.List(DirectPriceListTypeFilter);

                var DirectPriceListTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> DirectPriceListTypeData = new List<object[]>();
                for (int i = 0; i < DirectPriceListTypes.Count; i++)
                {
                    var DirectPriceListType = DirectPriceListTypes[i];
                    DirectPriceListTypeData.Add(new Object[]
                    {
                        DirectPriceListType.Id,
                        DirectPriceListType.Code,
                        DirectPriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("DirectPriceListType", DirectPriceListTypeHeaders, DirectPriceListTypeData);
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
                #region StoreGrouping
                var StoreGroupingFilter = new StoreGroupingFilter();
                StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
                StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
                StoreGroupingFilter.OrderType = OrderType.ASC;
                StoreGroupingFilter.Skip = 0;
                StoreGroupingFilter.Take = int.MaxValue;
                List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);

                var StoreGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                    }
                };
                List<object[]> StoreGroupingData = new List<object[]>();
                for (int i = 0; i < StoreGroupings.Count; i++)
                {
                    var StoreGrouping = StoreGroupings[i];
                    StoreGroupingData.Add(new Object[]
                    {
                        StoreGrouping.Id,
                        StoreGrouping.Code,
                        StoreGrouping.Name,
                        StoreGrouping.ParentId,
                        StoreGrouping.Path,
                        StoreGrouping.Level,
                        StoreGrouping.StatusId,
                    });
                }
                excel.GenerateWorksheet("StoreGrouping", StoreGroupingHeaders, StoreGroupingData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectPriceList.xlsx");
        }

        [Route(DirectPriceListRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] DirectPriceList_DirectPriceListFilterDTO DirectPriceList_DirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectPriceList
                var DirectPriceListHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "OrganizationId",
                        "DirectPriceListTypeId",
                    }
                };
                List<object[]> DirectPriceListData = new List<object[]>();
                excel.GenerateWorksheet("DirectPriceList", DirectPriceListHeaders, DirectPriceListData);
                #endregion
                
                #region DirectPriceListType
                var DirectPriceListTypeFilter = new DirectPriceListTypeFilter();
                DirectPriceListTypeFilter.Selects = DirectPriceListTypeSelect.ALL;
                DirectPriceListTypeFilter.OrderBy = DirectPriceListTypeOrder.Id;
                DirectPriceListTypeFilter.OrderType = OrderType.ASC;
                DirectPriceListTypeFilter.Skip = 0;
                DirectPriceListTypeFilter.Take = int.MaxValue;
                List<DirectPriceListType> DirectPriceListTypes = await DirectPriceListTypeService.List(DirectPriceListTypeFilter);

                var DirectPriceListTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> DirectPriceListTypeData = new List<object[]>();
                for (int i = 0; i < DirectPriceListTypes.Count; i++)
                {
                    var DirectPriceListType = DirectPriceListTypes[i];
                    DirectPriceListTypeData.Add(new Object[]
                    {
                        DirectPriceListType.Id,
                        DirectPriceListType.Code,
                        DirectPriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("DirectPriceListType", DirectPriceListTypeHeaders, DirectPriceListTypeData);
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
                #region StoreGrouping
                var StoreGroupingFilter = new StoreGroupingFilter();
                StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
                StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
                StoreGroupingFilter.OrderType = OrderType.ASC;
                StoreGroupingFilter.Skip = 0;
                StoreGroupingFilter.Take = int.MaxValue;
                List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);

                var StoreGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                    }
                };
                List<object[]> StoreGroupingData = new List<object[]>();
                for (int i = 0; i < StoreGroupings.Count; i++)
                {
                    var StoreGrouping = StoreGroupings[i];
                    StoreGroupingData.Add(new Object[]
                    {
                        StoreGrouping.Id,
                        StoreGrouping.Code,
                        StoreGrouping.Name,
                        StoreGrouping.ParentId,
                        StoreGrouping.Path,
                        StoreGrouping.Level,
                        StoreGrouping.StatusId,
                    });
                }
                excel.GenerateWorksheet("StoreGrouping", StoreGroupingHeaders, StoreGroupingData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectPriceList.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            DirectPriceListFilter DirectPriceListFilter = new DirectPriceListFilter();
            DirectPriceListFilter = DirectPriceListService.ToFilter(DirectPriceListFilter);
            if (Id == 0)
            {

            }
            else
            {
                DirectPriceListFilter.Id = new IdFilter { Equal = Id };
                int count = await DirectPriceListService.Count(DirectPriceListFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private DirectPriceList ConvertDTOToEntity(DirectPriceList_DirectPriceListDTO DirectPriceList_DirectPriceListDTO)
        {
            DirectPriceList DirectPriceList = new DirectPriceList();
            DirectPriceList.Id = DirectPriceList_DirectPriceListDTO.Id;
            DirectPriceList.Code = DirectPriceList_DirectPriceListDTO.Code;
            DirectPriceList.Name = DirectPriceList_DirectPriceListDTO.Name;
            DirectPriceList.StartDate = DirectPriceList_DirectPriceListDTO.StartDate;
            DirectPriceList.EndDate = DirectPriceList_DirectPriceListDTO.EndDate;
            DirectPriceList.StatusId = DirectPriceList_DirectPriceListDTO.StatusId;
            DirectPriceList.OrganizationId = DirectPriceList_DirectPriceListDTO.OrganizationId;
            DirectPriceList.DirectPriceListTypeId = DirectPriceList_DirectPriceListDTO.DirectPriceListTypeId;
            DirectPriceList.DirectPriceListType = DirectPriceList_DirectPriceListDTO.DirectPriceListType == null ? null : new DirectPriceListType
            {
                Id = DirectPriceList_DirectPriceListDTO.DirectPriceListType.Id,
                Code = DirectPriceList_DirectPriceListDTO.DirectPriceListType.Code,
                Name = DirectPriceList_DirectPriceListDTO.DirectPriceListType.Name,
            };
            DirectPriceList.Organization = DirectPriceList_DirectPriceListDTO.Organization == null ? null : new Organization
            {
                Id = DirectPriceList_DirectPriceListDTO.Organization.Id,
                Code = DirectPriceList_DirectPriceListDTO.Organization.Code,
                Name = DirectPriceList_DirectPriceListDTO.Organization.Name,
                ParentId = DirectPriceList_DirectPriceListDTO.Organization.ParentId,
                Path = DirectPriceList_DirectPriceListDTO.Organization.Path,
                Level = DirectPriceList_DirectPriceListDTO.Organization.Level,
                StatusId = DirectPriceList_DirectPriceListDTO.Organization.StatusId,
                Phone = DirectPriceList_DirectPriceListDTO.Organization.Phone,
                Email = DirectPriceList_DirectPriceListDTO.Organization.Email,
                Address = DirectPriceList_DirectPriceListDTO.Organization.Address,
            };
            DirectPriceList.Status = DirectPriceList_DirectPriceListDTO.Status == null ? null : new Status
            {
                Id = DirectPriceList_DirectPriceListDTO.Status.Id,
                Code = DirectPriceList_DirectPriceListDTO.Status.Code,
                Name = DirectPriceList_DirectPriceListDTO.Status.Name,
            };
            DirectPriceList.DirectPriceListItemMappings = DirectPriceList_DirectPriceListDTO.DirectPriceListItemMappings?
                .Select(x => new DirectPriceListItemMapping
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
            DirectPriceList.DirectPriceListStoreGroupingMappings = DirectPriceList_DirectPriceListDTO.DirectPriceListStoreGroupingMappings?
                .Select(x => new DirectPriceListStoreGroupingMapping
                {
                    StoreGroupingId = x.StoreGroupingId,
                    StoreGrouping = x.StoreGrouping == null ? null : new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                        StatusId = x.StoreGrouping.StatusId,
                    },
                }).ToList();
            DirectPriceList.DirectPriceListStoreMappings = DirectPriceList_DirectPriceListDTO.DirectPriceListStoreMappings?
                .Select(x => new DirectPriceListStoreMapping
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
                        TaxCode = x.Store.TaxCode,
                        LegalEntity = x.Store.LegalEntity,
                        StatusId = x.Store.StatusId,
                    },
                }).ToList();
            DirectPriceList.DirectPriceListStoreTypeMappings = DirectPriceList_DirectPriceListDTO.DirectPriceListStoreTypeMappings?
                .Select(x => new DirectPriceListStoreTypeMapping
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
            DirectPriceList.BaseLanguage = CurrentContext.Language;
            return DirectPriceList;
        }

        private DirectPriceListFilter ConvertFilterDTOToFilterEntity(DirectPriceList_DirectPriceListFilterDTO DirectPriceList_DirectPriceListFilterDTO)
        {
            DirectPriceListFilter DirectPriceListFilter = new DirectPriceListFilter();
            DirectPriceListFilter.Selects = DirectPriceListSelect.ALL;
            DirectPriceListFilter.Skip = DirectPriceList_DirectPriceListFilterDTO.Skip;
            DirectPriceListFilter.Take = DirectPriceList_DirectPriceListFilterDTO.Take;
            DirectPriceListFilter.OrderBy = DirectPriceList_DirectPriceListFilterDTO.OrderBy;
            DirectPriceListFilter.OrderType = DirectPriceList_DirectPriceListFilterDTO.OrderType;

            DirectPriceListFilter.Id = DirectPriceList_DirectPriceListFilterDTO.Id;
            DirectPriceListFilter.Code = DirectPriceList_DirectPriceListFilterDTO.Code;
            DirectPriceListFilter.Name = DirectPriceList_DirectPriceListFilterDTO.Name;
            DirectPriceListFilter.StartDate = DirectPriceList_DirectPriceListFilterDTO.StartDate;
            DirectPriceListFilter.EndDate = DirectPriceList_DirectPriceListFilterDTO.EndDate;
            DirectPriceListFilter.StatusId = DirectPriceList_DirectPriceListFilterDTO.StatusId;
            DirectPriceListFilter.OrganizationId = DirectPriceList_DirectPriceListFilterDTO.OrganizationId;
            DirectPriceListFilter.DirectPriceListTypeId = DirectPriceList_DirectPriceListFilterDTO.DirectPriceListTypeId;
            DirectPriceListFilter.CreatedAt = DirectPriceList_DirectPriceListFilterDTO.CreatedAt;
            DirectPriceListFilter.UpdatedAt = DirectPriceList_DirectPriceListFilterDTO.UpdatedAt;
            return DirectPriceListFilter;
        }

        [Route(DirectPriceListRoute.FilterListDirectPriceListType), HttpPost]
        public async Task<List<DirectPriceList_DirectPriceListTypeDTO>> FilterListDirectPriceListType([FromBody] DirectPriceList_DirectPriceListTypeFilterDTO DirectPriceList_DirectPriceListTypeFilterDTO)
        {
            DirectPriceListTypeFilter DirectPriceListTypeFilter = new DirectPriceListTypeFilter();
            DirectPriceListTypeFilter.Skip = 0;
            DirectPriceListTypeFilter.Take = 20;
            DirectPriceListTypeFilter.OrderBy = DirectPriceListTypeOrder.Id;
            DirectPriceListTypeFilter.OrderType = OrderType.ASC;
            DirectPriceListTypeFilter.Selects = DirectPriceListTypeSelect.ALL;
            DirectPriceListTypeFilter.Id = DirectPriceList_DirectPriceListTypeFilterDTO.Id;
            DirectPriceListTypeFilter.Code = DirectPriceList_DirectPriceListTypeFilterDTO.Code;
            DirectPriceListTypeFilter.Name = DirectPriceList_DirectPriceListTypeFilterDTO.Name;

            List<DirectPriceListType> DirectPriceListTypes = await DirectPriceListTypeService.List(DirectPriceListTypeFilter);
            List<DirectPriceList_DirectPriceListTypeDTO> DirectPriceList_DirectPriceListTypeDTOs = DirectPriceListTypes
                .Select(x => new DirectPriceList_DirectPriceListTypeDTO(x)).ToList();
            return DirectPriceList_DirectPriceListTypeDTOs;
        }
        [Route(DirectPriceListRoute.FilterListOrganization), HttpPost]
        public async Task<List<DirectPriceList_OrganizationDTO>> FilterListOrganization([FromBody] DirectPriceList_OrganizationFilterDTO DirectPriceList_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = DirectPriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = DirectPriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DirectPriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = DirectPriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = DirectPriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = DirectPriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = DirectPriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = DirectPriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = DirectPriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = DirectPriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DirectPriceList_OrganizationDTO> DirectPriceList_OrganizationDTOs = Organizations
                .Select(x => new DirectPriceList_OrganizationDTO(x)).ToList();
            return DirectPriceList_OrganizationDTOs;
        }
        [Route(DirectPriceListRoute.FilterListStatus), HttpPost]
        public async Task<List<DirectPriceList_StatusDTO>> FilterListStatus([FromBody] DirectPriceList_StatusFilterDTO DirectPriceList_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<DirectPriceList_StatusDTO> DirectPriceList_StatusDTOs = Statuses
                .Select(x => new DirectPriceList_StatusDTO(x)).ToList();
            return DirectPriceList_StatusDTOs;
        }
        [Route(DirectPriceListRoute.FilterListItem), HttpPost]
        public async Task<List<DirectPriceList_ItemDTO>> FilterListItem([FromBody] DirectPriceList_ItemFilterDTO DirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = DirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectPriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectPriceList_ItemDTO> DirectPriceList_ItemDTOs = Items
                .Select(x => new DirectPriceList_ItemDTO(x)).ToList();
            return DirectPriceList_ItemDTOs;
        }
        [Route(DirectPriceListRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<DirectPriceList_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] DirectPriceList_StoreGroupingFilterDTO DirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = DirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = DirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = DirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = DirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = DirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = DirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = DirectPriceList_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<DirectPriceList_StoreGroupingDTO> DirectPriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new DirectPriceList_StoreGroupingDTO(x)).ToList();
            return DirectPriceList_StoreGroupingDTOs;
        }
        [Route(DirectPriceListRoute.FilterListStore), HttpPost]
        public async Task<List<DirectPriceList_StoreDTO>> FilterListStore([FromBody] DirectPriceList_StoreFilterDTO DirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = DirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = DirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = DirectPriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = DirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = DirectPriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectPriceList_StoreDTO> DirectPriceList_StoreDTOs = Stores
                .Select(x => new DirectPriceList_StoreDTO(x)).ToList();
            return DirectPriceList_StoreDTOs;
        }
        [Route(DirectPriceListRoute.FilterListStoreType), HttpPost]
        public async Task<List<DirectPriceList_StoreTypeDTO>> FilterListStoreType([FromBody] DirectPriceList_StoreTypeFilterDTO DirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = DirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = DirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = DirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = DirectPriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<DirectPriceList_StoreTypeDTO> DirectPriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new DirectPriceList_StoreTypeDTO(x)).ToList();
            return DirectPriceList_StoreTypeDTOs;
        }

        [Route(DirectPriceListRoute.SingleListDirectPriceListType), HttpPost]
        public async Task<List<DirectPriceList_DirectPriceListTypeDTO>> SingleListDirectPriceListType([FromBody] DirectPriceList_DirectPriceListTypeFilterDTO DirectPriceList_DirectPriceListTypeFilterDTO)
        {
            DirectPriceListTypeFilter DirectPriceListTypeFilter = new DirectPriceListTypeFilter();
            DirectPriceListTypeFilter.Skip = 0;
            DirectPriceListTypeFilter.Take = 20;
            DirectPriceListTypeFilter.OrderBy = DirectPriceListTypeOrder.Id;
            DirectPriceListTypeFilter.OrderType = OrderType.ASC;
            DirectPriceListTypeFilter.Selects = DirectPriceListTypeSelect.ALL;
            DirectPriceListTypeFilter.Id = DirectPriceList_DirectPriceListTypeFilterDTO.Id;
            DirectPriceListTypeFilter.Code = DirectPriceList_DirectPriceListTypeFilterDTO.Code;
            DirectPriceListTypeFilter.Name = DirectPriceList_DirectPriceListTypeFilterDTO.Name;

            List<DirectPriceListType> DirectPriceListTypes = await DirectPriceListTypeService.List(DirectPriceListTypeFilter);
            List<DirectPriceList_DirectPriceListTypeDTO> DirectPriceList_DirectPriceListTypeDTOs = DirectPriceListTypes
                .Select(x => new DirectPriceList_DirectPriceListTypeDTO(x)).ToList();
            return DirectPriceList_DirectPriceListTypeDTOs;
        }
        [Route(DirectPriceListRoute.SingleListOrganization), HttpPost]
        public async Task<List<DirectPriceList_OrganizationDTO>> SingleListOrganization([FromBody] DirectPriceList_OrganizationFilterDTO DirectPriceList_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = DirectPriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = DirectPriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DirectPriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = DirectPriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = DirectPriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = DirectPriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = DirectPriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = DirectPriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = DirectPriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = DirectPriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DirectPriceList_OrganizationDTO> DirectPriceList_OrganizationDTOs = Organizations
                .Select(x => new DirectPriceList_OrganizationDTO(x)).ToList();
            return DirectPriceList_OrganizationDTOs;
        }
        [Route(DirectPriceListRoute.SingleListStatus), HttpPost]
        public async Task<List<DirectPriceList_StatusDTO>> SingleListStatus([FromBody] DirectPriceList_StatusFilterDTO DirectPriceList_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<DirectPriceList_StatusDTO> DirectPriceList_StatusDTOs = Statuses
                .Select(x => new DirectPriceList_StatusDTO(x)).ToList();
            return DirectPriceList_StatusDTOs;
        }
        [Route(DirectPriceListRoute.SingleListItem), HttpPost]
        public async Task<List<DirectPriceList_ItemDTO>> SingleListItem([FromBody] DirectPriceList_ItemFilterDTO DirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = DirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectPriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectPriceList_ItemDTO> DirectPriceList_ItemDTOs = Items
                .Select(x => new DirectPriceList_ItemDTO(x)).ToList();
            return DirectPriceList_ItemDTOs;
        }
        [Route(DirectPriceListRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<DirectPriceList_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] DirectPriceList_StoreGroupingFilterDTO DirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = DirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = DirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = DirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = DirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = DirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = DirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = DirectPriceList_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<DirectPriceList_StoreGroupingDTO> DirectPriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new DirectPriceList_StoreGroupingDTO(x)).ToList();
            return DirectPriceList_StoreGroupingDTOs;
        }
        [Route(DirectPriceListRoute.SingleListStore), HttpPost]
        public async Task<List<DirectPriceList_StoreDTO>> SingleListStore([FromBody] DirectPriceList_StoreFilterDTO DirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = DirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = DirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = DirectPriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = DirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = DirectPriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectPriceList_StoreDTO> DirectPriceList_StoreDTOs = Stores
                .Select(x => new DirectPriceList_StoreDTO(x)).ToList();
            return DirectPriceList_StoreDTOs;
        }
        [Route(DirectPriceListRoute.SingleListStoreType), HttpPost]
        public async Task<List<DirectPriceList_StoreTypeDTO>> SingleListStoreType([FromBody] DirectPriceList_StoreTypeFilterDTO DirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = DirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = DirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = DirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = DirectPriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<DirectPriceList_StoreTypeDTO> DirectPriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new DirectPriceList_StoreTypeDTO(x)).ToList();
            return DirectPriceList_StoreTypeDTOs;
        }

        [Route(DirectPriceListRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] DirectPriceList_ItemFilterDTO DirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = DirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = DirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await ItemService.Count(ItemFilter);
        }

        [Route(DirectPriceListRoute.ListItem), HttpPost]
        public async Task<List<DirectPriceList_ItemDTO>> ListItem([FromBody] DirectPriceList_ItemFilterDTO DirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = DirectPriceList_ItemFilterDTO.Skip;
            ItemFilter.Take = DirectPriceList_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = DirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectPriceList_ItemDTO> DirectPriceList_ItemDTOs = Items
                .Select(x => new DirectPriceList_ItemDTO(x)).ToList();
            return DirectPriceList_ItemDTOs;
        }
        [Route(DirectPriceListRoute.CountStoreGrouping), HttpPost]
        public async Task<long> CountStoreGrouping([FromBody] DirectPriceList_StoreGroupingFilterDTO DirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Id = DirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = DirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = DirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = DirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = DirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = DirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreGroupingService.Count(StoreGroupingFilter);
        }

        [Route(DirectPriceListRoute.ListStoreGrouping), HttpPost]
        public async Task<List<DirectPriceList_StoreGroupingDTO>> ListStoreGrouping([FromBody] DirectPriceList_StoreGroupingFilterDTO DirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = DirectPriceList_StoreGroupingFilterDTO.Skip;
            StoreGroupingFilter.Take = DirectPriceList_StoreGroupingFilterDTO.Take;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = DirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = DirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = DirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = DirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = DirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = DirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<DirectPriceList_StoreGroupingDTO> DirectPriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new DirectPriceList_StoreGroupingDTO(x)).ToList();
            return DirectPriceList_StoreGroupingDTOs;
        }
        [Route(DirectPriceListRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] DirectPriceList_StoreFilterDTO DirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = DirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = DirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = DirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            StoreFilter.WorkflowDefinitionId = DirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = DirectPriceList_StoreFilterDTO.RequestStateId;

            return await StoreService.Count(StoreFilter);
        }

        [Route(DirectPriceListRoute.ListStore), HttpPost]
        public async Task<List<DirectPriceList_StoreDTO>> ListStore([FromBody] DirectPriceList_StoreFilterDTO DirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = DirectPriceList_StoreFilterDTO.Skip;
            StoreFilter.Take = DirectPriceList_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = DirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = DirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            StoreFilter.WorkflowDefinitionId = DirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = DirectPriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectPriceList_StoreDTO> DirectPriceList_StoreDTOs = Stores
                .Select(x => new DirectPriceList_StoreDTO(x)).ToList();
            return DirectPriceList_StoreDTOs;
        }
        [Route(DirectPriceListRoute.CountStoreType), HttpPost]
        public async Task<long> CountStoreType([FromBody] DirectPriceList_StoreTypeFilterDTO DirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Id = DirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = DirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = DirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreTypeService.Count(StoreTypeFilter);
        }

        [Route(DirectPriceListRoute.ListStoreType), HttpPost]
        public async Task<List<DirectPriceList_StoreTypeDTO>> ListStoreType([FromBody] DirectPriceList_StoreTypeFilterDTO DirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = DirectPriceList_StoreTypeFilterDTO.Skip;
            StoreTypeFilter.Take = DirectPriceList_StoreTypeFilterDTO.Take;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = DirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = DirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = DirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<DirectPriceList_StoreTypeDTO> DirectPriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new DirectPriceList_StoreTypeDTO(x)).ToList();
            return DirectPriceList_StoreTypeDTOs;
        }
    }
}

