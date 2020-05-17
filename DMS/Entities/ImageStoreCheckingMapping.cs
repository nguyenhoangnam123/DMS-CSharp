using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ImageStoreCheckingMapping : DataEntity,  IEquatable<ImageStoreCheckingMapping>
    {
        public long ImageId { get; set; }
        public long StoreCheckingId { get; set; }
        public long AlbumId { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime ShootingAt { get; set; }
        public Album Album { get; set; }
        public AppUser AppUser { get; set; }
        public Image Image { get; set; }
        public Store Store { get; set; }
        public StoreChecking StoreChecking { get; set; }

        public bool Equals(ImageStoreCheckingMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ImageStoreCheckingMappingFilter : FilterEntity
    {
        public IdFilter ImageId { get; set; }
        public IdFilter StoreCheckingId { get; set; }
        public IdFilter AlbumId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter ShootingAt { get; set; }
        public List<ImageStoreCheckingMappingFilter> OrFilter { get; set; }
        public ImageStoreCheckingMappingOrder OrderBy {get; set;}
        public ImageStoreCheckingMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageStoreCheckingMappingOrder
    {
        Image = 0,
        StoreChecking = 1,
        Album = 2,
        Store = 3,
        AppUser = 4,
        ShootingAt = 5,
    }

    [Flags]
    public enum ImageStoreCheckingMappingSelect:long
    {
        ALL = E.ALL,
        Image = E._0,
        StoreChecking = E._1,
        Album = E._2,
        Store = E._3,
        AppUser = E._4,
        ShootingAt = E._5,
    }
}
