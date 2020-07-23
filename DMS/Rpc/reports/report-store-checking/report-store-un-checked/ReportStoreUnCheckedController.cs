//using Common;
//using DMS.Entities;
//using DMS.Enums;
//using DMS.Models;
//using DMS.Services.MAppUser;
//using DMS.Services.MERoute;
//using DMS.Services.MOrganization;
//using DMS.Services.MStore;
//using DMS.Services.MStoreGrouping;
//using DMS.Services.MStoreType;
//using Hangfire.Annotations;
//using Helpers;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DMS.Rpc.reports.report_store_checking.report_store_un_checked
//{
//    public class ReportStoreUnCheckedController : RpcController
//    {
//        private DataContext DataContext;
//        private IAppUserService AppUserService;
//        private IOrganizationService OrganizationService;
//        private IERouteService ERouteService;
//        private IStoreService StoreService;
//        private IStoreGroupingService StoreGroupingService;
//        private IStoreTypeService StoreTypeService;
//        private ICurrentContext CurrentContext;
//        public ReportStoreUnCheckedController(
//            DataContext DataContext,
//            IOrganizationService OrganizationService,
//            IAppUserService AppUserService,
//            IERouteService ERouteService,
//            IStoreService StoreService,
//            IStoreGroupingService StoreGroupingService,
//            IStoreTypeService StoreTypeService,
//            ICurrentContext CurrentContext)
//        {
//            this.DataContext = DataContext;
//            this.AppUserService = AppUserService;
//            this.OrganizationService = OrganizationService;
//            this.ERouteService = ERouteService;
//            this.StoreService = StoreService;
//            this.StoreGroupingService = StoreGroupingService;
//            this.StoreTypeService = StoreTypeService;
//            this.CurrentContext = CurrentContext;
//        }

//        #region Filter List
//        [Route(ReportStoreUnCheckedRoute.FilterListAppUser), HttpPost]
//        public async Task<List<ReportStoreUnChecked_AppUserDTO>> FilterListAppUser([FromBody] ReportStoreUnChecked_AppUserFilterDTO ReportStoreUnChecked_AppUserFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            AppUserFilter AppUserFilter = new AppUserFilter();
//            AppUserFilter.Skip = 0;
//            AppUserFilter.Take = 20;
//            AppUserFilter.OrderBy = AppUserOrder.Id;
//            AppUserFilter.OrderType = OrderType.ASC;
//            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
//            AppUserFilter.Id = ReportStoreUnChecked_AppUserFilterDTO.Id;
//            AppUserFilter.Username = ReportStoreUnChecked_AppUserFilterDTO.Username;
//            AppUserFilter.DisplayName = ReportStoreUnChecked_AppUserFilterDTO.DisplayName;
//            AppUserFilter.OrganizationId = ReportStoreUnChecked_AppUserFilterDTO.OrganizationId;
//            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

//            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
//            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

//            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
//            List<ReportStoreUnChecked_AppUserDTO> ReportStoreUnChecked_AppUserDTOs = AppUsers
//                .Select(x => new ReportStoreUnChecked_AppUserDTO(x)).ToList();
//            return ReportStoreUnChecked_AppUserDTOs;
//        }

//        [Route(ReportStoreUnCheckedRoute.FilterListOrganization), HttpPost]
//        public async Task<List<ReportStoreUnChecked_OrganizationDTO>> FilterListOrganization()
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            OrganizationFilter OrganizationFilter = new OrganizationFilter();
//            OrganizationFilter.Skip = 0;
//            OrganizationFilter.Take = int.MaxValue;
//            OrganizationFilter.OrderBy = OrganizationOrder.Id;
//            OrganizationFilter.OrderType = OrderType.ASC;
//            OrganizationFilter.Selects = OrganizationSelect.ALL;
//            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

//            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
//            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);
//            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

//            List<ReportStoreUnChecked_OrganizationDTO> ReportStoreUnChecked_OrganizationDTOs = Organizations
//                .Select(x => new ReportStoreUnChecked_OrganizationDTO(x)).ToList();
//            return ReportStoreUnChecked_OrganizationDTOs;
//        }

