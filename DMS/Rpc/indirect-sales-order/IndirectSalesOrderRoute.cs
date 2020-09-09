using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrderRoute : Root
    {
        public const string Parent = Module + "/sale-order";
        public const string Master = Module + "/sale-order/indirect-sales-order/indirect-sales-order-master";
        public const string Detail = Module + "/sale-order/indirect-sales-order/indirect-sales-order-detail/*";
        public const string Mobile = Module + ".indirect-sales-order.*";
        private const string Default = Rpc + Module + "/indirect-sales-order";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";

        public const string CountNew = Default + "/count-new";
        public const string ListNew = Default + "/list-new";
        public const string CountPending = Default + "/count-pending";
        public const string ListPending = Default + "/list-pending";
        public const string CountCompleted = Default + "/count-completed";
        public const string ListCompleted = Default + "/list-completed";

        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Send = Default + "/send";
        public const string Approve = Default + "/approve";
        public const string Reject = Default + "/reject";
        public const string Export = Default + "/export";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        public const string FilterListEditedPriceStatus = Default + "/filter-list-edit-price-status";
        public const string FilterListRequestState = Default + "/filter-list-request-state";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListIndirectSalesOrderStatus = Default + "/single-list-indirect-sales-order-status";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListEditedPriceStatus = Default + "/single-list-edit-price-status";
        public const string SingleListRequestState = Default + "/single-list-request-state";
        public const string SingleListTaxType = Default + "/single-list-tax-type";

        public const string CountBuyerStore = Default + "/count-buyer-store";
        public const string ListBuyerStore = Default + "/list-buyer-store";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(IndirectSalesOrderFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(IndirectSalesOrderFilter.BuyerStoreId), FieldTypeEnum.ID.Id },
            { nameof(IndirectSalesOrderFilter.SellerStoreId), FieldTypeEnum.ID.Id },
            { nameof(IndirectSalesOrderFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(IndirectSalesOrderFilter.OrderDate), FieldTypeEnum.DATE.Id },
            { nameof(IndirectSalesOrderFilter.RequestStateId), FieldTypeEnum.ID.Id },
            { nameof(IndirectSalesOrderFilter.Total), FieldTypeEnum.LONG.Id },
            { nameof(ItemFilter.ProductGroupingId), FieldTypeEnum.ID.Id },
            { nameof(ItemFilter.ProductTypeId), FieldTypeEnum.ID.Id },
            { nameof(ItemFilter.SalePrice), FieldTypeEnum.LONG.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };


        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Get,
                CountNew, ListNew, CountPending, ListPending, CountCompleted, ListCompleted,
                FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure, FilterListOrganization,
                CountItem, ListItem,} },

            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, Get,
                CountNew, ListNew, CountPending, ListPending, CountCompleted, ListCompleted,
                FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure, FilterListOrganization,
                Detail, Create,
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser,  SingleListItem, SingleListUnitOfMeasure, SingleListStoreType,
                SingleListStoreGrouping, SingleListSupplier, SingleListProductGrouping, SingleListProductType, SingleListTaxType,
                CountItem, ListItem, CountStore, ListStore, CountBuyerStore, ListBuyerStore} },

            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, Get,
                CountNew, ListNew, CountPending, ListPending, CountCompleted, ListCompleted,
                FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure, FilterListOrganization,
                Detail, Update,
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser,  SingleListItem, SingleListUnitOfMeasure, SingleListStoreType,
                SingleListStoreGrouping, SingleListSupplier, SingleListProductGrouping, SingleListProductType, SingleListTaxType,
                CountItem, ListItem, CountStore, ListStore, CountBuyerStore, ListBuyerStore} },

            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, Get,
                CountNew, ListNew, CountPending, ListPending, CountCompleted, ListCompleted,
                FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure, FilterListOrganization,
                Delete,
                } },

            { "Xuất excel", new List<string> {
                Parent,
                Master, Count, List, Get,
                CountNew, ListNew, CountPending, ListPending, CountCompleted, ListCompleted,
                FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure, FilterListOrganization,
                Export } },
            { "Phê duyệt", new List<string>
            {
                Parent,
                Master, Count, List, Get,
                CountNew, ListNew, CountPending, ListPending, CountCompleted, ListCompleted,
                FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure, FilterListOrganization,
                Detail, Approve, Reject, Send,
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser,  SingleListItem, SingleListUnitOfMeasure, SingleListStoreType,
                SingleListStoreGrouping, SingleListSupplier, SingleListProductGrouping, SingleListProductType, SingleListTaxType,
                CountItem, ListItem, CountStore, ListStore, CountBuyerStore, ListBuyerStore
            } },
        };
    }
}
