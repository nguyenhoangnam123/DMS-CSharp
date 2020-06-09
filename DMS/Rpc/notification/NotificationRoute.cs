using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.notification
{
    public class NotificationRoute : Root
    {
        public const string Master = Module + "/notification/notification-master";
        public const string Detail = Module + "/notification/notification-detail";
        private const string Default = Rpc + Module + "/notification";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string CreateDraft = Default + "/create-draft";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListNotificationStatus = Default + "/filter-list-notification-status";

        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListNotificationStatus = Default + "/single-list-notification-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(NotificationFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(NotificationFilter.Title), FieldTypeEnum.STRING.Id },
            { nameof(NotificationFilter.Content), FieldTypeEnum.STRING.Id },
            { nameof(NotificationFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(NotificationFilter.NotificationStatusId), FieldTypeEnum.ID.Id },
        };


        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { Master, Count, List, Get, FilterListOrganization, FilterListNotificationStatus } },
            { "Thêm", new List<string> { Master, Count, List, Get,  FilterListOrganization, Detail, Create, CreateDraft, SingleListOrganization, SingleListNotificationStatus } },
            { "Sửa", new List<string> { Master, Count, List, Get,  FilterListOrganization, Detail, Update,  SingleListOrganization, SingleListNotificationStatus } },
            { "Xoá", new List<string> { Master, Count, List, Get,  FilterListOrganization, Detail, Delete,  SingleListOrganization, SingleListNotificationStatus } },
            { "Xoá nhiều", new List<string> { Master, Count, List, Get, FilterListOrganization, FilterListNotificationStatus, BulkDelete } },
            { "Xuất excel", new List<string> { Master, Count, List, Get, FilterListOrganization, FilterListNotificationStatus, Export } },
            { "Nhập excel", new List<string> { Master, Count, List, Get, FilterListOrganization, FilterListNotificationStatus, ExportTemplate, Import } },
        };
    }
}
