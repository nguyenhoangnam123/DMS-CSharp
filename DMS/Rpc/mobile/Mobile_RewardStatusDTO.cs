using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_RewardStatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Mobile_RewardStatusDTO() {}
        public Mobile_RewardStatusDTO(RewardStatus RewardStatus)
        {
            this.Id = RewardStatus.Id;
            this.Code = RewardStatus.Code;
            this.Name = RewardStatus.Name;
            this.Errors = RewardStatus.Errors;
        }
    }

    public class Mobile_RewardStatusFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public RewardStatusOrder OrderBy { get; set; }
    }
}
