using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ItemImageMappingDTO : DataDTO
    {
        public long ItemId { get; set; }
        public long ImageId { get; set; }
        public MobileSync_ImageDTO Image { get; set; }

        public MobileSync_ItemImageMappingDTO() { }
        public MobileSync_ItemImageMappingDTO(ItemImageMappingDAO ItemImageMappingDAO)
        {
            this.ItemId = ItemImageMappingDAO.ItemId;
            this.ImageId = ItemImageMappingDAO.ImageId;
            this.Image = ItemImageMappingDAO.Image == null ? null : new MobileSync_ImageDTO(ItemImageMappingDAO.Image);
        }
    }
}
