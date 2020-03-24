using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MUnitOfMeasure
{
    public interface IUnitOfMeasureValidator : IServiceScoped
    {
        Task<bool> Create(UnitOfMeasure UnitOfMeasure);
        Task<bool> Update(UnitOfMeasure UnitOfMeasure);
        Task<bool> Delete(UnitOfMeasure UnitOfMeasure);
        Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures);
        Task<bool> Import(List<UnitOfMeasure> UnitOfMeasures);
    }

    public class UnitOfMeasureValidator : IUnitOfMeasureValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UnitOfMeasureValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UnitOfMeasure UnitOfMeasure)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfMeasure.Id },
                Selects = UnitOfMeasureSelect.Id
            };

            int count = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
            if (count == 0)
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(UnitOfMeasure UnitOfMeasure)
        {
            return UnitOfMeasure.IsValidated;
        }

        public async Task<bool> Update(UnitOfMeasure UnitOfMeasure)
        {
            if (await ValidateId(UnitOfMeasure))
            {
            }
            return UnitOfMeasure.IsValidated;
        }

        public async Task<bool> Delete(UnitOfMeasure UnitOfMeasure)
        {
            if (await ValidateId(UnitOfMeasure))
            {
            }
            return UnitOfMeasure.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures)
        {
            return true;
        }
        
        public async Task<bool> Import(List<UnitOfMeasure> UnitOfMeasures)
        {
            return true;
        }
    }
}
