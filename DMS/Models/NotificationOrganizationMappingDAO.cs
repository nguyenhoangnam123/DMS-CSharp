using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class NotificationOrganizationMappingDAO
    {
        public long NotificationId { get; set; }
        public long OrganizationId { get; set; }

        public virtual NotificationDAO Notification { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
    }
}
