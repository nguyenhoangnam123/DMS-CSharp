using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MSupplier
{
    public interface ISupplierValidator : IServiceScoped
    {
        Task<bool> Create(Supplier Supplier);
        Task<bool> Update(Supplier Supplier);
        Task<bool> Delete(Supplier Supplier);
        Task<bool> BulkDelete(List<Supplier> Suppliers);
        Task<bool> Import(List<Supplier> Suppliers);
    }

    public class SupplierValidator : ISupplierValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public SupplierValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Supplier Supplier)
        {
            SupplierFilter SupplierFilter = new SupplierFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Supplier.Id },
                Selects = SupplierSelect.Id
            };

            int count = await UOW.SupplierRepository.Count(SupplierFilter);
            if (count == 0)
                Supplier.AddError(nameof(SupplierValidator), nameof(Supplier.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(Supplier Supplier)
        {
            return Supplier.IsValidated;
        }

        public async Task<bool> Update(Supplier Supplier)
        {
            if (await ValidateId(Supplier))
            {
            }
            return Supplier.IsValidated;
        }

        public async Task<bool> Delete(Supplier Supplier)
        {
            if (await ValidateId(Supplier))
            {
            }
            return Supplier.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Supplier> Suppliers)
        {
            return true;
        }

        public async Task<bool> Import(List<Supplier> Suppliers)
        {
            return true;
        }
    }
}
