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
    public interface IProblemTypeRepository
    {
        Task<int> Count(ProblemTypeFilter ProblemTypeFilter);
        Task<List<ProblemType>> List(ProblemTypeFilter ProblemTypeFilter);
        Task<ProblemType> Get(long Id);
    }
    public class ProblemTypeRepository : IProblemTypeRepository
    {
        private DataContext DataContext;
        public ProblemTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProblemTypeDAO> DynamicFilter(IQueryable<ProblemTypeDAO> query, ProblemTypeFilter filter)
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

         private IQueryable<ProblemTypeDAO> OrFilter(IQueryable<ProblemTypeDAO> query, ProblemTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProblemTypeDAO> initQuery = query.Where(q => false);
            foreach (ProblemTypeFilter ProblemTypeFilter in filter.OrFilter)
            {
                IQueryable<ProblemTypeDAO> queryable = query;
                if (ProblemTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ProblemTypeFilter.Id);
                if (ProblemTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ProblemTypeFilter.Code);
                if (ProblemTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ProblemTypeFilter.Name);
                if (ProblemTypeFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, ProblemTypeFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ProblemTypeDAO> DynamicOrder(IQueryable<ProblemTypeDAO> query, ProblemTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProblemTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProblemTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProblemTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProblemTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProblemTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProblemTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProblemTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProblemTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ProblemType>> DynamicSelect(IQueryable<ProblemTypeDAO> query, ProblemTypeFilter filter)
        {
            List<ProblemType> ProblemTypes = await query.Select(q => new ProblemType()
            {
                Id = filter.Selects.Contains(ProblemTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProblemTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProblemTypeSelect.Name) ? q.Name : default(string),
                StatusId = filter.Selects.Contains(ProblemTypeSelect.Status) ? q.StatusId : default(long),
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return ProblemTypes;
        }

        public async Task<int> Count(ProblemTypeFilter filter)
        {
            IQueryable<ProblemTypeDAO> ProblemTypes = DataContext.ProblemType.AsNoTracking();
            ProblemTypes = DynamicFilter(ProblemTypes, filter);
            return await ProblemTypes.CountAsync();
        }

        public async Task<List<ProblemType>> List(ProblemTypeFilter filter)
        {
            if (filter == null) return new List<ProblemType>();
            IQueryable<ProblemTypeDAO> ProblemTypeDAOs = DataContext.ProblemType.AsNoTracking();
            ProblemTypeDAOs = DynamicFilter(ProblemTypeDAOs, filter);
            ProblemTypeDAOs = DynamicOrder(ProblemTypeDAOs, filter);
            List<ProblemType> ProblemTypes = await DynamicSelect(ProblemTypeDAOs, filter);
            return ProblemTypes;
        }

        public async Task<ProblemType> Get(long Id)
        {
            ProblemType ProblemType = await DataContext.ProblemType.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new ProblemType()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
            }).FirstOrDefaultAsync();

            if (ProblemType == null)
                return null;

            return ProblemType;
        }
    }
}
