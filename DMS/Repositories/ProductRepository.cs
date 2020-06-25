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
        Task<bool> BulkInsertNewProduct(List<Product> Products);
        Task<bool> BulkDeleteNewProduct(List<Product> Products);
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
            if (filter.UsedVariationId != null)
                query = query.Where(q => q.UsedVariationId, filter.UsedVariationId);
            if (filter.ProductGroupingId != null)
            {
                if (filter.ProductGroupingId.Equal != null)
                {
                    ProductGroupingDAO ProductGroupingDAO = DataContext.ProductGrouping
                        .Where(o => o.Id == filter.ProductGroupingId.Equal.Value).FirstOrDefault();
                    query = from q in query
                            join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                            join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                            where pg.Path.StartsWith(ProductGroupingDAO.Path)
                            select q;
                }
                if (filter.ProductGroupingId.NotEqual != null)
                {
                    ProductGroupingDAO ProductGroupingDAO = DataContext.ProductGrouping
                        .Where(o => o.Id == filter.ProductGroupingId.NotEqual.Value).FirstOrDefault();
                    query = from q in query
                            join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                            join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                            where !pg.Path.StartsWith(ProductGroupingDAO.Path)
                            select q;
                }
                if (filter.ProductGroupingId.In != null)
                {
                    List<ProductGroupingDAO> ProductGroupingDAOs = DataContext.ProductGrouping
                        .Where(o => o.DeletedAt == null).ToList();
                    List<ProductGroupingDAO> Parents = ProductGroupingDAOs.Where(o => filter.ProductGroupingId.In.Contains(o.Id)).ToList();
                    List<ProductGroupingDAO> Branches = ProductGroupingDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> ProductGroupingIds = Branches.Select(x => x.Id).ToList();
                    query = from q in query
                            join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                            join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                            where ProductGroupingIds.Contains(pg.Id)
                            select q;
                }
                if (filter.ProductGroupingId.NotIn != null)
                {
                    List<ProductGroupingDAO> ProductGroupingDAOs = DataContext.ProductGrouping
                        .Where(o => o.DeletedAt == null).ToList();
                    List<ProductGroupingDAO> Parents = ProductGroupingDAOs.Where(o => filter.ProductGroupingId.NotIn.Contains(o.Id)).ToList();
                    List<ProductGroupingDAO> Branches = ProductGroupingDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> ProductGroupingIds = Branches.Select(x => x.Id).ToList();
                    query = from q in query
                            join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                            join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                            where !ProductGroupingIds.Contains(pg.Id)
                            select q;
                }
            }
            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(q => q.Code.Contains(filter.Search) || q.Name.Contains(filter.Search));
            if (filter.IsNew != null)
                query = query.Where(q => q.IsNew.Equals(filter.IsNew));
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
                if (ProductFilter.ProductTypeId != null)
                    queryable = queryable.Where(q => q.ProductTypeId, ProductFilter.ProductTypeId);
                if (ProductFilter.ProductGroupingId != null)
                {
                    if (ProductFilter.ProductGroupingId.Equal != null)
                    {
                        ProductGroupingDAO ProductGroupingDAO = DataContext.ProductGrouping
                            .Where(o => o.Id == ProductFilter.ProductGroupingId.Equal.Value).FirstOrDefault();
                        queryable = from q in queryable
                                    join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                                    join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                                    where pg.Path.StartsWith(ProductGroupingDAO.Path)
                                    select q;
                    }
                    if (ProductFilter.ProductGroupingId.NotEqual != null)
                    {
                        ProductGroupingDAO ProductGroupingDAO = DataContext.ProductGrouping
                            .Where(o => o.Id == ProductFilter.ProductGroupingId.NotEqual.Value).FirstOrDefault();
                        queryable = from q in queryable
                                    join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                                    join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                                    where !pg.Path.StartsWith(ProductGroupingDAO.Path)
                                    select q;
                    }
                    if (ProductFilter.ProductGroupingId.In != null)
                    {
                        List<ProductGroupingDAO> ProductGroupingDAOs = DataContext.ProductGrouping
                            .Where(o => o.DeletedAt == null).ToList();
                        List<ProductGroupingDAO> Parents = ProductGroupingDAOs.Where(o => ProductFilter.ProductGroupingId.In.Contains(o.Id)).ToList();
                        List<ProductGroupingDAO> Branches = ProductGroupingDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> ProductGroupingIds = Branches.Select(o => o.Id).ToList();
                        queryable = from q in queryable
                                    join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                                    join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                                    where ProductGroupingIds.Contains(pg.Id)
                                    select q;
                    }
                    if (ProductFilter.ProductGroupingId.NotIn != null)
                    {
                        List<ProductGroupingDAO> ProductGroupingDAOs = DataContext.ProductGrouping
                            .Where(o => o.DeletedAt == null).ToList();
                        List<ProductGroupingDAO> Parents = ProductGroupingDAOs.Where(o => ProductFilter.ProductGroupingId.NotIn.Contains(o.Id)).ToList();
                        List<ProductGroupingDAO> Branches = ProductGroupingDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> ProductGroupingIds = Branches.Select(o => o.Id).ToList();
                        queryable = from q in queryable
                                    join ppg in DataContext.ProductProductGroupingMapping on q.Id equals ppg.ProductId
                                    join pg in DataContext.ProductGrouping on ppg.ProductGroupingId equals pg.Id
                                    where !ProductGroupingIds.Contains(pg.Id)
                                    select q;
                    }
                }

                if (ProductFilter.IsNew != null)
                    queryable = queryable.Where(q => q.IsNew.Equals(ProductFilter.IsNew));
                if (ProductFilter.UsedVariationId != null)
                    queryable = queryable.Where(q => q.UsedVariationId, ProductFilter.UsedVariationId);
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
                            query = query.OrderBy(q => q.ProductType.Name);
                            break;
                        case ProductOrder.Supplier:
                            query = query.OrderBy(q => q.Supplier.Name);
                            break;
                        case ProductOrder.Brand:
                            query = query.OrderBy(q => q.Brand.Name);
                            break;
                        case ProductOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasure.Name);
                            break;
                        case ProductOrder.UnitOfMeasureGrouping:
                            query = query.OrderBy(q => q.UnitOfMeasureGrouping.Name);
                            break;
                        case ProductOrder.SalePrice:
                            query = query.OrderBy(q => q.SalePrice);
                            break;
                        case ProductOrder.RetailPrice:
                            query = query.OrderBy(q => q.RetailPrice);
                            break;
                        case ProductOrder.TaxType:
                            query = query.OrderBy(q => q.TaxType.Code);
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
                        case ProductOrder.UsedVariation:
                            query = query.OrderBy(q => q.UsedVariationId);
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
                            query = query.OrderByDescending(q => q.ProductType.Name);
                            break;
                        case ProductOrder.Supplier:
                            query = query.OrderByDescending(q => q.Supplier.Name);
                            break;
                        case ProductOrder.Brand:
                            query = query.OrderByDescending(q => q.Brand.Name);
                            break;
                        case ProductOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasure.Name);
                            break;
                        case ProductOrder.UnitOfMeasureGrouping:
                            query = query.OrderByDescending(q => q.UnitOfMeasureGrouping.Name);
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
                        case ProductOrder.UsedVariation:
                            query = query.OrderByDescending(q => q.UsedVariationId);
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
                ERPCode = filter.Selects.Contains(ProductSelect.ERPCode) ? q.ERPCode : default(string),
                ProductTypeId = filter.Selects.Contains(ProductSelect.ProductType) ? q.ProductTypeId : default(long),
                SupplierId = filter.Selects.Contains(ProductSelect.Supplier) ? q.SupplierId : default(long?),
                BrandId = filter.Selects.Contains(ProductSelect.Brand) ? q.BrandId : default(long?),
                UnitOfMeasureId = filter.Selects.Contains(ProductSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                UnitOfMeasureGroupingId = filter.Selects.Contains(ProductSelect.UnitOfMeasureGrouping) ? q.UnitOfMeasureGroupingId : default(long?),
                SalePrice = filter.Selects.Contains(ProductSelect.SalePrice) ? q.SalePrice : default(long),
                RetailPrice = filter.Selects.Contains(ProductSelect.RetailPrice) ? q.RetailPrice : default(long?),
                TaxTypeId = filter.Selects.Contains(ProductSelect.TaxType) ? q.TaxTypeId : default(long),
                StatusId = filter.Selects.Contains(ProductSelect.Status) ? q.StatusId : default(long),
                OtherName = filter.Selects.Contains(ProductSelect.OtherName) ? q.OtherName : default(string),
                TechnicalName = filter.Selects.Contains(ProductSelect.TechnicalName) ? q.TechnicalName : default(string),
                Note = filter.Selects.Contains(ProductSelect.Note) ? q.Note : default(string),
                IsNew = filter.Selects.Contains(ProductSelect.IsNew) ? q.IsNew : default(bool),
                UsedVariationId = filter.Selects.Contains(ProductSelect.UsedVariation) ? q.UsedVariationId : default(long),
                Brand = filter.Selects.Contains(ProductSelect.Brand) && q.Brand != null ? new Brand
                {
                    Id = q.Brand.Id,
                    Code = q.Brand.Code,
                    Name = q.Brand.Name,
                    Description = q.Brand.Description,
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
                UsedVariation = filter.Selects.Contains(ProductSelect.UsedVariation) && q.UsedVariation != null ? new UsedVariation
                {
                    Id = q.UsedVariation.Id,
                    Code = q.UsedVariation.Code,
                    Name = q.UsedVariation.Name,
                } : null,
                ProductProductGroupingMappings = filter.Selects.Contains(ProductSelect.ProductProductGroupingMapping) && q.ProductProductGroupingMappings != null ?
                q.ProductProductGroupingMappings.Select(p => new ProductProductGroupingMapping
                {
                    ProductId = p.ProductId,
                    ProductGroupingId = p.ProductGroupingId,
                    ProductGrouping = new ProductGrouping
                    {
                        Id = p.ProductGrouping.Id,
                        Code = p.ProductGrouping.Code,
                        Name = p.ProductGrouping.Name,
                        ParentId = p.ProductGrouping.ParentId,
                        Path = p.ProductGrouping.Path,
                        Description = p.ProductGrouping.Description,
                    },
                }).ToList() : null,
                VariationGroupings = filter.Selects.Contains(ProductSelect.VariationGrouping) && q.VariationGroupings != null ?
                q.VariationGroupings.Select(v => new VariationGrouping
                {
                    ProductId = v.ProductId,
                    Name = v.Name,
                    Id = v.Id,
                    RowId = v.RowId,
                }).ToList() : null,
                Used = q.Used,
            }).ToListAsync();

            //Lấy ra 1 cái ảnh cho list product
            var Ids = Products.Select(x => x.Id).ToList();
            var ProductImageMappings = DataContext.ProductImageMapping.Include(x => x.Image).Where(x => Ids.Contains(x.ProductId)).ToList();
            foreach (var Product in Products)
            {
                Product.ProductImageMappings = new List<ProductImageMapping>();
                var ProductImageMappingDAO = ProductImageMappings.Where(x => x.ProductId == Product.Id).FirstOrDefault();
                if (ProductImageMappingDAO != null)
                {
                    ProductImageMapping ProductImageMapping = new ProductImageMapping
                    {
                        ImageId = ProductImageMappingDAO.ImageId,
                        ProductId = ProductImageMappingDAO.ProductId,
                        Image = new Image
                        {
                            Id = ProductImageMappingDAO.Image.Id,
                            Name = ProductImageMappingDAO.Image.Name,
                            Url = ProductImageMappingDAO.Image.Url
                        }
                    };
                    Product.ProductImageMappings.Add(ProductImageMapping);
                }
            }

            var VariationGroupingIds = Products.Where(x => x.VariationGroupings != null)
                .SelectMany(x => x.VariationGroupings).Select(x => x.Id).Distinct().ToList();
            if(VariationGroupingIds != null)
            {
                List<Variation> Variations = await DataContext.Variation.Where(x => VariationGroupingIds.Contains(x.VariationGroupingId))
                    .Select(x => new Variation
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        VariationGroupingId = x.VariationGroupingId,
                    }).ToListAsync();
                foreach (var Product in Products)
                {
                    if(Product.VariationGroupings != null)
                    {
                        foreach (var VariationGrouping in Product.VariationGroupings)
                        {
                            VariationGrouping.Variations = Variations.Where(x => x.VariationGroupingId == VariationGrouping.Id).ToList();
                        }
                    }
                }
            }
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
            IQueryable<ProductDAO> ProductDAOs = DataContext.Product.AsNoTracking();
            ProductDAOs = DynamicFilter(ProductDAOs, filter);
            ProductDAOs = DynamicOrder(ProductDAOs, filter);
            List<Product> Products = await DynamicSelect(ProductDAOs, filter);
            return Products;
        }

        public async Task<Product> Get(long Id)
        {
            Product Product = await DataContext.Product.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Product()
                {
                    Id = x.Id,
                    Code = x.Code,
                    SupplierCode = x.SupplierCode,
                    Name = x.Name,
                    ERPCode = x.ERPCode,
                    TechnicalName = x.TechnicalName,
                    OtherName = x.OtherName,
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
                    IsNew = x.IsNew,
                    UsedVariationId = x.UsedVariationId,
                    Used = x.Used,
                    Brand = x.Brand == null ? null : new Brand
                    {
                        Id = x.Brand.Id,
                        Code = x.Brand.Code,
                        Name = x.Brand.Name,
                        Description = x.Brand.Description,
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
                    UsedVariation = x.UsedVariation == null ? null : new UsedVariation
                    {
                        Id = x.UsedVariation.Id,
                        Code = x.UsedVariation.Code,
                        Name = x.UsedVariation.Name,
                    }
                }).FirstOrDefaultAsync();

            if (Product == null)
                return null;
            if (Product.UnitOfMeasureGrouping != null)
            {
                Product.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents = await DataContext.UnitOfMeasureGroupingContent
                    .Where(uomgc => uomgc.UnitOfMeasureGroupingId == Product.UnitOfMeasureGroupingId.Value)
                    .Select(uomgc => new UnitOfMeasureGroupingContent
                    {
                        Id = uomgc.Id,
                        Factor = uomgc.Factor,
                        UnitOfMeasureId = uomgc.UnitOfMeasureId,
                        UnitOfMeasure = new UnitOfMeasure
                        {
                            Id = uomgc.UnitOfMeasure.Id,
                            Code = uomgc.UnitOfMeasure.Code,
                            Name = uomgc.UnitOfMeasure.Name,
                            Description = uomgc.UnitOfMeasure.Description,
                            StatusId = uomgc.UnitOfMeasure.StatusId,
                        }
                    }).ToListAsync();
            }

            Product.Items = await DataContext.Item.AsNoTracking()
                .Where(x => x.ProductId == Product.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new Item
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Code = x.Code,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
                    StatusId = x.StatusId,
                    Used = x.Used,
                    ItemImageMappings = new List<ItemImageMapping>()
                }).ToListAsync();
            Product.ProductImageMappings = await DataContext.ProductImageMapping.AsNoTracking()
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
            Product.ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping.AsNoTracking()
                .Where(x => x.ProductId == Product.Id)
                .Where(x => x.ProductGrouping.DeletedAt == null)
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
            Product.VariationGroupings = await DataContext.VariationGrouping.Include(x => x.Variations)
                .Where(x => x.ProductId == Product.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new VariationGrouping
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductId = x.ProductId,
                    Variations = x.Variations.Select(v => new Variation
                    {
                        Id = v.Id,
                        Code = v.Code,
                        Name = v.Name,
                        VariationGroupingId = v.VariationGroupingId,
                    }).ToList(),
                }).ToListAsync();

            var ItemIds = Product.Items.Select(x => x.Id).ToList();
            List<ItemImageMapping> ItemImageMappings = await DataContext.ItemImageMapping.Where(x => ItemIds.Contains(x.ItemId)).Select(x => new ItemImageMapping
            {
                ImageId = x.ImageId,
                ItemId = x.ItemId,
                Image = new Image
                {
                    Id = x.Image.Id,
                    Url = x.Image.Url,
                    Name = x.Image.Name,
                }
            }).ToListAsync();

            foreach (var item in Product.Items)
            {
                item.ItemImageMappings = ItemImageMappings.Where(x => x.ItemId == item.Id).ToList();
            }

            List<ItemHistory> ItemHistories = await DataContext.ItemHistory.Where(x => ItemIds.Contains(x.ItemId)).Select(x => new ItemHistory
            {
                Id = x.Id,
                ItemId = x.ItemId,
                ModifierId = x.ModifierId,
                NewPrice = x.NewPrice,
                OldPrice = x.OldPrice,  
                Time = x.Time,
            }).ToListAsync();

            foreach (var item in Product.Items)
            {
                item.ItemHistories = ItemHistories.Where(x => x.ItemId == item.Id).ToList();
            }
            return Product;
        }
        public async Task<bool> Create(Product Product)
        {
            ProductDAO ProductDAO = new ProductDAO();
            ProductDAO.Id = Product.Id;
            ProductDAO.Code = Product.Code;
            ProductDAO.SupplierCode = Product.SupplierCode;
            ProductDAO.ERPCode = Product.ERPCode;
            ProductDAO.Name = Product.Name;
            ProductDAO.TechnicalName = Product.TechnicalName;
            ProductDAO.OtherName = Product.OtherName;
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
            ProductDAO.IsNew = Product.IsNew;
            ProductDAO.UsedVariationId = Product.UsedVariationId;
            ProductDAO.CreatedAt = StaticParams.DateTimeNow;
            ProductDAO.UpdatedAt = StaticParams.DateTimeNow;
            ProductDAO.Used = false;
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
            ProductDAO.ERPCode = Product.ERPCode;
            ProductDAO.Name = Product.Name;
            ProductDAO.TechnicalName = Product.TechnicalName;
            ProductDAO.OtherName = Product.OtherName;
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
            ProductDAO.IsNew = Product.IsNew;
            ProductDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Product);
            return true;
        }

        public async Task<bool> BulkInsertNewProduct(List<Product> Products)
        {
            var ProductIds = Products.Select(x => x.Id).ToList();
            await DataContext.Product.Where(x => ProductIds.Contains(x.Id)).UpdateFromQueryAsync(x => new ProductDAO
            {
                IsNew = true
            });

            return true;
        }

        public async Task<bool> Delete(Product Product)
        {
            await DataContext.ProductProductGroupingMapping.Where(x => x.ProductId == Product.Id).DeleteFromQueryAsync();
            var ItemIds = await DataContext.Item.Where(x => x.ProductId == Product.Id).Select(x => x.Id).ToListAsync();
            await DataContext.ItemHistory.Where(x => ItemIds.Contains(x.ItemId)).DeleteFromQueryAsync();
            await DataContext.Item.Where(x => x.ProductId == Product.Id).UpdateFromQueryAsync(x => new ItemDAO { DeletedAt = StaticParams.DateTimeNow });
            await DataContext.Product.Where(x => x.Id == Product.Id).UpdateFromQueryAsync(x => new ProductDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkDeleteNewProduct(List<Product> Products)
        {
            var ProductIds = Products.Select(x => x.Id).ToList();
            await DataContext.Product.Where(x => ProductIds.Contains(x.Id)).UpdateFromQueryAsync(x => new ProductDAO
            {
                IsNew = false
            });

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
                ProductDAO.ERPCode = Product.ERPCode;
                ProductDAO.Name = Product.Name;
                ProductDAO.TechnicalName = Product.TechnicalName;
                ProductDAO.OtherName = Product.OtherName;
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
                ProductDAO.IsNew = Product.IsNew;
                ProductDAO.UsedVariationId = Product.UsedVariationId;

                ProductDAO.CreatedAt = DateTime.Now;
                ProductDAO.UpdatedAt = DateTime.Now;
                ProductDAOs.Add(ProductDAO);
            }
            await DataContext.BulkMergeAsync(ProductDAOs);

            foreach (var Product in Products)
            {
                long ProductId = ProductDAOs.Where(p => p.Code == Product.Code).Select(p => p.Id).FirstOrDefault();
                if (Product.ProductProductGroupingMappings != null)
                    Product.ProductProductGroupingMappings.ForEach(x => x.ProductId = ProductId);

                if (Product.Items != null)
                    Product.Items.ForEach(i => i.ProductId = ProductId);

                if (Product.VariationGroupings != null)
                    Product.VariationGroupings.ForEach(vg => vg.ProductId = ProductId);
            }
            #region merge product grouping mapping
            List<ProductProductGroupingMapping> ProductProductGroupingMappings = Products.SelectMany(p => p.ProductProductGroupingMappings).ToList();
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            foreach (ProductProductGroupingMapping ProductProductGroupingMapping in ProductProductGroupingMappings)
            {
                ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO
                {
                    ProductId = ProductProductGroupingMapping.ProductId,
                    ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId,
                };
                ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
            }
            await DataContext.ProductProductGroupingMapping.BulkMergeAsync(ProductProductGroupingMappingDAOs);

            #endregion
            #region merge item
            List<Item> Items = Products.SelectMany(p => p.Items).ToList();
            List<ItemDAO> ItemDAOs = new List<ItemDAO>();
            foreach (Item Item in Items)
            {
                ItemDAO ItemDAO = new ItemDAO();
                ItemDAO.Id = Item.Id;
                ItemDAO.ProductId = Item.ProductId;
                ItemDAO.Code = Item.Code;
                ItemDAO.Name = Item.Name;
                ItemDAO.ScanCode = Item.ScanCode;
                ItemDAO.SalePrice = Item.SalePrice;
                ItemDAO.RetailPrice = Item.RetailPrice;
                ItemDAO.CreatedAt = StaticParams.DateTimeNow;
                ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                ItemDAO.StatusId = Item.StatusId;
                ItemDAOs.Add(ItemDAO);
            }
            await DataContext.Item.BulkMergeAsync(ItemDAOs);
            #endregion

            #region merge VariationGroupings
            List<VariationGrouping> VariationGroupings = Products.Where(x => x.VariationGroupings != null).SelectMany(p => p.VariationGroupings).ToList();
            VariationGroupings.ForEach(x => x.RowId = Guid.NewGuid());
            List<VariationGroupingDAO> VariationGroupingDAOs = new List<VariationGroupingDAO>();
            foreach (var VariationGrouping in VariationGroupings)
            {
                VariationGroupingDAO VariationGroupingDAO = new VariationGroupingDAO
                {
                    Id = VariationGrouping.Id,
                    Name = VariationGrouping.Name,
                    ProductId = VariationGrouping.ProductId,
                    RowId = VariationGrouping.RowId,
                    CreatedAt = StaticParams.DateTimeNow,
                    UpdatedAt = StaticParams.DateTimeNow
                };
                VariationGroupingDAOs.Add(VariationGroupingDAO);
            }

            await DataContext.VariationGrouping.BulkMergeAsync(VariationGroupingDAOs);
            #endregion

            #region merge Variation
            foreach (var VariationGrouping in VariationGroupings)
            {
                long VariationGroupingId = VariationGroupingDAOs
                    .Where(vg => vg.RowId == VariationGrouping.RowId)
                    .Select(vg => vg.Id).FirstOrDefault();
                if (VariationGrouping.Variations != null)
                    VariationGrouping.Variations.ForEach(v => v.VariationGroupingId = VariationGroupingId);
            }
            List<Variation> Variations = VariationGroupings.Where(x => x.Variations != null).SelectMany(p => p.Variations).ToList();
            List<VariationDAO> VariationDAOs = new List<VariationDAO>();
            foreach (var Variation in Variations)
            {
                VariationDAO VariationDAO = new VariationDAO
                {
                    Id = Variation.Id,
                    Code = Variation.Code,
                    Name = Variation.Name,
                    VariationGroupingId = Variation.VariationGroupingId,
                };
                VariationDAOs.Add(VariationDAO);
            }

            await DataContext.Variation.BulkMergeAsync(VariationDAOs);
            #endregion

            return true;
        }

        public async Task<bool> BulkDelete(List<Product> Products)
        {
            List<long> Ids = Products.Select(x => x.Id).ToList();
            List<long> ItemIds = DataContext.Item.Where(x => Ids.Contains(x.ProductId)).Select(x => x.Id).ToList();

            await DataContext.ItemHistory.Where(x => ItemIds.Contains(x.ItemId)).DeleteFromQueryAsync();
            await DataContext.Item.Where(x => Ids.Contains(x.ProductId))
                .UpdateFromQueryAsync(x => new ItemDAO { DeletedAt = StaticParams.DateTimeNow });
            await DataContext.ProductProductGroupingMapping
                .Where(x => Ids.Contains(x.ProductId))
                .DeleteFromQueryAsync();
            await DataContext.Product
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProductDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Product Product)
        {
            var ItemIds = await DataContext.Item.Where(x => x.ProductId == Product.Id).Select(x => x.Id).ToListAsync();
            await DataContext.ItemHistory.Where(x => ItemIds.Contains(x.ItemId)).DeleteFromQueryAsync();
            List<ItemDAO> ItemDAOs = await DataContext.Item
                .Where(x => x.ProductId == Product.Id).ToListAsync();
            foreach(ItemDAO ItemDAO in ItemDAOs)
            {
                if (ItemDAO.Used == false)
                {
                    ItemDAO.DeletedAt = StaticParams.DateTimeNow;
                }    
            }    
            if (Product.Items != null)
            {
                foreach (Item Item in Product.Items)
                {
                    ItemDAO ItemDAO = ItemDAOs
                        .Where(x => x.Id == Item.Id && x.Id != 0).FirstOrDefault();
                    if (ItemDAO == null)
                    {
                        ItemDAO = new ItemDAO()
                        {
                            ProductId = Product.Id,
                            Code = Item.Code,
                            Name = Item.Name,
                            ScanCode = Item.ScanCode,
                            SalePrice = Item.SalePrice,
                            RetailPrice = Item.RetailPrice,
                            StatusId = Item.StatusId,
                            CreatedAt = StaticParams.DateTimeNow,
                            UpdatedAt = StaticParams.DateTimeNow,
                            DeletedAt = null,
                            Used = false,
                        };
                        ItemDAOs.Add(ItemDAO);
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
                        ItemDAO.StatusId = Item.StatusId;
                        ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ItemDAO.DeletedAt = null;
                    }
                }
                await DataContext.Item.BulkMergeAsync(ItemDAOs);
                List<ItemHistoryDAO> ItemHistoryDAOs = new List<ItemHistoryDAO>();
                foreach (Item Item in Product.Items)
                {
                    Item.Id = ItemDAOs.Where(i => i.Code == Item.Code).Select(x => x.Id).FirstOrDefault();

                    var list = new List<ItemHistoryDAO>();
                    if (Item.ItemHistories != null)
                    {
                        list = Item.ItemHistories.Select(x => new ItemHistoryDAO
                        {
                            Id = x.Id,
                            Time = x.Time,
                            ItemId = Item.Id,
                            ModifierId = x.ModifierId,
                            OldPrice = x.OldPrice,
                            NewPrice = x.NewPrice,
                        }).ToList();
                        ItemHistoryDAOs.AddRange(list);
                    }
                }

                await DataContext.ItemHistory.BulkMergeAsync(ItemHistoryDAOs);
            }
            await DataContext.ProductImageMapping
                .Where(x => x.ProductId == Product.Id)
                .DeleteFromQueryAsync();
            List<ProductImageMappingDAO> ProductImageMappingDAOs = new List<ProductImageMappingDAO>();
            if (Product.ProductImageMappings != null)
            {
                foreach (ProductImageMapping ProductImageMapping in Product.ProductImageMappings)
                {
                    ProductImageMappingDAO ProductImageMappingDAO = new ProductImageMappingDAO()
                    {
                        ProductId = Product.Id,
                        ImageId = ProductImageMapping.ImageId,
                    };
                    ProductImageMappingDAOs.Add(ProductImageMappingDAO);
                }
                await DataContext.ProductImageMapping.BulkMergeAsync(ProductImageMappingDAOs);
            }
            ItemIds = ItemDAOs.Select(x => x.Id).ToList();
            await DataContext.ItemImageMapping
                .Where(x => ItemIds.Contains(x.ItemId))
                .DeleteFromQueryAsync();
            List<ItemImageMappingDAO> ItemImageMappingDAOs = new List<ItemImageMappingDAO>();
            if (Product.Items != null)
            {
                foreach (var Item in Product.Items)
                {
                    if (Item.ItemImageMappings != null)
                        foreach (ItemImageMapping ItemImageMapping in Item.ItemImageMappings)
                        {
                            ItemImageMappingDAO ItemImageMappingDAO = new ItemImageMappingDAO()
                            {
                                ItemId = Item.Id,
                                ImageId = ItemImageMapping.ImageId
                            };
                            ItemImageMappingDAOs.Add(ItemImageMappingDAO);
                        }
                }
                await DataContext.ItemImageMapping.BulkMergeAsync(ItemImageMappingDAOs);
            }
            await DataContext.ProductProductGroupingMapping
                .Where(x => x.ProductId == Product.Id)
                .DeleteFromQueryAsync();
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            if (Product.ProductProductGroupingMappings != null)
            {
                foreach (ProductProductGroupingMapping ProductProductGroupingMapping in Product.ProductProductGroupingMappings)
                {
                    ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO()
                    {
                        ProductId = Product.Id,
                        ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId
                    };
                    ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                }
                await DataContext.ProductProductGroupingMapping.BulkMergeAsync(ProductProductGroupingMappingDAOs);
            }
            List<VariationGroupingDAO> VariationGroupingDAOs = await DataContext.VariationGrouping.Include(x => x.Variations)
                .Where(x => x.ProductId == Product.Id).ToListAsync();
            VariationGroupingDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            var VariationIds = VariationGroupingDAOs.SelectMany(x => x.Variations).Select(x => x.Id);
            await DataContext.Variation.Where(x => VariationIds.Contains(x.Id)).DeleteFromQueryAsync();
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
                        VariationGroupingDAO.RowId = Guid.NewGuid();

                        VariationGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                        VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                        VariationGroupingDAO.DeletedAt = null;
                        VariationGroupingDAOs.Add(VariationGroupingDAO);
                        VariationGrouping.RowId = VariationGroupingDAO.RowId;
                    }
                    else
                    {
                        VariationGroupingDAO.Id = VariationGrouping.Id;
                        VariationGroupingDAO.Name = VariationGrouping.Name;
                        VariationGroupingDAO.ProductId = Product.Id;
                        VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                        VariationGroupingDAO.DeletedAt = null;
                        VariationGrouping.RowId = VariationGroupingDAO.RowId;
                    }
                }
                await DataContext.VariationGrouping.BulkMergeAsync(VariationGroupingDAOs);

                foreach (VariationGrouping VariationGrouping in Product.VariationGroupings)
                {
                    VariationGrouping.Id = VariationGroupingDAOs.Where(vg => vg.RowId == VariationGrouping.RowId).Select(vg => vg.Id).FirstOrDefault();

                    foreach (var Variation in VariationGrouping.Variations)
                    {
                        Variation.VariationGroupingId = VariationGrouping.Id;
                    }
                }

                List<Variation> Variations = Product.VariationGroupings.SelectMany(p => p.Variations).ToList();
                List<VariationDAO> VariationDAOs = Variations.Select(v => new VariationDAO
                {
                    Id = v.Id,
                    Code = v.Code,
                    Name = v.Name,
                    VariationGroupingId = v.VariationGroupingId
                }).ToList();

                await DataContext.Variation.BulkMergeAsync(VariationDAOs);
            }
        }

    }
}
