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

namespace DMS.Rpc.dashboards
{
    public partial class DashboardController : SimpleController
    {
        private DataContext DataContext;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IStoreCheckingService StoreCheckingService;
        public DashboardController(
            DataContext DataContext, 
            ICurrentContext CurrentContext,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IStoreCheckingService StoreCheckingService)
        {
            this.DataContext = DataContext;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.StoreCheckingService = StoreCheckingService;
        }

        [Route(DashboardRoute.StoreChecking), HttpPost]
        public async Task<Dashboard_StoreCheckingDTO> StoreChecking()
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.Id | StoreCheckingSelect.CheckOutAt,
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                CheckOutAt = new DateFilter { GreaterEqual = Now, Less = Now.AddDays(1) }
            };

            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<Dashboard_StoreCheckingHourDTO> NumberCheckingHours = StoreCheckings.GroupBy(x => x.CheckOutAt.Value.Hour)
                .Select(x => new Dashboard_StoreCheckingHourDTO { Hour = x.Key, Counter = x.Count() }).ToList();
            Dashboard_StoreCheckingDTO Dashboard_StoreCheckingDTO = new Dashboard_StoreCheckingDTO();
            Dashboard_StoreCheckingDTO.StoreCheckingHours = NumberCheckingHours;
            return Dashboard_StoreCheckingDTO;
        }

        [Route(DashboardRoute.IndirectSalesOrder), HttpPost]
        public async Task<Dashboard_IndirectSalesOrderDTO> IndirectSalesOrder()
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectSalesOrderSelect.Id | IndirectSalesOrderSelect.OrderDate,
                OrderDate = new DateFilter { GreaterEqual = Now, Less = Now.AddDays(1) },
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
            };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<Dashboard_IndirectSalesOrderHourDTO> Dashboard_IndirectSalesOrderHourDTOs = IndirectSalesOrders.GroupBy(x => x.OrderDate.Hour)
                .Select(x => new Dashboard_IndirectSalesOrderHourDTO { Hour = x.Key, Counter = x.Count() }).ToList();
            Dashboard_IndirectSalesOrderDTO Dashboard_IndirectSalesOrderDTO = new Dashboard_IndirectSalesOrderDTO();
            Dashboard_IndirectSalesOrderDTO.IndirectSalesOrderHours = Dashboard_IndirectSalesOrderHourDTOs;
            return Dashboard_IndirectSalesOrderDTO;
        }
    }
}
