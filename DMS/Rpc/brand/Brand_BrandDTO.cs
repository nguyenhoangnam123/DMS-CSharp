using Common;
using DMS.Entities;

namespace DMS.Rpc.brand
{
    public class Brand_BrandDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public Brand_StatusDTO Status { get; set; }
        public Brand_BrandDTO() { }
        public Brand_BrandDTO(Brand Brand)
        {
            this.Id = Brand.Id;
            this.Code = Brand.Code;
            this.Name = Brand.Name;
            this.StatusId = Brand.StatusId;
            this.Status = Brand.Status == null ? null : new Brand_StatusDTO(Brand.Status);
        }
    }

    public class Brand_BrandFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public BrandOrder OrderBy { get; set; }
    }
}
