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
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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
                  LocalStartDay(CurrentContext) :
                  ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value.AddDays(1).AddSeconds(-1);

            if (End.Subtract(Start).TotalDays > 7)
                return 0;

            long? AppUserId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.AppUserId?.Equal;
            long? ERouteId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.ERouteId?.Equal;

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            if (AppUserId.HasValue)
                AppUserIds = AppUserIds.Where(x => x == AppUserId.Value).ToList();
            AppUserIds = await (from su in DataContext.StoreUnchecking
                                join a in DataContext.AppUser on su.AppUserId equals a.Id
                                where AppUserIds.Contains(a.Id) &&
                                (a.OrganizationId.HasValue && OrganizationIds.Contains(a.OrganizationId.Value))
                                orderby a.Organization.Name, a.DisplayName
                                select su.AppUserId)
                                .Distinct()
                                .ToListAsync();
            int count = AppUserIds.Count();
            return count;
        }

        [Route(ReportStoreUncheckedRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportStoreUnchecked_ReportStoreUncheckedDTO>>> List([FromBody] ReportStoreUnchecked_ReportStoreUncheckedFilterDTO ReportStoreUnchecked_ReportStoreUncheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.HasValue == false)
                return new List<ReportStoreUnchecked_ReportStoreUncheckedDTO>();

            DateTime Start = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.GreaterEqual == null ?
                  LocalStartDay(CurrentContext) :
                  ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value.AddDays(1).AddSeconds(-1);

            long? AppUserId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.AppUserId?.Equal;
            long? ERouteId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.ERouteId?.Equal;

            List<long> AppUserIds, OrganizationIds;
            (AppUserIds, OrganizationIds) = await FilterOrganizationAndUser(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId,
                AppUserService, OrganizationService, CurrentContext, DataContext);

            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            if (AppUserId.HasValue)
                AppUserIds = AppUserIds.Where(x => x == AppUserId.Value).ToList();
            AppUserIds = (await DataContext.StoreUnchecking.Where(su => AppUserIds.Contains(su.AppUserId))
                                .Select(su => su.AppUser)
                                .Distinct()
                                .OrderBy(su => su.Organization.Path).ThenBy(su => su.DisplayName)
                                .Skip(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Skip)
                                .Take(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Take)
                                .ToListAsync())
                                .Select(x => x.Id)
                                .ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au => AppUserIds.Contains(au.Id))
                .Where(x => x.OrganizationId.HasValue && OrganizationIds.Contains(x.OrganizationId.Value))
                .Include(au => au.Organization)
                .OrderBy(su => su.Organization.Path)
                .ToListAsync();
            List<StoreUncheckingDAO> StoreUncheckingDAOs = await DataContext.StoreUnchecking
                .Where(su =>
                    AppUserIds.Contains(su.AppUserId) &&
                    Start <= su.Date && su.Date <= End
                    )
                .Include(su => su.Store.StoreType)
                .ToListAsync();

            var  OrganizationNames = AppUserDAOs.Select(x => new { Path = x.Organization.Path, Name = x.Organization.Name }).Distinct().ToList();
            List<ReportStoreUnchecked_ReportStoreUncheckedDTO> ReportStoreUnchecked_ReportStoreUncheckedDTOs = OrganizationNames.Select(x => new ReportStoreUnchecked_ReportStoreUncheckedDTO
            {
                OrganizationName = x.Name,
                OrganizationPath = x.Path,
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

                for (DateTime index = Start; index < End; index = index.AddDays(1))
                {
                    var ReportStoreUnchecked_StoreDTOs = StoreUncheckingDAOs.Where(e => e.AppUserId == AppUserDAO.Id && e.Date == index)
                        .Select(x => new ReportStoreUnchecked_StoreDTO
                        {
                            Date = x.Date,
                            AppUserId = x.AppUserId,
                            StoreAddress = x.Store.Address,
                            StoreCode = x.Store.Code,
                            StoreName = x.Store.Name,
                            StorePhone = x.Store.OwnerPhone,
                            StoreTypeName = x.Store.StoreType.Name,
                        })
                        .Distinct()
                        .ToList();
                    ReportStoreUnChecked_SaleEmployeeDTO.Stores.AddRange(ReportStoreUnchecked_StoreDTOs);
                }
                ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees.Add(ReportStoreUnChecked_SaleEmployeeDTO);
            }
            foreach (var ReportStoreUnchecked_ReportStoreUncheckedDTO in ReportStoreUnchecked_ReportStoreUncheckedDTOs)
            {
                ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees = ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees.Where(x => x.Stores.Any()).ToList();
            }
            ReportStoreUnchecked_ReportStoreUncheckedDTOs = ReportStoreUnchecked_ReportStoreUncheckedDTOs.Where(x => x.SaleEmployees.Any()).ToList();
            return ReportStoreUnchecked_ReportStoreUncheckedDTOs;
        }

        [Route(ReportStoreUncheckedRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportStoreUnchecked_ReportStoreUncheckedFilterDTO ReportStoreUnchecked_ReportStoreUncheckedFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Skip = 0;
            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Take = int.MaxValue;

            DateTime Start = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.GreaterEqual == null ?
                  LocalStartDay(CurrentContext) :
                  ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value.AddDays(1).AddSeconds(-1);

            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Skip = 0;
            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Take = int.MaxValue;
            List<ReportStoreUnchecked_ReportStoreUncheckedDTO> ReportStoreUnchecked_ReportStoreUncheckedDTOs = (await List(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO)).Value;

            long stt = 1;
            foreach (ReportStoreUnchecked_ReportStoreUncheckedDTO ReportStoreUnchecked_ReportStoreUncheckedDTO in ReportStoreUnchecked_ReportStoreUncheckedDTOs)
            {
                foreach (var SaleEmployee in ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees)
                {
                    foreach (var Store in SaleEmployee.Stores)
                    {
                        Store.STT = stt;
                        stt++;
                    }
                }
            }

            string path = "Templates/Report_Store_Unchecked.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportStoreUncheckeds = ReportStoreUnchecked_ReportStoreUncheckedDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportStoreUnchecked.xlsx");
        }
    }
}
