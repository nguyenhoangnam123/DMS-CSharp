using Common;
using DMS.Entities;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MOrganization;
using DMS.Services.MStoreChecking;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirectorController : SimpleController
    {
        private const long TODAY = 0;
        private const long THIS_WEEK = 1;
        private const long THIS_MONTH = 2;
        private const long LAST_MONTH = 3;

        private DataContext DataContext;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        private IOrganizationService OrganizationService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IStoreCheckingService StoreCheckingService;
        public DashboardDirectorController(
            DataContext DataContext,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext,
            IOrganizationService OrganizationService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IStoreCheckingService StoreCheckingService)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
            this.OrganizationService = OrganizationService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.StoreCheckingService = StoreCheckingService;
        }
        [Route(DashboardDirectorRoute.IndirectSalesOrderCounter), HttpPost]
        public async Task<long> CountIndirectSalesOrder()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        au.OrganizationId.HasValue && o.Path.StartsWith(CurrentUser.Organization.Path)
                        select i;

            return query.Count();
        }

        [Route(DashboardDirectorRoute.RevenueTotal), HttpPost]
        public async Task<decimal> RevenueTotal()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        au.OrganizationId.HasValue && o.Path.StartsWith(CurrentUser.Organization.Path)
                        select i;

            var RevenueTotal = query.Select(x => x.Total).Sum();
            return RevenueTotal;
        }

        [Route(DashboardDirectorRoute.ItemSalesTotal), HttpPost]
        public async Task<long> ItemSalesTotal()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from ic in DataContext.IndirectSalesOrderContent
                        join i in DataContext.IndirectSalesOrder on ic.IndirectSalesOrderId equals i.Id
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        au.OrganizationId.HasValue && o.Path.StartsWith(CurrentUser.Organization.Path)
                        select ic;

            var ItemSalesTotal = query.Select(x => x.RequestedQuantity).Sum();
            return ItemSalesTotal;
        }

        [Route(DashboardDirectorRoute.StoreCheckingCounter), HttpPost]
        public async Task<long> CountStoreChecking()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from sc in DataContext.StoreChecking
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        au.OrganizationId.HasValue && o.Path.StartsWith(CurrentUser.Organization.Path)
                        select sc;

            var StoreCheckingCounter = query.Count();
            return StoreCheckingCounter;
        }

        [Route(DashboardDirectorRoute.StoreCheckingCoverage), HttpPost]
        public async Task<List<DashboardDirector_StoreDTO>> StoreCheckingCoverage()
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from s in DataContext.Store
                        join sc in DataContext.StoreChecking on s.Id equals sc.StoreId
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        au.OrganizationId.HasValue && o.Path.StartsWith(CurrentUser.Organization.Path)
                        select new DashboardDirector_StoreDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Telephone = s.Telephone,
                        };
            List<DashboardDirector_StoreDTO> DashboardMonitor_StoreDTOs = await query.Distinct().ToListAsync();
            return DashboardMonitor_StoreDTOs;
        }

        [Route(DashboardDirectorRoute.SaleEmployeeLocation), HttpPost]
        public async Task<List<DashboardDirector_AppUserDTO>> SaleEmployeeLocation()
        {
            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from au in DataContext.AppUser
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where au.DeletedAt.HasValue == false && au.StatusId == Enums.StatusEnum.ACTIVE.Id &&
                        au.OrganizationId.HasValue && o.Path.StartsWith(AppUser.Organization.Path)
                        select new DashboardDirector_AppUserDTO
                        {
                            Id = au.Id,
                            DisplayName = au.DisplayName,
                            Username = au.Username,
                            Longitude = au.Longitude,
                            Latitude = au.Latitude,
                        };

            List<DashboardDirector_AppUserDTO> DashboardDirector_AppUserDTOs = await query.Distinct().ToListAsync();
            return DashboardDirector_AppUserDTOs;
        }

        [Route(DashboardDirectorRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<List<DashboardDirector_IndirectSalesOrderDTO>> ListIndirectSalesOrder()
        {
            var appUser = await AppUserService.Get(CurrentContext.UserId);
            var OrganizationIds = (await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { NotEqual = appUser.OrganizationId },
                Path = new StringFilter { StartWith = appUser.Organization.Path },
                Selects = OrganizationSelect.Id
            })).Select(x => x.Id).ToList();
            var AppUserIds = (await AppUserService.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id,
                OrganizationId = new IdFilter { In = OrganizationIds }
            })).Select(x => x.Id).ToList();
            AppUserIds.Add(appUser.Id);
            AppUserIds = AppUserIds.Distinct().ToList();
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = 5,
                Selects = IndirectSalesOrderSelect.Id | IndirectSalesOrderSelect.OrderDate
                | IndirectSalesOrderSelect.Code | IndirectSalesOrderSelect.SaleEmployee
                | IndirectSalesOrderSelect.RequestState | IndirectSalesOrderSelect.Total,
                OrderBy = IndirectSalesOrderOrder.OrderDate,
                OrderType = OrderType.DESC,
                SaleEmployeeId = new IdFilter { In = AppUserIds },
            };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<DashboardDirector_IndirectSalesOrderDTO> DashboardDirector_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(x => new DashboardDirector_IndirectSalesOrderDTO(x)).ToList();

            return DashboardDirector_IndirectSalesOrderDTOs;
        }
    }
}
