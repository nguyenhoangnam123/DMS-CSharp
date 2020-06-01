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
    public interface ITotalItemSpecificCriteriaRepository
    {
        Task<int> Count(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter);
        Task<List<TotalItemSpecificCriteria>> List(TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter);
        Task<TotalItemSpecificCriteria> Get(long Id);
        Task<bool> Create(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<bool> Update(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<bool> Delete(TotalItemSpecificCriteria TotalItemSpecificCriteria);
        Task<bool> BulkMerge(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias);
        Task<bool> BulkDelete(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias);
    }
    public class TotalItemSpecificCriteriaRepository : ITotalItemSpecificCriteriaRepository
    {
        private DataContext DataContext;
        public TotalItemSpecificCriteriaRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<TotalItemSpecificCriteriaDAO> DynamicFilter(IQueryable<TotalItemSpecificCriteriaDAO> query, TotalItemSpecificCriteriaFilter filter)
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

         private IQueryable<TotalItemSpecificCriteriaDAO> OrFilter(IQueryable<TotalItemSpecificCriteriaDAO> query, TotalItemSpecificCriteriaFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<TotalItemSpecificCriteriaDAO> initQuery = query.Where(q => false);
            foreach (TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter in filter.OrFilter)
            {
                IQueryable<TotalItemSpecificCriteriaDAO> queryable = query;
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

        private IQueryable<TotalItemSpecificCriteriaDAO> DynamicOrder(IQueryable<TotalItemSpecificCriteriaDAO> query, TotalItemSpecificCriteriaFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case TotalItemSpecificCriteriaOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case TotalItemSpecificCriteriaOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case TotalItemSpecificCriteriaOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case TotalItemSpecificCriteriaOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case TotalItemSpecificCriteriaOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case TotalItemSpecificCriteriaOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<TotalItemSpecificCriteria>> DynamicSelect(IQueryable<TotalItemSpecificCriteriaDAO> query, TotalItemSpecificCriteriaFilter filter)
        {
            List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await query.Select(q => new TotalItemSpecificCriteria()
            {
                Id = filter.Selects.Contains(TotalItemSpecificCriteriaSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(TotalItemSpecificCriteriaSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(TotalItemSpecificCriteriaSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return TotalItemSpecificCriterias;
        }

        public async Task<int> Count(TotalItemSpecificCriteriaFilter filter)
        {
            IQueryable<TotalItemSpecificCriteriaDAO> TotalItemSpecificCriterias = DataContext.TotalItemSpecificCriteria.AsNoTracking();
            TotalItemSpecificCriterias = DynamicFilter(TotalItemSpecificCriterias, filter);
            return await TotalItemSpecificCriterias.CountAsync();
        }

        public async Task<List<TotalItemSpecificCriteria>> List(TotalItemSpecificCriteriaFilter filter)
        {
            if (filter == null) return new List<TotalItemSpecificCriteria>();
            IQueryable<TotalItemSpecificCriteriaDAO> TotalItemSpecificCriteriaDAOs = DataContext.TotalItemSpecificCriteria.AsNoTracking();
            TotalItemSpecificCriteriaDAOs = DynamicFilter(TotalItemSpecificCriteriaDAOs, filter);
            TotalItemSpecificCriteriaDAOs = DynamicOrder(TotalItemSpecificCriteriaDAOs, filter);
            List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await DynamicSelect(TotalItemSpecificCriteriaDAOs, filter);
            return TotalItemSpecificCriterias;
        }

        public async Task<TotalItemSpecificCriteria> Get(long Id)
        {
            TotalItemSpecificCriteria TotalItemSpecificCriteria = await DataContext.TotalItemSpecificCriteria.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new TotalItemSpecificCriteria()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (TotalItemSpecificCriteria == null)
                return null;
            TotalItemSpecificCriteria.ItemSpecificKpiTotalItemSpecificCriteriaMappings = await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.AsNoTracking()
                .Where(x => x.TotalItemSpecificCriteriaId == TotalItemSpecificCriteria.Id)
                .Where(x => x.ItemSpecificKpi.DeletedAt == null)
                .Select(x => new ItemSpecificKpiTotalItemSpecificCriteriaMapping
                {
                    ItemSpecificKpiId = x.ItemSpecificKpiId,
                    TotalItemSpecificCriteriaId = x.TotalItemSpecificCriteriaId,
                    Value = x.Value,
                    ItemSpecificKpi = new ItemSpecificKpi
                    {
                        Id = x.ItemSpecificKpi.Id,
                        OrganizationId = x.ItemSpecificKpi.OrganizationId,
                        KpiPeriodId = x.ItemSpecificKpi.KpiPeriodId,
                        StatusId = x.ItemSpecificKpi.StatusId,
                        EmployeeId = x.ItemSpecificKpi.EmployeeId,
                        CreatorId = x.ItemSpecificKpi.CreatorId,
                    },
                }).ToListAsync();

            return TotalItemSpecificCriteria;
        }
        public async Task<bool> Create(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            TotalItemSpecificCriteriaDAO TotalItemSpecificCriteriaDAO = new TotalItemSpecificCriteriaDAO();
            TotalItemSpecificCriteriaDAO.Id = TotalItemSpecificCriteria.Id;
            TotalItemSpecificCriteriaDAO.Code = TotalItemSpecificCriteria.Code;
            TotalItemSpecificCriteriaDAO.Name = TotalItemSpecificCriteria.Name;
            DataContext.TotalItemSpecificCriteria.Add(TotalItemSpecificCriteriaDAO);
            await DataContext.SaveChangesAsync();
            TotalItemSpecificCriteria.Id = TotalItemSpecificCriteriaDAO.Id;
            await SaveReference(TotalItemSpecificCriteria);
            return true;
        }

        public async Task<bool> Update(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            TotalItemSpecificCriteriaDAO TotalItemSpecificCriteriaDAO = DataContext.TotalItemSpecificCriteria.Where(x => x.Id == TotalItemSpecificCriteria.Id).FirstOrDefault();
            if (TotalItemSpecificCriteriaDAO == null)
                return false;
            TotalItemSpecificCriteriaDAO.Id = TotalItemSpecificCriteria.Id;
            TotalItemSpecificCriteriaDAO.Code = TotalItemSpecificCriteria.Code;
            TotalItemSpecificCriteriaDAO.Name = TotalItemSpecificCriteria.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(TotalItemSpecificCriteria);
            return true;
        }

        public async Task<bool> Delete(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            await DataContext.TotalItemSpecificCriteria.Where(x => x.Id == TotalItemSpecificCriteria.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias)
        {
            List<TotalItemSpecificCriteriaDAO> TotalItemSpecificCriteriaDAOs = new List<TotalItemSpecificCriteriaDAO>();
            foreach (TotalItemSpecificCriteria TotalItemSpecificCriteria in TotalItemSpecificCriterias)
            {
                TotalItemSpecificCriteriaDAO TotalItemSpecificCriteriaDAO = new TotalItemSpecificCriteriaDAO();
                TotalItemSpecificCriteriaDAO.Id = TotalItemSpecificCriteria.Id;
                TotalItemSpecificCriteriaDAO.Code = TotalItemSpecificCriteria.Code;
                TotalItemSpecificCriteriaDAO.Name = TotalItemSpecificCriteria.Name;
                TotalItemSpecificCriteriaDAOs.Add(TotalItemSpecificCriteriaDAO);
            }
            await DataContext.BulkMergeAsync(TotalItemSpecificCriteriaDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<TotalItemSpecificCriteria> TotalItemSpecificCriterias)
        {
            List<long> Ids = TotalItemSpecificCriterias.Select(x => x.Id).ToList();
            await DataContext.TotalItemSpecificCriteria
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping
                .Where(x => x.TotalItemSpecificCriteriaId == TotalItemSpecificCriteria.Id)
                .DeleteFromQueryAsync();
            List<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO> ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs = new List<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO>();
            if (TotalItemSpecificCriteria.ItemSpecificKpiTotalItemSpecificCriteriaMappings != null)
            {
                foreach (ItemSpecificKpiTotalItemSpecificCriteriaMapping ItemSpecificKpiTotalItemSpecificCriteriaMapping in TotalItemSpecificCriteria.ItemSpecificKpiTotalItemSpecificCriteriaMappings)
                {
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO = new ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO();
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO.ItemSpecificKpiId = ItemSpecificKpiTotalItemSpecificCriteriaMapping.ItemSpecificKpiId;
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO.TotalItemSpecificCriteriaId = TotalItemSpecificCriteria.Id;
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO.Value = ItemSpecificKpiTotalItemSpecificCriteriaMapping.Value;
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs.Add(ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO);
                }
                await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.BulkMergeAsync(ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs);
            }
        }
        
    }
}
