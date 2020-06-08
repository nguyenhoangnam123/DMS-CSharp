using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ERouteContentDAO
    {
        public long Id { get; set; }
        public long ERouteId { get; set; }
        public long StoreId { get; set; }
        public long? OrderNumber { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool Week1 { get; set; }
        public bool Week2 { get; set; }
        public bool Week3 { get; set; }
        public bool Week4 { get; set; }

        public virtual ERouteDAO ERoute { get; set; }
        public virtual StoreDAO Store { get; set; }
    }
}
