using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionTypeDAO
    {
        public PromotionTypeDAO()
        {
            Promotions = new HashSet<PromotionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PromotionDAO> Promotions { get; set; }
    }
}
