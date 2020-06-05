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

namespace DMS.Services.MERouteChangeRequestContent
{
    public interface IERouteChangeRequestContentService :  IServiceScoped
    {
        Task<int> Count(ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter);
        Task<List<ERouteChangeRequestContent>> List(ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter);
        Task<ERouteChangeRequestContent> Get(long Id);
        Task<ERouteChangeRequestContent> Create(ERouteChangeRequestContent ERouteChangeRequestContent);
        Task<ERouteChangeRequestContent> Update(ERouteChangeRequestContent ERouteChangeRequestContent);
        Task<ERouteChangeRequestContent> Delete(ERouteChangeRequestContent ERouteChangeRequestContent);
        Task<List<ERouteChangeRequestContent>> BulkDelete(List<ERouteChangeRequestContent> ERouteChangeRequestContents);
        Task<List<ERouteChangeRequestContent>> Import(List<ERouteChangeRequestContent> ERouteChangeRequestContents);
        ERouteChangeRequestContentFilter ToFilter(ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter);
    }

    public class ERouteChangeRequestContentService : BaseService, IERouteChangeRequestContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IERouteChangeRequestContentValidator ERouteChangeRequestContentValidator;

        public ERouteChangeRequestContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IERouteChangeRequestContentValidator ERouteChangeRequestContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ERouteChangeRequestContentValidator = ERouteChangeRequestContentValidator;
        }
        public async Task<int> Count(ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter)
        {
            try
            {
                int result = await UOW.ERouteChangeRequestContentRepository.Count(ERouteChangeRequestContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERouteChangeRequestContent>> List(ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter)
        {
            try
            {
                List<ERouteChangeRequestContent> ERouteChangeRequestContents = await UOW.ERouteChangeRequestContentRepository.List(ERouteChangeRequestContentFilter);
                return ERouteChangeRequestContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ERouteChangeRequestContent> Get(long Id)
        {
            ERouteChangeRequestContent ERouteChangeRequestContent = await UOW.ERouteChangeRequestContentRepository.Get(Id);
            if (ERouteChangeRequestContent == null)
                return null;
            return ERouteChangeRequestContent;
        }
       
        public async Task<ERouteChangeRequestContent> Create(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            if (!await ERouteChangeRequestContentValidator.Create(ERouteChangeRequestContent))
                return ERouteChangeRequestContent;

            try
            {
                await UOW.Begin();
                await UOW.ERouteChangeRequestContentRepository.Create(ERouteChangeRequestContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(ERouteChangeRequestContent, new { }, nameof(ERouteChangeRequestContentService));
                return await UOW.ERouteChangeRequestContentRepository.Get(ERouteChangeRequestContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ERouteChangeRequestContent> Update(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            if (!await ERouteChangeRequestContentValidator.Update(ERouteChangeRequestContent))
                return ERouteChangeRequestContent;
            try
            {
                var oldData = await UOW.ERouteChangeRequestContentRepository.Get(ERouteChangeRequestContent.Id);

                await UOW.Begin();
                await UOW.ERouteChangeRequestContentRepository.Update(ERouteChangeRequestContent);
                await UOW.Commit();

                var newData = await UOW.ERouteChangeRequestContentRepository.Get(ERouteChangeRequestContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ERouteChangeRequestContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ERouteChangeRequestContent> Delete(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            if (!await ERouteChangeRequestContentValidator.Delete(ERouteChangeRequestContent))
                return ERouteChangeRequestContent;

            try
            {
                await UOW.Begin();
                await UOW.ERouteChangeRequestContentRepository.Delete(ERouteChangeRequestContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ERouteChangeRequestContent, nameof(ERouteChangeRequestContentService));
                return ERouteChangeRequestContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERouteChangeRequestContent>> BulkDelete(List<ERouteChangeRequestContent> ERouteChangeRequestContents)
        {
            if (!await ERouteChangeRequestContentValidator.BulkDelete(ERouteChangeRequestContents))
                return ERouteChangeRequestContents;

            try
            {
                await UOW.Begin();
                await UOW.ERouteChangeRequestContentRepository.BulkDelete(ERouteChangeRequestContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ERouteChangeRequestContents, nameof(ERouteChangeRequestContentService));
                return ERouteChangeRequestContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ERouteChangeRequestContent>> Import(List<ERouteChangeRequestContent> ERouteChangeRequestContents)
        {
            if (!await ERouteChangeRequestContentValidator.Import(ERouteChangeRequestContents))
                return ERouteChangeRequestContents;
            try
            {
                await UOW.Begin();
                await UOW.ERouteChangeRequestContentRepository.BulkMerge(ERouteChangeRequestContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(ERouteChangeRequestContents, new { }, nameof(ERouteChangeRequestContentService));
                return ERouteChangeRequestContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteChangeRequestContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ERouteChangeRequestContentFilter ToFilter(ERouteChangeRequestContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ERouteChangeRequestContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ERouteChangeRequestContentFilter subFilter = new ERouteChangeRequestContentFilter();
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
