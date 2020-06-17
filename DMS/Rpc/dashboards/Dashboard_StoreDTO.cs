using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_StoreDTO : DataDTO
    {
        public long Id { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public Dashboard_StoreDTO() { }
        public Dashboard_StoreDTO(Store Store)
        {
            this.Id = Store.Id;
            this.Longitude = Store.Longitude;
            this.Latitude = Store.Latitude;
            this.Name = Store.Name;
            this.Telephone = Store.Telephone;
        }
    }
}
