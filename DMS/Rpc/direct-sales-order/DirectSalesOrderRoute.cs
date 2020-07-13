using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrderRoute : Root
    {
        public const string Master = Module + "/sale-order/direct-sales-order/direct-sales-order-master";
        public const string Detail = Module + "/sale-order/direct-sales-order/direct-sales-order-detail/*";
        private const string Default = Rpc + Module + "/direct-sales-order";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        public const string FilterListEditedPriceStatus = Default + "/filter-list-edited-price-status";
        public const string FilterListRequestState = Default + "/filter-list-request-state";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListEditedPriceStatus = Default + "/single-list-edited-price-status";
        public const string SingleListRequestState = Default + "/single-list-request-state";

        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(DirectSalesOrderFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(DirectSalesOrderFilter.BuyerStoreId), FieldTypeEnum.ID.Id },
            { nameof(DirectSalesOrderFilter.StorePhone), FieldTypeEnum.STRING.Id },
            { nameof(DirectSalesOrderFilter.StoreAddress), FieldTypeEnum.STRING.Id },
            { nameof(DirectSalesOrderFilter.StoreDeliveryAddress), FieldTypeEnum.STRING.Id },
            { nameof(DirectSalesOrderFilter.TaxCode), FieldTypeEnum.STRING.Id },
            { nameof(DirectSalesOrderFilter.SaleEmployeeId), FieldTypeEnum.ID.Id },
            { nameof(DirectSalesOrderFilter.OrderDate), FieldTypeEnum.DATE.Id },
            { nameof(DirectSalesOrderFilter.DeliveryDate), FieldTypeEnum.DATE.Id },
            { nameof(DirectSalesOrderFilter.EditedPriceStatusId), FieldTypeEnum.ID.Id },
            { nameof(DirectSalesOrderFilter.Total), FieldTypeEnum.LONG.Id },
            { nameof(DirectSalesOrderFilter.RequestStateId), FieldTypeEnum.ID.Id },
        };


        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure } },

            { "Thêm", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,
                Detail, Create,
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser, SingleListItem, SingleListProductGrouping, SingleListProductType, 
                SingleListSupplier, SingleListStoreGrouping, SingleListStoreType, SingleListUnitOfMeasure, SingleListRequestState, CountStore, ListStore, CountItem, ListItem} },

            { "Sửa", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,
                Detail, Update,
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser, SingleListItem, SingleListProductGrouping, SingleListProductType, 
                SingleListSupplier, SingleListStoreGrouping, SingleListStoreType, SingleListUnitOfMeasure, SingleListRequestState, CountStore, ListStore, CountItem, ListItem} },

            { "Xoá", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,
                Detail, Delete,
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser, SingleListItem, SingleListProductGrouping, SingleListProductType,
                SingleListSupplier, SingleListStoreGrouping, SingleListStoreType, SingleListUnitOfMeasure, SingleListRequestState, CountStore, ListStore, CountItem, ListItem} },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,
                ExportTemplate, Import } },
        };
    }
}
