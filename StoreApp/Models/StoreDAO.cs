﻿using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class StoreDAO
    {
        public StoreDAO()
        {
            AlbumImageMappings = new HashSet<AlbumImageMappingDAO>();
            AppUserStoreMappings = new HashSet<AppUserStoreMappingDAO>();
            DirectSalesOrders = new HashSet<DirectSalesOrderDAO>();
            ERouteChangeRequestContents = new HashSet<ERouteChangeRequestContentDAO>();
            ERouteContents = new HashSet<ERouteContentDAO>();
            IndirectSalesOrderBuyerStores = new HashSet<IndirectSalesOrderDAO>();
            IndirectSalesOrderSellerStores = new HashSet<IndirectSalesOrderDAO>();
            InverseParentStore = new HashSet<StoreDAO>();
            PriceListStoreMappings = new HashSet<PriceListStoreMappingDAO>();
            Problems = new HashSet<ProblemDAO>();
            StoreCheckingImageMappings = new HashSet<StoreCheckingImageMappingDAO>();
            StoreCheckings = new HashSet<StoreCheckingDAO>();
            StoreImageMappings = new HashSet<StoreImageMappingDAO>();
            StoreUncheckings = new HashSet<StoreUncheckingDAO>();
            SurveyResults = new HashSet<SurveyResultDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
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
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public long? StoreScoutingId { get; set; }

        public virtual DistrictDAO District { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual StoreDAO ParentStore { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual ResellerDAO Reseller { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual StoreGroupingDAO StoreGrouping { get; set; }
        public virtual StoreScoutingDAO StoreScouting { get; set; }
        public virtual StoreTypeDAO StoreType { get; set; }
        public virtual WardDAO Ward { get; set; }
        public virtual ICollection<AlbumImageMappingDAO> AlbumImageMappings { get; set; }
        public virtual ICollection<AppUserStoreMappingDAO> AppUserStoreMappings { get; set; }
        public virtual ICollection<DirectSalesOrderDAO> DirectSalesOrders { get; set; }
        public virtual ICollection<ERouteChangeRequestContentDAO> ERouteChangeRequestContents { get; set; }
        public virtual ICollection<ERouteContentDAO> ERouteContents { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrderBuyerStores { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrderSellerStores { get; set; }
        public virtual ICollection<StoreDAO> InverseParentStore { get; set; }
        public virtual ICollection<PriceListStoreMappingDAO> PriceListStoreMappings { get; set; }
        public virtual ICollection<ProblemDAO> Problems { get; set; }
        public virtual ICollection<StoreCheckingImageMappingDAO> StoreCheckingImageMappings { get; set; }
        public virtual ICollection<StoreCheckingDAO> StoreCheckings { get; set; }
        public virtual ICollection<StoreImageMappingDAO> StoreImageMappings { get; set; }
        public virtual ICollection<StoreUncheckingDAO> StoreUncheckings { get; set; }
        public virtual ICollection<SurveyResultDAO> SurveyResults { get; set; }
    }
}