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

namespace DMS.Services.MDirectSalesOrderContent
{
    public interface IDirectSalesOrderContentService :  IServiceScoped
    {
        Task<int> Count(DirectSalesOrderContentFilter DirectSalesOrderContentFilter);
        Task<List<DirectSalesOrderContent>> List(DirectSalesOrderContentFilter DirectSalesOrderContentFilter);
        Task<DirectSalesOrderContent> Get(long Id);
        Task<DirectSalesOrderContent> Create(DirectSalesOrderContent DirectSalesOrderContent);
        Task<DirectSalesOrderContent> Update(DirectSalesOrderContent DirectSalesOrderContent);
        Task<DirectSalesOrderContent> Delete(DirectSalesOrderContent DirectSalesOrderContent);
        Task<List<DirectSalesOrderContent>> BulkDelete(List<DirectSalesOrderContent> DirectSalesOrderContents);
        Task<List<DirectSalesOrderContent>> Import(List<DirectSalesOrderContent> DirectSalesOrderContents);
        DirectSalesOrderContentFilter ToFilter(DirectSalesOrderContentFilter DirectSalesOrderContentFilter);
    }

    public class DirectSalesOrderContentService : BaseService, IDirectSalesOrderContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDirectSalesOrderContentValidator DirectSalesOrderContentValidator;

        public DirectSalesOrderContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDirectSalesOrderContentValidator DirectSalesOrderContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DirectSalesOrderContentValidator = DirectSalesOrderContentValidator;
        }
        public async Task<int> Count(DirectSalesOrderContentFilter DirectSalesOrderContentFilter)
        {
            try
            {
                int result = await UOW.DirectSalesOrderContentRepository.Count(DirectSalesOrderContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectSalesOrderContent>> List(DirectSalesOrderContentFilter DirectSalesOrderContentFilter)
        {
            try
            {
                List<DirectSalesOrderContent> DirectSalesOrderContents = await UOW.DirectSalesOrderContentRepository.List(DirectSalesOrderContentFilter);
                return DirectSalesOrderContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<DirectSalesOrderContent> Get(long Id)
        {
            DirectSalesOrderContent DirectSalesOrderContent = await UOW.DirectSalesOrderContentRepository.Get(Id);
            if (DirectSalesOrderContent == null)
                return null;
            return DirectSalesOrderContent;
        }
       
        public async Task<DirectSalesOrderContent> Create(DirectSalesOrderContent DirectSalesOrderContent)
        {
            if (!await DirectSalesOrderContentValidator.Create(DirectSalesOrderContent))
                return DirectSalesOrderContent;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderContentRepository.Create(DirectSalesOrderContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectSalesOrderContent, new { }, nameof(DirectSalesOrderContentService));
                return await UOW.DirectSalesOrderContentRepository.Get(DirectSalesOrderContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectSalesOrderContent> Update(DirectSalesOrderContent DirectSalesOrderContent)
        {
            if (!await DirectSalesOrderContentValidator.Update(DirectSalesOrderContent))
                return DirectSalesOrderContent;
            try
            {
                var oldData = await UOW.DirectSalesOrderContentRepository.Get(DirectSalesOrderContent.Id);

                await UOW.Begin();
                await UOW.DirectSalesOrderContentRepository.Update(DirectSalesOrderContent);
                await UOW.Commit();

                var newData = await UOW.DirectSalesOrderContentRepository.Get(DirectSalesOrderContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(DirectSalesOrderContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectSalesOrderContent> Delete(DirectSalesOrderContent DirectSalesOrderContent)
        {
            if (!await DirectSalesOrderContentValidator.Delete(DirectSalesOrderContent))
                return DirectSalesOrderContent;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderContentRepository.Delete(DirectSalesOrderContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectSalesOrderContent, nameof(DirectSalesOrderContentService));
                return DirectSalesOrderContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<DirectSalesOrderContent>> BulkDelete(List<DirectSalesOrderContent> DirectSalesOrderContents)
        {
            if (!await DirectSalesOrderContentValidator.BulkDelete(DirectSalesOrderContents))
                return DirectSalesOrderContents;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderContentRepository.BulkDelete(DirectSalesOrderContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectSalesOrderContents, nameof(DirectSalesOrderContentService));
                return DirectSalesOrderContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<DirectSalesOrderContent>> Import(List<DirectSalesOrderContent> DirectSalesOrderContents)
        {
            if (!await DirectSalesOrderContentValidator.Import(DirectSalesOrderContents))
                return DirectSalesOrderContents;
            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderContentRepository.BulkMerge(DirectSalesOrderContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectSalesOrderContents, new { }, nameof(DirectSalesOrderContentService));
                return DirectSalesOrderContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public DirectSalesOrderContentFilter ToFilter(DirectSalesOrderContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DirectSalesOrderContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                DirectSalesOrderContentFilter subFilter = new DirectSalesOrderContentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DirectSalesOrderId))
                        subFilter.DirectSalesOrderId = Map(subFilter.DirectSalesOrderId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = Map(subFilter.ItemId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureId))
                        subFilter.UnitOfMeasureId = Map(subFilter.UnitOfMeasureId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Quantity))
                        subFilter.Quantity = Map(subFilter.Quantity, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PrimaryUnitOfMeasureId))
                        subFilter.PrimaryUnitOfMeasureId = Map(subFilter.PrimaryUnitOfMeasureId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestedQuantity))
                        subFilter.RequestedQuantity = Map(subFilter.RequestedQuantity, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Price))
                        subFilter.Price = Map(subFilter.Price, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountPercentage))
                        subFilter.DiscountPercentage = Map(subFilter.DiscountPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountAmount))
                        subFilter.DiscountAmount = Map(subFilter.DiscountAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountPercentage))
                        subFilter.GeneralDiscountPercentage = Map(subFilter.GeneralDiscountPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountAmount))
                        subFilter.GeneralDiscountAmount = Map(subFilter.GeneralDiscountAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxPercentage))
                        subFilter.TaxPercentage = Map(subFilter.TaxPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxAmount))
                        subFilter.TaxAmount = Map(subFilter.TaxAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Amount))
                        subFilter.Amount = Map(subFilter.Amount, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
