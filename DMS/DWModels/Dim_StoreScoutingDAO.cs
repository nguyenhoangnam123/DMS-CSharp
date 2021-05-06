using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Dim_StoreScoutingDAO
    {
        public long StoreScoutingId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OwnerPhone { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public long CreatorId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreScoutingStatusId { get; set; }
        public long StoreScoutingTypeId { get; set; }
    }
}
