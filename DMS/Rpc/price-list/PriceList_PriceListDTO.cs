using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.price_list
{
    public class PriceList_PriceListDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long PriceListTypeId { get; set; }
        public PriceList_OrganizationDTO Organization { get; set; }
        public PriceList_PriceListTypeDTO PriceListType { get; set; }
        public PriceList_StatusDTO Status { get; set; }
        public List<PriceList_PriceListItemMappingDTO> PriceListItemMappings { get; set; }
        public List<PriceList_PriceListStoreMappingDTO> PriceListStoreMappings { get; set; }
        public List<PriceList_PriceListStoreTypeMappingDTO> PriceListStoreTypeMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PriceList_PriceListDTO() {}
        public PriceList_PriceListDTO(PriceList PriceList)
        {
            this.Id = PriceList.Id;
            this.Code = PriceList.Code;
            this.Name = PriceList.Name;
            this.StatusId = PriceList.StatusId;
            this.OrganizationId = PriceList.OrganizationId;
            this.PriceListTypeId = PriceList.PriceListTypeId;
            this.Organization = PriceList.Organization == null ? null : new PriceList_OrganizationDTO(PriceList.Organization);
            this.PriceListType = PriceList.PriceListType == null ? null : new PriceList_PriceListTypeDTO(PriceList.PriceListType);
            this.Status = PriceList.Status == null ? null : new PriceList_StatusDTO(PriceList.Status);
            this.PriceListItemMappings = PriceList.PriceListItemMappings?.Select(x => new PriceList_PriceListItemMappingDTO(x)).ToList();
            this.PriceListStoreMappings = PriceList.PriceListStoreMappings?.Select(x => new PriceList_PriceListStoreMappingDTO(x)).ToList();
            this.PriceListStoreTypeMappings = PriceList.PriceListStoreTypeMappings?.Select(x => new PriceList_PriceListStoreTypeMappingDTO(x)).ToList();
            this.CreatedAt = PriceList.CreatedAt;
            this.UpdatedAt = PriceList.UpdatedAt;
            this.Errors = PriceList.Errors;
        }
    }

    public class PriceList_PriceListFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter PriceListTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public PriceListOrder OrderBy { get; set; }
    }
}
