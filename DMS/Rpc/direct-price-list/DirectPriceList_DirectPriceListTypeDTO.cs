using Common;
using DMS.Entities;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_DirectPriceListTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public DirectPriceList_DirectPriceListTypeDTO() { }
        public DirectPriceList_DirectPriceListTypeDTO(DirectPriceListType DirectPriceListType)
        {

            this.Id = DirectPriceListType.Id;

            this.Code = DirectPriceListType.Code;

            this.Name = DirectPriceListType.Name;

            this.Errors = DirectPriceListType.Errors;
        }
    }

    public class DirectPriceList_DirectPriceListTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public DirectPriceListTypeOrder OrderBy { get; set; }
    }
}