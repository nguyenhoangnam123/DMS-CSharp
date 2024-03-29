using DMS.Common;
using DMS.Entities;
using DMS.Enums;
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
        Task<bool> Used(List<long> Ids);
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
                if (RoleFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, RoleFilter.Id);
                if (RoleFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, RoleFilter.Code);
                if (RoleFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, RoleFilter.Name);
                if (RoleFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, RoleFilter.StatusId);
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
                Status = filter.Selects.Contains(RoleSelect.Status) && q.Status == null ? null : new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                },
                Used = q.Used
            }).ToListAsync();
            return Roles;
        }

        public async Task<int> Count(RoleFilter filter)
        {
            IQueryable<RoleDAO> Roles = DataContext.Role.AsNoTracking();
            Roles = DynamicFilter(Roles, filter);
            return await Roles.CountAsync();
        }

        public async Task<List<Role>> List(RoleFilter filter)
        {
            if (filter == null) return new List<Role>();
            IQueryable<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking();
            RoleDAOs = DynamicFilter(RoleDAOs, filter);
            RoleDAOs = DynamicOrder(RoleDAOs, filter);
            List<Role> Roles = await DynamicSelect(RoleDAOs, filter);
            return Roles;
        }

        public async Task<Role> Get(long Id)
        {
            Role Role = await DataContext.Role.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Role()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    StatusId = x.StatusId,
                    Used = x.Used,
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
                .Where(x => x.RoleId == Role.Id && x.AppUser.StatusId == StatusEnum.ACTIVE.Id)
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId,
                    AppUser = new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                        Address = x.AppUser.Address,
                        Department = x.AppUser.Department,
                        PositionId = x.AppUser.PositionId,
                        RowId = x.AppUser.RowId,
                        SexId = x.AppUser.SexId,
                        StatusId = x.AppUser.StatusId,
                        OrganizationId = x.AppUser.OrganizationId,
                        Organization = x.AppUser.Organization == null ? null : new Organization
                        {
                            Id = x.AppUser.Organization.Id,
                            Code = x.AppUser.Organization.Code,
                            Name = x.AppUser.Organization.Name,
                        },
                    },
                }).ToListAsync();
            List<Menu> Menus = await DataContext.Menu.Where(m => m.Permissions.Any(p => p.RoleId == Role.Id))
                .Select(x => new Menu
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Path = x.Path,
                    IsDeleted = x.IsDeleted,
                    Fields = x.Fields.Select(f => new Field
                    {
                        Id = f.Id,
                        MenuId = f.MenuId,
                        Name = f.Name,
                        IsDeleted = f.IsDeleted
                    }).ToList(),
                    Actions = x.Actions.Select(p => new Entities.Action
                    {
                        Id = p.Id,
                        MenuId = p.MenuId,
                        Name = p.Name,
                        IsDeleted = p.IsDeleted
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
            RoleDAO.Used = false;
            DataContext.Role.Add(RoleDAO);
            await DataContext.SaveChangesAsync();
            Role.Id = RoleDAO.Id;
            await SaveReference(Role);
            return true;
        }

        public async Task<bool> Update(Role Role)
        {
            RoleDAO RoleDAO = DataContext.Role.Where(x => x.Id == Role.Id).FirstOrDefault();
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
            await DataContext.AppUserRoleMapping.Where(x => x.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.PermissionActionMapping.Where(x => x.Permission.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.PermissionContent.Where(x => x.Permission.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.Permission.Where(x => x.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.Role.Where(x => x.Id == Role.Id).DeleteFromQueryAsync();
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
            await DataContext.AppUserRoleMapping.Where(x => Ids.Contains(x.RoleId)).DeleteFromQueryAsync();
            await DataContext.PermissionActionMapping.Where(x => Ids.Contains(x.Permission.RoleId)).DeleteFromQueryAsync();
            await DataContext.PermissionContent.Where(x => Ids.Contains(x.Permission.RoleId)).DeleteFromQueryAsync();
            await DataContext.Permission.Where(x => Ids.Contains(x.RoleId)).DeleteFromQueryAsync();
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
                    AppUserRoleMappingDAO.RoleId = Role.Id;
                    AppUserRoleMappingDAOs.Add(AppUserRoleMappingDAO);
                }
                await DataContext.AppUserRoleMapping.BulkMergeAsync(AppUserRoleMappingDAOs);
            }
        }
        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Role.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new RoleDAO { Used = true });
            return true;
        }
    }
}
