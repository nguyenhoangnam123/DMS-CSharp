using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MDirectSalesOrderPromotion
{
    public interface IDirectSalesOrderPromotionValidator : IServiceScoped
    {
        Task<bool> Create(DirectSalesOrderPromotion DirectSalesOrderPromotion);
        Task<bool> Update(DirectSalesOrderPromotion DirectSalesOrderPromotion);
        Task<bool> Delete(DirectSalesOrderPromotion DirectSalesOrderPromotion);
        Task<bool> BulkDelete(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions);
        Task<bool> Import(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions);
    }

    public class DirectSalesOrderPromotionValidator : IDirectSalesOrderPromotionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DirectSalesOrderPromotionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrderPromotion.Id },
                Selects = DirectSalesOrderPromotionSelect.Id
            };

            int count = await UOW.DirectSalesOrderPromotionRepository.Count(DirectSalesOrderPromotionFilter);
            if (count == 0)
                DirectSalesOrderPromotion.AddError(nameof(DirectSalesOrderPromotionValidator), nameof(DirectSalesOrderPromotion.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            return DirectSalesOrderPromotion.IsValidated;
        }

        public async Task<bool> Update(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            if (await ValidateId(DirectSalesOrderPromotion))
            {
            }
            return DirectSalesOrderPromotion.IsValidated;
        }

        public async Task<bool> Delete(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            if (await ValidateId(DirectSalesOrderPromotion))
            {
            }
            return DirectSalesOrderPromotion.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions)
        {
            return true;
        }
        
        public async Task<bool> Import(List<DirectSalesOrderPromotion> DirectSalesOrderPromotions)
        {
            return true;
        }
    }
}
