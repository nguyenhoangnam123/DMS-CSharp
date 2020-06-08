using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_DirectPriceListDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long DirectPriceListTypeId { get; set; }
        public DirectPriceList_DirectPriceListTypeDTO DirectPriceListType { get; set; }
        public DirectPriceList_OrganizationDTO Organization { get; set; }
        public DirectPriceList_StatusDTO Status { get; set; }
        public List<DirectPriceList_DirectPriceListItemMappingDTO> DirectPriceListItemMappings { get; set; }
        public List<DirectPriceList_DirectPriceListStoreGroupingMappingDTO> DirectPriceListStoreGroupingMappings { get; set; }
        public List<DirectPriceList_DirectPriceListStoreMappingDTO> DirectPriceListStoreMappings { get; set; }
        public List<DirectPriceList_DirectPriceListStoreTypeMappingDTO> DirectPriceListStoreTypeMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DirectPriceList_DirectPriceListDTO() { }
        public DirectPriceList_DirectPriceListDTO(DirectPriceList DirectPriceList)
        {
            this.Id = DirectPriceList.Id;
            this.Code = DirectPriceList.Code;
            this.Name = DirectPriceList.Name;
            this.StartDate = DirectPriceList.StartDate;
            this.EndDate = DirectPriceList.EndDate;
            this.StatusId = DirectPriceList.StatusId;
            this.OrganizationId = DirectPriceList.OrganizationId;
            this.DirectPriceListTypeId = DirectPriceList.DirectPriceListTypeId;
            this.DirectPriceListType = DirectPriceList.DirectPriceListType == null ? null : new DirectPriceList_DirectPriceListTypeDTO(DirectPriceList.DirectPriceListType);
            this.Organization = DirectPriceList.Organization == null ? null : new DirectPriceList_OrganizationDTO(DirectPriceList.Organization);
            this.Status = DirectPriceList.Status == null ? null : new DirectPriceList_StatusDTO(DirectPriceList.Status);
            this.DirectPriceListItemMappings = DirectPriceList.DirectPriceListItemMappings?.Select(x => new DirectPriceList_DirectPriceListItemMappingDTO(x)).ToList();
            this.DirectPriceListStoreGroupingMappings = DirectPriceList.DirectPriceListStoreGroupingMappings?.Select(x => new DirectPriceList_DirectPriceListStoreGroupingMappingDTO(x)).ToList();
            this.DirectPriceListStoreMappings = DirectPriceList.DirectPriceListStoreMappings?.Select(x => new DirectPriceList_DirectPriceListStoreMappingDTO(x)).ToList();
            this.DirectPriceListStoreTypeMappings = DirectPriceList.DirectPriceListStoreTypeMappings?.Select(x => new DirectPriceList_DirectPriceListStoreTypeMappingDTO(x)).ToList();
            this.CreatedAt = DirectPriceList.CreatedAt;
            this.UpdatedAt = DirectPriceList.UpdatedAt;
            this.Errors = DirectPriceList.Errors;
        }
    }

    public class DirectPriceList_DirectPriceListFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public DateFilter StartDate { get; set; }
        public DateFilter EndDate { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter DirectPriceListTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public DirectPriceListOrder OrderBy { get; set; }
    }
}
