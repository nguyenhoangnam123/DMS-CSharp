﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.app_user
{
    public class AppUser_ProvinceDTO : DataDTO
    {

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public long? Priority { get; set; }

        public long StatusId { get; set; }


        public AppUser_ProvinceDTO() { }
        public AppUser_ProvinceDTO(Province Province)
        {

            this.Id = Province.Id;
            this.Code = Province.Code;
            this.Name = Province.Name;

            this.Priority = Province.Priority;

            this.StatusId = Province.StatusId;

        }
    }

    public class AppUser_ProvinceFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }

        public LongFilter Priority { get; set; }

        public IdFilter StatusId { get; set; }

        public ProvinceOrder OrderBy { get; set; }
    }
}
