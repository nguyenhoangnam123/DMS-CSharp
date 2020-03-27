using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.role
{
    public class Role_PermissionPageMappingDTO : DataDTO
    {
        public long PermissionId { get; set; }
        public long PageId { get; set; }
        public Role_PageDTO Page { get; set; }

        public Role_PermissionPageMappingDTO() { }
        public Role_PermissionPageMappingDTO(PermissionPageMapping PermissionPageMapping)
        {
            this.PermissionId = PermissionPageMapping.PermissionId;
            this.PageId = PermissionPageMapping.PageId;
            this.Page = PermissionPageMapping.Page == null ? null : new Role_PageDTO(PermissionPageMapping.Page);
        }
    }

    public class Role_PermissionPageMappingFilterDTO : FilterDTO
    {

        public IdFilter PermissionId { get; set; }

        public IdFilter PageId { get; set; }

        public PermissionPageMappingOrder OrderBy { get; set; }
    }
}
