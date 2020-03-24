using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class UnitOfMeasureDAO
    {
        public UnitOfMeasureDAO()
        {
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
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContents { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings { get; set; }
    }
}
