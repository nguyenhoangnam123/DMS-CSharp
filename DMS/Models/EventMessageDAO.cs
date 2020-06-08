using System;

namespace DMS.Models
{
    public partial class EventMessageDAO
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }
        public string EntityName { get; set; }
        public string Content { get; set; }
    }
}
