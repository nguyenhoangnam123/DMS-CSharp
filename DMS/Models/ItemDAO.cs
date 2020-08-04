using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemDAO
    {
        public ItemDAO()
        {
            DirectPriceListItemMappings = new HashSet<DirectPriceListItemMappingDAO>();
            DirectSalesOrderContents = new HashSet<DirectSalesOrderContentDAO>();
            DirectSalesOrderPromotions = new HashSet<DirectSalesOrderPromotionDAO>();
            IndirectPriceListItemMappings = new HashSet<IndirectPriceListItemMappingDAO>();
            IndirectSalesOrderContents = new HashSet<IndirectSalesOrderContentDAO>();
            IndirectSalesOrderPromotions = new HashSet<IndirectSalesOrderPromotionDAO>();
            IndirectSalesOrderTransactions = new HashSet<IndirectSalesOrderTransactionDAO>();
            Inventories = new HashSet<InventoryDAO>();
            ItemHistories = new HashSet<ItemHistoryDAO>();
            ItemImageMappings = new HashSet<ItemImageMappingDAO>();
            KpiItemContents = new HashSet<KpiItemContentDAO>();
        }

        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }

        public virtual ProductDAO Product { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<DirectPriceListItemMappingDAO> DirectPriceListItemMappings { get; set; }
        public virtual ICollection<DirectSalesOrderContentDAO> DirectSalesOrderContents { get; set; }
        public virtual ICollection<DirectSalesOrderPromotionDAO> DirectSalesOrderPromotions { get; set; }
        public virtual ICollection<IndirectPriceListItemMappingDAO> IndirectPriceListItemMappings { get; set; }
        public virtual ICollection<IndirectSalesOrderContentDAO> IndirectSalesOrderContents { get; set; }
        public virtual ICollection<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotions { get; set; }
        public virtual ICollection<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactions { get; set; }
        public virtual ICollection<InventoryDAO> Inventories { get; set; }
        public virtual ICollection<ItemHistoryDAO> ItemHistories { get; set; }
        public virtual ICollection<ItemImageMappingDAO> ItemImageMappings { get; set; }
        public virtual ICollection<KpiItemContentDAO> KpiItemContents { get; set; }
    }
}
