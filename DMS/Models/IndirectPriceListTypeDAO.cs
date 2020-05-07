using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectPriceListTypeDAO
    {
        public IndirectPriceListTypeDAO()
        {
            IndirectPriceLists = new HashSet<IndirectPriceListDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<IndirectPriceListDAO> IndirectPriceLists { get; set; }
    }
}
