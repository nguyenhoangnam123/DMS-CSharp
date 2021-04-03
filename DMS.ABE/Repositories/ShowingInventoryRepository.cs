using DMS.ABE.Common;
using DMS.ABE.Helpers;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Repositories
{
    public interface IShowingInventoryRepository
    {
        Task<int> Count(ShowingInventoryFilter ShowingInventoryFilter);
        Task<List<ShowingInventory>> List(ShowingInventoryFilter ShowingInventoryFilter);
        Task<List<ShowingInventory>> List(List<long> Ids);
        Task<ShowingInventory> Get(long Id);
        Task<bool> Create(ShowingInventory ShowingInventory);
        Task<bool> Update(ShowingInventory ShowingInventory);
        Task<bool> Delete(ShowingInventory ShowingInventory);
        Task<bool> BulkMerge(List<ShowingInventory> ShowingInventories);
        Task<bool> BulkDelete(List<ShowingInventory> ShowingInventories);
    }
    public class ShowingInventoryRepository : IShowingInventoryRepository
    {
        private DataContext DataContext;
        public ShowingInventoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ShowingInventoryDAO> DynamicFilter(IQueryable<ShowingInventoryDAO> query, ShowingInventoryFilter filter)
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
            if (filter.ShowingWarehouseId != null && filter.ShowingWarehouseId.HasValue)
                query = query.Where(q => q.ShowingWarehouseId, filter.ShowingWarehouseId);
            if (filter.ShowingItemId != null && filter.ShowingItemId.HasValue)
                query = query.Where(q => q.ShowingItemId, filter.ShowingItemId);
            if (filter.SaleStock != null && filter.SaleStock.HasValue)
                query = query.Where(q => q.SaleStock, filter.SaleStock);
            if (filter.AccountingStock != null && filter.AccountingStock.HasValue)
                query = query.Where(q => q.AccountingStock.HasValue).Where(q => q.AccountingStock.Value, filter.AccountingStock);
            if (filter.AppUserId != null && filter.AppUserId.HasValue)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ShowingInventoryDAO> OrFilter(IQueryable<ShowingInventoryDAO> query, ShowingInventoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ShowingInventoryDAO> initQuery = query.Where(q => false);
            foreach (ShowingInventoryFilter ShowingInventoryFilter in filter.OrFilter)
            {
                IQueryable<ShowingInventoryDAO> queryable = query;
                if (ShowingInventoryFilter.Id != null && ShowingInventoryFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (ShowingInventoryFilter.ShowingWarehouseId != null && ShowingInventoryFilter.ShowingWarehouseId.HasValue)
                    queryable = queryable.Where(q => q.ShowingWarehouseId, filter.ShowingWarehouseId);
                if (ShowingInventoryFilter.ShowingItemId != null && ShowingInventoryFilter.ShowingItemId.HasValue)
                    queryable = queryable.Where(q => q.ShowingItemId, filter.ShowingItemId);
                if (ShowingInventoryFilter.SaleStock != null && ShowingInventoryFilter.SaleStock.HasValue)
                    queryable = queryable.Where(q => q.SaleStock, filter.SaleStock);
                if (ShowingInventoryFilter.AccountingStock != null && ShowingInventoryFilter.AccountingStock.HasValue)
                    queryable = queryable.Where(q => q.AccountingStock.HasValue).Where(q => q.AccountingStock.Value, filter.AccountingStock);
                if (ShowingInventoryFilter.AppUserId != null && ShowingInventoryFilter.AppUserId.HasValue)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ShowingInventoryDAO> DynamicOrder(IQueryable<ShowingInventoryDAO> query, ShowingInventoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ShowingInventoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ShowingInventoryOrder.ShowingWarehouse:
                            query = query.OrderBy(q => q.ShowingWarehouseId);
                            break;
                        case ShowingInventoryOrder.ShowingItem:
                            query = query.OrderBy(q => q.ShowingItemId);
                            break;
                        case ShowingInventoryOrder.SaleStock:
                            query = query.OrderBy(q => q.SaleStock);
                            break;
                        case ShowingInventoryOrder.AccountingStock:
                            query = query.OrderBy(q => q.AccountingStock);
                            break;
                        case ShowingInventoryOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ShowingInventoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ShowingInventoryOrder.ShowingWarehouse:
                            query = query.OrderByDescending(q => q.ShowingWarehouseId);
                            break;
                        case ShowingInventoryOrder.ShowingItem:
                            query = query.OrderByDescending(q => q.ShowingItemId);
                            break;
                        case ShowingInventoryOrder.SaleStock:
                            query = query.OrderByDescending(q => q.SaleStock);
                            break;
                        case ShowingInventoryOrder.AccountingStock:
                            query = query.OrderByDescending(q => q.AccountingStock);
                            break;
                        case ShowingInventoryOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ShowingInventory>> DynamicSelect(IQueryable<ShowingInventoryDAO> query, ShowingInventoryFilter filter)
        {
            List<ShowingInventory> ShowingInventories = await query.Select(q => new ShowingInventory()
            {
                Id = filter.Selects.Contains(ShowingInventorySelect.Id) ? q.Id : default(long),
                ShowingWarehouseId = filter.Selects.Contains(ShowingInventorySelect.ShowingWarehouse) ? q.ShowingWarehouseId : default(long),
                ShowingItemId = filter.Selects.Contains(ShowingInventorySelect.ShowingItem) ? q.ShowingItemId : default(long),
                SaleStock = filter.Selects.Contains(ShowingInventorySelect.SaleStock) ? q.SaleStock : default(long),
                AccountingStock = filter.Selects.Contains(ShowingInventorySelect.AccountingStock) ? q.AccountingStock : default(long?),
                AppUserId = filter.Selects.Contains(ShowingInventorySelect.AppUser) ? q.AppUserId : default(long),
                AppUser = filter.Selects.Contains(ShowingInventorySelect.AppUser) && q.AppUser != null ? new AppUser
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
                ShowingItem = filter.Selects.Contains(ShowingInventorySelect.ShowingItem) && q.ShowingItem != null ? new ShowingItem
                {
                    Id = q.ShowingItem.Id,
                    Code = q.ShowingItem.Code,
                    Name = q.ShowingItem.Name,
                    ShowingCategoryId = q.ShowingItem.ShowingCategoryId,
                    UnitOfMeasureId = q.ShowingItem.UnitOfMeasureId,
                    SalePrice = q.ShowingItem.SalePrice,
                    Description = q.ShowingItem.Description,
                    StatusId = q.ShowingItem.StatusId,
                    Used = q.ShowingItem.Used,
                    RowId = q.ShowingItem.RowId,
                } : null,
                ShowingWarehouse = filter.Selects.Contains(ShowingInventorySelect.ShowingWarehouse) && q.ShowingWarehouse != null ? new ShowingWarehouse
                {
                    Id = q.ShowingWarehouse.Id,
                    Code = q.ShowingWarehouse.Code,
                    Name = q.ShowingWarehouse.Name,
                    Address = q.ShowingWarehouse.Address,
                    OrganizationId = q.ShowingWarehouse.OrganizationId,
                    ProvinceId = q.ShowingWarehouse.ProvinceId,
                    DistrictId = q.ShowingWarehouse.DistrictId,
                    WardId = q.ShowingWarehouse.WardId,
                    StatusId = q.ShowingWarehouse.StatusId,
                    RowId = q.ShowingWarehouse.RowId,
                } : null,
            }).ToListAsync();
            return ShowingInventories;
        }

        public async Task<int> Count(ShowingInventoryFilter filter)
        {
            IQueryable<ShowingInventoryDAO> ShowingInventories = DataContext.ShowingInventory.AsNoTracking();
            ShowingInventories = DynamicFilter(ShowingInventories, filter);
            return await ShowingInventories.CountAsync();
        }

        public async Task<List<ShowingInventory>> List(ShowingInventoryFilter filter)
        {
            if (filter == null) return new List<ShowingInventory>();
            IQueryable<ShowingInventoryDAO> ShowingInventoryDAOs = DataContext.ShowingInventory.AsNoTracking();
            ShowingInventoryDAOs = DynamicFilter(ShowingInventoryDAOs, filter);
            ShowingInventoryDAOs = DynamicOrder(ShowingInventoryDAOs, filter);
            List<ShowingInventory> ShowingInventories = await DynamicSelect(ShowingInventoryDAOs, filter);
            return ShowingInventories;
        }

        public async Task<List<ShowingInventory>> List(List<long> Ids)
        {
            List<ShowingInventory> ShowingInventories = await DataContext.ShowingInventory.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ShowingInventory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                ShowingWarehouseId = x.ShowingWarehouseId,
                ShowingItemId = x.ShowingItemId,
                SaleStock = x.SaleStock,
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
                ShowingItem = x.ShowingItem == null ? null : new ShowingItem
                {
                    Id = x.ShowingItem.Id,
                    Code = x.ShowingItem.Code,
                    Name = x.ShowingItem.Name,
                    ShowingCategoryId = x.ShowingItem.ShowingCategoryId,
                    UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                    SalePrice = x.ShowingItem.SalePrice,
                    Description = x.ShowingItem.Description,
                    StatusId = x.ShowingItem.StatusId,
                    Used = x.ShowingItem.Used,
                    RowId = x.ShowingItem.RowId,
                },
                ShowingWarehouse = x.ShowingWarehouse == null ? null : new ShowingWarehouse
                {
                    Id = x.ShowingWarehouse.Id,
                    Code = x.ShowingWarehouse.Code,
                    Name = x.ShowingWarehouse.Name,
                    Address = x.ShowingWarehouse.Address,
                    OrganizationId = x.ShowingWarehouse.OrganizationId,
                    ProvinceId = x.ShowingWarehouse.ProvinceId,
                    DistrictId = x.ShowingWarehouse.DistrictId,
                    WardId = x.ShowingWarehouse.WardId,
                    StatusId = x.ShowingWarehouse.StatusId,
                    RowId = x.ShowingWarehouse.RowId,
                },
            }).ToListAsync();
            

            return ShowingInventories;
        }

        public async Task<ShowingInventory> Get(long Id)
        {
            ShowingInventory ShowingInventory = await DataContext.ShowingInventory.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ShowingInventory()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                ShowingWarehouseId = x.ShowingWarehouseId,
                ShowingItemId = x.ShowingItemId,
                SaleStock = x.SaleStock,
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
                ShowingItem = x.ShowingItem == null ? null : new ShowingItem
                {
                    Id = x.ShowingItem.Id,
                    Code = x.ShowingItem.Code,
                    Name = x.ShowingItem.Name,
                    ShowingCategoryId = x.ShowingItem.ShowingCategoryId,
                    UnitOfMeasureId = x.ShowingItem.UnitOfMeasureId,
                    SalePrice = x.ShowingItem.SalePrice,
                    Description = x.ShowingItem.Description,
                    StatusId = x.ShowingItem.StatusId,
                    Used = x.ShowingItem.Used,
                    RowId = x.ShowingItem.RowId,
                },
                ShowingWarehouse = x.ShowingWarehouse == null ? null : new ShowingWarehouse
                {
                    Id = x.ShowingWarehouse.Id,
                    Code = x.ShowingWarehouse.Code,
                    Name = x.ShowingWarehouse.Name,
                    Address = x.ShowingWarehouse.Address,
                    OrganizationId = x.ShowingWarehouse.OrganizationId,
                    ProvinceId = x.ShowingWarehouse.ProvinceId,
                    DistrictId = x.ShowingWarehouse.DistrictId,
                    WardId = x.ShowingWarehouse.WardId,
                    StatusId = x.ShowingWarehouse.StatusId,
                    RowId = x.ShowingWarehouse.RowId,
                },
            }).FirstOrDefaultAsync();

            if (ShowingInventory == null)
                return null;

            return ShowingInventory;
        }
        public async Task<bool> Create(ShowingInventory ShowingInventory)
        {
            ShowingInventoryDAO ShowingInventoryDAO = new ShowingInventoryDAO();
            ShowingInventoryDAO.Id = ShowingInventory.Id;
            ShowingInventoryDAO.ShowingWarehouseId = ShowingInventory.ShowingWarehouseId;
            ShowingInventoryDAO.ShowingItemId = ShowingInventory.ShowingItemId;
            ShowingInventoryDAO.SaleStock = ShowingInventory.SaleStock;
            ShowingInventoryDAO.AccountingStock = ShowingInventory.AccountingStock;
            ShowingInventoryDAO.AppUserId = ShowingInventory.AppUserId;
            ShowingInventoryDAO.CreatedAt = StaticParams.DateTimeNow;
            ShowingInventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            ShowingInventoryDAO.RowId = Guid.NewGuid();
            DataContext.ShowingInventory.Add(ShowingInventoryDAO);
            await DataContext.SaveChangesAsync();
            ShowingInventory.Id = ShowingInventoryDAO.Id;
            await SaveReference(ShowingInventory);
            return true;
        }

        public async Task<bool> Update(ShowingInventory ShowingInventory)
        {
            ShowingInventoryDAO ShowingInventoryDAO = DataContext.ShowingInventory.Where(x => x.Id == ShowingInventory.Id).FirstOrDefault();
            if (ShowingInventoryDAO == null)
                return false;
            ShowingInventoryDAO.Id = ShowingInventory.Id;
            ShowingInventoryDAO.ShowingWarehouseId = ShowingInventory.ShowingWarehouseId;
            ShowingInventoryDAO.ShowingItemId = ShowingInventory.ShowingItemId;
            ShowingInventoryDAO.SaleStock = ShowingInventory.SaleStock;
            ShowingInventoryDAO.AccountingStock = ShowingInventory.AccountingStock;
            ShowingInventoryDAO.AppUserId = ShowingInventory.AppUserId;
            ShowingInventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ShowingInventory);
            return true;
        }

        public async Task<bool> Delete(ShowingInventory ShowingInventory)
        {
            await DataContext.ShowingInventory.Where(x => x.Id == ShowingInventory.Id).UpdateFromQueryAsync(x => new ShowingInventoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ShowingInventory> ShowingInventories)
        {
            List<ShowingInventoryDAO> ShowingInventoryDAOs = new List<ShowingInventoryDAO>();
            foreach (ShowingInventory ShowingInventory in ShowingInventories)
            {
                ShowingInventoryDAO ShowingInventoryDAO = new ShowingInventoryDAO();
                ShowingInventoryDAO.Id = ShowingInventory.Id;
                ShowingInventoryDAO.ShowingWarehouseId = ShowingInventory.ShowingWarehouseId;
                ShowingInventoryDAO.ShowingItemId = ShowingInventory.ShowingItemId;
                ShowingInventoryDAO.SaleStock = ShowingInventory.SaleStock;
                ShowingInventoryDAO.AccountingStock = ShowingInventory.AccountingStock;
                ShowingInventoryDAO.AppUserId = ShowingInventory.AppUserId;
                ShowingInventoryDAO.RowId = ShowingInventory.RowId;
                ShowingInventoryDAO.CreatedAt = StaticParams.DateTimeNow;
                ShowingInventoryDAO.UpdatedAt = StaticParams.DateTimeNow;
                ShowingInventoryDAOs.Add(ShowingInventoryDAO);
            }   
            await DataContext.BulkMergeAsync(ShowingInventoryDAOs);

            var Ids = ShowingInventoryDAOs.Select(x => x.Id).ToList();
            List<ShowingInventoryHistoryDAO> ShowingInventoryHistoryDAOs = await DataContext.ShowingInventoryHistory
                .Where(x => Ids.Contains(x.ShowingInventoryId))
                .ToListAsync();

            foreach (ShowingInventory ShowingInventory in ShowingInventories)
            {
                ShowingInventory.Id = ShowingInventoryDAOs.Where(x => x.RowId == ShowingInventory.RowId).Select(x => x.Id).FirstOrDefault();
                if(ShowingInventory.ShowingInventoryHistories != null)
                {
                    foreach (var ShowingInventoryHistory in ShowingInventory.ShowingInventoryHistories)
                    {
                        ShowingInventoryHistoryDAO ShowingInventoryHistoryDAO = ShowingInventoryHistoryDAOs
                            .Where(x => x.Id == ShowingInventoryHistory.Id && x.Id != 0)
                            .FirstOrDefault();
                        if (ShowingInventoryHistoryDAO == null)
                        {
                            ShowingInventoryHistoryDAO = new ShowingInventoryHistoryDAO
                            {
                                ShowingInventoryId = ShowingInventory.Id,
                                AppUserId = ShowingInventoryHistory.AppUserId,
                                SaleStock = ShowingInventoryHistory.SaleStock,
                                AccountingStock = ShowingInventoryHistory.AccountingStock,
                                OldAccountingStock = ShowingInventoryHistory.OldAccountingStock,
                                OldSaleStock = ShowingInventoryHistory.OldSaleStock,
                                CreatedAt = StaticParams.DateTimeNow,
                                UpdatedAt = StaticParams.DateTimeNow,
                                DeletedAt = null,
                            };
                            ShowingInventoryHistoryDAOs.Add(ShowingInventoryHistoryDAO);
                        }
                    }
                }
            }
            await DataContext.BulkMergeAsync(ShowingInventoryHistoryDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ShowingInventory> ShowingInventories)
        {
            List<long> Ids = ShowingInventories.Select(x => x.Id).ToList();
            await DataContext.ShowingInventory
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ShowingInventoryDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ShowingInventory ShowingInventory)
        {
            List<ShowingInventoryHistoryDAO> ShowingInventoryHistoryDAOs = await DataContext.ShowingInventoryHistory
                .Where(x => x.ShowingInventoryId == ShowingInventory.Id)
                .ToListAsync();
            if (ShowingInventory.ShowingInventoryHistories != null)
            {
                foreach (var ShowingInventoryHistory in ShowingInventory.ShowingInventoryHistories)
                {
                    ShowingInventoryHistoryDAO ShowingInventoryHistoryDAO = ShowingInventoryHistoryDAOs
                        .Where(x => x.Id == ShowingInventoryHistory.Id && x.Id != 0)
                        .FirstOrDefault();
                    if (ShowingInventoryHistoryDAO == null)
                    {
                        ShowingInventoryHistoryDAO = new ShowingInventoryHistoryDAO
                        {
                            ShowingInventoryId = ShowingInventory.Id,
                            AppUserId = ShowingInventoryHistory.AppUserId,
                            SaleStock = ShowingInventoryHistory.SaleStock,
                            AccountingStock = ShowingInventoryHistory.AccountingStock,
                            OldAccountingStock = ShowingInventoryHistory.OldAccountingStock,
                            OldSaleStock = ShowingInventoryHistory.OldSaleStock,
                            CreatedAt = StaticParams.DateTimeNow,
                            UpdatedAt = StaticParams.DateTimeNow,
                            DeletedAt = null,
                        };
                        ShowingInventoryHistoryDAOs.Add(ShowingInventoryHistoryDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(ShowingInventoryHistoryDAOs);
        }
        
    }
}
