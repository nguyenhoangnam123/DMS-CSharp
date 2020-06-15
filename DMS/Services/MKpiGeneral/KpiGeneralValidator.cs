using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MKpiGeneral
{
    public interface IKpiGeneralValidator : IServiceScoped
    {
        Task<bool> Create(KpiGeneral KpiGeneral);
        Task<bool> Update(KpiGeneral KpiGeneral);
        Task<bool> Delete(KpiGeneral KpiGeneral);
        Task<bool> BulkDelete(List<KpiGeneral> KpiGenerals);
        Task<bool> Import(List<KpiGeneral> KpiGenerals);
    }

    public class KpiGeneralValidator : IKpiGeneralValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public KpiGeneralValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(KpiGeneral KpiGeneral)
        {
            KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = KpiGeneral.Id },
                Selects = KpiGeneralSelect.Id
            };

            int count = await UOW.KpiGeneralRepository.Count(KpiGeneralFilter);
            if (count == 0)
                KpiGeneral.AddError(nameof(KpiGeneralValidator), nameof(KpiGeneral.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(KpiGeneral KpiGeneral)
        {
            return KpiGeneral.IsValidated;
        }

        public async Task<bool> Update(KpiGeneral KpiGeneral)
        {
            if (await ValidateId(KpiGeneral))
            {
            }
            return KpiGeneral.IsValidated;
        }

        public async Task<bool> Delete(KpiGeneral KpiGeneral)
        {
            if (await ValidateId(KpiGeneral))
            {
            }
            return KpiGeneral.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<KpiGeneral> KpiGenerals)
        {
            return true;
        }
        
        public async Task<bool> Import(List<KpiGeneral> KpiGenerals)
        {
            return true;
        }
    }
}
