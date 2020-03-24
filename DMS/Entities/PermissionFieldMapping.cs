using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PermissionFieldMapping : DataEntity,  IEquatable<PermissionFieldMapping>
    {
        public long PermissionId { get; set; }
        public long FieldId { get; set; }
        public string Value { get; set; }
        public Field Field { get; set; }
        public Permission Permission { get; set; }

        public bool Equals(PermissionFieldMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PermissionFieldMappingFilter : FilterEntity
    {
        public IdFilter PermissionId { get; set; }
        public IdFilter FieldId { get; set; }
        public StringFilter Value { get; set; }
        public List<PermissionFieldMappingFilter> OrFilter { get; set; }
        public PermissionFieldMappingOrder OrderBy {get; set;}
        public PermissionFieldMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionFieldMappingOrder
    {
        Permission = 0,
        Field = 1,
        Value = 2,
    }

    [Flags]
    public enum PermissionFieldMappingSelect:long
    {
        ALL = E.ALL,
        Permission = E._0,
        Field = E._1,
        Value = E._2,
    }
}
