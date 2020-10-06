using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_OrganizationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public long StatusId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public MobileSync_OrganizationDTO() { }
        public MobileSync_OrganizationDTO(OrganizationDAO OrganizationDAO)
        {
            this.Id = OrganizationDAO.Id;
            this.Code = OrganizationDAO.Code;
            this.Name = OrganizationDAO.Name;
            this.ParentId = OrganizationDAO.ParentId;
            this.Path = OrganizationDAO.Path;
            this.Level = OrganizationDAO.Level;
            this.StatusId = OrganizationDAO.StatusId;
            this.Phone = OrganizationDAO.Phone;
            this.Address = OrganizationDAO.Address;
            this.Email = OrganizationDAO.Email;
        }
    }
}
