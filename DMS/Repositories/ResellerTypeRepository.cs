using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace DMS.Repositories
{
    public interface IResellerTypeRepository
    {
        Task<int> Count(ResellerTypeFilter ResellerTypeFilter);
        Task<List<ResellerType>> List(ResellerTypeFilter ResellerTypeFilter);
        Task<ResellerType> Get(long Id);
        Task<bool> Create(ResellerType ResellerType);
        Task<bool> Update(ResellerType ResellerType);
        Task<bool> Delete(ResellerType ResellerType);
        Task<bool> BulkMerge(List<ResellerType> ResellerTypes);
        Task<bool> BulkDelete(List<ResellerType> ResellerTypes);
    }
    public class ResellerTypeRepository : IResellerTypeRepository
    {
        private DataContext DataContext;
        public ResellerTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ResellerTypeDAO> DynamicFilter(IQueryable<ResellerTypeDAO> query, ResellerTypeFilter filter)
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

         private IQueryable<ResellerTypeDAO> OrFilter(IQueryable<ResellerTypeDAO> query, ResellerTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ResellerTypeDAO> initQuery = query.Where(q => false);
            foreach (ResellerTypeFilter ResellerTypeFilter in filter.OrFilter)
            {
                IQueryable<ResellerTypeDAO> queryable = query;
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

        private IQueryable<ResellerTypeDAO> DynamicOrder(IQueryable<ResellerTypeDAO> query, ResellerTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ResellerTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ResellerTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ResellerTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ResellerTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ResellerTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ResellerTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ResellerTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ResellerTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ResellerType>> DynamicSelect(IQueryable<ResellerTypeDAO> query, ResellerTypeFilter filter)
        {
            List<ResellerType> ResellerTypes = await query.Select(q => new ResellerType()
            {
                Id = filter.Selects.Contains(ResellerTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ResellerTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ResellerTypeSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(ResellerTypeSelect.Status) ? q.StatusId : default(long),
            }).AsNoTracking().ToListAsync();
            return ResellerTypes;
        }

        public async Task<int> Count(ResellerTypeFilter filter)
        {
            IQueryable<ResellerTypeDAO> ResellerTypes = DataContext.ResellerType;
            ResellerTypes = DynamicFilter(ResellerTypes, filter);
            return await ResellerTypes.CountAsync();
        }

        public async Task<List<ResellerType>> List(ResellerTypeFilter filter)
        {
            if (filter == null) return new List<ResellerType>();
            IQueryable<ResellerTypeDAO> ResellerTypeDAOs = DataContext.ResellerType;
            ResellerTypeDAOs = DynamicFilter(ResellerTypeDAOs, filter);
            ResellerTypeDAOs = DynamicOrder(ResellerTypeDAOs, filter);
            List<ResellerType> ResellerTypes = await DynamicSelect(ResellerTypeDAOs, filter);
            return ResellerTypes;
        }

        public async Task<ResellerType> Get(long Id)
        {
            ResellerType ResellerType = await DataContext.ResellerType.Where(x => x.Id == Id).Select(x => new ResellerType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (ResellerType == null)
                return null;

            return ResellerType;
        }
        public async Task<bool> Create(ResellerType ResellerType)
        {
            ResellerTypeDAO ResellerTypeDAO = new ResellerTypeDAO();
            ResellerTypeDAO.Id = ResellerType.Id;
            ResellerTypeDAO.Code = ResellerType.Code;
            ResellerTypeDAO.Name = ResellerType.Name;
            ResellerTypeDAO.StatusId = ResellerType.StatusId;
            ResellerTypeDAO.CreatedAt = StaticParams.DateTimeNow;
            ResellerTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ResellerType.Add(ResellerTypeDAO);
            await DataContext.SaveChangesAsync();
            ResellerType.Id = ResellerTypeDAO.Id;
            await SaveReference(ResellerType);
            return true;
        }

        public async Task<bool> Update(ResellerType ResellerType)
        {
            ResellerTypeDAO ResellerTypeDAO = DataContext.ResellerType.Where(x => x.Id == ResellerType.Id).FirstOrDefault();
            if (ResellerTypeDAO == null)
                return false;
            ResellerTypeDAO.Id = ResellerType.Id;
            ResellerTypeDAO.Code = ResellerType.Code;
            ResellerTypeDAO.Name = ResellerType.Name;
            ResellerTypeDAO.StatusId = ResellerType.StatusId;
            ResellerTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ResellerType);
            return true;
        }

        public async Task<bool> Delete(ResellerType ResellerType)
        {
            await DataContext.ResellerType.Where(x => x.Id == ResellerType.Id).UpdateFromQueryAsync(x => new ResellerTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ResellerType> ResellerTypes)
        {
            List<ResellerTypeDAO> ResellerTypeDAOs = new List<ResellerTypeDAO>();
            foreach (ResellerType ResellerType in ResellerTypes)
            {
                ResellerTypeDAO ResellerTypeDAO = new ResellerTypeDAO();
                ResellerTypeDAO.Id = ResellerType.Id;
                ResellerTypeDAO.Code = ResellerType.Code;
                ResellerTypeDAO.Name = ResellerType.Name;
                ResellerTypeDAO.StatusId = ResellerType.StatusId;
                ResellerTypeDAO.CreatedAt = StaticParams.DateTimeNow;
                ResellerTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
                ResellerTypeDAOs.Add(ResellerTypeDAO);
            }
            await DataContext.BulkMergeAsync(ResellerTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ResellerType> ResellerTypes)
        {
            List<long> Ids = ResellerTypes.Select(x => x.Id).ToList();
            await DataContext.ResellerType
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ResellerTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ResellerType ResellerType)
        {
        }
        
    }
}
