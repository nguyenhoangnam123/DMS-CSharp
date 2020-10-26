using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MResellerStatus
{
    public interface IResellerStatusValidator : IServiceScoped
    {
        Task<bool> Create(ResellerStatus ResellerStatus);
        Task<bool> Update(ResellerStatus ResellerStatus);
        Task<bool> Delete(ResellerStatus ResellerStatus);
        Task<bool> BulkDelete(List<ResellerStatus> ResellerStatuses);
        Task<bool> Import(List<ResellerStatus> ResellerStatuses);
    }

    public class ResellerStatusValidator : IResellerStatusValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ResellerStatusValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ResellerStatus ResellerStatus)
        {
            ResellerStatusFilter ResellerStatusFilter = new ResellerStatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ResellerStatus.Id },
                Selects = ResellerStatusSelect.Id
            };

            int count = await UOW.ResellerStatusRepository.Count(ResellerStatusFilter);
            if (count == 0)
                ResellerStatus.AddError(nameof(ResellerStatusValidator), nameof(ResellerStatus.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(ResellerStatus ResellerStatus)
        {
            return ResellerStatus.IsValidated;
        }

        public async Task<bool> Update(ResellerStatus ResellerStatus)
        {
            if (await ValidateId(ResellerStatus))
            {
            }
            return ResellerStatus.IsValidated;
        }

        public async Task<bool> Delete(ResellerStatus ResellerStatus)
        {
            if (await ValidateId(ResellerStatus))
            {
            }
            return ResellerStatus.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ResellerStatus> ResellerStatuses)
        {
            return true;
        }

        public async Task<bool> Import(List<ResellerStatus> ResellerStatuses)
        {
            return true;
        }
    }
}
