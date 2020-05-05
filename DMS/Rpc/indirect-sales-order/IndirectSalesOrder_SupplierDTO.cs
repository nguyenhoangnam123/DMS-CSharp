using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_SupplierDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string TaxCode { get; set; }

        public long StatusId { get; set; }


        public IndirectSalesOrder_SupplierDTO() { }
        public IndirectSalesOrder_SupplierDTO(Supplier Supplier)
        {

            this.Id = Supplier.Id;

            this.Code = Supplier.Code;

            this.Name = Supplier.Name;

            this.TaxCode = Supplier.TaxCode;

            this.StatusId = Supplier.StatusId;

        }
    }

    public class IndirectSalesOrder_SupplierFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter TaxCode { get; set; }

        public IdFilter StatusId { get; set; }

        public SupplierOrder OrderBy { get; set; }
    }
}
