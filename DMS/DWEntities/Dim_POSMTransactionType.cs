using DMS.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DMS.DWEntities
{
    public class Dim_POSMTransactionType : DataEntity, IEquatable<Dim_POSMTransactionType>
    {
        public long POSMTransactionTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(Dim_POSMTransactionType other)
        {
            return other != null && POSMTransactionTypeId == other.POSMTransactionTypeId;
        }
    }
}
