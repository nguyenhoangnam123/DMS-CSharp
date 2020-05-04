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

namespace DMS.Services.MIndirectSalesOrderContent
{
    public interface IIndirectSalesOrderContentService :  IServiceScoped
    {
        Task<int> Count(IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter);
        Task<List<IndirectSalesOrderContent>> List(IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter);
        Task<IndirectSalesOrderContent> Get(long Id);
        Task<IndirectSalesOrderContent> Create(IndirectSalesOrderContent IndirectSalesOrderContent);
        Task<IndirectSalesOrderContent> Update(IndirectSalesOrderContent IndirectSalesOrderContent);
        Task<IndirectSalesOrderContent> Delete(IndirectSalesOrderContent IndirectSalesOrderContent);
        Task<List<IndirectSalesOrderContent>> BulkDelete(List<IndirectSalesOrderContent> IndirectSalesOrderContents);
        Task<List<IndirectSalesOrderContent>> Import(List<IndirectSalesOrderContent> IndirectSalesOrderContents);
        IndirectSalesOrderContentFilter ToFilter(IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter);
    }

    public class IndirectSalesOrderContentService : BaseService, IIndirectSalesOrderContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderContentValidator IndirectSalesOrderContentValidator;

        public IndirectSalesOrderContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectSalesOrderContentValidator IndirectSalesOrderContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderContentValidator = IndirectSalesOrderContentValidator;
        }
        public async Task<int> Count(IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter)
        {
            try
            {
                int result = await UOW.IndirectSalesOrderContentRepository.Count(IndirectSalesOrderContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrderContent>> List(IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter)
        {
            try
            {
                List<IndirectSalesOrderContent> IndirectSalesOrderContents = await UOW.IndirectSalesOrderContentRepository.List(IndirectSalesOrderContentFilter);
                return IndirectSalesOrderContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<IndirectSalesOrderContent> Get(long Id)
        {
            IndirectSalesOrderContent IndirectSalesOrderContent = await UOW.IndirectSalesOrderContentRepository.Get(Id);
            if (IndirectSalesOrderContent == null)
                return null;
            return IndirectSalesOrderContent;
        }
       
        public async Task<IndirectSalesOrderContent> Create(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            if (!await IndirectSalesOrderContentValidator.Create(IndirectSalesOrderContent))
                return IndirectSalesOrderContent;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderContentRepository.Create(IndirectSalesOrderContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrderContent, new { }, nameof(IndirectSalesOrderContentService));
                return await UOW.IndirectSalesOrderContentRepository.Get(IndirectSalesOrderContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectSalesOrderContent> Update(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            if (!await IndirectSalesOrderContentValidator.Update(IndirectSalesOrderContent))
                return IndirectSalesOrderContent;
            try
            {
                var oldData = await UOW.IndirectSalesOrderContentRepository.Get(IndirectSalesOrderContent.Id);

                await UOW.Begin();
                await UOW.IndirectSalesOrderContentRepository.Update(IndirectSalesOrderContent);
                await UOW.Commit();

                var newData = await UOW.IndirectSalesOrderContentRepository.Get(IndirectSalesOrderContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(IndirectSalesOrderContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectSalesOrderContent> Delete(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            if (!await IndirectSalesOrderContentValidator.Delete(IndirectSalesOrderContent))
                return IndirectSalesOrderContent;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderContentRepository.Delete(IndirectSalesOrderContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrderContent, nameof(IndirectSalesOrderContentService));
                return IndirectSalesOrderContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrderContent>> BulkDelete(List<IndirectSalesOrderContent> IndirectSalesOrderContents)
        {
            if (!await IndirectSalesOrderContentValidator.BulkDelete(IndirectSalesOrderContents))
                return IndirectSalesOrderContents;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderContentRepository.BulkDelete(IndirectSalesOrderContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrderContents, nameof(IndirectSalesOrderContentService));
                return IndirectSalesOrderContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<IndirectSalesOrderContent>> Import(List<IndirectSalesOrderContent> IndirectSalesOrderContents)
        {
            if (!await IndirectSalesOrderContentValidator.Import(IndirectSalesOrderContents))
                return IndirectSalesOrderContents;
            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderContentRepository.BulkMerge(IndirectSalesOrderContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrderContents, new { }, nameof(IndirectSalesOrderContentService));
                return IndirectSalesOrderContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public IndirectSalesOrderContentFilter ToFilter(IndirectSalesOrderContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectSalesOrderContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectSalesOrderContentFilter subFilter = new IndirectSalesOrderContentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.IndirectSalesOrderId))
                        subFilter.IndirectSalesOrderId = Map(subFilter.IndirectSalesOrderId, FilterPermissionDefinition);
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalePrice))
                        subFilter.SalePrice = Map(subFilter.SalePrice, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountPercentage))
                        subFilter.DiscountPercentage = Map(subFilter.DiscountPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountAmount))
                        subFilter.DiscountAmount = Map(subFilter.DiscountAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountPercentage))
                        subFilter.GeneralDiscountPercentage = Map(subFilter.GeneralDiscountPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountAmount))
                        subFilter.GeneralDiscountAmount = Map(subFilter.GeneralDiscountAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Amount))
                        subFilter.Amount = Map(subFilter.Amount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxPercentage))
                        subFilter.TaxPercentage = Map(subFilter.TaxPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxAmount))
                        subFilter.TaxAmount = Map(subFilter.TaxAmount, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
