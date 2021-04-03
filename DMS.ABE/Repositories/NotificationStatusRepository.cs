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
    public interface INotificationStatusRepository
    {
        Task<int> Count(NotificationStatusFilter NotificationStatusFilter);
        Task<List<NotificationStatus>> List(NotificationStatusFilter NotificationStatusFilter);
        Task<NotificationStatus> Get(long Id);
    }
    public class NotificationStatusRepository : INotificationStatusRepository
    {
        private DataContext DataContext;
        public NotificationStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<NotificationStatusDAO> DynamicFilter(IQueryable<NotificationStatusDAO> query, NotificationStatusFilter filter)
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

         private IQueryable<NotificationStatusDAO> OrFilter(IQueryable<NotificationStatusDAO> query, NotificationStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<NotificationStatusDAO> initQuery = query.Where(q => false);
            foreach (NotificationStatusFilter NotificationStatusFilter in filter.OrFilter)
            {
                IQueryable<NotificationStatusDAO> queryable = query;
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

        private IQueryable<NotificationStatusDAO> DynamicOrder(IQueryable<NotificationStatusDAO> query, NotificationStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case NotificationStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case NotificationStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case NotificationStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case NotificationStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case NotificationStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case NotificationStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<NotificationStatus>> DynamicSelect(IQueryable<NotificationStatusDAO> query, NotificationStatusFilter filter)
        {
            List<NotificationStatus> NotificationStatuses = await query.Select(q => new NotificationStatus()
            {
                Id = filter.Selects.Contains(NotificationStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(NotificationStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(NotificationStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return NotificationStatuses;
        }

        public async Task<int> Count(NotificationStatusFilter filter)
        {
            IQueryable<NotificationStatusDAO> NotificationStatuses = DataContext.NotificationStatus.AsNoTracking();
            NotificationStatuses = DynamicFilter(NotificationStatuses, filter);
            return await NotificationStatuses.CountAsync();
        }

        public async Task<List<NotificationStatus>> List(NotificationStatusFilter filter)
        {
            if (filter == null) return new List<NotificationStatus>();
            IQueryable<NotificationStatusDAO> NotificationStatusDAOs = DataContext.NotificationStatus.AsNoTracking();
            NotificationStatusDAOs = DynamicFilter(NotificationStatusDAOs, filter);
            NotificationStatusDAOs = DynamicOrder(NotificationStatusDAOs, filter);
            List<NotificationStatus> NotificationStatuses = await DynamicSelect(NotificationStatusDAOs, filter);
            return NotificationStatuses;
        }

        public async Task<NotificationStatus> Get(long Id)
        {
            NotificationStatus NotificationStatus = await DataContext.NotificationStatus.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new NotificationStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (NotificationStatus == null)
                return null;

            return NotificationStatus;
        }
        
    }
}
