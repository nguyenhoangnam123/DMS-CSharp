using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
using DMS.Services.MERouteContent;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MProblem;
using DMS.Services.MProduct;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MTaxType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checking
{
    public partial class StoreCheckingController
    {
        [Route(StoreCheckingRoute.FilterListAppUser), HttpPost]
        public async Task<List<StoreChecking_AppUserDTO>> FilterListAppUser([FromBody] StoreChecking_AppUserFilterDTO StoreChecking_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreChecking_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreChecking_AppUserFilterDTO.Username;
            AppUserFilter.Password = StoreChecking_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = StoreChecking_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = StoreChecking_AppUserFilterDTO.Address;
            AppUserFilter.Email = StoreChecking_AppUserFilterDTO.Email;
            AppUserFilter.Phone = StoreChecking_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = StoreChecking_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = StoreChecking_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = StoreChecking_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = StoreChecking_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = StoreChecking_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = StoreChecking_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = StoreChecking_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreChecking_AppUserDTO> StoreChecking_AppUserDTOs = AppUsers
                .Select(x => new StoreChecking_AppUserDTO(x)).ToList();
            return StoreChecking_AppUserDTOs;
        }

        [Route(StoreCheckingRoute.FilterListStore), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> FilterListStore([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreChecking_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreChecking_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.SingleListAlbum), HttpPost]
        public async Task<List<StoreChecking_AlbumDTO>> SingleListAlbum([FromBody] StoreChecking_AlbumFilterDTO StoreChecking_AlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AlbumFilter AlbumFilter = new AlbumFilter();
            AlbumFilter.Skip = 0;
            AlbumFilter.Take = 20;
            AlbumFilter.OrderBy = AlbumOrder.Id;
            AlbumFilter.OrderType = OrderType.ASC;
            AlbumFilter.Selects = AlbumSelect.ALL;
            AlbumFilter.Id = StoreChecking_AlbumFilterDTO.Id;
            AlbumFilter.Name = StoreChecking_AlbumFilterDTO.Name;
            AlbumFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<StoreChecking_AlbumDTO> StoreChecking_AlbumDTOs = Albums
                .Select(x => new StoreChecking_AlbumDTO(x)).ToList();
            return StoreChecking_AlbumDTOs;
        }
        [Route(StoreCheckingRoute.SingleListAppUser), HttpPost]
        public async Task<List<StoreChecking_AppUserDTO>> SingleListAppUser([FromBody] StoreChecking_AppUserFilterDTO StoreChecking_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = StoreChecking_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreChecking_AppUserFilterDTO.Username;
            AppUserFilter.Password = StoreChecking_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = StoreChecking_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = StoreChecking_AppUserFilterDTO.Address;
            AppUserFilter.Email = StoreChecking_AppUserFilterDTO.Email;
            AppUserFilter.Phone = StoreChecking_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = StoreChecking_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = StoreChecking_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = StoreChecking_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = StoreChecking_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = StoreChecking_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = StoreChecking_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = StoreChecking_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<StoreChecking_AppUserDTO> StoreChecking_AppUserDTOs = AppUsers
                .Select(x => new StoreChecking_AppUserDTO(x)).ToList();
            return StoreChecking_AppUserDTOs;
        }

        [Route(StoreCheckingRoute.SingleListEroute), HttpPost]
        public async Task<List<StoreChecking_ERouteDTO>> SingleListEroute(StoreChecking_ERouteFilterDTO StoreChecking_ERouteFilterDTO)
        {
            ERouteFilter ERouteFilter = new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                Selects = ERouteSelect.Id | ERouteSelect.Name | ERouteSelect.Code
            };

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            List<StoreChecking_ERouteDTO> StoreChecking_ERouteDTOs = ERoutes
                .Select(x => new StoreChecking_ERouteDTO(x)).ToList();
            return StoreChecking_ERouteDTOs;
        }

        [Route(StoreCheckingRoute.SingleListStore), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> SingleListStore([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<StoreChecking_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] StoreChecking_StoreGroupingFilterDTO StoreChecking_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = StoreChecking_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = StoreChecking_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = StoreChecking_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = StoreChecking_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<StoreChecking_StoreGroupingDTO> StoreChecking_StoreGroupingDTOs = StoreGroupings
                .Select(x => new StoreChecking_StoreGroupingDTO(x)).ToList();
            return StoreChecking_StoreGroupingDTOs;
        }
        [Route(StoreCheckingRoute.SingleListStoreType), HttpPost]
        public async Task<List<StoreChecking_StoreTypeDTO>> SingleListStoreType([FromBody] StoreChecking_StoreTypeFilterDTO StoreChecking_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = StoreChecking_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = StoreChecking_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = StoreChecking_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<StoreChecking_StoreTypeDTO> StoreChecking_StoreTypeDTOs = StoreTypes
                .Select(x => new StoreChecking_StoreTypeDTO(x)).ToList();
            return StoreChecking_StoreTypeDTOs;
        }

        [Route(StoreCheckingRoute.SingleListTaxType), HttpPost]
        public async Task<List<StoreChecking_TaxTypeDTO>> SingleListTaxType([FromBody] StoreChecking_TaxTypeFilterDTO StoreChecking_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = StoreChecking_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = StoreChecking_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = StoreChecking_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<StoreChecking_TaxTypeDTO> StoreChecking_TaxTypeDTOs = TaxTypes
                .Select(x => new StoreChecking_TaxTypeDTO(x)).ToList();
            return StoreChecking_TaxTypeDTOs;
        }

        [Route(StoreCheckingRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<StoreChecking_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] StoreChecking_UnitOfMeasureFilterDTO StoreChecking_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Product = await ProductService.Get(StoreChecking_UnitOfMeasureFilterDTO.ProductId.Equal ?? 0);

            List<StoreChecking_UnitOfMeasureDTO> StoreChecking_UnitOfMeasureDTOs = new List<StoreChecking_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null && Product.UnitOfMeasureGrouping.StatusId == Enums.StatusEnum.ACTIVE.Id)
            {
                StoreChecking_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new StoreChecking_UnitOfMeasureDTO(x)).ToList();
            }
            StoreChecking_UnitOfMeasureDTOs.Add(new StoreChecking_UnitOfMeasureDTO
            {
                Id = Product.UnitOfMeasure.Id,
                Code = Product.UnitOfMeasure.Code,
                Name = Product.UnitOfMeasure.Name,
                Description = Product.UnitOfMeasure.Description,
                StatusId = Product.UnitOfMeasure.StatusId,
                Factor = 1
            });
            return StoreChecking_UnitOfMeasureDTOs;
        }

        [Route(StoreCheckingRoute.SingleListProblemType), HttpPost]
        public async Task<List<StoreChecking_ProblemTypeDTO>> SingleListProblemType()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter();
            ProblemTypeFilter.Skip = 0;
            ProblemTypeFilter.Take = 20;
            ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
            ProblemTypeFilter.OrderType = OrderType.ASC;
            ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;

            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            List<StoreChecking_ProblemTypeDTO> StoreChecking_ProblemTypeDTOs = ProblemTypes

                .Select(x => new StoreChecking_ProblemTypeDTO(x)).ToList();
            return StoreChecking_ProblemTypeDTOs;
        }

        [Route(StoreCheckingRoute.SingleListProvince), HttpPost]
        public async Task<List<StoreChecking_ProvinceDTO>> SingleListProvince([FromBody] StoreChecking_ProvinceFilterDTO StoreChecking_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = StoreChecking_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = StoreChecking_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<StoreChecking_ProvinceDTO> StoreChecking_ProvinceDTOs = Provinces
                .Select(x => new StoreChecking_ProvinceDTO(x)).ToList();
            return StoreChecking_ProvinceDTOs;
        }
        [Route(StoreCheckingRoute.SingleListDistrict), HttpPost]
        public async Task<List<StoreChecking_DistrictDTO>> SingleListDistrict([FromBody] StoreChecking_DistrictFilterDTO StoreChecking_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = StoreChecking_DistrictFilterDTO.Id;
            DistrictFilter.Name = StoreChecking_DistrictFilterDTO.Name;
            DistrictFilter.Priority = StoreChecking_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = StoreChecking_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<StoreChecking_DistrictDTO> StoreChecking_DistrictDTOs = Districts
                .Select(x => new StoreChecking_DistrictDTO(x)).ToList();
            return StoreChecking_DistrictDTOs;
        }
        [Route(StoreCheckingRoute.SingleListWard), HttpPost]
        public async Task<List<StoreChecking_WardDTO>> SingleListWard([FromBody] StoreChecking_WardFilterDTO StoreChecking_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = StoreChecking_WardFilterDTO.Id;
            WardFilter.Name = StoreChecking_WardFilterDTO.Name;
            WardFilter.DistrictId = StoreChecking_WardFilterDTO.DistrictId;
            WardFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreChecking_WardDTO> StoreChecking_WardDTOs = Wards
                .Select(x => new StoreChecking_WardDTO(x)).ToList();
            return StoreChecking_WardDTOs;
        }

        [Route(StoreCheckingRoute.CountBanner), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreChecking_BannerFilterDTO StoreChecking_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = StoreChecking_BannerFilterDTO.Skip;
            BannerFilter.Take = StoreChecking_BannerFilterDTO.Take;
            BannerFilter.OrderBy = StoreChecking_BannerFilterDTO.OrderBy;
            BannerFilter.OrderType = StoreChecking_BannerFilterDTO.OrderType;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            BannerFilter = BannerService.ToFilter(BannerFilter);
            int count = await BannerService.Count(BannerFilter);
            return count;
        }

        [Route(StoreCheckingRoute.ListBanner), HttpPost]
        public async Task<ActionResult<List<StoreChecking_BannerDTO>>> List([FromBody] StoreChecking_BannerFilterDTO StoreChecking_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = StoreChecking_BannerFilterDTO.Skip;
            BannerFilter.Take = StoreChecking_BannerFilterDTO.Take;
            BannerFilter.OrderBy = StoreChecking_BannerFilterDTO.OrderBy;
            BannerFilter.OrderType = StoreChecking_BannerFilterDTO.OrderType;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            BannerFilter = BannerService.ToFilter(BannerFilter);
            List<Banner> Banners = await BannerService.List(BannerFilter);
            List<StoreChecking_BannerDTO> StoreChecking_BannerDTOs = Banners
                .Select(c => new StoreChecking_BannerDTO(c)).ToList();
            return StoreChecking_BannerDTOs;
        }

        [Route(StoreCheckingRoute.CountStorePlanned), HttpPost]
        public async Task<long> CountStorePlanned([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = StoreChecking_StoreFilterDTO.Skip;
            StoreFilter.Take = StoreChecking_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStorePlanned(StoreFilter, StoreChecking_StoreFilterDTO.ERouteId);
        }

        [Route(StoreCheckingRoute.ListStorePlanned), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> ListStorePlanned([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);
            
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = StoreChecking_StoreFilterDTO.Skip;
            StoreFilter.Take = StoreChecking_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStorePlanned(StoreFilter, StoreChecking_StoreFilterDTO.ERouteId);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.CountStoreUnPlanned), HttpPost]
        public async Task<long> CountStoreUnPlanned([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = StoreChecking_StoreFilterDTO.Skip;
            StoreFilter.Take = StoreChecking_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStoreUnPlanned(StoreFilter, StoreChecking_StoreFilterDTO.ERouteId);
        }

        [Route(StoreCheckingRoute.ListStoreUnPlanned), HttpPost]
        public async Task<List<StoreChecking_StoreDTO>> ListStoreUnPlanned([FromBody] StoreChecking_StoreFilterDTO StoreChecking_StoreFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = StoreChecking_StoreFilterDTO.Skip;
            StoreFilter.Take = StoreChecking_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreChecking_StoreFilterDTO.Id;
            StoreFilter.Code = StoreChecking_StoreFilterDTO.Code;
            StoreFilter.Name = StoreChecking_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreChecking_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreFilter.StoreTypeId = StoreChecking_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreChecking_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreChecking_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = StoreChecking_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = StoreChecking_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreChecking_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreChecking_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreChecking_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreChecking_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreChecking_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreChecking_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = StoreChecking_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = StoreChecking_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = StoreChecking_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreChecking_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreChecking_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStoreUnPlanned(StoreFilter, StoreChecking_StoreFilterDTO.ERouteId);
            List<StoreChecking_StoreDTO> StoreChecking_StoreDTOs = Stores
                .Select(x => new StoreChecking_StoreDTO(x)).ToList();
            return StoreChecking_StoreDTOs;
        }

        [Route(StoreCheckingRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] StoreChecking_ItemFilterDTO StoreChecking_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = StoreChecking_ItemFilterDTO.Id;
            ItemFilter.Code = StoreChecking_ItemFilterDTO.Code;
            ItemFilter.Name = StoreChecking_ItemFilterDTO.Name;
            ItemFilter.OtherName = StoreChecking_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = StoreChecking_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = StoreChecking_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = StoreChecking_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = StoreChecking_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = StoreChecking_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = StoreChecking_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = StoreChecking_ItemFilterDTO.SupplierId;
            return await ItemService.Count(ItemFilter);
        }

        [Route(StoreCheckingRoute.ListItem), HttpPost]
        public async Task<List<StoreChecking_ItemDTO>> ListItem([FromBody] StoreChecking_ItemFilterDTO StoreChecking_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = StoreChecking_ItemFilterDTO.Skip;
            ItemFilter.Take = StoreChecking_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = StoreChecking_ItemFilterDTO.Id;
            ItemFilter.Code = StoreChecking_ItemFilterDTO.Code;
            ItemFilter.Name = StoreChecking_ItemFilterDTO.Name;
            ItemFilter.OtherName = StoreChecking_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = StoreChecking_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = StoreChecking_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = StoreChecking_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = StoreChecking_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = StoreChecking_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = StoreChecking_ItemFilterDTO.ScanCode;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = StoreChecking_ItemFilterDTO.SupplierId;

            if (StoreChecking_ItemFilterDTO.StoreId != null && StoreChecking_ItemFilterDTO.StoreId.Equal.HasValue)
            {
                List<Item> Items = await IndirectSalesOrderService.ListItem(ItemFilter, StoreChecking_ItemFilterDTO.StoreId.Equal.Value);
                List<StoreChecking_ItemDTO> StoreChecking_ItemDTOs = Items
                    .Select(x => new StoreChecking_ItemDTO(x)).ToList();
                return StoreChecking_ItemDTOs;
            }
            else
            {
                List<Item> Items = await ItemService.List(ItemFilter);
                List<StoreChecking_ItemDTO> StoreChecking_ItemDTOs = Items
                    .Select(x => new StoreChecking_ItemDTO(x)).ToList();
                return StoreChecking_ItemDTOs;
            }
        }

        [Route(StoreCheckingRoute.CountProblem), HttpPost]
        public async Task<long> CountProblem([FromBody] StoreChecking_ProblemFilterDTO StoreChecking_ProblemFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Id = StoreChecking_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = StoreChecking_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.CreatorId = StoreChecking_ProblemFilterDTO.CreatorId;
            ProblemFilter.StoreId = StoreChecking_ProblemFilterDTO.StoreId;
            ProblemFilter.NoteAt = StoreChecking_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = StoreChecking_ProblemFilterDTO.CompletedAt;
            ProblemFilter.ProblemTypeId = StoreChecking_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.ProblemStatusId = StoreChecking_ProblemFilterDTO.ProblemStatusId;
            return await ProblemService.Count(ProblemFilter);
        }

        [Route(StoreCheckingRoute.ListProblem), HttpPost]
        public async Task<List<StoreChecking_ProblemDTO>> ListProblem([FromBody] StoreChecking_ProblemFilterDTO StoreChecking_ProblemFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Skip = StoreChecking_ProblemFilterDTO.Skip;
            ProblemFilter.Take = StoreChecking_ProblemFilterDTO.Take;
            ProblemFilter.OrderBy = ProblemOrder.NoteAt;
            ProblemFilter.OrderType = OrderType.ASC;
            ProblemFilter.Selects = ProblemSelect.ALL;
            ProblemFilter.Id = StoreChecking_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = StoreChecking_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.CreatorId = StoreChecking_ProblemFilterDTO.CreatorId;
            ProblemFilter.StoreId = StoreChecking_ProblemFilterDTO.StoreId;
            ProblemFilter.NoteAt = StoreChecking_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = StoreChecking_ProblemFilterDTO.CompletedAt;
            ProblemFilter.ProblemTypeId = StoreChecking_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.ProblemStatusId = StoreChecking_ProblemFilterDTO.ProblemStatusId;

            List<Problem> Problems = await ProblemService.List(ProblemFilter);
            List<StoreChecking_ProblemDTO> StoreChecking_ProblemDTOs = Problems
                .Select(x => new StoreChecking_ProblemDTO(x)).ToList();
            return StoreChecking_ProblemDTOs;
        }

        [Route(StoreCheckingRoute.CountSurvey), HttpPost]
        public async Task<long> CountSurvey([FromBody] StoreChecking_SurveyFilterDTO StoreChecking_SurveyFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = StoreChecking_SurveyFilterDTO.Skip;
            SurveyFilter.Take = StoreChecking_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = StoreChecking_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = StoreChecking_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = StoreChecking_SurveyFilterDTO.Id;
            SurveyFilter.Title = StoreChecking_SurveyFilterDTO.Title;
            SurveyFilter.Description = StoreChecking_SurveyFilterDTO.Description;
            SurveyFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await SurveyService.Count(SurveyFilter);
        }

        [Route(StoreCheckingRoute.ListSurvey), HttpPost]
        public async Task<List<StoreChecking_SurveyDTO>> ListSurvey([FromBody] StoreChecking_SurveyFilterDTO StoreChecking_SurveyFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = StoreChecking_SurveyFilterDTO.Skip;
            SurveyFilter.Take = StoreChecking_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = StoreChecking_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = StoreChecking_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = StoreChecking_SurveyFilterDTO.Id;
            SurveyFilter.Title = StoreChecking_SurveyFilterDTO.Title;
            SurveyFilter.Description = StoreChecking_SurveyFilterDTO.Description;
            SurveyFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Survey> Surveys = await SurveyService.List(SurveyFilter);
            List<StoreChecking_SurveyDTO> StoreChecking_SurveyDTOs = Surveys
                .Select(x => new StoreChecking_SurveyDTO(x)).ToList();
            return StoreChecking_SurveyDTOs;
        }

        [Route(StoreCheckingRoute.CountStoreScouting), HttpPost]
        public async Task<long> CountStoreScouting([FromBody] StoreChecking_StoreScoutingFilterDTO StoreChecking_StoreScoutingFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = StoreChecking_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = StoreChecking_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = StoreChecking_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = StoreChecking_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = StoreChecking_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = StoreChecking_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = StoreChecking_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.CreatedAt = StoreChecking_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.CreatorId = new IdFilter { Equal = appUser.Id };
            StoreScoutingFilter.DistrictId = StoreChecking_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.OrganizationId = StoreChecking_StoreScoutingFilterDTO.OrganizationId;
            StoreScoutingFilter.ProvinceId = StoreChecking_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.WardId = StoreChecking_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.StoreScoutingStatusId = StoreChecking_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.OwnerPhone = StoreChecking_StoreScoutingFilterDTO.OwnerPhone;

            return await StoreScoutingService.Count(StoreScoutingFilter);
        }

        public async Task<List<StoreChecking_StoreScoutingDTO>> ListStoreScouting([FromBody] StoreChecking_StoreScoutingFilterDTO StoreChecking_StoreScoutingFilterDTO)
        {
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = StoreChecking_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = StoreChecking_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = StoreChecking_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = StoreChecking_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = StoreChecking_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = StoreChecking_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = StoreChecking_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.CreatedAt = StoreChecking_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.CreatorId = new IdFilter { Equal = appUser.Id };
            StoreScoutingFilter.DistrictId = StoreChecking_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.OrganizationId = StoreChecking_StoreScoutingFilterDTO.OrganizationId;
            StoreScoutingFilter.ProvinceId = StoreChecking_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.WardId = StoreChecking_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.StoreScoutingStatusId = StoreChecking_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.OwnerPhone = StoreChecking_StoreScoutingFilterDTO.OwnerPhone;

            List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);
            List<StoreChecking_StoreScoutingDTO> StoreChecking_StoreScoutingDTOs = StoreScoutings
                .Select(x => new StoreChecking_StoreScoutingDTO(x)).ToList();
            return StoreChecking_StoreScoutingDTOs;
        }
    }
}

