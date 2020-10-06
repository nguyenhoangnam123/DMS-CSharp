using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public MobileSync_ProblemTypeDTO() { }
        public MobileSync_ProblemTypeDTO(ProblemTypeDAO ProblemTypeDAO)
        {
            this.Id = ProblemTypeDAO.Id;
            this.Code = ProblemTypeDAO.Code;
            this.Name = ProblemTypeDAO.Name;
        }
    }

    public class MonitorStoreProblem_ProblemTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public ProblemTypeOrder OrderBy { get; set; }
    }
}