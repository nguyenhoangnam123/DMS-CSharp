using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreScoutingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OwnerPhone { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public long OrganizationId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public long CreatorId { get; set; }
        public long StoreScoutingStatusId { get; set; }
        public long StoreScoutingTypeId { get; set; }
        public Mobile_OrganizationDTO Organization { get; set; }
        public Mobile_AppUserDTO Creator { get; set; }
        public Mobile_DistrictDTO District { get; set; }
        public Mobile_ProvinceDTO Province { get; set; }
        public Mobile_StoreScoutingStatusDTO StoreScoutingStatus { get; set; }
        public Mobile_StoreScoutingTypeDTO StoreScoutingType { get; set; }
        public Mobile_WardDTO Ward { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Mobile_StoreScoutingImageMappingDTO> StoreScoutingImageMappings { get; set; }
        public Mobile_StoreScoutingDTO() { }
        public Mobile_StoreScoutingDTO(StoreScouting StoreScouting)
        {
            this.Id = StoreScouting.Id;
            this.Code = StoreScouting.Code;
            this.Name = StoreScouting.Name;
            this.OwnerPhone = StoreScouting.OwnerPhone;
            this.ProvinceId = StoreScouting.ProvinceId;
            this.DistrictId = StoreScouting.DistrictId;
            this.WardId = StoreScouting.WardId;
            this.Address = StoreScouting.Address;
            this.Latitude = StoreScouting.Latitude;
            this.Longitude = StoreScouting.Longitude;
            this.CreatorId = StoreScouting.CreatorId;
            this.OrganizationId = StoreScouting.OrganizationId;
            this.StoreScoutingStatusId = StoreScouting.StoreScoutingStatusId;
            this.Creator = StoreScouting.Creator == null ? null : new Mobile_AppUserDTO(StoreScouting.Creator);
            this.Organization = StoreScouting.Organization == null ? null : new Mobile_OrganizationDTO(StoreScouting.Organization);
            this.District = StoreScouting.District == null ? null : new Mobile_DistrictDTO(StoreScouting.District);
            this.Province = StoreScouting.Province == null ? null : new Mobile_ProvinceDTO(StoreScouting.Province);
            this.StoreScoutingStatus = StoreScouting.StoreScoutingStatus == null ? null : new Mobile_StoreScoutingStatusDTO(StoreScouting.StoreScoutingStatus);
            this.StoreScoutingType = StoreScouting.StoreScoutingType == null ? null : new Mobile_StoreScoutingTypeDTO(StoreScouting.StoreScoutingType);
            this.Ward = StoreScouting.Ward == null ? null : new Mobile_WardDTO(StoreScouting.Ward);
            this.StoreScoutingImageMappings = StoreScouting.StoreScoutingImageMappings?.Select(x => new Mobile_StoreScoutingImageMappingDTO(x)).ToList();
            this.CreatedAt = StoreScouting.CreatedAt;
            this.UpdatedAt = StoreScouting.UpdatedAt;
            this.Errors = StoreScouting.Errors;
        }
    }

    public class Mobile_StoreScoutingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter OwnerPhone { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public StringFilter Address { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DecimalFilter Longitude { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter StoreScoutingStatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public StoreScoutingOrder OrderBy { get; set; }
    }
}
