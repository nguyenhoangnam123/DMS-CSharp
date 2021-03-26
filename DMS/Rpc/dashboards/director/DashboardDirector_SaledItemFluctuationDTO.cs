﻿using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirector_SaledItemFluctuationDTO : DataDTO
    {
        public List<DashboardDirector_SaledItemFluctuationByMonthDTO> SaledItemFluctuationByMonths { get; set; }
        public List<DashboardDirector_SaledItemFluctuationByQuarterDTO> SaledItemFluctuationByQuaters { get; set; }
        public List<DashboardDirector_SaledItemFluctuationByYearDTO> SaledItemFluctuationByYears { get; set; }
    }

    public class DashboardDirector_SaledItemFluctuationByMonthDTO : DataDTO
    {
        public long Day { get; set; }
        public decimal SaledItemCounter { get; set; }
    }

    public class DashboardDirector_SaledItemFluctuationByQuarterDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal SaledItemCounter { get; set; }
    }

    public class DashboardDirector_SaledItemFluctuationByYearDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal SaledItemCounter { get; set; }
    }

    public class DashboardDirector_SaledItemFluctuationFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
