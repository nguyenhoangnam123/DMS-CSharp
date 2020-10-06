using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProductProductGroupingMappingDTO : DataDTO
    {
        public long ProductId { get; set; }
        public long ProductGroupingId { get; set; }
        public MobileSync_ProductGroupingDTO ProductGrouping { get; set; }

        public MobileSync_ProductProductGroupingMappingDTO() { }
        public MobileSync_ProductProductGroupingMappingDTO(ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO)
        {
            this.ProductId = ProductProductGroupingMappingDAO.ProductId;
            this.ProductGroupingId = ProductProductGroupingMappingDAO.ProductGroupingId;
            this.ProductGrouping = ProductProductGroupingMappingDAO.ProductGrouping == null ? null : new MobileSync_ProductGroupingDTO(ProductProductGroupingMappingDAO.ProductGrouping);
        }
    }

    public class IndirectSalesOrder_ProductProductGroupingMappingFilterDTO : FilterDTO
    {

        public IdFilter ProductId { get; set; }

        public IdFilter ProductGroupingId { get; set; }

        public ProductProductGroupingMappingOrder OrderBy { get; set; }
    }
}
