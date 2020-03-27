using Common;
using DMS.Entities;
using DMS.Enums;
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

namespace DMS.Rpc.permission
{
    public class PermissionRoute : Root
    {
        public const string Master = Module + "/permission/permission-master";
        public const string Detail = Module + "/permission/permission-detail";
        private const string Default = Rpc + Module + "/permission";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListMenu = Default + "/single-list-menu";
        public const string SingleListRole = Default + "/single-list-role";
        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(PermissionFilter.Id), FieldType.ID },
            { nameof(PermissionFilter.Code), FieldType.STRING },
            { nameof(PermissionFilter.Name), FieldType.STRING },
            { nameof(PermissionFilter.RoleId), FieldType.ID },
            { nameof(PermissionFilter.MenuId), FieldType.ID },
            { nameof(PermissionFilter.StatusId), FieldType.ID },
        };
    }

    public class PermissionController : RpcController
    {
        private IMenuService MenuService;
        private IRoleService RoleService;
        private IStatusService StatusService;
        private IPermissionService PermissionService;
        private ICurrentContext CurrentContext;
        public PermissionController(
            IMenuService MenuService,
            IRoleService RoleService,
            IStatusService StatusService,
            IPermissionService PermissionService,
            ICurrentContext CurrentContext
        )
        {
            this.MenuService = MenuService;
            this.RoleService = RoleService;
            this.StatusService = StatusService;
            this.PermissionService = PermissionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PermissionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            PermissionFilter = PermissionService.ToFilter(PermissionFilter);
            int count = await PermissionService.Count(PermissionFilter);
            return count;
        }

        [Route(PermissionRoute.List), HttpPost]
        public async Task<ActionResult<List<Permission_PermissionDTO>>> List([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            PermissionFilter = PermissionService.ToFilter(PermissionFilter);
            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            List<Permission_PermissionDTO> Permission_PermissionDTOs = Permissions
                .Select(c => new Permission_PermissionDTO(c)).ToList();
            return Permission_PermissionDTOs;
        }

        [Route(PermissionRoute.Get), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Get([FromBody]Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Permission_PermissionDTO.Id))
                return Forbid();

            Permission Permission = await PermissionService.Get(Permission_PermissionDTO.Id);
            return new Permission_PermissionDTO(Permission);
        }

        [Route(PermissionRoute.Create), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Create([FromBody] Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Permission_PermissionDTO.Id))
                return Forbid();

            Permission Permission = ConvertDTOToEntity(Permission_PermissionDTO);
            Permission = await PermissionService.Create(Permission);
            Permission_PermissionDTO = new Permission_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Permission_PermissionDTO;
            else
                return BadRequest(Permission_PermissionDTO);
        }

        [Route(PermissionRoute.Update), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Update([FromBody] Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Permission_PermissionDTO.Id))
                return Forbid();

            Permission Permission = ConvertDTOToEntity(Permission_PermissionDTO);
            Permission = await PermissionService.Update(Permission);
            Permission_PermissionDTO = new Permission_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Permission_PermissionDTO;
            else
                return BadRequest(Permission_PermissionDTO);
        }

        [Route(PermissionRoute.Delete), HttpPost]
        public async Task<ActionResult<Permission_PermissionDTO>> Delete([FromBody] Permission_PermissionDTO Permission_PermissionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Permission_PermissionDTO.Id))
                return Forbid();

            Permission Permission = ConvertDTOToEntity(Permission_PermissionDTO);
            Permission = await PermissionService.Delete(Permission);
            Permission_PermissionDTO = new Permission_PermissionDTO(Permission);
            if (Permission.IsValidated)
                return Permission_PermissionDTO;
            else
                return BadRequest(Permission_PermissionDTO);
        }

        [Route(PermissionRoute.Import), HttpPost]
        public async Task<ActionResult<List<Permission_PermissionDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Permission> Permissions = await PermissionService.Import(DataFile);
            List<Permission_PermissionDTO> Permission_PermissionDTOs = Permissions
                .Select(c => new Permission_PermissionDTO(c)).ToList();
            return Permission_PermissionDTOs;
        }

        [Route(PermissionRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO);
            PermissionFilter = PermissionService.ToFilter(PermissionFilter);
            DataFile DataFile = await PermissionService.Export(PermissionFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(PermissionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Id = new IdFilter { In = Ids };
            PermissionFilter.Selects = PermissionSelect.Id;
            PermissionFilter.Skip = 0;
            PermissionFilter.Take = int.MaxValue;

            List<Permission> Permissions = await PermissionService.List(PermissionFilter);
            Permissions = await PermissionService.BulkDelete(Permissions);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter = PermissionService.ToFilter(PermissionFilter);
            if (Id == 0)
            {

            }
            else
            {
                PermissionFilter.Id = new IdFilter { Equal = Id };
                int count = await PermissionService.Count(PermissionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Permission ConvertDTOToEntity(Permission_PermissionDTO Permission_PermissionDTO)
        {
            Permission Permission = new Permission();
            Permission.Id = Permission_PermissionDTO.Id;
            Permission.Code = Permission_PermissionDTO.Code;
            Permission.Name = Permission_PermissionDTO.Name;
            Permission.RoleId = Permission_PermissionDTO.RoleId;
            Permission.MenuId = Permission_PermissionDTO.MenuId;
            Permission.StatusId = Permission_PermissionDTO.StatusId;
            Permission.Menu = Permission_PermissionDTO.Menu == null ? null : new Menu
            {
                Id = Permission_PermissionDTO.Menu.Id,
                Name = Permission_PermissionDTO.Menu.Name,
                Path = Permission_PermissionDTO.Menu.Path,
                IsDeleted = Permission_PermissionDTO.Menu.IsDeleted,
            };
            Permission.Role = Permission_PermissionDTO.Role == null ? null : new Role
            {
                Id = Permission_PermissionDTO.Role.Id,
                Code = Permission_PermissionDTO.Role.Code,
                Name = Permission_PermissionDTO.Role.Name,
                StatusId = Permission_PermissionDTO.Role.StatusId,
            };
            Permission.Status = Permission_PermissionDTO.Status == null ? null : new Status
            {
                Id = Permission_PermissionDTO.Status.Id,
                Code = Permission_PermissionDTO.Status.Code,
                Name = Permission_PermissionDTO.Status.Name,
            };
            Permission.PermissionFieldMappings = Permission_PermissionDTO.PermissionFieldMappings?
                .Select(x => new PermissionFieldMapping
                {
                    FieldId = x.FieldId,
                    Value = x.Value,
                    Field = new Field
                    {
                        Id = x.Field.Id,
                        Name = x.Field.Name,
                        Type = x.Field.Type,
                        MenuId = x.Field.MenuId,
                        IsDeleted = x.Field.IsDeleted,
                    },
                }).ToList();
            Permission.PermissionPageMappings = Permission_PermissionDTO.PermissionPageMappings?
                .Select(x => new PermissionPageMapping
                {
                    PageId = x.PageId,
                    Page = new Page
                    {
                        Id = x.Page.Id,
                        Name = x.Page.Name,
                        Path = x.Page.Path,
                        MenuId = x.Page.MenuId,
                        IsDeleted = x.Page.IsDeleted,
                    },
                }).ToList();
            Permission.BaseLanguage = CurrentContext.Language;
            return Permission;
        }

        private PermissionFilter ConvertFilterDTOToFilterEntity(Permission_PermissionFilterDTO Permission_PermissionFilterDTO)
        {
            PermissionFilter PermissionFilter = new PermissionFilter();
            PermissionFilter.Selects = PermissionSelect.ALL;
            PermissionFilter.Skip = Permission_PermissionFilterDTO.Skip;
            PermissionFilter.Take = Permission_PermissionFilterDTO.Take;
            PermissionFilter.OrderBy = Permission_PermissionFilterDTO.OrderBy;
            PermissionFilter.OrderType = Permission_PermissionFilterDTO.OrderType;

            PermissionFilter.Id = Permission_PermissionFilterDTO.Id;
            PermissionFilter.Code = Permission_PermissionFilterDTO.Code;
            PermissionFilter.Name = Permission_PermissionFilterDTO.Name;
            PermissionFilter.RoleId = Permission_PermissionFilterDTO.RoleId;
            PermissionFilter.MenuId = Permission_PermissionFilterDTO.MenuId;
            PermissionFilter.StatusId = Permission_PermissionFilterDTO.StatusId;
            return PermissionFilter;
        }

        [Route(PermissionRoute.SingleListMenu), HttpPost]
        public async Task<List<Permission_MenuDTO>> SingleListMenu([FromBody] Permission_MenuFilterDTO Permission_MenuFilterDTO)
        {
            MenuFilter MenuFilter = new MenuFilter();
            MenuFilter.Skip = 0;
            MenuFilter.Take = 20;
            MenuFilter.OrderBy = MenuOrder.Id;
            MenuFilter.OrderType = OrderType.ASC;
            MenuFilter.Selects = MenuSelect.ALL;
            MenuFilter.Id = Permission_MenuFilterDTO.Id;
            MenuFilter.Code = Permission_MenuFilterDTO.Code;
            MenuFilter.Name = Permission_MenuFilterDTO.Name;
            MenuFilter.Path = Permission_MenuFilterDTO.Path;

            List<Menu> Menus = await MenuService.List(MenuFilter);
            List<Permission_MenuDTO> Permission_MenuDTOs = Menus
                .Select(x => new Permission_MenuDTO(x)).ToList();
            return Permission_MenuDTOs;
        }
        [Route(PermissionRoute.SingleListRole), HttpPost]
        public async Task<List<Permission_RoleDTO>> SingleListRole([FromBody] Permission_RoleFilterDTO Permission_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = Permission_RoleFilterDTO.Id;
            RoleFilter.Code = Permission_RoleFilterDTO.Code;
            RoleFilter.Name = Permission_RoleFilterDTO.Name;
            RoleFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<Permission_RoleDTO> Permission_RoleDTOs = Roles
                .Select(x => new Permission_RoleDTO(x)).ToList();
            return Permission_RoleDTOs;
        }
        [Route(PermissionRoute.SingleListStatus), HttpPost]
        public async Task<List<Permission_StatusDTO>> SingleListStatus([FromBody] Permission_StatusFilterDTO Permission_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Permission_StatusFilterDTO.Id;
            StatusFilter.Code = Permission_StatusFilterDTO.Code;
            StatusFilter.Name = Permission_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Permission_StatusDTO> Permission_StatusDTOs = Statuses
                .Select(x => new Permission_StatusDTO(x)).ToList();
            return Permission_StatusDTOs;
        }
    }
}

