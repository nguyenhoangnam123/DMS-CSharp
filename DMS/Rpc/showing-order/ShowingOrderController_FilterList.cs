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
using DMS.Entities;
using DMS.Services.MShowingOrder;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MShowingWarehouse;
using DMS.Services.MStatus;
using DMS.Services.MShowingItem;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.showing_order
{
    public partial class ShowingOrderController : RpcController
    {
        [Route(ShowingOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<ShowingOrder_AppUserDTO>> FilterListAppUser([FromBody] ShowingOrder_AppUserFilterDTO ShowingOrder_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ShowingOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = ShowingOrder_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ShowingOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ShowingOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = ShowingOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ShowingOrder_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = ShowingOrder_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = ShowingOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.PositionId = ShowingOrder_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = ShowingOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = ShowingOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.ProvinceId = ShowingOrder_AppUserFilterDTO.ProvinceId;
            AppUserFilter.StatusId = ShowingOrder_AppUserFilterDTO.StatusId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ShowingOrder_AppUserDTO> ShowingOrder_AppUserDTOs = AppUsers
                .Select(x => new ShowingOrder_AppUserDTO(x)).ToList();
            return ShowingOrder_AppUserDTOs;
        }
        [Route(ShowingOrderRoute.FilterListOrganization), HttpPost]
        public async Task<List<ShowingOrder_OrganizationDTO>> FilterListOrganization([FromBody] ShowingOrder_OrganizationFilterDTO ShowingOrder_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ShowingOrder_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ShowingOrder_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ShowingOrder_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ShowingOrder_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ShowingOrder_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ShowingOrder_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = ShowingOrder_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = ShowingOrder_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = ShowingOrder_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = ShowingOrder_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ShowingOrder_OrganizationDTO> ShowingOrder_OrganizationDTOs = Organizations
                .Select(x => new ShowingOrder_OrganizationDTO(x)).ToList();
            return ShowingOrder_OrganizationDTOs;
        }
        [Route(ShowingOrderRoute.FilterListShowingWarehouse), HttpPost]
        public async Task<List<ShowingOrder_ShowingWarehouseDTO>> FilterListShowingWarehouse([FromBody] ShowingOrder_ShowingWarehouseFilterDTO ShowingOrder_ShowingWarehouseFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingWarehouseFilter ShowingWarehouseFilter = new ShowingWarehouseFilter();
            ShowingWarehouseFilter.Skip = 0;
            ShowingWarehouseFilter.Take = 20;
            ShowingWarehouseFilter.OrderBy = ShowingWarehouseOrder.Id;
            ShowingWarehouseFilter.OrderType = OrderType.ASC;
            ShowingWarehouseFilter.Selects = ShowingWarehouseSelect.ALL;
            ShowingWarehouseFilter.Id = ShowingOrder_ShowingWarehouseFilterDTO.Id;
            ShowingWarehouseFilter.Code = ShowingOrder_ShowingWarehouseFilterDTO.Code;
            ShowingWarehouseFilter.Name = ShowingOrder_ShowingWarehouseFilterDTO.Name;
            ShowingWarehouseFilter.Address = ShowingOrder_ShowingWarehouseFilterDTO.Address;
            ShowingWarehouseFilter.OrganizationId = ShowingOrder_ShowingWarehouseFilterDTO.OrganizationId;
            ShowingWarehouseFilter.ProvinceId = ShowingOrder_ShowingWarehouseFilterDTO.ProvinceId;
            ShowingWarehouseFilter.DistrictId = ShowingOrder_ShowingWarehouseFilterDTO.DistrictId;
            ShowingWarehouseFilter.WardId = ShowingOrder_ShowingWarehouseFilterDTO.WardId;
            ShowingWarehouseFilter.StatusId = ShowingOrder_ShowingWarehouseFilterDTO.StatusId;
            ShowingWarehouseFilter.RowId = ShowingOrder_ShowingWarehouseFilterDTO.RowId;

            List<ShowingWarehouse> ShowingWarehouses = await ShowingWarehouseService.List(ShowingWarehouseFilter);
            List<ShowingOrder_ShowingWarehouseDTO> ShowingOrder_ShowingWarehouseDTOs = ShowingWarehouses
                .Select(x => new ShowingOrder_ShowingWarehouseDTO(x)).ToList();
            return ShowingOrder_ShowingWarehouseDTOs;
        }
        [Route(ShowingOrderRoute.FilterListStatus), HttpPost]
        public async Task<List<ShowingOrder_StatusDTO>> FilterListStatus([FromBody] ShowingOrder_StatusFilterDTO ShowingOrder_StatusFilterDTO)
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
            List<ShowingOrder_StatusDTO> ShowingOrder_StatusDTOs = Statuses
                .Select(x => new ShowingOrder_StatusDTO(x)).ToList();
            return ShowingOrder_StatusDTOs;
        }
        [Route(ShowingOrderRoute.FilterListShowingItem), HttpPost]
        public async Task<List<ShowingOrder_ShowingItemDTO>> FilterListShowingItem([FromBody] ShowingOrder_ShowingItemFilterDTO ShowingOrder_ShowingItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter();
            ShowingItemFilter.Skip = 0;
            ShowingItemFilter.Take = 20;
            ShowingItemFilter.OrderBy = ShowingItemOrder.Id;
            ShowingItemFilter.OrderType = OrderType.ASC;
            ShowingItemFilter.Selects = ShowingItemSelect.ALL;
            ShowingItemFilter.Id = ShowingOrder_ShowingItemFilterDTO.Id;
            ShowingItemFilter.Code = ShowingOrder_ShowingItemFilterDTO.Code;
            ShowingItemFilter.Name = ShowingOrder_ShowingItemFilterDTO.Name;
            ShowingItemFilter.CategoryId = ShowingOrder_ShowingItemFilterDTO.CategoryId;
            ShowingItemFilter.UnitOfMeasureId = ShowingOrder_ShowingItemFilterDTO.UnitOfMeasureId;
            ShowingItemFilter.SalePrice = ShowingOrder_ShowingItemFilterDTO.SalePrice;
            ShowingItemFilter.Desception = ShowingOrder_ShowingItemFilterDTO.Desception;
            ShowingItemFilter.StatusId = ShowingOrder_ShowingItemFilterDTO.StatusId;
            ShowingItemFilter.RowId = ShowingOrder_ShowingItemFilterDTO.RowId;

            List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);
            List<ShowingOrder_ShowingItemDTO> ShowingOrder_ShowingItemDTOs = ShowingItems
                .Select(x => new ShowingOrder_ShowingItemDTO(x)).ToList();
            return ShowingOrder_ShowingItemDTOs;
        }
        [Route(ShowingOrderRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<ShowingOrder_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] ShowingOrder_UnitOfMeasureFilterDTO ShowingOrder_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = ShowingOrder_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = ShowingOrder_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = ShowingOrder_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = ShowingOrder_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = ShowingOrder_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<ShowingOrder_UnitOfMeasureDTO> ShowingOrder_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new ShowingOrder_UnitOfMeasureDTO(x)).ToList();
            return ShowingOrder_UnitOfMeasureDTOs;
        }
        [Route(ShowingOrderRoute.FilterListStore), HttpPost]
        public async Task<List<ShowingOrder_StoreDTO>> FilterListStore([FromBody] ShowingOrder_StoreFilterDTO ShowingOrder_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ShowingOrder_StoreFilterDTO.Id;
            StoreFilter.Code = ShowingOrder_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ShowingOrder_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ShowingOrder_StoreFilterDTO.Name;
            StoreFilter.UnsignName = ShowingOrder_StoreFilterDTO.UnsignName;
            StoreFilter.ParentStoreId = ShowingOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ShowingOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ShowingOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ShowingOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = ShowingOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ShowingOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ShowingOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ShowingOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = ShowingOrder_StoreFilterDTO.Address;
            StoreFilter.UnsignAddress = ShowingOrder_StoreFilterDTO.UnsignAddress;
            StoreFilter.DeliveryAddress = ShowingOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ShowingOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ShowingOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ShowingOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ShowingOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ShowingOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ShowingOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ShowingOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.CreatorId = ShowingOrder_StoreFilterDTO.CreatorId;
            StoreFilter.AppUserId = ShowingOrder_StoreFilterDTO.AppUserId;
            StoreFilter.StatusId = ShowingOrder_StoreFilterDTO.StatusId;
            StoreFilter.StoreStatusId = ShowingOrder_StoreFilterDTO.StoreStatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ShowingOrder_StoreDTO> ShowingOrder_StoreDTOs = Stores
                .Select(x => new ShowingOrder_StoreDTO(x)).ToList();
            return ShowingOrder_StoreDTOs;
        }
    }
}

