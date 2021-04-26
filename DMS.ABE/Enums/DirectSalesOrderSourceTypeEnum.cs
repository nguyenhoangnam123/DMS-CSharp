﻿using DMS.ABE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Enums
{
    public class DirectSalesOrderSourceTypeEnum
    {
        public static GenericEnum FROM_EMPLOYEE = new GenericEnum { Id = 1, Code = "CREATED_BY_SALE_EMPLOYEE", Name = "Tạo bởi nhân viên" };
        public static GenericEnum FROM_STORE = new GenericEnum { Id = 2, Code = "CREATED_BY_STORE", Name = "Tạo bởi đại lý" };

        public static List<GenericEnum> DirectSalesOrderSourceTypeEnumList = new List<GenericEnum>
        {
            FROM_EMPLOYEE, FROM_STORE
        };
    }
}
