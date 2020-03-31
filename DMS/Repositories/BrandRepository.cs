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
    public interface IBrandRepository
    {
        Task<int> Count(BrandFilter BrandFilter);
        Task<List<Brand>> List(BrandFilter BrandFilter);
        Task<Brand> Get(long Id);
        Task<bool> Create(Brand Brand);
        Task<bool> Update(Brand Brand);
        Task<bool> Delete(Brand Brand);
        Task<bool> BulkMerge(List<Brand> Brands);
        Task<bool> BulkDelete(List<Brand> Brands);
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
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
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
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
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
                        case BrandOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
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
                        case BrandOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
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
                StatusId = filter.Selects.Contains(BrandSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(BrandSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
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
            IQueryable<BrandDAO> BrandDAOs = DataContext.Brand;
            BrandDAOs = DynamicFilter(BrandDAOs, filter);
            BrandDAOs = DynamicOrder(BrandDAOs, filter);
            List<Brand> Brands = await DynamicSelect(BrandDAOs, filter);
            return Brands;
        }

        public async Task<Brand> Get(long Id)
        {
            Brand Brand = await DataContext.Brand.Where(x => x.Id == Id).AsNoTracking().Select(x => new Brand()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
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
        public async Task<bool> Create(Brand Brand)
        {
            BrandDAO BrandDAO = new BrandDAO();
            BrandDAO.Id = Brand.Id;
            BrandDAO.Code = Brand.Code;
            BrandDAO.Name = Brand.Name;
            BrandDAO.StatusId = Brand.StatusId;
            BrandDAO.CreatedAt = StaticParams.DateTimeNow;
            BrandDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Brand.Add(BrandDAO);
            await DataContext.SaveChangesAsync();
            Brand.Id = BrandDAO.Id;
            await SaveReference(Brand);
            return true;
        }

        public async Task<bool> Update(Brand Brand)
        {
            BrandDAO BrandDAO = DataContext.Brand.Where(x => x.Id == Brand.Id).AsNoTracking().FirstOrDefault();
            if (BrandDAO == null)
                return false;
            BrandDAO.Id = Brand.Id;
            BrandDAO.Code = Brand.Code;
            BrandDAO.Name = Brand.Name;
            BrandDAO.StatusId = Brand.StatusId;
            BrandDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Brand);
            return true;
        }

        public async Task<bool> Delete(Brand Brand)
        {
            await DataContext.Brand.Where(x => x.Id == Brand.Id).AsNoTracking().UpdateFromQueryAsync(x => new BrandDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Brand> Brands)
        {
            List<BrandDAO> BrandDAOs = new List<BrandDAO>();
            foreach (Brand Brand in Brands)
            {
                BrandDAO BrandDAO = new BrandDAO();
                BrandDAO.Id = Brand.Id;
                BrandDAO.Code = Brand.Code;
                BrandDAO.Name = Brand.Name;
                BrandDAO.StatusId = Brand.StatusId;
                BrandDAO.CreatedAt = StaticParams.DateTimeNow;
                BrandDAO.UpdatedAt = StaticParams.DateTimeNow;
                BrandDAOs.Add(BrandDAO);
            }
            await DataContext.BulkMergeAsync(BrandDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Brand> Brands)
        {
            List<long> Ids = Brands.Select(x => x.Id).ToList();
            await DataContext.Brand
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new BrandDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Brand Brand)
        {
        }

    }
}
