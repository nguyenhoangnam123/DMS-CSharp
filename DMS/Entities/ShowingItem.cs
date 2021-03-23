using System;
using System.Collections.Generic;
using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ShowingItem : DataEntity,  IEquatable<ShowingItem>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal SalePrice { get; set; }
        public string Desception { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Category Category { get; set; }
        public Status Status { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(ShowingItem other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.CategoryId != other.CategoryId) return false;
            if (this.UnitOfMeasureId != other.UnitOfMeasureId) return false;
            if (this.SalePrice != other.SalePrice) return false;
            if (this.Desception != other.Desception) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ShowingItemFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter CategoryId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public StringFilter Desception { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ShowingItemFilter> OrFilter { get; set; }
        public ShowingItemOrder OrderBy {get; set;}
        public ShowingItemSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShowingItemOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Category = 3,
        UnitOfMeasure = 4,
        SalePrice = 5,
        Desception = 6,
        Status = 7,
        Used = 11,
        Row = 12,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ShowingItemSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Category = E._3,
        UnitOfMeasure = E._4,
        SalePrice = E._5,
        Desception = E._6,
        Status = E._7,
        Used = E._11,
        Row = E._12,
    }
}
