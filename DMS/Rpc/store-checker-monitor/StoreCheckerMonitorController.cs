using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checker_monitor
{
    // Report
    public class StoreCheckerMonitorRoute : Root
    {
        public const string Master = Module + "/store-checker-monitor/store-checker-monitor-master";
        public const string Detail = Module + "/store-checker-monitor/store-checker-monitor-detail";

        private const string Default = Rpc + Module + "/store-checker-monitor";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListChecking = Default + "/filter-list-checking";
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListmageSalesOrder = Default + "/filter-list-sales-order";
    }

    public class StoreCheckerMonitorController : RpcController
    {
        private DataContext DataContext;
        public StoreCheckerMonitorController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public List<EnumList> FilterListChecking()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Chưa viếng thăm" });
            EnumList.Add(new EnumList { Id = 1, Name = "Đã viếng thăm" });
            return EnumList;
        }
        public List<EnumList> FilterListImage()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không hình ảnh" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có hình ảnh" });
            return EnumList;
        }

        public List<EnumList> FilterListmageSalesOrder()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không có đơn hàng" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có đơn hàng" });
            return EnumList;
        }

        public async Task<int> Count([FromBody] StoreCheckerMonitor_StoreCheckerMonitorFilterDTO StoreCheckerMonitor_StoreCheckerMonitorFilterDTO)
        {
            var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue
                        select ap.Id;
            return await query.Distinct().CountAsync();
        }
        public async Task<List<StoreCheckerMonitor_StoreCheckerMonitorDTO>> List([FromBody] StoreCheckerMonitor_StoreCheckerMonitorFilterDTO StoreCheckerMonitor_StoreCheckerMonitorFilterDTO)
        {
            DateTime Start = StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.CheckIn.GreaterEqual.HasValue ?
               StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.CheckIn.GreaterEqual.Value :
               StaticParams.DateTimeNow;
            DateTime End = StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.CheckIn.LessEqual.HasValue ?
                StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.CheckIn.LessEqual.Value :
                StaticParams.DateTimeNow;
            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End
                        select new StoreCheckerMonitor_StoreCheckerMonitorDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                        };
            List<StoreCheckerMonitor_StoreCheckerMonitorDTO> StoreCheckerMonitor_StoreCheckerMonitorDTOs = await query
                .Skip(StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.Skip)
                .Take(StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.Take)
                .ToListAsync();
            List<long> AppUserIds = StoreCheckerMonitor_StoreCheckerMonitorDTOs.Select(s => s.SaleEmployeeId).ToList();
            List<ERouteContent> ERouteContents = await (from ec in DataContext.ERouteContent
                                                        join e in DataContext.ERoute on ec.ERouteId equals e.Id
                                                        where AppUserIds.Contains(e.SaleEmployeeId) &&
                                                        End >= e.StartDate && (e.EndDate.HasValue == false || e.EndDate.Value >= Start)
                                                        select new ERouteContent
                                                        {
                                                            Id = ec.Id,
                                                            Monday = ec.Monday,
                                                            Tuesday = ec.Tuesday,
                                                            Wednesday = ec.Wednesday,
                                                            Thursday = ec.Thursday,
                                                            Friday = ec.Friday,
                                                            Saturday = ec.Saturday,
                                                            Sunday = ec.Sunday,
                                                            Week1 = ec.Week1,
                                                            Week2 = ec.Week2,
                                                            Week3 = ec.Week3,
                                                            Week4 = ec.Week4,
                                                            StoreId = ec.StoreId,
                                                            ERoute = new ERoute
                                                            {
                                                                StartDate = e.StartDate,
                                                                SaleEmployeeId = e.SaleEmployeeId,
                                                            }
                                                        }).ToListAsync();
            List<StoreCheckingDAO> StoreCheckingDAOs = new List<StoreCheckingDAO>();
            var StoreCheckingQuery = DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && sc.CheckOutAt.HasValue);
            // khong check
            if (StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.Checking.Equal == 0)
            { 
            }    
            else
            {
                if (StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.Image.Equal == 0)
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.CountImage == 0);
                }
                else
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.CountImage > 0);
                }

                if (StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.SalesOrder.Equal == 0)
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.CountIndirectSalesOrder == 0);
                }
                else
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.CountIndirectSalesOrder > 0);
                }
                StoreCheckingDAOs = await StoreCheckingQuery.ToListAsync();
            }    

            // khởi tạo khung dữ liệu
            foreach (StoreCheckerMonitor_StoreCheckerMonitorDTO StoreCheckerMonitor_StoreCheckerMonitorDTO in StoreCheckerMonitor_StoreCheckerMonitorDTOs)
            {
                StoreCheckerMonitor_StoreCheckerMonitorDTO.StoreCheckings = new List<StoreCheckerMonitor_StoreCheckingDTO>();
                for (DateTime i = Start; i < End; i.AddDays(1))
                {
                    StoreCheckerMonitor_StoreCheckingDTO StoreCheckerMonitor_StoreCheckingDTO = new StoreCheckerMonitor_StoreCheckingDTO();
                    StoreCheckerMonitor_StoreCheckingDTO.Date = i;
                    StoreCheckerMonitor_StoreCheckingDTO.Image = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.SalesOrder = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckingDTO.Plan = new HashSet<long>();
                    StoreCheckerMonitor_StoreCheckerMonitorDTO.StoreCheckings.Add(StoreCheckerMonitor_StoreCheckingDTO);
                }
            }

            // khởi tạo kế hoạch
            foreach (StoreCheckerMonitor_StoreCheckerMonitorDTO StoreCheckerMonitor_StoreCheckerMonitorDTO in StoreCheckerMonitor_StoreCheckerMonitorDTOs)
            {
                for (DateTime i = Start; i < End; i.AddDays(1))
                {
                    StoreCheckerMonitor_StoreCheckingDTO StoreCheckerMonitor_StoreCheckingDTO = StoreCheckerMonitor_StoreCheckerMonitorDTO.StoreCheckings
                        .Where(s => s.Date == i).FirstOrDefault();
                    foreach (ERouteContent ERouteContent in ERouteContents)
                    {
                        bool PlanWeek = false, PlanDay = false;
                        int week = Convert.ToInt32(StaticParams.DateTimeNow.Subtract(ERouteContent.ERoute.StartDate).TotalDays / 7) + 1;
                        switch (week / 4)
                        {
                            case 1:
                                if (ERouteContent.Week1)
                                    PlanWeek = true;
                                break;
                            case 2:
                                if (ERouteContent.Week2)
                                    PlanWeek = true;
                                break;
                            case 3:
                                if (ERouteContent.Week3)
                                    PlanWeek = true;
                                break;
                            case 0:
                                if (ERouteContent.Week4)
                                    PlanWeek = true;
                                break;
                        }
                        DayOfWeek day = StaticParams.DateTimeNow.DayOfWeek;
                        switch (day)
                        {
                            case DayOfWeek.Sunday:
                                if (ERouteContent.Sunday)
                                    PlanDay = true;
                                break;
                            case DayOfWeek.Monday:
                                if (ERouteContent.Monday)
                                    PlanDay = true;
                                break;
                            case DayOfWeek.Tuesday:
                                if (ERouteContent.Tuesday)
                                    PlanDay = true;
                                break;
                            case DayOfWeek.Wednesday:
                                if (ERouteContent.Wednesday)
                                    PlanDay = true;
                                break;
                            case DayOfWeek.Thursday:
                                if (ERouteContent.Thursday)
                                    PlanDay = true;
                                break;
                            case DayOfWeek.Friday:
                                if (ERouteContent.Friday)
                                    PlanDay = true;
                                break;
                            case DayOfWeek.Saturday:
                                if (ERouteContent.Saturday)
                                    PlanDay = true;
                                break;
                        }
                        if (PlanDay && PlanWeek)
                            StoreCheckerMonitor_StoreCheckingDTO.Plan.Add(ERouteContent.StoreId);
                    }
                    List<StoreCheckingDAO> Checked = StoreCheckingDAOs
                           .Where(s =>
                               s.SaleEmployeeId == StoreCheckerMonitor_StoreCheckerMonitorDTO.SaleEmployeeId &&
                               s.CheckOutAt.Value == i
                           ).ToList();
                    foreach (StoreCheckingDAO s in Checked)
                    {
                        if (StoreCheckerMonitor_StoreCheckingDTO.Plan.Contains(s.StoreId))
                            StoreCheckerMonitor_StoreCheckingDTO.Internal.Add(s.StoreId);
                        else
                            StoreCheckerMonitor_StoreCheckingDTO.External.Add(s.StoreId);
                        if (s.CountImage > 0)
                            StoreCheckerMonitor_StoreCheckingDTO.Image.Add(s.StoreId);
                        if (s.CountIndirectSalesOrder > 0)
                            StoreCheckerMonitor_StoreCheckingDTO.SalesOrder.Add(s.StoreId);
                    }
                }
            }
            return StoreCheckerMonitor_StoreCheckerMonitorDTOs;
        }

        public class EnumList
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
