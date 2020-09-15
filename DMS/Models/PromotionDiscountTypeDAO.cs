﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionDiscountTypeDAO
    {
        public PromotionDiscountTypeDAO()
        {
            Combos = new HashSet<ComboDAO>();
            PromotionDirectSalesOrders = new HashSet<PromotionDirectSalesOrderDAO>();
            PromotionStoreGroupings = new HashSet<PromotionStoreGroupingDAO>();
            PromotionStoreTypes = new HashSet<PromotionStoreTypeDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ComboDAO> Combos { get; set; }
        public virtual ICollection<PromotionDirectSalesOrderDAO> PromotionDirectSalesOrders { get; set; }
        public virtual ICollection<PromotionStoreGroupingDAO> PromotionStoreGroupings { get; set; }
        public virtual ICollection<PromotionStoreTypeDAO> PromotionStoreTypes { get; set; }
    }
}