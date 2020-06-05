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
    public interface IResellerStatusRepository
    {
        Task<int> Count(ResellerStatusFilter ResellerStatusFilter);
        Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter);
        Task<ResellerStatus> Get(long Id);
        Task<bool> Create(ResellerStatus ResellerStatus);
        Task<bool> Update(ResellerStatus ResellerStatus);
        Task<bool> Delete(ResellerStatus ResellerStatus);
        Task<bool> BulkMerge(List<ResellerStatus> ResellerStatuses);
        Task<bool> BulkDelete(List<ResellerStatus> ResellerStatuses);
    }
    public class ResellerStatusRepository : IResellerStatusRepository
    {
        private DataContext DataContext;
        public ResellerStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ResellerStatusDAO> DynamicFilter(IQueryable<ResellerStatusDAO> query, ResellerStatusFilter filter)
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

         private IQueryable<ResellerStatusDAO> OrFilter(IQueryable<ResellerStatusDAO> query, ResellerStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ResellerStatusDAO> initQuery = query.Where(q => false);
            foreach (ResellerStatusFilter ResellerStatusFilter in filter.OrFilter)
            {
                IQueryable<ResellerStatusDAO> queryable = query;
                if (ResellerStatusFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ResellerStatusFilter.Id);
                if (ResellerStatusFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ResellerStatusFilter.Code);
                if (ResellerStatusFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ResellerStatusFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ResellerStatusDAO> DynamicOrder(IQueryable<ResellerStatusDAO> query, ResellerStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ResellerStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ResellerStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ResellerStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ResellerStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ResellerStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ResellerStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ResellerStatus>> DynamicSelect(IQueryable<ResellerStatusDAO> query, ResellerStatusFilter filter)
        {
            List<ResellerStatus> ResellerStatuses = await query.Select(q => new ResellerStatus()
            {
                Id = filter.Selects.Contains(ResellerStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ResellerStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ResellerStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ResellerStatuses;
        }

        public async Task<int> Count(ResellerStatusFilter filter)
        {
            IQueryable<ResellerStatusDAO> ResellerStatuses = DataContext.ResellerStatus;
            ResellerStatuses = DynamicFilter(ResellerStatuses, filter);
            return await ResellerStatuses.CountAsync();
        }

        public async Task<List<ResellerStatus>> List(ResellerStatusFilter filter)
        {
            if (filter == null) return new List<ResellerStatus>();
            IQueryable<ResellerStatusDAO> ResellerStatusDAOs = DataContext.ResellerStatus.AsNoTracking();
            ResellerStatusDAOs = DynamicFilter(ResellerStatusDAOs, filter);
            ResellerStatusDAOs = DynamicOrder(ResellerStatusDAOs, filter);
            List<ResellerStatus> ResellerStatuses = await DynamicSelect(ResellerStatusDAOs, filter);
            return ResellerStatuses;
        }

        public async Task<ResellerStatus> Get(long Id)
        {
            ResellerStatus ResellerStatus = await DataContext.ResellerStatus.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new ResellerStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (ResellerStatus == null)
                return null;

            return ResellerStatus;
        }
        public async Task<bool> Create(ResellerStatus ResellerStatus)
        {
            ResellerStatusDAO ResellerStatusDAO = new ResellerStatusDAO();
            ResellerStatusDAO.Id = ResellerStatus.Id;
            ResellerStatusDAO.Code = ResellerStatus.Code;
            ResellerStatusDAO.Name = ResellerStatus.Name;
            DataContext.ResellerStatus.Add(ResellerStatusDAO);
            await DataContext.SaveChangesAsync();
            ResellerStatus.Id = ResellerStatusDAO.Id;
            await SaveReference(ResellerStatus);
            return true;
        }

        public async Task<bool> Update(ResellerStatus ResellerStatus)
        {
            ResellerStatusDAO ResellerStatusDAO = DataContext.ResellerStatus.Where(x => x.Id == ResellerStatus.Id).FirstOrDefault();
            if (ResellerStatusDAO == null)
                return false;
            ResellerStatusDAO.Id = ResellerStatus.Id;
            ResellerStatusDAO.Code = ResellerStatus.Code;
            ResellerStatusDAO.Name = ResellerStatus.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(ResellerStatus);
            return true;
        }

        public async Task<bool> Delete(ResellerStatus ResellerStatus)
        {
            await DataContext.ResellerStatus.Where(x => x.Id == ResellerStatus.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ResellerStatus> ResellerStatuses)
        {
            List<ResellerStatusDAO> ResellerStatusDAOs = new List<ResellerStatusDAO>();
            foreach (ResellerStatus ResellerStatus in ResellerStatuses)
            {
                ResellerStatusDAO ResellerStatusDAO = new ResellerStatusDAO();
                ResellerStatusDAO.Id = ResellerStatus.Id;
                ResellerStatusDAO.Code = ResellerStatus.Code;
                ResellerStatusDAO.Name = ResellerStatus.Name;
                ResellerStatusDAOs.Add(ResellerStatusDAO);
            }
            await DataContext.BulkMergeAsync(ResellerStatusDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ResellerStatus> ResellerStatuses)
        {
            List<long> Ids = ResellerStatuses.Select(x => x.Id).ToList();
            await DataContext.ResellerStatus
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ResellerStatus ResellerStatus)
        {
        }
        
    }
}
