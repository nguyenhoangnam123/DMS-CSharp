using Common;
using DMS.Entities;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_StoreTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }


        public DirectPriceList_StoreTypeDTO() { }
        public DirectPriceList_StoreTypeDTO(StoreType StoreType)
        {

            this.Id = StoreType.Id;

            this.Code = StoreType.Code;

            this.Name = StoreType.Name;

            this.StatusId = StoreType.StatusId;

            this.Errors = StoreType.Errors;
        }
    }

    public class DirectPriceList_StoreTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public StoreTypeOrder OrderBy { get; set; }
    }
}