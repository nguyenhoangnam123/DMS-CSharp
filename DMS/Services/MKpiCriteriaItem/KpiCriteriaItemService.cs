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

namespace DMS.Services.MKpiCriteriaItem
{
    public interface IKpiCriteriaItemService :  IServiceScoped
    {
        Task<int> Count(KpiCriteriaItemFilter KpiCriteriaItemFilter);
        Task<List<KpiCriteriaItem>> List(KpiCriteriaItemFilter KpiCriteriaItemFilter);
        Task<KpiCriteriaItem> Get(long Id);
        Task<KpiCriteriaItem> Create(KpiCriteriaItem KpiCriteriaItem);
        Task<KpiCriteriaItem> Update(KpiCriteriaItem KpiCriteriaItem);
        Task<KpiCriteriaItem> Delete(KpiCriteriaItem KpiCriteriaItem);
        Task<List<KpiCriteriaItem>> BulkDelete(List<KpiCriteriaItem> KpiCriteriaItems);
        Task<List<KpiCriteriaItem>> Import(List<KpiCriteriaItem> KpiCriteriaItems);
        KpiCriteriaItemFilter ToFilter(KpiCriteriaItemFilter KpiCriteriaItemFilter);
    }

    public class KpiCriteriaItemService : BaseService, IKpiCriteriaItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiCriteriaItemValidator KpiCriteriaItemValidator;

        public KpiCriteriaItemService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiCriteriaItemValidator KpiCriteriaItemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiCriteriaItemValidator = KpiCriteriaItemValidator;
        }
        public async Task<int> Count(KpiCriteriaItemFilter KpiCriteriaItemFilter)
        {
            try
            {
                int result = await UOW.KpiCriteriaItemRepository.Count(KpiCriteriaItemFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiCriteriaItem>> List(KpiCriteriaItemFilter KpiCriteriaItemFilter)
        {
            try
            {
                List<KpiCriteriaItem> KpiCriteriaItems = await UOW.KpiCriteriaItemRepository.List(KpiCriteriaItemFilter);
                return KpiCriteriaItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<KpiCriteriaItem> Get(long Id)
        {
            KpiCriteriaItem KpiCriteriaItem = await UOW.KpiCriteriaItemRepository.Get(Id);
            if (KpiCriteriaItem == null)
                return null;
            return KpiCriteriaItem;
        }
       
        public async Task<KpiCriteriaItem> Create(KpiCriteriaItem KpiCriteriaItem)
        {
            if (!await KpiCriteriaItemValidator.Create(KpiCriteriaItem))
                return KpiCriteriaItem;

            try
            {
                await UOW.Begin();
                await UOW.KpiCriteriaItemRepository.Create(KpiCriteriaItem);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiCriteriaItem, new { }, nameof(KpiCriteriaItemService));
                return await UOW.KpiCriteriaItemRepository.Get(KpiCriteriaItem.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiCriteriaItem> Update(KpiCriteriaItem KpiCriteriaItem)
        {
            if (!await KpiCriteriaItemValidator.Update(KpiCriteriaItem))
                return KpiCriteriaItem;
            try
            {
                var oldData = await UOW.KpiCriteriaItemRepository.Get(KpiCriteriaItem.Id);

                await UOW.Begin();
                await UOW.KpiCriteriaItemRepository.Update(KpiCriteriaItem);
                await UOW.Commit();

                var newData = await UOW.KpiCriteriaItemRepository.Get(KpiCriteriaItem.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(KpiCriteriaItemService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<KpiCriteriaItem> Delete(KpiCriteriaItem KpiCriteriaItem)
        {
            if (!await KpiCriteriaItemValidator.Delete(KpiCriteriaItem))
                return KpiCriteriaItem;

            try
            {
                await UOW.Begin();
                await UOW.KpiCriteriaItemRepository.Delete(KpiCriteriaItem);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiCriteriaItem, nameof(KpiCriteriaItemService));
                return KpiCriteriaItem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<KpiCriteriaItem>> BulkDelete(List<KpiCriteriaItem> KpiCriteriaItems)
        {
            if (!await KpiCriteriaItemValidator.BulkDelete(KpiCriteriaItems))
                return KpiCriteriaItems;

            try
            {
                await UOW.Begin();
                await UOW.KpiCriteriaItemRepository.BulkDelete(KpiCriteriaItems);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiCriteriaItems, nameof(KpiCriteriaItemService));
                return KpiCriteriaItems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<KpiCriteriaItem>> Import(List<KpiCriteriaItem> KpiCriteriaItems)
        {
            if (!await KpiCriteriaItemValidator.Import(KpiCriteriaItems))
                return KpiCriteriaItems;
            try
            {
                await UOW.Begin();
                await UOW.KpiCriteriaItemRepository.BulkMerge(KpiCriteriaItems);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiCriteriaItems, new { }, nameof(KpiCriteriaItemService));
                return KpiCriteriaItems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(KpiCriteriaItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public KpiCriteriaItemFilter ToFilter(KpiCriteriaItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiCriteriaItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiCriteriaItemFilter subFilter = new KpiCriteriaItemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
