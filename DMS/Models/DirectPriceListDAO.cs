using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectPriceListDAO
    {
        public DirectPriceListDAO()
        {
            DirectPriceListItemMappings = new HashSet<DirectPriceListItemMappingDAO>();
            DirectPriceListStoreGroupingMappings = new HashSet<DirectPriceListStoreGroupingMappingDAO>();
            DirectPriceListStoreMappings = new HashSet<DirectPriceListStoreMappingDAO>();
            DirectPriceListStoreTypeMappings = new HashSet<DirectPriceListStoreTypeMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long DirectPriceListTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual DirectPriceListTypeDAO DirectPriceListType { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<DirectPriceListItemMappingDAO> DirectPriceListItemMappings { get; set; }
        public virtual ICollection<DirectPriceListStoreGroupingMappingDAO> DirectPriceListStoreGroupingMappings { get; set; }
        public virtual ICollection<DirectPriceListStoreMappingDAO> DirectPriceListStoreMappings { get; set; }
        public virtual ICollection<DirectPriceListStoreTypeMappingDAO> DirectPriceListStoreTypeMappings { get; set; }
    }
}
