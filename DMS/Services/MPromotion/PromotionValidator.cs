using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MPromotion
{
    public interface IPromotionValidator : IServiceScoped
    {
        Task<bool> Create(Promotion Promotion);
        Task<bool> Update(Promotion Promotion);
        Task<bool> Delete(Promotion Promotion);
        Task<bool> BulkDelete(List<Promotion> Promotions);
        Task<bool> Import(List<Promotion> Promotions);
    }

    public class PromotionValidator : IPromotionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PromotionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Promotion Promotion)
        {
            PromotionFilter PromotionFilter = new PromotionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Promotion.Id },
                Selects = PromotionSelect.Id
            };

            int count = await UOW.PromotionRepository.Count(PromotionFilter);
            if (count == 0)
                Promotion.AddError(nameof(PromotionValidator), nameof(Promotion.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Promotion Promotion)
        {
            return Promotion.IsValidated;
        }

        public async Task<bool> Update(Promotion Promotion)
        {
            if (await ValidateId(Promotion))
            {
            }
            return Promotion.IsValidated;
        }

        public async Task<bool> Delete(Promotion Promotion)
        {
            if (await ValidateId(Promotion))
            {
            }
            return Promotion.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Promotion> Promotions)
        {
            foreach (Promotion Promotion in Promotions)
            {
                await Delete(Promotion);
            }
            return Promotions.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Promotion> Promotions)
        {
            return true;
        }
    }
}
