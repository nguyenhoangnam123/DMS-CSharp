using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Helpers;

namespace DMS.Repositories
{
    public interface IPromotionStoreGroupingRepository
    {
        Task<int> Count(PromotionStoreGroupingFilter PromotionStoreGroupingFilter);
        Task<List<PromotionStoreGrouping>> List(PromotionStoreGroupingFilter PromotionStoreGroupingFilter);
        Task<PromotionStoreGrouping> Get(long Id);
        Task<bool> Create(PromotionStoreGrouping PromotionStoreGrouping);
        Task<bool> Update(PromotionStoreGrouping PromotionStoreGrouping);
        Task<bool> Delete(PromotionStoreGrouping PromotionStoreGrouping);
        Task<bool> BulkMerge(List<PromotionStoreGrouping> PromotionStoreGroupings);
        Task<bool> BulkDelete(List<PromotionStoreGrouping> PromotionStoreGroupings);
    }
    public class PromotionStoreGroupingRepository : IPromotionStoreGroupingRepository
    {
        private DataContext DataContext;
        public PromotionStoreGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PromotionStoreGroupingDAO> DynamicFilter(IQueryable<PromotionStoreGroupingDAO> query, PromotionStoreGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.PromotionPolicyId != null)
                query = query.Where(q => q.PromotionPolicyId, filter.PromotionPolicyId);
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

