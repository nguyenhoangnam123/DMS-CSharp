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
    public interface IGeneralKpiRepository
    {
        Task<int> Count(GeneralKpiFilter GeneralKpiFilter);
        Task<List<GeneralKpi>> List(GeneralKpiFilter GeneralKpiFilter);
        Task<GeneralKpi> Get(long Id);
        Task<bool> Create(GeneralKpi GeneralKpi);
        Task<bool> Update(GeneralKpi GeneralKpi);
        Task<bool> Delete(GeneralKpi GeneralKpi);
        Task<bool> BulkMerge(List<GeneralKpi> GeneralKpis);
        Task<bool> BulkDelete(List<GeneralKpi> GeneralKpis);
    }
    public class GeneralKpiRepository : IGeneralKpiRepository
    {
        private DataContext DataContext;
        public GeneralKpiRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<GeneralKpiDAO> DynamicFilter(IQueryable<GeneralKpiDAO> query, GeneralKpiFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.EmployeeId != null)
                query = query.Where(q => q.EmployeeId, filter.EmployeeId);
            if (filter.KpiPeriodId != null)
                query = query.Where(q => q.KpiPeriodId, filter.KpiPeriodId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<GeneralKpiDAO> OrFilter(IQueryable<GeneralKpiDAO> query, GeneralKpiFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<GeneralKpiDAO> initQuery = query.Where(q => false);
            foreach (GeneralKpiFilter GeneralKpiFilter in filter.OrFilter)
            {
                IQueryable<GeneralKpiDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (filter.EmployeeId != null)
                    queryable = queryable.Where(q => q.EmployeeId, filter.EmployeeId);
                if (filter.KpiPeriodId != null)
                    queryable = queryable.Where(q => q.KpiPeriodId, filter.KpiPeriodId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, filter.CreatorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<GeneralKpiDAO> DynamicOrder(IQueryable<GeneralKpiDAO> query, GeneralKpiFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case GeneralKpiOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case GeneralKpiOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case GeneralKpiOrder.Employee:
                            query = query.OrderBy(q => q.EmployeeId);
                            break;
                        case GeneralKpiOrder.KpiPeriod:
                            query = query.OrderBy(q => q.KpiPeriodId);
                            break;
                        case GeneralKpiOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case GeneralKpiOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case GeneralKpiOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case GeneralKpiOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case GeneralKpiOrder.Employee:
                            query = query.OrderByDescending(q => q.EmployeeId);
                            break;
                        case GeneralKpiOrder.KpiPeriod:
                            query = query.OrderByDescending(q => q.KpiPeriodId);
                            break;
                        case GeneralKpiOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case GeneralKpiOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<GeneralKpi>> DynamicSelect(IQueryable<GeneralKpiDAO> query, GeneralKpiFilter filter)
        {
            List<GeneralKpi> GeneralKpis = await query.Select(q => new GeneralKpi()
            {
                Id = filter.Selects.Contains(GeneralKpiSelect.Id) ? q.Id : default(long),
                OrganizationId = filter.Selects.Contains(GeneralKpiSelect.Organization) ? q.OrganizationId : default(long),
                EmployeeId = filter.Selects.Contains(GeneralKpiSelect.Employee) ? q.EmployeeId : default(long),
                KpiPeriodId = filter.Selects.Contains(GeneralKpiSelect.KpiPeriod) ? q.KpiPeriodId : default(long),
                StatusId = filter.Selects.Contains(GeneralKpiSelect.Status) ? q.StatusId : default(long),
                CreatorId = filter.Selects.Contains(GeneralKpiSelect.Creator) ? q.CreatorId : default(long),
                KpiPeriod = filter.Selects.Contains(GeneralKpiSelect.KpiPeriod) && q.KpiPeriod != null ? new KpiPeriod
                {
                    Id = q.KpiPeriod.Id,
                    Code = q.KpiPeriod.Code,
                    Name = q.KpiPeriod.Name,
                } : null,
                Organization = filter.Selects.Contains(GeneralKpiSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Email = q.Organization.Email,
                    Address = q.Organization.Address,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return GeneralKpis;
        }

        public async Task<int> Count(GeneralKpiFilter filter)
        {
            IQueryable<GeneralKpiDAO> GeneralKpis = DataContext.GeneralKpi.AsNoTracking();
            GeneralKpis = DynamicFilter(GeneralKpis, filter);
            return await GeneralKpis.CountAsync();
        }

        public async Task<List<GeneralKpi>> List(GeneralKpiFilter filter)
        {
            if (filter == null) return new List<GeneralKpi>();
            IQueryable<GeneralKpiDAO> GeneralKpiDAOs = DataContext.GeneralKpi.AsNoTracking();
            GeneralKpiDAOs = DynamicFilter(GeneralKpiDAOs, filter);
            GeneralKpiDAOs = DynamicOrder(GeneralKpiDAOs, filter);
            List<GeneralKpi> GeneralKpis = await DynamicSelect(GeneralKpiDAOs, filter);
            return GeneralKpis;
        }

        public async Task<GeneralKpi> Get(long Id)
        {
            GeneralKpi GeneralKpi = await DataContext.GeneralKpi.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new GeneralKpi()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                OrganizationId = x.OrganizationId,
                EmployeeId = x.EmployeeId,
                KpiPeriodId = x.KpiPeriodId,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                KpiPeriod = x.KpiPeriod == null ? null : new KpiPeriod
                {
                    Id = x.KpiPeriod.Id,
                    Code = x.KpiPeriod.Code,
                    Name = x.KpiPeriod.Name,
                },
                Organization = x.Organization == null ? null : new Organization
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
            }).FirstOrDefaultAsync();

            if (GeneralKpi == null)
                return null;

            return GeneralKpi;
        }
        public async Task<bool> Create(GeneralKpi GeneralKpi)
        {
            GeneralKpiDAO GeneralKpiDAO = new GeneralKpiDAO();
            GeneralKpiDAO.Id = GeneralKpi.Id;
            GeneralKpiDAO.OrganizationId = GeneralKpi.OrganizationId;
            GeneralKpiDAO.EmployeeId = GeneralKpi.EmployeeId;
            GeneralKpiDAO.KpiPeriodId = GeneralKpi.KpiPeriodId;
            GeneralKpiDAO.StatusId = GeneralKpi.StatusId;
            GeneralKpiDAO.CreatorId = GeneralKpi.CreatorId;
            GeneralKpiDAO.CreatedAt = StaticParams.DateTimeNow;
            GeneralKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.GeneralKpi.Add(GeneralKpiDAO);
            await DataContext.SaveChangesAsync();
            GeneralKpi.Id = GeneralKpiDAO.Id;
            await SaveReference(GeneralKpi);
            return true;
        }

        public async Task<bool> Update(GeneralKpi GeneralKpi)
        {
            GeneralKpiDAO GeneralKpiDAO = DataContext.GeneralKpi.Where(x => x.Id == GeneralKpi.Id).FirstOrDefault();
            if (GeneralKpiDAO == null)
                return false;
            GeneralKpiDAO.Id = GeneralKpi.Id;
            GeneralKpiDAO.OrganizationId = GeneralKpi.OrganizationId;
            GeneralKpiDAO.EmployeeId = GeneralKpi.EmployeeId;
            GeneralKpiDAO.KpiPeriodId = GeneralKpi.KpiPeriodId;
            GeneralKpiDAO.StatusId = GeneralKpi.StatusId;
            GeneralKpiDAO.CreatorId = GeneralKpi.CreatorId;
            GeneralKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(GeneralKpi);
            return true;
        }

        public async Task<bool> Delete(GeneralKpi GeneralKpi)
        {
            await DataContext.GeneralKpi.Where(x => x.Id == GeneralKpi.Id).UpdateFromQueryAsync(x => new GeneralKpiDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<GeneralKpi> GeneralKpis)
        {
            List<GeneralKpiDAO> GeneralKpiDAOs = new List<GeneralKpiDAO>();
            foreach (GeneralKpi GeneralKpi in GeneralKpis)
            {
                GeneralKpiDAO GeneralKpiDAO = new GeneralKpiDAO();
                GeneralKpiDAO.Id = GeneralKpi.Id;
                GeneralKpiDAO.OrganizationId = GeneralKpi.OrganizationId;
                GeneralKpiDAO.EmployeeId = GeneralKpi.EmployeeId;
                GeneralKpiDAO.KpiPeriodId = GeneralKpi.KpiPeriodId;
                GeneralKpiDAO.StatusId = GeneralKpi.StatusId;
                GeneralKpiDAO.CreatorId = GeneralKpi.CreatorId;
                GeneralKpiDAO.CreatedAt = StaticParams.DateTimeNow;
                GeneralKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
                GeneralKpiDAOs.Add(GeneralKpiDAO);
            }
            await DataContext.BulkMergeAsync(GeneralKpiDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<GeneralKpi> GeneralKpis)
        {
            List<long> Ids = GeneralKpis.Select(x => x.Id).ToList();
            await DataContext.GeneralKpi
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new GeneralKpiDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(GeneralKpi GeneralKpi)
        {
        }
        
    }
}
