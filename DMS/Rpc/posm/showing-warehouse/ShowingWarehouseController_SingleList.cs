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
using DMS.Services.MShowingWarehouse;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MWard;
using DMS.Services.MShowingInventory;
using DMS.Services.MAppUser;
using DMS.Services.MShowingItem;

namespace DMS.Rpc.posm.showing_warehouse
{
    public partial class ShowingWarehouseController : RpcController
    {
        [Route(ShowingWarehouseRoute.SingleListDistrict), HttpPost]
        public async Task<List<ShowingWarehouse_DistrictDTO>> SingleListDistrict([FromBody] ShowingWarehouse_DistrictFilterDTO ShowingWarehouse_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = ShowingWarehouse_DistrictFilterDTO.Id;
            DistrictFilter.Code = ShowingWarehouse_DistrictFilterDTO.Code;
            DistrictFilter.Name = ShowingWarehouse_DistrictFilterDTO.Name;
            DistrictFilter.Priority = ShowingWarehouse_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = ShowingWarehouse_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = ShowingWarehouse_DistrictFilterDTO.StatusId;
            DistrictFilter.StatusId = new IdFilter{ Equal = 1 };
            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<ShowingWarehouse_DistrictDTO> ShowingWarehouse_DistrictDTOs = Districts
                .Select(x => new ShowingWarehouse_DistrictDTO(x)).ToList();
            return ShowingWarehouse_DistrictDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListOrganization), HttpPost]
        public async Task<List<ShowingWarehouse_OrganizationDTO>> SingleListOrganization([FromBody] ShowingWarehouse_OrganizationFilterDTO ShowingWarehouse_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ShowingWarehouse_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ShowingWarehouse_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ShowingWarehouse_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ShowingWarehouse_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ShowingWarehouse_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ShowingWarehouse_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = ShowingWarehouse_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = ShowingWarehouse_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = ShowingWarehouse_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = ShowingWarehouse_OrganizationFilterDTO.Address;
            OrganizationFilter.IsDisplay = true;
            OrganizationFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ShowingWarehouse_OrganizationDTO> ShowingWarehouse_OrganizationDTOs = Organizations
                .Select(x => new ShowingWarehouse_OrganizationDTO(x)).ToList();
            return ShowingWarehouse_OrganizationDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListProvince), HttpPost]
        public async Task<List<ShowingWarehouse_ProvinceDTO>> SingleListProvince([FromBody] ShowingWarehouse_ProvinceFilterDTO ShowingWarehouse_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = ShowingWarehouse_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = ShowingWarehouse_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = ShowingWarehouse_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = ShowingWarehouse_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = ShowingWarehouse_ProvinceFilterDTO.StatusId;
            ProvinceFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<ShowingWarehouse_ProvinceDTO> ShowingWarehouse_ProvinceDTOs = Provinces
                .Select(x => new ShowingWarehouse_ProvinceDTO(x)).ToList();
            return ShowingWarehouse_ProvinceDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListStatus), HttpPost]
        public async Task<List<ShowingWarehouse_StatusDTO>> SingleListStatus([FromBody] ShowingWarehouse_StatusFilterDTO ShowingWarehouse_StatusFilterDTO)
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
            List<ShowingWarehouse_StatusDTO> ShowingWarehouse_StatusDTOs = Statuses
                .Select(x => new ShowingWarehouse_StatusDTO(x)).ToList();
            return ShowingWarehouse_StatusDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListWard), HttpPost]
        public async Task<List<ShowingWarehouse_WardDTO>> SingleListWard([FromBody] ShowingWarehouse_WardFilterDTO ShowingWarehouse_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = ShowingWarehouse_WardFilterDTO.Id;
            WardFilter.Code = ShowingWarehouse_WardFilterDTO.Code;
            WardFilter.Name = ShowingWarehouse_WardFilterDTO.Name;
            WardFilter.Priority = ShowingWarehouse_WardFilterDTO.Priority;
            WardFilter.DistrictId = ShowingWarehouse_WardFilterDTO.DistrictId;
            WardFilter.StatusId = ShowingWarehouse_WardFilterDTO.StatusId;
            WardFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<ShowingWarehouse_WardDTO> ShowingWarehouse_WardDTOs = Wards
                .Select(x => new ShowingWarehouse_WardDTO(x)).ToList();
            return ShowingWarehouse_WardDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListShowingInventory), HttpPost]
        public async Task<List<ShowingWarehouse_ShowingInventoryDTO>> SingleListShowingInventory([FromBody] ShowingWarehouse_ShowingInventoryFilterDTO ShowingWarehouse_ShowingInventoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingInventoryFilter ShowingInventoryFilter = new ShowingInventoryFilter();
            ShowingInventoryFilter.Skip = 0;
            ShowingInventoryFilter.Take = 20;
            ShowingInventoryFilter.OrderBy = ShowingInventoryOrder.Id;
            ShowingInventoryFilter.OrderType = OrderType.ASC;
            ShowingInventoryFilter.Selects = ShowingInventorySelect.ALL;
            ShowingInventoryFilter.Id = ShowingWarehouse_ShowingInventoryFilterDTO.Id;
            ShowingInventoryFilter.ShowingWarehouseId = ShowingWarehouse_ShowingInventoryFilterDTO.ShowingWarehouseId;
            ShowingInventoryFilter.ShowingItemId = ShowingWarehouse_ShowingInventoryFilterDTO.ShowingItemId;
            ShowingInventoryFilter.SaleStock = ShowingWarehouse_ShowingInventoryFilterDTO.SaleStock;
            ShowingInventoryFilter.AccountingStock = ShowingWarehouse_ShowingInventoryFilterDTO.AccountingStock;
            ShowingInventoryFilter.AppUserId = ShowingWarehouse_ShowingInventoryFilterDTO.AppUserId;
            List<ShowingInventory> ShowingInventories = await ShowingInventoryService.List(ShowingInventoryFilter);
            List<ShowingWarehouse_ShowingInventoryDTO> ShowingWarehouse_ShowingInventoryDTOs = ShowingInventories
                .Select(x => new ShowingWarehouse_ShowingInventoryDTO(x)).ToList();
            return ShowingWarehouse_ShowingInventoryDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListAppUser), HttpPost]
        public async Task<List<ShowingWarehouse_AppUserDTO>> SingleListAppUser([FromBody] ShowingWarehouse_AppUserFilterDTO ShowingWarehouse_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ShowingWarehouse_AppUserFilterDTO.Id;
            AppUserFilter.Username = ShowingWarehouse_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ShowingWarehouse_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ShowingWarehouse_AppUserFilterDTO.Address;
            AppUserFilter.Email = ShowingWarehouse_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ShowingWarehouse_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = ShowingWarehouse_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = ShowingWarehouse_AppUserFilterDTO.Birthday;
            AppUserFilter.PositionId = ShowingWarehouse_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = ShowingWarehouse_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = ShowingWarehouse_AppUserFilterDTO.OrganizationId;
            AppUserFilter.ProvinceId = ShowingWarehouse_AppUserFilterDTO.ProvinceId;
            AppUserFilter.StatusId = ShowingWarehouse_AppUserFilterDTO.StatusId;
            AppUserFilter.StatusId = new IdFilter{ Equal = 1 };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ShowingWarehouse_AppUserDTO> ShowingWarehouse_AppUserDTOs = AppUsers
                .Select(x => new ShowingWarehouse_AppUserDTO(x)).ToList();
            return ShowingWarehouse_AppUserDTOs;
        }
        [Route(ShowingWarehouseRoute.SingleListShowingItem), HttpPost]
        public async Task<List<ShowingWarehouse_ShowingItemDTO>> SingleListShowingItem([FromBody] ShowingWarehouse_ShowingItemFilterDTO ShowingWarehouse_ShowingItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter();
            ShowingItemFilter.Skip = 0;
            ShowingItemFilter.Take = 20;
            ShowingItemFilter.OrderBy = ShowingItemOrder.Id;
            ShowingItemFilter.OrderType = OrderType.ASC;
            ShowingItemFilter.Selects = ShowingItemSelect.ALL;
            ShowingItemFilter.Id = ShowingWarehouse_ShowingItemFilterDTO.Id;
            ShowingItemFilter.Code = ShowingWarehouse_ShowingItemFilterDTO.Code;
            ShowingItemFilter.Name = ShowingWarehouse_ShowingItemFilterDTO.Name;
            ShowingItemFilter.ShowingCategoryId = ShowingWarehouse_ShowingItemFilterDTO.ShowingCategoryId;
            ShowingItemFilter.UnitOfMeasureId = ShowingWarehouse_ShowingItemFilterDTO.UnitOfMeasureId;
            ShowingItemFilter.SalePrice = ShowingWarehouse_ShowingItemFilterDTO.SalePrice;
            ShowingItemFilter.Description = ShowingWarehouse_ShowingItemFilterDTO.Description;
            ShowingItemFilter.StatusId = ShowingWarehouse_ShowingItemFilterDTO.StatusId;
            ShowingItemFilter.RowId = ShowingWarehouse_ShowingItemFilterDTO.RowId;
            ShowingItemFilter.StatusId = new IdFilter{ Equal = 1 };
            List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);
            List<ShowingWarehouse_ShowingItemDTO> ShowingWarehouse_ShowingItemDTOs = ShowingItems
                .Select(x => new ShowingWarehouse_ShowingItemDTO(x)).ToList();
            return ShowingWarehouse_ShowingItemDTOs;
        }
    }
}

