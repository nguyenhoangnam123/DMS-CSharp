using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DMS.Enums
{
    public class WorkflowTypeEnum
    {
        public static GenericEnum STORE = new GenericEnum { Id = 1, Code = "STORE", Name = "Cửa hàng" };
        public static GenericEnum PRODUCT = new GenericEnum { Id = 1, Code = "PRODUCT", Name = "Sản phẩm" };
        public static GenericEnum ROUTE = new GenericEnum { Id = 1, Code = "ROUTE", Name = "Tuyến" };
        public static GenericEnum ORDER = new GenericEnum { Id = 1, Code = "ORDER", Name = "Đơn hàng" };
    }
}
