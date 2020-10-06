using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StoreTypeDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }


        public MobileSync_StoreTypeDTO() { }
        public MobileSync_StoreTypeDTO(StoreTypeDAO StoreTypeDAO)
        {
            this.Id = StoreTypeDAO.Id;
            this.Code = StoreTypeDAO.Code;
            this.Name = StoreTypeDAO.Name;
            this.StatusId = StoreTypeDAO.StatusId;
        }
    }

    public class IndirectSalesOrder_StoreTypeFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public StoreTypeOrder OrderBy { get; set; }
    }
}
