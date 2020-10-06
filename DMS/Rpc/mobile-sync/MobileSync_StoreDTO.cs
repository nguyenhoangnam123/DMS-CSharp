using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StoreDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }
        public string CodeDraft { get; set; }

        public string Name { get; set; }

        public long? ParentStoreId { get; set; }

        public long OrganizationId { get; set; }

        public long StoreTypeId { get; set; }

        public long? StoreGroupingId { get; set; }

        public long? ResellerId { get; set; }

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

        public long StoreStatusId { get; set; }

        public long StatusId { get; set; }
        public MobileSync_StoreDTO ParentStore { get; set; }
        public MobileSync_StoreGroupingDTO StoreGrouping { get; set; }
        public MobileSync_StoreTypeDTO StoreType { get; set; }
        public MobileSync_StoreDTO() { }
        public MobileSync_StoreDTO(StoreDAO StoreDAO)
        {

            this.Id = StoreDAO.Id;

            this.Code = StoreDAO.Code;
            this.CodeDraft = StoreDAO.CodeDraft;

            this.Name = StoreDAO.Name;

            this.ParentStoreId = StoreDAO.ParentStoreId;

            this.OrganizationId = StoreDAO.OrganizationId;

            this.StoreTypeId = StoreDAO.StoreTypeId;

            this.StoreGroupingId = StoreDAO.StoreGroupingId;

            this.ResellerId = StoreDAO.ResellerId;

            this.Telephone = StoreDAO.Telephone;

            this.ProvinceId = StoreDAO.ProvinceId;

            this.DistrictId = StoreDAO.DistrictId;

            this.WardId = StoreDAO.WardId;

            this.Address = StoreDAO.Address;

            this.DeliveryAddress = StoreDAO.DeliveryAddress;

            this.Latitude = StoreDAO.Latitude;

            this.Longitude = StoreDAO.Longitude;

            this.DeliveryLatitude = StoreDAO.DeliveryLatitude;

            this.DeliveryLongitude = StoreDAO.DeliveryLongitude;

            this.OwnerName = StoreDAO.OwnerName;

            this.OwnerPhone = StoreDAO.OwnerPhone;

            this.OwnerEmail = StoreDAO.OwnerEmail;
            this.TaxCode = StoreDAO.TaxCode;
            this.LegalEntity = StoreDAO.LegalEntity;

            this.StatusId = StoreDAO.StatusId;

            this.ParentStore = StoreDAO.ParentStore == null ? null : new MobileSync_StoreDTO(StoreDAO.ParentStore);
            this.StoreGrouping = StoreDAO.StoreGrouping == null ? null : new MobileSync_StoreGroupingDTO(StoreDAO.StoreGrouping);
            this.StoreType = StoreDAO.StoreType == null ? null : new MobileSync_StoreTypeDTO(StoreDAO.StoreType);
        }
    }
}
