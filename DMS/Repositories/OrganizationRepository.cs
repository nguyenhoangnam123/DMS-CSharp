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
    public interface IOrganizationRepository
    {
        Task<int> Count(OrganizationFilter OrganizationFilter);
        Task<List<Organization>> List(OrganizationFilter OrganizationFilter);
        Task<Organization> Get(long Id);
    }
    public class OrganizationRepository : IOrganizationRepository
    {
        private DataContext DataContext;
        public OrganizationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<OrganizationDAO> DynamicFilter(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ParentId != null)
                query = query.Where(q => q.ParentId, filter.ParentId);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            if (filter.Level != null)
                query = query.Where(q => q.Level, filter.Level);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<OrganizationDAO> OrFilter(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<OrganizationDAO> initQuery = query.Where(q => false);
            foreach (OrganizationFilter OrganizationFilter in filter.OrFilter)
            {
                IQueryable<OrganizationDAO> queryable = query;
                if (OrganizationFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, OrganizationFilter.Id);
                if (OrganizationFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, OrganizationFilter.Code);
                if (OrganizationFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, OrganizationFilter.Name);
                if (OrganizationFilter.ParentId != null)
                    queryable = queryable.Where(q => q.ParentId, OrganizationFilter.ParentId);
                if (OrganizationFilter.Path != null)
                    queryable = queryable.Where(q => q.Path, OrganizationFilter.Path);
                if (OrganizationFilter.Level != null)
                    queryable = queryable.Where(q => q.Level, OrganizationFilter.Level);
                if (OrganizationFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, OrganizationFilter.StatusId);
                if (OrganizationFilter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, OrganizationFilter.Phone);
                if (OrganizationFilter.Email != null)
                    queryable = queryable.Where(q => q.Email, OrganizationFilter.Email);
                if (OrganizationFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, OrganizationFilter.Address);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<OrganizationDAO> DynamicOrder(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case OrganizationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case OrganizationOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case OrganizationOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case OrganizationOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case OrganizationOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case OrganizationOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case OrganizationOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case OrganizationOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case OrganizationOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case OrganizationOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case OrganizationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case OrganizationOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case OrganizationOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case OrganizationOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case OrganizationOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case OrganizationOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case OrganizationOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case OrganizationOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case OrganizationOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case OrganizationOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Organization>> DynamicSelect(IQueryable<OrganizationDAO> query, OrganizationFilter filter)
        {
            List<Organization> Organizations = await query.Select(q => new Organization()
            {
                Id = filter.Selects.Contains(OrganizationSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(OrganizationSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(OrganizationSelect.Name) ? q.Name : default(string),
                ParentId = filter.Selects.Contains(OrganizationSelect.Parent) ? q.ParentId : default(long?),
                Path = filter.Selects.Contains(OrganizationSelect.Path) ? q.Path : default(string),
                Level = filter.Selects.Contains(OrganizationSelect.Level) ? q.Level : default(long),
                StatusId = filter.Selects.Contains(OrganizationSelect.Status) ? q.StatusId : default(long),
                Phone = filter.Selects.Contains(OrganizationSelect.Phone) ? q.Phone : default(string),
                Address = filter.Selects.Contains(OrganizationSelect.Address) ? q.Address : default(string),
                Email = filter.Selects.Contains(OrganizationSelect.Email) ? q.Email : default(string),
                Parent = filter.Selects.Contains(OrganizationSelect.Parent) && q.Parent != null ? new Organization
                {
                    Id = q.Parent.Id,
                    Code = q.Parent.Code,
                    Name = q.Parent.Name,
                    ParentId = q.Parent.ParentId,
                    Path = q.Parent.Path,
                    Level = q.Parent.Level,
                    StatusId = q.Parent.StatusId,
                    Phone = q.Parent.Phone,
                    Address = q.Parent.Address,
                    Email = q.Parent.Email,
                } : null,
                Status = filter.Selects.Contains(OrganizationSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                RowId = filter.Selects.Contains(OrganizationSelect.RowId) ? q.RowId : default(Guid)
            }).ToListAsync();
            return Organizations;
        }

        public async Task<int> Count(OrganizationFilter filter)
        {
            IQueryable<OrganizationDAO> Organizations = DataContext.Organization;
            Organizations = DynamicFilter(Organizations, filter);
            return await Organizations.CountAsync();
        }

        public async Task<List<Organization>> List(OrganizationFilter filter)
        {
            if (filter == null) return new List<Organization>();
            IQueryable<OrganizationDAO> OrganizationDAOs = DataContext.Organization.AsNoTracking();
            OrganizationDAOs = DynamicFilter(OrganizationDAOs, filter);
            OrganizationDAOs = DynamicOrder(OrganizationDAOs, filter);
            List<Organization> Organizations = await DynamicSelect(OrganizationDAOs, filter);
            return Organizations;
        }

        public async Task<Organization> Get(long Id)
        {
            Organization Organization = await DataContext.Organization
                .AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Organization()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Path = x.Path,
                    Level = x.Level,
                    StatusId = x.StatusId,
                    Phone = x.Phone,
                    Address = x.Address,
                    Email = x.Email,
                    Parent = x.Parent == null ? null : new Organization
                    {
                        Id = x.Parent.Id,
                        Code = x.Parent.Code,
                        Name = x.Parent.Name,
                        ParentId = x.Parent.ParentId,
                        Path = x.Parent.Path,
                        Level = x.Parent.Level,
                        StatusId = x.Parent.StatusId,
                        Phone = x.Parent.Phone,
                        Address = x.Parent.Address,
                        Email = x.Parent.Email,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).FirstOrDefaultAsync();

            if (Organization == null)
                return null;
            Organization.AppUsers = await DataContext.AppUser
                .Where(x => x.OrganizationId == Organization.Id)
                .Select(x => new AppUser
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    Address = x.Address,
                    Email = x.Email,
                    Phone = x.Phone,
                    Department = x.Department,
                    OrganizationId = x.OrganizationId,
                    SexId = x.SexId,
                    StatusId = x.StatusId,
                    Sex = new Sex
                    {
                        Id = x.Sex.Id,
                        Code = x.Sex.Code,
                        Name = x.Sex.Name,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToListAsync();

            return Organization;
        }
    }
}
