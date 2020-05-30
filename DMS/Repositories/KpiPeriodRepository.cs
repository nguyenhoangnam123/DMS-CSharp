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
    public interface IKpiPeriodRepository
    {
        Task<int> Count(KpiPeriodFilter KpiPeriodFilter);
        Task<List<KpiPeriod>> List(KpiPeriodFilter KpiPeriodFilter);
        Task<KpiPeriod> Get(long Id);
        Task<bool> Create(KpiPeriod KpiPeriod);
        Task<bool> Update(KpiPeriod KpiPeriod);
        Task<bool> Delete(KpiPeriod KpiPeriod);
        Task<bool> BulkMerge(List<KpiPeriod> KpiPeriods);
        Task<bool> BulkDelete(List<KpiPeriod> KpiPeriods);
    }
    public class KpiPeriodRepository : IKpiPeriodRepository
    {
        private DataContext DataContext;
        public KpiPeriodRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiPeriodDAO> DynamicFilter(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
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

         private IQueryable<KpiPeriodDAO> OrFilter(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiPeriodDAO> initQuery = query.Where(q => false);
            foreach (KpiPeriodFilter KpiPeriodFilter in filter.OrFilter)
            {
                IQueryable<KpiPeriodDAO> queryable = query;
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

        private IQueryable<KpiPeriodDAO> DynamicOrder(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiPeriodOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiPeriodOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case KpiPeriodOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiPeriodOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiPeriodOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case KpiPeriodOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiPeriod>> DynamicSelect(IQueryable<KpiPeriodDAO> query, KpiPeriodFilter filter)
        {
            List<KpiPeriod> KpiPeriods = await query.Select(q => new KpiPeriod()
            {
                Id = filter.Selects.Contains(KpiPeriodSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(KpiPeriodSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(KpiPeriodSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return KpiPeriods;
        }

        public async Task<int> Count(KpiPeriodFilter filter)
        {
            IQueryable<KpiPeriodDAO> KpiPeriods = DataContext.KpiPeriod.AsNoTracking();
            KpiPeriods = DynamicFilter(KpiPeriods, filter);
            return await KpiPeriods.CountAsync();
        }

        public async Task<List<KpiPeriod>> List(KpiPeriodFilter filter)
        {
            if (filter == null) return new List<KpiPeriod>();
            IQueryable<KpiPeriodDAO> KpiPeriodDAOs = DataContext.KpiPeriod.AsNoTracking();
            KpiPeriodDAOs = DynamicFilter(KpiPeriodDAOs, filter);
            KpiPeriodDAOs = DynamicOrder(KpiPeriodDAOs, filter);
            List<KpiPeriod> KpiPeriods = await DynamicSelect(KpiPeriodDAOs, filter);
            return KpiPeriods;
        }

        public async Task<KpiPeriod> Get(long Id)
        {
            KpiPeriod KpiPeriod = await DataContext.KpiPeriod.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new KpiPeriod()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (KpiPeriod == null)
                return null;
            KpiPeriod.ItemSpecificKpis = await DataContext.ItemSpecificKpi.AsNoTracking()
                .Where(x => x.KpiPeriodId == KpiPeriod.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new ItemSpecificKpi
                {
                    Id = x.Id,
                    OrganizationId = x.OrganizationId,
                    KpiPeriodId = x.KpiPeriodId,
                    StatusId = x.StatusId,
                    EmployeeId = x.EmployeeId,
                    CreatorId = x.CreatorId,
                    Organization = new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        ParentId = x.Organization.ParentId,
                        Path = x.Organization.Path,
                        Level = x.Organization.Level,
                        StatusId = x.Organization.StatusId,
                        Phone = x.Organization.Phone,
                        Email = x.Organization.Email,
                        Address = x.Organization.Address,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToListAsync();

            return KpiPeriod;
        }
        public async Task<bool> Create(KpiPeriod KpiPeriod)
        {
            KpiPeriodDAO KpiPeriodDAO = new KpiPeriodDAO();
            KpiPeriodDAO.Id = KpiPeriod.Id;
            KpiPeriodDAO.Code = KpiPeriod.Code;
            KpiPeriodDAO.Name = KpiPeriod.Name;
            DataContext.KpiPeriod.Add(KpiPeriodDAO);
            await DataContext.SaveChangesAsync();
            KpiPeriod.Id = KpiPeriodDAO.Id;
            await SaveReference(KpiPeriod);
            return true;
        }

        public async Task<bool> Update(KpiPeriod KpiPeriod)
        {
            KpiPeriodDAO KpiPeriodDAO = DataContext.KpiPeriod.Where(x => x.Id == KpiPeriod.Id).FirstOrDefault();
            if (KpiPeriodDAO == null)
                return false;
            KpiPeriodDAO.Id = KpiPeriod.Id;
            KpiPeriodDAO.Code = KpiPeriod.Code;
            KpiPeriodDAO.Name = KpiPeriod.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(KpiPeriod);
            return true;
        }

        public async Task<bool> Delete(KpiPeriod KpiPeriod)
        {
            await DataContext.KpiPeriod.Where(x => x.Id == KpiPeriod.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<KpiPeriod> KpiPeriods)
        {
            List<KpiPeriodDAO> KpiPeriodDAOs = new List<KpiPeriodDAO>();
            foreach (KpiPeriod KpiPeriod in KpiPeriods)
            {
                KpiPeriodDAO KpiPeriodDAO = new KpiPeriodDAO();
                KpiPeriodDAO.Id = KpiPeriod.Id;
                KpiPeriodDAO.Code = KpiPeriod.Code;
                KpiPeriodDAO.Name = KpiPeriod.Name;
                KpiPeriodDAOs.Add(KpiPeriodDAO);
            }
            await DataContext.BulkMergeAsync(KpiPeriodDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<KpiPeriod> KpiPeriods)
        {
            List<long> Ids = KpiPeriods.Select(x => x.Id).ToList();
            await DataContext.KpiPeriod
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(KpiPeriod KpiPeriod)
        {
            List<ItemSpecificKpiDAO> ItemSpecificKpiDAOs = await DataContext.ItemSpecificKpi
                .Where(x => x.KpiPeriodId == KpiPeriod.Id).ToListAsync();
            ItemSpecificKpiDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (KpiPeriod.ItemSpecificKpis != null)
            {
                foreach (ItemSpecificKpi ItemSpecificKpi in KpiPeriod.ItemSpecificKpis)
                {
                    ItemSpecificKpiDAO ItemSpecificKpiDAO = ItemSpecificKpiDAOs
                        .Where(x => x.Id == ItemSpecificKpi.Id && x.Id != 0).FirstOrDefault();
                    if (ItemSpecificKpiDAO == null)
                    {
                        ItemSpecificKpiDAO = new ItemSpecificKpiDAO();
                        ItemSpecificKpiDAO.Id = ItemSpecificKpi.Id;
                        ItemSpecificKpiDAO.OrganizationId = ItemSpecificKpi.OrganizationId;
                        ItemSpecificKpiDAO.KpiPeriodId = KpiPeriod.Id;
                        ItemSpecificKpiDAO.StatusId = ItemSpecificKpi.StatusId;
                        ItemSpecificKpiDAO.EmployeeId = ItemSpecificKpi.EmployeeId;
                        ItemSpecificKpiDAO.CreatorId = ItemSpecificKpi.CreatorId;
                        ItemSpecificKpiDAOs.Add(ItemSpecificKpiDAO);
                        ItemSpecificKpiDAO.CreatedAt = StaticParams.DateTimeNow;
                        ItemSpecificKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ItemSpecificKpiDAO.DeletedAt = null;
                    }
                    else
                    {
                        ItemSpecificKpiDAO.Id = ItemSpecificKpi.Id;
                        ItemSpecificKpiDAO.OrganizationId = ItemSpecificKpi.OrganizationId;
                        ItemSpecificKpiDAO.KpiPeriodId = KpiPeriod.Id;
                        ItemSpecificKpiDAO.StatusId = ItemSpecificKpi.StatusId;
                        ItemSpecificKpiDAO.EmployeeId = ItemSpecificKpi.EmployeeId;
                        ItemSpecificKpiDAO.CreatorId = ItemSpecificKpi.CreatorId;
                        ItemSpecificKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ItemSpecificKpiDAO.DeletedAt = null;
                    }
                }
                await DataContext.ItemSpecificKpi.BulkMergeAsync(ItemSpecificKpiDAOs);
            }
        }
        
    }
}
