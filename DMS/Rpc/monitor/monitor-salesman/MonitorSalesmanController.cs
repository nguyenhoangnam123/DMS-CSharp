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

namespace DMS.Rpc.Monitor.monitor_salesman
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
            OrganizationDAO OrganizationDAO = null;
            if (OrganizationId != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == OrganizationId.Value).FirstOrDefaultAsync();
            }

            var query = from ap in DataContext.AppUser
                        join o in DataContext.Organization on ap.OrganizationId equals o.Id
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select ap.Id;
            int count = await query.Distinct().CountAsync();
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

            var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End
                        select new MonitorSalesman_SaleEmployeeDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                        };

            List<MonitorSalesman_SaleEmployeeDTO> MonitorSalesman_SaleEmployeeDTOs = await query.Distinct().OrderBy(q => q.DisplayName)
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
                .Include(sc => sc.Store)
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .ToListAsync();
            List<ERoutePerformanceDAO> ERoutePerformanceDAOs = await DataContext.ERoutePerformance.Where(ep => Start <= ep.Date && ep.Date <= End).ToListAsync();

            // khởi tạo kế hoạch
            foreach (MonitorSalesman_SaleEmployeeDTO MonitorSalesman_SaleEmployeeDTO in MonitorSalesman_SaleEmployeeDTOs)
            {
                if (MonitorSalesman_SaleEmployeeDTO.StoreCheckings == null)
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings = new List<MonitorSalesman_StoreCheckingDTO>();
                MonitorSalesman_SaleEmployeeDTO.PlanCounter = ERoutePerformanceDAOs
                            .Where(ep => ep.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId && Start <= ep.Date && ep.Date <= End)
                            .Select(ep => ep.PlanCounter).FirstOrDefault();

                List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                       .Where(s =>
                           s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId &&
                           Start <= s.CheckOutAt.Value && s.CheckOutAt.Value <= End
                       ).ToList();

                List<long> StoreCheckingIds = ListChecked.Select(sc => sc.Id).ToList();
                foreach (StoreCheckingDAO Checked in ListChecked)
                {
                    MonitorSalesman_StoreCheckingDTO MonitorSalesman_StoreCheckingDTO = new MonitorSalesman_StoreCheckingDTO
                    {
                        Id = Checked.Id,
                        Latitude = Checked.Latitude ?? 0,
                        Longitude = Checked.Longitude ?? 0,
                        StoreCode = Checked.Store?.Code,
                        StoreName = Checked.Store?.Name,
                    };
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings.Add(MonitorSalesman_StoreCheckingDTO);
                }
            }
            return MonitorSalesman_MonitorSalesmanDTOs;
        }

    }
}
