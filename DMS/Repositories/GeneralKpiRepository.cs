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
                if (GeneralKpiFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, GeneralKpiFilter.Id);
                if (GeneralKpiFilter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, GeneralKpiFilter.OrganizationId);
                if (GeneralKpiFilter.EmployeeId != null)
                    queryable = queryable.Where(q => q.EmployeeId, GeneralKpiFilter.EmployeeId);
                if (GeneralKpiFilter.KpiPeriodId != null)
                    queryable = queryable.Where(q => q.KpiPeriodId, GeneralKpiFilter.KpiPeriodId);
                if (GeneralKpiFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, GeneralKpiFilter.StatusId);
                if (GeneralKpiFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, GeneralKpiFilter.CreatorId);
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
                Creator = filter.Selects.Contains(GeneralKpiSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    PositionId = q.Creator.PositionId,
                    Department = q.Creator.Department,
                    OrganizationId = q.Creator.OrganizationId,
                    StatusId = q.Creator.StatusId,
                    Avatar = q.Creator.Avatar,
                    ProvinceId = q.Creator.ProvinceId,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                } : null,
                Employee = filter.Selects.Contains(GeneralKpiSelect.Employee) && q.Employee != null ? new AppUser
                {
                    Id = q.Employee.Id,
                    Username = q.Employee.Username,
                    DisplayName = q.Employee.DisplayName,
                    Address = q.Employee.Address,
                    Email = q.Employee.Email,
                    Phone = q.Employee.Phone,
                    PositionId = q.Employee.PositionId,
                    Department = q.Employee.Department,
                    OrganizationId = q.Employee.OrganizationId,
                    StatusId = q.Employee.StatusId,
                    Avatar = q.Employee.Avatar,
                    ProvinceId = q.Employee.ProvinceId,
                    SexId = q.Employee.SexId,
                    Birthday = q.Employee.Birthday,
                } : null,
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
                Status = filter.Selects.Contains(GeneralKpiSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
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
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    PositionId = x.Creator.PositionId,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    StatusId = x.Creator.StatusId,
                    Avatar = x.Creator.Avatar,
                    ProvinceId = x.Creator.ProvinceId,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                },
                Employee = x.Employee == null ? null : new AppUser
                {
                    Id = x.Employee.Id,
                    Username = x.Employee.Username,
                    DisplayName = x.Employee.DisplayName,
                    Address = x.Employee.Address,
                    Email = x.Employee.Email,
                    Phone = x.Employee.Phone,
                    PositionId = x.Employee.PositionId,
                    Department = x.Employee.Department,
                    OrganizationId = x.Employee.OrganizationId,
                    StatusId = x.Employee.StatusId,
                    Avatar = x.Employee.Avatar,
                    ProvinceId = x.Employee.ProvinceId,
                    SexId = x.Employee.SexId,
                    Birthday = x.Employee.Birthday,
                },
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
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
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
            GeneralKpiDAO.RowId = Guid.NewGuid();
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
            await DataContext.GeneralKpiCriteriaMapping.Where(x => x.GeneralKpiId == GeneralKpi.Id).DeleteFromQueryAsync();
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
                GeneralKpiDAO.RowId = Guid.NewGuid();
                GeneralKpiDAOs.Add(GeneralKpiDAO);
                GeneralKpi.RowId = GeneralKpiDAO.RowId;
            }
            await DataContext.BulkMergeAsync(GeneralKpiDAOs);

            var GeneralKpiCriteriaMappingDAOs = new List<GeneralKpiCriteriaMappingDAO>();
            foreach (var GeneralKpi in GeneralKpis)
            {
                GeneralKpi.Id = GeneralKpiDAOs.Where(x => x.RowId == GeneralKpi.RowId).Select(x => x.Id).FirstOrDefault();
                if(GeneralKpi.GeneralKpiCriteriaMappings != null && GeneralKpi.GeneralKpiCriteriaMappings.Any())
                {
                    var list = GeneralKpi.GeneralKpiCriteriaMappings.Select(x => new GeneralKpiCriteriaMappingDAO
                    {
                        GeneralKpiId = GeneralKpi.Id,
                        GeneralCriteriaId = x.GeneralCriteriaId,
                        M01 = x.M01,
                        M02 = x.M02,
                        M03 = x.M03,
                        M04 = x.M04,
                        M05 = x.M05,
                        M06 = x.M06,
                        M07 = x.M07,
                        M08 = x.M08,
                        M09 = x.M09,
                        M10 = x.M10,
                        M11 = x.M11,
                        M12 = x.M12,
                        Q01 = x.Q01,
                        Q02 = x.Q02,
                        Q03 = x.Q03,
                        Q04 = x.Q04,
                        Y01 = x.Y01,
                        StatusId = x.StatusId
                    }).ToList();
                    GeneralKpiCriteriaMappingDAOs.AddRange(list);
                }
            }

            await DataContext.GeneralKpiCriteriaMapping.BulkMergeAsync(GeneralKpiCriteriaMappingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<GeneralKpi> GeneralKpis)
        {
            List<long> Ids = GeneralKpis.Select(x => x.Id).ToList();
            await DataContext.GeneralKpiCriteriaMapping.Where(x => Ids.Contains(x.GeneralKpiId)).DeleteFromQueryAsync();
            await DataContext.GeneralKpi
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new GeneralKpiDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(GeneralKpi GeneralKpi)
        {
            await DataContext.GeneralKpiCriteriaMapping.Where(x => x.GeneralKpiId == GeneralKpi.Id).DeleteFromQueryAsync();
            if(GeneralKpi.GeneralKpiCriteriaMappings != null)
            {
                List<GeneralKpiCriteriaMappingDAO> GeneralKpiCriteriaMappingDAOs = new List<GeneralKpiCriteriaMappingDAO>();
                foreach (var GeneralKpiCriteriaMapping in GeneralKpi.GeneralKpiCriteriaMappings)
                {
                    GeneralKpiCriteriaMappingDAO GeneralKpiCriteriaMappingDAO = new GeneralKpiCriteriaMappingDAO()
                    {
                        GeneralKpiId = GeneralKpi.Id,
                        GeneralCriteriaId = GeneralKpiCriteriaMapping.GeneralCriteriaId,
                        M01 = GeneralKpiCriteriaMapping.M01,
                        M02 = GeneralKpiCriteriaMapping.M02,
                        M03 = GeneralKpiCriteriaMapping.M03,
                        M04 = GeneralKpiCriteriaMapping.M04,
                        M05 = GeneralKpiCriteriaMapping.M05,
                        M06 = GeneralKpiCriteriaMapping.M06,
                        M07 = GeneralKpiCriteriaMapping.M07,
                        M08 = GeneralKpiCriteriaMapping.M08,
                        M09 = GeneralKpiCriteriaMapping.M09,
                        M10 = GeneralKpiCriteriaMapping.M10,
                        M11 = GeneralKpiCriteriaMapping.M11,
                        M12 = GeneralKpiCriteriaMapping.M12,
                        Q01 = GeneralKpiCriteriaMapping.Q01,
                        Q02 = GeneralKpiCriteriaMapping.Q02,
                        Q03 = GeneralKpiCriteriaMapping.Q03,
                        Q04 = GeneralKpiCriteriaMapping.Q04,
                        Y01 = GeneralKpiCriteriaMapping.Y01,
                        StatusId = GeneralKpiCriteriaMapping.StatusId
                    };
                    GeneralKpiCriteriaMappingDAOs.Add(GeneralKpiCriteriaMappingDAO);
                }
                await DataContext.GeneralKpiCriteriaMapping.BulkMergeAsync(GeneralKpiCriteriaMappingDAOs);
            }
        }
        
    }
}
