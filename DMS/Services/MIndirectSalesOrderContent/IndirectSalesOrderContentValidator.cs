using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MIndirectSalesOrderContent
{
    public interface IIndirectSalesOrderContentValidator : IServiceScoped
    {
        Task<bool> Create(IndirectSalesOrderContent IndirectSalesOrderContent);
        Task<bool> Update(IndirectSalesOrderContent IndirectSalesOrderContent);
        Task<bool> Delete(IndirectSalesOrderContent IndirectSalesOrderContent);
        Task<bool> BulkDelete(List<IndirectSalesOrderContent> IndirectSalesOrderContents);
        Task<bool> Import(List<IndirectSalesOrderContent> IndirectSalesOrderContents);
    }

    public class IndirectSalesOrderContentValidator : IIndirectSalesOrderContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectSalesOrderContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrderContent.Id },
                Selects = IndirectSalesOrderContentSelect.Id
            };

            int count = await UOW.IndirectSalesOrderContentRepository.Count(IndirectSalesOrderContentFilter);
            if (count == 0)
                IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderContentValidator), nameof(IndirectSalesOrderContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            return IndirectSalesOrderContent.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            if (await ValidateId(IndirectSalesOrderContent))
            {
            }
            return IndirectSalesOrderContent.IsValidated;
        }

        public async Task<bool> Delete(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            if (await ValidateId(IndirectSalesOrderContent))
            {
            }
            return IndirectSalesOrderContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<IndirectSalesOrderContent> IndirectSalesOrderContents)
        {
            return true;
        }
        
        public async Task<bool> Import(List<IndirectSalesOrderContent> IndirectSalesOrderContents)
        {
            return true;
        }
    }
}
