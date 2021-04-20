using System;
using System.Collections.Generic;
using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ShowingOrderContentWithDraw : DataEntity,  IEquatable<ShowingOrderContentWithDraw>
    {
        public long Id { get; set; }
        public long ShowingOrderWithDrawId { get; set; }
        public long ShowingItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal SalePrice { get; set; }
        public long Quantity { get; set; }
        public decimal Amount { get; set; }
        public ShowingItem ShowingItem { get; set; }
        public ShowingOrderWithDraw ShowingOrderWithDraw { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        
        public bool Equals(ShowingOrderContentWithDraw other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ShowingOrderWithDrawId != other.ShowingOrderWithDrawId) return false;
            if (this.ShowingItemId != other.ShowingItemId) return false;
            if (this.UnitOfMeasureId != other.UnitOfMeasureId) return false;
            if (this.SalePrice != other.SalePrice) return false;
            if (this.Quantity != other.Quantity) return false;
            if (this.Amount != other.Amount) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ShowingOrderContentWithDrawFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ShowingOrderWithDrawId { get; set; }
        public IdFilter ShowingItemId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public LongFilter Quantity { get; set; }
        public DecimalFilter Amount { get; set; }
        public List<ShowingOrderContentWithDrawFilter> OrFilter { get; set; }
        public ShowingOrderContentWithDrawOrder OrderBy {get; set;}
        public ShowingOrderContentWithDrawSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShowingOrderContentWithDrawOrder
    {
        Id = 0,
        ShowingOrderWithDraw = 1,
        ShowingItem = 2,
        UnitOfMeasure = 3,
        SalePrice = 4,
        Quantity = 5,
        Amount = 6,
    }

    [Flags]
    public enum ShowingOrderContentWithDrawSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ShowingOrderWithDraw = E._1,
        ShowingItem = E._2,
        UnitOfMeasure = E._3,
        SalePrice = E._4,
        Quantity = E._5,
        Amount = E._6,
    }
}
