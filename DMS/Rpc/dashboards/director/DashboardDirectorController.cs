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
        public async Task<long> IndirectSalesOrderCounter()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        where i.OrderDate <= Start && i.OrderDate >= End &&
                        au.OrganizationId.HasValue && au.Organization.Path.StartsWith(CurrentUser.Organization.Path)
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
                        where i.OrderDate <= Start && i.OrderDate >= End &&
                        au.OrganizationId.HasValue && au.Organization.Path.StartsWith(CurrentUser.Organization.Path)
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
                        where i.OrderDate <= Start && i.OrderDate >= End &&
                        au.OrganizationId.HasValue && au.Organization.Path.StartsWith(CurrentUser.Organization.Path)
                        select ic;

            var ItemSalesTotal = query.Select(x => x.RequestedQuantity).Sum();
            return ItemSalesTotal;
        }

        [Route(DashboardDirectorRoute.StoreCheckingCounter), HttpPost]
        public async Task<long> StoreCheckingCounter()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from sc in DataContext.StoreChecking
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt <= Start && sc.CheckOutAt >= End &&
                        au.OrganizationId.HasValue && au.Organization.Path.StartsWith(CurrentUser.Organization.Path)
                        select sc;

            var StoreCheckingCounter = query.Count();
            return StoreCheckingCounter;
        }
    }
}
