using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MCombo
{
    public interface IComboValidator : IServiceScoped
    {
        Task<bool> Create(Combo Combo);
        Task<bool> Update(Combo Combo);
        Task<bool> Delete(Combo Combo);
        Task<bool> BulkDelete(List<Combo> Combos);
        Task<bool> Import(List<Combo> Combos);
    }

    public class ComboValidator : IComboValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ComboValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Combo Combo)
        {
            ComboFilter ComboFilter = new ComboFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Combo.Id },
                Selects = ComboSelect.Id
            };

            int count = await UOW.ComboRepository.Count(ComboFilter);
            if (count == 0)
                Combo.AddError(nameof(ComboValidator), nameof(Combo.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Combo Combo)
        {
            return Combo.IsValidated;
        }

        public async Task<bool> Update(Combo Combo)
        {
            if (await ValidateId(Combo))
            {
            }
            return Combo.IsValidated;
        }

        public async Task<bool> Delete(Combo Combo)
        {
            if (await ValidateId(Combo))
            {
            }
            return Combo.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Combo> Combos)
        {
            foreach (Combo Combo in Combos)
            {
                await Delete(Combo);
            }
            return Combos.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Combo> Combos)
        {
            return true;
        }
    }
}
