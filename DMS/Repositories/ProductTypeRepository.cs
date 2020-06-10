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
    public interface IProductTypeRepository
    {
        Task<int> Count(ProductTypeFilter ProductTypeFilter);
        Task<List<ProductType>> List(ProductTypeFilter ProductTypeFilter);
        Task<ProductType> Get(long Id);
        Task<bool> Create(ProductType ProductType);
        Task<bool> Update(ProductType ProductType);
        Task<bool> Delete(ProductType ProductType);
        Task<bool> BulkMerge(List<ProductType> ProductTypes);
        Task<bool> BulkDelete(List<ProductType> ProductTypes);
    }
    public class ProductTypeRepository : IProductTypeRepository
    {
        private DataContext DataContext;
        public ProductTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProductTypeDAO> DynamicFilter(IQueryable<ProductTypeDAO> query, ProductTypeFilter filter)
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
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.UpdatedTime != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedTime);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ProductTypeDAO> OrFilter(IQueryable<ProductTypeDAO> query, ProductTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProductTypeDAO> initQuery = query.Where(q => false);
            foreach (ProductTypeFilter ProductTypeFilter in filter.OrFilter)
            {
                IQueryable<ProductTypeDAO> queryable = query;
                if (ProductTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ProductTypeFilter.Id);
                if (ProductTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ProductTypeFilter.Code);
                if (ProductTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ProductTypeFilter.Name);
                if (ProductTypeFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, ProductTypeFilter.Description);
                if (ProductTypeFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, ProductTypeFilter.StatusId);
                if (ProductTypeFilter.UpdatedTime != null)
                    queryable = queryable.Where(q => q.UpdatedAt, ProductTypeFilter.UpdatedTime);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ProductTypeDAO> DynamicOrder(IQueryable<ProductTypeDAO> query, ProductTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProductTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProductTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ProductTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ProductTypeOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case ProductTypeOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ProductTypeOrder.UpdatedTime:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProductTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProductTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ProductTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ProductTypeOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case ProductTypeOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ProductTypeOrder.UpdatedTime:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ProductType>> DynamicSelect(IQueryable<ProductTypeDAO> query, ProductTypeFilter filter)
        {
            List<ProductType> ProductTypes = await query.Select(q => new ProductType()
            {
                Id = filter.Selects.Contains(ProductTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ProductTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ProductTypeSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(ProductTypeSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(ProductTypeSelect.Status) ? q.StatusId : default(long),
                UpdatedTime = filter.Selects.Contains(ProductTypeSelect.UpdatedTime) ? q.UpdatedAt : default(DateTime),
                Status = filter.Selects.Contains(ProductTypeSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Used = q.Used,
            }).ToListAsync();
            return ProductTypes;
        }

        public async Task<int> Count(ProductTypeFilter filter)
        {
            IQueryable<ProductTypeDAO> ProductTypes = DataContext.ProductType;
            ProductTypes = DynamicFilter(ProductTypes, filter);
            return await ProductTypes.CountAsync();
        }

        public async Task<List<ProductType>> List(ProductTypeFilter filter)
        {
            if (filter == null) return new List<ProductType>();
            IQueryable<ProductTypeDAO> ProductTypeDAOs = DataContext.ProductType.AsNoTracking();
            ProductTypeDAOs = DynamicFilter(ProductTypeDAOs, filter);
            ProductTypeDAOs = DynamicOrder(ProductTypeDAOs, filter);
            List<ProductType> ProductTypes = await DynamicSelect(ProductTypeDAOs, filter);
            return ProductTypes;
        }

        public async Task<ProductType> Get(long Id)
        {
            ProductType ProductType = await DataContext.ProductType.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new ProductType()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    StatusId = x.StatusId,
                    UpdatedTime = x.UpdatedAt,
                    Used = x.Used,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).FirstOrDefaultAsync();

            if (ProductType == null)
                return null;

            return ProductType;
        }
        public async Task<bool> Create(ProductType ProductType)
        {
            ProductTypeDAO ProductTypeDAO = new ProductTypeDAO();
            ProductTypeDAO.Id = ProductType.Id;
            ProductTypeDAO.Code = ProductType.Code;
            ProductTypeDAO.Name = ProductType.Name;
            ProductTypeDAO.Description = ProductType.Description;
            ProductTypeDAO.StatusId = ProductType.StatusId;
            ProductTypeDAO.CreatedAt = StaticParams.DateTimeNow;
            ProductTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            ProductTypeDAO.Used = false;
            DataContext.ProductType.Add(ProductTypeDAO);
            await DataContext.SaveChangesAsync();
            ProductType.Id = ProductTypeDAO.Id;
            return true;
        }

        public async Task<bool> Update(ProductType ProductType)
        {
            ProductTypeDAO ProductTypeDAO = DataContext.ProductType.Where(x => x.Id == ProductType.Id).FirstOrDefault();
            if (ProductTypeDAO == null)
                return false;
            ProductTypeDAO.Id = ProductType.Id;
            ProductTypeDAO.Code = ProductType.Code;
            ProductTypeDAO.Name = ProductType.Name;
            ProductTypeDAO.Description = ProductType.Description;
            ProductTypeDAO.StatusId = ProductType.StatusId;
            ProductTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ProductType);
            return true;
        }

        public async Task<bool> Delete(ProductType ProductType)
        {
            await DataContext.ProductType.Where(x => x.Id == ProductType.Id).UpdateFromQueryAsync(x => new ProductTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<ProductType> ProductTypes)
        {
            List<ProductTypeDAO> ProductTypeDAOs = new List<ProductTypeDAO>();
            foreach (ProductType ProductType in ProductTypes)
            {
                ProductTypeDAO ProductTypeDAO = new ProductTypeDAO();
                ProductTypeDAO.Id = ProductType.Id;
                ProductTypeDAO.Code = ProductType.Code;
                ProductTypeDAO.Name = ProductType.Name;
                ProductTypeDAO.Description = ProductType.Description;
                ProductTypeDAO.StatusId = ProductType.StatusId;
                ProductTypeDAO.CreatedAt = StaticParams.DateTimeNow;
                ProductTypeDAO.UpdatedAt = StaticParams.DateTimeNow;
                ProductTypeDAOs.Add(ProductTypeDAO);
            }
            await DataContext.BulkMergeAsync(ProductTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ProductType> ProductTypes)
        {
            List<long> Ids = ProductTypes.Select(x => x.Id).ToList();
            await DataContext.ProductType
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ProductTypeDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ProductType ProductType)
        {
        }

    }
}
