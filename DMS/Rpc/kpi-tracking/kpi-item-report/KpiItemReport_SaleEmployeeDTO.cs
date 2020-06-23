using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string? OrganizationName { get; set; }
        public long? OrganizationId { get; set; }
        public List<KpiItemReport_SaleEmployeeItemDTO> KpiItemReport_ItemDTOs { get; set; }
    }
}
