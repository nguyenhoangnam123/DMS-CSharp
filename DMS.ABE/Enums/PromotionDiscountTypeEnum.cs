using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.ABE.Common;

namespace DMS.ABE.Enums
{
    public class PromotionDiscountTypeEnum
    {
        public static GenericEnum PERCENTAGE = new GenericEnum { Id = 1, Code = "PERCENTAGE", Name = "Theo % giá trị" };
        public static GenericEnum AMOUNT = new GenericEnum { Id = 2, Code = "AMOUNT", Name = "Số tiền" };

        public static List<GenericEnum> PromotionDiscountTypeEnumList = new List<GenericEnum>()
        {
            PERCENTAGE, AMOUNT
        };
    }
}
