using Common;
using DMS.Entities;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public MobileSync_StatusDTO() { }
        public MobileSync_StatusDTO(Status Status)
        {

            this.Id = Status.Id;

            this.Code = Status.Code;

            this.Name = Status.Name;

            this.Errors = Status.Errors;
        }
    }
}