using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Item : DataEntity, IEquatable<Item>
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long SaleStock { get; set; }
        public bool CanDelete { get; set; }
        public long StatusId { get; set; }
        public bool HasInventory { get; set; }
        public bool Used { get; set; }
        public Product Product { get; set; }
        public Status Status { get; set; }
        public List<ItemImageMapping> ItemImageMappings { get; set; }
        public bool Equals(Item other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ItemFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ProductId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter ScanCode { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter RetailPrice { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter StatusId { get; set; }
        public bool? IsNew { get; set; }
        public List<ItemFilter> OrFilter { get; set; }
        public ItemOrder OrderBy { get; set; }
        public ItemSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemOrder
    {
        Id = 0,
        Product = 1,
        Code = 2,
        Name = 3,
        ScanCode = 4,
        SalePrice = 5,
        RetailPrice = 6,
    }

    [Flags]
    public enum ItemSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Product = E._1,
        Code = E._2,
        Name = E._3,
        ScanCode = E._4,
        SalePrice = E._5,
        RetailPrice = E._6,
        Status = E._7,
        ProductId = E._8,
    }
}