//        [Route(ReportStoreUnCheckedRoute.FilterListERoute), HttpPost]
//        public async Task<List<ReportStoreUnChecked_ERouteDTO>> FilterListERoute([FromBody] ReportStoreUnChecked_ERouteFilterDTO ReportStoreUnChecked_ERouteFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            ERouteFilter ERouteFilter = new ERouteFilter();
//            ERouteFilter.Skip = 0;
//            ERouteFilter.Take = 20;
//            ERouteFilter.OrderBy = ERouteOrder.Id;
//            ERouteFilter.OrderType = OrderType.ASC;
//            ERouteFilter.Selects = ERouteSelect.ALL;
//            ERouteFilter.Id = ReportStoreUnChecked_ERouteFilterDTO.Id;
//            ERouteFilter.Code = ReportStoreUnChecked_ERouteFilterDTO.Code;
//            ERouteFilter.Name = ReportStoreUnChecked_ERouteFilterDTO.Name;
//            ERouteFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

//            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);

//            List<ReportStoreUnChecked_ERouteDTO> ReportStoreUnChecked_ERouteDTOs = ERoutes
//                .Select(x => new ReportStoreUnChecked_ERouteDTO(x)).ToList();
//            return ReportStoreUnChecked_ERouteDTOs;
//        }
//        #endregion

//        [Route(ReportStoreUnCheckedRoute.Count), HttpPost]
//        public async Task<int> Count([FromBody] ReportStoreChecked_ReportStoreCheckedFilterDTO ReportStoreChecked_ReportStoreCheckedFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            if (ReportStoreChecked_ReportStoreCheckedFilterDTO.HasValue == false)
//                return 0;

//            DateTime Start = ReportStoreChecked_ReportStoreCheckedFilterDTO.Date?.GreaterEqual == null ?
//                    StaticParams.DateTimeNow :
//                    ReportStoreChecked_ReportStoreCheckedFilterDTO.Date.GreaterEqual.Value;

//            DateTime End = ReportStoreChecked_ReportStoreCheckedFilterDTO.Date?.LessEqual == null ?
//                    StaticParams.DateTimeNow :
//                    ReportStoreChecked_ReportStoreCheckedFilterDTO.Date.LessEqual.Value;

//            Start = new DateTime(Start.Year, Start.Month, Start.Day);
//            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

//            long? SaleEmployeeId = ReportStoreChecked_ReportStoreCheckedFilterDTO.AppUserId?.Equal;
//            long? ERouteId = ReportStoreChecked_ReportStoreCheckedFilterDTO.ERouteId?.Equal;

//            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
//            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
//            OrganizationDAO OrganizationDAO = null;
//            if (ReportStoreChecked_ReportStoreCheckedFilterDTO.OrganizationId?.Equal != null)
//            {
//                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreChecked_ReportStoreCheckedFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
//                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
//            }
//            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

//            var query = from e in DataContext.ERoute
//                        join ec in DataContext.ERouteContent on e.Id equals ec.ERouteId
//                        join s in DataContext.Store on ec.StoreId equals s.Id
//                        join au in DataContext.AppUser on e.SaleEmployeeId equals au.Id
//                        where e.StartDate >= Start && e.EndDate <= End &&
//                        (SaleEmployeeId.HasValue == false || e.SaleEmployeeId == SaleEmployeeId.Value) &&
//                        (ERouteId.HasValue == false || ec.ERouteId == ERouteId.Value) &&
//                        (au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
//                        select au;

//            int count = await query.Distinct().CountAsync();
//            return count;
//        }

//        [Route(ReportStoreUnCheckedRoute.List), HttpPost]
//        public async Task<List<ReportStoreUnChecked_ReportStoreUnCheckedDTO>> List([FromBody] ReportStoreChecked_ReportStoreCheckedFilterDTO ReportStoreChecker_ReportStoreCheckedFilterDTO)
//        {
//            if (!ModelState.IsValid)
//                throw new BindException(ModelState);

