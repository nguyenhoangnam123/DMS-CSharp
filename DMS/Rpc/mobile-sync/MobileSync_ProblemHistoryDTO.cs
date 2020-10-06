using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemHistoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long ProblemId { get; set; }
        public DateTime Time { get; set; }
        public long ModifierId { get; set; }
        public long ProblemStatusId { get; set; }
        public MobileSync_AppUserDTO Modifier { get; set; }
        public MobileSync_ProblemStatusDTO ProblemStatus { get; set; }
        public MobileSync_ProblemHistoryDTO() { }
        public MobileSync_ProblemHistoryDTO(ProblemHistoryDAO ProblemHistoryDAO)
        {
            this.Id = ProblemHistoryDAO.Id;
            this.ProblemId = ProblemHistoryDAO.ProblemId;
            this.Time = ProblemHistoryDAO.Time;
            this.ModifierId = ProblemHistoryDAO.ModifierId;
            this.ProblemStatusId = ProblemHistoryDAO.ProblemStatusId;
            this.Modifier = ProblemHistoryDAO.Modifier == null ? null : new MobileSync_AppUserDTO(ProblemHistoryDAO.Modifier);
            this.ProblemStatus = ProblemHistoryDAO.ProblemStatus == null ? null : new MobileSync_ProblemStatusDTO(ProblemHistoryDAO.ProblemStatus);
        }
    }

    public class MobileSync_ProblemHistoryFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ProblemId { get; set; }
        public DateFilter Time { get; set; }
        public IdFilter ModifierId { get; set; }
        public IdFilter ProblemStatusId { get; set; }
        public ProblemHistoryOrder OrderBy { get; set; }
    }
}
