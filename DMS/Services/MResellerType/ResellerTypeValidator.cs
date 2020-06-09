using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MResellerType
{
    public interface IResellerTypeValidator : IServiceScoped
    {
        Task<bool> Create(ResellerType ResellerType);
        Task<bool> Update(ResellerType ResellerType);
        Task<bool> Delete(ResellerType ResellerType);
        Task<bool> BulkDelete(List<ResellerType> ResellerTypes);
        Task<bool> Import(List<ResellerType> ResellerTypes);
    }

    public class ResellerTypeValidator : IResellerTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ResellerTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ResellerType ResellerType)
        {
            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ResellerType.Id },
                Selects = ResellerTypeSelect.Id
            };

            int count = await UOW.ResellerTypeRepository.Count(ResellerTypeFilter);
            if (count == 0)
                ResellerType.AddError(nameof(ResellerTypeValidator), nameof(ResellerType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(ResellerType ResellerType)
        {
            return ResellerType.IsValidated;
        }

        public async Task<bool> Update(ResellerType ResellerType)
        {
            if (await ValidateId(ResellerType))
            {
            }
            return ResellerType.IsValidated;
        }

        public async Task<bool> Delete(ResellerType ResellerType)
        {
            if (await ValidateId(ResellerType))
            {
            }
            return ResellerType.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ResellerType> ResellerTypes)
        {
            return true;
        }

        public async Task<bool> Import(List<ResellerType> ResellerTypes)
        {
            return true;
        }
    }
}
