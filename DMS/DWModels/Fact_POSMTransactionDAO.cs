using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Fact_POSMTransactionDAO
    {
        public long Id { get; set; }
        public long POSMTransactionId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreId { get; set; }
        public long ItemId { get; set; }
        public long TransactionTypeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
