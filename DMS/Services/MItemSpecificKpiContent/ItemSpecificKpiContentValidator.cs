using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MItemSpecificKpiContent
{
    public interface IItemSpecificKpiContentValidator : IServiceScoped
    {
        Task<bool> Create(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<bool> Update(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<bool> Delete(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<bool> BulkDelete(List<ItemSpecificKpiContent> ItemSpecificKpiContents);
        Task<bool> Import(List<ItemSpecificKpiContent> ItemSpecificKpiContents);
    }

    public class ItemSpecificKpiContentValidator : IItemSpecificKpiContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ItemSpecificKpiContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ItemSpecificKpiContent.Id },
                Selects = ItemSpecificKpiContentSelect.Id
            };

            int count = await UOW.ItemSpecificKpiContentRepository.Count(ItemSpecificKpiContentFilter);
            if (count == 0)
                ItemSpecificKpiContent.AddError(nameof(ItemSpecificKpiContentValidator), nameof(ItemSpecificKpiContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            return ItemSpecificKpiContent.IsValidated;
        }

        public async Task<bool> Update(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            if (await ValidateId(ItemSpecificKpiContent))
            {
            }
            return ItemSpecificKpiContent.IsValidated;
        }

        public async Task<bool> Delete(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            if (await ValidateId(ItemSpecificKpiContent))
            {
            }
            return ItemSpecificKpiContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ItemSpecificKpiContent> ItemSpecificKpiContents)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ItemSpecificKpiContent> ItemSpecificKpiContents)
        {
            return true;
        }
    }
}
