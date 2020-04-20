using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MMenu;
using DMS.Services.MPermission;
using DMS.Services.MRole;
using DMS.Services.MStatus;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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
        public const string AssignAppUser = Default + "/assign-app-user";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";


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
        private IPermissionService PermissionService;
        private IStatusService StatusService;

        public RoleController(
            IAppUserService AppUserService,
            IMenuService MenuService,
            IRoleService RoleService,
            IPermissionService PermissionService,
            IStatusService StatusService
        )
        {
            this.AppUserService = AppUserService;
            this.MenuService = MenuService;
            this.RoleService = RoleService;
            this.PermissionService = PermissionService;
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

        [Route(RoleRoute.AssignAppUser), HttpPost]
        public async Task<ActionResult<Role_RoleDTO>> AssignAppUser([FromBody] Role_RoleDTO Role_RoleDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Role_RoleDTO.Id))
                return Forbid();

            Role Role = ConvertDTOToEntity(Role_RoleDTO);
            Role = await RoleService.AssignAppUser(Role);
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

            List<Menu> MenusInDB = await MenuService.List(new MenuFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = MenuSelect.ALL
            });

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Role> Roles = new List<Role>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                #region page sheet
                ExcelWorksheet pageWorkSheet = excelPackage.Workbook.Worksheets[0];
                if (pageWorkSheet == null)
                    return null;
                int StartColumn = 1;
                int StartRow = 1;

                int RoleCodeColumn = 0 + StartColumn;
                int RoleNameColumn = 1 + StartColumn;
                int PermissionCodeColumn = 2 + StartColumn;
                int PermissionNameColumn = 3 + StartColumn;
                int MenuCodeColumn = 4 + StartColumn;
                int PagePathColumn = 5 + StartColumn;

                for (int i = StartRow; i <= pageWorkSheet.Dimension.End.Row; i++)
                {
                    string RoleCodeValue = pageWorkSheet.Cells[i + StartRow, RoleCodeColumn].Value?.ToString();
                    string RoleNameValue = pageWorkSheet.Cells[i + StartRow, RoleNameColumn].Value?.ToString();
                    string PermissionCodeValue = pageWorkSheet.Cells[i + StartRow, PermissionCodeColumn].Value?.ToString();
                    string PermissionNameValue = pageWorkSheet.Cells[i + StartRow, PermissionNameColumn].Value?.ToString();
                    string MenuCodeValue = pageWorkSheet.Cells[i + StartRow, MenuCodeColumn].Value?.ToString();
                    string PagePathValue = pageWorkSheet.Cells[i + StartRow, PagePathColumn].Value?.ToString();

                    if (string.IsNullOrEmpty(RoleCodeValue))
                        continue;

                    var Role = Roles.Where(r => r.Code == RoleCodeValue).FirstOrDefault();
                    if (Role == null)
                    {
                        Role = new Role
                        {
                            Code = RoleCodeValue,
                            Name = RoleNameValue,
                            Permissions = new List<Permission>()
                        };

                        Permission Permission = new Permission
                        {
                            Code = PermissionCodeValue,
                            Name = PermissionNameValue,
                            PermissionPageMappings = new List<PermissionPageMapping>()
                        };

                        var Menu = MenusInDB.Where(m => m.Code == MenuCodeValue).FirstOrDefault();
                        if (Menu == null) continue;

                        var Page = Menu.Pages.Where(p => p.Path == PagePathValue).FirstOrDefault();
                        if (Page == null) continue;
                        Permission.PermissionPageMappings.Add(new PermissionPageMapping { PageId = Page.Id, Page = Page });
                        Role.Permissions.Add(Permission);
                        Roles.Add(Role);
                    }
                    else
                    {
                        Permission Permission = Role.Permissions.Where(p => p.Code == PermissionCodeValue).FirstOrDefault();
                        if (Permission == null)
                        {
                            Permission = new Permission
                            {
                                Code = PermissionCodeValue,
                                Name = PermissionNameValue,
                                PermissionPageMappings = new List<PermissionPageMapping>()
                            };

                            var Menu = MenusInDB.Where(m => m.Code == MenuCodeValue).FirstOrDefault();
                            if (Menu == null) continue;

                            var Page = Menu.Pages.Where(p => p.Path == PagePathValue).FirstOrDefault();
                            if (Page == null) continue;
                            Permission.PermissionPageMappings.Add(new PermissionPageMapping { PageId = Page.Id, Page = Page });
                            Role.Permissions.Add(Permission);
                        }
                        else
                        {
                            if (!Permission.PermissionPageMappings.Any(p => p.Page.Path == PagePathValue))
                            {
                                var Menu = MenusInDB.Where(m => m.Code == MenuCodeValue).FirstOrDefault();
                                if (Menu == null) continue;

                                var Page = Menu.Pages.Where(p => p.Path == PagePathValue).FirstOrDefault();
                                if (Page == null) continue;
                                Permission.PermissionPageMappings.Add(new PermissionPageMapping { PageId = Page.Id, Page = Page });
                            }
                        }
                    }
                }
                #endregion

                #region field sheet
                ExcelWorksheet fieldWorkSheet = excelPackage.Workbook.Worksheets[1];
                if (fieldWorkSheet == null)
                    return null;
                StartColumn = 1;
                StartRow = 1;

                RoleCodeColumn = 0 + StartColumn;
                RoleNameColumn = 1 + StartColumn;
                PermissionCodeColumn = 2 + StartColumn;
                PermissionNameColumn = 3 + StartColumn;
                MenuCodeColumn = 4 + StartColumn;
                int FieldNameColumn = 5 + StartColumn;
                int FieldValueColumn = 5 + StartColumn;

                for (int i = StartRow; i <= fieldWorkSheet.Dimension.End.Row; i++)
                {
                    string RoleCodeValue = fieldWorkSheet.Cells[i + StartRow, RoleCodeColumn].Value?.ToString();
                    string RoleNameValue = fieldWorkSheet.Cells[i + StartRow, RoleNameColumn].Value?.ToString();
                    string PermissionCodeValue = fieldWorkSheet.Cells[i + StartRow, PermissionCodeColumn].Value?.ToString();
                    string PermissionNameValue = fieldWorkSheet.Cells[i + StartRow, PermissionNameColumn].Value?.ToString();
                    string MenuCodeValue = fieldWorkSheet.Cells[i + StartRow, MenuCodeColumn].Value?.ToString();
                    string FieldNameValue = fieldWorkSheet.Cells[i + StartRow, FieldNameColumn].Value?.ToString();
                    string FieldValueValue = fieldWorkSheet.Cells[i + StartRow, FieldValueColumn].Value?.ToString();

                    if (string.IsNullOrEmpty(RoleCodeValue))
                        continue;

                    var Role = Roles.Where(r => r.Code == RoleCodeValue).FirstOrDefault();
                    if (Role == null)
                    {
                        Role = new Role
                        {
                            Code = RoleCodeValue,
                            Name = RoleNameValue,
                            Permissions = new List<Permission>()
                        };

                        Permission Permission = new Permission
                        {
                            Code = PermissionCodeValue,
                            Name = PermissionNameValue,
                            PermissionFieldMappings = new List<PermissionFieldMapping>()
                        };

                        var Menu = MenusInDB.Where(m => m.Code == MenuCodeValue).FirstOrDefault();
                        if (Menu == null) continue;

                        var Field = Menu.Fields.Where(p => p.Name == FieldNameValue).FirstOrDefault();
                        if (Field == null) continue;
                        Permission.PermissionFieldMappings.Add(new PermissionFieldMapping { FieldId = Field.Id, Field = Field, Value = FieldValueValue });
                        Role.Permissions.Add(Permission);
                        Roles.Add(Role);
                    }
                    else
                    {
                        Permission Permission = Role.Permissions.Where(p => p.Code == PermissionCodeValue).FirstOrDefault();
                        if (Permission == null)
                        {
                            Permission = new Permission
                            {
                                Code = PermissionCodeValue,
                                Name = PermissionNameValue,
                                PermissionFieldMappings = new List<PermissionFieldMapping>()
                            };

                            var Menu = MenusInDB.Where(m => m.Code == MenuCodeValue).FirstOrDefault();
                            if (Menu == null) continue;

                            var Field = Menu.Fields.Where(p => p.Name == FieldNameValue).FirstOrDefault();
                            if (Field == null) continue;
                            Permission.PermissionFieldMappings.Add(new PermissionFieldMapping { FieldId = Field.Id, Field = Field, Value = FieldValueValue });
                            Role.Permissions.Add(Permission);
                        }
                        else
                        {
                            if (!Permission.PermissionFieldMappings.Any(p => p.Field.Name == FieldNameValue))
                            {
                                var Menu = MenusInDB.Where(m => m.Code == MenuCodeValue).FirstOrDefault();
                                if (Menu == null) continue;

                                var Field = Menu.Fields.Where(p => p.Name == FieldNameValue).FirstOrDefault();
                                if (Field == null) continue;
                                Permission.PermissionFieldMappings.Add(new PermissionFieldMapping { FieldId = Field.Id, Field = Field, Value = FieldValueValue });
                            }
                        }
                    }
                }
                #endregion
            }
            Roles = await RoleService.Import(Roles);
            List<Role_RoleDTO> Role_RoleDTOs = Roles
                .Select(c => new Role_RoleDTO(c)).ToList();
            return Role_RoleDTOs;
        }

        [Route(RoleRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Role_RoleFilterDTO Role_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = ConvertFilterDTOToFilterEntity(Role_RoleFilterDTO);
            RoleFilter.Skip = 0;
            RoleFilter.Take = int.MaxValue;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter = RoleService.ToFilter(RoleFilter);

            List<Role> Roles = await RoleService.List(RoleFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(MemoryStream))
            {
                var FieldHeader = new List<string[]>()
                {
                    new string[] { "Mã vai trò", "Tên vai trò", "Mã quyền","Tên quyền", "Menu", "Field", "Giá trị"}
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Roles.Count; i++)
                {
                    Role Role = Roles[i];
                    if (Role.Permissions != null)
                        foreach (var rolePermission in Role.Permissions)
                        {
                            foreach (var permissionFieldMapping in rolePermission.PermissionFieldMappings)
                            {
                                data.Add(new object[] {
                                Role.Code,
                                Role.Name,
                                rolePermission.Code,
                                rolePermission.Name,
                                rolePermission.Menu.Code,
                                permissionFieldMapping.Field.Name,
                                permissionFieldMapping.Value,
                                });
                            }

                        }
                    excel.GenerateWorksheet("Role_Field", FieldHeader, data);
                }
                data.Clear();
                var PageHeader = new List<string[]>()
                {
                    new string[] { "Mã vai trò", "Tên vai trò", "Mã quyền", "Tên quyền", "Menu", "Page Path" }
                };
                for (int i = 0; i < Roles.Count; i++)
                {
                    Role Role = Roles[i];
                    if (Role.Permissions != null)
                        foreach (var rolePermission in Role.Permissions)
                        {
                            foreach (var permissionPageMapping in rolePermission.PermissionPageMappings)
                            {
                                data.Add(new object[] {
                                Role.Code,
                                Role.Name,
                                rolePermission.Code,
                                rolePermission.Name,
                                rolePermission.Menu.Code,
                                permissionPageMapping.Page.Path
                                });
                            }

                        }
                }
                excel.GenerateWorksheet("Role_Page", FieldHeader, data);
                excel.Save();
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Role.xlsx");
        }

        [Route(RoleRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Role_Export.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                var nameexcel = "Export vai trò" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Sonhx5";
                xlPackage.Workbook.Properties.Subject = string.Format("{0}", "RD-DMS");
                xlPackage.Workbook.Properties.Category = "RD-DMS";
                xlPackage.Workbook.Properties.Company = "FPT-FIS-ERP-ESC";
                xlPackage.SaveAs(MemoryStream);
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Role" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
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
            Role.Permissions = Role_RoleDTO.Permissions?
                .Select(x => new Permission
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    RoleId = x.RoleId,
                    MenuId = x.MenuId,
                    StatusId = x.StatusId,
                    Menu = new Menu
                    {
                        Id = x.Menu.Id,
                        Code = x.Menu.Code,
                        Name = x.Menu.Name,
                        Path = x.Menu.Path,
                        IsDeleted = x.Menu.IsDeleted,
                        Fields = x.Menu.Fields?.Select(f => new Field
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Type = f.Type,
                        }).ToList(),
                        Pages = x.Menu.Pages?.Select(p => new Page
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Path = p.Path,
                        }).ToList(),
                    },
                    PermissionFieldMappings = x.PermissionFieldMappings?.Select(pf => new PermissionFieldMapping
                    {
                        FieldId = pf.FieldId
                    }).ToList(),
                    PermissionPageMappings = x.PermissionPageMappings?.Select(pp => new PermissionPageMapping
                    {
                        PageId = pp.PageId,
                    }).ToList(),
                }).ToList();
            Role.AppUserRoleMappings = Role_RoleDTO.AppUserRoleMappings?
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId
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
            MenuFilter.Code = Role_MenuFilterDTO.Code;
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
            AppUserFilter.Id = Role_AppUserFilterDTO.Id;
            AppUserFilter.Username = Role_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Role_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Role_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Role_AppUserFilterDTO.Phone;
            AppUserFilter.OrganizationId = Role_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

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
            AppUserFilter.Id = Role_AppUserFilterDTO.Id;
            AppUserFilter.Username = Role_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Role_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Role_AppUserFilterDTO.Email;
            AppUserFilter.OrganizationId = Role_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Phone = Role_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Role_AppUserDTO> Role_AppUserDTOs = AppUsers
                .Select(x => new Role_AppUserDTO(x)).ToList();
            return Role_AppUserDTOs;
        }
    }
}

