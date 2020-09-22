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
    public interface IPromotionPolicyRepository
    {
        Task<int> Count(PromotionPolicyFilter PromotionPolicyFilter);
        Task<List<PromotionPolicy>> List(PromotionPolicyFilter PromotionPolicyFilter);
        Task<PromotionPolicy> Get(long Id);
    }
    public class PromotionPolicyRepository : IPromotionPolicyRepository
    {
        private DataContext DataContext;
        public PromotionPolicyRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PromotionPolicyDAO> DynamicFilter(IQueryable<PromotionPolicyDAO> query, PromotionPolicyFilter filter)
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

        private IQueryable<PromotionPolicyDAO> OrFilter(IQueryable<PromotionPolicyDAO> query, PromotionPolicyFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PromotionPolicyDAO> initQuery = query.Where(q => false);
            foreach (PromotionPolicyFilter PromotionPolicyFilter in filter.OrFilter)
            {
                IQueryable<PromotionPolicyDAO> queryable = query;
                if (PromotionPolicyFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PromotionPolicyFilter.Id);
                if (PromotionPolicyFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, PromotionPolicyFilter.Code);
                if (PromotionPolicyFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PromotionPolicyFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PromotionPolicyDAO> DynamicOrder(IQueryable<PromotionPolicyDAO> query, PromotionPolicyFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PromotionPolicyOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PromotionPolicyOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PromotionPolicyOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PromotionPolicyOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PromotionPolicyOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PromotionPolicyOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PromotionPolicy>> DynamicSelect(IQueryable<PromotionPolicyDAO> query, PromotionPolicyFilter filter)
        {
            List<PromotionPolicy> PromotionPolicies = await query.Select(q => new PromotionPolicy()
            {
                Id = filter.Selects.Contains(PromotionPolicySelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PromotionPolicySelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PromotionPolicySelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return PromotionPolicies;
        }

        public async Task<int> Count(PromotionPolicyFilter filter)
        {
            IQueryable<PromotionPolicyDAO> PromotionPolicies = DataContext.PromotionPolicy.AsNoTracking();
            PromotionPolicies = DynamicFilter(PromotionPolicies, filter);
            return await PromotionPolicies.CountAsync();
        }

        public async Task<List<PromotionPolicy>> List(PromotionPolicyFilter filter)
        {
            if (filter == null) return new List<PromotionPolicy>();
            IQueryable<PromotionPolicyDAO> PromotionPolicyDAOs = DataContext.PromotionPolicy.AsNoTracking();
            PromotionPolicyDAOs = DynamicFilter(PromotionPolicyDAOs, filter);
            PromotionPolicyDAOs = DynamicOrder(PromotionPolicyDAOs, filter);
            List<PromotionPolicy> PromotionPolicies = await DynamicSelect(PromotionPolicyDAOs, filter);
            return PromotionPolicies;
        }

        public async Task<PromotionPolicy> Get(long Id)
        {
            PromotionPolicy PromotionPolicy = await DataContext.PromotionPolicy.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PromotionPolicy()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (PromotionPolicy == null)
                return null;

            return PromotionPolicy;
        }
    }
}
