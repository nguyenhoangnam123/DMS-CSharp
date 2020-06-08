using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MDirectPriceList
{
    public interface IDirectPriceListValidator : IServiceScoped
    {
        Task<bool> Create(DirectPriceList DirectPriceList);
        Task<bool> Update(DirectPriceList DirectPriceList);
        Task<bool> Delete(DirectPriceList DirectPriceList);
        Task<bool> BulkDelete(List<DirectPriceList> DirectPriceLists);
        Task<bool> Import(List<DirectPriceList> DirectPriceLists);
    }

    public class DirectPriceListValidator : IDirectPriceListValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DirectPriceListValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectPriceList DirectPriceList)
        {
            DirectPriceListFilter DirectPriceListFilter = new DirectPriceListFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectPriceList.Id },
                Selects = DirectPriceListSelect.Id
            };

            int count = await UOW.DirectPriceListRepository.Count(DirectPriceListFilter);
            if (count == 0)
                DirectPriceList.AddError(nameof(DirectPriceListValidator), nameof(DirectPriceList.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(DirectPriceList DirectPriceList)
        {
            return DirectPriceList.IsValidated;
        }

        public async Task<bool> Update(DirectPriceList DirectPriceList)
        {
            if (await ValidateId(DirectPriceList))
            {
            }
            return DirectPriceList.IsValidated;
        }

        public async Task<bool> Delete(DirectPriceList DirectPriceList)
        {
            if (await ValidateId(DirectPriceList))
            {
            }
            return DirectPriceList.IsValidated;
        }

        public async Task<bool> BulkDelete(List<DirectPriceList> DirectPriceLists)
        {
            return true;
        }

        public async Task<bool> Import(List<DirectPriceList> DirectPriceLists)
        {
            return true;
        }
    }
}
