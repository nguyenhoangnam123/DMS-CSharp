using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.problem
{
    public class Problem_ProblemDTO : DataDTO
    {
        public long Id { get; set; }
        public long? StoreCheckingId { get; set; }
        public long StoreId { get; set; }
        public long CreatorId { get; set; }
        public long ProblemTypeId { get; set; }
        public DateTime NoteAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Content { get; set; }
        public long ProblemStatusId { get; set; }
        public Problem_AppUserDTO Creator { get; set; }
        public Problem_ProblemStatusDTO ProblemStatus { get; set; }
        public Problem_ProblemTypeDTO ProblemType { get; set; }
        public Problem_StoreDTO Store { get; set; }
        public Problem_StoreCheckingDTO StoreChecking { get; set; }
        public List<Problem_ProblemImageMappingDTO> ProblemImageMappings { get; set; }
        public Problem_ProblemDTO() { }
        public Problem_ProblemDTO(Problem Problem)
        {
            this.Id = Problem.Id;
            this.StoreCheckingId = Problem.StoreCheckingId;
            this.StoreId = Problem.StoreId;
            this.CreatorId = Problem.CreatorId;
            this.ProblemTypeId = Problem.ProblemTypeId;
            this.NoteAt = Problem.NoteAt;
            this.CompletedAt = Problem.CompletedAt;
            this.Content = Problem.Content;
            this.ProblemStatusId = Problem.ProblemStatusId;
            this.Creator = Problem.Creator == null ? null : new Problem_AppUserDTO(Problem.Creator);
            this.ProblemStatus = Problem.ProblemStatus == null ? null : new Problem_ProblemStatusDTO(Problem.ProblemStatus);
            this.ProblemType = Problem.ProblemType == null ? null : new Problem_ProblemTypeDTO(Problem.ProblemType);
            this.Store = Problem.Store == null ? null : new Problem_StoreDTO(Problem.Store);
            this.StoreChecking = Problem.StoreChecking == null ? null : new Problem_StoreCheckingDTO(Problem.StoreChecking);
            this.ProblemImageMappings = Problem.ProblemImageMappings?.Select(x => new Problem_ProblemImageMappingDTO(x)).ToList();
            this.Errors = Problem.Errors;
        }
    }

    public class Problem_ProblemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreCheckingId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter ProblemTypeId { get; set; }
        public DateFilter NoteAt { get; set; }
        public DateFilter CompletedAt { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter ProblemStatusId { get; set; }
        public ProblemOrder OrderBy { get; set; }
    }
}
