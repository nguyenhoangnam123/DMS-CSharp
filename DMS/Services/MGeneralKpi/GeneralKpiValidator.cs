using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;
using DMS.Services.MAppUser;
using DMS.Enums;

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
            OrganizationIdNotExisted,
            EmployeeIdsEmpty,
            StatusNotExisted
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

        private async Task<bool> ValidateOrganization(GeneralKpi GeneralKpi)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Id = new IdFilter { Equal = GeneralKpi.OrganizationId }
            };

            var count = await UOW.OrganizationRepository.Count(OrganizationFilter);
            if(count == 0)
                GeneralKpi.AddError(nameof(GeneralKpiValidator), nameof(GeneralKpi.Organization), ErrorCode.OrganizationIdNotExisted);
            return GeneralKpi.IsValidated;
        }

        private async Task<bool> ValidateEmployees(GeneralKpi GeneralKpi)
        {
            if(GeneralKpi.EmployeeIds == null || !GeneralKpi.EmployeeIds.Any())
                GeneralKpi.AddError(nameof(GeneralKpiValidator), nameof(GeneralKpi.EmployeeIds), ErrorCode.EmployeeIdsEmpty);
            else
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = GeneralKpi.EmployeeIds },
                    OrganizationId = new IdFilter(),
                    Selects = AppUserSelect.Id
                };

                var EmployeeIdsInDB = (await UOW.AppUserRepository.List(AppUserFilter)).Select(x => x.Id).ToList();
                var listIdsNotExisted = GeneralKpi.EmployeeIds.Except(EmployeeIdsInDB).ToList();

                if(listIdsNotExisted != null && listIdsNotExisted.Any())
                {
                    foreach (var Id in listIdsNotExisted)
                    {
                        GeneralKpi.AddError(nameof(GeneralKpiValidator), nameof(GeneralKpi.EmployeeIds), ErrorCode.IdNotExisted);
                    }
                }
                
            }
            return GeneralKpi.IsValidated;
        }

        public async Task<bool> ValidateStatus(GeneralKpi GeneralKpi)
        {
            if (StatusEnum.ACTIVE.Id != GeneralKpi.StatusId && StatusEnum.INACTIVE.Id != GeneralKpi.StatusId)
                GeneralKpi.AddError(nameof(GeneralKpiValidator), nameof(GeneralKpi.Status), ErrorCode.StatusNotExisted);
            return GeneralKpi.IsValidated;
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
