using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_SupplierDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string TaxCode { get; set; }

        public long StatusId { get; set; }


        public MobileSync_SupplierDTO() { }
        public MobileSync_SupplierDTO(SupplierDAO SupplierDAO)
        {
            this.Id = SupplierDAO.Id;
            this.Code = SupplierDAO.Code;
            this.Name = SupplierDAO.Name;
            this.TaxCode = SupplierDAO.TaxCode;
            this.StatusId = SupplierDAO.StatusId;
        }
    }
}
