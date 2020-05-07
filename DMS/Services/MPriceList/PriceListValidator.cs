using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MPriceList
{
    public interface IPriceListValidator : IServiceScoped
    {
        Task<bool> Create(PriceList PriceList);
        Task<bool> Update(PriceList PriceList);
        Task<bool> Delete(PriceList PriceList);
        Task<bool> BulkDelete(List<PriceList> PriceLists);
        Task<bool> Import(List<PriceList> PriceLists);
    }

    public class PriceListValidator : IPriceListValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PriceListValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(PriceList PriceList)
        {
            PriceListFilter PriceListFilter = new PriceListFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PriceList.Id },
                Selects = PriceListSelect.Id
            };

            int count = await UOW.PriceListRepository.Count(PriceListFilter);
            if (count == 0)
                PriceList.AddError(nameof(PriceListValidator), nameof(PriceList.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(PriceList PriceList)
        {
            return PriceList.IsValidated;
        }

        public async Task<bool> Update(PriceList PriceList)
        {
            if (await ValidateId(PriceList))
            {
            }
            return PriceList.IsValidated;
        }

        public async Task<bool> Delete(PriceList PriceList)
        {
            if (await ValidateId(PriceList))
            {
            }
            return PriceList.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<PriceList> PriceLists)
        {
            return true;
        }
        
        public async Task<bool> Import(List<PriceList> PriceLists)
        {
            return true;
        }
    }
}
