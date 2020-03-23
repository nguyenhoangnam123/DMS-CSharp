using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MUnitOfMeasureGroupingContent
{
    public interface IUnitOfMeasureGroupingContentValidator : IServiceScoped
    {
        Task<bool> Create(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<bool> Update(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<bool> Delete(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<bool> BulkDelete(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents);
        Task<bool> Import(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents);
    }

    public class UnitOfMeasureGroupingContentValidator : IUnitOfMeasureGroupingContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UnitOfMeasureGroupingContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = new UnitOfMeasureGroupingContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfMeasureGroupingContent.Id },
                Selects = UnitOfMeasureGroupingContentSelect.Id
            };

            int count = await UOW.UnitOfMeasureGroupingContentRepository.Count(UnitOfMeasureGroupingContentFilter);
            if (count == 0)
                UnitOfMeasureGroupingContent.AddError(nameof(UnitOfMeasureGroupingContentValidator), nameof(UnitOfMeasureGroupingContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            return UnitOfMeasureGroupingContent.IsValidated;
        }

        public async Task<bool> Update(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            if (await ValidateId(UnitOfMeasureGroupingContent))
            {
            }
            return UnitOfMeasureGroupingContent.IsValidated;
        }

        public async Task<bool> Delete(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            if (await ValidateId(UnitOfMeasureGroupingContent))
            {
            }
            return UnitOfMeasureGroupingContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents)
        {
            return true;
        }
        
        public async Task<bool> Import(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents)
        {
            return true;
        }
    }
}
