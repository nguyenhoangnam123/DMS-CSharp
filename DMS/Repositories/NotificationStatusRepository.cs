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
    public interface INotificationStatusRepository
    {
        Task<int> Count(NotificationStatusFilter NotificationStatusFilter);
        Task<List<NotificationStatus>> List(NotificationStatusFilter NotificationStatusFilter);
        Task<NotificationStatus> Get(long Id);
        Task<bool> Create(NotificationStatus NotificationStatus);
        Task<bool> Update(NotificationStatus NotificationStatus);
        Task<bool> Delete(NotificationStatus NotificationStatus);
        Task<bool> BulkMerge(List<NotificationStatus> NotificationStatuses);
        Task<bool> BulkDelete(List<NotificationStatus> NotificationStatuses);
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
        public async Task<bool> Create(NotificationStatus NotificationStatus)
        {
            NotificationStatusDAO NotificationStatusDAO = new NotificationStatusDAO();
            NotificationStatusDAO.Id = NotificationStatus.Id;
            NotificationStatusDAO.Code = NotificationStatus.Code;
            NotificationStatusDAO.Name = NotificationStatus.Name;
            DataContext.NotificationStatus.Add(NotificationStatusDAO);
            await DataContext.SaveChangesAsync();
            NotificationStatus.Id = NotificationStatusDAO.Id;
            await SaveReference(NotificationStatus);
            return true;
        }

        public async Task<bool> Update(NotificationStatus NotificationStatus)
        {
            NotificationStatusDAO NotificationStatusDAO = DataContext.NotificationStatus.Where(x => x.Id == NotificationStatus.Id).FirstOrDefault();
            if (NotificationStatusDAO == null)
                return false;
            NotificationStatusDAO.Id = NotificationStatus.Id;
            NotificationStatusDAO.Code = NotificationStatus.Code;
            NotificationStatusDAO.Name = NotificationStatus.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(NotificationStatus);
            return true;
        }

        public async Task<bool> Delete(NotificationStatus NotificationStatus)
        {
            await DataContext.NotificationStatus.Where(x => x.Id == NotificationStatus.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<NotificationStatus> NotificationStatuses)
        {
            List<NotificationStatusDAO> NotificationStatusDAOs = new List<NotificationStatusDAO>();
            foreach (NotificationStatus NotificationStatus in NotificationStatuses)
            {
                NotificationStatusDAO NotificationStatusDAO = new NotificationStatusDAO();
                NotificationStatusDAO.Id = NotificationStatus.Id;
                NotificationStatusDAO.Code = NotificationStatus.Code;
                NotificationStatusDAO.Name = NotificationStatus.Name;
                NotificationStatusDAOs.Add(NotificationStatusDAO);
            }
            await DataContext.BulkMergeAsync(NotificationStatusDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<NotificationStatus> NotificationStatuses)
        {
            List<long> Ids = NotificationStatuses.Select(x => x.Id).ToList();
            await DataContext.NotificationStatus
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(NotificationStatus NotificationStatus)
        {
        }
        
    }
}
