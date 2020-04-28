using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MIndirectSalesOrderPromotion
{
    public interface IIndirectSalesOrderPromotionValidator : IServiceScoped
    {
        Task<bool> Create(IndirectSalesOrderPromotion IndirectSalesOrderPromotion);
        Task<bool> Update(IndirectSalesOrderPromotion IndirectSalesOrderPromotion);
        Task<bool> Delete(IndirectSalesOrderPromotion IndirectSalesOrderPromotion);
        Task<bool> BulkDelete(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions);
        Task<bool> Import(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions);
    }

    public class IndirectSalesOrderPromotionValidator : IIndirectSalesOrderPromotionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectSalesOrderPromotionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrderPromotion.Id },
                Selects = IndirectSalesOrderPromotionSelect.Id
            };

            int count = await UOW.IndirectSalesOrderPromotionRepository.Count(IndirectSalesOrderPromotionFilter);
            if (count == 0)
                IndirectSalesOrderPromotion.AddError(nameof(IndirectSalesOrderPromotionValidator), nameof(IndirectSalesOrderPromotion.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            return IndirectSalesOrderPromotion.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            if (await ValidateId(IndirectSalesOrderPromotion))
            {
            }
            return IndirectSalesOrderPromotion.IsValidated;
        }

        public async Task<bool> Delete(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            if (await ValidateId(IndirectSalesOrderPromotion))
            {
            }
            return IndirectSalesOrderPromotion.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions)
        {
            return true;
        }
        
        public async Task<bool> Import(List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions)
        {
            return true;
        }
    }
}