        private IQueryable<PromotionStoreGroupingDAO> OrFilter(IQueryable<PromotionStoreGroupingDAO> query, PromotionStoreGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PromotionStoreGroupingDAO> initQuery = query.Where(q => false);
            foreach (PromotionStoreGroupingFilter PromotionStoreGroupingFilter in filter.OrFilter)
            {
                IQueryable<PromotionStoreGroupingDAO> queryable = query;
                if (PromotionStoreGroupingFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PromotionStoreGroupingFilter.Id);
                if (PromotionStoreGroupingFilter.PromotionPolicyId != null)
                    queryable = queryable.Where(q => q.PromotionPolicyId, PromotionStoreGroupingFilter.PromotionPolicyId);
                if (PromotionStoreGroupingFilter.PromotionId != null)
                    queryable = queryable.Where(q => q.PromotionId, PromotionStoreGroupingFilter.PromotionId);
                if (PromotionStoreGroupingFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, PromotionStoreGroupingFilter.Note);
                if (PromotionStoreGroupingFilter.FromValue != null)
                    queryable = queryable.Where(q => q.FromValue, PromotionStoreGroupingFilter.FromValue);
                if (PromotionStoreGroupingFilter.ToValue != null)
                    queryable = queryable.Where(q => q.ToValue, PromotionStoreGroupingFilter.ToValue);
                if (PromotionStoreGroupingFilter.PromotionDiscountTypeId != null)
                    queryable = queryable.Where(q => q.PromotionDiscountTypeId, PromotionStoreGroupingFilter.PromotionDiscountTypeId);
                if (PromotionStoreGroupingFilter.DiscountPercentage != null)
                    queryable = queryable.Where(q => q.DiscountPercentage.HasValue).Where(q => q.DiscountPercentage, PromotionStoreGroupingFilter.DiscountPercentage);
                if (PromotionStoreGroupingFilter.DiscountValue != null)
                    queryable = queryable.Where(q => q.DiscountValue.HasValue).Where(q => q.DiscountValue, PromotionStoreGroupingFilter.DiscountValue);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PromotionStoreGroupingDAO> DynamicOrder(IQueryable<PromotionStoreGroupingDAO> query, PromotionStoreGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PromotionStoreGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PromotionStoreGroupingOrder.PromotionPolicy:
                            query = query.OrderBy(q => q.PromotionPolicyId);
                            break;
                        case PromotionStoreGroupingOrder.Promotion:
                            query = query.OrderBy(q => q.PromotionId);
                            break;
                        case PromotionStoreGroupingOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case PromotionStoreGroupingOrder.FromValue:
                            query = query.OrderBy(q => q.FromValue);
                            break;
                        case PromotionStoreGroupingOrder.ToValue:
                            query = query.OrderBy(q => q.ToValue);
                            break;
                        case PromotionStoreGroupingOrder.PromotionDiscountType:
                            query = query.OrderBy(q => q.PromotionDiscountTypeId);
                            break;
                        case PromotionStoreGroupingOrder.DiscountPercentage:
                            query = query.OrderBy(q => q.DiscountPercentage);
                            break;
                        case PromotionStoreGroupingOrder.DiscountValue:
                            query = query.OrderBy(q => q.DiscountValue);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PromotionStoreGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PromotionStoreGroupingOrder.PromotionPolicy:
                            query = query.OrderByDescending(q => q.PromotionPolicyId);
                            break;
                        case PromotionStoreGroupingOrder.Promotion:
                            query = query.OrderByDescending(q => q.PromotionId);
                            break;
                        case PromotionStoreGroupingOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case PromotionStoreGroupingOrder.FromValue:
                            query = query.OrderByDescending(q => q.FromValue);
                            break;
                        case PromotionStoreGroupingOrder.ToValue:
                            query = query.OrderByDescending(q => q.ToValue);
                            break;
                        case PromotionStoreGroupingOrder.PromotionDiscountType:
                            query = query.OrderByDescending(q => q.PromotionDiscountTypeId);
                            break;
                        case PromotionStoreGroupingOrder.DiscountPercentage:
                            query = query.OrderByDescending(q => q.DiscountPercentage);
                            break;
                        case PromotionStoreGroupingOrder.DiscountValue:
                            query = query.OrderByDescending(q => q.DiscountValue);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PromotionStoreGrouping>> DynamicSelect(IQueryable<PromotionStoreGroupingDAO> query, PromotionStoreGroupingFilter filter)
        {
            List<PromotionStoreGrouping> PromotionStoreGroupings = await query.Select(q => new PromotionStoreGrouping()
            {
                Id = filter.Selects.Contains(PromotionStoreGroupingSelect.Id) ? q.Id : default(long),
                PromotionPolicyId = filter.Selects.Contains(PromotionStoreGroupingSelect.PromotionPolicy) ? q.PromotionPolicyId : default(long),
                PromotionId = filter.Selects.Contains(PromotionStoreGroupingSelect.Promotion) ? q.PromotionId : default(long),
                Note = filter.Selects.Contains(PromotionStoreGroupingSelect.Note) ? q.Note : default(string),
                FromValue = filter.Selects.Contains(PromotionStoreGroupingSelect.FromValue) ? q.FromValue : default(decimal),
                ToValue = filter.Selects.Contains(PromotionStoreGroupingSelect.ToValue) ? q.ToValue : default(decimal),
                PromotionDiscountTypeId = filter.Selects.Contains(PromotionStoreGroupingSelect.PromotionDiscountType) ? q.PromotionDiscountTypeId : default(long),
                DiscountPercentage = filter.Selects.Contains(PromotionStoreGroupingSelect.DiscountPercentage) ? q.DiscountPercentage : default(decimal?),
                DiscountValue = filter.Selects.Contains(PromotionStoreGroupingSelect.DiscountValue) ? q.DiscountValue : default(decimal?),
                Promotion = filter.Selects.Contains(PromotionStoreGroupingSelect.Promotion) && q.Promotion != null ? new Promotion
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
                PromotionDiscountType = filter.Selects.Contains(PromotionStoreGroupingSelect.PromotionDiscountType) && q.PromotionDiscountType != null ? new PromotionDiscountType
                {
                    Id = q.PromotionDiscountType.Id,
                    Code = q.PromotionDiscountType.Code,
                    Name = q.PromotionDiscountType.Name,
                } : null,
                PromotionPolicy = filter.Selects.Contains(PromotionStoreGroupingSelect.PromotionPolicy) && q.PromotionPolicy != null ? new PromotionPolicy
                {
                    Id = q.PromotionPolicy.Id,
                    Code = q.PromotionPolicy.Code,
                    Name = q.PromotionPolicy.Name,
                } : null,
            }).ToListAsync();
            return PromotionStoreGroupings;
        }

        public async Task<int> Count(PromotionStoreGroupingFilter filter)
        {
            IQueryable<PromotionStoreGroupingDAO> PromotionStoreGroupings = DataContext.PromotionStoreGrouping.AsNoTracking();
            PromotionStoreGroupings = DynamicFilter(PromotionStoreGroupings, filter);
            return await PromotionStoreGroupings.CountAsync();
        }

        public async Task<List<PromotionStoreGrouping>> List(PromotionStoreGroupingFilter filter)
        {
            if (filter == null) return new List<PromotionStoreGrouping>();
            IQueryable<PromotionStoreGroupingDAO> PromotionStoreGroupingDAOs = DataContext.PromotionStoreGrouping.AsNoTracking();
            PromotionStoreGroupingDAOs = DynamicFilter(PromotionStoreGroupingDAOs, filter);
            PromotionStoreGroupingDAOs = DynamicOrder(PromotionStoreGroupingDAOs, filter);
            List<PromotionStoreGrouping> PromotionStoreGroupings = await DynamicSelect(PromotionStoreGroupingDAOs, filter);
            return PromotionStoreGroupings;
        }

        public async Task<PromotionStoreGrouping> Get(long Id)
        {
            PromotionStoreGrouping PromotionStoreGrouping = await DataContext.PromotionStoreGrouping.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PromotionStoreGrouping()
            {
                Id = x.Id,
                PromotionPolicyId = x.PromotionPolicyId,
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
                PromotionPolicy = x.PromotionPolicy == null ? null : new PromotionPolicy
                {
                    Id = x.PromotionPolicy.Id,
                    Code = x.PromotionPolicy.Code,
                    Name = x.PromotionPolicy.Name,
                },
            }).FirstOrDefaultAsync();

            if (PromotionStoreGrouping == null)
                return null;
            PromotionStoreGrouping.PromotionStoreGroupingItemMappings = await DataContext.PromotionStoreGroupingItemMapping.AsNoTracking()
                .Where(x => x.PromotionStoreGroupingId == PromotionStoreGrouping.Id)
                .Where(x => x.item.DeletedAt == null)
                .Select(x => new PromotionStoreGroupingItemMapping
                {
                    PromotionStoreGroupingId = x.PromotionStoreGroupingId,
                    itemId = x.itemId,
                    Quantity = x.Quantity,
                    item = new Item
                    {
                        Id = x.item.Id,
                        ProductId = x.item.ProductId,
                        Code = x.item.Code,
                        Name = x.item.Name,
                        ScanCode = x.item.ScanCode,
                        SalePrice = x.item.SalePrice,
                        RetailPrice = x.item.RetailPrice,
                        StatusId = x.item.StatusId,
                        Used = x.item.Used,
                    },
                }).ToListAsync();

            return PromotionStoreGrouping;
        }
        public async Task<bool> Create(PromotionStoreGrouping PromotionStoreGrouping)
        {
            PromotionStoreGroupingDAO PromotionStoreGroupingDAO = new PromotionStoreGroupingDAO();
            PromotionStoreGroupingDAO.Id = PromotionStoreGrouping.Id;
            PromotionStoreGroupingDAO.PromotionPolicyId = PromotionStoreGrouping.PromotionPolicyId;
            PromotionStoreGroupingDAO.PromotionId = PromotionStoreGrouping.PromotionId;
            PromotionStoreGroupingDAO.Note = PromotionStoreGrouping.Note;
            PromotionStoreGroupingDAO.FromValue = PromotionStoreGrouping.FromValue;
            PromotionStoreGroupingDAO.ToValue = PromotionStoreGrouping.ToValue;
            PromotionStoreGroupingDAO.PromotionDiscountTypeId = PromotionStoreGrouping.PromotionDiscountTypeId;
            PromotionStoreGroupingDAO.DiscountPercentage = PromotionStoreGrouping.DiscountPercentage;
            PromotionStoreGroupingDAO.DiscountValue = PromotionStoreGrouping.DiscountValue;
            DataContext.PromotionStoreGrouping.Add(PromotionStoreGroupingDAO);
            await DataContext.SaveChangesAsync();
            PromotionStoreGrouping.Id = PromotionStoreGroupingDAO.Id;
            await SaveReference(PromotionStoreGrouping);
            return true;
        }

        public async Task<bool> Update(PromotionStoreGrouping PromotionStoreGrouping)
        {
            PromotionStoreGroupingDAO PromotionStoreGroupingDAO = DataContext.PromotionStoreGrouping.Where(x => x.Id == PromotionStoreGrouping.Id).FirstOrDefault();
            if (PromotionStoreGroupingDAO == null)
                return false;
            PromotionStoreGroupingDAO.Id = PromotionStoreGrouping.Id;
            PromotionStoreGroupingDAO.PromotionPolicyId = PromotionStoreGrouping.PromotionPolicyId;
            PromotionStoreGroupingDAO.PromotionId = PromotionStoreGrouping.PromotionId;
            PromotionStoreGroupingDAO.Note = PromotionStoreGrouping.Note;
            PromotionStoreGroupingDAO.FromValue = PromotionStoreGrouping.FromValue;
            PromotionStoreGroupingDAO.ToValue = PromotionStoreGrouping.ToValue;
            PromotionStoreGroupingDAO.PromotionDiscountTypeId = PromotionStoreGrouping.PromotionDiscountTypeId;
            PromotionStoreGroupingDAO.DiscountPercentage = PromotionStoreGrouping.DiscountPercentage;
            PromotionStoreGroupingDAO.DiscountValue = PromotionStoreGrouping.DiscountValue;
            await DataContext.SaveChangesAsync();
            await SaveReference(PromotionStoreGrouping);
            return true;
        }

        public async Task<bool> Delete(PromotionStoreGrouping PromotionStoreGrouping)
        {
            await DataContext.PromotionStoreGrouping.Where(x => x.Id == PromotionStoreGrouping.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PromotionStoreGrouping> PromotionStoreGroupings)
        {
            List<PromotionStoreGroupingDAO> PromotionStoreGroupingDAOs = new List<PromotionStoreGroupingDAO>();
            foreach (PromotionStoreGrouping PromotionStoreGrouping in PromotionStoreGroupings)
            {
                PromotionStoreGroupingDAO PromotionStoreGroupingDAO = new PromotionStoreGroupingDAO();
                PromotionStoreGroupingDAO.Id = PromotionStoreGrouping.Id;
                PromotionStoreGroupingDAO.PromotionPolicyId = PromotionStoreGrouping.PromotionPolicyId;
                PromotionStoreGroupingDAO.PromotionId = PromotionStoreGrouping.PromotionId;
                PromotionStoreGroupingDAO.Note = PromotionStoreGrouping.Note;
                PromotionStoreGroupingDAO.FromValue = PromotionStoreGrouping.FromValue;
                PromotionStoreGroupingDAO.ToValue = PromotionStoreGrouping.ToValue;
                PromotionStoreGroupingDAO.PromotionDiscountTypeId = PromotionStoreGrouping.PromotionDiscountTypeId;
                PromotionStoreGroupingDAO.DiscountPercentage = PromotionStoreGrouping.DiscountPercentage;
                PromotionStoreGroupingDAO.DiscountValue = PromotionStoreGrouping.DiscountValue;
                PromotionStoreGroupingDAOs.Add(PromotionStoreGroupingDAO);
            }
            await DataContext.BulkMergeAsync(PromotionStoreGroupingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PromotionStoreGrouping> PromotionStoreGroupings)
        {
            List<long> Ids = PromotionStoreGroupings.Select(x => x.Id).ToList();
            await DataContext.PromotionStoreGrouping
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PromotionStoreGrouping PromotionStoreGrouping)
        {
            await DataContext.PromotionStoreGroupingItemMapping
                .Where(x => x.PromotionStoreGroupingId == PromotionStoreGrouping.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreGroupingItemMappingDAO> PromotionStoreGroupingItemMappingDAOs = new List<PromotionStoreGroupingItemMappingDAO>();
            if (PromotionStoreGrouping.PromotionStoreGroupingItemMappings != null)
            {
                foreach (PromotionStoreGroupingItemMapping PromotionStoreGroupingItemMapping in PromotionStoreGrouping.PromotionStoreGroupingItemMappings)
                {
                    PromotionStoreGroupingItemMappingDAO PromotionStoreGroupingItemMappingDAO = new PromotionStoreGroupingItemMappingDAO();
                    PromotionStoreGroupingItemMappingDAO.PromotionStoreGroupingId = PromotionStoreGrouping.Id;
                    PromotionStoreGroupingItemMappingDAO.itemId = PromotionStoreGroupingItemMapping.itemId;
                    PromotionStoreGroupingItemMappingDAO.Quantity = PromotionStoreGroupingItemMapping.Quantity;
                    PromotionStoreGroupingItemMappingDAOs.Add(PromotionStoreGroupingItemMappingDAO);
                }
                await DataContext.PromotionStoreGroupingItemMapping.BulkMergeAsync(PromotionStoreGroupingItemMappingDAOs);
            }
        }
        
    }
}
