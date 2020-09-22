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
using DMS.Services.MPromotion;
using DMS.Services.MOrganization;
using DMS.Services.MPromotionType;
using DMS.Services.MStatus;
using DMS.Services.MPromotionCombo;
using DMS.Services.MPromotionDirectSalesOrder;
using DMS.Services.MPromotionDiscountType;
using DMS.Services.MPromotionPolicy;
using DMS.Services.MPromotionSamePrice;
using DMS.Services.MStoreGrouping;
using DMS.Services.MPromotionStoreGrouping;
using DMS.Services.MStore;
using DMS.Services.MStoreType;
using DMS.Services.MPromotionStoreType;
using DMS.Services.MPromotionStore;

namespace DMS.Rpc.promotion
{
    public class PromotionRoute : Root
    {
        public const string Master = Module + "/price-list-and-promotion/promotion/promotion-master";
        public const string Detail = Module + "/price-list-and-promotion/promotion/promotion-detail";
        private const string Default = Rpc + Module + "/promotion";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        
        public const string FilterListPromotionType = Default + "/filter-list-promotion-type";
        
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string FilterListPromotionCombo = Default + "/filter-list-promotion-combo";
        
        public const string FilterListPromotionDirectSalesOrder = Default + "/filter-list-promotion-direct-sales-order";
        
        public const string FilterListPromotionDiscountType = Default + "/filter-list-promotion-discount-type";
        
        public const string FilterListPromotionPolicy = Default + "/filter-list-promotion-policy";
        
        public const string FilterListPromotionSamePrice = Default + "/filter-list-promotion-same-price";
        
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        
        public const string FilterListPromotionStoreGrouping = Default + "/filter-list-promotion-store-grouping";
        
        public const string FilterListStore = Default + "/filter-list-store";
        
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        
        public const string FilterListPromotionStoreType = Default + "/filter-list-promotion-store-type";
        
        public const string FilterListPromotionStore = Default + "/filter-list-promotion-store";
        

        
        public const string SingleListOrganization = Default + "/single-list-organization";
        
        public const string SingleListPromotionType = Default + "/single-list-promotion-type";
        
        public const string SingleListStatus = Default + "/single-list-status";
        
        public const string SingleListPromotionCombo = Default + "/single-list-promotion-combo";
        
        public const string SingleListPromotionDirectSalesOrder = Default + "/single-list-promotion-direct-sales-order";
        
        public const string SingleListPromotionDiscountType = Default + "/single-list-promotion-discount-type";
        
        public const string SingleListPromotionPolicy = Default + "/single-list-promotion-policy";
        
        public const string SingleListPromotionSamePrice = Default + "/single-list-promotion-same-price";
        
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        
        public const string SingleListPromotionStoreGrouping = Default + "/single-list-promotion-store-grouping";
        
        public const string SingleListStore = Default + "/single-list-store";
        
        public const string SingleListStoreType = Default + "/single-list-store-type";
        
        public const string SingleListPromotionStoreType = Default + "/single-list-promotion-store-type";
        
        public const string SingleListPromotionStore = Default + "/single-list-promotion-store";
        
        public const string CountPromotionPolicy = Default + "/count-promotion-policy";
        public const string ListPromotionPolicy = Default + "/list-promotion-policy";
        public const string CountStoreGrouping = Default + "/count-store-grouping";
        public const string ListStoreGrouping = Default + "/list-store-grouping";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountStoreType = Default + "/count-store-type";
        public const string ListStoreType = Default + "/list-store-type";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(PromotionFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(PromotionFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(PromotionFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(PromotionFilter.StartDate), FieldTypeEnum.DATE.Id },
            { nameof(PromotionFilter.EndDate), FieldTypeEnum.DATE.Id },
            { nameof(PromotionFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(PromotionFilter.PromotionTypeId), FieldTypeEnum.ID.Id },
            { nameof(PromotionFilter.Note), FieldTypeEnum.STRING.Id },
            { nameof(PromotionFilter.Priority), FieldTypeEnum.LONG.Id },
            { nameof(PromotionFilter.StatusId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                Get, GetPreview,
                
                 FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore, } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore,  
                Detail, Create, 
                SingleListOrganization, SingleListPromotionType, SingleListStatus, SingleListPromotionCombo, SingleListPromotionDirectSalesOrder, SingleListPromotionDiscountType, SingleListPromotionPolicy, SingleListPromotionSamePrice, SingleListStoreGrouping, SingleListPromotionStoreGrouping, SingleListStore, SingleListStoreType, SingleListPromotionStoreType, SingleListPromotionStore, 
                Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List,  } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore,  
                Detail, Update, 
                SingleListOrganization, SingleListPromotionType, SingleListStatus, SingleListPromotionCombo, SingleListPromotionDirectSalesOrder, SingleListPromotionDiscountType, SingleListPromotionPolicy, SingleListPromotionSamePrice, SingleListStoreGrouping, SingleListPromotionStoreGrouping, SingleListStore, SingleListStoreType, SingleListPromotionStoreType, SingleListPromotionStore,  
                Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List, Count, List,  } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore,  
                Delete, 
                SingleListOrganization, SingleListPromotionType, SingleListStatus, SingleListPromotionCombo, SingleListPromotionDirectSalesOrder, SingleListPromotionDiscountType, SingleListPromotionPolicy, SingleListPromotionSamePrice, SingleListStoreGrouping, SingleListPromotionStoreGrouping, SingleListStore, SingleListStoreType, SingleListPromotionStoreType, SingleListPromotionStore,  } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore,  
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore,  
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPromotionType, FilterListStatus, FilterListPromotionCombo, FilterListPromotionDirectSalesOrder, FilterListPromotionDiscountType, FilterListPromotionPolicy, FilterListPromotionSamePrice, FilterListStoreGrouping, FilterListPromotionStoreGrouping, FilterListStore, FilterListStoreType, FilterListPromotionStoreType, FilterListPromotionStore,  
                ExportTemplate, Import } },
        };
    }
}
