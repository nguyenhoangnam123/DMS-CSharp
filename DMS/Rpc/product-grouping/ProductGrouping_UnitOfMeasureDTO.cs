﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product_grouping
{
    public class ProductGrouping_UnitOfMeasureDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long StatusId { get; set; }


        public ProductGrouping_UnitOfMeasureDTO() { }
        public ProductGrouping_UnitOfMeasureDTO(UnitOfMeasure UnitOfMeasure)
        {

            this.Id = UnitOfMeasure.Id;

            this.Code = UnitOfMeasure.Code;

            this.Name = UnitOfMeasure.Name;

            this.Description = UnitOfMeasure.Description;

            this.StatusId = UnitOfMeasure.StatusId;

        }
    }

    public class ProductGrouping_UnitOfMeasureFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Description { get; set; }

        public IdFilter StatusId { get; set; }

        public UnitOfMeasureOrder OrderBy { get; set; }
    }
}
