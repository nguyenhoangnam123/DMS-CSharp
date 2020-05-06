using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MERouteChangeRequestContent
{
    public interface IERouteChangeRequestContentValidator : IServiceScoped
    {
        Task<bool> Create(ERouteChangeRequestContent ERouteChangeRequestContent);
        Task<bool> Update(ERouteChangeRequestContent ERouteChangeRequestContent);
        Task<bool> Delete(ERouteChangeRequestContent ERouteChangeRequestContent);
        Task<bool> BulkDelete(List<ERouteChangeRequestContent> ERouteChangeRequestContents);
        Task<bool> Import(List<ERouteChangeRequestContent> ERouteChangeRequestContents);
    }

    public class ERouteChangeRequestContentValidator : IERouteChangeRequestContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ERouteChangeRequestContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter = new ERouteChangeRequestContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ERouteChangeRequestContent.Id },
                Selects = ERouteChangeRequestContentSelect.Id
            };

            int count = await UOW.ERouteChangeRequestContentRepository.Count(ERouteChangeRequestContentFilter);
            if (count == 0)
                ERouteChangeRequestContent.AddError(nameof(ERouteChangeRequestContentValidator), nameof(ERouteChangeRequestContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            return ERouteChangeRequestContent.IsValidated;
        }

        public async Task<bool> Update(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            if (await ValidateId(ERouteChangeRequestContent))
            {
            }
            return ERouteChangeRequestContent.IsValidated;
        }

        public async Task<bool> Delete(ERouteChangeRequestContent ERouteChangeRequestContent)
        {
            if (await ValidateId(ERouteChangeRequestContent))
            {
            }
            return ERouteChangeRequestContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ERouteChangeRequestContent> ERouteChangeRequestContents)
        {
            return true;
        }
        
        public async Task<bool> Import(List<ERouteChangeRequestContent> ERouteChangeRequestContents)
        {
            return true;
        }
    }
}
