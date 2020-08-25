using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class DirectSalesOrderTransactionTypeEnum
    {
        public static GenericEnum SALES_CONTENT = new GenericEnum { Id = 1, Code = "SALE", Name = "Sản phẩm bán" };
        public static GenericEnum PROMOTION = new GenericEnum { Id = 2, Code = "PROMOTION", Name = "Sản phẩm khuyến mại" };
        public static List<GenericEnum> DirectSalesOrderTransactionTypeEnumList = new List<GenericEnum>
        {
            SALES_CONTENT, PROMOTION,
        };
    }
}
