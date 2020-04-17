using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace DMS.Repositories
{
    public interface IWarehouseRepository
    {
        Task<int> Count(WarehouseFilter WarehouseFilter);
        Task<List<Warehouse>> List(WarehouseFilter WarehouseFilter);
        Task<Warehouse> Get(long Id);
        Task<bool> Create(Warehouse Warehouse);
        Task<bool> Update(Warehouse Warehouse);
        Task<bool> Delete(Warehouse Warehouse);
        Task<bool> BulkMerge(List<Warehouse> Warehouses);
        Task<bool> BulkDelete(List<Warehouse> Warehouses);
    }
    public class WarehouseRepository : IWarehouseRepository
    {
        private DataContext DataContext;
        public WarehouseRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WarehouseDAO> DynamicFilter(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.DeletedAt == null);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.DistrictId != null)
                query = query.Where(q => q.DistrictId, filter.DistrictId);
            if (filter.WardId != null)
                query = query.Where(q => q.WardId, filter.WardId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WarehouseDAO> OrFilter(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WarehouseDAO> initQuery = query.Where(q => false);
            foreach (WarehouseFilter WarehouseFilter in filter.OrFilter)
            {
                IQueryable<WarehouseDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (filter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, filter.ProvinceId);
                if (filter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, filter.DistrictId);
                if (filter.WardId != null)
                    queryable = queryable.Where(q => q.WardId, filter.WardId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WarehouseDAO> DynamicOrder(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WarehouseOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WarehouseOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WarehouseOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WarehouseOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case WarehouseOrder.Organization:
                            query = query.OrderBy(q => q.Organization.Name);
                            break;
                        case WarehouseOrder.Province:
                            query = query.OrderBy(q => q.Province.Name);
                            break;
                        case WarehouseOrder.District:
                            query = query.OrderBy(q => q.District.Name);
                            break;
                        case WarehouseOrder.Ward:
                            query = query.OrderBy(q => q.Ward.Name);
                            break;
                        case WarehouseOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WarehouseOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WarehouseOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WarehouseOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WarehouseOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case WarehouseOrder.Organization:
                            query = query.OrderByDescending(q => q.Organization.Name);
                            break;
                        case WarehouseOrder.Province:
                            query = query.OrderByDescending(q => q.Province.Name);
                            break;
                        case WarehouseOrder.District:
                            query = query.OrderByDescending(q => q.District.Name);
                            break;
                        case WarehouseOrder.Ward:
                            query = query.OrderByDescending(q => q.Ward.Name);
                            break;
                        case WarehouseOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Warehouse>> DynamicSelect(IQueryable<WarehouseDAO> query, WarehouseFilter filter)
        {
            List<Warehouse> Warehouses = await query.Select(q => new Warehouse()
            {
                Id = filter.Selects.Contains(WarehouseSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(WarehouseSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WarehouseSelect.Name) ? q.Name : default(string),
                Address = filter.Selects.Contains(WarehouseSelect.Address) ? q.Address : default(string),
                OrganizationId = filter.Selects.Contains(WarehouseSelect.Organization) ? q.OrganizationId : default(long),
                ProvinceId = filter.Selects.Contains(WarehouseSelect.Province) ? q.ProvinceId : default(long?),
                DistrictId = filter.Selects.Contains(WarehouseSelect.District) ? q.DistrictId : default(long?),
                WardId = filter.Selects.Contains(WarehouseSelect.Ward) ? q.WardId : default(long?),
                StatusId = filter.Selects.Contains(WarehouseSelect.Status) ? q.StatusId : default(long),
                Organization = filter.Selects.Contains(WarehouseSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                } : null,
                District = filter.Selects.Contains(WarehouseSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Code = q.District.Code,
                    Name = q.District.Name,
                } : null,
                Province = filter.Selects.Contains(WarehouseSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                } : null,
                Status = filter.Selects.Contains(WarehouseSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Ward = filter.Selects.Contains(WarehouseSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Code = q.Ward.Code,
                    Name = q.Ward.Name,
                } : null,
            }).AsNoTracking().ToListAsync();
            return Warehouses;
        }

        public async Task<int> Count(WarehouseFilter filter)
        {
            IQueryable<WarehouseDAO> Warehouses = DataContext.Warehouse;
            Warehouses = DynamicFilter(Warehouses, filter);
            return await Warehouses.CountAsync();
        }

        public async Task<List<Warehouse>> List(WarehouseFilter filter)
        {
            if (filter == null) return new List<Warehouse>();
            IQueryable<WarehouseDAO> WarehouseDAOs = DataContext.Warehouse;
            WarehouseDAOs = DynamicFilter(WarehouseDAOs, filter);
            WarehouseDAOs = DynamicOrder(WarehouseDAOs, filter);
            List<Warehouse> Warehouses = await DynamicSelect(WarehouseDAOs, filter);
            return Warehouses;
        }

        public async Task<Warehouse> Get(long Id)
        {
            Warehouse Warehouse = await DataContext.Warehouse
                .Where(x => x.Id == Id).AsNoTracking()
                .Select(x => new Warehouse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Address = x.Address,
                    OrganizationId = x.OrganizationId,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                    StatusId = x.StatusId,
                    Organization = x.Organization == null ? null : new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                    },
                    District = x.District == null ? null : new District
                    {
                        Id = x.District.Id,
                        Code = x.District.Code,
                        Name = x.District.Name,
                    },
                    Province = x.Province == null ? null : new Province
                    {
                        Id = x.Province.Id,
                        Code = x.Province.Code,
                        Name = x.Province.Name,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Ward = x.Ward == null ? null : new Ward
                    {
                        Id = x.Ward.Id,
                        Code = x.Ward.Code,
                        Name = x.Ward.Name,
                    },
                }).AsNoTracking().FirstOrDefaultAsync();

            if (Warehouse == null)
                return null;
            Warehouse.Inventories = await DataContext.Inventory
                .Where(x => x.WarehouseId == Warehouse.Id)
                .Where(x => x.DeletedAt == null && x.Item.DeletedAt == null)
                .Select(x => new Inventory
                {
                    Id = x.Id,
                    WarehouseId = x.WarehouseId,
                    ItemId = x.ItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        Product = new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            Name = x.Item.Product.Name,
                            ERPCode = x.Item.Product.ERPCode,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasure = new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                            }
                        }
                    },
                }).ToListAsync();

            return Warehouse;
        }
        public async Task<bool> Create(Warehouse Warehouse)
        {
            WarehouseDAO WarehouseDAO = new WarehouseDAO();
            WarehouseDAO.Id = Warehouse.Id;
            WarehouseDAO.Code = Warehouse.Code;
            WarehouseDAO.Name = Warehouse.Name;
            WarehouseDAO.Address = Warehouse.Address;
            WarehouseDAO.OrganizationId = Warehouse.OrganizationId;
            WarehouseDAO.ProvinceId = Warehouse.ProvinceId;
            WarehouseDAO.DistrictId = Warehouse.DistrictId;
            WarehouseDAO.WardId = Warehouse.WardId;
            WarehouseDAO.StatusId = Warehouse.StatusId;
            WarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
            WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Warehouse.Add(WarehouseDAO);
            await DataContext.SaveChangesAsync();
            Warehouse.Id = WarehouseDAO.Id;
            await SaveReference(Warehouse);
            return true;
        }

        public async Task<bool> Update(Warehouse Warehouse)
        {
            WarehouseDAO WarehouseDAO = DataContext.Warehouse.Where(x => x.Id == Warehouse.Id).FirstOrDefault();
            if (WarehouseDAO == null)
                return false;
            WarehouseDAO.Id = Warehouse.Id;
            WarehouseDAO.Code = Warehouse.Code;
            WarehouseDAO.Name = Warehouse.Name;
            WarehouseDAO.Address = Warehouse.Address;
            WarehouseDAO.OrganizationId = Warehouse.OrganizationId;
            WarehouseDAO.ProvinceId = Warehouse.ProvinceId;
            WarehouseDAO.DistrictId = Warehouse.DistrictId;
            WarehouseDAO.WardId = Warehouse.WardId;
            WarehouseDAO.StatusId = Warehouse.StatusId;
            WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Warehouse);
            return true;
        }

        public async Task<bool> Delete(Warehouse Warehouse)
        {
            await DataContext.Warehouse.Where(x => x.Id == Warehouse.Id).UpdateFromQueryAsync(x => new WarehouseDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Warehouse> Warehouses)
        {
            List<WarehouseDAO> WarehouseDAOs = new List<WarehouseDAO>();
            foreach (Warehouse Warehouse in Warehouses)
            {
                WarehouseDAO WarehouseDAO = new WarehouseDAO();
                WarehouseDAO.Id = Warehouse.Id;
                WarehouseDAO.Code = Warehouse.Code;
                WarehouseDAO.Name = Warehouse.Name;
                WarehouseDAO.Address = Warehouse.Address;
                WarehouseDAO.OrganizationId = Warehouse.OrganizationId;
                WarehouseDAO.ProvinceId = Warehouse.ProvinceId;
                WarehouseDAO.DistrictId = Warehouse.DistrictId;
                WarehouseDAO.WardId = Warehouse.WardId;
                WarehouseDAO.StatusId = Warehouse.StatusId;
                WarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
                WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
                WarehouseDAOs.Add(WarehouseDAO);
            }
            await DataContext.BulkMergeAsync(WarehouseDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Warehouse> Warehouses)
        {
            List<long> Ids = Warehouses.Select(x => x.Id).ToList();
            await DataContext.Warehouse
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new WarehouseDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Warehouse Warehouse)
        {
            List<InventoryDAO> InventoryDAOs = await DataContext.Inventory
                .Where(x => x.WarehouseId == Warehouse.Id).ToListAsync();
            InventoryDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Warehouse.Inventories != null)
            {
                foreach (Inventory Inventory in Warehouse.Inventories)
                {
                    InventoryDAO InventoryDAO = InventoryDAOs
                        .Where(x => x.Id == Inventory.Id && x.Id != 0).FirstOrDefault();
                    if (InventoryDAO == null)
                    {
                        InventoryDAO = new InventoryDAO();
                        InventoryDAO.Id = Inventory.Id;
                        InventoryDAO.WarehouseId = Warehouse.Id;
                        InventoryDAO.ItemId = Inventory.ItemId;
                        InventoryDAO.SaleStock = Inventory.SaleStock;
                        InventoryDAO.AccountingStock = Inventory.AccountingStock;
                        InventoryDAOs.Add(InventoryDAO);
                        InventoryDAO.CreatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.DeletedAt = null;
                    }
                    else
                    {
                        InventoryDAO.WarehouseId = Warehouse.Id;
                        InventoryDAO.ItemId = Inventory.ItemId;
                        InventoryDAO.SaleStock = Inventory.SaleStock;
                        InventoryDAO.AccountingStock = Inventory.AccountingStock;
                        InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.DeletedAt = null;
                    }
                }
                await DataContext.Inventory.BulkMergeAsync(InventoryDAOs);

                List<InventoryHistoryDAO> InventoryHistoryDAOs = new List<InventoryHistoryDAO>();
                foreach (Inventory Inventory in Warehouse.Inventories)
                {
                    InventoryDAO InventoryDAO = InventoryDAOs
                       .Where(x => x.Id == Inventory.Id && x.Id != 0).FirstOrDefault();
                    if (InventoryDAO != null)
                    {
                        Inventory.Id = InventoryDAO.Id;
                        if (Inventory.InventoryHistories != null)
                        {
                            foreach (InventoryHistory inventoryHistory in Inventory.InventoryHistories)
                            {
                                InventoryHistoryDAO InventoryHistoryDAO = new InventoryHistoryDAO
                                {
                                    InventoryId = InventoryDAO.Id,
                                    AppUserId = inventoryHistory.AppUserId,
                                    SaleStock = inventoryHistory.SaleStock,
                                    AccountingStock = inventoryHistory.AccountingStock,
                                    CreatedAt = StaticParams.DateTimeNow,
                                    UpdatedAt = StaticParams.DateTimeNow,
                                    DeletedAt = null,
                                };
                                InventoryHistoryDAOs.Add(InventoryHistoryDAO);
                            }
                        }
                    }
                }

                await DataContext.InventoryHistory.BulkMergeAsync(InventoryHistoryDAOs);
            }
        }
    }
}