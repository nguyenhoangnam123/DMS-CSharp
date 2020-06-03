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
    public interface IItemSpecificCriteriaRepository
    {
        Task<int> Count(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter);
        Task<List<ItemSpecificCriteria>> List(ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter);
        Task<ItemSpecificCriteria> Get(long Id);
        Task<bool> Create(ItemSpecificCriteria ItemSpecificCriteria);
        Task<bool> Update(ItemSpecificCriteria ItemSpecificCriteria);
        Task<bool> Delete(ItemSpecificCriteria ItemSpecificCriteria);
        Task<bool> BulkMerge(List<ItemSpecificCriteria> ItemSpecificCriterias);
        Task<bool> BulkDelete(List<ItemSpecificCriteria> ItemSpecificCriterias);
    }
    public class ItemSpecificCriteriaRepository : IItemSpecificCriteriaRepository
    {
        private DataContext DataContext;
        public ItemSpecificCriteriaRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ItemSpecificCriteriaDAO> DynamicFilter(IQueryable<ItemSpecificCriteriaDAO> query, ItemSpecificCriteriaFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ItemSpecificCriteriaDAO> OrFilter(IQueryable<ItemSpecificCriteriaDAO> query, ItemSpecificCriteriaFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ItemSpecificCriteriaDAO> initQuery = query.Where(q => false);
            foreach (ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter in filter.OrFilter)
            {
                IQueryable<ItemSpecificCriteriaDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ItemSpecificCriteriaDAO> DynamicOrder(IQueryable<ItemSpecificCriteriaDAO> query, ItemSpecificCriteriaFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ItemSpecificCriteriaOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ItemSpecificCriteriaOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ItemSpecificCriteriaOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ItemSpecificCriteriaOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ItemSpecificCriteriaOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ItemSpecificCriteriaOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ItemSpecificCriteria>> DynamicSelect(IQueryable<ItemSpecificCriteriaDAO> query, ItemSpecificCriteriaFilter filter)
        {
            List<ItemSpecificCriteria> ItemSpecificCriterias = await query.Select(q => new ItemSpecificCriteria()
            {
                Id = filter.Selects.Contains(ItemSpecificCriteriaSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ItemSpecificCriteriaSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ItemSpecificCriteriaSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ItemSpecificCriterias;
        }

        public async Task<int> Count(ItemSpecificCriteriaFilter filter)
        {
            IQueryable<ItemSpecificCriteriaDAO> ItemSpecificCriterias = DataContext.ItemSpecificCriteria.AsNoTracking();
            ItemSpecificCriterias = DynamicFilter(ItemSpecificCriterias, filter);
            return await ItemSpecificCriterias.CountAsync();
        }

        public async Task<List<ItemSpecificCriteria>> List(ItemSpecificCriteriaFilter filter)
        {
            if (filter == null) return new List<ItemSpecificCriteria>();
            IQueryable<ItemSpecificCriteriaDAO> ItemSpecificCriteriaDAOs = DataContext.ItemSpecificCriteria.AsNoTracking();
            ItemSpecificCriteriaDAOs = DynamicFilter(ItemSpecificCriteriaDAOs, filter);
            ItemSpecificCriteriaDAOs = DynamicOrder(ItemSpecificCriteriaDAOs, filter);
            List<ItemSpecificCriteria> ItemSpecificCriterias = await DynamicSelect(ItemSpecificCriteriaDAOs, filter);
            return ItemSpecificCriterias;
        }

        public async Task<ItemSpecificCriteria> Get(long Id)
        {
            ItemSpecificCriteria ItemSpecificCriteria = await DataContext.ItemSpecificCriteria.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new ItemSpecificCriteria()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (ItemSpecificCriteria == null)
                return null;

            return ItemSpecificCriteria;
        }
        public async Task<bool> Create(ItemSpecificCriteria ItemSpecificCriteria)
        {
            ItemSpecificCriteriaDAO ItemSpecificCriteriaDAO = new ItemSpecificCriteriaDAO();
            ItemSpecificCriteriaDAO.Id = ItemSpecificCriteria.Id;
            ItemSpecificCriteriaDAO.Code = ItemSpecificCriteria.Code;
            ItemSpecificCriteriaDAO.Name = ItemSpecificCriteria.Name;
            DataContext.ItemSpecificCriteria.Add(ItemSpecificCriteriaDAO);
            await DataContext.SaveChangesAsync();
            ItemSpecificCriteria.Id = ItemSpecificCriteriaDAO.Id;
            await SaveReference(ItemSpecificCriteria);
            return true;
        }

        public async Task<bool> Update(ItemSpecificCriteria ItemSpecificCriteria)
        {
            ItemSpecificCriteriaDAO ItemSpecificCriteriaDAO = DataContext.ItemSpecificCriteria.Where(x => x.Id == ItemSpecificCriteria.Id).FirstOrDefault();
            if (ItemSpecificCriteriaDAO == null)
                return false;
            ItemSpecificCriteriaDAO.Id = ItemSpecificCriteria.Id;
            ItemSpecificCriteriaDAO.Code = ItemSpecificCriteria.Code;
            ItemSpecificCriteriaDAO.Name = ItemSpecificCriteria.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(ItemSpecificCriteria);
            return true;
        }

        public async Task<bool> Delete(ItemSpecificCriteria ItemSpecificCriteria)
        {
            await DataContext.ItemSpecificCriteria.Where(x => x.Id == ItemSpecificCriteria.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ItemSpecificCriteria> ItemSpecificCriterias)
        {
            List<ItemSpecificCriteriaDAO> ItemSpecificCriteriaDAOs = new List<ItemSpecificCriteriaDAO>();
            foreach (ItemSpecificCriteria ItemSpecificCriteria in ItemSpecificCriterias)
            {
                ItemSpecificCriteriaDAO ItemSpecificCriteriaDAO = new ItemSpecificCriteriaDAO();
                ItemSpecificCriteriaDAO.Id = ItemSpecificCriteria.Id;
                ItemSpecificCriteriaDAO.Code = ItemSpecificCriteria.Code;
                ItemSpecificCriteriaDAO.Name = ItemSpecificCriteria.Name;
                ItemSpecificCriteriaDAOs.Add(ItemSpecificCriteriaDAO);
            }
            await DataContext.BulkMergeAsync(ItemSpecificCriteriaDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ItemSpecificCriteria> ItemSpecificCriterias)
        {
            List<long> Ids = ItemSpecificCriterias.Select(x => x.Id).ToList();
            await DataContext.ItemSpecificCriteria
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ItemSpecificCriteria ItemSpecificCriteria)
        {
        }
        
    }
}
