using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Helpers;

namespace DMS.Repositories
{
    public interface IStoreScoutingTypeRepository
    {
        Task<int> Count(StoreScoutingTypeFilter StoreScoutingTypeFilter);
        Task<List<StoreScoutingType>> List(StoreScoutingTypeFilter StoreScoutingTypeFilter);
        Task<StoreScoutingType> Get(long Id);
        Task<bool> Create(StoreScoutingType StoreScoutingType);
        Task<bool> Update(StoreScoutingType StoreScoutingType);
        Task<bool> Delete(StoreScoutingType StoreScoutingType);
        Task<bool> BulkMerge(List<StoreScoutingType> StoreScoutingTypes);
        Task<bool> BulkDelete(List<StoreScoutingType> StoreScoutingTypes);
    }
    public class StoreScoutingTypeRepository : IStoreScoutingTypeRepository
    {
        private DataContext DataContext;
        public StoreScoutingTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreScoutingTypeDAO> DynamicFilter(IQueryable<StoreScoutingTypeDAO> query, StoreScoutingTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
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

         private IQueryable<StoreScoutingTypeDAO> OrFilter(IQueryable<StoreScoutingTypeDAO> query, StoreScoutingTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreScoutingTypeDAO> initQuery = query.Where(q => false);
            foreach (StoreScoutingTypeFilter StoreScoutingTypeFilter in filter.OrFilter)
            {
                IQueryable<StoreScoutingTypeDAO> queryable = query;
                if (StoreScoutingTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, StoreScoutingTypeFilter.Id);
                if (StoreScoutingTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, StoreScoutingTypeFilter.Code);
                if (StoreScoutingTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, StoreScoutingTypeFilter.Name);
                if (StoreScoutingTypeFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, StoreScoutingTypeFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<StoreScoutingTypeDAO> DynamicOrder(IQueryable<StoreScoutingTypeDAO> query, StoreScoutingTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreScoutingTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreScoutingTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StoreScoutingTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case StoreScoutingTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreScoutingTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreScoutingTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StoreScoutingTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case StoreScoutingTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<StoreScoutingType>> DynamicSelect(IQueryable<StoreScoutingTypeDAO> query, StoreScoutingTypeFilter filter)
        {
            List<StoreScoutingType> StoreScoutingTypes = await query.Select(q => new StoreScoutingType()
            {
                Id = filter.Selects.Contains(StoreScoutingTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StoreScoutingTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(StoreScoutingTypeSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(StoreScoutingTypeSelect.Status) ? q.StatusId : default(long),
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                Used = q.Used,
            }).ToListAsync();
            return StoreScoutingTypes;
        }

        public async Task<int> Count(StoreScoutingTypeFilter filter)
        {
            IQueryable<StoreScoutingTypeDAO> StoreScoutingTypes = DataContext.StoreScoutingType.AsNoTracking();
            StoreScoutingTypes = DynamicFilter(StoreScoutingTypes, filter);
            return await StoreScoutingTypes.CountAsync();
        }

        public async Task<List<StoreScoutingType>> List(StoreScoutingTypeFilter filter)
        {
            if (filter == null) return new List<StoreScoutingType>();
            IQueryable<StoreScoutingTypeDAO> StoreScoutingTypeDAOs = DataContext.StoreScoutingType.AsNoTracking();
            StoreScoutingTypeDAOs = DynamicFilter(StoreScoutingTypeDAOs, filter);
            StoreScoutingTypeDAOs = DynamicOrder(StoreScoutingTypeDAOs, filter);
            List<StoreScoutingType> StoreScoutingTypes = await DynamicSelect(StoreScoutingTypeDAOs, filter);
            return StoreScoutingTypes;
        }

        public async Task<StoreScoutingType> Get(long Id)
        {
            StoreScoutingType StoreScoutingType = await DataContext.StoreScoutingType.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new StoreScoutingType()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Used = x.Used,
                StatusId = x.StatusId,
            }).FirstOrDefaultAsync();

            if (StoreScoutingType == null)
                return null;

            return StoreScoutingType;
        }
        public async Task<bool> Create(StoreScoutingType StoreScoutingType)
        {
            StoreScoutingTypeDAO StoreScoutingTypeDAO = new StoreScoutingTypeDAO();
            StoreScoutingTypeDAO.Id = StoreScoutingType.Id;
            StoreScoutingTypeDAO.Code = StoreScoutingType.Code;
            StoreScoutingTypeDAO.Name = StoreScoutingType.Name;
            StoreScoutingTypeDAO.StatusId = StoreScoutingType.StatusId;
            StoreScoutingTypeDAO.CreatedAt = StaticParams.DateTimeNow;
            StoreScoutingTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.StoreScoutingType.Add(StoreScoutingTypeDAO);
            await DataContext.SaveChangesAsync();
            StoreScoutingType.Id = StoreScoutingTypeDAO.Id;
            await SaveReference(StoreScoutingType);
            return true;
        }

        public async Task<bool> Update(StoreScoutingType StoreScoutingType)
        {
            StoreScoutingTypeDAO StoreScoutingTypeDAO = DataContext.StoreScoutingType.Where(x => x.Id == StoreScoutingType.Id).FirstOrDefault();
            if (StoreScoutingTypeDAO == null)
                return false;
            StoreScoutingTypeDAO.Id = StoreScoutingType.Id;
            StoreScoutingTypeDAO.Code = StoreScoutingType.Code;
            StoreScoutingTypeDAO.Name = StoreScoutingType.Name;
            StoreScoutingTypeDAO.StatusId = StoreScoutingType.StatusId;
            StoreScoutingTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(StoreScoutingType);
            return true;
        }

        public async Task<bool> Delete(StoreScoutingType StoreScoutingType)
        {
            await DataContext.StoreScoutingType.Where(x => x.Id == StoreScoutingType.Id).UpdateFromQueryAsync(x => new StoreScoutingTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<StoreScoutingType> StoreScoutingTypes)
        {
            List<StoreScoutingTypeDAO> StoreScoutingTypeDAOs = new List<StoreScoutingTypeDAO>();
            foreach (StoreScoutingType StoreScoutingType in StoreScoutingTypes)
            {
                StoreScoutingTypeDAO StoreScoutingTypeDAO = new StoreScoutingTypeDAO();
                StoreScoutingTypeDAO.Id = StoreScoutingType.Id;
                StoreScoutingTypeDAO.Code = StoreScoutingType.Code;
                StoreScoutingTypeDAO.Name = StoreScoutingType.Name;
                StoreScoutingTypeDAO.StatusId = StoreScoutingType.StatusId;
                StoreScoutingTypeDAO.CreatedAt = StaticParams.DateTimeNow;
                StoreScoutingTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
                StoreScoutingTypeDAOs.Add(StoreScoutingTypeDAO);
            }
            await DataContext.BulkMergeAsync(StoreScoutingTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<StoreScoutingType> StoreScoutingTypes)
        {
            List<long> Ids = StoreScoutingTypes.Select(x => x.Id).ToList();
            await DataContext.StoreScoutingType
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new StoreScoutingTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(StoreScoutingType StoreScoutingType)
        {
        }
        
    }
}
