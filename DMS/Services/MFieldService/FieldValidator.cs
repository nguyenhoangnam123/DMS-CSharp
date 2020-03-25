using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MField
{
    public interface IFieldValidator : IServiceScoped
    {
        Task<bool> Create(Field Field);
        Task<bool> Update(Field Field);
        Task<bool> Delete(Field Field);
        Task<bool> BulkDelete(List<Field> Fields);
        Task<bool> Import(List<Field> Fields);
    }

    public class FieldValidator : IFieldValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public FieldValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Field Field)
        {
            FieldFilter FieldFilter = new FieldFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Field.Id },
                Selects = FieldSelect.Id
            };

            int count = await UOW.FieldRepository.Count(FieldFilter);
            if (count == 0)
                Field.AddError(nameof(FieldValidator), nameof(Field.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(Field Field)
        {
            return Field.IsValidated;
        }

        public async Task<bool> Update(Field Field)
        {
            if (await ValidateId(Field))
            {
            }
            return Field.IsValidated;
        }

        public async Task<bool> Delete(Field Field)
        {
            if (await ValidateId(Field))
            {
            }
            return Field.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Field> Fields)
        {
            return true;
        }

        public async Task<bool> Import(List<Field> Fields)
        {
            return true;
        }
    }
}
