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
    public interface IIndirectSalesOrderStatusRepository
    {
        Task<int> Count(IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter);
        Task<List<IndirectSalesOrderStatus>> List(IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter);
        Task<IndirectSalesOrderStatus> Get(long Id);
    }
    public class IndirectSalesOrderStatusRepository : IIndirectSalesOrderStatusRepository
    {
        private DataContext DataContext;
        public IndirectSalesOrderStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<IndirectSalesOrderStatusDAO> DynamicFilter(IQueryable<IndirectSalesOrderStatusDAO> query, IndirectSalesOrderStatusFilter filter)
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

         private IQueryable<IndirectSalesOrderStatusDAO> OrFilter(IQueryable<IndirectSalesOrderStatusDAO> query, IndirectSalesOrderStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<IndirectSalesOrderStatusDAO> initQuery = query.Where(q => false);
            foreach (IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter in filter.OrFilter)
            {
                IQueryable<IndirectSalesOrderStatusDAO> queryable = query;
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

        private IQueryable<IndirectSalesOrderStatusDAO> DynamicOrder(IQueryable<IndirectSalesOrderStatusDAO> query, IndirectSalesOrderStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case IndirectSalesOrderStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case IndirectSalesOrderStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case IndirectSalesOrderStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case IndirectSalesOrderStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case IndirectSalesOrderStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case IndirectSalesOrderStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<IndirectSalesOrderStatus>> DynamicSelect(IQueryable<IndirectSalesOrderStatusDAO> query, IndirectSalesOrderStatusFilter filter)
        {
            List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await query.Select(q => new IndirectSalesOrderStatus()
            {
                Id = filter.Selects.Contains(IndirectSalesOrderStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(IndirectSalesOrderStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(IndirectSalesOrderStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return IndirectSalesOrderStatuses;
        }

        public async Task<int> Count(IndirectSalesOrderStatusFilter filter)
        {
            IQueryable<IndirectSalesOrderStatusDAO> IndirectSalesOrderStatuses = DataContext.IndirectSalesOrderStatus.AsNoTracking();
            IndirectSalesOrderStatuses = DynamicFilter(IndirectSalesOrderStatuses, filter);
            return await IndirectSalesOrderStatuses.CountAsync();
        }

        public async Task<List<IndirectSalesOrderStatus>> List(IndirectSalesOrderStatusFilter filter)
        {
            if (filter == null) return new List<IndirectSalesOrderStatus>();
            IQueryable<IndirectSalesOrderStatusDAO> IndirectSalesOrderStatusDAOs = DataContext.IndirectSalesOrderStatus.AsNoTracking();
            IndirectSalesOrderStatusDAOs = DynamicFilter(IndirectSalesOrderStatusDAOs, filter);
            IndirectSalesOrderStatusDAOs = DynamicOrder(IndirectSalesOrderStatusDAOs, filter);
            List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await DynamicSelect(IndirectSalesOrderStatusDAOs, filter);
            return IndirectSalesOrderStatuses;
        }

        public async Task<IndirectSalesOrderStatus> Get(long Id)
        {
            IndirectSalesOrderStatus IndirectSalesOrderStatus = await DataContext.IndirectSalesOrderStatus.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new IndirectSalesOrderStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (IndirectSalesOrderStatus == null)
                return null;

            return IndirectSalesOrderStatus;
        }
    }
}
