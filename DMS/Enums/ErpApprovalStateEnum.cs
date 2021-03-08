﻿using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class ErpApprovalStateEnum
    {
        public static GenericEnum PENDING = new GenericEnum { Id = 1, Code = "PENDING", Name = "Chờ duyệt" };
        public static GenericEnum APPROVED = new GenericEnum { Id = 2, Code = "APPROVED", Name = "Hoàn thành" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 3, Code = "REJECTED", Name = "Từ chối" };


        public static List<GenericEnum> ErpApprovalStateEnumList = new List<GenericEnum> {
            PENDING, APPROVED, REJECTED
        };
    }
}
