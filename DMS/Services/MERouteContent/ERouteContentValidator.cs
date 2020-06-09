using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MERouteContent
{
    public interface IERouteContentValidator : IServiceScoped
    {
        Task<bool> Create(ERouteContent ERouteContent);
        Task<bool> Update(ERouteContent ERouteContent);
        Task<bool> Delete(ERouteContent ERouteContent);
        Task<bool> BulkDelete(List<ERouteContent> ERouteContents);
        Task<bool> Import(List<ERouteContent> ERouteContents);
    }

    public class ERouteContentValidator : IERouteContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ERouteContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ERouteContent ERouteContent)
        {
            ERouteContentFilter ERouteContentFilter = new ERouteContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ERouteContent.Id },
                Selects = ERouteContentSelect.Id
            };

            int count = await UOW.ERouteContentRepository.Count(ERouteContentFilter);
            if (count == 0)
                ERouteContent.AddError(nameof(ERouteContentValidator), nameof(ERouteContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(ERouteContent ERouteContent)
        {
            return ERouteContent.IsValidated;
        }

        public async Task<bool> Update(ERouteContent ERouteContent)
        {
            if (await ValidateId(ERouteContent))
            {
            }
            return ERouteContent.IsValidated;
        }

        public async Task<bool> Delete(ERouteContent ERouteContent)
        {
            if (await ValidateId(ERouteContent))
            {
            }
            return ERouteContent.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ERouteContent> ERouteContents)
        {
            return true;
        }

        public async Task<bool> Import(List<ERouteContent> ERouteContents)
        {
            return true;
        }
    }
}
