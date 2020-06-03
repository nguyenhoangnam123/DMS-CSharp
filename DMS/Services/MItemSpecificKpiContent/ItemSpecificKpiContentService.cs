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

namespace DMS.Services.MItemSpecificKpiContent
{
    public interface IItemSpecificKpiContentService :  IServiceScoped
    {
        Task<int> Count(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter);
        Task<List<ItemSpecificKpiContent>> List(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter);
        Task<ItemSpecificKpiContent> Get(long Id);
        Task<ItemSpecificKpiContent> Create(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<ItemSpecificKpiContent> Update(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<ItemSpecificKpiContent> Delete(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<List<ItemSpecificKpiContent>> BulkDelete(List<ItemSpecificKpiContent> ItemSpecificKpiContents);
        Task<List<ItemSpecificKpiContent>> Import(List<ItemSpecificKpiContent> ItemSpecificKpiContents);
        ItemSpecificKpiContentFilter ToFilter(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter);
    }

    public class ItemSpecificKpiContentService : BaseService, IItemSpecificKpiContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IItemSpecificKpiContentValidator ItemSpecificKpiContentValidator;

        public ItemSpecificKpiContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IItemSpecificKpiContentValidator ItemSpecificKpiContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ItemSpecificKpiContentValidator = ItemSpecificKpiContentValidator;
        }
        public async Task<int> Count(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter)
        {
            try
            {
                int result = await UOW.ItemSpecificKpiContentRepository.Count(ItemSpecificKpiContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ItemSpecificKpiContent>> List(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter)
        {
            try
            {
                List<ItemSpecificKpiContent> ItemSpecificKpiContents = await UOW.ItemSpecificKpiContentRepository.List(ItemSpecificKpiContentFilter);
                return ItemSpecificKpiContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ItemSpecificKpiContent> Get(long Id)
        {
            ItemSpecificKpiContent ItemSpecificKpiContent = await UOW.ItemSpecificKpiContentRepository.Get(Id);
            if (ItemSpecificKpiContent == null)
                return null;
            return ItemSpecificKpiContent;
        }
       
        public async Task<ItemSpecificKpiContent> Create(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            if (!await ItemSpecificKpiContentValidator.Create(ItemSpecificKpiContent))
                return ItemSpecificKpiContent;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiContentRepository.Create(ItemSpecificKpiContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemSpecificKpiContent, new { }, nameof(ItemSpecificKpiContentService));
                return await UOW.ItemSpecificKpiContentRepository.Get(ItemSpecificKpiContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ItemSpecificKpiContent> Update(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            if (!await ItemSpecificKpiContentValidator.Update(ItemSpecificKpiContent))
                return ItemSpecificKpiContent;
            try
            {
                var oldData = await UOW.ItemSpecificKpiContentRepository.Get(ItemSpecificKpiContent.Id);

                await UOW.Begin();
                await UOW.ItemSpecificKpiContentRepository.Update(ItemSpecificKpiContent);
                await UOW.Commit();

                var newData = await UOW.ItemSpecificKpiContentRepository.Get(ItemSpecificKpiContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ItemSpecificKpiContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ItemSpecificKpiContent> Delete(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            if (!await ItemSpecificKpiContentValidator.Delete(ItemSpecificKpiContent))
                return ItemSpecificKpiContent;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiContentRepository.Delete(ItemSpecificKpiContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemSpecificKpiContent, nameof(ItemSpecificKpiContentService));
                return ItemSpecificKpiContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ItemSpecificKpiContent>> BulkDelete(List<ItemSpecificKpiContent> ItemSpecificKpiContents)
        {
            if (!await ItemSpecificKpiContentValidator.BulkDelete(ItemSpecificKpiContents))
                return ItemSpecificKpiContents;

            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiContentRepository.BulkDelete(ItemSpecificKpiContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ItemSpecificKpiContents, nameof(ItemSpecificKpiContentService));
                return ItemSpecificKpiContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ItemSpecificKpiContent>> Import(List<ItemSpecificKpiContent> ItemSpecificKpiContents)
        {
            if (!await ItemSpecificKpiContentValidator.Import(ItemSpecificKpiContents))
                return ItemSpecificKpiContents;
            try
            {
                await UOW.Begin();
                await UOW.ItemSpecificKpiContentRepository.BulkMerge(ItemSpecificKpiContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(ItemSpecificKpiContents, new { }, nameof(ItemSpecificKpiContentService));
                return ItemSpecificKpiContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemSpecificKpiContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ItemSpecificKpiContentFilter ToFilter(ItemSpecificKpiContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemSpecificKpiContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemSpecificKpiContentFilter subFilter = new ItemSpecificKpiContentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemSpecificKpiId))
                        subFilter.ItemSpecificKpiId = Map(subFilter.ItemSpecificKpiId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = Map(subFilter.ItemId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
