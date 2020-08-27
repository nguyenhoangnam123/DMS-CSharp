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
        internal long? Quantity { get; set; }
        public string eQuantity => (Quantity.HasValue == false || (Quantity.HasValue && Quantity == 0)) ? "" : Quantity.ToString();
        internal decimal? Revenue { get; set; }
        public string eRevenue => (Revenue.HasValue == false || (Revenue.HasValue && Revenue == 0)) ? "" : Math.Round(Revenue.Value, 0).ToString();
        internal long? SalesAmount { get; set; }
        public string eSalesAmount => (SalesAmount.HasValue == false || (SalesAmount.HasValue && SalesAmount == 0)) ? "" : SalesAmount.ToString();
        internal long? StoreAmount { get; set; }
        public string eStoreAmount => (StoreAmount.HasValue == false || (StoreAmount.HasValue && StoreAmount == 0)) ? "" : StoreAmount.ToString();
    }
}
