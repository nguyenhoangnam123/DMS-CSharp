using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace DMS.Enums
{
    public class PromotionDiscountTypeEnum
    {
        public static GenericEnum PERCENTAGE = new GenericEnum { Id = 1, Code = "PERCENTAGE", Name = "Theo % giá trị" };
        public static GenericEnum AMOUNT = new GenericEnum { Id = 2, Code = "AMOUNT", Name = "Số tiền" };
        public static GenericEnum PRICE_FIXED = new GenericEnum { Id = 3, Code = "PRICE_FIXED", Name = "Giá cố định" };
        public static GenericEnum BONUS = new GenericEnum { Id = 4, Code = "BONUS", Name = "Tặng kèm" };

        public static List<GenericEnum> PromotionDiscountTypeEnumList = new List<GenericEnum>()
        {
            PERCENTAGE, AMOUNT, PRICE_FIXED, BONUS
        };
    }
}
