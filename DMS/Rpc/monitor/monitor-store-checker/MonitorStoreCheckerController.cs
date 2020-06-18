using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Hangfire.Annotations;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreCheckerController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public MonitorStoreCheckerController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MonitorStoreCheckerRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorStoreChecker_AppUserDTO>> FilterListAppUser([FromBody] StoreCheckerMonitor_AppUserFilterDTO StoreCheckerMonitor_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = StoreCheckerMonitor_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreCheckerMonitor_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = StoreCheckerMonitor_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Id.In = await FilterAppUser();
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreChecker_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreChecker_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreCheckerRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreChecker_OrganizationDTO>> FilterListOrganization([FromBody] StoreCheckerMonitor_OrganizationFilterDTO StoreCheckerMonitor_OrganizationFilterDTO)
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
            OrganizationFilter.Id = StoreCheckerMonitor_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = StoreCheckerMonitor_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = StoreCheckerMonitor_OrganizationFilterDTO.Name;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization();
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<MonitorStoreChecker_OrganizationDTO> StoreCheckerMonitor_OrganizationDTOs = Organizations
                .Select(x => new MonitorStoreChecker_OrganizationDTO(x)).ToList();
            return StoreCheckerMonitor_OrganizationDTOs;
        }

        [Route(MonitorStoreCheckerRoute.FilterListChecking), HttpPost]
        public List<EnumList> FilterListChecking()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Chưa viếng thăm" });
            EnumList.Add(new EnumList { Id = 1, Name = "Đã viếng thăm" });
            return EnumList;
        }

        [Route(MonitorStoreCheckerRoute.FilterListImage), HttpPost]
        public List<EnumList> FilterListImage()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không hình ảnh" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có hình ảnh" });
            return EnumList;
        }

        [Route(MonitorStoreCheckerRoute.FilterListSalesOrder), HttpPost]
        public List<EnumList> FilterListSalesOrder()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không có đơn hàng" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có đơn hàng" });
            return EnumList;
        }

        [Route(MonitorStoreCheckerRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SaleEmployeeId?.Equal;

            List<long> OrganizationIds = await FilterOrganization();
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            int count = await DataContext.AppUser.Where(au =>
                au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
                (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value)
            ).CountAsync();
            return count;
        }

        [Route(MonitorStoreCheckerRoute.List), HttpPost]
        public async Task<List<MonitorStoreChecker_MonitorStoreCheckerDTO>> List([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SaleEmployeeId?.Equal;

            List<long> OrganizationIds = await FilterOrganization();
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au =>
               au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
               (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value)
            )
                .Include(au => au.Organization)
                .OrderBy(au => au.DisplayName)
                .Skip(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Skip)
                .Take(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Take)
                .ToListAsync();

            List<MonitorStoreChecker_SaleEmployeeDTO> MonitorStoreChecker_SaleEmployeeDTOs = AppUserDAOs.Select(au => new MonitorStoreChecker_SaleEmployeeDTO
            {
                SaleEmployeeId = au.Id,
                Username = au.Username,
                DisplayName = au.DisplayName,
                OrganizationName = au.Organization?.Name,
            }).ToList();

            List<string> OrganizationNames = MonitorStoreChecker_SaleEmployeeDTOs.Select(se => se.OrganizationName).Distinct().ToList();
            List<MonitorStoreChecker_MonitorStoreCheckerDTO> MonitorStoreChecker_MonitorStoreCheckerDTOs = OrganizationNames.Select(on => new MonitorStoreChecker_MonitorStoreCheckerDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (MonitorStoreChecker_MonitorStoreCheckerDTO MonitorStoreChecker_MonitorStoreCheckerDTO in MonitorStoreChecker_MonitorStoreCheckerDTOs)
            {
                MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees = MonitorStoreChecker_SaleEmployeeDTOs
                    .Where(se => se.OrganizationName == MonitorStoreChecker_MonitorStoreCheckerDTO.OrganizationName)
                    .ToList();
            }

            List<long> AppUserIds = AppUserDAOs.Select(s => s.Id).ToList();

            // khởi tạo khung dữ liệu
            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = new List<MonitorStoreChecker_StoreCheckingDTO>();
                for (DateTime i = Start; i < End; i = i.AddDays(1))
                {
                    MonitorStoreChecker_StoreCheckingDTO StoreCheckerMonitor_StoreCheckingDTO = new MonitorStoreChecker_StoreCheckingDTO();
                    StoreCheckerMonitor_StoreCheckingDTO.SaleEmployeeId = MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId;
                    StoreCheckerMonitor_StoreCheckingDTO.Date = i;
                    StoreCheckerMonitor_StoreCheckingDTO.Image = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.External = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.Internal = new HashSet<long>();
                    MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Add(StoreCheckerMonitor_StoreCheckingDTO);
                }
            }
            List<ERoutePerformanceDAO> ERoutePerformanceDAOs = await DataContext.ERoutePerformance.Where(ep => Start <= ep.Date && ep.Date <= End).ToListAsync();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder.Where(o => Start <= o.OrderDate && o.OrderDate <= End).ToListAsync();
            // khởi tạo kế hoạch
            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                for (DateTime i = Start; i < End; i = i.AddDays(1))
                {
                    MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings
                        .Where(s => s.Date == i).FirstOrDefault();
                    MonitorStoreChecker_StoreCheckingDTO.RevenueCounter = IndirectSalesOrderDAOs.Where(o => o.OrderDate.Date == i)
                         .Select(o => o.Total).DefaultIfEmpty(0).Sum();

                    List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                           .Where(s =>
                               s.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId &&
                               s.CheckOutAt.Value.Date == i
                           ).ToList();
                    List<long> StoreCheckingIds = ListChecked.Select(sc => sc.Id).ToList();

                    foreach (StoreCheckingDAO Checked in ListChecked)
                    {
                        MonitorStoreChecker_StoreCheckingDTO.PlanCounter = ERoutePerformanceDAOs
                            .Where(ep => ep.SaleEmployeeId == Checked.SaleEmployeeId && ep.Date == Checked.CheckInAt.Value.Date).Select(ep => ep.PlanCounter).FirstOrDefault();

                        if (Checked.Planned)
                            MonitorStoreChecker_StoreCheckingDTO.Internal.Add(Checked.StoreId);
                        else
                            MonitorStoreChecker_StoreCheckingDTO.External.Add(Checked.StoreId);
                        if (Checked.ImageCounter > 0)
                            MonitorStoreChecker_StoreCheckingDTO.Image.Add(Checked.StoreId);
                    }
                }
            }

            return MonitorStoreChecker_MonitorStoreCheckerDTOs;
        }

        [Route(MonitorStoreCheckerRoute.Get), HttpPost]
        public async Task<List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>> Get([FromBody] MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.Date;
            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.Where(sc => sc.CheckOutAt.HasValue && sc.CheckInAt.HasValue &&
                    sc.SaleEmployeeId == MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.SaleEmployeeId &&
                    Start <= sc.CheckInAt.Value && sc.CheckInAt.Value <= End)
                .Include(sc => sc.Store)
                .ToListAsync();
            List<long> StoreCheckingIds = StoreCheckingDAOs.Select(s => s.Id).ToList();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder.Where(i => i.StoreCheckingId.HasValue && StoreCheckingIds.Contains(i.StoreCheckingId.Value)).ToListAsync();
            List<ProblemDAO> ProblemDAOs = await DataContext.Problem.Where(p => p.StoreCheckingId.HasValue && StoreCheckingIds.Contains(p.StoreCheckingId.Value)).ToListAsync();
            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping.Where(sc => StoreCheckingIds.Contains(sc.StoreCheckingId))
                .Include(sc => sc.Image)
                .ToListAsync();

            List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO> MonitorStoreChecker_MonitorStoreCheckerDetailDTOs = new List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>();
            foreach (StoreCheckingDAO StoreCheckingDAO in StoreCheckingDAOs)
            {
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.StoreCheckingId.Value == StoreCheckingDAO.Id).ToList();
                List<ProblemDAO> CompetitorProblems = ProblemDAOs.Where(p => p.StoreCheckingId.Value == StoreCheckingDAO.Id && p.ProblemTypeId == ProblemTypeEnum.COMPETITOR.Id).ToList();
                List<ProblemDAO> StoreProblems = ProblemDAOs.Where(p => p.StoreCheckingId.Value == StoreCheckingDAO.Id && p.ProblemTypeId == ProblemTypeEnum.STORE.Id).ToList();
                List<StoreCheckingImageMappingDAO> SubStoreCheckingImageMappingDAOs = StoreCheckingImageMappingDAOs.Where(sc => sc.StoreCheckingId == StoreCheckingDAO.Id).ToList();

                int Max = 0;
                Max = SubIndirectSalesOrderDAOs.Count > Max ? SubIndirectSalesOrderDAOs.Count : Max;
                Max = CompetitorProblems.Count > Max ? CompetitorProblems.Count : Max;
                Max = StoreProblems.Count > Max ? StoreProblems.Count : Max;
                for (int i = 0; i < Max; i++)
                {
                    MonitorStoreChecker_MonitorStoreCheckerDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorStoreChecker_MonitorStoreCheckerDetailDTO
                    {
                        StoreCode = StoreCheckingDAO.Store.Code,
                        StoreName = StoreCheckingDAO.Store.Name,
                    };
                    if (i == 0)
                    {
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.ImagePath = SubStoreCheckingImageMappingDAOs.Select(i => i.Image.Url).FirstOrDefault();
                    }

                    MonitorStoreChecker_MonitorStoreCheckerDetailDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDetailDTO);
                    if (SubIndirectSalesOrderDAOs.Count > i)
                    {
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.IndirectSalesOrderCode = SubIndirectSalesOrderDAOs[i].Code;
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.Sales = SubIndirectSalesOrderDAOs[i].Total;
                    }
                    if (CompetitorProblems.Count > i)
                    {
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.CompetitorProblemId = CompetitorProblems[i].Id;
                    }
                    if (StoreProblems.Count > i)
                    {
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.StoreProblemId = StoreProblems[i].Id;
                    }
                }
            }
            return MonitorStoreChecker_MonitorStoreCheckerDetailDTOs;
        }

        private async Task<List<long>> FilterOrganization()
        {
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return new List<long>();

            List<long> In = new List<long>();
            List<long> NotIn = new List<long>();
            foreach (var currentFilter in CurrentContext.Filters)
            {

                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId))
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal != null)
                            In.Add(FilterPermissionDefinition.IdFilter.Equal.Value);
                        if (FilterPermissionDefinition.IdFilter.In != null)
                            In.AddRange(FilterPermissionDefinition.IdFilter.In);

                        if (FilterPermissionDefinition.IdFilter.NotEqual != null)
                            NotIn.Add(FilterPermissionDefinition.IdFilter.NotEqual.Value);
                        if (FilterPermissionDefinition.IdFilter.NotIn != null)
                            NotIn.AddRange(FilterPermissionDefinition.IdFilter.NotIn);
                    }
                }
            }
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });
            List<string> InPaths = Organizations.Where(o => In.Contains(o.Id)).Select(o => o.Path).ToList();
            List<string> NotInPaths = Organizations.Where(o => NotIn.Contains(o.Id)).Select(o => o.Path).ToList();
            Organizations = Organizations.Where(o => InPaths.Any(p => o.Path.StartsWith(p))).ToList();
            Organizations = Organizations.Where(o => !NotInPaths.Any(p => o.Path.StartsWith(p))).ToList();

            List<long> organizationIds = Organizations.Select(o => o.Id).ToList();

            return organizationIds;
        }
        private async Task<List<long>> FilterAppUser()
        {
            List<long> organizationIds = await FilterOrganization();
            List<AppUser> AppUsers = await AppUserService.List(new AppUserFilter
            {
                OrganizationId = new IdFilter { In = organizationIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id,
            });
            List<long> AppUserIds = AppUsers.Select(a => a.Id).ToList();
            return AppUserIds;
        }
    }
}
