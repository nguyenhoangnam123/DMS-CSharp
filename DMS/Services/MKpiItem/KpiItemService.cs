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
using DMS.Helpers;

namespace DMS.Services.MKpiItem
{
    public interface IKpiItemService :  IServiceScoped
    {
        Task<int> Count(KpiItemFilter KpiItemFilter);
        Task<List<KpiItem>> List(KpiItemFilter KpiItemFilter);
        Task<KpiItem> Get(long Id);
        Task<KpiItem> Create(KpiItem KpiItem);
        Task<KpiItem> Update(KpiItem KpiItem);
        Task<KpiItem> Delete(KpiItem KpiItem);
        Task<List<KpiItem>> BulkDelete(List<KpiItem> KpiItems);
        Task<List<KpiItem>> Import(List<KpiItem> KpiItems);
        KpiItemFilter ToFilter(KpiItemFilter KpiItemFilter);
    }

    public class KpiItemService : BaseService, IKpiItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiItemValidator KpiItemValidator;

        public KpiItemService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiItemValidator KpiItemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiItemValidator = KpiItemValidator;
        }
        public async Task<int> Count(KpiItemFilter KpiItemFilter)
        {
            try
            {
                int result = await UOW.KpiItemRepository.Count(KpiItemFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiItem>> List(KpiItemFilter KpiItemFilter)
        {
            try
            {
                List<KpiItem> KpiItems = await UOW.KpiItemRepository.List(KpiItemFilter);
                return KpiItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<KpiItem> Get(long Id)
        {
            KpiItem KpiItem = await UOW.KpiItemRepository.Get(Id);
            if (KpiItem == null)
                return null;
            return KpiItem;
        }
       
        public async Task<KpiItem> Create(KpiItem KpiItem)
        {
            if (!await KpiItemValidator.Create(KpiItem))
                return KpiItem;

            try
            {
                await UOW.Begin();
                List<KpiItem> KpiItems = new List<KpiItem>();
                if (KpiItem.EmployeeIds != null && KpiItem.EmployeeIds.Any())
                {
                    foreach (var EmployeeId in KpiItem.EmployeeIds)
                    {
                        var newObj = Utils.Clone(KpiItem);
                        newObj.EmployeeId = EmployeeId;
                        newObj.CreatorId = CurrentContext.UserId;
                        KpiItems.Add(newObj);
                    }
                }
                await UOW.KpiItemRepository.BulkMerge(KpiItems);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiItem, new { }, nameof(KpiItemService));
                return KpiItem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiItem> Update(KpiItem KpiItem)
        {
            if (!await KpiItemValidator.Update(KpiItem))
                return KpiItem;
            try
            {
                var oldData = await UOW.KpiItemRepository.Get(KpiItem.Id);

                await UOW.Begin();
                await UOW.KpiItemRepository.Update(KpiItem);
                await UOW.Commit();

                var newData = await UOW.KpiItemRepository.Get(KpiItem.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(KpiItemService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiItem> Delete(KpiItem KpiItem)
        {
            if (!await KpiItemValidator.Delete(KpiItem))
                return KpiItem;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemRepository.Delete(KpiItem);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiItem, nameof(KpiItemService));
                return KpiItem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiItem>> BulkDelete(List<KpiItem> KpiItems)
        {
            if (!await KpiItemValidator.BulkDelete(KpiItems))
                return KpiItems;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemRepository.BulkDelete(KpiItems);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiItems, nameof(KpiItemService));
                return KpiItems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<KpiItem>> Import(List<KpiItem> KpiItems)
        {
            if (!await KpiItemValidator.Import(KpiItems))
                return KpiItems;
            try
            {
                await UOW.Begin();
                await UOW.KpiItemRepository.BulkMerge(KpiItems);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiItems, new { }, nameof(KpiItemService));
                return KpiItems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public KpiItemFilter ToFilter(KpiItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiItemFilter subFilter = new KpiItemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = Map(subFilter.OrganizationId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.KpiPeriodId))
                        subFilter.KpiPeriodId = Map(subFilter.KpiPeriodId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EmployeeId))
                        subFilter.EmployeeId = Map(subFilter.EmployeeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = Map(subFilter.CreatorId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
