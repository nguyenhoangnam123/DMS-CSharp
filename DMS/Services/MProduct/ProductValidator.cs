﻿using Common;
using DMS.Entities;
using DMS.Enums;
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
            CodeHasSpecialCharacter,
            CodeEmpty,
            NameEmpty,
            NameExisted,
            NameOverLength,
            ProductTypeNotExisted,
            SupplierNotExisted,
            BrandNotExisted,
            UnitOfMeasureNotExisted,
            UnitOfMeasureEmpty,
            UnitOfMeasureGroupingNotExisted,
            StatusNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ProductValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        private async Task<bool> ValidateId(Product Product)
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
        private async Task<bool> ValidateName(Product Product)
        {
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Name), ErrorCode.NameEmpty);
            }
            else if (Product.Name.Length > 3000)
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Name), ErrorCode.NameOverLength);
            }
            return Product.IsValidated;
        }
        private async Task<bool> ValidateCode(Product Product)
        {
            if (string.IsNullOrWhiteSpace(Product.Code))
            {
                Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Product.Code;
                if (Product.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Product.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                ProductFilter ProductFilter = new ProductFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Product.Id },
                    Code = new StringFilter { Equal = Product.Code },
                    Selects = ProductSelect.Code
                };

                int count = await UOW.ProductRepository.Count(ProductFilter);
                if (count != 0)
                    Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeExisted);
            }

            return Product.IsValidated;
        }

        private async Task<bool> ValidateProductType(Product Product)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.ProductTypeId },
                Selects = ProductTypeSelect.Id
            };

            int count = await UOW.ProductTypeRepository.Count(ProductTypeFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.ProductType), ErrorCode.ProductTypeNotExisted);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateSupplier(Product Product)
        {
            SupplierFilter SupplierFilter = new SupplierFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.SupplierId },
                Selects = SupplierSelect.Id
            };

            int count = await UOW.SupplierRepository.Count(SupplierFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.Supplier), ErrorCode.SupplierNotExisted);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateBrand(Product Product)
        {
            BrandFilter BrandFilter = new BrandFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.BrandId },
                Selects = BrandSelect.Id
            };

            int count = await UOW.BrandRepository.Count(BrandFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.Brand), ErrorCode.BrandNotExisted);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateUnitOfMeasure(Product Product)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.UnitOfMeasureId },
                Selects = UnitOfMeasureSelect.Id
            };

            int count = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateUnitOfMeasureGrouping(Product Product)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Product.UnitOfMeasureGroupingId },
                Selects = UnitOfMeasureGroupingSelect.Id
            };

            int count = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
            if (count == 0)
                Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasureGrouping), ErrorCode.UnitOfMeasureGroupingNotExisted);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateStatusId(Product Product)
        {
            if (StatusEnum.ACTIVE.Id != Product.StatusId && StatusEnum.INACTIVE.Id != Product.StatusId)
                Product.AddError(nameof(ProductValidator), nameof(Product.Status), ErrorCode.StatusNotExisted);
            return Product.IsValidated;
        }

        private async Task<bool> ValidateItem(Product Product)
        {
            if(Product.Items != null && Product.Items.Any())
            {
                foreach (var item in Product.Items)
                {
                    ItemFilter ItemFilter = new ItemFilter
                    {
                        Code = new StringFilter { Equal = item.Code }
                    };

                    int count = await UOW.ItemRepository.Count(ItemFilter);
                    if(count > 0)
                    {
                        Product.AddError(nameof(ProductValidator), nameof(item) + item.Code, ErrorCode.CodeExisted);
                    }

                    ItemFilter = new ItemFilter
                    {
                        Name = new StringFilter { Equal = item.Name }
                    };

                    count = await UOW.ItemRepository.Count(ItemFilter);
                    if (count > 0)
                    {
                        Product.AddError(nameof(ProductValidator), nameof(item) + item.Name, ErrorCode.NameExisted);
                    }
                }
            }

            return Product.IsValidated;
        }

        public async Task<bool> Create(Product Product)
        {
            await ValidateCode(Product);
            await ValidateName(Product);
            await ValidateProductType(Product);
            await ValidateSupplier(Product);
            await ValidateBrand(Product);
            await ValidateUnitOfMeasure(Product);
            await ValidateUnitOfMeasureGrouping(Product);
            await ValidateStatusId(Product);
            await ValidateItem(Product);
            return Product.IsValidated;
        }

        public async Task<bool> Update(Product Product)
        {
            if (await ValidateId(Product))
            {
                await ValidateCode(Product);
                await ValidateName(Product);
                await ValidateProductType(Product);
                await ValidateSupplier(Product);
                await ValidateBrand(Product);
                await ValidateUnitOfMeasure(Product);
                await ValidateUnitOfMeasureGrouping(Product);
                await ValidateStatusId(Product);
                await ValidateItem(Product);
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
            foreach (Product Product in Products)
            {
                await Delete(Product);
            }
            return Products.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<Product> Products)
        {
            var listCodeInDB = (await UOW.ProductRepository.List(new ProductFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.Code
            })).Select(e => e.Code);
            var listProductTypeCodeInDB = (await UOW.ProductTypeRepository.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.Code
            })).Select(e => e.Code);
            var listSupplierCodeInDB = (await UOW.SupplierRepository.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.Code
            })).Select(e => e.Code);
            var listBrandCodeInDB = (await UOW.BrandRepository.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.Code
            })).Select(e => e.Code);
            var listUOMCodeInDB = (await UOW.UnitOfMeasureRepository.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.Code
            })).Select(e => e.Code);
            var listUOMGroupingCodeInDB = (await UOW.UnitOfMeasureGroupingRepository.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.Code
            })).Select(e => e.Code);

            foreach (var Product in Products)
            {
                if (listCodeInDB.Contains(Product.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Code), ErrorCode.CodeExisted);
                }
                if (!listProductTypeCodeInDB.Contains(Product.ProductType.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.ProductType), ErrorCode.ProductTypeNotExisted);
                }
                if (!listSupplierCodeInDB.Contains(Product.Supplier.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Supplier), ErrorCode.SupplierNotExisted);
                }
                if (!listBrandCodeInDB.Contains(Product.Brand.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.Brand), ErrorCode.BrandNotExisted);
                }
                if (!listUOMCodeInDB.Contains(Product.UnitOfMeasure.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                }
                if (!listUOMGroupingCodeInDB.Contains(Product.UnitOfMeasureGrouping.Code))
                {
                    Product.AddError(nameof(ProductValidator), nameof(Product.UnitOfMeasureGrouping), ErrorCode.UnitOfMeasureGroupingNotExisted);
                }

                await ValidateName(Product);
            }
            return Products.Any(s => !s.IsValidated) ? false : true;
        }
    }
}
