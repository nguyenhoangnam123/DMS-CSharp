

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MProduct;
using DMS.Services.MBrand;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using DMS.Services.MTaxType;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MUsedVariation;
using DMS.Services.MItem;
using DMS.Services.MImage;
using DMS.Services.MProductGrouping;
using DMS.Services.MVariationGrouping;

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
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ProductFilter.Code), FieldType.STRING },
            { nameof(ProductFilter.SupplierCode), FieldType.STRING },
            { nameof(ProductFilter.Name), FieldType.STRING },
            { nameof(ProductFilter.Description), FieldType.STRING },
            { nameof(ProductFilter.ScanCode), FieldType.STRING },
            { nameof(ProductFilter.ProductTypeId), FieldType.ID },
            { nameof(ProductFilter.SupplierId), FieldType.ID },
            { nameof(ProductFilter.BrandId), FieldType.ID },
            { nameof(ProductFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(ProductFilter.UnitOfMeasureGroupingId), FieldType.ID },
            { nameof(ProductFilter.SalePrice), FieldType.DECIMAL },
            { nameof(ProductFilter.RetailPrice), FieldType.DECIMAL },
            { nameof(ProductFilter.TaxTypeId), FieldType.ID },
            { nameof(ProductFilter.StatusId), FieldType.ID },
            { nameof(ProductFilter.UsedVariationId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure, 
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,  } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure, 
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping, 
                Detail, Create,
                SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping,
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping,  } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure, 
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,  
                Detail, Update, 
                SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping, 
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping,  } },

            { "Xoá", new List<string> {
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure, 
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,  
                Detail, Delete,
                SingleListBrand, SingleListProductType, SingleListStatus, SingleListSupplier, SingleListTaxType, SingleListUnitOfMeasure, SingleListUnitOfMeasureGrouping,
                SingleListUsedVariation, SingleListItem, SingleListImage, SingleListProductGrouping, } },
            { "Lưu ảnh", new List<string>
            {
                SaveImage}
            },
            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListBrand, FilterListProductType, FilterListStatus, FilterListSupplier, FilterListTaxType, FilterListUnitOfMeasure,
                FilterListUnitOfMeasureGrouping, FilterListUsedVariation, FilterListItem, FilterListImage, FilterListProductGrouping,  
                ExportTemplate, Import } },
        };
    }
}
