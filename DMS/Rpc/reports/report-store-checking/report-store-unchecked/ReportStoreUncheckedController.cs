using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
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

namespace DMS.Rpc.reports.report_store_checking.report_store_unchecked
{
    public class ReportStoreUncheckedController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IERouteService ERouteService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportStoreUncheckedController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IERouteService ERouteService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.ERouteService = ERouteService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportStoreUncheckedRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportStoreUnChecked_AppUserDTO>> FilterListAppUser([FromBody] ReportStoreUnChecked_AppUserFilterDTO ReportStoreUnChecked_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportStoreUnChecked_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportStoreUnChecked_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportStoreUnChecked_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportStoreUnChecked_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportStoreUnChecked_AppUserDTO> ReportStoreUnChecked_AppUserDTOs = AppUsers
                .Select(x => new ReportStoreUnChecked_AppUserDTO(x)).ToList();
            return ReportStoreUnChecked_AppUserDTOs;
        }

        [Route(ReportStoreUncheckedRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStoreUnchecked_OrganizationDTO>> FilterListOrganization()
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

            List<ReportStoreUnchecked_OrganizationDTO> ReportStoreUnChecked_OrganizationDTOs = Organizations
                .Select(x => new ReportStoreUnchecked_OrganizationDTO(x)).ToList();
            return ReportStoreUnChecked_OrganizationDTOs;
        }

        [Route(ReportStoreUncheckedRoute.FilterListERoute), HttpPost]
        public async Task<List<ReportStoreUnChecked_ERouteDTO>> FilterListERoute([FromBody] ReportStoreUnChecked_ERouteFilterDTO ReportStoreUnChecked_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter.Skip = 0;
            ERouteFilter.Take = 20;
            ERouteFilter.OrderBy = ERouteOrder.Id;
            ERouteFilter.OrderType = OrderType.ASC;
            ERouteFilter.Selects = ERouteSelect.ALL;
            ERouteFilter.Id = ReportStoreUnChecked_ERouteFilterDTO.Id;
            ERouteFilter.Code = ReportStoreUnChecked_ERouteFilterDTO.Code;
            ERouteFilter.Name = ReportStoreUnChecked_ERouteFilterDTO.Name;
            ERouteFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);

            List<ReportStoreUnChecked_ERouteDTO> ReportStoreUnChecked_ERouteDTOs = ERoutes
                .Select(x => new ReportStoreUnChecked_ERouteDTO(x)).ToList();
            return ReportStoreUnChecked_ERouteDTOs;
        }
        #endregion

        [Route(ReportStoreUncheckedRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStoreUnchecked_ReportStoreUncheckedFilterDTO ReportStoreUnchecked_ReportStoreUncheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.GreaterEqual == null ?
                  StaticParams.DateTimeNow.Date :
                  ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.GreaterEqual.Value.Date;

            DateTime End = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            if (End.Subtract(Start).TotalDays > 7)
                return 0;

            long? AppUserId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.AppUserId?.Equal;
            long? ERouteId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.ERouteId?.Equal;

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);
            if (AppUserId.HasValue)
                AppUserIds = AppUserIds.Where(x => x == AppUserId.Value).ToList();
            int count = AppUserIds.Count();
            return count;
        }

        [Route(ReportStoreUncheckedRoute.List), HttpPost]
        public async Task<List<ReportStoreUnchecked_ReportStoreUncheckedDTO>> List([FromBody] ReportStoreUnchecked_ReportStoreUncheckedFilterDTO ReportStoreUnchecked_ReportStoreUncheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.HasValue == false)
                return new List<ReportStoreUnchecked_ReportStoreUncheckedDTO>();

            DateTime Start = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.GreaterEqual == null ?
                 StaticParams.DateTimeNow.Date :
                 ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.GreaterEqual.Value.Date;

