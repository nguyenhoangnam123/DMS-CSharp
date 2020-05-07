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
    public interface IPriceListTypeRepository
    {
        Task<int> Count(PriceListTypeFilter PriceListTypeFilter);
        Task<List<PriceListType>> List(PriceListTypeFilter PriceListTypeFilter);
        Task<PriceListType> Get(long Id);
    }
    public class PriceListTypeRepository : IPriceListTypeRepository
    {
        private DataContext DataContext;
        public PriceListTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PriceListTypeDAO> DynamicFilter(IQueryable<PriceListTypeDAO> query, PriceListTypeFilter filter)
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

         private IQueryable<PriceListTypeDAO> OrFilter(IQueryable<PriceListTypeDAO> query, PriceListTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PriceListTypeDAO> initQuery = query.Where(q => false);
            foreach (PriceListTypeFilter PriceListTypeFilter in filter.OrFilter)
            {
                IQueryable<PriceListTypeDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PriceListTypeDAO> DynamicOrder(IQueryable<PriceListTypeDAO> query, PriceListTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PriceListTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PriceListTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PriceListTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PriceListTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PriceListTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PriceListTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PriceListType>> DynamicSelect(IQueryable<PriceListTypeDAO> query, PriceListTypeFilter filter)
        {
            List<PriceListType> PriceListTypes = await query.Select(q => new PriceListType()
            {
                Id = filter.Selects.Contains(PriceListTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PriceListTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PriceListTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return PriceListTypes;
        }

        public async Task<int> Count(PriceListTypeFilter filter)
        {
            IQueryable<PriceListTypeDAO> PriceListTypes = DataContext.PriceListType.AsNoTracking();
            PriceListTypes = DynamicFilter(PriceListTypes, filter);
            return await PriceListTypes.CountAsync();
        }

        public async Task<List<PriceListType>> List(PriceListTypeFilter filter)
        {
            if (filter == null) return new List<PriceListType>();
            IQueryable<PriceListTypeDAO> PriceListTypeDAOs = DataContext.PriceListType.AsNoTracking();
            PriceListTypeDAOs = DynamicFilter(PriceListTypeDAOs, filter);
            PriceListTypeDAOs = DynamicOrder(PriceListTypeDAOs, filter);
            List<PriceListType> PriceListTypes = await DynamicSelect(PriceListTypeDAOs, filter);
            return PriceListTypes;
        }

        public async Task<PriceListType> Get(long Id)
        {
            PriceListType PriceListType = await DataContext.PriceListType.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PriceListType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (PriceListType == null)
                return null;

            return PriceListType;
        }
    }
}
