using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.notification
{
    public class Notification_NotificationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<Notification_NotificationOrganizationMappingDTO> NotificationOrganizationMappingDTOs { get; set; }
        public Notification_NotificationDTO() {}
        public Notification_NotificationDTO(Notification Notification)
        {
            this.Id = Notification.Id;
            this.Title = Notification.Title;
            this.Content = Notification.Content;
            this.NotificationOrganizationMappingDTOs = Notification.NotificationOrganizationMappings?.Select(x => new Notification_NotificationOrganizationMappingDTO(x)).ToList();
            this.Errors = Notification.Errors;
        }
    }

    public class Notification_NotificationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Title { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter OrganizationId { get; set; }
        public NotificationOrder OrderBy { get; set; }
    }
}
