using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using DMS.Entities;
using DMS.Services.MShowingWarehouse;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MWard;
using DMS.Services.MShowingInventory;
using DMS.Services.MAppUser;
using DMS.Services.MShowingItem;
using System.Text;
using DMS.Enums;
using DMS.Services.MShowingShowingInventory;

namespace DMS.Rpc.posm.showing_warehouse
{
    public partial class ShowingWarehouseController : RpcController
    {
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private IWardService WardService;
        private IShowingInventoryService ShowingInventoryService;
        private IAppUserService AppUserService;
        private IShowingItemService ShowingItemService;
        private IShowingWarehouseService ShowingWarehouseService;
        private IShowingInventoryHistoryService ShowingInventoryHistoryService;
        private ICurrentContext CurrentContext;
        public ShowingWarehouseController(
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            IWardService WardService,
            IShowingInventoryService ShowingInventoryService,
            IAppUserService AppUserService,
            IShowingItemService ShowingItemService,
            IShowingWarehouseService ShowingWarehouseService,
            IShowingInventoryHistoryService ShowingInventoryHistoryService,
            ICurrentContext CurrentContext
        )
        {
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.WardService = WardService;
            this.ShowingInventoryService = ShowingInventoryService;
            this.AppUserService = AppUserService;
            this.ShowingItemService = ShowingItemService;
            this.ShowingWarehouseService = ShowingWarehouseService;
            this.ShowingInventoryHistoryService = ShowingInventoryHistoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ShowingWarehouseRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ShowingWarehouse_ShowingWarehouseFilterDTO ShowingWarehouse_ShowingWarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingWarehouseFilter ShowingWarehouseFilter = ConvertFilterDTOToFilterEntity(ShowingWarehouse_ShowingWarehouseFilterDTO);
            ShowingWarehouseFilter = await ShowingWarehouseService.ToFilter(ShowingWarehouseFilter);
            int count = await ShowingWarehouseService.Count(ShowingWarehouseFilter);
            return count;
        }

        [Route(ShowingWarehouseRoute.List), HttpPost]
        public async Task<ActionResult<List<ShowingWarehouse_ShowingWarehouseDTO>>> List([FromBody] ShowingWarehouse_ShowingWarehouseFilterDTO ShowingWarehouse_ShowingWarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingWarehouseFilter ShowingWarehouseFilter = ConvertFilterDTOToFilterEntity(ShowingWarehouse_ShowingWarehouseFilterDTO);
            ShowingWarehouseFilter = await ShowingWarehouseService.ToFilter(ShowingWarehouseFilter);
            List<ShowingWarehouse> ShowingWarehouses = await ShowingWarehouseService.List(ShowingWarehouseFilter);
            List<ShowingWarehouse_ShowingWarehouseDTO> ShowingWarehouse_ShowingWarehouseDTOs = ShowingWarehouses
                .Select(c => new ShowingWarehouse_ShowingWarehouseDTO(c)).ToList();
            return ShowingWarehouse_ShowingWarehouseDTOs;
        }

        [Route(ShowingWarehouseRoute.Get), HttpPost]
        public async Task<ActionResult<ShowingWarehouse_ShowingWarehouseDTO>> Get([FromBody]ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingWarehouse_ShowingWarehouseDTO.Id))
                return Forbid();

