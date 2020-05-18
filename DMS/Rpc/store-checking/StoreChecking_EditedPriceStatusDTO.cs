using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_EditedPriceStatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public StoreChecking_EditedPriceStatusDTO() { }
        public StoreChecking_EditedPriceStatusDTO(EditedPriceStatus EditedPriceStatus)
        {
            this.Id = EditedPriceStatus.Id;
            this.Code = EditedPriceStatus.Code;
            this.Name = EditedPriceStatus.Name;
            this.Errors = EditedPriceStatus.Errors;
        }
    }

    public class StoreChecking_EditedPriceStatusFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public EditedPriceStatusOrder OrderBy { get; set; }
    }
}
