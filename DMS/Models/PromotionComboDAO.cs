using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionComboDAO
    {
        public PromotionComboDAO()
        {
            Combos = new HashSet<ComboDAO>();
        }

        public long Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public long PromotionId { get; set; }
        public Guid RowId { get; set; }

        public virtual PromotionDAO Promotion { get; set; }
        public virtual ICollection<ComboDAO> Combos { get; set; }
    }
}
