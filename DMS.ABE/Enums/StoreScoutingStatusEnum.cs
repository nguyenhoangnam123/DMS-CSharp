﻿using DMS.ABE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Enums
{
    public class StoreScoutingStatusEnum
    {
        public static GenericEnum NOTOPEN = new GenericEnum { Id = 0, Code = "NOTOPEN", Name = "Chưa mở" };
        public static GenericEnum OPENED = new GenericEnum { Id = 1, Code = "OPENED", Name = "Đã mở" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 2, Code = "REJECTED", Name = "Đã từ chối" };
        
        public static List<GenericEnum> StoreScoutingStatusEnumList = new List<GenericEnum>()
        {
            NOTOPEN, OPENED, REJECTED
        };
    }
}
