using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MUsedVariation
{
    public interface IUsedVariationValidator : IServiceScoped
    {
        Task<bool> Create(UsedVariation UsedVariation);
        Task<bool> Update(UsedVariation UsedVariation);
        Task<bool> Delete(UsedVariation UsedVariation);
        Task<bool> BulkDelete(List<UsedVariation> UsedVariations);
        Task<bool> Import(List<UsedVariation> UsedVariations);
    }

    public class UsedVariationValidator : IUsedVariationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UsedVariationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UsedVariation UsedVariation)
        {
            UsedVariationFilter UsedVariationFilter = new UsedVariationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UsedVariation.Id },
                Selects = UsedVariationSelect.Id
            };

            int count = await UOW.UsedVariationRepository.Count(UsedVariationFilter);
            if (count == 0)
                UsedVariation.AddError(nameof(UsedVariationValidator), nameof(UsedVariation.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(UsedVariation UsedVariation)
        {
            return UsedVariation.IsValidated;
        }

        public async Task<bool> Update(UsedVariation UsedVariation)
        {
            if (await ValidateId(UsedVariation))
            {
            }
            return UsedVariation.IsValidated;
        }

        public async Task<bool> Delete(UsedVariation UsedVariation)
        {
            if (await ValidateId(UsedVariation))
            {
            }
            return UsedVariation.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<UsedVariation> UsedVariations)
        {
            return true;
        }
        
        public async Task<bool> Import(List<UsedVariation> UsedVariations)
        {
            return true;
        }
    }
}
