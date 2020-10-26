using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MKpiCriteriaTotal
{
    public interface IKpiCriteriaTotalValidator : IServiceScoped
    {
        Task<bool> Create(KpiCriteriaTotal KpiCriteriaTotal);
        Task<bool> Update(KpiCriteriaTotal KpiCriteriaTotal);
        Task<bool> Delete(KpiCriteriaTotal KpiCriteriaTotal);
        Task<bool> BulkDelete(List<KpiCriteriaTotal> KpiCriteriaTotals);
        Task<bool> Import(List<KpiCriteriaTotal> KpiCriteriaTotals);
    }

    public class KpiCriteriaTotalValidator : IKpiCriteriaTotalValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public KpiCriteriaTotalValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(KpiCriteriaTotal KpiCriteriaTotal)
        {
            KpiCriteriaTotalFilter KpiCriteriaTotalFilter = new KpiCriteriaTotalFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = KpiCriteriaTotal.Id },
                Selects = KpiCriteriaTotalSelect.Id
            };

            int count = await UOW.KpiCriteriaTotalRepository.Count(KpiCriteriaTotalFilter);
            if (count == 0)
                KpiCriteriaTotal.AddError(nameof(KpiCriteriaTotalValidator), nameof(KpiCriteriaTotal.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(KpiCriteriaTotal KpiCriteriaTotal)
        {
            return KpiCriteriaTotal.IsValidated;
        }

        public async Task<bool> Update(KpiCriteriaTotal KpiCriteriaTotal)
        {
            if (await ValidateId(KpiCriteriaTotal))
            {
            }
            return KpiCriteriaTotal.IsValidated;
        }

        public async Task<bool> Delete(KpiCriteriaTotal KpiCriteriaTotal)
        {
            if (await ValidateId(KpiCriteriaTotal))
            {
            }
            return KpiCriteriaTotal.IsValidated;
        }

        public async Task<bool> BulkDelete(List<KpiCriteriaTotal> KpiCriteriaTotals)
        {
            return true;
        }

        public async Task<bool> Import(List<KpiCriteriaTotal> KpiCriteriaTotals)
        {
            return true;
        }
    }
}
