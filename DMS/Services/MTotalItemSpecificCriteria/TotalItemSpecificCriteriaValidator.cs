using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MTotalItemSpecificCriteria
{
    public interface ITotalItemSpecificCriteriaValidator : IServiceScoped
    {
        Task<bool> Create(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<bool> Update(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<bool> Delete(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<bool> BulkDelete(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias);
        Task<bool> Import(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias);
    }

    public class TotalItemSpecificCriteriaValidator : ITotalItemSpecificCriteriaValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public TotalItemSpecificCriteriaValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter = new TotalItemSpecificCriteriaFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = TotalItemSpecificCriteria.Id },
                Selects = TotalItemSpecificCriteriaSelect.Id
            };

            int count = await UOW.TotalItemSpecificCriteriaRepository.Count(TotalItemSpecificCriteriaFilter);
            if (count == 0)
                TotalItemSpecificCriteria.AddError(nameof(TotalItemSpecificCriteriaValidator), nameof(TotalItemSpecificCriteria.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            return TotalItemSpecificCriteria.IsValidated;
        }

        public async Task<bool> Update(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            if (await ValidateId(TotalItemSpecificCriteria))
            {
            }
            return TotalItemSpecificCriteria.IsValidated;
        }

        public async Task<bool> Delete(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            if (await ValidateId(TotalItemSpecificCriteria))
            {
            }
            return TotalItemSpecificCriteria.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias)
        {
            return true;
        }
        
        public async Task<bool> Import(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias)
        {
            return true;
        }
    }
}
