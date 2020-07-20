﻿using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_checked
{
    public class ReportStoreChecked_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }
        public List<ReportStoreChecked_StoreCheckingGroupByDateDTO> StoreCheckingGroupByDates { get; set; }
    }
}