using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectPriceListTypeDAO
    {
        public DirectPriceListTypeDAO()
        {
            DirectPriceLists = new HashSet<DirectPriceListDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DirectPriceListDAO> DirectPriceLists { get; set; }
    }
}
