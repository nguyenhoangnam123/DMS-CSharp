﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemSyncDTO
    {
        public List<MobileSync_ProblemDTO> Created { get; set; }
        public List<MobileSync_ProblemDTO> Updated { get; set; }
        public List<MobileSync_ProblemDTO> Deleted { get; set; }
    }
}
