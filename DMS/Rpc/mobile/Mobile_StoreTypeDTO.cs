﻿using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long? ColorId { get; set; }
        public long StatusId { get; set; }
        public Mobile_ColorDTO Color { get; set; }

        public Mobile_StoreTypeDTO() { }
        public Mobile_StoreTypeDTO(StoreType StoreType)
        {

            this.Id = StoreType.Id;

            this.Code = StoreType.Code;

            this.Name = StoreType.Name;

            this.ColorId = StoreType.ColorId;
            this.StatusId = StoreType.StatusId;
            this.Color = StoreType.Color == null ? null : new Mobile_ColorDTO(StoreType.Color);
        }
    }

    public class Mobile_StoreTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter ColorId { get; set; }
        public IdFilter StatusId { get; set; }

        public StoreTypeOrder OrderBy { get; set; }
    }
}
