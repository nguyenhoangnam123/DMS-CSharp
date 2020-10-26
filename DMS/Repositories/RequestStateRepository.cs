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
    public interface IRequestStateRepository
    {
        Task<int> Count(RequestStateFilter RequestStateFilter);
        Task<List<RequestState>> List(RequestStateFilter RequestStateFilter);
    }
    public class RequestStateRepository : IRequestStateRepository
    {
        private DataContext DataContext;
        public RequestStateRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<RequestStateDAO> DynamicFilter(IQueryable<RequestStateDAO> query, RequestStateFilter filter)
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

        private IQueryable<RequestStateDAO> OrFilter(IQueryable<RequestStateDAO> query, RequestStateFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<RequestStateDAO> initQuery = query.Where(q => false);
            foreach (RequestStateFilter RequestStateFilter in filter.OrFilter)
            {
                IQueryable<RequestStateDAO> queryable = query;
                if (RequestStateFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, RequestStateFilter.Id);
                if (RequestStateFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, RequestStateFilter.Code);
                if (RequestStateFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, RequestStateFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<RequestStateDAO> DynamicOrder(IQueryable<RequestStateDAO> query, RequestStateFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case RequestStateOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case RequestStateOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case RequestStateOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case RequestStateOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case RequestStateOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case RequestStateOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<RequestState>> DynamicSelect(IQueryable<RequestStateDAO> query, RequestStateFilter filter)
        {
            List<RequestState> RequestStates = await query.Select(q => new RequestState()
            {
                Id = filter.Selects.Contains(RequestStateSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(RequestStateSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(RequestStateSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return RequestStates;
        }

        public async Task<int> Count(RequestStateFilter filter)
        {
            IQueryable<RequestStateDAO> RequestStates = DataContext.RequestState.AsNoTracking();
            RequestStates = DynamicFilter(RequestStates, filter);
            return await RequestStates.CountAsync();
        }

        public async Task<List<RequestState>> List(RequestStateFilter filter)
        {
            if (filter == null) return new List<RequestState>();
            IQueryable<RequestStateDAO> RequestStateDAOs = DataContext.RequestState.AsNoTracking();
            RequestStateDAOs = DynamicFilter(RequestStateDAOs, filter);
            RequestStateDAOs = DynamicOrder(RequestStateDAOs, filter);
            List<RequestState> RequestStates = await DynamicSelect(RequestStateDAOs, filter);
            return RequestStates;
        }

    }
}
