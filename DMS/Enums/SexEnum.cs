﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class SexEnum
    {
        public static GenericEnum Male => new GenericEnum { Id = 1, Name = "Nam", Code = "Male" };
        public static GenericEnum Female => new GenericEnum { Id = 2, Name = "Nữ", Code = "Female" };
        public static List<GenericEnum> SexEnumList = new List<GenericEnum>()
        {
            Male, Female
        };
    }
}
