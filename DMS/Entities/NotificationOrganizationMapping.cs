using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Entities
{
    public class NotificationOrganizationMapping : DataEntity, IEquatable<NotificationOrganizationMapping>
    {
        public long NotificationId { get; set; }
        public long OrganizationId { get; set; }
        public Notification Notification { get; set; }
        public Organization Organization { get; set; }
        public bool Equals(NotificationOrganizationMapping other)
        {
            return other != null && NotificationId == other.NotificationId && OrganizationId == other.OrganizationId;
        }
        public override int GetHashCode()
        {
            return NotificationId.GetHashCode();
        }
    }

    public class NotificationOrganizationMappingFilter : FilterEntity
    {
        public IdFilter NotificationId { get; set; }
        public IdFilter OrganizationId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationOrganizationMappingOrder
    {
        Notification = 0,
        Organization = 1,
    }

    [Flags]
    public enum NotificationOrganizationMappingSelect : long
    {
        ALL = E.ALL,
        Notification = E._0,
        Organization = E._1,
    }
}
