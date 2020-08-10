using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MProblemType
{
    public interface IProblemTypeValidator : IServiceScoped
    {
        Task<bool> Create(ProblemType ProblemType);
        Task<bool> Update(ProblemType ProblemType);
        Task<bool> Delete(ProblemType ProblemType);
        Task<bool> BulkDelete(List<ProblemType> ProblemTypes);
        Task<bool> Import(List<ProblemType> ProblemTypes);
    }

    public class ProblemTypeValidator : IProblemTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProblemTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ProblemType ProblemType)
        {
            ProblemTypeFilter ProblemTypeFilter = new ProblemTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ProblemType.Id },
                Selects = ProblemTypeSelect.Id
            };

            int count = await UOW.ProblemTypeRepository.Count(ProblemTypeFilter);
            if (count == 0)
                ProblemType.AddError(nameof(ProblemTypeValidator), nameof(ProblemType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ProblemType ProblemType)
        {
            return ProblemType.IsValidated;
        }

        public async Task<bool> Update(ProblemType ProblemType)
        {
            if (await ValidateId(ProblemType))
            {
            }
            return ProblemType.IsValidated;
        }

        public async Task<bool> Delete(ProblemType ProblemType)
        {
            if (await ValidateId(ProblemType))
            {
            }
            return ProblemType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ProblemType> ProblemTypes)
        {
            foreach (ProblemType ProblemType in ProblemTypes)
            {
                await Delete(ProblemType);
            }
            return ProblemTypes.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ProblemType> ProblemTypes)
        {
            return true;
        }
    }
}
