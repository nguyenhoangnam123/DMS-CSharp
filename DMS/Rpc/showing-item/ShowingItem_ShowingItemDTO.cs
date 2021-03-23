using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.showing_item
{
    public class ShowingItem_ShowingItemDTO : DataDTO
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
        public ShowingItem_CategoryDTO Category { get; set; }
        public ShowingItem_StatusDTO Status { get; set; }
        public ShowingItem_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ShowingItem_ShowingItemDTO() {}
        public ShowingItem_ShowingItemDTO(ShowingItem ShowingItem)
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
            this.Category = ShowingItem.Category == null ? null : new ShowingItem_CategoryDTO(ShowingItem.Category);
            this.Status = ShowingItem.Status == null ? null : new ShowingItem_StatusDTO(ShowingItem.Status);
            this.UnitOfMeasure = ShowingItem.UnitOfMeasure == null ? null : new ShowingItem_UnitOfMeasureDTO(ShowingItem.UnitOfMeasure);
            this.CreatedAt = ShowingItem.CreatedAt;
            this.UpdatedAt = ShowingItem.UpdatedAt;
            this.Errors = ShowingItem.Errors;
        }
    }

    public class ShowingItem_ShowingItemFilterDTO : FilterDTO
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
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ShowingItemOrder OrderBy { get; set; }
    }
}
