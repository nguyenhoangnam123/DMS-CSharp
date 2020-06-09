using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MIndirectPriceList
{
    public interface IIndirectPriceListValidator : IServiceScoped
    {
        Task<bool> Create(IndirectPriceList IndirectPriceList);
        Task<bool> Update(IndirectPriceList IndirectPriceList);
        Task<bool> Delete(IndirectPriceList IndirectPriceList);
        Task<bool> BulkDelete(List<IndirectPriceList> IndirectPriceLists);
        Task<bool> Import(List<IndirectPriceList> IndirectPriceLists);
    }

    public class IndirectPriceListValidator : IIndirectPriceListValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectPriceListValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectPriceList IndirectPriceList)
        {
            IndirectPriceListFilter IndirectPriceListFilter = new IndirectPriceListFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectPriceList.Id },
                Selects = IndirectPriceListSelect.Id
            };

            int count = await UOW.IndirectPriceListRepository.Count(IndirectPriceListFilter);
            if (count == 0)
                IndirectPriceList.AddError(nameof(IndirectPriceListValidator), nameof(IndirectPriceList.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(IndirectPriceList IndirectPriceList)
        {
            return IndirectPriceList.IsValidated;
        }

        public async Task<bool> Update(IndirectPriceList IndirectPriceList)
        {
            if (await ValidateId(IndirectPriceList))
            {
            }
            return IndirectPriceList.IsValidated;
        }

        public async Task<bool> Delete(IndirectPriceList IndirectPriceList)
        {
            if (await ValidateId(IndirectPriceList))
            {
            }
            return IndirectPriceList.IsValidated;
        }

        public async Task<bool> BulkDelete(List<IndirectPriceList> IndirectPriceLists)
        {
            return true;
        }

        public async Task<bool> Import(List<IndirectPriceList> IndirectPriceLists)
        {
            return true;
        }
    }
}
