using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.problem
{
    public class Problem_ProblemHistoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long ProblemId { get; set; }
        public DateTime Time { get; set; }
        public long ModifierId { get; set; }
        public long ProblemStatusId { get; set; }
        public Problem_AppUserDTO Modifier { get; set; }
        public Problem_ProblemStatusDTO ProblemStatus { get; set; }
        public Problem_ProblemHistoryDTO() { }
        public Problem_ProblemHistoryDTO(ProblemHistory ProblemHistory)
        {
            this.Id = ProblemHistory.Id;
            this.ProblemId = ProblemHistory.ProblemId;
            this.Time = ProblemHistory.Time;
            this.ModifierId = ProblemHistory.ModifierId;
            this.ProblemStatusId = ProblemHistory.ProblemStatusId;
            this.Modifier = ProblemHistory.Modifier == null ? null : new Problem_AppUserDTO(ProblemHistory.Modifier);
            this.ProblemStatus = ProblemHistory.ProblemStatus == null ? null : new Problem_ProblemStatusDTO(ProblemHistory.ProblemStatus);
            this.Errors = ProblemHistory.Errors;
        }
    }

    public class Problem_ProblemHistoryFilterDTO : FilterDTO
    {
        public long Id { get; set; }
        public long ProblemId { get; set; }
        public DateTime Time { get; set; }
        public long ModifierId { get; set; }
        public long ProblemStatusId { get; set; }
        public ProblemHistoryOrder OrderBy { get; set; }
    }
}
