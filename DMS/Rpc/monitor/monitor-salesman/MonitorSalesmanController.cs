using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Repositories;
using DMS.Rpc.monitor;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

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
        public async Task<List<MonitorSalesman_OrganizationDTO>> FilterListOrganization()
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
                             LocalStartDay(CurrentContext) :
                             MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value;

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

            //var storeCheckingQuery = from sc in DataContext.StoreChecking
            //                         join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
            //                         where AppUserIds.Contains(sc.SaleEmployeeId) &&
            //                         OrganizationIds.Contains(sc.OrganizationId) &&
            //                         (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
            //                         au.DeletedAt == null &&
            //                         sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt && sc.CheckOutAt <= End
            //                         select sc;

            //var storeImageQuery = from si in DataContext.StoreImage
            //                      join au in DataContext.AppUser on si.SaleEmployeeId equals au.Id
            //                      where si.SaleEmployeeId.HasValue && AppUserIds.Contains(si.SaleEmployeeId.Value) &&
            //                      OrganizationIds.Contains(si.OrganizationId) &&
            //                      (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
            //                      au.DeletedAt == null &&
            //                      Start <= si.ShootingAt && si.ShootingAt <= End
            //                      select si;

            //var salesOrderQuery = from i in DataContext.IndirectSalesOrder
            //                      join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
            //                      where AppUserIds.Contains(i.SaleEmployeeId) &&
            //                      OrganizationIds.Contains(i.OrganizationId) &&
            //                      (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
            //                      au.DeletedAt == null &&
            //                      Start <= i.OrderDate && i.OrderDate <= End
            //                      select new IndirectSalesOrderDAO
            //                      {
            //                          Id = i.Id,
            //                          OrganizationId = i.OrganizationId,
            //                          SaleEmployeeId = i.SaleEmployeeId,
            //                          BuyerStoreId = i.BuyerStoreId,
            //                          Total = i.Total
            //                      };

            //var Ids1 = storeCheckingQuery.ToListAsync();
            //var Ids2 = storeImageQuery.ToListAsync();
            //var Ids3 = salesOrderQuery.ToListAsync();
            int count = await DataContext.AppUser.Where(au =>
                au.DeletedAt == null &&
                AppUserIds.Contains(au.Id) &&
                OrganizationIds.Contains(au.OrganizationId) &&
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
                             LocalStartDay(CurrentContext) :
                             MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value;

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

            var storeCheckingQuery = from sc in DataContext.StoreChecking
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     where AppUserIds.Contains(sc.SaleEmployeeId) &&
                                     OrganizationIds.Contains(sc.OrganizationId) &&
                                     (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                                     au.DeletedAt == null &&
                                     sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt && sc.CheckOutAt <= End
                                     select sc;

            var storeImageQuery = from si in DataContext.StoreImage
                                  join au in DataContext.AppUser on si.SaleEmployeeId equals au.Id
                                  where si.SaleEmployeeId.HasValue && AppUserIds.Contains(si.SaleEmployeeId.Value) &&
                                  OrganizationIds.Contains(si.OrganizationId) &&
                                  (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                                  au.DeletedAt == null &&
                                  Start <= si.ShootingAt && si.ShootingAt <= End
                                  select si;

            var salesOrderQuery = from i in DataContext.IndirectSalesOrder
                                  join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                                  where AppUserIds.Contains(i.SaleEmployeeId) &&
                                  OrganizationIds.Contains(i.OrganizationId) &&
                                  (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                                  au.DeletedAt == null &&
                                  Start <= i.OrderDate && i.OrderDate <= End &&
                                  (i.RequestStateId == RequestStateEnum.APPROVED.Id || i.RequestStateId == RequestStateEnum.PENDING.Id)
                                  select new IndirectSalesOrderDAO
                                  {
                                      Id = i.Id,
                                      OrganizationId = i.OrganizationId,
                                      SaleEmployeeId = i.SaleEmployeeId,
                                      BuyerStoreId = i.BuyerStoreId,
                                      Total = i.Total
                                  };

            var storeUncheckingQuery = from su in DataContext.StoreUnchecking
                                       where AppUserIds.Contains(su.AppUserId) &&
                                       OrganizationIds.Contains(su.OrganizationId) &&
                                       (SaleEmployeeId == null || su.AppUserId == SaleEmployeeId.Value) &&
                                       Start <= su.Date && su.Date <= End
                                       select su;

            var Ids1 = await storeCheckingQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.SaleEmployeeId,
            }).ToListAsync();
            var Ids2 = await storeImageQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.SaleEmployeeId.Value,
            }).ToListAsync();
            var Ids3 = await salesOrderQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.SaleEmployeeId,
            }).ToListAsync();
            var Ids4 = await storeUncheckingQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.AppUserId,
            }).ToListAsync();
            var Ids = Ids1;
            Ids.AddRange(Ids2);
            Ids.AddRange(Ids3);
            Ids.AddRange(Ids4);
            Ids = Ids.Distinct().ToList();

            AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(x => AppUserIds.Contains(x.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .Select(x => new AppUserDAO
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Username = x.Username,
                    OrganizationId = x.OrganizationId
                }).ToListAsync();

            var Organizations = await DataContext.Organization
                .Where(x => x.DeletedAt == null)
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            List<StoreCheckingDAO> StoreCheckingDAOs = await storeCheckingQuery.ToListAsync();
            List<StoreImageDAO> StoreImageDAOs = await storeImageQuery.ToListAsync();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await salesOrderQuery.ToListAsync();
            List<StoreUncheckingDAO> StoreUncheckingDAOs = await storeUncheckingQuery.ToListAsync();

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem
                .Where(p => AppUserIds.Contains(p.CreatorId) && 
                Start <= p.NoteAt && p.NoteAt <= End)
                .ToListAsync();

            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
                .Where(x => x.ERoute.DeletedAt == null && x.ERoute.StatusId == StatusEnum.ACTIVE.Id)
                .Where(ec => ec.ERoute.RealStartDate <= End && (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Start) && AppUserIds.Contains(ec.ERoute.SaleEmployeeId))
                .Include(ec => ec.ERouteContentDays)
                .Include(ec => ec.ERoute)
                .ToListAsync();

            List<long> StoreIds = new List<long>();
            StoreIds.AddRange(StoreCheckingDAOs.Select(x => x.StoreId).ToList());
            StoreIds.AddRange(StoreImageDAOs.Select(x => x.StoreId).ToList());
            StoreIds.AddRange(ProblemDAOs.Select(x => x.StoreId).ToList());
            StoreIds.AddRange(IndirectSalesOrderDAOs.Select(x => x.BuyerStoreId).ToList());
            StoreIds.AddRange(StoreUncheckingDAOs.Select(x => x.StoreId).ToList());
            StoreIds = StoreIds.Distinct().ToList();

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                     .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query2 = from s in DataContext.Store
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         select new StoreDAO
                         {
                             Id = s.Id,
                             Code = s.Code,
                             Name = s.Name,
                             Address = s.Address,
                         };
            List<StoreDAO> StoreDAOs = await query2.ToListAsync();

            List<MonitorSalesman_MonitorSalesmanDTO> MonitorSalesman_MonitorSalesmanDTOs = new List<MonitorSalesman_MonitorSalesmanDTO>();
            foreach (var Organization in Organizations)
            {
                MonitorSalesman_MonitorSalesmanDTO MonitorSalesman_MonitorSalesmanDTO = new MonitorSalesman_MonitorSalesmanDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<MonitorSalesman_SaleEmployeeDTO>()
                };
                MonitorSalesman_MonitorSalesmanDTO.SaleEmployees = Ids
                    .Where(x => x.OrganizationId == Organization.Id)
                    .Select(x => new MonitorSalesman_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.SalesEmployeeId
                    }).ToList();
                MonitorSalesman_MonitorSalesmanDTOs.Add(MonitorSalesman_MonitorSalesmanDTO);
            }

            Parallel.ForEach(MonitorSalesman_MonitorSalesmanDTOs, MonitorSalesman_MonitorSalesmanDTO =>
            {
                foreach (var MonitorSalesman_SaleEmployeeDTO in MonitorSalesman_MonitorSalesmanDTO.SaleEmployees)
                {
                    var Employee = AppUserDAOs.Where(x => x.Id == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId).FirstOrDefault();
                    if (Employee != null)
                    {
                        MonitorSalesman_SaleEmployeeDTO.Username = Employee.Username;
                        MonitorSalesman_SaleEmployeeDTO.DisplayName = Employee.DisplayName;
                    }

                    if (MonitorSalesman_SaleEmployeeDTO.StoreCheckings == null)
                        MonitorSalesman_SaleEmployeeDTO.StoreCheckings = new List<MonitorSalesman_StoreCheckingDTO>();
                    MonitorSalesman_SaleEmployeeDTO.PlanCounter = CountPlan(Start, MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId, ERouteContentDAOs);
                    List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId).ToList();
                    MonitorSalesman_SaleEmployeeDTO.SalesOrderCounter = SubIndirectSalesOrderDAOs.Count();
                    MonitorSalesman_SaleEmployeeDTO.Revenue = SubIndirectSalesOrderDAOs.Select(o => o.Total).DefaultIfEmpty(0).Sum();
                    MonitorSalesman_SaleEmployeeDTO.Unchecking = StoreUncheckingDAOs.Where(x => x.AppUserId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId).Count();

                    // Lấy tất cả các StoreChecking của AppUserId đang xét
                    List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                           .Where(s => s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId)
                           .ToList();

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

                    MonitorSalesman_SaleEmployeeDTO.ImageCounter = StoreImageDAOs
                        .Where(x => x.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId).Count();
                    foreach (long StoreId in StoreIds)
                    {
                        List<StoreCheckingDAO> SubStoreCheckingDAOs = StoreCheckingDAOs.Where(s => s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId &&
                                StoreId == s.StoreId &&
                                Start <= s.CheckOutAt.Value && s.CheckOutAt.Value <= End).OrderByDescending(s => s.CheckInAt).ToList();
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
                        }

                        MonitorSalesman_StoreCheckingDTO.Image = StoreImageDAOs.Where(si => si.StoreId == StoreId).Select(si => si.Url).FirstOrDefault();
                        MonitorSalesman_StoreCheckingDTO.StoreId = StoreDAO.Id;
                        MonitorSalesman_StoreCheckingDTO.StoreCode = StoreDAO.Code;
                        MonitorSalesman_StoreCheckingDTO.StoreName = StoreDAO.Name;
                        MonitorSalesman_StoreCheckingDTO.Address = StoreDAO.Address;

                        MonitorSalesman_StoreCheckingDTO.Problem = ProblemDAOs.Where(p => p.StoreId == StoreId)
                         .OrderByDescending(x => x.NoteAt)
                         .Select(p => new MonitorSalesman_ProblemDTO
                         {
                             Id = p.Id,
                             Code = p.Code,
                         }).FirstOrDefault();
                        MonitorSalesman_StoreCheckingDTO.IndirectSalesOrder = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId)
                         .OrderByDescending(x => x.OrderDate)
                         .Select(i => new MonitorSalesman_IndirectSalesOrderDTO
                         {
                             Id = i.Id,
                             Code = i.Code,
                         }).FirstOrDefault();

                    };
                }
            });

            AppUserDAOs = AppUserDAOs.Where(x => !Ids.Select(x => x.SalesEmployeeId).Contains(x.Id)).ToList();
            Parallel.ForEach(AppUserDAOs, AppUserDAO =>
            {
                MonitorSalesman_SaleEmployeeDTO MonitorSalesman_SaleEmployeeDTO = new MonitorSalesman_SaleEmployeeDTO();
                MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId = AppUserDAO.Id;
                MonitorSalesman_SaleEmployeeDTO.Username = AppUserDAO.Username;
                MonitorSalesman_SaleEmployeeDTO.DisplayName = AppUserDAO.DisplayName;
                var MonitorSalesman_MonitorSalesmanDTO = MonitorSalesman_MonitorSalesmanDTOs.Where(x => x.OrganizationId == AppUserDAO.OrganizationId).FirstOrDefault();
                MonitorSalesman_MonitorSalesmanDTO.SaleEmployees.Add(MonitorSalesman_SaleEmployeeDTO);
            });

            return MonitorSalesman_MonitorSalesmanDTOs.Where(x =>x.SaleEmployees.Any()).ToList();
        }

        [Route(MonitorSalesmanRoute.Get), HttpPost]
        public async Task<List<MonitorSalesman_MonitorSalesmanDetailDTO>> Get([FromBody] MonitorSalesman_MonitorSalesmanDetailFilterDTO MonitorSalesman_MonitorSalesmanDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long SaleEmployeeId = MonitorSalesman_MonitorSalesmanDetailFilterDTO.SaleEmployeeId;
            DateTime Start = MonitorSalesman_MonitorSalesmanDetailFilterDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            List<long> StoreCheckingStoreIds = await DataContext.StoreChecking
                .Where(sc =>
                    sc.CheckOutAt.HasValue &&
                    sc.SaleEmployeeId == SaleEmployeeId &&
                    Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .Select(x => x.StoreId)
                .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(o => Start <= o.OrderDate && o.OrderDate <= End && 
                o.SaleEmployeeId == SaleEmployeeId &&
                (o.RequestStateId == RequestStateEnum.APPROVED.Id || o.RequestStateId == RequestStateEnum.PENDING.Id))
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Code = x.Code,
                    BuyerStoreId = x.BuyerStoreId,
                    Total = x.Total,
                })
                .ToListAsync();

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem.Where(p =>
                p.CreatorId == SaleEmployeeId &&
                p.NoteAt >= Start && p.NoteAt <= End
            ).ToListAsync();

            List<StoreImageDAO> StoreImageDAOs = await DataContext.StoreImage
                .Where(x => x.SaleEmployeeId.HasValue && x.SaleEmployeeId == SaleEmployeeId &&
                Start <= x.ShootingAt && x.ShootingAt <= End)
                .ToListAsync();

            List<long> StoreIds = new List<long>();
            StoreIds.AddRange(StoreCheckingStoreIds);
            List<long> SalesOrder_StoreIds = IndirectSalesOrderDAOs.Select(o => o.BuyerStoreId).ToList();
            StoreIds.AddRange(SalesOrder_StoreIds);
            var Problem_StoreIds = ProblemDAOs.Select(x => x.StoreId).Distinct().ToList();
            StoreIds.AddRange(Problem_StoreIds);
            var StoreImage_StoreId = StoreImageDAOs.Select(x => x.StoreId).Distinct().ToList();
            StoreIds.AddRange(StoreImage_StoreId);

            StoreIds = StoreIds.Distinct().ToList();
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                     .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query2 = from s in DataContext.Store
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         select new StoreDAO
                         {
                             Id = s.Id,
                             Code = s.Code,
                             Name = s.Name,
                             Address = s.Address,
                         };
            List<StoreDAO> StoreDAOs = await query2.ToListAsync();

            List<MonitorSalesman_MonitorSalesmanDetailDTO> MonitorStoreChecker_MonitorStoreCheckerDetailDTOs = new List<MonitorSalesman_MonitorSalesmanDetailDTO>();
            foreach (long StoreId in StoreIds)
            {
                List<ProblemDAO> Problems = ProblemDAOs.Where(p => p.StoreId == StoreId).ToList();
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId).ToList();
                List<StoreImageDAO> SubStoreImageDAOs = StoreImageDAOs.Where(x => x.StoreId == StoreId).ToList();

                int Max = 1;
                Max = SubIndirectSalesOrderDAOs.Count > Max ? IndirectSalesOrderDAOs.Count : Max;
                Max = Problems.Count > Max ? Problems.Count : Max;
                StoreDAO storeDAO = StoreDAOs.Where(s => s.Id == StoreId).FirstOrDefault();
                MonitorSalesman_MonitorSalesmanDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorSalesman_MonitorSalesmanDetailDTO
                {
                    StoreId = storeDAO.Id,
                    StoreCode = storeDAO.Code,
                    StoreName = storeDAO.Name,
                    ImageCounter = SubStoreImageDAOs.Count(),
                    Infoes = new List<MonitorSalesman_MonitorSalesmanDetailInfoDTO>(),
                };
                MonitorStoreChecker_MonitorStoreCheckerDetailDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDetailDTO);
                for (int i = 0; i < Max; i++)
                {
                    MonitorSalesman_MonitorSalesmanDetailInfoDTO Info = new MonitorSalesman_MonitorSalesmanDetailInfoDTO();
                    MonitorStoreChecker_MonitorStoreCheckerDetailDTO.Infoes.Add(Info);

                    if (SubStoreImageDAOs.Count > i)
                    {
                        Info.ImagePath = SubStoreImageDAOs[i].Url;
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

        [Route(MonitorSalesmanRoute.ListImage), HttpPost]
        public async Task<List<MonitorSalesman_StoreImageDTO>> ListImage([FromBody] MonitorSalesman_MonitorSalesmanDetailFilterDTO MonitorSalesman_MonitorSalesmanDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = MonitorSalesman_MonitorSalesmanDetailFilterDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1);

            var query = from si in DataContext.StoreImage
                        where si.StoreId == MonitorSalesman_MonitorSalesmanDetailFilterDTO.StoreId &&
                        si.SaleEmployeeId.HasValue && si.SaleEmployeeId.Value == MonitorSalesman_MonitorSalesmanDetailFilterDTO.SaleEmployeeId &&
                        Start <= si.ShootingAt && si.ShootingAt < End
                        select new MonitorSalesman_StoreImageDTO
                        {
                            AlbumId = si.AlbumId,
                            ImageId = si.ImageId,
                            SaleEmployeeId = si.SaleEmployeeId.Value,
                            ShootingAt = si.ShootingAt,
                            StoreId = si.StoreId,
                            Distance = si.Distance,
                            Album = new MonitorSalesman_AlbumDTO
                            {
                                Id = si.AlbumId,
                                Name = si.AlbumName
                            },
                            Image = new MonitorSalesman_ImageDTO
                            {
                                Url = si.Url
                            },
                            SaleEmployee = new MonitorSalesman_AppUserDTO
                            {
                                Id = si.SaleEmployeeId.Value,
                                DisplayName = si.DisplayName
                            },
                            Store = new MonitorSalesman_StoreDTO
                            {
                                Id = si.StoreId,
                                Address = si.StoreAddress,
                                Name = si.StoreName
                            }
                        };

            return await query.ToListAsync();
        }

        [Route(MonitorSalesmanRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MonitorSalesman_MonitorSalesmanFilterDTO MonitorSalesman_MonitorSalesmanFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MonitorSalesman_MonitorSalesmanFilterDTO.Skip = 0;
            MonitorSalesman_MonitorSalesmanFilterDTO.Take = int.MaxValue;
            List<MonitorSalesman_MonitorSalesmanDTO> MonitorSalesman_MonitorSalesmanDTOs = await List(MonitorSalesman_MonitorSalesmanFilterDTO);
            int stt = 1;
            foreach (MonitorSalesman_MonitorSalesmanDTO MonitorSalesman_MonitorSalesmanDTO in MonitorSalesman_MonitorSalesmanDTOs)
            {
                foreach (var SaleEmployee in MonitorSalesman_MonitorSalesmanDTO.SaleEmployees)
                {
                    SaleEmployee.STT = stt;
                    stt++;
                }
            }

            DateTime Start = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.GreaterEqual == null ?
                             LocalStartDay(CurrentContext) :
                             MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn.LessEqual.Value.AddDays(1).AddSeconds(-1);

            string path = "Templates/Monitor_Salesman_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.MonitorSalesmans = MonitorSalesman_MonitorSalesmanDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "Monitor_Salesman_Report.xlsx");
        }

        [Route(MonitorSalesmanRoute.ExportUnchecking), HttpPost]
        public async Task<ActionResult> ExExportUncheckingport([FromBody] MonitorSalesman_MonitorSalesmanFilterDTO MonitorSalesman_MonitorSalesmanFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            DateTime Now = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone);
            
            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
                .Where(ec => (!ec.ERoute.EndDate.HasValue || Start <= ec.ERoute.EndDate) && ec.ERoute.StartDate <= End)
                .Include(ec => ec.ERoute)
                .Include(ec => ec.ERouteContentDays)
                .ToListAsync();
            List<StoreUncheckingDAO> PlannedStoreUncheckingDAOs = new List<StoreUncheckingDAO>();
            List<StoreUncheckingDAO> StoreUncheckingDAOs = new List<StoreUncheckingDAO>();
            foreach (ERouteContentDAO ERouteContentDAO in ERouteContentDAOs)
            {
                StoreUncheckingDAO StoreUncheckingDAO = PlannedStoreUncheckingDAOs.Where(su =>
                    su.Date == Start &&
                    su.AppUserId == ERouteContentDAO.ERoute.SaleEmployeeId &&
                    su.StoreId == ERouteContentDAO.StoreId
                ).FirstOrDefault();
                if (StoreUncheckingDAO == null)
                {
                    if (Start >= ERouteContentDAO.ERoute.RealStartDate)
                    {
                        long gap = (Start - ERouteContentDAO.ERoute.RealStartDate).Days % 28;
                        if (ERouteContentDAO.ERouteContentDays.Any(ecd => ecd.OrderDay == gap && ecd.Planned))
                        {
                            StoreUncheckingDAO = new StoreUncheckingDAO
                            {
                                AppUserId = ERouteContentDAO.ERoute.SaleEmployeeId,
                                Date = Start,
                                StoreId = ERouteContentDAO.StoreId,
                                OrganizationId = ERouteContentDAO.ERoute.OrganizationId
                            };
                            PlannedStoreUncheckingDAOs.Add(StoreUncheckingDAO);
                        }
                    }
                }
            }
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.Where(sc => sc.CheckOutAt.HasValue &&
                Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End).ToListAsync();
            foreach (StoreUncheckingDAO StoreUncheckingDAO in PlannedStoreUncheckingDAOs)
            {
                if (!StoreCheckingDAOs.Any(sc => sc.SaleEmployeeId == StoreUncheckingDAO.AppUserId && sc.StoreId == StoreUncheckingDAO.StoreId))
                {
                    StoreUncheckingDAOs.Add(StoreUncheckingDAO);
                }
            }

            var AppUserIds = StoreUncheckingDAOs.Select(x => x.AppUserId).Distinct().ToList();
            var StoreIds = StoreUncheckingDAOs.Select(x => x.StoreId).Distinct().ToList();

            var AppUserDAOs = await DataContext.AppUser.Where(x => AppUserIds.Contains(x.Id)).Select(x => new AppUserDAO
            {
                Id = x.Id,
                Username = x.Username,
                DisplayName = x.DisplayName,
                OrganizationId = x.OrganizationId
            }).OrderBy(x => x.OrganizationId).ThenBy(x => x.DisplayName).ToListAsync();

            var StoreDAOs = await DataContext.Store.Where(x => StoreIds.Contains(x.Id)).Select(x => new StoreDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                CodeDraft = x.CodeDraft,
                Address = x.Address,
                Telephone = x.Telephone,
                StoreType = x.StoreType == null ? null : new StoreTypeDAO
                {
                    Name = x.Name
                },
                StoreStatus = x.StoreStatus == null ? null : new StoreStatusDAO
                {
                    Name = x.StoreStatus.Name
                }
            }).ToListAsync();

            var MonitorSalesman_ExportEmployeeUncheckedDTOs = AppUserDAOs.Select(x => new MonitorSalesman_ExportEmployeeUncheckedDTO
            {
                AppUserId = x.Id,
                DisplayName = x.DisplayName,
            }).ToList();

            Parallel.ForEach(StoreUncheckingDAOs, StoreUncheckingDAO =>
            {
                StoreUncheckingDAO.Store = StoreDAOs.Where(x => x.Id == StoreUncheckingDAO.StoreId).FirstOrDefault();
                StoreUncheckingDAO.AppUser = AppUserDAOs.Where(x => x.Id == StoreUncheckingDAO.AppUserId).FirstOrDefault();
            });

            Parallel.ForEach(MonitorSalesman_ExportEmployeeUncheckedDTOs, MonitorSalesman_ExportEmployeeUncheckedDTO =>
            {
                MonitorSalesman_ExportEmployeeUncheckedDTO.Contents = StoreUncheckingDAOs
                .Where(x => x.AppUserId == MonitorSalesman_ExportEmployeeUncheckedDTO.AppUserId)
                .Select(x => new MonitorSalesman_ExportUncheckedDTO
                {
                    AppUserId = x.AppUserId,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    StoreCode = x.Store.Code,
                    StoreCodeDraft = x.Store.CodeDraft,
                    StoreAddress = x.Store.Address,
                    StoreName = x.Store.Name,
                    StorePhone = x.Store.Telephone,
                    StoreTypeName = x.Store.StoreType?.Name,
                    StoreStatusName = x.Store.StoreStatus?.Name,
                }).ToList();
            });

            int stt = 1;
            foreach (MonitorSalesman_ExportEmployeeUncheckedDTO MonitorSalesman_ExportEmployeeUncheckedDTO in MonitorSalesman_ExportEmployeeUncheckedDTOs)
            {
                foreach (var Content in MonitorSalesman_ExportEmployeeUncheckedDTO.Contents)
                {
                    Content.STT = stt;
                    stt++;
                }
            }
            string path = "Templates/Daily_Unchecking_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Date = Now.ToString("HH:mm") + " ngày " + Now.ToString("dd-MM-yyyy");
            Data.Employees = MonitorSalesman_ExportEmployeeUncheckedDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "Daily_Unchecking_Report.xlsx");
        }

    }
}
