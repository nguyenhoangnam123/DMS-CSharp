﻿using Common;
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
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

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
            AppUserFilter.OrganizationId = MonitorStoreAlbum_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
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

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
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

            long? SaleEmployeeId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AppUserId?.Equal;
            long? StoreId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.StoreId?.Equal;

            DateTime Start = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.LessEqual.Value;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                      .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from si in DataContext.StoreImage
                        join tt in tempTableQuery.Query on si.StoreId equals tt.Column1
                        where Start <= si.ShootingAt && si.ShootingAt <= End &&
                        si.AlbumId == AlbumId.Value &&
                        (
                            (
                                SaleEmployeeId.HasValue == false &&
                                (si.SaleEmployeeId.HasValue == false || FilterAppUserIds.Contains(si.SaleEmployeeId.Value))
                            ) ||
                            (
                                SaleEmployeeId.HasValue &&
                                SaleEmployeeId.Value == si.SaleEmployeeId.Value
                            )
                        ) &&
                        OrganizationIds.Contains(si.OrganizationId) &&
                        (SaleEmployeeId.HasValue == false || si.SaleEmployeeId == SaleEmployeeId.Value)
                        select si;

            return await query.CountAsync();
        }

        [Route(MonitorStoreAlbumRoute.List), HttpPost]
        public async Task<List<MonitorStoreAlbum_StoreCheckingImageMappingDTO>> List([FromBody] MonitorStoreAlbum_MonitorStoreAlbumFilterDTO MonitorStoreAlbum_MonitorStoreAlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? AlbumId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AlbumId?.Equal;
            if (!AlbumId.HasValue) return new List<MonitorStoreAlbum_StoreCheckingImageMappingDTO>();

            long? SaleEmployeeId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.AppUserId?.Equal;
            long? StoreId = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.StoreId?.Equal;

            DateTime Start = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.GreaterEqual == null ?
                      LocalStartDay(CurrentContext) :
                      MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.CheckIn.LessEqual.Value;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                      .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query = from si in DataContext.StoreImage
                        join tt in tempTableQuery.Query on si.StoreId equals tt.Column1
                        where Start <= si.ShootingAt && si.ShootingAt <= End &&
                        si.AlbumId == AlbumId.Value &&
                        (
                            (
                                SaleEmployeeId.HasValue == false &&
                                (si.SaleEmployeeId.HasValue == false || FilterAppUserIds.Contains(si.SaleEmployeeId.Value))
                            ) ||
                            (
                                SaleEmployeeId.HasValue &&
                                SaleEmployeeId.Value == si.SaleEmployeeId.Value
                            )
                        ) &&
                        OrganizationIds.Contains(si.OrganizationId) &&
                        (SaleEmployeeId.HasValue == false || si.SaleEmployeeId == SaleEmployeeId.Value)
                        select si;

            List<MonitorStoreAlbum_StoreCheckingImageMappingDTO> MonitorStoreAlbum_StoreCheckingImageMappingDTOs = await query.OrderByDescending(x => x.ShootingAt)
                .Skip(MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.Skip)
                .Take(MonitorStoreAlbum_MonitorStoreAlbumFilterDTO.Take)
                .Select(x => new MonitorStoreAlbum_StoreCheckingImageMappingDTO(x))
                .ToListAsync();

            return MonitorStoreAlbum_StoreCheckingImageMappingDTOs;
        }
    }
}
