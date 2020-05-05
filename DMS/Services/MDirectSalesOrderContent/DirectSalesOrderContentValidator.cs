using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MDirectSalesOrderContent
{
    public interface IDirectSalesOrderContentValidator : IServiceScoped
    {
        Task<bool> Create(DirectSalesOrderContent DirectSalesOrderContent);
        Task<bool> Update(DirectSalesOrderContent DirectSalesOrderContent);
        Task<bool> Delete(DirectSalesOrderContent DirectSalesOrderContent);
        Task<bool> BulkDelete(List<DirectSalesOrderContent> DirectSalesOrderContents);
        Task<bool> Import(List<DirectSalesOrderContent> DirectSalesOrderContents);
    }

    public class DirectSalesOrderContentValidator : IDirectSalesOrderContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DirectSalesOrderContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectSalesOrderContent DirectSalesOrderContent)
        {
            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrderContent.Id },
                Selects = DirectSalesOrderContentSelect.Id
            };

            int count = await UOW.DirectSalesOrderContentRepository.Count(DirectSalesOrderContentFilter);
            if (count == 0)
                DirectSalesOrderContent.AddError(nameof(DirectSalesOrderContentValidator), nameof(DirectSalesOrderContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(DirectSalesOrderContent DirectSalesOrderContent)
        {
            return DirectSalesOrderContent.IsValidated;
        }

        public async Task<bool> Update(DirectSalesOrderContent DirectSalesOrderContent)
        {
            if (await ValidateId(DirectSalesOrderContent))
            {
            }
            return DirectSalesOrderContent.IsValidated;
        }

        public async Task<bool> Delete(DirectSalesOrderContent DirectSalesOrderContent)
        {
            if (await ValidateId(DirectSalesOrderContent))
            {
            }
            return DirectSalesOrderContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<DirectSalesOrderContent> DirectSalesOrderContents)
        {
            return true;
        }
        
        public async Task<bool> Import(List<DirectSalesOrderContent> DirectSalesOrderContents)
        {
            return true;
        }
    }
}
