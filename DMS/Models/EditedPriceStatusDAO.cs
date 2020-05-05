﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class EditedPriceStatusDAO
    {
        public EditedPriceStatusDAO()
        {
            DirectSalesOrders = new HashSet<DirectSalesOrderDAO>();
            IndirectSalesOrders = new HashSet<IndirectSalesOrderDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DirectSalesOrderDAO> DirectSalesOrders { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrders { get; set; }
    }
}