using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class StoreChecking : DataEntity,  IEquatable<StoreChecking>
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long AppUserId { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public long? CountIndirectSalesOrder { get; set; }
        public long? CountImage { get; set; }
        public List<ImageStoreCheckingMapping> ImageStoreCheckingMappings { get; set; }

        public bool Equals(StoreChecking other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class StoreCheckingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DecimalFilter Longtitude { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DateFilter CheckInAt { get; set; }
        public DateFilter CheckOutAt { get; set; }
        public LongFilter CountIndirectSalesOrder { get; set; }
        public LongFilter CountImage { get; set; }
        public List<StoreCheckingFilter> OrFilter { get; set; }
        public StoreCheckingOrder OrderBy {get; set;}
        public StoreCheckingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreCheckingOrder
    {
        Id = 0,
        Store = 1,
        AppUser = 2,
        Longtitude = 3,
        Latitude = 4,
        CheckInAt = 5,
        CheckOutAt = 6,
        CountIndirectSalesOrder = 7,
        CountImage = 8,
    }

    [Flags]
    public enum StoreCheckingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Store = E._1,
        AppUser = E._2,
        Longtitude = E._3,
        Latitude = E._4,
        CheckInAt = E._5,
        CheckOutAt = E._6,
        CountIndirectSalesOrder = E._7,
        CountImage = E._8,
    }
}
