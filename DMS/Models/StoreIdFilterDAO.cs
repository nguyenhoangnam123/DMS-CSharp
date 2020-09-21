using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreIdFilterDAO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public Guid RequestId { get; set; }
    }
}
