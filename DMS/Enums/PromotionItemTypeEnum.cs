using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class PromotionProductAppliedTypeEnum
    {
        public static GenericEnum ALL = new GenericEnum { Id = 1, Code = "ALL", Name = "Tất cả" };
        public static GenericEnum PRODUCT = new GenericEnum { Id = 2, Code = "PRODUCT", Name = "Chi tiết" };

        public static List<GenericEnum> PromotionProductAppliedTypeEnumList = new List<GenericEnum>()
        {
            ALL, PRODUCT
        };
    }
}
