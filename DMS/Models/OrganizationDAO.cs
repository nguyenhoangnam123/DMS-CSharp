using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class OrganizationDAO
    {
        public OrganizationDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
            DirectSalesOrderTransactions = new HashSet<DirectSalesOrderTransactionDAO>();
            DirectSalesOrders = new HashSet<DirectSalesOrderDAO>();
            ERoutes = new HashSet<ERouteDAO>();
            IndirectSalesOrderTransactions = new HashSet<IndirectSalesOrderTransactionDAO>();
            IndirectSalesOrders = new HashSet<IndirectSalesOrderDAO>();
            InverseParent = new HashSet<OrganizationDAO>();
            KpiGenerals = new HashSet<KpiGeneralDAO>();
            KpiItems = new HashSet<KpiItemDAO>();
            Notifications = new HashSet<NotificationDAO>();
            PriceLists = new HashSet<PriceListDAO>();
            Promotions = new HashSet<PromotionDAO>();
            Resellers = new HashSet<ResellerDAO>();
            StoreScoutings = new HashSet<StoreScoutingDAO>();
            Stores = new HashSet<StoreDAO>();
            Warehouses = new HashSet<WarehouseDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public long StatusId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual OrganizationDAO Parent { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<DirectSalesOrderTransactionDAO> DirectSalesOrderTransactions { get; set; }
        public virtual ICollection<DirectSalesOrderDAO> DirectSalesOrders { get; set; }
        public virtual ICollection<ERouteDAO> ERoutes { get; set; }
        public virtual ICollection<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactions { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrders { get; set; }
        public virtual ICollection<OrganizationDAO> InverseParent { get; set; }
        public virtual ICollection<KpiGeneralDAO> KpiGenerals { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItems { get; set; }
        public virtual ICollection<NotificationDAO> Notifications { get; set; }
        public virtual ICollection<PriceListDAO> PriceLists { get; set; }
        public virtual ICollection<PromotionDAO> Promotions { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<StoreScoutingDAO> StoreScoutings { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
        public virtual ICollection<WarehouseDAO> Warehouses { get; set; }
    }
}
