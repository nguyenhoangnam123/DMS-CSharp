using DMS.Common;
using DMS.Helpers;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IStoreHistoryRepository
    {
        Task<int> Count(StoreHistoryFilter StoreHistoryFilter);
        Task<List<StoreHistory>> List(StoreHistoryFilter StoreHistoryFilter);
        Task<bool> Create(StoreHistory StoreHistory);
    }
    public class StoreHistoryRepository : IStoreHistoryRepository
    {
        private DataContext DataContext;
        public StoreHistoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<StoreHistoryDAO> DynamicFilter(IQueryable<StoreHistoryDAO> query, StoreHistoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.StoreId != null && filter.StoreId.HasValue)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.AppUserId != null && filter.AppUserId.HasValue)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.StoreStatusId != null && filter.StoreStatusId.HasValue)
                query = query.Where(q => q.StoreStatusId, filter.StoreStatusId);
            if (filter.PreviousStoreStatusId != null && filter.PreviousStoreStatusId.HasValue)
                query = query.Where(q => q.PreviousStoreStatusId.HasValue).Where(q => q.PreviousStoreStatusId.Value, filter.PreviousStoreStatusId);
            return query;
        }


        private IQueryable<StoreHistoryDAO> DynamicOrder(IQueryable<StoreHistoryDAO> query, StoreHistoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreHistoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreHistoryOrder.Store:
                            query = query.OrderBy(q => q.StoreId);
                            break;
                        case StoreHistoryOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case StoreHistoryOrder.StoreStatus:
                            query = query.OrderBy(q => q.StoreStatusId);
                            break;
                        case StoreHistoryOrder.PreviousStoreStatus:
                            query = query.OrderBy(q => q.PreviousStoreStatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreHistoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreHistoryOrder.Store:
                            query = query.OrderByDescending(q => q.StoreId);
                            break;
                        case StoreHistoryOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case StoreHistoryOrder.StoreStatus:
                            query = query.OrderByDescending(q => q.StoreStatusId);
                            break;
                        case StoreHistoryOrder.PreviousStoreStatus:
                            query = query.OrderByDescending(q => q.PreviousStoreStatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<StoreHistory>> DynamicSelect(IQueryable<StoreHistoryDAO> query, StoreHistoryFilter filter)
        {
            List<StoreHistory> StoreHistories = await query.Select(q => new StoreHistory()
            {
                Id = filter.Selects.Contains(StoreHistorySelect.Id) ? q.Id : default(long),
                StoreId = filter.Selects.Contains(StoreHistorySelect.Store) ? q.StoreId : default(long),
                AppUserId = filter.Selects.Contains(StoreHistorySelect.AppUser) ? q.AppUserId : default(long),
                StoreStatusId = filter.Selects.Contains(StoreHistorySelect.StoreStatus) ? q.StoreStatusId : default(long),
                AppUser = filter.Selects.Contains(StoreHistorySelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                } : null,
                PreviousStoreStatus = filter.Selects.Contains(StoreHistorySelect.PreviousStoreStatus) && q.StoreStatus != null ? new StoreStatus
                {
                    Id = q.StoreStatus.Id,
                    Code = q.StoreStatus.Code,
                    Name = q.StoreStatus.Name,
                } : null,
                StoreStatus = filter.Selects.Contains(StoreHistorySelect.StoreStatus) && q.StoreStatus != null ? new StoreStatus
                {
                    Id = q.StoreStatus.Id,
                    Code = q.StoreStatus.Code,
                    Name = q.StoreStatus.Name,
                } : null,
            }).ToListAsync();
            return StoreHistories;
        }

        public async Task<int> Count(StoreHistoryFilter filter)
        {
            IQueryable<StoreHistoryDAO> StoreHistories = DataContext.StoreHistory.AsNoTracking();
            StoreHistories = DynamicFilter(StoreHistories, filter);
            return await StoreHistories.CountAsync();
        }

        public async Task<List<StoreHistory>> List(StoreHistoryFilter filter)
        {
            if (filter == null) return new List<StoreHistory>();
            IQueryable<StoreHistoryDAO> StoreHistoryDAOs = DataContext.StoreHistory.AsNoTracking();
            StoreHistoryDAOs = DynamicFilter(StoreHistoryDAOs, filter);
            StoreHistoryDAOs = DynamicOrder(StoreHistoryDAOs, filter);
            List<StoreHistory> StoreHistories = await DynamicSelect(StoreHistoryDAOs, filter);
            return StoreHistories;
        }


        public async Task<bool> Create(StoreHistory StoreHistory)
        {
            StoreHistoryDAO StoreHistoryDAO = new StoreHistoryDAO();
            StoreHistoryDAO.Id = StoreHistory.Id;
            StoreHistoryDAO.StoreId = StoreHistory.StoreId;
            StoreHistoryDAO.AppUserId = StoreHistory.AppUserId;
            StoreHistoryDAO.PreviousStoreStatusId = StoreHistory.PreviousStoreStatusId;
            StoreHistoryDAO.StoreStatusId = StoreHistory.StoreStatusId;
            StoreHistoryDAO.CreatedAt = StaticParams.DateTimeNow;
            DataContext.StoreHistory.Add(StoreHistoryDAO);
            await DataContext.SaveChangesAsync();
            StoreHistory.Id = StoreHistoryDAO.Id;
            return true;
        }

    }
}
