using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class InventoryHistory : DataEntity,  IEquatable<InventoryHistory>
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public long SaleStock { get; set; }
        public long AccountingStock { get; set; }
        public long AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Inventory Inventory { get; set; }

        public bool Equals(InventoryHistory other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class InventoryHistoryFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter InventoryId { get; set; }
        public LongFilter SaleStock { get; set; }
        public LongFilter AccountingStock { get; set; }
        public IdFilter AppUserId { get; set; }
        public List<InventoryHistoryFilter> OrFilter { get; set; }
        public InventoryHistoryOrder OrderBy {get; set;}
        public InventoryHistorySelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InventoryHistoryOrder
    {
        Id = 0,
        Inventory = 1,
        SaleStock = 2,
        AccountingStock = 3,
        AppUser = 4,
    }

    [Flags]
    public enum InventoryHistorySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Inventory = E._1,
        SaleStock = E._2,
        AccountingStock = E._3,
        AppUser = E._4,
    }
}
