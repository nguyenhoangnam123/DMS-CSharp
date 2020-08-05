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

namespace DMS.Services.MIndirectSalesOrderTransaction
{
    public interface IIndirectSalesOrderTransactionService :  IServiceScoped
    {
        Task<int> Count(IndirectSalesOrderTransactionFilter IndirectSalesOrderTransactionFilter);
        Task<List<IndirectSalesOrderTransaction>> List(IndirectSalesOrderTransactionFilter IndirectSalesOrderTransactionFilter);
        Task<IndirectSalesOrderTransaction> Get(long Id);
        Task<IndirectSalesOrderTransaction> Create(IndirectSalesOrderTransaction IndirectSalesOrderTransaction);
        Task<IndirectSalesOrderTransaction> Update(IndirectSalesOrderTransaction IndirectSalesOrderTransaction);
        Task<IndirectSalesOrderTransaction> Delete(IndirectSalesOrderTransaction IndirectSalesOrderTransaction);
        Task<List<IndirectSalesOrderTransaction>> BulkDelete(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions);
        Task<List<IndirectSalesOrderTransaction>> Import(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions);
        Task<IndirectSalesOrderTransactionFilter> ToFilter(IndirectSalesOrderTransactionFilter IndirectSalesOrderTransactionFilter);
    }

    public class IndirectSalesOrderTransactionService : BaseService, IIndirectSalesOrderTransactionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderTransactionValidator IndirectSalesOrderTransactionValidator;

        public IndirectSalesOrderTransactionService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectSalesOrderTransactionValidator IndirectSalesOrderTransactionValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderTransactionValidator = IndirectSalesOrderTransactionValidator;
        }
        public async Task<int> Count(IndirectSalesOrderTransactionFilter IndirectSalesOrderTransactionFilter)
        {
            try
            {
                int result = await UOW.IndirectSalesOrderTransactionRepository.Count(IndirectSalesOrderTransactionFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<IndirectSalesOrderTransaction>> List(IndirectSalesOrderTransactionFilter IndirectSalesOrderTransactionFilter)
        {
            try
            {
                List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions = await UOW.IndirectSalesOrderTransactionRepository.List(IndirectSalesOrderTransactionFilter);
                return IndirectSalesOrderTransactions;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<IndirectSalesOrderTransaction> Get(long Id)
        {
            IndirectSalesOrderTransaction IndirectSalesOrderTransaction = await UOW.IndirectSalesOrderTransactionRepository.Get(Id);
            if (IndirectSalesOrderTransaction == null)
                return null;
            return IndirectSalesOrderTransaction;
        }
       
        public async Task<IndirectSalesOrderTransaction> Create(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            if (!await IndirectSalesOrderTransactionValidator.Create(IndirectSalesOrderTransaction))
                return IndirectSalesOrderTransaction;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderTransactionRepository.Create(IndirectSalesOrderTransaction);
                await UOW.Commit();
                IndirectSalesOrderTransaction = await UOW.IndirectSalesOrderTransactionRepository.Get(IndirectSalesOrderTransaction.Id);
                await Logging.CreateAuditLog(IndirectSalesOrderTransaction, new { }, nameof(IndirectSalesOrderTransactionService));
                return IndirectSalesOrderTransaction;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<IndirectSalesOrderTransaction> Update(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            if (!await IndirectSalesOrderTransactionValidator.Update(IndirectSalesOrderTransaction))
                return IndirectSalesOrderTransaction;
            try
            {
                var oldData = await UOW.IndirectSalesOrderTransactionRepository.Get(IndirectSalesOrderTransaction.Id);

                await UOW.Begin();
                await UOW.IndirectSalesOrderTransactionRepository.Update(IndirectSalesOrderTransaction);
                await UOW.Commit();

                IndirectSalesOrderTransaction = await UOW.IndirectSalesOrderTransactionRepository.Get(IndirectSalesOrderTransaction.Id);
                await Logging.CreateAuditLog(IndirectSalesOrderTransaction, oldData, nameof(IndirectSalesOrderTransactionService));
                return IndirectSalesOrderTransaction;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<IndirectSalesOrderTransaction> Delete(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            if (!await IndirectSalesOrderTransactionValidator.Delete(IndirectSalesOrderTransaction))
                return IndirectSalesOrderTransaction;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderTransactionRepository.Delete(IndirectSalesOrderTransaction);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrderTransaction, nameof(IndirectSalesOrderTransactionService));
                return IndirectSalesOrderTransaction;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<IndirectSalesOrderTransaction>> BulkDelete(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions)
        {
            if (!await IndirectSalesOrderTransactionValidator.BulkDelete(IndirectSalesOrderTransactions))
                return IndirectSalesOrderTransactions;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderTransactionRepository.BulkDelete(IndirectSalesOrderTransactions);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrderTransactions, nameof(IndirectSalesOrderTransactionService));
                return IndirectSalesOrderTransactions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<IndirectSalesOrderTransaction>> Import(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions)
        {
            if (!await IndirectSalesOrderTransactionValidator.Import(IndirectSalesOrderTransactions))
                return IndirectSalesOrderTransactions;
            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderTransactionRepository.BulkMerge(IndirectSalesOrderTransactions);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrderTransactions, new { }, nameof(IndirectSalesOrderTransactionService));
                return IndirectSalesOrderTransactions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderTransactionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<IndirectSalesOrderTransactionFilter> ToFilter(IndirectSalesOrderTransactionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectSalesOrderTransactionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectSalesOrderTransactionFilter subFilter = new IndirectSalesOrderTransactionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.IndirectSalesOrderId))
                        subFilter.IndirectSalesOrderId = FilterBuilder.Merge(subFilter.IndirectSalesOrderId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.ItemId))
                        subFilter.ItemId = FilterBuilder.Merge(subFilter.ItemId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureId))
                        subFilter.UnitOfMeasureId = FilterBuilder.Merge(subFilter.UnitOfMeasureId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Quantity))
                        
                        subFilter.Quantity = FilterBuilder.Merge(subFilter.Quantity, FilterPermissionDefinition.LongFilter);
                        
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Discount))
                        
                        
                        subFilter.Discount = FilterBuilder.Merge(subFilter.Discount, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Revenue))
                        
                        
                        subFilter.Revenue = FilterBuilder.Merge(subFilter.Revenue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TypeId))
                        subFilter.TypeId = FilterBuilder.Merge(subFilter.TypeId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
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
