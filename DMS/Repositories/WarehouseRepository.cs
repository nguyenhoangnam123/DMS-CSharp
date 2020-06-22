using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            {
                if (filter.OrganizationId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value).FirstOrDefault();
                    query = query.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => Ids.Contains(q.OrganizationId));
                }
                if (filter.OrganizationId.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => !Ids.Contains(q.OrganizationId));
                }
            }
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
                if (WarehouseFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WarehouseFilter.Id);
                if (WarehouseFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WarehouseFilter.Code);
                if (WarehouseFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WarehouseFilter.Name);
                if (WarehouseFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, WarehouseFilter.Address);
                if (WarehouseFilter.OrganizationId != null)
                {
                    if (WarehouseFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == WarehouseFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (WarehouseFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == WarehouseFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (WarehouseFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => WarehouseFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => Ids.Contains(q.OrganizationId));
                    }
                    if (WarehouseFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => WarehouseFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => !Ids.Contains(q.OrganizationId));
                    }
                }
                if (WarehouseFilter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, WarehouseFilter.ProvinceId);
                if (WarehouseFilter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, WarehouseFilter.DistrictId);
                if (WarehouseFilter.WardId != null)
                    queryable = queryable.Where(q => q.WardId, WarehouseFilter.WardId);
                if (WarehouseFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, WarehouseFilter.StatusId);
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
                Id = q.Id,
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
            List<long> WarehouseIds = Warehouses.Select(x => x.Id).ToList();
            List<InventoryDAO> InventoryDAOs = await DataContext.Inventory.Where(i => WarehouseIds.Contains(i.WarehouseId)).ToListAsync();
            foreach (Warehouse Warehouse in Warehouses)
            {
                Warehouse.Used = InventoryDAOs.Where(x => x.WarehouseId == Warehouse.Id).Select(x => x.SaleStock).DefaultIfEmpty(0).Sum() > 0;
            }
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
                    RowId = x.RowId,
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
            List<Item> Items = await DataContext.Item.AsNoTracking()
                .Where(i => i.DeletedAt == null && i.StatusId == StatusEnum.ACTIVE.Id)
                .Where(i => i.Product.StatusId == StatusEnum.ACTIVE.Id && i.Product.DeletedAt == null)
                .Select(x => new Item
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Code = x.Code,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        Name = x.Product.Name,
                        ERPCode = x.Product.ERPCode,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        UnitOfMeasure = new UnitOfMeasure
                        {
                            Id = x.Product.UnitOfMeasure.Id,
                            Code = x.Product.UnitOfMeasure.Code,
                            Name = x.Product.UnitOfMeasure.Name,
                        },
                    }
                }).ToListAsync();

            Warehouse.Inventories = await DataContext.Inventory.AsNoTracking()
                .Where(x => x.WarehouseId == Warehouse.Id)
                .Where(x => x.DeletedAt == null && x.Item.DeletedAt == null && x.Item.StatusId == StatusEnum.ACTIVE.Id)
                .Select(x => new Inventory
                {
                    Id = x.Id,
                    WarehouseId = x.WarehouseId,
                    ItemId = x.ItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                }).ToListAsync();
            Warehouse.Used = Warehouse.Inventories.Select(i => i.SaleStock).DefaultIfEmpty(0).Sum() > 0;
            foreach (Item Item in Items)
            {
                Inventory Inventory = Warehouse.Inventories.Where(i => i.ItemId == Item.Id).FirstOrDefault();
                if (Inventory == null)
                {
                    Inventory = new Inventory
                    {
                        Id = 0,
                        WarehouseId = Warehouse.Id,
                        ItemId = Item.Id,
                        SaleStock = 0,
                        AccountingStock = 0,
                    };
                    Warehouse.Inventories.Add(Inventory);
                }
                Inventory.Item = Item;
            }
            Warehouse.Inventories = Warehouse.Inventories.OrderBy(i => i.ItemId).ToList();
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
            WarehouseDAO.RowId = Warehouse.RowId;
            WarehouseDAO.CreatedAt = StaticParams.DateTimeNow;
            WarehouseDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Warehouse.Add(WarehouseDAO);
            await DataContext.SaveChangesAsync();
            Warehouse.Id = WarehouseDAO.Id;
            Warehouse.RowId = WarehouseDAO.RowId;
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
                WarehouseDAO.RowId = Warehouse.RowId;
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

            List<long> InventoryIds = InventoryDAOs.Select(x => x.Id).ToList();
            List<InventoryHistoryDAO> InventoryHistoryDAOs = await DataContext.InventoryHistory
                .Where(x => InventoryIds.Contains(x.InventoryId)).ToListAsync();
            InventoryHistoryDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);

            if (Warehouse.Inventories != null)
            {
                foreach (Inventory Inventory in Warehouse.Inventories)
                {
                    InventoryDAO InventoryDAO = InventoryDAOs
                        .Where(x => x.RowId == Inventory.RowId).FirstOrDefault();
                    if (InventoryDAO == null)
                    {
                        InventoryDAO = new InventoryDAO();
                        InventoryDAO.Id = Inventory.Id;
                        InventoryDAO.WarehouseId = Warehouse.Id;
                        InventoryDAO.ItemId = Inventory.ItemId;
                        InventoryDAO.SaleStock = Inventory.SaleStock;
                        InventoryDAO.AccountingStock = Inventory.AccountingStock;
                        InventoryDAO.RowId = Guid.NewGuid();
                        InventoryDAO.CreatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.DeletedAt = null;
                        InventoryDAOs.Add(InventoryDAO);
                        Inventory.RowId = InventoryDAO.RowId;
                    }
                    else
                    {
                        InventoryDAO.WarehouseId = Warehouse.Id;
                        InventoryDAO.ItemId = Inventory.ItemId;
                        InventoryDAO.SaleStock = Inventory.SaleStock;
                        InventoryDAO.AccountingStock = Inventory.AccountingStock;
                        InventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                        InventoryDAO.DeletedAt = null;
                        Inventory.RowId = InventoryDAO.RowId;
                    }
                }
                await DataContext.Inventory.BulkMergeAsync(InventoryDAOs);

                foreach (Inventory Inventory in Warehouse.Inventories)
                {
                    InventoryDAO InventoryDAO = InventoryDAOs
                       .Where(x => x.RowId == Inventory.RowId).FirstOrDefault();
                    if (InventoryDAO != null)
                    {
                        if (Inventory.InventoryHistories != null)
                        {
                            foreach (InventoryHistory inventoryHistory in Inventory.InventoryHistories)
                            {
                                InventoryHistoryDAO InventoryHistoryDAO = InventoryHistoryDAOs.Where(x => x.Id == inventoryHistory.Id).FirstOrDefault();
                                if(InventoryHistoryDAO == null)
                                {
                                    InventoryHistoryDAO = new InventoryHistoryDAO
                                    {
                                        InventoryId = InventoryDAO.Id,
                                        AppUserId = inventoryHistory.AppUserId,
                                        SaleStock = inventoryHistory.SaleStock,
                                        AccountingStock = inventoryHistory.AccountingStock,
                                        OldAccountingStock = inventoryHistory.OldAccountingStock,
                                        OldSaleStock = inventoryHistory.OldSaleStock,
                                        CreatedAt = StaticParams.DateTimeNow,
                                        UpdatedAt = StaticParams.DateTimeNow,
                                        DeletedAt = null,
                                    };
                                    InventoryHistoryDAOs.Add(InventoryHistoryDAO);
                                }
                            }
                        }
                    }
                }
                await DataContext.InventoryHistory.BulkMergeAsync(InventoryHistoryDAOs);
            }
        }
    }
}
