using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using DMS.Services.MStoreGrouping;
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
        private ICurrentContext CurrentContext;
        public ReportStoreCheckedController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IStoreCheckingService StoreCheckingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.StoreCheckingService = StoreCheckingService;
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

            //if (End.Subtract(Start).Days > 31)
            //    return 0;

            long? SaleEmployeeId = ReportStoreChecked_ReportStoreCheckedFilterDTO.AppUserId?.Equal;
            long? StoreId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreChecked_ReportStoreCheckedFilterDTO.StoreGroupingId?.Equal;

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
                        StoreIds.Contains(s.Id) &&
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
                        OrganizationIds.Contains(au.OrganizationId)
                        select au;

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

            //if (End.Subtract(Start).Days > 31)
            //    return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? SaleEmployeeId = ReportStoreChecker_ReportStoreCheckedFilterDTO.AppUserId?.Equal;
            long? StoreId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreGroupingId?.Equal;

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
                        StoreIds.Contains(s.Id) &&
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
                        OrganizationIds.Contains(au.OrganizationId)
                        select au;

            var AppUserIds = await query.Select(x => x.Id).Distinct().ToListAsync();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au => AppUserIds.Contains(au.Id) && OrganizationIds.Contains(au.OrganizationId))
                .Include(au => au.Organization)
                .OrderBy(su => su.OrganizationId)
                .Skip(ReportStoreChecker_ReportStoreCheckedFilterDTO.Skip)
                .Take(ReportStoreChecker_ReportStoreCheckedFilterDTO.Take)
                .ToListAsync();

            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => OrganizationIds.Contains(x.OrganizationId) &&
                        StoreIds.Contains(x.Id) &&
                        StoreTypeIds.Contains(x.StoreTypeId) &&
                        (
                            (
                                StoreGroupingId.HasValue == false &&
                                (x.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(x.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue &&
                                StoreGroupingId.Value == x.StoreGroupingId.Value
                            )
                        )
                )
                .ToListAsync();

            List<ReportStoreChecked_SaleEmployeeDTO> ReportStoreChecked_SaleEmployeeDTOs = AppUserDAOs.Select(au => new ReportStoreChecked_SaleEmployeeDTO
            {
                SaleEmployeeId = au.Id,
                Username = au.Username,
                DisplayName = au.DisplayName,
                OrganizationName = au.Organization?.Name,
                OrganizationPath = au.Organization?.Path,
            }).ToList();

            List<string> OrganizationNames = ReportStoreChecked_SaleEmployeeDTOs.Select(se => se.OrganizationName).Distinct().ToList();
            OrganizationNames = OrganizationNames.OrderBy(x => x).ToList();
            List<ReportStoreChecked_ReportStoreCheckedDTO> ReportStoreChecked_ReportStoreCheckedDTOs = OrganizationNames.Select(on => new ReportStoreChecked_ReportStoreCheckedDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportStoreChecked_ReportStoreCheckedDTO ReportStoreChecked_ReportStoreCheckedDTO in ReportStoreChecked_ReportStoreCheckedDTOs)
            {
                ReportStoreChecked_ReportStoreCheckedDTO.SaleEmployees = ReportStoreChecked_SaleEmployeeDTOs
                    .Where(se => se.OrganizationName == ReportStoreChecked_ReportStoreCheckedDTO.OrganizationName)
                    .ToList();
            }

            AppUserIds = AppUserDAOs.Select(s => s.Id).ToList();
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.ALL,
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End },
                SaleEmployeeId = new IdFilter { In = AppUserIds },
                StoreId = new IdFilter { In = StoreIds }
            };
            List<StoreChecking> storeCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            var StoreCheckingIds = storeCheckings.Select(x => x.Id).ToList();
            var SalesOrders = await DataContext.IndirectSalesOrder.Where(x => x.StoreCheckingId.HasValue &&
                StoreCheckingIds.Contains(x.StoreCheckingId.Value))
                .Select(x => new
                {
                    Id = x.Id,
                    StoreCheckingId = x.StoreCheckingId
                }).ToListAsync();

            // khởi tạo khung dữ liệu
            foreach (ReportStoreChecked_SaleEmployeeDTO ReportStoreChecked_SaleEmployeeDTO in ReportStoreChecked_SaleEmployeeDTOs)
            {
                ReportStoreChecked_SaleEmployeeDTO.StoreCheckingGroupByDates = new List<ReportStoreChecked_StoreCheckingGroupByDateDTO>();
                for (DateTime i = Start; i <= End; i = i.AddDays(1))
                {
                    ReportStoreChecked_StoreCheckingGroupByDateDTO ReportStoreChecked_StoreCheckingGroupByDateDTO = new ReportStoreChecked_StoreCheckingGroupByDateDTO();

                    ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings = storeCheckings.Where(x => x.SaleEmployeeId == ReportStoreChecked_SaleEmployeeDTO.SaleEmployeeId)
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
                    ReportStoreChecked_SaleEmployeeDTO.StoreCheckingGroupByDates.Add(ReportStoreChecked_StoreCheckingGroupByDateDTO);
                }
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
                        foreach(var Content in ReportStoreChecked_ExportGroupByDateDTO.Contents)
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
