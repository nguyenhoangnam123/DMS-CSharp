using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.tax_type
{
    public class TaxType_StatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public TaxType_StatusDTO() { }
        public TaxType_StatusDTO(Status Status)
        {

            this.Id = Status.Id;

            this.Code = Status.Code;

            this.Name = Status.Name;

        }
    }

    public class TaxType_StatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StatusOrder OrderBy { get; set; }
    }
}