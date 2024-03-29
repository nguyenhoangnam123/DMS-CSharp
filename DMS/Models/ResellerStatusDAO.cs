﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ResellerStatusDAO
    {
        public ResellerStatusDAO()
        {
            Resellers = new HashSet<ResellerDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ResellerDAO> Resellers { get; set; }
    }
}
