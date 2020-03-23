using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MRole;
using DMS.Services.MStatus;
using DMS.Services.MPermission;
using DMS.Services.MMenu;

namespace DMS.Rpc.role
{
    public class RoleRoute : Root
    {
        public const string Master = Module + "/role/role-master";
        public const string Detail = Module + "/role/role-detail";
        private const string Default = Rpc + Module + "/role";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListPermission = Default + "/single-list-permission";
        public const string SingleListMenu = Default + "/single-list-menu";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(RoleFilter.Id), FieldType.ID },
            { nameof(RoleFilter.Code), FieldType.STRING },
            { nameof(RoleFilter.Name), FieldType.STRING },
            { nameof(RoleFilter.StatusId), FieldType.ID },
        };
    }

    public class RoleController : RpcController
    {
        private IStatusService StatusService;
        private IPermissionService PermissionService;
        private IMenuService MenuService;
        private IRoleService RoleService;
        private ICurrentContext CurrentContext;
        public RoleController(
            IStatusService StatusService,
            IPermissionService PermissionService,
            IMenuService MenuService,
            IRoleService RoleService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.PermissionService = PermissionService;
            this.MenuService = MenuService;
            this.RoleService = RoleService;
            this.CurrentContext = CurrentContext;
        }

        [Route(RoleRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            RoleFilter = RoleService.ToFilter(RoleFilter);
            int count = await RoleService.Count(RoleFilter);
            return count;
        }

        [Route(RoleRoute.List), HttpPost]
        public async Task<ActionResult<List<Role_RoleDTO>>> List([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            RoleFilter = RoleService.ToFilter(RoleFilter);
            List<Role> Roles = await RoleService.List(RoleFilter);
            List<Role_RoleDTO> Role_RoleDTOs = Roles
                .Select(c => new Role_RoleDTO(c)).ToList();
            return Role_RoleDTOs;
        }

        [Route(RoleRoute.Get), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Get([FromBody]Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = await RoleService.Get(Role_RoleDTO.Id);
            return new Role_RoleDTO(Role);
        }

        [Route(RoleRoute.Create), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Create([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Create(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Update), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Update([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Update(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Delete), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> Delete([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.Delete(Role);
            Role_RoleDTO = new Role_RoleDTO(Role);
            if (Role.IsValidated)
                return Role_RoleDTO;
            else
                return BadRequest(Role_RoleDTO);
        }

        [Route(RoleRoute.Import), HttpPost]
        public async Task<ActionResult<List<Role_RoleDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Role> Roles = await RoleService.Import(DataFile);
            List<Role_RoleDTO> Role_RoleDTOs = Roles
                .Select(c => new Role_RoleDTO(c)).ToList();
            return Role_RoleDTOs;
        }

        [Route(RoleRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            RoleFilter = RoleService.ToFilter(RoleFilter);
            DataFile DataFile = await RoleService.Export(RoleFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(RoleRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Id.In = Ids;
            RoleFilter.Selects = RoleSelect.Id;
            RoleFilter.Skip = 0;
            RoleFilter.Take = int.MaxValue;

            List<Role> Roles = await RoleService.List(RoleFilter);
            Roles = await RoleService.BulkDelete(Roles);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            RoleFilter RoleFilter = new RoleFilter();
            if (Id > 0)
                RoleFilter.Id = new IdFilter { Equal = Id };
            RoleFilter = RoleService.ToFilter(RoleFilter);
            int count = await RoleService.Count(RoleFilter);
            if (count == 0)
                return false;
            return true;
        }

        private Role ConvertDTOToEntity(Role_RoleDTO Role_RoleDTO)
        {
            Role Role = new Role();
            Role.Id = Role_RoleDTO.Id;
            Role.Code = Role_RoleDTO.Code;
            Role.Name = Role_RoleDTO.Name;
            Role.StatusId = Role_RoleDTO.StatusId;
            Role.Status = Role_RoleDTO.Status == null ? null : new Status
            {
                Id = Role_RoleDTO.Status.Id,
                Code = Role_RoleDTO.Status.Code,
                Name = Role_RoleDTO.Status.Name,
            };
            Role.Permissions = Role_RoleDTO.Permissions?
                .Select(x => new Permission
                {
                    Id = x.Id,
                    Name = x.Name,
                    RoleId = x.RoleId,
                    MenuId = x.MenuId,
                    StatusId = x.StatusId,
                    Menu = new Menu
                    {
                        Id = x.Menu.Id,
                        Name = x.Menu.Name,
                        Path = x.Menu.Path,
                        IsDeleted = x.Menu.IsDeleted,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToList();
            Role.BaseLanguage = CurrentContext.Language;
            return Role;
        }

        private RoleFilter ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Skip = Role_RoleFilterDTO.Skip;
            RoleFilter.Take = Role_RoleFilterDTO.Take;
            RoleFilter.OrderBy = Role_RoleFilterDTO.OrderBy;
            RoleFilter.OrderType = Role_RoleFilterDTO.OrderType;

            RoleFilter.Id = Role_RoleFilterDTO.Id;
            RoleFilter.Code = Role_RoleFilterDTO.Code;
            RoleFilter.Name = Role_RoleFilterDTO.Name;
            RoleFilter.StatusId = Role_RoleFilterDTO.StatusId;
            return RoleFilter;
        }

        [Route(RoleRoute.SingleListStatus), HttpPost]
        public async Task<List<Role_StatusDTO>> SingleListStatus([FromBody] Role_StatusFilterDTO Role_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Role_StatusFilterDTO.Id;
            StatusFilter.Code = Role_StatusFilterDTO.Code;
            StatusFilter.Name = Role_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Role_StatusDTO> Role_StatusDTOs = Statuses
                .Select(x => new Role_StatusDTO(x)).ToList();
            return Role_StatusDTOs;
        }
        [Route(RoleRoute.SingleListPermission), HttpPost]
        public async Task<List<Role_PermissionDTO>> SingleListPermission([FromBody] Role_PermissionFilterDTO Role_PermissionFilterDTO)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Skip = 0;
            PermissionFilter.Take = 20;
            PermissionFilter.OrderBy = PermissionOrder.Id;
            PermissionFilter.OrderType = OrderType.ASC;
            PermissionFilter.Selects = PermissionSelect.ALL;
            PermissionFilter.Id = Role_PermissionFilterDTO.Id;
            PermissionFilter.Name = Role_PermissionFilterDTO.Name;
            PermissionFilter.RoleId = Role_PermissionFilterDTO.RoleId;
            PermissionFilter.MenuId = Role_PermissionFilterDTO.MenuId;
            PermissionFilter.StatusId = Role_PermissionFilterDTO.StatusId;

            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            List<Role_PermissionDTO> Role_PermissionDTOs = Permissions
                .Select(x => new Role_PermissionDTO(x)).ToList();
            return Role_PermissionDTOs;
        }
        [Route(RoleRoute.SingleListMenu), HttpPost]
        public async Task<List<Role_MenuDTO>> SingleListMenu([FromBody] Role_MenuFilterDTO Role_MenuFilterDTO)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Skip = 0;
            MenuFilter.Take = 20;
            MenuFilter.OrderBy = MenuOrder.Id;
            MenuFilter.OrderType = OrderType.ASC;
            MenuFilter.Selects = MenuSelect.ALL;
            MenuFilter.Id = Role_MenuFilterDTO.Id;
            MenuFilter.Name = Role_MenuFilterDTO.Name;
            MenuFilter.Path = Role_MenuFilterDTO.Path;

            List<Menu> Menus = await MenuService.List(MenuFilter);
            List<Role_MenuDTO> Role_MenuDTOs = Menus
                .Select(x => new Role_MenuDTO(x)).ToList();
            return Role_MenuDTOs;
        }

    }
}

