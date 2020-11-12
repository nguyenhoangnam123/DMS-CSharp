using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_ProblemStatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Mobile_ProblemStatusDTO() { }
        public Mobile_ProblemStatusDTO(ProblemStatus ProblemStatus)
        {
            this.Id = ProblemStatus.Id;
            this.Code = ProblemStatus.Code;
            this.Name = ProblemStatus.Name;
        }
    }

    public class Mobile_ProblemStatusFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public ProblemOrder OrderBy { get; set; }
    }
}
