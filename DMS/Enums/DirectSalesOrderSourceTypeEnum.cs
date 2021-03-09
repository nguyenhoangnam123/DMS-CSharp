using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class DirectSalesOrderSourceTypeEnum
    {
        public static GenericEnum FROM_DMS = new GenericEnum { Id = 1, Code = "FROM_DMS", Name = "Được tạo trên DMS" };
        public static GenericEnum FROM_AMS = new GenericEnum { Id = 2, Code = "FROM_AMS", Name = "Được tạo trên AMS" };

        public static List<GenericEnum> DirectSalesOrderSourceTypeEnumList = new List<GenericEnum>
        {
            FROM_DMS, FROM_AMS
        };
    }
}
