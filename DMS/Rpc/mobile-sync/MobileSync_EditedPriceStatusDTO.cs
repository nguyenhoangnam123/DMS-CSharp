using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_EditedPriceStatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public MobileSync_EditedPriceStatusDTO() { }
        public MobileSync_EditedPriceStatusDTO(EditedPriceStatusDAO EditedPriceStatusDAO)
        {
            this.Id = EditedPriceStatusDAO.Id;
            this.Code = EditedPriceStatusDAO.Code;
            this.Name = EditedPriceStatusDAO.Name;
        }
    }
}