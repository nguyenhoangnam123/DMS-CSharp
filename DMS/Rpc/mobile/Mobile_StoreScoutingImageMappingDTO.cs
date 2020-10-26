using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreScoutingImageMappingDTO : DataDTO
    {
        public long StoreScoutingId { get; set; }
        public long ImageId { get; set; }
        public Mobile_ImageDTO Image { get; set; }
        public Mobile_StoreScoutingImageMappingDTO() { }
        public Mobile_StoreScoutingImageMappingDTO(StoreScoutingImageMapping StoreScoutingImageMapping)
        {
            this.StoreScoutingId = StoreScoutingImageMapping.StoreScoutingId;
            this.ImageId = StoreScoutingImageMapping.ImageId;
            this.Image = StoreScoutingImageMapping.Image == null ? null : new Mobile_ImageDTO(StoreScoutingImageMapping.Image);
        }

        public class StoreScouting_StoreScoutingImageMappingFilterDTO : FilterDTO
        {
            public IdFilter StoreScoutingId { get; set; }

            public IdFilter ImageId { get; set; }

            public StoreScoutingImageMappingOrder OrderBy { get; set; }
        }
    }
}