            DateTime End = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).TotalDays > 7)
                return new List<ReportStoreUnchecked_ReportStoreUncheckedDTO>();

            long? AppUserId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.AppUserId?.Equal;
            long? ERouteId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.ERouteId?.Equal;

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            if (AppUserId.HasValue)
                AppUserIds = AppUserIds.Where(x => x == AppUserId.Value).ToList();

            AppUserIds = AppUserIds
                .Skip(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Skip)
                .Take(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Take)
                .ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au => AppUserIds.Contains(au.Id))
                .Include(x => x.Organization)
                .ToListAsync();
            List<ERouteDAO> ERouteDAOs = await DataContext.ERoute.Where(e =>
                AppUserIds.Contains(e.SaleEmployeeId) &&
                Start <= e.EndDate && e.StartDate <= End)
                .Include(e => e.ERouteContents)
                    .ThenInclude(ec => ec.ERouteContentDays)
                .ToListAsync();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.Where(sc =>
                sc.CheckOutAt.HasValue &&
                Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            List<long> StoreIds = ERouteDAOs.SelectMany(e => e.ERouteContents.Select(ec => ec.StoreId)).ToList();
            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(s => StoreIds.Contains(s.Id))
                .Include(s => s.StoreType)
                .ToListAsync();

            List<string> OrganizationNames = AppUserDAOs.Select(x => x.Organization.Name).Distinct().ToList();
            List<ReportStoreUnchecked_ReportStoreUncheckedDTO> ReportStoreUnchecked_ReportStoreUncheckedDTOs = OrganizationNames.Select(x => new ReportStoreUnchecked_ReportStoreUncheckedDTO
            {
                OrganizationName = x,
                SaleEmployees = new List<ReportStoreUnchecked_SaleEmployeeDTO>()
            }).ToList();
            foreach (AppUserDAO AppUserDAO in AppUserDAOs)
            {
                ReportStoreUnchecked_ReportStoreUncheckedDTO ReportStoreUnchecked_ReportStoreUncheckedDTO = ReportStoreUnchecked_ReportStoreUncheckedDTOs
                    .Where(x => x.OrganizationName == AppUserDAO.Organization.Name)
                    .FirstOrDefault();
                ReportStoreUnchecked_SaleEmployeeDTO ReportStoreUnChecked_SaleEmployeeDTO = new ReportStoreUnchecked_SaleEmployeeDTO
                {
                    SaleEmployeeId = AppUserDAO.Id,
                    Username = AppUserDAO.Username,
                    DisplayName = AppUserDAO.DisplayName,
                    Stores = new List<ReportStoreUnchecked_StoreDTO>()
                };

                DateTime Now = StaticParams.DateTimeNow.Date;
                for (DateTime index = Start; index < End; index = index.AddDays(1))
                {
                    var SubERouteDAOs = ERouteDAOs.Where(e => e.SaleEmployeeId == AppUserDAO.Id).ToList();
                    foreach (var ERouteDAO in SubERouteDAOs)
                    {
                        foreach (var ERouteContentDAO in ERouteDAO.ERouteContents)
                        {
                            var gap = (Now - ERouteDAO.RealStartDate).Days % 28;
                            if (ERouteContentDAO.ERouteContentDays.ElementAt(gap).Planned == true)
                            {
                                if (!StoreCheckingDAOs.Any(sc => sc.StoreId == ERouteContentDAO.StoreId))
                                {
                                    StoreDAO StoreDAO = StoreDAOs.Where(s => s.Id == ERouteContentDAO.StoreId).FirstOrDefault();
                                    ReportStoreUnchecked_StoreDTO ReportStoreUnchecked_StoreDTO = new ReportStoreUnchecked_StoreDTO
                                    {
                                        Date = index,
                                        StoreAddress = StoreDAO.Address,
                                        StoreCode = StoreDAO.Code,
                                        StoreName = StoreDAO.Name,
                                        StorePhone = StoreDAO.OwnerPhone,
                                        StoreTypeName = StoreDAO.StoreType.Name,
                                    };
                                    ReportStoreUnChecked_SaleEmployeeDTO.Stores.Add(ReportStoreUnchecked_StoreDTO);
                                }
                            }
                        }
                    }
                }
                ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees.Add(ReportStoreUnChecked_SaleEmployeeDTO);
            }
            return ReportStoreUnchecked_ReportStoreUncheckedDTOs;
        }
    }
}
