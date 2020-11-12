using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.price_list
{
    public class PriceList_RoleDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public PriceList_RoleDTO() { }
        public PriceList_RoleDTO(Role Role)
        {
            this.Id = Role.Id;
            this.Code = Role.Code;
            this.Name = Role.Name;
        }
    }
}
