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
using DMS.Services.MSalesOrderType;
using DMS.Services.MProduct;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Enums;
using DMS.Services.MProvince;
using DMS.Services.MAppUser;
using System.Text;
using DMS.Services.MPriceListItemHistory;

namespace DMS.Rpc.price_list
{
    public partial class PriceListController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IItemService ItemService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreService StoreService;
        private IStoreTypeService StoreTypeService;
        private IPriceListTypeService PriceListTypeService;
        private IPriceListItemHistoryService PriceListItemHistoryService;
        private ISalesOrderTypeService SalesOrderTypeService;
        private IStatusService StatusService;
        private IPriceListService PriceListService;
        private IProductTypeService ProductTypeService;
        private IProvinceService ProvinceService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public PriceListController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IStoreGroupingService StoreGroupingService,
            IStoreService StoreService,
            IStoreTypeService StoreTypeService,
            IPriceListTypeService PriceListTypeService,
            IPriceListItemHistoryService PriceListItemHistoryService,
            ISalesOrderTypeService SalesOrderTypeService,
            IStatusService StatusService,
            IPriceListService PriceListService,
            IProvinceService ProvinceService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.ItemService = ItemService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreService = StoreService;
            this.StoreTypeService = StoreTypeService;
            this.PriceListTypeService = PriceListTypeService;
            this.PriceListItemHistoryService = PriceListItemHistoryService;
            this.SalesOrderTypeService = SalesOrderTypeService;
            this.StatusService = StatusService;
            this.PriceListService = PriceListService;
            this.ProvinceService = ProvinceService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PriceListRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            int count = await PriceListService.Count(PriceListFilter);
            return count;
        }

        [Route(PriceListRoute.List), HttpPost]
        public async Task<ActionResult<List<PriceList_PriceListDTO>>> List([FromBody] PriceList_PriceListFilterDTO PriceList_PriceListFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PriceListFilter PriceListFilter = ConvertFilterDTOToFilterEntity(PriceList_PriceListFilterDTO);
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            List<PriceList_PriceListDTO> PriceList_PriceListDTOs = PriceLists
                .Select(c => new PriceList_PriceListDTO(c)).ToList();
            return PriceList_PriceListDTOs;
        }

        [Route(PriceListRoute.Get), HttpPost]
        public async Task<ActionResult<PriceList_PriceListDTO>> Get([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
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
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
            PriceListFilter.Id = new IdFilter { In = Ids };
            PriceListFilter.Selects = PriceListSelect.Id;
            PriceListFilter.Skip = 0;
            PriceListFilter.Take = int.MaxValue;

            List<PriceList> PriceLists = await PriceListService.List(PriceListFilter);
            PriceLists = await PriceListService.BulkDelete(PriceLists);
            if (PriceLists.Any(x => !x.IsValidated))
                return BadRequest(PriceLists.Where(x => !x.IsValidated));
            return true;
        }

        [Route(PriceListRoute.ImportItem), HttpPost]
        public async Task<ActionResult<List<PriceList_PriceListItemMappingDTO>>> ImportItem([FromForm] long PriceListId, [FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceListId))
                return Forbid();

            PriceList PriceList = await PriceListService.Get(PriceListId);
            if (PriceList == null)
                PriceList = new PriceList();
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            ItemFilter ItemFilter = new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.ALL
            };

            List<Item> Items = await ItemService.List(ItemFilter);
            StringBuilder errorContent = new StringBuilder();
            PriceList.PriceListItemMappings = new List<PriceListItemMapping>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Gia ban"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");
                int StartColumn = 1;
                int StartRow = 1;
                int SttColumnn = 0 + StartColumn;
                int ItemCodeColumn = 1 + StartColumn;
                int PriceColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string stt = worksheet.Cells[i + StartRow, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;

                    string ItemCodeValue = worksheet.Cells[i + StartRow, ItemCodeColumn].Value?.ToString();
                    string PriceValue = worksheet.Cells[i + StartRow, PriceColumn].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(ItemCodeValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập mã sản phẩm");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(ItemCodeValue) && i == worksheet.Dimension.End.Row)
                        break;

                    PriceListItemMapping PriceListItemMapping = new PriceListItemMapping();
                    var Item = Items.Where(x => x.Code == ItemCodeValue).FirstOrDefault();
                    if(Item == null)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Mã sản phẩm không tồn tại");
                        continue;
                    }
                    PriceListItemMapping.PriceListId = PriceList.Id;
                    PriceListItemMapping.ItemId = Item.Id;
                    PriceListItemMapping.Item = Item;
                    if (long.TryParse(PriceValue, out long Price))
                    {
                        PriceListItemMapping.Price = Price;
                        if(Price < 0)
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Giá bán không hợp lệ");
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập giá bán");
                    }

                    PriceList.PriceListItemMappings.Add(PriceListItemMapping);
                }
                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
            }
            PriceList.PriceListItemMappings = PriceList.PriceListItemMappings.Distinct().ToList();
            List<PriceList_PriceListItemMappingDTO> PriceList_PriceListItemMappingDTOs = PriceList.PriceListItemMappings
                 .Select(c => new PriceList_PriceListItemMappingDTO(c)).ToList();
            return PriceList_PriceListItemMappingDTOs;
        }

        [Route(PriceListRoute.ImportStore), HttpPost]
        public async Task<ActionResult<List<PriceList_PriceListStoreMappingDTO>>> ImportStore([FromForm] long PriceListId, [FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PriceListId))
                return Forbid();

            PriceList PriceList = await PriceListService.Get(PriceListId);
            if (PriceList == null)
                PriceList = new PriceList();
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.ALL
            };

            List<Store> Stores = await StoreService.List(StoreFilter);
            StringBuilder errorContent = new StringBuilder();
            PriceList.PriceListStoreMappings = new List<PriceListStoreMapping>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Daily_Apdung"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");
                int StartColumn = 1;
                int StartRow = 1;
                int SttColumnn = 0 + StartColumn;
                int StoreCodeColumn = 1 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string stt = worksheet.Cells[i + StartRow, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;

                    string StoreCodeValue = worksheet.Cells[i + StartRow, StoreCodeColumn].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(StoreCodeValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập mã đại lý");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(StoreCodeValue) && i == worksheet.Dimension.End.Row)
                        break;

                    PriceListStoreMapping PriceListStoreMapping = new PriceListStoreMapping();
                    var Store = Stores.Where(x => x.Code == StoreCodeValue).FirstOrDefault();
                    if(Store == null)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Mã đại lý không tồn tại");
                        continue;
                    }
                    PriceListStoreMapping.PriceListId = PriceList.Id;
                    PriceListStoreMapping.StoreId = Store.Id;
                    PriceListStoreMapping.Store = Store;
                    PriceList.PriceListStoreMappings.Add(PriceListStoreMapping);
                }
                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
            }
            PriceList.PriceListStoreMappings = PriceList.PriceListStoreMappings.Distinct().ToList();

            List<PriceList_PriceListStoreMappingDTO> PriceList_PriceListStoreMappingDTOs = PriceList.PriceListStoreMappings
                 .Select(c => new PriceList_PriceListStoreMappingDTO(c)).ToList();
            return PriceList_PriceListStoreMappingDTOs;
        }

        [Route(PriceListRoute.ExportItem), HttpPost]
        public async Task<ActionResult> ExportItem([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long PriceListId = PriceList_PriceListDTO?.Id ?? 0;
            PriceList PriceList = await PriceListService.Get(PriceListId);
            if (PriceList == null)
                return null;

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var PriceListItemMappingHeader = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã sản phẩm",
                        "Tên sản phẩm",
                        "Giá bán"
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < PriceList.PriceListItemMappings.Count; i++)
                {
                    var PriceListItemMapping = PriceList.PriceListItemMappings[i];
                    data.Add(new Object[]
                    {
                        i+1,
                        PriceListItemMapping.Item?.Code,
                        PriceListItemMapping.Item?.Name,
                        PriceListItemMapping.Price,
                    });
                }
                excel.GenerateWorksheet("Gia ban", PriceListItemMappingHeader, data);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", $"{PriceList.Code}_Item.xlsx");
        }

        [Route(PriceListRoute.ExportStore), HttpPost]
        public async Task<ActionResult> ExportStore([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long PriceListId = PriceList_PriceListDTO?.Id ?? 0;
            PriceList PriceList = await PriceListService.Get(PriceListId);
            if (PriceList == null)
                return null;

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var PriceListStoreMappingHeader = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã đại lý",
                        "Tên đại lý"
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < PriceList.PriceListStoreMappings.Count; i++)
                {
                    var PriceListStoreMapping = PriceList.PriceListStoreMappings[i];
                    data.Add(new Object[]
                    {
                        i+1,
                        PriceListStoreMapping.Store?.Code,
                        PriceListStoreMapping.Store?.Name,
                    });
                }
                excel.GenerateWorksheet("Daily_Apdung", PriceListStoreMappingHeader, data);

                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", $"{PriceList.Code}_Store.xlsx");
        }

        [Route(PriceListRoute.ExportTemplateItem), HttpPost]
        public async Task<ActionResult> ExportTemplateItem([FromBody] PriceList_PriceListDTO PriceList_PriceListDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var ItemFilter = new ItemFilter
            {
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name,
                Skip = 0,
                Take = int.MaxValue,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var Items = await ItemService.List(ItemFilter);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            string tempPath = "Templates/Pricelist_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(tempPath);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            using (var xlPackage = new ExcelPackage(input))
            {
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow = 2;
                int numberCell = 1;
                var priceListSheet = xlPackage.Workbook.Worksheets["Gia ban"];

                long PriceListId = PriceList_PriceListDTO?.Id ?? 0;
                PriceList PriceList = await PriceListService.Get(PriceListId);
                if (PriceList == null)
                {
                    priceListSheet.Cells[startRow , numberCell].Value = "END";
                }
                else
                {
                    for (var i = 0; i < PriceList.PriceListItemMappings.Count; i++)
                    {
                        PriceListItemMapping PriceListItemMapping = PriceList.PriceListItemMappings[i];
                        priceListSheet.Cells[startRow + i, numberCell].Value = i + 1;
                        priceListSheet.Cells[startRow + i, numberCell + 1].Value = PriceListItemMapping.Item.Code;
                        priceListSheet.Cells[startRow + i, numberCell + 2].Value = PriceListItemMapping.Price;
                    }

                    priceListSheet.Cells[startRow + PriceList.PriceListItemMappings.Count, numberCell].Value = "END";
                }
                
                var itemSheet = xlPackage.Workbook.Worksheets["San pham"];
                for (var i = 0; i < Items.Count; i++)
                {
                    Item Item = Items[i];
                    itemSheet.Cells[startRow + i, numberCell].Value = Item.Code;
                    itemSheet.Cells[startRow + i, numberCell + 1].Value = Item.Name;
                }
                
                xlPackage.SaveAs(output);
            }
            return File(output.ToArray(), "application/octet-stream", "Template_PriceList_Item.xlsx");
        }

        [Route(PriceListRoute.ExportTemplateStore), HttpPost]
        public async Task<ActionResult> ExportTemplateStore()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var appUser = await AppUserService.Get(CurrentContext.UserId);
            var StoreFilter = new StoreFilter
            {
                Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name,
                Skip = 0,
                Take = int.MaxValue,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                OrganizationId = new IdFilter { Equal = appUser.OrganizationId }
            };
            var Stores = await StoreService.List(StoreFilter);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string tempPath = "Templates/Pricelist_Store.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(tempPath);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            using (var xlPackage = new ExcelPackage(input))
            {
                var worksheet = xlPackage.Workbook.Worksheets["Daily"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow = 2;
                int numberCell = 1;
                for (var i = 0; i < Stores.Count; i++)
                {
                    Store Store = Stores[i];
                    worksheet.Cells[startRow + i, numberCell].Value = Store.Code;
                    worksheet.Cells[startRow + i, numberCell + 1].Value = Store.Name;
                }
                xlPackage.SaveAs(output);
            }
            return File(output.ToArray(), "application/octet-stream", "Template_PriceList_Store.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PriceListFilter PriceListFilter = new PriceListFilter();
            PriceListFilter = await PriceListService.ToFilter(PriceListFilter);
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
            PriceList.StartDate = PriceList_PriceListDTO.StartDate;
            PriceList.EndDate = PriceList_PriceListDTO.EndDate;
            PriceList.StatusId = PriceList_PriceListDTO.StatusId;
            PriceList.OrganizationId = PriceList_PriceListDTO.OrganizationId;
            PriceList.PriceListTypeId = PriceList_PriceListDTO.PriceListTypeId;
            PriceList.SalesOrderTypeId = PriceList_PriceListDTO.SalesOrderTypeId;
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
            PriceList.SalesOrderType = PriceList_PriceListDTO.SalesOrderType == null ? null : new SalesOrderType
            {
                Id = PriceList_PriceListDTO.SalesOrderType.Id,
                Code = PriceList_PriceListDTO.SalesOrderType.Code,
                Name = PriceList_PriceListDTO.SalesOrderType.Name,
            };
            PriceList.Status = PriceList_PriceListDTO.Status == null ? null : new Status
            {
                Id = PriceList_PriceListDTO.Status.Id,
                Code = PriceList_PriceListDTO.Status.Code,
                Name = PriceList_PriceListDTO.Status.Name,
            };
            PriceList.PriceListItemMappings = PriceList_PriceListDTO.PriceListItemMappings?.Select(x => new PriceListItemMapping
            {
                ItemId = x.ItemId,
                PriceListId = x.PriceListId,
                Price = x.Price,
                Item = new Item
                {
                    Id = x.Item.Id,
                    ProductId = x.Item.ProductId,
                    Code = x.Item.Code,
                    Name = x.Item.Name,
                    ScanCode = x.Item.ScanCode,
                    SalePrice = x.Item.SalePrice,
                    RetailPrice = x.Item.RetailPrice,
                    StatusId = x.Item.StatusId,
                    Product = x.Item.Product == null ? null : new Product
                    {
                        Id = x.Item.Product.Id,
                        ProductProductGroupingMappings = x.Item.Product.ProductProductGroupingMappings != null ?
                            x.Item.Product.ProductProductGroupingMappings.Select(p => new ProductProductGroupingMapping
                            {
                                ProductId = p.ProductId,
                                ProductGroupingId = p.ProductGroupingId,
                                ProductGrouping = new ProductGrouping
                                {
                                    Id = p.ProductGrouping.Id,
                                    Code = p.ProductGrouping.Code,
                                    Name = p.ProductGrouping.Name,
                                    ParentId = p.ProductGrouping.ParentId,
                                    Path = p.ProductGrouping.Path,
                                    Description = p.ProductGrouping.Description,
                                },
                            }).ToList() : null,
                    }
                },
                PriceListItemHistories = x.PriceListItemHistories?.Select(x => new PriceListItemHistory
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    PriceListId = x.PriceListId,
                    ModifierId = x.ModifierId,
                    NewPrice = x.NewPrice,
                    OldPrice = x.OldPrice,
                    UpdatedAt = x.UpdatedAt,
                }).ToList()
            }).ToList();
            PriceList.PriceListStoreMappings = PriceList_PriceListDTO.PriceListStoreMappings?.Select(x => new PriceListStoreMapping
            {
                StoreId = x.StoreId,
                PriceListId = x.PriceListId,
                Store = new Store
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
                    DeliveryLatitude = x.Store.DeliveryLatitude,
                    DeliveryLongitude = x.Store.DeliveryLongitude,
                    OwnerName = x.Store.OwnerName,
                    OwnerPhone = x.Store.OwnerPhone,
                    OwnerEmail = x.Store.OwnerEmail,
                    TaxCode = x.Store.TaxCode,
                    LegalEntity = x.Store.LegalEntity,
                    StatusId = x.Store.StatusId,
                    Province = x.Store.Province == null ? null : new Province
                    {
                        Id = x.Store.Province.Id,
                        Name = x.Store.Province.Name,
                        Priority = x.Store.Province.Priority,
                        StatusId = x.Store.Province.StatusId,
                    },
                    StoreGrouping = x.Store.StoreGrouping == null ? null : new StoreGrouping
                    {
                        Id = x.Store.StoreGrouping.Id,
                        Code = x.Store.StoreGrouping.Code,
                        Name = x.Store.StoreGrouping.Name,
                        ParentId = x.Store.StoreGrouping.ParentId,
                        Path = x.Store.StoreGrouping.Path,
                        Level = x.Store.StoreGrouping.Level,
                    },
                    StoreType = x.Store.StoreType == null ? null : new StoreType
                    {
                        Id = x.Store.StoreType.Id,
                        Code = x.Store.StoreType.Code,
                        Name = x.Store.StoreType.Name,
                        StatusId = x.Store.StoreType.StatusId,
                    },
                },
            }).ToList();
            PriceList.PriceListStoreTypeMappings = PriceList_PriceListDTO.PriceListStoreTypeMappings?.Select(x => new PriceListStoreTypeMapping
            {
                StoreTypeId = x.StoreTypeId,
                PriceListId = x.PriceListId,
                StoreType = new StoreType
                {
                    Id = x.StoreType.Id,
                    Code = x.StoreType.Code,
                    Name = x.StoreType.Name,
                    StatusId = x.StoreType.StatusId,
                },
            }).ToList();
            PriceList.PriceListStoreGroupingMappings = PriceList_PriceListDTO.PriceListStoreGroupingMappings?.Select(x => new PriceListStoreGroupingMapping
            {
                StoreGroupingId = x.StoreGroupingId,
                PriceListId = x.PriceListId,
                StoreGrouping = new StoreGrouping
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
            PriceListFilter.StartDate = PriceList_PriceListFilterDTO.StartDate;
            PriceListFilter.EndDate = PriceList_PriceListFilterDTO.EndDate;
            PriceListFilter.StatusId = PriceList_PriceListFilterDTO.StatusId;
            PriceListFilter.OrganizationId = PriceList_PriceListFilterDTO.OrganizationId;
            PriceListFilter.PriceListTypeId = PriceList_PriceListFilterDTO.PriceListTypeId;
            PriceListFilter.SalesOrderTypeId = PriceList_PriceListFilterDTO.SalesOrderTypeId;
            PriceListFilter.CreatedAt = PriceList_PriceListFilterDTO.CreatedAt;
            PriceListFilter.UpdatedAt = PriceList_PriceListFilterDTO.UpdatedAt;
            return PriceListFilter;
        }
    }
}

