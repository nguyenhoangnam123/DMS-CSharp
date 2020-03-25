using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IProductRepository
    {
        Task<int> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
        Task<bool> Create(Product Product);
        Task<bool> Update(Product Product);
        Task<bool> Delete(Product Product);
        Task<bool> BulkMerge(List<Product> Products);
        Task<bool> BulkDelete(List<Product> Products);
    }
    public class ProductRepository : IProductRepository
    {
        private DataContext DataContext;
        public ProductRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProductDAO> DynamicFilter(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.SupplierCode != null)
                query = query.Where(q => q.SupplierCode, filter.SupplierCode);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.ScanCode != null)
                query = query.Where(q => q.ScanCode, filter.ScanCode);
            if (filter.ProductTypeId != null)
                query = query.Where(q => q.ProductTypeId, filter.ProductTypeId);
            if (filter.SupplierId != null)
                query = query.Where(q => q.SupplierId, filter.SupplierId);
            if (filter.BrandId != null)
                query = query.Where(q => q.BrandId, filter.BrandId);
            if (filter.UnitOfMeasureId != null)
                query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
            if (filter.UnitOfMeasureGroupingId != null)
                query = query.Where(q => q.UnitOfMeasureGroupingId, filter.UnitOfMeasureGroupingId);
            if (filter.SalePrice != null)
                query = query.Where(q => q.SalePrice, filter.SalePrice);
            if (filter.RetailPrice != null)
                query = query.Where(q => q.RetailPrice, filter.RetailPrice);
            if (filter.TaxTypeId != null)
                query = query.Where(q => q.TaxTypeId, filter.TaxTypeId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.OtherName != null)
                query = query.Where(q => q.OtherName, filter.OtherName);
            if (filter.TechnicalName != null)
                query = query.Where(q => q.TechnicalName, filter.TechnicalName);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ProductDAO> OrFilter(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProductDAO> initQuery = query.Where(q => false);
            foreach (ProductFilter ProductFilter in filter.OrFilter)
            {
                IQueryable<ProductDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.SupplierCode != null)
                    queryable = queryable.Where(q => q.SupplierCode, filter.SupplierCode);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Description != null)
                    queryable = queryable.Where(q => q.Description, filter.Description);
                if (filter.ScanCode != null)
                    queryable = queryable.Where(q => q.ScanCode, filter.ScanCode);
                if (filter.ProductTypeId != null)
                    queryable = queryable.Where(q => q.ProductTypeId, filter.ProductTypeId);
                if (filter.SupplierId != null)
                    queryable = queryable.Where(q => q.SupplierId, filter.SupplierId);
                if (filter.BrandId != null)
                    queryable = queryable.Where(q => q.BrandId, filter.BrandId);
                if (filter.UnitOfMeasureId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
                if (filter.UnitOfMeasureGroupingId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureGroupingId, filter.UnitOfMeasureGroupingId);
                if (filter.SalePrice != null)
                    queryable = queryable.Where(q => q.SalePrice, filter.SalePrice);
                if (filter.RetailPrice != null)
                    queryable = queryable.Where(q => q.RetailPrice, filter.RetailPrice);
                if (filter.TaxTypeId != null)
                    queryable = queryable.Where(q => q.TaxTypeId, filter.TaxTypeId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.OtherName != null)
                    queryable = queryable.Where(q => q.OtherName, filter.OtherName);
                if (filter.TechnicalName != null)
                    queryable = queryable.Where(q => q.TechnicalName, filter.TechnicalName);
                if (filter.Note != null)
                    queryable = queryable.Where(q => q.Note, filter.Note);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProductDAO> DynamicOrder(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProductOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProductOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProductOrder.SupplierCode:
                            query = query.OrderBy(q => q.SupplierCode);
                            break;
                        case ProductOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProductOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case ProductOrder.ScanCode:
                            query = query.OrderBy(q => q.ScanCode);
                            break;
                        case ProductOrder.ProductType:
                            query = query.OrderBy(q => q.ProductTypeId);
                            break;
                        case ProductOrder.Supplier:
                            query = query.OrderBy(q => q.SupplierId);
                            break;
                        case ProductOrder.Brand:
                            query = query.OrderBy(q => q.BrandId);
                            break;
                        case ProductOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case ProductOrder.UnitOfMeasureGrouping:
                            query = query.OrderBy(q => q.UnitOfMeasureGroupingId);
                            break;
                        case ProductOrder.SalePrice:
                            query = query.OrderBy(q => q.SalePrice);
                            break;
                        case ProductOrder.RetailPrice:
                            query = query.OrderBy(q => q.RetailPrice);
                            break;
                        case ProductOrder.TaxType:
                            query = query.OrderBy(q => q.TaxTypeId);
                            break;
                        case ProductOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ProductOrder.OtherName:
                            query = query.OrderBy(q => q.OtherName);
                            break;
                        case ProductOrder.TechnicalName:
                            query = query.OrderBy(q => q.TechnicalName);
                            break;
                        case ProductOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProductOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProductOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProductOrder.SupplierCode:
                            query = query.OrderByDescending(q => q.SupplierCode);
                            break;
                        case ProductOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProductOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case ProductOrder.ScanCode:
                            query = query.OrderByDescending(q => q.ScanCode);
                            break;
                        case ProductOrder.ProductType:
                            query = query.OrderByDescending(q => q.ProductTypeId);
                            break;
                        case ProductOrder.Supplier:
                            query = query.OrderByDescending(q => q.SupplierId);
                            break;
                        case ProductOrder.Brand:
                            query = query.OrderByDescending(q => q.BrandId);
                            break;
                        case ProductOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case ProductOrder.UnitOfMeasureGrouping:
                            query = query.OrderByDescending(q => q.UnitOfMeasureGroupingId);
                            break;
                        case ProductOrder.SalePrice:
                            query = query.OrderByDescending(q => q.SalePrice);
                            break;
                        case ProductOrder.RetailPrice:
                            query = query.OrderByDescending(q => q.RetailPrice);
                            break;
                        case ProductOrder.TaxType:
                            query = query.OrderByDescending(q => q.TaxTypeId);
                            break;
                        case ProductOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ProductOrder.OtherName:
                            query = query.OrderByDescending(q => q.OtherName);
                            break;
                        case ProductOrder.TechnicalName:
                            query = query.OrderByDescending(q => q.TechnicalName);
                            break;
                        case ProductOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Product>> DynamicSelect(IQueryable<ProductDAO> query, ProductFilter filter)
        {
            List<Product> Products = await query.Select(q => new Product()
            {
                Id = filter.Selects.Contains(ProductSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProductSelect.Code) ? q.Code : default(string),
                SupplierCode = filter.Selects.Contains(ProductSelect.SupplierCode) ? q.SupplierCode : default(string),
                Name = filter.Selects.Contains(ProductSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(ProductSelect.Description) ? q.Description : default(string),
                ScanCode = filter.Selects.Contains(ProductSelect.ScanCode) ? q.ScanCode : default(string),
                ProductTypeId = filter.Selects.Contains(ProductSelect.ProductType) ? q.ProductTypeId : default(long),
                SupplierId = filter.Selects.Contains(ProductSelect.Supplier) ? q.SupplierId : default(long?),
                BrandId = filter.Selects.Contains(ProductSelect.Brand) ? q.BrandId : default(long?),
                UnitOfMeasureId = filter.Selects.Contains(ProductSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                UnitOfMeasureGroupingId = filter.Selects.Contains(ProductSelect.UnitOfMeasureGrouping) ? q.UnitOfMeasureGroupingId : default(long?),
                SalePrice = filter.Selects.Contains(ProductSelect.SalePrice) ? q.SalePrice : default(decimal?),
                RetailPrice = filter.Selects.Contains(ProductSelect.RetailPrice) ? q.RetailPrice : default(decimal?),
                TaxTypeId = filter.Selects.Contains(ProductSelect.TaxType) ? q.TaxTypeId : default(long?),
                StatusId = filter.Selects.Contains(ProductSelect.Status) ? q.StatusId : default(long),
                OtherName = filter.Selects.Contains(ProductSelect.OtherName) ? q.OtherName : default(string),
                TechnicalName = filter.Selects.Contains(ProductSelect.TechnicalName) ? q.TechnicalName : default(string),
                Note = filter.Selects.Contains(ProductSelect.Note) ? q.Note : default(string),
                Brand = filter.Selects.Contains(ProductSelect.Brand) && q.Brand != null ? new Brand
                {
                    Id = q.Brand.Id,
                    Code = q.Brand.Code,
                    Name = q.Brand.Name,
                    StatusId = q.Brand.StatusId,
                } : null,
                ProductType = filter.Selects.Contains(ProductSelect.ProductType) && q.ProductType != null ? new ProductType
                {
                    Id = q.ProductType.Id,
                    Code = q.ProductType.Code,
                    Name = q.ProductType.Name,
                    Description = q.ProductType.Description,
                    StatusId = q.ProductType.StatusId,
                } : null,
                Status = filter.Selects.Contains(ProductSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Supplier = filter.Selects.Contains(ProductSelect.Supplier) && q.Supplier != null ? new Supplier
                {
                    Id = q.Supplier.Id,
                    Code = q.Supplier.Code,
                    Name = q.Supplier.Name,
                    TaxCode = q.Supplier.TaxCode,
                    StatusId = q.Supplier.StatusId,
                } : null,
                TaxType = filter.Selects.Contains(ProductSelect.TaxType) && q.TaxType != null ? new TaxType
                {
                    Id = q.TaxType.Id,
                    Code = q.TaxType.Code,
                    Name = q.TaxType.Name,
                    Percentage = q.TaxType.Percentage,
                    StatusId = q.TaxType.StatusId,
                } : null,
                UnitOfMeasure = filter.Selects.Contains(ProductSelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                } : null,
                UnitOfMeasureGrouping = filter.Selects.Contains(ProductSelect.UnitOfMeasureGrouping) && q.UnitOfMeasureGrouping != null ? new UnitOfMeasureGrouping
                {
                    Id = q.UnitOfMeasureGrouping.Id,
                    Name = q.UnitOfMeasureGrouping.Name,
                    UnitOfMeasureId = q.UnitOfMeasureGrouping.UnitOfMeasureId,
                    StatusId = q.UnitOfMeasureGrouping.StatusId,
                } : null,
            }).ToListAsync();
            return Products;
        }

        public async Task<int> Count(ProductFilter filter)
        {
            IQueryable<ProductDAO> Products = DataContext.Product;
            Products = DynamicFilter(Products, filter);
            return await Products.CountAsync();
        }

        public async Task<List<Product>> List(ProductFilter filter)
        {
            if (filter == null) return new List<Product>();
            IQueryable<ProductDAO> ProductDAOs = DataContext.Product;
            ProductDAOs = DynamicFilter(ProductDAOs, filter);
            ProductDAOs = DynamicOrder(ProductDAOs, filter);
            List<Product> Products = await DynamicSelect(ProductDAOs, filter);
            return Products;
        }

        public async Task<Product> Get(long Id)
        {
            Product Product = await DataContext.Product.Where(x => x.Id == Id).Select(x => new Product()
            {
                Id = x.Id,
                Code = x.Code,
                SupplierCode = x.SupplierCode,
                Name = x.Name,
                Description = x.Description,
                ScanCode = x.ScanCode,
                ProductTypeId = x.ProductTypeId,
                SupplierId = x.SupplierId,
                BrandId = x.BrandId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                SalePrice = x.SalePrice,
                RetailPrice = x.RetailPrice,
                TaxTypeId = x.TaxTypeId,
                StatusId = x.StatusId,
                Brand = x.Brand == null ? null : new Brand
                {
                    Id = x.Brand.Id,
                    Code = x.Brand.Code,
                    Name = x.Brand.Name,
                    StatusId = x.Brand.StatusId,
                },
                ProductType = x.ProductType == null ? null : new ProductType
                {
                    Id = x.ProductType.Id,
                    Code = x.ProductType.Code,
                    Name = x.ProductType.Name,
                    Description = x.ProductType.Description,
                    StatusId = x.ProductType.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Supplier = x.Supplier == null ? null : new Supplier
                {
                    Id = x.Supplier.Id,
                    Code = x.Supplier.Code,
                    Name = x.Supplier.Name,
                    TaxCode = x.Supplier.TaxCode,
                    StatusId = x.Supplier.StatusId,
                },
                TaxType = x.TaxType == null ? null : new TaxType
                {
                    Id = x.TaxType.Id,
                    Code = x.TaxType.Code,
                    Name = x.TaxType.Name,
                    Percentage = x.TaxType.Percentage,
                    StatusId = x.TaxType.StatusId,
                },
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                },
                UnitOfMeasureGrouping = x.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                {
                    Id = x.UnitOfMeasureGrouping.Id,
                    Name = x.UnitOfMeasureGrouping.Name,
                    UnitOfMeasureId = x.UnitOfMeasureGrouping.UnitOfMeasureId,
                    StatusId = x.UnitOfMeasureGrouping.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (Product == null)
                return null;
            Product.Items = await DataContext.Item
                .Where(x => x.ProductId == Product.Id)
                .Select(x => new Item
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Code = x.Code,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
                }).ToListAsync();
            Product.ProductImageMappings = await DataContext.ProductImageMapping
                .Where(x => x.ProductId == Product.Id)
                .Select(x => new ProductImageMapping
                {
                    ProductId = x.ProductId,
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                }).ToListAsync();
            Product.ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping
                .Where(x => x.ProductId == Product.Id)
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    ProductGroupingId = x.ProductGroupingId,
                    ProductGrouping = new ProductGrouping
                    {
                        Id = x.ProductGrouping.Id,
                        Code = x.ProductGrouping.Code,
                        Name = x.ProductGrouping.Name,
                        ParentId = x.ProductGrouping.ParentId,
                        Path = x.ProductGrouping.Path,
                        Description = x.ProductGrouping.Description,
                    },
                }).ToListAsync();
            Product.VariationGroupings = await DataContext.VariationGrouping
                .Where(x => x.ProductId == Product.Id)
                .Select(x => new VariationGrouping
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductId = x.ProductId,
                }).ToListAsync();

            return Product;
        }
        public async Task<bool> Create(Product Product)
        {
            ProductDAO ProductDAO = new ProductDAO();
            ProductDAO.Id = Product.Id;
            ProductDAO.Code = Product.Code;
            ProductDAO.SupplierCode = Product.SupplierCode;
            ProductDAO.Name = Product.Name;
            ProductDAO.Description = Product.Description;
            ProductDAO.ScanCode = Product.ScanCode;
            ProductDAO.ProductTypeId = Product.ProductTypeId;
            ProductDAO.SupplierId = Product.SupplierId;
            ProductDAO.BrandId = Product.BrandId;
            ProductDAO.UnitOfMeasureId = Product.UnitOfMeasureId;
            ProductDAO.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
            ProductDAO.SalePrice = Product.SalePrice;
            ProductDAO.RetailPrice = Product.RetailPrice;
            ProductDAO.TaxTypeId = Product.TaxTypeId;
            ProductDAO.StatusId = Product.StatusId;
            ProductDAO.CreatedAt = StaticParams.DateTimeNow;
            ProductDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Product.Add(ProductDAO);
            await DataContext.SaveChangesAsync();
            Product.Id = ProductDAO.Id;
            await SaveReference(Product);
            return true;
        }

        public async Task<bool> Update(Product Product)
        {
            ProductDAO ProductDAO = DataContext.Product.Where(x => x.Id == Product.Id).FirstOrDefault();
            if (ProductDAO == null)
                return false;
            ProductDAO.Id = Product.Id;
            ProductDAO.Code = Product.Code;
            ProductDAO.SupplierCode = Product.SupplierCode;
            ProductDAO.Name = Product.Name;
            ProductDAO.Description = Product.Description;
            ProductDAO.ScanCode = Product.ScanCode;
            ProductDAO.ProductTypeId = Product.ProductTypeId;
            ProductDAO.SupplierId = Product.SupplierId;
            ProductDAO.BrandId = Product.BrandId;
            ProductDAO.UnitOfMeasureId = Product.UnitOfMeasureId;
            ProductDAO.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
            ProductDAO.SalePrice = Product.SalePrice;
            ProductDAO.RetailPrice = Product.RetailPrice;
            ProductDAO.TaxTypeId = Product.TaxTypeId;
            ProductDAO.StatusId = Product.StatusId;
            ProductDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Product);
            return true;
        }

        public async Task<bool> Delete(Product Product)
        {
            await DataContext.Product.Where(x => x.Id == Product.Id).UpdateFromQueryAsync(x => new ProductDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Product> Products)
        {
            List<ProductDAO> ProductDAOs = new List<ProductDAO>();
            foreach (Product Product in Products)
            {
                ProductDAO ProductDAO = new ProductDAO();
                ProductDAO.Id = Product.Id;
                ProductDAO.Code = Product.Code;
                ProductDAO.SupplierCode = Product.SupplierCode;
                ProductDAO.Name = Product.Name;
                ProductDAO.Description = Product.Description;
                ProductDAO.ScanCode = Product.ScanCode;
                ProductDAO.ProductTypeId = Product.ProductTypeId;
                ProductDAO.SupplierId = Product.SupplierId;
                ProductDAO.BrandId = Product.BrandId;
                ProductDAO.UnitOfMeasureId = Product.UnitOfMeasureId;
                ProductDAO.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
                ProductDAO.SalePrice = Product.SalePrice;
                ProductDAO.RetailPrice = Product.RetailPrice;
                ProductDAO.TaxTypeId = Product.TaxTypeId;
                ProductDAO.StatusId = Product.StatusId;
                ProductDAO.CreatedAt = StaticParams.DateTimeNow;
                ProductDAO.UpdatedAt = StaticParams.DateTimeNow;
                ProductDAOs.Add(ProductDAO);
            }
            await DataContext.BulkMergeAsync(ProductDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Product> Products)
        {
            List<long> Ids = Products.Select(x => x.Id).ToList();
            await DataContext.Product
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProductDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Product Product)
        {
            List<ItemDAO> ItemDAOs = await DataContext.Item
                .Where(x => x.ProductId == Product.Id).ToListAsync();
            ItemDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Product.Items != null)
            {
                foreach (Item Item in Product.Items)
                {
                    ItemDAO ItemDAO = ItemDAOs
                        .Where(x => x.Id == Item.Id && x.Id != 0).FirstOrDefault();
                    if (ItemDAO == null)
                    {
                        ItemDAO = new ItemDAO();
                        ItemDAO.Id = Item.Id;
                        ItemDAO.ProductId = Product.Id;
                        ItemDAO.Code = Item.Code;
                        ItemDAO.Name = Item.Name;
                        ItemDAO.ScanCode = Item.ScanCode;
                        ItemDAO.SalePrice = Item.SalePrice;
                        ItemDAO.RetailPrice = Item.RetailPrice;
                        ItemDAOs.Add(ItemDAO);
                        ItemDAO.CreatedAt = StaticParams.DateTimeNow;
                        ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ItemDAO.DeletedAt = null;
                    }
                    else
                    {
                        ItemDAO.Id = Item.Id;
                        ItemDAO.ProductId = Product.Id;
                        ItemDAO.Code = Item.Code;
                        ItemDAO.Name = Item.Name;
                        ItemDAO.ScanCode = Item.ScanCode;
                        ItemDAO.SalePrice = Item.SalePrice;
                        ItemDAO.RetailPrice = Item.RetailPrice;
                        ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ItemDAO.DeletedAt = null;
                    }
                }
                await DataContext.Item.BulkMergeAsync(ItemDAOs);
            }
            await DataContext.ProductImageMapping
                .Where(x => x.ProductId == Product.Id)
                .DeleteFromQueryAsync();
            List<ProductImageMappingDAO> ProductImageMappingDAOs = new List<ProductImageMappingDAO>();
            if (Product.ProductImageMappings != null)
            {
                foreach (ProductImageMapping ProductImageMapping in Product.ProductImageMappings)
                {
                    ProductImageMappingDAO ProductImageMappingDAO = new ProductImageMappingDAO();
                    ProductImageMappingDAO.ProductId = Product.Id;
                    ProductImageMappingDAO.ImageId = ProductImageMapping.ImageId;
                    ProductImageMappingDAOs.Add(ProductImageMappingDAO);
                }
                await DataContext.ProductImageMapping.BulkMergeAsync(ProductImageMappingDAOs);
            }
            await DataContext.ProductProductGroupingMapping
                .Where(x => x.ProductId == Product.Id)
                .DeleteFromQueryAsync();
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            if (Product.ProductProductGroupingMappings != null)
            {
                foreach (ProductProductGroupingMapping ProductProductGroupingMapping in Product.ProductProductGroupingMappings)
                {
                    ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO();
                    ProductProductGroupingMappingDAO.ProductId = Product.Id;
                    ProductProductGroupingMappingDAO.ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId;
                    ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                }
                await DataContext.ProductProductGroupingMapping.BulkMergeAsync(ProductProductGroupingMappingDAOs);
            }
            List<VariationGroupingDAO> VariationGroupingDAOs = await DataContext.VariationGrouping
                .Where(x => x.ProductId == Product.Id).ToListAsync();
            VariationGroupingDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Product.VariationGroupings != null)
            {
                foreach (VariationGrouping VariationGrouping in Product.VariationGroupings)
                {
                    VariationGroupingDAO VariationGroupingDAO = VariationGroupingDAOs
                        .Where(x => x.Id == VariationGrouping.Id && x.Id != 0).FirstOrDefault();
                    if (VariationGroupingDAO == null)
                    {
                        VariationGroupingDAO = new VariationGroupingDAO();
                        VariationGroupingDAO.Id = VariationGrouping.Id;
                        VariationGroupingDAO.Name = VariationGrouping.Name;
                        VariationGroupingDAO.ProductId = Product.Id;
                        VariationGroupingDAOs.Add(VariationGroupingDAO);
                        VariationGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                        VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                        VariationGroupingDAO.DeletedAt = null;
                    }
                    else
                    {
                        VariationGroupingDAO.Id = VariationGrouping.Id;
                        VariationGroupingDAO.Name = VariationGrouping.Name;
                        VariationGroupingDAO.ProductId = Product.Id;
                        VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                        VariationGroupingDAO.DeletedAt = null;
                    }
                }
                await DataContext.VariationGrouping.BulkMergeAsync(VariationGroupingDAOs);
            }
        }

    }
}
