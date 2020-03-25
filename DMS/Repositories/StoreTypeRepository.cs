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
    public interface IStoreTypeRepository
    {
        Task<int> Count(StoreTypeFilter StoreTypeFilter);
        Task<List<StoreType>> List(StoreTypeFilter StoreTypeFilter);
        Task<StoreType> Get(long Id);
        Task<bool> Create(StoreType StoreType);
        Task<bool> Update(StoreType StoreType);
        Task<bool> Delete(StoreType StoreType);
        Task<bool> BulkMerge(List<StoreType> StoreTypes);
        Task<bool> BulkDelete(List<StoreType> StoreTypes);
    }
    public class StoreTypeRepository : IStoreTypeRepository
    {
        private DataContext DataContext;
        public StoreTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreTypeDAO> DynamicFilter(IQueryable<StoreTypeDAO> query, StoreTypeFilter filter)
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
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<StoreTypeDAO> OrFilter(IQueryable<StoreTypeDAO> query, StoreTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreTypeDAO> initQuery = query.Where(q => false);
            foreach (StoreTypeFilter StoreTypeFilter in filter.OrFilter)
            {
                IQueryable<StoreTypeDAO> queryable = query;
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

        private IQueryable<StoreTypeDAO> DynamicOrder(IQueryable<StoreTypeDAO> query, StoreTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StoreTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case StoreTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StoreTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case StoreTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<StoreType>> DynamicSelect(IQueryable<StoreTypeDAO> query, StoreTypeFilter filter)
        {
            List<StoreType> StoreTypes = await query.Select(q => new StoreType()
            {
                Id = filter.Selects.Contains(StoreTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StoreTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(StoreTypeSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(StoreTypeSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(StoreTypeSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return StoreTypes;
        }

        public async Task<int> Count(StoreTypeFilter filter)
        {
            IQueryable<StoreTypeDAO> StoreTypes = DataContext.StoreType;
            StoreTypes = DynamicFilter(StoreTypes, filter);
            return await StoreTypes.CountAsync();
        }

        public async Task<List<StoreType>> List(StoreTypeFilter filter)
        {
            if (filter == null) return new List<StoreType>();
            IQueryable<StoreTypeDAO> StoreTypeDAOs = DataContext.StoreType;
            StoreTypeDAOs = DynamicFilter(StoreTypeDAOs, filter);
            StoreTypeDAOs = DynamicOrder(StoreTypeDAOs, filter);
            List<StoreType> StoreTypes = await DynamicSelect(StoreTypeDAOs, filter);
            return StoreTypes;
        }

        public async Task<StoreType> Get(long Id)
        {
            StoreType StoreType = await DataContext.StoreType.Where(x => x.Id == Id).Select(x => new StoreType()
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

            if (StoreType == null)
                return null;

            return StoreType;
        }
        public async Task<bool> Create(StoreType StoreType)
        {
            StoreTypeDAO StoreTypeDAO = new StoreTypeDAO();
            StoreTypeDAO.Id = StoreType.Id;
            StoreTypeDAO.Code = StoreType.Code;
            StoreTypeDAO.Name = StoreType.Name;
            StoreTypeDAO.StatusId = StoreType.StatusId;
            StoreTypeDAO.CreatedAt = StaticParams.DateTimeNow;
            StoreTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.StoreType.Add(StoreTypeDAO);
            await DataContext.SaveChangesAsync();
            StoreType.Id = StoreTypeDAO.Id;
            await SaveReference(StoreType);
            return true;
        }

        public async Task<bool> Update(StoreType StoreType)
        {
            StoreTypeDAO StoreTypeDAO = DataContext.StoreType.Where(x => x.Id == StoreType.Id).FirstOrDefault();
            if (StoreTypeDAO == null)
                return false;
            StoreTypeDAO.Id = StoreType.Id;
            StoreTypeDAO.Code = StoreType.Code;
            StoreTypeDAO.Name = StoreType.Name;
            StoreTypeDAO.StatusId = StoreType.StatusId;
            StoreTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(StoreType);
            return true;
        }

        public async Task<bool> Delete(StoreType StoreType)
        {
            await DataContext.StoreType.Where(x => x.Id == StoreType.Id).UpdateFromQueryAsync(x => new StoreTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<StoreType> StoreTypes)
        {
            List<StoreTypeDAO> StoreTypeDAOs = new List<StoreTypeDAO>();
            foreach (StoreType StoreType in StoreTypes)
            {
                StoreTypeDAO StoreTypeDAO = new StoreTypeDAO();
                StoreTypeDAO.Id = StoreType.Id;
                StoreTypeDAO.Code = StoreType.Code;
                StoreTypeDAO.Name = StoreType.Name;
                StoreTypeDAO.StatusId = StoreType.StatusId;
                StoreTypeDAO.CreatedAt = StaticParams.DateTimeNow;
                StoreTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
                StoreTypeDAOs.Add(StoreTypeDAO);
            }
            await DataContext.BulkMergeAsync(StoreTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<StoreType> StoreTypes)
        {
            List<long> Ids = StoreTypes.Select(x => x.Id).ToList();
            await DataContext.StoreType
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new StoreTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(StoreType StoreType)
        {
        }

    }
}
