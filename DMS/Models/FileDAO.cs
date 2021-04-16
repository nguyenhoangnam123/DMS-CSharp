using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class FileDAO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
    }
}
