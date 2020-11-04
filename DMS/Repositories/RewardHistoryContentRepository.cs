using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Helpers;

namespace DMS.Repositories
{
    public interface IRewardHistoryContentRepository
    {
        Task<int> Count(RewardHistoryContentFilter RewardHistoryContentFilter);
        Task<List<RewardHistoryContent>> List(RewardHistoryContentFilter RewardHistoryContentFilter);
        Task<RewardHistoryContent> Get(long Id);
        Task<bool> Create(RewardHistoryContent RewardHistoryContent);
        Task<bool> Update(RewardHistoryContent RewardHistoryContent);
        Task<bool> Delete(RewardHistoryContent RewardHistoryContent);
        Task<bool> BulkMerge(List<RewardHistoryContent> RewardHistoryContents);
        Task<bool> BulkDelete(List<RewardHistoryContent> RewardHistoryContents);
    }
    public class RewardHistoryContentRepository : IRewardHistoryContentRepository
    {
        private DataContext DataContext;
        public RewardHistoryContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<RewardHistoryContentDAO> DynamicFilter(IQueryable<RewardHistoryContentDAO> query, RewardHistoryContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.RewardHistoryId != null)
                query = query.Where(q => q.RewardHistoryId, filter.RewardHistoryId);
            if (filter.LuckeyNumberId != null)
                query = query.Where(q => q.LuckeyNumberId, filter.LuckeyNumberId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<RewardHistoryContentDAO> OrFilter(IQueryable<RewardHistoryContentDAO> query, RewardHistoryContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<RewardHistoryContentDAO> initQuery = query.Where(q => false);
            foreach (RewardHistoryContentFilter RewardHistoryContentFilter in filter.OrFilter)
            {
                IQueryable<RewardHistoryContentDAO> queryable = query;
                if (RewardHistoryContentFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, RewardHistoryContentFilter.Id);
                if (RewardHistoryContentFilter.RewardHistoryId != null)
                    queryable = queryable.Where(q => q.RewardHistoryId, RewardHistoryContentFilter.RewardHistoryId);
                if (RewardHistoryContentFilter.LuckeyNumberId != null)
                    queryable = queryable.Where(q => q.LuckeyNumberId, RewardHistoryContentFilter.LuckeyNumberId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<RewardHistoryContentDAO> DynamicOrder(IQueryable<RewardHistoryContentDAO> query, RewardHistoryContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case RewardHistoryContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case RewardHistoryContentOrder.RewardHistory:
                            query = query.OrderBy(q => q.RewardHistoryId);
                            break;
                        case RewardHistoryContentOrder.LuckeyNumber:
                            query = query.OrderBy(q => q.LuckeyNumberId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case RewardHistoryContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case RewardHistoryContentOrder.RewardHistory:
                            query = query.OrderByDescending(q => q.RewardHistoryId);
                            break;
                        case RewardHistoryContentOrder.LuckeyNumber:
                            query = query.OrderByDescending(q => q.LuckeyNumberId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<RewardHistoryContent>> DynamicSelect(IQueryable<RewardHistoryContentDAO> query, RewardHistoryContentFilter filter)
        {
            List<RewardHistoryContent> RewardHistoryContents = await query.Select(q => new RewardHistoryContent()
            {
                Id = filter.Selects.Contains(RewardHistoryContentSelect.Id) ? q.Id : default(long),
                RewardHistoryId = filter.Selects.Contains(RewardHistoryContentSelect.RewardHistory) ? q.RewardHistoryId : default(long),
                LuckeyNumberId = filter.Selects.Contains(RewardHistoryContentSelect.LuckeyNumber) ? q.LuckeyNumberId : default(long),
                LuckeyNumber = filter.Selects.Contains(RewardHistoryContentSelect.LuckeyNumber) && q.LuckeyNumber != null ? new LuckyNumber
                {
                    Id = q.LuckeyNumber.Id,
                    Code = q.LuckeyNumber.Code,
                    Name = q.LuckeyNumber.Name,
                    RewardStatusId = q.LuckeyNumber.RewardStatusId,
                    RowId = q.LuckeyNumber.RowId,
                } : null,
                RewardHistory = filter.Selects.Contains(RewardHistoryContentSelect.RewardHistory) && q.RewardHistory != null ? new RewardHistory
                {
                    Id = q.RewardHistory.Id,
                    AppUserId = q.RewardHistory.AppUserId,
                    StoreId = q.RewardHistory.StoreId,
                    TurnCounter = q.RewardHistory.TurnCounter,
                    RowId = q.RewardHistory.RowId,
                } : null,
            }).ToListAsync();
            return RewardHistoryContents;
        }

        public async Task<int> Count(RewardHistoryContentFilter filter)
        {
            IQueryable<RewardHistoryContentDAO> RewardHistoryContents = DataContext.RewardHistoryContent.AsNoTracking();
            RewardHistoryContents = DynamicFilter(RewardHistoryContents, filter);
            return await RewardHistoryContents.CountAsync();
        }

        public async Task<List<RewardHistoryContent>> List(RewardHistoryContentFilter filter)
        {
            if (filter == null) return new List<RewardHistoryContent>();
            IQueryable<RewardHistoryContentDAO> RewardHistoryContentDAOs = DataContext.RewardHistoryContent.AsNoTracking();
            RewardHistoryContentDAOs = DynamicFilter(RewardHistoryContentDAOs, filter);
            RewardHistoryContentDAOs = DynamicOrder(RewardHistoryContentDAOs, filter);
            List<RewardHistoryContent> RewardHistoryContents = await DynamicSelect(RewardHistoryContentDAOs, filter);
            return RewardHistoryContents;
        }

        public async Task<RewardHistoryContent> Get(long Id)
        {
            RewardHistoryContent RewardHistoryContent = await DataContext.RewardHistoryContent.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new RewardHistoryContent()
            {
                Id = x.Id,
                RewardHistoryId = x.RewardHistoryId,
                LuckeyNumberId = x.LuckeyNumberId,
                LuckeyNumber = x.LuckeyNumber == null ? null : new LuckyNumber
                {
                    Id = x.LuckeyNumber.Id,
                    Code = x.LuckeyNumber.Code,
                    Name = x.LuckeyNumber.Name,
                    RewardStatusId = x.LuckeyNumber.RewardStatusId,
                    RowId = x.LuckeyNumber.RowId,
                },
                RewardHistory = x.RewardHistory == null ? null : new RewardHistory
                {
                    Id = x.RewardHistory.Id,
                    AppUserId = x.RewardHistory.AppUserId,
                    StoreId = x.RewardHistory.StoreId,
                    TurnCounter = x.RewardHistory.TurnCounter,
                    RowId = x.RewardHistory.RowId,
                },
            }).FirstOrDefaultAsync();

            if (RewardHistoryContent == null)
                return null;

            return RewardHistoryContent;
        }
        public async Task<bool> Create(RewardHistoryContent RewardHistoryContent)
        {
            RewardHistoryContentDAO RewardHistoryContentDAO = new RewardHistoryContentDAO();
            RewardHistoryContentDAO.Id = RewardHistoryContent.Id;
            RewardHistoryContentDAO.RewardHistoryId = RewardHistoryContent.RewardHistoryId;
            RewardHistoryContentDAO.LuckeyNumberId = RewardHistoryContent.LuckeyNumberId;
            DataContext.RewardHistoryContent.Add(RewardHistoryContentDAO);
            await DataContext.SaveChangesAsync();
            RewardHistoryContent.Id = RewardHistoryContentDAO.Id;
            await SaveReference(RewardHistoryContent);
            return true;
        }

        public async Task<bool> Update(RewardHistoryContent RewardHistoryContent)
        {
            RewardHistoryContentDAO RewardHistoryContentDAO = DataContext.RewardHistoryContent.Where(x => x.Id == RewardHistoryContent.Id).FirstOrDefault();
            if (RewardHistoryContentDAO == null)
                return false;
            RewardHistoryContentDAO.Id = RewardHistoryContent.Id;
            RewardHistoryContentDAO.RewardHistoryId = RewardHistoryContent.RewardHistoryId;
            RewardHistoryContentDAO.LuckeyNumberId = RewardHistoryContent.LuckeyNumberId;
            await DataContext.SaveChangesAsync();
            await SaveReference(RewardHistoryContent);
            return true;
        }

        public async Task<bool> Delete(RewardHistoryContent RewardHistoryContent)
        {
            await DataContext.RewardHistoryContent.Where(x => x.Id == RewardHistoryContent.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<RewardHistoryContent> RewardHistoryContents)
        {
            List<RewardHistoryContentDAO> RewardHistoryContentDAOs = new List<RewardHistoryContentDAO>();
            foreach (RewardHistoryContent RewardHistoryContent in RewardHistoryContents)
            {
                RewardHistoryContentDAO RewardHistoryContentDAO = new RewardHistoryContentDAO();
                RewardHistoryContentDAO.Id = RewardHistoryContent.Id;
                RewardHistoryContentDAO.RewardHistoryId = RewardHistoryContent.RewardHistoryId;
                RewardHistoryContentDAO.LuckeyNumberId = RewardHistoryContent.LuckeyNumberId;
                RewardHistoryContentDAOs.Add(RewardHistoryContentDAO);
            }
            await DataContext.BulkMergeAsync(RewardHistoryContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<RewardHistoryContent> RewardHistoryContents)
        {
            List<long> Ids = RewardHistoryContents.Select(x => x.Id).ToList();
            await DataContext.RewardHistoryContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(RewardHistoryContent RewardHistoryContent)
        {
        }
        
    }
}
