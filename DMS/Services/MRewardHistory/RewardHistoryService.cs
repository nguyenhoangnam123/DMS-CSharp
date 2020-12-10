using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MRewardHistory
{
    public interface IRewardHistoryService :  IServiceScoped
    {
        Task<int> Count(RewardHistoryFilter RewardHistoryFilter);
        Task<List<RewardHistory>> List(RewardHistoryFilter RewardHistoryFilter);
        Task<RewardHistory> Get(long Id);
        Task<RewardHistory> Create(RewardHistory RewardHistory);
        Task<RewardHistory> Update(RewardHistory RewardHistory);
        Task<RewardHistory> Delete(RewardHistory RewardHistory);
        Task<List<RewardHistory>> BulkDelete(List<RewardHistory> RewardHistories);
        Task<List<RewardHistory>> Import(List<RewardHistory> RewardHistories);
        Task<RewardHistoryFilter> ToFilter(RewardHistoryFilter RewardHistoryFilter);
    }

    public class RewardHistoryService : BaseService, IRewardHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IRewardHistoryValidator RewardHistoryValidator;

        public RewardHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IRewardHistoryValidator RewardHistoryValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.RewardHistoryValidator = RewardHistoryValidator;
        }
        public async Task<int> Count(RewardHistoryFilter RewardHistoryFilter)
        {
            try
            {
                int result = await UOW.RewardHistoryRepository.Count(RewardHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<RewardHistory>> List(RewardHistoryFilter RewardHistoryFilter)
        {
            try
            {
                List<RewardHistory> RewardHistorys = await UOW.RewardHistoryRepository.List(RewardHistoryFilter);
                return RewardHistorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<RewardHistory> Get(long Id)
        {
            RewardHistory RewardHistory = await UOW.RewardHistoryRepository.Get(Id);
            if (RewardHistory == null)
                return null;
            return RewardHistory;
        }
       
        public async Task<RewardHistory> Create(RewardHistory RewardHistory)
        {
            if (!await RewardHistoryValidator.Create(RewardHistory))
                return RewardHistory;

            try
            {
                RewardHistory.TurnCounter = (long)(RewardHistory.Revenue / 20000000);
                await UOW.Begin();
                await UOW.RewardHistoryRepository.Create(RewardHistory);
                await UOW.Commit();
                RewardHistory = await UOW.RewardHistoryRepository.Get(RewardHistory.Id);
                await Logging.CreateAuditLog(RewardHistory, new { }, nameof(RewardHistoryService));
                return RewardHistory;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<RewardHistory> Update(RewardHistory RewardHistory)
        {
            if (!await RewardHistoryValidator.Update(RewardHistory))
                return RewardHistory;
            try
            {
                var oldData = await UOW.RewardHistoryRepository.Get(RewardHistory.Id);

                await UOW.Begin();
                await UOW.RewardHistoryRepository.Update(RewardHistory);
                await UOW.Commit();

                RewardHistory = await UOW.RewardHistoryRepository.Get(RewardHistory.Id);
                await Logging.CreateAuditLog(RewardHistory, oldData, nameof(RewardHistoryService));
                return RewardHistory;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<RewardHistory> Delete(RewardHistory RewardHistory)
        {
            if (!await RewardHistoryValidator.Delete(RewardHistory))
                return RewardHistory;

            try
            {
                await UOW.Begin();
                await UOW.RewardHistoryRepository.Delete(RewardHistory);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, RewardHistory, nameof(RewardHistoryService));
                return RewardHistory;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<RewardHistory>> BulkDelete(List<RewardHistory> RewardHistories)
        {
            if (!await RewardHistoryValidator.BulkDelete(RewardHistories))
                return RewardHistories;

            try
            {
                await UOW.Begin();
                await UOW.RewardHistoryRepository.BulkDelete(RewardHistories);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, RewardHistories, nameof(RewardHistoryService));
                return RewardHistories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<RewardHistory>> Import(List<RewardHistory> RewardHistories)
        {
            if (!await RewardHistoryValidator.Import(RewardHistories))
                return RewardHistories;
            try
            {
                await UOW.Begin();
                await UOW.RewardHistoryRepository.BulkMerge(RewardHistories);
                await UOW.Commit();

                await Logging.CreateAuditLog(RewardHistories, new { }, nameof(RewardHistoryService));
                return RewardHistories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(RewardHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(RewardHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<RewardHistoryFilter> ToFilter(RewardHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<RewardHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                RewardHistoryFilter subFilter = new RewardHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreId))
                        subFilter.StoreId = FilterBuilder.Merge(subFilter.StoreId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TurnCounter))
                        subFilter.TurnCounter = FilterBuilder.Merge(subFilter.TurnCounter, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
