using DMS.Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class KpiItemTypeEnum
    {
        public static GenericEnum NEW_PRODUCT = new GenericEnum { Id = 1, Code = "NewProduct", Name = "Sản phẩm mới" };
        public static GenericEnum ALL_PRODUCT = new GenericEnum { Id = 2, Code = "AllProduct", Name = "Sản phẩm trọng tâm" };

        public static List<GenericEnum> KpiItemTypeEnumList = new List<GenericEnum>()
        {
            NEW_PRODUCT,
            ALL_PRODUCT,
        };
    }
}
