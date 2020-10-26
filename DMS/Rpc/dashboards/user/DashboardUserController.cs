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

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUserController : SimpleController
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
        public DashboardUserController(
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

        [Route(DashboardUserRoute.FilterListTime), HttpPost]
        public List<DashboardUser_EnumList> FilterListTime()
        {
            List<DashboardUser_EnumList> Dashborad_EnumLists = new List<DashboardUser_EnumList>();
            Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = TODAY, Name = "Hôm nay" });
            Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
            Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Dashborad_EnumLists.Add(new DashboardUser_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            return Dashborad_EnumLists;
        }

        [Route(DashboardUserRoute.SalesQuantity), HttpPost]
        public async Task<long> SalesQuantity([FromBody] DashboardUser_DashboardUserFilterDTO DashboardUser_DashboardUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start, End;
            (Start, End) = ConvertDateTime(DashboardUser_DashboardUserFilterDTO);

            var query = from i in DataContext.IndirectSalesOrder
                        join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.OrderDate >= Start && i.OrderDate <= End &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select ic;

            var results = await query.ToListAsync();
            return results.Select(x => x.RequestedQuantity).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardUserRoute.StoreChecking), HttpPost]
        public async Task<long> StoreChecking([FromBody] DashboardUser_DashboardUserFilterDTO DashboardUser_DashboardUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start, End;
            (Start, End) = ConvertDateTime(DashboardUser_DashboardUserFilterDTO);

            var query = from sc in DataContext.StoreChecking
                        where sc.SaleEmployeeId == CurrentContext.UserId &&
                        sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End
                        select sc;

            var count = await query.CountAsync();
            return count;
        }

        [Route(DashboardUserRoute.Revenue), HttpPost]
        public async Task<decimal> Revenue([FromBody] DashboardUser_DashboardUserFilterDTO DashboardUser_DashboardUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start, End;
            (Start, End) = ConvertDateTime(DashboardUser_DashboardUserFilterDTO);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.OrderDate >= Start && i.OrderDate <= End &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardUserRoute.StatisticIndirectSalesOrder), HttpPost]
        public async Task<long> StatisticIndirectSalesOrder([FromBody] DashboardUser_DashboardUserFilterDTO DashboardUser_DashboardUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start, End;
            (Start, End) = ConvertDateTime(DashboardUser_DashboardUserFilterDTO);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.OrderDate >= Start && i.OrderDate <= End &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            var count = await query.CountAsync();
            return count;
        }

        [Route(DashboardUserRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<List<DashboardUser_IndirectSalesOrderDTO>> ListIndirectSalesOrder()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        join r in DataContext.RequestState on i.RequestStateId equals r.Id into rq
                        from r in rq.DefaultIfEmpty()
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId != RequestStateEnum.NEW.Id
                        orderby i.OrderDate descending
                        select new DashboardUser_IndirectSalesOrderDTO
                        {
                            Id = i.Id,
                            Code = i.Code,
                            OrderDate = i.OrderDate,
                            RequestStateId = r.Id,
                            SaleEmployeeId = i.SaleEmployeeId,
                            BuyerStoreId = i.BuyerStoreId,
                            SellerStoreId = i.SellerStoreId,
                            Total = i.Total,
                            SaleEmployee = i.SaleEmployee == null ? null : new DashboardUser_AppUserDTO
                            {
                                Id = i.SaleEmployee.Id,
                                DisplayName = i.SaleEmployee.DisplayName,
                                Username = i.SaleEmployee.Username,
                            },
                            RequestState = new DashboardUser_RequestStateDTO
                            {
                                Id = r.Id,
                                Code = r.Code,
                                Name = r.Name,
                            },
                            BuyerStore = i.BuyerStore == null ? null : new DashboardUser_StoreDTO
                            {
                                Id = i.BuyerStore.Id,
                                Name = i.BuyerStore.Name,
                                Address = i.BuyerStore.Address,
                                OwnerEmail = i.BuyerStore.OwnerEmail,
                            },
                            SellerStore = i.SellerStore == null ? null : new DashboardUser_StoreDTO
                            {
                                Id = i.SellerStore.Id,
                                Name = i.SellerStore.Name,
                                Address = i.SellerStore.Address,
                                OwnerEmail = i.SellerStore.OwnerEmail,
                            }
                        };

            return await query.Skip(0).Take(10).ToListAsync();
        }

        [Route(DashboardUserRoute.ListComment), HttpPost]
        public async Task<List<DashboardUser_CommentDTO>> ListComment()
        {
            DashboardUser_CommentFilterDTO DashboardUser_CommentFilterDTO = new DashboardUser_CommentFilterDTO
            {
                AppUserId = CurrentContext.UserId
            };

            RestClient restClient = new RestClient(InternalServices.UTILS);
            RestRequest restRequest = new RestRequest("/rpc/utils/discussion/list-mentioned");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddJsonBody(DashboardUser_CommentFilterDTO);
            try
            {
                var response = restClient.Execute<List<DashboardUser_CommentDTO>>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Data;
                }
            }
            catch
            {
                return null;
            }
            return new List<DashboardUser_CommentDTO>();
        }

        private Tuple<DateTime, DateTime> ConvertDateTime(DashboardUser_DashboardUserFilterDTO DashboardUser_DashboardUserFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime LocalTimeNow = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);

            if (DashboardUser_DashboardUserFilterDTO.Time.Equal.HasValue == false)
            {
                DashboardUser_DashboardUserFilterDTO.Time.Equal = 0;
                Start = LocalTimeNow;
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == TODAY)
            {
                Start = LocalTimeNow;
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = LocalTimeNow.AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                End = Start.AddMonths(1).AddSeconds(-1);
            }
            else if (DashboardUser_DashboardUserFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1).AddMonths(-1).AddHours(0 - CurrentContext.TimeZone);
                End = Start.AddMonths(1).AddSeconds(-1);
            }

            return Tuple.Create(Start, End);
        }
    }
}
