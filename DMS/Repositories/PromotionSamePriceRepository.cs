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
    public interface IPromotionSamePriceRepository
    {
        Task<int> Count(PromotionSamePriceFilter PromotionSamePriceFilter);
        Task<List<PromotionSamePrice>> List(PromotionSamePriceFilter PromotionSamePriceFilter);
        Task<PromotionSamePrice> Get(long Id);
        Task<bool> Create(PromotionSamePrice PromotionSamePrice);
        Task<bool> Update(PromotionSamePrice PromotionSamePrice);
        Task<bool> Delete(PromotionSamePrice PromotionSamePrice);
        Task<bool> BulkMerge(List<PromotionSamePrice> PromotionSamePrices);
        Task<bool> BulkDelete(List<PromotionSamePrice> PromotionSamePrices);
    }
    public class PromotionSamePriceRepository : IPromotionSamePriceRepository
    {
        private DataContext DataContext;
        public PromotionSamePriceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PromotionSamePriceDAO> DynamicFilter(IQueryable<PromotionSamePriceDAO> query, PromotionSamePriceFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.PromotionId != null)
                query = query.Where(q => q.PromotionId, filter.PromotionId);
            if (filter.Price != null)
                query = query.Where(q => q.Price, filter.Price);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PromotionSamePriceDAO> OrFilter(IQueryable<PromotionSamePriceDAO> query, PromotionSamePriceFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PromotionSamePriceDAO> initQuery = query.Where(q => false);
            foreach (PromotionSamePriceFilter PromotionSamePriceFilter in filter.OrFilter)
            {
                IQueryable<PromotionSamePriceDAO> queryable = query;
                if (PromotionSamePriceFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PromotionSamePriceFilter.Id);
                if (PromotionSamePriceFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, PromotionSamePriceFilter.Note);
                if (PromotionSamePriceFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PromotionSamePriceFilter.Name);
                if (PromotionSamePriceFilter.PromotionId != null)
                    queryable = queryable.Where(q => q.PromotionId, PromotionSamePriceFilter.PromotionId);
                if (PromotionSamePriceFilter.Price != null)
                    queryable = queryable.Where(q => q.Price, PromotionSamePriceFilter.Price);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PromotionSamePriceDAO> DynamicOrder(IQueryable<PromotionSamePriceDAO> query, PromotionSamePriceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PromotionSamePriceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PromotionSamePriceOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case PromotionSamePriceOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PromotionSamePriceOrder.Promotion:
                            query = query.OrderBy(q => q.PromotionId);
                            break;
                        case PromotionSamePriceOrder.Price:
                            query = query.OrderBy(q => q.Price);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PromotionSamePriceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PromotionSamePriceOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case PromotionSamePriceOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PromotionSamePriceOrder.Promotion:
                            query = query.OrderByDescending(q => q.PromotionId);
                            break;
                        case PromotionSamePriceOrder.Price:
                            query = query.OrderByDescending(q => q.Price);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PromotionSamePrice>> DynamicSelect(IQueryable<PromotionSamePriceDAO> query, PromotionSamePriceFilter filter)
        {
            List<PromotionSamePrice> PromotionSamePrices = await query.Select(q => new PromotionSamePrice()
            {
                Id = filter.Selects.Contains(PromotionSamePriceSelect.Id) ? q.Id : default(long),
                Note = filter.Selects.Contains(PromotionSamePriceSelect.Note) ? q.Note : default(string),
                Name = filter.Selects.Contains(PromotionSamePriceSelect.Name) ? q.Name : default(string),
                PromotionId = filter.Selects.Contains(PromotionSamePriceSelect.Promotion) ? q.PromotionId : default(long),
                Price = filter.Selects.Contains(PromotionSamePriceSelect.Price) ? q.Price : default(decimal),
                Promotion = filter.Selects.Contains(PromotionSamePriceSelect.Promotion) && q.Promotion != null ? new Promotion
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
            }).ToListAsync();
            return PromotionSamePrices;
        }

        public async Task<int> Count(PromotionSamePriceFilter filter)
        {
            IQueryable<PromotionSamePriceDAO> PromotionSamePrices = DataContext.PromotionSamePrice.AsNoTracking();
            PromotionSamePrices = DynamicFilter(PromotionSamePrices, filter);
            return await PromotionSamePrices.CountAsync();
        }

        public async Task<List<PromotionSamePrice>> List(PromotionSamePriceFilter filter)
        {
            if (filter == null) return new List<PromotionSamePrice>();
            IQueryable<PromotionSamePriceDAO> PromotionSamePriceDAOs = DataContext.PromotionSamePrice.AsNoTracking();
            PromotionSamePriceDAOs = DynamicFilter(PromotionSamePriceDAOs, filter);
            PromotionSamePriceDAOs = DynamicOrder(PromotionSamePriceDAOs, filter);
            List<PromotionSamePrice> PromotionSamePrices = await DynamicSelect(PromotionSamePriceDAOs, filter);
            return PromotionSamePrices;
        }

        public async Task<PromotionSamePrice> Get(long Id)
        {
            PromotionSamePrice PromotionSamePrice = await DataContext.PromotionSamePrice.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PromotionSamePrice()
            {
                Id = x.Id,
                Note = x.Note,
                Name = x.Name,
                PromotionId = x.PromotionId,
                Price = x.Price,
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
            }).FirstOrDefaultAsync();

            if (PromotionSamePrice == null)
                return null;

            return PromotionSamePrice;
        }
        public async Task<bool> Create(PromotionSamePrice PromotionSamePrice)
        {
            PromotionSamePriceDAO PromotionSamePriceDAO = new PromotionSamePriceDAO();
            PromotionSamePriceDAO.Id = PromotionSamePrice.Id;
            PromotionSamePriceDAO.Note = PromotionSamePrice.Note;
            PromotionSamePriceDAO.Name = PromotionSamePrice.Name;
            PromotionSamePriceDAO.PromotionId = PromotionSamePrice.PromotionId;
            PromotionSamePriceDAO.Price = PromotionSamePrice.Price;
            DataContext.PromotionSamePrice.Add(PromotionSamePriceDAO);
            await DataContext.SaveChangesAsync();
            PromotionSamePrice.Id = PromotionSamePriceDAO.Id;
            await SaveReference(PromotionSamePrice);
            return true;
        }

        public async Task<bool> Update(PromotionSamePrice PromotionSamePrice)
        {
            PromotionSamePriceDAO PromotionSamePriceDAO = DataContext.PromotionSamePrice.Where(x => x.Id == PromotionSamePrice.Id).FirstOrDefault();
            if (PromotionSamePriceDAO == null)
                return false;
            PromotionSamePriceDAO.Id = PromotionSamePrice.Id;
            PromotionSamePriceDAO.Note = PromotionSamePrice.Note;
            PromotionSamePriceDAO.Name = PromotionSamePrice.Name;
            PromotionSamePriceDAO.PromotionId = PromotionSamePrice.PromotionId;
            PromotionSamePriceDAO.Price = PromotionSamePrice.Price;
            await DataContext.SaveChangesAsync();
            await SaveReference(PromotionSamePrice);
            return true;
        }

        public async Task<bool> Delete(PromotionSamePrice PromotionSamePrice)
        {
            await DataContext.PromotionSamePrice.Where(x => x.Id == PromotionSamePrice.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PromotionSamePrice> PromotionSamePrices)
        {
            List<PromotionSamePriceDAO> PromotionSamePriceDAOs = new List<PromotionSamePriceDAO>();
            foreach (PromotionSamePrice PromotionSamePrice in PromotionSamePrices)
            {
                PromotionSamePriceDAO PromotionSamePriceDAO = new PromotionSamePriceDAO();
                PromotionSamePriceDAO.Id = PromotionSamePrice.Id;
                PromotionSamePriceDAO.Note = PromotionSamePrice.Note;
                PromotionSamePriceDAO.Name = PromotionSamePrice.Name;
                PromotionSamePriceDAO.PromotionId = PromotionSamePrice.PromotionId;
                PromotionSamePriceDAO.Price = PromotionSamePrice.Price;
                PromotionSamePriceDAOs.Add(PromotionSamePriceDAO);
            }
            await DataContext.BulkMergeAsync(PromotionSamePriceDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PromotionSamePrice> PromotionSamePrices)
        {
            List<long> Ids = PromotionSamePrices.Select(x => x.Id).ToList();
            await DataContext.PromotionSamePrice
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PromotionSamePrice PromotionSamePrice)
        {
        }
        
    }
}
