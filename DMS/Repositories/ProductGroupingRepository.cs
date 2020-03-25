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
    public interface IProductGroupingRepository
    {
        Task<int> Count(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter);
        Task<ProductGrouping> Get(long Id);
        Task<bool> Create(ProductGrouping ProductGrouping);
        Task<bool> Update(ProductGrouping ProductGrouping);
        Task<bool> Delete(ProductGrouping ProductGrouping);
        Task<bool> BulkMerge(List<ProductGrouping> ProductGroupings);
        Task<bool> BulkDelete(List<ProductGrouping> ProductGroupings);
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
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.ParentId != null)
                    queryable = queryable.Where(q => q.ParentId, filter.ParentId);
                if (filter.Path != null)
                    queryable = queryable.Where(q => q.Path, filter.Path);
                if (filter.Description != null)
                    queryable = queryable.Where(q => q.Description, filter.Description);
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
                Path = filter.Selects.Contains(ProductGroupingSelect.Path) ? q.Path : default(string),
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
            ProductGrouping ProductGrouping = await DataContext.ProductGrouping.Where(x => x.Id == Id).Select(x => new ProductGrouping()
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
            ProductGrouping.ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping
                .Where(x => x.ProductGrouping.Path.StartsWith(ProductGrouping.Path))
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    ProductGroupingId = x.ProductGroupingId,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        SupplierCode = x.Product.SupplierCode,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ScanCode = x.Product.ScanCode,
                        ProductTypeId = x.Product.ProductTypeId,
                        SupplierId = x.Product.SupplierId,
                        BrandId = x.Product.BrandId,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                        SalePrice = x.Product.SalePrice,
                        RetailPrice = x.Product.RetailPrice,
                        TaxTypeId = x.Product.TaxTypeId,
                        StatusId = x.Product.StatusId,
                    },
                }).ToListAsync();

            return ProductGrouping;
        }
        public async Task<bool> Create(ProductGrouping ProductGrouping)
        {
            ProductGroupingDAO ProductGroupingDAO = new ProductGroupingDAO();
            ProductGroupingDAO.Id = ProductGrouping.Id;
            ProductGroupingDAO.Code = ProductGrouping.Code;
            ProductGroupingDAO.Name = ProductGrouping.Name;
            ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
            ProductGroupingDAO.Path = "";
            ProductGroupingDAO.Description = ProductGrouping.Description;
            ProductGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            ProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ProductGrouping.Add(ProductGroupingDAO);
            await DataContext.SaveChangesAsync();
            ProductGrouping.Id = ProductGroupingDAO.Id;
            await SaveReference(ProductGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(ProductGrouping ProductGrouping)
        {
            ProductGroupingDAO ProductGroupingDAO = DataContext.ProductGrouping.Where(x => x.Id == ProductGrouping.Id).FirstOrDefault();
            if (ProductGroupingDAO == null)
                return false;
            ProductGroupingDAO.Id = ProductGrouping.Id;
            ProductGroupingDAO.Code = ProductGrouping.Code;
            ProductGroupingDAO.Name = ProductGrouping.Name;
            ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
            ProductGroupingDAO.Path = "";
            ProductGroupingDAO.Description = ProductGrouping.Description;
            ProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ProductGrouping);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(ProductGrouping ProductGrouping)
        {
            ProductGroupingDAO ProductGroupingDAO = await DataContext.ProductGrouping.Where(x => x.Id == ProductGrouping.Id).FirstOrDefaultAsync();
            await DataContext.ProductGrouping.Where(x => x.Path.StartsWith(ProductGroupingDAO.Id + ".")).UpdateFromQueryAsync(x => new ProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await DataContext.ProductGrouping.Where(x => x.Id == ProductGrouping.Id).UpdateFromQueryAsync(x => new ProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkMerge(List<ProductGrouping> ProductGroupings)
        {
            List<ProductGroupingDAO> ProductGroupingDAOs = new List<ProductGroupingDAO>();
            foreach (ProductGrouping ProductGrouping in ProductGroupings)
            {
                ProductGroupingDAO ProductGroupingDAO = new ProductGroupingDAO();
                ProductGroupingDAO.Id = ProductGrouping.Id;
                ProductGroupingDAO.Code = ProductGrouping.Code;
                ProductGroupingDAO.Name = ProductGrouping.Name;
                ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
                ProductGroupingDAO.Path = ProductGrouping.Path;
                ProductGroupingDAO.Description = ProductGrouping.Description;
                ProductGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                ProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                ProductGroupingDAOs.Add(ProductGroupingDAO);
            }
            await DataContext.BulkMergeAsync(ProductGroupingDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<ProductGrouping> ProductGroupings)
        {
            List<long> Ids = ProductGroupings.Select(x => x.Id).ToList();
            await DataContext.ProductGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(ProductGrouping ProductGrouping)
        {
            await DataContext.ProductProductGroupingMapping
                .Where(x => x.ProductGroupingId == ProductGrouping.Id)
                .DeleteFromQueryAsync();
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            if (ProductGrouping.ProductProductGroupingMappings != null)
            {
                foreach (ProductProductGroupingMapping ProductProductGroupingMapping in ProductGrouping.ProductProductGroupingMappings)
                {
                    ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO();
                    ProductProductGroupingMappingDAO.ProductId = ProductProductGroupingMapping.ProductId;
                    ProductProductGroupingMappingDAO.ProductGroupingId = ProductGrouping.Id;
                    ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                }
                await DataContext.ProductProductGroupingMapping.BulkMergeAsync(ProductProductGroupingMappingDAOs);
            }
        }

        private async Task BuildPath()
        {
            List<ProductGroupingDAO> ProductGroupingDAOs = await DataContext.ProductGrouping
                .Where(x => x.DeletedAt == null)
                .ToListAsync();
            Queue<ProductGroupingDAO> queue = new Queue<ProductGroupingDAO>();
            ProductGroupingDAOs.ForEach(x =>
            {
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    queue.Enqueue(x);
                }
            });
            while (queue.Count > 0)
            {
                ProductGroupingDAO Parent = queue.Dequeue();
                foreach (ProductGroupingDAO ProductGroupingDAO in ProductGroupingDAOs)
                {
                    if (ProductGroupingDAO.ParentId == Parent.Id)
                    {
                        ProductGroupingDAO.Path = Parent.Path + ProductGroupingDAO.Id + ".";
                        queue.Enqueue(ProductGroupingDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(ProductGroupingDAOs);
        }
    }
}
