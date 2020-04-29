using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DMS
{
    public class Setup
    {
        private readonly DataContext DataContext;
        public Setup(IConfiguration Configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
            DataContext = new DataContext(optionsBuilder.Options);
            DataContext.Database.Migrate();
            InitEnum();
            Init();
        }

        private void Init()
        {
            InitRoute();

            RoleDAO Admin = DataContext.Role
               .Where(r => r.Name == "ADMIN")
               .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new RoleDAO
                {
                    Name = "ADMIN",
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            AppUserDAO AppUser = DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (AppUser == null)
            {
                return;
            }

            AppUserRoleMappingDAO AppUserRoleMappingDAO = DataContext.AppUserRoleMapping
                .Where(ur => ur.RoleId == Admin.Id && ur.AppUserId == AppUser.Id)
                .FirstOrDefault();
            if (AppUserRoleMappingDAO == null)
            {
                AppUserRoleMappingDAO = new AppUserRoleMappingDAO
                {
                    AppUserId = AppUser.Id,
                    RoleId = Admin.Id,
                };
                DataContext.AppUserRoleMapping.Add(AppUserRoleMappingDAO);
                DataContext.SaveChanges();
            }

            List<MenuDAO> Menus = DataContext.Menu
                .Include(v => v.Pages)
                .Include(v => v.Fields)
                .ToList();
            List<PermissionDAO> permissions = DataContext.Permission
                .Include(p => p.PermissionFieldMappings)
                .Include(p => p.PermissionPageMappings)
                .ToList();
            foreach (MenuDAO Menu in Menus)
            {
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == Admin.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
                        Code = Admin + "_" + Menu.Name,
                        Name = Admin + "_" + Menu.Name,
                        MenuId = Menu.Id,
                        RoleId = Admin.Id,
                        PermissionFieldMappings = new List<PermissionFieldMappingDAO>(),
                        PermissionPageMappings = new List<PermissionPageMappingDAO>(),
                    };
                    permissions.Add(permission);
                }
                else
                {
                    if (permission.PermissionFieldMappings == null)
                        permission.PermissionFieldMappings = new List<PermissionFieldMappingDAO>();
                    if (permission.PermissionPageMappings == null)
                        permission.PermissionPageMappings = new List<PermissionPageMappingDAO>();
                }
                foreach (PageDAO page in Menu.Pages)
                {
                    PermissionPageMappingDAO PermissionPageMappingDAO = permission.PermissionPageMappings
                        .Where(ppm => ppm.PageId == page.Id).FirstOrDefault();
                    if (PermissionPageMappingDAO == null)
                    {
                        PermissionPageMappingDAO = new PermissionPageMappingDAO
                        {
                            PageId = page.Id
                        };
                        permission.PermissionPageMappings.Add(PermissionPageMappingDAO);
                    }
                }
                foreach (FieldDAO field in Menu.Fields)
                {
                    PermissionFieldMappingDAO permissionFieldMapping = permission.PermissionFieldMappings
                        .Where(pfm => pfm.FieldId == field.Id).FirstOrDefault();
                    if (permissionFieldMapping == null)
                    {
                        permissionFieldMapping = new PermissionFieldMappingDAO
                        {
                            FieldId = field.Id
                        };
                        permission.PermissionFieldMappings.Add(permissionFieldMapping);
                    }
                }
            }
            DataContext.Permission.BulkMerge(permissions);
            permissions.ForEach(p =>
            {
                foreach (var field in p.PermissionFieldMappings)
                {
                    field.PermissionId = p.Id;
                }
                foreach (var page in p.PermissionPageMappings)
                {
                    page.PermissionId = p.Id;
                }
            });
            List<PermissionFieldMappingDAO> permissionFieldMappings = permissions
                .SelectMany(p => p.PermissionFieldMappings).ToList();
            List<PermissionPageMappingDAO> permissionPageMappings = permissions
                .SelectMany(p => p.PermissionPageMappings).ToList();
            DataContext.PermissionFieldMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionFieldMapping.BulkMerge(permissionFieldMappings);
            DataContext.PermissionPageMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionPageMapping.BulkMerge(permissionPageMappings);
        }

        private void InitRoute()
        {
            List<Type> routeTypes = typeof(Setup).Assembly.GetTypes()
               .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass)
               .ToList();

            List<MenuDAO> Menus = DataContext.Menu.ToList();
            Menus.ForEach(m => m.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Name == type.Name).FirstOrDefault();
                if (Menu == null)
                {
                    Menu = new MenuDAO
                    {
                        Code = type.Name,
                        Name = type.Name,
                        IsDeleted = false,
                    };
                    Menus.Add(Menu);
                }
                else
                {
                    Menu.IsDeleted = false;
                }
            }
            DataContext.BulkMerge(Menus);
            Menus = DataContext.Menu.ToList();
            List<PageDAO> pages = DataContext.Page.OrderBy(p => p.Path).ToList();
            pages.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var values = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
                foreach (string value in values)
                {
                    PageDAO page = pages.Where(p => p.Path == value).FirstOrDefault();
                    if (page == null)
                    {
                        page = new PageDAO
                        {
                            Name = value,
                            Path = value,
                            IsDeleted = false,
                            MenuId = Menu.Id,
                        };
                        pages.Add(page);
                    }
                    else
                    {
                        page.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(pages);
            List<FieldDAO> fields = DataContext.Field.ToList();
            fields.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, FieldType>))
                .Select(x => (Dictionary<string, FieldType>)x.GetValue(x))
                .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    FieldDAO field = fields
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (field == null)
                    {
                        field = new FieldDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            Type = pair.Value.ToString(),
                            IsDeleted = false,
                        };
                        fields.Add(field);
                    }
                    else
                    {
                        field.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(fields);
            DataContext.PermissionPageMapping.Where(ppm => ppm.Page.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.PermissionFieldMapping.Where(pd => pd.Field.IsDeleted).DeleteFromQuery();
            DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQuery();
        }

        private void InitEnum()
        {
            InitStatusEnum();
            InitResellerStatusEnum();
            InitStoreStatusEnum();
            InitWorkflowStateEnum();
            InitWorkflowTypeEnum();
            InitRequestStateEnum();
            DataContext.SaveChanges();
        }

        private void InitStatusEnum()
        {
            List<StatusDAO> statuses = DataContext.Status.ToList();
            if (!statuses.Any(pt => pt.Id == StatusEnum.ACTIVE.Id))
            {
                DataContext.Status.Add(new StatusDAO
                {
                    Id = StatusEnum.ACTIVE.Id,
                    Code = StatusEnum.ACTIVE.Code,
                    Name = StatusEnum.ACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == StatusEnum.INACTIVE.Id))
            {
                DataContext.Status.Add(new StatusDAO
                {
                    Id = StatusEnum.INACTIVE.Id,
                    Code = StatusEnum.INACTIVE.Code,
                    Name = StatusEnum.INACTIVE.Name,
                });
            }
        }

        private void InitResellerStatusEnum()
        {
            List<ResellerStatusDAO> resellerStatuses = DataContext.ResellerStatus.ToList();
            if (!resellerStatuses.Any(pt => pt.Id == ResellerStatusEnum.NEW.Id))
            {
                DataContext.ResellerStatus.Add(new ResellerStatusDAO
                {
                    Id = ResellerStatusEnum.NEW.Id,
                    Code = ResellerStatusEnum.NEW.Code,
                    Name = ResellerStatusEnum.NEW.Name,
                });
            }

            if (!resellerStatuses.Any(pt => pt.Id == ResellerStatusEnum.PENDING.Id))
            {
                DataContext.ResellerStatus.Add(new ResellerStatusDAO
                {
                    Id = ResellerStatusEnum.PENDING.Id,
                    Code = ResellerStatusEnum.PENDING.Code,
                    Name = ResellerStatusEnum.PENDING.Name,
                });
            }

            if (!resellerStatuses.Any(pt => pt.Id == ResellerStatusEnum.APPROVED.Id))
            {
                DataContext.ResellerStatus.Add(new ResellerStatusDAO
                {
                    Id = ResellerStatusEnum.APPROVED.Id,
                    Code = ResellerStatusEnum.APPROVED.Code,
                    Name = ResellerStatusEnum.APPROVED.Name,
                });
            }

            if (!resellerStatuses.Any(pt => pt.Id == ResellerStatusEnum.REJECTED.Id))
            {
                DataContext.ResellerStatus.Add(new ResellerStatusDAO
                {
                    Id = ResellerStatusEnum.REJECTED.Id,
                    Code = ResellerStatusEnum.REJECTED.Code,
                    Name = ResellerStatusEnum.REJECTED.Name,
                });
            }
        }

        private void InitStoreStatusEnum()
        {
            List<StoreStatusDAO> statuses = DataContext.StoreStatus.ToList();
            if (!statuses.Any(pt => pt.Id == StoreStatusEnum.NEW.Id))
            {
                DataContext.StoreStatus.Add(new StoreStatusDAO
                {
                    Id = StoreStatusEnum.NEW.Id,
                    Code = StoreStatusEnum.NEW.Code,
                    Name = StoreStatusEnum.NEW.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == StoreStatusEnum.PENDING.Id))
            {
                DataContext.StoreStatus.Add(new StoreStatusDAO
                {
                    Id = StoreStatusEnum.PENDING.Id,
                    Code = StoreStatusEnum.PENDING.Code,
                    Name = StoreStatusEnum.PENDING.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == StoreStatusEnum.APPROVED.Id))
            {
                DataContext.StoreStatus.Add(new StoreStatusDAO
                {
                    Id = StoreStatusEnum.APPROVED.Id,
                    Code = StoreStatusEnum.APPROVED.Code,
                    Name = StoreStatusEnum.APPROVED.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == StoreStatusEnum.REJECTED.Id))
            {
                DataContext.StoreStatus.Add(new StoreStatusDAO
                {
                    Id = StoreStatusEnum.REJECTED.Id,
                    Code = StoreStatusEnum.REJECTED.Code,
                    Name = StoreStatusEnum.REJECTED.Name,
                });
            }
        }

        private void InitWorkflowStateEnum()
        {
            List<WorkflowStateDAO> list = DataContext.WorkflowState.ToList();
            if (!list.Any(pt => pt.Id == WorkflowStateEnum.APPROVED.Id))
            {
                DataContext.WorkflowState.Add(new WorkflowStateDAO
                {
                    Id = WorkflowStateEnum.APPROVED.Id,
                    Code = WorkflowStateEnum.APPROVED.Code,
                    Name = WorkflowStateEnum.APPROVED.Name,
                });
            }

            if (!list.Any(pt => pt.Id == WorkflowStateEnum.NEW.Id))
            {
                DataContext.WorkflowState.Add(new WorkflowStateDAO
                {
                    Id = WorkflowStateEnum.NEW.Id,
                    Code = WorkflowStateEnum.NEW.Code,
                    Name = WorkflowStateEnum.NEW.Name,
                });
            }

            if (!list.Any(pt => pt.Id == WorkflowStateEnum.PENDING.Id))
            {
                DataContext.WorkflowState.Add(new WorkflowStateDAO
                {
                    Id = WorkflowStateEnum.PENDING.Id,
                    Code = WorkflowStateEnum.PENDING.Code,
                    Name = WorkflowStateEnum.PENDING.Name,
                });
            }

            if (!list.Any(pt => pt.Id == WorkflowStateEnum.REJECTED.Id))
            {
                DataContext.WorkflowState.Add(new WorkflowStateDAO
                {
                    Id = WorkflowStateEnum.REJECTED.Id,
                    Code = WorkflowStateEnum.REJECTED.Code,
                    Name = WorkflowStateEnum.REJECTED.Name,
                });
            }
        }

        private void InitWorkflowTypeEnum()
        {
            List<WorkflowTypeDAO> list = DataContext.WorkflowType.ToList();
            if (!list.Any(pt => pt.Id == WorkflowTypeEnum.ORDER.Id))
            {
                DataContext.WorkflowType.Add(new WorkflowTypeDAO
                {
                    Id = WorkflowTypeEnum.ORDER.Id,
                    Code = WorkflowTypeEnum.ORDER.Code,
                    Name = WorkflowTypeEnum.ORDER.Name,
                });
            }

            if (!list.Any(pt => pt.Id == WorkflowTypeEnum.PRODUCT.Id))
            {
                DataContext.WorkflowType.Add(new WorkflowTypeDAO
                {
                    Id = WorkflowTypeEnum.PRODUCT.Id,
                    Code = WorkflowTypeEnum.PRODUCT.Code,
                    Name = WorkflowTypeEnum.PRODUCT.Name,
                });
            }

            if (!list.Any(pt => pt.Id == WorkflowTypeEnum.ROUTER.Id))
            {
                DataContext.WorkflowType.Add(new WorkflowTypeDAO
                {
                    Id = WorkflowTypeEnum.ROUTER.Id,
                    Code = WorkflowTypeEnum.ROUTER.Code,
                    Name = WorkflowTypeEnum.ROUTER.Name,
                });
            }

            if (!list.Any(pt => pt.Id == WorkflowTypeEnum.STORE.Id))
            {
                DataContext.WorkflowType.Add(new WorkflowTypeDAO
                {
                    Id = WorkflowTypeEnum.STORE.Id,
                    Code = WorkflowTypeEnum.STORE.Code,
                    Name = WorkflowTypeEnum.STORE.Name,
                });
            }
        }

        private void InitRequestStateEnum()
        {
            List<RequestStateDAO> list = DataContext.RequestState.ToList();
            if (!list.Any(pt => pt.Id == RequestStateEnum.APPROVED.Id))
            {
                DataContext.RequestState.Add(new RequestStateDAO
                {
                    Id = RequestStateEnum.APPROVED.Id,
                    Code = RequestStateEnum.APPROVED.Code,
                    Name = RequestStateEnum.APPROVED.Name,
                });
            }

            if (!list.Any(pt => pt.Id == RequestStateEnum.NEW.Id))
            {
                DataContext.RequestState.Add(new RequestStateDAO
                {
                    Id = RequestStateEnum.NEW.Id,
                    Code = RequestStateEnum.NEW.Code,
                    Name = RequestStateEnum.NEW.Name,
                });
            }

            if (!list.Any(pt => pt.Id == RequestStateEnum.APPROVING.Id))
            {
                DataContext.RequestState.Add(new RequestStateDAO
                {
                    Id = RequestStateEnum.APPROVING.Id,
                    Code = RequestStateEnum.APPROVING.Code,
                    Name = RequestStateEnum.APPROVING.Name,
                });
            }

            if (!list.Any(pt => pt.Id == RequestStateEnum.REJECTED.Id))
            {
                DataContext.RequestState.Add(new RequestStateDAO
                {
                    Id = RequestStateEnum.REJECTED.Id,
                    Code = RequestStateEnum.REJECTED.Code,
                    Name = RequestStateEnum.REJECTED.Name,
                });
            }
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement()
        {
        }
    }
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private ICurrentContext CurrentContext;
        private DataContext DataContext;
        public PermissionHandler(ICurrentContext CurrentContext, IConfiguration Configuration)
        {
            this.CurrentContext = CurrentContext;
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
            DataContext = new DataContext(optionsBuilder.Options);
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var types = context.User.Claims.Select(c => c.Type).ToList();
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return;
            }
            long UserId = long.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
            string UserName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var mvcContext = context.Resource as AuthorizationFilterContext;
            var HttpContext = mvcContext.HttpContext;
            string url = HttpContext.Request.Path.Value;
            string TimeZone = HttpContext.Request.Headers["X-TimeZone"];
            string Language = HttpContext.Request.Headers["X-Language"];
            CurrentContext.UserId = UserId;
            CurrentContext.TimeZone = int.TryParse(TimeZone, out int t) ? t : 0;
            CurrentContext.Language = Language ?? "vi";

            List<long> permissionIds = await
                (from p in DataContext.Permission
                 join ru in DataContext.AppUserRoleMapping on p.RoleId equals ru.RoleId
                 join ppm in DataContext.PermissionPageMapping on p.Id equals ppm.PermissionId
                 join page in DataContext.Page on ppm.PageId equals page.Id
                 where page.Path == url && ru.AppUserId == UserId
                 select p.Id).Distinct().ToListAsync();

            List<PermissionDAO> PermissionDAOs = await DataContext.Permission.AsNoTracking()
                .Include(p => p.PermissionFieldMappings).ThenInclude(pf => pf.Field)
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();
            CurrentContext.RoleIds = PermissionDAOs.Select(p => p.RoleId).Distinct().ToList();
            CurrentContext.Filters = new Dictionary<long, List<FilterPermissionDefinition>>();
            foreach (PermissionDAO PermissionDAO in PermissionDAOs)
            {
                List<FilterPermissionDefinition> FilterPermissionDefinitions = new List<FilterPermissionDefinition>();
                CurrentContext.Filters.Add(PermissionDAO.Id, FilterPermissionDefinitions);
                foreach(PermissionFieldMappingDAO PermissionFieldMappingDAO in PermissionDAO.PermissionFieldMappings)
                {
                    FilterPermissionDefinition FilterPermissionDefinition = new FilterPermissionDefinition(PermissionFieldMappingDAO.Field.Name, PermissionFieldMappingDAO.Field.Type);
                    FilterPermissionDefinition.Value = PermissionFieldMappingDAO.Value;
                    FilterPermissionDefinitions.Add(FilterPermissionDefinition);
                }
                
            }    
            context.Succeed(requirement);
        }
    }
}
