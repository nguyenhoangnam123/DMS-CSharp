using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.store_information
{
    public class DashboardStoreInformation_StoreCounterDTO : DataDTO
    {
        public long SurveyedStoreCounter { get; set; }
        public long StoreCounter { get; set; }
        public decimal Rate => StoreCounter == 0 ? 0 : Math.Round(((decimal)SurveyedStoreCounter / StoreCounter) * 100, 2);
    }

    public class DashboardStoreInformation_StoreCounterFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter BrandId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
    }
}