            ShowingWarehouse ShowingWarehouse = await ShowingWarehouseService.Get(ShowingWarehouse_ShowingWarehouseDTO.Id);
            return new ShowingWarehouse_ShowingWarehouseDTO(ShowingWarehouse);
        }

        [Route(ShowingWarehouseRoute.Create), HttpPost]
        public async Task<ActionResult<ShowingWarehouse_ShowingWarehouseDTO>> Create([FromBody] ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ShowingWarehouse_ShowingWarehouseDTO.Id))
                return Forbid();

            ShowingWarehouse ShowingWarehouse = ConvertDTOToEntity(ShowingWarehouse_ShowingWarehouseDTO);
            ShowingWarehouse = await ShowingWarehouseService.Create(ShowingWarehouse);
            ShowingWarehouse_ShowingWarehouseDTO = new ShowingWarehouse_ShowingWarehouseDTO(ShowingWarehouse);
            if (ShowingWarehouse.IsValidated)
                return ShowingWarehouse_ShowingWarehouseDTO;
            else
                return BadRequest(ShowingWarehouse_ShowingWarehouseDTO);
        }

        [Route(ShowingWarehouseRoute.Update), HttpPost]
        public async Task<ActionResult<ShowingWarehouse_ShowingWarehouseDTO>> Update([FromBody] ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ShowingWarehouse_ShowingWarehouseDTO.Id))
                return Forbid();

            ShowingWarehouse ShowingWarehouse = ConvertDTOToEntity(ShowingWarehouse_ShowingWarehouseDTO);
            ShowingWarehouse = await ShowingWarehouseService.Update(ShowingWarehouse);
            ShowingWarehouse_ShowingWarehouseDTO = new ShowingWarehouse_ShowingWarehouseDTO(ShowingWarehouse);
            if (ShowingWarehouse.IsValidated)
                return ShowingWarehouse_ShowingWarehouseDTO;
            else
                return BadRequest(ShowingWarehouse_ShowingWarehouseDTO);
        }

        [Route(ShowingWarehouseRoute.Delete), HttpPost]
        public async Task<ActionResult<ShowingWarehouse_ShowingWarehouseDTO>> Delete([FromBody] ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingWarehouse_ShowingWarehouseDTO.Id))
                return Forbid();

            ShowingWarehouse ShowingWarehouse = ConvertDTOToEntity(ShowingWarehouse_ShowingWarehouseDTO);
            ShowingWarehouse = await ShowingWarehouseService.Delete(ShowingWarehouse);
            ShowingWarehouse_ShowingWarehouseDTO = new ShowingWarehouse_ShowingWarehouseDTO(ShowingWarehouse);
            if (ShowingWarehouse.IsValidated)
                return ShowingWarehouse_ShowingWarehouseDTO;
            else
                return BadRequest(ShowingWarehouse_ShowingWarehouseDTO);
        }
        
        [Route(ShowingWarehouseRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingWarehouseFilter ShowingWarehouseFilter = new ShowingWarehouseFilter();
            ShowingWarehouseFilter = await ShowingWarehouseService.ToFilter(ShowingWarehouseFilter);
            ShowingWarehouseFilter.Id = new IdFilter { In = Ids };
            ShowingWarehouseFilter.Selects = ShowingWarehouseSelect.Id;
            ShowingWarehouseFilter.Skip = 0;
            ShowingWarehouseFilter.Take = int.MaxValue;

            List<ShowingWarehouse> ShowingWarehouses = await ShowingWarehouseService.List(ShowingWarehouseFilter);
            ShowingWarehouses = await ShowingWarehouseService.BulkDelete(ShowingWarehouses);
            if (ShowingWarehouses.Any(x => !x.IsValidated))
                return BadRequest(ShowingWarehouses.Where(x => !x.IsValidated));
            return true;
        }

        [Route(ShowingWarehouseRoute.Import), HttpPost]
        public async Task<ActionResult<List<ShowingWarehouse_ShowingInventoryDTO>>> ImportShowingInventory([FromForm] long ShowingWarehouseId, [FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingWarehouseId))
                return Forbid();

            ShowingWarehouse ShowingWarehouse = await ShowingWarehouseService.Get(ShowingWarehouseId);
            if (ShowingWarehouse == null)
                return BadRequest("Kho không tồn tại");
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ShowingItemSelect.ALL
            };

            List<ShowingWarehouse> ShowingWarehouses = await ShowingWarehouseService.List(new ShowingWarehouseFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ShowingWarehouseSelect.ALL
            });
            List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);
            StringBuilder errorContent = new StringBuilder();
            ShowingWarehouse.ShowingInventories = new List<ShowingInventory>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["ShowingInventory"];
                if (worksheet == null)
                    return BadRequest("File không đúng biểu mẫu import");
                int StartColumn = 1;
                int StartRow = 1;
                int SttColumnn = 0 + StartColumn;
                int ShowingWarehouseCodeColumn = 1 + StartColumn;
                int ShowingWarehouseNameColumn = 2 + StartColumn;
                int ShowingItemCodeColumn = 3 + StartColumn;
                int ShowingItemNameColumn = 4 + StartColumn;
                int SaleStockColumn = 5 + StartColumn;
                int AccountingStockColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string stt = worksheet.Cells[i + StartRow, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;

                    string ShowingWarehouseCodeValue = worksheet.Cells[i + StartRow, ShowingWarehouseCodeColumn].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(ShowingWarehouseCodeValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập mã kho");
                    }
                    else if (string.IsNullOrWhiteSpace(ShowingWarehouseCodeValue) && i == worksheet.Dimension.End.Row)
                        break;
                    string ShowingWarehouseNameValue = worksheet.Cells[i + StartRow, ShowingWarehouseNameColumn].Value?.ToString();
                    string ShowingItemCodeValue = worksheet.Cells[i + StartRow, ShowingItemCodeColumn].Value?.ToString();
                    string ShowingItemNameValue = worksheet.Cells[i + StartRow, ShowingItemNameColumn].Value?.ToString();
                    string SaleStockValue = worksheet.Cells[i + StartRow, SaleStockColumn].Value?.ToString();
                    string AccountingStockValue = worksheet.Cells[i + StartRow, AccountingStockColumn].Value?.ToString();

                    ShowingWarehouse ShowingWarehouseInDB = ShowingWarehouses.Where(x => x.Code == ShowingWarehouseCodeValue.Trim()).FirstOrDefault();
                    if (ShowingWarehouseInDB == null)
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Mã kho không đúng");
                    else if (ShowingWarehouseInDB.Id != ShowingWarehouse.Id)
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Mã kho không đúng");
                    var ShowingInventoryShowingItem = ShowingItems.Where(x => x.Code == ShowingItemCodeValue).FirstOrDefault();

                    ShowingInventory ShowingInventory = new ShowingInventory();
                    ShowingInventory.SaleStock = string.IsNullOrEmpty(SaleStockValue) ? 0 : long.Parse(SaleStockValue);
                    ShowingInventory.AccountingStock = string.IsNullOrEmpty(AccountingStockValue) ? 0 : long.Parse(AccountingStockValue);
                    ShowingInventory.ShowingItemId = ShowingInventory.ShowingItem == null ? 0 : ShowingInventory.ShowingItem.Id;
                    ShowingInventory.ShowingItemId = ShowingInventoryShowingItem == null ? 0 : ShowingInventoryShowingItem.Id;
                    ShowingInventory.ShowingWarehouseId = ShowingWarehouse.Id;

                    ShowingWarehouse.ShowingInventories.Add(ShowingInventory);
                }
                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
            }
            ShowingWarehouse = await ShowingWarehouseService.Update(ShowingWarehouse);
            List<ShowingWarehouse_ShowingInventoryDTO> ShowingWarehouse_ShowingInventoryDTOs = ShowingWarehouse.ShowingInventories
                 .Select(c => new ShowingWarehouse_ShowingInventoryDTO(c)).ToList();
            for (int i = 0; i < ShowingWarehouse.ShowingInventories.Count; i++)
            {
                if (!ShowingWarehouse.ShowingInventories[i].IsValidated)
                {
                    foreach (var Error in ShowingWarehouse.ShowingInventories[i].Errors)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 2}: {Error.Value}");
                    }
                }
            }
            if (ShowingWarehouse.ShowingInventories.Any(x => !x.IsValidated))
                return BadRequest(errorContent.ToString());
            return ShowingWarehouse_ShowingInventoryDTOs;
        }

        [Route(ShowingWarehouseRoute.Export), HttpPost]
        public async Task<ActionResult> ExportShowingInventory([FromBody] ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingWarehouse_ShowingWarehouseDTO.Id))
                return Forbid();

            long ShowingWarehouseId = ShowingWarehouse_ShowingWarehouseDTO?.Id ?? 0;
            ShowingWarehouse ShowingWarehouse = await ShowingWarehouseService.Get(ShowingWarehouseId);
            if (ShowingWarehouse == null)
                return null;

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var ShowingInventoryHeaders = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã kho",
                        "Tên kho",
                        "Mã sản phẩm",
                        "Tên sản phẩm",
                        "Có thể bán",
                        "Tồn kế toán",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < ShowingWarehouse.ShowingInventories.Count; i++)
                {
                    var ShowingInventory = ShowingWarehouse.ShowingInventories[i];
                    data.Add(new Object[]
                    {
                        i+1,
                        ShowingWarehouse.Code,
                        ShowingWarehouse.Name,
                        ShowingInventory.ShowingItem.Code,
                        ShowingInventory.ShowingItem.Name,
                        ShowingInventory.SaleStock,
                        ShowingInventory.AccountingStock
                    });
                }
                excel.GenerateWorksheet("ShowingInventory", ShowingInventoryHeaders, data);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", $"{ShowingWarehouse.Code}_ShowingInventory.xlsx");
        }

        [Route(ShowingWarehouseRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long ShowingWarehouseId = ShowingWarehouse_ShowingWarehouseDTO?.Id ?? 0;
            ShowingWarehouse ShowingWarehouse = await ShowingWarehouseService.Get(ShowingWarehouseId);
            if (ShowingWarehouse == null)
                ShowingWarehouse = new ShowingWarehouse
                {
                    ShowingInventories = new List<ShowingInventory>()
                };

            var ShowingItemFilter = new ShowingItemFilter
            {
                Selects = ShowingItemSelect.ALL,
                Skip = 0,
                Take = int.MaxValue,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var ShowingItems = await ShowingItemService.List(ShowingItemFilter);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var ShowingInventoryHeaders = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã kho",
                        "Tên kho",
                        "Mã sản phẩm",
                        "Tên sản phẩm",
                        "Có thể bán",
                        "Tồn kế toán",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < ShowingItems.Count; i++)
                {
                    var ShowingItem = ShowingItems[i];
                    data.Add(new Object[]
                    {
                        i+1,
                        ShowingWarehouse.Code,
                        ShowingWarehouse.Name,
                        ShowingItem.Code,
                        ShowingItem.Name,
                        0,
                        0,
                    });
                }
                excel.GenerateWorksheet("ShowingInventory", ShowingInventoryHeaders, data);

                data.Clear();
                var ShowingWarehouseHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã",
                        "Tên",
                    }
                };
                data.Add(new object[]
                {
                    ShowingWarehouse.Code,
                    ShowingWarehouse.Name,
                });
                excel.GenerateWorksheet("ShowingWarehouse", ShowingWarehouseHeader, data);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Template_ShowingInventory.xlsx");
        }


        private async Task<bool> HasPermission(long Id)
        {
            ShowingWarehouseFilter ShowingWarehouseFilter = new ShowingWarehouseFilter();
            ShowingWarehouseFilter = await ShowingWarehouseService.ToFilter(ShowingWarehouseFilter);
            if (Id == 0)
            {

            }
            else
            {
                ShowingWarehouseFilter.Id = new IdFilter { Equal = Id };
                int count = await ShowingWarehouseService.Count(ShowingWarehouseFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ShowingWarehouse ConvertDTOToEntity(ShowingWarehouse_ShowingWarehouseDTO ShowingWarehouse_ShowingWarehouseDTO)
        {
            ShowingWarehouse ShowingWarehouse = new ShowingWarehouse();
            ShowingWarehouse.Id = ShowingWarehouse_ShowingWarehouseDTO.Id;
            ShowingWarehouse.Code = ShowingWarehouse_ShowingWarehouseDTO.Code;
            ShowingWarehouse.Name = ShowingWarehouse_ShowingWarehouseDTO.Name;
            ShowingWarehouse.Address = ShowingWarehouse_ShowingWarehouseDTO.Address;
            ShowingWarehouse.OrganizationId = ShowingWarehouse_ShowingWarehouseDTO.OrganizationId;
            ShowingWarehouse.ProvinceId = ShowingWarehouse_ShowingWarehouseDTO.ProvinceId;
            ShowingWarehouse.DistrictId = ShowingWarehouse_ShowingWarehouseDTO.DistrictId;
            ShowingWarehouse.WardId = ShowingWarehouse_ShowingWarehouseDTO.WardId;
            ShowingWarehouse.StatusId = ShowingWarehouse_ShowingWarehouseDTO.StatusId;
            ShowingWarehouse.RowId = ShowingWarehouse_ShowingWarehouseDTO.RowId;
            ShowingWarehouse.District = ShowingWarehouse_ShowingWarehouseDTO.District == null ? null : new District
            {
                Id = ShowingWarehouse_ShowingWarehouseDTO.District.Id,
                Code = ShowingWarehouse_ShowingWarehouseDTO.District.Code,
                Name = ShowingWarehouse_ShowingWarehouseDTO.District.Name,
                Priority = ShowingWarehouse_ShowingWarehouseDTO.District.Priority,
                ProvinceId = ShowingWarehouse_ShowingWarehouseDTO.District.ProvinceId,
                StatusId = ShowingWarehouse_ShowingWarehouseDTO.District.StatusId,
                RowId = ShowingWarehouse_ShowingWarehouseDTO.District.RowId,
            };
            ShowingWarehouse.Organization = ShowingWarehouse_ShowingWarehouseDTO.Organization == null ? null : new Organization
            {
                Id = ShowingWarehouse_ShowingWarehouseDTO.Organization.Id,
                Code = ShowingWarehouse_ShowingWarehouseDTO.Organization.Code,
                Name = ShowingWarehouse_ShowingWarehouseDTO.Organization.Name,
                ParentId = ShowingWarehouse_ShowingWarehouseDTO.Organization.ParentId,
                Path = ShowingWarehouse_ShowingWarehouseDTO.Organization.Path,
                Level = ShowingWarehouse_ShowingWarehouseDTO.Organization.Level,
                StatusId = ShowingWarehouse_ShowingWarehouseDTO.Organization.StatusId,
                Phone = ShowingWarehouse_ShowingWarehouseDTO.Organization.Phone,
                Email = ShowingWarehouse_ShowingWarehouseDTO.Organization.Email,
                Address = ShowingWarehouse_ShowingWarehouseDTO.Organization.Address,
                RowId = ShowingWarehouse_ShowingWarehouseDTO.Organization.RowId,
            };
            ShowingWarehouse.Province = ShowingWarehouse_ShowingWarehouseDTO.Province == null ? null : new Province
            {
                Id = ShowingWarehouse_ShowingWarehouseDTO.Province.Id,
                Code = ShowingWarehouse_ShowingWarehouseDTO.Province.Code,
                Name = ShowingWarehouse_ShowingWarehouseDTO.Province.Name,
                Priority = ShowingWarehouse_ShowingWarehouseDTO.Province.Priority,
                StatusId = ShowingWarehouse_ShowingWarehouseDTO.Province.StatusId,
                RowId = ShowingWarehouse_ShowingWarehouseDTO.Province.RowId,
            };
            ShowingWarehouse.Status = ShowingWarehouse_ShowingWarehouseDTO.Status == null ? null : new Status
            {
                Id = ShowingWarehouse_ShowingWarehouseDTO.Status.Id,
                Code = ShowingWarehouse_ShowingWarehouseDTO.Status.Code,
                Name = ShowingWarehouse_ShowingWarehouseDTO.Status.Name,
            };
            ShowingWarehouse.Ward = ShowingWarehouse_ShowingWarehouseDTO.Ward == null ? null : new Ward
            {
                Id = ShowingWarehouse_ShowingWarehouseDTO.Ward.Id,
                Code = ShowingWarehouse_ShowingWarehouseDTO.Ward.Code,
                Name = ShowingWarehouse_ShowingWarehouseDTO.Ward.Name,
                Priority = ShowingWarehouse_ShowingWarehouseDTO.Ward.Priority,
                DistrictId = ShowingWarehouse_ShowingWarehouseDTO.Ward.DistrictId,
                StatusId = ShowingWarehouse_ShowingWarehouseDTO.Ward.StatusId,
                RowId = ShowingWarehouse_ShowingWarehouseDTO.Ward.RowId,
            };
            ShowingWarehouse.ShowingInventories = ShowingWarehouse_ShowingWarehouseDTO.ShowingInventories?
                .Select(x => new ShowingInventory
                {
                    Id = x.Id,
                    ShowingItemId = x.ShowingItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                    AppUserId = x.AppUserId,
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Address = x.AppUser.Address,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                        SexId = x.AppUser.SexId,
                        Birthday = x.AppUser.Birthday,
                        Avatar = x.AppUser.Avatar,
                        PositionId = x.AppUser.PositionId,
                        Department = x.AppUser.Department,
                        OrganizationId = x.AppUser.OrganizationId,
                        ProvinceId = x.AppUser.ProvinceId,
                        Longitude = x.AppUser.Longitude,
                        Latitude = x.AppUser.Latitude,
                        StatusId = x.AppUser.StatusId,
                        GPSUpdatedAt = x.AppUser.GPSUpdatedAt,
                        RowId = x.AppUser.RowId,
                    },
                    ShowingItem = x.ShowingItem == null ? null : new ShowingItem
                    {
                        Id = x.ShowingItem.Id,
                        Code = x.ShowingItem.Code,
                        Name = x.ShowingItem.Name,
                        ShowingCategoryId = x.ShowingItem.ShowingCategoryId,
                        UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                        SalePrice = x.ShowingItem.SalePrice,
                        Desception = x.ShowingItem.Desception,
                        StatusId = x.ShowingItem.StatusId,
                        Used = x.ShowingItem.Used,
                        RowId = x.ShowingItem.RowId,
                    },
                }).ToList();
            ShowingWarehouse.BaseLanguage = CurrentContext.Language;
            return ShowingWarehouse;
        }

        private ShowingWarehouseFilter ConvertFilterDTOToFilterEntity(ShowingWarehouse_ShowingWarehouseFilterDTO ShowingWarehouse_ShowingWarehouseFilterDTO)
        {
            ShowingWarehouseFilter ShowingWarehouseFilter = new ShowingWarehouseFilter();
            ShowingWarehouseFilter.Selects = ShowingWarehouseSelect.ALL;
            ShowingWarehouseFilter.Skip = ShowingWarehouse_ShowingWarehouseFilterDTO.Skip;
            ShowingWarehouseFilter.Take = ShowingWarehouse_ShowingWarehouseFilterDTO.Take;
            ShowingWarehouseFilter.OrderBy = ShowingWarehouse_ShowingWarehouseFilterDTO.OrderBy;
            ShowingWarehouseFilter.OrderType = ShowingWarehouse_ShowingWarehouseFilterDTO.OrderType;

            ShowingWarehouseFilter.Id = ShowingWarehouse_ShowingWarehouseFilterDTO.Id;
            ShowingWarehouseFilter.Code = ShowingWarehouse_ShowingWarehouseFilterDTO.Code;
            ShowingWarehouseFilter.Name = ShowingWarehouse_ShowingWarehouseFilterDTO.Name;
            ShowingWarehouseFilter.Address = ShowingWarehouse_ShowingWarehouseFilterDTO.Address;
            ShowingWarehouseFilter.OrganizationId = ShowingWarehouse_ShowingWarehouseFilterDTO.OrganizationId;
            ShowingWarehouseFilter.ProvinceId = ShowingWarehouse_ShowingWarehouseFilterDTO.ProvinceId;
            ShowingWarehouseFilter.DistrictId = ShowingWarehouse_ShowingWarehouseFilterDTO.DistrictId;
            ShowingWarehouseFilter.WardId = ShowingWarehouse_ShowingWarehouseFilterDTO.WardId;
            ShowingWarehouseFilter.StatusId = ShowingWarehouse_ShowingWarehouseFilterDTO.StatusId;
            ShowingWarehouseFilter.RowId = ShowingWarehouse_ShowingWarehouseFilterDTO.RowId;
            ShowingWarehouseFilter.CreatedAt = ShowingWarehouse_ShowingWarehouseFilterDTO.CreatedAt;
            ShowingWarehouseFilter.UpdatedAt = ShowingWarehouse_ShowingWarehouseFilterDTO.UpdatedAt;
            return ShowingWarehouseFilter;
        }
    }
}

