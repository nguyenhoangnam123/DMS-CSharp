using Common;
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
        internal long? IndirectQuantity { get; set; }
        public string eIndirectQuantity => (IndirectQuantity.HasValue == false || (IndirectQuantity.HasValue && IndirectQuantity == 0)) ? "" : IndirectQuantity.ToString();
        internal decimal? IndirectRevenue { get; set; }
        public string eIndirectRevenue => (IndirectRevenue.HasValue == false || (IndirectRevenue.HasValue && IndirectRevenue == 0)) ? "" : Math.Round(IndirectRevenue.Value, 0).ToString();
        internal long? IndirectCounter { get; set; }
        public string eIndirectCounter => (IndirectCounter.HasValue == false || (IndirectCounter.HasValue && IndirectCounter == 0)) ? "" : IndirectCounter.ToString();
        internal long? IndirectStoreCounter { get; set; }
        public string eIndirectStoreCounter => (IndirectStoreCounter.HasValue == false || (IndirectStoreCounter.HasValue && IndirectStoreCounter == 0)) ? "" : IndirectStoreCounter.ToString();
        internal long? DirectQuantity { get; set; }
        public string eDirectQuantity => (DirectQuantity.HasValue == false || (DirectQuantity.HasValue && DirectQuantity == 0)) ? "" : DirectQuantity.ToString();
        internal decimal? DirectRevenue { get; set; }
        public string eDirectRevenue => (DirectRevenue.HasValue == false || (DirectRevenue.HasValue && DirectRevenue == 0)) ? "" : Math.Round(DirectRevenue.Value, 0).ToString();
        internal long? DirectCounter { get; set; }
        public string eDirectCounter => (DirectCounter.HasValue == false || (DirectCounter.HasValue && DirectCounter == 0)) ? "" : DirectCounter.ToString();
        internal long? DirectStoreCounter { get; set; }
        public string eDirectStoreCounter => (DirectStoreCounter.HasValue == false || (DirectStoreCounter.HasValue && DirectStoreCounter == 0)) ? "" : DirectStoreCounter.ToString();
    }
}
