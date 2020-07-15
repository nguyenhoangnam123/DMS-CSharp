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
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreCheckerController : MonitorController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public MonitorStoreCheckerController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
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
            AppUserFilter.OrganizationId = StoreCheckerMonitor_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreChecker_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreChecker_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreCheckerRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreChecker_OrganizationDTO>> FilterListOrganization()
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

            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            int count = await DataContext.AppUser.Where(au =>
                FilterAppUserIds.Contains(au.Id) &&
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
                StaticParams.DateTimeNow.Date :
                MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au =>
                FilterAppUserIds.Contains(au.Id) &&
               au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
               (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value)
            )
                .Include(au => au.Organization)
                .OrderBy(au => au.Organization.Path).ThenBy(x => x.DisplayName)
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

            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
                .Where(ec => ec.ERoute.RealStartDate <= End && (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Start) && AppUserIds.Contains(ec.ERoute.SaleEmployeeId))
                .Include(ec => ec.ERouteContentDays)
                .Include(ec => ec.ERoute)
                .ToListAsync();

            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder.Where(o => Start <= o.OrderDate && o.OrderDate <= End && AppUserIds.Contains(o.SaleEmployeeId)).ToListAsync();

            // khởi tạo kế hoạch
            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                for (DateTime i = Start; i <= End; i = i.AddDays(1))
                {
                    MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings
                        .Where(s => s.Date == i).FirstOrDefault();
                    MonitorStoreChecker_StoreCheckingDTO.SalesOrderCounter = IndirectSalesOrderDAOs
                        .Where(o => o.OrderDate.Date == i && o.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Count();
                    MonitorStoreChecker_StoreCheckingDTO.RevenueCounter = IndirectSalesOrderDAOs
                        .Where(o => o.OrderDate.Date == i && o.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Select(o => o.Total).DefaultIfEmpty(0).Sum();

                    MonitorStoreChecker_StoreCheckingDTO.PlanCounter = CountPlan(i, MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId, ERouteContentDAOs);

                    List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                           .Where(s =>
                               s.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId &&
                               s.CheckOutAt.Value.Date == i
                           ).ToList();
                    foreach (StoreCheckingDAO Checked in ListChecked)
                    {
                        if (Checked.Planned)
                            MonitorStoreChecker_StoreCheckingDTO.Internal.Add(Checked.StoreId);
                        else
                            MonitorStoreChecker_StoreCheckingDTO.External.Add(Checked.StoreId);
                        if (Checked.ImageCounter > 0)
                            MonitorStoreChecker_StoreCheckingDTO.Image.Add(Checked.StoreId);
                    }
                }
            }

            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking?.Equal != null)
                {
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking.Equal.Value == 0)
                        MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Where(sc => sc.InternalCounter + sc.ExternalCounter == 0).ToList();
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking.Equal.Value == 1)
                        MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Where(sc => sc.InternalCounter + sc.ExternalCounter > 0).ToList();
                }
                if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image?.Equal != null)
                {
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image.Equal.Value == 0)
                        MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Where(sc => sc.Image.Count == 0).ToList();
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image.Equal.Value == 1)
                        MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Where(sc => sc.Image.Count > 0).ToList();
                }
                if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder?.Equal != null)
                {
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder.Equal.Value == 0)
                        MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Where(sc => sc.SalesOrderCounter == 0).ToList();
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder.Equal.Value == 1)
                        MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings.Where(sc => sc.SalesOrderCounter > 0).ToList();
                }
            }

            return MonitorStoreChecker_MonitorStoreCheckerDTOs;
        }

        [Route(MonitorStoreCheckerRoute.Get), HttpPost]
        public async Task<List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>> Get([FromBody] MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.SaleEmployeeId;
            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.Date;
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

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem.Where(p => p.StoreCheckingId.HasValue && StoreCheckingIds.Contains(p.StoreCheckingId.Value)).ToListAsync();
            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping.Where(sc => StoreCheckingIds.Contains(sc.StoreCheckingId))
                .Include(sc => sc.Image)
                .ToListAsync();

            List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO> MonitorStoreChecker_MonitorStoreCheckerDetailDTOs = new List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>();
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
                MonitorStoreChecker_MonitorStoreCheckerDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorStoreChecker_MonitorStoreCheckerDetailDTO
                {
                    StoreCode = storeDAO.Code,
                    StoreName = storeDAO.Name,
                    Infoes = new List<MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO>(),
                };
                MonitorStoreChecker_MonitorStoreCheckerDetailDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDetailDTO);
                for (int i = 0; i < Max; i++)
                {
                    MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO Info = new MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO();
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
