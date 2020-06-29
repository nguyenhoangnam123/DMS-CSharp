using Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_ProblemTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Mobile_ProblemTypeDTO() { }
        public Mobile_ProblemTypeDTO(ProblemType ProblemType)
        {
            this.Id = ProblemType.Id;
            this.Code = ProblemType.Code;
            this.Name = ProblemType.Name;
        }
    }
}
