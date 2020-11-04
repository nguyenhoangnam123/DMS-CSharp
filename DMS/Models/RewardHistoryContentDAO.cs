using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class RewardHistoryContentDAO
    {
        public long Id { get; set; }
        public long RewardHistoryId { get; set; }
        public long LuckeyNumberId { get; set; }

        public virtual LuckyNumberDAO LuckeyNumber { get; set; }
        public virtual RewardHistoryDAO RewardHistory { get; set; }
    }
}
