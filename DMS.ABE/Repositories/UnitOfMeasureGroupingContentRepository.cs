using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Repositories
{
    public interface IUnitOfMeasureGroupingContentRepository
    {
        Task<int> Count(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        Task<List<UnitOfMeasureGroupingContent>> List(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        Task<UnitOfMeasureGroupingContent> Get(long Id);
    }
    public class UnitOfMeasureGroupingContentRepository : IUnitOfMeasureGroupingContentRepository
    {
        private DataContext DataContext;
        public UnitOfMeasureGroupingContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<UnitOfMeasureGroupingContentDAO> DynamicFilter(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.UnitOfMeasureGroupingId != null)
                query = query.Where(q => q.UnitOfMeasureGroupingId, filter.UnitOfMeasureGroupingId);
            if (filter.UnitOfMeasureId != null)
                query = query.Where(q => q.UnitOfMeasureId, filter.UnitOfMeasureId);
            if (filter.Factor != null)
                query = query.Where(q => q.Factor, filter.Factor);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<UnitOfMeasureGroupingContentDAO> OrFilter(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<UnitOfMeasureGroupingContentDAO> initQuery = query.Where(q => false);
            foreach (UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter in filter.OrFilter)
            {
                IQueryable<UnitOfMeasureGroupingContentDAO> queryable = query;
                if (UnitOfMeasureGroupingContentFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, UnitOfMeasureGroupingContentFilter.Id);
                if (UnitOfMeasureGroupingContentFilter.UnitOfMeasureGroupingId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureGroupingId, UnitOfMeasureGroupingContentFilter.UnitOfMeasureGroupingId);
                if (UnitOfMeasureGroupingContentFilter.UnitOfMeasureId != null)
                    queryable = queryable.Where(q => q.UnitOfMeasureId, UnitOfMeasureGroupingContentFilter.UnitOfMeasureId);
                if (UnitOfMeasureGroupingContentFilter.Factor != null)
                    queryable = queryable.Where(q => q.Factor, UnitOfMeasureGroupingContentFilter.Factor);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<UnitOfMeasureGroupingContentDAO> DynamicOrder(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureGroupingContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasureGrouping:
                            query = query.OrderBy(q => q.UnitOfMeasureGroupingId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasure:
                            query = query.OrderBy(q => q.UnitOfMeasureId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.Factor:
                            query = query.OrderBy(q => q.Factor);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case UnitOfMeasureGroupingContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasureGrouping:
                            query = query.OrderByDescending(q => q.UnitOfMeasureGroupingId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.UnitOfMeasure:
                            query = query.OrderByDescending(q => q.UnitOfMeasureId);
                            break;
                        case UnitOfMeasureGroupingContentOrder.Factor:
                            query = query.OrderByDescending(q => q.Factor);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<UnitOfMeasureGroupingContent>> DynamicSelect(IQueryable<UnitOfMeasureGroupingContentDAO> query, UnitOfMeasureGroupingContentFilter filter)
        {
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await query.Select(q => new UnitOfMeasureGroupingContent()
            {
                Id = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.Id) ? q.Id : default(long),
                UnitOfMeasureGroupingId = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasureGrouping) ? q.UnitOfMeasureGroupingId : default(long),
                UnitOfMeasureId = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasure) ? q.UnitOfMeasureId : default(long),
                Factor = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.Factor) ? q.Factor : default(long?),
                UnitOfMeasure = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasure) && q.UnitOfMeasure != null ? new UnitOfMeasure
                {
                    Id = q.UnitOfMeasure.Id,
                    Code = q.UnitOfMeasure.Code,
                    Name = q.UnitOfMeasure.Name,
                    Description = q.UnitOfMeasure.Description,
                    StatusId = q.UnitOfMeasure.StatusId,
                } : null,
                UnitOfMeasureGrouping = filter.Selects.Contains(UnitOfMeasureGroupingContentSelect.UnitOfMeasureGrouping) && q.UnitOfMeasureGrouping != null ? new UnitOfMeasureGrouping
                {
                    Id = q.UnitOfMeasureGrouping.Id,
                    Name = q.UnitOfMeasureGrouping.Name,
                    UnitOfMeasureId = q.UnitOfMeasureGrouping.UnitOfMeasureId,
                    StatusId = q.UnitOfMeasureGrouping.StatusId,
                } : null,
            }).ToListAsync();
            return UnitOfMeasureGroupingContents;
        }

        public async Task<int> Count(UnitOfMeasureGroupingContentFilter filter)
        {
            IQueryable<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContents = DataContext.UnitOfMeasureGroupingContent;
            UnitOfMeasureGroupingContents = DynamicFilter(UnitOfMeasureGroupingContents, filter);
            return await UnitOfMeasureGroupingContents.CountAsync();
        }

        public async Task<List<UnitOfMeasureGroupingContent>> List(UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter == null) return new List<UnitOfMeasureGroupingContent>();
            IQueryable<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContentDAOs = DataContext.UnitOfMeasureGroupingContent;
            UnitOfMeasureGroupingContentDAOs = DynamicFilter(UnitOfMeasureGroupingContentDAOs, filter);
            UnitOfMeasureGroupingContentDAOs = DynamicOrder(UnitOfMeasureGroupingContentDAOs, filter);
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await DynamicSelect(UnitOfMeasureGroupingContentDAOs, filter);
            return UnitOfMeasureGroupingContents;
        }

        public async Task<UnitOfMeasureGroupingContent> Get(long Id)
        {
            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = await DataContext.UnitOfMeasureGroupingContent.Where(x => x.Id == Id).AsNoTracking().Select(x => new UnitOfMeasureGroupingContent()
            {
                Id = x.Id,
                UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Factor = x.Factor,
                UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                {
                    Id = x.UnitOfMeasure.Id,
                    Code = x.UnitOfMeasure.Code,
                    Name = x.UnitOfMeasure.Name,
                    Description = x.UnitOfMeasure.Description,
                    StatusId = x.UnitOfMeasure.StatusId,
                },
                UnitOfMeasureGrouping = x.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                {
                    Id = x.UnitOfMeasureGrouping.Id,
                    Name = x.UnitOfMeasureGrouping.Name,
                    UnitOfMeasureId = x.UnitOfMeasureGrouping.UnitOfMeasureId,
                    StatusId = x.UnitOfMeasureGrouping.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (UnitOfMeasureGroupingContent == null)
                return null;

            return UnitOfMeasureGroupingContent;
        }
    }
}
