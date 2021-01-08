using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProductType
{
    public interface IProductTypeService : IServiceScoped
    {
        Task<int> Count(ProductTypeFilter ProductTypeFilter);
        Task<List<ProductType>> List(ProductTypeFilter ProductTypeFilter);
        Task<ProductType> Get(long Id);
    }

    public class ProductTypeService : BaseService, IProductTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductTypeValidator ProductTypeValidator;

        public ProductTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductTypeValidator ProductTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProductTypeValidator = ProductTypeValidator;
        }
        public async Task<int> Count(ProductTypeFilter ProductTypeFilter)
        {
            try
            {
                int result = await UOW.ProductTypeRepository.Count(ProductTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ProductType>> List(ProductTypeFilter ProductTypeFilter)
        {
            try
            {
                List<ProductType> ProductTypes = await UOW.ProductTypeRepository.List(ProductTypeFilter);
                return ProductTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<ProductType> Get(long Id)
        {
            ProductType ProductType = await UOW.ProductTypeRepository.Get(Id);
            if (ProductType == null)
                return null;
            return ProductType;
        }
    }
}
