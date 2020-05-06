using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class ERouteTypeEnum
    {
        public static GenericEnum PERMANENT = new GenericEnum { Id = 1, Code = "PERMANENT", Name = "Cố định" };
        public static GenericEnum INCURRED = new GenericEnum { Id = 2, Code = "INCURRED", Name = "Phát sinh" };
    }
}
