using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            AppUserRoleMappings = new HashSet<AppUserRoleMappingDAO>();
            InventoryHistories = new HashSet<InventoryHistoryDAO>();
            Resellers = new HashSet<ResellerDAO>();
            StoreWorkflows = new HashSet<StoreWorkflowDAO>();
            Suppliers = new HashSet<SupplierDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? ProvinceId { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public long? OrganizationId { get; set; }
        public long? SexId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public Guid RowId { get; set; }

        public virtual OrganizationDAO Organization { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual SexDAO Sex { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserRoleMappingDAO> AppUserRoleMappings { get; set; }
        public virtual ICollection<InventoryHistoryDAO> InventoryHistories { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<StoreWorkflowDAO> StoreWorkflows { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
    }
}
