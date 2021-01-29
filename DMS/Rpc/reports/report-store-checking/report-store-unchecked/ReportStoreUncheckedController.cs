using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreStatus;
using DMS.Services.MStoreType;
using Hangfire.Annotations;
using DMS.Helpers;
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
        private IStoreStatusService StoreStatusService;
        private ICurrentContext CurrentContext;
        public ReportStoreUncheckedController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IERouteService ERouteService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IStoreStatusService StoreStatusService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.ERouteService = ERouteService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.StoreStatusService = StoreStatusService;
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

        [Route(ReportStoreUncheckedRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportStoreUnchecked_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportStoreUnchecked_StoreStatusFilterDTO ReportStoreUnchecked_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportStoreUnchecked_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportStoreUnchecked_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportStoreUnchecked_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportStoreUnchecked_StoreStatusDTO> ReportStoreUnchecked_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportStoreUnchecked_StoreStatusDTO(x)).ToList();
            return ReportStoreUnchecked_StoreStatusDTOs;
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
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).TotalDays > 7)
                return 0;

            long? AppUserId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.AppUserId?.Equal;
            long? ERouteId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.ERouteId?.Equal;
            long? StoreStatusId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.StoreStatusId?.Equal;

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
            var query = from su in DataContext.StoreUnchecking
                        join s in DataContext.Store on su.StoreId equals s.Id
                        where AppUserIds.Contains(su.AppUserId) &&
                        OrganizationIds.Contains(su.OrganizationId) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        Start <= su.Date && su.Date <= End &&
                        s.DeletedAt == null
                        select new
                        {
                            AppUserId = su.AppUserId,
                            OrganizationId = su.OrganizationId
                        };
            return await query.Distinct().CountAsync();
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
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 7)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 7 ngày" });

            List<ReportStoreUnchecked_ReportStoreUncheckedDTO> ReportStoreUnchecked_ReportStoreUncheckedDTOs = await ListData(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO, Start, End);
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
                    ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Date.LessEqual.Value;

            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Skip = 0;
            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Take = int.MaxValue;
            List<ReportStoreUnchecked_ReportStoreUncheckedDTO> ReportStoreUnchecked_ReportStoreUncheckedDTOs = await ListData(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO,Start, End);

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

        private async Task<List<ReportStoreUnchecked_ReportStoreUncheckedDTO>> ListData(
            ReportStoreUnchecked_ReportStoreUncheckedFilterDTO ReportStoreUnchecked_ReportStoreUncheckedFilterDTO,
            DateTime Start, DateTime End)
        {
            long? AppUserId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.AppUserId?.Equal;
            long? ERouteId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.ERouteId?.Equal;
            long? StoreStatusId = ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.StoreStatusId?.Equal;

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
            var query = from su in DataContext.StoreUnchecking
                        join s in DataContext.Store on su.StoreId equals s.Id
                        join au in DataContext.AppUser on su.AppUserId equals au.Id
                        where AppUserIds.Contains(su.AppUserId) &&
                        OrganizationIds.Contains(su.OrganizationId) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        Start <= su.Date && su.Date <= End &&
                        s.DeletedAt == null && au.DeletedAt == null
                        select new
                        {
                            AppUserId = su.AppUserId,
                            OrganizationId = su.OrganizationId
                        };

            var Ids = await query
                 .Distinct()
                 .OrderBy(x => x.OrganizationId)
                 .Skip(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Skip)
                 .Take(ReportStoreUnchecked_ReportStoreUncheckedFilterDTO.Take)
                 .ToListAsync();
            AppUserIds = Ids.Select(x => x.AppUserId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => AppUserIds.Contains(x.Id))
                .Where(x => x.DeletedAt == null)
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.DisplayName)
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

            var storeUncheckingQuery = from su in DataContext.StoreUnchecking
                                       join s in DataContext.Store on su.StoreId equals s.Id
                                       join st in DataContext.StoreType on s.StoreTypeId equals st.Id
                                       join ss in DataContext.StoreStatus on s.StoreStatusId equals ss.Id
                                       where s.DeletedAt == null &&
                                       AppUserIds.Contains(su.AppUserId) &&
                                       OrganizationIds.Contains(su.OrganizationId) &&
                                       Start <= su.Date && su.Date <= End &&
                                       (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || su.Store.StoreStatusId == StoreStatusId.Value)
                                       select new StoreUncheckingDAO
                                       {
                                           Id = su.Id,
                                           AppUserId = su.AppUserId,
                                           Date = su.Date,
                                           OrganizationId = su.OrganizationId,
                                           StoreId = su.StoreId,
                                           Store = new StoreDAO
                                           {
                                               Id = s.Id,
                                               Code = s.Code,
                                               CodeDraft = s.CodeDraft,
                                               Name = s.Name,
                                               Address = s.Address,
                                               Telephone = s.Telephone,
                                               StoreStatusId = s.StoreStatusId,
                                               StoreTypeId = st.Id,
                                               StoreType = new StoreTypeDAO
                                               {
                                                   Code = st.Code,
                                                   Name = st.Name,
                                               },
                                               StoreStatus = new StoreStatusDAO
                                               {
                                                   Code = ss.Code,
                                                   Name = ss.Name,
                                               }
                                           }
                                       };
            List<StoreUncheckingDAO> StoreUncheckingDAOs = await storeUncheckingQuery.ToListAsync();

            List<ReportStoreUnchecked_ReportStoreUncheckedDTO> ReportStoreUnchecked_ReportStoreUncheckedDTOs = new List<ReportStoreUnchecked_ReportStoreUncheckedDTO>();
            foreach (var Organization in Organizations)
            {
                ReportStoreUnchecked_ReportStoreUncheckedDTO ReportStoreUnchecked_ReportStoreUncheckedDTO = new ReportStoreUnchecked_ReportStoreUncheckedDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<ReportStoreUnchecked_SaleEmployeeDTO>()
                };
                ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees = Ids
                    .Where(x => x.OrganizationId == Organization.Id)
                    .Select(x => new ReportStoreUnchecked_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.AppUserId,
                    }).ToList();
                ReportStoreUnchecked_ReportStoreUncheckedDTOs.Add(ReportStoreUnchecked_ReportStoreUncheckedDTO);
            }

            foreach (var ReportStoreUnchecked_ReportStoreUncheckedDTO in ReportStoreUnchecked_ReportStoreUncheckedDTOs)
            {
                Parallel.ForEach(ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees, SaleEmployee =>
                {
                    var Employee = AppUserDAOs.Where(x => x.Id == SaleEmployee.SaleEmployeeId).FirstOrDefault();
                    if (Employee != null)
                    {
                        SaleEmployee.Username = Employee.Username;
                        SaleEmployee.DisplayName = Employee.DisplayName;
                    }
                });

                Parallel.ForEach(ReportStoreUnchecked_ReportStoreUncheckedDTO.SaleEmployees, SaleEmployee =>
                {
                    for (DateTime index = Start; index <= End; index = index.AddDays(1))
                    {
                        var StoreUncheckings = StoreUncheckingDAOs.Where(e => e.AppUserId == SaleEmployee.SaleEmployeeId && index <= e.Date && e.Date < index.AddDays(1))
                            .Select(x => new ReportStoreUnchecked_StoreDTO
                            {
                                Date = x.Date.AddHours(CurrentContext.TimeZone),
                                AppUserId = x.AppUserId,
                                StoreAddress = x.Store.Address,
                                StoreCode = x.Store.Code,
                                StoreCodeDraft = x.Store.CodeDraft,
                                StoreName = x.Store.Name,
                                StoreStatusName = x.Store.StoreStatus.Name,
                                StorePhone = x.Store.Telephone,
                                StoreTypeName = x.Store.StoreType.Name,
                            })
                            .Distinct()
                            .ToList();
                        if (SaleEmployee.Stores == null)
                        {
                            SaleEmployee.Stores = new List<ReportStoreUnchecked_StoreDTO>();
                        }
                        SaleEmployee.Stores.AddRange(StoreUncheckings);
                    }
                });
            }

            return ReportStoreUnchecked_ReportStoreUncheckedDTOs;
        }

    }
}
