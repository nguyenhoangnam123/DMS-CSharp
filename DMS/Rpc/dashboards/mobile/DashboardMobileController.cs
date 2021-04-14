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
        public async Task<long> CountStoreChecking(DashboardMobile_FilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from sc in DataContext.StoreChecking
                        where sc.SaleEmployeeId == CurrentContext.UserId
                        && (sc.CheckOutAt.HasValue && sc.CheckOutAt.Value >= Start && sc.CheckOutAt.Value <= End)
                        select sc;

            var count = await query.CountAsync();
            return count;
        } // số lượt viếng thăm

        [Route(DashboardMobileRoute.CountStore), HttpPost]
        public async Task<long> CountStore(DashboardMobile_FilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from sc in DataContext.StoreChecking
                        where sc.SaleEmployeeId == CurrentContext.UserId
                        && (sc.CheckOutAt.HasValue && sc.CheckOutAt.Value >= Start && sc.CheckOutAt.Value <= End)
                        select new
                        {
                            Id = sc.StoreId
                        };

            var count = await query
                .Distinct()
                .CountAsync();
            return count;
        } // số lượt viếng thăm

        #region IndirectSalesOrder
        [Route(DashboardMobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<long> CountIndirectSalesOrder(DashboardMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from ind in DataContext.IndirectSalesOrder
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        where ind.OrderDate >= Start
                        where ind.OrderDate <= End
                        select ind; // group transaction theo id don hang
            return await query.CountAsync();
        }

        [Route(DashboardMobileRoute.IndirectSalesOrderRevenue), HttpPost]
        public async Task<decimal> IndirectSalesOrderRevenue(DashboardMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from ind in DataContext.IndirectSalesOrder
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        where ind.OrderDate >= Start
                        where ind.OrderDate <= End
                        select ind;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total)
                .DefaultIfEmpty(0)
                .Sum();
        }

        [Route(DashboardMobileRoute.TopIndirectSaleEmployeeRevenue), HttpPost]
        public async Task<List<DashboardMobile_TopRevenueBySalesEmployeeDTO>> TopIndirectSaleEmployeeRevenue(DashboardMobile_FilterDTO filter)
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

        [Route(DashboardMobileRoute.IndirectRevenueGrowth), HttpPost]
        public async Task<DashboardMobile_RevenueGrowthDTO> IndirectRevenueGrowth([FromBody] DashboardMobile_RevenueGrowthFilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(filter.Time);

            if (filter.Time.Equal.HasValue == false
                || filter.Time.Equal.Value == THIS_MONTH)
            {
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && t.OrderDate >= Start
                            && t.OrderDate <= End
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardMobile_RevenueGrowthDTO DashboardMobile_RevenueGrowthDTO = new DashboardMobile_RevenueGrowthDTO();
                DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByMonths = new List<DashboardMobile_RevenueGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardMobile_RevenueGrowthByMonthDTO RevenueGrowthByMonth = new DashboardMobile_RevenueGrowthByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByMonths.Add(RevenueGrowthByMonth);
                }

                foreach (var RevenueGrowthByMonth in DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, (int)RevenueGrowthByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByMonth.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_MONTH)
            {
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && t.OrderDate >= Start
                            && t.OrderDate <= End
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardMobile_RevenueGrowthDTO DashboardMobile_RevenueGrowthDTO = new DashboardMobile_RevenueGrowthDTO();
                DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByMonths = new List<DashboardMobile_RevenueGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardMobile_RevenueGrowthByMonthDTO RevenueGrowthByMonth = new DashboardMobile_RevenueGrowthByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByMonths.Add(RevenueGrowthByMonth);
                }

                foreach (var RevenueGrowthByMonth in DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).AddMonths(-1).Month, (int)RevenueGrowthByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByMonth.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && t.OrderDate >= Start
                            && t.OrderDate <= End
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardMobile_RevenueGrowthDTO DashboardMobile_RevenueGrowthDTO = new DashboardMobile_RevenueGrowthDTO();
                DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByQuaters = new List<DashboardMobile_RevenueGrowthByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardMobile_RevenueGrowthByQuarterDTO RevenueGrowthByQuarter = new DashboardMobile_RevenueGrowthByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByQuaters.Add(RevenueGrowthByQuarter);
                }

                foreach (var RevenueGrowthByQuarter in DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByQuarter.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && t.OrderDate >= Start
                            && t.OrderDate <= End
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardMobile_RevenueGrowthDTO DashboardMobile_RevenueGrowthDTO = new DashboardMobile_RevenueGrowthDTO();
                DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByQuaters = new List<DashboardMobile_RevenueGrowthByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardMobile_RevenueGrowthByQuarterDTO RevenueGrowthByQuarter = new DashboardMobile_RevenueGrowthByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByQuaters.Add(RevenueGrowthByQuarter);
                }

                foreach (var RevenueGrowthByQuarter in DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByQuarter.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == YEAR)
            {
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && t.OrderDate >= Start
                            && t.OrderDate <= End
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardMobile_RevenueGrowthDTO DashboardMobile_RevenueGrowthDTO = new DashboardMobile_RevenueGrowthDTO();
                DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByYears = new List<DashboardMobile_RevenueGrowthByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    DashboardMobile_RevenueGrowthByYearDTO RevenueGrowthByYear = new DashboardMobile_RevenueGrowthByYearDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByYears.Add(RevenueGrowthByYear);
                }

                foreach (var RevenueGrowthByYear in DashboardMobile_RevenueGrowthDTO.IndirectRevenueGrowthByYears)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByYear.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByYear.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardMobile_RevenueGrowthDTO;
            }
            return new DashboardMobile_RevenueGrowthDTO();
        } // tăng trưởng doanh thu gián tiếp

        [Route(DashboardMobileRoute.IndirectQuantityGrowth), HttpPost]
        public async Task<DashboardMobile_QuantityGrowthDTO> IndirectSalesOrderGrowth([FromBody] DashboardMobile_QuantityGrowthFilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(filter.Time);

            if (filter.Time.Equal.HasValue == false
                || filter.Time.Equal.Value == THIS_MONTH)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && i.OrderDate >= Start
                            && i.OrderDate <= End
                            group i by i.OrderDate.Day into x
                            select new DashboardMobile_QuantityGrowthByMonthDTO
                            {
                                Day = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardMobile_IndirectSalesOrderGrowthByMonthDTOs = await query.ToListAsync();
                DashboardMobile_QuantityGrowthDTO DashboardMobile_QuantityGrowthDTO = new DashboardMobile_QuantityGrowthDTO();
                DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths = new List<DashboardMobile_QuantityGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardMobile_QuantityGrowthByMonthDTO IndirectSalesOrderQuantityGrowthByMonth = new DashboardMobile_QuantityGrowthByMonthDTO
                    {
                        Day = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths.Add(IndirectSalesOrderQuantityGrowthByMonth);
                }

                foreach (var IndirectSalesOrderGrowthByMonth in DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths)
                {
                    var data = DashboardMobile_IndirectSalesOrderGrowthByMonthDTOs.Where(x => x.Day == IndirectSalesOrderGrowthByMonth.Day).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByMonth.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_MONTH)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && i.OrderDate >= Start
                            && i.OrderDate <= End
                            group i by i.OrderDate.Day into x
                            select new DashboardMobile_QuantityGrowthByMonthDTO
                            {
                                Day = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardMobile_IndirectSalesOrderGrowthByMonthDTOs = await query.ToListAsync();
                DashboardMobile_QuantityGrowthDTO DashboardMobile_QuantityGrowthDTO = new DashboardMobile_QuantityGrowthDTO();
                DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths = new List<DashboardMobile_QuantityGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardMobile_QuantityGrowthByMonthDTO IndirectSalesOrderQuantityGrowthByMonth = new DashboardMobile_QuantityGrowthByMonthDTO
                    {
                        Day = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths.Add(IndirectSalesOrderQuantityGrowthByMonth);
                }

                foreach (var IndirectSalesOrderGrowthByMonth in DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths)
                {
                    var data = DashboardMobile_IndirectSalesOrderGrowthByMonthDTOs.Where(x => x.Day == IndirectSalesOrderGrowthByMonth.Day).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByMonth.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));

                var query = from i in DataContext.IndirectSalesOrder
                            where i.RequestStateId == RequestStateEnum.APPROVED.Id
                            && i.OrderDate >= Start
                            && i.OrderDate <= End
                            group i by i.OrderDate.Month into x
                            select new DashboardMobile_QuantityGrowthByQuarterDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardMobile_IndirectSalesOrderGrowthByQuarterDTOs = await query.ToListAsync();
                DashboardMobile_QuantityGrowthDTO DashboardMobile_QuantityGrowthDTO = new DashboardMobile_QuantityGrowthDTO();
                DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters = new List<DashboardMobile_QuantityGrowthByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardMobile_QuantityGrowthByQuarterDTO IndirectSalesOrderQuantityGrowthByQuarter = new DashboardMobile_QuantityGrowthByQuarterDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters.Add(IndirectSalesOrderQuantityGrowthByQuarter);
                }

                foreach (var IndirectSalesOrderGrowthByQuater in DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters)
                {
                    var data = DashboardMobile_IndirectSalesOrderGrowthByQuarterDTOs.Where(x => x.Month == IndirectSalesOrderGrowthByQuater.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByQuater.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                var query = from i in DataContext.IndirectSalesOrder
                            group i by i.OrderDate.Month into x
                            select new DashboardMobile_QuantityGrowthByQuarterDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardMobile_IndirectSalesOrderGrowthByQuarterDTOs = await query.ToListAsync();
                DashboardMobile_QuantityGrowthDTO DashboardMobile_QuantityGrowthDTO = new DashboardMobile_QuantityGrowthDTO();
                DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters = new List<DashboardMobile_QuantityGrowthByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardMobile_QuantityGrowthByQuarterDTO IndirectSalesOrderGrowthByQuarter = new DashboardMobile_QuantityGrowthByQuarterDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters.Add(IndirectSalesOrderGrowthByQuarter);
                }

                foreach (var IndirectSalesOrderGrowthByQuater in DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters)
                {
                    var data = DashboardMobile_IndirectSalesOrderGrowthByQuarterDTOs.Where(x => x.Month == IndirectSalesOrderGrowthByQuater.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByQuater.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == YEAR)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            group i by i.OrderDate.Month into x
                            select new DashboardMobile_QuantityGrowthByYearDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardMobile_IndirectSalesOrderGrowthByYearDTO = await query.ToListAsync();
                DashboardMobile_QuantityGrowthDTO DashboardMobile_QuantityGrowthDTO = new DashboardMobile_QuantityGrowthDTO();
                DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByYears = new List<DashboardMobile_QuantityGrowthByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    DashboardMobile_QuantityGrowthByYearDTO IndirectSalesOrderGrowthByYear = new DashboardMobile_QuantityGrowthByYearDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByYears.Add(IndirectSalesOrderGrowthByYear);
                }

                foreach (var IndirectSalesOrderGrowthByYear in DashboardMobile_QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByYears)
                {
                    var data = DashboardMobile_IndirectSalesOrderGrowthByYearDTO.Where(x => x.Month == IndirectSalesOrderGrowthByYear.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByYear.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardMobile_QuantityGrowthDTO;
            }
            return new DashboardMobile_QuantityGrowthDTO();
        } //  tăng trưởng số lượng đơn gián tiếp
        #endregion

        #region DirectSalesOrder
        [Route(DashboardMobileRoute.CountDirectSalesOrder), HttpPost]
        public async Task<long> DirectSalesOrder(DashboardMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from di in DataContext.DirectSalesOrder
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        where di.OrderDate >= Start
                        where di.OrderDate <= End
                        select di;
            return await query.CountAsync();
        }

        [Route(DashboardMobileRoute.DirectSalesOrderRevenue), HttpPost]
        public async Task<decimal> DirectSalesOrderRevenue(DashboardMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            var query = from di in DataContext.DirectSalesOrder
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        where di.OrderDate >= Start
                        where di.OrderDate <= End
                        select di;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total)
                .DefaultIfEmpty(0)
                .Sum();
        }

        [Route(DashboardMobileRoute.TopDirectSaleEmployeeRevenue), HttpPost]
        public async Task<List<DashboardMobile_TopRevenueBySalesEmployeeDTO>> TopDirectSaleEmployeeRevenue(DashboardMobile_FilterDTO filter)
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

        [Route(DashboardMobileRoute.TopDirecItemRevenue), HttpPost]
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
