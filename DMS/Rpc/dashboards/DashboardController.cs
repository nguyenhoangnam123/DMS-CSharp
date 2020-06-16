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
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.Id | StoreCheckingSelect.CheckOutAt,
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End }
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
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectSalesOrderSelect.Id | IndirectSalesOrderSelect.OrderDate,
                OrderDate = new DateFilter { GreaterEqual = Start, LessEqual = End },
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
            };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<Dashboard_IndirectSalesOrderHourDTO> Dashboard_IndirectSalesOrderHourDTOs = IndirectSalesOrders.GroupBy(x => x.OrderDate.Hour)
                .Select(x => new Dashboard_IndirectSalesOrderHourDTO { Hour = x.Key, Counter = x.Count() }).ToList();
            Dashboard_IndirectSalesOrderDTO Dashboard_IndirectSalesOrderDTO = new Dashboard_IndirectSalesOrderDTO();
            Dashboard_IndirectSalesOrderDTO.IndirectSalesOrderHours = Dashboard_IndirectSalesOrderHourDTOs;
            return Dashboard_IndirectSalesOrderDTO;
        }

        [Route(DashboardRoute.ImageStoreCheking), HttpPost]
        public async Task<Dashboard_StoreCheckingImageMappingDTO> ImageStoreCheking()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            var query = from scim in DataContext.StoreCheckingImageMapping
                        where scim.SaleEmployeeId == CurrentContext.UserId &&
                        Now <= scim.ShootingAt && scim.ShootingAt <= End
                        group scim by scim.ShootingAt.Hour into i
                        select new Dashboard_StoreCheckingImageMappingHourDTO
                        {
                            Hour = i.Key,
                            Counter = i.Count()
                        };

            List<Dashboard_StoreCheckingImageMappingHourDTO> Dashboard_StoreCheckingImageMappingHourDTOs = await query.ToListAsync();
            Dashboard_StoreCheckingImageMappingDTO Dashboard_StoreCheckingImageMappingDTO = new Dashboard_StoreCheckingImageMappingDTO();
            Dashboard_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours = Dashboard_StoreCheckingImageMappingHourDTOs;
            return Dashboard_StoreCheckingImageMappingDTO;
        }
    }
}
