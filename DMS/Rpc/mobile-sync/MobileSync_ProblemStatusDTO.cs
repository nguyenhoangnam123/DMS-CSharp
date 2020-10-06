using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProblemStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public MobileSync_ProblemStatusDTO() { }
        public MobileSync_ProblemStatusDTO(ProblemStatusDAO ProblemStatusDAO)
        {
            this.Id = ProblemStatusDAO.Id;
            this.Code = ProblemStatusDAO.Code;
            this.Name = ProblemStatusDAO.Name;
        }
    }
}