using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Field : DataEntity, IEquatable<Field>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }
        public Menu Menu { get; set; }

        public bool Equals(Field other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class FieldFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Type { get; set; }
        public IdFilter MenuId { get; set; }
        public List<FieldFilter> OrFilter { get; set; }
        public FieldOrder OrderBy { get; set; }
        public FieldSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldOrder
    {
        Id = 0,
        Name = 1,
        Type = 2,
        Menu = 3,
        IsDeleted = 4,
    }

    [Flags]
    public enum FieldSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Type = E._2,
        Menu = E._3,
        IsDeleted = E._4,
    }
}
