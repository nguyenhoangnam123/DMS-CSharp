using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class ERouteContentDayDAO
    {
        public long Id { get; set; }
        public long ERouteContentId { get; set; }
        public long OrderDay { get; set; }
        public bool Planned { get; set; }

        public virtual ERouteContentDAO ERouteContent { get; set; }
    }
}
