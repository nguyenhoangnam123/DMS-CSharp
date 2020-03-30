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
    public interface IStoreGroupingRepository
    {
        Task<int> Count(StoreGroupingFilter StoreGroupingFilter);
        Task<List<StoreGrouping>> List(StoreGroupingFilter StoreGroupingFilter);
        Task<StoreGrouping> Get(long Id);
        Task<bool> Create(StoreGrouping StoreGrouping);
        Task<bool> Update(StoreGrouping StoreGrouping);
        Task<bool> Delete(StoreGrouping StoreGrouping);
        Task<bool> BulkMerge(List<StoreGrouping> StoreGroupings);
        Task<bool> BulkDelete(List<StoreGrouping> StoreGroupings);
    }
    public class StoreGroupingRepository : IStoreGroupingRepository
    {
        private DataContext DataContext;
        public StoreGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreGroupingDAO> DynamicFilter(IQueryable<StoreGroupingDAO> query, StoreGroupingFilter filter)
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
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<StoreGroupingDAO> OrFilter(IQueryable<StoreGroupingDAO> query, StoreGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreGroupingDAO> initQuery = query.Where(q => false);
            foreach (StoreGroupingFilter StoreGroupingFilter in filter.OrFilter)
            {
                IQueryable<StoreGroupingDAO> queryable = query;
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
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<StoreGroupingDAO> DynamicOrder(IQueryable<StoreGroupingDAO> query, StoreGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreGroupingOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StoreGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case StoreGroupingOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case StoreGroupingOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case StoreGroupingOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case StoreGroupingOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreGroupingOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StoreGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case StoreGroupingOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case StoreGroupingOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case StoreGroupingOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case StoreGroupingOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<StoreGrouping>> DynamicSelect(IQueryable<StoreGroupingDAO> query, StoreGroupingFilter filter)
        {
            List<StoreGrouping> StoreGroupings = await query.Select(q => new StoreGrouping()
            {
                Id = filter.Selects.Contains(StoreGroupingSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StoreGroupingSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(StoreGroupingSelect.Name) ? q.Name : default(string),
                ParentId = filter.Selects.Contains(StoreGroupingSelect.Parent) ? q.ParentId : default(long?),
                Path = filter.Selects.Contains(StoreGroupingSelect.Path) ? q.Path : default(string),
                Level = filter.Selects.Contains(StoreGroupingSelect.Level) ? q.Level : default(long),
                StatusId = filter.Selects.Contains(StoreGroupingSelect.Status) ? q.StatusId : default(long),
            }).ToListAsync();
            return StoreGroupings;
        }

        public async Task<int> Count(StoreGroupingFilter filter)
        {
            IQueryable<StoreGroupingDAO> StoreGroupings = DataContext.StoreGrouping;
            StoreGroupings = DynamicFilter(StoreGroupings, filter);
            return await StoreGroupings.CountAsync();
        }

        public async Task<List<StoreGrouping>> List(StoreGroupingFilter filter)
        {
            if (filter == null) return new List<StoreGrouping>();
            IQueryable<StoreGroupingDAO> StoreGroupingDAOs = DataContext.StoreGrouping;
            StoreGroupingDAOs = DynamicFilter(StoreGroupingDAOs, filter);
            StoreGroupingDAOs = DynamicOrder(StoreGroupingDAOs, filter);
            List<StoreGrouping> StoreGroupings = await DynamicSelect(StoreGroupingDAOs, filter);
            return StoreGroupings;
        }

        public async Task<StoreGrouping> Get(long Id)
        {
            StoreGrouping StoreGrouping = await DataContext.StoreGrouping.Where(x => x.Id == Id).Select(x => new StoreGrouping()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ParentId = x.ParentId,
                Path = x.Path,
                Level = x.Level,
                StatusId = x.StatusId,
                Parent = x.Parent == null ? null : new StoreGrouping
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Path = x.Path,
                    Level = x.Level,
                    StatusId = x.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (StoreGrouping == null)
                return null;
            StoreGrouping.Stores = await DataContext.Store
                .Where(x => x.StoreGroupingId == StoreGrouping.Id)
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
                    Organization = new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        ParentId = x.Organization.ParentId,
                        Path = x.Organization.Path,
                        Level = x.Organization.Level,
                        StatusId = x.Organization.StatusId,
                        Phone = x.Organization.Phone,
                        Address = x.Organization.Address,
                        Latitude = x.Organization.Latitude,
                        Longitude = x.Organization.Longitude,
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

            return StoreGrouping;
        }
        public async Task<bool> Create(StoreGrouping StoreGrouping)
        {
            StoreGroupingDAO StoreGroupingDAO = new StoreGroupingDAO();
            StoreGroupingDAO.Id = StoreGrouping.Id;
            StoreGroupingDAO.Code = StoreGrouping.Code;
            StoreGroupingDAO.Name = StoreGrouping.Name;
            StoreGroupingDAO.ParentId = StoreGrouping.ParentId;
            StoreGroupingDAO.Path = "";
            StoreGroupingDAO.Level = 1;
            StoreGroupingDAO.StatusId = StoreGrouping.StatusId;
            StoreGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            StoreGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.StoreGrouping.Add(StoreGroupingDAO);
            await DataContext.SaveChangesAsync();
            StoreGrouping.Id = StoreGroupingDAO.Id;
            await SaveReference(StoreGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(StoreGrouping StoreGrouping)
        {
            StoreGroupingDAO StoreGroupingDAO = DataContext.StoreGrouping.Where(x => x.Id == StoreGrouping.Id).FirstOrDefault();
            if (StoreGroupingDAO == null)
                return false;
            StoreGroupingDAO.Id = StoreGrouping.Id;
            StoreGroupingDAO.Code = StoreGrouping.Code;
            StoreGroupingDAO.Name = StoreGrouping.Name;
            StoreGroupingDAO.ParentId = StoreGrouping.ParentId;
            StoreGroupingDAO.Path = "";
            StoreGroupingDAO.Level = 1;
            StoreGroupingDAO.StatusId = StoreGrouping.StatusId;
            StoreGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(StoreGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(StoreGrouping StoreGrouping)
        {
            StoreGroupingDAO StoreGroupingDAO = await DataContext.StoreGrouping.Where(x => x.Id == StoreGrouping.Id).FirstOrDefaultAsync();
            await DataContext.StoreGrouping.Where(x => x.Path.StartsWith(StoreGroupingDAO.Id + ".")).UpdateFromQueryAsync(x => new StoreGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await DataContext.StoreGrouping.Where(x => x.Id == StoreGrouping.Id).UpdateFromQueryAsync(x => new StoreGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkMerge(List<StoreGrouping> StoreGroupings)
        {
            List<StoreGroupingDAO> StoreGroupingDAOs = new List<StoreGroupingDAO>();
            foreach (StoreGrouping StoreGrouping in StoreGroupings)
            {
                StoreGroupingDAO StoreGroupingDAO = new StoreGroupingDAO();
                StoreGroupingDAO.Id = StoreGrouping.Id;
                StoreGroupingDAO.Code = StoreGrouping.Code;
                StoreGroupingDAO.Name = StoreGrouping.Name;
                StoreGroupingDAO.ParentId = StoreGrouping.ParentId;
                StoreGroupingDAO.Path = StoreGrouping.Path;
                StoreGroupingDAO.Level = StoreGrouping.Level;
                StoreGroupingDAO.StatusId = StoreGrouping.StatusId;
                StoreGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                StoreGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                StoreGroupingDAOs.Add(StoreGroupingDAO);
            }
            await DataContext.BulkMergeAsync(StoreGroupingDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<StoreGrouping> StoreGroupings)
        {
            List<long> Ids = StoreGroupings.Select(x => x.Id).ToList();
            await DataContext.StoreGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new StoreGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(StoreGrouping StoreGrouping)
        {
            List<StoreDAO> StoreDAOs = await DataContext.Store
                .Where(x => x.StoreGroupingId == StoreGrouping.Id).ToListAsync();
            StoreDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (StoreGrouping.Stores != null)
            {
                foreach (Store Store in StoreGrouping.Stores)
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
                        StoreDAO.OrganizationId = Store.OrganizationId;
                        StoreDAO.StoreTypeId = Store.StoreTypeId;
                        StoreDAO.StoreGroupingId = StoreGrouping.Id;
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
                        StoreDAO.OrganizationId = Store.OrganizationId;
                        StoreDAO.StoreTypeId = Store.StoreTypeId;
                        StoreDAO.StoreGroupingId = StoreGrouping.Id;
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
            List<StoreGroupingDAO> StoreGroupingDAOs = await DataContext.StoreGrouping
                .Where(x => x.DeletedAt == null)
                .ToListAsync();
            Queue<StoreGroupingDAO> queue = new Queue<StoreGroupingDAO>();
            StoreGroupingDAOs.ForEach(x =>
            {
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    queue.Enqueue(x);
                }
            });
            while (queue.Count > 0)
            {
                StoreGroupingDAO Parent = queue.Dequeue();
                foreach (StoreGroupingDAO StoreGroupingDAO in StoreGroupingDAOs)
                {
                    if (StoreGroupingDAO.ParentId == Parent.Id)
                    {
                        StoreGroupingDAO.Path = Parent.Path + StoreGroupingDAO.Id + ".";
                        queue.Enqueue(StoreGroupingDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(StoreGroupingDAOs);
        }
    }
}
