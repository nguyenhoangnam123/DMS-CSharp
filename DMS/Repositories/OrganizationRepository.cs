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
    public interface IOrganizationRepository
    {
        Task<int> Count(OrganizationFilter OrganizationFilter);
        Task<List<Organization>> List(OrganizationFilter OrganizationFilter);
        Task<Organization> Get(long Id);
        Task<bool> Create(Organization Organization);
        Task<bool> Update(Organization Organization);
        Task<bool> Delete(Organization Organization);
        Task<bool> BulkMerge(List<Organization> Organizations);
        Task<bool> BulkDelete(List<Organization> Organizations);
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
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.Latitude != null)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.Longitude != null)
                query = query.Where(q => q.Longitude, filter.Longitude);
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
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.ParentId != null)
                    queryable = queryable.Where(q => q.ParentId, filter.ParentId);
                if (filter.Path != null)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                if (filter.Level != null)
                    queryable = queryable.Where(q => q.Level, filter.Level);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.Latitude != null)
                    queryable = queryable.Where(q => q.Latitude, filter.Latitude);
                if (filter.Longitude != null)
                    queryable = queryable.Where(q => q.Longitude, filter.Longitude);
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
                        case OrganizationOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case OrganizationOrder.Longitude:
                            query = query.OrderBy(q => q.Longitude);
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
                        case OrganizationOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case OrganizationOrder.Longitude:
                            query = query.OrderByDescending(q => q.Longitude);
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
                Latitude = filter.Selects.Contains(OrganizationSelect.Latitude) ? q.Latitude : default(decimal?),
                Longitude = filter.Selects.Contains(OrganizationSelect.Longitude) ? q.Longitude : default(decimal?),
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
                    Latitude = q.Parent.Latitude,
                    Longitude = q.Parent.Longitude,
                } : null,
                Status = filter.Selects.Contains(OrganizationSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
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
            IQueryable<OrganizationDAO> OrganizationDAOs = DataContext.Organization;
            OrganizationDAOs = DynamicFilter(OrganizationDAOs, filter);
            OrganizationDAOs = DynamicOrder(OrganizationDAOs, filter);
            List<Organization> Organizations = await DynamicSelect(OrganizationDAOs, filter);
            return Organizations;
        }

        public async Task<Organization> Get(long Id)
        {
            Organization Organization = await DataContext.Organization.Where(x => x.Id == Id).AsNoTracking().Select(x => new Organization()
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
                Latitude = x.Latitude,
                Longitude = x.Longitude,
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
                    Latitude = x.Parent.Latitude,
                    Longitude = x.Parent.Longitude,
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
            Organization.Stores = await DataContext.Store
                .Where(x => x.OrganizationId == Organization.Id)
                .Select(x => new Store
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentStoreId = x.ParentStoreId,
                    OrganizationId = x.OrganizationId,
                    StoreTypeId = x.StoreTypeId,
                    StoreGroupingId = x.StoreGroupingId,
                    Telephone = x.Telephone,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                    Address = x.Address,
                    DeliveryAddress = x.DeliveryAddress,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    OwnerName = x.OwnerName,
                    OwnerPhone = x.OwnerPhone,
                    OwnerEmail = x.OwnerEmail,
                    StatusId = x.StatusId,
                    District = new District
                    {
                        Id = x.District.Id,
                        Name = x.District.Name,
                        Priority = x.District.Priority,
                        ProvinceId = x.District.ProvinceId,
                        StatusId = x.District.StatusId,
                    },
                    ParentStore = new Store
                    {
                        Id = x.ParentStore.Id,
                        Code = x.ParentStore.Code,
                        Name = x.ParentStore.Name,
                        ParentStoreId = x.ParentStore.ParentStoreId,
                        OrganizationId = x.ParentStore.OrganizationId,
                        StoreTypeId = x.ParentStore.StoreTypeId,
                        StoreGroupingId = x.ParentStore.StoreGroupingId,
                        Telephone = x.ParentStore.Telephone,
                        ProvinceId = x.ParentStore.ProvinceId,
                        DistrictId = x.ParentStore.DistrictId,
                        WardId = x.ParentStore.WardId,
                        Address = x.ParentStore.Address,
                        DeliveryAddress = x.ParentStore.DeliveryAddress,
                        Latitude = x.ParentStore.Latitude,
                        Longitude = x.ParentStore.Longitude,
                        OwnerName = x.ParentStore.OwnerName,
                        OwnerPhone = x.ParentStore.OwnerPhone,
                        OwnerEmail = x.ParentStore.OwnerEmail,
                        StatusId = x.ParentStore.StatusId,
                    },
                    Province = new Province
                    {
                        Id = x.Province.Id,
                        Name = x.Province.Name,
                        Priority = x.Province.Priority,
                        StatusId = x.Province.StatusId,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    StoreGrouping = new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                    },
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                    Ward = new Ward
                    {
                        Id = x.Ward.Id,
                        Name = x.Ward.Name,
                        Priority = x.Ward.Priority,
                        DistrictId = x.Ward.DistrictId,
                        StatusId = x.Ward.StatusId,
                    },
                }).ToListAsync();

            return Organization;
        }
        public async Task<bool> Create(Organization Organization)
        {
            OrganizationDAO OrganizationDAO = new OrganizationDAO();
            OrganizationDAO.Id = Organization.Id;
            OrganizationDAO.Code = Organization.Code;
            OrganizationDAO.Name = Organization.Name;
            OrganizationDAO.ParentId = Organization.ParentId;
            OrganizationDAO.Path = Organization.Path;
            OrganizationDAO.Level = Organization.Level;
            OrganizationDAO.StatusId = Organization.StatusId;
            OrganizationDAO.Phone = Organization.Phone;
            OrganizationDAO.Address = Organization.Address;
            OrganizationDAO.Latitude = Organization.Latitude;
            OrganizationDAO.Longitude = Organization.Longitude;
            OrganizationDAO.CreatedAt = StaticParams.DateTimeNow;
            OrganizationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Organization.Add(OrganizationDAO);
            await DataContext.SaveChangesAsync();
            Organization.Id = OrganizationDAO.Id;
            await SaveReference(Organization);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(Organization Organization)
        {
            OrganizationDAO OrganizationDAO = DataContext.Organization.Where(x => x.Id == Organization.Id).AsNoTracking().FirstOrDefault();
            if (OrganizationDAO == null)
                return false;
            OrganizationDAO.Id = Organization.Id;
            OrganizationDAO.Code = Organization.Code;
            OrganizationDAO.Name = Organization.Name;
            OrganizationDAO.ParentId = Organization.ParentId;
            OrganizationDAO.Path = Organization.Path;
            OrganizationDAO.Level = Organization.Level;
            OrganizationDAO.StatusId = Organization.StatusId;
            OrganizationDAO.Phone = Organization.Phone;
            OrganizationDAO.Address = Organization.Address;
            OrganizationDAO.Latitude = Organization.Latitude;
            OrganizationDAO.Longitude = Organization.Longitude;
            OrganizationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Organization);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(Organization Organization)
        {
            OrganizationDAO OrganizationDAO = await DataContext.Organization.Where(x => x.Id == Organization.Id).AsNoTracking().FirstOrDefaultAsync();
            await DataContext.Organization.Where(x => x.Path.StartsWith(OrganizationDAO.Id + ".")).UpdateFromQueryAsync(x => new OrganizationDAO { DeletedAt = StaticParams.DateTimeNow });
            await DataContext.Organization.Where(x => x.Id == Organization.Id).AsNoTracking().UpdateFromQueryAsync(x => new OrganizationDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkMerge(List<Organization> Organizations)
        {
            List<OrganizationDAO> OrganizationDAOs = new List<OrganizationDAO>();
            foreach (Organization Organization in Organizations)
            {
                OrganizationDAO OrganizationDAO = new OrganizationDAO();
                OrganizationDAO.Id = Organization.Id;
                OrganizationDAO.Code = Organization.Code;
                OrganizationDAO.Name = Organization.Name;
                OrganizationDAO.ParentId = Organization.ParentId;
                OrganizationDAO.Path = Organization.Path;
                OrganizationDAO.Level = Organization.Level;
                OrganizationDAO.StatusId = Organization.StatusId;
                OrganizationDAO.Phone = Organization.Phone;
                OrganizationDAO.Address = Organization.Address;
                OrganizationDAO.Latitude = Organization.Latitude;
                OrganizationDAO.Longitude = Organization.Longitude;
                OrganizationDAO.CreatedAt = StaticParams.DateTimeNow;
                OrganizationDAO.UpdatedAt = StaticParams.DateTimeNow;
                OrganizationDAOs.Add(OrganizationDAO);
            }
            await DataContext.BulkMergeAsync(OrganizationDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<Organization> Organizations)
        {
            List<long> Ids = Organizations.Select(x => x.Id).ToList();
            await DataContext.Organization
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new OrganizationDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(Organization Organization)
        {
            List<StoreDAO> StoreDAOs = await DataContext.Store
                .Where(x => x.OrganizationId == Organization.Id).ToListAsync();
            StoreDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Organization.Stores != null)
            {
                foreach (Store Store in Organization.Stores)
                {
                    StoreDAO StoreDAO = StoreDAOs
                        .Where(x => x.Id == Store.Id && x.Id != 0).FirstOrDefault();
                    if (StoreDAO == null)
                    {
                        StoreDAO = new StoreDAO();
                        StoreDAO.Id = Store.Id;
                        StoreDAO.Code = Store.Code;
                        StoreDAO.Name = Store.Name;
                        StoreDAO.ParentStoreId = Store.ParentStoreId;
                        StoreDAO.OrganizationId = Organization.Id;
                        StoreDAO.StoreTypeId = Store.StoreTypeId;
                        StoreDAO.StoreGroupingId = Store.StoreGroupingId;
                        StoreDAO.Telephone = Store.Telephone;
                        StoreDAO.ProvinceId = Store.ProvinceId;
                        StoreDAO.DistrictId = Store.DistrictId;
                        StoreDAO.WardId = Store.WardId;
                        StoreDAO.Address = Store.Address;
                        StoreDAO.DeliveryAddress = Store.DeliveryAddress;
                        StoreDAO.Latitude = Store.Latitude;
                        StoreDAO.Longitude = Store.Longitude;
                        StoreDAO.OwnerName = Store.OwnerName;
                        StoreDAO.OwnerPhone = Store.OwnerPhone;
                        StoreDAO.OwnerEmail = Store.OwnerEmail;
                        StoreDAO.StatusId = Store.StatusId;
                        StoreDAOs.Add(StoreDAO);
                        StoreDAO.CreatedAt = StaticParams.DateTimeNow;
                        StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
                        StoreDAO.DeletedAt = null;
                    }
                    else
                    {
                        StoreDAO.Id = Store.Id;
                        StoreDAO.Code = Store.Code;
                        StoreDAO.Name = Store.Name;
                        StoreDAO.ParentStoreId = Store.ParentStoreId;
                        StoreDAO.OrganizationId = Organization.Id;
                        StoreDAO.StoreTypeId = Store.StoreTypeId;
                        StoreDAO.StoreGroupingId = Store.StoreGroupingId;
                        StoreDAO.Telephone = Store.Telephone;
                        StoreDAO.ProvinceId = Store.ProvinceId;
                        StoreDAO.DistrictId = Store.DistrictId;
                        StoreDAO.WardId = Store.WardId;
                        StoreDAO.Address = Store.Address;
                        StoreDAO.DeliveryAddress = Store.DeliveryAddress;
                        StoreDAO.Latitude = Store.Latitude;
                        StoreDAO.Longitude = Store.Longitude;
                        StoreDAO.OwnerName = Store.OwnerName;
                        StoreDAO.OwnerPhone = Store.OwnerPhone;
                        StoreDAO.OwnerEmail = Store.OwnerEmail;
                        StoreDAO.StatusId = Store.StatusId;
                        StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
                        StoreDAO.DeletedAt = null;
                    }
                }
                await DataContext.Store.BulkMergeAsync(StoreDAOs);
            }
        }

        private async Task BuildPath()
        {
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization
                .Where(x => x.DeletedAt != null)
                .ToListAsync();
            Queue<OrganizationDAO> queue = new Queue<OrganizationDAO>();
            OrganizationDAOs.ForEach(x =>
            {
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    queue.Enqueue(x);
                }
            });
            while (queue.Count > 0)
            {
                OrganizationDAO Parent = queue.Dequeue();
                foreach (OrganizationDAO OrganizationDAO in OrganizationDAOs)
                {
                    if (OrganizationDAO.ParentId == Parent.Id)
                    {
                        OrganizationDAO.Path = Parent.Path + OrganizationDAO.Id + ".";
                        queue.Enqueue(OrganizationDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(OrganizationDAOs);
        }
    }
}
