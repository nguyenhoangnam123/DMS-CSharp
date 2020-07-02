using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class GeneralCriteriaEnum
    {
        public static GenericEnum TOTALINDIRECTORDERS = new GenericEnum { Id = 1, Code = "TotalIndirectOrders", Name = "Số đơn hàng gián tiếp" };
        public static GenericEnum TOTALINDIRECTOUTPUT = new GenericEnum { Id = 2, Code = "TotalIndirectOutput", Name = "Tổng sản lượng đơn hàng gián tiếp" };
        public static GenericEnum TOTALINDIRECTSALESAMOUNT = new GenericEnum { Id = 3, Code = "TotalIndirectSalesAmount", Name = "Doanh thu đơn hàng gián tiếp" };
        public static GenericEnum SKUINDIRECTORDER = new GenericEnum { Id = 4, Code = "SKUIndirectOrder", Name = "SKU/ Đơn hàng gián tiếp" };
        public static GenericEnum STORESVISITED = new GenericEnum { Id = 5, Code = "StoresVisited", Name = "Số cửa hàng viếng thăm" };
        public static GenericEnum NEWSTORECREATED = new GenericEnum { Id = 6, Code = "NewStoresCreated", Name = "Số cửa hàng tạo mới" };

        public static List<GenericEnum> KpiCriteriaEnumList = new List<GenericEnum>()
        {
            TOTALINDIRECTORDERS, TOTALINDIRECTOUTPUT, TOTALINDIRECTSALESAMOUNT, SKUINDIRECTORDER, STORESVISITED, NEWSTORECREATED
        };
    }
}
