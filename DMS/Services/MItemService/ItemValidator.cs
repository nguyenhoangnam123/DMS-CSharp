using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MItem
{
    public interface IItemValidator : IServiceScoped
    {
        Task<bool> Create(Item Item);
        Task<bool> Update(Item Item);
        Task<bool> Delete(Item Item);
        Task<bool> BulkDelete(List<Item> Items);
        Task<bool> Import(List<Item> Items);
    }

    public class ItemValidator : IItemValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ItemValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Item Item)
        {
            ItemFilter ItemFilter = new ItemFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Item.Id },
                Selects = ItemSelect.Id
            };

            int count = await UOW.ItemRepository.Count(ItemFilter);
            if (count == 0)
                Item.AddError(nameof(ItemValidator), nameof(Item.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(Item Item)
        {
            return Item.IsValidated;
        }

        public async Task<bool> Update(Item Item)
        {
            if (await ValidateId(Item))
            {
            }
            return Item.IsValidated;
        }

        public async Task<bool> Delete(Item Item)
        {
            if (await ValidateId(Item))
            {
            }
            return Item.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Item> Items)
        {
            return true;
        }

        public async Task<bool> Import(List<Item> Items)
        {
            return true;
        }
    }
}
