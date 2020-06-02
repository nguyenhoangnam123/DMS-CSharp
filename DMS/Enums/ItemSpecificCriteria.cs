using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class ItemSpecificCriteriaEnum
    {
        public static GenericEnum INDIRECTOUTPUTOFKEYITEM = new GenericEnum { Id = 1, Code = "IndirectOutputOfKeyItem", Name = "Sản lượng theo đơn gián tiếp" };
        public static GenericEnum INDIRECTSALESOFKEYITEM = new GenericEnum { Id = 2, Code = "IndirectSalesOfKeyItem", Name = "Doanh số theo đơn gián tiếp" };
        public static GenericEnum INDIRECTORDERSOFKEYITEM = new GenericEnum { Id = 3, Code = "IndirectOrdersOfKeyItem", Name = "Số đơn hàng theo đơn gián tiếp" };
        public static GenericEnum INDIRECTSTORESOFKEYITEM = new GenericEnum { Id = 4, Code = "IndirectStoresOfKeyItem", Name = "Số khách hàng theo đơn gián tiếp" };
        public static GenericEnum DIRECTOUTPUTOFKEYITEM = new GenericEnum { Id = 5, Code = "DirectOutputOfKeyItem", Name = "Sản lượng theo đơn trực tiếp" };
        public static GenericEnum DIRECTSALESOFKEYITEM = new GenericEnum { Id = 6, Code = "DirectSalesOfKeyItem", Name = "Doanh số theo đơn trực tiếp" };
        public static GenericEnum DIRECTORDERSOFKEYITEM = new GenericEnum { Id = 7, Code = "DirectOrdersOfKeyItem", Name = "Số đơn hàng theo đơn trực tiếp" };
        public static GenericEnum DIRECTSTORESOFKEYITEM = new GenericEnum { Id = 8, Code = "DirectStoresOfKeyItem", Name = "Số khách hàng theo đơn trực tiếp" };

        public static List<GenericEnum> ItemSpecificCriteriaEnumList = new List<GenericEnum>()
        {
            INDIRECTOUTPUTOFKEYITEM, INDIRECTSALESOFKEYITEM, INDIRECTORDERSOFKEYITEM, INDIRECTSTORESOFKEYITEM, DIRECTOUTPUTOFKEYITEM, DIRECTSALESOFKEYITEM,
            DIRECTORDERSOFKEYITEM, DIRECTSTORESOFKEYITEM
        };
    }
}
