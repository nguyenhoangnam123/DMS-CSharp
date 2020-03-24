using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class Menu : DataEntity,  IEquatable<Menu>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }
        public List<Field> Fields { get; set; }
        public List<Page> Pages { get; set; }

        public bool Equals(Menu other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class MenuFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Path { get; set; }
        public List<MenuFilter> OrFilter { get; set; }
        public MenuOrder OrderBy {get; set;}
        public MenuSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MenuOrder
    {
        Id = 0,
        Name = 1,
        Path = 2,
        IsDeleted = 3,
    }

    [Flags]
    public enum MenuSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Path = E._2,
        IsDeleted = E._3,
    }
}
