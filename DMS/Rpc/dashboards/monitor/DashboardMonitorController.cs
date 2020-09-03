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

namespace DMS.Rpc.dashboards.monitor
{
    public class DashboardMonitorController : RpcController
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
        public DashboardMonitorController(
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

        [Route(DashboardMonitorRoute.FilterListTime), HttpPost]
        public List<DashboardMonitor_EnumList> FilterListTime()
        {
            List<DashboardMonitor_EnumList> Dashborad_EnumLists = new List<DashboardMonitor_EnumList>();
            Dashborad_EnumLists.Add(new DashboardMonitor_EnumList { Id = TODAY, Name = "Hôm nay" });
            Dashborad_EnumLists.Add(new DashboardMonitor_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
            Dashborad_EnumLists.Add(new DashboardMonitor_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Dashborad_EnumLists.Add(new DashboardMonitor_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            return Dashborad_EnumLists;
        }

        [Route(DashboardMonitorRoute.StoreChecking), HttpPost]
        public async Task<DashboardMonitor_StoreCheckingDTO> StoreChecking()
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var AppUserIds = await DataContext.AppUser
                .Where(x => x.OrganizationId.HasValue && x.Organization.Path.StartsWith(CurrentUser.Organization.Path))
                .Select(x => x.Id)
                .ToListAsync();

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.Id | StoreCheckingSelect.CheckOutAt,
                SaleEmployeeId = new IdFilter { In = AppUserIds },
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End }
            };

            List<StoreChecking> StoreCheckings = await StoreCheckingService.List(StoreCheckingFilter);
            List<DashboardMonitor_StoreCheckingHourDTO> NumberCheckingHours = StoreCheckings.GroupBy(x => x.CheckOutAt.Value.Hour)
                .Select(x => new DashboardMonitor_StoreCheckingHourDTO { Hour = x.Key.ToString(), Counter = x.Count() }).ToList();

            DashboardMonitor_StoreCheckingDTO DashboardMonitor_StoreCheckingDTO = new DashboardMonitor_StoreCheckingDTO();
            DashboardMonitor_StoreCheckingDTO.StoreCheckingHours = new List<DashboardMonitor_StoreCheckingHourDTO>();
            for (int i = 0; i < 24; i++)
            {
                DashboardMonitor_StoreCheckingDTO.StoreCheckingHours.Add(new DashboardMonitor_StoreCheckingHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var NumberCheckingHour in NumberCheckingHours)
            {
                var x = DashboardMonitor_StoreCheckingDTO.StoreCheckingHours.Where(x => x.Hour == NumberCheckingHour.Hour).FirstOrDefault();
                x.Counter = NumberCheckingHour.Counter;
            }
            
            return DashboardMonitor_StoreCheckingDTO;
        }

        [Route(DashboardMonitorRoute.SaleEmployeeOnline), HttpPost]
        public async Task<DashboardMonitor_SaleEmployeeOnlineDTO> SaleEmployeeOnline()
        {
            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            var OnlineTime = StaticParams.DateTimeNow.AddMinutes(-30);

            var query = from au in DataContext.AppUser
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where au.DeletedAt.HasValue == false && au.StatusId == Enums.StatusEnum.ACTIVE.Id &&
                        o.Path.StartsWith(AppUser.Organization.Path) &&
                        au.UpdatedAt > OnlineTime
                        group au by au.UpdatedAt.Hour into x
                        select new DashboardMonitor_SaleEmployeeOnlineHourDTO
                        {
                            Hour = x.Key.ToString(),
                            Counter = x.Count()
                        };

            List<DashboardMonitor_SaleEmployeeOnlineHourDTO> DashboardMonitor_SaleEmployeeOnlineHourDTOs = await query.ToListAsync();
            DashboardMonitor_SaleEmployeeOnlineDTO DashboardMonitor_SaleEmployeeOnlineDTO = new DashboardMonitor_SaleEmployeeOnlineDTO();

            DashboardMonitor_SaleEmployeeOnlineDTO.SaleEmployeeOnlineHours = new List<DashboardMonitor_SaleEmployeeOnlineHourDTO>();
            for (int i = 0; i < 24; i++)
            {
                DashboardMonitor_SaleEmployeeOnlineDTO.SaleEmployeeOnlineHours.Add(new DashboardMonitor_SaleEmployeeOnlineHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var SaleEmployeeOnlineHour in DashboardMonitor_SaleEmployeeOnlineHourDTOs)
            {
                var x = DashboardMonitor_SaleEmployeeOnlineDTO.SaleEmployeeOnlineHours.Where(x => x.Hour == SaleEmployeeOnlineHour.Hour).FirstOrDefault();
                x.Counter = SaleEmployeeOnlineHour.Counter;
            }
            return DashboardMonitor_SaleEmployeeOnlineDTO;
        }

        [Route(DashboardMonitorRoute.StatisticIndirectSalesOrder), HttpPost]
        public async Task<DashboardMonitor_StatisticIndirectSalesOrderDTO> StatisticIndirectSalesOrder()
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var AppUserIds = await DataContext.AppUser
                .Where(x => x.OrganizationId.HasValue && x.Organization.Path.StartsWith(CurrentUser.Organization.Path))
                .Select(x => x.Id)
                .ToListAsync();

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectSalesOrderSelect.Id | IndirectSalesOrderSelect.OrderDate,
                OrderDate = new DateFilter { GreaterEqual = Start, LessEqual = End },
                AppUserId = new IdFilter { In = AppUserIds },
            };

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<DashboardMonitor_StatisticIndirectSalesOrderHourDTO> DashboardMonitor_StatisticIndirectSalesOrderHourDTOs = IndirectSalesOrders.GroupBy(x => x.OrderDate.Hour)
                .Select(x => new DashboardMonitor_StatisticIndirectSalesOrderHourDTO { Hour = x.Key.ToString(), Counter = x.Count() }).ToList();

            DashboardMonitor_StatisticIndirectSalesOrderDTO DashboardMonitor_StatisticIndirectSalesOrderDTO = new DashboardMonitor_StatisticIndirectSalesOrderDTO();
            DashboardMonitor_StatisticIndirectSalesOrderDTO.StatisticIndirectSalesOrderHours = new List<DashboardMonitor_StatisticIndirectSalesOrderHourDTO>();

            for (int i = 0; i < 24; i++)
            {
                DashboardMonitor_StatisticIndirectSalesOrderDTO.StatisticIndirectSalesOrderHours.Add(new DashboardMonitor_StatisticIndirectSalesOrderHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var IndirectSalesOrderHour in DashboardMonitor_StatisticIndirectSalesOrderHourDTOs)
            {
                var x = DashboardMonitor_StatisticIndirectSalesOrderDTO.StatisticIndirectSalesOrderHours.Where(x => x.Hour == IndirectSalesOrderHour.Hour).FirstOrDefault();
                x.Counter = IndirectSalesOrderHour.Counter;
            }
            return DashboardMonitor_StatisticIndirectSalesOrderDTO;
        }

        [Route(DashboardMonitorRoute.ImageStoreCheking), HttpPost]
        public async Task<DashboardMonitor_StoreCheckingImageMappingDTO> ImageStoreCheking()
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var AppUserIds = await DataContext.AppUser
                .Where(x => x.OrganizationId.HasValue && x.Organization.Path.StartsWith(CurrentUser.Organization.Path))
                .Select(x => x.Id)
                .ToListAsync();

            var query = from scim in DataContext.StoreCheckingImageMapping
                        where AppUserIds.Contains(scim.SaleEmployeeId) &&
                        Now <= scim.ShootingAt && scim.ShootingAt <= End
                        group scim by scim.ShootingAt.Hour into i
                        select new DashboardMonitor_StoreCheckingImageMappingHourDTO
                        {
                            Hour = i.Key.ToString(),
                            Counter = i.Count()
                        };

            List<DashboardMonitor_StoreCheckingImageMappingHourDTO> DashboardMonitor_StoreCheckingImageMappingHourDTOs = await query.ToListAsync();
            DashboardMonitor_StoreCheckingImageMappingDTO DashboardMonitor_StoreCheckingImageMappingDTO = new DashboardMonitor_StoreCheckingImageMappingDTO();

            DashboardMonitor_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours = new List<DashboardMonitor_StoreCheckingImageMappingHourDTO>();
            for (int i = 0; i < 24; i++)
            {
                DashboardMonitor_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours.Add(new DashboardMonitor_StoreCheckingImageMappingHourDTO
                {
                    Counter = 0,
                    Hour = i.ToString()
                });
            }
            foreach (var StoreCheckingImageMappingHour in DashboardMonitor_StoreCheckingImageMappingHourDTOs)
            {
                var x = DashboardMonitor_StoreCheckingImageMappingDTO.StoreCheckingImageMappingHours.Where(x => x.Hour == StoreCheckingImageMappingHour.Hour).FirstOrDefault();
                x.Counter = StoreCheckingImageMappingHour.Counter;
            }
            return DashboardMonitor_StoreCheckingImageMappingDTO;
        }

        [Route(DashboardMonitorRoute.StoreCoverage), HttpPost]
        public async Task<List<DashboardMonitor_StoreDTO>> StoreCoverage()
        {
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from s in DataContext.Store
                        join o in DataContext.Organization on s.OrganizationId equals o.Id
                        where o.Path.StartsWith(CurrentUser.Organization.Path)
                        select new DashboardMonitor_StoreDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Telephone = s.Telephone,
                        };
            List<DashboardMonitor_StoreDTO> DashboardMonitor_StoreDTOs = await query.Distinct().ToListAsync();
            return DashboardMonitor_StoreDTOs;
        }

