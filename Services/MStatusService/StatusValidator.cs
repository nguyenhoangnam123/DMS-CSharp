using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MStatus
{
    public interface IStatusValidator : IServiceScoped
    {
        Task<bool> Create(Status Status);
        Task<bool> Update(Status Status);
        Task<bool> Delete(Status Status);
        Task<bool> BulkDelete(List<Status> Statuses);
        Task<bool> Import(List<Status> Statuses);
    }

    public class StatusValidator : IStatusValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StatusValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Status Status)
        {
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Status.Id },
                Selects = StatusSelect.Id
            };

            int count = await UOW.StatusRepository.Count(StatusFilter);
            if (count == 0)
                Status.AddError(nameof(StatusValidator), nameof(Status.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Status Status)
        {
            return Status.IsValidated;
        }

        public async Task<bool> Update(Status Status)
        {
            if (await ValidateId(Status))
            {
            }
            return Status.IsValidated;
        }

        public async Task<bool> Delete(Status Status)
        {
            if (await ValidateId(Status))
            {
            }
            return Status.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Status> Statuses)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Status> Statuses)
        {
            return true;
        }
    }
}
