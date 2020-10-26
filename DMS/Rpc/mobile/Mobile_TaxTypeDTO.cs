﻿using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_TaxTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public decimal Percentage { get; set; }

        public long StatusId { get; set; }


        public Mobile_TaxTypeDTO() { }
        public Mobile_TaxTypeDTO(TaxType TaxType)
        {

            this.Id = TaxType.Id;

            this.Code = TaxType.Code;

            this.Name = TaxType.Name;

            this.Percentage = TaxType.Percentage;

            this.StatusId = TaxType.StatusId;

        }
    }

    public class Mobile_TaxTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public DecimalFilter Percentage { get; set; }

        public IdFilter StatusId { get; set; }

        public TaxTypeOrder OrderBy { get; set; }
    }
}
