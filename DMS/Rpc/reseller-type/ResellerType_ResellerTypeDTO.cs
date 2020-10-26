using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.reseller_type
{
    public class ResellerType_ResellerTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public ResellerType_ResellerTypeDTO() { }
        public ResellerType_ResellerTypeDTO(ResellerType ResellerType)
        {
            this.Id = ResellerType.Id;
            this.Code = ResellerType.Code;
            this.Name = ResellerType.Name;
            this.StatusId = ResellerType.StatusId;
            this.Errors = ResellerType.Errors;
        }
    }

    public class ResellerType_ResellerTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public ResellerTypeOrder OrderBy { get; set; }
    }
}
