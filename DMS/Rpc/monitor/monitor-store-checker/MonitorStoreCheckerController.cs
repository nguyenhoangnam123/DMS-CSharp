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
        public MonitorStoreCheckerController(DataContext DataContext, IOrganizationService OrganizationService, IAppUserService AppUserService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
        }

        [Route(MonitorStoreCheckerRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorStoreChecker_AppUserDTO>> FilterListAppUser([FromBody] StoreCheckerMonitor_AppUserFilterDTO StoreCheckerMonitor_AppUserFilterDTO)
        {
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

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreChecker_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreChecker_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreCheckerRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreChecker_OrganizationDTO>> FilterListOrganization([FromBody] StoreCheckerMonitor_OrganizationFilterDTO StoreCheckerMonitor_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

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
            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
                   StaticParams.DateTimeNow :
                   MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;
            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            OrganizationDAO OrganizationDAO = null;
            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SaleEmployeeId?.Equal;
            long? Image = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image?.Equal;
            long? SalesOrder = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder?.Equal;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefault();
            }
            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value) &&
                        (
                            Image == null ||
                            (Image.Value == 0 && sc.ImageCounter == 0) ||
                            (Image.Value == 1 && sc.ImageCounter > 0)
                        ) &&
                        (
                            SalesOrder == null ||
                            (SalesOrder.Value == 0 && sc.IndirectSalesOrderCounter == 0) ||
                            (SalesOrder.Value == 1 && sc.IndirectSalesOrderCounter > 0)
                        )
                        select ap.Id;
            return await query.Distinct().CountAsync();
        }

        [Route(MonitorStoreCheckerRoute.List), HttpPost]
        public async Task<List<MonitorStoreChecker_MonitorStoreCheckerDTO>> List([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End
                        select new MonitorStoreChecker_SaleEmployeeDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                        };

            List<MonitorStoreChecker_SaleEmployeeDTO> MonitorStoreChecker_SaleEmployeeDTOs = await query.Distinct().OrderBy(q => q.DisplayName)
                .Skip(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Skip)
                .Take(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Take)
                .ToListAsync();
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

            List<long> AppUserIds = MonitorStoreChecker_SaleEmployeeDTOs.Select(s => s.SaleEmployeeId).ToList();

            List<StoreCheckingDAO> StoreCheckingDAOs = new List<StoreCheckingDAO>();
            var StoreCheckingQuery = DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End);
            // khong check
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking?.Equal == null || MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking?.Equal.Value == 1)
            {
                if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image?.Equal != null)
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image.Equal == 0)
                    {
                        StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.ImageCounter == 0);
                    }
                    else
                    {
                        StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.ImageCounter > 0);
                    }

                if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder?.Equal != null)
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder.Equal == 0)
                    {
                        StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.IndirectSalesOrderCounter == 0);
                    }
                    else
                    {
                        StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.IndirectSalesOrderCounter > 0);
                    }
                StoreCheckingDAOs = await StoreCheckingQuery.ToListAsync();
            }

            List<ERoutePerformanceDAO> ERoutePerformanceDAOs = await DataContext.ERoutePerformance.Where(ep => Start <= ep.Date && ep.Date <= End).ToListAsync();

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
                    StoreCheckerMonitor_StoreCheckingDTO.SalesOrder = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.External = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.Internal = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.Revenue = new Dictionary<long, decimal>();
                    MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Add(StoreCheckerMonitor_StoreCheckingDTO);
                }
            }

            // khởi tạo kế hoạch
            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                for (DateTime i = Start; i < End; i = i.AddDays(1))
                {
                    MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings
                        .Where(s => s.Date == i).FirstOrDefault();

                    List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                           .Where(s =>
                               s.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId &&
                               s.CheckOutAt.Value.Date == i
                           ).ToList();
                    List<long> StoreCheckingIds = ListChecked.Select(sc => sc.Id).ToList();
                    List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder.Where(so => so.StoreCheckingId.HasValue &&
                        StoreCheckingIds.Contains(so.StoreCheckingId.Value))
                        .Select(so => new IndirectSalesOrderDAO
                        {
                            Id = so.Id,
                            Total = so.Total,
                            StoreCheckingId = so.StoreCheckingId,
                        })
                        .ToListAsync();

                    foreach (StoreCheckingDAO Checked in ListChecked)
                    {
                        List<IndirectSalesOrderDAO> CheckedIndirectSalesOrderCounter = IndirectSalesOrderDAOs.Where(so => so.StoreCheckingId.Value == Checked.Id).ToList();
                        Checked.IndirectSalesOrderCounter = CheckedIndirectSalesOrderCounter.Count();
                        MonitorStoreChecker_StoreCheckingDTO.PlanCounter = ERoutePerformanceDAOs
                            .Where(ep => ep.SaleEmployeeId == Checked.SaleEmployeeId && ep.Date == Checked.CheckInAt.Value.Date).Select(ep => ep.PlanCounter).FirstOrDefault();
                      
                        if (Checked.Planned)
                            MonitorStoreChecker_StoreCheckingDTO.Internal.Add(Checked.StoreId);
                        else
                            MonitorStoreChecker_StoreCheckingDTO.External.Add(Checked.StoreId);
                        if (Checked.ImageCounter > 0)
                            MonitorStoreChecker_StoreCheckingDTO.Image.Add(Checked.StoreId);
                        if (Checked.IndirectSalesOrderCounter > 0)
                        {
                            MonitorStoreChecker_StoreCheckingDTO.SalesOrder.Add(Checked.StoreId);
                            if (!MonitorStoreChecker_StoreCheckingDTO.Revenue.ContainsKey(Checked.StoreId))
                                MonitorStoreChecker_StoreCheckingDTO.Revenue.Add(Checked.StoreId, 0);
                            MonitorStoreChecker_StoreCheckingDTO.Revenue[Checked.StoreId] += CheckedIndirectSalesOrderCounter.Select(so => so.Total).DefaultIfEmpty().Sum();
                        }
                    }
                }
            }
            return MonitorStoreChecker_MonitorStoreCheckerDTOs;
        }

        [Route(MonitorStoreCheckerRoute.Get), HttpPost]
        public async Task<List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>> Get([FromBody] MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO)
        {
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

    }
}
