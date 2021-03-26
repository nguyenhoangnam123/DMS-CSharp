using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.export_template.dto
{
    public class ExportTemplate_StoreDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }
        public string CodeDraft { get; set; }

        public string Name { get; set; }

        public string Telephone { get; set; }
        public string Address { get; set; }

        public string DeliveryAddress { get; set; }
        public ExportTemplate_StoreDTO ParentStore { get; set; }
        public ExportTemplate_StoreGroupingDTO StoreGrouping { get; set; }
        public ExportTemplate_StoreTypeDTO StoreType { get; set; }
        public ExportTemplate_StoreStatusDTO StoreStatus { get; set; }
        public ExportTemplate_StoreDTO() { }
        public ExportTemplate_StoreDTO(Store Store)
        {

            this.Id = Store.Id;

            this.Code = Store.Code;
            this.CodeDraft = Store.CodeDraft;

            this.Name = Store.Name;


            this.Telephone = Store.Telephone;

            this.Address = Store.Address;

            this.DeliveryAddress = Store.DeliveryAddress;

            this.ParentStore = Store.ParentStore == null ? null : new ExportTemplate_StoreDTO(Store.ParentStore);
            this.StoreGrouping = Store.StoreGrouping == null ? null : new ExportTemplate_StoreGroupingDTO(Store.StoreGrouping);
            this.StoreType = Store.StoreType == null ? null : new ExportTemplate_StoreTypeDTO(Store.StoreType);
            this.StoreStatus = Store.StoreStatus == null ? null : new ExportTemplate_StoreStatusDTO(Store.StoreStatus);
            this.Errors = Store.Errors;
        }
    }
}