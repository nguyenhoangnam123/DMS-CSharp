using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MDirectPriceListType
{
    public interface IDirectPriceListTypeValidator : IServiceScoped
    {
        Task<bool> Create(DirectPriceListType DirectPriceListType);
        Task<bool> Update(DirectPriceListType DirectPriceListType);
        Task<bool> Delete(DirectPriceListType DirectPriceListType);
        Task<bool> BulkDelete(List<DirectPriceListType> DirectPriceListTypes);
        Task<bool> Import(List<DirectPriceListType> DirectPriceListTypes);
    }

    public class DirectPriceListTypeValidator : IDirectPriceListTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DirectPriceListTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectPriceListType DirectPriceListType)
        {
            DirectPriceListTypeFilter DirectPriceListTypeFilter = new DirectPriceListTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectPriceListType.Id },
                Selects = DirectPriceListTypeSelect.Id
            };

            int count = await UOW.DirectPriceListTypeRepository.Count(DirectPriceListTypeFilter);
            if (count == 0)
                DirectPriceListType.AddError(nameof(DirectPriceListTypeValidator), nameof(DirectPriceListType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(DirectPriceListType DirectPriceListType)
        {
            return DirectPriceListType.IsValidated;
        }

        public async Task<bool> Update(DirectPriceListType DirectPriceListType)
        {
            if (await ValidateId(DirectPriceListType))
            {
            }
            return DirectPriceListType.IsValidated;
        }

        public async Task<bool> Delete(DirectPriceListType DirectPriceListType)
        {
            if (await ValidateId(DirectPriceListType))
            {
            }
            return DirectPriceListType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<DirectPriceListType> DirectPriceListTypes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<DirectPriceListType> DirectPriceListTypes)
        {
            return true;
        }
    }
}
