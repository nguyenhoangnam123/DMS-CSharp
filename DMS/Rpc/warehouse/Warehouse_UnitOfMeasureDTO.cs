using Common;
using DMS.Entities;

namespace DMS.Rpc.warehouse
{
    public class Warehouse_UnitOfMeasureDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
        public string NameLower { get; set; }

        public string Description { get; set; }

        public long StatusId { get; set; }


        public Warehouse_UnitOfMeasureDTO() { }
        public Warehouse_UnitOfMeasureDTO(UnitOfMeasure UnitOfMeasure)
        {

            this.Id = UnitOfMeasure.Id;

            this.Code = UnitOfMeasure.Code;

            this.Name = UnitOfMeasure.Name;
            this.NameLower = UnitOfMeasure.Name.ToLower();

            this.Description = UnitOfMeasure.Description;

            this.StatusId = UnitOfMeasure.StatusId;

        }
    }

    public class Warehouse_UnitOfMeasureFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter Description { get; set; }

        public IdFilter StatusId { get; set; }

        public UnitOfMeasureOrder OrderBy { get; set; }
    }
}