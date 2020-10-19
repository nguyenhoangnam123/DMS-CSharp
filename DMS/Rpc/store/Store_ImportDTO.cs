using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store
{
    public class Store_ImportDTO : DataDTO
    {
        public long Stt { get; set; }
        public string CodeValue { get; set; }
        public string CodeDraftValue { get; set; }
        public string NameValue { get; set; }
        public string OrganizationCodeValue { get; set; }
        public string ParentStoreCodeValue { get; set; }
        public string StoreTypeCodeValue { get; set; }
        public string StoreGroupingCodeValue { get; set; }
        public string LegalEntityValue { get; set; }
        public string TaxCodeValue { get; set; }
        public string ProvinceCodeValue { get; set; }
        public string DistrictCodeValue { get; set; }
        public string WardCodeValue { get; set; }
        public string AddressValue { get; set; }
        public string LongitudeValue { get; set; }
        public string LatitudeValue { get; set; }
        public string DeliveryAddressValue { get; set; }
        public string DeliveryLongitudeValue { get; set; }
        public string DeliveryLatitudeValue { get; set; }
        public string TelephoneValue { get; set; }
        public string OwnerNameValue { get; set; }
        public string OwnerPhoneValue { get; set; }
        public string OwnerEmailValue { get; set; }
        public string StatusNameValue { get; set; }
        public bool IsNew { get; set; }
        public Store Store { get; set; }
        public long OrganizationId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public decimal DeliveryLongitude { get; set; }
        public decimal DeliveryLatitude { get; set; }
        public long StoreTypeId { get; set; }
        public long StoreGroupingId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public long StatusId { get; set; }
    }
}
