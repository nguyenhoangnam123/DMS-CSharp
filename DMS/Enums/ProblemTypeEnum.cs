using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class ProblemTypeEnum
    {
        public static GenericEnum COMPETITOR = new GenericEnum { Id = 1, Code = "COMPETITOR", Name = "Đối thủ" };
        public static GenericEnum STORE = new GenericEnum { Id = 2, Code = "STORE", Name = "Cửa hàng" };

        public static List<GenericEnum> List = new List<GenericEnum> { COMPETITOR, STORE };
    }
}
