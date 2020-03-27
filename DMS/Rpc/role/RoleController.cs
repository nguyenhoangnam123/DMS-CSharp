using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MMenu;
using DMS.Services.MPermission;
using DMS.Services.MRole;
using DMS.Services.MStatus;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListMenu = Default + "/single-list-menu";
        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";
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
        private IAppUserService AppUserService;
        private IMenuService MenuService;
        private IRoleService RoleService;
        private IStatusService StatusService;

        public RoleController(
            IAppUserService AppUserService,
            IMenuService MenuService,
            IRoleService RoleService,
            IStatusService StatusService
        )
        {
            this.AppUserService = AppUserService;
            this.MenuService = MenuService;
            this.RoleService = RoleService;
            this.StatusService = StatusService;
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

        private async Task<bool> HasPermission(long Id)
        {
            RoleFilter RoleFilter = new RoleFilter();
            if (Id == 0)
            {

            }
            else
            {
                RoleFilter.Id = new IdFilter { Equal = Id };
                RoleFilter = RoleService.ToFilter(RoleFilter);
                int count = await RoleService.Count(RoleFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        public Role ConvertDTOToEntity(Role_RoleDTO Role_RoleDTO)
        {
            Role Role = new Role();
            Role.Id = Role_RoleDTO.Id;
            Role.Code = Role_RoleDTO.Code;
            Role.Name = Role_RoleDTO.Name;
            Role.StatusId = Role_RoleDTO.StatusId;
            Role.AppUserRoleMappings = Role_RoleDTO.AppUserRoleMappings?
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId,
                    AppUser = new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        Password = x.AppUser.Password,
                        DisplayName = x.AppUser.DisplayName,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                        StatusId = x.AppUser.StatusId,
                    },
                }).ToList();
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
                }).ToList();

            return Role;
        }

        public RoleFilter ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO Role_RoleFilterDTO)
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

        [Route(RoleRoute.SingleListAppUser), HttpPost]
        public async Task<List<Role_AppUserDTO>> SingleListAppUser([FromBody] Role_AppUserFilterDTO Role_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Role_AppUserFilterDTO.Id;
            AppUserFilter.Username = Role_AppUserFilterDTO.Username;
            AppUserFilter.Password = Role_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Role_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Role_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Role_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Role_AppUserDTO> Role_AppUserDTOs = AppUsers
                .Select(x => new Role_AppUserDTO(x)).ToList();
            return Role_AppUserDTOs;
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
        [Route(RoleRoute.CountAppUser), HttpPost]
        public async Task<long> CountAppUser([FromBody] Role_AppUserFilterDTO Role_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();

            return await AppUserService.Count(AppUserFilter);
        }

        [Route(RoleRoute.ListAppUser), HttpPost]
        public async Task<List<Role_AppUserDTO>> ListAppUser([FromBody] Role_AppUserFilterDTO Role_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = Role_AppUserFilterDTO.Skip;
            AppUserFilter.Take = Role_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Role_AppUserDTO> Role_AppUserDTOs = AppUsers
                .Select(x => new Role_AppUserDTO(x)).ToList();
            return Role_AppUserDTOs;
        }
    }
}

