using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.e_route
{
    public class ERoute_StoreExportDTO : DataDTO
    {
        public long STT { get; set; }
        public string Code { get; set; }
        public string CodeDraft { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public ERoute_StoreExportDTO() { }
        public ERoute_StoreExportDTO(Store Store)
        {
            this.Code = Store.Code;
            this.CodeDraft = Store.CodeDraft;
            this.Name = Store.Name;
            this.Address = Store.Address;
            this.Errors = Store.Errors;
        }
    }

}
