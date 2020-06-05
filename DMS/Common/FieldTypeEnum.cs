using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common
{
    public class FieldType : DataEntity
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class FieldTypeEnum
    {
        public static GenericEnum ID = new GenericEnum { Id = 1, Code = "ID", Name = "ID" };
        public static GenericEnum STRING = new GenericEnum { Id = 2, Code = "STRING", Name = "STRING" };
        public static GenericEnum LONG = new GenericEnum { Id = 3, Code = "LONG", Name = "LONG" };
        public static GenericEnum DECIMAL = new GenericEnum { Id = 4, Code = "DECIMAL", Name = "DATE" };
        public static GenericEnum DATE = new GenericEnum { Id = 5, Code = "DATE", Name = "DATE" };
        

        public static List<GenericEnum> List = new List<GenericEnum>()
        {
            ID, STRING, LONG, DATE,
        };
    }
}
