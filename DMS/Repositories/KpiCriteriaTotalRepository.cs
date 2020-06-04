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
    public interface IKpiCriteriaTotalRepository
    {
        Task<int> Count(KpiCriteriaTotalFilter KpiCriteriaTotalFilter);
        Task<List<KpiCriteriaTotal>> List(KpiCriteriaTotalFilter KpiCriteriaTotalFilter);
        Task<KpiCriteriaTotal> Get(long Id);
    }
    public class KpiCriteriaTotalRepository : IKpiCriteriaTotalRepository
    {
        private DataContext DataContext;
        public KpiCriteriaTotalRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiCriteriaTotalDAO> DynamicFilter(IQueryable<KpiCriteriaTotalDAO> query, KpiCriteriaTotalFilter filter)
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

         private IQueryable<KpiCriteriaTotalDAO> OrFilter(IQueryable<KpiCriteriaTotalDAO> query, KpiCriteriaTotalFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiCriteriaTotalDAO> initQuery = query.Where(q => false);
            foreach (KpiCriteriaTotalFilter KpiCriteriaTotalFilter in filter.OrFilter)
            {
                IQueryable<KpiCriteriaTotalDAO> queryable = query;
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

        private IQueryable<KpiCriteriaTotalDAO> DynamicOrder(IQueryable<KpiCriteriaTotalDAO> query, KpiCriteriaTotalFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiCriteriaTotalOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiCriteriaTotalOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case KpiCriteriaTotalOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiCriteriaTotalOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiCriteriaTotalOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case KpiCriteriaTotalOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiCriteriaTotal>> DynamicSelect(IQueryable<KpiCriteriaTotalDAO> query, KpiCriteriaTotalFilter filter)
        {
            List<KpiCriteriaTotal> KpiCriteriaTotals = await query.Select(q => new KpiCriteriaTotal()
            {
                Id = filter.Selects.Contains(KpiCriteriaTotalSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(KpiCriteriaTotalSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(KpiCriteriaTotalSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return KpiCriteriaTotals;
        }

        public async Task<int> Count(KpiCriteriaTotalFilter filter)
        {
            IQueryable<KpiCriteriaTotalDAO> KpiCriteriaTotals = DataContext.KpiCriteriaTotal.AsNoTracking();
            KpiCriteriaTotals = DynamicFilter(KpiCriteriaTotals, filter);
            return await KpiCriteriaTotals.CountAsync();
        }

        public async Task<List<KpiCriteriaTotal>> List(KpiCriteriaTotalFilter filter)
        {
            if (filter == null) return new List<KpiCriteriaTotal>();
            IQueryable<KpiCriteriaTotalDAO> KpiCriteriaTotalDAOs = DataContext.KpiCriteriaTotal.AsNoTracking();
            KpiCriteriaTotalDAOs = DynamicFilter(KpiCriteriaTotalDAOs, filter);
            KpiCriteriaTotalDAOs = DynamicOrder(KpiCriteriaTotalDAOs, filter);
            List<KpiCriteriaTotal> KpiCriteriaTotals = await DynamicSelect(KpiCriteriaTotalDAOs, filter);
            return KpiCriteriaTotals;
        }

        public async Task<KpiCriteriaTotal> Get(long Id)
        {
            KpiCriteriaTotal KpiCriteriaTotal = await DataContext.KpiCriteriaTotal.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new KpiCriteriaTotal()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (KpiCriteriaTotal == null)
                return null;

            return KpiCriteriaTotal;
        }
        public async Task<bool> Create(KpiCriteriaTotal KpiCriteriaTotal)
        {
            KpiCriteriaTotalDAO KpiCriteriaTotalDAO = new KpiCriteriaTotalDAO();
            KpiCriteriaTotalDAO.Id = KpiCriteriaTotal.Id;
            KpiCriteriaTotalDAO.Code = KpiCriteriaTotal.Code;
            KpiCriteriaTotalDAO.Name = KpiCriteriaTotal.Name;
            DataContext.KpiCriteriaTotal.Add(KpiCriteriaTotalDAO);
            await DataContext.SaveChangesAsync();
            KpiCriteriaTotal.Id = KpiCriteriaTotalDAO.Id;
            await SaveReference(KpiCriteriaTotal);
            return true;
        }

        public async Task<bool> Update(KpiCriteriaTotal KpiCriteriaTotal)
        {
            KpiCriteriaTotalDAO KpiCriteriaTotalDAO = DataContext.KpiCriteriaTotal.Where(x => x.Id == KpiCriteriaTotal.Id).FirstOrDefault();
            if (KpiCriteriaTotalDAO == null)
                return false;
            KpiCriteriaTotalDAO.Id = KpiCriteriaTotal.Id;
            KpiCriteriaTotalDAO.Code = KpiCriteriaTotal.Code;
            KpiCriteriaTotalDAO.Name = KpiCriteriaTotal.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(KpiCriteriaTotal);
            return true;
        }

        public async Task<bool> Delete(KpiCriteriaTotal KpiCriteriaTotal)
        {
            await DataContext.KpiCriteriaTotal.Where(x => x.Id == KpiCriteriaTotal.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<KpiCriteriaTotal> KpiCriteriaTotals)
        {
            List<KpiCriteriaTotalDAO> KpiCriteriaTotalDAOs = new List<KpiCriteriaTotalDAO>();
            foreach (KpiCriteriaTotal KpiCriteriaTotal in KpiCriteriaTotals)
            {
                KpiCriteriaTotalDAO KpiCriteriaTotalDAO = new KpiCriteriaTotalDAO();
                KpiCriteriaTotalDAO.Id = KpiCriteriaTotal.Id;
                KpiCriteriaTotalDAO.Code = KpiCriteriaTotal.Code;
                KpiCriteriaTotalDAO.Name = KpiCriteriaTotal.Name;
                KpiCriteriaTotalDAOs.Add(KpiCriteriaTotalDAO);
            }
            await DataContext.BulkMergeAsync(KpiCriteriaTotalDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<KpiCriteriaTotal> KpiCriteriaTotals)
        {
            List<long> Ids = KpiCriteriaTotals.Select(x => x.Id).ToList();
            await DataContext.KpiCriteriaTotal
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(KpiCriteriaTotal KpiCriteriaTotal)
        {
        }
        
    }
}
