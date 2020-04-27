using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class UnitOfMeasureDAO
    {
        public UnitOfMeasureDAO()
        {
            IndirectSalesOrderContentPrimaryUnitOfMeasures = new HashSet<IndirectSalesOrderContentDAO>();
            IndirectSalesOrderContentUnitOfMeasures = new HashSet<IndirectSalesOrderContentDAO>();
            IndirectSalesOrderPromotionPrimaryUnitOfMeasures = new HashSet<IndirectSalesOrderPromotionDAO>();
            IndirectSalesOrderPromotionUnitOfMeasures = new HashSet<IndirectSalesOrderPromotionDAO>();
            Products = new HashSet<ProductDAO>();
            UnitOfMeasureGroupingContents = new HashSet<UnitOfMeasureGroupingContentDAO>();
            UnitOfMeasureGroupings = new HashSet<UnitOfMeasureGroupingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<IndirectSalesOrderContentDAO> IndirectSalesOrderContentPrimaryUnitOfMeasures { get; set; }
        public virtual ICollection<IndirectSalesOrderContentDAO> IndirectSalesOrderContentUnitOfMeasures { get; set; }
        public virtual ICollection<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionPrimaryUnitOfMeasures { get; set; }
        public virtual ICollection<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionUnitOfMeasures { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContents { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings { get; set; }
    }
}
