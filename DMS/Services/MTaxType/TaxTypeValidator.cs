using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MTaxType
{
    public interface ITaxTypeValidator : IServiceScoped
    {
        Task<bool> Create(TaxType TaxType);
        Task<bool> Update(TaxType TaxType);
        Task<bool> Delete(TaxType TaxType);
        Task<bool> BulkDelete(List<TaxType> TaxTypes);
        Task<bool> Import(List<TaxType> TaxTypes);
    }

    public class TaxTypeValidator : ITaxTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            TaxTypeInUsed
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public TaxTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(TaxType TaxType)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = TaxType.Id },
                Selects = TaxTypeSelect.Id
            };

            int count = await UOW.TaxTypeRepository.Count(TaxTypeFilter);
            if (count == 0)
                TaxType.AddError(nameof(TaxTypeValidator), nameof(TaxType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateTaxTypeInUsed(TaxType TaxType)
        {
            ProductFilter ProductFilter = new ProductFilter
            {
                TaxTypeId = new IdFilter { Equal = TaxType.Id },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
            };
            int count = await UOW.ProductRepository.Count(ProductFilter);
            if (count > 0)
                TaxType.AddError(nameof(TaxTypeValidator), nameof(TaxType.Id), ErrorCode.TaxTypeInUsed);

            return TaxType.IsValidated;
        }

        public async Task<bool> Create(TaxType TaxType)
        {
            return TaxType.IsValidated;
        }

        public async Task<bool> Update(TaxType TaxType)
        {
            if (await ValidateId(TaxType))
            {
            }
            return TaxType.IsValidated;
        }

        public async Task<bool> Delete(TaxType TaxType)
        {
            if (await ValidateId(TaxType))
            {
                await ValidateTaxTypeInUsed(TaxType);
            }
            return TaxType.IsValidated;
        }

        public async Task<bool> BulkDelete(List<TaxType> TaxTypes)
        {
            return true;
        }

        public async Task<bool> Import(List<TaxType> TaxTypes)
        {
            return true;
        }
    }
}
