using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;
using Html2Markdown;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_NotificationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long? OrganizationId { get; set; }
        public long NotificationStatusId { get; set; }
        public GeneralMobile_OrganizationDTO Organization { get; set; }
        public GeneralMobile_NotificationDTO() { }
        public GeneralMobile_NotificationDTO(Notification Notification)
        {
            this.Id = Notification.Id;
            this.Title = Notification.Title;
            this.Content = Notification.Content;
            this.OrganizationId = Notification.OrganizationId;
            this.NotificationStatusId = Notification.NotificationStatusId;
            this.Organization = Notification.Organization == null ? null : new GeneralMobile_OrganizationDTO(Notification.Organization);
            this.Errors = Notification.Errors;
        }
    }

    public class GeneralMobile_NotificationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Title { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter NotificationStatusId { get; set; }
        public NotificationOrder OrderBy { get; set; }
    }
}
