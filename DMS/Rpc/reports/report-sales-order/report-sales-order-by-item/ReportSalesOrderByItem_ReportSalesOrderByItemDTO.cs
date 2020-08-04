using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_item
{
    public class ReportSalesOrderByItem_ReportSalesOrderByItemDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportSalesOrderByItem_ItemDetailDTO> ItemDetails { get; set; }
    }

    public class ReportSalesOrderByItem_ItemDetailDTO : DataDTO
    {
        public long ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitOfMeasureName { get; set; }
        public long SaleStock { get; set; }
        public long PromotionStock { get; set; }
        public decimal Discount { get; set; }
        public decimal Revenue { get; set; }
        public int IndirectSalesOrderCounter => IndirectSalesOrderIds.Count();
        public int BuyerStoreCounter => BuyerStoreIds.Count();
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
        internal HashSet<long> BuyerStoreIds { get; set; }
    }

    public class ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public DateFilter Date { get; set; }
        internal bool HasValue => (ItemId != null && ItemId.HasValue) ||
            (ProductGroupingId != null && ProductGroupingId.HasValue) ||
            (ProductTypeId != null && ProductTypeId.HasValue) ||
            (Date != null && Date.HasValue);
    }
}
