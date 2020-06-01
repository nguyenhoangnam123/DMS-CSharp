using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MGeneralKpi
{
    public interface IGeneralKpiValidator : IServiceScoped
    {
        Task<bool> Create(GeneralKpi GeneralKpi);
        Task<bool> Update(GeneralKpi GeneralKpi);
        Task<bool> Delete(GeneralKpi GeneralKpi);
        Task<bool> BulkDelete(List<GeneralKpi> GeneralKpis);
        Task<bool> Import(List<GeneralKpi> GeneralKpis);
    }

    public class GeneralKpiValidator : IGeneralKpiValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public GeneralKpiValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(GeneralKpi GeneralKpi)
        {
            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = GeneralKpi.Id },
                Selects = GeneralKpiSelect.Id
            };

            int count = await UOW.GeneralKpiRepository.Count(GeneralKpiFilter);
            if (count == 0)
                GeneralKpi.AddError(nameof(GeneralKpiValidator), nameof(GeneralKpi.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(GeneralKpi GeneralKpi)
        {
            return GeneralKpi.IsValidated;
        }

        public async Task<bool> Update(GeneralKpi GeneralKpi)
        {
            if (await ValidateId(GeneralKpi))
            {
            }
            return GeneralKpi.IsValidated;
        }

        public async Task<bool> Delete(GeneralKpi GeneralKpi)
        {
            if (await ValidateId(GeneralKpi))
            {
            }
            return GeneralKpi.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<GeneralKpi> GeneralKpis)
        {
            return true;
        }
        
        public async Task<bool> Import(List<GeneralKpi> GeneralKpis)
        {
            return true;
        }
    }
}
