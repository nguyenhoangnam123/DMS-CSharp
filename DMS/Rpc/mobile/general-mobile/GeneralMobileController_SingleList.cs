using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MProblem;
using DMS.Services.MProduct;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MTaxType;
using DMS.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.general_mobile
{
    public partial class MobileController
    {
        private const long THIS_MONTH = 1;
        private const long LAST_MONTH = 2;
        private const long THIS_QUARTER = 3;
        private const long YEAR = 4;

        [Route(GeneralMobileRoute.SingleListTime), HttpPost]
        public List<GeneralMobile_EnumList> SingleListTime()
        {
            List<GeneralMobile_EnumList> Dashborad_EnumLists = new List<GeneralMobile_EnumList>();
            Dashborad_EnumLists.Add(new GeneralMobile_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Dashborad_EnumLists.Add(new GeneralMobile_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            Dashborad_EnumLists.Add(new GeneralMobile_EnumList { Id = THIS_QUARTER, Name = "Quý này" });
            Dashborad_EnumLists.Add(new GeneralMobile_EnumList { Id = YEAR, Name = "Năm" });
            return Dashborad_EnumLists;
        }

        [Route(GeneralMobileRoute.SingleListAlbum), HttpPost]
        public async Task<List<GeneralMobile_AlbumDTO>> SingleListAlbum([FromBody] GeneralMobile_AlbumFilterDTO GeneralMobile_AlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AlbumFilter AlbumFilter = new AlbumFilter();
            AlbumFilter.Skip = 0;
            AlbumFilter.Take = 20;
            AlbumFilter.OrderBy = AlbumOrder.Id;
            AlbumFilter.OrderType = OrderType.ASC;
            AlbumFilter.Selects = AlbumSelect.ALL;
            AlbumFilter.Id = GeneralMobile_AlbumFilterDTO.Id;
            AlbumFilter.Name = GeneralMobile_AlbumFilterDTO.Name;
            AlbumFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<GeneralMobile_AlbumDTO> GeneralMobile_AlbumDTOs = Albums
                .Select(x => new GeneralMobile_AlbumDTO(x)).ToList();
            return GeneralMobile_AlbumDTOs;
        }
        [Route(GeneralMobileRoute.SingleListAppUser), HttpPost]
        public async Task<List<GeneralMobile_AppUserDTO>> SingleListAppUser([FromBody] GeneralMobile_AppUserFilterDTO GeneralMobile_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = GeneralMobile_AppUserFilterDTO.Id;
            AppUserFilter.Username = GeneralMobile_AppUserFilterDTO.Username;
            AppUserFilter.Password = GeneralMobile_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = GeneralMobile_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = GeneralMobile_AppUserFilterDTO.Address;
            AppUserFilter.Email = GeneralMobile_AppUserFilterDTO.Email;
            AppUserFilter.Phone = GeneralMobile_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = GeneralMobile_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = GeneralMobile_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = GeneralMobile_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = GeneralMobile_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = GeneralMobile_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = GeneralMobile_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = GeneralMobile_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<GeneralMobile_AppUserDTO> GeneralMobile_AppUserDTOs = AppUsers
                .Select(x => new GeneralMobile_AppUserDTO(x)).ToList();
            return GeneralMobile_AppUserDTOs;
        }

        [Route(GeneralMobileRoute.SingleListEroute), HttpPost]
        public async Task<List<GeneralMobile_ERouteDTO>> SingleListEroute(GeneralMobile_ERouteFilterDTO GeneralMobile_ERouteFilterDTO)
        {
            ERouteFilter ERouteFilter = new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                AppUserId = new IdFilter { Equal = CurrentContext.UserId },
                Selects = ERouteSelect.Id | ERouteSelect.Name | ERouteSelect.Code
            };

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            List<GeneralMobile_ERouteDTO> GeneralMobile_ERouteDTOs = ERoutes
                .Select(x => new GeneralMobile_ERouteDTO(x)).ToList();
            return GeneralMobile_ERouteDTOs;
        }

        [Route(GeneralMobileRoute.SingleListStore), HttpPost]
        public async Task<List<GeneralMobile_StoreDTO>> SingleListStore([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            StoreFilter.OrganizationId = new IdFilter { Equal = AppUser.OrganizationId };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<GeneralMobile_StoreDTO> GeneralMobile_StoreDTOs = Stores
                .Select(x => new GeneralMobile_StoreDTO(x)).ToList();
            return GeneralMobile_StoreDTOs;
        }

        [Route(GeneralMobileRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<GeneralMobile_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] GeneralMobile_StoreGroupingFilterDTO GeneralMobile_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = GeneralMobile_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = GeneralMobile_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = GeneralMobile_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = GeneralMobile_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<GeneralMobile_StoreGroupingDTO> GeneralMobile_StoreGroupingDTOs = StoreGroupings
                .Select(x => new GeneralMobile_StoreGroupingDTO(x)).ToList();
            return GeneralMobile_StoreGroupingDTOs;
        }

        [Route(GeneralMobileRoute.SingleListStoreStatus), HttpPost]
        public async Task<List<GeneralMobile_StoreStatusDTO>> SingleListStoreStatus([FromBody] GeneralMobile_StoreStatusFilterDTO GeneralMobile_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = new IdFilter { NotEqual = StoreStatusEnum.ALL.Id };
            StoreStatusFilter.Code = GeneralMobile_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = GeneralMobile_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<GeneralMobile_StoreStatusDTO> GeneralMobile_StoreStatusDTOs = StoreStatuses
                .Select(x => new GeneralMobile_StoreStatusDTO(x)).ToList();
            return GeneralMobile_StoreStatusDTOs;
        }

        [Route(GeneralMobileRoute.SingleListStoreType), HttpPost]
        public async Task<List<GeneralMobile_StoreTypeDTO>> SingleListStoreType([FromBody] GeneralMobile_StoreTypeFilterDTO GeneralMobile_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = GeneralMobile_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = GeneralMobile_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = GeneralMobile_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<GeneralMobile_StoreTypeDTO> GeneralMobile_StoreTypeDTOs = StoreTypes
                .Select(x => new GeneralMobile_StoreTypeDTO(x)).ToList();
            return GeneralMobile_StoreTypeDTOs;
        }

        [Route(GeneralMobileRoute.SingleListTaxType), HttpPost]
        public async Task<List<GeneralMobile_TaxTypeDTO>> SingleListTaxType([FromBody] GeneralMobile_TaxTypeFilterDTO GeneralMobile_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = GeneralMobile_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = GeneralMobile_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = GeneralMobile_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<GeneralMobile_TaxTypeDTO> GeneralMobile_TaxTypeDTOs = TaxTypes
                .Select(x => new GeneralMobile_TaxTypeDTO(x)).ToList();
            return GeneralMobile_TaxTypeDTOs;
        }

        [Route(GeneralMobileRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<GeneralMobile_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] GeneralMobile_UnitOfMeasureFilterDTO GeneralMobile_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Product = await ProductService.Get(GeneralMobile_UnitOfMeasureFilterDTO.ProductId.Equal ?? 0);

            List<GeneralMobile_UnitOfMeasureDTO> GeneralMobile_UnitOfMeasureDTOs = new List<GeneralMobile_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null && Product.UnitOfMeasureGrouping.StatusId == Enums.StatusEnum.ACTIVE.Id)
            {
                GeneralMobile_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new GeneralMobile_UnitOfMeasureDTO(x)).ToList();
            }
            GeneralMobile_UnitOfMeasureDTOs.Add(new GeneralMobile_UnitOfMeasureDTO
            {
                Id = Product.UnitOfMeasure.Id,
                Code = Product.UnitOfMeasure.Code,
                Name = Product.UnitOfMeasure.Name,
                Description = Product.UnitOfMeasure.Description,
                StatusId = Product.UnitOfMeasure.StatusId,
                Factor = 1
            });
            return GeneralMobile_UnitOfMeasureDTOs;
        }

        [Route(GeneralMobileRoute.SingleListProblemType), HttpPost]
        public async Task<List<GeneralMobile_ProblemTypeDTO>> SingleListProblemType()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter();
            ProblemTypeFilter.Skip = 0;
            ProblemTypeFilter.Take = 20;
            ProblemTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ProblemTypeFilter.OrderBy = ProblemTypeOrder.Id;
            ProblemTypeFilter.OrderType = OrderType.ASC;
            ProblemTypeFilter.Selects = ProblemTypeSelect.ALL;

            List<ProblemType> ProblemTypes = await ProblemTypeService.List(ProblemTypeFilter);
            List<GeneralMobile_ProblemTypeDTO> GeneralMobile_ProblemTypeDTOs = ProblemTypes

                .Select(x => new GeneralMobile_ProblemTypeDTO(x)).ToList();
            return GeneralMobile_ProblemTypeDTOs;
        }

        [Route(GeneralMobileRoute.SingleListStoreScoutingType), HttpPost]
        public async Task<List<GeneralMobile_StoreScoutingTypeDTO>> SingleListStoreScoutingType()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreScoutingTypeFilter StoreScoutingTypeFilter = new StoreScoutingTypeFilter();
            StoreScoutingTypeFilter.Skip = 0;
            StoreScoutingTypeFilter.Take = 20;
            StoreScoutingTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            StoreScoutingTypeFilter.OrderBy = StoreScoutingTypeOrder.Id;
            StoreScoutingTypeFilter.OrderType = OrderType.ASC;
            StoreScoutingTypeFilter.Selects = StoreScoutingTypeSelect.ALL;

            List<StoreScoutingType> StoreScoutingTypes = await StoreScoutingTypeService.List(StoreScoutingTypeFilter);
            List<GeneralMobile_StoreScoutingTypeDTO> GeneralMobile_StoreScoutingTypeDTOs = StoreScoutingTypes

                .Select(x => new GeneralMobile_StoreScoutingTypeDTO(x)).ToList();
            return GeneralMobile_StoreScoutingTypeDTOs;
        }

        [Route(GeneralMobileRoute.SingleListBrand), HttpPost]
        public async Task<List<GeneralMobile_BrandDTO>> SingleListBrand([FromBody] GeneralMobile_BrandFilterDTO GeneralMobile_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Id = GeneralMobile_BrandFilterDTO.Id;
            BrandFilter.Code = GeneralMobile_BrandFilterDTO.Code;
            BrandFilter.Name = GeneralMobile_BrandFilterDTO.Name;
            BrandFilter.Description = GeneralMobile_BrandFilterDTO.Description;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            BrandFilter.UpdateTime = GeneralMobile_BrandFilterDTO.UpdateTime;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<GeneralMobile_BrandDTO> GeneralMobile_BrandDTOs = Brands
                .Select(x => new GeneralMobile_BrandDTO(x)).ToList();
            return GeneralMobile_BrandDTOs;
        }

        [Route(GeneralMobileRoute.SingleListColor), HttpPost]
        public async Task<List<GeneralMobile_ColorDTO>> SingleListColor([FromBody] GeneralMobile_ColorFilterDTO GeneralMobile_ColorFilterDTO)
        {
            ColorFilter ColorFilter = new ColorFilter();
            ColorFilter.Skip = 0;
            ColorFilter.Take = 20;
            ColorFilter.OrderBy = ColorOrder.Id;
            ColorFilter.OrderType = OrderType.ASC;
            ColorFilter.Selects = ColorSelect.ALL;

            List<Color> Colores = await ColorService.List(ColorFilter);
            List<GeneralMobile_ColorDTO> GeneralMobile_ColorDTOs = Colores
                .Select(x => new GeneralMobile_ColorDTO(x)).ToList();
            return GeneralMobile_ColorDTOs;
        }

        [Route(GeneralMobileRoute.SingleListSupplier), HttpPost]
        public async Task<List<GeneralMobile_SupplierDTO>> SingleListSupplier([FromBody] GeneralMobile_SupplierFilterDTO GeneralMobile_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = GeneralMobile_SupplierFilterDTO.Id;
            SupplierFilter.Code = GeneralMobile_SupplierFilterDTO.Code;
            SupplierFilter.Name = GeneralMobile_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = GeneralMobile_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<GeneralMobile_SupplierDTO> GeneralMobile_SupplierDTOs = Suppliers
                .Select(x => new GeneralMobile_SupplierDTO(x)).ToList();
            return GeneralMobile_SupplierDTOs;
        }

        [Route(GeneralMobileRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<GeneralMobile_ProductGroupingDTO>> SingleListProductGrouping([FromBody] GeneralMobile_ProductGroupingFilterDTO GeneralMobile_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<GeneralMobile_ProductGroupingDTO> GeneralMobile_ProductGroupingDTOs = ProductGroupings
                .Select(x => new GeneralMobile_ProductGroupingDTO(x)).ToList();
            return GeneralMobile_ProductGroupingDTOs;
        }

        [Route(GeneralMobileRoute.SingleListProvince), HttpPost]
        public async Task<List<GeneralMobile_ProvinceDTO>> SingleListProvince([FromBody] GeneralMobile_ProvinceFilterDTO GeneralMobile_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = GeneralMobile_ProvinceFilterDTO.Skip;
            ProvinceFilter.Take = GeneralMobile_ProvinceFilterDTO.Take;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = GeneralMobile_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = GeneralMobile_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<GeneralMobile_ProvinceDTO> GeneralMobile_ProvinceDTOs = Provinces
                .Select(x => new GeneralMobile_ProvinceDTO(x)).ToList();
            return GeneralMobile_ProvinceDTOs;
        }
        [Route(GeneralMobileRoute.SingleListDistrict), HttpPost]
        public async Task<List<GeneralMobile_DistrictDTO>> SingleListDistrict([FromBody] GeneralMobile_DistrictFilterDTO GeneralMobile_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = GeneralMobile_DistrictFilterDTO.Skip;
            DistrictFilter.Take = GeneralMobile_DistrictFilterDTO.Take;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = GeneralMobile_DistrictFilterDTO.Id;
            DistrictFilter.Name = GeneralMobile_DistrictFilterDTO.Name;
            DistrictFilter.Priority = GeneralMobile_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = GeneralMobile_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<GeneralMobile_DistrictDTO> GeneralMobile_DistrictDTOs = Districts
                .Select(x => new GeneralMobile_DistrictDTO(x)).ToList();
            return GeneralMobile_DistrictDTOs;
        }
        [Route(GeneralMobileRoute.SingleListWard), HttpPost]
        public async Task<List<GeneralMobile_WardDTO>> SingleListWard([FromBody] GeneralMobile_WardFilterDTO GeneralMobile_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = GeneralMobile_WardFilterDTO.Skip;
            WardFilter.Take = GeneralMobile_WardFilterDTO.Take;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = GeneralMobile_WardFilterDTO.Id;
            WardFilter.Name = GeneralMobile_WardFilterDTO.Name;
            WardFilter.DistrictId = GeneralMobile_WardFilterDTO.DistrictId;
            WardFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<GeneralMobile_WardDTO> GeneralMobile_WardDTOs = Wards
                .Select(x => new GeneralMobile_WardDTO(x)).ToList();
            return GeneralMobile_WardDTOs;
        }

        [Route(GeneralMobileRoute.SingleListStoreCheckingStatus), HttpPost]
        public async Task<List<GenericEnum>> SingleListStoreCheckingStatus()
        {
            return StoreCheckingStatusEnum.StoreCheckingStatusEnumList;
        }

        [Route(GeneralMobileRoute.SingleListStoreDraftType), HttpPost]
        public async Task<List<GenericEnum>> SingleListStoreDraftType()
        {
            return StoreDraftTypeEnum.StoreDraftTypeEnumList;
        }

        [Route(GeneralMobileRoute.CountBanner), HttpPost]
        public async Task<ActionResult<int>> CountBanner([FromBody] GeneralMobile_BannerFilterDTO GeneralMobile_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = GeneralMobile_BannerFilterDTO.Skip;
            BannerFilter.Take = GeneralMobile_BannerFilterDTO.Take;
            BannerFilter.OrderBy = GeneralMobile_BannerFilterDTO.OrderBy;
            BannerFilter.OrderType = GeneralMobile_BannerFilterDTO.OrderType;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            int count = await BannerService.Count(BannerFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListBanner), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_BannerDTO>>> ListBanner([FromBody] GeneralMobile_BannerFilterDTO GeneralMobile_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = GeneralMobile_BannerFilterDTO.Skip;
            BannerFilter.Take = GeneralMobile_BannerFilterDTO.Take;
            BannerFilter.OrderBy = BannerOrder.Priority;
            BannerFilter.OrderType = OrderType.ASC;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Banner> Banners = await BannerService.List(BannerFilter);
            List<GeneralMobile_BannerDTO> GeneralMobile_BannerDTOs = Banners
                .Select(c => new GeneralMobile_BannerDTO(c)).ToList();
            return GeneralMobile_BannerDTOs;
        }

        [Route(GeneralMobileRoute.GetBanner), HttpPost]
        public async Task<ActionResult<GeneralMobile_BannerDTO>> GetBanner([FromBody] GeneralMobile_BannerDTO GeneralMobile_BannerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Banner Banner = await BannerService.Get(GeneralMobile_BannerDTO.Id);
            return new GeneralMobile_BannerDTO(Banner);
        }

        [Route(GeneralMobileRoute.GetStore), HttpPost]
        public async Task<ActionResult<GeneralMobile_StoreDTO>> Get([FromBody] GeneralMobile_StoreDTO GeneralMobile_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Store Store = await StoreService.Get(GeneralMobile_StoreDTO.Id);
            GeneralMobile_StoreDTO = new GeneralMobile_StoreDTO(Store);
            DateTime Start = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            List<Album> Albums = await AlbumService.List(new AlbumFilter
            {
                StoreId = new IdFilter { Equal = GeneralMobile_StoreDTO.Id },
                Selects = AlbumSelect.ALL,
                ShootingAt = new DateFilter { GreaterEqual = Start, LessEqual = End },
                Skip = 0,
                Take = int.MaxValue,
            });
            GeneralMobile_StoreDTO.AlbumImageMappings = Albums
                .SelectMany(x => x.AlbumImageMappings
                .Where(x => x.StoreId == GeneralMobile_StoreDTO.Id)
                .Where(x => x.SaleEmployeeId.HasValue && x.SaleEmployeeId.Value == CurrentContext.UserId)
                .Where(x => Start <= x.ShootingAt && x.ShootingAt <= End)
                .Select(m => new GeneralMobile_AlbumImageMappingDTO
                {
                    AlbumId = m.AlbumId,
                    ImageId = m.ImageId,
                    StoreId = m.StoreId,
                    Distance = m.Distance,
                    Album = m.Album == null ? null : new GeneralMobile_AlbumDTO
                    {
                        Id = m.Album.Id,
                        Name = m.Album.Name,
                    },
                    Image = m.Image == null ? null : new GeneralMobile_ImageDTO
                    {
                        Id = m.Image.Id,
                        Name = m.Image.Name,
                        Url = m.Image.Url,
                    },
                })).ToList();

            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.Id,
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End },
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                StoreId = new IdFilter { Equal = GeneralMobile_StoreDTO.Id }
            });
            var StoreCheckingIds = StoreCheckings.Select(x => x.Id).ToList();
            var query = from scim in DataContext.StoreCheckingImageMapping
                        join a in DataContext.Album on scim.AlbumId equals a.Id
                        join i in DataContext.Image on scim.ImageId equals i.Id
                        where StoreCheckingIds.Contains(scim.StoreCheckingId) &&
                        scim.SaleEmployeeId == CurrentContext.UserId
                        select new GeneralMobile_AlbumImageMappingDTO
                        {
                            AlbumId = scim.AlbumId,
                            ImageId = scim.ImageId,
                            StoreId = scim.StoreId,
                            Album = new GeneralMobile_AlbumDTO
                            {
                                Id = a.Id,
                                Name = a.Name,
                            },
                            Image = new GeneralMobile_ImageDTO
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Url = i.Url,
                            },
                        };
            var StoreCheckingImageMappings = await query.ToListAsync();
            GeneralMobile_StoreDTO.AlbumImageMappings.AddRange(StoreCheckingImageMappings);
            return GeneralMobile_StoreDTO;
        }

        [Route(GeneralMobileRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStore(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
        }

        [Route(GeneralMobileRoute.ListStore), HttpPost]
        public async Task<List<GeneralMobile_StoreDTO>> ListStore([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStore(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
            List<GeneralMobile_StoreDTO> GeneralMobile_StoreDTOs = Stores
                .Select(x => new GeneralMobile_StoreDTO(x)).ToList();
            return GeneralMobile_StoreDTOs;
        }

        [Route(GeneralMobileRoute.CountBuyerStore), HttpPost]
        public async Task<long> CountBuyerStore([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            var StoreIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
            if (StoreIds.Any())
            {
                StoreFilter.Id = new IdFilter { In = StoreIds };
            }

            return await StoreCheckingService.CountStore(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
        }

        [Route(GeneralMobileRoute.ListBuyerStore), HttpPost]
        public async Task<List<GeneralMobile_StoreDTO>> ListBuyerStore([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            var StoreIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
            if (StoreIds.Any())
            {
                StoreFilter.Id = new IdFilter { In = StoreIds };
            }
            List<Store> Stores = await StoreCheckingService.ListStore(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
            List<GeneralMobile_StoreDTO> GeneralMobile_StoreDTOs = Stores
                .Select(x => new GeneralMobile_StoreDTO(x)).ToList();
            return GeneralMobile_StoreDTOs;
        }

        [Route(GeneralMobileRoute.CountStorePlanned), HttpPost]
        public async Task<long> CountStorePlanned([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStorePlanned(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
        }

        [Route(GeneralMobileRoute.ListStorePlanned), HttpPost]
        public async Task<List<GeneralMobile_StoreDTO>> ListStorePlanned([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStorePlanned(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
            List<GeneralMobile_StoreDTO> GeneralMobile_StoreDTOs = Stores
                .Select(x => new GeneralMobile_StoreDTO(x)).ToList();
            return GeneralMobile_StoreDTOs;
        }

        [Route(GeneralMobileRoute.CountStoreUnPlanned), HttpPost]
        public async Task<long> CountStoreUnPlanned([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStoreUnPlanned(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
        }

        [Route(GeneralMobileRoute.ListStoreUnPlanned), HttpPost]
        public async Task<List<GeneralMobile_StoreDTO>> ListStoreUnPlanned([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStoreUnPlanned(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
            List<GeneralMobile_StoreDTO> GeneralMobile_StoreDTOs = Stores
                .Select(x => new GeneralMobile_StoreDTO(x)).ToList();
            return GeneralMobile_StoreDTOs;
        }

        [Route(GeneralMobileRoute.CountStoreInScope), HttpPost]
        public async Task<long> CountStoreInScope([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStoreInScope(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
        }

        [Route(GeneralMobileRoute.ListStoreInScope), HttpPost]
        public async Task<List<GeneralMobile_StoreDTO>> ListStoreInScope([FromBody] GeneralMobile_StoreFilterDTO GeneralMobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = GeneralMobile_StoreFilterDTO.Search;
            StoreFilter.Skip = GeneralMobile_StoreFilterDTO.Skip;
            StoreFilter.Take = GeneralMobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = GeneralMobile_StoreFilterDTO.Id;
            StoreFilter.Code = GeneralMobile_StoreFilterDTO.Code;
            StoreFilter.Name = GeneralMobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = GeneralMobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = GeneralMobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = GeneralMobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = GeneralMobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = GeneralMobile_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = GeneralMobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = GeneralMobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = GeneralMobile_StoreFilterDTO.WardId;
            StoreFilter.Address = GeneralMobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = GeneralMobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = GeneralMobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = GeneralMobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = GeneralMobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = GeneralMobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = GeneralMobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = GeneralMobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = GeneralMobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = GeneralMobile_StoreFilterDTO.StoreStatusId;
            StoreFilter.StoreDraftTypeId = GeneralMobile_StoreFilterDTO.StoreDraftTypeId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStoreInScope(StoreFilter, GeneralMobile_StoreFilterDTO.ERouteId);
            List<GeneralMobile_StoreDTO> GeneralMobile_StoreDTOs = Stores
                .Select(x => new GeneralMobile_StoreDTO(x)).ToList();
            return GeneralMobile_StoreDTOs;
        }

        [Route(GeneralMobileRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] GeneralMobile_ItemFilterDTO GeneralMobile_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Search = GeneralMobile_ItemFilterDTO.Search;
            ItemFilter.Id = GeneralMobile_ItemFilterDTO.Id;
            ItemFilter.Code = GeneralMobile_ItemFilterDTO.Code;
            ItemFilter.Name = GeneralMobile_ItemFilterDTO.Name;
            ItemFilter.OtherName = GeneralMobile_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = GeneralMobile_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = GeneralMobile_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = GeneralMobile_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = GeneralMobile_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = GeneralMobile_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = GeneralMobile_ItemFilterDTO.ScanCode;
            ItemFilter.IsNew = GeneralMobile_ItemFilterDTO.IsNew;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = GeneralMobile_ItemFilterDTO.SupplierId;
            return await ItemService.Count(ItemFilter);
        }

        [Route(GeneralMobileRoute.ListItem), HttpPost]
        public async Task<List<GeneralMobile_ItemDTO>> ListItem([FromBody] GeneralMobile_ItemFilterDTO GeneralMobile_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Search = GeneralMobile_ItemFilterDTO.Search;
            ItemFilter.Skip = GeneralMobile_ItemFilterDTO.Skip;
            ItemFilter.Take = GeneralMobile_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.DESC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = GeneralMobile_ItemFilterDTO.Id;
            ItemFilter.Code = GeneralMobile_ItemFilterDTO.Code;
            ItemFilter.Name = GeneralMobile_ItemFilterDTO.Name;
            ItemFilter.OtherName = GeneralMobile_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = GeneralMobile_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = GeneralMobile_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = GeneralMobile_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = GeneralMobile_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = GeneralMobile_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = GeneralMobile_ItemFilterDTO.ScanCode;
            ItemFilter.IsNew = GeneralMobile_ItemFilterDTO.IsNew;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = GeneralMobile_ItemFilterDTO.SupplierId;

            if (GeneralMobile_ItemFilterDTO.StoreId == null)
                GeneralMobile_ItemFilterDTO.StoreId = new IdFilter();
            List<Item> Items = await IndirectSalesOrderService.ListItem(ItemFilter, CurrentContext.UserId, GeneralMobile_ItemFilterDTO.StoreId.Equal);
            List<GeneralMobile_ItemDTO> GeneralMobile_ItemDTOs = Items
                .Select(x => new GeneralMobile_ItemDTO(x)).ToList();
            return GeneralMobile_ItemDTOs;
        }

        [Route(GeneralMobileRoute.GetItem), HttpPost]
        public async Task<GeneralMobile_ItemDTO> GetItem([FromBody] GeneralMobile_ItemDTO GeneralMobile_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Item Item = await ItemService.Get(GeneralMobile_ItemDTO.Id);
            if (Item == null)
                return null;
            GeneralMobile_ItemDTO = new GeneralMobile_ItemDTO(Item);
            return GeneralMobile_ItemDTO;
        }

        [Route(GeneralMobileRoute.CountProblem), HttpPost]
        public async Task<long> CountProblem([FromBody] GeneralMobile_ProblemFilterDTO GeneralMobile_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Id = GeneralMobile_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = GeneralMobile_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            ProblemFilter.StoreId = GeneralMobile_ProblemFilterDTO.StoreId;
            ProblemFilter.NoteAt = GeneralMobile_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = GeneralMobile_ProblemFilterDTO.CompletedAt;
            ProblemFilter.ProblemTypeId = GeneralMobile_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.ProblemStatusId = GeneralMobile_ProblemFilterDTO.ProblemStatusId;
            return await ProblemService.Count(ProblemFilter);
        }

        [Route(GeneralMobileRoute.ListProblem), HttpPost]
        public async Task<List<GeneralMobile_ProblemDTO>> ListProblem([FromBody] GeneralMobile_ProblemFilterDTO GeneralMobile_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Skip = GeneralMobile_ProblemFilterDTO.Skip;
            ProblemFilter.Take = GeneralMobile_ProblemFilterDTO.Take;
            ProblemFilter.OrderBy = ProblemOrder.NoteAt;
            ProblemFilter.OrderType = OrderType.DESC;
            ProblemFilter.Selects = ProblemSelect.ALL;
            ProblemFilter.Id = GeneralMobile_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = GeneralMobile_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            ProblemFilter.StoreId = GeneralMobile_ProblemFilterDTO.StoreId;
            ProblemFilter.NoteAt = GeneralMobile_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = GeneralMobile_ProblemFilterDTO.CompletedAt;
            ProblemFilter.ProblemTypeId = GeneralMobile_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.ProblemStatusId = GeneralMobile_ProblemFilterDTO.ProblemStatusId;

            List<Problem> Problems = await ProblemService.List(ProblemFilter);
            List<GeneralMobile_ProblemDTO> GeneralMobile_ProblemDTOs = Problems
                .Select(x => new GeneralMobile_ProblemDTO(x)).ToList();
            return GeneralMobile_ProblemDTOs;
        }

        [Route(GeneralMobileRoute.GetProblem), HttpPost]
        public async Task<ActionResult<GeneralMobile_ProblemDTO>> Get([FromBody] GeneralMobile_ProblemDTO GeneralMobile_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Problem Problem = await ProblemService.Get(GeneralMobile_ProblemDTO.Id);
            return new GeneralMobile_ProblemDTO(Problem);
        }

        [Route(GeneralMobileRoute.CountSurvey), HttpPost]
        public async Task<long> CountSurvey([FromBody] GeneralMobile_SurveyFilterDTO GeneralMobile_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Now = StaticParams.DateTimeNow;
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = GeneralMobile_SurveyFilterDTO.Skip;
            SurveyFilter.Take = GeneralMobile_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = GeneralMobile_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = GeneralMobile_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = GeneralMobile_SurveyFilterDTO.Id;
            SurveyFilter.Title = GeneralMobile_SurveyFilterDTO.Title;
            SurveyFilter.Description = GeneralMobile_SurveyFilterDTO.Description;
            SurveyFilter.StartAt = new DateFilter { LessEqual = Now };
            SurveyFilter.EndAt = new DateFilter { GreaterEqual = Now };
            SurveyFilter.CreatorId = GeneralMobile_SurveyFilterDTO.CreatorId;
            SurveyFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await SurveyService.Count(SurveyFilter);
        }

        [Route(GeneralMobileRoute.ListSurvey), HttpPost]
        public async Task<List<GeneralMobile_SurveyDTO>> ListSurvey([FromBody] GeneralMobile_SurveyFilterDTO GeneralMobile_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Now = StaticParams.DateTimeNow;
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = GeneralMobile_SurveyFilterDTO.Skip;
            SurveyFilter.Take = GeneralMobile_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = GeneralMobile_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = GeneralMobile_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = GeneralMobile_SurveyFilterDTO.Id;
            SurveyFilter.Title = GeneralMobile_SurveyFilterDTO.Title;
            SurveyFilter.Description = GeneralMobile_SurveyFilterDTO.Description;
            SurveyFilter.StartAt = new DateFilter { LessEqual = Now };
            SurveyFilter.EndAt = new DateFilter { GreaterEqual = Now };
            SurveyFilter.CreatorId = GeneralMobile_SurveyFilterDTO.CreatorId;
            SurveyFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Survey> Surveys = await SurveyService.List(SurveyFilter);
            List<GeneralMobile_SurveyDTO> GeneralMobile_SurveyDTOs = Surveys
                .Select(x => new GeneralMobile_SurveyDTO(x)).ToList();
            return GeneralMobile_SurveyDTOs;
        }

        [Route(GeneralMobileRoute.CountStoreScouting), HttpPost]
        public async Task<long> CountStoreScouting([FromBody] GeneralMobile_StoreScoutingFilterDTO GeneralMobile_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = GeneralMobile_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = GeneralMobile_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = GeneralMobile_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = GeneralMobile_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = GeneralMobile_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = GeneralMobile_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = GeneralMobile_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.CreatedAt = GeneralMobile_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            StoreScoutingFilter.DistrictId = GeneralMobile_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreScoutingFilter.ProvinceId = GeneralMobile_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.WardId = GeneralMobile_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.StoreScoutingStatusId = GeneralMobile_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.OwnerPhone = GeneralMobile_StoreScoutingFilterDTO.OwnerPhone;

            return await StoreScoutingService.Count(StoreScoutingFilter);
        }

        [Route(GeneralMobileRoute.ListStoreScouting), HttpPost]
        public async Task<List<GeneralMobile_StoreScoutingDTO>> ListStoreScouting([FromBody] GeneralMobile_StoreScoutingFilterDTO GeneralMobile_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = GeneralMobile_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = GeneralMobile_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = GeneralMobile_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = GeneralMobile_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = GeneralMobile_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = GeneralMobile_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = GeneralMobile_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.CreatedAt = GeneralMobile_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            StoreScoutingFilter.DistrictId = GeneralMobile_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            StoreScoutingFilter.ProvinceId = GeneralMobile_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.WardId = GeneralMobile_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.StoreScoutingStatusId = GeneralMobile_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.OwnerPhone = GeneralMobile_StoreScoutingFilterDTO.OwnerPhone;

            List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);
            List<GeneralMobile_StoreScoutingDTO> GeneralMobile_StoreScoutingDTOs = StoreScoutings
                .Select(x => new GeneralMobile_StoreScoutingDTO(x)).ToList();
            return GeneralMobile_StoreScoutingDTOs;
        }

        [Route(GeneralMobileRoute.CountRewardHistory), HttpPost]
        public async Task<long> CountRewardHistory([FromBody] GeneralMobile_RewardHistoryFilterDTO GeneralMobile_RewardHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            RewardHistoryFilter RewardHistoryFilter = new RewardHistoryFilter();
            RewardHistoryFilter.Selects = RewardHistorySelect.ALL;
            RewardHistoryFilter.Skip = GeneralMobile_RewardHistoryFilterDTO.Skip;
            RewardHistoryFilter.Take = GeneralMobile_RewardHistoryFilterDTO.Take;
            RewardHistoryFilter.OrderBy = GeneralMobile_RewardHistoryFilterDTO.OrderBy;
            RewardHistoryFilter.OrderType = GeneralMobile_RewardHistoryFilterDTO.OrderType;

            RewardHistoryFilter.Id = GeneralMobile_RewardHistoryFilterDTO.Id;
            RewardHistoryFilter.CreatedAt = GeneralMobile_RewardHistoryFilterDTO.CreatedAt;
            RewardHistoryFilter.StoreId = GeneralMobile_RewardHistoryFilterDTO.StoreId;
            RewardHistoryFilter.Search = GeneralMobile_RewardHistoryFilterDTO.Search;
            RewardHistoryFilter.AppUserId = new IdFilter { Equal = appUser.Id };

            return await RewardHistoryService.Count(RewardHistoryFilter);
        }

        [Route(GeneralMobileRoute.ListRewardHistory), HttpPost]
        public async Task<List<GeneralMobile_RewardHistoryDTO>> ListRewardHistory([FromBody] GeneralMobile_RewardHistoryFilterDTO GeneralMobile_RewardHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            RewardHistoryFilter RewardHistoryFilter = new RewardHistoryFilter();
            RewardHistoryFilter.Selects = RewardHistorySelect.ALL;
            RewardHistoryFilter.Skip = GeneralMobile_RewardHistoryFilterDTO.Skip;
            RewardHistoryFilter.Take = GeneralMobile_RewardHistoryFilterDTO.Take;
            RewardHistoryFilter.OrderBy = GeneralMobile_RewardHistoryFilterDTO.OrderBy;
            RewardHistoryFilter.OrderType = GeneralMobile_RewardHistoryFilterDTO.OrderType;

            RewardHistoryFilter.Id = GeneralMobile_RewardHistoryFilterDTO.Id;
            RewardHistoryFilter.CreatedAt = GeneralMobile_RewardHistoryFilterDTO.CreatedAt;
            RewardHistoryFilter.StoreId = GeneralMobile_RewardHistoryFilterDTO.StoreId;
            RewardHistoryFilter.Search = GeneralMobile_RewardHistoryFilterDTO.Search;
            RewardHistoryFilter.AppUserId = new IdFilter { Equal = appUser.Id };

            List<RewardHistory> RewardHistorys = await RewardHistoryService.List(RewardHistoryFilter);
            List<GeneralMobile_RewardHistoryDTO> GeneralMobile_RewardHistoryDTOs = RewardHistorys
                .Select(x => new GeneralMobile_RewardHistoryDTO(x)).ToList();
            return GeneralMobile_RewardHistoryDTOs;
        }

        [Route(GeneralMobileRoute.GetRewardHistory), HttpPost]
        public async Task<ActionResult<GeneralMobile_RewardHistoryDTO>> GetRewardHistory([FromBody] GeneralMobile_RewardHistoryDTO GeneralMobile_RewardHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RewardHistory RewardHistory = await RewardHistoryService.Get(GeneralMobile_RewardHistoryDTO.Id);
            return new GeneralMobile_RewardHistoryDTO(RewardHistory);
        }

        [Route(GeneralMobileRoute.CreateRewardHistory), HttpPost]
        public async Task<ActionResult<GeneralMobile_RewardHistoryDTO>> CreateRewardHistory([FromBody] GeneralMobile_RewardHistoryDTO GeneralMobile_RewardHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RewardHistory RewardHistory = new RewardHistory
            {
                Id = GeneralMobile_RewardHistoryDTO.Id,
                AppUserId = CurrentContext.UserId,
                TurnCounter = GeneralMobile_RewardHistoryDTO.TurnCounter,
                Revenue = GeneralMobile_RewardHistoryDTO.Revenue,
                StoreId = GeneralMobile_RewardHistoryDTO.StoreId
            };
            RewardHistory.BaseLanguage = CurrentContext.Language;
            RewardHistory = await RewardHistoryService.Create(RewardHistory);
            GeneralMobile_RewardHistoryDTO = new GeneralMobile_RewardHistoryDTO(RewardHistory);
            if (RewardHistory.IsValidated)
            {
                return GeneralMobile_RewardHistoryDTO;
            }
            else
                return BadRequest(GeneralMobile_RewardHistoryDTO);
        }

        [Route(GeneralMobileRoute.LuckyDraw), HttpPost]
        public async Task<ActionResult<GeneralMobile_LuckyNumberDTO>> LuckyNumber([FromBody] GeneralMobile_RewardHistoryDTO GeneralMobile_RewardHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RewardHistory RewardHistory = await RewardHistoryService.Get(GeneralMobile_RewardHistoryDTO.Id);
            if(RewardHistory != null)
            {
                if(RewardHistory.TurnCounter <= RewardHistory.RewardHistoryContents.Count())
                {
                    return BadRequest("Đã hết số lần quay thưởng");
                }
                else
                {
                    LuckyNumber LuckyNumber = await LuckyNumberService.LuckyDraw(GeneralMobile_RewardHistoryDTO.Id);
                    if (LuckyNumber == null)
                        return BadRequest("Đã hết giải thưởng");
                    else
                    {
                        return Ok(new GeneralMobile_LuckyNumberDTO(LuckyNumber));
                    }
                }
            }
            else
            {
                return BadRequest();
            }
        }


        #region IndirectSalesOrder
        [Route(GeneralMobileRoute.CountCompletedIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountCompletedIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderFilterDTO GeneralMobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = GeneralMobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = GeneralMobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = GeneralMobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = GeneralMobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = GeneralMobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = GeneralMobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = GeneralMobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = GeneralMobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.Search = GeneralMobile_IndirectSalesOrderFilterDTO.Search;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await IndirectSalesOrderService.CountCompleted(IndirectSalesOrderFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListCompletedIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_IndirectSalesOrderDTO>>> ListCompletedIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderFilterDTO GeneralMobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = GeneralMobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = GeneralMobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = GeneralMobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = GeneralMobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = GeneralMobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = GeneralMobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = GeneralMobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = GeneralMobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.StoreCheckingId = GeneralMobile_IndirectSalesOrderFilterDTO.StoreCheckingId;
            IndirectSalesOrderFilter.Search = GeneralMobile_IndirectSalesOrderFilterDTO.Search;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.ListCompleted(IndirectSalesOrderFilter);
            List<GeneralMobile_IndirectSalesOrderDTO> GeneralMobile_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new GeneralMobile_IndirectSalesOrderDTO(c)).ToList();
            return GeneralMobile_IndirectSalesOrderDTOs;
        }


        [Route(GeneralMobileRoute.CountNewIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountNewIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderFilterDTO GeneralMobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = GeneralMobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = GeneralMobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = GeneralMobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = GeneralMobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = GeneralMobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = GeneralMobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = GeneralMobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = GeneralMobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.Search = GeneralMobile_IndirectSalesOrderFilterDTO.Search;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await IndirectSalesOrderService.CountNew(IndirectSalesOrderFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListNewIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_IndirectSalesOrderDTO>>> ListNewIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderFilterDTO GeneralMobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = GeneralMobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = GeneralMobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = GeneralMobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = GeneralMobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = GeneralMobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = GeneralMobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = GeneralMobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = GeneralMobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.StoreCheckingId = GeneralMobile_IndirectSalesOrderFilterDTO.StoreCheckingId;
            IndirectSalesOrderFilter.Search = GeneralMobile_IndirectSalesOrderFilterDTO.Search;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.ListNew(IndirectSalesOrderFilter);
            List<GeneralMobile_IndirectSalesOrderDTO> GeneralMobile_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new GeneralMobile_IndirectSalesOrderDTO(c)).ToList();
            return GeneralMobile_IndirectSalesOrderDTOs;
        }

        [Route(GeneralMobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderFilterDTO GeneralMobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = GeneralMobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = GeneralMobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = GeneralMobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = GeneralMobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = GeneralMobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = GeneralMobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = GeneralMobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = GeneralMobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.RequestStateId = GeneralMobile_IndirectSalesOrderFilterDTO.RequestStateId;

            IndirectSalesOrderFilter.Search = GeneralMobile_IndirectSalesOrderFilterDTO.Search;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_IndirectSalesOrderDTO>>> ListIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderFilterDTO GeneralMobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = GeneralMobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = GeneralMobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = GeneralMobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = GeneralMobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = GeneralMobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = GeneralMobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = GeneralMobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = GeneralMobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.StoreCheckingId = GeneralMobile_IndirectSalesOrderFilterDTO.StoreCheckingId;
            IndirectSalesOrderFilter.RequestStateId = GeneralMobile_IndirectSalesOrderFilterDTO.RequestStateId;

            IndirectSalesOrderFilter.Search = GeneralMobile_IndirectSalesOrderFilterDTO.Search;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<GeneralMobile_IndirectSalesOrderDTO> GeneralMobile_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new GeneralMobile_IndirectSalesOrderDTO(c)).ToList();
            return GeneralMobile_IndirectSalesOrderDTOs;
        }

        #endregion

        #region directSalesOrder
        [Route(GeneralMobileRoute.CountCompletedDirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountCompletedDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderFilterDTO GeneralMobile_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = GeneralMobile_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = GeneralMobile_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = GeneralMobile_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = GeneralMobile_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = GeneralMobile_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = GeneralMobile_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrderDate = GeneralMobile_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.BuyerStoreId = GeneralMobile_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await DirectSalesOrderService.CountCompleted(DirectSalesOrderFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListCompletedDirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_DirectSalesOrderDTO>>> ListCompletedDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderFilterDTO GeneralMobile_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = GeneralMobile_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = GeneralMobile_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = GeneralMobile_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = GeneralMobile_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = GeneralMobile_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = GeneralMobile_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrderDate = GeneralMobile_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.BuyerStoreId = GeneralMobile_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.ListCompleted(DirectSalesOrderFilter);
            List<GeneralMobile_DirectSalesOrderDTO> GeneralMobile_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(c => new GeneralMobile_DirectSalesOrderDTO(c)).ToList();
            return GeneralMobile_DirectSalesOrderDTOs;
        }


        [Route(GeneralMobileRoute.CountNewDirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountNewDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderFilterDTO GeneralMobile_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = GeneralMobile_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = GeneralMobile_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = GeneralMobile_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = GeneralMobile_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = GeneralMobile_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = GeneralMobile_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrderDate = GeneralMobile_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.BuyerStoreId = GeneralMobile_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await DirectSalesOrderService.CountNew(DirectSalesOrderFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListNewDirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_DirectSalesOrderDTO>>> ListNewDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderFilterDTO GeneralMobile_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = GeneralMobile_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = GeneralMobile_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = GeneralMobile_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = GeneralMobile_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = GeneralMobile_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = GeneralMobile_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrderDate = GeneralMobile_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.BuyerStoreId = GeneralMobile_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.ListNew(DirectSalesOrderFilter);
            List<GeneralMobile_DirectSalesOrderDTO> GeneralMobile_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(c => new GeneralMobile_DirectSalesOrderDTO(c)).ToList();
            return GeneralMobile_DirectSalesOrderDTOs;
        }

        [Route(GeneralMobileRoute.CountDirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderFilterDTO GeneralMobile_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = GeneralMobile_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = GeneralMobile_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = GeneralMobile_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = GeneralMobile_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = GeneralMobile_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = GeneralMobile_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrderDate = GeneralMobile_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.RequestStateId = GeneralMobile_DirectSalesOrderFilterDTO.RequestStateId;
            DirectSalesOrderFilter.BuyerStoreId = GeneralMobile_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await DirectSalesOrderService.Count(DirectSalesOrderFilter);
            return count;
        }

        [Route(GeneralMobileRoute.ListDirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<GeneralMobile_DirectSalesOrderDTO>>> ListDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderFilterDTO GeneralMobile_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = GeneralMobile_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = GeneralMobile_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = GeneralMobile_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = GeneralMobile_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = GeneralMobile_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = GeneralMobile_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.OrderDate = GeneralMobile_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.RequestStateId = GeneralMobile_DirectSalesOrderFilterDTO.RequestStateId;
            DirectSalesOrderFilter.BuyerStoreId = GeneralMobile_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<GeneralMobile_DirectSalesOrderDTO> GeneralMobile_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(c => new GeneralMobile_DirectSalesOrderDTO(c)).ToList();
            return GeneralMobile_DirectSalesOrderDTOs;
        }
        #endregion


        [Route(GeneralMobileRoute.GetIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<GeneralMobile_IndirectSalesOrderDTO>> GetIndirectSalesOrder([FromBody] GeneralMobile_IndirectSalesOrderDTO GeneralMobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = await IndirectSalesOrderService.Get(GeneralMobile_IndirectSalesOrderDTO.Id);
            return new GeneralMobile_IndirectSalesOrderDTO(IndirectSalesOrder);
        }

        [Route(GeneralMobileRoute.GetDirectSalesOrder), HttpPost]
        public async Task<ActionResult<GeneralMobile_DirectSalesOrderDTO>> GetDirectSalesOrder([FromBody] GeneralMobile_DirectSalesOrderDTO GeneralMobile_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrder DirectSalesOrder = await DirectSalesOrderService.Get(GeneralMobile_DirectSalesOrderDTO.Id);
            return new GeneralMobile_DirectSalesOrderDTO(DirectSalesOrder);
        }
    }
}

