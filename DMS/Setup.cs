using DMS.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Helpers;
using Common;
using System.Reflection;
using DMS.Models;

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
            foreach(MenuDAO Menu in Menus)
            {
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == Admin.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
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
                foreach(FieldDAO field in Menu.Fields)
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
                foreach(var field in p.PermissionFieldMappings)
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
            string sql = DataContext.PermissionPageMapping.Where(ppm => ppm.Page.IsDeleted).ToSql();
            DataContext.PermissionPageMapping.Where(ppm => ppm.Page.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted ||  p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.PermissionFieldMapping.Where(pd => pd.Field.IsDeleted).DeleteFromQuery();
            DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQuery();
        }

        private void InitEnum()
        {
            InitUserStatusEnum();
            DataContext.SaveChanges();
        }

        private void InitUserStatusEnum()
        {
            List<UserStatusDAO> statuses = DataContext.UserStatus.ToList();
            if (!statuses.Any(pt => pt.Id == UserStatusEnum.ACTIVE.Id))
            {
                DataContext.UserStatus.Add(new UserStatusDAO
                {
                    Id = UserStatusEnum.ACTIVE.Id,
                    Code = UserStatusEnum.ACTIVE.Code,
                    Name = UserStatusEnum.ACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == UserStatusEnum.INACTIVE.Id))
            {
                DataContext.UserStatus.Add(new UserStatusDAO
                {
                    Id = UserStatusEnum.INACTIVE.Id,
                    Code = UserStatusEnum.INACTIVE.Code,
                    Name = UserStatusEnum.INACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == UserStatusEnum.LOCKED.Id))
            {
                DataContext.UserStatus.Add(new UserStatusDAO
                {
                    Id = UserStatusEnum.LOCKED.Id,
                    Code = UserStatusEnum.LOCKED.Code,
                    Name = UserStatusEnum.LOCKED.Name,
                });
            }
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}
