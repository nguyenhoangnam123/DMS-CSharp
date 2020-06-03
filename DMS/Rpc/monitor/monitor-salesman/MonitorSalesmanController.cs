using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.Monitor.monitor_salesman
{
    public class MonitorSalesmanController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        public MonitorSalesmanController(DataContext DataContext, IOrganizationService OrganizationService, IAppUserService AppUserService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
        }
        [Route(MonitorSalesmanRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorSalesman_AppUserDTO>> FilterListAppUser([FromBody] SalesmanMonitor_AppUserFilterDTO SalesmanMonitor_AppUserFilterDTO)
        {
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

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorSalesman_AppUserDTO> SalesmanMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorSalesman_AppUserDTO(x)).ToList();
            return SalesmanMonitor_AppUserDTOs;
        }

        [Route(MonitorSalesmanRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorSalesman_OrganizationDTO>> FilterListOrganization([FromBody] MonitorSalesman_OrganizationFilterDTO MonitorSalesman_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<MonitorSalesman_OrganizationDTO> SalesmanMonitor_OrganizationDTOs = Organizations
                .Select(x => new MonitorSalesman_OrganizationDTO(x)).ToList();
            return SalesmanMonitor_OrganizationDTOs;
        }

        [Route(MonitorSalesmanRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] MonitorSalesman_MonitorSalesmanFilterDTO MonitorSalesman_MonitorSalesmanFilterDTO)
        {
            DateTime CheckIn = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.Date ?? StaticParams.DateTimeNow.Date;

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
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt.Value.Date == CheckIn &&
                        (OrganizationDAO == null || o.Path.StartsWith(OrganizationDAO.Path)) &&
                        (SaleEmployeeId == null || ap.Id == SaleEmployeeId.Value)
                        select ap.Id;
            int count = await query.Distinct().CountAsync();
            return count;
        }


        [Route(MonitorSalesmanRoute.List), HttpPost]
        public async Task<List<MonitorSalesman_MonitorSalesmanDTO>> List([FromBody] MonitorSalesman_MonitorSalesmanFilterDTO MonitorSalesman_MonitorSalesmanFilterDTO)
        {
            DateTime CheckIn = MonitorSalesman_MonitorSalesmanFilterDTO.CheckIn?.Date ?? StaticParams.DateTimeNow.Date;

                        var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt.Value.Date == CheckIn 
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
            List<ERouteContent> ERouteContents = await (from ec in DataContext.ERouteContent
                                                        join e in DataContext.ERoute on ec.ERouteId equals e.Id
                                                        where AppUserIds.Contains(e.SaleEmployeeId) &&
                                                        CheckIn >= e.StartDate && (e.EndDate.HasValue == false || e.EndDate.Value >= CheckIn)
                                                        select new ERouteContent
                                                        {
                                                            Id = ec.Id,
                                                            Monday = ec.Monday,
                                                            Tuesday = ec.Tuesday,
                                                            Wednesday = ec.Wednesday,
                                                            Thursday = ec.Thursday,
                                                            Friday = ec.Friday,
                                                            Saturday = ec.Saturday,
                                                            Sunday = ec.Sunday,
                                                            Week1 = ec.Week1,
                                                            Week2 = ec.Week2,
                                                            Week3 = ec.Week3,
                                                            Week4 = ec.Week4,
                                                            StoreId = ec.StoreId,
                                                            ERoute = new ERoute
                                                            {
                                                                StartDate = e.StartDate,
                                                                SaleEmployeeId = e.SaleEmployeeId,
                                                            }
                                                        }).ToListAsync();

            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue && CheckIn == sc.CheckOutAt.Value.Date)
                .ToListAsync();

            // khởi tạo kế hoạch
            foreach (MonitorSalesman_SaleEmployeeDTO MonitorSalesman_SaleEmployeeDTO in MonitorSalesman_SaleEmployeeDTOs)
            {
                if (MonitorSalesman_SaleEmployeeDTO.StoreCheckings == null)
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings = new List<MonitorSalesman_StoreCheckingDTO>();
                foreach (ERouteContent ERouteContent in ERouteContents)
                {
                    bool PlanWeek = false, PlanDay = false;
                    int week = Convert.ToInt32(StaticParams.DateTimeNow.Subtract(ERouteContent.ERoute.StartDate).TotalDays / 7) + 1;
                    switch (week / 4)
                    {
                        case 1:
                            if (ERouteContent.Week1)
                                PlanWeek = true;
                            break;
                        case 2:
                            if (ERouteContent.Week2)
                                PlanWeek = true;
                            break;
                        case 3:
                            if (ERouteContent.Week3)
                                PlanWeek = true;
                            break;
                        case 0:
                            if (ERouteContent.Week4)
                                PlanWeek = true;
                            break;
                    }
                    DayOfWeek day = StaticParams.DateTimeNow.DayOfWeek;
                    switch (day)
                    {
                        case DayOfWeek.Sunday:
                            if (ERouteContent.Sunday)
                                PlanDay = true;
                            break;
                        case DayOfWeek.Monday:
                            if (ERouteContent.Monday)
                                PlanDay = true;
                            break;
                        case DayOfWeek.Tuesday:
                            if (ERouteContent.Tuesday)
                                PlanDay = true;
                            break;
                        case DayOfWeek.Wednesday:
                            if (ERouteContent.Wednesday)
                                PlanDay = true;
                            break;
                        case DayOfWeek.Thursday:
                            if (ERouteContent.Thursday)
                                PlanDay = true;
                            break;
                        case DayOfWeek.Friday:
                            if (ERouteContent.Friday)
                                PlanDay = true;
                            break;
                        case DayOfWeek.Saturday:
                            if (ERouteContent.Saturday)
                                PlanDay = true;
                            break;
                    }
                    if (PlanDay && PlanWeek)
                        MonitorSalesman_SaleEmployeeDTO.PlanCounter++;
                }
                List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                       .Where(s =>
                           s.SaleEmployeeId == MonitorSalesman_SaleEmployeeDTO.SaleEmployeeId &&
                           s.CheckOutAt.Value.Date == CheckIn
                       ).ToList();
                List<long> StoreCheckingIds = ListChecked.Select(sc => sc.Id).ToList();
                foreach (StoreCheckingDAO Checked in ListChecked)
                {
                    MonitorSalesman_StoreCheckingDTO MonitorSalesman_StoreCheckingDTO = new MonitorSalesman_StoreCheckingDTO
                    {
                        Id = Checked.Id,
                        Latitude = Checked.Latitude ?? 0,
                        Longtitude = Checked.Longtitude ?? 0,
                        StoreCode = Checked.Store.Code,
                        StoreName = Checked.Store.Name,
                    };
                    MonitorSalesman_SaleEmployeeDTO.StoreCheckings.Add(MonitorSalesman_StoreCheckingDTO);
                }
            }
            return MonitorSalesman_MonitorSalesmanDTOs;
        }

    }
}
