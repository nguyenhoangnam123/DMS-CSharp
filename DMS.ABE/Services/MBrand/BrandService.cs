using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MBrand
{
    public interface IBrandService : IServiceScoped
    {
        Task<int> Count(BrandFilter BrandFilter);
        Task<List<Brand>> List(BrandFilter BrandFilter);
        Task<Brand> Get(long Id);
    }

    public class BrandService : BaseService, IBrandService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IBrandValidator BrandValidator;

        public BrandService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IBrandValidator BrandValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.BrandValidator = BrandValidator;
        }
        public async Task<int> Count(BrandFilter BrandFilter)
        {
            try
            {
                int result = await UOW.BrandRepository.Count(BrandFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Brand>> List(BrandFilter BrandFilter)
        {
            try
            {
                List<Brand> Brands = await UOW.BrandRepository.List(BrandFilter);
                return Brands;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BrandService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Brand> Get(long Id)
        {
            Brand Brand = await UOW.BrandRepository.Get(Id);
            if (Brand == null)
                return null;
            return Brand;
        }
    }
}
