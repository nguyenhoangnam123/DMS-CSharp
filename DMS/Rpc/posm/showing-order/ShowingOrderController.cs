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
using DMS.Services.MShowingOrder;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MShowingWarehouse;
using DMS.Services.MStatus;
using DMS.Services.MShowingItem;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MStore;

namespace DMS.Rpc.posm.showing_order
{
    public partial class ShowingOrderController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IShowingWarehouseService ShowingWarehouseService;
        private IStatusService StatusService;
        private IShowingItemService ShowingItemService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IShowingOrderService ShowingOrderService;
        private IStoreService StoreService;
        private ICurrentContext CurrentContext;
        public ShowingOrderController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IShowingWarehouseService ShowingWarehouseService,
            IStatusService StatusService,
            IShowingItemService ShowingItemService,
            IUnitOfMeasureService UnitOfMeasureService,
            IShowingOrderService ShowingOrderService,
            IStoreService StoreService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.ShowingWarehouseService = ShowingWarehouseService;
            this.StatusService = StatusService;
            this.ShowingItemService = ShowingItemService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.ShowingOrderService = ShowingOrderService;
            this.StoreService = StoreService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ShowingOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ShowingOrder_ShowingOrderFilterDTO ShowingOrder_ShowingOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingOrderFilter ShowingOrderFilter = ConvertFilterDTOToFilterEntity(ShowingOrder_ShowingOrderFilterDTO);
            ShowingOrderFilter = await ShowingOrderService.ToFilter(ShowingOrderFilter);
            int count = await ShowingOrderService.Count(ShowingOrderFilter);
            return count;
        }

        [Route(ShowingOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<ShowingOrder_ShowingOrderDTO>>> List([FromBody] ShowingOrder_ShowingOrderFilterDTO ShowingOrder_ShowingOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingOrderFilter ShowingOrderFilter = ConvertFilterDTOToFilterEntity(ShowingOrder_ShowingOrderFilterDTO);
            ShowingOrderFilter = await ShowingOrderService.ToFilter(ShowingOrderFilter);
            List<ShowingOrder> ShowingOrders = await ShowingOrderService.List(ShowingOrderFilter);
            List<ShowingOrder_ShowingOrderDTO> ShowingOrder_ShowingOrderDTOs = ShowingOrders
                .Select(c => new ShowingOrder_ShowingOrderDTO(c)).ToList();
            return ShowingOrder_ShowingOrderDTOs;
        }

        [Route(ShowingOrderRoute.Get), HttpPost]
        public async Task<ActionResult<ShowingOrder_ShowingOrderDTO>> Get([FromBody] ShowingOrder_ShowingOrderDTO ShowingOrder_ShowingOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingOrder_ShowingOrderDTO.Id))
                return Forbid();

            ShowingOrder ShowingOrder = await ShowingOrderService.Get(ShowingOrder_ShowingOrderDTO.Id);
            return new ShowingOrder_ShowingOrderDTO(ShowingOrder);
        }

        [Route(ShowingOrderRoute.Create), HttpPost]
        public async Task<ActionResult<ShowingOrder_ShowingOrderDTO>> Create([FromBody] ShowingOrder_ShowingOrderDTO ShowingOrder_ShowingOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingOrder_ShowingOrderDTO.Id))
                return Forbid();

            ShowingOrder ShowingOrder = ConvertDTOToEntity(ShowingOrder_ShowingOrderDTO);
            ShowingOrder = await ShowingOrderService.Create(ShowingOrder);
            ShowingOrder_ShowingOrderDTO = new ShowingOrder_ShowingOrderDTO(ShowingOrder);
            if (ShowingOrder.IsValidated)
                return ShowingOrder_ShowingOrderDTO;
            else
                return BadRequest(ShowingOrder_ShowingOrderDTO);
        }

        [Route(ShowingOrderRoute.Update), HttpPost]
        public async Task<ActionResult<ShowingOrder_ShowingOrderDTO>> Update([FromBody] ShowingOrder_ShowingOrderDTO ShowingOrder_ShowingOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingOrder_ShowingOrderDTO.Id))
                return Forbid();

            ShowingOrder ShowingOrder = ConvertDTOToEntity(ShowingOrder_ShowingOrderDTO);
            ShowingOrder = await ShowingOrderService.Update(ShowingOrder);
            ShowingOrder_ShowingOrderDTO = new ShowingOrder_ShowingOrderDTO(ShowingOrder);
            if (ShowingOrder.IsValidated)
                return ShowingOrder_ShowingOrderDTO;
            else
                return BadRequest(ShowingOrder_ShowingOrderDTO);
        }

        [Route(ShowingOrderRoute.Delete), HttpPost]
        public async Task<ActionResult<ShowingOrder_ShowingOrderDTO>> Delete([FromBody] ShowingOrder_ShowingOrderDTO ShowingOrder_ShowingOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingOrder_ShowingOrderDTO.Id))
                return Forbid();

            ShowingOrder ShowingOrder = ConvertDTOToEntity(ShowingOrder_ShowingOrderDTO);
            ShowingOrder = await ShowingOrderService.Delete(ShowingOrder);
            ShowingOrder_ShowingOrderDTO = new ShowingOrder_ShowingOrderDTO(ShowingOrder);
            if (ShowingOrder.IsValidated)
                return ShowingOrder_ShowingOrderDTO;
            else
                return BadRequest(ShowingOrder_ShowingOrderDTO);
        }

        [Route(ShowingOrderRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ShowingOrder_ShowingOrderFilterDTO ShowingOrder_ShowingOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ShowingOrder
                var ShowingOrderFilter = ConvertFilterDTOToFilterEntity(ShowingOrder_ShowingOrderFilterDTO);
                ShowingOrderFilter.Skip = 0;
                ShowingOrderFilter.Take = int.MaxValue;
                ShowingOrderFilter = await ShowingOrderService.ToFilter(ShowingOrderFilter);
                List<ShowingOrder> ShowingOrders = await ShowingOrderService.List(ShowingOrderFilter);

                var ShowingOrderHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "AppUserId",
                        "OrganizationId",
                        "Date",
                        "ShowingWarehouseId",
                        "StatusId",
                        "Total",
                        "RowId",
                    }
                };
                List<object[]> ShowingOrderData = new List<object[]>();
                for (int i = 0; i < ShowingOrders.Count; i++)
                {
                    var ShowingOrder = ShowingOrders[i];
                    ShowingOrderData.Add(new Object[]
                    {
                        ShowingOrder.Id,
                        ShowingOrder.Code,
                        ShowingOrder.AppUserId,
                        ShowingOrder.OrganizationId,
                        ShowingOrder.Date,
                        ShowingOrder.ShowingWarehouseId,
                        ShowingOrder.StatusId,
                        ShowingOrder.Total,
                        ShowingOrder.RowId,
                    });
                }
                excel.GenerateWorksheet("ShowingOrder", ShowingOrderHeaders, ShowingOrderData);
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
                #region ShowingWarehouse
                var ShowingWarehouseFilter = new ShowingWarehouseFilter();
                ShowingWarehouseFilter.Selects = ShowingWarehouseSelect.ALL;
                ShowingWarehouseFilter.OrderBy = ShowingWarehouseOrder.Id;
                ShowingWarehouseFilter.OrderType = OrderType.ASC;
                ShowingWarehouseFilter.Skip = 0;
                ShowingWarehouseFilter.Take = int.MaxValue;
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
                #region UnitOfMeasure
                var UnitOfMeasureFilter = new UnitOfMeasureFilter();
                UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
                UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
                UnitOfMeasureFilter.OrderType = OrderType.ASC;
                UnitOfMeasureFilter.Skip = 0;
                UnitOfMeasureFilter.Take = int.MaxValue;
                List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);

                var UnitOfMeasureHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "Description",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> UnitOfMeasureData = new List<object[]>();
                for (int i = 0; i < UnitOfMeasures.Count; i++)
                {
                    var UnitOfMeasure = UnitOfMeasures[i];
                    UnitOfMeasureData.Add(new Object[]
                    {
                        UnitOfMeasure.Id,
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name,
                        UnitOfMeasure.Description,
                        UnitOfMeasure.StatusId,
                        UnitOfMeasure.Used,
                        UnitOfMeasure.RowId,
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasure", UnitOfMeasureHeaders, UnitOfMeasureData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ShowingOrder.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ShowingOrderFilter ShowingOrderFilter = new ShowingOrderFilter();
            ShowingOrderFilter = await ShowingOrderService.ToFilter(ShowingOrderFilter);
            if (Id == 0)
            {

            }
            else
            {
                ShowingOrderFilter.Id = new IdFilter { Equal = Id };
                int count = await ShowingOrderService.Count(ShowingOrderFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ShowingOrder ConvertDTOToEntity(ShowingOrder_ShowingOrderDTO ShowingOrder_ShowingOrderDTO)
        {
            ShowingOrder ShowingOrder = new ShowingOrder();
            ShowingOrder.Id = ShowingOrder_ShowingOrderDTO.Id;
            ShowingOrder.Code = ShowingOrder_ShowingOrderDTO.Code;
            ShowingOrder.AppUserId = ShowingOrder_ShowingOrderDTO.AppUserId;
            ShowingOrder.StoreId = ShowingOrder_ShowingOrderDTO.StoreId;
            ShowingOrder.OrganizationId = ShowingOrder_ShowingOrderDTO.OrganizationId;
            ShowingOrder.Date = ShowingOrder_ShowingOrderDTO.Date;
            ShowingOrder.ShowingWarehouseId = ShowingOrder_ShowingOrderDTO.ShowingWarehouseId;
            ShowingOrder.StatusId = ShowingOrder_ShowingOrderDTO.StatusId;
            ShowingOrder.Total = ShowingOrder_ShowingOrderDTO.Total;
            ShowingOrder.RowId = ShowingOrder_ShowingOrderDTO.RowId;
            ShowingOrder.AppUser = ShowingOrder_ShowingOrderDTO.AppUser == null ? null : new AppUser
            {
                Id = ShowingOrder_ShowingOrderDTO.AppUser.Id,
                Username = ShowingOrder_ShowingOrderDTO.AppUser.Username,
                DisplayName = ShowingOrder_ShowingOrderDTO.AppUser.DisplayName,
                Address = ShowingOrder_ShowingOrderDTO.AppUser.Address,
                Email = ShowingOrder_ShowingOrderDTO.AppUser.Email,
                Phone = ShowingOrder_ShowingOrderDTO.AppUser.Phone,
                SexId = ShowingOrder_ShowingOrderDTO.AppUser.SexId,
                Birthday = ShowingOrder_ShowingOrderDTO.AppUser.Birthday,
                Avatar = ShowingOrder_ShowingOrderDTO.AppUser.Avatar,
                PositionId = ShowingOrder_ShowingOrderDTO.AppUser.PositionId,
                Department = ShowingOrder_ShowingOrderDTO.AppUser.Department,
                OrganizationId = ShowingOrder_ShowingOrderDTO.AppUser.OrganizationId,
                ProvinceId = ShowingOrder_ShowingOrderDTO.AppUser.ProvinceId,
                Longitude = ShowingOrder_ShowingOrderDTO.AppUser.Longitude,
                Latitude = ShowingOrder_ShowingOrderDTO.AppUser.Latitude,
                StatusId = ShowingOrder_ShowingOrderDTO.AppUser.StatusId,
                GPSUpdatedAt = ShowingOrder_ShowingOrderDTO.AppUser.GPSUpdatedAt,
                RowId = ShowingOrder_ShowingOrderDTO.AppUser.RowId,
            };
            ShowingOrder.Organization = ShowingOrder_ShowingOrderDTO.Organization == null ? null : new Organization
            {
                Id = ShowingOrder_ShowingOrderDTO.Organization.Id,
                Code = ShowingOrder_ShowingOrderDTO.Organization.Code,
                Name = ShowingOrder_ShowingOrderDTO.Organization.Name,
                ParentId = ShowingOrder_ShowingOrderDTO.Organization.ParentId,
                Path = ShowingOrder_ShowingOrderDTO.Organization.Path,
                Level = ShowingOrder_ShowingOrderDTO.Organization.Level,
                StatusId = ShowingOrder_ShowingOrderDTO.Organization.StatusId,
                Phone = ShowingOrder_ShowingOrderDTO.Organization.Phone,
                Email = ShowingOrder_ShowingOrderDTO.Organization.Email,
                Address = ShowingOrder_ShowingOrderDTO.Organization.Address,
                RowId = ShowingOrder_ShowingOrderDTO.Organization.RowId,
            };
            ShowingOrder.ShowingWarehouse = ShowingOrder_ShowingOrderDTO.ShowingWarehouse == null ? null : new ShowingWarehouse
            {
                Id = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.Id,
                Code = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.Code,
                Name = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.Name,
                Address = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.Address,
                OrganizationId = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.OrganizationId,
                ProvinceId = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.ProvinceId,
                DistrictId = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.DistrictId,
                WardId = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.WardId,
                StatusId = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.StatusId,
                RowId = ShowingOrder_ShowingOrderDTO.ShowingWarehouse.RowId,
            };
            ShowingOrder.Status = ShowingOrder_ShowingOrderDTO.Status == null ? null : new Status
            {
                Id = ShowingOrder_ShowingOrderDTO.Status.Id,
                Code = ShowingOrder_ShowingOrderDTO.Status.Code,
                Name = ShowingOrder_ShowingOrderDTO.Status.Name,
            };
            ShowingOrder.Store = ShowingOrder_ShowingOrderDTO.Store == null ? null : new Store
            {
                Id = ShowingOrder_ShowingOrderDTO.Store.Id,
                Code = ShowingOrder_ShowingOrderDTO.Store.Code,
                CodeDraft = ShowingOrder_ShowingOrderDTO.Store.CodeDraft,
                Name = ShowingOrder_ShowingOrderDTO.Store.Name,
                UnsignName = ShowingOrder_ShowingOrderDTO.Store.UnsignName,
                ParentStoreId = ShowingOrder_ShowingOrderDTO.Store.ParentStoreId,
                OrganizationId = ShowingOrder_ShowingOrderDTO.Store.OrganizationId,
                StoreTypeId = ShowingOrder_ShowingOrderDTO.Store.StoreTypeId,
                StoreGroupingId = ShowingOrder_ShowingOrderDTO.Store.StoreGroupingId,
                Telephone = ShowingOrder_ShowingOrderDTO.Store.Telephone,
                ProvinceId = ShowingOrder_ShowingOrderDTO.Store.ProvinceId,
                DistrictId = ShowingOrder_ShowingOrderDTO.Store.DistrictId,
                WardId = ShowingOrder_ShowingOrderDTO.Store.WardId,
                Address = ShowingOrder_ShowingOrderDTO.Store.Address,
                UnsignAddress = ShowingOrder_ShowingOrderDTO.Store.UnsignAddress,
                DeliveryAddress = ShowingOrder_ShowingOrderDTO.Store.DeliveryAddress,
                Latitude = ShowingOrder_ShowingOrderDTO.Store.Latitude,
                Longitude = ShowingOrder_ShowingOrderDTO.Store.Longitude,
                DeliveryLatitude = ShowingOrder_ShowingOrderDTO.Store.DeliveryLatitude,
                DeliveryLongitude = ShowingOrder_ShowingOrderDTO.Store.DeliveryLongitude,
                OwnerName = ShowingOrder_ShowingOrderDTO.Store.OwnerName,
                OwnerPhone = ShowingOrder_ShowingOrderDTO.Store.OwnerPhone,
                OwnerEmail = ShowingOrder_ShowingOrderDTO.Store.OwnerEmail,
                TaxCode = ShowingOrder_ShowingOrderDTO.Store.TaxCode,
                LegalEntity = ShowingOrder_ShowingOrderDTO.Store.LegalEntity,
                CreatorId = ShowingOrder_ShowingOrderDTO.Store.CreatorId,
                AppUserId = ShowingOrder_ShowingOrderDTO.Store.AppUserId,
                StatusId = ShowingOrder_ShowingOrderDTO.Store.StatusId,
                RowId = ShowingOrder_ShowingOrderDTO.Store.RowId,
                Used = ShowingOrder_ShowingOrderDTO.Store.Used,
                StoreScoutingId = ShowingOrder_ShowingOrderDTO.Store.StoreScoutingId,
                StoreStatusId = ShowingOrder_ShowingOrderDTO.Store.StoreStatusId,
            };
            ShowingOrder.ShowingOrderContents = ShowingOrder_ShowingOrderDTO.ShowingOrderContents?
                .Select(x => new ShowingOrderContent
                {
                    Id = x.Id,
                    ShowingItemId = x.ShowingItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    SalePrice = x.SalePrice,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
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
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                        Used = x.UnitOfMeasure.Used,
                        RowId = x.UnitOfMeasure.RowId,
                    },
                }).ToList();
            ShowingOrder.Stores = ShowingOrder_ShowingOrderDTO.Stores?.Select(x => new Store
            {
                Id = x.Id,
                Code = x.Code,
                CodeDraft = x.CodeDraft,
                Name = x.Name,
                UnsignName = x.UnsignName,
                ParentStoreId = x.ParentStoreId,
                OrganizationId = x.OrganizationId,
                StoreTypeId = x.StoreTypeId,
                StoreGroupingId = x.StoreGroupingId,
                Telephone = x.Telephone,
                ProvinceId = x.ProvinceId,
                DistrictId = x.DistrictId,
                WardId = x.WardId,
                Address = x.Address,
                UnsignAddress = x.UnsignAddress,
                DeliveryAddress = x.DeliveryAddress,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                DeliveryLatitude = x.DeliveryLatitude,
                DeliveryLongitude = x.DeliveryLongitude,
                OwnerName = x.OwnerName,
                OwnerPhone = x.OwnerPhone,
                OwnerEmail = x.OwnerEmail,
                TaxCode = x.TaxCode,
                LegalEntity = x.LegalEntity,
                CreatorId = x.CreatorId,
                AppUserId = x.AppUserId,
                StatusId = x.StatusId,
                RowId = x.RowId,
                Used = x.Used,
                StoreScoutingId = x.StoreScoutingId,
                StoreStatusId = x.StoreStatusId,
            }).ToList();
            ShowingOrder.BaseLanguage = CurrentContext.Language;
            return ShowingOrder;
        }

        private ShowingOrderFilter ConvertFilterDTOToFilterEntity(ShowingOrder_ShowingOrderFilterDTO ShowingOrder_ShowingOrderFilterDTO)
        {
            ShowingOrderFilter ShowingOrderFilter = new ShowingOrderFilter();
            ShowingOrderFilter.Selects = ShowingOrderSelect.ALL;
            ShowingOrderFilter.Skip = ShowingOrder_ShowingOrderFilterDTO.Skip;
            ShowingOrderFilter.Take = ShowingOrder_ShowingOrderFilterDTO.Take;
            ShowingOrderFilter.OrderBy = ShowingOrder_ShowingOrderFilterDTO.OrderBy;
            ShowingOrderFilter.OrderType = ShowingOrder_ShowingOrderFilterDTO.OrderType;

            ShowingOrderFilter.Id = ShowingOrder_ShowingOrderFilterDTO.Id;
            ShowingOrderFilter.Code = ShowingOrder_ShowingOrderFilterDTO.Code;
            ShowingOrderFilter.AppUserId = ShowingOrder_ShowingOrderFilterDTO.AppUserId;
            ShowingOrderFilter.OrganizationId = ShowingOrder_ShowingOrderFilterDTO.OrganizationId;
            ShowingOrderFilter.Date = ShowingOrder_ShowingOrderFilterDTO.Date;
            ShowingOrderFilter.ShowingWarehouseId = ShowingOrder_ShowingOrderFilterDTO.ShowingWarehouseId;
            ShowingOrderFilter.StatusId = ShowingOrder_ShowingOrderFilterDTO.StatusId;
            ShowingOrderFilter.Total = ShowingOrder_ShowingOrderFilterDTO.Total;
            ShowingOrderFilter.RowId = ShowingOrder_ShowingOrderFilterDTO.RowId;
            ShowingOrderFilter.CreatedAt = ShowingOrder_ShowingOrderFilterDTO.CreatedAt;
            ShowingOrderFilter.UpdatedAt = ShowingOrder_ShowingOrderFilterDTO.UpdatedAt;
            return ShowingOrderFilter;
        }
    }
}

