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
    public interface IPermissionRepository
    {
        Task<int> Count(PermissionFilter PermissionFilter);
        Task<List<Permission>> List(PermissionFilter PermissionFilter);
        Task<Permission> Get(long Id);
        Task<bool> Create(Permission Permission);
        Task<bool> Update(Permission Permission);
        Task<bool> Delete(Permission Permission);
        Task<bool> BulkMerge(List<Permission> Permissions);
        Task<bool> BulkDelete(List<Permission> Permissions);
    }
    public class PermissionRepository : IPermissionRepository
    {
        private DataContext DataContext;
        public PermissionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PermissionDAO> DynamicFilter(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.RoleId != null)
                query = query.Where(q => q.RoleId, filter.RoleId);
            if (filter.MenuId != null)
                query = query.Where(q => q.MenuId, filter.MenuId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PermissionDAO> OrFilter(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PermissionDAO> initQuery = query.Where(q => false);
            foreach (PermissionFilter PermissionFilter in filter.OrFilter)
            {
                IQueryable<PermissionDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.RoleId != null)
                    queryable = queryable.Where(q => q.RoleId, filter.RoleId);
                if (filter.MenuId != null)
                    queryable = queryable.Where(q => q.MenuId, filter.MenuId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<PermissionDAO> DynamicOrder(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PermissionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PermissionOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PermissionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PermissionOrder.Role:
                            query = query.OrderBy(q => q.RoleId);
                            break;
                        case PermissionOrder.Menu:
                            query = query.OrderBy(q => q.MenuId);
                            break;
                        case PermissionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PermissionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PermissionOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PermissionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PermissionOrder.Role:
                            query = query.OrderByDescending(q => q.RoleId);
                            break;
                        case PermissionOrder.Menu:
                            query = query.OrderByDescending(q => q.MenuId);
                            break;
                        case PermissionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Permission>> DynamicSelect(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            List<Permission> Permissions = await query.Select(q => new Permission()
            {
                Id = filter.Selects.Contains(PermissionSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PermissionSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PermissionSelect.Name) ? q.Name : default(string),
                RoleId = filter.Selects.Contains(PermissionSelect.Role) ? q.RoleId : default(long),
                MenuId = filter.Selects.Contains(PermissionSelect.Menu) ? q.MenuId : default(long),
                StatusId = filter.Selects.Contains(PermissionSelect.Status) ? q.StatusId : default(long),
                Menu = filter.Selects.Contains(PermissionSelect.Menu) && q.Menu != null ? new Menu
                {
                    Id = q.Menu.Id,
                    Code = q.Menu.Code,
                    Name = q.Menu.Name,
                    Path = q.Menu.Path,
                    IsDeleted = q.Menu.IsDeleted,
                } : null,
                Role = filter.Selects.Contains(PermissionSelect.Role) && q.Role != null ? new Role
                {
                    Id = q.Role.Id,
                    Code = q.Role.Code,
                    Name = q.Role.Name,
                    StatusId = q.StatusId
                } : null,
            }).ToListAsync();
            return Permissions;
        }

        public async Task<int> Count(PermissionFilter filter)
        {
            IQueryable<PermissionDAO> Permissions = DataContext.Permission;
            Permissions = DynamicFilter(Permissions, filter);
            return await Permissions.CountAsync();
        }

        public async Task<List<Permission>> List(PermissionFilter filter)
        {
            if (filter == null) return new List<Permission>();
            IQueryable<PermissionDAO> PermissionDAOs = DataContext.Permission;
            PermissionDAOs = DynamicFilter(PermissionDAOs, filter);
            PermissionDAOs = DynamicOrder(PermissionDAOs, filter);
            List<Permission> Permissions = await DynamicSelect(PermissionDAOs, filter);
            return Permissions;
        }

        public async Task<Permission> Get(long Id)
        {
            Permission Permission = await DataContext.Permission.Where(x => x.Id == Id).AsNoTracking().Select(x => new Permission()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                RoleId = x.RoleId,
                MenuId = x.MenuId,
                Menu = x.Menu == null ? null : new Menu
                {
                    Id = x.Menu.Id,
                    Code = x.Menu.Code,
                    Name = x.Menu.Name,
                    Path = x.Menu.Path,
                    IsDeleted = x.Menu.IsDeleted,
                },
                Role = x.Role == null ? null : new Role
                {
                    Id = x.Role.Id,
                    Code = x.Role.Code,
                    Name = x.Role.Name,
                    StatusId = x.Role.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (Permission == null)
                return null;
            Permission.PermissionFieldMappings = await DataContext.PermissionFieldMapping
                .Where(x => x.PermissionId == Permission.Id)
                .Select(x => new PermissionFieldMapping
                {
                    PermissionId = x.PermissionId,
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
                }).ToListAsync();
            Permission.PermissionPageMappings = await DataContext.PermissionPageMapping
                .Where(x => x.PermissionId == Permission.Id)
                .Select(x => new PermissionPageMapping
                {
                    PermissionId = x.PermissionId,
                    PageId = x.PageId,
                    Page = new Page
                    {
                        Id = x.Page.Id,
                        Name = x.Page.Name,
                        Path = x.Page.Path,
                        MenuId = x.Page.MenuId,
                        IsDeleted = x.Page.IsDeleted,
                    },
                }).ToListAsync();

            return Permission;
        }
        public async Task<bool> Create(Permission Permission)
        {
            PermissionDAO PermissionDAO = new PermissionDAO();
            PermissionDAO.Id = Permission.Id;
            PermissionDAO.Code = Permission.Code;
            PermissionDAO.Name = Permission.Name;
            PermissionDAO.RoleId = Permission.RoleId;
            PermissionDAO.MenuId = Permission.MenuId;
            PermissionDAO.StatusId = Permission.StatusId;
            DataContext.Permission.Add(PermissionDAO);
            await DataContext.SaveChangesAsync();
            Permission.Id = PermissionDAO.Id;
            await SaveReference(Permission);
            return true;
        }

        public async Task<bool> Update(Permission Permission)
        {
            PermissionDAO PermissionDAO = DataContext.Permission.Where(x => x.Id == Permission.Id).AsNoTracking().FirstOrDefault();
            if (PermissionDAO == null)
                return false;
            PermissionDAO.Id = Permission.Id;
            PermissionDAO.Code = Permission.Code;
            PermissionDAO.Name = Permission.Name;
            PermissionDAO.RoleId = Permission.RoleId;
            PermissionDAO.MenuId = Permission.MenuId;
            PermissionDAO.StatusId = Permission.StatusId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Permission);
            return true;
        }

        public async Task<bool> Delete(Permission Permission)
        {
            await DataContext.Permission.Where(x => x.Id == Permission.Id).AsNoTracking().DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<Permission> Permissions)
        {
            List<PermissionDAO> PermissionDAOs = new List<PermissionDAO>();
            foreach (Permission Permission in Permissions)
            {
                PermissionDAO PermissionDAO = new PermissionDAO();
                PermissionDAO.Id = Permission.Id;
                PermissionDAO.Code = Permission.Code;
                PermissionDAO.Name = Permission.Name;
                PermissionDAO.RoleId = Permission.RoleId;
                PermissionDAO.MenuId = Permission.MenuId;
                PermissionDAO.StatusId = Permission.StatusId;
                PermissionDAOs.Add(PermissionDAO);
            }
            await DataContext.BulkMergeAsync(PermissionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Permission> Permissions)
        {
            List<long> Ids = Permissions.Select(x => x.Id).ToList();
            await DataContext.Permission
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Permission Permission)
        {
            await DataContext.PermissionFieldMapping
                .Where(x => x.PermissionId == Permission.Id)
                .DeleteFromQueryAsync();
            List<PermissionFieldMappingDAO> PermissionFieldMappingDAOs = new List<PermissionFieldMappingDAO>();
            if (Permission.PermissionFieldMappings != null)
            {
                foreach (PermissionFieldMapping PermissionFieldMapping in Permission.PermissionFieldMappings)
                {
                    PermissionFieldMappingDAO PermissionFieldMappingDAO = new PermissionFieldMappingDAO();
                    PermissionFieldMappingDAO.PermissionId = PermissionFieldMapping.PermissionId;
                    PermissionFieldMappingDAO.FieldId = PermissionFieldMapping.FieldId;
                    PermissionFieldMappingDAO.Value = PermissionFieldMapping.Value;
                    PermissionFieldMappingDAOs.Add(PermissionFieldMappingDAO);
                }
                await DataContext.PermissionFieldMapping.BulkMergeAsync(PermissionFieldMappingDAOs);
            }
            await DataContext.PermissionPageMapping
                .Where(x => x.PermissionId == Permission.Id)
                .DeleteFromQueryAsync();
            List<PermissionPageMappingDAO> PermissionPageMappingDAOs = new List<PermissionPageMappingDAO>();
            if (Permission.PermissionPageMappings != null)
            {
                foreach (PermissionPageMapping PermissionPageMapping in Permission.PermissionPageMappings)
                {
                    PermissionPageMappingDAO PermissionPageMappingDAO = new PermissionPageMappingDAO();
                    PermissionPageMappingDAO.PermissionId = PermissionPageMapping.PermissionId;
                    PermissionPageMappingDAO.PageId = PermissionPageMapping.PageId;
                    PermissionPageMappingDAOs.Add(PermissionPageMappingDAO);
                }
                await DataContext.PermissionPageMapping.BulkMergeAsync(PermissionPageMappingDAOs);
            }
        }

    }
}