        [Route(DashboardMonitorRoute.SaleEmployeeLocation), HttpPost]
        public async Task<List<DashboardMonitor_AppUserDTO>> SaleEmployeeLocation()
        {
            var AppUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from au in DataContext.AppUser
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where au.DeletedAt.HasValue == false && au.StatusId == Enums.StatusEnum.ACTIVE.Id &&
                        o.Path.StartsWith(AppUser.Organization.Path)
                        select new DashboardMonitor_AppUserDTO
                        {
                            Id = au.Id,
                            DisplayName = au.DisplayName,
                            Username = au.Username,
                            Longitude = au.Longitude,
                            Latitude = au.Latitude,
                        };

            List<DashboardMonitor_AppUserDTO> DashboardMonitor_AppUserDTOs = await query.Distinct().ToListAsync();
            return DashboardMonitor_AppUserDTOs;
        }

        [Route(DashboardMonitorRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<List<DashboardMonitor_IndirectSalesOrderDTO>> ListIndirectSalesOrder()
        {
            var appUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where appUser.OrganizationId.HasValue && o.Path.StartsWith(appUser.Organization.Path)
                        orderby i.OrderDate descending
                        select new DashboardMonitor_IndirectSalesOrderDTO
                        {
                            Id = i.Id,
                            Code = i.Code,
                            OrderDate = i.OrderDate,
                            RequestStateId = i.RequestStateId,
                            SaleEmployeeId = i.SaleEmployeeId,
                            Total = i.Total,
                            SaleEmployee = i.SaleEmployee == null ? null : new DashboardMonitor_AppUserDTO
                            {
                                Id = i.SaleEmployee.Id,
                                DisplayName = i.SaleEmployee.DisplayName,
                                Username = i.SaleEmployee.Username,
                            },
                            RequestState = i.RequestState == null ? null : new DashboardMonitor_RequestStateDTO
                            {
                                Id = i.RequestState.Id,
                                Code = i.RequestState.Code,
                                Name = i.RequestState.Name,
                            }
                        };

            return await query.Skip(0).Take(5).ToListAsync();
        }

        [Route(DashboardMonitorRoute.TopSaleEmployeeStoreChecking), HttpPost]
        public async Task<List<DashboardMonitor_TopSaleEmployeeStoreCheckingDTO>> TopSaleEmployeeStoreChecking([FromBody] DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);

            if (DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.HasValue == false)
            {
                DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal = 0;
                Start = new DateTime(Now.Year, Now.Month, Now.Day);
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == TODAY)
            {
                Start = new DateTime(Now.Year, Now.Month, Now.Day);
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = Now.AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1);
                End = Start.AddMonths(1).AddSeconds(-1);
            }
            else if (DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1).AddMonths(-1);
                End = Start.AddMonths(1).AddSeconds(-1);
            }

            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            var AppUserIds = await DataContext.AppUser
                .Where(x => x.OrganizationId.HasValue && x.Organization.Path.StartsWith(CurrentUser.Organization.Path))
                .Select(x => x.Id)
                .ToListAsync();

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.Id | StoreCheckingSelect.CheckOutAt | StoreCheckingSelect.SaleEmployee,
                CheckOutAt = new DateFilter { GreaterEqual = Start, LessEqual = End },
                SaleEmployeeId = new IdFilter { In = AppUserIds }
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

            List<DashboardMonitor_TopSaleEmployeeStoreCheckingDTO> DashboardMonitor_TopSaleEmployeeStoreCheckingDTOs = StoreCheckings.GroupBy(x => x.SaleEmployeeId)
                .Select(x => new DashboardMonitor_TopSaleEmployeeStoreCheckingDTO
                {
                    SaleEmployeeId = x.Key,
                    Counter = x.Count()
                }).ToList();
            foreach (var DashboardMonitor_TopSaleEmployeeStoreCheckingDTO in DashboardMonitor_TopSaleEmployeeStoreCheckingDTOs)
            {
                DashboardMonitor_TopSaleEmployeeStoreCheckingDTO.DisplayName = AppUsers.Where(x => x.Id == DashboardMonitor_TopSaleEmployeeStoreCheckingDTO.SaleEmployeeId)
                    .Select(x => x.DisplayName).FirstOrDefault();
            }

            return DashboardMonitor_TopSaleEmployeeStoreCheckingDTOs.OrderByDescending(x => x.Counter).Skip(0).Take(5).ToList();
        }
    }
}
