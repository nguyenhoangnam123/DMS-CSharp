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

namespace DMS.Services.MKpiItemContent
{
    public interface IKpiItemContentService :  IServiceScoped
    {
        Task<int> Count(KpiItemContentFilter KpiItemContentFilter);
        Task<List<KpiItemContent>> List(KpiItemContentFilter KpiItemContentFilter);
        Task<KpiItemContent> Get(long Id);
        Task<KpiItemContent> Create(KpiItemContent KpiItemContent);
        Task<KpiItemContent> Update(KpiItemContent KpiItemContent);
        Task<KpiItemContent> Delete(KpiItemContent KpiItemContent);
        Task<List<KpiItemContent>> BulkDelete(List<KpiItemContent> KpiItemContents);
        Task<List<KpiItemContent>> Import(List<KpiItemContent> KpiItemContents);
        Task<KpiItemContentFilter> ToFilter(KpiItemContentFilter KpiItemContentFilter);
    }

    public class KpiItemContentService : BaseService, IKpiItemContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IKpiItemContentValidator KpiItemContentValidator;

        public KpiItemContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IKpiItemContentValidator KpiItemContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.KpiItemContentValidator = KpiItemContentValidator;
        }
        public async Task<int> Count(KpiItemContentFilter KpiItemContentFilter)
        {
            try
            {
                int result = await UOW.KpiItemContentRepository.Count(KpiItemContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<KpiItemContent>> List(KpiItemContentFilter KpiItemContentFilter)
        {
            try
            {
                List<KpiItemContent> KpiItemContents = await UOW.KpiItemContentRepository.List(KpiItemContentFilter);
                return KpiItemContents;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<KpiItemContent> Get(long Id)
        {
            KpiItemContent KpiItemContent = await UOW.KpiItemContentRepository.Get(Id);
            if (KpiItemContent == null)
                return null;
            return KpiItemContent;
        }
       
        public async Task<KpiItemContent> Create(KpiItemContent KpiItemContent)
        {
            if (!await KpiItemContentValidator.Create(KpiItemContent))
                return KpiItemContent;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemContentRepository.Create(KpiItemContent);
                await UOW.Commit();
                KpiItemContent = await UOW.KpiItemContentRepository.Get(KpiItemContent.Id);
                await Logging.CreateAuditLog(KpiItemContent, new { }, nameof(KpiItemContentService));
                return KpiItemContent;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<KpiItemContent> Update(KpiItemContent KpiItemContent)
        {
            if (!await KpiItemContentValidator.Update(KpiItemContent))
                return KpiItemContent;
            try
            {
                var oldData = await UOW.KpiItemContentRepository.Get(KpiItemContent.Id);

                await UOW.Begin();
                await UOW.KpiItemContentRepository.Update(KpiItemContent);
                await UOW.Commit();

                KpiItemContent = await UOW.KpiItemContentRepository.Get(KpiItemContent.Id);
                await Logging.CreateAuditLog(KpiItemContent, oldData, nameof(KpiItemContentService));
                return KpiItemContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<KpiItemContent> Delete(KpiItemContent KpiItemContent)
        {
            if (!await KpiItemContentValidator.Delete(KpiItemContent))
                return KpiItemContent;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemContentRepository.Delete(KpiItemContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiItemContent, nameof(KpiItemContentService));
                return KpiItemContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<KpiItemContent>> BulkDelete(List<KpiItemContent> KpiItemContents)
        {
            if (!await KpiItemContentValidator.BulkDelete(KpiItemContents))
                return KpiItemContents;

            try
            {
                await UOW.Begin();
                await UOW.KpiItemContentRepository.BulkDelete(KpiItemContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, KpiItemContents, nameof(KpiItemContentService));
                return KpiItemContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<KpiItemContent>> Import(List<KpiItemContent> KpiItemContents)
        {
            if (!await KpiItemContentValidator.Import(KpiItemContents))
                return KpiItemContents;
            try
            {
                await UOW.Begin();
                await UOW.KpiItemContentRepository.BulkMerge(KpiItemContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(KpiItemContents, new { }, nameof(KpiItemContentService));
                return KpiItemContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(KpiItemContentService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(KpiItemContentService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<KpiItemContentFilter> ToFilter(KpiItemContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<KpiItemContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                KpiItemContentFilter subFilter = new KpiItemContentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.KpiItemId))
                        subFilter.KpiItemId = FilterBuilder.Merge(subFilter.KpiItemId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = FilterBuilder.Merge(subFilter.ItemId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
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
