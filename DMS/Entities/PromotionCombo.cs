using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PromotionCombo : DataEntity,  IEquatable<PromotionCombo>
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public long PromotionPolicyId { get; set; }
        public long PromotionId { get; set; }
        public Guid RowId { get; set; }
        public Promotion Promotion { get; set; }
        public PromotionPolicy PromotionPolicy { get; set; }
        public List<Combo> Combos { get; set; }

        public bool Equals(PromotionCombo other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PromotionComboFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Note { get; set; }
        public IdFilter PromotionPolicyId { get; set; }
        public IdFilter PromotionId { get; set; }
        public List<PromotionComboFilter> OrFilter { get; set; }
        public PromotionComboOrder OrderBy {get; set;}
        public PromotionComboSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PromotionComboOrder
    {
        Id = 0,
        Note = 1,
        PromotionPolicy = 2,
        Promotion = 3,
    }

    [Flags]
    public enum PromotionComboSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Note = E._1,
        PromotionPolicy = E._2,
        Promotion = E._3,
    }
}
