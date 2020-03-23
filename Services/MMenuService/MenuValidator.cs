using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MMenu
{
    public interface IMenuValidator : IServiceScoped
    {
        Task<bool> Create(Menu Menu);
        Task<bool> Update(Menu Menu);
        Task<bool> Delete(Menu Menu);
        Task<bool> BulkDelete(List<Menu> Menus);
        Task<bool> Import(List<Menu> Menus);
    }

    public class MenuValidator : IMenuValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public MenuValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Menu Menu)
        {
            MenuFilter MenuFilter = new MenuFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Menu.Id },
                Selects = MenuSelect.Id
            };

            int count = await UOW.MenuRepository.Count(MenuFilter);
            if (count == 0)
                Menu.AddError(nameof(MenuValidator), nameof(Menu.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Menu Menu)
        {
            return Menu.IsValidated;
        }

        public async Task<bool> Update(Menu Menu)
        {
            if (await ValidateId(Menu))
            {
            }
            return Menu.IsValidated;
        }

        public async Task<bool> Delete(Menu Menu)
        {
            if (await ValidateId(Menu))
            {
            }
            return Menu.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Menu> Menus)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Menu> Menus)
        {
            return true;
        }
    }
}
