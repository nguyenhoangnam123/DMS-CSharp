using Common;
using DMS.Entities;

namespace DMS.Rpc.problem
{
    public class Problem_ProblemStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Problem_ProblemStatusDTO() { }
        public Problem_ProblemStatusDTO(ProblemStatus ProblemStatus)
        {

            this.Id = ProblemStatus.Id;

            this.Code = ProblemStatus.Code;

            this.Name = ProblemStatus.Name;

            this.Errors = ProblemStatus.Errors;
        }
    }

    public class Problem_ProblemStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public ProblemStatusOrder OrderBy { get; set; }
    }
}