﻿using Common;
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

namespace DMS.Rpc.dashboards
{
    public partial class DashboardController : SimpleController
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
        public DashboardController(
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

        [Route(DashboardRoute.FilterListTime), HttpPost]
        public List<Dashborad_EnumList> FilterListTime()
        {
            List<Dashborad_EnumList> Dashborad_EnumLists = new List<Dashborad_EnumList>();
            Dashborad_EnumLists.Add(new Dashborad_EnumList { Id = TODAY, Name = "Hôm nay" });
            Dashborad_EnumLists.Add(new Dashborad_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
            Dashborad_EnumLists.Add(new Dashborad_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Dashborad_EnumLists.Add(new Dashborad_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            return Dashborad_EnumLists;
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
                .Select(x => new Dashboard_StoreCheckingHourDTO { Hour = x.Key.ToString(), Counter = x.Count() }).ToList();

            Dashboard_StoreCheckingDTO Dashboard_StoreCheckingDTO = new Dashboard_StoreCheckingDTO();
            Dashboard_StoreCheckingDTO.StoreCheckingHours = new List<Dashboard_StoreCheckingHourDTO>();
            for (int i = 0; i < 24; i++)
            {
                Dashboard_StoreCheckingDTO.StoreCheckingHours.Add(new Dashboard_StoreCheckingHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var NumberCheckingHour in NumberCheckingHours)
            {
                var x = Dashboard_StoreCheckingDTO.StoreCheckingHours.Where(x => x.Hour == NumberCheckingHour.Hour).FirstOrDefault();
                x.Counter = NumberCheckingHour.Counter;
            }
            
            return Dashboard_StoreCheckingDTO;
        }

        [Route(DashboardRoute.SaleEmployeeOnline), HttpPost]
        public async Task<Dashboard_SaleEmployeeOnlineDTO> SaleEmployeeOnline()
        {
            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            var OnlineTime = StaticParams.DateTimeNow.AddMinutes(-30);

            var query = from au in DataContext.AppUser
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where au.DeletedAt.HasValue == false && au.StatusId == Enums.StatusEnum.ACTIVE.Id &&
                        o.Path.StartsWith(AppUser.Organization.Path) &&
                        au.UpdatedAt > OnlineTime
                        group au by au.UpdatedAt.Hour into x
                        select new Dashboard_SaleEmployeeOnlineHourDTO
                        {
                            Hour = x.Key.ToString(),
                            Counter = x.Count()
                        };

            List<Dashboard_SaleEmployeeOnlineHourDTO> Dashboard_SaleEmployeeOnlineHourDTOs = await query.ToListAsync();
            Dashboard_SaleEmployeeOnlineDTO Dashboard_SaleEmployeeOnlineDTO = new Dashboard_SaleEmployeeOnlineDTO();

            Dashboard_SaleEmployeeOnlineDTO.SaleEmployeeOnlineHours = new List<Dashboard_SaleEmployeeOnlineHourDTO>();
            for (int i = 0; i < 24; i++)
            {
                Dashboard_SaleEmployeeOnlineDTO.SaleEmployeeOnlineHours.Add(new Dashboard_SaleEmployeeOnlineHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var SaleEmployeeOnlineHour in Dashboard_SaleEmployeeOnlineHourDTOs)
            {
                var x = Dashboard_SaleEmployeeOnlineDTO.SaleEmployeeOnlineHours.Where(x => x.Hour == SaleEmployeeOnlineHour.Hour).FirstOrDefault();
                x.Counter = SaleEmployeeOnlineHour.Counter;
            }
            return Dashboard_SaleEmployeeOnlineDTO;
        }

        [Route(DashboardRoute.StatisticIndirectSalesOrder), HttpPost]
        public async Task<Dashboard_StatisticIndirectSalesOrderDTO> StatisticIndirectSalesOrder()
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
            List<Dashboard_StatisticIndirectSalesOrderHourDTO> Dashboard_StatisticIndirectSalesOrderHourDTOs = IndirectSalesOrders.GroupBy(x => x.OrderDate.Hour)
                .Select(x => new Dashboard_StatisticIndirectSalesOrderHourDTO { Hour = x.Key.ToString(), Counter = x.Count() }).ToList();

            Dashboard_StatisticIndirectSalesOrderDTO Dashboard_StatisticIndirectSalesOrderDTO = new Dashboard_StatisticIndirectSalesOrderDTO();
            Dashboard_StatisticIndirectSalesOrderDTO.StatisticIndirectSalesOrderHours = new List<Dashboard_StatisticIndirectSalesOrderHourDTO>();

            for (int i = 0; i < 24; i++)
            {
                Dashboard_StatisticIndirectSalesOrderDTO.StatisticIndirectSalesOrderHours.Add(new Dashboard_StatisticIndirectSalesOrderHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var IndirectSalesOrderHour in Dashboard_StatisticIndirectSalesOrderHourDTOs)
            {
                var x = Dashboard_StatisticIndirectSalesOrderDTO.StatisticIndirectSalesOrderHours.Where(x => x.Hour == IndirectSalesOrderHour.Hour).FirstOrDefault();
                x.Counter = IndirectSalesOrderHour.Counter;
            }
            return Dashboard_StatisticIndirectSalesOrderDTO;
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
                            Hour = i.Key.ToString(),
                            Counter = i.Count()
                        };

            List<Dashboard_StoreCheckingImageMappingHourDTO> Dashboard_StoreCheckingImageMappingHourDTOs = await query.ToListAsync();
            Dashboard_StoreCheckingImageMappingDTO Dashboard_StoreCheckingImageMappingDTO = new Dashboard_StoreCheckingImageMappingDTO();

            Dashboard_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours = new List<Dashboard_StoreCheckingImageMappingHourDTO>();
            for (int i = 0; i < 24; i++)
            {
                Dashboard_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours.Add(new Dashboard_StoreCheckingImageMappingHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var StoreCheckingImageMappingHour in Dashboard_StoreCheckingImageMappingHourDTOs)
            {
                var x = Dashboard_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours.Where(x => x.Hour == StoreCheckingImageMappingHour.Hour).FirstOrDefault();
                x.Counter = StoreCheckingImageMappingHour.Counter;
            }
            return Dashboard_StoreCheckingImageMappingDTO;
        }

        [Route(DashboardRoute.StoreCheckingCoverage), HttpPost]
        public async Task<List<Dashboard_StoreDTO>> StoreCheckingCoverage()
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);

            var query = from s in DataContext.Store
                        join sc in DataContext.StoreChecking on s.Id equals sc.StoreId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End
                        select new Dashboard_StoreDTO 
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Telephone = s.Telephone,
                        };
            List<Dashboard_StoreDTO> Dashboard_StoreDTOs = await query.Distinct().ToListAsync();
            return Dashboard_StoreDTOs;
        }

        [Route(DashboardRoute.SaleEmployeeLocation), HttpPost]
        public async Task<List<Dashboard_AppUserDTO>> SaleEmployeeLocation()
        {
            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from au in DataContext.AppUser
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where au.DeletedAt.HasValue == false && au.StatusId == Enums.StatusEnum.ACTIVE.Id &&
                        o.Path.StartsWith(AppUser.Organization.Path)
                        select new Dashboard_AppUserDTO
                        {
                            Id = au.Id,
                            DisplayName = au.DisplayName,
                            Username = au.Username,
                            Longitude = au.Longitude,
                            Latitude = au.Latitude,
                        };

            List<Dashboard_AppUserDTO> Dashboard_AppUserDTOs = await query.Distinct().ToListAsync();
            return Dashboard_AppUserDTOs;
        }

        [Route(DashboardRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<List<Dashboard_IndirectSalesOrderDTO>> ListIndirectSalesOrder()
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
            List<Dashboard_IndirectSalesOrderDTO> Dashboard_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(x => new Dashboard_IndirectSalesOrderDTO(x)).ToList();

            return Dashboard_IndirectSalesOrderDTOs;
        }

        [Route(DashboardRoute.TopSaleEmployeeStoreChecking), HttpPost]
        public async Task<List<Dashboard_TopSaleEmployeeStoreCheckingDTO>> TopSaleEmployeeStoreChecking([FromBody] Dashboard_TopSaleEmployeeStoreCheckingFilterDTO Dashboard_TopSaleEmployeeStoreCheckingFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);

            if (Dashboard_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.HasValue == false)
            {
                Dashboard_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal = 0;
                Start = new DateTime(Now.Year, Now.Month, Now.Day);
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (Dashboard_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == TODAY)
            {
                Start = new DateTime(Now.Year, Now.Month, Now.Day);
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (Dashboard_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = Now.AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (Dashboard_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1);
                End = Start.AddMonths(1).AddSeconds(-1);
            }
            else if (Dashboard_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1).AddMonths(-1);
                End = Start.AddMonths(1).AddSeconds(-1);
            }

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.Id | StoreCheckingSelect.CheckOutAt | StoreCheckingSelect.SaleEmployee,
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End }
            };

            var StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            var SaleEmployeeIds = StoreCheckings.Select(x => x.SaleEmployeeId).Distinct().ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName,
                Id = new IdFilter { In = SaleEmployeeIds }
            };
            var AppUsers = await AppUserService.List(AppUserFilter);

            List<Dashboard_TopSaleEmployeeStoreCheckingDTO> Dashboard_TopSaleEmployeeStoreCheckingDTOs = StoreCheckings.GroupBy(x => x.SaleEmployeeId)
                .Select(x => new Dashboard_TopSaleEmployeeStoreCheckingDTO
                {
                    SaleEmployeeId = x.Key,
                    Counter = x.Count()
                }).ToList();
            foreach (var Dashboard_TopSaleEmployeeStoreCheckingDTO in Dashboard_TopSaleEmployeeStoreCheckingDTOs)
            {
                Dashboard_TopSaleEmployeeStoreCheckingDTO.DisplayName = AppUsers.Where(x => x.Id == Dashboard_TopSaleEmployeeStoreCheckingDTO.SaleEmployeeId)
                    .Select(x => x.DisplayName).FirstOrDefault();
            }

            return Dashboard_TopSaleEmployeeStoreCheckingDTOs;
        }
    }
}
