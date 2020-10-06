using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_UnitOfMeasureDTO : DataDTO , IEquatable<MobileSync_UnitOfMeasureDTO>
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public long? Factor { get; set; }
        public long StatusId { get; set; }
        public MobileSync_UnitOfMeasureDTO() { }
        public MobileSync_UnitOfMeasureDTO(UnitOfMeasureDAO UnitOfMeasureDAO)
        {
            this.Id = UnitOfMeasureDAO.Id;
            this.Code = UnitOfMeasureDAO.Code;
            this.Name = UnitOfMeasureDAO.Name;
            this.Description = UnitOfMeasureDAO.Description;
            this.StatusId = UnitOfMeasureDAO.StatusId;
        }
        public MobileSync_UnitOfMeasureDTO(UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO)
        {

            this.Id = UnitOfMeasureGroupingContentDAO.UnitOfMeasure == null ? 0 : UnitOfMeasureGroupingContentDAO.UnitOfMeasure.Id;

            this.Code = UnitOfMeasureGroupingContentDAO.UnitOfMeasure == null ? string.Empty : UnitOfMeasureGroupingContentDAO.UnitOfMeasure.Code;

            this.Name = UnitOfMeasureGroupingContentDAO.UnitOfMeasure == null ? string.Empty : UnitOfMeasureGroupingContentDAO.UnitOfMeasure.Name;

            this.Description = UnitOfMeasureGroupingContentDAO.UnitOfMeasure == null ? string.Empty : UnitOfMeasureGroupingContentDAO.UnitOfMeasure.Description;

            this.StatusId = UnitOfMeasureGroupingContentDAO.UnitOfMeasure == null ? 0 : UnitOfMeasureGroupingContentDAO.UnitOfMeasure.StatusId;

            this.Factor = UnitOfMeasureGroupingContentDAO.Factor;
        }

        public bool Equals(MobileSync_UnitOfMeasureDTO other)
        {
            if (other == null) return false;
            return other.Id == this.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class IndirectSalesOrder_UnitOfMeasureFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }
        public IdFilter ProductId { get; set; }
        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Description { get; set; }

        public IdFilter StatusId { get; set; }
        public IdFilter UnitOfMeasureGroupingId { get; set; }
        public UnitOfMeasureOrder OrderBy { get; set; }
    }
}