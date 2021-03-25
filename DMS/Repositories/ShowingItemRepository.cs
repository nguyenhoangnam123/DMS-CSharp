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
    public interface IShowingItemRepository
    {
        Task<int> Count(ShowingItemFilter ShowingItemFilter);
        Task<List<ShowingItem>> List(ShowingItemFilter ShowingItemFilter);
        Task<List<ShowingItem>> List(List<long> Ids);
        Task<ShowingItem> Get(long Id);
        Task<bool> Create(ShowingItem ShowingItem);
        Task<bool> Update(ShowingItem ShowingItem);
        Task<bool> Delete(ShowingItem ShowingItem);
        Task<bool> BulkMerge(List<ShowingItem> ShowingItems);
        Task<bool> BulkDelete(List<ShowingItem> ShowingItems);
    }
    public class ShowingItemRepository : IShowingItemRepository
    {
        private DataContext DataContext;
        public ShowingItemRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ShowingItemDAO> DynamicFilter(IQueryable<ShowingItemDAO> query, ShowingItemFilter filter)
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
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ShowingCategoryId != null && filter.ShowingCategoryId.HasValue)
                query = query.Where(q => q.ShowingCategoryId, filter.ShowingCategoryId);
            if (filter.UnitOfMeasureId != null && filter.UnitOfMeasureId.HasValue)
                query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
            if (filter.SalePrice != null && filter.SalePrice.HasValue)
                query = query.Where(q => q.SalePrice, filter.SalePrice);
            if (filter.Desception != null && filter.Desception.HasValue)
                query = query.Where(q => q.Desception, filter.Desception);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);
            if (filter.Search != null)
            {
                List<string> Tokens = filter.Search.Split(" ").Select(x => x.ToLower()).ToList();
                var queryForCode = query;
                var queryForName = query;
                foreach (string Token in Tokens)
                {
                    if (string.IsNullOrWhiteSpace(Token))
                        continue;
                    queryForCode = queryForCode.Where(x => x.Code.ToLower().Contains(Token));
                    queryForName = queryForName.Where(x => x.Name.ToLower().Contains(Token));
                }
                query = queryForCode.Union(queryForName);
                query = query.Distinct();
            }
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ShowingItemDAO> OrFilter(IQueryable<ShowingItemDAO> query, ShowingItemFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ShowingItemDAO> initQuery = query.Where(q => false);
            foreach (ShowingItemFilter ShowingItemFilter in filter.OrFilter)
            {
                IQueryable<ShowingItemDAO> queryable = query;
                if (ShowingItemFilter.Id != null && ShowingItemFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (ShowingItemFilter.Code != null && ShowingItemFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (ShowingItemFilter.Name != null && ShowingItemFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (ShowingItemFilter.ShowingCategoryId != null && ShowingItemFilter.ShowingCategoryId.HasValue)
                    queryable = queryable.Where(q => q.ShowingCategoryId, filter.ShowingCategoryId);
                if (ShowingItemFilter.UnitOfMeasureId != null && ShowingItemFilter.UnitOfMeasureId.HasValue)
                    queryable = queryable.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
                if (ShowingItemFilter.SalePrice != null && ShowingItemFilter.SalePrice.HasValue)
                    queryable = queryable.Where(q => q.SalePrice, filter.SalePrice);
                if (ShowingItemFilter.Desception != null && ShowingItemFilter.Desception.HasValue)
                    queryable = queryable.Where(q => q.Desception, filter.Desception);
                if (ShowingItemFilter.StatusId != null && ShowingItemFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (ShowingItemFilter.RowId != null && ShowingItemFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ShowingItemDAO> DynamicOrder(IQueryable<ShowingItemDAO> query, ShowingItemFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ShowingItemOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ShowingItemOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ShowingItemOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ShowingItemOrder.ShowingCategory:
                            query = query.OrderBy(q => q.ShowingCategoryId);
                            break;
                        case ShowingItemOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case ShowingItemOrder.SalePrice:
                            query = query.OrderBy(q => q.SalePrice);
                            break;
                        case ShowingItemOrder.Desception:
                            query = query.OrderBy(q => q.Desception);
                            break;
                        case ShowingItemOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ShowingItemOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                        case ShowingItemOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ShowingItemOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ShowingItemOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ShowingItemOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ShowingItemOrder.ShowingCategory:
                            query = query.OrderByDescending(q => q.ShowingCategoryId);
                            break;
                        case ShowingItemOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case ShowingItemOrder.SalePrice:
                            query = query.OrderByDescending(q => q.SalePrice);
                            break;
                        case ShowingItemOrder.Desception:
                            query = query.OrderByDescending(q => q.Desception);
                            break;
                        case ShowingItemOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ShowingItemOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                        case ShowingItemOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ShowingItem>> DynamicSelect(IQueryable<ShowingItemDAO> query, ShowingItemFilter filter)
        {
            List<ShowingItem> ShowingItems = await query.Select(q => new ShowingItem()
            {
                Id = filter.Selects.Contains(ShowingItemSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ShowingItemSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ShowingItemSelect.Name) ? q.Name : default(string),
                ShowingCategoryId = filter.Selects.Contains(ShowingItemSelect.ShowingCategory) ? q.ShowingCategoryId : default(long),
                UnitOfMeasureId = filter.Selects.Contains(ShowingItemSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                SalePrice = filter.Selects.Contains(ShowingItemSelect.SalePrice) ? q.SalePrice : default(decimal),
                Desception = filter.Selects.Contains(ShowingItemSelect.Desception) ? q.Desception : default(string),
                StatusId = filter.Selects.Contains(ShowingItemSelect.Status) ? q.StatusId : default(long),
                Used = filter.Selects.Contains(ShowingItemSelect.Used) ? q.Used : default(bool),
                RowId = filter.Selects.Contains(ShowingItemSelect.Row) ? q.RowId : default(Guid),
                ShowingCategory = filter.Selects.Contains(ShowingItemSelect.ShowingCategory) && q.ShowingCategory != null ? new ShowingCategory
                {
                    Id = q.ShowingCategory.Id,
                    Code = q.ShowingCategory.Code,
                    Name = q.ShowingCategory.Name,
                    ParentId = q.ShowingCategory.ParentId,
                    Path = q.ShowingCategory.Path,
                    Level = q.ShowingCategory.Level,
                    StatusId = q.ShowingCategory.StatusId,
                    ImageId = q.ShowingCategory.ImageId,
                    RowId = q.ShowingCategory.RowId,
                    Used = q.ShowingCategory.Used,
                } : null,
                Status = filter.Selects.Contains(ShowingItemSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                UnitOfMeasure = filter.Selects.Contains(ShowingItemSelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                    Used = q.UnitOfMeasure.Used,
                    RowId = q.UnitOfMeasure.RowId,
                } : null,
            }).ToListAsync();
            return ShowingItems;
        }

        public async Task<int> Count(ShowingItemFilter filter)
        {
            IQueryable<ShowingItemDAO> ShowingItems = DataContext.ShowingItem.AsNoTracking();
            ShowingItems = DynamicFilter(ShowingItems, filter);
            return await ShowingItems.CountAsync();
        }

        public async Task<List<ShowingItem>> List(ShowingItemFilter filter)
        {
            if (filter == null) return new List<ShowingItem>();
            IQueryable<ShowingItemDAO> ShowingItemDAOs = DataContext.ShowingItem.AsNoTracking();
            ShowingItemDAOs = DynamicFilter(ShowingItemDAOs, filter);
            ShowingItemDAOs = DynamicOrder(ShowingItemDAOs, filter);
            List<ShowingItem> ShowingItems = await DynamicSelect(ShowingItemDAOs, filter);
            return ShowingItems;
        }

        public async Task<List<ShowingItem>> List(List<long> Ids)
        {
            List<ShowingItem> ShowingItems = await DataContext.ShowingItem.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ShowingItem()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ShowingCategoryId = x.ShowingCategoryId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                SalePrice = x.SalePrice,
                Desception = x.Desception,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
                ShowingCategory = x.ShowingCategory == null ? null : new ShowingCategory
                {
                    Id = x.ShowingCategory.Id,
                    Code = x.ShowingCategory.Code,
                    Name = x.ShowingCategory.Name,
                    ParentId = x.ShowingCategory.ParentId,
                    Path = x.ShowingCategory.Path,
                    Level = x.ShowingCategory.Level,
                    StatusId = x.ShowingCategory.StatusId,
                    ImageId = x.ShowingCategory.ImageId,
                    RowId = x.ShowingCategory.RowId,
                    Used = x.ShowingCategory.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                    Used = x.UnitOfMeasure.Used,
                    RowId = x.UnitOfMeasure.RowId,
                },
            }).ToListAsync();
            

            return ShowingItems;
        }

        public async Task<ShowingItem> Get(long Id)
        {
            ShowingItem ShowingItem = await DataContext.ShowingItem.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ShowingItem()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ShowingCategoryId = x.ShowingCategoryId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                SalePrice = x.SalePrice,
                Desception = x.Desception,
                StatusId = x.StatusId,
                Used = x.Used,
                RowId = x.RowId,
                ShowingCategory = x.ShowingCategory == null ? null : new ShowingCategory
                {
                    Id = x.ShowingCategory.Id,
                    Code = x.ShowingCategory.Code,
                    Name = x.ShowingCategory.Name,
                    ParentId = x.ShowingCategory.ParentId,
                    Path = x.ShowingCategory.Path,
                    Level = x.ShowingCategory.Level,
                    StatusId = x.ShowingCategory.StatusId,
                    ImageId = x.ShowingCategory.ImageId,
                    RowId = x.ShowingCategory.RowId,
                    Used = x.ShowingCategory.Used,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                    Used = x.UnitOfMeasure.Used,
                    RowId = x.UnitOfMeasure.RowId,
                },
            }).FirstOrDefaultAsync();

            if (ShowingItem == null)
                return null;

            return ShowingItem;
        }
        public async Task<bool> Create(ShowingItem ShowingItem)
        {
            ShowingItemDAO ShowingItemDAO = new ShowingItemDAO();
            ShowingItemDAO.Id = ShowingItem.Id;
            ShowingItemDAO.Code = ShowingItem.Code;
            ShowingItemDAO.Name = ShowingItem.Name;
            ShowingItemDAO.ShowingCategoryId = ShowingItem.ShowingCategoryId;
            ShowingItemDAO.UnitOfMeasureId = ShowingItem.UnitOfMeasureId;
            ShowingItemDAO.SalePrice = ShowingItem.SalePrice;
            ShowingItemDAO.Desception = ShowingItem.Desception;
            ShowingItemDAO.StatusId = ShowingItem.StatusId;
            ShowingItemDAO.Used = ShowingItem.Used;
            ShowingItemDAO.RowId = ShowingItem.RowId;
            ShowingItemDAO.CreatedAt = StaticParams.DateTimeNow;
            ShowingItemDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ShowingItem.Add(ShowingItemDAO);
            await DataContext.SaveChangesAsync();
            ShowingItem.Id = ShowingItemDAO.Id;
            await SaveReference(ShowingItem);
            return true;
        }

        public async Task<bool> Update(ShowingItem ShowingItem)
        {
            ShowingItemDAO ShowingItemDAO = DataContext.ShowingItem.Where(x => x.Id == ShowingItem.Id).FirstOrDefault();
            if (ShowingItemDAO == null)
                return false;
            ShowingItemDAO.Id = ShowingItem.Id;
            ShowingItemDAO.Code = ShowingItem.Code;
            ShowingItemDAO.Name = ShowingItem.Name;
            ShowingItemDAO.ShowingCategoryId = ShowingItem.ShowingCategoryId;
            ShowingItemDAO.UnitOfMeasureId = ShowingItem.UnitOfMeasureId;
            ShowingItemDAO.SalePrice = ShowingItem.SalePrice;
            ShowingItemDAO.Desception = ShowingItem.Desception;
            ShowingItemDAO.StatusId = ShowingItem.StatusId;
            ShowingItemDAO.Used = ShowingItem.Used;
            ShowingItemDAO.RowId = ShowingItem.RowId;
            ShowingItemDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ShowingItem);
            return true;
        }

        public async Task<bool> Delete(ShowingItem ShowingItem)
        {
            await DataContext.ShowingItem.Where(x => x.Id == ShowingItem.Id).UpdateFromQueryAsync(x => new ShowingItemDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ShowingItem> ShowingItems)
        {
            List<ShowingItemDAO> ShowingItemDAOs = new List<ShowingItemDAO>();
            foreach (ShowingItem ShowingItem in ShowingItems)
            {
                ShowingItemDAO ShowingItemDAO = new ShowingItemDAO();
                ShowingItemDAO.Id = ShowingItem.Id;
                ShowingItemDAO.Code = ShowingItem.Code;
                ShowingItemDAO.Name = ShowingItem.Name;
                ShowingItemDAO.ShowingCategoryId = ShowingItem.ShowingCategoryId;
                ShowingItemDAO.UnitOfMeasureId = ShowingItem.UnitOfMeasureId;
                ShowingItemDAO.SalePrice = ShowingItem.SalePrice;
                ShowingItemDAO.Desception = ShowingItem.Desception;
                ShowingItemDAO.StatusId = ShowingItem.StatusId;
                ShowingItemDAO.Used = ShowingItem.Used;
                ShowingItemDAO.RowId = ShowingItem.RowId;
                ShowingItemDAO.CreatedAt = StaticParams.DateTimeNow;
                ShowingItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                ShowingItemDAOs.Add(ShowingItemDAO);
            }
            await DataContext.BulkMergeAsync(ShowingItemDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ShowingItem> ShowingItems)
        {
            List<long> Ids = ShowingItems.Select(x => x.Id).ToList();
            await DataContext.ShowingItem
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ShowingItemDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ShowingItem ShowingItem)
        {
        }
        
    }
}
