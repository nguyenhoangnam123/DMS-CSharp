using DMS.Common;
using System;

namespace DMS.Entities
{
    public class POSMTransactionType : DataEntity, IEquatable<POSMTransactionType>
    {
        public long POSMTransactionTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(POSMTransactionType other)
        {
            return other != null && POSMTransactionTypeId == other.POSMTransactionTypeId;
        }
    }
}
