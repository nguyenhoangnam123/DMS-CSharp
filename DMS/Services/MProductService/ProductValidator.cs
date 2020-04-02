using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProduct
{
    public interface IProductValidator : IServiceScoped
    {
        Task<bool> Create(Product Product);
        Task<bool> Update(Product Product);
        Task<bool> Delete(Product Product);
        Task<bool> BulkDelete(List<Product> Products);
        Task<bool> Import(List<Product> Products);
    }

    public class ProductValidator : IProductValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            ERPCodeExisted,
            ScanCodeExisted,
            NameEmpty,
            NameOverLength,
            CodeEmpty,
            CodeOverLength,
            ERPCodeEmpty,
            ERPCodeOverLength,
            ScanCodeEmpty,
            ScanCodeOverLength,
            ProductTypeNotExisted,
            UnitOfMeasureNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProductValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Product Product)
        {
            ProductFilter ProductFilter = new ProductFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.Id },
                Selects = ProductSelect.Id
            };

            int count = await UOW.ProductRepository.Count(ProductFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        public async Task<bool> ValidateName(Product Product)
        {
            if (string.IsNullOrEmpty(Product.Name))
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Name), ErrorCode.NameEmpty);
            }
            if (Product.Name.Length > 3000)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Name), ErrorCode.NameOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateCode(Product Product)
        {
            if (string.IsNullOrEmpty(Product.Code))
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeEmpty);
            }
            if (!string.IsNullOrEmpty(Product.Code) && Product.Code.Length > 500)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateERPCode(Product Product)
        {
            if (string.IsNullOrEmpty(Product.ERPCode))
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.ERPCode), ErrorCode.ERPCodeEmpty);
            }
            if (!string.IsNullOrEmpty(Product.ERPCode) && Product.ERPCode.Length > 500)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.ERPCode), ErrorCode.ERPCodeOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateScanCode(Product Product)
        {
            if (string.IsNullOrEmpty(Product.ScanCode))
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.ScanCode), ErrorCode.ScanCodeEmpty);
            }
            if (!string.IsNullOrEmpty(Product.ScanCode) && Product.ScanCode.Length > 500)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.ScanCode), ErrorCode.ScanCodeOverLength);
            }
            return true;
        }
        public async Task<bool> Create(Product Product)
        {
            return Product.IsValidated;
        }

        public async Task<bool> Update(Product Product)
        {
            if (await ValidateId(Product))
            {
            }
            return Product.IsValidated;
        }

        public async Task<bool> Delete(Product Product)
        {
            if (await ValidateId(Product))
            {
            }
            return Product.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Product> Products)
        {
            return true;
        }

        public async Task<bool> Import(List<Product> Products)
        {
            var listCodeInDB = (await UOW.ProductRepository.List(new ProductFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.Code
            })).Select(e => e.Code);
            var listCodeERPInDB = (await UOW.ProductRepository.List(new ProductFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.ERPCode
            })).Select(e => e.ERPCode);
            var listScanCodeERPInDB = (await UOW.ProductRepository.List(new ProductFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.ScanCode
            })).Select(e => e.ScanCode);
            foreach (var Product in Products)
            {
                await ValidateName(Product);
                await ValidateCode(Product);
                await ValidateScanCode(Product);
                await ValidateERPCode(Product);
                if (!Product.IsValidated) return false;
                if (listCodeInDB.Contains(Product.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeExisted);
                    return false;
                }
                if (listCodeERPInDB.Contains(Product.ERPCode))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ERPCode), ErrorCode.ERPCodeExisted);
                    return false;
                }
                if (listScanCodeERPInDB.Contains(Product.ScanCode))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ScanCode), ErrorCode.ScanCodeExisted);
                    return false;
                }
                if (Product.ProductTypeId == 0)
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ProductTypeId), ErrorCode.ProductTypeNotExisted);
                    return false;
                }

                if (Product.UnitOfMeasureId == 0)
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasureId), ErrorCode.UnitOfMeasureNotExisted);
                    return false;
                }
            } 
            return true;
        }
    }
}
