using Common;
using DMS.Entities;
using DMS.Services.MIndirectPriceList;
using DMS.Services.MIndirectPriceListType;
using DMS.Services.MItem;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceListController : RpcController
    {
        private IIndirectPriceListTypeService IndirectPriceListTypeService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IItemService ItemService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IIndirectPriceListService IndirectPriceListService;
        private ICurrentContext CurrentContext;
        public IndirectPriceListController(
            IIndirectPriceListTypeService IndirectPriceListTypeService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IItemService ItemService,
            IStoreGroupingService StoreGroupingService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IIndirectPriceListService IndirectPriceListService,
            ICurrentContext CurrentContext
        )
        {
            this.IndirectPriceListTypeService = IndirectPriceListTypeService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.ItemService = ItemService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.IndirectPriceListService = IndirectPriceListService;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectPriceListRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectPriceList_IndirectPriceListFilterDTO IndirectPriceList_IndirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectPriceListFilter IndirectPriceListFilter = ConvertFilterDTOToFilterEntity(IndirectPriceList_IndirectPriceListFilterDTO);
            IndirectPriceListFilter = IndirectPriceListService.ToFilter(IndirectPriceListFilter);
            int count = await IndirectPriceListService.Count(IndirectPriceListFilter);
            return count;
        }

        [Route(IndirectPriceListRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectPriceList_IndirectPriceListDTO>>> List([FromBody] IndirectPriceList_IndirectPriceListFilterDTO IndirectPriceList_IndirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectPriceListFilter IndirectPriceListFilter = ConvertFilterDTOToFilterEntity(IndirectPriceList_IndirectPriceListFilterDTO);
            IndirectPriceListFilter = IndirectPriceListService.ToFilter(IndirectPriceListFilter);
            List<IndirectPriceList> IndirectPriceLists = await IndirectPriceListService.List(IndirectPriceListFilter);
            List<IndirectPriceList_IndirectPriceListDTO> IndirectPriceList_IndirectPriceListDTOs = IndirectPriceLists
                .Select(c => new IndirectPriceList_IndirectPriceListDTO(c)).ToList();
            return IndirectPriceList_IndirectPriceListDTOs;
        }

        [Route(IndirectPriceListRoute.Get), HttpPost]
        public async Task<ActionResult<IndirectPriceList_IndirectPriceListDTO>> Get([FromBody]IndirectPriceList_IndirectPriceListDTO IndirectPriceList_IndirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectPriceList_IndirectPriceListDTO.Id))
                return Forbid();

            IndirectPriceList IndirectPriceList = await IndirectPriceListService.Get(IndirectPriceList_IndirectPriceListDTO.Id);
            return new IndirectPriceList_IndirectPriceListDTO(IndirectPriceList);
        }

        [Route(IndirectPriceListRoute.Create), HttpPost]
        public async Task<ActionResult<IndirectPriceList_IndirectPriceListDTO>> Create([FromBody] IndirectPriceList_IndirectPriceListDTO IndirectPriceList_IndirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectPriceList_IndirectPriceListDTO.Id))
                return Forbid();

            IndirectPriceList IndirectPriceList = ConvertDTOToEntity(IndirectPriceList_IndirectPriceListDTO);
            IndirectPriceList = await IndirectPriceListService.Create(IndirectPriceList);
            IndirectPriceList_IndirectPriceListDTO = new IndirectPriceList_IndirectPriceListDTO(IndirectPriceList);
            if (IndirectPriceList.IsValidated)
                return IndirectPriceList_IndirectPriceListDTO;
            else
                return BadRequest(IndirectPriceList_IndirectPriceListDTO);
        }

        [Route(IndirectPriceListRoute.Update), HttpPost]
        public async Task<ActionResult<IndirectPriceList_IndirectPriceListDTO>> Update([FromBody] IndirectPriceList_IndirectPriceListDTO IndirectPriceList_IndirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectPriceList_IndirectPriceListDTO.Id))
                return Forbid();

            IndirectPriceList IndirectPriceList = ConvertDTOToEntity(IndirectPriceList_IndirectPriceListDTO);
            IndirectPriceList = await IndirectPriceListService.Update(IndirectPriceList);
            IndirectPriceList_IndirectPriceListDTO = new IndirectPriceList_IndirectPriceListDTO(IndirectPriceList);
            if (IndirectPriceList.IsValidated)
                return IndirectPriceList_IndirectPriceListDTO;
            else
                return BadRequest(IndirectPriceList_IndirectPriceListDTO);
        }

        [Route(IndirectPriceListRoute.Delete), HttpPost]
        public async Task<ActionResult<IndirectPriceList_IndirectPriceListDTO>> Delete([FromBody] IndirectPriceList_IndirectPriceListDTO IndirectPriceList_IndirectPriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectPriceList_IndirectPriceListDTO.Id))
                return Forbid();

            IndirectPriceList IndirectPriceList = ConvertDTOToEntity(IndirectPriceList_IndirectPriceListDTO);
            IndirectPriceList = await IndirectPriceListService.Delete(IndirectPriceList);
            IndirectPriceList_IndirectPriceListDTO = new IndirectPriceList_IndirectPriceListDTO(IndirectPriceList);
            if (IndirectPriceList.IsValidated)
                return IndirectPriceList_IndirectPriceListDTO;
            else
                return BadRequest(IndirectPriceList_IndirectPriceListDTO);
        }

        [Route(IndirectPriceListRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectPriceListFilter IndirectPriceListFilter = new IndirectPriceListFilter();
            IndirectPriceListFilter = IndirectPriceListService.ToFilter(IndirectPriceListFilter);
            IndirectPriceListFilter.Id = new IdFilter { In = Ids };
            IndirectPriceListFilter.Selects = IndirectPriceListSelect.Id;
            IndirectPriceListFilter.Skip = 0;
            IndirectPriceListFilter.Take = int.MaxValue;

            List<IndirectPriceList> IndirectPriceLists = await IndirectPriceListService.List(IndirectPriceListFilter);
            IndirectPriceLists = await IndirectPriceListService.BulkDelete(IndirectPriceLists);
            if (IndirectPriceLists.Any(x => !x.IsValidated))
                return BadRequest(IndirectPriceLists.Where(x => !x.IsValidated));

            return true;
        }

        [Route(IndirectPriceListRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            IndirectPriceListTypeFilter IndirectPriceListTypeFilter = new IndirectPriceListTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectPriceListTypeSelect.ALL
            };
            List<IndirectPriceListType> IndirectPriceListTypes = await IndirectPriceListTypeService.List(IndirectPriceListTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<IndirectPriceList> IndirectPriceLists = new List<IndirectPriceList>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(IndirectPriceLists);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                int OrganizationIdColumn = 4 + StartColumn;
                int IndirectPriceListTypeIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string IndirectPriceListTypeIdValue = worksheet.Cells[i + StartRow, IndirectPriceListTypeIdColumn].Value?.ToString();

                    IndirectPriceList IndirectPriceList = new IndirectPriceList();
                    IndirectPriceList.Code = CodeValue;
                    IndirectPriceList.Name = NameValue;
                    IndirectPriceListType IndirectPriceListType = IndirectPriceListTypes.Where(x => x.Id.ToString() == IndirectPriceListTypeIdValue).FirstOrDefault();
                    IndirectPriceList.IndirectPriceListTypeId = IndirectPriceListType == null ? 0 : IndirectPriceListType.Id;
                    IndirectPriceList.IndirectPriceListType = IndirectPriceListType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    IndirectPriceList.StatusId = Status == null ? 0 : Status.Id;
                    IndirectPriceList.Status = Status;

                    IndirectPriceLists.Add(IndirectPriceList);
                }
            }
            IndirectPriceLists = await IndirectPriceListService.Import(IndirectPriceLists);
            if (IndirectPriceLists.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < IndirectPriceLists.Count; i++)
                {
                    IndirectPriceList IndirectPriceList = IndirectPriceLists[i];
                    if (!IndirectPriceList.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (IndirectPriceList.Errors.ContainsKey(nameof(IndirectPriceList.Id)))
                            Error += IndirectPriceList.Errors[nameof(IndirectPriceList.Id)];
                        if (IndirectPriceList.Errors.ContainsKey(nameof(IndirectPriceList.Code)))
                            Error += IndirectPriceList.Errors[nameof(IndirectPriceList.Code)];
                        if (IndirectPriceList.Errors.ContainsKey(nameof(IndirectPriceList.Name)))
                            Error += IndirectPriceList.Errors[nameof(IndirectPriceList.Name)];
                        if (IndirectPriceList.Errors.ContainsKey(nameof(IndirectPriceList.StatusId)))
                            Error += IndirectPriceList.Errors[nameof(IndirectPriceList.StatusId)];
                        if (IndirectPriceList.Errors.ContainsKey(nameof(IndirectPriceList.OrganizationId)))
                            Error += IndirectPriceList.Errors[nameof(IndirectPriceList.OrganizationId)];
                        if (IndirectPriceList.Errors.ContainsKey(nameof(IndirectPriceList.IndirectPriceListTypeId)))
                            Error += IndirectPriceList.Errors[nameof(IndirectPriceList.IndirectPriceListTypeId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(IndirectPriceListRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] IndirectPriceList_IndirectPriceListFilterDTO IndirectPriceList_IndirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectPriceList
                var IndirectPriceListFilter = ConvertFilterDTOToFilterEntity(IndirectPriceList_IndirectPriceListFilterDTO);
                IndirectPriceListFilter.Skip = 0;
                IndirectPriceListFilter.Take = int.MaxValue;
                IndirectPriceListFilter = IndirectPriceListService.ToFilter(IndirectPriceListFilter);
                List<IndirectPriceList> IndirectPriceLists = await IndirectPriceListService.List(IndirectPriceListFilter);

                var IndirectPriceListHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "OrganizationId",
                        "IndirectPriceListTypeId",
                    }
                };
                List<object[]> IndirectPriceListData = new List<object[]>();
                for (int i = 0; i < IndirectPriceLists.Count; i++)
                {
                    var IndirectPriceList = IndirectPriceLists[i];
                    IndirectPriceListData.Add(new Object[]
                    {
                        IndirectPriceList.Id,
                        IndirectPriceList.Code,
                        IndirectPriceList.Name,
                        IndirectPriceList.StatusId,
                        IndirectPriceList.OrganizationId,
                        IndirectPriceList.IndirectPriceListTypeId,
                    });
                }
                excel.GenerateWorksheet("IndirectPriceList", IndirectPriceListHeaders, IndirectPriceListData);
                #endregion

                #region IndirectPriceListType
                var IndirectPriceListTypeFilter = new IndirectPriceListTypeFilter();
                IndirectPriceListTypeFilter.Selects = IndirectPriceListTypeSelect.ALL;
                IndirectPriceListTypeFilter.OrderBy = IndirectPriceListTypeOrder.Id;
                IndirectPriceListTypeFilter.OrderType = OrderType.ASC;
                IndirectPriceListTypeFilter.Skip = 0;
                IndirectPriceListTypeFilter.Take = int.MaxValue;
                List<IndirectPriceListType> IndirectPriceListTypes = await IndirectPriceListTypeService.List(IndirectPriceListTypeFilter);

                var IndirectPriceListTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> IndirectPriceListTypeData = new List<object[]>();
                for (int i = 0; i < IndirectPriceListTypes.Count; i++)
                {
                    var IndirectPriceListType = IndirectPriceListTypes[i];
                    IndirectPriceListTypeData.Add(new Object[]
                    {
                        IndirectPriceListType.Id,
                        IndirectPriceListType.Code,
                        IndirectPriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("IndirectPriceListType", IndirectPriceListTypeHeaders, IndirectPriceListTypeData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectPriceList.xlsx");
        }

        [Route(IndirectPriceListRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] IndirectPriceList_IndirectPriceListFilterDTO IndirectPriceList_IndirectPriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectPriceList
                var IndirectPriceListHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "OrganizationId",
                        "IndirectPriceListTypeId",
                    }
                };
                List<object[]> IndirectPriceListData = new List<object[]>();
                excel.GenerateWorksheet("IndirectPriceList", IndirectPriceListHeaders, IndirectPriceListData);
                #endregion

                #region IndirectPriceListType
                var IndirectPriceListTypeFilter = new IndirectPriceListTypeFilter();
                IndirectPriceListTypeFilter.Selects = IndirectPriceListTypeSelect.ALL;
                IndirectPriceListTypeFilter.OrderBy = IndirectPriceListTypeOrder.Id;
                IndirectPriceListTypeFilter.OrderType = OrderType.ASC;
                IndirectPriceListTypeFilter.Skip = 0;
                IndirectPriceListTypeFilter.Take = int.MaxValue;
                List<IndirectPriceListType> IndirectPriceListTypes = await IndirectPriceListTypeService.List(IndirectPriceListTypeFilter);

                var IndirectPriceListTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> IndirectPriceListTypeData = new List<object[]>();
                for (int i = 0; i < IndirectPriceListTypes.Count; i++)
                {
                    var IndirectPriceListType = IndirectPriceListTypes[i];
                    IndirectPriceListTypeData.Add(new Object[]
                    {
                        IndirectPriceListType.Id,
                        IndirectPriceListType.Code,
                        IndirectPriceListType.Name,
                    });
                }
                excel.GenerateWorksheet("IndirectPriceListType", IndirectPriceListTypeHeaders, IndirectPriceListTypeData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectPriceList.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            IndirectPriceListFilter IndirectPriceListFilter = new IndirectPriceListFilter();
            IndirectPriceListFilter = IndirectPriceListService.ToFilter(IndirectPriceListFilter);
            if (Id == 0)
            {

            }
            else
            {
                IndirectPriceListFilter.Id = new IdFilter { Equal = Id };
                int count = await IndirectPriceListService.Count(IndirectPriceListFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private IndirectPriceList ConvertDTOToEntity(IndirectPriceList_IndirectPriceListDTO IndirectPriceList_IndirectPriceListDTO)
        {
            IndirectPriceList IndirectPriceList = new IndirectPriceList();
            IndirectPriceList.Id = IndirectPriceList_IndirectPriceListDTO.Id;
            IndirectPriceList.Code = IndirectPriceList_IndirectPriceListDTO.Code;
            IndirectPriceList.Name = IndirectPriceList_IndirectPriceListDTO.Name;
            IndirectPriceList.StartDate = IndirectPriceList_IndirectPriceListDTO.StartDate;
            IndirectPriceList.EndDate = IndirectPriceList_IndirectPriceListDTO.EndDate;
            IndirectPriceList.StatusId = IndirectPriceList_IndirectPriceListDTO.StatusId;
            IndirectPriceList.OrganizationId = IndirectPriceList_IndirectPriceListDTO.OrganizationId;
            IndirectPriceList.IndirectPriceListTypeId = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListTypeId;
            IndirectPriceList.IndirectPriceListType = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListType == null ? null : new IndirectPriceListType
            {
                Id = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListType.Id,
                Code = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListType.Code,
                Name = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListType.Name,
            };
            IndirectPriceList.Organization = IndirectPriceList_IndirectPriceListDTO.Organization == null ? null : new Organization
            {
                Id = IndirectPriceList_IndirectPriceListDTO.Organization.Id,
                Code = IndirectPriceList_IndirectPriceListDTO.Organization.Code,
                Name = IndirectPriceList_IndirectPriceListDTO.Organization.Name,
                ParentId = IndirectPriceList_IndirectPriceListDTO.Organization.ParentId,
                Path = IndirectPriceList_IndirectPriceListDTO.Organization.Path,
                Level = IndirectPriceList_IndirectPriceListDTO.Organization.Level,
                StatusId = IndirectPriceList_IndirectPriceListDTO.Organization.StatusId,
                Phone = IndirectPriceList_IndirectPriceListDTO.Organization.Phone,
                Email = IndirectPriceList_IndirectPriceListDTO.Organization.Email,
                Address = IndirectPriceList_IndirectPriceListDTO.Organization.Address,
            };
            IndirectPriceList.Status = IndirectPriceList_IndirectPriceListDTO.Status == null ? null : new Status
            {
                Id = IndirectPriceList_IndirectPriceListDTO.Status.Id,
                Code = IndirectPriceList_IndirectPriceListDTO.Status.Code,
                Name = IndirectPriceList_IndirectPriceListDTO.Status.Name,
            };
            IndirectPriceList.IndirectPriceListItemMappings = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListItemMappings?
                .Select(x => new IndirectPriceListItemMapping
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
            IndirectPriceList.IndirectPriceListStoreGroupingMappings = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListStoreGroupingMappings?
                .Select(x => new IndirectPriceListStoreGroupingMapping
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
            IndirectPriceList.IndirectPriceListStoreMappings = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListStoreMappings?
                .Select(x => new IndirectPriceListStoreMapping
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
            IndirectPriceList.IndirectPriceListStoreTypeMappings = IndirectPriceList_IndirectPriceListDTO.IndirectPriceListStoreTypeMappings?
                .Select(x => new IndirectPriceListStoreTypeMapping
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
            IndirectPriceList.BaseLanguage = CurrentContext.Language;
            return IndirectPriceList;
        }

        private IndirectPriceListFilter ConvertFilterDTOToFilterEntity(IndirectPriceList_IndirectPriceListFilterDTO IndirectPriceList_IndirectPriceListFilterDTO)
        {
            IndirectPriceListFilter IndirectPriceListFilter = new IndirectPriceListFilter();
            IndirectPriceListFilter.Selects = IndirectPriceListSelect.ALL;
            IndirectPriceListFilter.Skip = IndirectPriceList_IndirectPriceListFilterDTO.Skip;
            IndirectPriceListFilter.Take = IndirectPriceList_IndirectPriceListFilterDTO.Take;
            IndirectPriceListFilter.OrderBy = IndirectPriceList_IndirectPriceListFilterDTO.OrderBy;
            IndirectPriceListFilter.OrderType = IndirectPriceList_IndirectPriceListFilterDTO.OrderType;

            IndirectPriceListFilter.Id = IndirectPriceList_IndirectPriceListFilterDTO.Id;
            IndirectPriceListFilter.Code = IndirectPriceList_IndirectPriceListFilterDTO.Code;
            IndirectPriceListFilter.Name = IndirectPriceList_IndirectPriceListFilterDTO.Name;
            IndirectPriceListFilter.StartDate = IndirectPriceList_IndirectPriceListFilterDTO.StartDate;
            IndirectPriceListFilter.EndDate = IndirectPriceList_IndirectPriceListFilterDTO.EndDate;
            IndirectPriceListFilter.StatusId = IndirectPriceList_IndirectPriceListFilterDTO.StatusId;
            IndirectPriceListFilter.OrganizationId = IndirectPriceList_IndirectPriceListFilterDTO.OrganizationId;
            IndirectPriceListFilter.IndirectPriceListTypeId = IndirectPriceList_IndirectPriceListFilterDTO.IndirectPriceListTypeId;
            IndirectPriceListFilter.CreatedAt = IndirectPriceList_IndirectPriceListFilterDTO.CreatedAt;
            IndirectPriceListFilter.UpdatedAt = IndirectPriceList_IndirectPriceListFilterDTO.UpdatedAt;
            return IndirectPriceListFilter;
        }

        [Route(IndirectPriceListRoute.FilterListIndirectPriceListType), HttpPost]
        public async Task<List<IndirectPriceList_IndirectPriceListTypeDTO>> FilterListIndirectPriceListType([FromBody] IndirectPriceList_IndirectPriceListTypeFilterDTO IndirectPriceList_IndirectPriceListTypeFilterDTO)
        {
            IndirectPriceListTypeFilter IndirectPriceListTypeFilter = new IndirectPriceListTypeFilter();
            IndirectPriceListTypeFilter.Skip = 0;
            IndirectPriceListTypeFilter.Take = 20;
            IndirectPriceListTypeFilter.OrderBy = IndirectPriceListTypeOrder.Id;
            IndirectPriceListTypeFilter.OrderType = OrderType.ASC;
            IndirectPriceListTypeFilter.Selects = IndirectPriceListTypeSelect.ALL;
            IndirectPriceListTypeFilter.Id = IndirectPriceList_IndirectPriceListTypeFilterDTO.Id;
            IndirectPriceListTypeFilter.Code = IndirectPriceList_IndirectPriceListTypeFilterDTO.Code;
            IndirectPriceListTypeFilter.Name = IndirectPriceList_IndirectPriceListTypeFilterDTO.Name;

            List<IndirectPriceListType> IndirectPriceListTypes = await IndirectPriceListTypeService.List(IndirectPriceListTypeFilter);
            List<IndirectPriceList_IndirectPriceListTypeDTO> IndirectPriceList_IndirectPriceListTypeDTOs = IndirectPriceListTypes
                .Select(x => new IndirectPriceList_IndirectPriceListTypeDTO(x)).ToList();
            return IndirectPriceList_IndirectPriceListTypeDTOs;
        }
        [Route(IndirectPriceListRoute.FilterListOrganization), HttpPost]
        public async Task<List<IndirectPriceList_OrganizationDTO>> FilterListOrganization([FromBody] IndirectPriceList_OrganizationFilterDTO IndirectPriceList_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = IndirectPriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = IndirectPriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = IndirectPriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = IndirectPriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = IndirectPriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = IndirectPriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = IndirectPriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = IndirectPriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = IndirectPriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = IndirectPriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<IndirectPriceList_OrganizationDTO> IndirectPriceList_OrganizationDTOs = Organizations
                .Select(x => new IndirectPriceList_OrganizationDTO(x)).ToList();
            return IndirectPriceList_OrganizationDTOs;
        }
        [Route(IndirectPriceListRoute.FilterListStatus), HttpPost]
        public async Task<List<IndirectPriceList_StatusDTO>> FilterListStatus([FromBody] IndirectPriceList_StatusFilterDTO IndirectPriceList_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<IndirectPriceList_StatusDTO> IndirectPriceList_StatusDTOs = Statuses
                .Select(x => new IndirectPriceList_StatusDTO(x)).ToList();
            return IndirectPriceList_StatusDTOs;
        }
        [Route(IndirectPriceListRoute.FilterListItem), HttpPost]
        public async Task<List<IndirectPriceList_ItemDTO>> FilterListItem([FromBody] IndirectPriceList_ItemFilterDTO IndirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectPriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectPriceList_ItemDTO> IndirectPriceList_ItemDTOs = Items
                .Select(x => new IndirectPriceList_ItemDTO(x)).ToList();
            return IndirectPriceList_ItemDTOs;
        }
        [Route(IndirectPriceListRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<IndirectPriceList_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] IndirectPriceList_StoreGroupingFilterDTO IndirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = IndirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = IndirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = IndirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = IndirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = IndirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = IndirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = IndirectPriceList_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<IndirectPriceList_StoreGroupingDTO> IndirectPriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new IndirectPriceList_StoreGroupingDTO(x)).ToList();
            return IndirectPriceList_StoreGroupingDTOs;
        }
        [Route(IndirectPriceListRoute.FilterListStore), HttpPost]
        public async Task<List<IndirectPriceList_StoreDTO>> FilterListStore([FromBody] IndirectPriceList_StoreFilterDTO IndirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = IndirectPriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = IndirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = IndirectPriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectPriceList_StoreDTO> IndirectPriceList_StoreDTOs = Stores
                .Select(x => new IndirectPriceList_StoreDTO(x)).ToList();
            return IndirectPriceList_StoreDTOs;
        }
        [Route(IndirectPriceListRoute.FilterListStoreType), HttpPost]
        public async Task<List<IndirectPriceList_StoreTypeDTO>> FilterListStoreType([FromBody] IndirectPriceList_StoreTypeFilterDTO IndirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = IndirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = IndirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = IndirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = IndirectPriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<IndirectPriceList_StoreTypeDTO> IndirectPriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new IndirectPriceList_StoreTypeDTO(x)).ToList();
            return IndirectPriceList_StoreTypeDTOs;
        }

        [Route(IndirectPriceListRoute.SingleListIndirectPriceListType), HttpPost]
        public async Task<List<IndirectPriceList_IndirectPriceListTypeDTO>> SingleListIndirectPriceListType([FromBody] IndirectPriceList_IndirectPriceListTypeFilterDTO IndirectPriceList_IndirectPriceListTypeFilterDTO)
        {
            IndirectPriceListTypeFilter IndirectPriceListTypeFilter = new IndirectPriceListTypeFilter();
            IndirectPriceListTypeFilter.Skip = 0;
            IndirectPriceListTypeFilter.Take = 20;
            IndirectPriceListTypeFilter.OrderBy = IndirectPriceListTypeOrder.Id;
            IndirectPriceListTypeFilter.OrderType = OrderType.ASC;
            IndirectPriceListTypeFilter.Selects = IndirectPriceListTypeSelect.ALL;
            IndirectPriceListTypeFilter.Id = IndirectPriceList_IndirectPriceListTypeFilterDTO.Id;
            IndirectPriceListTypeFilter.Code = IndirectPriceList_IndirectPriceListTypeFilterDTO.Code;
            IndirectPriceListTypeFilter.Name = IndirectPriceList_IndirectPriceListTypeFilterDTO.Name;

            List<IndirectPriceListType> IndirectPriceListTypes = await IndirectPriceListTypeService.List(IndirectPriceListTypeFilter);
            List<IndirectPriceList_IndirectPriceListTypeDTO> IndirectPriceList_IndirectPriceListTypeDTOs = IndirectPriceListTypes
                .Select(x => new IndirectPriceList_IndirectPriceListTypeDTO(x)).ToList();
            return IndirectPriceList_IndirectPriceListTypeDTOs;
        }
        [Route(IndirectPriceListRoute.SingleListOrganization), HttpPost]
        public async Task<List<IndirectPriceList_OrganizationDTO>> SingleListOrganization([FromBody] IndirectPriceList_OrganizationFilterDTO IndirectPriceList_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = IndirectPriceList_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = IndirectPriceList_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = IndirectPriceList_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = IndirectPriceList_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = IndirectPriceList_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = IndirectPriceList_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = IndirectPriceList_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = IndirectPriceList_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = IndirectPriceList_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = IndirectPriceList_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<IndirectPriceList_OrganizationDTO> IndirectPriceList_OrganizationDTOs = Organizations
                .Select(x => new IndirectPriceList_OrganizationDTO(x)).ToList();
            return IndirectPriceList_OrganizationDTOs;
        }
        [Route(IndirectPriceListRoute.SingleListStatus), HttpPost]
        public async Task<List<IndirectPriceList_StatusDTO>> SingleListStatus([FromBody] IndirectPriceList_StatusFilterDTO IndirectPriceList_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<IndirectPriceList_StatusDTO> IndirectPriceList_StatusDTOs = Statuses
                .Select(x => new IndirectPriceList_StatusDTO(x)).ToList();
            return IndirectPriceList_StatusDTOs;
        }
        [Route(IndirectPriceListRoute.SingleListItem), HttpPost]
        public async Task<List<IndirectPriceList_ItemDTO>> SingleListItem([FromBody] IndirectPriceList_ItemFilterDTO IndirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectPriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectPriceList_ItemDTO> IndirectPriceList_ItemDTOs = Items
                .Select(x => new IndirectPriceList_ItemDTO(x)).ToList();
            return IndirectPriceList_ItemDTOs;
        }
        [Route(IndirectPriceListRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<IndirectPriceList_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] IndirectPriceList_StoreGroupingFilterDTO IndirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = IndirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = IndirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = IndirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = IndirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = IndirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = IndirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = IndirectPriceList_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<IndirectPriceList_StoreGroupingDTO> IndirectPriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new IndirectPriceList_StoreGroupingDTO(x)).ToList();
            return IndirectPriceList_StoreGroupingDTOs;
        }
        [Route(IndirectPriceListRoute.SingleListStore), HttpPost]
        public async Task<List<IndirectPriceList_StoreDTO>> SingleListStore([FromBody] IndirectPriceList_StoreFilterDTO IndirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = IndirectPriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = IndirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = IndirectPriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectPriceList_StoreDTO> IndirectPriceList_StoreDTOs = Stores
                .Select(x => new IndirectPriceList_StoreDTO(x)).ToList();
            return IndirectPriceList_StoreDTOs;
        }
        [Route(IndirectPriceListRoute.SingleListStoreType), HttpPost]
        public async Task<List<IndirectPriceList_StoreTypeDTO>> SingleListStoreType([FromBody] IndirectPriceList_StoreTypeFilterDTO IndirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = IndirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = IndirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = IndirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = IndirectPriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<IndirectPriceList_StoreTypeDTO> IndirectPriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new IndirectPriceList_StoreTypeDTO(x)).ToList();
            return IndirectPriceList_StoreTypeDTOs;
        }

        [Route(IndirectPriceListRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] IndirectPriceList_ItemFilterDTO IndirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = IndirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectPriceList_ItemFilterDTO.StatusId;

            return await ItemService.Count(ItemFilter);
        }

        [Route(IndirectPriceListRoute.ListItem), HttpPost]
        public async Task<List<IndirectPriceList_ItemDTO>> ListItem([FromBody] IndirectPriceList_ItemFilterDTO IndirectPriceList_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = IndirectPriceList_ItemFilterDTO.Skip;
            ItemFilter.Take = IndirectPriceList_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectPriceList_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectPriceList_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectPriceList_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectPriceList_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectPriceList_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectPriceList_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectPriceList_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectPriceList_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectPriceList_ItemDTO> IndirectPriceList_ItemDTOs = Items
                .Select(x => new IndirectPriceList_ItemDTO(x)).ToList();
            return IndirectPriceList_ItemDTOs;
        }
        [Route(IndirectPriceListRoute.CountStoreGrouping), HttpPost]
        public async Task<long> CountStoreGrouping([FromBody] IndirectPriceList_StoreGroupingFilterDTO IndirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Id = IndirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = IndirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = IndirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = IndirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = IndirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = IndirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = IndirectPriceList_StoreGroupingFilterDTO.StatusId;

            return await StoreGroupingService.Count(StoreGroupingFilter);
        }

        [Route(IndirectPriceListRoute.ListStoreGrouping), HttpPost]
        public async Task<List<IndirectPriceList_StoreGroupingDTO>> ListStoreGrouping([FromBody] IndirectPriceList_StoreGroupingFilterDTO IndirectPriceList_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = IndirectPriceList_StoreGroupingFilterDTO.Skip;
            StoreGroupingFilter.Take = IndirectPriceList_StoreGroupingFilterDTO.Take;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = IndirectPriceList_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = IndirectPriceList_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = IndirectPriceList_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = IndirectPriceList_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = IndirectPriceList_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = IndirectPriceList_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.StatusId = IndirectPriceList_StoreGroupingFilterDTO.StatusId;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<IndirectPriceList_StoreGroupingDTO> IndirectPriceList_StoreGroupingDTOs = StoreGroupings
                .Select(x => new IndirectPriceList_StoreGroupingDTO(x)).ToList();
            return IndirectPriceList_StoreGroupingDTOs;
        }
        [Route(IndirectPriceListRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] IndirectPriceList_StoreFilterDTO IndirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = IndirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = IndirectPriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = IndirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = IndirectPriceList_StoreFilterDTO.RequestStateId;

            return await StoreService.Count(StoreFilter);
        }

        [Route(IndirectPriceListRoute.ListStore), HttpPost]
        public async Task<List<IndirectPriceList_StoreDTO>> ListStore([FromBody] IndirectPriceList_StoreFilterDTO IndirectPriceList_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = IndirectPriceList_StoreFilterDTO.Skip;
            StoreFilter.Take = IndirectPriceList_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectPriceList_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectPriceList_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectPriceList_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectPriceList_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectPriceList_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectPriceList_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectPriceList_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectPriceList_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectPriceList_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectPriceList_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectPriceList_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectPriceList_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectPriceList_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectPriceList_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectPriceList_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectPriceList_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectPriceList_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectPriceList_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectPriceList_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectPriceList_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectPriceList_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = IndirectPriceList_StoreFilterDTO.StatusId;
            StoreFilter.WorkflowDefinitionId = IndirectPriceList_StoreFilterDTO.WorkflowDefinitionId;
            StoreFilter.RequestStateId = IndirectPriceList_StoreFilterDTO.RequestStateId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectPriceList_StoreDTO> IndirectPriceList_StoreDTOs = Stores
                .Select(x => new IndirectPriceList_StoreDTO(x)).ToList();
            return IndirectPriceList_StoreDTOs;
        }
        [Route(IndirectPriceListRoute.CountStoreType), HttpPost]
        public async Task<long> CountStoreType([FromBody] IndirectPriceList_StoreTypeFilterDTO IndirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Id = IndirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = IndirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = IndirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = IndirectPriceList_StoreTypeFilterDTO.StatusId;

            return await StoreTypeService.Count(StoreTypeFilter);
        }

        [Route(IndirectPriceListRoute.ListStoreType), HttpPost]
        public async Task<List<IndirectPriceList_StoreTypeDTO>> ListStoreType([FromBody] IndirectPriceList_StoreTypeFilterDTO IndirectPriceList_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = IndirectPriceList_StoreTypeFilterDTO.Skip;
            StoreTypeFilter.Take = IndirectPriceList_StoreTypeFilterDTO.Take;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = IndirectPriceList_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = IndirectPriceList_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = IndirectPriceList_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = IndirectPriceList_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<IndirectPriceList_StoreTypeDTO> IndirectPriceList_StoreTypeDTOs = StoreTypes
                .Select(x => new IndirectPriceList_StoreTypeDTO(x)).ToList();
            return IndirectPriceList_StoreTypeDTOs;
        }
    }
}

