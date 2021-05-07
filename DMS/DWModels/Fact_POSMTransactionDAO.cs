using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Fact_POSMTransactionDAO
    {
        public long Id { get; set; }
        public long? ShowingOrderId { get; set; }
        public long? ShowingOrderWithDrawId { get; set; }
        public long OrganizationId { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long TransactionTypeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public decimal Amount { get; set; }
    }
}
