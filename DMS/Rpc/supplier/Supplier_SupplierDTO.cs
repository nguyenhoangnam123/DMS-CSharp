using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.supplier
{
    public class Supplier_SupplierDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public long StatusId { get; set; }
        public Supplier_StatusDTO Status { get; set; }
        public Supplier_SupplierDTO() {}
        public Supplier_SupplierDTO(Supplier Supplier)
        {
            this.Id = Supplier.Id;
            this.Code = Supplier.Code;
            this.Name = Supplier.Name;
            this.TaxCode = Supplier.TaxCode;
            this.StatusId = Supplier.StatusId;
            this.Status = Supplier.Status == null ? null : new Supplier_StatusDTO(Supplier.Status);
        }
    }

    public class Supplier_SupplierFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter TaxCode { get; set; }
        public IdFilter StatusId { get; set; }
        public SupplierOrder OrderBy { get; set; }
    }
}
