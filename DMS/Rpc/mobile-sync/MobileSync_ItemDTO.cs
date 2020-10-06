using Common;
using DMS.Entities;
using DMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ItemDTO : DataDTO
    {

        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long SaleStock { get; set; }
        public long StatusId { get; set; }
        public MobileSync_ProductDTO Product { get; set; }
        public List<MobileSync_ItemImageMappingDTO> ItemImageMappings { get; set; }
        public MobileSync_ItemDTO() { }
        public MobileSync_ItemDTO(ItemDAO ItemDAO)
        {
            this.Id = ItemDAO.Id;
            this.ProductId = ItemDAO.ProductId;
            this.Code = ItemDAO.Code;
            this.Name = ItemDAO.Name;
            this.ScanCode = ItemDAO.ScanCode;
            this.SalePrice = ItemDAO.SalePrice;
            this.RetailPrice = ItemDAO.RetailPrice;
            this.SaleStock = 0;
            this.StatusId = ItemDAO.StatusId;
            this.Product = ItemDAO.Product == null ? null : new MobileSync_ProductDTO(ItemDAO.Product);
            this.ItemImageMappings = ItemDAO.ItemImageMappings?.Select(x => new MobileSync_ItemImageMappingDTO(x)).ToList();
        }
    }

    public class MobileSync_ItemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ProductId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter ScanCode { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter RetailPrice { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter SalesEmployeeId { get; set; }
        public ItemOrder OrderBy { get; set; }
    }
}