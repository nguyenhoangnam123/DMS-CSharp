using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.permission
{
    public class Permission_PermissionDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        public long StatusId { get; set; }
        public Permission_MenuDTO Menu { get; set; }
        public Permission_RoleDTO Role { get; set; }
        public Permission_StatusDTO Status { get; set; }
        public List<Permission_PermissionFieldMappingDTO> PermissionFieldMappings { get; set; }
        public List<Permission_PermissionPageMappingDTO> PermissionPageMappings { get; set; }
        public Permission_PermissionDTO() { }
        public Permission_PermissionDTO(Permission Permission)
        {
            this.Id = Permission.Id;
            this.Code = Permission.Code;
            this.Name = Permission.Name;
            this.RoleId = Permission.RoleId;
            this.MenuId = Permission.MenuId;
            this.StatusId = Permission.StatusId;
            this.Menu = Permission.Menu == null ? null : new Permission_MenuDTO(Permission.Menu);
            this.Role = Permission.Role == null ? null : new Permission_RoleDTO(Permission.Role);
            this.Status = Permission.Status == null ? null : new Permission_StatusDTO(Permission.Status);
            this.PermissionFieldMappings = Permission.PermissionFieldMappings?.Select(x => new Permission_PermissionFieldMappingDTO(x)).ToList();
            this.PermissionPageMappings = Permission.PermissionPageMappings?.Select(x => new Permission_PermissionPageMappingDTO(x)).ToList();
        }
    }

    public class Permission_PermissionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter RoleId { get; set; }
        public IdFilter MenuId { get; set; }
        public IdFilter StatusId { get; set; }
        public PermissionOrder OrderBy { get; set; }
    }
}
