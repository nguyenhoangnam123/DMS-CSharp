using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product
{
    public class NewProductRoute : Root
    {
        public const string Master = Module + "/new-product/product-master";
        public const string Detail = Module + "/new-product/product-detail";
        private const string Default = Rpc + Module + "/new-product";
        public const string Mobile = Default + "/master-data.new-products";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListSupplier = Default + "/filter-list-supplier";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";

        public const string CountProduct = Default + "/count-product";
        public const string ListProduct = Default + "/list-product";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ProductFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ProductFilter.SupplierCode), FieldTypeEnum.STRING.Id },
            { nameof(ProductFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ProductFilter.Description), FieldTypeEnum.STRING.Id },
            { nameof(ProductFilter.ScanCode), FieldTypeEnum.STRING.Id },
            { nameof(ProductFilter.ProductTypeId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.SupplierId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.BrandId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.UnitOfMeasureId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.UnitOfMeasureGroupingId), FieldTypeEnum.ID.Id },
            { nameof(ProductFilter.StatusId), FieldTypeEnum.ID.Id },
        };
        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, 
                FilterListProductType, FilterListStatus, FilterListSupplier, FilterListProductGrouping,
                CountProduct, ListProduct} },
            { "Xoá", new List<string> {
                Master, Count, List, Get,
                FilterListProductType, FilterListStatus, FilterListSupplier, FilterListProductGrouping,
                Delete, BulkDelete,
                CountProduct, ListProduct} },
             { "Xoá nhiều", new List<string> {
                Master, Count, List, Get,
                FilterListProductType, FilterListStatus, FilterListSupplier, FilterListProductGrouping,
                BulkDelete,
                CountProduct, ListProduct} },
        };
    }
}
