using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MIndirectSalesOrderTransaction
{
    public interface IIndirectSalesOrderTransactionValidator : IServiceScoped
    {
        Task<bool> Create(IndirectSalesOrderTransaction IndirectSalesOrderTransaction);
        Task<bool> Update(IndirectSalesOrderTransaction IndirectSalesOrderTransaction);
        Task<bool> Delete(IndirectSalesOrderTransaction IndirectSalesOrderTransaction);
        Task<bool> BulkDelete(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions);
        Task<bool> Import(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions);
    }

    public class IndirectSalesOrderTransactionValidator : IIndirectSalesOrderTransactionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectSalesOrderTransactionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            IndirectSalesOrderTransactionFilter IndirectSalesOrderTransactionFilter = new IndirectSalesOrderTransactionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrderTransaction.Id },
                Selects = IndirectSalesOrderTransactionSelect.Id
            };

            int count = await UOW.IndirectSalesOrderTransactionRepository.Count(IndirectSalesOrderTransactionFilter);
            if (count == 0)
                IndirectSalesOrderTransaction.AddError(nameof(IndirectSalesOrderTransactionValidator), nameof(IndirectSalesOrderTransaction.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            return IndirectSalesOrderTransaction.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            if (await ValidateId(IndirectSalesOrderTransaction))
            {
            }
            return IndirectSalesOrderTransaction.IsValidated;
        }

        public async Task<bool> Delete(IndirectSalesOrderTransaction IndirectSalesOrderTransaction)
        {
            if (await ValidateId(IndirectSalesOrderTransaction))
            {
            }
            return IndirectSalesOrderTransaction.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions)
        {
            foreach (IndirectSalesOrderTransaction IndirectSalesOrderTransaction in IndirectSalesOrderTransactions)
            {
                await Delete(IndirectSalesOrderTransaction);
            }
            return IndirectSalesOrderTransactions.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<IndirectSalesOrderTransaction> IndirectSalesOrderTransactions)
        {
            return true;
        }
    }
}
