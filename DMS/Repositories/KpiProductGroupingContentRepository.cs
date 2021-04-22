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
    public interface IKpiProductGroupingContentRepository
    {
        Task<int> Count(KpiProductGroupingContentFilter KpiProductGroupingContentFilter);
        Task<List<KpiProductGroupingContent>> List(KpiProductGroupingContentFilter KpiProductGroupingContentFilter);
        Task<List<KpiProductGroupingContent>> List(List<long> Ids);
        Task<KpiProductGroupingContent> Get(long Id);
        Task<bool> Create(KpiProductGroupingContent KpiProductGroupingContent);
        Task<bool> Update(KpiProductGroupingContent KpiProductGroupingContent);
        Task<bool> Delete(KpiProductGroupingContent KpiProductGroupingContent);
        Task<bool> BulkMerge(List<KpiProductGroupingContent> KpiProductGroupingContents);
        Task<bool> BulkDelete(List<KpiProductGroupingContent> KpiProductGroupingContents);
    }
    public class KpiProductGroupingContentRepository : IKpiProductGroupingContentRepository
    {
        private DataContext DataContext;
        public KpiProductGroupingContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiProductGroupingContentDAO> DynamicFilter(IQueryable<KpiProductGroupingContentDAO> query, KpiProductGroupingContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.KpiProductGroupingId != null && filter.KpiProductGroupingId.HasValue)
                query = query.Where(q => q.KpiProductGroupingId, filter.KpiProductGroupingId);
            if (filter.ProductGroupingId != null && filter.ProductGroupingId.HasValue)
                query = query.Where(q => q.ProductGroupingId, filter.ProductGroupingId);
            if (filter.RowId != null && filter.RowId.HasValue)
                query = query.Where(q => q.RowId, filter.RowId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<KpiProductGroupingContentDAO> OrFilter(IQueryable<KpiProductGroupingContentDAO> query, KpiProductGroupingContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiProductGroupingContentDAO> initQuery = query.Where(q => false);
            foreach (KpiProductGroupingContentFilter KpiProductGroupingContentFilter in filter.OrFilter)
            {
                IQueryable<KpiProductGroupingContentDAO> queryable = query;
                if (KpiProductGroupingContentFilter.Id != null && KpiProductGroupingContentFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (KpiProductGroupingContentFilter.KpiProductGroupingId != null && KpiProductGroupingContentFilter.KpiProductGroupingId.HasValue)
                    queryable = queryable.Where(q => q.KpiProductGroupingId, filter.KpiProductGroupingId);
                if (KpiProductGroupingContentFilter.ProductGroupingId != null && KpiProductGroupingContentFilter.ProductGroupingId.HasValue)
                    queryable = queryable.Where(q => q.ProductGroupingId, filter.ProductGroupingId);
                if (KpiProductGroupingContentFilter.RowId != null && KpiProductGroupingContentFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<KpiProductGroupingContentDAO> DynamicOrder(IQueryable<KpiProductGroupingContentDAO> query, KpiProductGroupingContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiProductGroupingContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiProductGroupingContentOrder.KpiProductGrouping:
                            query = query.OrderBy(q => q.KpiProductGroupingId);
                            break;
                        case KpiProductGroupingContentOrder.ProductGrouping:
                            query = query.OrderBy(q => q.ProductGroupingId);
                            break;
                        case KpiProductGroupingContentOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiProductGroupingContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiProductGroupingContentOrder.KpiProductGrouping:
                            query = query.OrderByDescending(q => q.KpiProductGroupingId);
                            break;
                        case KpiProductGroupingContentOrder.ProductGrouping:
                            query = query.OrderByDescending(q => q.ProductGroupingId);
                            break;
                        case KpiProductGroupingContentOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiProductGroupingContent>> DynamicSelect(IQueryable<KpiProductGroupingContentDAO> query, KpiProductGroupingContentFilter filter)
        {
            List<KpiProductGroupingContent> KpiProductGroupingContents = await query.Select(q => new KpiProductGroupingContent()
            {
                Id = filter.Selects.Contains(KpiProductGroupingContentSelect.Id) ? q.Id : default(long),
                KpiProductGroupingId = filter.Selects.Contains(KpiProductGroupingContentSelect.KpiProductGrouping) ? q.KpiProductGroupingId : default(long),
                ProductGroupingId = filter.Selects.Contains(KpiProductGroupingContentSelect.ProductGrouping) ? q.ProductGroupingId : default(long),
                RowId = filter.Selects.Contains(KpiProductGroupingContentSelect.Row) ? q.RowId : default(Guid),
                KpiProductGrouping = filter.Selects.Contains(KpiProductGroupingContentSelect.KpiProductGrouping) && q.KpiProductGrouping != null ? new KpiProductGrouping
                {
                    Id = q.KpiProductGrouping.Id,
                    OrganizationId = q.KpiProductGrouping.OrganizationId,
                    KpiYearId = q.KpiProductGrouping.KpiYearId,
                    KpiPeriodId = q.KpiProductGrouping.KpiPeriodId,
                    KpiProductGroupingTypeId = q.KpiProductGrouping.KpiProductGroupingTypeId,
                    StatusId = q.KpiProductGrouping.StatusId,
                    EmployeeId = q.KpiProductGrouping.EmployeeId,
                    CreatorId = q.KpiProductGrouping.CreatorId,
                    RowId = q.KpiProductGrouping.RowId,
                } : null,
            }).ToListAsync();
            return KpiProductGroupingContents;
        }

        public async Task<int> Count(KpiProductGroupingContentFilter filter)
        {
            IQueryable<KpiProductGroupingContentDAO> KpiProductGroupingContents = DataContext.KpiProductGroupingContent.AsNoTracking();
            KpiProductGroupingContents = DynamicFilter(KpiProductGroupingContents, filter);
            return await KpiProductGroupingContents.CountAsync();
        }

        public async Task<List<KpiProductGroupingContent>> List(KpiProductGroupingContentFilter filter)
        {
            if (filter == null) return new List<KpiProductGroupingContent>();
            IQueryable<KpiProductGroupingContentDAO> KpiProductGroupingContentDAOs = DataContext.KpiProductGroupingContent.AsNoTracking();
            KpiProductGroupingContentDAOs = DynamicFilter(KpiProductGroupingContentDAOs, filter);
            KpiProductGroupingContentDAOs = DynamicOrder(KpiProductGroupingContentDAOs, filter);
            List<KpiProductGroupingContent> KpiProductGroupingContents = await DynamicSelect(KpiProductGroupingContentDAOs, filter);
            return KpiProductGroupingContents;
        }

        public async Task<List<KpiProductGroupingContent>> List(List<long> Ids)
        {
            List<KpiProductGroupingContent> KpiProductGroupingContents = await DataContext.KpiProductGroupingContent.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new KpiProductGroupingContent()
            {
                Id = x.Id,
                KpiProductGroupingId = x.KpiProductGroupingId,
                ProductGroupingId = x.ProductGroupingId,
                RowId = x.RowId,
                KpiProductGrouping = x.KpiProductGrouping == null ? null : new KpiProductGrouping
                {
                    Id = x.KpiProductGrouping.Id,
                    OrganizationId = x.KpiProductGrouping.OrganizationId,
                    KpiYearId = x.KpiProductGrouping.KpiYearId,
                    KpiPeriodId = x.KpiProductGrouping.KpiPeriodId,
                    KpiProductGroupingTypeId = x.KpiProductGrouping.KpiProductGroupingTypeId,
                    StatusId = x.KpiProductGrouping.StatusId,
                    EmployeeId = x.KpiProductGrouping.EmployeeId,
                    CreatorId = x.KpiProductGrouping.CreatorId,
                    RowId = x.KpiProductGrouping.RowId,
                },
            }).ToListAsync();
            

            return KpiProductGroupingContents;
        }

        public async Task<KpiProductGroupingContent> Get(long Id)
        {
            KpiProductGroupingContent KpiProductGroupingContent = await DataContext.KpiProductGroupingContent.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new KpiProductGroupingContent()
            {
                Id = x.Id,
                KpiProductGroupingId = x.KpiProductGroupingId,
                ProductGroupingId = x.ProductGroupingId,
                RowId = x.RowId,
                KpiProductGrouping = x.KpiProductGrouping == null ? null : new KpiProductGrouping
                {
                    Id = x.KpiProductGrouping.Id,
                    OrganizationId = x.KpiProductGrouping.OrganizationId,
                    KpiYearId = x.KpiProductGrouping.KpiYearId,
                    KpiPeriodId = x.KpiProductGrouping.KpiPeriodId,
                    KpiProductGroupingTypeId = x.KpiProductGrouping.KpiProductGroupingTypeId,
                    StatusId = x.KpiProductGrouping.StatusId,
                    EmployeeId = x.KpiProductGrouping.EmployeeId,
                    CreatorId = x.KpiProductGrouping.CreatorId,
                    RowId = x.KpiProductGrouping.RowId,
                },
            }).FirstOrDefaultAsync();

            if (KpiProductGroupingContent == null)
                return null;

            return KpiProductGroupingContent;
        }
        public async Task<bool> Create(KpiProductGroupingContent KpiProductGroupingContent)
        {
            KpiProductGroupingContentDAO KpiProductGroupingContentDAO = new KpiProductGroupingContentDAO();
            KpiProductGroupingContentDAO.Id = KpiProductGroupingContent.Id;
            KpiProductGroupingContentDAO.KpiProductGroupingId = KpiProductGroupingContent.KpiProductGroupingId;
            KpiProductGroupingContentDAO.ProductGroupingId = KpiProductGroupingContent.ProductGroupingId;
            KpiProductGroupingContentDAO.RowId = KpiProductGroupingContent.RowId;
            KpiProductGroupingContentDAO.RowId = Guid.NewGuid();
            DataContext.KpiProductGroupingContent.Add(KpiProductGroupingContentDAO);
            await DataContext.SaveChangesAsync();
            KpiProductGroupingContent.Id = KpiProductGroupingContentDAO.Id;
            await SaveReference(KpiProductGroupingContent);
            return true;
        }

        public async Task<bool> Update(KpiProductGroupingContent KpiProductGroupingContent)
        {
            KpiProductGroupingContentDAO KpiProductGroupingContentDAO = DataContext.KpiProductGroupingContent.Where(x => x.Id == KpiProductGroupingContent.Id).FirstOrDefault();
            if (KpiProductGroupingContentDAO == null)
                return false;
            KpiProductGroupingContentDAO.Id = KpiProductGroupingContent.Id;
            KpiProductGroupingContentDAO.KpiProductGroupingId = KpiProductGroupingContent.KpiProductGroupingId;
            KpiProductGroupingContentDAO.ProductGroupingId = KpiProductGroupingContent.ProductGroupingId;
            KpiProductGroupingContentDAO.RowId = KpiProductGroupingContent.RowId;
            await DataContext.SaveChangesAsync();
            await SaveReference(KpiProductGroupingContent);
            return true;
        }

        public async Task<bool> Delete(KpiProductGroupingContent KpiProductGroupingContent)
        {
            await DataContext.KpiProductGroupingContent.Where(x => x.Id == KpiProductGroupingContent.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<KpiProductGroupingContent> KpiProductGroupingContents)
        {
            List<KpiProductGroupingContentDAO> KpiProductGroupingContentDAOs = new List<KpiProductGroupingContentDAO>();
            foreach (KpiProductGroupingContent KpiProductGroupingContent in KpiProductGroupingContents)
            {
                KpiProductGroupingContentDAO KpiProductGroupingContentDAO = new KpiProductGroupingContentDAO();
                KpiProductGroupingContentDAO.Id = KpiProductGroupingContent.Id;
                KpiProductGroupingContentDAO.KpiProductGroupingId = KpiProductGroupingContent.KpiProductGroupingId;
                KpiProductGroupingContentDAO.ProductGroupingId = KpiProductGroupingContent.ProductGroupingId;
                KpiProductGroupingContentDAO.RowId = KpiProductGroupingContent.RowId;
                KpiProductGroupingContentDAOs.Add(KpiProductGroupingContentDAO);
            }
            await DataContext.BulkMergeAsync(KpiProductGroupingContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<KpiProductGroupingContent> KpiProductGroupingContents)
        {
            List<long> Ids = KpiProductGroupingContents.Select(x => x.Id).ToList();
            await DataContext.KpiProductGroupingContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(KpiProductGroupingContent KpiProductGroupingContent)
        {
        }
        
    }
}
