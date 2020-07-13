using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneral_ReportStoreGeneralDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportStoreGeneral_StoreDTO> Stores { get; set; }
    }


    public class ReportStoreGeneral_ReportStoreGeneralFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public DateFilter CheckIn { get; set; }
    }

}
