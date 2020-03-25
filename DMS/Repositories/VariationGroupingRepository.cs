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
    public interface IVariationGroupingRepository
    {
        Task<int> Count(VariationGroupingFilter VariationGroupingFilter);
        Task<List<VariationGrouping>> List(VariationGroupingFilter VariationGroupingFilter);
        Task<VariationGrouping> Get(long Id);
        Task<bool> Create(VariationGrouping VariationGrouping);
        Task<bool> Update(VariationGrouping VariationGrouping);
        Task<bool> Delete(VariationGrouping VariationGrouping);
        Task<bool> BulkMerge(List<VariationGrouping> VariationGroupings);
        Task<bool> BulkDelete(List<VariationGrouping> VariationGroupings);
    }
    public class VariationGroupingRepository : IVariationGroupingRepository
    {
        private DataContext DataContext;
        public VariationGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<VariationGroupingDAO> DynamicFilter(IQueryable<VariationGroupingDAO> query, VariationGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ProductId != null)
                query = query.Where(q => q.ProductId, filter.ProductId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<VariationGroupingDAO> OrFilter(IQueryable<VariationGroupingDAO> query, VariationGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<VariationGroupingDAO> initQuery = query.Where(q => false);
            foreach (VariationGroupingFilter VariationGroupingFilter in filter.OrFilter)
            {
                IQueryable<VariationGroupingDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.ProductId != null)
                    queryable = queryable.Where(q => q.ProductId, filter.ProductId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<VariationGroupingDAO> DynamicOrder(IQueryable<VariationGroupingDAO> query, VariationGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case VariationGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case VariationGroupingOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case VariationGroupingOrder.Product:
                            query = query.OrderBy(q => q.ProductId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case VariationGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case VariationGroupingOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case VariationGroupingOrder.Product:
                            query = query.OrderByDescending(q => q.ProductId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<VariationGrouping>> DynamicSelect(IQueryable<VariationGroupingDAO> query, VariationGroupingFilter filter)
        {
            List<VariationGrouping> VariationGroupings = await query.Select(q => new VariationGrouping()
            {
                Id = filter.Selects.Contains(VariationGroupingSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(VariationGroupingSelect.Name) ? q.Name : default(string),
                ProductId = filter.Selects.Contains(VariationGroupingSelect.Product) ? q.ProductId : default(long),
                Product = filter.Selects.Contains(VariationGroupingSelect.Product) && q.Product != null ? new Product
                {
                    Id = q.Product.Id,
                    Code = q.Product.Code,
                    SupplierCode = q.Product.SupplierCode,
                    Name = q.Product.Name,
                    Description = q.Product.Description,
                    ScanCode = q.Product.ScanCode,
                    ProductTypeId = q.Product.ProductTypeId,
                    SupplierId = q.Product.SupplierId,
                    BrandId = q.Product.BrandId,
                    UnitOfMeasureId = q.Product.UnitOfMeasureId,
                    UnitOfMeasureGroupingId = q.Product.UnitOfMeasureGroupingId,
                    SalePrice = q.Product.SalePrice,
                    RetailPrice = q.Product.RetailPrice,
                    TaxTypeId = q.Product.TaxTypeId,
                    StatusId = q.Product.StatusId,
                } : null,
            }).ToListAsync();
            return VariationGroupings;
        }

        public async Task<int> Count(VariationGroupingFilter filter)
        {
            IQueryable<VariationGroupingDAO> VariationGroupings = DataContext.VariationGrouping;
            VariationGroupings = DynamicFilter(VariationGroupings, filter);
            return await VariationGroupings.CountAsync();
        }

        public async Task<List<VariationGrouping>> List(VariationGroupingFilter filter)
        {
            if (filter == null) return new List<VariationGrouping>();
            IQueryable<VariationGroupingDAO> VariationGroupingDAOs = DataContext.VariationGrouping;
            VariationGroupingDAOs = DynamicFilter(VariationGroupingDAOs, filter);
            VariationGroupingDAOs = DynamicOrder(VariationGroupingDAOs, filter);
            List<VariationGrouping> VariationGroupings = await DynamicSelect(VariationGroupingDAOs, filter);
            return VariationGroupings;
        }

        public async Task<VariationGrouping> Get(long Id)
        {
            VariationGrouping VariationGrouping = await DataContext.VariationGrouping.Where(x => x.Id == Id).Select(x => new VariationGrouping()
            {
                Id = x.Id,
                Name = x.Name,
                ProductId = x.ProductId,
                Product = x.Product == null ? null : new Product
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
            }).FirstOrDefaultAsync();

            if (VariationGrouping == null)
                return null;

            return VariationGrouping;
        }
        public async Task<bool> Create(VariationGrouping VariationGrouping)
        {
            VariationGroupingDAO VariationGroupingDAO = new VariationGroupingDAO();
            VariationGroupingDAO.Id = VariationGrouping.Id;
            VariationGroupingDAO.Name = VariationGrouping.Name;
            VariationGroupingDAO.ProductId = VariationGrouping.ProductId;
            VariationGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.VariationGrouping.Add(VariationGroupingDAO);
            await DataContext.SaveChangesAsync();
            VariationGrouping.Id = VariationGroupingDAO.Id;
            await SaveReference(VariationGrouping);
            return true;
        }

        public async Task<bool> Update(VariationGrouping VariationGrouping)
        {
            VariationGroupingDAO VariationGroupingDAO = DataContext.VariationGrouping.Where(x => x.Id == VariationGrouping.Id).FirstOrDefault();
            if (VariationGroupingDAO == null)
                return false;
            VariationGroupingDAO.Id = VariationGrouping.Id;
            VariationGroupingDAO.Name = VariationGrouping.Name;
            VariationGroupingDAO.ProductId = VariationGrouping.ProductId;
            VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(VariationGrouping);
            return true;
        }

        public async Task<bool> Delete(VariationGrouping VariationGrouping)
        {
            await DataContext.VariationGrouping.Where(x => x.Id == VariationGrouping.Id).UpdateFromQueryAsync(x => new VariationGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<VariationGrouping> VariationGroupings)
        {
            List<VariationGroupingDAO> VariationGroupingDAOs = new List<VariationGroupingDAO>();
            foreach (VariationGrouping VariationGrouping in VariationGroupings)
            {
                VariationGroupingDAO VariationGroupingDAO = new VariationGroupingDAO();
                VariationGroupingDAO.Id = VariationGrouping.Id;
                VariationGroupingDAO.Name = VariationGrouping.Name;
                VariationGroupingDAO.ProductId = VariationGrouping.ProductId;
                VariationGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                VariationGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                VariationGroupingDAOs.Add(VariationGroupingDAO);
            }
            await DataContext.BulkMergeAsync(VariationGroupingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<VariationGrouping> VariationGroupings)
        {
            List<long> Ids = VariationGroupings.Select(x => x.Id).ToList();
            await DataContext.VariationGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new VariationGroupingDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(VariationGrouping VariationGrouping)
        {
        }

    }
}
