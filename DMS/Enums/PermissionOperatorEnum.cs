using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class PermissionOperatorEnum
    {
        public static GenericEnum ID_EQ = new GenericEnum { Id = 101, Code = "ID_EQ", Name = "=" };
        public static GenericEnum ID_NE = new GenericEnum { Id = 102, Code = "ID_NE", Name = "!=" };
        public static GenericEnum ID_IN = new GenericEnum { Id = 103, Code = "ID_IN", Name = "Chứa" };
        public static GenericEnum ID_NI = new GenericEnum { Id = 104, Code = "ID_NI", Name = "Không chứa" };

        public static GenericEnum LONG_GT = new GenericEnum { Id = 201, Code = "LONG_GT", Name = ">" };
        public static GenericEnum LONG_GE = new GenericEnum { Id = 202, Code = "LONG_GE", Name = ">=" };
        public static GenericEnum LONG_LT = new GenericEnum { Id = 203, Code = "LONG_LT", Name = "<" };
        public static GenericEnum LONG_LE = new GenericEnum { Id = 204, Code = "LONG_LE", Name = "<=" };
        public static GenericEnum LONG_NE = new GenericEnum { Id = 205, Code = "LONG_NE", Name = "!=" };
        public static GenericEnum LONG_EQ = new GenericEnum { Id = 206, Code = "LONG_EQ", Name = "=" };

        public static GenericEnum DATE_GT = new GenericEnum { Id = 301, Code = "DATE_GT", Name = ">" };
        public static GenericEnum DATE_GE = new GenericEnum { Id = 302, Code = "DATE_GE", Name = ">=" };
        public static GenericEnum DATE_LT = new GenericEnum { Id = 303, Code = "DATE_LT", Name = "<" };
        public static GenericEnum DATE_LE = new GenericEnum { Id = 304, Code = "DATE_LE", Name = "<=" };
        public static GenericEnum DATE_NE = new GenericEnum { Id = 305, Code = "DATE_NE", Name = "!=" };
        public static GenericEnum DATE_EQ = new GenericEnum { Id = 306, Code = "DATE_EQ", Name = "=" };

        public static GenericEnum STRING_GT = new GenericEnum { Id = 401, Code = "STRING_GT", Name = ">" };
        public static GenericEnum STRING_GE = new GenericEnum { Id = 402, Code = "STRING_GE", Name = ">=" };
        public static GenericEnum STRING_LT = new GenericEnum { Id = 403, Code = "STRING_LT", Name = "<" };
        public static GenericEnum STRING_LE = new GenericEnum { Id = 404, Code = "STRING_LE", Name = "<=" };
        public static GenericEnum STRING_NE = new GenericEnum { Id = 405, Code = "STRING_NE", Name = "!=" };
        public static GenericEnum STRING_EQ = new GenericEnum { Id = 406, Code = "STRING_EQ", Name = "=" };
        public static GenericEnum STRING_SW = new GenericEnum { Id = 407, Code = "STRING_SW", Name = "Bắt đầu bởi" };
        public static GenericEnum STRING_NSW = new GenericEnum { Id = 408, Code = "STRING_NSW", Name = "Không bắt đầu bởi" };
        public static GenericEnum STRING_EW = new GenericEnum { Id = 409, Code = "STRING_EW", Name = "Kết thúc bởi" };
        public static GenericEnum STRING_NEW = new GenericEnum { Id = 410, Code = "STRING_NEW", Name = "Không kết thúc bởi" };
        public static GenericEnum STRING_CT = new GenericEnum { Id = 411, Code = "STRING_CT", Name = "Chứa" };
        public static GenericEnum STRING_NC = new GenericEnum { Id = 412, Code = "STRING_NC", Name = "Không chứa" };

        public static List<GenericEnum> PermissionOperatorEnumForID = new List<GenericEnum>()
        {
            ID_EQ, ID_NE, ID_IN, ID_NI,
        };

        public static List<GenericEnum> PermissionOperatorEnumForLONG = new List<GenericEnum>()
        {
            LONG_GT, LONG_GE, LONG_LT, LONG_LE, LONG_NE, LONG_EQ,
        };

        public static List<GenericEnum> PermissionOperatorEnumForDATE = new List<GenericEnum>()
        {
            DATE_GT, DATE_GE, DATE_LT, DATE_LE, DATE_NE, DATE_EQ,
        };

        public static List<GenericEnum> PermissionOperatorEnumForSTRING = new List<GenericEnum>()
        {
            STRING_GT, STRING_GE, STRING_LT, STRING_LE, STRING_NE, STRING_EQ, STRING_SW, STRING_NSW, STRING_EW, STRING_NEW, STRING_CT, STRING_NC,
        };
    }
}
