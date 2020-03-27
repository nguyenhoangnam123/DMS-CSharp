﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store
{
    public class Store_StoreImageMappingDTO : DataDTO
    {
        public long StoreId { get; set; }
        public long ImageId { get; set; }
        public Store_StoreDTO Store { get; set; }
        public Store_ImageDTO Image { get; set; }
        public Store_StoreImageMappingDTO() { }
        public Store_StoreImageMappingDTO(StoreImageMapping StoreImageMapping)
        {
            this.StoreId = StoreImageMapping.StoreId;
            this.ImageId = StoreImageMapping.ImageId;
            this.Store = StoreImageMapping.Store == null ? null : new Store_StoreDTO(StoreImageMapping.Store);
            this.Image = StoreImageMapping.Image == null ? null : new Store_ImageDTO(StoreImageMapping.Image);
        }

        public class Store_StoreImageMappingFilterDTO : FilterDTO
        {
            public IdFilter StoreId { get; set; }

            public IdFilter ImageId { get; set; }

            public StoreImageMappingOrder OrderBy { get; set; }
        }
    }
}
