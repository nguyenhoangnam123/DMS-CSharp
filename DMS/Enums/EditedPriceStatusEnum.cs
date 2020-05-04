using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class EditedPriceStatusEnum
    {
        public static GenericEnum ACTIVE = new GenericEnum { Id = 1, Code = "ACTIVE", Name = "Active" };
        public static GenericEnum INACTIVE = new GenericEnum { Id = 2, Code = "INACTIVE", Name = "Inactive" };
    }
}
