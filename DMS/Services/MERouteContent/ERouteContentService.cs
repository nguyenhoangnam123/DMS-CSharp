using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MERouteContent
{
    public interface IERouteContentService : IServiceScoped
    {
        Task<int> Count(ERouteContentFilter ERouteContentFilter);
        Task<List<ERouteContent>> List(ERouteContentFilter ERouteContentFilter);
        Task<ERouteContent> Get(long Id);
        Task<ERouteContent> Create(ERouteContent ERouteContent);
        Task<ERouteContent> Update(ERouteContent ERouteContent);
        Task<ERouteContent> Delete(ERouteContent ERouteContent);
        Task<List<ERouteContent>> BulkDelete(List<ERouteContent> ERouteContents);
        Task<List<ERouteContent>> Import(List<ERouteContent> ERouteContents);
        ERouteContentFilter ToFilter(ERouteContentFilter ERouteContentFilter);
    }

    public class ERouteContentService : BaseService, IERouteContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IERouteContentValidator ERouteContentValidator;

        public ERouteContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IERouteContentValidator ERouteContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ERouteContentValidator = ERouteContentValidator;
        }
        public async Task<int> Count(ERouteContentFilter ERouteContentFilter)
        {
            try
            {
                int result = await UOW.ERouteContentRepository.Count(ERouteContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERouteContent>> List(ERouteContentFilter ERouteContentFilter)
        {
            try
            {
                List<ERouteContent> ERouteContents = await UOW.ERouteContentRepository.List(ERouteContentFilter);
                return ERouteContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ERouteContent> Get(long Id)
        {
            ERouteContent ERouteContent = await UOW.ERouteContentRepository.Get(Id);
            if (ERouteContent == null)
                return null;
            return ERouteContent;
        }

        public async Task<ERouteContent> Create(ERouteContent ERouteContent)
        {
            if (!await ERouteContentValidator.Create(ERouteContent))
                return ERouteContent;

            try
            {
                await UOW.Begin();
                await UOW.ERouteContentRepository.Create(ERouteContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(ERouteContent, new { }, nameof(ERouteContentService));
                return await UOW.ERouteContentRepository.Get(ERouteContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ERouteContent> Update(ERouteContent ERouteContent)
        {
            if (!await ERouteContentValidator.Update(ERouteContent))
                return ERouteContent;
            try
            {
                var oldData = await UOW.ERouteContentRepository.Get(ERouteContent.Id);

                await UOW.Begin();
                await UOW.ERouteContentRepository.Update(ERouteContent);
                await UOW.Commit();

                var newData = await UOW.ERouteContentRepository.Get(ERouteContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ERouteContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ERouteContent> Delete(ERouteContent ERouteContent)
        {
            if (!await ERouteContentValidator.Delete(ERouteContent))
                return ERouteContent;

            try
            {
                await UOW.Begin();
                await UOW.ERouteContentRepository.Delete(ERouteContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ERouteContent, nameof(ERouteContentService));
                return ERouteContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERouteContent>> BulkDelete(List<ERouteContent> ERouteContents)
        {
            if (!await ERouteContentValidator.BulkDelete(ERouteContents))
                return ERouteContents;

            try
            {
                await UOW.Begin();
                await UOW.ERouteContentRepository.BulkDelete(ERouteContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ERouteContents, nameof(ERouteContentService));
                return ERouteContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERouteContent>> Import(List<ERouteContent> ERouteContents)
        {
            if (!await ERouteContentValidator.Import(ERouteContents))
                return ERouteContents;
            try
            {
                await UOW.Begin();
                await UOW.ERouteContentRepository.BulkMerge(ERouteContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(ERouteContents, new { }, nameof(ERouteContentService));
                return ERouteContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public ERouteContentFilter ToFilter(ERouteContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ERouteContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ERouteContentFilter subFilter = new ERouteContentFilter();
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
