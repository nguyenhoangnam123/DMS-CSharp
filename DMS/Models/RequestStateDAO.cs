using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class RequestStateDAO
    {
        public RequestStateDAO()
        {
            IndirectSalesOrders = new HashSet<IndirectSalesOrderDAO>();
            Stores = new HashSet<StoreDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrders { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
    }
}
