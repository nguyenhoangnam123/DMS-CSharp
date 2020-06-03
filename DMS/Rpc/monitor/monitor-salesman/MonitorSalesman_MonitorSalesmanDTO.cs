using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.Monitor.monitor_salesman
{
    public class MonitorSalesman_MonitorSalesmanDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<MonitorSalesman_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class MonitorSalesman_SaleEmployeeDTO : DataDTO
    {
        public long AppUserId { get; set; }
        public string AppUserCode { get; set; }
        public string AppUserName { get; set; }
        public long PlanCounter { get; set; }
        public long CheckinCounter { get; set; }
        public long ImageCounter { get; set; }
        public long Revenue { get; set; }
    }

    public class MonitorSalesman_MonitorSalesmanFilterDTO: DataDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateTime Date { get; set; }
    }
}
