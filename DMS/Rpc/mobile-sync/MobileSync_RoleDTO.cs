using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_RoleDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public MobileSync_RoleDTO() { }
        public MobileSync_RoleDTO(RoleDAO RoleDAO)
        {
            this.Id = RoleDAO.Id;
            this.Code = RoleDAO.Code;
            this.Name = RoleDAO.Name;
        }
    }
}
