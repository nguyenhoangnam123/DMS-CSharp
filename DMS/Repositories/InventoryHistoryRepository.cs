using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IInventoryHistoryRepository
    {
        Task<int> Count(InventoryHistoryFilter InventoryHistoryFilter);
        Task<List<InventoryHistory>> List(InventoryHistoryFilter InventoryHistoryFilter);
        Task<InventoryHistory> Get(long Id);
    }
    public class InventoryHistoryRepository : IInventoryHistoryRepository
    {
        private DataContext DataContext;
        public InventoryHistoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<InventoryHistoryDAO> DynamicFilter(IQueryable<InventoryHistoryDAO> query, InventoryHistoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.InventoryId != null)
                query = query.Where(q => q.InventoryId, filter.InventoryId);
            if (filter.OldSaleStock != null)
                query = query.Where(q => q.OldSaleStock, filter.OldSaleStock);
            if (filter.OldAccountingStock != null)
                query = query.Where(q => q.OldAccountingStock, filter.OldAccountingStock);
            if (filter.SaleStock != null)
                query = query.Where(q => q.SaleStock, filter.SaleStock);
            if (filter.AccountingStock != null)
                query = query.Where(q => q.AccountingStock, filter.AccountingStock);
            if (filter.AppUserId != null)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.UpdateTime != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdateTime);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<InventoryHistoryDAO> OrFilter(IQueryable<InventoryHistoryDAO> query, InventoryHistoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<InventoryHistoryDAO> initQuery = query.Where(q => false);
            foreach (InventoryHistoryFilter InventoryHistoryFilter in filter.OrFilter)
            {
                IQueryable<InventoryHistoryDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.InventoryId != null)
                    queryable = queryable.Where(q => q.InventoryId, filter.InventoryId);
                if (filter.OldSaleStock != null)
                    queryable = queryable.Where(q => q.OldSaleStock, filter.OldSaleStock);
                if (filter.OldAccountingStock != null)
                    queryable = queryable.Where(q => q.OldAccountingStock, filter.OldAccountingStock);
                if (filter.SaleStock != null)
                    queryable = queryable.Where(q => q.SaleStock, filter.SaleStock);
                if (filter.AccountingStock != null)
                    queryable = queryable.Where(q => q.AccountingStock, filter.AccountingStock);
                if (filter.AppUserId != null)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                if (filter.UpdateTime != null)
                    queryable = queryable.Where(q => q.UpdatedAt, filter.UpdateTime);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<InventoryHistoryDAO> DynamicOrder(IQueryable<InventoryHistoryDAO> query, InventoryHistoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case InventoryHistoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case InventoryHistoryOrder.Inventory:
                            query = query.OrderBy(q => q.InventoryId);
                            break;
                        case InventoryHistoryOrder.OldSaleStock:
                            query = query.OrderBy(q => q.OldSaleStock);
                            break;
                        case InventoryHistoryOrder.SaleStock:
                            query = query.OrderBy(q => q.SaleStock);
                            break;
                        case InventoryHistoryOrder.OldAccountingStock:
                            query = query.OrderBy(q => q.OldAccountingStock);
                            break;
                        case InventoryHistoryOrder.AccountingStock:
                            query = query.OrderBy(q => q.AccountingStock);
                            break;
                        case InventoryHistoryOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case InventoryHistoryOrder.UpdateTime:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case InventoryHistoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case InventoryHistoryOrder.Inventory:
                            query = query.OrderByDescending(q => q.InventoryId);
                            break;
                        case InventoryHistoryOrder.OldSaleStock:
                            query = query.OrderByDescending(q => q.OldSaleStock);
                            break;
                        case InventoryHistoryOrder.SaleStock:
                            query = query.OrderByDescending(q => q.SaleStock);
                            break;
                        case InventoryHistoryOrder.OldAccountingStock:
                            query = query.OrderByDescending(q => q.OldAccountingStock);
                            break;
                        case InventoryHistoryOrder.AccountingStock:
                            query = query.OrderByDescending(q => q.AccountingStock);
                            break;
                        case InventoryHistoryOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case InventoryHistoryOrder.UpdateTime:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<InventoryHistory>> DynamicSelect(IQueryable<InventoryHistoryDAO> query, InventoryHistoryFilter filter)
        {
            List<InventoryHistory> InventoryHistorys = await query.Select(q => new InventoryHistory()
            {
                Id = filter.Selects.Contains(InventoryHistorySelect.Id) ? q.Id : default(long),
                InventoryId = filter.Selects.Contains(InventoryHistorySelect.Inventory) ? q.InventoryId : default(long),
                OldSaleStock = filter.Selects.Contains(InventoryHistorySelect.OldSaleStock) ? q.OldSaleStock : default(long),
                SaleStock = filter.Selects.Contains(InventoryHistorySelect.SaleStock) ? q.SaleStock : default(long),
                OldAccountingStock = filter.Selects.Contains(InventoryHistorySelect.OldAccountingStock) ? q.OldAccountingStock : default(long),
                AccountingStock = filter.Selects.Contains(InventoryHistorySelect.AccountingStock) ? q.AccountingStock : default(long),
                AppUserId = filter.Selects.Contains(InventoryHistorySelect.AppUser) ? q.AppUserId : default(long),
                UpdateTime = filter.Selects.Contains(InventoryHistorySelect.UpdateTime) ? q.UpdatedAt : default(DateTime),
                Inventory = filter.Selects.Contains(InventoryHistorySelect.Inventory) && q.Inventory != null ? new Inventory
                {
                    Id = q.Inventory.Id,
                    WarehouseId = q.Inventory.WarehouseId,
                    ItemId = q.Inventory.ItemId,
                    SaleStock = q.Inventory.SaleStock,
                    AccountingStock = q.Inventory.AccountingStock,
                } : null,
                AppUser = filter.Selects.Contains(InventoryHistorySelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Address = q.AppUser.Address,
                    DisplayName = q.AppUser.DisplayName,
                    Department = q.AppUser.Department,
                    Birthday = q.AppUser.Birthday,
                    Email = q.AppUser.Email,
                    OrganizationId = q.AppUser.OrganizationId,
                    Phone = q.AppUser.Phone,
                    Position = q.AppUser.Position,
                    RowId = q.AppUser.RowId,
                    SexId = q.AppUser.SexId,
                    StatusId = q.AppUser.StatusId,
                } : null,
            }).ToListAsync();
            return InventoryHistorys;
        }

        public async Task<int> Count(InventoryHistoryFilter filter)
        {
            IQueryable<InventoryHistoryDAO> InventoryHistorys = DataContext.InventoryHistory;
            InventoryHistorys = DynamicFilter(InventoryHistorys, filter);
            return await InventoryHistorys.CountAsync();
        }

        public async Task<List<InventoryHistory>> List(InventoryHistoryFilter filter)
        {
            if (filter == null) return new List<InventoryHistory>();
            IQueryable<InventoryHistoryDAO> InventoryHistoryDAOs = DataContext.InventoryHistory.AsNoTracking();
            InventoryHistoryDAOs = DynamicFilter(InventoryHistoryDAOs, filter);
            InventoryHistoryDAOs = DynamicOrder(InventoryHistoryDAOs, filter);
            List<InventoryHistory> InventoryHistorys = await DynamicSelect(InventoryHistoryDAOs, filter);
            return InventoryHistorys;
        }

        public async Task<InventoryHistory> Get(long Id)
        {
            InventoryHistory InventoryHistory = await DataContext.InventoryHistory.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new InventoryHistory()
                {
                    Id = x.Id,
                    InventoryId = x.InventoryId,
                    OldSaleStock = x.OldSaleStock,
                    SaleStock = x.SaleStock,
                    OldAccountingStock = x.OldAccountingStock,
                    AccountingStock = x.AccountingStock,
                    AppUserId = x.AppUserId,
                    UpdateTime = x.UpdatedAt,
                    Inventory = x.Inventory == null ? null : new Inventory
                    {
                        Id = x.Inventory.Id,
                        WarehouseId = x.Inventory.WarehouseId,
                        ItemId = x.Inventory.ItemId,
                        SaleStock = x.Inventory.SaleStock,
                        AccountingStock = x.Inventory.AccountingStock,
                    },
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        Address = x.AppUser.Address,
                        DisplayName = x.AppUser.DisplayName,
                        Department = x.AppUser.Department,
                        Birthday = x.AppUser.Birthday,
                        Email = x.AppUser.Email,
                        OrganizationId = x.AppUser.OrganizationId,
                        Phone = x.AppUser.Phone,
                        Position = x.AppUser.Position,
                        RowId = x.AppUser.RowId,
                        SexId = x.AppUser.SexId,
                        StatusId = x.AppUser.StatusId,
                    }
                }).FirstOrDefaultAsync();

            if (InventoryHistory == null)
                return null;
            return InventoryHistory;
        }
    }
}
