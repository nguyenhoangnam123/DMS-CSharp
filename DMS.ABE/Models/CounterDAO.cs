﻿using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class CounterDAO
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
