﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.product
{
    public class Product_ItemImageMappingDTO : DataDTO
    {
        public long ItemId { get; set; }
        public long ImageId { get; set; }
        public Product_ImageDTO Image { get; set; }

        public Product_ItemImageMappingDTO() { }
        public Product_ItemImageMappingDTO(ItemImageMapping ItemImageMapping)
        {
            this.ItemId = ItemImageMapping.ItemId;
            this.ImageId = ItemImageMapping.ImageId;
            this.Image = ItemImageMapping.Image == null ? null : new Product_ImageDTO(ItemImageMapping.Image);
        }
    }

    public class Product_ItemImageMappingFilterDTO : FilterDTO
    {

        public IdFilter ItemId { get; set; }

        public IdFilter ImageId { get; set; }

        public ItemImageMappingOrder OrderBy { get; set; }
    }
}
