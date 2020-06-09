using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class ProblemTypeEnum
    {
        public static GenericEnum COMPETITOR = new GenericEnum { Id = 1, Code = "COMPETITOR", Name = "Đối thủ" };
        public static GenericEnum STORE = new GenericEnum { Id = 2, Code = "STORE", Name = "Cửa hàng" };
        public static List<GenericEnum> ProblemTypeEnumList = new List<GenericEnum>()
        {
            COMPETITOR, STORE
        };
    }
}
