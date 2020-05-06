﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PriceListDAO
    {
        public PriceListDAO()
        {
            PriceListItemMappings = new HashSet<PriceListItemMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long PriceListTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual OrganizationDAO Organization { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<PriceListItemMappingDAO> PriceListItemMappings { get; set; }
    }
}
