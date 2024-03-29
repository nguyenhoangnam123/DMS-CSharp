using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IProductGroupingRepository
    {
        Task<int> Count(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter);
        Task<ProductGrouping> Get(long Id);
        Task<bool> BulkMerge(List<ProductGrouping> ProductGroupings);
    }
    public class ProductGroupingRepository : IProductGroupingRepository
    {
        private DataContext DataContext;
        public ProductGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProductGroupingDAO> DynamicFilter(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ParentId != null)
                query = query.Where(q => q.ParentId, filter.ParentId);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ProductGroupingDAO> OrFilter(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProductGroupingDAO> initQuery = query.Where(q => false);
            foreach (ProductGroupingFilter ProductGroupingFilter in filter.OrFilter)
            {
                IQueryable<ProductGroupingDAO> queryable = query;
                if (ProductGroupingFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ProductGroupingFilter.Id);
                if (ProductGroupingFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ProductGroupingFilter.Code);
                if (ProductGroupingFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ProductGroupingFilter.Name);
                if (ProductGroupingFilter.ParentId != null)
                    queryable = queryable.Where(q => q.ParentId, ProductGroupingFilter.ParentId);
                if (ProductGroupingFilter.Path != null)
                    queryable = queryable.Where(q => q.Path, ProductGroupingFilter.Path);
                if (ProductGroupingFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, ProductGroupingFilter.Description);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProductGroupingDAO> DynamicOrder(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProductGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProductGroupingOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProductGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProductGroupingOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case ProductGroupingOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case ProductGroupingOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProductGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProductGroupingOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProductGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProductGroupingOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case ProductGroupingOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case ProductGroupingOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ProductGrouping>> DynamicSelect(IQueryable<ProductGroupingDAO> query, ProductGroupingFilter filter)
        {
            List<ProductGrouping> ProductGroupings = await query.Select(q => new ProductGrouping()
            {
                Id = filter.Selects.Contains(ProductGroupingSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProductGroupingSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProductGroupingSelect.Name) ? q.Name : default(string),
                ParentId = filter.Selects.Contains(ProductGroupingSelect.Parent) ? q.ParentId : default(long?),
                Path = q.Path,
                Description = filter.Selects.Contains(ProductGroupingSelect.Description) ? q.Description : default(string),
                Parent = filter.Selects.Contains(ProductGroupingSelect.Parent) && q.Parent != null ? new ProductGrouping
                {
                    Id = q.Parent.Id,
                    Code = q.Parent.Code,
                    Name = q.Parent.Name,
                    ParentId = q.Parent.ParentId,
                    Path = q.Parent.Path,
                    Description = q.Parent.Description,
                } : null,
            }).ToListAsync();

            var ProductGroupingDAOs = await DataContext.ProductGrouping.Where(x => x.DeletedAt == null).ToListAsync();
            foreach (var ProductGrouping in ProductGroupings)
            {
                var count = ProductGroupingDAOs.Where(x => x.Path.StartsWith(ProductGrouping.Path) && x.Id != ProductGrouping.Id).Count();
                if (count > 0)
                    ProductGrouping.HasChildren = true;
            }
            return ProductGroupings;
        }

        public async Task<int> Count(ProductGroupingFilter filter)
        {
            IQueryable<ProductGroupingDAO> ProductGroupings = DataContext.ProductGrouping;
            ProductGroupings = DynamicFilter(ProductGroupings, filter);
            return await ProductGroupings.CountAsync();
        }

        public async Task<List<ProductGrouping>> List(ProductGroupingFilter filter)
        {
            if (filter == null) return new List<ProductGrouping>();
            IQueryable<ProductGroupingDAO> ProductGroupingDAOs = DataContext.ProductGrouping;
            ProductGroupingDAOs = DynamicFilter(ProductGroupingDAOs, filter);
            ProductGroupingDAOs = DynamicOrder(ProductGroupingDAOs, filter);
            List<ProductGrouping> ProductGroupings = await DynamicSelect(ProductGroupingDAOs, filter);
            return ProductGroupings;
        }

        public async Task<ProductGrouping> Get(long Id)
        {
            ProductGrouping ProductGrouping = await DataContext.ProductGrouping.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new ProductGrouping()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Path = x.Path,
                    Description = x.Description,
                    Parent = x.Parent == null ? null : new ProductGrouping
                    {
                        Id = x.Parent.Id,
                        Code = x.Parent.Code,
                        Name = x.Parent.Name,
                        ParentId = x.Parent.ParentId,
                        Path = x.Parent.Path,
                        Description = x.Parent.Description,
                    },
                }).FirstOrDefaultAsync();

            if (ProductGrouping == null)
                return null;
            ProductGrouping.ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping.Include(x => x.Product)
                .Where(x => x.ProductGrouping.Id == Id && x.Product.DeletedAt == null)
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    ProductGroupingId = x.ProductGroupingId,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ScanCode = x.Product.ScanCode,
                        ProductTypeId = x.Product.ProductTypeId,
                        BrandId = x.Product.BrandId,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                        SalePrice = x.Product.SalePrice,
                        RetailPrice = x.Product.RetailPrice,
                        TaxTypeId = x.Product.TaxTypeId,
                        StatusId = x.Product.StatusId,
                        IsNew = x.Product.IsNew,
                        Brand = x.Product.Brand == null ? null : new Brand
                        {
                            Id = x.Product.Brand.Id,
                            Code = x.Product.Brand.Code,
                            Name = x.Product.Brand.Name,
                            Description = x.Product.Brand.Description,
                            StatusId = x.Product.Brand.StatusId,
                        },
                        ProductType = x.Product.ProductType == null ? null : new ProductType
                        {
                            Id = x.Product.ProductType.Id,
                            Code = x.Product.ProductType.Code,
                            Name = x.Product.ProductType.Name,
                            Description = x.Product.ProductType.Description,
                            StatusId = x.Product.ProductType.StatusId,
                        },
                        Status = x.Product.Status == null ? null : new Status
                        {
                            Id = x.Product.Status.Id,
                            Code = x.Product.Status.Code,
                            Name = x.Product.Status.Name,
                        },
                        TaxType = x.Product.TaxType == null ? null : new TaxType
                        {
                            Id = x.Product.TaxType.Id,
                            Code = x.Product.TaxType.Code,
                            Name = x.Product.TaxType.Name,
                            Percentage = x.Product.TaxType.Percentage,
                            StatusId = x.Product.TaxType.StatusId,
                        },
                        UnitOfMeasure = x.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                        {
                            Id = x.Product.UnitOfMeasure.Id,
                            Code = x.Product.UnitOfMeasure.Code,
                            Name = x.Product.UnitOfMeasure.Name,
                            Description = x.Product.UnitOfMeasure.Description,
                            StatusId = x.Product.UnitOfMeasure.StatusId,
                        },
                        UnitOfMeasureGrouping = x.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                        {
                            Id = x.Product.UnitOfMeasureGrouping.Id,
                            Name = x.Product.UnitOfMeasureGrouping.Name,
                            UnitOfMeasureId = x.Product.UnitOfMeasureGrouping.UnitOfMeasureId,
                            StatusId = x.Product.UnitOfMeasureGrouping.StatusId
                        },
                    },
                }).ToListAsync();

            return ProductGrouping;
        }
        public async Task<bool> BulkMerge(List<ProductGrouping> ProductGroupings)
        {
            List<ProductGroupingDAO> ProductGroupingDAOs = new List<ProductGroupingDAO>();
            foreach (ProductGrouping ProductGrouping in ProductGroupings)
            {
                ProductGroupingDAO ProductGroupingDAO = new ProductGroupingDAO();
                ProductGroupingDAO.Id = ProductGrouping.Id;
                ProductGroupingDAO.Code = ProductGrouping.Code;
                ProductGroupingDAO.CreatedAt = ProductGrouping.CreatedAt;
                ProductGroupingDAO.UpdatedAt = ProductGrouping.UpdatedAt;
                ProductGroupingDAO.DeletedAt = ProductGrouping.DeletedAt;
                ProductGroupingDAO.Id = ProductGrouping.Id;
                ProductGroupingDAO.Name = ProductGrouping.Name;
                ProductGroupingDAO.RowId = ProductGrouping.RowId;
                ProductGroupingDAO.Description = ProductGrouping.Description;
                ProductGroupingDAO.Level = ProductGrouping.Level;
                ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
                ProductGroupingDAO.Path = ProductGrouping.Path;
                ProductGroupingDAOs.Add(ProductGroupingDAO);
            }
            await DataContext.BulkMergeAsync(ProductGroupingDAOs);
            return true;
        }
    }
}
