using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_images
{
    public class MonitorStoreImageRoute : Root
    {
        public const string Master = Module + "/monitor-store-image/monitor-store-image-master";

        private const string Default = Rpc + Module + "/monitor-store-image";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListHasImage = Default + "/filter-list-has-image";
        public const string FilterListHasOrder = Default + "/filter-list-has-order";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(MonitorStoreImage_MonitorStoreImageFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, Export, FilterListOrganization, FilterListAppUser, FilterListStore, FilterListHasImage, FilterListHasOrder } },
        };
    }
}
