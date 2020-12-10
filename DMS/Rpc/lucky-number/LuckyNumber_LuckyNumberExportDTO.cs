using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.lucky_number
{
    public class LuckyNumber_LuckyNumberExportDTO : DataDTO
    {
        public long STT { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string RewardStatus { get; set; }
        public string Date { get; set; }
    }
}
