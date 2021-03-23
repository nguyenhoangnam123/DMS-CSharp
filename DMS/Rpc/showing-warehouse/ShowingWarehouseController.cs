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

namespace DMS.Rpc.showing_warehouse
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
            List<ShowingWarehouse> ShowingWarehouses = new List<ShowingWarehouse>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ShowingWarehouses);
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
                int RowIdColumn = 12 + StartColumn;

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
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    
                    ShowingWarehouse ShowingWarehouse = new ShowingWarehouse();
                    ShowingWarehouse.Code = CodeValue;
                    ShowingWarehouse.Name = NameValue;
                    ShowingWarehouse.Address = AddressValue;
                    District District = Districts.Where(x => x.Id.ToString() == DistrictIdValue).FirstOrDefault();
                    ShowingWarehouse.DistrictId = District == null ? 0 : District.Id;
                    ShowingWarehouse.District = District;
                    Province Province = Provinces.Where(x => x.Id.ToString() == ProvinceIdValue).FirstOrDefault();
                    ShowingWarehouse.ProvinceId = Province == null ? 0 : Province.Id;
                    ShowingWarehouse.Province = Province;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    ShowingWarehouse.StatusId = Status == null ? 0 : Status.Id;
                    ShowingWarehouse.Status = Status;
                    Ward Ward = Wards.Where(x => x.Id.ToString() == WardIdValue).FirstOrDefault();
                    ShowingWarehouse.WardId = Ward == null ? 0 : Ward.Id;
                    ShowingWarehouse.Ward = Ward;
                    
                    ShowingWarehouses.Add(ShowingWarehouse);
                }
            }
            ShowingWarehouses = await ShowingWarehouseService.Import(ShowingWarehouses);
            if (ShowingWarehouses.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ShowingWarehouses.Count; i++)
                {
                    ShowingWarehouse ShowingWarehouse = ShowingWarehouses[i];
                    if (!ShowingWarehouse.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.Id)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.Id)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.Code)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.Code)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.Name)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.Name)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.Address)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.Address)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.OrganizationId)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.OrganizationId)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.ProvinceId)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.ProvinceId)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.DistrictId)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.DistrictId)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.WardId)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.WardId)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.StatusId)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.StatusId)];
                        if (ShowingWarehouse.Errors.ContainsKey(nameof(ShowingWarehouse.RowId)))
                            Error += ShowingWarehouse.Errors[nameof(ShowingWarehouse.RowId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ShowingWarehouseRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ShowingWarehouse_ShowingWarehouseFilterDTO ShowingWarehouse_ShowingWarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ShowingWarehouse
                var ShowingWarehouseFilter = ConvertFilterDTOToFilterEntity(ShowingWarehouse_ShowingWarehouseFilterDTO);
                ShowingWarehouseFilter.Skip = 0;
                ShowingWarehouseFilter.Take = int.MaxValue;
                ShowingWarehouseFilter = await ShowingWarehouseService.ToFilter(ShowingWarehouseFilter);
                List<ShowingWarehouse> ShowingWarehouses = await ShowingWarehouseService.List(ShowingWarehouseFilter);

                var ShowingWarehouseHeaders = new List<string[]>()
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
                        "RowId",
                    }
                };
                List<object[]> ShowingWarehouseData = new List<object[]>();
                for (int i = 0; i < ShowingWarehouses.Count; i++)
                {
                    var ShowingWarehouse = ShowingWarehouses[i];
                    ShowingWarehouseData.Add(new Object[]
                    {
                        ShowingWarehouse.Id,
                        ShowingWarehouse.Code,
                        ShowingWarehouse.Name,
                        ShowingWarehouse.Address,
                        ShowingWarehouse.OrganizationId,
                        ShowingWarehouse.ProvinceId,
                        ShowingWarehouse.DistrictId,
                        ShowingWarehouse.WardId,
                        ShowingWarehouse.StatusId,
                        ShowingWarehouse.RowId,
                    });
                }
                excel.GenerateWorksheet("ShowingWarehouse", ShowingWarehouseHeaders, ShowingWarehouseData);
                #endregion
                
                #region District
                var DistrictFilter = new DistrictFilter();
                DistrictFilter.Selects = DistrictSelect.ALL;
                DistrictFilter.OrderBy = DistrictOrder.Id;
                DistrictFilter.OrderType = OrderType.ASC;
                DistrictFilter.Skip = 0;
                DistrictFilter.Take = int.MaxValue;
                List<District> Districts = await DistrictService.List(DistrictFilter);

                var DistrictHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "ProvinceId",
                        "StatusId",
                        "RowId",
                    }
                };
                List<object[]> DistrictData = new List<object[]>();
                for (int i = 0; i < Districts.Count; i++)
                {
                    var District = Districts[i];
                    DistrictData.Add(new Object[]
                    {
                        District.Id,
                        District.Code,
                        District.Name,
                        District.Priority,
                        District.ProvinceId,
                        District.StatusId,
                        District.RowId,
                    });
                }
                excel.GenerateWorksheet("District", DistrictHeaders, DistrictData);
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
                        "RowId",
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
                        Organization.RowId,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
                #endregion
                #region Province
                var ProvinceFilter = new ProvinceFilter();
                ProvinceFilter.Selects = ProvinceSelect.ALL;
                ProvinceFilter.OrderBy = ProvinceOrder.Id;
                ProvinceFilter.OrderType = OrderType.ASC;
                ProvinceFilter.Skip = 0;
                ProvinceFilter.Take = int.MaxValue;
                List<Province> Provinces = await ProvinceService.List(ProvinceFilter);

                var ProvinceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "StatusId",
                        "RowId",
                    }
                };
                List<object[]> ProvinceData = new List<object[]>();
                for (int i = 0; i < Provinces.Count; i++)
                {
                    var Province = Provinces[i];
                    ProvinceData.Add(new Object[]
                    {
                        Province.Id,
                        Province.Code,
                        Province.Name,
                        Province.Priority,
                        Province.StatusId,
                        Province.RowId,
                    });
                }
                excel.GenerateWorksheet("Province", ProvinceHeaders, ProvinceData);
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
                #region Ward
                var WardFilter = new WardFilter();
                WardFilter.Selects = WardSelect.ALL;
                WardFilter.OrderBy = WardOrder.Id;
                WardFilter.OrderType = OrderType.ASC;
                WardFilter.Skip = 0;
                WardFilter.Take = int.MaxValue;
                List<Ward> Wards = await WardService.List(WardFilter);

                var WardHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Priority",
                        "DistrictId",
                        "StatusId",
                        "RowId",
                    }
                };
                List<object[]> WardData = new List<object[]>();
                for (int i = 0; i < Wards.Count; i++)
                {
                    var Ward = Wards[i];
                    WardData.Add(new Object[]
                    {
                        Ward.Id,
                        Ward.Code,
                        Ward.Name,
                        Ward.Priority,
                        Ward.DistrictId,
                        Ward.StatusId,
                        Ward.RowId,
                    });
                }
                excel.GenerateWorksheet("Ward", WardHeaders, WardData);
                #endregion
                #region ShowingInventory
                var ShowingInventoryFilter = new ShowingInventoryFilter();
                ShowingInventoryFilter.Selects = ShowingInventorySelect.ALL;
                ShowingInventoryFilter.OrderBy = ShowingInventoryOrder.Id;
                ShowingInventoryFilter.OrderType = OrderType.ASC;
                ShowingInventoryFilter.Skip = 0;
                ShowingInventoryFilter.Take = int.MaxValue;
                List<ShowingInventory> ShowingInventories = await ShowingInventoryService.List(ShowingInventoryFilter);

                var ShowingInventoryHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ShowingWarehouseId",
                        "ShowingItemId",
                        "SaleStock",
                        "AccountingStock",
                        "AppUserId",
                    }
                };
                List<object[]> ShowingInventoryData = new List<object[]>();
                for (int i = 0; i < ShowingInventories.Count; i++)
                {
                    var ShowingInventory = ShowingInventories[i];
                    ShowingInventoryData.Add(new Object[]
                    {
                        ShowingInventory.Id,
                        ShowingInventory.ShowingWarehouseId,
                        ShowingInventory.ShowingItemId,
                        ShowingInventory.SaleStock,
                        ShowingInventory.AccountingStock,
                        ShowingInventory.AppUserId,
                    });
                }
                excel.GenerateWorksheet("ShowingInventory", ShowingInventoryHeaders, ShowingInventoryData);
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
                        "SexId",
                        "Birthday",
                        "Avatar",
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "ProvinceId",
                        "Longitude",
                        "Latitude",
                        "StatusId",
                        "GPSUpdatedAt",
                        "RowId",
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
                        AppUser.SexId,
                        AppUser.Birthday,
                        AppUser.Avatar,
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.ProvinceId,
                        AppUser.Longitude,
                        AppUser.Latitude,
                        AppUser.StatusId,
                        AppUser.GPSUpdatedAt,
                        AppUser.RowId,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region ShowingItem
                var ShowingItemFilter = new ShowingItemFilter();
                ShowingItemFilter.Selects = ShowingItemSelect.ALL;
                ShowingItemFilter.OrderBy = ShowingItemOrder.Id;
                ShowingItemFilter.OrderType = OrderType.ASC;
                ShowingItemFilter.Skip = 0;
                ShowingItemFilter.Take = int.MaxValue;
                List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);

                var ShowingItemHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "CategoryId",
                        "UnitOfMeasureId",
                        "SalePrice",
                        "Desception",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> ShowingItemData = new List<object[]>();
                for (int i = 0; i < ShowingItems.Count; i++)
                {
                    var ShowingItem = ShowingItems[i];
                    ShowingItemData.Add(new Object[]
                    {
                        ShowingItem.Id,
                        ShowingItem.Code,
                        ShowingItem.Name,
                        ShowingItem.CategoryId,
                        ShowingItem.UnitOfMeasureId,
                        ShowingItem.SalePrice,
                        ShowingItem.Desception,
                        ShowingItem.StatusId,
                        ShowingItem.Used,
                        ShowingItem.RowId,
                    });
                }
                excel.GenerateWorksheet("ShowingItem", ShowingItemHeaders, ShowingItemData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ShowingWarehouse.xlsx");
        }

        [Route(ShowingWarehouseRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ShowingWarehouse_ShowingWarehouseFilterDTO ShowingWarehouse_ShowingWarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/ShowingWarehouse_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "ShowingWarehouse.xlsx");
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
                        CategoryId = x.ShowingItem.CategoryId,
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

