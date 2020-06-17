using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public Dashboard_AppUserDTO() { }
        public Dashboard_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Latitude = AppUser.Latitude;
            this.Longitude = AppUser.Longitude;
            this.Errors = AppUser.Errors;
        }
    }

    public class Dashboard_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter DisplayName { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
