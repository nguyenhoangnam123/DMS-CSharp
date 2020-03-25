using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MDistrict
{
    public interface IDistrictValidator : IServiceScoped
    {
        Task<bool> Create(District District);
        Task<bool> Update(District District);
        Task<bool> Delete(District District);
        Task<bool> BulkDelete(List<District> Districts);
        Task<bool> Import(List<District> Districts);
    }

    public class DistrictValidator : IDistrictValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DistrictValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(District District)
        {
            DistrictFilter DistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = District.Id },
                Selects = DistrictSelect.Id
            };

            int count = await UOW.DistrictRepository.Count(DistrictFilter);
            if (count == 0)
                District.AddError(nameof(DistrictValidator), nameof(District.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(District District)
        {
            return District.IsValidated;
        }

        public async Task<bool> Update(District District)
        {
            if (await ValidateId(District))
            {
            }
            return District.IsValidated;
        }

        public async Task<bool> Delete(District District)
        {
            if (await ValidateId(District))
            {
            }
            return District.IsValidated;
        }

        public async Task<bool> BulkDelete(List<District> Districts)
        {
            return true;
        }

        public async Task<bool> Import(List<District> Districts)
        {
            return true;
        }
    }
}
