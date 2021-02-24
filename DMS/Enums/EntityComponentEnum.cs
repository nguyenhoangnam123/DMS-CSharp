using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class EntityComponentEnum
    {
        public static GenericEnum AUTO_NUMBER = new GenericEnum { Id = 1, Code = "AutoNumber", Name = "Mã số tự sinh" };
        public static GenericEnum TEXT = new GenericEnum { Id = 2, Code = "Text", Name = "Text" };

        public static GenericEnum STORE_ORGANIZATION = new GenericEnum { Id = 31, Code = "StoreOrganization", Name = "Đơn vị tổ chức" };
        public static GenericEnum STORE_STORE_TYPE = new GenericEnum { Id = 32, Code = "StoreType", Name = "Cấp đại lý" };
        public static List<GenericEnum> StoreEnumList = new List<GenericEnum>
        {
            AUTO_NUMBER, TEXT, STORE_ORGANIZATION, STORE_STORE_TYPE
        };

        public static List<GenericEnum> EntityComponentEnumList = new List<GenericEnum>
        {
            AUTO_NUMBER, TEXT, STORE_ORGANIZATION, STORE_STORE_TYPE
        };
    }
}
