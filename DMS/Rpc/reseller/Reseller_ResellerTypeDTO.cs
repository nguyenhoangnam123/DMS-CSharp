using Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.reseller
{
    public class Reseller_ResellerTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }

        public DateTime? DeteledAt { get; set; }


        public Reseller_ResellerTypeDTO() { }
        public Reseller_ResellerTypeDTO(ResellerType ResellerType)
        {

            this.Id = ResellerType.Id;

            this.Code = ResellerType.Code;

            this.Name = ResellerType.Name;

            this.StatusId = ResellerType.StatusId;

            this.Errors = ResellerType.Errors;
        }
    }

    public class Reseller_ResellerTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public ResellerTypeOrder OrderBy { get; set; }
    }
}