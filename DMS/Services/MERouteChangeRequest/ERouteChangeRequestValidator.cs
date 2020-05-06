using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MERouteChangeRequest
{
    public interface IERouteChangeRequestValidator : IServiceScoped
    {
        Task<bool> Create(ERouteChangeRequest ERouteChangeRequest);
        Task<bool> Update(ERouteChangeRequest ERouteChangeRequest);
        Task<bool> Delete(ERouteChangeRequest ERouteChangeRequest);
        Task<bool> BulkDelete(List<ERouteChangeRequest> ERouteChangeRequests);
        Task<bool> Import(List<ERouteChangeRequest> ERouteChangeRequests);
    }

    public class ERouteChangeRequestValidator : IERouteChangeRequestValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ERouteChangeRequestValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ERouteChangeRequest ERouteChangeRequest)
        {
            ERouteChangeRequestFilter ERouteChangeRequestFilter = new ERouteChangeRequestFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ERouteChangeRequest.Id },
                Selects = ERouteChangeRequestSelect.Id
            };

            int count = await UOW.ERouteChangeRequestRepository.Count(ERouteChangeRequestFilter);
            if (count == 0)
                ERouteChangeRequest.AddError(nameof(ERouteChangeRequestValidator), nameof(ERouteChangeRequest.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ERouteChangeRequest ERouteChangeRequest)
        {
            return ERouteChangeRequest.IsValidated;
        }

        public async Task<bool> Update(ERouteChangeRequest ERouteChangeRequest)
        {
            if (await ValidateId(ERouteChangeRequest))
            {
            }
            return ERouteChangeRequest.IsValidated;
        }

        public async Task<bool> Delete(ERouteChangeRequest ERouteChangeRequest)
        {
            if (await ValidateId(ERouteChangeRequest))
            {
            }
            return ERouteChangeRequest.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ERouteChangeRequest> ERouteChangeRequests)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ERouteChangeRequest> ERouteChangeRequests)
        {
            return true;
        }
    }
}
