using Common;
using DMS.Entities;
using DMS.Repositories;
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
        Task<Product> Update(Product Product);
        Task<Product> Delete(Product Product);
        Task<List<Product>> BulkDelete(List<Product> Products);
        Task<List<Product>> Import(DataFile DataFile);
        Task<DataFile> Export(ProductFilter ProductFilter);
        ProductFilter ToFilter(ProductFilter ProductFilter);
    }

    public class ProductService : BaseService, IProductService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductValidator ProductValidator;

        public ProductService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductValidator ProductValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProductValidator = ProductValidator;
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
            return Product;
        }

        public async Task<Product> Create(Product Product)
        {
            if (!await ProductValidator.Create(Product))
                return Product;

            try
            {
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

        public async Task<List<Product>> Import(DataFile DataFile)
        {
            List<Product> Products = new List<Product>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Products;
                int StartColumn = 1;
                int StartRow = 1;
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
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string SupplierCodeValue = worksheet.Cells[i + StartRow, SupplierCodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string ScanCodeValue = worksheet.Cells[i + StartRow, ScanCodeColumn].Value?.ToString();
                    string ProductTypeIdValue = worksheet.Cells[i + StartRow, ProductTypeIdColumn].Value?.ToString();
                    string SupplierIdValue = worksheet.Cells[i + StartRow, SupplierIdColumn].Value?.ToString();
                    string BrandIdValue = worksheet.Cells[i + StartRow, BrandIdColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string UnitOfMeasureGroupingIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureGroupingIdColumn].Value?.ToString();
                    string SalePriceValue = worksheet.Cells[i + StartRow, SalePriceColumn].Value?.ToString();
                    string RetailPriceValue = worksheet.Cells[i + StartRow, RetailPriceColumn].Value?.ToString();
                    string TaxTypeIdValue = worksheet.Cells[i + StartRow, TaxTypeIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    Product Product = new Product();
                    Product.Code = CodeValue;
                    Product.SupplierCode = SupplierCodeValue;
                    Product.Name = NameValue;
                    Product.Description = DescriptionValue;
                    Product.ScanCode = ScanCodeValue;
                    Product.SalePrice = decimal.TryParse(SalePriceValue, out decimal SalePrice) ? SalePrice : 0;
                    Product.RetailPrice = decimal.TryParse(RetailPriceValue, out decimal RetailPrice) ? RetailPrice : 0;
                    Products.Add(Product);
                }
            }

            if (!await ProductValidator.Import(Products))
                return Products;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.BulkMerge(Products);
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
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.SupplierCode))
                    subFilter.SupplierCode = Map(subFilter.SupplierCode, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Description))
                    subFilter.Description = Map(subFilter.Description, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ScanCode))
                    subFilter.ScanCode = Map(subFilter.ScanCode, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ProductTypeId))
                    subFilter.ProductTypeId = Map(subFilter.ProductTypeId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.SupplierId))
                    subFilter.SupplierId = Map(subFilter.SupplierId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.BrandId))
                    subFilter.BrandId = Map(subFilter.BrandId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.UnitOfMeasureId))
                    subFilter.UnitOfMeasureId = Map(subFilter.UnitOfMeasureId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.UnitOfMeasureGroupingId))
                    subFilter.UnitOfMeasureGroupingId = Map(subFilter.UnitOfMeasureGroupingId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.SalePrice))
                    subFilter.SalePrice = Map(subFilter.SalePrice, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.RetailPrice))
                    subFilter.RetailPrice = Map(subFilter.RetailPrice, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.TaxTypeId))
                    subFilter.TaxTypeId = Map(subFilter.TaxTypeId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
