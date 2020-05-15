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
using DMS.Services.MStoreChecking;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MImage;
using DMS.Services.MStore;

namespace DMS.Rpc.store_checking
{
    public class StoreCheckingRoute : Root
    {
        public const string Master = Module + "/store-checking/store-checking-master";
        public const string Detail = Module + "/store-checking/store-checking-detail";
        private const string Default = Rpc + Module + "/store-checking";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        
        public const string FilterListAlbum = Default + "/filter-list-album";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListStore = Default + "/filter-list-store";

        public const string SingleListAlbum = Default + "/single-list-album";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListStore = Default + "/single-list-store";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(StoreCheckingFilter.Longtitude), FieldType.DECIMAL },
            { nameof(StoreCheckingFilter.Latitude), FieldType.DECIMAL },
            { nameof(StoreCheckingFilter.CheckInAt), FieldType.DATE },
            { nameof(StoreCheckingFilter.CheckOutAt), FieldType.DATE },
            { nameof(StoreCheckingFilter.CountIndirectSalesOrder), FieldType.LONG },
            { nameof(StoreCheckingFilter.CountImage), FieldType.LONG },
        };
    }

    public class StoreCheckingController : RpcController
    {
        private IAlbumService AlbumService;
        private IAppUserService AppUserService;
        private IImageService ImageService;
        private IStoreService StoreService;
        private IStoreCheckingService StoreCheckingService;
        private ICurrentContext CurrentContext;
        public StoreCheckingController(
            IAlbumService AlbumService,
            IAppUserService AppUserService,
            IImageService ImageService,
            IStoreService StoreService,
            IStoreCheckingService StoreCheckingService,
            ICurrentContext CurrentContext
        )
        {
            this.AlbumService = AlbumService;
            this.AppUserService = AppUserService;
            this.ImageService = ImageService;
            this.StoreService = StoreService;
            this.StoreCheckingService = StoreCheckingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreCheckingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreChecking_StoreCheckingFilterDTO StoreChecking_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(StoreChecking_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            int count = await StoreCheckingService.Count(StoreCheckingFilter);
            return count;
        }

        [Route(StoreCheckingRoute.List), HttpPost]
        public async Task<ActionResult<List<StoreChecking_StoreCheckingDTO>>> List([FromBody] StoreChecking_StoreCheckingFilterDTO StoreChecking_StoreCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingFilter StoreCheckingFilter = ConvertFilterDTOToFilterEntity(StoreChecking_StoreCheckingFilterDTO);
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<StoreChecking_StoreCheckingDTO> StoreChecking_StoreCheckingDTOs = StoreCheckings
                .Select(c => new StoreChecking_StoreCheckingDTO(c)).ToList();
            return StoreChecking_StoreCheckingDTOs;
        }

        [Route(StoreCheckingRoute.Get), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Get([FromBody]StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = await StoreCheckingService.Get(StoreChecking_StoreCheckingDTO.Id);
            return new StoreChecking_StoreCheckingDTO(StoreChecking);
        }

        [Route(StoreCheckingRoute.Create), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Create([FromBody] StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(StoreChecking_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Create(StoreChecking);
            StoreChecking_StoreCheckingDTO = new StoreChecking_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return StoreChecking_StoreCheckingDTO;
            else
                return BadRequest(StoreChecking_StoreCheckingDTO);
        }

        [Route(StoreCheckingRoute.Update), HttpPost]
        public async Task<ActionResult<StoreChecking_StoreCheckingDTO>> Update([FromBody] StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(StoreChecking_StoreCheckingDTO.Id))
                return Forbid();

            StoreChecking StoreChecking = ConvertDTOToEntity(StoreChecking_StoreCheckingDTO);
            StoreChecking = await StoreCheckingService.Update(StoreChecking);
            StoreChecking_StoreCheckingDTO = new StoreChecking_StoreCheckingDTO(StoreChecking);
            if (StoreChecking.IsValidated)
                return StoreChecking_StoreCheckingDTO;
            else
                return BadRequest(StoreChecking_StoreCheckingDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter = StoreCheckingService.ToFilter(StoreCheckingFilter);
            if (Id == 0)
            {

            }
            else
            {
                StoreCheckingFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreCheckingService.Count(StoreCheckingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private StoreChecking ConvertDTOToEntity(StoreChecking_StoreCheckingDTO StoreChecking_StoreCheckingDTO)
        {
            StoreChecking StoreChecking = new StoreChecking();
            StoreChecking.Id = StoreChecking_StoreCheckingDTO.Id;
            StoreChecking.StoreId = StoreChecking_StoreCheckingDTO.StoreId;
            StoreChecking.AppUserId = StoreChecking_StoreCheckingDTO.AppUserId;
            StoreChecking.Longtitude = StoreChecking_StoreCheckingDTO.Longtitude;
            StoreChecking.Latitude = StoreChecking_StoreCheckingDTO.Latitude;
            StoreChecking.CheckInAt = StoreChecking_StoreCheckingDTO.CheckInAt;
            StoreChecking.CheckOutAt = StoreChecking_StoreCheckingDTO.CheckOutAt;
            StoreChecking.CountIndirectSalesOrder = StoreChecking_StoreCheckingDTO.CountIndirectSalesOrder;
            StoreChecking.CountImage = StoreChecking_StoreCheckingDTO.CountImage;
            StoreChecking.ImageStoreCheckingMappings = StoreChecking_StoreCheckingDTO.ImageStoreCheckingMappings?
                .Select(x => new ImageStoreCheckingMapping
                {
                    ImageId = x.ImageId,
                    AlbumId = x.AlbumId,
                    StoreId = x.StoreId,
                    AppUserId = x.AppUserId,
                    ShootingAt = x.ShootingAt,
                    Album = x.Album == null ? null : new Album
                    {
                        Id = x.Album.Id,
                        Name = x.Album.Name,
                    },
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        Password = x.AppUser.Password,
                        DisplayName = x.AppUser.DisplayName,
                        Address = x.AppUser.Address,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                        Position = x.AppUser.Position,
                        Department = x.AppUser.Department,
                        OrganizationId = x.AppUser.OrganizationId,
                        SexId = x.AppUser.SexId,
                        StatusId = x.AppUser.StatusId,
                        Avatar = x.AppUser.Avatar,
                        Birthday = x.AppUser.Birthday,
                        ProvinceId = x.AppUser.ProvinceId,
                    },
                    Image = x.Image == null ? null : new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                    Store = x.Store == null ? null : new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        ParentStoreId = x.Store.ParentStoreId,
                        OrganizationId = x.Store.OrganizationId,
                        StoreTypeId = x.Store.StoreTypeId,
                        StoreGroupingId = x.Store.StoreGroupingId,
                        ResellerId = x.Store.ResellerId,
                        Telephone = x.Store.Telephone,
                        ProvinceId = x.Store.ProvinceId,
                        DistrictId = x.Store.DistrictId,
                        WardId = x.Store.WardId,
                        Address = x.Store.Address,
                        DeliveryAddress = x.Store.DeliveryAddress,
                        Latitude = x.Store.Latitude,
                        Longitude = x.Store.Longitude,
                        DeliveryLatitude = x.Store.DeliveryLatitude,
                        DeliveryLongitude = x.Store.DeliveryLongitude,
                        OwnerName = x.Store.OwnerName,
                        OwnerPhone = x.Store.OwnerPhone,
                        OwnerEmail = x.Store.OwnerEmail,
                        StatusId = x.Store.StatusId,
                    },
                }).ToList();
            StoreChecking.BaseLanguage = CurrentContext.Language;
            return StoreChecking;
        }

        private StoreCheckingFilter ConvertFilterDTOToFilterEntity(StoreChecking_StoreCheckingFilterDTO StoreChecking_StoreCheckingFilterDTO)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter();
            StoreCheckingFilter.Selects = StoreCheckingSelect.ALL;
            StoreCheckingFilter.Skip = StoreChecking_StoreCheckingFilterDTO.Skip;
            StoreCheckingFilter.Take = StoreChecking_StoreCheckingFilterDTO.Take;
            StoreCheckingFilter.OrderBy = StoreChecking_StoreCheckingFilterDTO.OrderBy;
            StoreCheckingFilter.OrderType = StoreChecking_StoreCheckingFilterDTO.OrderType;

            StoreCheckingFilter.Id = StoreChecking_StoreCheckingFilterDTO.Id;
            StoreCheckingFilter.StoreId = StoreChecking_StoreCheckingFilterDTO.StoreId;
            StoreCheckingFilter.AppUserId = StoreChecking_StoreCheckingFilterDTO.AppUserId;
            StoreCheckingFilter.Longtitude = StoreChecking_StoreCheckingFilterDTO.Longtitude;
            StoreCheckingFilter.Latitude = StoreChecking_StoreCheckingFilterDTO.Latitude;
            StoreCheckingFilter.CheckInAt = StoreChecking_StoreCheckingFilterDTO.CheckInAt;
            StoreCheckingFilter.CheckOutAt = StoreChecking_StoreCheckingFilterDTO.CheckOutAt;
            StoreCheckingFilter.CountIndirectSalesOrder = StoreChecking_StoreCheckingFilterDTO.CountIndirectSalesOrder;
            StoreCheckingFilter.CountImage = StoreChecking_StoreCheckingFilterDTO.CountImage;
            return StoreCheckingFilter;
        }

        [Route(StoreCheckingRoute.FilterListAlbum), HttpPost]
        public async Task<List<StoreChecking_AlbumDTO>> FilterListAlbum([FromBody] StoreChecking_AlbumFilterDTO StoreChecking_AlbumFilterDTO)
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

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<StoreChecking_AlbumDTO> StoreChecking_AlbumDTOs = Albums
                .Select(x => new StoreChecking_AlbumDTO(x)).ToList();
            return StoreChecking_AlbumDTOs;
        }
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
            AppUserFilter.Position = StoreChecking_AppUserFilterDTO.Position;
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
            AppUserFilter.Position = StoreChecking_AppUserFilterDTO.Position;
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
    }
}

