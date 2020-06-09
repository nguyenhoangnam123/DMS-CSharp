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
    public interface IDirectPriceListTypeRepository
    {
        Task<int> Count(DirectPriceListTypeFilter DirectPriceListTypeFilter);
        Task<List<DirectPriceListType>> List(DirectPriceListTypeFilter DirectPriceListTypeFilter);
        Task<DirectPriceListType> Get(long Id);
        Task<bool> Create(DirectPriceListType DirectPriceListType);
        Task<bool> Update(DirectPriceListType DirectPriceListType);
        Task<bool> Delete(DirectPriceListType DirectPriceListType);
        Task<bool> BulkMerge(List<DirectPriceListType> DirectPriceListTypes);
        Task<bool> BulkDelete(List<DirectPriceListType> DirectPriceListTypes);
    }
    public class DirectPriceListTypeRepository : IDirectPriceListTypeRepository
    {
        private DataContext DataContext;
        public DirectPriceListTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<DirectPriceListTypeDAO> DynamicFilter(IQueryable<DirectPriceListTypeDAO> query, DirectPriceListTypeFilter filter)
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

        private IQueryable<DirectPriceListTypeDAO> OrFilter(IQueryable<DirectPriceListTypeDAO> query, DirectPriceListTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<DirectPriceListTypeDAO> initQuery = query.Where(q => false);
            foreach (DirectPriceListTypeFilter DirectPriceListTypeFilter in filter.OrFilter)
            {
                IQueryable<DirectPriceListTypeDAO> queryable = query;
                if (DirectPriceListTypeFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, DirectPriceListTypeFilter.Id);
                if (DirectPriceListTypeFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, DirectPriceListTypeFilter.Code);
                if (DirectPriceListTypeFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, DirectPriceListTypeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<DirectPriceListTypeDAO> DynamicOrder(IQueryable<DirectPriceListTypeDAO> query, DirectPriceListTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case DirectPriceListTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case DirectPriceListTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case DirectPriceListTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case DirectPriceListTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case DirectPriceListTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case DirectPriceListTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<DirectPriceListType>> DynamicSelect(IQueryable<DirectPriceListTypeDAO> query, DirectPriceListTypeFilter filter)
        {
            List<DirectPriceListType> DirectPriceListTypes = await query.Select(q => new DirectPriceListType()
            {
                Id = filter.Selects.Contains(DirectPriceListTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(DirectPriceListTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(DirectPriceListTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return DirectPriceListTypes;
        }

        public async Task<int> Count(DirectPriceListTypeFilter filter)
        {
            IQueryable<DirectPriceListTypeDAO> DirectPriceListTypes = DataContext.DirectPriceListType.AsNoTracking();
            DirectPriceListTypes = DynamicFilter(DirectPriceListTypes, filter);
            return await DirectPriceListTypes.CountAsync();
        }

        public async Task<List<DirectPriceListType>> List(DirectPriceListTypeFilter filter)
        {
            if (filter == null) return new List<DirectPriceListType>();
            IQueryable<DirectPriceListTypeDAO> DirectPriceListTypeDAOs = DataContext.DirectPriceListType.AsNoTracking();
            DirectPriceListTypeDAOs = DynamicFilter(DirectPriceListTypeDAOs, filter);
            DirectPriceListTypeDAOs = DynamicOrder(DirectPriceListTypeDAOs, filter);
            List<DirectPriceListType> DirectPriceListTypes = await DynamicSelect(DirectPriceListTypeDAOs, filter);
            return DirectPriceListTypes;
        }

        public async Task<DirectPriceListType> Get(long Id)
        {
            DirectPriceListType DirectPriceListType = await DataContext.DirectPriceListType.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new DirectPriceListType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (DirectPriceListType == null)
                return null;

            return DirectPriceListType;
        }
        public async Task<bool> Create(DirectPriceListType DirectPriceListType)
        {
            DirectPriceListTypeDAO DirectPriceListTypeDAO = new DirectPriceListTypeDAO();
            DirectPriceListTypeDAO.Id = DirectPriceListType.Id;
            DirectPriceListTypeDAO.Code = DirectPriceListType.Code;
            DirectPriceListTypeDAO.Name = DirectPriceListType.Name;
            DataContext.DirectPriceListType.Add(DirectPriceListTypeDAO);
            await DataContext.SaveChangesAsync();
            DirectPriceListType.Id = DirectPriceListTypeDAO.Id;
            await SaveReference(DirectPriceListType);
            return true;
        }

        public async Task<bool> Update(DirectPriceListType DirectPriceListType)
        {
            DirectPriceListTypeDAO DirectPriceListTypeDAO = DataContext.DirectPriceListType.Where(x => x.Id == DirectPriceListType.Id).FirstOrDefault();
            if (DirectPriceListTypeDAO == null)
                return false;
            DirectPriceListTypeDAO.Id = DirectPriceListType.Id;
            DirectPriceListTypeDAO.Code = DirectPriceListType.Code;
            DirectPriceListTypeDAO.Name = DirectPriceListType.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(DirectPriceListType);
            return true;
        }

        public async Task<bool> Delete(DirectPriceListType DirectPriceListType)
        {
            await DataContext.DirectPriceListType.Where(x => x.Id == DirectPriceListType.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<DirectPriceListType> DirectPriceListTypes)
        {
            List<DirectPriceListTypeDAO> DirectPriceListTypeDAOs = new List<DirectPriceListTypeDAO>();
            foreach (DirectPriceListType DirectPriceListType in DirectPriceListTypes)
            {
                DirectPriceListTypeDAO DirectPriceListTypeDAO = new DirectPriceListTypeDAO();
                DirectPriceListTypeDAO.Id = DirectPriceListType.Id;
                DirectPriceListTypeDAO.Code = DirectPriceListType.Code;
                DirectPriceListTypeDAO.Name = DirectPriceListType.Name;
                DirectPriceListTypeDAOs.Add(DirectPriceListTypeDAO);
            }
            await DataContext.BulkMergeAsync(DirectPriceListTypeDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<DirectPriceListType> DirectPriceListTypes)
        {
            List<long> Ids = DirectPriceListTypes.Select(x => x.Id).ToList();
            await DataContext.DirectPriceListType
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(DirectPriceListType DirectPriceListType)
        {
        }

    }
}
