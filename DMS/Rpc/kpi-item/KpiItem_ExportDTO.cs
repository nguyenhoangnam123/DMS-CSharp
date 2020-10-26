using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_item
{
    public class KpiItem_ExportDTO : DataDTO
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public List<KpiItem_ExportContentDTO> Contents { get; set; }
    }

    public class KpiItem_ExportContentDTO : DataDTO
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public long? IndirectQuantity { get; set; }
        public decimal? IndirectRevenue { get; set; }
        public long? IndirectCounter { get; set; }
        public long? IndirectStoreCounter { get; set; }
        public long? DirectQuantity { get; set; }
        public decimal? DirectRevenue { get; set; }
        public long? DirectCounter { get; set; }
        public long? DirectStoreCounter { get; set; }
    }
}
