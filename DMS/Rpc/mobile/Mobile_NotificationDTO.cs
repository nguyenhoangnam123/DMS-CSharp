using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_NotificationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long? OrganizationId { get; set; }
        public long NotificationStatusId { get; set; }
        public Mobile_OrganizationDTO Organization { get; set; }
        public Mobile_NotificationDTO() {}
        public Mobile_NotificationDTO(Notification Notification)
        {
            this.Id = Notification.Id;
            this.Title = Notification.Title;
            this.Content = Notification.Content;
            this.OrganizationId = Notification.OrganizationId;
            this.NotificationStatusId = Notification.NotificationStatusId;
            this.Organization = Notification.Organization == null ? null : new Mobile_OrganizationDTO(Notification.Organization);
            this.Errors = Notification.Errors;
        }
    }

    public class Mobile_NotificationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Title { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter NotificationStatusId { get; set; }
        public NotificationOrder OrderBy { get; set; }
    }
}