//            if (ReportStoreChecker_ReportStoreCheckedFilterDTO.HasValue == false)
//                return new List<ReportStoreUnChecked_ReportStoreUnCheckedDTO>();

//            DateTime Start = ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn?.GreaterEqual == null ?
//                    StaticParams.DateTimeNow :
//                    ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn.GreaterEqual.Value;

//            DateTime End = ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn?.LessEqual == null ?
//                    StaticParams.DateTimeNow :
//                    ReportStoreChecker_ReportStoreCheckedFilterDTO.CheckIn.LessEqual.Value;

//            Start = new DateTime(Start.Year, Start.Month, Start.Day);
//            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

//            long? SaleEmployeeId = ReportStoreChecker_ReportStoreCheckedFilterDTO.AppUserId?.Equal;
//            long? StoreId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreId?.Equal;
//            long? StoreTypeId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreTypeId?.Equal;
//            long? StoreGroupingId = ReportStoreChecker_ReportStoreCheckedFilterDTO.StoreGroupingId?.Equal;

//            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
//            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
//            OrganizationDAO OrganizationDAO = null;
//            if (ReportStoreChecker_ReportStoreCheckedFilterDTO.OrganizationId?.Equal != null)
//            {
//                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreChecker_ReportStoreCheckedFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
//                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
//            }
//            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

//            var query = from sc in DataContext.StoreChecking
//                        join s in DataContext.Store on sc.StoreId equals s.Id
//                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
//                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
//                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
//                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
//                        (StoreTypeId.HasValue == false || s.StoreTypeId == StoreTypeId.Value) &&
//                        (StoreGroupingId.HasValue == false || s.StoreGroupingId == StoreGroupingId.Value) &&
//                        (au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
//                        select au;

//            var AppUserIds = await query.Select(x => x.Id).Distinct().ToListAsync();
            
//            AppUserFilter AppUserFilter = new AppUserFilter
//            {
//                Id = new IdFilter { In = AppUserIds },
//                OrganizationId = new IdFilter { In = OrganizationIds },
//                Skip = ReportStoreChecker_ReportStoreCheckedFilterDTO.Skip,
//                Take = ReportStoreChecker_ReportStoreCheckedFilterDTO.Take,
//                OrderBy = AppUserOrder.DisplayName,
//                Selects = AppUserSelect.ALL
//            };
//            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

//            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(x => OrganizationIds.Contains(x.OrganizationId) &&
//                (StoreId == null || x.Id == StoreId.Value) &&
//                (StoreTypeId == null || x.StoreTypeId == StoreTypeId.Value) &&
//                (StoreGroupingId == null || x.StoreGroupingId == StoreGroupingId.Value))
//                .ToListAsync();

//            List<ReportStoreUnChecked_SaleEmployeeDTO> ReportStoreChecked_SaleEmployeeDTOs = AppUsers.Select(au => new ReportStoreUnChecked_SaleEmployeeDTO
//            {
//                SaleEmployeeId = au.Id,
//                Username = au.Username,
//                DisplayName = au.DisplayName,
//                OrganizationName = au.Organization?.Name,
//            }).ToList();

//            List<string> OrganizationNames = ReportStoreChecked_SaleEmployeeDTOs.Select(se => se.OrganizationName).Distinct().ToList();
//            OrganizationNames = OrganizationNames.OrderBy(x => x).ToList();
//            List<ReportStoreUnChecked_ReportStoreUnCheckedDTO> ReportStoreChecked_ReportStoreCheckedDTOs = OrganizationNames.Select(on => new ReportStoreUnChecked_ReportStoreUnCheckedDTO
//            {
//                OrganizationName = on,
//            }).ToList();
//            foreach (ReportStoreUnChecked_ReportStoreUnCheckedDTO ReportStoreChecked_ReportStoreCheckedDTO in ReportStoreChecked_ReportStoreCheckedDTOs)
//            {
//                ReportStoreChecked_ReportStoreCheckedDTO.SaleEmployees = ReportStoreChecked_SaleEmployeeDTOs
//                    .Where(se => se.OrganizationName == ReportStoreChecked_ReportStoreCheckedDTO.OrganizationName)
//                    .ToList();
//            }

