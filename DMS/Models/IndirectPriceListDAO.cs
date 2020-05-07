using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectPriceListDAO
    {
        public IndirectPriceListDAO()
        {
            IndirectPriceListItemMappings = new HashSet<IndirectPriceListItemMappingDAO>();
            IndirectPriceListStoreMappings = new HashSet<IndirectPriceListStoreMappingDAO>();
            IndirectPriceListStoreTypeMappings = new HashSet<IndirectPriceListStoreTypeMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long IndirectPriceListTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual IndirectPriceListTypeDAO IndirectPriceListType { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<IndirectPriceListItemMappingDAO> IndirectPriceListItemMappings { get; set; }
        public virtual ICollection<IndirectPriceListStoreMappingDAO> IndirectPriceListStoreMappings { get; set; }
        public virtual ICollection<IndirectPriceListStoreTypeMappingDAO> IndirectPriceListStoreTypeMappings { get; set; }
    }
}
