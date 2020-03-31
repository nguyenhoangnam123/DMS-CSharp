using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SupplierDAO
    {
        public SupplierDAO()
        {
            Products = new HashSet<ProductDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
        public long WardId { get; set; }
        public string OwnerName { get; set; }
        public long PersonInChargeId { get; set; }
        public long StatusId { get; set; }
        public string Descreption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual DistrictDAO District { get; set; }
        public virtual AppUserDAO PersonInCharge { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual WardDAO Ward { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
    }
}
