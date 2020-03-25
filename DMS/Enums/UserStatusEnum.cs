﻿using Common;

namespace DMS.Enums
{
    public class UserStatusEnum
    {
        public static GenericEnum ACTIVE = new GenericEnum { Id = 1, Code = "Active", Name = "Hoạt động" };
        public static GenericEnum INACTIVE = new GenericEnum { Id = 2, Code = "Inactive", Name = "Dừng hoạt động" };
        public static GenericEnum LOCKED = new GenericEnum { Id = 3, Code = "Locked", Name = "Đang bị khóa" };
    }
}
