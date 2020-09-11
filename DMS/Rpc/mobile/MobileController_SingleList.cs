using Common;
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
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public partial class MobileController
    {
        [Route(MobileRoute.SingleListAlbum), HttpPost]
        public async Task<List<Mobile_AlbumDTO>> SingleListAlbum([FromBody] Mobile_AlbumFilterDTO Mobile_AlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AlbumFilter AlbumFilter = new AlbumFilter();
            AlbumFilter.Skip = 0;
            AlbumFilter.Take = 20;
            AlbumFilter.OrderBy = AlbumOrder.Id;
            AlbumFilter.OrderType = OrderType.ASC;
            AlbumFilter.Selects = AlbumSelect.ALL;
            AlbumFilter.Id = Mobile_AlbumFilterDTO.Id;
            AlbumFilter.Name = Mobile_AlbumFilterDTO.Name;
            AlbumFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<Mobile_AlbumDTO> Mobile_AlbumDTOs = Albums
                .Select(x => new Mobile_AlbumDTO(x)).ToList();
            return Mobile_AlbumDTOs;
        }
        [Route(MobileRoute.SingleListAppUser), HttpPost]
        public async Task<List<Mobile_AppUserDTO>> SingleListAppUser([FromBody] Mobile_AppUserFilterDTO Mobile_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Mobile_AppUserFilterDTO.Id;
            AppUserFilter.Username = Mobile_AppUserFilterDTO.Username;
            AppUserFilter.Password = Mobile_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Mobile_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = Mobile_AppUserFilterDTO.Address;
            AppUserFilter.Email = Mobile_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Mobile_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = Mobile_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = Mobile_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = Mobile_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = Mobile_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = Mobile_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = Mobile_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = Mobile_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Mobile_AppUserDTO> Mobile_AppUserDTOs = AppUsers
                .Select(x => new Mobile_AppUserDTO(x)).ToList();
            return Mobile_AppUserDTOs;
        }

        [Route(MobileRoute.SingleListEroute), HttpPost]
        public async Task<List<Mobile_ERouteDTO>> SingleListEroute(Mobile_ERouteFilterDTO Mobile_ERouteFilterDTO)
        {
            ERouteFilter ERouteFilter = new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                AppUserId = new IdFilter { Equal = CurrentContext.UserId },
                Selects = ERouteSelect.Id | ERouteSelect.Name | ERouteSelect.Code
            };

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            List<Mobile_ERouteDTO> Mobile_ERouteDTOs = ERoutes
                .Select(x => new Mobile_ERouteDTO(x)).ToList();
            return Mobile_ERouteDTOs;
        }

        [Route(MobileRoute.SingleListStore), HttpPost]
        public async Task<List<Mobile_StoreDTO>> SingleListStore([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            StoreFilter.OrganizationId = new IdFilter { Equal = AppUser.OrganizationId };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Mobile_StoreDTO> Mobile_StoreDTOs = Stores
                .Select(x => new Mobile_StoreDTO(x)).ToList();
            return Mobile_StoreDTOs;
        }

        [Route(MobileRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<Mobile_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] Mobile_StoreGroupingFilterDTO Mobile_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = Mobile_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Mobile_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = Mobile_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = Mobile_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<Mobile_StoreGroupingDTO> Mobile_StoreGroupingDTOs = StoreGroupings
                .Select(x => new Mobile_StoreGroupingDTO(x)).ToList();
            return Mobile_StoreGroupingDTOs;
        }
        [Route(MobileRoute.SingleListStoreType), HttpPost]
        public async Task<List<Mobile_StoreTypeDTO>> SingleListStoreType([FromBody] Mobile_StoreTypeFilterDTO Mobile_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = Mobile_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Mobile_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Mobile_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<Mobile_StoreTypeDTO> Mobile_StoreTypeDTOs = StoreTypes
                .Select(x => new Mobile_StoreTypeDTO(x)).ToList();
            return Mobile_StoreTypeDTOs;
        }

        [Route(MobileRoute.SingleListTaxType), HttpPost]
        public async Task<List<Mobile_TaxTypeDTO>> SingleListTaxType([FromBody] Mobile_TaxTypeFilterDTO Mobile_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = Mobile_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = Mobile_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = Mobile_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<Mobile_TaxTypeDTO> Mobile_TaxTypeDTOs = TaxTypes
                .Select(x => new Mobile_TaxTypeDTO(x)).ToList();
            return Mobile_TaxTypeDTOs;
        }

        [Route(MobileRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<Mobile_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] Mobile_UnitOfMeasureFilterDTO Mobile_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Product = await ProductService.Get(Mobile_UnitOfMeasureFilterDTO.ProductId.Equal ?? 0);

            List<Mobile_UnitOfMeasureDTO> Mobile_UnitOfMeasureDTOs = new List<Mobile_UnitOfMeasureDTO>();
            if (Product.UnitOfMeasureGrouping != null && Product.UnitOfMeasureGrouping.StatusId == Enums.StatusEnum.ACTIVE.Id)
            {
                Mobile_UnitOfMeasureDTOs = Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents.Select(x => new Mobile_UnitOfMeasureDTO(x)).ToList();
            }
            Mobile_UnitOfMeasureDTOs.Add(new Mobile_UnitOfMeasureDTO
            {
                Id = Product.UnitOfMeasure.Id,
                Code = Product.UnitOfMeasure.Code,
                Name = Product.UnitOfMeasure.Name,
                Description = Product.UnitOfMeasure.Description,
                StatusId = Product.UnitOfMeasure.StatusId,
                Factor = 1
            });
            return Mobile_UnitOfMeasureDTOs;
        }

        [Route(MobileRoute.SingleListProblemType), HttpPost]
        public async Task<List<Mobile_ProblemTypeDTO>> SingleListProblemType()
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
            List<Mobile_ProblemTypeDTO> Mobile_ProblemTypeDTOs = ProblemTypes

                .Select(x => new Mobile_ProblemTypeDTO(x)).ToList();
            return Mobile_ProblemTypeDTOs;
        }

        [Route(MobileRoute.SingleListBrand), HttpPost]
        public async Task<List<Mobile_BrandDTO>> SingleListBrand([FromBody] Mobile_BrandFilterDTO Mobile_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Id = Mobile_BrandFilterDTO.Id;
            BrandFilter.Code = Mobile_BrandFilterDTO.Code;
            BrandFilter.Name = Mobile_BrandFilterDTO.Name;
            BrandFilter.Description = Mobile_BrandFilterDTO.Description;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            BrandFilter.UpdateTime = Mobile_BrandFilterDTO.UpdateTime;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Mobile_BrandDTO> Mobile_BrandDTOs = Brands
                .Select(x => new Mobile_BrandDTO(x)).ToList();
            return Mobile_BrandDTOs;
        }

        [Route(MobileRoute.SingleListColor), HttpPost]
        public async Task<List<Mobile_ColorDTO>> SingleListColor([FromBody] Mobile_ColorFilterDTO Mobile_ColorFilterDTO)
        {
            ColorFilter ColorFilter = new ColorFilter();
            ColorFilter.Skip = 0;
            ColorFilter.Take = 20;
            ColorFilter.OrderBy = ColorOrder.Id;
            ColorFilter.OrderType = OrderType.ASC;
            ColorFilter.Selects = ColorSelect.ALL;

            List<Color> Colores = await ColorService.List(ColorFilter);
            List<Mobile_ColorDTO> Mobile_ColorDTOs = Colores
                .Select(x => new Mobile_ColorDTO(x)).ToList();
            return Mobile_ColorDTOs;
        }

        [Route(MobileRoute.SingleListSupplier), HttpPost]
        public async Task<List<Mobile_SupplierDTO>> SingleListSupplier([FromBody] Mobile_SupplierFilterDTO Mobile_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = Mobile_SupplierFilterDTO.Id;
            SupplierFilter.Code = Mobile_SupplierFilterDTO.Code;
            SupplierFilter.Name = Mobile_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Mobile_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Mobile_SupplierDTO> Mobile_SupplierDTOs = Suppliers
                .Select(x => new Mobile_SupplierDTO(x)).ToList();
            return Mobile_SupplierDTOs;
        }

        [Route(MobileRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<Mobile_ProductGroupingDTO>> SingleListProductGrouping([FromBody] Mobile_ProductGroupingFilterDTO Mobile_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Mobile_ProductGroupingDTO> Mobile_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Mobile_ProductGroupingDTO(x)).ToList();
            return Mobile_ProductGroupingDTOs;
        }

        [Route(MobileRoute.SingleListProvince), HttpPost]
        public async Task<List<Mobile_ProvinceDTO>> SingleListProvince([FromBody] Mobile_ProvinceFilterDTO Mobile_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = Mobile_ProvinceFilterDTO.Skip;
            ProvinceFilter.Take = Mobile_ProvinceFilterDTO.Take;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Mobile_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Mobile_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Mobile_ProvinceDTO> Mobile_ProvinceDTOs = Provinces
                .Select(x => new Mobile_ProvinceDTO(x)).ToList();
            return Mobile_ProvinceDTOs;
        }
        [Route(MobileRoute.SingleListDistrict), HttpPost]
        public async Task<List<Mobile_DistrictDTO>> SingleListDistrict([FromBody] Mobile_DistrictFilterDTO Mobile_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = Mobile_DistrictFilterDTO.Skip;
            DistrictFilter.Take = Mobile_DistrictFilterDTO.Take;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Mobile_DistrictFilterDTO.Id;
            DistrictFilter.Name = Mobile_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Mobile_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Mobile_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Mobile_DistrictDTO> Mobile_DistrictDTOs = Districts
                .Select(x => new Mobile_DistrictDTO(x)).ToList();
            return Mobile_DistrictDTOs;
        }
        [Route(MobileRoute.SingleListWard), HttpPost]
        public async Task<List<Mobile_WardDTO>> SingleListWard([FromBody] Mobile_WardFilterDTO Mobile_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = Mobile_WardFilterDTO.Skip;
            WardFilter.Take = Mobile_WardFilterDTO.Take;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Mobile_WardFilterDTO.Id;
            WardFilter.Name = Mobile_WardFilterDTO.Name;
            WardFilter.DistrictId = Mobile_WardFilterDTO.DistrictId;
            WardFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Mobile_WardDTO> Mobile_WardDTOs = Wards
                .Select(x => new Mobile_WardDTO(x)).ToList();
            return Mobile_WardDTOs;
        }

        [Route(MobileRoute.SingleListStoreCheckingStatus), HttpPost]
        public async Task<List<GenericEnum>> SingleListStoreCheckingStatus()
        {
            return StoreCheckingStatusEnum.StoreCheckingStatusEnumList;
        }

        [Route(MobileRoute.CountBanner), HttpPost]
        public async Task<ActionResult<int>> CountBanner([FromBody] Mobile_BannerFilterDTO Mobile_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = Mobile_BannerFilterDTO.Skip;
            BannerFilter.Take = Mobile_BannerFilterDTO.Take;
            BannerFilter.OrderBy = Mobile_BannerFilterDTO.OrderBy;
            BannerFilter.OrderType = Mobile_BannerFilterDTO.OrderType;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            int count = await BannerService.Count(BannerFilter);
            return count;
        }

        [Route(MobileRoute.ListBanner), HttpPost]
        public async Task<ActionResult<List<Mobile_BannerDTO>>> ListBanner([FromBody] Mobile_BannerFilterDTO Mobile_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = Mobile_BannerFilterDTO.Skip;
            BannerFilter.Take = Mobile_BannerFilterDTO.Take;
            BannerFilter.OrderBy = BannerOrder.Priority;
            BannerFilter.OrderType = OrderType.ASC;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Banner> Banners = await BannerService.List(BannerFilter);
            List<Mobile_BannerDTO> Mobile_BannerDTOs = Banners
                .Select(c => new Mobile_BannerDTO(c)).ToList();
            return Mobile_BannerDTOs;
        }

        [Route(MobileRoute.GetBanner), HttpPost]
        public async Task<ActionResult<Mobile_BannerDTO>> GetBanner([FromBody] Mobile_BannerDTO Mobile_BannerDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Banner Banner = await BannerService.Get(Mobile_BannerDTO.Id);
            return new Mobile_BannerDTO(Banner);
        }

        [Route(MobileRoute.GetStore), HttpPost]
        public async Task<ActionResult<Mobile_StoreDTO>> Get([FromBody] Mobile_StoreDTO Mobile_StoreDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Store Store = await StoreService.Get(Mobile_StoreDTO.Id);
            Mobile_StoreDTO = new Mobile_StoreDTO(Store);
            DateTime Start = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date;
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            List<Album> Albums = await AlbumService.List(new AlbumFilter
            {
                StoreId = new IdFilter { Equal = Mobile_StoreDTO.Id },
                Selects = AlbumSelect.ALL,
                ShootingAt = new DateFilter { GreaterEqual = Start, LessEqual = End},
                Skip = 0,
                Take = int.MaxValue,
            });
            Mobile_StoreDTO.AlbumImageMappings = Albums
                .SelectMany(x => x.AlbumImageMappings
                .Where(x => x.StoreId == Mobile_StoreDTO.Id)
                .Where(x => x.ShootingAt.AddHours(CurrentContext.TimeZone).Date == StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date)
                .Select(m => new Mobile_AlbumImageMappingDTO
                {
                    AlbumId = m.AlbumId,
                    ImageId = m.ImageId,
                    StoreId = m.StoreId,
                    Distance = m.Distance,
                    Album = m.Album == null ? null : new Mobile_AlbumDTO
                    {
                        Id = m.Album.Id,
                        Name = m.Album.Name,
                    },
                    Image = m.Image == null ? null : new Mobile_ImageDTO
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
                StoreId = new IdFilter { Equal = Mobile_StoreDTO.Id }
            });
            var StoreCheckingIds = StoreCheckings.Select(x => x.Id).ToList();
            var query = from scim in DataContext.StoreCheckingImageMapping
                        join a in DataContext.Album on scim.AlbumId equals a.Id
                        join i in DataContext.Image on scim.ImageId equals i.Id
                        where StoreCheckingIds.Contains(scim.StoreCheckingId)
                        select new Mobile_AlbumImageMappingDTO
                        {
                            AlbumId = scim.AlbumId,
                            ImageId = scim.ImageId,
                            StoreId = scim.StoreId,
                            Album = new Mobile_AlbumDTO
                            {
                                Id = a.Id,
                                Name = a.Name,
                            },
                            Image = new Mobile_ImageDTO
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Url = i.Url,
                            },
                        };
            var StoreCheckingImageMappings = await query.ToListAsync();
            Mobile_StoreDTO.AlbumImageMappings.AddRange(StoreCheckingImageMappings);
            return Mobile_StoreDTO;
        }

        [Route(MobileRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStore(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
        }

        [Route(MobileRoute.ListStore), HttpPost]
        public async Task<List<Mobile_StoreDTO>> ListStore([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStore(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
            List<Mobile_StoreDTO> Mobile_StoreDTOs = Stores
                .Select(x => new Mobile_StoreDTO(x)).ToList();
            return Mobile_StoreDTOs;
        }

        [Route(MobileRoute.CountBuyerStore), HttpPost]
        public async Task<long> CountBuyerStore([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            var StoreIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
            if (StoreIds.Any())
            {
                StoreFilter.Id = new IdFilter { In = StoreIds };
            }

            return await StoreCheckingService.CountStore(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
        }

        [Route(MobileRoute.ListBuyerStore), HttpPost]
        public async Task<List<Mobile_StoreDTO>> ListBuyerStore([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            var StoreIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
            if (StoreIds.Any())
            {
                StoreFilter.Id = new IdFilter { In = StoreIds };
            }
            List<Store> Stores = await StoreCheckingService.ListStore(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
            List<Mobile_StoreDTO> Mobile_StoreDTOs = Stores
                .Select(x => new Mobile_StoreDTO(x)).ToList();
            return Mobile_StoreDTOs;
        }

        [Route(MobileRoute.CountStorePlanned), HttpPost]
        public async Task<long> CountStorePlanned([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStorePlanned(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
        }

        [Route(MobileRoute.ListStorePlanned), HttpPost]
        public async Task<List<Mobile_StoreDTO>> ListStorePlanned([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStorePlanned(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
            List<Mobile_StoreDTO> Mobile_StoreDTOs = Stores
                .Select(x => new Mobile_StoreDTO(x)).ToList();
            return Mobile_StoreDTOs;
        }

        [Route(MobileRoute.CountStoreUnPlanned), HttpPost]
        public async Task<long> CountStoreUnPlanned([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStoreUnPlanned(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
        }

        [Route(MobileRoute.ListStoreUnPlanned), HttpPost]
        public async Task<List<Mobile_StoreDTO>> ListStoreUnPlanned([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStoreUnPlanned(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
            List<Mobile_StoreDTO> Mobile_StoreDTOs = Stores
                .Select(x => new Mobile_StoreDTO(x)).ToList();
            return Mobile_StoreDTOs;
        }

        [Route(MobileRoute.CountStoreInScope), HttpPost]
        public async Task<long> CountStoreInScope([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await StoreCheckingService.CountStoreInScope(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
        }

        [Route(MobileRoute.ListStoreInScope), HttpPost]
        public async Task<List<Mobile_StoreDTO>> ListStoreInScope([FromBody] Mobile_StoreFilterDTO Mobile_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Search = Mobile_StoreFilterDTO.Search;
            StoreFilter.Skip = Mobile_StoreFilterDTO.Skip;
            StoreFilter.Take = Mobile_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.DESC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Mobile_StoreFilterDTO.Id;
            StoreFilter.Code = Mobile_StoreFilterDTO.Code;
            StoreFilter.Name = Mobile_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Mobile_StoreFilterDTO.ParentStoreId;
            StoreFilter.StoreTypeId = Mobile_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Mobile_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StoreCheckingStatusId = Mobile_StoreFilterDTO.StoreCheckingStatusId;
            StoreFilter.Telephone = Mobile_StoreFilterDTO.Telephone;
            StoreFilter.ResellerId = Mobile_StoreFilterDTO.ResellerId;
            StoreFilter.ProvinceId = Mobile_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Mobile_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Mobile_StoreFilterDTO.WardId;
            StoreFilter.Address = Mobile_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Mobile_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Mobile_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Mobile_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = Mobile_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = Mobile_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = Mobile_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Mobile_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Mobile_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreCheckingService.ListStoreInScope(StoreFilter, Mobile_StoreFilterDTO.ERouteId);
            List<Mobile_StoreDTO> Mobile_StoreDTOs = Stores
                .Select(x => new Mobile_StoreDTO(x)).ToList();
            return Mobile_StoreDTOs;
        }

        [Route(MobileRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] Mobile_ItemFilterDTO Mobile_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Search = Mobile_ItemFilterDTO.Search;
            ItemFilter.Id = Mobile_ItemFilterDTO.Id;
            ItemFilter.Code = Mobile_ItemFilterDTO.Code;
            ItemFilter.Name = Mobile_ItemFilterDTO.Name;
            ItemFilter.OtherName = Mobile_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = Mobile_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = Mobile_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = Mobile_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = Mobile_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = Mobile_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Mobile_ItemFilterDTO.ScanCode;
            ItemFilter.IsNew = Mobile_ItemFilterDTO.IsNew;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = Mobile_ItemFilterDTO.SupplierId;
            return await ItemService.Count(ItemFilter);
        }

        [Route(MobileRoute.ListItem), HttpPost]
        public async Task<List<Mobile_ItemDTO>> ListItem([FromBody] Mobile_ItemFilterDTO Mobile_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Search = Mobile_ItemFilterDTO.Search;
            ItemFilter.Skip = Mobile_ItemFilterDTO.Skip;
            ItemFilter.Take = Mobile_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.DESC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Mobile_ItemFilterDTO.Id;
            ItemFilter.Code = Mobile_ItemFilterDTO.Code;
            ItemFilter.Name = Mobile_ItemFilterDTO.Name;
            ItemFilter.OtherName = Mobile_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = Mobile_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = Mobile_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = Mobile_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = Mobile_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = Mobile_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Mobile_ItemFilterDTO.ScanCode;
            ItemFilter.IsNew = Mobile_ItemFilterDTO.IsNew;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = Mobile_ItemFilterDTO.SupplierId;

            if (Mobile_ItemFilterDTO.StoreId == null)
                Mobile_ItemFilterDTO.StoreId = new IdFilter();
            List<Item> Items = await IndirectSalesOrderService.ListItem(ItemFilter, Mobile_ItemFilterDTO.StoreId.Equal);
            List<Mobile_ItemDTO> Mobile_ItemDTOs = Items
                .Select(x => new Mobile_ItemDTO(x)).ToList();
            return Mobile_ItemDTOs;
        }

        [Route(MobileRoute.GetItem), HttpPost]
        public async Task<Mobile_ItemDTO> GetItem([FromBody] Mobile_ItemDTO Mobile_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Item Item = await ItemService.Get(Mobile_ItemDTO.Id);
            if (Item == null)
                return null;
            Mobile_ItemDTO = new Mobile_ItemDTO(Item);
            return Mobile_ItemDTO;
        }

        [Route(MobileRoute.CountProblem), HttpPost]
        public async Task<long> CountProblem([FromBody] Mobile_ProblemFilterDTO Mobile_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Id = Mobile_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = Mobile_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            ProblemFilter.StoreId = Mobile_ProblemFilterDTO.StoreId;
            ProblemFilter.NoteAt = Mobile_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = Mobile_ProblemFilterDTO.CompletedAt;
            ProblemFilter.ProblemTypeId = Mobile_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.ProblemStatusId = Mobile_ProblemFilterDTO.ProblemStatusId;
            return await ProblemService.Count(ProblemFilter);
        }

        [Route(MobileRoute.ListProblem), HttpPost]
        public async Task<List<Mobile_ProblemDTO>> ListProblem([FromBody] Mobile_ProblemFilterDTO Mobile_ProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            ProblemFilter ProblemFilter = new ProblemFilter();
            ProblemFilter.Skip = Mobile_ProblemFilterDTO.Skip;
            ProblemFilter.Take = Mobile_ProblemFilterDTO.Take;
            ProblemFilter.OrderBy = ProblemOrder.NoteAt;
            ProblemFilter.OrderType = OrderType.DESC;
            ProblemFilter.Selects = ProblemSelect.ALL;
            ProblemFilter.Id = Mobile_ProblemFilterDTO.Id;
            ProblemFilter.StoreCheckingId = Mobile_ProblemFilterDTO.StoreCheckingId;
            ProblemFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            ProblemFilter.StoreId = Mobile_ProblemFilterDTO.StoreId;
            ProblemFilter.NoteAt = Mobile_ProblemFilterDTO.NoteAt;
            ProblemFilter.CompletedAt = Mobile_ProblemFilterDTO.CompletedAt;
            ProblemFilter.ProblemTypeId = Mobile_ProblemFilterDTO.ProblemTypeId;
            ProblemFilter.ProblemStatusId = Mobile_ProblemFilterDTO.ProblemStatusId;

            List<Problem> Problems = await ProblemService.List(ProblemFilter);
            List<Mobile_ProblemDTO> Mobile_ProblemDTOs = Problems
                .Select(x => new Mobile_ProblemDTO(x)).ToList();
            return Mobile_ProblemDTOs;
        }

        [Route(MobileRoute.GetProblem), HttpPost]
        public async Task<ActionResult<Mobile_ProblemDTO>> Get([FromBody] Mobile_ProblemDTO Mobile_ProblemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Problem Problem = await ProblemService.Get(Mobile_ProblemDTO.Id);
            return new Mobile_ProblemDTO(Problem);
        }

        [Route(MobileRoute.CountSurvey), HttpPost]
        public async Task<long> CountSurvey([FromBody] Mobile_SurveyFilterDTO Mobile_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Now = StaticParams.DateTimeNow;
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = Mobile_SurveyFilterDTO.Skip;
            SurveyFilter.Take = Mobile_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = Mobile_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = Mobile_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = Mobile_SurveyFilterDTO.Id;
            SurveyFilter.Title = Mobile_SurveyFilterDTO.Title;
            SurveyFilter.Description = Mobile_SurveyFilterDTO.Description;
            SurveyFilter.StartAt = new DateFilter { LessEqual = Now };
            SurveyFilter.EndAt = new DateFilter { GreaterEqual = Now };
            SurveyFilter.CreatorId = Mobile_SurveyFilterDTO.CreatorId;
            SurveyFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            return await SurveyService.Count(SurveyFilter);
        }

        [Route(MobileRoute.ListSurvey), HttpPost]
        public async Task<List<Mobile_SurveyDTO>> ListSurvey([FromBody] Mobile_SurveyFilterDTO Mobile_SurveyFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Now = StaticParams.DateTimeNow;
            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);
            
            SurveyFilter SurveyFilter = new SurveyFilter();
            SurveyFilter.Selects = SurveySelect.ALL;
            SurveyFilter.Skip = Mobile_SurveyFilterDTO.Skip;
            SurveyFilter.Take = Mobile_SurveyFilterDTO.Take;
            SurveyFilter.OrderBy = Mobile_SurveyFilterDTO.OrderBy;
            SurveyFilter.OrderType = Mobile_SurveyFilterDTO.OrderType;

            SurveyFilter.Id = Mobile_SurveyFilterDTO.Id;
            SurveyFilter.Title = Mobile_SurveyFilterDTO.Title;
            SurveyFilter.Description = Mobile_SurveyFilterDTO.Description;
            SurveyFilter.StartAt = new DateFilter { LessEqual = Now };
            SurveyFilter.EndAt = new DateFilter { GreaterEqual = Now };
            SurveyFilter.CreatorId = Mobile_SurveyFilterDTO.CreatorId;
            SurveyFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            
            List<Survey> Surveys = await SurveyService.List(SurveyFilter);
            List<Mobile_SurveyDTO> Mobile_SurveyDTOs = Surveys
                .Select(x => new Mobile_SurveyDTO(x)).ToList();
            return Mobile_SurveyDTOs;
        }

        [Route(MobileRoute.CountStoreScouting), HttpPost]
        public async Task<long> CountStoreScouting([FromBody] Mobile_StoreScoutingFilterDTO Mobile_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = Mobile_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = Mobile_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = Mobile_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = Mobile_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = Mobile_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = Mobile_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = Mobile_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.CreatedAt = Mobile_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            StoreScoutingFilter.DistrictId = Mobile_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId.Value };
            StoreScoutingFilter.ProvinceId = Mobile_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.WardId = Mobile_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.StoreScoutingStatusId = Mobile_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.OwnerPhone = Mobile_StoreScoutingFilterDTO.OwnerPhone;

            return await StoreScoutingService.Count(StoreScoutingFilter);
        }

        [Route(MobileRoute.ListStoreScouting), HttpPost]
        public async Task<List<Mobile_StoreScoutingDTO>> ListStoreScouting([FromBody] Mobile_StoreScoutingFilterDTO Mobile_StoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser appUser = await AppUserService.Get(CurrentContext.UserId);

            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter();
            StoreScoutingFilter.Selects = StoreScoutingSelect.ALL;
            StoreScoutingFilter.Skip = Mobile_StoreScoutingFilterDTO.Skip;
            StoreScoutingFilter.Take = Mobile_StoreScoutingFilterDTO.Take;
            StoreScoutingFilter.OrderBy = Mobile_StoreScoutingFilterDTO.OrderBy;
            StoreScoutingFilter.OrderType = Mobile_StoreScoutingFilterDTO.OrderType;

            StoreScoutingFilter.Id = Mobile_StoreScoutingFilterDTO.Id;
            StoreScoutingFilter.Code = Mobile_StoreScoutingFilterDTO.Code;
            StoreScoutingFilter.Name = Mobile_StoreScoutingFilterDTO.Name;
            StoreScoutingFilter.CreatedAt = Mobile_StoreScoutingFilterDTO.CreatedAt;
            StoreScoutingFilter.AppUserId = new IdFilter { Equal = appUser.Id };
            StoreScoutingFilter.DistrictId = Mobile_StoreScoutingFilterDTO.DistrictId;
            StoreScoutingFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId.Value };
            StoreScoutingFilter.ProvinceId = Mobile_StoreScoutingFilterDTO.ProvinceId;
            StoreScoutingFilter.WardId = Mobile_StoreScoutingFilterDTO.WardId;
            StoreScoutingFilter.StoreScoutingStatusId = Mobile_StoreScoutingFilterDTO.StoreScoutingStatusId;
            StoreScoutingFilter.OwnerPhone = Mobile_StoreScoutingFilterDTO.OwnerPhone;

            List<StoreScouting> StoreScoutings = await StoreScoutingService.List(StoreScoutingFilter);
            List<Mobile_StoreScoutingDTO> Mobile_StoreScoutingDTOs = StoreScoutings
                .Select(x => new Mobile_StoreScoutingDTO(x)).ToList();
            return Mobile_StoreScoutingDTOs;
        }

        [Route(MobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<int>> CountIndirectSalesOrder([FromBody] Mobile_IndirectSalesOrderFilterDTO Mobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = Mobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = Mobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = Mobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = Mobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = Mobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = Mobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = Mobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = Mobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
            return count;
        }

        [Route(MobileRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<List<Mobile_IndirectSalesOrderDTO>>> ListIndirectSalesOrder([FromBody] Mobile_IndirectSalesOrderFilterDTO Mobile_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = Mobile_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = Mobile_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = Mobile_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = Mobile_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = Mobile_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = Mobile_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.OrderDate = Mobile_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.BuyerStoreId = Mobile_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.StoreCheckingId = Mobile_IndirectSalesOrderFilterDTO.StoreCheckingId;
            IndirectSalesOrderFilter.AppUserId = new IdFilter { Equal = CurrentContext.UserId };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<Mobile_IndirectSalesOrderDTO> Mobile_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new Mobile_IndirectSalesOrderDTO(c)).ToList();
            return Mobile_IndirectSalesOrderDTOs;
        }


        [Route(MobileRoute.GetIndirectSalesOrder), HttpPost]
        public async Task<ActionResult<Mobile_IndirectSalesOrderDTO>> GetIndirectSalesOrder([FromBody] Mobile_IndirectSalesOrderDTO Mobile_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = await IndirectSalesOrderService.Get(Mobile_IndirectSalesOrderDTO.Id);
            return new Mobile_IndirectSalesOrderDTO(IndirectSalesOrder);
        }


    }
}

