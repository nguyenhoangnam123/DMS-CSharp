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
    public interface IBrandRepository
    {
        Task<int> Count(BrandFilter BrandFilter);
        Task<List<Brand>> List(BrandFilter BrandFilter);
        Task<Brand> Get(long Id);
        Task<bool> BulkMerge(List<Brand> Brands);
    }
    public class BrandRepository : IBrandRepository
    {
        private DataContext DataContext;
        public BrandRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<BrandDAO> DynamicFilter(IQueryable<BrandDAO> query, BrandFilter filter)
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
            if (filter.UpdateTime != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdateTime);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<BrandDAO> OrFilter(IQueryable<BrandDAO> query, BrandFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<BrandDAO> initQuery = query.Where(q => false);
            foreach (BrandFilter BrandFilter in filter.OrFilter)
            {
                IQueryable<BrandDAO> queryable = query;
                if (BrandFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, BrandFilter.Id);
                if (BrandFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, BrandFilter.Code);
                if (BrandFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, BrandFilter.Name);
                if (BrandFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, BrandFilter.Description);
                if (BrandFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, BrandFilter.StatusId);
                if (BrandFilter.UpdateTime != null)
                    queryable = queryable.Where(q => q.UpdatedAt, BrandFilter.UpdateTime);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<BrandDAO> DynamicOrder(IQueryable<BrandDAO> query, BrandFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case BrandOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case BrandOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case BrandOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case BrandOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case BrandOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case BrandOrder.UpdateTime:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case BrandOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case BrandOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case BrandOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case BrandOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case BrandOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case BrandOrder.UpdateTime:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Brand>> DynamicSelect(IQueryable<BrandDAO> query, BrandFilter filter)
        {
            List<Brand> Brands = await query.Select(q => new Brand()
            {
                Id = filter.Selects.Contains(BrandSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(BrandSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(BrandSelect.Name) ? q.Name : default(string),
                Description = filter.Selects.Contains(BrandSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(BrandSelect.Status) ? q.StatusId : default(long),
                UpdateTime = filter.Selects.Contains(BrandSelect.UpdateTime) ? q.UpdatedAt : default(DateTime),
                Status = filter.Selects.Contains(BrandSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Used = q.Used,
            }).ToListAsync();
            return Brands;
        }

        public async Task<int> Count(BrandFilter filter)
        {
            IQueryable<BrandDAO> Brands = DataContext.Brand;
            Brands = DynamicFilter(Brands, filter);
            return await Brands.CountAsync();
        }

        public async Task<List<Brand>> List(BrandFilter filter)
        {
            if (filter == null) return new List<Brand>();
            IQueryable<BrandDAO> BrandDAOs = DataContext.Brand.AsNoTracking();
            BrandDAOs = DynamicFilter(BrandDAOs, filter);
            BrandDAOs = DynamicOrder(BrandDAOs, filter);
            List<Brand> Brands = await DynamicSelect(BrandDAOs, filter);
            return Brands;
        }

        public async Task<Brand> Get(long Id)
        {
            Brand Brand = await DataContext.Brand.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Brand()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    StatusId = x.StatusId,
                    Used = x.Used,
                    UpdateTime = x.UpdatedAt,
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).FirstOrDefaultAsync();

            if (Brand == null)
                return null;

            return Brand;
        }

        public async Task<bool> BulkMerge(List<Brand> Brands)
        {
            List<BrandDAO> BrandDAOs = Brands.Select(x => new BrandDAO
            {
                Code = x.Code,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Used = x.Used,
                Description = x.Description,
                Id = x.Id,
                Name = x.Name,
                RowId = x.RowId,
                StatusId = x.StatusId,
            }).ToList();
            await DataContext.BulkMergeAsync(BrandDAOs);
            return true;
        }
    }
}
