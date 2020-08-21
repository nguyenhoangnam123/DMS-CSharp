using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store_checking.report_store_checked
{
    public class ReportStoreChecked_ExportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Date { get; set; }
        public string DayOfWeek { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string CheckInDistance { get; set; }
        public string CheckOutDistance { get; set; }
        public string Duration { get; set; }
        public string Device { get; set; }
        public string Planned { get; set; }
        public string Image { get; set; }
        public string Order { get; set; }
    }
}
