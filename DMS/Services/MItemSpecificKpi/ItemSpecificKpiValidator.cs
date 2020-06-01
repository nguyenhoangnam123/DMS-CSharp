using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MItemSpecificKpi
{
    public interface IItemSpecificKpiValidator : IServiceScoped
    {
        Task<bool> Create(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> Update(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> Delete(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis);
        Task<bool> Import(List<ItemSpecificKpi> ItemSpecificKpis);
    }

    public class ItemSpecificKpiValidator : IItemSpecificKpiValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ItemSpecificKpiValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ItemSpecificKpi ItemSpecificKpi)
        {
            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ItemSpecificKpi.Id },
                Selects = ItemSpecificKpiSelect.Id
            };

            int count = await UOW.ItemSpecificKpiRepository.Count(ItemSpecificKpiFilter);
            if (count == 0)
                ItemSpecificKpi.AddError(nameof(ItemSpecificKpiValidator), nameof(ItemSpecificKpi.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ItemSpecificKpi ItemSpecificKpi)
        {
            return ItemSpecificKpi.IsValidated;
        }

        public async Task<bool> Update(ItemSpecificKpi ItemSpecificKpi)
        {
            if (await ValidateId(ItemSpecificKpi))
            {
            }
            return ItemSpecificKpi.IsValidated;
        }

        public async Task<bool> Delete(ItemSpecificKpi ItemSpecificKpi)
        {
            if (await ValidateId(ItemSpecificKpi))
            {
            }
            return ItemSpecificKpi.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            return true;
        }
    }
}
