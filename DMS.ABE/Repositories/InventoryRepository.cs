﻿using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Repositories
{
    public interface IInventoryRepository
    {
        Task<int> Count(InventoryFilter InventoryFilter);
        Task<List<Inventory>> List(InventoryFilter InventoryFilter);
        Task<Inventory> Get(long Id);
    }
    public class InventoryRepository : IInventoryRepository
    {
        private DataContext DataContext;
        public InventoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<InventoryDAO> DynamicFilter(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.WarehouseId != null)
                query = query.Where(q => q.WarehouseId, filter.WarehouseId);
            if (filter.ItemId != null)
                query = query.Where(q => q.ItemId, filter.ItemId);
            if (filter.SaleStock != null)
                query = query.Where(q => q.SaleStock, filter.SaleStock);
            if (filter.AccountingStock != null)
                query = query.Where(q => q.AccountingStock, filter.AccountingStock);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<InventoryDAO> OrFilter(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<InventoryDAO> initQuery = query.Where(q => false);
            foreach (InventoryFilter InventoryFilter in filter.OrFilter)
            {
                IQueryable<InventoryDAO> queryable = query;
                if (InventoryFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, InventoryFilter.Id);
                if (InventoryFilter.WarehouseId != null)
                    queryable = queryable.Where(q => q.WarehouseId, InventoryFilter.WarehouseId);
                if (InventoryFilter.ItemId != null)
                    queryable = queryable.Where(q => q.ItemId, InventoryFilter.ItemId);
                if (InventoryFilter.SaleStock != null)
                    queryable = queryable.Where(q => q.SaleStock, InventoryFilter.SaleStock);
                if (InventoryFilter.AccountingStock != null)
                    queryable = queryable.Where(q => q.AccountingStock, InventoryFilter.AccountingStock);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<InventoryDAO> DynamicOrder(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case InventoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case InventoryOrder.Warehouse:
                            query = query.OrderBy(q => q.WarehouseId);
                            break;
                        case InventoryOrder.Item:
                            query = query.OrderBy(q => q.ItemId);
                            break;
                        case InventoryOrder.SaleStock:
                            query = query.OrderBy(q => q.SaleStock);
                            break;
                        case InventoryOrder.AccountingStock:
                            query = query.OrderBy(q => q.AccountingStock);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case InventoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case InventoryOrder.Warehouse:
                            query = query.OrderByDescending(q => q.WarehouseId);
                            break;
                        case InventoryOrder.Item:
                            query = query.OrderByDescending(q => q.ItemId);
                            break;
                        case InventoryOrder.SaleStock:
                            query = query.OrderByDescending(q => q.SaleStock);
                            break;
                        case InventoryOrder.AccountingStock:
                            query = query.OrderByDescending(q => q.AccountingStock);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Inventory>> DynamicSelect(IQueryable<InventoryDAO> query, InventoryFilter filter)
        {
            List<Inventory> Inventorys = await query.Select(q => new Inventory()
            {
                Id = filter.Selects.Contains(InventorySelect.Id) ? q.Id : default(long),
                WarehouseId = filter.Selects.Contains(InventorySelect.Warehouse) ? q.WarehouseId : default(long),
                ItemId = filter.Selects.Contains(InventorySelect.Item) ? q.ItemId : default(long),
                SaleStock = filter.Selects.Contains(InventorySelect.SaleStock) ? q.SaleStock : default(long),
                AccountingStock = filter.Selects.Contains(InventorySelect.AccountingStock) ? q.AccountingStock : default(long),
                UpdatedAt = filter.Selects.Contains(InventorySelect.UpdatedAt) ? q.UpdatedAt : default(DateTime),
                Warehouse = filter.Selects.Contains(InventorySelect.Warehouse) && q.Warehouse != null ? new Warehouse
                {
                    Id = q.Warehouse.Id,
                    Code = q.Warehouse.Code,
                    Name = q.Warehouse.Name,
                    Address = q.Warehouse.Address,
                    OrganizationId = q.Warehouse.OrganizationId,
                    DistrictId = q.Warehouse.DistrictId,
                    ProvinceId = q.Warehouse.ProvinceId,
                    WardId = q.Warehouse.WardId,
                    StatusId = q.Warehouse.StatusId,
                } : null,
                Item = filter.Selects.Contains(InventorySelect.Item) && q.Item != null ? new Item
                {
                    Id = q.Item.Id,
                    Code = q.Item.Code,
                    Name = q.Item.Name,
                    ProductId = q.Item.ProductId,
                    SalePrice = q.Item.SalePrice,
                    RetailPrice = q.Item.RetailPrice,
                    ScanCode = q.Item.ScanCode,
                } : null,
            }).ToListAsync();
            return Inventorys;
        }

        public async Task<int> Count(InventoryFilter filter)
        {
            IQueryable<InventoryDAO> Inventorys = DataContext.Inventory;
            Inventorys = DynamicFilter(Inventorys, filter);
            return await Inventorys.CountAsync();
        }

        public async Task<List<Inventory>> List(InventoryFilter filter)
        {
            if (filter == null) return new List<Inventory>();
            IQueryable<InventoryDAO> InventoryDAOs = DataContext.Inventory.AsNoTracking();
            InventoryDAOs = DynamicFilter(InventoryDAOs, filter);
            InventoryDAOs = DynamicOrder(InventoryDAOs, filter);
            List<Inventory> Inventorys = await DynamicSelect(InventoryDAOs, filter);
            return Inventorys;
        }

        public async Task<Inventory> Get(long Id)
        {
            Inventory Inventory = await DataContext.Inventory.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Inventory()
                {
                    Id = x.Id,
                    WarehouseId = x.WarehouseId,
                    ItemId = x.ItemId,
                    SaleStock = x.SaleStock,
                    AccountingStock = x.AccountingStock,
                    UpdatedAt = x.UpdatedAt,
                    Warehouse = x.Warehouse == null ? null : new Warehouse
                    {
                        Id = x.Warehouse.Id,
                        Code = x.Warehouse.Code,
                        Name = x.Warehouse.Name,
                        Address = x.Warehouse.Address,
                        OrganizationId = x.Warehouse.OrganizationId,
                        DistrictId = x.Warehouse.DistrictId,
                        ProvinceId = x.Warehouse.ProvinceId,
                        WardId = x.Warehouse.WardId,
                        StatusId = x.Warehouse.StatusId,
                    },
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        ScanCode = x.Item.ScanCode,
                    }
                }).FirstOrDefaultAsync();

            if (Inventory == null)
                return null;
            Inventory.InventoryHistories = await DataContext.InventoryHistory
                .Where(x => x.InventoryId == Inventory.Id)
                .Select(x => new InventoryHistory
                {
                    Id = x.Id,
                    InventoryId = x.InventoryId,
                    SaleStock = x.SaleStock,
                    OldSaleStock = x.OldSaleStock,
                    OldAccountingStock = x.OldAccountingStock,
                    AccountingStock = x.AccountingStock,
                    AppUserId = x.AppUserId,
                    AppUser = new AppUser
                    {
                        Address = x.AppUser.Address,
                        DisplayName = x.AppUser.DisplayName,
                        Department = x.AppUser.Department,
                        Birthday = x.AppUser.Birthday,
                        Email = x.AppUser.Email,
                        OrganizationId = x.AppUser.OrganizationId,
                        Phone = x.AppUser.Phone,
                        PositionId = x.AppUser.PositionId,
                        RowId = x.AppUser.RowId,
                        SexId = x.AppUser.SexId,
                        StatusId = x.AppUser.StatusId,
                    }
                }).ToListAsync();

            return Inventory;
        }
    }
}
