using Common;
using DMS.Entities;

namespace DMS.Rpc.product
{
    public class Product_BrandDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }


        public Product_BrandDTO() { }
        public Product_BrandDTO(Brand Brand)
        {

            this.Id = Brand.Id;

            this.Code = Brand.Code;

            this.Name = Brand.Name;

            this.StatusId = Brand.StatusId;

        }
    }

    public class Product_BrandFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public BrandOrder OrderBy { get; set; }
    }
}