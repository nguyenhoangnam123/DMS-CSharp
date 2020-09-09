using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.monitor_store_problems
{
    public class MonitorStoreProblem_ExportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<MonitorStoreProblem_ProblemDTO> MonitorStoreProblems { get; set; }
    }
}
