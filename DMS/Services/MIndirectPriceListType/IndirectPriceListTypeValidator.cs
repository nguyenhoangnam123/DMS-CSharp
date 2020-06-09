using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MIndirectPriceListType
{
    public interface IIndirectPriceListTypeValidator : IServiceScoped
    {
        Task<bool> Create(IndirectPriceListType IndirectPriceListType);
        Task<bool> Update(IndirectPriceListType IndirectPriceListType);
        Task<bool> Delete(IndirectPriceListType IndirectPriceListType);
        Task<bool> BulkDelete(List<IndirectPriceListType> IndirectPriceListTypes);
        Task<bool> Import(List<IndirectPriceListType> IndirectPriceListTypes);
    }

    public class IndirectPriceListTypeValidator : IIndirectPriceListTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectPriceListTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectPriceListType IndirectPriceListType)
        {
            IndirectPriceListTypeFilter IndirectPriceListTypeFilter = new IndirectPriceListTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectPriceListType.Id },
                Selects = IndirectPriceListTypeSelect.Id
            };

            int count = await UOW.IndirectPriceListTypeRepository.Count(IndirectPriceListTypeFilter);
            if (count == 0)
                IndirectPriceListType.AddError(nameof(IndirectPriceListTypeValidator), nameof(IndirectPriceListType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(IndirectPriceListType IndirectPriceListType)
        {
            return IndirectPriceListType.IsValidated;
        }

        public async Task<bool> Update(IndirectPriceListType IndirectPriceListType)
        {
            if (await ValidateId(IndirectPriceListType))
            {
            }
            return IndirectPriceListType.IsValidated;
        }

        public async Task<bool> Delete(IndirectPriceListType IndirectPriceListType)
        {
            if (await ValidateId(IndirectPriceListType))
            {
            }
            return IndirectPriceListType.IsValidated;
        }

        public async Task<bool> BulkDelete(List<IndirectPriceListType> IndirectPriceListTypes)
        {
            return true;
        }

        public async Task<bool> Import(List<IndirectPriceListType> IndirectPriceListTypes)
        {
            return true;
        }
    }
}
