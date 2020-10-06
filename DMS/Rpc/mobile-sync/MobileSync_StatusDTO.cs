using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StatusDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public MobileSync_StatusDTO() { }
        public MobileSync_StatusDTO(StatusDAO StatusDAO)
        {
            this.Id = StatusDAO.Id;
            this.Code = StatusDAO.Code;
            this.Name = StatusDAO.Name;
        }
    }
}