using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneral_StoreStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public ReportStoreGeneral_StoreStatusDTO() { }
        public ReportStoreGeneral_StoreStatusDTO(StoreStatus StoreStatus)
        {

            this.Id = StoreStatus.Id;

            this.Code = StoreStatus.Code;

            this.Name = StoreStatus.Name;

        }
    }

    public class ReportStoreGeneral_StoreStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StatusOrder OrderBy { get; set; }
    }
}