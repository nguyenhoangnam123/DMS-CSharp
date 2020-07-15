﻿using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreChecker_ReportStoreCheckerDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportStoreChecker_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class ReportStoreChecker_ReportStoreCheckerFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public DateFilter CheckIn { get; set; }


        internal bool HasValue => (OrganizationId != null && OrganizationId.HasValue) ||
            (AppUserId != null && AppUserId.HasValue) ||
            (StoreId != null && StoreId.HasValue) ||
            (StoreTypeId != null && StoreTypeId.HasValue) ||
            (CheckIn != null && CheckIn.HasValue) ||
            (StoreGroupingId != null && StoreGroupingId.HasValue);
    }

}
