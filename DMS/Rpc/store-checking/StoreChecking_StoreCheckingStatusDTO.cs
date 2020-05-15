using Common;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_StoreCheckingStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public StoreChecking_StoreCheckingStatusDTO() { }
        public StoreChecking_StoreCheckingStatusDTO(Status Status)
        {

            this.Id = Status.Id;

            this.Code = Status.Code;

            this.Name = Status.Name;

        }
    }

    public class StoreChecking_StoreCheckingStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StatusOrder OrderBy { get; set; }
    }
}
