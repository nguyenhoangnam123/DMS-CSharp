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
        Task<bool> Update(AppUser AppUser);
        Task<bool> SimpleUpdate(AppUser AppUser);
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
            if (filter.Birthday != null)
                query = query.Where(q => q.Birthday, filter.Birthday);
            if (filter.PositionId != null)
                query = query.Where(q => q.PositionId, filter.PositionId);
            if (filter.Department != null)
                query = query.Where(q => q.Department, filter.Department);
            if (filter.OrganizationId != null)
            {
                if (filter.OrganizationId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value).FirstOrDefault();
                    query = query.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => q.OrganizationId.HasValue && Ids.Contains(q.OrganizationId.Value));
                }
                if (filter.OrganizationId.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => q.OrganizationId.HasValue && !Ids.Contains(q.OrganizationId.Value));
                }
            }
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.OrganizationId);
            if (filter.RoleId != null)
            {
                if (filter.RoleId.Equal.HasValue)
                {
                    query = from q in query
                            join ar in DataContext.AppUserRoleMapping on q.Id equals ar.AppUserId
                            where ar.RoleId == filter.RoleId.Equal.Value
                            select q;
                }
            }
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
                if (AppUserFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, AppUserFilter.Id);
                if (AppUserFilter.Username != null)
                    queryable = queryable.Where(q => q.Username, AppUserFilter.Username);
                if (AppUserFilter.DisplayName != null)
                    queryable = queryable.Where(q => q.DisplayName, AppUserFilter.DisplayName);
                if (AppUserFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, AppUserFilter.Address);
                if (AppUserFilter.Email != null)
                    queryable = queryable.Where(q => q.Email, AppUserFilter.Email);
                if (AppUserFilter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, AppUserFilter.Phone);
                if (AppUserFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, AppUserFilter.StatusId);
                if (AppUserFilter.SexId != null)
                    queryable = queryable.Where(q => q.SexId, AppUserFilter.SexId);
                if (AppUserFilter.Birthday != null)
                    queryable = queryable.Where(q => q.Birthday, AppUserFilter.Birthday);
                if (AppUserFilter.PositionId != null)
                    queryable = queryable.Where(q => q.PositionId, AppUserFilter.PositionId);
                if (AppUserFilter.Department != null)
                    queryable = queryable.Where(q => q.Department, AppUserFilter.Department);
                if (AppUserFilter.OrganizationId != null)
                {
                    if (AppUserFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == AppUserFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (AppUserFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == AppUserFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (AppUserFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => AppUserFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => q.OrganizationId.HasValue && Ids.Contains(q.OrganizationId.Value));
                    }
                    if (AppUserFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => AppUserFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => q.OrganizationId.HasValue && !Ids.Contains(q.OrganizationId.Value));
                    }
                }
                if (AppUserFilter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
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
                        case AppUserOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                        case AppUserOrder.Position:
                            query = query.OrderBy(q => q.PositionId);
                            break;
                        case AppUserOrder.Department:
                            query = query.OrderBy(q => q.Department);
                            break;
                        case AppUserOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case AppUserOrder.ERouteScope:
                            query = query.OrderBy(q => q.ERouteScopeId);
                            break;
                        case AppUserOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
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
                        case AppUserOrder.Birthday:
                            query = query.OrderByDescending(q => q.Birthday);
                            break;
                        case AppUserOrder.Position:
                            query = query.OrderByDescending(q => q.PositionId);
                            break;
                        case AppUserOrder.Department:
                            query = query.OrderByDescending(q => q.Department);
                            break;
                        case AppUserOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case AppUserOrder.ERouteScope:
                            query = query.OrderByDescending(q => q.ERouteScopeId);
                            break;
                        case AppUserOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
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
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Address = filter.Selects.Contains(AppUserSelect.Address) ? q.Address : default(string),
                Avatar = filter.Selects.Contains(AppUserSelect.Avatar) ? q.Avatar : default(string),
                Email = filter.Selects.Contains(AppUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AppUserSelect.Phone) ? q.Phone : default(string),
                StatusId = filter.Selects.Contains(AppUserSelect.Status) ? q.StatusId : default(long),
                SexId = filter.Selects.Contains(AppUserSelect.Sex) ? q.SexId : default(long),
                Birthday = filter.Selects.Contains(AppUserSelect.Birthday) ? q.Birthday : default(DateTime?),
                PositionId = filter.Selects.Contains(AppUserSelect.Position) ? q.PositionId : default(long),
                Department = filter.Selects.Contains(AppUserSelect.Department) ? q.Department : default(string),
                OrganizationId = filter.Selects.Contains(AppUserSelect.Organization) ? q.OrganizationId : default(long?),
                ERouteScopeId = filter.Selects.Contains(AppUserSelect.ERouteScope) ? q.ERouteScopeId : default(long?),
                ProvinceId = filter.Selects.Contains(AppUserSelect.Province) ? q.ProvinceId : default(long),
                Latitude = filter.Selects.Contains(AppUserSelect.Latitude) ? q.Latitude : default(decimal?),
                Longitude = filter.Selects.Contains(AppUserSelect.Longitude) ? q.Longitude : default(decimal?),
                Organization = filter.Selects.Contains(AppUserSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    Address = q.Organization.Address,
                    Phone = q.Organization.Phone,
                    Path = q.Organization.Path,
                    ParentId = q.Organization.ParentId,
                    Email = q.Organization.Email,
                    StatusId = q.Organization.StatusId,
                    Level = q.Organization.Level
                } : null,
                ERouteScope = filter.Selects.Contains(AppUserSelect.ERouteScope) && q.ERouteScope != null ? new Organization
                {
                    Id = q.ERouteScope.Id,
                    Code = q.ERouteScope.Code,
                    Name = q.ERouteScope.Name,
                    Address = q.ERouteScope.Address,
                    Phone = q.ERouteScope.Phone,
                    Path = q.ERouteScope.Path,
                    ParentId = q.ERouteScope.ParentId,
                    Email = q.ERouteScope.Email,
                    StatusId = q.ERouteScope.StatusId,
                    Level = q.ERouteScope.Level
                } : null,
                Province = filter.Selects.Contains(AppUserSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                Position = filter.Selects.Contains(AppUserSelect.Position) && q.Position != null ? new Position
                {
                    Id = q.Position.Id,
                    Code = q.Position.Code,
                    Name = q.Position.Name,
                    StatusId = q.Position.StatusId,
                } : null,
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
                RowId = filter.Selects.Contains(AppUserSelect.RowId) ? q.RowId : default(Guid)
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
                DisplayName = x.DisplayName,
                Address = x.Address,
                Avatar = x.Avatar,
                Birthday = x.Birthday,
                Email = x.Email,
                Phone = x.Phone,
                StatusId = x.StatusId,
                SexId = x.SexId,
                PositionId = x.PositionId,
                Department = x.Department,
                OrganizationId = x.OrganizationId,
                ERouteScopeId = x.ERouteScopeId,
                ProvinceId = x.ProvinceId,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                RowId = x.RowId,
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    Address = x.Organization.Address,
                    Phone = x.Organization.Phone,
                    Path = x.Organization.Path,
                    ParentId = x.Organization.ParentId,
                    Email = x.Organization.Email,
                    StatusId = x.Organization.StatusId,
                    Level = x.Organization.Level
                },
                ERouteScope = x.ERouteScope == null ? null : new Organization
                {
                    Id = x.ERouteScope.Id,
                    Code = x.ERouteScope.Code,
                    Name = x.ERouteScope.Name,
                    Address = x.ERouteScope.Address,
                    Phone = x.ERouteScope.Phone,
                    Path = x.ERouteScope.Path,
                    ParentId = x.ERouteScope.ParentId,
                    Email = x.ERouteScope.Email,
                    StatusId = x.ERouteScope.StatusId,
                    Level = x.ERouteScope.Level
                },
                Position = x.Position == null ? null : new Position
                {
                    Id = x.Position.Id,
                    Code = x.Position.Code,
                    Name = x.Position.Name,
                    StatusId = x.Position.StatusId,
                },
                Province = x.Province == null ? null : new Province
                {
                    Id = x.Province.Id,
                    Code = x.Province.Code,
                    Name = x.Province.Name,
                    Priority = x.Province.Priority,
                    StatusId = x.Province.StatusId,
                },
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

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser.Where(x => x.Id == AppUser.Id).FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.ERouteScopeId = AppUser.ERouteScopeId;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SimpleUpdate(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser.Where(x => x.Id == AppUser.Id).FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Latitude = AppUser.Latitude;
            AppUserDAO.Longitude = AppUser.Longitude;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            return true;
        }

    }
}
