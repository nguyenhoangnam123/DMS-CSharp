using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_EmployeeKpiDTO
    {
        public List<Mobile_EmployeeKpiGeneralDTO> KpiGenerals { get; set; }
        public List<Mobile_EmployeeKpiItemDTO> KpiItems { get; set; }
    }
    public class Mobile_EmployeeKpiFilterDTO : FilterDTO
    {
        public IdFilter EmployeeId { get; set; }
    }
}
