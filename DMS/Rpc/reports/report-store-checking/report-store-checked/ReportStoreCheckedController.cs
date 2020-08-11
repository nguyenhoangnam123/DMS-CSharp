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
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    StaticParams.DateTimeNow :
                    ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreChecked_ReportStoreCheckedFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return 0;

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

            var query = from sc in DataContext.StoreChecking
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (StoreTypeId.HasValue == false || s.StoreTypeId == StoreTypeId.Value) &&
                        (StoreGroupingId.HasValue == false || s.StoreGroupingId == StoreGroupingId.Value) &&
                        (au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
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
                    StaticParams.DateTimeNow :
                    ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return BadRequest("Chỉ được phép xem tối đa trong vòng 31 ngày");

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

            var query = from sc in DataContext.StoreChecking
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (StoreTypeId.HasValue == false || s.StoreTypeId == StoreTypeId.Value) &&
                        (StoreGroupingId.HasValue == false || s.StoreGroupingId == StoreGroupingId.Value) &&
                        (au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
                        select au;

            var AppUserIds = await query.Select(x => x.Id).Distinct().ToListAsync();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Id = new IdFilter { In = AppUserIds },
                OrganizationId = new IdFilter { In = OrganizationIds },
                Skip = ReportStoreChecker_ReportStoreCheckedFilterDTO.Skip,
                Take = ReportStoreChecker_ReportStoreCheckedFilterDTO.Take,
                OrderBy = AppUserOrder.DisplayName,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => OrganizationIds.Contains(x.OrganizationId) &&
                (StoreId == null || x.Id == StoreId.Value) &&
                (StoreTypeId == null || x.StoreTypeId == StoreTypeId.Value) &&
                (StoreGroupingId == null || x.StoreGroupingId == StoreGroupingId.Value))
                .ToListAsync();

            List<ReportStoreChecked_SaleEmployeeDTO> ReportStoreChecked_SaleEmployeeDTOs = AppUsers.Select(au => new ReportStoreChecked_SaleEmployeeDTO
            {
                SaleEmployeeId = au.Id,
                Username = au.Username,
                DisplayName = au.DisplayName,
                OrganizationName = au.Organization?.Name,
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

            AppUserIds = AppUsers.Select(s => s.Id).ToList();
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.ALL,
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End },
                SaleEmployeeId = new IdFilter { In = AppUserIds }
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
                for (DateTime i = Start; i < End; i = i.AddDays(1))
                {
                    ReportStoreChecked_StoreCheckingGroupByDateDTO ReportStoreChecked_StoreCheckingGroupByDateDTO = new ReportStoreChecked_StoreCheckingGroupByDateDTO();
                    ReportStoreChecked_StoreCheckingGroupByDateDTO.Date = i;
                    ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings = storeCheckings.Where(x => x.SaleEmployeeId == ReportStoreChecked_SaleEmployeeDTO.SaleEmployeeId)
                        .Where(x => x.CheckOutAt.Value.Date == i)
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
                    foreach (var StoreChecking in ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings)
                    {
                        var TotalMinuteChecking = StoreChecking.CheckOut.Subtract(StoreChecking.CheckIn).TotalSeconds;
                        TimeSpan timeSpan = TimeSpan.FromSeconds(TotalMinuteChecking);
                        StoreChecking.Duaration = $"{timeSpan.Hours.ToString().PadLeft(2,'0')}: {timeSpan.Minutes.ToString().PadLeft(2, '0')}";
                        var HasSalesOrder = SalesOrders.Where(x => x.StoreCheckingId == StoreChecking.Id).FirstOrDefault();
                        if (HasSalesOrder == null)
                            StoreChecking.SalesOrder = false;
                        else
                            StoreChecking.SalesOrder = true;
                    }
                    ReportStoreChecked_SaleEmployeeDTO.StoreCheckingGroupByDates.Add(ReportStoreChecked_StoreCheckingGroupByDateDTO);
                }
            }

            foreach (var ReportStoreChecked_ReportStoreCheckerDTO in ReportStoreChecked_ReportStoreCheckedDTOs)
            {
                foreach (var SaleEmployee in ReportStoreChecked_ReportStoreCheckerDTO.SaleEmployees)
                {
                    SaleEmployee.StoreCheckingGroupByDates = SaleEmployee.StoreCheckingGroupByDates.Where(x => x.StoreCheckings.Any()).ToList();
                }
                ReportStoreChecked_ReportStoreCheckerDTO.SaleEmployees = ReportStoreChecked_ReportStoreCheckerDTO.SaleEmployees.Where(x => x.StoreCheckingGroupByDates.Any()).ToList();
            }
            ReportStoreChecked_ReportStoreCheckedDTOs = ReportStoreChecked_ReportStoreCheckedDTOs.Where(x => x.SaleEmployees.Any()).ToList();
            return ReportStoreChecked_ReportStoreCheckedDTOs;
        }
    }
}
