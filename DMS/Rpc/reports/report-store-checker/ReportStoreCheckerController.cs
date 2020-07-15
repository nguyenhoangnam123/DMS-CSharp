using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
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

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreCheckerController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportStoreCheckerController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportStoreCheckerRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportStoreChecker_AppUserDTO>> FilterListAppUser([FromBody] ReportStoreChecker_AppUserFilterDTO ReportStoreChecker_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportStoreChecker_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportStoreChecker_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportStoreChecker_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportStoreChecker_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportStoreChecker_AppUserDTO> StoreCheckerReport_AppUserDTOs = AppUsers
                .Select(x => new ReportStoreChecker_AppUserDTO(x)).ToList();
            return StoreCheckerReport_AppUserDTOs;
        }

        [Route(ReportStoreCheckerRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStoreChecker_OrganizationDTO>> FilterListOrganization()
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

            List<ReportStoreChecker_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportStoreChecker_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportStoreCheckerRoute.FilterListStore), HttpPost]
        public async Task<List<ReportStoreChecker_StoreDTO>> FilterListStore([FromBody] ReportStoreChecker_StoreFilterDTO ReportStoreChecker_StoreFilterDTO)
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

            List<ReportStoreChecker_StoreDTO> ReportStoreChecker_StoreDTOs = Stores
                .Select(x => new ReportStoreChecker_StoreDTO(x)).ToList();
            return ReportStoreChecker_StoreDTOs;
        }

        [Route(ReportStoreCheckerRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<ReportStoreChecker_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] ReportStoreChecker_StoreGroupingFilterDTO ReportStoreChecker_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ReportStoreChecker_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ReportStoreChecker_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ReportStoreChecker_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ReportStoreChecker_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ReportStoreChecker_StoreGroupingDTO> ReportStoreChecker_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ReportStoreChecker_StoreGroupingDTO(x)).ToList();
            return ReportStoreChecker_StoreGroupingDTOs;
        }
        [Route(ReportStoreCheckerRoute.FilterListStoreType), HttpPost]
        public async Task<List<ReportStoreChecker_StoreTypeDTO>> FilterListStoreType([FromBody] ReportStoreChecker_StoreTypeFilterDTO ReportStoreChecker_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ReportStoreChecker_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ReportStoreChecker_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ReportStoreChecker_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportStoreChecker_StoreTypeDTO> ReportStoreChecker_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportStoreChecker_StoreTypeDTO(x)).ToList();
            return ReportStoreChecker_StoreTypeDTOs;
        }
        #endregion

        [Route(ReportStoreCheckerRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStoreChecker_ReportStoreCheckerFilterDTO ReportStoreChecker_ReportStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreChecker_ReportStoreCheckerFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = ReportStoreChecker_ReportStoreCheckerFilterDTO.AppUserId?.Equal;
            long? StoreId = ReportStoreChecker_ReportStoreCheckerFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreChecker_ReportStoreCheckerFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreChecker_ReportStoreCheckerFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreChecker_ReportStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreChecker_ReportStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

        [Route(ReportStoreCheckerRoute.List), HttpPost]
        public async Task<List<ReportStoreChecker_ReportStoreCheckerDTO>> List([FromBody] ReportStoreChecker_ReportStoreCheckerFilterDTO ReportStoreChecker_ReportStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreChecker_ReportStoreCheckerFilterDTO.HasValue == false)
                return new List<ReportStoreChecker_ReportStoreCheckerDTO>();

            DateTime Start = ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreChecker_ReportStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = ReportStoreChecker_ReportStoreCheckerFilterDTO.AppUserId?.Equal;
            long? StoreId = ReportStoreChecker_ReportStoreCheckerFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreChecker_ReportStoreCheckerFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreChecker_ReportStoreCheckerFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreChecker_ReportStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreChecker_ReportStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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
                Skip = ReportStoreChecker_ReportStoreCheckerFilterDTO.Skip,
                Take = ReportStoreChecker_ReportStoreCheckerFilterDTO.Take,
                OrderBy = AppUserOrder.DisplayName,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => OrganizationIds.Contains(x.OrganizationId) &&
                (StoreId == null || x.Id == StoreId.Value) &&
                (StoreTypeId == null || x.StoreTypeId == StoreTypeId.Value) &&
                (StoreGroupingId == null || x.StoreGroupingId == StoreGroupingId.Value))
                .ToListAsync();

            List<ReportStoreChecker_SaleEmployeeDTO> ReportStoreChecker_SaleEmployeeDTOs = AppUsers.Select(au => new ReportStoreChecker_SaleEmployeeDTO
            {
                SaleEmployeeId = au.Id,
                Username = au.Username,
                DisplayName = au.DisplayName,
                OrganizationName = au.Organization?.Name,
            }).ToList();

            List<string> OrganizationNames = ReportStoreChecker_SaleEmployeeDTOs.Select(se => se.OrganizationName).Distinct().ToList();
            OrganizationNames = OrganizationNames.OrderBy(x => x).ToList();
            List<ReportStoreChecker_ReportStoreCheckerDTO> ReportStoreChecker_ReportStoreCheckerDTOs = OrganizationNames.Select(on => new ReportStoreChecker_ReportStoreCheckerDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportStoreChecker_ReportStoreCheckerDTO ReportStoreChecker_ReportStoreCheckerDTO in ReportStoreChecker_ReportStoreCheckerDTOs)
            {
                ReportStoreChecker_ReportStoreCheckerDTO.SaleEmployees = ReportStoreChecker_SaleEmployeeDTOs
                    .Where(se => se.OrganizationName == ReportStoreChecker_ReportStoreCheckerDTO.OrganizationName)
                    .ToList();
            }

            AppUserIds = AppUsers.Select(s => s.Id).ToList();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            foreach (var StoreCheckingDAO in StoreCheckingDAOs)
            {
                StoreDAO StoreDAO = StoreDAOs.Where(x => x.Id == StoreCheckingDAO.Id).FirstOrDefault();
                StoreCheckingDAO.Store = StoreDAO;
            }
            // khởi tạo khung dữ liệu
            foreach (ReportStoreChecker_SaleEmployeeDTO ReportStoreChecker_SaleEmployeeDTO in ReportStoreChecker_SaleEmployeeDTOs)
            {
                ReportStoreChecker_SaleEmployeeDTO.StoreCheckingGroupByDates = new List<ReportStoreChecker_StoreCheckingGroupByDateDTO>();
                for (DateTime i = Start; i < End; i = i.AddDays(1))
                {
                    ReportStoreChecker_StoreCheckingGroupByDateDTO ReportStoreChecker_StoreCheckingGroupByDateDTO = new ReportStoreChecker_StoreCheckingGroupByDateDTO();
                    ReportStoreChecker_StoreCheckingGroupByDateDTO.Date = i;
                    ReportStoreChecker_StoreCheckingGroupByDateDTO.StoreCheckings = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == ReportStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Where(x => x.CheckOutAt.Value.Date == i)
                        .Select(x => new ReportStoreChecker_StoreCheckingDTO
                        {
                            CheckIn = x.CheckInAt.Value,
                            CheckOut = x.CheckOutAt.Value,
                            Duaration = x.CheckOutAt.Value.AddSeconds(x.CheckInAt.Value.Second * -1),
                            DeviceName = x.DeviceName,
                            ImageCounter = x.ImageCounter ?? 0,
                            Planned = x.Planned,
                            SalesOrder = x.IndirectSalesOrderCounter > 0 ? true : false,
                            SaleEmployeeId = x.SaleEmployeeId,
                            StoreName = x.Store.Name,
                            StoreCode = x.Store.Code,
                            StoreAddress = x.Store.Address
                        }).ToList();
                    ReportStoreChecker_SaleEmployeeDTO.StoreCheckingGroupByDates.Add(ReportStoreChecker_StoreCheckingGroupByDateDTO);
                }
            }

            foreach (var ReportStoreChecker_ReportStoreCheckerDTO in ReportStoreChecker_ReportStoreCheckerDTOs)
            {
                foreach (var SaleEmployee in ReportStoreChecker_ReportStoreCheckerDTO.SaleEmployees)
                {
                    SaleEmployee.StoreCheckingGroupByDates = SaleEmployee.StoreCheckingGroupByDates.Where(x => x.StoreCheckings.Any()).ToList();
                }
                ReportStoreChecker_ReportStoreCheckerDTO.SaleEmployees = ReportStoreChecker_ReportStoreCheckerDTO.SaleEmployees.Where(x => x.StoreCheckingGroupByDates.Any()).ToList();
            }
            ReportStoreChecker_ReportStoreCheckerDTOs = ReportStoreChecker_ReportStoreCheckerDTOs.Where(x => x.SaleEmployees.Any()).ToList();
            return ReportStoreChecker_ReportStoreCheckerDTOs;
        }
    }
}
