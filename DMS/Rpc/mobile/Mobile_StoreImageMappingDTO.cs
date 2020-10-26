using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreImageMappingDTO : DataDTO
    {
        public long StoreId { get; set; }
        public long ImageId { get; set; }
        public Mobile_ImageDTO Image { get; set; }
        public Mobile_StoreImageMappingDTO() { }
        public Mobile_StoreImageMappingDTO(StoreImageMapping StoreImageMapping)
        {
            this.StoreId = StoreImageMapping.StoreId;
            this.ImageId = StoreImageMapping.ImageId;
            this.Image = StoreImageMapping.Image == null ? null : new Mobile_ImageDTO(StoreImageMapping.Image);
        }

        public class Store_StoreImageMappingFilterDTO : FilterDTO
        {
            public IdFilter StoreId { get; set; }

            public IdFilter ImageId { get; set; }

            public StoreImageMappingOrder OrderBy { get; set; }
        }
    }
}
