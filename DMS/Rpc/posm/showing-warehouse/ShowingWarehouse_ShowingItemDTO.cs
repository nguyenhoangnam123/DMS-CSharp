using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.showing_warehouse
{
    public class ShowingWarehouse_ShowingItemDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long CategoryId { get; set; }
        
        public long UnitOfMeasureId { get; set; }
        
        public decimal SalePrice { get; set; }
        
        public string Desception { get; set; }
        
        public long StatusId { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        

        public ShowingWarehouse_ShowingItemDTO() {}
        public ShowingWarehouse_ShowingItemDTO(ShowingItem ShowingItem)
        {
            
            this.Id = ShowingItem.Id;
            
            this.Code = ShowingItem.Code;
            
            this.Name = ShowingItem.Name;
            
            this.CategoryId = ShowingItem.CategoryId;
            
            this.UnitOfMeasureId = ShowingItem.UnitOfMeasureId;
            
            this.SalePrice = ShowingItem.SalePrice;
            
            this.Desception = ShowingItem.Desception;
            
            this.StatusId = ShowingItem.StatusId;
            
            this.Used = ShowingItem.Used;
            
            this.RowId = ShowingItem.RowId;
            
            this.Errors = ShowingItem.Errors;
        }
    }

    public class ShowingWarehouse_ShowingItemFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter CategoryId { get; set; }
        
        public IdFilter UnitOfMeasureId { get; set; }
        
        public DecimalFilter SalePrice { get; set; }
        
        public StringFilter Desception { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public ShowingItemOrder OrderBy { get; set; }
    }
}