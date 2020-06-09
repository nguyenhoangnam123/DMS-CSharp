using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MGeneralCriteria
{
    public interface IGeneralCriteriaValidator : IServiceScoped
    {
        Task<bool> Create(GeneralCriteria GeneralCriteria);
        Task<bool> Update(GeneralCriteria GeneralCriteria);
        Task<bool> Delete(GeneralCriteria GeneralCriteria);
        Task<bool> BulkDelete(List<GeneralCriteria> GeneralCriterias);
        Task<bool> Import(List<GeneralCriteria> GeneralCriterias);
    }

    public class GeneralCriteriaValidator : IGeneralCriteriaValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public GeneralCriteriaValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(GeneralCriteria GeneralCriteria)
        {
            GeneralCriteriaFilter GeneralCriteriaFilter = new GeneralCriteriaFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = GeneralCriteria.Id },
                Selects = GeneralCriteriaSelect.Id
            };

            int count = await UOW.GeneralCriteriaRepository.Count(GeneralCriteriaFilter);
            if (count == 0)
                GeneralCriteria.AddError(nameof(GeneralCriteriaValidator), nameof(GeneralCriteria.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(GeneralCriteria GeneralCriteria)
        {
            return GeneralCriteria.IsValidated;
        }

        public async Task<bool> Update(GeneralCriteria GeneralCriteria)
        {
            if (await ValidateId(GeneralCriteria))
            {
            }
            return GeneralCriteria.IsValidated;
        }

        public async Task<bool> Delete(GeneralCriteria GeneralCriteria)
        {
            if (await ValidateId(GeneralCriteria))
            {
            }
            return GeneralCriteria.IsValidated;
        }

        public async Task<bool> BulkDelete(List<GeneralCriteria> GeneralCriterias)
        {
            return true;
        }

        public async Task<bool> Import(List<GeneralCriteria> GeneralCriterias)
        {
            return true;
        }
    }
}
