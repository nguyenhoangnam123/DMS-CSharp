using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MProblemStatus
{
    public interface IProblemStatusValidator : IServiceScoped
    {
        Task<bool> Create(ProblemStatus ProblemStatus);
        Task<bool> Update(ProblemStatus ProblemStatus);
        Task<bool> Delete(ProblemStatus ProblemStatus);
        Task<bool> BulkDelete(List<ProblemStatus> ProblemStatuses);
        Task<bool> Import(List<ProblemStatus> ProblemStatuses);
    }

    public class ProblemStatusValidator : IProblemStatusValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProblemStatusValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ProblemStatus ProblemStatus)
        {
            ProblemStatusFilter ProblemStatusFilter = new ProblemStatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ProblemStatus.Id },
                Selects = ProblemStatusSelect.Id
            };

            int count = await UOW.ProblemStatusRepository.Count(ProblemStatusFilter);
            if (count == 0)
                ProblemStatus.AddError(nameof(ProblemStatusValidator), nameof(ProblemStatus.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(ProblemStatus ProblemStatus)
        {
            return ProblemStatus.IsValidated;
        }

        public async Task<bool> Update(ProblemStatus ProblemStatus)
        {
            if (await ValidateId(ProblemStatus))
            {
            }
            return ProblemStatus.IsValidated;
        }

        public async Task<bool> Delete(ProblemStatus ProblemStatus)
        {
            if (await ValidateId(ProblemStatus))
            {
            }
            return ProblemStatus.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ProblemStatus> ProblemStatuses)
        {
            return true;
        }

        public async Task<bool> Import(List<ProblemStatus> ProblemStatuses)
        {
            return true;
        }
    }
}
