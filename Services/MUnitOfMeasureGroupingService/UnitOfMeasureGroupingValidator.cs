using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MUnitOfMeasureGrouping
{
    public interface IUnitOfMeasureGroupingValidator : IServiceScoped
    {
        Task<bool> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
        Task<bool> Import(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
    }

    public class UnitOfMeasureGroupingValidator : IUnitOfMeasureGroupingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UnitOfMeasureGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfMeasureGrouping.Id },
                Selects = UnitOfMeasureGroupingSelect.Id
            };

            int count = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
            if (count == 0)
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            return UnitOfMeasureGrouping.IsValidated;
        }

        public async Task<bool> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (await ValidateId(UnitOfMeasureGrouping))
            {
            }
            return UnitOfMeasureGrouping.IsValidated;
        }

        public async Task<bool> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (await ValidateId(UnitOfMeasureGrouping))
            {
            }
            return UnitOfMeasureGrouping.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            return true;
        }
        
        public async Task<bool> Import(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            return true;
        }
    }
}
