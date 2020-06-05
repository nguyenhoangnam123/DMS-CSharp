using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class FieldTypeEnum
    {
        public static GenericEnum ID = new GenericEnum { Id = 1, Code = "ID", Name = "ID" };
        public static GenericEnum STRING = new GenericEnum { Id = 2, Code = "STRING", Name = "STRING" };
        public static GenericEnum LONG = new GenericEnum { Id = 3, Code = "LONG", Name = "LONG" };
        public static GenericEnum DATE = new GenericEnum { Id = 4, Code = "DATE", Name = "DATE" };

        public static List<GenericEnum> List = new List<GenericEnum>()
        {
            ID, STRING, LONG, DATE,
        };
    }
}
