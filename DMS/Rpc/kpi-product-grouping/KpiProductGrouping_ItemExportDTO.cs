using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_product_grouping
{
    public class KpiProductGrouping_ItemExportDTO : DataDTO
    {
        public string ItemCode { get; set; }
        public bool IsNew { get; set; }
        public string ItemName { get; set; }
        public string ProductGroupingCode { get; set; }
        public string ProductGroupingName { get; set; }
    }
}
