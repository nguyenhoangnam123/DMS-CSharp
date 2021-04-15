using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ShowingOrderWithDrawDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long AppUserId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreId { get; set; }
        public DateTime Date { get; set; }
        public long StatusId { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
    }
}
