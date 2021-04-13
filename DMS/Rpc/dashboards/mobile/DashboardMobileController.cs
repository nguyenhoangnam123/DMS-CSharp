using DMS.Common;
using DMS.Models;
using DMS.Services.MStoreChecking;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DMS.Services.MIndirectSalesOrder;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using RestSharp;
using DMS.Helpers;
using DMS.Enums;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobileController : SimpleController
    {
        private const long TODAY = 1;
        private const long THIS_WEEK = 2;
        private const long THIS_MONTH = 3;
        private const long LAST_MONTH = 4;
        private const long THIS_QUARTER = 5;
        private const long LAST_QUATER = 6;
        private const long YEAR = 7;

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

        [Route(DashboardMobileRoute.SingleListPeriod), HttpPost]
        public async Task<List<DashboardMobile_EnumList>> SingleListPeriod()
        {
            List<DashboardMobile_EnumList> Periods = new List<DashboardMobile_EnumList>();
            Periods.Add(new DashboardMobile_EnumList { Id = TODAY, Name = "Hôm nay" });
            Periods.Add(new DashboardMobile_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Periods.Add(new DashboardMobile_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            Periods.Add(new DashboardMobile_EnumList { Id = THIS_QUARTER, Name = "Quý này" });
            Periods.Add(new DashboardMobile_EnumList { Id = LAST_QUATER, Name = "Quý trước" });
            Periods.Add(new DashboardMobile_EnumList { Id = YEAR, Name = "Năm" });
            return Periods;
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
        } // số lượt viếng thăm

        #region IndirectSalesOrder
        [Route(DashboardMobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<long> CountIndirectSalesOrder()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId != RequestStateEnum.NEW.Id
                        select i;

            return await query.CountAsync();
        }

        [Route(DashboardMobileRoute.IndirectSalesOrderRevenue), HttpPost]
        public async Task<decimal> IndirectSalesOrderRevenue()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardMobileRoute.TopIndirectSaleEmployeeRevenue), HttpPost]
        public async Task<List<DashboardMobile_TopRevenueBySalesEmployeeDTO>> TopIndirectSaleEmployeeRevenue(DashboardMobile_TopRevenueBySalesEmployeeFilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from transaction in DataContext.IndirectSalesOrderTransaction
                        join ind in DataContext.IndirectSalesOrder on transaction.IndirectSalesOrderId equals ind.Id
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        where transaction.OrderDate >= Start
                        where transaction.OrderDate <= End
                        select transaction; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<DashboardMobile_TopRevenueBySalesEmployeeDTO> Result = await query
                .GroupBy(x => x.SalesEmployeeId)
                .Select(group => new DashboardMobile_TopRevenueBySalesEmployeeDTO
                {
                    SaleEmployeeId = group.Key,
                    Revenue = group.Where(x => x.Revenue.HasValue).Sum(y => y.Revenue.Value)
                })
                .OrderByDescending(o => o.Revenue)
                .Take(5)
                .ToListAsync(); // lấy ra top 5 nhân viên có tổng doanh thu đơn hàng cao nhất

            List<long> UserIds = Result.Select(x => x.SaleEmployeeId).ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => UserIds.Contains(x.Id))
                .ToListAsync();

            Parallel.ForEach(Result, Item =>
            {
                Item.SaleEmployeeName = AppUserDAOs
                    .Where(x => x.Id == Item.SaleEmployeeId)
                    .Select(y => y.DisplayName)
                    .FirstOrDefault();
            }); // gán tên nhân viên cho kết quả

            return Result;
        } // top 5 doanh thu đơn gián tiếp theo nhân viên

        [Route(DashboardMobileRoute.TopIndirecItemRevenue), HttpPost]
        public async Task<List<DashboardMobile_TopRevenueByItemDTO>> TopIndirecItemRevenue(DashboardMobile_TopRevenueByItemFilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from transaction in DataContext.IndirectSalesOrderTransaction
                        join ind in DataContext.IndirectSalesOrder on transaction.IndirectSalesOrderId equals ind.Id
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        where transaction.OrderDate >= Start
                        where transaction.OrderDate <= End
                        select transaction; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<DashboardMobile_TopRevenueByItemDTO> Result = await query
                .GroupBy(x => x.ItemId)
                .Select(group => new DashboardMobile_TopRevenueByItemDTO
                {
                    ItemId = group.Key,
                    Revenue = group.Where(x => x.Revenue.HasValue).Sum(y => y.Revenue.Value)
                })
                .OrderByDescending(o => o.Revenue)
                .Take(5)
                .ToListAsync(); // lấy ra top 5 nhân viên có tổng doanh thu đơn hàng cao nhất

            List<long> ItemIds = Result.Select(x => x.ItemId).ToList();

            List<ItemDAO> ItemDAOs = await DataContext.Item
                .Where(x => ItemIds.Contains(x.Id))
                .ToListAsync();

            Parallel.ForEach(Result, Item =>
            {
                Item.ItemName = ItemDAOs
                    .Where(x => x.Id == Item.ItemId)
                    .Select(y => y.Name)
                    .FirstOrDefault();
            }); // gán tên item cho kết quả

            return Result;
        } // top 5 doanh thu đơn gián tiếp theo item
        #endregion

        #region DirectSalesOrder
        [Route(DashboardMobileRoute.CountDirectSalesOrder), HttpPost]
        public async Task<long> DirectSalesOrder()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.DirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId != RequestStateEnum.NEW.Id
                        select i;

            return await query.CountAsync();
        }

        [Route(DashboardMobileRoute.DirectSalesOrderRevenue), HttpPost]
        public async Task<decimal> DirectSalesOrderRevenue()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.DirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardMobileRoute.TopDirectSaleEmployeeRevenue), HttpPost]
        public async Task<List<DashboardMobile_TopRevenueBySalesEmployeeDTO>> TopDirectSaleEmployeeRevenue(DashboardMobile_TopRevenueBySalesEmployeeFilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from transaction in DataContext.DirectSalesOrderTransaction
                        join di in DataContext.DirectSalesOrder on transaction.DirectSalesOrderId equals di.Id
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        where transaction.OrderDate >= Start
                        where transaction.OrderDate <= End
                        select transaction; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<DashboardMobile_TopRevenueBySalesEmployeeDTO> Result = await query
                .GroupBy(x => x.SalesEmployeeId)
                .Select(group => new DashboardMobile_TopRevenueBySalesEmployeeDTO
                {
                    SaleEmployeeId = group.Key,
                    Revenue = group.Where(x => x.Revenue.HasValue).Sum(y => y.Revenue.Value)
                })
                .OrderByDescending(o => o.Revenue)
                .Take(5)
                .ToListAsync(); // lấy ra top 5 nhân viên có tổng doanh thu đơn hàng cao nhất

            List<long> UserIds = Result.Select(x => x.SaleEmployeeId).ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => UserIds.Contains(x.Id))
                .ToListAsync();

            Parallel.ForEach(Result, Item =>
            {
                Item.SaleEmployeeName = AppUserDAOs
                    .Where(x => x.Id == Item.SaleEmployeeId)
                    .Select(y => y.DisplayName)
                    .FirstOrDefault();
            }); // gán tên nhân viên cho kết quả

            return Result;
        } // top 5 doanh thu đơn trực tiếp theo nhân viên

        [Route(DashboardMobileRoute.TopIndirecItemRevenue), HttpPost]
        public async Task<List<DashboardMobile_TopRevenueByItemDTO>> TopDirecItemRevenue(DashboardMobile_TopRevenueByItemFilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from transaction in DataContext.DirectSalesOrderTransaction
                        join di in DataContext.IndirectSalesOrder on transaction.DirectSalesOrderId equals di.Id
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        where transaction.OrderDate >= Start
                        where transaction.OrderDate <= End
                        select transaction; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<DashboardMobile_TopRevenueByItemDTO> Result = await query
                .GroupBy(x => x.ItemId)
                .Select(group => new DashboardMobile_TopRevenueByItemDTO
                {
                    ItemId = group.Key,
                    Revenue = group.Where(x => x.Revenue.HasValue).Sum(y => y.Revenue.Value)
                })
                .OrderByDescending(o => o.Revenue)
                .Take(5)
                .ToListAsync(); // lấy ra top 5 nhân viên có tổng doanh thu đơn hàng cao nhất

            List<long> ItemIds = Result.Select(x => x.ItemId).ToList();

            List<ItemDAO> ItemDAOs = await DataContext.Item
                .Where(x => ItemIds.Contains(x.Id))
                .ToListAsync();

            Parallel.ForEach(Result, Item =>
            {
                Item.ItemName = ItemDAOs
                    .Where(x => x.Id == Item.ItemId)
                    .Select(y => y.Name)
                    .FirstOrDefault();
            }); // gán tên item cho kết quả

            return Result;
        } // top 5 doanh thu đơn gián tiếp theo item
        #endregion

        [Route(DashboardMobileRoute.SalesQuantity), HttpPost]
        public async Task<long> SalesQuantity()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select ic;

            var results = await query.ToListAsync();
            return results.Select(x => x.RequestedQuantity).DefaultIfEmpty(0).Sum();
        }

        private Tuple<DateTime, DateTime> ConvertTime(IdFilter Time)
        {
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            DateTime Now = StaticParams.DateTimeNow;
            if (Time.Equal.HasValue == false)
            {
                Time.Equal = 0;
                Start = LocalStartDay(CurrentContext);
                End = LocalEndDay(CurrentContext);
            }
            else if (Time.Equal.Value == TODAY)
            {
                Start = LocalStartDay(CurrentContext);
                End = LocalEndDay(CurrentContext);
            }
            else if (Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.AddHours(CurrentContext.TimeZone).DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = LocalStartDay(CurrentContext).AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddMonths(-1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(-3).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == YEAR)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, 1, 1).AddYears(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            return Tuple.Create(Start, End);
        }
    }
}
