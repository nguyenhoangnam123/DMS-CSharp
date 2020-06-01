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
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MStore;
using DMS.Services.MEditedPriceStatus;
using DMS.Services.MRequestState;
using DMS.Services.MAppUser;
using DMS.Services.MItem;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrderRoute : Root
    {
        public const string Master = Module + "/indirect-sales-order/indirect-sales-order-master";
        public const string Detail = Module + "/indirect-sales-order/indirect-sales-order-detail";
        private const string Default = Rpc + Module + "/indirect-sales-order";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Approve = Default + "/approve";
        public const string Reject = Default + "/reject";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
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

        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(IndirectSalesOrderFilter.Code), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.BuyerStoreId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.PhoneNumber), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.StoreAddress), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.DeliveryAddress), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.SellerStoreId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.SaleEmployeeId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.OrderDate), FieldType.DATE },
            { nameof(IndirectSalesOrderFilter.DeliveryDate), FieldType.DATE },
            { nameof(IndirectSalesOrderFilter.RequestStateId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.EditedPriceStatusId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.Note), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.SubTotal), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.GeneralDiscountPercentage), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.GeneralDiscountAmount), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.TotalTaxAmount), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.Total), FieldType.LONG },
        };


        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser, FilterListItem, FilterListUnitOfMeasure,  } },

            { "Thêm", new List<string> {
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure, 
                Detail, Create, 
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser,  SingleListItem, SingleListUnitOfMeasure,
                CountItem, ListItem, CountStore, ListStore, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure, 
                Detail, Update, 
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser,  SingleListItem, SingleListUnitOfMeasure,
                CountItem, ListItem, CountStore, ListStore, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get, FilterListStore, FilterListEditedPriceStatus, FilterListRequestState, FilterListAppUser,  FilterListItem, FilterListUnitOfMeasure,  
                Detail, Delete,  
                SingleListStore, SingleListEditedPriceStatus, SingleListRequestState, SingleListAppUser,  SingleListItem, SingleListUnitOfMeasure,  } },
        };
    }
}
