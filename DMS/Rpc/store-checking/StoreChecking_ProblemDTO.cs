using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ProblemDTO : DataDTO
    {
        public long Id { get; set; }
        public long? StoreCheckingId { get; set; }
        public long StoreId { get; set; }
        public long ProblemTypeId { get; set; }
        public DateTime NoteAt { get; set; }
        public string Content { get; set; }
        public StoreChecking_ProblemTypeDTO ProblemType { get; set; }
        public StoreChecking_StoreDTO Store { get; set; }
        public List<StoreChecking_ProblemImageMappingDTO> ProblemImageMappings { get; set; }

        public StoreChecking_ProblemDTO() { }
        public StoreChecking_ProblemDTO(Problem Problem)
        {
            this.Id = Problem.Id;
            this.StoreCheckingId = Problem.StoreCheckingId;
            this.StoreId = Problem.StoreId;
            this.ProblemTypeId = Problem.ProblemTypeId;
            this.NoteAt = Problem.NoteAt;
            this.Content = Problem.Content;
            this.ProblemType = Problem.ProblemType == null ? null : new StoreChecking_ProblemTypeDTO(Problem.ProblemType);
            this.Store = Problem.Store == null ? null : new StoreChecking_StoreDTO(Problem.Store);
            this.ProblemImageMappings = Problem.ProblemImageMappings?.Select(pi => new StoreChecking_ProblemImageMappingDTO(pi)).ToList();
        }
    }

    public class StoreChecking_ProblemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreCheckingId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter ProblemTypeId { get; set; }
        public DateFilter NoteAt { get; set; }
        public StringFilter Content { get; set; }
        public ProblemOrder OrderBy { get; set; }
        public ProblemSelect Selects { get; set; }
    }
}
