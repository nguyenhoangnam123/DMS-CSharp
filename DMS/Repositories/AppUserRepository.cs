using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IAppUserRepository
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AppUserDAO> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Username != null)
                query = query.Where(q => q.Username, filter.Username);
            if (filter.Password != null)
                query = query.Where(q => q.Password, filter.Password);
            if (filter.DisplayName != null)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.SexId != null)
                query = query.Where(q => q.SexId, filter.SexId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<AppUserDAO> OrFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AppUserDAO> initQuery = query.Where(q => false);
            foreach (AppUserFilter AppUserFilter in filter.OrFilter)
            {
                IQueryable<AppUserDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Username != null)
                    queryable = queryable.Where(q => q.Username, filter.Username);
                if (filter.Password != null)
                    queryable = queryable.Where(q => q.Password, filter.Password);
                if (filter.DisplayName != null)
                    queryable = queryable.Where(q => q.DisplayName, filter.DisplayName);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.Email != null)
                    queryable = queryable.Where(q => q.Email, filter.Email);
                if (filter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.SexId != null)
                    queryable = queryable.Where(q => q.SexId, filter.SexId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<AppUserDAO> DynamicOrder(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AppUserOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AppUserOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderBy(q => q.Sex);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AppUserOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AppUserOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderByDescending(q => q.Sex);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AppUser>> DynamicSelect(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            List<AppUser> AppUsers = await query.Select(q => new AppUser()
            {
                Id = filter.Selects.Contains(AppUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AppUserSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(AppUserSelect.Password) ? q.Password : default(string),
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Address = filter.Selects.Contains(AppUserSelect.Address) ? q.Address : default(string),
                Email = filter.Selects.Contains(AppUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AppUserSelect.Phone) ? q.Phone : default(string),
                StatusId = filter.Selects.Contains(AppUserSelect.Status) ? q.StatusId : default(long),
                SexId = filter.Selects.Contains(AppUserSelect.Sex) ? q.SexId : default(long),
                Status = filter.Selects.Contains(AppUserSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Sex = filter.Selects.Contains(AppUserSelect.Sex) && q.Sex != null ? new Sex
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return AppUsers;
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUsers = DataContext.AppUser;
            AppUsers = DynamicFilter(AppUsers, filter);
            return await AppUsers.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser;
            AppUserDAOs = DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUser.Where(x => x.Id == Id).Select(x => new AppUser()
            {
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                DisplayName = x.DisplayName,
                Address = x.Address,
                Email = x.Email,
                Phone = x.Phone,
                StatusId = x.StatusId,
                SexId = x.SexId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Sex = x.Sex == null ? null : new Sex
                {
                    Id = x.Sex.Id,
                    Code = x.Sex.Code,
                    Name = x.Sex.Name,
                }
            }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;
            AppUser.AppUserRoleMappings = await DataContext.AppUserRoleMapping
                .Where(x => x.AppUserId == AppUser.Id)
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId,
                    Role = new Role
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name,
                    },
                }).ToListAsync();

            return AppUser;
        }
        public async Task<bool> Create(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = new AppUserDAO();
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Address = AppUser.Address;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.StatusId = AppUser.StatusId;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            AppUserDAO.SexId = AppUser.SexId;
            DataContext.AppUser.Add(AppUserDAO);
            await DataContext.SaveChangesAsync();
            AppUser.Id = AppUserDAO.Id;
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser.Where(x => x.Id == AppUser.Id).FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Address = AppUser.Address;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.StatusId = AppUser.StatusId;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await DataContext.AppUser.Where(x => x.Id == AppUser.Id).UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            List<AppUserDAO> AppUserDAOs = new List<AppUserDAO>();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUserDAO AppUserDAO = new AppUserDAO();
                AppUserDAO.Id = AppUser.Id;
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.Password = AppUser.Password;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.Address = AppUser.Address;
                AppUserDAO.Email = AppUser.Email;
                AppUserDAO.Phone = AppUser.Phone;
                AppUserDAO.StatusId = AppUser.StatusId;
                AppUserDAO.SexId = AppUser.SexId;
                AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
                AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
                AppUserDAOs.Add(AppUserDAO);
            }
            await DataContext.BulkMergeAsync(AppUserDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUser
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(AppUser AppUser)
        {
            await DataContext.AppUserRoleMapping
                .Where(x => x.AppUserId == AppUser.Id)
                .DeleteFromQueryAsync();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = new List<AppUserRoleMappingDAO>();
            if (AppUser.AppUserRoleMappings != null)
            {
                foreach (AppUserRoleMapping AppUserRoleMapping in AppUser.AppUserRoleMappings)
                {
                    AppUserRoleMappingDAO AppUserRoleMappingDAO = new AppUserRoleMappingDAO();
                    AppUserRoleMappingDAO.AppUserId = AppUserRoleMapping.AppUserId;
                    AppUserRoleMappingDAO.RoleId = AppUserRoleMapping.RoleId;
                    AppUserRoleMappingDAOs.Add(AppUserRoleMappingDAO);
                }
                await DataContext.AppUserRoleMapping.BulkMergeAsync(AppUserRoleMappingDAOs);
            }
        }

    }
}
