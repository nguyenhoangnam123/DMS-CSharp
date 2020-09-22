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
    public interface IPromotionStoreTypeRepository
    {
        Task<int> Count(PromotionStoreTypeFilter PromotionStoreTypeFilter);
        Task<List<PromotionStoreType>> List(PromotionStoreTypeFilter PromotionStoreTypeFilter);
        Task<PromotionStoreType> Get(long Id);
        Task<bool> Create(PromotionStoreType PromotionStoreType);
        Task<bool> Update(PromotionStoreType PromotionStoreType);
        Task<bool> Delete(PromotionStoreType PromotionStoreType);
        Task<bool> BulkMerge(List<PromotionStoreType> PromotionStoreTypes);
        Task<bool> BulkDelete(List<PromotionStoreType> PromotionStoreTypes);
    }
    public class PromotionStoreTypeRepository : IPromotionStoreTypeRepository
    {
        private DataContext DataContext;
        public PromotionStoreTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PromotionStoreTypeDAO> DynamicFilter(IQueryable<PromotionStoreTypeDAO> query, PromotionStoreTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.PromotionId != null)
                query = query.Where(q => q.PromotionId, filter.PromotionId);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            if (filter.FromValue != null)
                query = query.Where(q => q.FromValue, filter.FromValue);
            if (filter.ToValue != null)
                query = query.Where(q => q.ToValue, filter.ToValue);
            if (filter.PromotionDiscountTypeId != null)
                query = query.Where(q => q.PromotionDiscountTypeId, filter.PromotionDiscountTypeId);
            if (filter.DiscountPercentage != null)
                query = query.Where(q => q.DiscountPercentage.HasValue).Where(q => q.DiscountPercentage, filter.DiscountPercentage);
            if (filter.DiscountValue != null)
                query = query.Where(q => q.DiscountValue.HasValue).Where(q => q.DiscountValue, filter.DiscountValue);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PromotionStoreTypeDAO> OrFilter(IQueryable<PromotionStoreTypeDAO> query, PromotionStoreTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PromotionStoreTypeDAO> initQuery = query.Where(q => false);
            foreach (PromotionStoreTypeFilter PromotionStoreTypeFilter in filter.OrFilter)
            {
                IQueryable<PromotionStoreTypeDAO> queryable = query;
                if (PromotionStoreTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PromotionStoreTypeFilter.Id);
                if (PromotionStoreTypeFilter.PromotionId != null)
                    queryable = queryable.Where(q => q.PromotionId, PromotionStoreTypeFilter.PromotionId);
                if (PromotionStoreTypeFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, PromotionStoreTypeFilter.Note);
                if (PromotionStoreTypeFilter.FromValue != null)
                    queryable = queryable.Where(q => q.FromValue, PromotionStoreTypeFilter.FromValue);
                if (PromotionStoreTypeFilter.ToValue != null)
                    queryable = queryable.Where(q => q.ToValue, PromotionStoreTypeFilter.ToValue);
                if (PromotionStoreTypeFilter.PromotionDiscountTypeId != null)
                    queryable = queryable.Where(q => q.PromotionDiscountTypeId, PromotionStoreTypeFilter.PromotionDiscountTypeId);
                if (PromotionStoreTypeFilter.DiscountPercentage != null)
                    queryable = queryable.Where(q => q.DiscountPercentage.HasValue).Where(q => q.DiscountPercentage, PromotionStoreTypeFilter.DiscountPercentage);
                if (PromotionStoreTypeFilter.DiscountValue != null)
                    queryable = queryable.Where(q => q.DiscountValue.HasValue).Where(q => q.DiscountValue, PromotionStoreTypeFilter.DiscountValue);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PromotionStoreTypeDAO> DynamicOrder(IQueryable<PromotionStoreTypeDAO> query, PromotionStoreTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PromotionStoreTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PromotionStoreTypeOrder.Promotion:
                            query = query.OrderBy(q => q.PromotionId);
                            break;
                        case PromotionStoreTypeOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case PromotionStoreTypeOrder.FromValue:
                            query = query.OrderBy(q => q.FromValue);
                            break;
                        case PromotionStoreTypeOrder.ToValue:
                            query = query.OrderBy(q => q.ToValue);
                            break;
                        case PromotionStoreTypeOrder.PromotionDiscountType:
                            query = query.OrderBy(q => q.PromotionDiscountTypeId);
                            break;
                        case PromotionStoreTypeOrder.DiscountPercentage:
                            query = query.OrderBy(q => q.DiscountPercentage);
                            break;
                        case PromotionStoreTypeOrder.DiscountValue:
                            query = query.OrderBy(q => q.DiscountValue);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PromotionStoreTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PromotionStoreTypeOrder.Promotion:
                            query = query.OrderByDescending(q => q.PromotionId);
                            break;
                        case PromotionStoreTypeOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case PromotionStoreTypeOrder.FromValue:
                            query = query.OrderByDescending(q => q.FromValue);
                            break;
                        case PromotionStoreTypeOrder.ToValue:
                            query = query.OrderByDescending(q => q.ToValue);
                            break;
                        case PromotionStoreTypeOrder.PromotionDiscountType:
                            query = query.OrderByDescending(q => q.PromotionDiscountTypeId);
                            break;
                        case PromotionStoreTypeOrder.DiscountPercentage:
                            query = query.OrderByDescending(q => q.DiscountPercentage);
                            break;
                        case PromotionStoreTypeOrder.DiscountValue:
                            query = query.OrderByDescending(q => q.DiscountValue);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PromotionStoreType>> DynamicSelect(IQueryable<PromotionStoreTypeDAO> query, PromotionStoreTypeFilter filter)
        {
            List<PromotionStoreType> PromotionStoreTypes = await query.Select(q => new PromotionStoreType()
            {
                Id = filter.Selects.Contains(PromotionStoreTypeSelect.Id) ? q.Id : default(long),
                PromotionId = filter.Selects.Contains(PromotionStoreTypeSelect.Promotion) ? q.PromotionId : default(long),
                Note = filter.Selects.Contains(PromotionStoreTypeSelect.Note) ? q.Note : default(string),
                FromValue = filter.Selects.Contains(PromotionStoreTypeSelect.FromValue) ? q.FromValue : default(decimal),
                ToValue = filter.Selects.Contains(PromotionStoreTypeSelect.ToValue) ? q.ToValue : default(decimal),
                PromotionDiscountTypeId = filter.Selects.Contains(PromotionStoreTypeSelect.PromotionDiscountType) ? q.PromotionDiscountTypeId : default(long),
                DiscountPercentage = filter.Selects.Contains(PromotionStoreTypeSelect.DiscountPercentage) ? q.DiscountPercentage : default(decimal?),
                DiscountValue = filter.Selects.Contains(PromotionStoreTypeSelect.DiscountValue) ? q.DiscountValue : default(decimal?),
                Promotion = filter.Selects.Contains(PromotionStoreTypeSelect.Promotion) && q.Promotion != null ? new Promotion
                {
                    Id = q.Promotion.Id,
                    Code = q.Promotion.Code,
                    Name = q.Promotion.Name,
                    StartDate = q.Promotion.StartDate,
                    EndDate = q.Promotion.EndDate,
                    OrganizationId = q.Promotion.OrganizationId,
                    PromotionTypeId = q.Promotion.PromotionTypeId,
                    Note = q.Promotion.Note,
                    Priority = q.Promotion.Priority,
                    StatusId = q.Promotion.StatusId,
                } : null,
                PromotionDiscountType = filter.Selects.Contains(PromotionStoreTypeSelect.PromotionDiscountType) && q.PromotionDiscountType != null ? new PromotionDiscountType
                {
                    Id = q.PromotionDiscountType.Id,
                    Code = q.PromotionDiscountType.Code,
                    Name = q.PromotionDiscountType.Name,
                } : null,
            }).ToListAsync();
            return PromotionStoreTypes;
        }

        public async Task<int> Count(PromotionStoreTypeFilter filter)
        {
            IQueryable<PromotionStoreTypeDAO> PromotionStoreTypes = DataContext.PromotionStoreType.AsNoTracking();
            PromotionStoreTypes = DynamicFilter(PromotionStoreTypes, filter);
            return await PromotionStoreTypes.CountAsync();
        }

        public async Task<List<PromotionStoreType>> List(PromotionStoreTypeFilter filter)
        {
            if (filter == null) return new List<PromotionStoreType>();
            IQueryable<PromotionStoreTypeDAO> PromotionStoreTypeDAOs = DataContext.PromotionStoreType.AsNoTracking();
            PromotionStoreTypeDAOs = DynamicFilter(PromotionStoreTypeDAOs, filter);
            PromotionStoreTypeDAOs = DynamicOrder(PromotionStoreTypeDAOs, filter);
            List<PromotionStoreType> PromotionStoreTypes = await DynamicSelect(PromotionStoreTypeDAOs, filter);
            return PromotionStoreTypes;
        }

        public async Task<PromotionStoreType> Get(long Id)
        {
            PromotionStoreType PromotionStoreType = await DataContext.PromotionStoreType.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PromotionStoreType()
            {
                Id = x.Id,
                PromotionId = x.PromotionId,
                Note = x.Note,
                FromValue = x.FromValue,
                ToValue = x.ToValue,
                PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                DiscountPercentage = x.DiscountPercentage,
                DiscountValue = x.DiscountValue,
                Promotion = x.Promotion == null ? null : new Promotion
                {
                    Id = x.Promotion.Id,
                    Code = x.Promotion.Code,
                    Name = x.Promotion.Name,
                    StartDate = x.Promotion.StartDate,
                    EndDate = x.Promotion.EndDate,
                    OrganizationId = x.Promotion.OrganizationId,
                    PromotionTypeId = x.Promotion.PromotionTypeId,
                    Note = x.Promotion.Note,
                    Priority = x.Promotion.Priority,
                    StatusId = x.Promotion.StatusId,
                },
                PromotionDiscountType = x.PromotionDiscountType == null ? null : new PromotionDiscountType
                {
                    Id = x.PromotionDiscountType.Id,
                    Code = x.PromotionDiscountType.Code,
                    Name = x.PromotionDiscountType.Name,
                },
            }).FirstOrDefaultAsync();

            if (PromotionStoreType == null)
                return null;
            PromotionStoreType.PromotionStoreTypeItemMappings = await DataContext.PromotionStoreTypeItemMapping.AsNoTracking()
                .Where(x => x.PromotionStoreTypeId == PromotionStoreType.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new PromotionStoreTypeItemMapping
                {
                    PromotionStoreTypeId = x.PromotionStoreTypeId,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                        Used = x.Item.Used,
                    },
                }).ToListAsync();

            return PromotionStoreType;
        }
        public async Task<bool> Create(PromotionStoreType PromotionStoreType)
        {
            PromotionStoreTypeDAO PromotionStoreTypeDAO = new PromotionStoreTypeDAO();
            PromotionStoreTypeDAO.Id = PromotionStoreType.Id;
            PromotionStoreTypeDAO.PromotionId = PromotionStoreType.PromotionId;
            PromotionStoreTypeDAO.Note = PromotionStoreType.Note;
            PromotionStoreTypeDAO.FromValue = PromotionStoreType.FromValue;
            PromotionStoreTypeDAO.ToValue = PromotionStoreType.ToValue;
            PromotionStoreTypeDAO.PromotionDiscountTypeId = PromotionStoreType.PromotionDiscountTypeId;
            PromotionStoreTypeDAO.DiscountPercentage = PromotionStoreType.DiscountPercentage;
            PromotionStoreTypeDAO.DiscountValue = PromotionStoreType.DiscountValue;
            DataContext.PromotionStoreType.Add(PromotionStoreTypeDAO);
            await DataContext.SaveChangesAsync();
            PromotionStoreType.Id = PromotionStoreTypeDAO.Id;
            await SaveReference(PromotionStoreType);
            return true;
        }

        public async Task<bool> Update(PromotionStoreType PromotionStoreType)
        {
            PromotionStoreTypeDAO PromotionStoreTypeDAO = DataContext.PromotionStoreType.Where(x => x.Id == PromotionStoreType.Id).FirstOrDefault();
            if (PromotionStoreTypeDAO == null)
                return false;
            PromotionStoreTypeDAO.Id = PromotionStoreType.Id;
            PromotionStoreTypeDAO.PromotionId = PromotionStoreType.PromotionId;
            PromotionStoreTypeDAO.Note = PromotionStoreType.Note;
            PromotionStoreTypeDAO.FromValue = PromotionStoreType.FromValue;
            PromotionStoreTypeDAO.ToValue = PromotionStoreType.ToValue;
            PromotionStoreTypeDAO.PromotionDiscountTypeId = PromotionStoreType.PromotionDiscountTypeId;
            PromotionStoreTypeDAO.DiscountPercentage = PromotionStoreType.DiscountPercentage;
            PromotionStoreTypeDAO.DiscountValue = PromotionStoreType.DiscountValue;
            await DataContext.SaveChangesAsync();
            await SaveReference(PromotionStoreType);
            return true;
        }

        public async Task<bool> Delete(PromotionStoreType PromotionStoreType)
        {
            await DataContext.PromotionStoreType.Where(x => x.Id == PromotionStoreType.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PromotionStoreType> PromotionStoreTypes)
        {
            List<PromotionStoreTypeDAO> PromotionStoreTypeDAOs = new List<PromotionStoreTypeDAO>();
            foreach (PromotionStoreType PromotionStoreType in PromotionStoreTypes)
            {
                PromotionStoreTypeDAO PromotionStoreTypeDAO = new PromotionStoreTypeDAO();
                PromotionStoreTypeDAO.Id = PromotionStoreType.Id;
                PromotionStoreTypeDAO.PromotionId = PromotionStoreType.PromotionId;
                PromotionStoreTypeDAO.Note = PromotionStoreType.Note;
                PromotionStoreTypeDAO.FromValue = PromotionStoreType.FromValue;
                PromotionStoreTypeDAO.ToValue = PromotionStoreType.ToValue;
                PromotionStoreTypeDAO.PromotionDiscountTypeId = PromotionStoreType.PromotionDiscountTypeId;
                PromotionStoreTypeDAO.DiscountPercentage = PromotionStoreType.DiscountPercentage;
                PromotionStoreTypeDAO.DiscountValue = PromotionStoreType.DiscountValue;
                PromotionStoreTypeDAOs.Add(PromotionStoreTypeDAO);
            }
            await DataContext.BulkMergeAsync(PromotionStoreTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PromotionStoreType> PromotionStoreTypes)
        {
            List<long> Ids = PromotionStoreTypes.Select(x => x.Id).ToList();
            await DataContext.PromotionStoreType
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PromotionStoreType PromotionStoreType)
        {
            await DataContext.PromotionStoreTypeItemMapping
                .Where(x => x.PromotionStoreTypeId == PromotionStoreType.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreTypeItemMappingDAO> PromotionStoreTypeItemMappingDAOs = new List<PromotionStoreTypeItemMappingDAO>();
            if (PromotionStoreType.PromotionStoreTypeItemMappings != null)
            {
                foreach (PromotionStoreTypeItemMapping PromotionStoreTypeItemMapping in PromotionStoreType.PromotionStoreTypeItemMappings)
                {
                    PromotionStoreTypeItemMappingDAO PromotionStoreTypeItemMappingDAO = new PromotionStoreTypeItemMappingDAO();
                    PromotionStoreTypeItemMappingDAO.PromotionStoreTypeId = PromotionStoreType.Id;
                    PromotionStoreTypeItemMappingDAO.ItemId = PromotionStoreTypeItemMapping.ItemId;
                    PromotionStoreTypeItemMappingDAO.Quantity = PromotionStoreTypeItemMapping.Quantity;
                    PromotionStoreTypeItemMappingDAOs.Add(PromotionStoreTypeItemMappingDAO);
                }
                await DataContext.PromotionStoreTypeItemMapping.BulkMergeAsync(PromotionStoreTypeItemMappingDAOs);
            }
        }
        
    }
}
