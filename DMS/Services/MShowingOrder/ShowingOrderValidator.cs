using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MShowingOrder
{
    public interface IShowingOrderValidator : IServiceScoped
    {
        Task<bool> Create(ShowingOrder ShowingOrder);
        Task<bool> Update(ShowingOrder ShowingOrder);
        Task<bool> Delete(ShowingOrder ShowingOrder);
        Task<bool> BulkDelete(List<ShowingOrder> ShowingOrders);
        Task<bool> Import(List<ShowingOrder> ShowingOrders);
    }

    public class ShowingOrderValidator : IShowingOrderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ShowingOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ShowingOrder ShowingOrder)
        {
            ShowingOrderFilter ShowingOrderFilter = new ShowingOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ShowingOrder.Id },
                Selects = ShowingOrderSelect.Id
            };

            int count = await UOW.ShowingOrderRepository.Count(ShowingOrderFilter);
            if (count == 0)
                ShowingOrder.AddError(nameof(ShowingOrderValidator), nameof(ShowingOrder.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ShowingOrder ShowingOrder)
        {
            return ShowingOrder.IsValidated;
        }

        public async Task<bool> Update(ShowingOrder ShowingOrder)
        {
            if (await ValidateId(ShowingOrder))
            {
            }
            return ShowingOrder.IsValidated;
        }

        public async Task<bool> Delete(ShowingOrder ShowingOrder)
        {
            if (await ValidateId(ShowingOrder))
            {
            }
            return ShowingOrder.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ShowingOrder> ShowingOrders)
        {
            foreach (ShowingOrder ShowingOrder in ShowingOrders)
            {
                await Delete(ShowingOrder);
            }
            return ShowingOrders.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ShowingOrder> ShowingOrders)
        {
            return true;
        }
    }
}
