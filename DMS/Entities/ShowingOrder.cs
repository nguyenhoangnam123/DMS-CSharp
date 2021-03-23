using System;
using System.Collections.Generic;
using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ShowingOrder : DataEntity,  IEquatable<ShowingOrder>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long AppUserId { get; set; }
        public long OrganizationId { get; set; }
        public DateTime Date { get; set; }
        public long ShowingWarehouseId { get; set; }
        public long StatusId { get; set; }
        public decimal Total { get; set; }
        public Guid RowId { get; set; }
        public AppUser AppUser { get; set; }
        public Organization Organization { get; set; }
        public ShowingWarehouse ShowingWarehouse { get; set; }
        public Status Status { get; set; }
        public List<ShowingOrderContent> ShowingOrderContents { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(ShowingOrder other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.OrganizationId != other.OrganizationId) return false;
            if (this.Date != other.Date) return false;
            if (this.ShowingWarehouseId != other.ShowingWarehouseId) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.Total != other.Total) return false;
            if (this.RowId != other.RowId) return false;
            if (this.ShowingOrderContents?.Count != other.ShowingOrderContents?.Count) return false;
            else if (this.ShowingOrderContents != null && other.ShowingOrderContents != null)
            {
                for (int i = 0; i < ShowingOrderContents.Count; i++)
                {
                    ShowingOrderContent ShowingOrderContent = ShowingOrderContents[i];
                    ShowingOrderContent otherShowingOrderContent = other.ShowingOrderContents[i];
                    if (ShowingOrderContent == null && otherShowingOrderContent != null)
                        return false;
                    if (ShowingOrderContent != null && otherShowingOrderContent == null)
                        return false;
                    if (ShowingOrderContent.Equals(otherShowingOrderContent) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ShowingOrderFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public DateFilter Date { get; set; }
        public IdFilter ShowingWarehouseId { get; set; }
        public IdFilter StatusId { get; set; }
        public DecimalFilter Total { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ShowingOrderFilter> OrFilter { get; set; }
        public ShowingOrderOrder OrderBy {get; set;}
        public ShowingOrderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShowingOrderOrder
    {
        Id = 0,
        Code = 1,
        AppUser = 2,
        Organization = 3,
        Date = 4,
        ShowingWarehouse = 5,
        Status = 6,
        Total = 7,
        Row = 11,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ShowingOrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        AppUser = E._2,
        Organization = E._3,
        Date = E._4,
        ShowingWarehouse = E._5,
        Status = E._6,
        Total = E._7,
        Row = E._11,
    }
}
