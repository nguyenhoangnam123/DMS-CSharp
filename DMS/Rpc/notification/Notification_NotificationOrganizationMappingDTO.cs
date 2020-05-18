using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.notification
{
    public class Notification_NotificationOrganizationMappingDTO : DataDTO
    {
        public long NotificationId { get; set; }
        public long OrganizationId { get; set; }
        public Notification_NotificationDTO Notification { get; set; }
        public Notification_OrganizationDTO Organization { get; set; }
        public Notification_NotificationOrganizationMappingDTO() { }
        public Notification_NotificationOrganizationMappingDTO(NotificationOrganizationMapping NotificationOrganizationMapping)
        {
            this.NotificationId = NotificationOrganizationMapping.NotificationId;
            this.OrganizationId = NotificationOrganizationMapping.OrganizationId;
            this.Notification = NotificationOrganizationMapping.Notification == null ? null : new Notification_NotificationDTO(NotificationOrganizationMapping.Notification);
            this.Organization = NotificationOrganizationMapping.Organization == null ? null : new Notification_OrganizationDTO(NotificationOrganizationMapping.Organization);
        }
    }
}
