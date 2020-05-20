using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MBanner
{
    public interface IBannerValidator : IServiceScoped
    {
        Task<bool> Create(Banner Banner);
        Task<bool> Update(Banner Banner);
        Task<bool> Delete(Banner Banner);
        Task<bool> BulkDelete(List<Banner> Banners);
        Task<bool> Import(List<Banner> Banners);
    }

    public class BannerValidator : IBannerValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public BannerValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Banner Banner)
        {
            BannerFilter BannerFilter = new BannerFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Banner.Id },
                Selects = BannerSelect.Id
            };

            int count = await UOW.BannerRepository.Count(BannerFilter);
            if (count == 0)
                Banner.AddError(nameof(BannerValidator), nameof(Banner.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Banner Banner)
        {
            return Banner.IsValidated;
        }

        public async Task<bool> Update(Banner Banner)
        {
            if (await ValidateId(Banner))
            {
            }
            return Banner.IsValidated;
        }

        public async Task<bool> Delete(Banner Banner)
        {
            if (await ValidateId(Banner))
            {
            }
            return Banner.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Banner> Banners)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Banner> Banners)
        {
            return true;
        }
    }
}
