using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ResellerDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string CompanyName { get; set; }
        public string DeputyName { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public long ResellerTypeId { get; set; }
        public long ResellerStatusId { get; set; }
        public long StaffId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ResellerStatusDAO ResellerStatus { get; set; }
        public virtual ResellerTypeDAO ResellerType { get; set; }
        public virtual AppUserDAO Staff { get; set; }
        public virtual StatusDAO Status { get; set; }
    }
}
