using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Supplier : DataEntity, IEquatable<Supplier>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public long StatusId { get; set; }
        public Status Status { get; set; }

        public bool Equals(Supplier other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SupplierFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter TaxCode { get; set; }
        public IdFilter StatusId { get; set; }
        public List<SupplierFilter> OrFilter { get; set; }
        public SupplierOrder OrderBy { get; set; }
        public SupplierSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        TaxCode = 3,
        Status = 4,
    }

    [Flags]
    public enum SupplierSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        TaxCode = E._3,
        Status = E._4,
    }
}
