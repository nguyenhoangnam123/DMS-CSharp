﻿using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class RoutingKeyEnum
    {
        public static GenericEnum AppUserSync = new GenericEnum { Id = 1, Code = "AppUser.Sync", Name = "Đồng bộ AppUser" };
        public static GenericEnum OrganizationSync = new GenericEnum { Id = 2, Code = "Organization.Sync", Name = "Đồng bộ Organization" };
        public static GenericEnum StorenSync = new GenericEnum { Id = 3, Code = "Store.Sync", Name = "Đồng bộ Store" };
        public static GenericEnum SendMail = new GenericEnum { Id = 4, Code = "Mail.Send", Name = "Gửi Mail" };
        public static GenericEnum AuditLogSend = new GenericEnum { Id = 5, Code = "AuditLog.Send", Name = "Audit Log" };
        public static GenericEnum SystemLogSend = new GenericEnum { Id = 6, Code = "SystemLog.Send", Name = "System Log" };

        public static List<GenericEnum> RoutingKeyEnumList = new List<GenericEnum>()
        {
            AppUserSync, OrganizationSync, StorenSync, SendMail, AuditLogSend, SystemLogSend
        };

        public static GenericEnum AlbumUsed = new GenericEnum { Id = 101, Code = "Album.Used", Name = "Album Used" };
        public static GenericEnum BrandUsed = new GenericEnum { Id = 102, Code = "Brand.Used", Name = "Brand Used" };
        public static GenericEnum ItemUsed = new GenericEnum { Id = 103, Code = "Item.Used", Name = "Item Used" };
        public static GenericEnum ProductUsed = new GenericEnum { Id = 104, Code = "Product.Used", Name = "Product Used" };
        public static GenericEnum ProductTypeUsed = new GenericEnum { Id = 105, Code = "ProductType.Used", Name = "ProductType Used" };
        public static GenericEnum StoreUsed = new GenericEnum { Id = 106, Code = "Store.Used", Name = "Store Used" };
        public static GenericEnum StoreTypeUsed = new GenericEnum { Id = 107, Code = "StoreType.Used", Name = "StoreType Used" };
        public static GenericEnum SupplierUsed = new GenericEnum { Id = 108, Code = "Supplier.Used", Name = "Supplier Used" };
        public static GenericEnum SurveyUsed = new GenericEnum { Id = 109, Code = "Survey.Used", Name = "Survey Used" };
        public static GenericEnum TaxTypeUsed = new GenericEnum { Id = 110, Code = "TaxType.Used", Name = "TaxType Used" };
        public static GenericEnum UnitOfMeasureUsed = new GenericEnum { Id = 111, Code = "UnitOfMeasure.Used", Name = "UnitOfMeasure Used" };
    }
}
