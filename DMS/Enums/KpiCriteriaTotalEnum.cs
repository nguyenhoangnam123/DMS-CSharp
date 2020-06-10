﻿using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class KpiCriteriaTotalEnum
    {
        public static GenericEnum TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM = new GenericEnum { Id = 1, Code = "TotalIndirectOutputOfKeyItems", Name = "Tổng sản lượng theo đơn gián tiếp" };
        public static GenericEnum TOTAL_INDIRECT_SALES_OF_KEY_ITEM = new GenericEnum { Id = 2, Code = "TotalIndirectSalesOfKeyItems", Name = "Tổng doanh số theo đơn gián tiếp" };
        public static GenericEnum TOTALINDIRECTORDERSOFKEYITEM = new GenericEnum { Id = 3, Code = "TotalIndirectOrdersOfKeyItems", Name = "Tổng số đơn hàng theo đơn gián tiếp" };
        public static GenericEnum TOTALINDIRECTSTORESOFKEYITEM = new GenericEnum { Id = 4, Code = "TotalIndirectStoresOfKeyItems", Name = "Tổng số khách hàng theo đơn gián tiếp" };
        public static GenericEnum TOTALDIRECTOUTPUTOFKEYITEM = new GenericEnum { Id = 5, Code = "TotalDirectOutputOfKeyItems", Name = "Tổng sản lượng theo đơn trực tiếp" };
        public static GenericEnum TOTALDIRECTSALESOFKEYITEM = new GenericEnum { Id = 6, Code = "TotalDirectSalesOfKeyItems", Name = "Tổng doanh số theo đơn trực tiếp" };
        public static GenericEnum TOTALDIRECTORDERSOFKEYITEM = new GenericEnum { Id = 7, Code = "TotalDirectOrdersOfKeyItems", Name = "Tổng số đơn hàng theo đơn trực tiếp" };
        public static GenericEnum TOTALDIRECTSTORESOFKEYITEM = new GenericEnum { Id = 8, Code = "TotalDirectStoresOfKeyItems", Name = "Tổng số khách hàng theo đơn trực tiếp" };

        public static List<GenericEnum> KpiCriteriaTotalEnumList = new List<GenericEnum>()
        {
            TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM, TOTAL_INDIRECT_SALES_OF_KEY_ITEM, TOTALINDIRECTORDERSOFKEYITEM, TOTALINDIRECTSTORESOFKEYITEM,
            TOTALDIRECTOUTPUTOFKEYITEM, TOTALDIRECTSALESOFKEYITEM, TOTALDIRECTORDERSOFKEYITEM, TOTALDIRECTSTORESOFKEYITEM
        };
    }
}