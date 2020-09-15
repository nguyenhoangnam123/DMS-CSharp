using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionPolicyDAO
    {
        public PromotionPolicyDAO()
        {
            PromotionPromotionPolicyMappings = new HashSet<PromotionPromotionPolicyMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PromotionPromotionPolicyMappingDAO> PromotionPromotionPolicyMappings { get; set; }
    }
}
