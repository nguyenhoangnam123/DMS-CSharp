using Common;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ProblemTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public StoreChecking_ProblemTypeDTO() { }
        public StoreChecking_ProblemTypeDTO(ProblemType ProblemType)
        {
            this.Id = ProblemType.Id;
            this.Code = ProblemType.Code;
            this.Name = ProblemType.Name;
        }
    }
}
