using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS.Repositories;

namespace DMS.Services.MRewardHistory
{
    public interface IRewardHistoryValidator : IServiceScoped
    {
        Task<bool> Create(RewardHistory RewardHistory);
        Task<bool> Update(RewardHistory RewardHistory);
        Task<bool> Delete(RewardHistory RewardHistory);
        Task<bool> BulkDelete(List<RewardHistory> RewardHistories);
        Task<bool> Import(List<RewardHistory> RewardHistories);
    }

    public class RewardHistoryValidator : IRewardHistoryValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            RevenueEmpty,
            RevenueNotEnough
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public RewardHistoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(RewardHistory RewardHistory)
        {
            RewardHistoryFilter RewardHistoryFilter = new RewardHistoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = RewardHistory.Id },
                Selects = RewardHistorySelect.Id
            };

            int count = await UOW.RewardHistoryRepository.Count(RewardHistoryFilter);
            if (count == 0)
                RewardHistory.AddError(nameof(RewardHistoryValidator), nameof(RewardHistory.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateRevenue(RewardHistory RewardHistory)
        {
            if (RewardHistory.Revenue <= 0)
            {
                RewardHistory.AddError(nameof(RewardHistoryValidator), nameof(RewardHistory.Revenue), ErrorCode.RevenueEmpty);
            }
            else if (RewardHistory.Revenue < 20000000)
            {
                RewardHistory.AddError(nameof(RewardHistoryValidator), nameof(RewardHistory.Revenue), ErrorCode.RevenueNotEnough);
            }
            return RewardHistory.IsValidated;
        }

        public async Task<bool> Create(RewardHistory RewardHistory)
        {
            await ValidateRevenue(RewardHistory);
            return RewardHistory.IsValidated;
        }

        public async Task<bool> Update(RewardHistory RewardHistory)
        {
            if (await ValidateId(RewardHistory))
            {
            }
            return RewardHistory.IsValidated;
        }

        public async Task<bool> Delete(RewardHistory RewardHistory)
        {
            if (await ValidateId(RewardHistory))
            {
            }
            return RewardHistory.IsValidated;
        }

        public async Task<bool> BulkDelete(List<RewardHistory> RewardHistories)
        {
            foreach (RewardHistory RewardHistory in RewardHistories)
            {
                await Delete(RewardHistory);
            }
            return RewardHistories.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<RewardHistory> RewardHistories)
        {
            return true;
        }
    }
}
