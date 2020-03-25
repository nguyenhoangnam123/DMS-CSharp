using Common;
using DMS.Entities;

namespace DMS.Rpc.permission
{
    public class Permission_PermissionPageMappingDTO : DataDTO
    {
        public long PermissionId { get; set; }
        public long PageId { get; set; }
        public Permission_PageDTO Page { get; set; }

        public Permission_PermissionPageMappingDTO() { }
        public Permission_PermissionPageMappingDTO(PermissionPageMapping PermissionPageMapping)
        {
            this.PermissionId = PermissionPageMapping.PermissionId;
            this.PageId = PermissionPageMapping.PageId;
            this.Page = PermissionPageMapping.Page == null ? null : new Permission_PageDTO(PermissionPageMapping.Page);
        }
    }

    public class Permission_PermissionPageMappingFilterDTO : FilterDTO
    {

        public IdFilter PermissionId { get; set; }

        public IdFilter PageId { get; set; }

        public PermissionPageMappingOrder OrderBy { get; set; }
    }
}