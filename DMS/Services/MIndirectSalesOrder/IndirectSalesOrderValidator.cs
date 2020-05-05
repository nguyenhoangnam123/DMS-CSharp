using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MIndirectSalesOrder
{
    public interface IIndirectSalesOrderValidator : IServiceScoped
    {
        Task<bool> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<bool> Import(List<IndirectSalesOrder> IndirectSalesOrders);
    }

    public class IndirectSalesOrderValidator : IIndirectSalesOrderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectSalesOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrder.Id },
                Selects = IndirectSalesOrderSelect.Id
            };

            int count = await UOW.IndirectSalesOrderRepository.Count(IndirectSalesOrderFilter);
            if (count == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateStore(IndirectSalesOrder IndirectSalesOrder)
        {
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { In = new List<long> { IndirectSalesOrder.BuyerStoreId } },
                Selects = StoreSelect.Id
            };

            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool>Create(IndirectSalesOrder IndirectSalesOrder)
        {
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            if (await ValidateId(IndirectSalesOrder))
            {
            }
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder)
        {
            if (await ValidateId(IndirectSalesOrder))
            {
            }
            return IndirectSalesOrder.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            return true;
        }
        
        public async Task<bool> Import(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            return true;
        }
    }
}
