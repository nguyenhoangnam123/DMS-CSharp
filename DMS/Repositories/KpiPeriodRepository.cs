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
    public interface IKpiPeriodRepository
    {
        Task<int> Count(KpiPeriodFilter KpiPeriodFilter);
        Task<List<KpiPeriod>> List(KpiPeriodFilter KpiPeriodFilter);
        Task<KpiPeriod> Get(long Id);
    }
    public class KpiPeriodRepository : IKpiPeriodRepository
    {
        private DataContext DataContext;
        public KpiPeriodRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiPeriodDAO> DynamicFilter(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
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

        private IQueryable<KpiPeriodDAO> OrFilter(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiPeriodDAO> initQuery = query.Where(q => false);
            foreach (KpiPeriodFilter KpiPeriodFilter in filter.OrFilter)
            {
                IQueryable<KpiPeriodDAO> queryable = query;
                if (KpiPeriodFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, KpiPeriodFilter.Id);
                if (KpiPeriodFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, KpiPeriodFilter.Code);
                if (KpiPeriodFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, KpiPeriodFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<KpiPeriodDAO> DynamicOrder(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiPeriodOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiPeriodOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case KpiPeriodOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiPeriodOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiPeriodOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case KpiPeriodOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiPeriod>> DynamicSelect(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
        {
            List<KpiPeriod> KpiPeriods = await query.Select(q => new KpiPeriod()
            {
                Id = filter.Selects.Contains(KpiPeriodSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(KpiPeriodSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(KpiPeriodSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return KpiPeriods;
        }

        public async Task<int> Count(KpiPeriodFilter filter)
        {
            IQueryable<KpiPeriodDAO> KpiPeriods = DataContext.KpiPeriod.AsNoTracking();
            KpiPeriods = DynamicFilter(KpiPeriods, filter);
            return await KpiPeriods.CountAsync();
        }

        public async Task<List<KpiPeriod>> List(KpiPeriodFilter filter)
        {
            if (filter == null) return new List<KpiPeriod>();
            IQueryable<KpiPeriodDAO> KpiPeriodDAOs = DataContext.KpiPeriod.AsNoTracking();
            KpiPeriodDAOs = DynamicFilter(KpiPeriodDAOs, filter);
            KpiPeriodDAOs = DynamicOrder(KpiPeriodDAOs, filter);
            List<KpiPeriod> KpiPeriods = await DynamicSelect(KpiPeriodDAOs, filter);
            return KpiPeriods;
        }

        public async Task<KpiPeriod> Get(long Id)
        {
            KpiPeriod KpiPeriod = await DataContext.KpiPeriod.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new KpiPeriod()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();
            return KpiPeriod;
        }
    }
}
