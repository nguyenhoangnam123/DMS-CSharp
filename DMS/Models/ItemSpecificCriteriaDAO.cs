﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificCriteriaDAO
    {
        public ItemSpecificCriteriaDAO()
        {
            ItemSpecificKpiContents = new HashSet<ItemSpecificKpiContentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ItemSpecificKpiContentDAO> ItemSpecificKpiContents { get; set; }
    }
}
