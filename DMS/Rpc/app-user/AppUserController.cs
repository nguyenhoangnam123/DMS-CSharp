using Common;
using DMS.Entities;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MPosition;
using DMS.Services.MRole;
using DMS.Services.MSex;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.app_user
{
    public partial class AppUserController : RpcController
    {
        private IOrganizationService OrganizationService;
        private IPositionService PositionService;
        private ISexService SexService;
        private IStatusService StatusService;
        private IRoleService RoleService;
        private IAppUserService AppUserService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            IOrganizationService OrganizationService,
            IPositionService PositionService,
            ISexService SexService,
            IStatusService StatusService,
            IRoleService RoleService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.PositionService = PositionService;
            this.SexService = SexService;
            this.StatusService = StatusService;
            this.RoleService = RoleService;
            this.AppUserService = AppUserService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
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
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = await AppUserService.Get(AppUser_AppUserDTO.Id);
            return new AppUser_AppUserDTO(AppUser);
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
                Selects = OrganizationSelect.Code | OrganizationSelect.Name,
                OrderType = OrderType.ASC,
                OrderBy = OrganizationOrder.Id,
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
            AppUser.AppUserStoreMappings = AppUser_AppUserDTO.AppUserStoreMappings?
                .Select(x => new AppUserStoreMapping
                {
                    StoreId = x.StoreId,
                    Store = x.Store == null ? null : new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        StatusId = x.Store.StatusId,
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
    }
}

