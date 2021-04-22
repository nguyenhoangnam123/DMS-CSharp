using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MKpiProductGrouping
{
    public interface IKpiProductGroupingValidator : IServiceScoped
    {
        Task<bool> Create(KpiProductGrouping KpiProductGrouping);
        Task<bool> Update(KpiProductGrouping KpiProductGrouping);
        Task<bool> Delete(KpiProductGrouping KpiProductGrouping);
        Task<bool> BulkDelete(List<KpiProductGrouping> KpiProductGroupings);
        Task<bool> Import(List<KpiProductGrouping> KpiProductGroupings);
    }

    public class KpiProductGroupingValidator : IKpiProductGroupingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public KpiProductGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(KpiProductGrouping KpiProductGrouping)
        {
            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = KpiProductGrouping.Id },
                Selects = KpiProductGroupingSelect.Id
            };

            int count = await UOW.KpiProductGroupingRepository.Count(KpiProductGroupingFilter);
            if (count == 0)
                KpiProductGrouping.AddError(nameof(KpiProductGroupingValidator), nameof(KpiProductGrouping.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(KpiProductGrouping KpiProductGrouping)
        {
            return KpiProductGrouping.IsValidated;
        }

        public async Task<bool> Update(KpiProductGrouping KpiProductGrouping)
        {
            if (await ValidateId(KpiProductGrouping))
            {
            }
            return KpiProductGrouping.IsValidated;
        }

        public async Task<bool> Delete(KpiProductGrouping KpiProductGrouping)
        {
            if (await ValidateId(KpiProductGrouping))
            {
            }
            return KpiProductGrouping.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<KpiProductGrouping> KpiProductGroupings)
        {
            foreach (KpiProductGrouping KpiProductGrouping in KpiProductGroupings)
            {
                await Delete(KpiProductGrouping);
            }
            return KpiProductGroupings.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<KpiProductGrouping> KpiProductGroupings)
        {
            return true;
        }
    }
}
