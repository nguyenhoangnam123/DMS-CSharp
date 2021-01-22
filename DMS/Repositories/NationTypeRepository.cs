using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface INationTypeRepository
    {
        Task<int> Count(NationTypeFilter NationTypeFilter);
        Task<List<NationType>> List(NationTypeFilter NationTypeFilter);
        Task<NationType> Get(long Id);
    }
    public class NationTypeRepository : INationTypeRepository
    {
        private DataContext DataContext;
        public NationTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<NationTypeDAO> DynamicFilter(IQueryable<NationTypeDAO> query, NationTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<NationTypeDAO> OrFilter(IQueryable<NationTypeDAO> query, NationTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NationTypeDAO> initQuery = query.Where(q => false);
            foreach (NationTypeFilter NationTypeFilter in filter.OrFilter)
            {
                IQueryable<NationTypeDAO> queryable = query;
                if (NationTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, NationTypeFilter.Id);
                if (NationTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, NationTypeFilter.Code);
                if (NationTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, NationTypeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<NationTypeDAO> DynamicOrder(IQueryable<NationTypeDAO> query, NationTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NationTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NationTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case NationTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NationTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NationTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case NationTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<NationType>> DynamicSelect(IQueryable<NationTypeDAO> query, NationTypeFilter filter)
        {
            List<NationType> NationTypees = await query.Select(q => new NationType()
            {
                Id = filter.Selects.Contains(NationTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(NationTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(NationTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return NationTypees;
        }

        public async Task<int> Count(NationTypeFilter filter)
        {
            IQueryable<NationTypeDAO> NationTypes = DataContext.NationType;
            NationTypes = DynamicFilter(NationTypes, filter);
            return await NationTypes.CountAsync();
        }

        public async Task<List<NationType>> List(NationTypeFilter filter)
        {
            if (filter == null) return new List<NationType>();
            IQueryable<NationTypeDAO> NationTypeDAOs = DataContext.NationType;
            NationTypeDAOs = DynamicFilter(NationTypeDAOs, filter);
            NationTypeDAOs = DynamicOrder(NationTypeDAOs, filter);
            List<NationType> NationTypees = await DynamicSelect(NationTypeDAOs, filter);
            return NationTypees;
        }

        public async Task<NationType> Get(long Id)
        {
            NationType NationType = await DataContext.NationType.Where(x => x.Id == Id).Select(x => new NationType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (NationType == null)
                return null;

            return NationType;
        }
    }
}
