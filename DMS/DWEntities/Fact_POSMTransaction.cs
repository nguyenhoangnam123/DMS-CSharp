using DMS.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DMS.DWEntities
{
    public class Fact_POSMTransaction : DataEntity, IEquatable<Fact_POSMTransaction>
    {
        public long Id { get; set; }
        public long? ShowingOrderId { get; set; }
        public long? ShowingOrderWithDrawId { get; set; }
        public long POSMTransactionId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreId { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long TransactionTypeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }

        public bool Equals(Fact_POSMTransaction other)
        {
            return other != null && Id == other.Id;
        }
    }
}
