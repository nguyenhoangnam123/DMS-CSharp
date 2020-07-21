using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MImage;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using Hangfire.Storage;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_albums
{
    public class MonitorStoreAlbumController : MonitorController
    {
        private DataContext DataContext;
        private IAlbumService AlbumService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IImageService ImageService;
        private IStoreService StoreService;
        private IStoreCheckingService StoreCheckingService;
        private ICurrentContext CurrentContext;

        public MonitorStoreAlbumController
            (DataContext DataContext,
            IAlbumService AlbumService,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IImageService ImageService,
            IStoreService StoreService,
            IStoreCheckingService StoreCheckingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AlbumService = AlbumService;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
            this.ImageService = ImageService;
            this.StoreService = StoreService;
            this.StoreCheckingService = StoreCheckingService;
        }

        [Route(MonitorStoreAlbumRoute.FilterListAlbum), HttpPost]
        public async Task<List<MonitorStoreAlbum_AlbumDTO>> FilterListAlbum([FromBody] MonitorStoreAlbum_AlbumFilterDTO MonitorStoreAlbum_AlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AlbumFilter AlbumFilter = new AlbumFilter();
            AlbumFilter.Skip = 0;
            AlbumFilter.Take = 20;
            AlbumFilter.OrderBy = AlbumOrder.Id;
            AlbumFilter.OrderType = OrderType.ASC;
            AlbumFilter.Selects = AlbumSelect.ALL;
            AlbumFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<MonitorStoreAlbum_AlbumDTO> MonitorStoreAlbum_AlbumDTOs = Albums
                .Select(x => new MonitorStoreAlbum_AlbumDTO(x)).ToList();
            return MonitorStoreAlbum_AlbumDTOs;
        }

        [Route(MonitorStoreAlbumRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorStoreAlbum_AppUserDTO>> FilterListAppUser([FromBody] MonitorStoreAlbum_AppUserFilterDTO MonitorStoreAlbum_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = MonitorStoreAlbum_AppUserFilterDTO.Id;
            AppUserFilter.Username = MonitorStoreAlbum_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = MonitorStoreAlbum_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService,OrganizationService,CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreAlbum_AppUserDTO> MonitorStoreAlbum_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreAlbum_AppUserDTO(x)).ToList();
            return MonitorStoreAlbum_AppUserDTOs;
        }

        [Route(MonitorStoreAlbumRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreAlbum_OrganizationDTO>> FilterListOrganization()
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
            List<MonitorStoreAlbum_OrganizationDTO> StoreCheckerMonitor_OrganizationDTOs = Organizations
                .Select(x => new MonitorStoreAlbum_OrganizationDTO(x)).ToList();
            return StoreCheckerMonitor_OrganizationDTOs;
        }

        [Route(MonitorStoreAlbumRoute.FilterListStore), HttpPost]
        public async Task<List<MonitorStoreAlbum_StoreDTO>> FilterListStore([FromBody] MonitorStoreAlbum_StoreFilterDTO MonitorStoreAlbum_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = MonitorStoreAlbum_StoreFilterDTO.Id;
            StoreFilter.Code = MonitorStoreAlbum_StoreFilterDTO.Code;
            StoreFilter.Name = MonitorStoreAlbum_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = MonitorStoreAlbum_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = MonitorStoreAlbum_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = MonitorStoreAlbum_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = MonitorStoreAlbum_StoreFilterDTO.StoreGroupingId;
            StoreFilter.OwnerName = MonitorStoreAlbum_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = MonitorStoreAlbum_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = MonitorStoreAlbum_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<MonitorStoreAlbum_StoreDTO> MonitorStoreAlbum_StoreDTOs = Stores
                .Select(x => new MonitorStoreAlbum_StoreDTO(x)).ToList();
            return MonitorStoreAlbum_StoreDTOs;
        }

        [Route(MonitorStoreAlbumRoute.Count), HttpPost]
        public async Task<long> Count([FromBody] MonitorStoreAlbum_MonitorStoreAlbumFilterDTO MonitorStoreAlbum_MonitorStoreAlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long? AlbumId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AlbumId?.Equal;
            if (!AlbumId.HasValue) return 0;
            long? OrganizationId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AppUserId?.Equal;
            long? StoreId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.StoreId?.Equal;

            DateTime Start = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.GreaterEqual == null ?
                     StaticParams.DateTimeNow.Date :
                     MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            var query = from sc in DataContext.StoreChecking
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join scim in DataContext.StoreCheckingImageMapping on sc.Id equals scim.StoreCheckingId
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        FilterAppUserIds.Contains(sc.SaleEmployeeId) &&
                        (OrganizationId.HasValue == false || au.OrganizationId == OrganizationId.Value) &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (scim.AlbumId == AlbumId.Value)
                        select sc;
            List<StoreCheckingDAO> StoreCheckingDAOs = await query.Distinct().ToListAsync();
            List<long> StoreCheckingIds = StoreCheckingDAOs.Select(x => x.Id).ToList();
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = StoreCheckingIds },
                Selects = StoreCheckingSelect.ALL
            };
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);

            var StoreCheckingImageMappingQuery = from scim in DataContext.StoreCheckingImageMapping
                                                 join i in DataContext.Image on scim.ImageId equals i.Id
                                                 where StoreCheckingIds.Contains(scim.StoreCheckingId) &&
                                                 scim.AlbumId == AlbumId.Value
                                                 select scim;
            return await StoreCheckingImageMappingQuery.CountAsync();
        }

        [Route(MonitorStoreAlbumRoute.List), HttpPost]
        public async Task<MonitorStoreAlbum_MonitorStoreAlbumDTO> List([FromBody] MonitorStoreAlbum_MonitorStoreAlbumFilterDTO MonitorStoreAlbum_MonitorStoreAlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long? AlbumId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AlbumId?.Equal;
            if (!AlbumId.HasValue) return new MonitorStoreAlbum_MonitorStoreAlbumDTO();
            long? OrganizationId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AppUserId?.Equal;
            long? StoreId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.StoreId?.Equal;

            DateTime Start = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.GreaterEqual == null ?
            StaticParams.DateTimeNow.Date :
            MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            var query = from sc in DataContext.StoreChecking
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join scim in DataContext.StoreCheckingImageMapping on sc.Id equals scim.StoreCheckingId
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        FilterAppUserIds.Contains(sc.SaleEmployeeId) &&
                        (OrganizationId.HasValue == false || au.OrganizationId == OrganizationId.Value) &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (scim.AlbumId == AlbumId.Value)
                        select sc;
            List<StoreCheckingDAO> StoreCheckingDAOs = await query.Distinct().ToListAsync();
            List<long> StoreCheckingIds = StoreCheckingDAOs.Select(x => x.Id).ToList();
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = StoreCheckingIds },
                Selects = StoreCheckingSelect.ALL
            };
            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);

            var StoreCheckingImageMappingQuery = from scim in DataContext.StoreCheckingImageMapping
                                                 join i in DataContext.Image on scim.ImageId equals i.Id
                                                 where StoreCheckingIds.Contains(scim.StoreCheckingId) &&
                                                 scim.AlbumId == AlbumId.Value
                                                 select new MonitorStoreAlbum_StoreCheckingImageMappingDTO 
                                                 {
                                                     AlbumId = scim.AlbumId,
                                                     ImageId = scim.ImageId,
                                                     ShootingAt = scim.ShootingAt,
                                                     StoreCheckingId = scim.StoreCheckingId,
                                                     Image = scim.Image == null ? null : new MonitorStoreAlbum_ImageDTO
                                                     {
                                                         Id = scim.Image.Id,
                                                         Url = scim.Image.Url,
                                                         Name = scim.Image.Name,
                                                     }
                                                 };
            List<MonitorStoreAlbum_StoreCheckingImageMappingDTO> MonitorStoreAlbum_StoreCheckingImageMappingDTOs = await StoreCheckingImageMappingQuery.OrderBy(x => x.ShootingAt).ToListAsync();

            List<Album> Albums = await AlbumService.List(new AlbumFilter 
            { 
                Skip = 0,
                Take = int.MaxValue,
                Selects = AlbumSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            });
            MonitorStoreAlbum_MonitorStoreAlbumDTO MonitorStoreAlbum_MonitorStoreAlbumDTO = new MonitorStoreAlbum_MonitorStoreAlbumDTO();
            MonitorStoreAlbum_MonitorStoreAlbumDTO.StoreCheckingImageMappings = MonitorStoreAlbum_StoreCheckingImageMappingDTOs;
            foreach (var StoreCheckingImageMapping in MonitorStoreAlbum_MonitorStoreAlbumDTO.StoreCheckingImageMappings)
            {
                StoreCheckingImageMapping.Album = Albums.Where(x => x.Id == StoreCheckingImageMapping.AlbumId).Select(x => new MonitorStoreAlbum_AlbumDTO(x)).FirstOrDefault();
                StoreCheckingImageMapping.StoreChecking = StoreCheckings.Where(x => x.Id == StoreCheckingImageMapping.StoreCheckingId).Select(x => new MonitorStoreAlbum_StoreCheckingDTO(x)).FirstOrDefault();
            }
            return MonitorStoreAlbum_MonitorStoreAlbumDTO;
        }

        [Route(MonitorStoreAlbumRoute.Get), HttpPost]
        public async Task<MonitorStoreAlbum_StoreCheckingImageMappingDTO> Get([FromBody] MonitorStoreAlbum_StoreCheckingImageMappingDTO MonitorStoreAlbum_StoreCheckingImageMappingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreCheckingImageMappingDAO StoreCheckingImageMappingDAO = await DataContext.StoreCheckingImageMapping
                .Where(x => x.ImageId == MonitorStoreAlbum_StoreCheckingImageMappingDTO.ImageId && x.StoreCheckingId == MonitorStoreAlbum_StoreCheckingImageMappingDTO.StoreCheckingId).FirstOrDefaultAsync();
            if (StoreCheckingImageMappingDAO == null)
                return new MonitorStoreAlbum_StoreCheckingImageMappingDTO();

            Image Image = await ImageService.Get(StoreCheckingImageMappingDAO.ImageId);
            Album Album = await AlbumService.Get(StoreCheckingImageMappingDAO.AlbumId);
            StoreChecking StoreChecking = await StoreCheckingService.Get(MonitorStoreAlbum_StoreCheckingImageMappingDTO.StoreCheckingId);
            MonitorStoreAlbum_StoreCheckingImageMappingDTO = new MonitorStoreAlbum_StoreCheckingImageMappingDTO() 
            {
                ImageId = StoreCheckingImageMappingDAO.ImageId,
                StoreCheckingId = StoreCheckingImageMappingDAO.StoreCheckingId,
                AlbumId = StoreCheckingImageMappingDAO.AlbumId,
                ShootingAt = StoreCheckingImageMappingDAO.ShootingAt,
                Album = Album == null ? null : new MonitorStoreAlbum_AlbumDTO
                {
                    Id = Album.Id,
                    Name = Album.Name,
                    StatusId = Album.StatusId
                },
                Image = Image == null ? null : new MonitorStoreAlbum_ImageDTO
                {
                    Id = Image.Id,
                    Name = Image.Name,
                    Url = Image.Url,
                },
                StoreChecking = new MonitorStoreAlbum_StoreCheckingDTO
                {
                    Id = StoreChecking.Id,
                    CheckInAt = StoreChecking.CheckInAt,
                    CheckOutAt = StoreChecking.CheckOutAt,
                    Latitude = StoreChecking.Latitude,
                    Longitude = StoreChecking.Longitude,
                    SaleEmployeeId = StoreChecking.SaleEmployeeId,
                    StoreId = StoreChecking.StoreId,
                    SaleEmployee = StoreChecking.SaleEmployee == null ? null : new MonitorStoreAlbum_AppUserDTO
                    {
                        Id = StoreChecking.SaleEmployee.Id,
                        DisplayName = StoreChecking.SaleEmployee.DisplayName,
                        Username = StoreChecking.SaleEmployee.Username,
                    },
                    Store = StoreChecking.Store == null ? null : new MonitorStoreAlbum_StoreDTO
                    {
                        Id = StoreChecking.Store.Id,
                        Code = StoreChecking.Store.Code,
                        Name = StoreChecking.Store.Name,
                        Telephone = StoreChecking.Store.Telephone,
                        StatusId = StoreChecking.Store.StatusId,
                    }
                }
            };

            return MonitorStoreAlbum_StoreCheckingImageMappingDTO;
        }

    }
}
