using Common;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_StoreTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }


        public IndirectPriceList_StoreTypeDTO() { }
        public IndirectPriceList_StoreTypeDTO(StoreType StoreType)
        {

            this.Id = StoreType.Id;

            this.Code = StoreType.Code;

            this.Name = StoreType.Name;

            this.StatusId = StoreType.StatusId;

            this.Errors = StoreType.Errors;
        }
    }

    public class IndirectPriceList_StoreTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public StoreTypeOrder OrderBy { get; set; }
    }
}