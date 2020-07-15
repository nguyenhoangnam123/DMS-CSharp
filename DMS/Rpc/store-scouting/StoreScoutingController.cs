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
using DMS.Services.MStoreScouting;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStore;
using DMS.Services.MStoreScoutingStatus;
using DMS.Services.MWard;
using DMS.Enums;

namespace DMS.Rpc.store_scouting
{
    public class StoreScoutingController : RpcController
    {
        private IAppUserService AppUserService;
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStoreService StoreService;
        private IStoreScoutingStatusService StoreScoutingStatusService;
        private IWardService WardService;
        private IStoreScoutingService StoreScoutingService;
        private ICurrentContext CurrentContext;
        public StoreScoutingController(
            IAppUserService AppUserService,
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStoreService StoreService,
            IStoreScoutingStatusService StoreScoutingStatusService,
            IWardService WardService,
            IStoreScoutingService StoreScoutingService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StoreService = StoreService;
            this.StoreScoutingStatusService = StoreScoutingStatusService;
            this.WardService = WardService;
            this.StoreScoutingService = StoreScoutingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreScoutingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingFilter StoreScoutingFilter = ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO);
            StoreScoutingFilter = await StoreScoutingService.ToFilter(StoreScoutingFilter);
            int count = await StoreScoutingService.Count(StoreScoutingFilter);
            return count;
        }

