using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class PermissionPageMapping : DataEntity, IEquatable<PermissionPageMapping>
    {
        public long PermissionId { get; set; }
        public long PageId { get; set; }
        public Page Page { get; set; }
        public Permission Permission { get; set; }

        public bool Equals(PermissionPageMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PermissionPageMappingFilter : FilterEntity
    {
        public IdFilter PermissionId { get; set; }
        public IdFilter PageId { get; set; }
        public List<PermissionPageMappingFilter> OrFilter { get; set; }
        public PermissionPageMappingOrder OrderBy { get; set; }
        public PermissionPageMappingSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionPageMappingOrder
    {
        Permission = 0,
        Page = 1,
    }

    [Flags]
    public enum PermissionPageMappingSelect : long
    {
        ALL = E.ALL,
        Permission = E._0,
        Page = E._1,
    }
}
