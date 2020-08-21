using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MPriceListItemHistory
{
    public interface IPriceListItemHistoryService : IServiceScoped
    {
        Task<int> Count(PriceListItemHistoryFilter PriceListItemHistoryFilter);
        Task<List<PriceListItemHistory>> List(PriceListItemHistoryFilter PriceListItemHistoryFilter);
        Task<PriceListItemHistory> Get(long Id);
        Task<PriceListItemHistory> Create(PriceListItemHistory PriceListItemHistory);
        Task<PriceListItemHistory> Update(PriceListItemHistory PriceListItemHistory);
        Task<PriceListItemHistory> Delete(PriceListItemHistory PriceListItemHistory);
        Task<List<PriceListItemHistory>> BulkDelete(List<PriceListItemHistory> PriceListItemHistories);
        Task<List<PriceListItemHistory>> Import(List<PriceListItemHistory> PriceListItemHistories);
        Task<PriceListItemHistoryFilter> ToFilter(PriceListItemHistoryFilter PriceListItemHistoryFilter);
    }

    public class PriceListItemHistoryService : BaseService, IPriceListItemHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPriceListItemHistoryValidator PriceListItemHistoryValidator;

        public PriceListItemHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPriceListItemHistoryValidator PriceListItemHistoryValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PriceListItemHistoryValidator = PriceListItemHistoryValidator;
        }
        public async Task<int> Count(PriceListItemHistoryFilter PriceListItemHistoryFilter)
        {
            try
            {
                int result = await UOW.PriceListItemHistoryRepository.Count(PriceListItemHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceListItemHistory>> List(PriceListItemHistoryFilter PriceListItemHistoryFilter)
        {
            try
            {
                List<PriceListItemHistory> PriceListItemHistorys = await UOW.PriceListItemHistoryRepository.List(PriceListItemHistoryFilter);
                return PriceListItemHistorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PriceListItemHistory> Get(long Id)
        {
            PriceListItemHistory PriceListItemHistory = await UOW.PriceListItemHistoryRepository.Get(Id);
            if (PriceListItemHistory == null)
                return null;
            return PriceListItemHistory;
        }

        public async Task<PriceListItemHistory> Create(PriceListItemHistory PriceListItemHistory)
        {
            if (!await PriceListItemHistoryValidator.Create(PriceListItemHistory))
                return PriceListItemHistory;

            try
            {
                await UOW.Begin();
                await UOW.PriceListItemHistoryRepository.Create(PriceListItemHistory);
                await UOW.Commit();
                PriceListItemHistory = await UOW.PriceListItemHistoryRepository.Get(PriceListItemHistory.Id);
                await Logging.CreateAuditLog(PriceListItemHistory, new { }, nameof(PriceListItemHistoryService));
                return PriceListItemHistory;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceListItemHistory> Update(PriceListItemHistory PriceListItemHistory)
        {
            if (!await PriceListItemHistoryValidator.Update(PriceListItemHistory))
                return PriceListItemHistory;
            try
            {
                var oldData = await UOW.PriceListItemHistoryRepository.Get(PriceListItemHistory.Id);

                await UOW.Begin();
                await UOW.PriceListItemHistoryRepository.Update(PriceListItemHistory);
                await UOW.Commit();

                PriceListItemHistory = await UOW.PriceListItemHistoryRepository.Get(PriceListItemHistory.Id);
                await Logging.CreateAuditLog(PriceListItemHistory, oldData, nameof(PriceListItemHistoryService));
                return PriceListItemHistory;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceListItemHistory> Delete(PriceListItemHistory PriceListItemHistory)
        {
            if (!await PriceListItemHistoryValidator.Delete(PriceListItemHistory))
                return PriceListItemHistory;

            try
            {
                await UOW.Begin();
                await UOW.PriceListItemHistoryRepository.Delete(PriceListItemHistory);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceListItemHistory, nameof(PriceListItemHistoryService));
                return PriceListItemHistory;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceListItemHistory>> BulkDelete(List<PriceListItemHistory> PriceListItemHistories)
        {
            if (!await PriceListItemHistoryValidator.BulkDelete(PriceListItemHistories))
                return PriceListItemHistories;

            try
            {
                await UOW.Begin();
                await UOW.PriceListItemHistoryRepository.BulkDelete(PriceListItemHistories);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceListItemHistories, nameof(PriceListItemHistoryService));
                return PriceListItemHistories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceListItemHistory>> Import(List<PriceListItemHistory> PriceListItemHistories)
        {
            if (!await PriceListItemHistoryValidator.Import(PriceListItemHistories))
                return PriceListItemHistories;
            try
            {
                await UOW.Begin();
                await UOW.PriceListItemHistoryRepository.BulkMerge(PriceListItemHistories);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceListItemHistories, new { }, nameof(PriceListItemHistoryService));
                return PriceListItemHistories;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListItemHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceListItemHistoryFilter> ToFilter(PriceListItemHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PriceListItemHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PriceListItemHistoryFilter subFilter = new PriceListItemHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter); 
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PriceListId))
                        subFilter.PriceListId = FilterBuilder.Merge(subFilter.PriceListId, FilterPermissionDefinition.IdFilter); 
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = FilterBuilder.Merge(subFilter.ItemId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OldPrice))
                        subFilter.OldPrice = FilterBuilder.Merge(subFilter.OldPrice, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.NewPrice))
                        subFilter.NewPrice = FilterBuilder.Merge(subFilter.NewPrice, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ModifierId))
                        subFilter.ModifierId = FilterBuilder.Merge(subFilter.ModifierId, FilterPermissionDefinition.IdFilter); 
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
