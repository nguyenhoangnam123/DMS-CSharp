﻿using DMS.ABE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Enums
{
    public class CheckingPlanStatusEnum
    {
        public static GenericEnum UNPLANNED = new GenericEnum { Id = 2, Code = "UNPLANNED", Name = "Ngoài tuyến" };
        public static GenericEnum PLANNED = new GenericEnum { Id = 1, Code = "PLANNED", Name = "Trong tuyến" };
        public static List<GenericEnum> CheckingPlanStatusEnumList = new List<GenericEnum>()
        {
            PLANNED, UNPLANNED
        };
    }
}
