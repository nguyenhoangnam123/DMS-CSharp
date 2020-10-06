using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_RequestStateDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public MobileSync_RequestStateDTO() { }
        public MobileSync_RequestStateDTO(RequestStateDAO RequestStateDAO)
        {
            this.Id = RequestStateDAO.Id;
            this.Code = RequestStateDAO.Code;
            this.Name = RequestStateDAO.Name;
        }
    }
}
