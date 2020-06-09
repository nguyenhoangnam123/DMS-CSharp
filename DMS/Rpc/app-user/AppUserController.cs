using Common;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MPosition;
using DMS.Services.MRole;
using DMS.Services.MSex;
using DMS.Services.MStatus;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.app_user
{
    public class AppUserController : RpcController
    {
        private IOrganizationService OrganizationService;
        private IPositionService PositionService;
        private ISexService SexService;
        private IStatusService StatusService;
        private IRoleService RoleService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            IOrganizationService OrganizationService,
            IPositionService PositionService,
            ISexService SexService,
            IStatusService StatusService,
            IRoleService RoleService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.PositionService = PositionService;
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
            return new AppUser_AppUserDTO(AppUser);
        }

        [Route(AppUserRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter = AppUserService.ToFilter(AppUserFilter);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code | OrganizationSelect.Name
            });
            List<Sex> Sexes = await SexService.List(new SexFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SexSelect.ALL
            });
            List<Role> Roles = await RoleService.List(new RoleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = RoleSelect.Code | RoleSelect.Name
            });
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Appuser sheet
                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { "STT", "Mã nhân viên","Tên nhân viên","Địa chỉ","Điện thọi","Email","Giới tính","Chức vụ","Phòng ban","Bộ phận quản lý"}
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var appUser = AppUsers[i];
                    data.Add(new Object[]
                    {
                        i+1,
                        appUser.Username,
                        appUser.DisplayName,
                        appUser.Address,
                        appUser.Phone,
                        appUser.Email,
                        appUser.Sex.Name,
                        appUser.Position,
                        appUser.Department,
                        appUser.Organization.Code
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, data);
                #endregion

                #region Org sheet
                data.Clear();
                var OrganizationHeader = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên"
                    }
                };
                foreach (var Organization in Organizations)
                {
                    data.Add(new object[]
                    {
                        Organization.Code,
                        Organization.Name,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeader, data);
                #endregion

                #region Sex sheet
                data.Clear();
                var SexHeader = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên"
                    }
                };
                foreach (var Sex in Sexes)
                {
                    data.Add(new object[]
                    {
                        Sex.Code,
                        Sex.Name,
                    });
                }
                excel.GenerateWorksheet("Sex", SexHeader, data);
                #endregion

                #region Role sheet
                data.Clear();
                var RoleHeader = new List<string[]>()
                {
                    new string[]
                    {
                        "Mã",
                        "Tên"
                    }
                };
                foreach (var Role in Roles)
                {
                    data.Add(new object[]
                    {
                        Role.Code,
                        Role.Name,
                    });
                }
                excel.GenerateWorksheet("Role", RoleHeader, data);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "AppUser.xlsx");
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
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.Address = AppUser_AppUserDTO.Address;
            AppUser.Avatar = AppUser_AppUserDTO.Avatar;
            AppUser.Birthday = AppUser_AppUserDTO.Birthday;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.PositionId = AppUser_AppUserDTO.PositionId;
            AppUser.Department = AppUser_AppUserDTO.Department;
            AppUser.OrganizationId = AppUser_AppUserDTO.OrganizationId;
            AppUser.ProvinceId = AppUser_AppUserDTO.ProvinceId;
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
                Email = AppUser_AppUserDTO.Organization.Email,
            };
            AppUser.Province = AppUser_AppUserDTO.Province == null ? null : new Province
            {
                Id = AppUser_AppUserDTO.Province.Id,
                Code = AppUser_AppUserDTO.Province.Code,
                Name = AppUser_AppUserDTO.Province.Name,
                Priority = AppUser_AppUserDTO.Province.Priority,
                StatusId = AppUser_AppUserDTO.Province.StatusId,
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
            AppUserFilter.PositionId = AppUser_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = AppUser_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = AppUser_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = AppUser_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = AppUser_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = AppUser_AppUserFilterDTO.ProvinceId;
            return AppUserFilter;
        }

        [Route(AppUserRoute.FilterListOrganization), HttpPost]
        public async Task<List<AppUser_OrganizationDTO>> FilterListOrganization([FromBody] AppUser_OrganizationFilterDTO AppUser_OrganizationFilterDTO)
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
            OrganizationFilter.Email = AppUser_OrganizationFilterDTO.Email;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<AppUser_OrganizationDTO> AppUser_OrganizationDTOs = Organizations
                .Select(x => new AppUser_OrganizationDTO(x)).ToList();
            return AppUser_OrganizationDTOs;
        }

        [Route(AppUserRoute.FilterListPosition), HttpPost]
        public async Task<List<AppUser_PositionDTO>> FilterListPosition([FromBody] AppUser_PositionFilterDTO AppUser_PositionFilterDTO)
        {
            PositionFilter PositionFilter = new PositionFilter();
            PositionFilter.Skip = 0;
            PositionFilter.Take = 99999;
            PositionFilter.OrderBy = PositionOrder.Id;
            PositionFilter.OrderType = OrderType.ASC;
            PositionFilter.Selects = PositionSelect.ALL;
            PositionFilter.Id = AppUser_PositionFilterDTO.Id;
            PositionFilter.Code = AppUser_PositionFilterDTO.Code;
            PositionFilter.Name = AppUser_PositionFilterDTO.Name;
            PositionFilter.StatusId = AppUser_PositionFilterDTO.StatusId;

            List<Position> Positions = await PositionService.List(PositionFilter);
            List<AppUser_PositionDTO> AppUser_PositionDTOs = Positions
                .Select(x => new AppUser_PositionDTO(x)).ToList();
            return AppUser_PositionDTOs;
        }

        [Route(AppUserRoute.FilterListStatus), HttpPost]
        public async Task<List<AppUser_StatusDTO>> FilterListStatus([FromBody] AppUser_StatusFilterDTO AppUser_StatusFilterDTO)
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
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = AppUser_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = AppUser_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = AppUser_OrganizationFilterDTO.Email;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<AppUser_OrganizationDTO> AppUser_OrganizationDTOs = Organizations
                .Select(x => new AppUser_OrganizationDTO(x)).ToList();
            return AppUser_OrganizationDTOs;
        }

        [Route(AppUserRoute.SingleListPosition), HttpPost]
        public async Task<List<AppUser_PositionDTO>> SingleListPosition([FromBody] AppUser_PositionFilterDTO AppUser_PositionFilterDTO)
        {
            PositionFilter PositionFilter = new PositionFilter();
            PositionFilter.Skip = 0;
            PositionFilter.Take = 99999;
            PositionFilter.OrderBy = PositionOrder.Id;
            PositionFilter.OrderType = OrderType.ASC;
            PositionFilter.Selects = PositionSelect.ALL;
            PositionFilter.Id = AppUser_PositionFilterDTO.Id;
            PositionFilter.Code = AppUser_PositionFilterDTO.Code;
            PositionFilter.Name = AppUser_PositionFilterDTO.Name;
            PositionFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<Position> Positions = await PositionService.List(PositionFilter);
            List<AppUser_PositionDTO> AppUser_PositionDTOs = Positions
                .Select(x => new AppUser_PositionDTO(x)).ToList();
            return AppUser_PositionDTOs;
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
            RoleFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

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

