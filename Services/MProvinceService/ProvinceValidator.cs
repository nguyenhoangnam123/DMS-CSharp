using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MProvince
{
    public interface IProvinceValidator : IServiceScoped
    {
        Task<bool> Create(Province Province);
        Task<bool> Update(Province Province);
        Task<bool> Delete(Province Province);
        Task<bool> BulkDelete(List<Province> Provinces);
        Task<bool> Import(List<Province> Provinces);
    }

    public class ProvinceValidator : IProvinceValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProvinceValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Province Province)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Province.Id },
                Selects = ProvinceSelect.Id
            };

            int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
            if (count == 0)
                Province.AddError(nameof(ProvinceValidator), nameof(Province.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Province Province)
        {
            return Province.IsValidated;
        }

        public async Task<bool> Update(Province Province)
        {
            if (await ValidateId(Province))
            {
            }
            return Province.IsValidated;
        }

        public async Task<bool> Delete(Province Province)
        {
            if (await ValidateId(Province))
            {
            }
            return Province.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Province> Provinces)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Province> Provinces)
        {
            return true;
        }
    }
}
