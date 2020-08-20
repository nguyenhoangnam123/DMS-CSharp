using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MPriceList
{
    public interface IPriceListService : IServiceScoped
    {
        Task<int> Count(PriceListFilter PriceListFilter);
        Task<List<PriceList>> List(PriceListFilter PriceListFilter);
        Task<PriceList> Get(long Id);
        Task<PriceList> Create(PriceList PriceList);
        Task<PriceList> Update(PriceList PriceList);
        Task<PriceList> Delete(PriceList PriceList);
        Task<List<PriceList>> BulkDelete(List<PriceList> PriceLists);
        Task<List<PriceList>> Import(List<PriceList> PriceLists);
        Task<PriceListFilter> ToFilter(PriceListFilter filter);
    }

    public class PriceListService : BaseService, IPriceListService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPriceListValidator PriceListValidator;

        public PriceListService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPriceListValidator PriceListValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PriceListValidator = PriceListValidator;
        }
        public async Task<int> Count(PriceListFilter PriceListFilter)
        {
            try
            {
                int result = await UOW.PriceListRepository.Count(PriceListFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceList>> List(PriceListFilter PriceListFilter)
        {
            try
            {
                List<PriceList> PriceLists = await UOW.PriceListRepository.List(PriceListFilter);
                return PriceLists;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PriceList> Get(long Id)
        {
            PriceList PriceList = await UOW.PriceListRepository.Get(Id);
            if (PriceList == null)
                return null;
            return PriceList;
        }

        public async Task<PriceList> Create(PriceList PriceList)
        {
            if (!await PriceListValidator.Create(PriceList))
                return PriceList;

            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.Create(PriceList);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceList, new { }, nameof(PriceListService));
                return await UOW.PriceListRepository.Get(PriceList.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceList> Update(PriceList PriceList)
        {
            if (!await PriceListValidator.Update(PriceList))
                return PriceList;
            try
            {
                var oldData = await UOW.PriceListRepository.Get(PriceList.Id);
                await BuildData(PriceList);

                await UOW.Begin();
                await UOW.PriceListRepository.Update(PriceList);
                await UOW.Commit();

                var newData = await UOW.PriceListRepository.Get(PriceList.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(PriceListService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceList> Delete(PriceList PriceList)
        {
            if (!await PriceListValidator.Delete(PriceList))
                return PriceList;

            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.Delete(PriceList);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceList, nameof(PriceListService));
                return PriceList;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceList>> BulkDelete(List<PriceList> PriceLists)
        {
            if (!await PriceListValidator.BulkDelete(PriceLists))
                return PriceLists;

            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.BulkDelete(PriceLists);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PriceLists, nameof(PriceListService));
                return PriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PriceList>> Import(List<PriceList> PriceLists)
        {
            if (!await PriceListValidator.Import(PriceLists))
                return PriceLists;
            try
            {
                await UOW.Begin();
                await UOW.PriceListRepository.BulkMerge(PriceLists);
                await UOW.Commit();

                await Logging.CreateAuditLog(PriceLists, new { }, nameof(PriceListService));
                return PriceLists;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PriceListService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PriceListService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PriceListFilter> ToFilter(PriceListFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PriceListFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PriceListFilter subFilter = new PriceListFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter); if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartDate))
                        subFilter.StartDate = FilterBuilder.Merge(subFilter.StartDate, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndDate))
                        subFilter.EndDate = FilterBuilder.Merge(subFilter.EndDate, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter); if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter); if (FilterPermissionDefinition.Name == nameof(subFilter.PriceListTypeId))
                        subFilter.PriceListTypeId = FilterBuilder.Merge(subFilter.PriceListTypeId, FilterPermissionDefinition.IdFilter); if (FilterPermissionDefinition.Name == nameof(subFilter.SalesOrderTypeId))
                        subFilter.SalesOrderTypeId = FilterBuilder.Merge(subFilter.SalesOrderTypeId, FilterPermissionDefinition.IdFilter); if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
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

        private async Task<PriceList> BuildData(PriceList PriceList)
        {
            PriceList oldData = await UOW.PriceListRepository.Get(PriceList.Id);
            if (oldData != null)
            {
                foreach (PriceListItemMapping PriceListItemMapping in PriceList.PriceListItemMappings)
                {
                    if (PriceListItemMapping.PriceListItemHistories == null)
                        PriceListItemMapping.PriceListItemHistories = new List<PriceListItemHistory>();
                    PriceListItemMapping PriceListItemMappingInDB = oldData.PriceListItemMappings.Where(i => i.ItemId == PriceListItemMapping.ItemId).FirstOrDefault();
                    if (PriceListItemMappingInDB != null)
                    {
                        if (PriceListItemMapping.Price != PriceListItemMappingInDB.Price)
                        {
                            PriceListItemHistory PriceListItemHistory = new PriceListItemHistory();
                            PriceListItemHistory.ItemId = PriceListItemMapping.ItemId;
                            PriceListItemHistory.PriceListId = PriceList.Id;
                            PriceListItemHistory.OldPrice = PriceListItemMappingInDB.Price;
                            PriceListItemHistory.NewPrice = PriceListItemMapping.Price;
                            PriceListItemHistory.ModifierId = CurrentContext.UserId;
                            PriceListItemMapping.PriceListItemHistories.Add(PriceListItemHistory);
                        }
                    }
                }
            }
            return PriceList;
        }
    }
}
