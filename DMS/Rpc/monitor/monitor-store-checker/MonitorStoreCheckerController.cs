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
using NGS.Templater;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
              LocalStartDay(CurrentContext) :
              MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

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
            List<long> AppUserIds = new List<long>();

            var ProblemDAOAppUserIds = await DataContext.Problem.Where(p => OrganizationIds.Contains(p.Store.OrganizationId))
                .Select(x => x.CreatorId)
                .ToListAsync();
            AppUserIds.AddRange(ProblemDAOAppUserIds);

            var StoreCheckingAppUserIds = await DataContext.StoreChecking
                .Where(sc => OrganizationIds.Contains(sc.SaleEmployee.OrganizationId.Value) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .Select(x => x.SaleEmployeeId)
                .ToListAsync();
            AppUserIds.AddRange(StoreCheckingAppUserIds);

            var IndirectSalesOrderAppUserIds = await DataContext.IndirectSalesOrder
                .Where(o => Start <= o.OrderDate && o.OrderDate <= End && OrganizationIds.Contains(o.SaleEmployee.OrganizationId.Value))
                .Select(x => x.SaleEmployeeId)
                .ToListAsync();
            AppUserIds.AddRange(IndirectSalesOrderAppUserIds);
            AppUserIds = FilterAppUserIds.Intersect(AppUserIds).ToList();

            if (SaleEmployeeId.HasValue)
                AppUserIds = AppUserIds.Where(x => x == SaleEmployeeId.Value).ToList();
            AppUserIds = AppUserIds.Distinct().ToList();

            int count = await DataContext.AppUser.Where(au =>
                AppUserIds.Contains(au.Id) &&
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
             LocalStartDay(CurrentContext) :
             MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

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
            List<long> AppUserIds = new List<long>();

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem.Where(p => OrganizationIds.Contains(p.Store.OrganizationId)).ToListAsync();
            AppUserIds.AddRange(ProblemDAOs.Select(e => e.CreatorId).ToList());

            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc =>
                    OrganizationIds.Contains(sc.SaleEmployee.OrganizationId.Value) &&
                    sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            AppUserIds.AddRange(StoreCheckingDAOs.Select(e => e.SaleEmployeeId).ToList());
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(o =>
                    Start <= o.OrderDate && o.OrderDate <= End &&
                    (!SaleEmployeeId.HasValue || SaleEmployeeId.Value == o.SaleEmployeeId) &&
                    OrganizationIds.Contains(o.OrganizationId)).ToListAsync();

            AppUserIds.AddRange(IndirectSalesOrderDAOs.Select(e => e.SaleEmployeeId).ToList());

            AppUserIds = FilterAppUserIds.Intersect(AppUserIds).ToList();
            if (SaleEmployeeId.HasValue)
                AppUserIds = AppUserIds.Where(x => x == SaleEmployeeId.Value).ToList();
            AppUserIds = AppUserIds.Distinct().ToList();

            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
              .Where(ec => ec.ERoute.RealStartDate <= End &&
                    (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Start) &&
                    AppUserIds.Contains(ec.ERoute.SaleEmployeeId))
              .Include(ec => ec.ERouteContentDays)
              .Include(ec => ec.ERoute)
              .ToListAsync();
            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au =>
               AppUserIds.Contains(au.Id) &&
               au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value) &&
               (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value)
            )
                .Include(au => au.Organization)
                .OrderBy(au => au.Organization.Path).ThenBy(x => x.DisplayName)
                .Skip(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Skip)
                .Take(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Take)
                .ToListAsync();
            AppUserIds = AppUserDAOs.Select(x => x.Id).ToList();

            var StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId))
                .Where(x => x.ShootingAt <= Start && x.ShootingAt >= End)
                .ToListAsync();
            var AlbumImageMappingDAOs = await DataContext.AlbumImageMapping
                .Where(x => x.SaleEmployeeId.HasValue)
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId.Value))
                .Where(x => x.ShootingAt <= Start && x.ShootingAt >= End)
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

            // khởi tạo kế hoạch
            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                for (DateTime i = Start; i <= End; i = i.AddDays(1))
                {
                    MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO = MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings
                        .Where(s => s.Date == i).FirstOrDefault();
                    MonitorStoreChecker_StoreCheckingDTO.SalesOrderCounter = IndirectSalesOrderDAOs
                        .Where(o => i <= o.OrderDate && o.OrderDate < i.AddDays(1) && o.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Count();
                    MonitorStoreChecker_StoreCheckingDTO.RevenueCounter = IndirectSalesOrderDAOs
                        .Where(o => i <= o.OrderDate && o.OrderDate < i.AddDays(1) && o.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Select(o => o.Total).DefaultIfEmpty(0).Sum();

                    MonitorStoreChecker_StoreCheckingDTO.PlanCounter = CountPlan(i, MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId, ERouteContentDAOs);

                    List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                           .Where(s =>
                               s.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId &&
                               i <= s.CheckOutAt.Value && s.CheckOutAt.Value < i.AddDays(1)
                           ).ToList();
                    foreach (StoreCheckingDAO Checked in ListChecked)
                    {
                        if (Checked.Planned)
                            MonitorStoreChecker_StoreCheckingDTO.Internal.Add(Checked.StoreId);
                        else
                            MonitorStoreChecker_StoreCheckingDTO.External.Add(Checked.StoreId);
                    }

                    var StoreCheckingImageMappingIds = StoreCheckingImageMappingDAOs
                        .Where(x => x.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Where(x => x.ShootingAt.Date == i.Date)
                        .Select(x => x.ImageId)
                        .ToList();
                    var AlbumImageMappingIds = AlbumImageMappingDAOs
                        .Where(x => x.SaleEmployeeId == MonitorStoreChecker_SaleEmployeeDTO.SaleEmployeeId)
                        .Where(x => x.ShootingAt.Date == i.Date)
                        .Select(x => x.ImageId)
                        .ToList();
                    StoreCheckingImageMappingIds.ForEach(x => MonitorStoreChecker_StoreCheckingDTO.Image.Add(x));
                    AlbumImageMappingIds.ForEach(x => MonitorStoreChecker_StoreCheckingDTO.Image.Add(x));
                }
            }

            foreach (MonitorStoreChecker_SaleEmployeeDTO MonitorStoreChecker_SaleEmployeeDTO in MonitorStoreChecker_SaleEmployeeDTOs)
            {
                foreach (var StoreChecking in MonitorStoreChecker_SaleEmployeeDTO.StoreCheckings)
                {
                    StoreChecking.Date = StoreChecking.Date.AddHours(CurrentContext.TimeZone);
                }
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

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
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
            List<StoreCheckingImageMappingDAO> StoreCheckingImageMappingDAOs = await DataContext.StoreCheckingImageMapping
                .Where(sc => StoreCheckingIds.Contains(sc.StoreCheckingId))
                .Include(x => x.Image)
                .ToListAsync();
            List<AlbumImageMappingDAO> AlbumImageMappingDAOs = await DataContext.AlbumImageMapping
                .Where(x => x.SaleEmployeeId == SaleEmployeeId)
                .Where(x => x.ShootingAt >= Start && x.ShootingAt <= End)
                .ToListAsync();

            List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO> MonitorStoreChecker_MonitorStoreCheckerDetailDTOs = new List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>();
            foreach (long StoreId in StoreIds)
            {
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId).ToList();
                List<long> SubStoreCheckingIds = StoreCheckingDAOs.Where(sc => sc.StoreId == StoreId).Select(sc => sc.Id).ToList();
                List<ProblemDAO> Problems = ProblemDAOs.Where(p => SubStoreCheckingIds.Contains(p.StoreCheckingId.Value)).ToList();
                List<StoreCheckingImageMappingDAO> SubStoreCheckingImageMappingDAOs = StoreCheckingImageMappingDAOs.Where(sc => SubStoreCheckingIds.Contains(sc.StoreCheckingId)).ToList();
                List<AlbumImageMappingDAO> SubAlbumImageMappingDAOs = AlbumImageMappingDAOs.Where(x => x.StoreId == StoreId).ToList();
                int Max = 1;
                Max = SubIndirectSalesOrderDAOs.Count > Max ? IndirectSalesOrderDAOs.Count : Max;
                Max = Problems.Count > Max ? Problems.Count : Max;
                StoreDAO storeDAO = StoreDAOs.Where(s => s.Id == StoreId).FirstOrDefault();
                MonitorStoreChecker_MonitorStoreCheckerDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorStoreChecker_MonitorStoreCheckerDetailDTO
                {
                    StoreId = StoreId,
                    StoreCode = storeDAO.Code,
                    StoreName = storeDAO.Name,
                    ImageCounter = SubStoreCheckingImageMappingDAOs.Count() + SubAlbumImageMappingDAOs.Count(),
                    Infoes = new List<MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO>(),
                };

                MonitorStoreChecker_MonitorStoreCheckerDetailDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDetailDTO);
                for (int i = 0; i < Max; i++)
                {
                    MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO Info = new MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO();
                    MonitorStoreChecker_MonitorStoreCheckerDetailDTO.Infoes.Add(Info);
                    if (SubStoreCheckingImageMappingDAOs.Count > i)
                    {
                        Info.ImagePath = SubStoreCheckingImageMappingDAOs[i].Image.Url;
                    }
                    if (SubIndirectSalesOrderDAOs.Count > i)
                    {
                        Info.IndirectSalesOrderCode = SubIndirectSalesOrderDAOs[i].Code;
                        Info.Sales = SubIndirectSalesOrderDAOs[i].Total;
                    }
                    if (Problems.Count > i)
                    {
                        Info.ProblemCode = Problems[i].Code;
                        Info.ProblemId = Problems[i].Id;
                    }
                }
            }
            return MonitorStoreChecker_MonitorStoreCheckerDetailDTOs;
        }

        [Route(MonitorStoreCheckerRoute.ListImage), HttpPost]
        public async Task<List<MonitorStoreChecker_StoreCheckingImageMappingDTO>> ListImage([FromBody] MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = MonitorStoreChecker_StoreCheckingDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1);
            var query = from scim in DataContext.StoreCheckingImageMapping
                        join sc in DataContext.StoreChecking on scim.StoreCheckingId equals sc.Id
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join a in DataContext.Album on scim.AlbumId equals a.Id
                        join i in DataContext.Image on scim.ImageId equals i.Id
                        join au in DataContext.AppUser on scim.SaleEmployeeId equals au.Id
                        where scim.StoreId == MonitorStoreChecker_StoreCheckingDTO.StoreId &&
                        scim.SaleEmployeeId == MonitorStoreChecker_StoreCheckingDTO.SaleEmployeeId &&
                        sc.CheckOutAt.HasValue &&
                        Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value < End
                        select new MonitorStoreChecker_StoreCheckingImageMappingDTO
                        {
                            AlbumId = scim.AlbumId,
                            ImageId = scim.ImageId,
                            SaleEmployeeId = scim.SaleEmployeeId,
                            ShootingAt = scim.ShootingAt,
                            StoreCheckingId = scim.StoreCheckingId,
                            StoreId = scim.StoreId,
                            Distance = scim.Distance,
                            Album = new MonitorStoreChecker_AlbumDTO
                            {
                                Id = a.Id,
                                Name = a.Name
                            },
                            Image = new MonitorStoreChecker_ImageDTO
                            {
                                Id = i.Id,
                                Url = i.Url
                            },
                            SaleEmployee = new MonitorStoreChecker_AppUserDTO
                            {
                                Id = au.Id,
                                DisplayName = au.DisplayName
                            },
                            Store = new MonitorStoreChecker_StoreDTO
                            {
                                Id = s.Id,
                                Address = s.Address,
                                Name = s.Name
                            }
                        };

            var query2 = from aim in DataContext.AlbumImageMapping
                         join s in DataContext.Store on aim.StoreId equals s.Id
                         join a in DataContext.Album on aim.AlbumId equals a.Id
                         join i in DataContext.Image on aim.ImageId equals i.Id
                         join au in DataContext.AppUser on aim.SaleEmployeeId equals au.Id
                         where aim.StoreId == MonitorStoreChecker_StoreCheckingDTO.StoreId &&
                         aim.SaleEmployeeId == MonitorStoreChecker_StoreCheckingDTO.SaleEmployeeId &&
                         Start <= aim.ShootingAt && aim.ShootingAt <= End
                         select new MonitorStoreChecker_StoreCheckingImageMappingDTO
                         {
                             AlbumId = aim.AlbumId,
                             ImageId = aim.ImageId,
                             SaleEmployeeId = aim.SaleEmployeeId ?? 0,
                             ShootingAt = aim.ShootingAt,
                             StoreId = aim.StoreId,
                             Album = new MonitorStoreChecker_AlbumDTO
                             {
                                 Id = a.Id,
                                 Name = a.Name
                             },
                             Image = new MonitorStoreChecker_ImageDTO
                             {
                                 Id = i.Id,
                                 Url = i.Url
                             },
                             SaleEmployee = new MonitorStoreChecker_AppUserDTO
                             {
                                 Id = au.Id,
                                 DisplayName = au.DisplayName
                             },
                             Store = new MonitorStoreChecker_StoreDTO
                             {
                                 Id = s.Id,
                                 Address = s.Address,
                                 Name = s.Name
                             }
                         };
            var list1 = await query.ToListAsync();
            var list2 = await query2.ToListAsync();
            var result = new List<MonitorStoreChecker_StoreCheckingImageMappingDTO>();
            result.AddRange(list1);
            result.AddRange(list2);
            return result;
        }

        [Route(MonitorStoreCheckerRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Skip = 0;
            MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Take = int.MaxValue;
            List<MonitorStoreChecker_MonitorStoreCheckerDTO> MonitorStoreChecker_MonitorStoreCheckerDTOs = await List(MonitorStoreChecker_MonitorStoreCheckerFilterDTO);
            long stt = 1;
            foreach (MonitorStoreChecker_MonitorStoreCheckerDTO MonitorStoreChecker_MonitorStoreCheckerDTO in MonitorStoreChecker_MonitorStoreCheckerDTOs)
            {
                foreach (var SaleEmployee in MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees)
                {
                    foreach (var storeChecking in SaleEmployee.StoreCheckings)
                    {
                        storeChecking.STT = stt;
                        stt++;
                        storeChecking.Date = storeChecking.Date.AddHours(CurrentContext.TimeZone);
                    }
                }
            }

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
               LocalStartDay(CurrentContext) :
               MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            string path = "Templates/Monitor_Store_Checker_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.MonitorStoreCheckers = MonitorStoreChecker_MonitorStoreCheckerDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "MonitorStoreChecker.xlsx");
        }

    }
}
