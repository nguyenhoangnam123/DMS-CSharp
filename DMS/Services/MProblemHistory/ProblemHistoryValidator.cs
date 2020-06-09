using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MProblemHistory
{
    public interface IProblemHistoryValidator : IServiceScoped
    {
        Task<bool> Create(ProblemHistory ProblemHistory);
        Task<bool> Update(ProblemHistory ProblemHistory);
        Task<bool> Delete(ProblemHistory ProblemHistory);
        Task<bool> BulkDelete(List<ProblemHistory> ProblemHistories);
        Task<bool> Import(List<ProblemHistory> ProblemHistories);
    }

    public class ProblemHistoryValidator : IProblemHistoryValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProblemHistoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ProblemHistory ProblemHistory)
        {
            ProblemHistoryFilter ProblemHistoryFilter = new ProblemHistoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ProblemHistory.Id },
                Selects = ProblemHistorySelect.Id
            };

            int count = await UOW.ProblemHistoryRepository.Count(ProblemHistoryFilter);
            if (count == 0)
                ProblemHistory.AddError(nameof(ProblemHistoryValidator), nameof(ProblemHistory.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ProblemHistory ProblemHistory)
        {
            return ProblemHistory.IsValidated;
        }

        public async Task<bool> Update(ProblemHistory ProblemHistory)
        {
            if (await ValidateId(ProblemHistory))
            {
            }
            return ProblemHistory.IsValidated;
        }

        public async Task<bool> Delete(ProblemHistory ProblemHistory)
        {
            if (await ValidateId(ProblemHistory))
            {
            }
            return ProblemHistory.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ProblemHistory> ProblemHistories)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ProblemHistory> ProblemHistories)
        {
            return true;
        }
    }
}
