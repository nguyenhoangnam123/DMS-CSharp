﻿using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MImage;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProduct
{
    public interface IProductService : IServiceScoped
    {
        Task<int> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
        Task<Product> Create(Product Product);
        Task<List<Product>> BulkInsertNewProduct(List<Product> Products);
        Task<Product> Update(Product Product);
        Task<Product> Delete(Product Product);
        Task<List<Product>> BulkDeleteNewProduct(List<Product> Products);
        Task<List<Product>> BulkDelete(List<Product> Products);
        Task<List<Product>> Import(List<Product> Products);
        Task<DataFile> Export(ProductFilter ProductFilter);
        ProductFilter ToFilter(ProductFilter ProductFilter);

        Task<Image> SaveImage(Image Image);
    }

    public class ProductService : BaseService, IProductService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductValidator ProductValidator;
        private IImageService ImageService;

        public ProductService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductValidator ProductValidator,
            IImageService ImageService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProductValidator = ProductValidator;
            this.ImageService = ImageService;
        }
        public async Task<int> Count(ProductFilter ProductFilter)
        {
            try
            {
                int result = await UOW.ProductRepository.Count(ProductFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> List(ProductFilter ProductFilter)
        {
            try
            {
                List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
                List<long> ProductIds = Products.Select(p => p.Id).ToList();
                ItemFilter ItemFilter = new ItemFilter
                {
                    ProductId = new IdFilter { In = ProductIds },
                    StatusId = null,
                    Selects = ItemSelect.Id | ItemSelect.ProductId,
                    Skip = 0,
                    Take = int.MaxValue,
                };
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                foreach (Product Product in Products)
                {
                    Product.CanDelete = true;
                    Product.VariationCounter = Items.Where(i => i.ProductId == Product.Id).Count();
                    List<long> ItemIds = Items.Where(i => i.ProductId == Product.Id).Select(i => i.Id).ToList();
                    IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter()
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };

                    int count = await UOW.IndirectSalesOrderContentRepository.Count(IndirectSalesOrderContentFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }

                    IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };
                    count = await UOW.IndirectSalesOrderPromotionRepository.Count(IndirectSalesOrderPromotionFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }

                    DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter()
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };

                    count = await UOW.DirectSalesOrderContentRepository.Count(DirectSalesOrderContentFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }

                    DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };
                    count = await UOW.DirectSalesOrderPromotionRepository.Count(DirectSalesOrderPromotionFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }
                }
                return Products;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Product> Get(long Id)
        {
            Product Product = await UOW.ProductRepository.Get(Id);
            if (Product == null)
                return null;
            Product.CanDelete = true;
            if (Product.Items != null && Product.Items.Any())
            {
                Product.VariationCounter = Product.Items.Count;
                Product.Items.ForEach(x => x.CanDelete = true);

                foreach (var Item in Product.Items)
                {
                    Item.CanDelete = await CanDelete(Item);
                    if (Item.CanDelete == false)
                        Product.CanDelete = false;
                }
            }
            return Product;
        }

        public async Task<Product> Create(Product Product)
        {
            if (!await ProductValidator.Create(Product))
                return Product;

            try
            {
                if (Product.UsedVariationId == Enums.UsedVariationEnum.NOTUSED.Id)
                {
                    Product.Items = new List<Item>();
                    Product.Items.Add(new Item
                    {
                        Code = Product.Code,
                        Name = Product.Name,
                        ScanCode = Product.ScanCode,
                        RetailPrice = Product.RetailPrice,
                        SalePrice = Product.SalePrice,
                        ProductId = Product.Id,
                        StatusId = StatusEnum.ACTIVE.Id
                    });
                }
                await UOW.Begin();
                await UOW.ProductRepository.Create(Product);
                await UOW.Commit();

                await Logging.CreateAuditLog(Product, new { }, nameof(ProductService));
                return await UOW.ProductRepository.Get(Product.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> BulkInsertNewProduct(List<Product> Products)
        {
            if (!await ProductValidator.BulkMergeNewProduct(Products))
                return Products;

            try
            {
                Products.ForEach(x => x.IsNew = true);
                await UOW.Begin();
                await UOW.ProductRepository.BulkInsertNewProduct(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> BulkDeleteNewProduct(List<Product> Products)
        {
            if (!await ProductValidator.BulkMergeNewProduct(Products))
                return Products;

            try
            {
                Products.ForEach(x => x.IsNew = false);
                await UOW.Begin();
                await UOW.ProductRepository.BulkDeleteNewProduct(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Product> Update(Product Product)
        {
            if (!await ProductValidator.Update(Product))
                return Product;
            try
            {
                var oldData = await UOW.ProductRepository.Get(Product.Id);

                await UOW.Begin();
                await UOW.ProductRepository.Update(Product);
                await UOW.Commit();

                var newData = await UOW.ProductRepository.Get(Product.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProductService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Product> Delete(Product Product)
        {
            if (!await ProductValidator.Delete(Product))
                return Product;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.Delete(Product);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Product, nameof(ProductService));
                return Product;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> BulkDelete(List<Product> Products)
        {
            if (!await ProductValidator.BulkDelete(Products))
                return Products;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.BulkDelete(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> Import(List<Product> Products)
        {
            var ProductProductGroupingMappings = new List<ProductProductGroupingMapping>();
            if (!await ProductValidator.Import(Products))
                return Products;
            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.BulkMerge(Products);
                //Lấy danh sách product để check the code lấy ra productId vừa mới thêm
                var listProductInDB = (await UOW.ProductRepository.List(new ProductFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ProductSelect.ALL
                }));
                // Add reference ProductProductGroupingMapping
                foreach (var Product in Products)
                {
                    var p = listProductInDB.Where(p => p.Code == Product.Code).FirstOrDefault();
                    Product.Id = p != null ? p.Id : 0;
                    if (Product.ProductProductGroupingMappings.Any())
                    {
                        foreach (var ProductProductGroupingMapping in Product.ProductProductGroupingMappings)
                        {
                            ProductProductGroupingMapping.ProductId = Product.Id;
                            ProductProductGroupingMappings.Add(ProductProductGroupingMapping);
                        }
                    }
                }
                await UOW.ProductProductGroupingMappingRepository.BulkMerge(ProductProductGroupingMappings);
                await UOW.Commit();
                await Logging.CreateAuditLog(Products, new { }, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(ProductFilter ProductFilter)
        {
            List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Product);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int SupplierCodeColumn = 2 + StartColumn;
                int NameColumn = 3 + StartColumn;
                int DescriptionColumn = 4 + StartColumn;
                int ScanCodeColumn = 5 + StartColumn;
                int ProductTypeIdColumn = 6 + StartColumn;
                int SupplierIdColumn = 7 + StartColumn;
                int BrandIdColumn = 8 + StartColumn;
                int UnitOfMeasureIdColumn = 9 + StartColumn;
                int UnitOfMeasureGroupingIdColumn = 10 + StartColumn;
                int SalePriceColumn = 11 + StartColumn;
                int RetailPriceColumn = 12 + StartColumn;
                int TaxTypeIdColumn = 13 + StartColumn;
                int StatusIdColumn = 14 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Product.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Product.Code);
                worksheet.Cells[1, SupplierCodeColumn].Value = nameof(Product.SupplierCode);
                worksheet.Cells[1, NameColumn].Value = nameof(Product.Name);
                worksheet.Cells[1, DescriptionColumn].Value = nameof(Product.Description);
                worksheet.Cells[1, ScanCodeColumn].Value = nameof(Product.ScanCode);
                worksheet.Cells[1, ProductTypeIdColumn].Value = nameof(Product.ProductTypeId);
                worksheet.Cells[1, SupplierIdColumn].Value = nameof(Product.SupplierId);
                worksheet.Cells[1, BrandIdColumn].Value = nameof(Product.BrandId);
                worksheet.Cells[1, UnitOfMeasureIdColumn].Value = nameof(Product.UnitOfMeasureId);
                worksheet.Cells[1, UnitOfMeasureGroupingIdColumn].Value = nameof(Product.UnitOfMeasureGroupingId);
                worksheet.Cells[1, SalePriceColumn].Value = nameof(Product.SalePrice);
                worksheet.Cells[1, RetailPriceColumn].Value = nameof(Product.RetailPrice);
                worksheet.Cells[1, TaxTypeIdColumn].Value = nameof(Product.TaxTypeId);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Product.StatusId);

                for (int i = 0; i < Products.Count; i++)
                {
                    Product Product = Products[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Product.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Product.Code;
                    worksheet.Cells[i + StartRow, SupplierCodeColumn].Value = Product.SupplierCode;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Product.Name;
                    worksheet.Cells[i + StartRow, DescriptionColumn].Value = Product.Description;
                    worksheet.Cells[i + StartRow, ScanCodeColumn].Value = Product.ScanCode;
                    worksheet.Cells[i + StartRow, ProductTypeIdColumn].Value = Product.ProductTypeId;
                    worksheet.Cells[i + StartRow, SupplierIdColumn].Value = Product.SupplierId;
                    worksheet.Cells[i + StartRow, BrandIdColumn].Value = Product.BrandId;
                    worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value = Product.UnitOfMeasureId;
                    worksheet.Cells[i + StartRow, UnitOfMeasureGroupingIdColumn].Value = Product.UnitOfMeasureGroupingId;
                    worksheet.Cells[i + StartRow, SalePriceColumn].Value = Product.SalePrice;
                    worksheet.Cells[i + StartRow, RetailPriceColumn].Value = Product.RetailPrice;
                    worksheet.Cells[i + StartRow, TaxTypeIdColumn].Value = Product.TaxTypeId;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Product.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Product),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public ProductFilter ToFilter(ProductFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProductFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProductFilter subFilter = new ProductFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SupplierCode))
                        subFilter.SupplierCode = Map(subFilter.SupplierCode, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = Map(subFilter.Description, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ScanCode))
                        subFilter.ScanCode = Map(subFilter.ScanCode, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ERPCode))
                        subFilter.ERPCode = Map(subFilter.ERPCode, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductTypeId))
                        subFilter.ProductTypeId = Map(subFilter.ProductTypeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SupplierId))
                        subFilter.SupplierId = Map(subFilter.SupplierId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.BrandId))
                        subFilter.BrandId = Map(subFilter.BrandId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureId))
                        subFilter.UnitOfMeasureId = Map(subFilter.UnitOfMeasureId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureGroupingId))
                        subFilter.UnitOfMeasureGroupingId = Map(subFilter.UnitOfMeasureGroupingId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalePrice))
                        subFilter.SalePrice = Map(subFilter.SalePrice, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RetailPrice))
                        subFilter.RetailPrice = Map(subFilter.RetailPrice, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxTypeId))
                        subFilter.TaxTypeId = Map(subFilter.TaxTypeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OtherName))
                        subFilter.OtherName = Map(subFilter.OtherName, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TechnicalName))
                        subFilter.TechnicalName = Map(subFilter.TechnicalName, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        subFilter.Note = Map(subFilter.Note, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UsedVariationId))
                        subFilter.UsedVariationId = Map(subFilter.UsedVariationId, FilterPermissionDefinition);
                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
        }

        private async Task<bool> CanDelete(Item Item)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter()
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };

            int count = await UOW.IndirectSalesOrderContentRepository.Count(IndirectSalesOrderContentFilter);
            if (count != 0)
                return false;

            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };
            count = await UOW.IndirectSalesOrderPromotionRepository.Count(IndirectSalesOrderPromotionFilter);
            if (count != 0)
                return false;

            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter()
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };

            count = await UOW.DirectSalesOrderContentRepository.Count(DirectSalesOrderContentFilter);
            if (count != 0)
                return false;

            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };
            count = await UOW.DirectSalesOrderPromotionRepository.Count(DirectSalesOrderPromotionFilter);
            if (count != 0)
                return false;
            return true;
        }
    }
}
