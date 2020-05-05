using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MERoute
{
    public interface IERouteValidator : IServiceScoped
    {
        Task<bool> Create(ERoute ERoute);
        Task<bool> Update(ERoute ERoute);
        Task<bool> Delete(ERoute ERoute);
        Task<bool> BulkDelete(List<ERoute> ERoutes);
        Task<bool> Import(List<ERoute> ERoutes);
    }

    public class ERouteValidator : IERouteValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ERouteValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ERoute ERoute)
        {
            ERouteFilter ERouteFilter = new ERouteFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ERoute.Id },
                Selects = ERouteSelect.Id
            };

            int count = await UOW.ERouteRepository.Count(ERouteFilter);
            if (count == 0)
                ERoute.AddError(nameof(ERouteValidator), nameof(ERoute.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ERoute ERoute)
        {
            return ERoute.IsValidated;
        }

        public async Task<bool> Update(ERoute ERoute)
        {
            if (await ValidateId(ERoute))
            {
            }
            return ERoute.IsValidated;
        }

        public async Task<bool> Delete(ERoute ERoute)
        {
            if (await ValidateId(ERoute))
            {
            }
            return ERoute.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ERoute> ERoutes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ERoute> ERoutes)
        {
            return true;
        }
    }
}
