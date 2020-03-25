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
    public interface ISupplierRepository
    {
        Task<int> Count(SupplierFilter SupplierFilter);
        Task<List<Supplier>> List(SupplierFilter SupplierFilter);
        Task<Supplier> Get(long Id);
        Task<bool> Create(Supplier Supplier);
        Task<bool> Update(Supplier Supplier);
        Task<bool> Delete(Supplier Supplier);
        Task<bool> BulkMerge(List<Supplier> Suppliers);
        Task<bool> BulkDelete(List<Supplier> Suppliers);
    }
    public class SupplierRepository : ISupplierRepository
    {
        private DataContext DataContext;
        public SupplierRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SupplierDAO> DynamicFilter(IQueryable<SupplierDAO> query, SupplierFilter filter)
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
            if (filter.TaxCode != null)
                query = query.Where(q => q.TaxCode, filter.TaxCode);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<SupplierDAO> OrFilter(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SupplierDAO> initQuery = query.Where(q => false);
            foreach (SupplierFilter SupplierFilter in filter.OrFilter)
            {
                IQueryable<SupplierDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.TaxCode != null)
                    queryable = queryable.Where(q => q.TaxCode, filter.TaxCode);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<SupplierDAO> DynamicOrder(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SupplierOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SupplierOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case SupplierOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SupplierOrder.TaxCode:
                            query = query.OrderBy(q => q.TaxCode);
                            break;
                        case SupplierOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SupplierOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SupplierOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case SupplierOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SupplierOrder.TaxCode:
                            query = query.OrderByDescending(q => q.TaxCode);
                            break;
                        case SupplierOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Supplier>> DynamicSelect(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            List<Supplier> Suppliers = await query.Select(q => new Supplier()
            {
                Id = filter.Selects.Contains(SupplierSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(SupplierSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(SupplierSelect.Name) ? q.Name : default(string),
                TaxCode = filter.Selects.Contains(SupplierSelect.TaxCode) ? q.TaxCode : default(string),
                StatusId = filter.Selects.Contains(SupplierSelect.Status) ? q.StatusId : default(long),
                Status = filter.Selects.Contains(SupplierSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).ToListAsync();
            return Suppliers;
        }

        public async Task<int> Count(SupplierFilter filter)
        {
            IQueryable<SupplierDAO> Suppliers = DataContext.Supplier;
            Suppliers = DynamicFilter(Suppliers, filter);
            return await Suppliers.CountAsync();
        }

        public async Task<List<Supplier>> List(SupplierFilter filter)
        {
            if (filter == null) return new List<Supplier>();
            IQueryable<SupplierDAO> SupplierDAOs = DataContext.Supplier;
            SupplierDAOs = DynamicFilter(SupplierDAOs, filter);
            SupplierDAOs = DynamicOrder(SupplierDAOs, filter);
            List<Supplier> Suppliers = await DynamicSelect(SupplierDAOs, filter);
            return Suppliers;
        }

        public async Task<Supplier> Get(long Id)
        {
            Supplier Supplier = await DataContext.Supplier.Where(x => x.Id == Id).Select(x => new Supplier()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                TaxCode = x.TaxCode,
                StatusId = x.StatusId,
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Supplier == null)
                return null;

            return Supplier;
        }
        public async Task<bool> Create(Supplier Supplier)
        {
            SupplierDAO SupplierDAO = new SupplierDAO();
            SupplierDAO.Id = Supplier.Id;
            SupplierDAO.Code = Supplier.Code;
            SupplierDAO.Name = Supplier.Name;
            SupplierDAO.TaxCode = Supplier.TaxCode;
            SupplierDAO.StatusId = Supplier.StatusId;
            SupplierDAO.CreatedAt = StaticParams.DateTimeNow;
            SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Supplier.Add(SupplierDAO);
            await DataContext.SaveChangesAsync();
            Supplier.Id = SupplierDAO.Id;
            await SaveReference(Supplier);
            return true;
        }

        public async Task<bool> Update(Supplier Supplier)
        {
            SupplierDAO SupplierDAO = DataContext.Supplier.Where(x => x.Id == Supplier.Id).FirstOrDefault();
            if (SupplierDAO == null)
                return false;
            SupplierDAO.Id = Supplier.Id;
            SupplierDAO.Code = Supplier.Code;
            SupplierDAO.Name = Supplier.Name;
            SupplierDAO.TaxCode = Supplier.TaxCode;
            SupplierDAO.StatusId = Supplier.StatusId;
            SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Supplier);
            return true;
        }

        public async Task<bool> Delete(Supplier Supplier)
        {
            await DataContext.Supplier.Where(x => x.Id == Supplier.Id).UpdateFromQueryAsync(x => new SupplierDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Supplier> Suppliers)
        {
            List<SupplierDAO> SupplierDAOs = new List<SupplierDAO>();
            foreach (Supplier Supplier in Suppliers)
            {
                SupplierDAO SupplierDAO = new SupplierDAO();
                SupplierDAO.Id = Supplier.Id;
                SupplierDAO.Code = Supplier.Code;
                SupplierDAO.Name = Supplier.Name;
                SupplierDAO.TaxCode = Supplier.TaxCode;
                SupplierDAO.StatusId = Supplier.StatusId;
                SupplierDAO.CreatedAt = StaticParams.DateTimeNow;
                SupplierDAO.UpdatedAt = StaticParams.DateTimeNow;
                SupplierDAOs.Add(SupplierDAO);
            }
            await DataContext.BulkMergeAsync(SupplierDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Supplier> Suppliers)
        {
            List<long> Ids = Suppliers.Select(x => x.Id).ToList();
            await DataContext.Supplier
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new SupplierDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Supplier Supplier)
        {
        }

    }
}
