using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MProductGrouping
{
    public interface IProductGroupingValidator : IServiceScoped
    {
        Task<bool> Create(ProductGrouping ProductGrouping);
        Task<bool> Update(ProductGrouping ProductGrouping);
        Task<bool> Delete(ProductGrouping ProductGrouping);
        Task<bool> BulkDelete(List<ProductGrouping> ProductGroupings);
        Task<bool> Import(List<ProductGrouping> ProductGroupings);
    }

    public class ProductGroupingValidator : IProductGroupingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProductGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ProductGrouping ProductGrouping)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ProductGrouping.Id },
                Selects = ProductGroupingSelect.Id
            };

            int count = await UOW.ProductGroupingRepository.Count(ProductGroupingFilter);
            if (count == 0)
                ProductGrouping.AddError(nameof(ProductGroupingValidator), nameof(ProductGrouping.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(ProductGrouping ProductGrouping)
        {
            return ProductGrouping.IsValidated;
        }

        public async Task<bool> Update(ProductGrouping ProductGrouping)
        {
            if (await ValidateId(ProductGrouping))
            {
            }
            return ProductGrouping.IsValidated;
        }

        public async Task<bool> Delete(ProductGrouping ProductGrouping)
        {
            if (await ValidateId(ProductGrouping))
            {
            }
            return ProductGrouping.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ProductGrouping> ProductGroupings)
        {
            return true;
        }

        public async Task<bool> Import(List<ProductGrouping> ProductGroupings)
        {
            return true;
        }
    }
}
