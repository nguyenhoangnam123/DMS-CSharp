using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class LastestEventMessageDAO
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public string RoutingKey { get; set; }
        public Guid RowId { get; set; }
        public string EntityName { get; set; }
        public string Content { get; set; }
    }
}
