﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_SupplierDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string TaxCode { get; set; }

        public long StatusId { get; set; }


        public StoreChecking_SupplierDTO() { }
        public StoreChecking_SupplierDTO(Supplier Supplier)
        {

            this.Id = Supplier.Id;

            this.Code = Supplier.Code;

            this.Name = Supplier.Name;

            this.TaxCode = Supplier.TaxCode;

            this.StatusId = Supplier.StatusId;

        }
    }

    public class StoreChecking_SupplierFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter TaxCode { get; set; }

        public IdFilter StatusId { get; set; }

        public SupplierOrder OrderBy { get; set; }
    }
}
