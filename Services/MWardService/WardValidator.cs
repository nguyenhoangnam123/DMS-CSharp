using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MWard
{
    public interface IWardValidator : IServiceScoped
    {
        Task<bool> Create(Ward Ward);
        Task<bool> Update(Ward Ward);
        Task<bool> Delete(Ward Ward);
        Task<bool> BulkDelete(List<Ward> Wards);
        Task<bool> Import(List<Ward> Wards);
    }

    public class WardValidator : IWardValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WardValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Ward Ward)
        {
            WardFilter WardFilter = new WardFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Ward.Id },
                Selects = WardSelect.Id
            };

            int count = await UOW.WardRepository.Count(WardFilter);
            if (count == 0)
                Ward.AddError(nameof(WardValidator), nameof(Ward.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Ward Ward)
        {
            return Ward.IsValidated;
        }

        public async Task<bool> Update(Ward Ward)
        {
            if (await ValidateId(Ward))
            {
            }
            return Ward.IsValidated;
        }

        public async Task<bool> Delete(Ward Ward)
        {
            if (await ValidateId(Ward))
            {
            }
            return Ward.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Ward> Wards)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Ward> Wards)
        {
            return true;
        }
    }
}
