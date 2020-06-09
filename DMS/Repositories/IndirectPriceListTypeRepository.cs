using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IIndirectPriceListTypeRepository
    {
        Task<int> Count(IndirectPriceListTypeFilter IndirectPriceListTypeFilter);
        Task<List<IndirectPriceListType>> List(IndirectPriceListTypeFilter IndirectPriceListTypeFilter);
        Task<IndirectPriceListType> Get(long Id);
        Task<bool> Create(IndirectPriceListType IndirectPriceListType);
        Task<bool> Update(IndirectPriceListType IndirectPriceListType);
        Task<bool> Delete(IndirectPriceListType IndirectPriceListType);
        Task<bool> BulkMerge(List<IndirectPriceListType> IndirectPriceListTypes);
        Task<bool> BulkDelete(List<IndirectPriceListType> IndirectPriceListTypes);
    }
    public class IndirectPriceListTypeRepository : IIndirectPriceListTypeRepository
    {
        private DataContext DataContext;
        public IndirectPriceListTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<IndirectPriceListTypeDAO> DynamicFilter(IQueryable<IndirectPriceListTypeDAO> query, IndirectPriceListTypeFilter filter)
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

        private IQueryable<IndirectPriceListTypeDAO> OrFilter(IQueryable<IndirectPriceListTypeDAO> query, IndirectPriceListTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<IndirectPriceListTypeDAO> initQuery = query.Where(q => false);
            foreach (IndirectPriceListTypeFilter IndirectPriceListTypeFilter in filter.OrFilter)
            {
                IQueryable<IndirectPriceListTypeDAO> queryable = query;
                if (IndirectPriceListTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, IndirectPriceListTypeFilter.Id);
                if (IndirectPriceListTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, IndirectPriceListTypeFilter.Code);
                if (IndirectPriceListTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, IndirectPriceListTypeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<IndirectPriceListTypeDAO> DynamicOrder(IQueryable<IndirectPriceListTypeDAO> query, IndirectPriceListTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case IndirectPriceListTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case IndirectPriceListTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case IndirectPriceListTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case IndirectPriceListTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case IndirectPriceListTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case IndirectPriceListTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<IndirectPriceListType>> DynamicSelect(IQueryable<IndirectPriceListTypeDAO> query, IndirectPriceListTypeFilter filter)
        {
            List<IndirectPriceListType> IndirectPriceListTypes = await query.Select(q => new IndirectPriceListType()
            {
                Id = filter.Selects.Contains(IndirectPriceListTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(IndirectPriceListTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(IndirectPriceListTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return IndirectPriceListTypes;
        }

        public async Task<int> Count(IndirectPriceListTypeFilter filter)
        {
            IQueryable<IndirectPriceListTypeDAO> IndirectPriceListTypes = DataContext.IndirectPriceListType.AsNoTracking();
            IndirectPriceListTypes = DynamicFilter(IndirectPriceListTypes, filter);
            return await IndirectPriceListTypes.CountAsync();
        }

        public async Task<List<IndirectPriceListType>> List(IndirectPriceListTypeFilter filter)
        {
            if (filter == null) return new List<IndirectPriceListType>();
            IQueryable<IndirectPriceListTypeDAO> IndirectPriceListTypeDAOs = DataContext.IndirectPriceListType.AsNoTracking();
            IndirectPriceListTypeDAOs = DynamicFilter(IndirectPriceListTypeDAOs, filter);
            IndirectPriceListTypeDAOs = DynamicOrder(IndirectPriceListTypeDAOs, filter);
            List<IndirectPriceListType> IndirectPriceListTypes = await DynamicSelect(IndirectPriceListTypeDAOs, filter);
            return IndirectPriceListTypes;
        }

        public async Task<IndirectPriceListType> Get(long Id)
        {
            IndirectPriceListType IndirectPriceListType = await DataContext.IndirectPriceListType.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new IndirectPriceListType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (IndirectPriceListType == null)
                return null;

            return IndirectPriceListType;
        }
        public async Task<bool> Create(IndirectPriceListType IndirectPriceListType)
        {
            IndirectPriceListTypeDAO IndirectPriceListTypeDAO = new IndirectPriceListTypeDAO();
            IndirectPriceListTypeDAO.Id = IndirectPriceListType.Id;
            IndirectPriceListTypeDAO.Code = IndirectPriceListType.Code;
            IndirectPriceListTypeDAO.Name = IndirectPriceListType.Name;
            DataContext.IndirectPriceListType.Add(IndirectPriceListTypeDAO);
            await DataContext.SaveChangesAsync();
            IndirectPriceListType.Id = IndirectPriceListTypeDAO.Id;
            await SaveReference(IndirectPriceListType);
            return true;
        }

        public async Task<bool> Update(IndirectPriceListType IndirectPriceListType)
        {
            IndirectPriceListTypeDAO IndirectPriceListTypeDAO = DataContext.IndirectPriceListType.Where(x => x.Id == IndirectPriceListType.Id).FirstOrDefault();
            if (IndirectPriceListTypeDAO == null)
                return false;
            IndirectPriceListTypeDAO.Id = IndirectPriceListType.Id;
            IndirectPriceListTypeDAO.Code = IndirectPriceListType.Code;
            IndirectPriceListTypeDAO.Name = IndirectPriceListType.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(IndirectPriceListType);
            return true;
        }

        public async Task<bool> Delete(IndirectPriceListType IndirectPriceListType)
        {
            await DataContext.IndirectPriceListType.Where(x => x.Id == IndirectPriceListType.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<IndirectPriceListType> IndirectPriceListTypes)
        {
            List<IndirectPriceListTypeDAO> IndirectPriceListTypeDAOs = new List<IndirectPriceListTypeDAO>();
            foreach (IndirectPriceListType IndirectPriceListType in IndirectPriceListTypes)
            {
                IndirectPriceListTypeDAO IndirectPriceListTypeDAO = new IndirectPriceListTypeDAO();
                IndirectPriceListTypeDAO.Id = IndirectPriceListType.Id;
                IndirectPriceListTypeDAO.Code = IndirectPriceListType.Code;
                IndirectPriceListTypeDAO.Name = IndirectPriceListType.Name;
                IndirectPriceListTypeDAOs.Add(IndirectPriceListTypeDAO);
            }
            await DataContext.BulkMergeAsync(IndirectPriceListTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<IndirectPriceListType> IndirectPriceListTypes)
        {
            List<long> Ids = IndirectPriceListTypes.Select(x => x.Id).ToList();
            await DataContext.IndirectPriceListType
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(IndirectPriceListType IndirectPriceListType)
        {
        }

    }
}
