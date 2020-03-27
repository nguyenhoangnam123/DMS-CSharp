using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MRole;
using DMS.Services.MSexService;
using DMS.Services.MStatus;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Services.MOrganization;

namespace DMS.Rpc.app_user
{
    public class AppUserRoute : Root
    {
        public const string Master = Module + "/app-user/app-user-master";
        public const string Detail = Module + "/app-user/app-user-detail";
        private const string Default = Rpc + Module + "/app-user";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListSex = Default + "/single-list-sex";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListRole = Default + "/single-list-role";
        public const string CountRole = Default + "/count-role";
        public const string ListRole = Default + "/list-role";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(AppUserFilter.Id), FieldType.ID },
            { nameof(AppUserFilter.Username), FieldType.STRING },
            { nameof(AppUserFilter.Password), FieldType.STRING },
            { nameof(AppUserFilter.DisplayName), FieldType.STRING },
            { nameof(AppUserFilter.Address), FieldType.STRING },
            { nameof(AppUserFilter.Email), FieldType.STRING },
            { nameof(AppUserFilter.Phone), FieldType.STRING },
            { nameof(AppUserFilter.Department), FieldType.STRING },
            { nameof(AppUserFilter.OrganizationId), FieldType.ID },
            { nameof(AppUserFilter.SexId), FieldType.ID },
            { nameof(AppUserFilter.StatusId), FieldType.ID },
        };
    }

    public class AppUserController : RpcController
    {
        private IOrganizationService OrganizationService;
        private ISexService SexService;
        private IStatusService StatusService;
        private IRoleService RoleService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            IOrganizationService OrganizationService,
            ISexService SexService,
            IStatusService StatusService,
            IRoleService RoleService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.SexService = SexService;
            this.StatusService = StatusService;
            this.RoleService = RoleService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AppUserRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(AppUserRoute.List), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> List([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody]AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = await AppUserService.Get(AppUser_AppUserDTO.Id);
            //AppUser AppUser = await AppUserService.Get(3);
            return new AppUser_AppUserDTO(AppUser);
        }

        [Route(AppUserRoute.Create), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Create([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Create(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Update([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Delete), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Delete([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Delete(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Import), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<AppUser> AppUsers = await AppUserService.Import(DataFile);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            DataFile DataFile = await AppUserService.Export(AppUserFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(AppUserRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            AppUserFilter.Id = new IdFilter { In = Ids };
            AppUserFilter.Selects = AppUserSelect.Id;
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            AppUsers = await AppUserService.BulkDelete(AppUsers);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);
            if (Id == 0)
            {

            }
            else
            {
                AppUserFilter.Id = new IdFilter { Equal = Id };
                int count = await AppUserService.Count(AppUserFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.Address = AppUser_AppUserDTO.Address;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.Department = AppUser_AppUserDTO.Department;
            AppUser.OrganizationId = AppUser_AppUserDTO.OrganizationId;
            AppUser.SexId = AppUser_AppUserDTO.SexId;
            AppUser.StatusId = AppUser_AppUserDTO.StatusId;
            AppUser.Organization = AppUser_AppUserDTO.Organization == null ? null : new Organization
            {
                Id = AppUser_AppUserDTO.Organization.Id,
                Code = AppUser_AppUserDTO.Organization.Code,
                Name = AppUser_AppUserDTO.Organization.Name,
                ParentId = AppUser_AppUserDTO.Organization.ParentId,
                Path = AppUser_AppUserDTO.Organization.Path,
                Level = AppUser_AppUserDTO.Organization.Level,
                StatusId = AppUser_AppUserDTO.Organization.StatusId,
                Phone = AppUser_AppUserDTO.Organization.Phone,
                Address = AppUser_AppUserDTO.Organization.Address,
                Latitude = AppUser_AppUserDTO.Organization.Latitude,
                Longitude = AppUser_AppUserDTO.Organization.Longitude,
            };
            AppUser.Sex = AppUser_AppUserDTO.Sex == null ? null : new Sex
            {
                Id = AppUser_AppUserDTO.Sex.Id,
                Code = AppUser_AppUserDTO.Sex.Code,
                Name = AppUser_AppUserDTO.Sex.Name,
            };
            AppUser.Status = AppUser_AppUserDTO.Status == null ? null : new Status
            {
                Id = AppUser_AppUserDTO.Status.Id,
                Code = AppUser_AppUserDTO.Status.Code,
                Name = AppUser_AppUserDTO.Status.Name,
            };
            AppUser.AppUserRoleMappings = AppUser_AppUserDTO.AppUserRoleMappings?
                .Select(x => new AppUserRoleMapping
                {
                    RoleId = x.RoleId,
                    Role = x.Role == null ? null : new Role
                    {
                        Id = x.Role.Id,
                        Code = x.Role.Code,
                        Name = x.Role.Name,
                        StatusId = x.Role.StatusId,
                    },
                }).ToList();
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        private AppUserFilter ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Skip = AppUser_AppUserFilterDTO.Skip;
            AppUserFilter.Take = AppUser_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUser_AppUserFilterDTO.OrderBy;
            AppUserFilter.OrderType = AppUser_AppUserFilterDTO.OrderType;

            AppUserFilter.Id = AppUser_AppUserFilterDTO.Id;
            AppUserFilter.Username = AppUser_AppUserFilterDTO.Username;
            AppUserFilter.Password = AppUser_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = AppUser_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = AppUser_AppUserFilterDTO.Address;
            AppUserFilter.Email = AppUser_AppUserFilterDTO.Email;
            AppUserFilter.Phone = AppUser_AppUserFilterDTO.Phone;
            AppUserFilter.Department = AppUser_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = AppUser_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = AppUser_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = AppUser_AppUserFilterDTO.StatusId;
            return AppUserFilter;
        }

        [Route(AppUserRoute.SingleListOrganization), HttpPost]
        public async Task<List<AppUser_OrganizationDTO>> SingleListOrganization([FromBody] AppUser_OrganizationFilterDTO AppUser_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = AppUser_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = AppUser_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = AppUser_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = AppUser_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = AppUser_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = AppUser_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = AppUser_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = AppUser_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = AppUser_OrganizationFilterDTO.Address;
            OrganizationFilter.Latitude = AppUser_OrganizationFilterDTO.Latitude;
            OrganizationFilter.Longitude = AppUser_OrganizationFilterDTO.Longitude;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<AppUser_OrganizationDTO> AppUser_OrganizationDTOs = Organizations
                .Select(x => new AppUser_OrganizationDTO(x)).ToList();
            return AppUser_OrganizationDTOs;
        }
        [Route(AppUserRoute.SingleListSex), HttpPost]
        public async Task<List<AppUser_SexDTO>> SingleListSex([FromBody] AppUser_SexFilterDTO AppUser_SexFilterDTO)
        {
            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = AppUser_SexFilterDTO.Id;
            SexFilter.Code = AppUser_SexFilterDTO.Code;
            SexFilter.Name = AppUser_SexFilterDTO.Name;

            List<Sex> Sexes = await SexService.List(SexFilter);
            List<AppUser_SexDTO> AppUser_SexDTOs = Sexes
                .Select(x => new AppUser_SexDTO(x)).ToList();
            return AppUser_SexDTOs;
        }
        [Route(AppUserRoute.SingleListStatus), HttpPost]
        public async Task<List<AppUser_StatusDTO>> SingleListStatus([FromBody] AppUser_StatusFilterDTO AppUser_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = AppUser_StatusFilterDTO.Id;
            StatusFilter.Code = AppUser_StatusFilterDTO.Code;
            StatusFilter.Name = AppUser_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<AppUser_StatusDTO> AppUser_StatusDTOs = Statuses
                .Select(x => new AppUser_StatusDTO(x)).ToList();
            return AppUser_StatusDTOs;
        }
        [Route(AppUserRoute.SingleListRole), HttpPost]
        public async Task<List<AppUser_RoleDTO>> SingleListRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;
            RoleFilter.StatusId = AppUser_RoleFilterDTO.StatusId;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<AppUser_RoleDTO> AppUser_RoleDTOs = Roles
                .Select(x => new AppUser_RoleDTO(x)).ToList();
            return AppUser_RoleDTOs;
        }

        [Route(AppUserRoute.CountRole), HttpPost]
        public async Task<long> CountRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;
            RoleFilter.StatusId = AppUser_RoleFilterDTO.StatusId;

            return await RoleService.Count(RoleFilter);
        }

        [Route(AppUserRoute.ListRole), HttpPost]
        public async Task<List<AppUser_RoleDTO>> ListRole([FromBody] AppUser_RoleFilterDTO AppUser_RoleFilterDTO)
        {
            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = AppUser_RoleFilterDTO.Skip;
            RoleFilter.Take = AppUser_RoleFilterDTO.Take;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = AppUser_RoleFilterDTO.Id;
            RoleFilter.Code = AppUser_RoleFilterDTO.Code;
            RoleFilter.Name = AppUser_RoleFilterDTO.Name;
            RoleFilter.StatusId = AppUser_RoleFilterDTO.StatusId;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<AppUser_RoleDTO> AppUser_RoleDTOs = Roles
                .Select(x => new AppUser_RoleDTO(x)).ToList();
            return AppUser_RoleDTOs;
        }
    }
}

