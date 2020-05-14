using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_StoreDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? ParentStoreId { get; set; }
        
        public long OrganizationId { get; set; }
        
        public long StoreTypeId { get; set; }
        
        public long? StoreGroupingId { get; set; }
        
        public long? ResellerId { get; set; }
        
        public string Telephone { get; set; }
        
        public long ProvinceId { get; set; }
        
        public long DistrictId { get; set; }
        
        public long WardId { get; set; }
        
        public string Address { get; set; }
        
        public string DeliveryAddress { get; set; }
        
        public decimal? Latitude { get; set; }
        
        public decimal? Longitude { get; set; }
        
        public decimal? DeliveryLatitude { get; set; }
        
        public decimal? DeliveryLongitude { get; set; }
        
        public string OwnerName { get; set; }
        
        public string OwnerPhone { get; set; }
        
        public string OwnerEmail { get; set; }
        
        public string TaxCode { get; set; }
        
        public string LegalEntity { get; set; }
        
        public long StatusId { get; set; }
        

        public StoreChecking_StoreDTO() {}
        public StoreChecking_StoreDTO(Store Store)
        {
            
            this.Id = Store.Id;
            
            this.Code = Store.Code;
            
            this.Name = Store.Name;
            
            this.ParentStoreId = Store.ParentStoreId;
            
            this.OrganizationId = Store.OrganizationId;
            
            this.StoreTypeId = Store.StoreTypeId;
            
            this.StoreGroupingId = Store.StoreGroupingId;
            
            this.ResellerId = Store.ResellerId;
            
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
            
            this.StatusId = Store.StatusId;
            
            this.Errors = Store.Errors;
        }
    }

    public class StoreChecking_StoreFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter ParentStoreId { get; set; }
        
        public IdFilter OrganizationId { get; set; }
        
        public IdFilter StoreTypeId { get; set; }
        
        public IdFilter StoreGroupingId { get; set; }
        
        public IdFilter ResellerId { get; set; }
        
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
        
        public StoreOrder OrderBy { get; set; }
    }
}