//            AppUserIds = AppUsers.Select(s => s.Id).ToList();
//            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
//                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
//                .ToListAsync();
//            foreach (var StoreCheckingDAO in StoreCheckingDAOs)
//            {
//                StoreDAO StoreDAO = StoreDAOs.Where(x => x.Id == StoreCheckingDAO.Id).FirstOrDefault();
//                StoreCheckingDAO.Store = StoreDAO;
//            }
//            // khởi tạo khung dữ liệu
//            foreach (ReportStoreUnChecked_SaleEmployeeDTO ReportStoreChecked_SaleEmployeeDTO in ReportStoreChecked_SaleEmployeeDTOs)
//            {
//                ReportStoreChecked_SaleEmployeeDTO.StoreCheckingGroupByDates = new List<ReportStoreChecked_StoreCheckingGroupByDateDTO>();
//                for (DateTime i = Start; i < End; i = i.AddDays(1))
//                {
//                    ReportStoreChecked_StoreCheckingGroupByDateDTO ReportStoreChecked_StoreCheckingGroupByDateDTO = new ReportStoreChecked_StoreCheckingGroupByDateDTO();
//                    ReportStoreChecked_StoreCheckingGroupByDateDTO.Date = i;
//                    ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == ReportStoreChecked_SaleEmployeeDTO.SaleEmployeeId)
//                        .Where(x => x.CheckOutAt.Value.Date == i)
//                        .Select(x => new ReportStoreChecked_StoreCheckingDTO
//                        {
//                            CheckIn = x.CheckInAt.Value,
//                            CheckOut = x.CheckOutAt.Value,
//                            DeviceName = x.DeviceName,
//                            ImageCounter = x.ImageCounter ?? 0,
//                            Planned = x.Planned,
//                            SalesOrder = x.IndirectSalesOrderCounter > 0 ? true : false,
//                            SaleEmployeeId = x.SaleEmployeeId,
//                            StoreName = x.Store.Name,
//                            StoreCode = x.Store.Code,
//                            StoreAddress = x.Store.Address
//                        }).ToList();
//                    foreach (var StoreChecking in ReportStoreChecked_StoreCheckingGroupByDateDTO.StoreCheckings)
//                    {
//                        var TotalMinuteChecking = StoreChecking.CheckOut.Subtract(StoreChecking.CheckIn).Minutes;
//                        StoreChecking.Duaration = $"{TotalMinuteChecking / 60} : {TotalMinuteChecking % 60}";
//                    }
//                    ReportStoreChecked_SaleEmployeeDTO.StoreCheckingGroupByDates.Add(ReportStoreChecked_StoreCheckingGroupByDateDTO);
//                }
//            }

//            foreach (var ReportStoreChecked_ReportStoreCheckerDTO in ReportStoreChecked_ReportStoreCheckedDTOs)
//            {
//                foreach (var SaleEmployee in ReportStoreChecked_ReportStoreCheckerDTO.SaleEmployees)
//                {
//                    SaleEmployee.StoreCheckingGroupByDates = SaleEmployee.StoreCheckingGroupByDates.Where(x => x.StoreCheckings.Any()).ToList();
//                }
//                ReportStoreChecked_ReportStoreCheckerDTO.SaleEmployees = ReportStoreChecked_ReportStoreCheckerDTO.SaleEmployees.Where(x => x.StoreCheckingGroupByDates.Any()).ToList();
//            }
//            ReportStoreChecked_ReportStoreCheckedDTOs = ReportStoreChecked_ReportStoreCheckedDTOs.Where(x => x.SaleEmployees.Any()).ToList();
//            return ReportStoreChecked_ReportStoreCheckedDTOs;
//        }
//    }
//}
