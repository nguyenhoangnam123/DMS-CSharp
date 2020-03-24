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
using DMS.Services.MAppUser;
using DMS.Services.MUserStatus;
using DMS.Services.MRole;

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

        public const string SingleListUserStatus = Default + "/single-list-user-status";
        public const string SingleListRole = Default + "/single-list-role";
        public const string CountRole = Default + "/count-role";
        public const string ListRole = Default + "/list-role";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(AppUserFilter.Id), FieldType.ID },
            { nameof(AppUserFilter.Username), FieldType.STRING },
            { nameof(AppUserFilter.Password), FieldType.STRING },
            { nameof(AppUserFilter.DisplayName), FieldType.STRING },
            { nameof(AppUserFilter.Email), FieldType.STRING },
            { nameof(AppUserFilter.Phone), FieldType.STRING },
            { nameof(AppUserFilter.UserStatusId), FieldType.ID },
        };
    }

    public class AppUserController : RpcController
    {
        private IUserStatusService UserStatusService;
        private IRoleService RoleService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            IUserStatusService UserStatusService,
            IRoleService RoleService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.UserStatusService = UserStatusService;
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
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.UserStatusId = AppUser_AppUserDTO.UserStatusId;
            AppUser.UserStatus = AppUser_AppUserDTO.UserStatus == null ? null : new UserStatus
            {
                Id = AppUser_AppUserDTO.UserStatus.Id,
                Code = AppUser_AppUserDTO.UserStatus.Code,
                Name = AppUser_AppUserDTO.UserStatus.Name,
            };
            AppUser.AppUserRoleMappings = AppUser_AppUserDTO.AppUserRoleMappings?
                .Select(x => new AppUserRoleMapping
                {
                    RoleId = x.RoleId,
                    Role = new Role
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
            AppUserFilter.Email = AppUser_AppUserFilterDTO.Email;
            AppUserFilter.Phone = AppUser_AppUserFilterDTO.Phone;
            AppUserFilter.UserStatusId = AppUser_AppUserFilterDTO.UserStatusId;
            return AppUserFilter;
        }

        [Route(AppUserRoute.SingleListUserStatus), HttpPost]
        public async Task<List<AppUser_UserStatusDTO>> SingleListUserStatus([FromBody] AppUser_UserStatusFilterDTO AppUser_UserStatusFilterDTO)
        {
            UserStatusFilter UserStatusFilter = new UserStatusFilter();
            UserStatusFilter.Skip = 0;
            UserStatusFilter.Take = 20;
            UserStatusFilter.OrderBy = UserStatusOrder.Id;
            UserStatusFilter.OrderType = OrderType.ASC;
            UserStatusFilter.Selects = UserStatusSelect.ALL;
            UserStatusFilter.Id = AppUser_UserStatusFilterDTO.Id;
            UserStatusFilter.Code = AppUser_UserStatusFilterDTO.Code;
            UserStatusFilter.Name = AppUser_UserStatusFilterDTO.Name;

            List<UserStatus> UserStatuses = await UserStatusService.List(UserStatusFilter);
            List<AppUser_UserStatusDTO> AppUser_UserStatusDTOs = UserStatuses
                .Select(x => new AppUser_UserStatusDTO(x)).ToList();
            return AppUser_UserStatusDTOs;
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

