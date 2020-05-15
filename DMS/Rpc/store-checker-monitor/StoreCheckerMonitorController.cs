using Common;
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
                            EmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                        };
            List<StoreCheckerMonitor_StoreCheckerMonitorDTO> StoreCheckerMonitor_StoreCheckerMonitorDTOs = await query
                .Skip(StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.Skip)
                .Take(StoreCheckerMonitor_StoreCheckerMonitorFilterDTO.Take)
                .ToListAsync();
            List<long> AppUserIds = StoreCheckerMonitor_StoreCheckerMonitorDTOs.Select(s => s.EmployeeId).ToList();
            List<ERouteDAO> ERouteDAOs = await DataContext.ERoute.Where(e =>
                AppUserIds.Contains(e.SaleEmployeeId) &&
                End >= e.StartDate && (e.EndDate.HasValue == false || e.EndDate.Value >= Start))
                .Include(e => e.ERouteContents)
                .ToListAsync();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking.Where(sc => AppUserIds.Contains(sc.SaleEmployeeId)).ToListAsync();

            // khởi tạo khung dữ liệu
            foreach (StoreCheckerMonitor_StoreCheckerMonitorDTO StoreCheckerMonitor_StoreCheckerMonitorDTO in StoreCheckerMonitor_StoreCheckerMonitorDTOs)
            {
                StoreCheckerMonitor_StoreCheckerMonitorDTO.StoreCheckings = new List<StoreCheckerMonitor_StoreCheckingDTO>();
                for (DateTime i = Start; i < End; i.AddDays(1))
                {
                    StoreCheckerMonitor_StoreCheckingDTO StoreCheckerMonitor_StoreCheckingDTO = new StoreCheckerMonitor_StoreCheckingDTO();
                    StoreCheckerMonitor_StoreCheckingDTO.Date = i;
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
                    StoreCheckerMonitor_StoreCheckingDTO.PlanCounter = 0;
                    switch(i.DayOfWeek)
                    {

                    }    
                }
            }
            return null;
        }

    }

    public class EnumList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
