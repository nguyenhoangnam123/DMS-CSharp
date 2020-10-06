using Common;
using DMS.Entities;
using DMS.Models;
using System;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProductTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MobileSync_ProductTypeDTO() { }
        public MobileSync_ProductTypeDTO(ProductTypeDAO ProductTypeDAO)
        {

            this.Id = ProductTypeDAO.Id;

            this.Code = ProductTypeDAO.Code;

            this.Name = ProductTypeDAO.Name;

            this.Description = ProductTypeDAO.Description;

            this.StatusId = ProductTypeDAO.StatusId;
            this.UpdatedAt = ProductTypeDAO.UpdatedAt;
        }
    }

    public class IndirectSalesOrder_ProductTypeFilterDTO : FilterDTO
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
