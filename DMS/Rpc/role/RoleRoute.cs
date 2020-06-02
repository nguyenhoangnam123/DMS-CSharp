﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.role
{
    public class RoleRoute : Root
    {
        public const string Master = Module + "/role/role-master";
        public const string Detail = Module + "/role/role-detail";
        private const string Default = Rpc + Module + "/role";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string AssignAppUser = Default + "/assign-app-user";
        public const string GetMenu = Default + "/get-menu";
        public const string CreatePermission = Default + "/create-permission";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListMenu = Default + "/single-list-menu";
        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProduct = Default + "/single-list-product";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListReseller = Default + "/single-list-reseller";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListWarehouse = Default + "/single-list-warehouse";

        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(RoleFilter.Code), FieldType.STRING },
            { nameof(RoleFilter.Name), FieldType.STRING },
            { nameof(RoleFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, 
                SingleListAppUser, SingleListStatus, SingleListMenu, SingleListBrand, SingleListOrganization, SingleListProduct, SingleListProductGrouping, SingleListProductType, SingleListReseller,
                SingleListStore, SingleListStoreGrouping, SingleListStoreType, SingleListSupplier, SingleListWarehouse } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get,
                SingleListAppUser, SingleListStatus, SingleListMenu, SingleListBrand, SingleListOrganization, SingleListProduct, SingleListProductGrouping, SingleListProductType, SingleListReseller,
                SingleListStore, SingleListStoreGrouping, SingleListStoreType, SingleListSupplier, SingleListWarehouse, 
                Detail, Create, GetMenu,
                SingleListStatus } },
            { "Sửa", new List<string> { 
                Master, Count, List, Get,
                SingleListAppUser, SingleListStatus, SingleListMenu, SingleListBrand, SingleListOrganization, SingleListProduct, SingleListProductGrouping, SingleListProductType, SingleListReseller,
                SingleListStore, SingleListStoreGrouping, SingleListStoreType, SingleListSupplier, SingleListWarehouse,
                Detail, Update, GetMenu,
                 } },
             { "Gán người dùng", new List<string> {
                Master, Count, List, Get,
                CountAppUser, ListAppUser,
                SingleListAppUser, SingleListStatus, SingleListMenu, SingleListBrand, SingleListOrganization, SingleListProduct, SingleListProductGrouping, SingleListProductType, SingleListReseller,
                SingleListStore, SingleListStoreGrouping, SingleListStoreType, SingleListSupplier, SingleListWarehouse,
                Detail, AssignAppUser,
                SingleListAppUser, SingleListMenu, SingleListStatus } },
             { "Tạo nhanh quyền", new List<string> {
                Master, Count, List, Get,
                Detail, CreatePermission, GetMenu,
                SingleListAppUser, SingleListMenu, SingleListStatus } },
            { "Xoá", new List<string> { 
                Master, Count, List, Get,
                SingleListAppUser, SingleListStatus, SingleListMenu, SingleListBrand, SingleListOrganization, SingleListProduct, SingleListProductGrouping, SingleListProductType, SingleListReseller,
                SingleListStore, SingleListStoreGrouping, SingleListStoreType, SingleListSupplier, SingleListWarehouse,
                Detail, Delete, 
                SingleListStatus } },
        };
    }
}
