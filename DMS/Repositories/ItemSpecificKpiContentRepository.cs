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
    public interface IItemSpecificKpiContentRepository
    {
        Task<int> Count(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter);
        Task<List<ItemSpecificKpiContent>> List(ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter);
        Task<ItemSpecificKpiContent> Get(long Id);
        Task<bool> Create(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<bool> Update(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<bool> Delete(ItemSpecificKpiContent ItemSpecificKpiContent);
        Task<bool> BulkMerge(List<ItemSpecificKpiContent> ItemSpecificKpiContents);
        Task<bool> BulkDelete(List<ItemSpecificKpiContent> ItemSpecificKpiContents);
    }
    public class ItemSpecificKpiContentRepository : IItemSpecificKpiContentRepository
    {
        private DataContext DataContext;
        public ItemSpecificKpiContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ItemSpecificKpiContentDAO> DynamicFilter(IQueryable<ItemSpecificKpiContentDAO> query, ItemSpecificKpiContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.ItemSpecificKpiId != null)
                query = query.Where(q => q.ItemSpecificKpiId, filter.ItemSpecificKpiId);
            if (filter.ItemSpecificCriteriaId != null)
                query = query.Where(q => q.ItemSpecificCriteriaId, filter.ItemSpecificCriteriaId);
            if (filter.ItemId != null)
                query = query.Where(q => q.ItemId, filter.ItemId);
            if (filter.Value != null)
                query = query.Where(q => q.Value, filter.Value);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ItemSpecificKpiContentDAO> OrFilter(IQueryable<ItemSpecificKpiContentDAO> query, ItemSpecificKpiContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ItemSpecificKpiContentDAO> initQuery = query.Where(q => false);
            foreach (ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter in filter.OrFilter)
            {
                IQueryable<ItemSpecificKpiContentDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.ItemSpecificKpiId != null)
                    queryable = queryable.Where(q => q.ItemSpecificKpiId, filter.ItemSpecificKpiId);
                if (filter.ItemSpecificCriteriaId != null)
                    queryable = queryable.Where(q => q.ItemSpecificCriteriaId, filter.ItemSpecificCriteriaId);
                if (filter.ItemId != null)
                    queryable = queryable.Where(q => q.ItemId, filter.ItemId);
                if (filter.Value != null)
                    queryable = queryable.Where(q => q.Value, filter.Value);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ItemSpecificKpiContentDAO> DynamicOrder(IQueryable<ItemSpecificKpiContentDAO> query, ItemSpecificKpiContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ItemSpecificKpiContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ItemSpecificKpiContentOrder.ItemSpecificKpi:
                            query = query.OrderBy(q => q.ItemSpecificKpiId);
                            break;
                        case ItemSpecificKpiContentOrder.ItemSpecificCriteria:
                            query = query.OrderBy(q => q.ItemSpecificCriteriaId);
                            break;
                        case ItemSpecificKpiContentOrder.Item:
                            query = query.OrderBy(q => q.ItemId);
                            break;
                        case ItemSpecificKpiContentOrder.Value:
                            query = query.OrderBy(q => q.Value);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ItemSpecificKpiContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ItemSpecificKpiContentOrder.ItemSpecificKpi:
                            query = query.OrderByDescending(q => q.ItemSpecificKpiId);
                            break;
                        case ItemSpecificKpiContentOrder.ItemSpecificCriteria:
                            query = query.OrderByDescending(q => q.ItemSpecificCriteriaId);
                            break;
                        case ItemSpecificKpiContentOrder.Item:
                            query = query.OrderByDescending(q => q.ItemId);
                            break;
                        case ItemSpecificKpiContentOrder.Value:
                            query = query.OrderByDescending(q => q.Value);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ItemSpecificKpiContent>> DynamicSelect(IQueryable<ItemSpecificKpiContentDAO> query, ItemSpecificKpiContentFilter filter)
        {
            List<ItemSpecificKpiContent> ItemSpecificKpiContents = await query.Select(q => new ItemSpecificKpiContent()
            {
                Id = filter.Selects.Contains(ItemSpecificKpiContentSelect.Id) ? q.Id : default(long),
                ItemSpecificKpiId = filter.Selects.Contains(ItemSpecificKpiContentSelect.ItemSpecificKpi) ? q.ItemSpecificKpiId : default(long),
                ItemSpecificCriteriaId = filter.Selects.Contains(ItemSpecificKpiContentSelect.ItemSpecificCriteria) ? q.ItemSpecificCriteriaId : default(long),
                ItemId = filter.Selects.Contains(ItemSpecificKpiContentSelect.Item) ? q.ItemId : default(long),
                Value = filter.Selects.Contains(ItemSpecificKpiContentSelect.Value) ? q.Value : default(long),
                Item = filter.Selects.Contains(ItemSpecificKpiContentSelect.Item) && q.Item != null ? new Item
                {
                    Id = q.Item.Id,
                    ProductId = q.Item.ProductId,
                    Code = q.Item.Code,
                    Name = q.Item.Name,
                    ScanCode = q.Item.ScanCode,
                    SalePrice = q.Item.SalePrice,
                    RetailPrice = q.Item.RetailPrice,
                    StatusId = q.Item.StatusId,
                } : null,
                ItemSpecificCriteria = filter.Selects.Contains(ItemSpecificKpiContentSelect.ItemSpecificCriteria) && q.ItemSpecificCriteria != null ? new ItemSpecificCriteria
                {
                    Id = q.ItemSpecificCriteria.Id,
                    Code = q.ItemSpecificCriteria.Code,
                    Name = q.ItemSpecificCriteria.Name,
                } : null,
                ItemSpecificKpi = filter.Selects.Contains(ItemSpecificKpiContentSelect.ItemSpecificKpi) && q.ItemSpecificKpi != null ? new ItemSpecificKpi
                {
                    Id = q.ItemSpecificKpi.Id,
                    OrganizationId = q.ItemSpecificKpi.OrganizationId,
                    KpiPeriodId = q.ItemSpecificKpi.KpiPeriodId,
                    StatusId = q.ItemSpecificKpi.StatusId,
                    EmployeeId = q.ItemSpecificKpi.EmployeeId,
                    CreatorId = q.ItemSpecificKpi.CreatorId,
                } : null,
            }).ToListAsync();
            return ItemSpecificKpiContents;
        }

        public async Task<int> Count(ItemSpecificKpiContentFilter filter)
        {
            IQueryable<ItemSpecificKpiContentDAO> ItemSpecificKpiContents = DataContext.ItemSpecificKpiContent.AsNoTracking();
            ItemSpecificKpiContents = DynamicFilter(ItemSpecificKpiContents, filter);
            return await ItemSpecificKpiContents.CountAsync();
        }

        public async Task<List<ItemSpecificKpiContent>> List(ItemSpecificKpiContentFilter filter)
        {
            if (filter == null) return new List<ItemSpecificKpiContent>();
            IQueryable<ItemSpecificKpiContentDAO> ItemSpecificKpiContentDAOs = DataContext.ItemSpecificKpiContent.AsNoTracking();
            ItemSpecificKpiContentDAOs = DynamicFilter(ItemSpecificKpiContentDAOs, filter);
            ItemSpecificKpiContentDAOs = DynamicOrder(ItemSpecificKpiContentDAOs, filter);
            List<ItemSpecificKpiContent> ItemSpecificKpiContents = await DynamicSelect(ItemSpecificKpiContentDAOs, filter);
            return ItemSpecificKpiContents;
        }

        public async Task<ItemSpecificKpiContent> Get(long Id)
        {
            ItemSpecificKpiContent ItemSpecificKpiContent = await DataContext.ItemSpecificKpiContent.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new ItemSpecificKpiContent()
            {
                Id = x.Id,
                ItemSpecificKpiId = x.ItemSpecificKpiId,
                ItemSpecificCriteriaId = x.ItemSpecificCriteriaId,
                ItemId = x.ItemId,
                Value = x.Value,
                Item = x.Item == null ? null : new Item
                {
                    Id = x.Item.Id,
                    ProductId = x.Item.ProductId,
                    Code = x.Item.Code,
                    Name = x.Item.Name,
                    ScanCode = x.Item.ScanCode,
                    SalePrice = x.Item.SalePrice,
                    RetailPrice = x.Item.RetailPrice,
                    StatusId = x.Item.StatusId,
                },
                ItemSpecificCriteria = x.ItemSpecificCriteria == null ? null : new ItemSpecificCriteria
                {
                    Id = x.ItemSpecificCriteria.Id,
                    Code = x.ItemSpecificCriteria.Code,
                    Name = x.ItemSpecificCriteria.Name,
                },
                ItemSpecificKpi = x.ItemSpecificKpi == null ? null : new ItemSpecificKpi
                {
                    Id = x.ItemSpecificKpi.Id,
                    OrganizationId = x.ItemSpecificKpi.OrganizationId,
                    KpiPeriodId = x.ItemSpecificKpi.KpiPeriodId,
                    StatusId = x.ItemSpecificKpi.StatusId,
                    EmployeeId = x.ItemSpecificKpi.EmployeeId,
                    CreatorId = x.ItemSpecificKpi.CreatorId,
                },
            }).FirstOrDefaultAsync();

            if (ItemSpecificKpiContent == null)
                return null;

            return ItemSpecificKpiContent;
        }
        public async Task<bool> Create(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            ItemSpecificKpiContentDAO ItemSpecificKpiContentDAO = new ItemSpecificKpiContentDAO();
            ItemSpecificKpiContentDAO.Id = ItemSpecificKpiContent.Id;
            ItemSpecificKpiContentDAO.ItemSpecificKpiId = ItemSpecificKpiContent.ItemSpecificKpiId;
            ItemSpecificKpiContentDAO.ItemSpecificCriteriaId = ItemSpecificKpiContent.ItemSpecificCriteriaId;
            ItemSpecificKpiContentDAO.ItemId = ItemSpecificKpiContent.ItemId;
            ItemSpecificKpiContentDAO.Value = ItemSpecificKpiContent.Value;
            DataContext.ItemSpecificKpiContent.Add(ItemSpecificKpiContentDAO);
            await DataContext.SaveChangesAsync();
            ItemSpecificKpiContent.Id = ItemSpecificKpiContentDAO.Id;
            await SaveReference(ItemSpecificKpiContent);
            return true;
        }

        public async Task<bool> Update(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            ItemSpecificKpiContentDAO ItemSpecificKpiContentDAO = DataContext.ItemSpecificKpiContent.Where(x => x.Id == ItemSpecificKpiContent.Id).FirstOrDefault();
            if (ItemSpecificKpiContentDAO == null)
                return false;
            ItemSpecificKpiContentDAO.Id = ItemSpecificKpiContent.Id;
            ItemSpecificKpiContentDAO.ItemSpecificKpiId = ItemSpecificKpiContent.ItemSpecificKpiId;
            ItemSpecificKpiContentDAO.ItemSpecificCriteriaId = ItemSpecificKpiContent.ItemSpecificCriteriaId;
            ItemSpecificKpiContentDAO.ItemId = ItemSpecificKpiContent.ItemId;
            ItemSpecificKpiContentDAO.Value = ItemSpecificKpiContent.Value;
            await DataContext.SaveChangesAsync();
            await SaveReference(ItemSpecificKpiContent);
            return true;
        }

        public async Task<bool> Delete(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            await DataContext.ItemSpecificKpiContent.Where(x => x.Id == ItemSpecificKpiContent.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ItemSpecificKpiContent> ItemSpecificKpiContents)
        {
            List<ItemSpecificKpiContentDAO> ItemSpecificKpiContentDAOs = new List<ItemSpecificKpiContentDAO>();
            foreach (ItemSpecificKpiContent ItemSpecificKpiContent in ItemSpecificKpiContents)
            {
                ItemSpecificKpiContentDAO ItemSpecificKpiContentDAO = new ItemSpecificKpiContentDAO();
                ItemSpecificKpiContentDAO.Id = ItemSpecificKpiContent.Id;
                ItemSpecificKpiContentDAO.ItemSpecificKpiId = ItemSpecificKpiContent.ItemSpecificKpiId;
                ItemSpecificKpiContentDAO.ItemSpecificCriteriaId = ItemSpecificKpiContent.ItemSpecificCriteriaId;
                ItemSpecificKpiContentDAO.ItemId = ItemSpecificKpiContent.ItemId;
                ItemSpecificKpiContentDAO.Value = ItemSpecificKpiContent.Value;
                ItemSpecificKpiContentDAOs.Add(ItemSpecificKpiContentDAO);
            }
            await DataContext.BulkMergeAsync(ItemSpecificKpiContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ItemSpecificKpiContent> ItemSpecificKpiContents)
        {
            List<long> Ids = ItemSpecificKpiContents.Select(x => x.Id).ToList();
            await DataContext.ItemSpecificKpiContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
        }
        
    }
}
