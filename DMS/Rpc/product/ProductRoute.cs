using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.product
{
    public class ProductRoute : Root
    {
        public const string Master = Module + "/product/product-master";
        public const string Detail = Module + "/product/product-detail";
        private const string Default = Rpc + Module + "/product";
        public const string Mobile = Default + "/master-data.products";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";
        public const string SaveItemImage = Default + "/save-item-image";

        public const string FilterListBrand = Default + "/filter-list-brand";
        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListSupplier = Default + "/filter-list-supplier";
        public const string FilterListTaxType = Default + "/filter-list-tax-type";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        public const string FilterListUnitOfMeasureGrouping = Default + "/filter-list-unit-of-measure-grouping";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";
        public const string FilterListUsedVariation = Default + "/filter-list-used-variation";

        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListUnitOfMeasureGrouping = Default + "/single-list-unit-of-measure-grouping";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListUsedVariation = Default + "/single-list-used-variation";

        public const string CountProductGrouping = Default + "/count-product-grouping";
        public const string ListProductGrouping = Default + "/list-product-grouping";
        public const string ListItem = Default + "/list-item";
        public const string CountItem = Default + "/count-item";
        public const string ListInventory = Default + "/list-inventory";
        public const string CountInventory = Default + "/count-inventory";
        public const string ListItemHistory = Default + "/list-item-history";
        public const string CountItemHistory = Default + "/count-item-history";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ProductFilter.ProductTypeId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.ProductGroupingId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping, CountItem, ListItem,  } },

            { "Thêm", new List<string> {
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,
                Detail, Create, SaveImage, SaveItemImage,
                CountItem, ListItem,
                SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping,
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping,  } },

            { "Sửa", new List<string> {
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,
                Detail, Update, SaveImage, SaveItemImage,
                CountItem, ListItem,
                SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping,
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping,  } },

            { "Xoá", new List<string> {
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,
                Delete,
                CountItem, ListItem,
                SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping,
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping, } },
            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get,
                CountItem, ListItem,
                FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get,
                CountItem, ListItem,
                FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get,
                CountItem, ListItem,
                FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,
                ExportTemplate, Import } },
        };
    }
}
