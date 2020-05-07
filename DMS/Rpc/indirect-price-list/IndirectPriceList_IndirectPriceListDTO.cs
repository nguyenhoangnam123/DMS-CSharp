using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_IndirectPriceListDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long IndirectPriceListTypeId { get; set; }
        public IndirectPriceList_IndirectPriceListTypeDTO IndirectPriceListType { get; set; }
        public IndirectPriceList_OrganizationDTO Organization { get; set; }
        public IndirectPriceList_StatusDTO Status { get; set; }
        public List<IndirectPriceList_IndirectPriceListItemMappingDTO> IndirectPriceListItemMappings { get; set; }
        public List<IndirectPriceList_IndirectPriceListStoreMappingDTO> IndirectPriceListStoreMappings { get; set; }
        public List<IndirectPriceList_IndirectPriceListStoreTypeMappingDTO> IndirectPriceListStoreTypeMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IndirectPriceList_IndirectPriceListDTO() {}
        public IndirectPriceList_IndirectPriceListDTO(IndirectPriceList IndirectPriceList)
        {
            this.Id = IndirectPriceList.Id;
            this.Code = IndirectPriceList.Code;
            this.Name = IndirectPriceList.Name;
            this.StatusId = IndirectPriceList.StatusId;
            this.OrganizationId = IndirectPriceList.OrganizationId;
            this.IndirectPriceListTypeId = IndirectPriceList.IndirectPriceListTypeId;
            this.IndirectPriceListType = IndirectPriceList.IndirectPriceListType == null ? null : new IndirectPriceList_IndirectPriceListTypeDTO(IndirectPriceList.IndirectPriceListType);
            this.Organization = IndirectPriceList.Organization == null ? null : new IndirectPriceList_OrganizationDTO(IndirectPriceList.Organization);
            this.Status = IndirectPriceList.Status == null ? null : new IndirectPriceList_StatusDTO(IndirectPriceList.Status);
            this.IndirectPriceListItemMappings = IndirectPriceList.IndirectPriceListItemMappings?.Select(x => new IndirectPriceList_IndirectPriceListItemMappingDTO(x)).ToList();
            this.IndirectPriceListStoreMappings = IndirectPriceList.IndirectPriceListStoreMappings?.Select(x => new IndirectPriceList_IndirectPriceListStoreMappingDTO(x)).ToList();
            this.IndirectPriceListStoreTypeMappings = IndirectPriceList.IndirectPriceListStoreTypeMappings?.Select(x => new IndirectPriceList_IndirectPriceListStoreTypeMappingDTO(x)).ToList();
            this.CreatedAt = IndirectPriceList.CreatedAt;
            this.UpdatedAt = IndirectPriceList.UpdatedAt;
            this.Errors = IndirectPriceList.Errors;
        }
    }

    public class IndirectPriceList_IndirectPriceListFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter IndirectPriceListTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public IndirectPriceListOrder OrderBy { get; set; }
    }
}
