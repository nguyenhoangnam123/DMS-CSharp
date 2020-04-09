using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MVariation
{
    public interface IVariationValidator : IServiceScoped
    {
        Task<bool> Create(Variation Variation);
        Task<bool> Update(Variation Variation);
        Task<bool> Delete(Variation Variation);
        Task<bool> BulkDelete(List<Variation> Variations);
        Task<bool> Import(List<Variation> Variations);
    }

    public class VariationValidator : IVariationValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public VariationValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Variation Variation)
        {
            VariationFilter VariationFilter = new VariationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Variation.Id },
                Selects = VariationSelect.Id
            };

            int count = await UOW.VariationRepository.Count(VariationFilter);
            if (count == 0)
                Variation.AddError(nameof(VariationValidator), nameof(Variation.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(Variation Variation)
        {
            return Variation.IsValidated;
        }

        public async Task<bool> Update(Variation Variation)
        {
            if (await ValidateId(Variation))
            {
            }
            return Variation.IsValidated;
        }

        public async Task<bool> Delete(Variation Variation)
        {
            if (await ValidateId(Variation))
            {
            }
            return Variation.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Variation> Variations)
        {
            return true;
        }

        public async Task<bool> Import(List<Variation> Variations)
        {
            return true;
        }
    }
}
