using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IRoleRepository
    {
        Task<int> Count(RoleFilter RoleFilter);
        Task<List<Role>> List(RoleFilter RoleFilter);
        Task<Role> Get(long Id);
        Task<bool> Create(Role Role);
        Task<bool> Update(Role Role);
        Task<bool> Delete(Role Role);
        Task<bool> BulkMerge(List<Role> Roles);
        Task<bool> BulkDelete(List<Role> Roles);
    }
    public class RoleRepository : IRoleRepository
    {
        private DataContext DataContext;
        public RoleRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<RoleDAO> DynamicFilter(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<RoleDAO> OrFilter(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<RoleDAO> initQuery = query.Where(q => false);
            foreach (RoleFilter RoleFilter in filter.OrFilter)
            {
                IQueryable<RoleDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<RoleDAO> DynamicOrder(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case RoleOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case RoleOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case RoleOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case RoleOrder.Status:
                            query = query.OrderBy(q => q.Status);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case RoleOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case RoleOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case RoleOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case RoleOrder.Status:
                            query = query.OrderByDescending(q => q.Status);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Role>> DynamicSelect(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            List<Role> Roles = await query.Select(q => new Role()
            {
                Id = filter.Selects.Contains(RoleSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(RoleSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(RoleSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(RoleSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(RoleSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return Roles;
        }

        public async Task<int> Count(RoleFilter filter)
        {
            IQueryable<RoleDAO> Roles = DataContext.Role;
            Roles = DynamicFilter(Roles, filter);
            return await Roles.CountAsync();
        }

        public async Task<List<Role>> List(RoleFilter filter)
        {
            if (filter == null) return new List<Role>();
            IQueryable<RoleDAO> RoleDAOs = DataContext.Role;
            RoleDAOs = DynamicFilter(RoleDAOs, filter);
            RoleDAOs = DynamicOrder(RoleDAOs, filter);
            List<Role> Roles = await DynamicSelect(RoleDAOs, filter);
            return Roles;
        }

        public async Task<Role> Get(long Id)
        {
            Role Role = await DataContext.Role.Where(x => x.Id == Id).AsNoTracking().Select(x => new Role()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();
            if (Role == null)
                return null;
            Role.AppUserRoleMappings = await DataContext.AppUserRoleMapping
                .Where(x => x.RoleId == Role.Id)
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
                }).ToListAsync();
            Role.Permissions = await DataContext.Permission.Include(p => p.PermissionFieldMappings).Include(p => p.PermissionPageMappings)
                .Where(x => x.RoleId == Role.Id)
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
                        Fields = x.Menu.Fields.Select(f => new Field
                        {
                            Id = f.Id,
                            MenuId = f.MenuId,
                            Name = f.Name,
                            Type = f.Type,
                            IsDeleted = f.IsDeleted
                        }).ToList(),
                        Pages = x.Menu.Pages.Select(p => new Page
                        {
                            Id = p.Id,
                            MenuId = p.MenuId,
                            Name = p.Name,
                            Path = p.Path,
                            IsDeleted = p.IsDeleted
                        }).ToList()
                    },
                    PermissionFieldMappings = x.PermissionFieldMappings.Select(pf => new PermissionFieldMapping
                    {
                        PermissionId = pf.PermissionId,
                        FieldId = pf.FieldId,
                        Value = pf.Value
                    }).ToList(),
                    PermissionPageMappings = x.PermissionPageMappings.Select(pp => new PermissionPageMapping
                    {
                        PermissionId = pp.PermissionId,
                        PageId = pp.PageId,
                    }).ToList()
                }).ToListAsync();
            return Role;
        }
        public async Task<bool> Create(Role Role)
        {
            RoleDAO RoleDAO = new RoleDAO();
            RoleDAO.Id = Role.Id;
            RoleDAO.Code = Role.Code;
            RoleDAO.Name = Role.Name;
            RoleDAO.StatusId = Role.StatusId;
            DataContext.Role.Add(RoleDAO);
            await DataContext.SaveChangesAsync();
            Role.Id = RoleDAO.Id;
            await SaveReference(Role);
            return true;
        }

        public async Task<bool> Update(Role Role)
        {
            RoleDAO RoleDAO = DataContext.Role.Where(x => x.Id == Role.Id).AsNoTracking().FirstOrDefault();
            if (RoleDAO == null)
                return false;
            RoleDAO.Id = Role.Id;
            RoleDAO.Code = Role.Code;
            RoleDAO.Name = Role.Name;
            RoleDAO.StatusId = Role.StatusId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Role);
            return true;
        }

        public async Task<bool> Delete(Role Role)
        {
            await DataContext.Role.Where(x => x.Id == Role.Id).AsNoTracking().DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<Role> Roles)
        {
            List<RoleDAO> RoleDAOs = new List<RoleDAO>();
            foreach (Role Role in Roles)
            {
                RoleDAO RoleDAO = new RoleDAO();
                RoleDAO.Id = Role.Id;
                RoleDAO.Code = Role.Code;
                RoleDAO.Name = Role.Name;
                RoleDAO.StatusId = Role.StatusId;
                RoleDAOs.Add(RoleDAO);
            }
            await DataContext.BulkMergeAsync(RoleDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Role> Roles)
        {
            List<long> Ids = Roles.Select(x => x.Id).ToList();
            await DataContext.Role
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Role Role)
        {
            await DataContext.AppUserRoleMapping
                .Where(x => x.RoleId == Role.Id)
                .DeleteFromQueryAsync();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = new List<AppUserRoleMappingDAO>();
            if (Role.AppUserRoleMappings != null)
            {
                foreach (AppUserRoleMapping AppUserRoleMapping in Role.AppUserRoleMappings)
                {
                    AppUserRoleMappingDAO AppUserRoleMappingDAO = new AppUserRoleMappingDAO();
                    AppUserRoleMappingDAO.AppUserId = AppUserRoleMapping.AppUserId;
                    AppUserRoleMappingDAO.RoleId = AppUserRoleMapping.RoleId;
                    AppUserRoleMappingDAOs.Add(AppUserRoleMappingDAO);
                }
                await DataContext.AppUserRoleMapping.BulkMergeAsync(AppUserRoleMappingDAOs);
            }
            await DataContext.Permission
                .Where(x => x.RoleId == Role.Id)
                .DeleteFromQueryAsync();
            List<PermissionDAO> PermissionDAOs = new List<PermissionDAO>();
            if (Role.Permissions != null)
            {
                foreach (Permission Permission in Role.Permissions)
                {
                    PermissionDAO PermissionDAO = new PermissionDAO();
                    PermissionDAO.Id = Permission.Id;
                    PermissionDAO.Name = Permission.Name;
                    PermissionDAO.RoleId = Permission.RoleId;
                    PermissionDAO.MenuId = Permission.MenuId;
                    PermissionDAO.StatusId = Permission.StatusId;
                    if (Permission.PermissionFieldMappings != null)
                    {
                        foreach (var PermissionFieldMapping in Permission.PermissionFieldMappings)
                        {
                            PermissionFieldMappingDAO PermissionFieldMappingDAO = new PermissionFieldMappingDAO
                            {
                                PermissionId = PermissionFieldMapping.PermissionId,
                                FieldId = PermissionFieldMapping.FieldId,
                                Value = PermissionFieldMapping.Value
                            };
                            PermissionDAO.PermissionFieldMappings.Add(PermissionFieldMappingDAO);
                        }
                    }
                    if (Permission.PermissionPageMappings != null)
                    {
                        foreach (var PermissionPageMapping in Permission.PermissionPageMappings)
                        {
                            PermissionPageMappingDAO PermissionPageMappingDAO = new PermissionPageMappingDAO
                            {
                                PermissionId = PermissionPageMapping.PermissionId,
                                PageId = PermissionPageMapping.PageId,
                            };
                            PermissionDAO.PermissionPageMappings.Add(PermissionPageMappingDAO);
                        }
                    }
                    PermissionDAOs.Add(PermissionDAO);
                }
                await DataContext.Permission.BulkMergeAsync(PermissionDAOs);
            }
        }
    }
}
