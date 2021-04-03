using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.ABE.Helpers;

namespace DMS.ABE.Repositories
{
    public interface IKpiCriteriaGeneralRepository
    {
        Task<int> Count(KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter);
        Task<List<KpiCriteriaGeneral>> List(KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter);
        Task<KpiCriteriaGeneral> Get(long Id);
    }
    public class KpiCriteriaGeneralRepository : IKpiCriteriaGeneralRepository
    {
        private DataContext DataContext;
        public KpiCriteriaGeneralRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiCriteriaGeneralDAO> DynamicFilter(IQueryable<KpiCriteriaGeneralDAO> query, KpiCriteriaGeneralFilter filter)
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

         private IQueryable<KpiCriteriaGeneralDAO> OrFilter(IQueryable<KpiCriteriaGeneralDAO> query, KpiCriteriaGeneralFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiCriteriaGeneralDAO> initQuery = query.Where(q => false);
            foreach (KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter in filter.OrFilter)
            {
                IQueryable<KpiCriteriaGeneralDAO> queryable = query;
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

        private IQueryable<KpiCriteriaGeneralDAO> DynamicOrder(IQueryable<KpiCriteriaGeneralDAO> query, KpiCriteriaGeneralFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiCriteriaGeneralOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiCriteriaGeneralOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case KpiCriteriaGeneralOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiCriteriaGeneralOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiCriteriaGeneralOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case KpiCriteriaGeneralOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiCriteriaGeneral>> DynamicSelect(IQueryable<KpiCriteriaGeneralDAO> query, KpiCriteriaGeneralFilter filter)
        {
            List<KpiCriteriaGeneral> KpiCriteriaGenerals = await query.Select(q => new KpiCriteriaGeneral()
            {
                Id = filter.Selects.Contains(KpiCriteriaGeneralSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(KpiCriteriaGeneralSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(KpiCriteriaGeneralSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return KpiCriteriaGenerals;
        }

        public async Task<int> Count(KpiCriteriaGeneralFilter filter)
        {
            IQueryable<KpiCriteriaGeneralDAO> KpiCriteriaGenerals = DataContext.KpiCriteriaGeneral.AsNoTracking();
            KpiCriteriaGenerals = DynamicFilter(KpiCriteriaGenerals, filter);
            return await KpiCriteriaGenerals.CountAsync();
        }

        public async Task<List<KpiCriteriaGeneral>> List(KpiCriteriaGeneralFilter filter)
        {
            if (filter == null) return new List<KpiCriteriaGeneral>();
            IQueryable<KpiCriteriaGeneralDAO> KpiCriteriaGeneralDAOs = DataContext.KpiCriteriaGeneral.AsNoTracking();
            KpiCriteriaGeneralDAOs = DynamicFilter(KpiCriteriaGeneralDAOs, filter);
            KpiCriteriaGeneralDAOs = DynamicOrder(KpiCriteriaGeneralDAOs, filter);
            List<KpiCriteriaGeneral> KpiCriteriaGenerals = await DynamicSelect(KpiCriteriaGeneralDAOs, filter);
            return KpiCriteriaGenerals;
        }

        public async Task<KpiCriteriaGeneral> Get(long Id)
        {
            KpiCriteriaGeneral KpiCriteriaGeneral = await DataContext.KpiCriteriaGeneral.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new KpiCriteriaGeneral()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (KpiCriteriaGeneral == null)
                return null;

            return KpiCriteriaGeneral;
        }
    }
}
