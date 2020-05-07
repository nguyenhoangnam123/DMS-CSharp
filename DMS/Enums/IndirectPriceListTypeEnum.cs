using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class IndirectPriceListTypeEnum
    {
        public static GenericEnum ALLSTORE = new GenericEnum { Id = 1, Code = "ALLSTORE", Name = "Tất cả cửa hàng" };
        public static GenericEnum STORETYPE = new GenericEnum { Id = 2, Code = "STORETYPE", Name = "Theo loại cửa hàng" };
        public static GenericEnum STOREGROUPING = new GenericEnum { Id = 3, Code = "STOREGROUPING", Name = "Theo nhóm cửa hàng" };
        public static GenericEnum DETAILS = new GenericEnum { Id = 4, Code = "DETAILS", Name = "Chọn cửa hàng" };
    }
}
