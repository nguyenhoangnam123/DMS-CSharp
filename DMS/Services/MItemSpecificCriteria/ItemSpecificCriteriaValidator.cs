using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MItemSpecificCriteria
{
    public interface IItemSpecificCriteriaValidator : IServiceScoped
    {
        Task<bool> Create(ItemSpecificCriteria ItemSpecificCriteria);
        Task<bool> Update(ItemSpecificCriteria ItemSpecificCriteria);
        Task<bool> Delete(ItemSpecificCriteria ItemSpecificCriteria);
        Task<bool> BulkDelete(List<ItemSpecificCriteria> ItemSpecificCriterias);
        Task<bool> Import(List<ItemSpecificCriteria> ItemSpecificCriterias);
    }

    public class ItemSpecificCriteriaValidator : IItemSpecificCriteriaValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ItemSpecificCriteriaValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ItemSpecificCriteria ItemSpecificCriteria)
        {
            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ItemSpecificCriteria.Id },
                Selects = ItemSpecificCriteriaSelect.Id
            };

            int count = await UOW.ItemSpecificCriteriaRepository.Count(ItemSpecificCriteriaFilter);
            if (count == 0)
                ItemSpecificCriteria.AddError(nameof(ItemSpecificCriteriaValidator), nameof(ItemSpecificCriteria.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ItemSpecificCriteria ItemSpecificCriteria)
        {
            return ItemSpecificCriteria.IsValidated;
        }

        public async Task<bool> Update(ItemSpecificCriteria ItemSpecificCriteria)
        {
            if (await ValidateId(ItemSpecificCriteria))
            {
            }
            return ItemSpecificCriteria.IsValidated;
        }

        public async Task<bool> Delete(ItemSpecificCriteria ItemSpecificCriteria)
        {
            if (await ValidateId(ItemSpecificCriteria))
            {
            }
            return ItemSpecificCriteria.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ItemSpecificCriteria> ItemSpecificCriterias)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ItemSpecificCriteria> ItemSpecificCriterias)
        {
            return true;
        }
    }
}
