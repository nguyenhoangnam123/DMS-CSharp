using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_StoreDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string CodeDraft { get; set; }
        public string Name { get; set; }
        public long? ParentStoreId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreTypeId { get; set; }
        public long? StoreGroupingId { get; set; }
        public string Telephone { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public string Address { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? DeliveryLatitude { get; set; }
        public decimal? DeliveryLongitude { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerEmail { get; set; }
        public string TaxCode { get; set; }
        public string LegalEntity { get; set; }
        public long StatusId { get; set; }
        public bool HasEroute { get; set; }
        public bool HasChecking { get; set; }
        public bool HasOrder { get; set; }
        public bool Used { get; set; }
        public long? StoreScoutingId { get; set; }
        public long? StoreId { get; set; }
        public long StoreStatusId { get; set; }
        public GeneralMobile_DistrictDTO District { get; set; }
        public GeneralMobile_OrganizationDTO Organization { get; set; }
        public GeneralMobile_StoreDTO ParentStore { get; set; }
        public GeneralMobile_ProvinceDTO Province { get; set; }
        public GeneralMobile_StoreGroupingDTO StoreGrouping { get; set; }
        public GeneralMobile_StoreTypeDTO StoreType { get; set; }
        public GeneralMobile_WardDTO Ward { get; set; }
        public GeneralMobile_StatusDTO Status { get; set; }
        public GeneralMobile_StoreScoutingDTO StoreScouting { get; set; }
        public GeneralMobile_StoreStatusDTO StoreStatus { get; set; }
        public List<GeneralMobile_StoreImageMappingDTO> StoreImageMappings { get; set; }
        public List<GeneralMobile_AlbumImageMappingDTO> AlbumImageMappings { get; set; }
        public List<GeneralMobile_StoreCheckingDTO> StoreCheckings { get; set; }
        public List<GeneralMobile_BrandInStoreDTO> BrandInStores { get; set; }
        public double Distance { get; set; }
        public GeneralMobile_StoreDTO() { }
        public GeneralMobile_StoreDTO(Store Store)
        {
            this.Id = Store.Id;
            this.Code = Store.Code;
            this.CodeDraft = Store.CodeDraft;
            this.Name = Store.Name;
            this.ParentStoreId = Store.ParentStoreId;
            this.OrganizationId = Store.OrganizationId;
            this.StoreTypeId = Store.StoreTypeId;
            this.StoreGroupingId = Store.StoreGroupingId;
            this.Telephone = Store.Telephone;
            this.ProvinceId = Store.ProvinceId;
            this.DistrictId = Store.DistrictId;
            this.WardId = Store.WardId;
            this.Address = Store.Address;
            this.DeliveryAddress = Store.DeliveryAddress;
            this.Latitude = Store.Latitude;
            this.Longitude = Store.Longitude;
            this.DeliveryLatitude = Store.DeliveryLatitude;
            this.DeliveryLongitude = Store.DeliveryLongitude;
            this.OwnerName = Store.OwnerName;
            this.OwnerPhone = Store.OwnerPhone;
            this.OwnerEmail = Store.OwnerEmail;
            this.TaxCode = Store.TaxCode;
            this.LegalEntity = Store.LegalEntity;
            this.StatusId = Store.StatusId;
            this.HasEroute = Store.HasEroute;
            this.HasChecking = Store.HasChecking;
            this.HasOrder = Store.HasOrder;
            this.Used = Store.Used;
            this.StoreScoutingId = Store.StoreScoutingId;
            this.Distance = Store.Distance;
            this.StoreStatusId = Store.StoreStatusId;
            this.Status = Store.Status == null ? null : new GeneralMobile_StatusDTO(Store.Status);
            this.District = Store.District == null ? null : new GeneralMobile_DistrictDTO(Store.District);
            this.Organization = Store.Organization == null ? null : new GeneralMobile_OrganizationDTO(Store.Organization);
            this.ParentStore = Store.ParentStore == null ? null : new GeneralMobile_StoreDTO(Store.ParentStore);
            this.Province = Store.Province == null ? null : new GeneralMobile_ProvinceDTO(Store.Province);
            this.StoreGrouping = Store.StoreGrouping == null ? null : new GeneralMobile_StoreGroupingDTO(Store.StoreGrouping);
            this.StoreType = Store.StoreType == null ? null : new GeneralMobile_StoreTypeDTO(Store.StoreType);
            this.Ward = Store.Ward == null ? null : new GeneralMobile_WardDTO(Store.Ward);
            this.StoreScouting = Store.StoreScouting == null ? null : new GeneralMobile_StoreScoutingDTO(Store.StoreScouting);
            this.StoreStatus = Store.StoreStatus == null ? null : new GeneralMobile_StoreStatusDTO(Store.StoreStatus);
            this.StoreImageMappings = Store.StoreImageMappings?.Select(x => new GeneralMobile_StoreImageMappingDTO(x)).ToList();
            this.StoreCheckings = Store.StoreCheckings?.Select(x => new GeneralMobile_StoreCheckingDTO(x)).ToList();
            this.BrandInStores = Store.BrandInStores?.Select(x => new GeneralMobile_BrandInStoreDTO(x)).ToList();
            this.Errors = Store.Errors;
        }
    }

    public class GeneralMobile_StoreFilterDTO : FilterDTO
    {
        public string Search { get; set; }
        public IdFilter Id { get; set; }
        public IdFilter ERouteId { get; set; }

        public StringFilter Code { get; set; }
        public StringFilter CodeDraft { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter ParentStoreId { get; set; }

        public IdFilter OrganizationId { get; set; }

        public IdFilter StoreTypeId { get; set; }

        public IdFilter StoreGroupingId { get; set; }
        public IdFilter StoreCheckingStatusId { get; set; }

        public StringFilter Telephone { get; set; }

        public IdFilter ProvinceId { get; set; }

        public IdFilter DistrictId { get; set; }

        public IdFilter WardId { get; set; }

        public StringFilter Address { get; set; }

        public StringFilter DeliveryAddress { get; set; }

        public DecimalFilter Latitude { get; set; }

        public DecimalFilter Longitude { get; set; }

        public DecimalFilter DeliveryLatitude { get; set; }

        public DecimalFilter DeliveryLongitude { get; set; }

        public StringFilter OwnerName { get; set; }

        public StringFilter OwnerPhone { get; set; }

        public StringFilter OwnerEmail { get; set; }

        public StringFilter TaxCode { get; set; }

        public StringFilter LegalEntity { get; set; }

        public IdFilter StatusId { get; set; }
        public IdFilter StoreStatusId { get; set; }
        public IdFilter StoreDraftTypeId { get; set; }

        public StoreOrder OrderBy { get; set; }
    }
}