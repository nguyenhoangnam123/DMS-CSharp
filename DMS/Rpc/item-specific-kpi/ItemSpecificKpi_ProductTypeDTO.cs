using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_ProductTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public DateTime UpdatedTime { get; set; }
        public ItemSpecificKpi_ProductTypeDTO() { }
        public ItemSpecificKpi_ProductTypeDTO(ProductType ProductType)
        {

            this.Id = ProductType.Id;

            this.Code = ProductType.Code;

            this.Name = ProductType.Name;

            this.Description = ProductType.Description;

            this.StatusId = ProductType.StatusId;
            this.UpdatedTime = ProductType.UpdatedTime;
        }
    }

    public class ItemSpecificKpi_ProductTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Description { get; set; }

        public IdFilter StatusId { get; set; }
        public DateFilter UpdatedTime { get; set; }
        public ProductTypeOrder OrderBy { get; set; }
    }
}