        [Route(StoreScoutingRoute.List), HttpPost]
        public async Task<ActionResult<List<StoreScouting_StoreScoutingDTO>>> List([FromBody] StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingFilter StoreScoutingFilter = ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO);
            StoreScoutingFilter = await StoreScoutingService.ToFilter(StoreScoutingFilter);
            List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);
            List<StoreScouting_StoreScoutingDTO> StoreScouting_StoreScoutingDTOs = StoreScoutings
                .Select(c => new StoreScouting_StoreScoutingDTO(c)).ToList();
            return StoreScouting_StoreScoutingDTOs;
        }

        [Route(StoreScoutingRoute.Get), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Get([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = await StoreScoutingService.Get(StoreScouting_StoreScoutingDTO.Id);
            return new StoreScouting_StoreScoutingDTO(StoreScouting);
        }

        [Route(StoreScoutingRoute.Create), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Create([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = ConvertDTOToEntity(StoreScouting_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Create(StoreScouting);
            StoreScouting_StoreScoutingDTO = new StoreScouting_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return StoreScouting_StoreScoutingDTO;
            else
                return BadRequest(StoreScouting_StoreScoutingDTO);
        }

        [Route(StoreScoutingRoute.Update), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Update([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = ConvertDTOToEntity(StoreScouting_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Update(StoreScouting);
            StoreScouting_StoreScoutingDTO = new StoreScouting_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return StoreScouting_StoreScoutingDTO;
            else
                return BadRequest(StoreScouting_StoreScoutingDTO);
        }

        [Route(StoreScoutingRoute.Reject), HttpPost]
        public async Task<ActionResult<StoreScouting_StoreScoutingDTO>> Reject([FromBody] StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreScouting_StoreScoutingDTO.Id))
                return Forbid();

            StoreScouting StoreScouting = ConvertDTOToEntity(StoreScouting_StoreScoutingDTO);
            StoreScouting = await StoreScoutingService.Reject(StoreScouting);
            StoreScouting_StoreScoutingDTO = new StoreScouting_StoreScoutingDTO(StoreScouting);
            if (StoreScouting.IsValidated)
                return StoreScouting_StoreScoutingDTO;
            else
                return BadRequest(StoreScouting_StoreScoutingDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter = await StoreScoutingService.ToFilter(StoreScoutingFilter);
            if (Id == 0)
            {

            }
            else
            {
                StoreScoutingFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreScoutingService.Count(StoreScoutingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private StoreScouting ConvertDTOToEntity(StoreScouting_StoreScoutingDTO StoreScouting_StoreScoutingDTO)
        {
            StoreScouting StoreScouting = new StoreScouting();
            StoreScouting.Id = StoreScouting_StoreScoutingDTO.Id;
            StoreScouting.Code = StoreScouting_StoreScoutingDTO.Code;
            StoreScouting.Name = StoreScouting_StoreScoutingDTO.Name;
            StoreScouting.OwnerPhone = StoreScouting_StoreScoutingDTO.OwnerPhone;
            StoreScouting.ProvinceId = StoreScouting_StoreScoutingDTO.ProvinceId;
            StoreScouting.DistrictId = StoreScouting_StoreScoutingDTO.DistrictId;
            StoreScouting.WardId = StoreScouting_StoreScoutingDTO.WardId;
            StoreScouting.Address = StoreScouting_StoreScoutingDTO.Address;
            StoreScouting.Latitude = StoreScouting_StoreScoutingDTO.Latitude;
            StoreScouting.Longitude = StoreScouting_StoreScoutingDTO.Longitude;
            StoreScouting.CreatorId = StoreScouting_StoreScoutingDTO.CreatorId;
            StoreScouting.StoreScoutingStatusId = StoreScouting_StoreScoutingDTO.StoreScoutingStatusId;
            StoreScouting.Link = StoreScouting_StoreScoutingDTO.Link;
            StoreScouting.RowId = StoreScouting_StoreScoutingDTO.RowId;
            StoreScouting.Creator = StoreScouting_StoreScoutingDTO.Creator == null ? null : new AppUser
            {
                Id = StoreScouting_StoreScoutingDTO.Creator.Id,
                Username = StoreScouting_StoreScoutingDTO.Creator.Username,
                DisplayName = StoreScouting_StoreScoutingDTO.Creator.DisplayName,
                Address = StoreScouting_StoreScoutingDTO.Creator.Address,
                Email = StoreScouting_StoreScoutingDTO.Creator.Email,
                Phone = StoreScouting_StoreScoutingDTO.Creator.Phone,
                PositionId = StoreScouting_StoreScoutingDTO.Creator.PositionId,
                Department = StoreScouting_StoreScoutingDTO.Creator.Department,
                OrganizationId = StoreScouting_StoreScoutingDTO.Creator.OrganizationId,
                StatusId = StoreScouting_StoreScoutingDTO.Creator.StatusId,
                Avatar = StoreScouting_StoreScoutingDTO.Creator.Avatar,
                ProvinceId = StoreScouting_StoreScoutingDTO.Creator.ProvinceId,
                SexId = StoreScouting_StoreScoutingDTO.Creator.SexId,
                Birthday = StoreScouting_StoreScoutingDTO.Creator.Birthday,
            };
            StoreScouting.District = StoreScouting_StoreScoutingDTO.District == null ? null : new District
            {
                Id = StoreScouting_StoreScoutingDTO.District.Id,
                Code = StoreScouting_StoreScoutingDTO.District.Code,
                Name = StoreScouting_StoreScoutingDTO.District.Name,
                Priority = StoreScouting_StoreScoutingDTO.District.Priority,
                ProvinceId = StoreScouting_StoreScoutingDTO.District.ProvinceId,
                StatusId = StoreScouting_StoreScoutingDTO.District.StatusId,
            };
            StoreScouting.Province = StoreScouting_StoreScoutingDTO.Province == null ? null : new Province
            {
                Id = StoreScouting_StoreScoutingDTO.Province.Id,
                Code = StoreScouting_StoreScoutingDTO.Province.Code,
                Name = StoreScouting_StoreScoutingDTO.Province.Name,
                Priority = StoreScouting_StoreScoutingDTO.Province.Priority,
                StatusId = StoreScouting_StoreScoutingDTO.Province.StatusId,
            };
            StoreScouting.StoreScoutingStatus = StoreScouting_StoreScoutingDTO.StoreScoutingStatus == null ? null : new StoreScoutingStatus
            {
                Id = StoreScouting_StoreScoutingDTO.StoreScoutingStatus.Id,
                Code = StoreScouting_StoreScoutingDTO.StoreScoutingStatus.Code,
                Name = StoreScouting_StoreScoutingDTO.StoreScoutingStatus.Name,
            };
            StoreScouting.Ward = StoreScouting_StoreScoutingDTO.Ward == null ? null : new Ward
            {
                Id = StoreScouting_StoreScoutingDTO.Ward.Id,
                Code = StoreScouting_StoreScoutingDTO.Ward.Code,
                Name = StoreScouting_StoreScoutingDTO.Ward.Name,
                Priority = StoreScouting_StoreScoutingDTO.Ward.Priority,
                DistrictId = StoreScouting_StoreScoutingDTO.Ward.DistrictId,
                StatusId = StoreScouting_StoreScoutingDTO.Ward.StatusId,
            };
            StoreScouting.BaseLanguage = CurrentContext.Language;
            return StoreScouting;
        }

        private StoreScoutingFilter ConvertFilterDTOToFilterEntity(StoreScouting_StoreScoutingFilterDTO StoreScouting_StoreScoutingFilterDTO)
        {
            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = StoreScouting_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = StoreScouting_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = StoreScouting_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = StoreScouting_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = StoreScouting_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = StoreScouting_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = StoreScouting_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.OwnerPhone = StoreScouting_StoreScoutingFilterDTO.OwnerPhone;
            StoreScoutingFilter.ProvinceId = StoreScouting_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.DistrictId = StoreScouting_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.WardId = StoreScouting_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.OrganizationId = StoreScouting_StoreScoutingFilterDTO.OrganizationId;
            StoreScoutingFilter.Address = StoreScouting_StoreScoutingFilterDTO.Address;
            StoreScoutingFilter.Latitude = StoreScouting_StoreScoutingFilterDTO.Latitude;
            StoreScoutingFilter.Longitude = StoreScouting_StoreScoutingFilterDTO.Longitude;
            StoreScoutingFilter.AppUserId = StoreScouting_StoreScoutingFilterDTO.AppUserId;
            StoreScoutingFilter.StoreScoutingStatusId = StoreScouting_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.CreatedAt = StoreScouting_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.UpdatedAt = StoreScouting_StoreScoutingFilterDTO.UpdatedAt;
            return StoreScoutingFilter;
        }

        [Route(StoreScoutingRoute.FilterListAppUser), HttpPost]
        public async Task<List<StoreScouting_AppUserDTO>> FilterListAppUser([FromBody] StoreScouting_AppUserFilterDTO StoreScouting_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreScouting_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreScouting_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = StoreScouting_AppUserFilterDTO.DisplayName;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreScouting_AppUserDTO> StoreScouting_AppUserDTOs = AppUsers
                .Select(x => new StoreScouting_AppUserDTO(x)).ToList();
            return StoreScouting_AppUserDTOs;
        }
        [Route(StoreScoutingRoute.FilterListDistrict), HttpPost]
        public async Task<List<StoreScouting_DistrictDTO>> FilterListDistrict([FromBody] StoreScouting_DistrictFilterDTO StoreScouting_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = StoreScouting_DistrictFilterDTO.Id;
            DistrictFilter.Code = StoreScouting_DistrictFilterDTO.Code;
            DistrictFilter.Name = StoreScouting_DistrictFilterDTO.Name;
            DistrictFilter.Priority = StoreScouting_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = StoreScouting_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = StoreScouting_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<StoreScouting_DistrictDTO> StoreScouting_DistrictDTOs = Districts
                .Select(x => new StoreScouting_DistrictDTO(x)).ToList();
            return StoreScouting_DistrictDTOs;
        }
        [Route(StoreScoutingRoute.FilterListOrganization), HttpPost]
        public async Task<List<StoreScouting_OrganizationDTO>> FilterListOrganization()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<StoreScouting_OrganizationDTO> StoreScouting_OrganizationDTOs = Organizations
                .Select(x => new StoreScouting_OrganizationDTO(x)).ToList();
            return StoreScouting_OrganizationDTOs;
        }
        [Route(StoreScoutingRoute.FilterListProvince), HttpPost]
        public async Task<List<StoreScouting_ProvinceDTO>> FilterListProvince([FromBody] StoreScouting_ProvinceFilterDTO StoreScouting_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = StoreScouting_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = StoreScouting_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = StoreScouting_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = StoreScouting_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = StoreScouting_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<StoreScouting_ProvinceDTO> StoreScouting_ProvinceDTOs = Provinces
                .Select(x => new StoreScouting_ProvinceDTO(x)).ToList();
            return StoreScouting_ProvinceDTOs;
        }
        [Route(StoreScoutingRoute.FilterListStore), HttpPost]
        public async Task<List<StoreScouting_StoreDTO>> FilterListStore([FromBody] StoreScouting_StoreFilterDTO StoreScouting_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreScouting_StoreFilterDTO.Id;
            StoreFilter.Code = StoreScouting_StoreFilterDTO.Code;
            StoreFilter.Name = StoreScouting_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreScouting_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreScouting_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreScouting_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreScouting_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = StoreScouting_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = StoreScouting_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreScouting_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreScouting_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreScouting_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreScouting_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreScouting_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreScouting_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreScouting_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreScouting_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreScouting_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreScouting_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreScouting_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreScouting_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreScouting_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreScouting_StoreDTO> StoreScouting_StoreDTOs = Stores
                .Select(x => new StoreScouting_StoreDTO(x)).ToList();
            return StoreScouting_StoreDTOs;
        }
        [Route(StoreScoutingRoute.FilterListStoreScoutingStatus), HttpPost]
        public async Task<List<StoreScouting_StoreScoutingStatusDTO>> FilterListStoreScoutingStatus([FromBody] StoreScouting_StoreScoutingStatusFilterDTO StoreScouting_StoreScoutingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingStatusFilter StoreScoutingStatusFilter = new StoreScoutingStatusFilter();
            StoreScoutingStatusFilter.Skip = 0;
            StoreScoutingStatusFilter.Take = 20;
            StoreScoutingStatusFilter.OrderBy = StoreScoutingStatusOrder.Id;
            StoreScoutingStatusFilter.OrderType = OrderType.ASC;
            StoreScoutingStatusFilter.Selects = StoreScoutingStatusSelect.ALL;
            StoreScoutingStatusFilter.Id = StoreScouting_StoreScoutingStatusFilterDTO.Id;
            StoreScoutingStatusFilter.Code = StoreScouting_StoreScoutingStatusFilterDTO.Code;
            StoreScoutingStatusFilter.Name = StoreScouting_StoreScoutingStatusFilterDTO.Name;

            List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);
            List<StoreScouting_StoreScoutingStatusDTO> StoreScouting_StoreScoutingStatusDTOs = StoreScoutingStatuses
                .Select(x => new StoreScouting_StoreScoutingStatusDTO(x)).ToList();
            return StoreScouting_StoreScoutingStatusDTOs;
        }
        [Route(StoreScoutingRoute.FilterListWard), HttpPost]
        public async Task<List<StoreScouting_WardDTO>> FilterListWard([FromBody] StoreScouting_WardFilterDTO StoreScouting_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = StoreScouting_WardFilterDTO.Id;
            WardFilter.Code = StoreScouting_WardFilterDTO.Code;
            WardFilter.Name = StoreScouting_WardFilterDTO.Name;
            WardFilter.Priority = StoreScouting_WardFilterDTO.Priority;
            WardFilter.DistrictId = StoreScouting_WardFilterDTO.DistrictId;
            WardFilter.StatusId = StoreScouting_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreScouting_WardDTO> StoreScouting_WardDTOs = Wards
                .Select(x => new StoreScouting_WardDTO(x)).ToList();
            return StoreScouting_WardDTOs;
        }

        [Route(StoreScoutingRoute.SingleListAppUser), HttpPost]
        public async Task<List<StoreScouting_AppUserDTO>> SingleListAppUser([FromBody] StoreScouting_AppUserFilterDTO StoreScouting_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreScouting_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreScouting_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = StoreScouting_AppUserFilterDTO.DisplayName;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreScouting_AppUserDTO> StoreScouting_AppUserDTOs = AppUsers
                .Select(x => new StoreScouting_AppUserDTO(x)).ToList();
            return StoreScouting_AppUserDTOs;
        }
        [Route(StoreScoutingRoute.SingleListDistrict), HttpPost]
        public async Task<List<StoreScouting_DistrictDTO>> SingleListDistrict([FromBody] StoreScouting_DistrictFilterDTO StoreScouting_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = StoreScouting_DistrictFilterDTO.Id;
            DistrictFilter.Code = StoreScouting_DistrictFilterDTO.Code;
            DistrictFilter.Name = StoreScouting_DistrictFilterDTO.Name;
            DistrictFilter.Priority = StoreScouting_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = StoreScouting_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = StoreScouting_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<StoreScouting_DistrictDTO> StoreScouting_DistrictDTOs = Districts
                .Select(x => new StoreScouting_DistrictDTO(x)).ToList();
            return StoreScouting_DistrictDTOs;
        }
        [Route(StoreScoutingRoute.SingleListOrganization), HttpPost]
        public async Task<List<StoreScouting_OrganizationDTO>> SingleListOrganization()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<StoreScouting_OrganizationDTO> StoreScouting_OrganizationDTOs = Organizations
                .Select(x => new StoreScouting_OrganizationDTO(x)).ToList();
            return StoreScouting_OrganizationDTOs;
        }
        [Route(StoreScoutingRoute.SingleListProvince), HttpPost]
        public async Task<List<StoreScouting_ProvinceDTO>> SingleListProvince([FromBody] StoreScouting_ProvinceFilterDTO StoreScouting_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = StoreScouting_ProvinceFilterDTO.Id;
            ProvinceFilter.Code = StoreScouting_ProvinceFilterDTO.Code;
            ProvinceFilter.Name = StoreScouting_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = StoreScouting_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = StoreScouting_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<StoreScouting_ProvinceDTO> StoreScouting_ProvinceDTOs = Provinces
                .Select(x => new StoreScouting_ProvinceDTO(x)).ToList();
            return StoreScouting_ProvinceDTOs;
        }
        [Route(StoreScoutingRoute.SingleListStore), HttpPost]
        public async Task<List<StoreScouting_StoreDTO>> SingleListStore([FromBody] StoreScouting_StoreFilterDTO StoreScouting_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreScouting_StoreFilterDTO.Id;
            StoreFilter.Code = StoreScouting_StoreFilterDTO.Code;
            StoreFilter.Name = StoreScouting_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreScouting_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreScouting_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreScouting_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreScouting_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = StoreScouting_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = StoreScouting_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreScouting_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreScouting_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreScouting_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreScouting_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreScouting_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreScouting_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreScouting_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreScouting_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreScouting_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreScouting_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreScouting_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreScouting_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreScouting_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreScouting_StoreDTO> StoreScouting_StoreDTOs = Stores
                .Select(x => new StoreScouting_StoreDTO(x)).ToList();
            return StoreScouting_StoreDTOs;
        }
        [Route(StoreScoutingRoute.SingleListStoreScoutingStatus), HttpPost]
        public async Task<List<StoreScouting_StoreScoutingStatusDTO>> SingleListStoreScoutingStatus([FromBody] StoreScouting_StoreScoutingStatusFilterDTO StoreScouting_StoreScoutingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingStatusFilter StoreScoutingStatusFilter = new StoreScoutingStatusFilter();
            StoreScoutingStatusFilter.Skip = 0;
            StoreScoutingStatusFilter.Take = 20;
            StoreScoutingStatusFilter.OrderBy = StoreScoutingStatusOrder.Id;
            StoreScoutingStatusFilter.OrderType = OrderType.ASC;
            StoreScoutingStatusFilter.Selects = StoreScoutingStatusSelect.ALL;
            StoreScoutingStatusFilter.Id = StoreScouting_StoreScoutingStatusFilterDTO.Id;
            StoreScoutingStatusFilter.Code = StoreScouting_StoreScoutingStatusFilterDTO.Code;
            StoreScoutingStatusFilter.Name = StoreScouting_StoreScoutingStatusFilterDTO.Name;

            List<StoreScoutingStatus> StoreScoutingStatuses = await StoreScoutingStatusService.List(StoreScoutingStatusFilter);
            List<StoreScouting_StoreScoutingStatusDTO> StoreScouting_StoreScoutingStatusDTOs = StoreScoutingStatuses
                .Select(x => new StoreScouting_StoreScoutingStatusDTO(x)).ToList();
            return StoreScouting_StoreScoutingStatusDTOs;
        }
        [Route(StoreScoutingRoute.SingleListWard), HttpPost]
        public async Task<List<StoreScouting_WardDTO>> SingleListWard([FromBody] StoreScouting_WardFilterDTO StoreScouting_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = StoreScouting_WardFilterDTO.Id;
            WardFilter.Code = StoreScouting_WardFilterDTO.Code;
            WardFilter.Name = StoreScouting_WardFilterDTO.Name;
            WardFilter.Priority = StoreScouting_WardFilterDTO.Priority;
            WardFilter.DistrictId = StoreScouting_WardFilterDTO.DistrictId;
            WardFilter.StatusId = StoreScouting_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreScouting_WardDTO> StoreScouting_WardDTOs = Wards
                .Select(x => new StoreScouting_WardDTO(x)).ToList();
            return StoreScouting_WardDTOs;
        }

    }
}

