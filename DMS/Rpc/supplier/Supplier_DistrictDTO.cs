﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.supplier
{
    public class Supplier_DistrictDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public long? Priority { get; set; }

        public long ProvinceId { get; set; }

        public long StatusId { get; set; }


        public Supplier_DistrictDTO() { }
        public Supplier_DistrictDTO(District District)
        {

            this.Id = District.Id;
            this.Code = District.Code;
            this.Name = District.Name;

            this.Priority = District.Priority;

            this.ProvinceId = District.ProvinceId;

            this.StatusId = District.StatusId;

        }
    }

    public class Supplier_DistrictFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }

        public LongFilter Priority { get; set; }

        public IdFilter ProvinceId { get; set; }

        public IdFilter StatusId { get; set; }

        public DistrictOrder OrderBy { get; set; }
    }
}
