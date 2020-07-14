using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Rpc.monitor;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_salesman
{
    public class MonitorSalesmanController : MonitorController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public MonitorSalesmanController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
        }
        [Route(MonitorSalesmanRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorSalesman_AppUserDTO>> FilterListAppUser([FromBody] SalesmanMonitor_AppUserFilterDTO SalesmanMonitor_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = SalesmanMonitor_AppUserFilterDTO.Id;
            AppUserFilter.Username = SalesmanMonitor_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = SalesmanMonitor_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = SalesmanMonitor_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorSalesman_AppUserDTO> SalesmanMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorSalesman_AppUserDTO(x)).ToList();
            return SalesmanMonitor_AppUserDTOs;
        }

        [Route(MonitorSalesmanRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorSalesman_OrganizationDTO>> FilterListOrganization([FromBody] MonitorSalesman_OrganizationFilterDTO MonitorSalesman_OrganizationFilterDTO)
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
            List<MonitorSalesman_OrganizationDTO> SalesmanMonitor_OrganizationDTOs = Organizations
                .Select(x => new MonitorSalesman_OrganizationDTO(x)).ToList();
            return SalesmanMonitor_OrganizationDTOs;
        }

        [Route(MonitorSalesmanRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] MonitorSalesman_MonitorSalesmanFilterDTO MonitorSalesman_MonitorSalesmanFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.GreaterEqual == null ?
                             StaticParams.DateTimeNow.Date :
                             MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            long? OrganizationId = MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorSalesman_MonitorSalesmanFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> AppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            int count = await DataContext.AppUser.Where(au =>
                AppUserIds.Contains(au.Id) &&
                au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
                (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value)
            ).CountAsync();
            return count;
        }


        [Route(MonitorSalesmanRoute.List), HttpPost]
        public async Task<List<MonitorSalesman_MonitorSalesmanDTO>> List([FromBody] MonitorSalesman_MonitorSalesmanFilterDTO MonitorSalesman_MonitorSalesmanFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.GreaterEqual == null ?
                   StaticParams.DateTimeNow.Date :
                   MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            long? OrganizationId = MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorSalesman_MonitorSalesmanFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            var query = DataContext.AppUser
                .Where(au =>
                    FilterAppUserIds.Contains(au.Id) &&
                    au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
                    (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value))
                .OrderBy(q => q.Organization.Path).ThenBy(q => q.DisplayName)
                .Distinct()
                .Select(ap => new MonitorSalesman_SaleEmployeeDTO
                {
                    SaleEmployeeId = ap.Id,
                    Username = ap.Username,
                    DisplayName = ap.DisplayName,
                    OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                });

            List<MonitorSalesman_SaleEmployeeDTO> MonitorSalesman_SaleEmployeeDTOs = await query
                .Skip(MonitorSalesman_MonitorSalesmanFilterDTO.Skip)
                .Take(MonitorSalesman_MonitorSalesmanFilterDTO.Take)
                .ToListAsync();
            List<string> OrganizationNames = MonitorSalesman_SaleEmployeeDTOs.Select(se => se.OrganizationName).Distinct().ToList();
            List<MonitorSalesman_MonitorSalesmanDTO> MonitorSalesman_MonitorSalesmanDTOs = OrganizationNames.Select(on => new MonitorSalesman_MonitorSalesmanDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (MonitorSalesman_MonitorSalesmanDTO MonitorSalesman_MonitorSalesmanDTO in MonitorSalesman_MonitorSalesmanDTOs)
            {
                MonitorSalesman_MonitorSalesmanDTO.SaleEmployees = MonitorSalesman_SaleEmployeeDTOs
                    .Where(se => se.OrganizationName == MonitorSalesman_MonitorSalesmanDTO.OrganizationName)
                    .ToList();
            }

            List<long> AppUserIds = MonitorSalesman_SaleEmployeeDTOs.Select(s => s.SaleEmployeeId).ToList();

            // Lấy tất cả StoreChecking của các app user id trong 1 trang
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            // lấy ra được tất cả các StoreCheckingId
            List<long> StoreCheckingIds = StoreCheckingDAOs.Select(sc => sc.Id).Distinct().ToList();
            // Lấy ra được tất cả StoreIds và các dữ liệu liên quan tới Store đó

            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping
                .Where(sc => StoreCheckingIds.Contains(sc.StoreCheckingId))
                .ToListAsync();

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem
                .Where(p => AppUserIds.Contains(p.CreatorId) && Start <= p.NoteAt && p.NoteAt <= End)
                .ToListAsync();

            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
               .Where(ec => ec.ERoute.RealStartDate <= End && (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Start) && AppUserIds.Contains(ec.ERoute.SaleEmployeeId))
               .Include(ec => ec.ERouteContentDays)
               .Include(ec => ec.ERoute)
               .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(o => Start <= o.OrderDate && o.OrderDate <= End && AppUserIds.Contains(o.SaleEmployeeId))
                .ToListAsync();

            List<long> StoreIds = new List<long>();
            StoreIds.AddRange(StoreCheckingDAOs.Select(x => x.StoreId).ToList());
            StoreIds.AddRange(ProblemDAOs.Select(x => x.StoreId).ToList());
            StoreIds.AddRange(IndirectSalesOrderDAOs.Select(x => x.BuyerStoreId).ToList());
            StoreIds = StoreIds.Distinct().ToList();
            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(s => StoreIds.Contains(s.Id)).ToListAsync();
            // khởi tạo kế hoạch
            foreach (MonitorSalesman_SaleEmployeeDTO MonitorSalesman_SaleEmployeeDTO in MonitorSalesman_SaleEmployeeDTOs)
            {
                if (MonitorSalesman_SaleEmployeeDTO.StoreCheckings == null)
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings = new List<MonitorSalesman_StoreCheckingDTO>();
                MonitorSalesman_SaleEmployeeDTO.PlanCounter = CountPlan(Start, MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId, ERouteContentDAOs);
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId).ToList();
                MonitorSalesman_SaleEmployeeDTO.SalesOrderCounter = SubIndirectSalesOrderDAOs.Count();
                MonitorSalesman_SaleEmployeeDTO.Revenue = SubIndirectSalesOrderDAOs.Select(o => o.Total).DefaultIfEmpty(0).Sum();

                // Lấy tất cả các StoreChecking của AppUserId đang xét
                List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                       .Where(s =>
                           s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId &&
                           Start <= s.CheckOutAt.Value && s.CheckOutAt.Value <= End
                       ).ToList();

                // Lất tất cả StoreIds với appuser đang xét
                foreach (StoreCheckingDAO Checked in ListChecked)
                {
                    if (Checked.Planned)
                    {
                        if (MonitorSalesman_SaleEmployeeDTO.Internal == null)
                            MonitorSalesman_SaleEmployeeDTO.Internal = new HashSet<long>();
                        MonitorSalesman_SaleEmployeeDTO.Internal.Add(Checked.StoreId);
                    }
                    else
                    {
                        if (MonitorSalesman_SaleEmployeeDTO.External == null)
                            MonitorSalesman_SaleEmployeeDTO.External = new HashSet<long>();
                        MonitorSalesman_SaleEmployeeDTO.External.Add(Checked.StoreId);
                    }
                }
                foreach (long StoreId in StoreIds)
                {
                    List<StoreCheckingDAO> SubStoreCheckingDAOs = StoreCheckingDAOs.Where(s => s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId &&
                            StoreId == s.StoreId &&
                            Start <= s.CheckOutAt.Value && s.CheckOutAt.Value <= End).OrderByDescending(s => s.CheckOutAt).ToList();
                    StoreCheckingDAO Checked = SubStoreCheckingDAOs.FirstOrDefault();
                    StoreDAO StoreDAO = StoreDAOs.Where(s => s.Id == StoreId).FirstOrDefault();
                   
                    MonitorSalesman_StoreCheckingDTO MonitorSalesman_StoreCheckingDTO = new MonitorSalesman_StoreCheckingDTO();
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings.Add(MonitorSalesman_StoreCheckingDTO);
                    if (Checked != null)
                    {
                        MonitorSalesman_StoreCheckingDTO.Id = Checked.Id;
                        MonitorSalesman_StoreCheckingDTO.Latitude = Checked.Latitude ?? 0;
                        MonitorSalesman_StoreCheckingDTO.Longitude = Checked.Longitude ?? 0;
                        MonitorSalesman_StoreCheckingDTO.CheckIn = Checked.CheckInAt;
                        MonitorSalesman_StoreCheckingDTO.CheckOut = Checked.CheckOutAt;
                        MonitorSalesman_StoreCheckingDTO.Image = StoreCheckingImageMappingDAOs.Where(sc => sc.StoreCheckingId == Checked.Id).Select(sc => sc.Image?.Url).FirstOrDefault();
                    }
                    
                    MonitorSalesman_StoreCheckingDTO.StoreCode = StoreDAO.Code;
                    MonitorSalesman_StoreCheckingDTO.StoreName = StoreDAO.Name;
                    MonitorSalesman_StoreCheckingDTO.Address = StoreDAO.Address;

                    MonitorSalesman_StoreCheckingDTO.CompetitorProblems = ProblemDAOs.Where(p => p.StoreId == StoreId && p.ProblemTypeId == ProblemTypeEnum.COMPETITOR.Id)
                     .Select(p => new MonitorSalesman_ProblemDTO
                     {
                         Id = p.Id,
                         Code = p.Code,
                     }).ToList();
                    MonitorSalesman_StoreCheckingDTO.StoreProblems = ProblemDAOs.Where(p => p.StoreId == StoreId && p.ProblemTypeId == ProblemTypeEnum.STORE.Id)
                     .Select(p => new MonitorSalesman_ProblemDTO
                     {
                         Id = p.Id,
                         Code = p.Code,
                     }).ToList();
                    MonitorSalesman_StoreCheckingDTO.IndirectSalesOrders = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId)
                     .Select(i => new MonitorSalesman_IndirectSalesOrderDTO
                     {
                         Id = i.Id,
                         Code = i.Code,
                     }).ToList();

                };
            }
            return MonitorSalesman_MonitorSalesmanDTOs;
        }

        [Route(MonitorSalesmanRoute.Get), HttpPost]
        public async Task<List<MonitorSalesman_MonitorSalesmanDetailDTO>> Get([FromBody] MonitorSalesman_MonitorSalesmanDetailFilterDTO MonitorSalesman_MonitorSalesmanDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long SaleEmployeeId = MonitorSalesman_MonitorSalesmanDetailFilterDTO.SaleEmployeeId;
            DateTime Start = MonitorSalesman_MonitorSalesmanDetailFilterDTO.Date.Date;
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            List<long> StoreIds = new List<long>();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc =>
                    sc.CheckOutAt.HasValue &&
                    sc.SaleEmployeeId == SaleEmployeeId &&
                    Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            List<long> Mobile_StoreIds = StoreCheckingDAOs.Select(s => s.StoreId).Distinct().ToList();
            StoreIds.AddRange(Mobile_StoreIds);
            List<long> StoreCheckingIds = StoreCheckingDAOs.Select(s => s.Id).Distinct().ToList();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(o => Start <= o.OrderDate && o.OrderDate <= End && o.SaleEmployeeId == SaleEmployeeId)
                .ToListAsync();
            List<long> SalesOrder_StoreIds = IndirectSalesOrderDAOs.Select(o => o.BuyerStoreId).ToList();
            StoreIds.AddRange(SalesOrder_StoreIds);
            StoreIds = StoreIds.Distinct().ToList();
            List<StoreDAO> StoreDAOs = await DataContext.Store.Where(s => StoreIds.Contains(s.Id)).ToListAsync();

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem.Where(p =>
                StoreIds.Contains(p.StoreId) &&
                p.NoteAt >= Start && p.NoteAt <= End
            ).ToListAsync();
            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping.Where(sc => StoreCheckingIds.Contains(sc.StoreCheckingId))
                .Include(sc => sc.Image)
                .ToListAsync();

            List<MonitorSalesman_MonitorSalesmanDetailDTO> MonitorStoreChecker_MonitorStoreCheckerDetailDTOs = new List<MonitorSalesman_MonitorSalesmanDetailDTO>();
            foreach (long StoreId in StoreIds)
            {
                List<ProblemDAO> CompetitorProblems = ProblemDAOs.Where(p => p.StoreId == StoreId && p.ProblemTypeId == ProblemTypeEnum.COMPETITOR.Id).ToList();
                List<ProblemDAO> StoreProblems = ProblemDAOs.Where(p => p.StoreId == StoreId && p.ProblemTypeId == ProblemTypeEnum.STORE.Id).ToList();
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId).ToList();
                List<long> SubStoreCheckingIds = StoreCheckingDAOs.Where(sc => sc.StoreId == StoreId).Select(sc => sc.Id).ToList();
                List<StoreCheckingImageMappingDAO> SubStoreCheckingImageMappingDAOs = StoreCheckingImageMappingDAOs.Where(sc => SubStoreCheckingIds.Contains(sc.StoreCheckingId)).ToList();

                int Max = 1;
                Max = SubIndirectSalesOrderDAOs.Count > Max ? IndirectSalesOrderDAOs.Count : Max;
                Max = CompetitorProblems.Count > Max ? CompetitorProblems.Count : Max;
                Max = StoreProblems.Count > Max ? StoreProblems.Count : Max;
                StoreDAO storeDAO = StoreDAOs.Where(s => s.Id == StoreId).FirstOrDefault();
                MonitorSalesman_MonitorSalesmanDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorSalesman_MonitorSalesmanDetailDTO
                {
                    StoreCode = storeDAO.Code,
                    StoreName = storeDAO.Name,
                    Infoes = new List<MonitorSalesman_MonitorSalesmanDetailInfoDTO>(),
                };
                MonitorStoreChecker_MonitorStoreCheckerDetailDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDetailDTO);
                for (int i = 0; i < Max; i++)
                {
                    MonitorSalesman_MonitorSalesmanDetailInfoDTO Info = new MonitorSalesman_MonitorSalesmanDetailInfoDTO();
                    MonitorStoreChecker_MonitorStoreCheckerDetailDTO.Infoes.Add(Info);
                    if (i == 0)
                    {
                        Info.ImagePath = SubStoreCheckingImageMappingDAOs.Select(i => i.Image.Url).FirstOrDefault();
                    }

                    if (SubIndirectSalesOrderDAOs.Count > i)
                    {
                        Info.IndirectSalesOrderCode = SubIndirectSalesOrderDAOs[i].Code;
                        Info.Sales = SubIndirectSalesOrderDAOs[i].Total;
                    }
                    if (CompetitorProblems.Count > i)
                    {
                        Info.CompetitorProblemCode = CompetitorProblems[i].Code;
                    }
                    if (StoreProblems.Count > i)
                    {
                        Info.StoreProblemCode = StoreProblems[i].Code;
                    }
                }
            }
            return MonitorStoreChecker_MonitorStoreCheckerDetailDTOs;
        }
    }
}
