using Common;
using DMS.Models;
using DMS.Services.MStoreChecking;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DMS.Services.MIndirectSalesOrder;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using RestSharp;
using DMS.Helpers;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobileController : SimpleController
    {
        //private const long TODAY = 0;
        //private const long THIS_WEEK = 1;
        //private const long THIS_MONTH = 2;
        //private const long LAST_MONTH = 3;

        private DataContext DataContext;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        private IOrganizationService OrganizationService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IStoreCheckingService StoreCheckingService;
        public DashboardMobileController(
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

        //[Route(DashboardMobileRoute.FilterListTime), HttpPost]
        //public List<DashboardUser_EnumList> FilterListTime()
        //{
        //    List<DashboardUser_EnumList> Dashborad_EnumLists = new List<DashboardUser_EnumList>();
        //    Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = TODAY, Name = "Hôm nay" });
        //    Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
        //    Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
        //    Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
        //    return Dashborad_EnumLists;
        //}

        [Route(DashboardMobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<long> CountIndirectSalesOrder()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId
                        select i;

            return await query.CountAsync();
        }

        [Route(DashboardMobileRoute.CountStoreChecking), HttpPost]
        public async Task<long> CountStoreChecking()
        {
            var query = from sc in DataContext.StoreChecking
                        where sc.SaleEmployeeId == CurrentContext.UserId &&
                        sc.CheckOutAt.HasValue
                        select sc;

            var count = await query.CountAsync();
            return count;
        }

        [Route(DashboardMobileRoute.Revenue), HttpPost]
        public async Task<decimal> Revenue()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId
                        select i;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardMobileRoute.SalesQuantity), HttpPost]
        public async Task<long> SalesQuantity()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                        where i.SaleEmployeeId == CurrentContext.UserId
                        select ic;

            var results = await query.ToListAsync();
            return results.Select(x => x.RequestedQuantity).DefaultIfEmpty(0).Sum();
        }
        //private Tuple<DateTime, DateTime> ConvertDateTime(DashboardUser_DashboardUserFilterDTO DashboardUser_DashboardUserFilterDTO)
        //{
        //    DateTime Now = StaticParams.DateTimeNow.Date;
        //    DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
        //    DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);

        //    if (DashboardUser_DashboardUserFilterDTO.Time.Equal.HasValue == false)
        //    {
        //        DashboardUser_DashboardUserFilterDTO.Time.Equal = 0;
        //        Start = new DateTime(Now.Year, Now.Month, Now.Day);
        //        End = Start.AddDays(1).AddSeconds(-1);
        //    }
        //    else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == TODAY)
        //    {
        //        Start = new DateTime(Now.Year, Now.Month, Now.Day);
        //        End = Start.AddDays(1).AddSeconds(-1);
        //    }
        //    else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == THIS_WEEK)
        //    {
        //        int diff = (7 + (Now.DayOfWeek - DayOfWeek.Monday)) % 7;
        //        Start = Now.AddDays(-1 * diff);
        //        End = Start.AddDays(7).AddSeconds(-1);
        //    }
        //    else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == THIS_MONTH)
        //    {
        //        Start = new DateTime(Now.Year, Now.Month, 1);
        //        End = Start.AddMonths(1).AddSeconds(-1);
        //    }
        //    else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == LAST_MONTH)
        //    {
        //        Start = new DateTime(Now.Year, Now.Month, 1).AddMonths(-1);
        //        End = Start.AddMonths(1).AddSeconds(-1);
        //    }

        //    return Tuple.Create(Start, End);
        //}
    }
}
