using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ProblemDTO : DataDTO
    {
        public long Id { get; set; }
        public long? StoreCheckingId { get; set; }
        public long StoreId { get; set; }
        public long CreatorId { get; set; }
        public long ProblemTypeId { get; set; }
        public long ProblemStatusId { get; set; }
        public DateTime NoteAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Content { get; set; }
        public Guid RowId { get; set; }
        public StoreChecking_AppUserDTO Creator { get; set; }
        public StoreChecking_ProblemTypeDTO ProblemType { get; set; }
        public StoreChecking_ProblemStatusDTO ProblemStatus { get; set; }
        public StoreChecking_StoreDTO Store { get; set; }
        public List<StoreChecking_ProblemImageMappingDTO> ProblemImageMappings { get; set; }

        public StoreChecking_ProblemDTO() { }
        public StoreChecking_ProblemDTO(Problem Problem)
        {
            this.Id = Problem.Id;
            this.StoreCheckingId = Problem.StoreCheckingId;
            this.StoreId = Problem.StoreId;
            this.CreatorId = Problem.CreatorId;
            this.ProblemTypeId = Problem.ProblemTypeId;
            this.ProblemStatusId = Problem.ProblemStatusId;
            this.NoteAt = Problem.NoteAt;
            this.CompletedAt = Problem.CompletedAt;
            this.Content = Problem.Content;
            this.RowId = Problem.RowId;
            this.Creator = Problem.Creator == null ? null : new StoreChecking_AppUserDTO(Problem.Creator);
            this.ProblemType = Problem.ProblemType == null ? null : new StoreChecking_ProblemTypeDTO(Problem.ProblemType);
            this.ProblemStatus = Problem.ProblemStatus == null ? null : new StoreChecking_ProblemStatusDTO(Problem.ProblemStatus);
            this.Store = Problem.Store == null ? null : new StoreChecking_StoreDTO(Problem.Store);
            this.ProblemImageMappings = Problem.ProblemImageMappings?.Select(pi => new StoreChecking_ProblemImageMappingDTO(pi)).ToList();
        }
    }

    public class StoreChecking_ProblemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreCheckingId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter ProblemTypeId { get; set; }
        public IdFilter ProblemStatusId { get; set; }
        public DateFilter NoteAt { get; set; }
        public DateFilter CompletedAt { get; set; }
        public StringFilter Content { get; set; }
        public ProblemOrder OrderBy { get; set; }
        public ProblemSelect Selects { get; set; }
    }
}
