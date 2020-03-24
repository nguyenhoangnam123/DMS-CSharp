using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ProductType : DataEntity,  IEquatable<ProductType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public Status Status { get; set; }

        public bool Equals(ProductType other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ProductTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter StatusId { get; set; }
        public List<ProductTypeFilter> OrFilter { get; set; }
        public ProductTypeOrder OrderBy {get; set;}
        public ProductTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Description = 3,
        Status = 4,
    }

    [Flags]
    public enum ProductTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Description = E._3,
        Status = E._4,
    }
}
