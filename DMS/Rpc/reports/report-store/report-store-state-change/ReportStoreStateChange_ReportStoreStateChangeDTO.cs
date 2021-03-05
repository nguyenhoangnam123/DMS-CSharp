using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_store_state_change
{
    public class ReportStoreStateChange_ReportStoreStateChangeDTO : DataDTO
    {

    }

    public class ReportStoreStateChange_ReportStoreStateChangeFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public IdFilter StoreStatusId { get; set; }
    }
}
