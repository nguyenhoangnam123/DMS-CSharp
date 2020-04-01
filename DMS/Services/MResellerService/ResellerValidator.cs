using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MReseller
{
    public interface IResellerValidator : IServiceScoped
    {
        Task<bool> Create(Reseller Reseller);
        Task<bool> Update(Reseller Reseller);
        Task<bool> Delete(Reseller Reseller);
        Task<bool> BulkDelete(List<Reseller> Resellers);
        Task<bool> Import(List<Reseller> Resellers);
    }

    public class ResellerValidator : IResellerValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ResellerValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Reseller Reseller)
        {
            ResellerFilter ResellerFilter = new ResellerFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Reseller.Id },
                Selects = ResellerSelect.Id
            };

            int count = await UOW.ResellerRepository.Count(ResellerFilter);
            if (count == 0)
                Reseller.AddError(nameof(ResellerValidator), nameof(Reseller.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Reseller Reseller)
        {
            return Reseller.IsValidated;
        }

        public async Task<bool> Update(Reseller Reseller)
        {
            if (await ValidateId(Reseller))
            {
            }
            return Reseller.IsValidated;
        }

        public async Task<bool> Delete(Reseller Reseller)
        {
            if (await ValidateId(Reseller))
            {
            }
            return Reseller.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Reseller> Resellers)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Reseller> Resellers)
        {
            return true;
        }
    }
}
