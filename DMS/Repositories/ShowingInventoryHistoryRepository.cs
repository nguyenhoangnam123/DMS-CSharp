using DMS.Common;
using DMS.Helpers;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IShowingInventoryHistoryRepository
    {
        Task<int> Count(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter);
        Task<List<ShowingInventoryHistory>> List(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter);
        Task<List<ShowingInventoryHistory>> List(List<long> Ids);
        Task<ShowingInventoryHistory> Get(long Id);
        Task<bool> Create(ShowingInventoryHistory ShowingInventoryHistory);
        Task<bool> Update(ShowingInventoryHistory ShowingInventoryHistory);
        Task<bool> Delete(ShowingInventoryHistory ShowingInventoryHistory);
        Task<bool> BulkMerge(List<ShowingInventoryHistory> ShowingInventoryHistories);
        Task<bool> BulkDelete(List<ShowingInventoryHistory> ShowingInventoryHistories);
    }
    public class ShowingInventoryHistoryRepository : IShowingInventoryHistoryRepository
    {
        private DataContext DataContext;
        public ShowingInventoryHistoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ShowingInventoryHistoryDAO> DynamicFilter(IQueryable<ShowingInventoryHistoryDAO> query, ShowingInventoryHistoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.ShowingInventoryId != null && filter.ShowingInventoryId.HasValue)
                query = query.Where(q => q.ShowingInventoryId, filter.ShowingInventoryId);
            if (filter.OldSaleStock != null && filter.OldSaleStock.HasValue)
                query = query.Where(q => q.OldSaleStock, filter.OldSaleStock);
            if (filter.SaleStock != null && filter.SaleStock.HasValue)
                query = query.Where(q => q.SaleStock, filter.SaleStock);
            if (filter.OldAccountingStock != null && filter.OldAccountingStock.HasValue)
                query = query.Where(q => q.OldAccountingStock, filter.OldAccountingStock);
            if (filter.AccountingStock != null && filter.AccountingStock.HasValue)
                query = query.Where(q => q.AccountingStock, filter.AccountingStock);
            if (filter.AppUserId != null && filter.AppUserId.HasValue)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ShowingInventoryHistoryDAO> OrFilter(IQueryable<ShowingInventoryHistoryDAO> query, ShowingInventoryHistoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ShowingInventoryHistoryDAO> initQuery = query.Where(q => false);
            foreach (ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter in filter.OrFilter)
            {
                IQueryable<ShowingInventoryHistoryDAO> queryable = query;
                if (ShowingInventoryHistoryFilter.Id != null && ShowingInventoryHistoryFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (ShowingInventoryHistoryFilter.ShowingInventoryId != null && ShowingInventoryHistoryFilter.ShowingInventoryId.HasValue)
                    queryable = queryable.Where(q => q.ShowingInventoryId, filter.ShowingInventoryId);
                if (ShowingInventoryHistoryFilter.OldSaleStock != null && ShowingInventoryHistoryFilter.OldSaleStock.HasValue)
                    queryable = queryable.Where(q => q.OldSaleStock, filter.OldSaleStock);
                if (ShowingInventoryHistoryFilter.SaleStock != null && ShowingInventoryHistoryFilter.SaleStock.HasValue)
                    queryable = queryable.Where(q => q.SaleStock, filter.SaleStock);
                if (ShowingInventoryHistoryFilter.OldAccountingStock != null && ShowingInventoryHistoryFilter.OldAccountingStock.HasValue)
                    queryable = queryable.Where(q => q.OldAccountingStock, filter.OldAccountingStock);
                if (ShowingInventoryHistoryFilter.AccountingStock != null && ShowingInventoryHistoryFilter.AccountingStock.HasValue)
                    queryable = queryable.Where(q => q.AccountingStock, filter.AccountingStock);
                if (ShowingInventoryHistoryFilter.AppUserId != null && ShowingInventoryHistoryFilter.AppUserId.HasValue)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ShowingInventoryHistoryDAO> DynamicOrder(IQueryable<ShowingInventoryHistoryDAO> query, ShowingInventoryHistoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ShowingInventoryHistoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ShowingInventoryHistoryOrder.ShowingInventory:
                            query = query.OrderBy(q => q.ShowingInventoryId);
                            break;
                        case ShowingInventoryHistoryOrder.OldSaleStock:
                            query = query.OrderBy(q => q.OldSaleStock);
                            break;
                        case ShowingInventoryHistoryOrder.SaleStock:
                            query = query.OrderBy(q => q.SaleStock);
                            break;
                        case ShowingInventoryHistoryOrder.OldAccountingStock:
                            query = query.OrderBy(q => q.OldAccountingStock);
                            break;
                        case ShowingInventoryHistoryOrder.AccountingStock:
                            query = query.OrderBy(q => q.AccountingStock);
                            break;
                        case ShowingInventoryHistoryOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ShowingInventoryHistoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ShowingInventoryHistoryOrder.ShowingInventory:
                            query = query.OrderByDescending(q => q.ShowingInventoryId);
                            break;
                        case ShowingInventoryHistoryOrder.OldSaleStock:
                            query = query.OrderByDescending(q => q.OldSaleStock);
                            break;
                        case ShowingInventoryHistoryOrder.SaleStock:
                            query = query.OrderByDescending(q => q.SaleStock);
                            break;
                        case ShowingInventoryHistoryOrder.OldAccountingStock:
                            query = query.OrderByDescending(q => q.OldAccountingStock);
                            break;
                        case ShowingInventoryHistoryOrder.AccountingStock:
                            query = query.OrderByDescending(q => q.AccountingStock);
                            break;
                        case ShowingInventoryHistoryOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ShowingInventoryHistory>> DynamicSelect(IQueryable<ShowingInventoryHistoryDAO> query, ShowingInventoryHistoryFilter filter)
        {
            List<ShowingInventoryHistory> ShowingInventoryHistories = await query.Select(q => new ShowingInventoryHistory()
            {
                Id = filter.Selects.Contains(ShowingInventoryHistorySelect.Id) ? q.Id : default(long),
                ShowingInventoryId = filter.Selects.Contains(ShowingInventoryHistorySelect.ShowingInventory) ? q.ShowingInventoryId : default(long),
                OldSaleStock = filter.Selects.Contains(ShowingInventoryHistorySelect.OldSaleStock) ? q.OldSaleStock : default(long),
                SaleStock = filter.Selects.Contains(ShowingInventoryHistorySelect.SaleStock) ? q.SaleStock : default(long),
                OldAccountingStock = filter.Selects.Contains(ShowingInventoryHistorySelect.OldAccountingStock) ? q.OldAccountingStock : default(long),
                AccountingStock = filter.Selects.Contains(ShowingInventoryHistorySelect.AccountingStock) ? q.AccountingStock : default(long),
                AppUserId = filter.Selects.Contains(ShowingInventoryHistorySelect.AppUser) ? q.AppUserId : default(long),
                AppUser = filter.Selects.Contains(ShowingInventoryHistorySelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                    Address = q.AppUser.Address,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                    SexId = q.AppUser.SexId,
                    Birthday = q.AppUser.Birthday,
                    Avatar = q.AppUser.Avatar,
                    PositionId = q.AppUser.PositionId,
                    Department = q.AppUser.Department,
                    OrganizationId = q.AppUser.OrganizationId,
                    ProvinceId = q.AppUser.ProvinceId,
                    Longitude = q.AppUser.Longitude,
                    Latitude = q.AppUser.Latitude,
                    StatusId = q.AppUser.StatusId,
                    GPSUpdatedAt = q.AppUser.GPSUpdatedAt,
                    RowId = q.AppUser.RowId,
                } : null,
                ShowingInventory = filter.Selects.Contains(ShowingInventoryHistorySelect.ShowingInventory) && q.ShowingInventory != null ? new ShowingInventory
                {
                    Id = q.ShowingInventory.Id,
                    ShowingWarehouseId = q.ShowingInventory.ShowingWarehouseId,
                    ShowingItemId = q.ShowingInventory.ShowingItemId,
                    SaleStock = q.ShowingInventory.SaleStock,
                    AccountingStock = q.ShowingInventory.AccountingStock,
                    AppUserId = q.ShowingInventory.AppUserId,
                } : null,
            }).ToListAsync();
            return ShowingInventoryHistories;
        }

        public async Task<int> Count(ShowingInventoryHistoryFilter filter)
        {
            IQueryable<ShowingInventoryHistoryDAO> ShowingInventoryHistories = DataContext.ShowingInventoryHistory.AsNoTracking();
            ShowingInventoryHistories = DynamicFilter(ShowingInventoryHistories, filter);
            return await ShowingInventoryHistories.CountAsync();
        }

        public async Task<List<ShowingInventoryHistory>> List(ShowingInventoryHistoryFilter filter)
        {
            if (filter == null) return new List<ShowingInventoryHistory>();
            IQueryable<ShowingInventoryHistoryDAO> ShowingInventoryHistoryDAOs = DataContext.ShowingInventoryHistory.AsNoTracking();
            ShowingInventoryHistoryDAOs = DynamicFilter(ShowingInventoryHistoryDAOs, filter);
            ShowingInventoryHistoryDAOs = DynamicOrder(ShowingInventoryHistoryDAOs, filter);
            List<ShowingInventoryHistory> ShowingInventoryHistories = await DynamicSelect(ShowingInventoryHistoryDAOs, filter);
            return ShowingInventoryHistories;
        }

        public async Task<List<ShowingInventoryHistory>> List(List<long> Ids)
        {
            List<ShowingInventoryHistory> ShowingInventoryHistories = await DataContext.ShowingInventoryHistory.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ShowingInventoryHistory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                ShowingInventoryId = x.ShowingInventoryId,
                OldSaleStock = x.OldSaleStock,
                SaleStock = x.SaleStock,
                OldAccountingStock = x.OldAccountingStock,
                AccountingStock = x.AccountingStock,
                AppUserId = x.AppUserId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    SexId = x.AppUser.SexId,
                    Birthday = x.AppUser.Birthday,
                    Avatar = x.AppUser.Avatar,
                    PositionId = x.AppUser.PositionId,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    ProvinceId = x.AppUser.ProvinceId,
                    Longitude = x.AppUser.Longitude,
                    Latitude = x.AppUser.Latitude,
                    StatusId = x.AppUser.StatusId,
                    GPSUpdatedAt = x.AppUser.GPSUpdatedAt,
                    RowId = x.AppUser.RowId,
                },
                ShowingInventory = x.ShowingInventory == null ? null : new ShowingInventory
                {
                    Id = x.ShowingInventory.Id,
                    ShowingWarehouseId = x.ShowingInventory.ShowingWarehouseId,
                    ShowingItemId = x.ShowingInventory.ShowingItemId,
                    SaleStock = x.ShowingInventory.SaleStock,
                    AccountingStock = x.ShowingInventory.AccountingStock,
                    AppUserId = x.ShowingInventory.AppUserId,
                },
            }).ToListAsync();
            

            return ShowingInventoryHistories;
        }

        public async Task<ShowingInventoryHistory> Get(long Id)
        {
            ShowingInventoryHistory ShowingInventoryHistory = await DataContext.ShowingInventoryHistory.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ShowingInventoryHistory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                ShowingInventoryId = x.ShowingInventoryId,
                OldSaleStock = x.OldSaleStock,
                SaleStock = x.SaleStock,
                OldAccountingStock = x.OldAccountingStock,
                AccountingStock = x.AccountingStock,
                AppUserId = x.AppUserId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    SexId = x.AppUser.SexId,
                    Birthday = x.AppUser.Birthday,
                    Avatar = x.AppUser.Avatar,
                    PositionId = x.AppUser.PositionId,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    ProvinceId = x.AppUser.ProvinceId,
                    Longitude = x.AppUser.Longitude,
                    Latitude = x.AppUser.Latitude,
                    StatusId = x.AppUser.StatusId,
                    GPSUpdatedAt = x.AppUser.GPSUpdatedAt,
                    RowId = x.AppUser.RowId,
                },
                ShowingInventory = x.ShowingInventory == null ? null : new ShowingInventory
                {
                    Id = x.ShowingInventory.Id,
                    ShowingWarehouseId = x.ShowingInventory.ShowingWarehouseId,
                    ShowingItemId = x.ShowingInventory.ShowingItemId,
                    SaleStock = x.ShowingInventory.SaleStock,
                    AccountingStock = x.ShowingInventory.AccountingStock,
                    AppUserId = x.ShowingInventory.AppUserId,
                },
            }).FirstOrDefaultAsync();

            if (ShowingInventoryHistory == null)
                return null;

            return ShowingInventoryHistory;
        }
        public async Task<bool> Create(ShowingInventoryHistory ShowingInventoryHistory)
        {
            ShowingInventoryHistoryDAO ShowingInventoryHistoryDAO = new ShowingInventoryHistoryDAO();
            ShowingInventoryHistoryDAO.Id = ShowingInventoryHistory.Id;
            ShowingInventoryHistoryDAO.ShowingInventoryId = ShowingInventoryHistory.ShowingInventoryId;
            ShowingInventoryHistoryDAO.OldSaleStock = ShowingInventoryHistory.OldSaleStock;
            ShowingInventoryHistoryDAO.SaleStock = ShowingInventoryHistory.SaleStock;
            ShowingInventoryHistoryDAO.OldAccountingStock = ShowingInventoryHistory.OldAccountingStock;
            ShowingInventoryHistoryDAO.AccountingStock = ShowingInventoryHistory.AccountingStock;
            ShowingInventoryHistoryDAO.AppUserId = ShowingInventoryHistory.AppUserId;
            ShowingInventoryHistoryDAO.CreatedAt = StaticParams.DateTimeNow;
            ShowingInventoryHistoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ShowingInventoryHistory.Add(ShowingInventoryHistoryDAO);
            await DataContext.SaveChangesAsync();
            ShowingInventoryHistory.Id = ShowingInventoryHistoryDAO.Id;
            await SaveReference(ShowingInventoryHistory);
            return true;
        }

        public async Task<bool> Update(ShowingInventoryHistory ShowingInventoryHistory)
        {
            ShowingInventoryHistoryDAO ShowingInventoryHistoryDAO = DataContext.ShowingInventoryHistory.Where(x => x.Id == ShowingInventoryHistory.Id).FirstOrDefault();
            if (ShowingInventoryHistoryDAO == null)
                return false;
            ShowingInventoryHistoryDAO.Id = ShowingInventoryHistory.Id;
            ShowingInventoryHistoryDAO.ShowingInventoryId = ShowingInventoryHistory.ShowingInventoryId;
            ShowingInventoryHistoryDAO.OldSaleStock = ShowingInventoryHistory.OldSaleStock;
            ShowingInventoryHistoryDAO.SaleStock = ShowingInventoryHistory.SaleStock;
            ShowingInventoryHistoryDAO.OldAccountingStock = ShowingInventoryHistory.OldAccountingStock;
            ShowingInventoryHistoryDAO.AccountingStock = ShowingInventoryHistory.AccountingStock;
            ShowingInventoryHistoryDAO.AppUserId = ShowingInventoryHistory.AppUserId;
            ShowingInventoryHistoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ShowingInventoryHistory);
            return true;
        }

        public async Task<bool> Delete(ShowingInventoryHistory ShowingInventoryHistory)
        {
            await DataContext.ShowingInventoryHistory.Where(x => x.Id == ShowingInventoryHistory.Id).UpdateFromQueryAsync(x => new ShowingInventoryHistoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ShowingInventoryHistory> ShowingInventoryHistories)
        {
            List<ShowingInventoryHistoryDAO> ShowingInventoryHistoryDAOs = new List<ShowingInventoryHistoryDAO>();
            foreach (ShowingInventoryHistory ShowingInventoryHistory in ShowingInventoryHistories)
            {
                ShowingInventoryHistoryDAO ShowingInventoryHistoryDAO = new ShowingInventoryHistoryDAO();
                ShowingInventoryHistoryDAO.Id = ShowingInventoryHistory.Id;
                ShowingInventoryHistoryDAO.ShowingInventoryId = ShowingInventoryHistory.ShowingInventoryId;
                ShowingInventoryHistoryDAO.OldSaleStock = ShowingInventoryHistory.OldSaleStock;
                ShowingInventoryHistoryDAO.SaleStock = ShowingInventoryHistory.SaleStock;
                ShowingInventoryHistoryDAO.OldAccountingStock = ShowingInventoryHistory.OldAccountingStock;
                ShowingInventoryHistoryDAO.AccountingStock = ShowingInventoryHistory.AccountingStock;
                ShowingInventoryHistoryDAO.AppUserId = ShowingInventoryHistory.AppUserId;
                ShowingInventoryHistoryDAO.CreatedAt = StaticParams.DateTimeNow;
                ShowingInventoryHistoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                ShowingInventoryHistoryDAOs.Add(ShowingInventoryHistoryDAO);
            }
            await DataContext.BulkMergeAsync(ShowingInventoryHistoryDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ShowingInventoryHistory> ShowingInventoryHistories)
        {
            List<long> Ids = ShowingInventoryHistories.Select(x => x.Id).ToList();
            await DataContext.ShowingInventoryHistory
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ShowingInventoryHistoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ShowingInventoryHistory ShowingInventoryHistory)
        {
        }
        
    }
}
