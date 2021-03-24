using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.posm_report
{
    public class POSMReport_POSMReportDTO : DataDTO
    {
        public long StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public decimal Total { get; set; }
        public List<POSMReport_POSMReportContentDTO> Contents { get; set; }
    }

    public class POSMReport_POSMReportContentDTO : DataDTO
    {
        public long ShowingItemId { get; set; }
        public string ShowingItemCode { get; set; }
        public string ShowingItemName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal SalePrice { get; set; }
        public long Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class POSMReport_POSMReportFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter ShowingItemId { get; set; }
        public DateFilter Date { get; set; }
    }
}
