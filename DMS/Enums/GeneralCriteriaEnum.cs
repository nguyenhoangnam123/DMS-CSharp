using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class GeneralCriteriaEnum
    {
        public static GenericEnum TOTALINDIRECTORDERS = new GenericEnum { Id = 1, Code = "TotalIndirectOrders", Name = "Số đơn hàng gián tiếp" };
        public static GenericEnum TOTALINDIRECTOUTPUT = new GenericEnum { Id = 2, Code = "TotalIndirectOutput", Name = "Tổng sản lượng đơn hàng gián tiếp" };
        public static GenericEnum TOTALINDIRECTSALESAMOUNT = new GenericEnum { Id = 3, Code = "TotalIndirectSalesAmount", Name = "Doanh số đơn hàng gián tiếp" };
        public static GenericEnum SKUINDIRECTORDER = new GenericEnum { Id = 4, Code = "SKUIndirectOrder", Name = "SKU/ Đơn hàng gián tiếp" };
        public static GenericEnum STORESVISITED = new GenericEnum { Id = 5, Code = "StoresVisited", Name = "Số KH viếng thăm" };
        public static GenericEnum NEWSTORECREATED = new GenericEnum { Id = 6, Code = "NewStoresCreated", Name = "Số KH tạo mới" };
        public static GenericEnum TOTALDIRECTORDERS = new GenericEnum { Id = 7, Code = "TotalDirectOrders", Name = "Số đơn hàng trực tiếp" };
        public static GenericEnum TOTALDIRECTOUTPUT = new GenericEnum { Id = 8, Code = "TotaldirectOutput", Name = "Tổng sản lượng đơn hàng trực tiếp" };
        public static GenericEnum TOTALDIRECTSALESAMOUNT = new GenericEnum { Id = 9, Code = "TotalDirectSalesAmount", Name = "Doanh số đơn hàng trực tiếp" };
        public static GenericEnum SKUDIRECTORDER = new GenericEnum { Id = 10, Code = "SKUDirectOrder", Name = "SKU/ Đơn hàng trực tiếp" };

        public static List<GenericEnum> KpiCriteriaEnumList = new List<GenericEnum>()
        {
            TOTALINDIRECTORDERS, TOTALINDIRECTOUTPUT, TOTALINDIRECTSALESAMOUNT, SKUINDIRECTORDER, STORESVISITED, NEWSTORECREATED, TOTALDIRECTORDERS, TOTALDIRECTOUTPUT,
            TOTALDIRECTSALESAMOUNT, SKUDIRECTORDER
        };
    }
}
