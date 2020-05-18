using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class NotificationDAO
    {
        public NotificationDAO()
        {
            NotificationOrganizationMappings = new HashSet<NotificationOrganizationMappingDAO>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public virtual ICollection<NotificationOrganizationMappingDAO> NotificationOrganizationMappings { get; set; }
    }
}
