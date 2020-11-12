using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.lucky_number
{
    public class LuckyNumber_RewardHistoryContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long RewardHistoryId { get; set; }
        public long LuckyNumberId { get; set; }
        public LuckyNumber_LuckyNumberDTO LuckyNumber { get; set; }   

        public LuckyNumber_RewardHistoryContentDTO() {}
        public LuckyNumber_RewardHistoryContentDTO(RewardHistoryContent RewardHistoryContent)
        {
            this.Id = RewardHistoryContent.Id;
            this.RewardHistoryId = RewardHistoryContent.RewardHistoryId;
            this.LuckyNumberId = RewardHistoryContent.LuckyNumberId;
            this.LuckyNumber = RewardHistoryContent.LuckyNumber == null ? null : new LuckyNumber_LuckyNumberDTO(RewardHistoryContent.LuckyNumber);
            this.Errors = RewardHistoryContent.Errors;
        }
    }

    public class LuckyNumber_RewardHistoryContentFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter RewardHistoryId { get; set; }
        
        public IdFilter LuckyNumberId { get; set; }
        
        public RewardHistoryContentOrder OrderBy { get; set; }
    }
}