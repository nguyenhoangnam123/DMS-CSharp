using Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreStatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Mobile_StoreStatusDTO() { }
        public Mobile_StoreStatusDTO(StoreStatus StoreStatus)
        {

            this.Id = StoreStatus.Id;

            this.Code = StoreStatus.Code;

            this.Name = StoreStatus.Name;

        }
    }

    public class Mobile_StoreStatusFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StatusOrder OrderBy { get; set; }
    }
}