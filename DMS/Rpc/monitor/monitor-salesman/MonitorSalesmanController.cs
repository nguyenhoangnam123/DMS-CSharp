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
        public MonitorSalesmanController(DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext) : base(AppUserService, OrganizationService, CurrentContext)
        {
            this.DataContext = DataContext;
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
            AppUserFilter.Id.In = await FilterAppUser();
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
            OrganizationFilter.Id.In = await FilterOrganization();

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
                    StaticParams.DateTimeNow :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value;

            long? OrganizationId = MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorSalesman_MonitorSalesmanFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization();
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            int count = await DataContext.AppUser.Where(au =>
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
                   StaticParams.DateTimeNow :
                   MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value;
            long? OrganizationId = MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorSalesman_MonitorSalesmanFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization();
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();


            var query = DataContext.AppUser
                .Where(au =>
                    au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
                    (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value))
                .Select(ap => new MonitorSalesman_SaleEmployeeDTO
                {
                    SaleEmployeeId = ap.Id,
                    Username = ap.Username,
                    DisplayName = ap.DisplayName,
                    OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                });

            List<MonitorSalesman_SaleEmployeeDTO> MonitorSalesman_SaleEmployeeDTOs = await query.Distinct()
                .OrderBy(q => q.DisplayName)
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

            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .Include(sc => sc.Problems)
                .ToListAsync();
            List<long> StoreCheckingIds = StoreCheckingDAOs.Select(sc => sc.Id).ToList();
            List<long> StoreIds = StoreCheckingDAOs.Select(sc => sc.StoreId).ToList();
            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping
                .Where(sc => StoreCheckingIds.Contains(sc.StoreCheckingId))
                .ToListAsync();
            List<ProblemDAO> ProblemDAOs = await DataContext.Problem
                .Where(p => StoreIds.Contains(p.StoreId))
                .ToListAsync();

            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
               .Where(ec => ec.ERoute.RealStartDate <= End && (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Start) && AppUserIds.Contains(ec.ERoute.SaleEmployeeId))
               .Include(ec => ec.ERouteContentDays)
               .Include(ec => ec.ERoute)
               .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(o => Start <= o.OrderDate && o.OrderDate <= End && AppUserIds.Contains(o.SaleEmployeeId))
                .ToListAsync();

            // khởi tạo kế hoạch
            foreach (MonitorSalesman_SaleEmployeeDTO MonitorSalesman_SaleEmployeeDTO in MonitorSalesman_SaleEmployeeDTOs)
            {
                if (MonitorSalesman_SaleEmployeeDTO.StoreCheckings == null)
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings = new List<MonitorSalesman_StoreCheckingDTO>();
                MonitorSalesman_SaleEmployeeDTO.PlanCounter = CountPlan(Start, MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId, ERouteContentDAOs);
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId).ToList();
                MonitorSalesman_SaleEmployeeDTO.SalesOrderCounter = SubIndirectSalesOrderDAOs.Count();
                MonitorSalesman_SaleEmployeeDTO.Revenue = SubIndirectSalesOrderDAOs.Select(o => o.Total).DefaultIfEmpty(0).Sum();

                List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                       .Where(s =>
                           s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId &&
                           Start <= s.CheckOutAt.Value && s.CheckOutAt.Value <= End
                       ).ToList();

                StoreCheckingIds = ListChecked.Select(sc => sc.Id).ToList();
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

                    MonitorSalesman_StoreCheckingDTO MonitorSalesman_StoreCheckingDTO = new MonitorSalesman_StoreCheckingDTO
                    {
                        Id = Checked.Id,
                        Latitude = Checked.Latitude ?? 0,
                        Longitude = Checked.Longitude ?? 0,
                        StoreCode = Checked.Store?.Code,
                        StoreName = Checked.Store?.Name,
                        Address = Checked.Store?.Address,
                        CheckIn = Checked.CheckInAt,
                        CheckOut = Checked.CheckOutAt,
                        Image = StoreCheckingImageMappingDAOs.Where(sc => sc.StoreCheckingId == Checked.Id).Select(sc => sc.Image?.Url).FirstOrDefault(),
                        CompetitorProblems = ProblemDAOs.Where(p => p.StoreId == Checked.StoreId && p.ProblemTypeId == ProblemTypeEnum.COMPETITOR.Id)
                        .Select(p => new MonitorSalesman_ProblemDTO
                        {
                            Id = p.Id,
                            Code = p.Code,
                        }).ToList(),
                        StoreProblems = ProblemDAOs.Where(p => p.StoreId == Checked.StoreId && p.ProblemTypeId == ProblemTypeEnum.STORE.Id)
                        .Select(p => new MonitorSalesman_ProblemDTO
                        {
                            Id = p.Id,
                            Code = p.Code,
                        }).ToList(),
                        IndirectSalesOrders = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == Checked.StoreId)
                        .Select(i => new MonitorSalesman_IndirectSalesOrderDTO
                        {
                            Id = i.Id,
                            Code = i.Code,
                        }).ToList(),
                    };

                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings.Add(MonitorSalesman_StoreCheckingDTO);
                }
            }
            return MonitorSalesman_MonitorSalesmanDTOs;
        }

        [Route(MonitorSalesmanRoute.Get), HttpPost]
        public async Task<List<MonitorSalesman_MonitorSalesmanDetailDTO>> Get([FromBody] MonitorSalesman_MonitorSalesmanDetailFilterDTO MonitorSalesman_MonitorSalesmanDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long SaleEmployeeId = MonitorSalesman_MonitorSalesmanDetailFilterDTO.SaleEmployeeId;
            DateTime Start = MonitorSalesman_MonitorSalesmanDetailFilterDTO.Date;
            Start = new DateTime(Start.Year, Start.Month, Start.Day);
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
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId).ToList();
                List<long> SubStoreCheckingIds = StoreCheckingDAOs.Where(sc => sc.StoreId == StoreId).Select(sc => sc.Id).ToList();
                List<ProblemDAO> CompetitorProblems = ProblemDAOs.Where(p => SubStoreCheckingIds.Contains(p.StoreCheckingId.Value) && p.ProblemTypeId == ProblemTypeEnum.COMPETITOR.Id).ToList();
                List<ProblemDAO> StoreProblems = ProblemDAOs.Where(p => SubStoreCheckingIds.Contains(p.StoreCheckingId.Value) && p.ProblemTypeId == ProblemTypeEnum.STORE.Id).ToList();
                List<StoreCheckingImageMappingDAO> SubStoreCheckingImageMappingDAOs = StoreCheckingImageMappingDAOs.Where(sc => SubStoreCheckingIds.Contains(sc.StoreCheckingId)).ToList();

                int Max = 1;
                Max = SubIndirectSalesOrderDAOs.Count > Max ? IndirectSalesOrderDAOs.Count : Max;
                Max = CompetitorProblems.Count > Max ? CompetitorProblems.Count : Max;
                Max = StoreProblems.Count > Max ? StoreProblems.Count : Max;
                StoreDAO storeDAO = StoreDAOs.Where(s => s.Id == StoreId).FirstOrDefault();
                for (int i = 0; i < Max; i++)
                {
                    MonitorSalesman_MonitorSalesmanDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorSalesman_MonitorSalesmanDetailDTO
                    {
                        StoreCode = storeDAO.Code,
                        StoreName = storeDAO.Name,
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
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.CompetitorProblemCode = CompetitorProblems[i].Code;
                    }
                    if (StoreProblems.Count > i)
                    {
                        MonitorStoreChecker_MonitorStoreCheckerDetailDTO.StoreProblemCode = StoreProblems[i].Code;
                    }
                }
            }
            return MonitorStoreChecker_MonitorStoreCheckerDetailDTOs;
        }
    }
}
