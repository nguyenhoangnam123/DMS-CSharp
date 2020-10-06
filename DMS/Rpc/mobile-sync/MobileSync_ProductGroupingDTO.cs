using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProductGroupingDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long? ParentId { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }


        public MobileSync_ProductGroupingDTO() { }
        public MobileSync_ProductGroupingDTO(ProductGroupingDAO ProductGroupingDAO)
        {
            this.Id = ProductGroupingDAO.Id;
            this.Code = ProductGroupingDAO.Code;
            this.Name = ProductGroupingDAO.Name;
            this.ParentId = ProductGroupingDAO.ParentId;
            this.Path = ProductGroupingDAO.Path;
            this.Description = ProductGroupingDAO.Description;
        }
    }

    public class IndirectSalesOrder_ProductGroupingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter ParentId { get; set; }

        public StringFilter Path { get; set; }

        public StringFilter Description { get; set; }

        public ProductGroupingOrder OrderBy { get; set; }
    }
}
