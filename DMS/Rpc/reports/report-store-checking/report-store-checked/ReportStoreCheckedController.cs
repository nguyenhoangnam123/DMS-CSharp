﻿using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreStatus;
using DMS.Services.MStoreType;
using Hangfire.Annotations;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace DMS.Rpc.reports.report_store_checking.report_store_checked
{
    public class ReportStoreCheckedController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IStoreCheckingService StoreCheckingService;
        private IStoreStatusService StoreStatusService;
        private ICurrentContext CurrentContext;
        public ReportStoreCheckedController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IStoreCheckingService StoreCheckingService,
            IStoreStatusService StoreStatusService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.StoreCheckingService = StoreCheckingService;
            this.StoreStatusService = StoreStatusService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportStoreCheckedRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportStoreChecked_AppUserDTO>> FilterListAppUser([FromBody] ReportStoreChecked_AppUserFilterDTO ReportStoreChecked_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportStoreChecked_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportStoreChecked_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportStoreChecked_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportStoreChecked_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportStoreChecked_AppUserDTO> ReportStoreChecked_AppUserDTOs = AppUsers
                .Select(x => new ReportStoreChecked_AppUserDTO(x)).ToList();
            return ReportStoreChecked_AppUserDTOs;
        }

        [Route(ReportStoreCheckedRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStoreChecked_OrganizationDTO>> FilterListOrganization()
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

            List<ReportStoreChecked_OrganizationDTO> ReportStoreChecked_OrganizationDTOs = Organizations
                .Select(x => new ReportStoreChecked_OrganizationDTO(x)).ToList();
            return ReportStoreChecked_OrganizationDTOs;
        }

        [Route(ReportStoreCheckedRoute.FilterListStore), HttpPost]
        public async Task<List<ReportStoreChecked_StoreDTO>> FilterListStore([FromBody] ReportStoreChecker_StoreFilterDTO ReportStoreChecker_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportStoreChecker_StoreFilterDTO.Id;
            StoreFilter.Code = ReportStoreChecker_StoreFilterDTO.Code;
            StoreFilter.Name = ReportStoreChecker_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ReportStoreChecker_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ReportStoreChecker_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportStoreChecker_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportStoreChecker_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);

            List<ReportStoreChecked_StoreDTO> ReportStoreChecked_StoreDTOs = Stores
                .Select(x => new ReportStoreChecked_StoreDTO(x)).ToList();
            return ReportStoreChecked_StoreDTOs;
        }

        [Route(ReportStoreCheckedRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<ReportStoreChecked_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] ReportStoreChecked_StoreGroupingFilterDTO ReportStoreChecked_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ReportStoreChecked_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ReportStoreChecked_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ReportStoreChecked_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ReportStoreChecked_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ReportStoreChecked_StoreGroupingDTO> ReportStoreChecked_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ReportStoreChecked_StoreGroupingDTO(x)).ToList();
            return ReportStoreChecked_StoreGroupingDTOs;
        }
        [Route(ReportStoreCheckedRoute.FilterListStoreType), HttpPost]
        public async Task<List<ReportStoreChecked_StoreTypeDTO>> FilterListStoreType([FromBody] ReportStoreChecked_StoreTypeFilterDTO ReportStoreChecked_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ReportStoreChecked_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ReportStoreChecked_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ReportStoreChecked_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportStoreChecked_StoreTypeDTO> ReportStoreChecked_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportStoreChecked_StoreTypeDTO(x)).ToList();
            return ReportStoreChecked_StoreTypeDTOs;
        }

        [Route(ReportStoreCheckedRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportStoreChecked_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportStoreChecked_StoreStatusFilterDTO ReportStoreChecked_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportStoreChecked_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportStoreChecked_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportStoreChecked_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportStoreChecked_StoreStatusDTO> ReportStoreChecked_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportStoreChecked_StoreStatusDTO(x)).ToList();
            return ReportStoreChecked_StoreStatusDTOs;
        }

        #endregion

        [Route(ReportStoreCheckedRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStoreChecked_ReportStoreCheckedFilterDTO ReportStoreChecked_ReportStoreCheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreChecked_ReportStoreCheckedFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? SaleEmployeeId = ReportStoreChecked_ReportStoreCheckedFilterDTO.AppUserId?.Equal;
            long? StoreId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreChecked_ReportStoreCheckedFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreChecked_ReportStoreCheckedFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from sc in DataContext.StoreChecking
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        StoreTypeIds.Contains(s.StoreTypeId) &&
                        (
                            (
                                StoreGroupingId.HasValue == false &&
                                (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue &&
                                StoreGroupingId.Value == s.StoreGroupingId.Value
                            )
                        ) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        OrganizationIds.Contains(sc.OrganizationId) &&
                        s.DeletedAt == null
                        select new 
                        {
                            SalesEmployeeId = au.Id,
                            OrganizationId = sc.OrganizationId
                        };

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportStoreCheckedRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportStoreChecked_ReportStoreCheckedDTO>>> List([FromBody] ReportStoreChecked_ReportStoreCheckedFilterDTO ReportStoreChecker_ReportStoreCheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreChecker_ReportStoreCheckedFilterDTO.HasValue == false)
                return new List<ReportStoreChecked_ReportStoreCheckedDTO>();

            DateTime Start = ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? SaleEmployeeId = ReportStoreChecker_ReportStoreCheckedFilterDTO.AppUserId?.Equal;
            long? StoreId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreChecker_ReportStoreCheckedFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreChecker_ReportStoreCheckedFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from sc in DataContext.StoreChecking
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        StoreTypeIds.Contains(s.StoreTypeId) &&
                        (
                            (
                                StoreGroupingId.HasValue == false &&
                                (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue &&
                                StoreGroupingId.Value == s.StoreGroupingId.Value
                            )
                        ) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        OrganizationIds.Contains(sc.OrganizationId) &&
                        s.DeletedAt == null
                        select new
                        {
                            SalesEmployeeId = au.Id,
                            OrganizationId = sc.OrganizationId
                        };

            var Ids = await query
                .OrderBy(x => x.OrganizationId)
                .Skip(ReportStoreChecker_ReportStoreCheckedFilterDTO.Skip)
                .Take(ReportStoreChecker_ReportStoreCheckedFilterDTO.Take)
                .Distinct().ToListAsync();
            var AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(au => AppUserIds.Contains(au.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .ToListAsync();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            AppUserIds = AppUserDAOs.Select(s => s.Id).ToList();
            var query2 = from sc in DataContext.StoreChecking
                         join s in DataContext.Store on sc.StoreId equals s.Id
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         where AppUserIds.Contains(sc.SaleEmployeeId) &&
                         (sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End) &&
                         StoreTypeIds.Contains(s.StoreTypeId) &&
                         (
                             (
                                 StoreGroupingId.HasValue == false &&
                                 (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                             ) ||
                             (
                                 StoreGroupingId.HasValue &&
                                 StoreGroupingId.Value == s.StoreGroupingId.Value
                             )
                         ) &&
                         (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                         OrganizationIds.Contains(sc.OrganizationId) &&
                         s.DeletedAt == null
                         select sc;

            List<StoreCheckingDAO> storeCheckings = await query2.ToListAsync();
            StoreIds = storeCheckings.Select(x => x.StoreId).Distinct().ToList();
            tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query3 = from s in DataContext.Store
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         where s.DeletedAt == null
                         select new StoreDAO
                         {
                             Id = s.Id,
                             Code = s.Code,
                             Name = s.Name,
                             Address = s.Address,
                             StoreStatusId = s.StoreStatusId
                         };
            var StoreDAOs = await query3.ToListAsync();

            var StoreCheckingIds = storeCheckings.Select(x => x.Id).ToList();
            tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreCheckingIds);
            var query4 = from i in DataContext.IndirectSalesOrder
                         join tt in tempTableQuery.Query on i.StoreCheckingId equals tt.Column1
                         where i.StoreCheckingId.HasValue &&
                         i.RequestStateId == RequestStateEnum.APPROVED.Id
                         select new
                         {
                             Id = i.Id,
                             StoreCheckingId = i.StoreCheckingId
                         };
            var SalesOrders = await query4.ToListAsync();

            Parallel.ForEach(storeCheckings, storeChecking =>
            {
                var Store = StoreDAOs.Where(x => x.Id == storeChecking.StoreId).FirstOrDefault();
                if (Store != null)
                {
                    storeChecking.Store = new StoreDAO
                    {
                        Id = Store.Id,
                        Code = Store.Code,
                        Name = Store.Name,
                        Address = Store.Address,
                        StoreStatusId = Store.StoreStatusId,
                    };
                    storeChecking.Store.StoreStatus = new StoreStatusDAO
                    {
                        Name = StoreStatusEnum.StoreStatusEnumList.Where(x => x.Id == Store.StoreStatusId).Select(x => x.Name).FirstOrDefault()
                    };
                }
            });

            List<ReportStoreChecked_ReportStoreCheckedDTO> ReportStoreChecked_ReportStoreCheckedDTOs = new List<ReportStoreChecked_ReportStoreCheckedDTO>();
            foreach (var Organization in Organizations)
            {
                ReportStoreChecked_ReportStoreCheckedDTO ReportStoreChecked_ReportStoreCheckedDTO = new ReportStoreChecked_ReportStoreCheckedDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<ReportStoreChecked_SaleEmployeeDTO>()
                };
                ReportStoreChecked_ReportStoreCheckedDTO.SaleEmployees = Ids
                    .Where(x => x.OrganizationId == Organization.Id)
                    .Select(x => new ReportStoreChecked_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.SalesEmployeeId
                    }).ToList();
                ReportStoreChecked_ReportStoreCheckedDTOs.Add(ReportStoreChecked_ReportStoreCheckedDTO);
            }

            foreach (var ReportStoreChecked_ReportStoreCheckedDTO in ReportStoreChecked_ReportStoreCheckedDTOs)
            {
                Parallel.ForEach(ReportStoreChecked_ReportStoreCheckedDTO.SaleEmployees, SaleEmployee =>
                {
                    var appUser = AppUserDAOs.Where(x => x.Id == SaleEmployee.SaleEmployeeId).FirstOrDefault();
                    if (appUser != null)
                    {
                        SaleEmployee.Username = appUser.Username;
                        SaleEmployee.DisplayName = appUser.DisplayName;
                        SaleEmployee.StoreCheckingGroupByDates = new List<ReportStoreChecked_StoreCheckingGroupByDateDTO>();
                    }
                });

                Parallel.ForEach(ReportStoreChecked_ReportStoreCheckedDTO.SaleEmployees, SaleEmployee =>
                {
                    SaleEmployee.StoreCheckingGroupByDates = new List<ReportStoreChecked_StoreCheckingGroupByDateDTO>();
                    for (DateTime i = Start; i <= End; i = i.AddDays(1))
                    {
                        ReportStoreChecked_StoreCheckingGroupByDateDTO ReportStoreChecked_StoreCheckingGroupByDateDTO = new ReportStoreChecked_StoreCheckingGroupByDateDTO();

                        ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings = storeCheckings
                            .Where(x => x.SaleEmployeeId == SaleEmployee.SaleEmployeeId)
                            .Where(x => x.OrganizationId == ReportStoreChecked_ReportStoreCheckedDTO.OrganizationId)
                            .Where(x => i <= x.CheckOutAt.Value && x.CheckOutAt.Value < i.AddDays(1))
                            .Select(x => new ReportStoreChecked_StoreCheckingDTO
                            {
                                Id = x.Id,
                                CheckIn = x.CheckInAt.Value,
                                CheckOut = x.CheckOutAt.Value,
                                DeviceName = x.DeviceName,
                                ImageCounter = x.ImageCounter ?? 0,
                                Planned = x.Planned,
                                SaleEmployeeId = x.SaleEmployeeId,
                                StoreStatusId = x.Store.StoreStatusId,
                                StoreStatusName = x.Store.StoreStatus.Name,
                                StoreName = x.Store.Name,
                                StoreCode = x.Store.Code,
                                StoreAddress = x.Store.Address,
                                CheckInDistance = $"{x.CheckInDistance} m",
                                CheckOutDistance = $"{x.CheckOutDistance} m",
                            }).ToList();

                        if (!ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings.Any())
                            continue;

                        ReportStoreChecked_StoreCheckingGroupByDateDTO.Date = i;
                        ReportStoreChecked_StoreCheckingGroupByDateDTO.DateString = ReportStoreChecked_StoreCheckingGroupByDateDTO.Date.ToString("dd-MM-yyyy");
                        var dayOfWeek = ReportStoreChecked_StoreCheckingGroupByDateDTO.Date.AddHours(CurrentContext.TimeZone).DayOfWeek.ToString();
                        ReportStoreChecked_StoreCheckingGroupByDateDTO.DayOfWeek = DayOfWeekEnum.DayOfWeekEnumList.Where(x => x.Code == dayOfWeek).Select(x => x.Name).FirstOrDefault();

                        foreach (var StoreChecking in ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings)
                        {
                            StoreChecking.eCheckIn = StoreChecking.CheckIn.AddHours(CurrentContext.TimeZone).ToString("HH:mm:ss");
                            StoreChecking.eCheckOut = StoreChecking.CheckOut.AddHours(CurrentContext.TimeZone).ToString("HH:mm:ss");

                            var TotalMinuteChecking = StoreChecking.CheckOut.Subtract(StoreChecking.CheckIn).TotalSeconds;
                            TimeSpan timeSpan = TimeSpan.FromSeconds(TotalMinuteChecking);
                            StoreChecking.Duaration = $"{timeSpan.Hours.ToString().PadLeft(2, '0')} : {timeSpan.Minutes.ToString().PadLeft(2, '0')} : {timeSpan.Seconds.ToString().PadLeft(2, '0')}";
                            var HasSalesOrder = SalesOrders.Where(x => x.StoreCheckingId == StoreChecking.Id).FirstOrDefault();
                            if (HasSalesOrder == null)
                                StoreChecking.SalesOrder = false;
                            else
                                StoreChecking.SalesOrder = true;
                        }
                        SaleEmployee.StoreCheckingGroupByDates.Add(ReportStoreChecked_StoreCheckingGroupByDateDTO);
                    }
                });
            }

            return ReportStoreChecked_ReportStoreCheckedDTOs;
        }

        [Route(ReportStoreCheckedRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportStoreChecked_ReportStoreCheckedFilterDTO ReportStoreChecked_ReportStoreCheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn?.GreaterEqual == null ?
                LocalStartDay(CurrentContext) :
                ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn?.LessEqual == null ?
                LocalEndDay(CurrentContext) :
                ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn.LessEqual.Value;

            ReportStoreChecked_ReportStoreCheckedFilterDTO.Skip = 0;
            ReportStoreChecked_ReportStoreCheckedFilterDTO.Take = int.MaxValue;
            List<ReportStoreChecked_ReportStoreCheckedDTO> ReportStoreChecked_ReportStoreCheckedDTOs = (await List(ReportStoreChecked_ReportStoreCheckedFilterDTO)).Value;

            List<ReportStoreChecked_ExportDTO> ReportStoreChecked_ExportDTOs = new List<ReportStoreChecked_ExportDTO>();

            foreach (ReportStoreChecked_ReportStoreCheckedDTO ReportStoreChecked_ReportStoreCheckedDTO in ReportStoreChecked_ReportStoreCheckedDTOs)
            {
                ReportStoreChecked_ExportDTO ReportStoreChecked_ExportDTO = new ReportStoreChecked_ExportDTO(ReportStoreChecked_ReportStoreCheckedDTO);
                ReportStoreChecked_ExportDTOs.Add(ReportStoreChecked_ExportDTO);
            }

            var STT = 1;
            foreach (ReportStoreChecked_ExportDTO ReportStoreChecked_ExportDTO in ReportStoreChecked_ExportDTOs)
            {
                foreach (var SaleEmployee in ReportStoreChecked_ExportDTO.SalesEmployees)
                {
                    foreach (var ReportStoreChecked_ExportGroupByDateDTO in SaleEmployee.Dates)
                    {
                        ReportStoreChecked_ExportGroupByDateDTO.Date = ReportStoreChecked_ExportGroupByDateDTO.Date.AddHours(CurrentContext.TimeZone);
                        ReportStoreChecked_ExportGroupByDateDTO.DateString = ReportStoreChecked_ExportGroupByDateDTO.Date.ToString("dd-MM-yyyy");
                        foreach (var Content in ReportStoreChecked_ExportGroupByDateDTO.Contents)
                        {
                            Content.STT = STT;
                            STT++;
                            Content.Username = SaleEmployee.Username;
                            Content.DisplayName = SaleEmployee.DisplayName;
                            Content.DateString = ReportStoreChecked_ExportGroupByDateDTO.DateString;
                            Content.DayOfWeek = ReportStoreChecked_ExportGroupByDateDTO.DayOfWeek;
                        }
                    }

                }
            }

            string path = "Templates/Report_Store_Checked.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportStoreCheckeds = ReportStoreChecked_ExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportStoreChecked.xlsx");
        }
    }
}
