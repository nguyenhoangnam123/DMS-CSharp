using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.store_information
{
    public class DashboardStoreInformation_StoreDTO : DataDTO
    {
        public long Id { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public bool IsScouting { get; set; }
        public long StoreStatusId { get; set; }
        public DashboardStoreInformation_StoreDTO() { }
        public DashboardStoreInformation_StoreDTO(Store Store)
        {
            this.Id = Store.Id;
            this.Longitude = Store.Longitude;
            this.Latitude = Store.Latitude;
            this.Name = Store.Name;
            this.Address = Store.Address;
            this.StoreStatusId = Store.StoreStatusId;
            this.Telephone = Store.Telephone;
        }
    }

    public class DashboardStoreInformation_StoreFilterDTO : FilterDTO
    {   
        public IdFilter OrganizationId { get; set; }
        public IdFilter BrandId { get; set; }
    }
}
