using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MIndirectSalesOrderStatus
{
    public interface IIndirectSalesOrderStatusValidator : IServiceScoped
    {
        Task<bool> Create(IndirectSalesOrderStatus IndirectSalesOrderStatus);
        Task<bool> Update(IndirectSalesOrderStatus IndirectSalesOrderStatus);
        Task<bool> Delete(IndirectSalesOrderStatus IndirectSalesOrderStatus);
        Task<bool> BulkDelete(List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses);
        Task<bool> Import(List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses);
    }

    public class IndirectSalesOrderStatusValidator : IIndirectSalesOrderStatusValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectSalesOrderStatusValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectSalesOrderStatus IndirectSalesOrderStatus)
        {
            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrderStatus.Id },
                Selects = IndirectSalesOrderStatusSelect.Id
            };

            int count = await UOW.IndirectSalesOrderStatusRepository.Count(IndirectSalesOrderStatusFilter);
            if (count == 0)
                IndirectSalesOrderStatus.AddError(nameof(IndirectSalesOrderStatusValidator), nameof(IndirectSalesOrderStatus.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(IndirectSalesOrderStatus IndirectSalesOrderStatus)
        {
            return IndirectSalesOrderStatus.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrderStatus IndirectSalesOrderStatus)
        {
            if (await ValidateId(IndirectSalesOrderStatus))
            {
            }
            return IndirectSalesOrderStatus.IsValidated;
        }

        public async Task<bool> Delete(IndirectSalesOrderStatus IndirectSalesOrderStatus)
        {
            if (await ValidateId(IndirectSalesOrderStatus))
            {
            }
            return IndirectSalesOrderStatus.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses)
        {
            return true;
        }
        
        public async Task<bool> Import(List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses)
        {
            return true;
        }
    }
}
