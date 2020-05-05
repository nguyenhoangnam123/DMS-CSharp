using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MDirectSalesOrder
{
    public interface IDirectSalesOrderValidator : IServiceScoped
    {
        Task<bool> Create(DirectSalesOrder DirectSalesOrder);
        Task<bool> Update(DirectSalesOrder DirectSalesOrder);
        Task<bool> Delete(DirectSalesOrder DirectSalesOrder);
        Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders);
        Task<bool> Import(List<DirectSalesOrder> DirectSalesOrders);
    }

    public class DirectSalesOrderValidator : IDirectSalesOrderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DirectSalesOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrder.Id },
                Selects = DirectSalesOrderSelect.Id
            };

            int count = await UOW.DirectSalesOrderRepository.Count(DirectSalesOrderFilter);
            if (count == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(DirectSalesOrder DirectSalesOrder)
        {
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Update(DirectSalesOrder DirectSalesOrder)
        {
            if (await ValidateId(DirectSalesOrder))
            {
            }
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Delete(DirectSalesOrder DirectSalesOrder)
        {
            if (await ValidateId(DirectSalesOrder))
            {
            }
            return DirectSalesOrder.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders)
        {
            return true;
        }
        
        public async Task<bool> Import(List<DirectSalesOrder> DirectSalesOrders)
        {
            return true;
        }
    }
}
