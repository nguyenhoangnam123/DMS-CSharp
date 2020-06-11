﻿using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
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
    public class MonitorStoreAlbumController : RpcController
    {
        private DataContext DataContext;
        private IAlbumService AlbumService;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IStoreService StoreService;
        private IStoreCheckingService StoreCheckingService;

        public MonitorStoreAlbumController
            (DataContext DataContext,
            IAlbumService AlbumService,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreCheckingService StoreCheckingService)
        {
            this.DataContext = DataContext;
            this.AlbumService = AlbumService;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.StoreService = StoreService;
            this.StoreCheckingService = StoreCheckingService;
        }

        [Route(MonitorStoreAlbumRoute.FilterListAlbum), HttpPost]
        public async Task<List<MonitorStoreAlbum_AlbumDTO>> FilterListAlbum([FromBody] MonitorStoreAlbum_AlbumFilterDTO MonitorStoreAlbum_AlbumFilterDTO)
        {
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

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreAlbum_AppUserDTO> MonitorStoreAlbum_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreAlbum_AppUserDTO(x)).ToList();
            return MonitorStoreAlbum_AppUserDTOs;
        }

        [Route(MonitorStoreAlbumRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreAlbum_OrganizationDTO>> FilterListOrganization([FromBody] MonitorStoreAlbum_OrganizationFilterDTO MonitorStoreAlbum_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

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
            StoreFilter.ResellerId = MonitorStoreAlbum_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = MonitorStoreAlbum_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = MonitorStoreAlbum_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = MonitorStoreAlbum_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = MonitorStoreAlbum_StoreFilterDTO.WardId;
            StoreFilter.Address = MonitorStoreAlbum_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = MonitorStoreAlbum_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = MonitorStoreAlbum_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = MonitorStoreAlbum_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = MonitorStoreAlbum_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = MonitorStoreAlbum_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = MonitorStoreAlbum_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = MonitorStoreAlbum_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = MonitorStoreAlbum_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<MonitorStoreAlbum_StoreDTO> MonitorStoreAlbum_StoreDTOs = Stores
                .Select(x => new MonitorStoreAlbum_StoreDTO(x)).ToList();
            return MonitorStoreAlbum_StoreDTOs;
        }

        [Route(MonitorStoreAlbumRoute.List), HttpPost]
        public async Task<List<MonitorStoreAlbum_MonitorStoreAlbumDTO>> List([FromBody] MonitorStoreAlbum_MonitorStoreAlbumFilterDTO MonitorStoreAlbum_MonitorStoreAlbumFilterDTO)
        {
            long? AlbumId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AlbumId?.Equal;
            long? OrganizationId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.SaleEmployeeId?.Equal;
            long? StoreId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.StoreId?.Equal;
            
            DateTime Start = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            var query = from sc in DataContext.StoreChecking
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join scim in DataContext.StoreCheckingImageMapping on sc.Id equals scim.StoreCheckingId
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationId.HasValue == false || au.OrganizationId == OrganizationId.Value) &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (AlbumId.HasValue == false || scim.AlbumId == AlbumId.Value)
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
                                                 where StoreCheckingIds.Contains(scim.StoreCheckingId)
                                                 select new MonitorStoreAlbum_StoreCheckingImageMappingDTO 
                                                 {
                                                     AlbumId = scim.AlbumId,
                                                     ImageId = scim.ImageId,
                                                     SaleEmployeeId = scim.SaleEmployeeId,
                                                     ShootingAt = scim.ShootingAt,
                                                     StoreCheckingId = scim.StoreCheckingId,
                                                     StoreId = scim.StoreId,
                                                     Image = new MonitorStoreAlbum_ImageDTO
                                                     {
                                                         Id = scim.Image.Id,
                                                         Url = scim.Image.Url,
                                                         Name = scim.Image.Name,
                                                     }
                                                 };
            List<MonitorStoreAlbum_StoreCheckingImageMappingDTO> MonitorStoreAlbum_StoreCheckingImageMappingDTOs = await StoreCheckingImageMappingQuery.OrderBy(x => x.ShootingAt).ToListAsync();

            List<MonitorStoreAlbum_MonitorStoreAlbumDTO> MonitorStoreAlbum_MonitorStoreAlbumDTOs = new List<MonitorStoreAlbum_MonitorStoreAlbumDTO>();
            foreach (StoreChecking StoreChecking in StoreCheckings)
            {
                MonitorStoreAlbum_MonitorStoreAlbumDTO MonitorStoreAlbum_MonitorStoreAlbumDTO = new MonitorStoreAlbum_MonitorStoreAlbumDTO()
                {
                    CheckInAt = StoreChecking.CheckInAt,
                    CheckOutAt = StoreChecking.CheckOutAt,
                    Latitude = StoreChecking.Latitude,
                    Longtitude = StoreChecking.Longtitude,
                    SaleEmployeeId = StoreChecking.SaleEmployeeId,
                    StoreId = StoreChecking.StoreId,
                    SaleEmployee = new MonitorStoreAlbum_AppUserDTO
                    {
                        Id = StoreChecking.SaleEmployee.Id,
                        Username = StoreChecking.SaleEmployee.Username,
                        DisplayName = StoreChecking.SaleEmployee.DisplayName,
                    },
                    Store = new MonitorStoreAlbum_StoreDTO
                    {
                        Id = StoreChecking.Store.Id,
                        Code = StoreChecking.Store.Code,
                        Name = StoreChecking.Store.Name,
                        Telephone = StoreChecking.Store.Telephone,
                        StatusId = StoreChecking.Store.StatusId,
                    }
                };
                MonitorStoreAlbum_MonitorStoreAlbumDTO.StoreCheckingImageMappings = MonitorStoreAlbum_StoreCheckingImageMappingDTOs.Where(x => x.StoreCheckingId == StoreChecking.Id).ToList();
                MonitorStoreAlbum_MonitorStoreAlbumDTOs.Add(MonitorStoreAlbum_MonitorStoreAlbumDTO);
            }

            return MonitorStoreAlbum_MonitorStoreAlbumDTOs;
        }

        public async Task<StoreChecking> Get([FromBody] MonitorStoreAlbum_StoreCheckingImageMappingDTO MonitorStoreAlbum_StoreCheckingImageMappingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreChecking StoreChecking = new StoreChecking
            {
                Id = MonitorStoreAlbum_StoreCheckingDTO.Id
            };

            StoreChecking = await StoreCheckingService.Get(StoreChecking.Id);
            return StoreChecking;
        }
    }
}
