using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class NationTypeDAO
    {
        public NationTypeDAO()
        {
            Suppliers = new HashSet<SupplierDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
    }
}
