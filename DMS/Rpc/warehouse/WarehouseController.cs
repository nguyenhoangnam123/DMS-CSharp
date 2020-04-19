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
using DMS.Services.MWarehouse;
using DMS.Services.MDistrict;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MWard;
using DMS.Services.MItem;
using DMS.Enums;
using DMS.Services.MOrganization;

namespace DMS.Rpc.warehouse
{
    public class WarehouseRoute : Root
    {
        public const string Master = Module + "/warehouse/warehouse-master";
        public const string Detail = Module + "/warehouse/warehouse-detail";
        private const string Default = Rpc + Module + "/warehouse";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListItem = Default + "/single-list-item";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(WarehouseFilter.Id), FieldType.ID },
            { nameof(WarehouseFilter.Code), FieldType.STRING },
            { nameof(WarehouseFilter.Name), FieldType.STRING },
            { nameof(WarehouseFilter.Address), FieldType.STRING },
            { nameof(WarehouseFilter.OrganizationId), FieldType.ID },
            { nameof(WarehouseFilter.ProvinceId), FieldType.ID },
            { nameof(WarehouseFilter.DistrictId), FieldType.ID },
            { nameof(WarehouseFilter.WardId), FieldType.ID },
            { nameof(WarehouseFilter.StatusId), FieldType.ID },
        };
    }

    public class WarehouseController : RpcController
    {
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private IWardService WardService;
        private IItemService ItemService;
        private IWarehouseService WarehouseService;
        private ICurrentContext CurrentContext;
        public WarehouseController(
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            IWardService WardService,
            IItemService ItemService,
            IWarehouseService WarehouseService,
            ICurrentContext CurrentContext
        )
        {
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.WardService = WardService;
            this.ItemService = ItemService;
            this.WarehouseService = WarehouseService;
            this.CurrentContext = CurrentContext;
        }

        [Route(WarehouseRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WarehouseFilter WarehouseFilter = ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO);
            WarehouseFilter = WarehouseService.ToFilter(WarehouseFilter);
            int count = await WarehouseService.Count(WarehouseFilter);
            return count;
        }

        [Route(WarehouseRoute.List), HttpPost]
        public async Task<ActionResult<List<Warehouse_WarehouseDTO>>> List([FromBody] Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WarehouseFilter WarehouseFilter = ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO);
            WarehouseFilter = WarehouseService.ToFilter(WarehouseFilter);
            List<Warehouse> Warehouses = await WarehouseService.List(WarehouseFilter);
            List<Warehouse_WarehouseDTO> Warehouse_WarehouseDTOs = Warehouses
                .Select(c => new Warehouse_WarehouseDTO(c)).ToList();
            return Warehouse_WarehouseDTOs;
        }

        [Route(WarehouseRoute.Get), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Get([FromBody]Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = await WarehouseService.Get(Warehouse_WarehouseDTO.Id);
            return new Warehouse_WarehouseDTO(Warehouse);
        }

        [Route(WarehouseRoute.Create), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Create([FromBody] Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = ConvertDTOToEntity(Warehouse_WarehouseDTO);
            Warehouse = await WarehouseService.Create(Warehouse);
            Warehouse_WarehouseDTO = new Warehouse_WarehouseDTO(Warehouse);
            if (Warehouse.IsValidated)
                return Warehouse_WarehouseDTO;
            else
                return BadRequest(Warehouse_WarehouseDTO);
        }

        [Route(WarehouseRoute.Update), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Update([FromBody] Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = ConvertDTOToEntity(Warehouse_WarehouseDTO);
            Warehouse = await WarehouseService.Update(Warehouse);
            Warehouse_WarehouseDTO = new Warehouse_WarehouseDTO(Warehouse);
            if (Warehouse.IsValidated)
                return Warehouse_WarehouseDTO;
            else
                return BadRequest(Warehouse_WarehouseDTO);
        }

        [Route(WarehouseRoute.Delete), HttpPost]
        public async Task<ActionResult<Warehouse_WarehouseDTO>> Delete([FromBody] Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Warehouse_WarehouseDTO.Id))
                return Forbid();

            Warehouse Warehouse = ConvertDTOToEntity(Warehouse_WarehouseDTO);
            Warehouse = await WarehouseService.Delete(Warehouse);
            Warehouse_WarehouseDTO = new Warehouse_WarehouseDTO(Warehouse);
            if (Warehouse.IsValidated)
                return Warehouse_WarehouseDTO;
            else
                return BadRequest(Warehouse_WarehouseDTO);
        }
        
        [Route(WarehouseRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter = WarehouseService.ToFilter(WarehouseFilter);
            WarehouseFilter.Id = new IdFilter { In = Ids };
            WarehouseFilter.Selects = WarehouseSelect.Id;
            WarehouseFilter.Skip = 0;
            WarehouseFilter.Take = int.MaxValue;

            List<Warehouse> Warehouses = await WarehouseService.List(WarehouseFilter);
            Warehouses = await WarehouseService.BulkDelete(Warehouses);
            return true;
        }
        
        [Route(WarehouseRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DistrictFilter DistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.ALL
            };
            List<District> Districts = await DistrictService.List(DistrictFilter);
            ProvinceFilter ProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.ALL
            };
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            WardFilter WardFilter = new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.ALL
            };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Warehouse> Warehouses = new List<Warehouse>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Warehouses);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int AddressColumn = 3 + StartColumn;
                int OrganizationIdColumn = 4 + StartColumn;
                int ProvinceIdColumn = 5 + StartColumn;
                int DistrictIdColumn = 6 + StartColumn;
                int WardIdColumn = 7 + StartColumn;
                int StatusIdColumn = 8 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i + StartRow, AddressColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string ProvinceIdValue = worksheet.Cells[i + StartRow, ProvinceIdColumn].Value?.ToString();
                    string DistrictIdValue = worksheet.Cells[i + StartRow, DistrictIdColumn].Value?.ToString();
                    string WardIdValue = worksheet.Cells[i + StartRow, WardIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    
                    Warehouse Warehouse = new Warehouse();
                    Warehouse.Code = CodeValue;
                    Warehouse.Name = NameValue;
                    Warehouse.Address = AddressValue;
                    District District = Districts.Where(x => x.Id.ToString() == DistrictIdValue).FirstOrDefault();
                    Warehouse.DistrictId = District == null ? 0 : District.Id;
                    Warehouse.District = District;
                    Province Province = Provinces.Where(x => x.Id.ToString() == ProvinceIdValue).FirstOrDefault();
                    Warehouse.ProvinceId = Province == null ? 0 : Province.Id;
                    Warehouse.Province = Province;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    Warehouse.StatusId = Status == null ? 0 : Status.Id;
                    Warehouse.Status = Status;
                    Ward Ward = Wards.Where(x => x.Id.ToString() == WardIdValue).FirstOrDefault();
                    Warehouse.WardId = Ward == null ? 0 : Ward.Id;
                    Warehouse.Ward = Ward;
                    
                    Warehouses.Add(Warehouse);
                }
            }
            Warehouses = await WarehouseService.Import(Warehouses);
            if (Warehouses.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Warehouses.Count; i++)
                {
                    Warehouse Warehouse = Warehouses[i];
                    if (!Warehouse.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.Id)))
                            Error += Warehouse.Errors[nameof(Warehouse.Id)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.Code)))
                            Error += Warehouse.Errors[nameof(Warehouse.Code)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.Name)))
                            Error += Warehouse.Errors[nameof(Warehouse.Name)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.Address)))
                            Error += Warehouse.Errors[nameof(Warehouse.Address)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.OrganizationId)))
                            Error += Warehouse.Errors[nameof(Warehouse.OrganizationId)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.ProvinceId)))
                            Error += Warehouse.Errors[nameof(Warehouse.ProvinceId)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.DistrictId)))
                            Error += Warehouse.Errors[nameof(Warehouse.DistrictId)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.WardId)))
                            Error += Warehouse.Errors[nameof(Warehouse.WardId)];
                        if (Warehouse.Errors.ContainsKey(nameof(Warehouse.StatusId)))
                            Error += Warehouse.Errors[nameof(Warehouse.StatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(WarehouseRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var WarehouseFilter = ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO);
            WarehouseFilter.Skip = 0;
            WarehouseFilter.Take = int.MaxValue;
            WarehouseFilter = WarehouseService.ToFilter(WarehouseFilter);

            List<Warehouse> Warehouses = await WarehouseService.List(WarehouseFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var WarehouseHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Address",
                        "OrganizationId",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "StatusId",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Warehouses.Count; i++)
                {
                    var Warehouse = Warehouses[i];
                    data.Add(new Object[]
                    {
                        Warehouse.Id,
                        Warehouse.Code,
                        Warehouse.Name,
                        Warehouse.Address,
                        Warehouse.OrganizationId,
                        Warehouse.ProvinceId,
                        Warehouse.DistrictId,
                        Warehouse.WardId,
                        Warehouse.StatusId,
                    });
                    excel.GenerateWorksheet("Warehouse", WarehouseHeaders, data);
                }
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Warehouse.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter = WarehouseService.ToFilter(WarehouseFilter);
            if (Id == 0)
            {

            }
            else
            {
                WarehouseFilter.Id = new IdFilter { Equal = Id };
                int count = await WarehouseService.Count(WarehouseFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Warehouse ConvertDTOToEntity(Warehouse_WarehouseDTO Warehouse_WarehouseDTO)
        {
            Warehouse Warehouse = new Warehouse();
            Warehouse.Id = Warehouse_WarehouseDTO.Id;
            Warehouse.Code = Warehouse_WarehouseDTO.Code;
            Warehouse.Name = Warehouse_WarehouseDTO.Name;
            Warehouse.Address = Warehouse_WarehouseDTO.Address;
            Warehouse.OrganizationId = Warehouse_WarehouseDTO.OrganizationId;
            Warehouse.ProvinceId = Warehouse_WarehouseDTO.ProvinceId;
            Warehouse.DistrictId = Warehouse_WarehouseDTO.DistrictId;
            Warehouse.WardId = Warehouse_WarehouseDTO.WardId;
            Warehouse.StatusId = Warehouse_WarehouseDTO.StatusId;
            Warehouse.District = Warehouse_WarehouseDTO.District == null ? null : new District
            {
                Id = Warehouse_WarehouseDTO.District.Id,
                Code = Warehouse_WarehouseDTO.District.Code,
                Name = Warehouse_WarehouseDTO.District.Name,
                Priority = Warehouse_WarehouseDTO.District.Priority,
                ProvinceId = Warehouse_WarehouseDTO.District.ProvinceId,
                StatusId = Warehouse_WarehouseDTO.District.StatusId,
            };
            Warehouse.Province = Warehouse_WarehouseDTO.Province == null ? null : new Province
            {
                Id = Warehouse_WarehouseDTO.Province.Id,
                Code = Warehouse_WarehouseDTO.Province.Code,
                Name = Warehouse_WarehouseDTO.Province.Name,
                Priority = Warehouse_WarehouseDTO.Province.Priority,
                StatusId = Warehouse_WarehouseDTO.Province.StatusId,
            };
            Warehouse.Status = Warehouse_WarehouseDTO.Status == null ? null : new Status
            {
                Id = Warehouse_WarehouseDTO.Status.Id,
                Code = Warehouse_WarehouseDTO.Status.Code,
                Name = Warehouse_WarehouseDTO.Status.Name,
            };
            Warehouse.Ward = Warehouse_WarehouseDTO.Ward == null ? null : new Ward
            {
                Id = Warehouse_WarehouseDTO.Ward.Id,
                Code = Warehouse_WarehouseDTO.Ward.Code,
                Name = Warehouse_WarehouseDTO.Ward.Name,
                Priority = Warehouse_WarehouseDTO.Ward.Priority,
                DistrictId = Warehouse_WarehouseDTO.Ward.DistrictId,
                StatusId = Warehouse_WarehouseDTO.Ward.StatusId,
            };
            Warehouse.Inventories = Warehouse_WarehouseDTO.Inventories?
                .Select(x => new Inventory
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            Name = x.Item.Product.Name,
                            ERPCode = x.Item.Product.ERPCode,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                            }
                        }
                    },
                }).ToList();
            Warehouse.BaseLanguage = CurrentContext.Language;
            return Warehouse;
        }

        private WarehouseFilter ConvertFilterDTOToFilterEntity(Warehouse_WarehouseFilterDTO Warehouse_WarehouseFilterDTO)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter();
            WarehouseFilter.Selects = WarehouseSelect.ALL;
            WarehouseFilter.Skip = Warehouse_WarehouseFilterDTO.Skip;
            WarehouseFilter.Take = Warehouse_WarehouseFilterDTO.Take;
            WarehouseFilter.OrderBy = Warehouse_WarehouseFilterDTO.OrderBy;
            WarehouseFilter.OrderType = Warehouse_WarehouseFilterDTO.OrderType;

            WarehouseFilter.Id = Warehouse_WarehouseFilterDTO.Id;
            WarehouseFilter.Code = Warehouse_WarehouseFilterDTO.Code;
            WarehouseFilter.Name = Warehouse_WarehouseFilterDTO.Name;
            WarehouseFilter.Address = Warehouse_WarehouseFilterDTO.Address;
            WarehouseFilter.OrganizationId = Warehouse_WarehouseFilterDTO.OrganizationId;
            WarehouseFilter.ProvinceId = Warehouse_WarehouseFilterDTO.ProvinceId;
            WarehouseFilter.DistrictId = Warehouse_WarehouseFilterDTO.DistrictId;
            WarehouseFilter.WardId = Warehouse_WarehouseFilterDTO.WardId;
            WarehouseFilter.StatusId = Warehouse_WarehouseFilterDTO.StatusId;
            return WarehouseFilter;
        }

        [Route(WarehouseRoute.SingleListDistrict), HttpPost]
        public async Task<List<Warehouse_DistrictDTO>> SingleListDistrict([FromBody] Warehouse_DistrictFilterDTO Warehouse_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Warehouse_DistrictFilterDTO.Id;
            DistrictFilter.Code = Warehouse_DistrictFilterDTO.Code;
            DistrictFilter.Name = Warehouse_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Warehouse_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Warehouse_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = Warehouse_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Warehouse_DistrictDTO> Warehouse_DistrictDTOs = Districts
                .Select(x => new Warehouse_DistrictDTO(x)).ToList();
            return Warehouse_DistrictDTOs;
        }

        [Route(WarehouseRoute.SingleListOrganization), HttpPost]
        public async Task<List<Warehouse_OrganizationDTO>> SingleListOrganization([FromBody] Warehouse_OrganizationFilterDTO Warehouse_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            OrganizationFilter.Code = Warehouse_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Warehouse_OrganizationFilterDTO.Name;
            OrganizationFilter.Path = Warehouse_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Warehouse_OrganizationFilterDTO.Level;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Warehouse_OrganizationDTO> Warehouse_OrganizationDTOs = Organizations
                .Select(x => new Warehouse_OrganizationDTO(x)).ToList();
            return Warehouse_OrganizationDTOs;
        }
        [Route(WarehouseRoute.SingleListProvince), HttpPost]
        public async Task<List<Warehouse_ProvinceDTO>> SingleListProvince([FromBody] Warehouse_ProvinceFilterDTO Warehouse_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Warehouse_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = Warehouse_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = Warehouse_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = Warehouse_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = Warehouse_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Warehouse_ProvinceDTO> Warehouse_ProvinceDTOs = Provinces
                .Select(x => new Warehouse_ProvinceDTO(x)).ToList();
            return Warehouse_ProvinceDTOs;
        }
        [Route(WarehouseRoute.SingleListStatus), HttpPost]
        public async Task<List<Warehouse_StatusDTO>> SingleListStatus([FromBody] Warehouse_StatusFilterDTO Warehouse_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Warehouse_StatusDTO> Warehouse_StatusDTOs = Statuses
                .Select(x => new Warehouse_StatusDTO(x)).ToList();
            return Warehouse_StatusDTOs;
        }
        [Route(WarehouseRoute.SingleListWard), HttpPost]
        public async Task<List<Warehouse_WardDTO>> SingleListWard([FromBody] Warehouse_WardFilterDTO Warehouse_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Warehouse_WardFilterDTO.Id;
            WardFilter.Code = Warehouse_WardFilterDTO.Code;
            WardFilter.Name = Warehouse_WardFilterDTO.Name;
            WardFilter.Priority = Warehouse_WardFilterDTO.Priority;
            WardFilter.DistrictId = Warehouse_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Warehouse_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<Warehouse_WardDTO> Warehouse_WardDTOs = Wards
                .Select(x => new Warehouse_WardDTO(x)).ToList();
            return Warehouse_WardDTOs;
        }
        [Route(WarehouseRoute.SingleListItem), HttpPost]
        public async Task<List<Warehouse_ItemDTO>> SingleListItem([FromBody] Warehouse_ItemFilterDTO Warehouse_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Warehouse_ItemFilterDTO.Id;
            ItemFilter.ProductId = Warehouse_ItemFilterDTO.ProductId;
            ItemFilter.Code = Warehouse_ItemFilterDTO.Code;
            ItemFilter.Name = Warehouse_ItemFilterDTO.Name;
            ItemFilter.ScanCode = Warehouse_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = Warehouse_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = Warehouse_ItemFilterDTO.RetailPrice;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Warehouse_ItemDTO> Warehouse_ItemDTOs = Items
                .Select(x => new Warehouse_ItemDTO(x)).ToList();
            return Warehouse_ItemDTOs;
        }

    }
}

