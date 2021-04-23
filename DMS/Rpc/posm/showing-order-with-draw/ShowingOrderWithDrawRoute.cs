using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DMS.Rpc.posm.showing_order_with_draw
{
    [DisplayName("Quản lý POSM")]
    public class ShowingOrderWithDrawRoute : Root
    {
        public const string Parent = Module + "/posm-order";
        public const string Master = Module + "/posm-order/showing-order-with-draw/showing-order-with-draw-master";
        public const string Detail = Module + "/posm-order/showing-order-with-draw/showing-order-with-draw-detail/*";
        public const string Preview = Module + "/showing-order/showing-order-with-draw-preview";
        private const string Default = Rpc + Module + "/showing-order-with-draw";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Export = Default + "/export";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListShowingCategory = Default + "/filter-list-showing-category";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListShowingWarehouse = Default + "/filter-list-showing-warehouse";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListShowingItem = Default + "/filter-list-showing-item";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListShowingWarehouse = Default + "/single-list-showing-warehouse";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListShowingItem = Default + "/single-list-showing-item";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";

        public const string CountShowingItem = Default + "/count-showing-item";
        public const string ListShowingItem = Default + "/list-showing-item";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ShowingOrderWithDrawFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ShowingOrderWithDrawFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ShowingOrderWithDrawFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(ShowingOrderWithDrawFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ShowingOrderWithDrawFilter.Date), FieldTypeEnum.DATE.Id },
            { nameof(ShowingOrderWithDrawFilter.ShowingWarehouseId), FieldTypeEnum.ID.Id },
            { nameof(ShowingOrderWithDrawFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(ShowingOrderWithDrawFilter.Total), FieldTypeEnum.DECIMAL.Id },
            { nameof(ShowingOrderWithDrawFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> {
            FilterListAppUser, FilterListShowingCategory, FilterListOrganization, FilterListShowingWarehouse, FilterListStatus, FilterListStore, FilterListShowingItem,
            FilterListUnitOfMeasure,
        };
        private static List<string> SingleList = new List<string> {
            SingleListAppUser, SingleListOrganization, SingleListShowingWarehouse, SingleListStatus, SingleListShowingItem, SingleListUnitOfMeasure,
        };
        private static List<string> CountList = new List<string> {
            CountShowingItem, ListShowingItem, CountStore, ListStore
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                    Parent,
                    Master, Preview, Count, List,
                    Get,
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Update,
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete,
                }.Concat(SingleList).Concat(FilterList)
            },

            { "Xuất excel", new List<string> {
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export
                }.Concat(FilterList)
            },
        };
    }
}
