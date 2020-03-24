using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MProductType
{
    public interface IProductTypeValidator : IServiceScoped
    {
        Task<bool> Create(ProductType ProductType);
        Task<bool> Update(ProductType ProductType);
        Task<bool> Delete(ProductType ProductType);
        Task<bool> BulkDelete(List<ProductType> ProductTypes);
        Task<bool> Import(List<ProductType> ProductTypes);
    }

    public class ProductTypeValidator : IProductTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProductTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ProductType ProductType)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ProductType.Id },
                Selects = ProductTypeSelect.Id
            };

            int count = await UOW.ProductTypeRepository.Count(ProductTypeFilter);
            if (count == 0)
                ProductType.AddError(nameof(ProductTypeValidator), nameof(ProductType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ProductType ProductType)
        {
            return ProductType.IsValidated;
        }

        public async Task<bool> Update(ProductType ProductType)
        {
            if (await ValidateId(ProductType))
            {
            }
            return ProductType.IsValidated;
        }

        public async Task<bool> Delete(ProductType ProductType)
        {
            if (await ValidateId(ProductType))
            {
            }
            return ProductType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ProductType> ProductTypes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ProductType> ProductTypes)
        {
            return true;
        }
    }
}
