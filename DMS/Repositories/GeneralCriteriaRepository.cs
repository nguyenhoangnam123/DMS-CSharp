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
    public interface IGeneralCriteriaRepository
    {
        Task<int> Count(GeneralCriteriaFilter GeneralCriteriaFilter);
        Task<List<GeneralCriteria>> List(GeneralCriteriaFilter GeneralCriteriaFilter);
        Task<GeneralCriteria> Get(long Id);
        Task<bool> Create(GeneralCriteria GeneralCriteria);
        Task<bool> Update(GeneralCriteria GeneralCriteria);
        Task<bool> Delete(GeneralCriteria GeneralCriteria);
        Task<bool> BulkMerge(List<GeneralCriteria> GeneralCriterias);
        Task<bool> BulkDelete(List<GeneralCriteria> GeneralCriterias);
    }
    public class GeneralCriteriaRepository : IGeneralCriteriaRepository
    {
        private DataContext DataContext;
        public GeneralCriteriaRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<GeneralCriteriaDAO> DynamicFilter(IQueryable<GeneralCriteriaDAO> query, GeneralCriteriaFilter filter)
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

         private IQueryable<GeneralCriteriaDAO> OrFilter(IQueryable<GeneralCriteriaDAO> query, GeneralCriteriaFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<GeneralCriteriaDAO> initQuery = query.Where(q => false);
            foreach (GeneralCriteriaFilter GeneralCriteriaFilter in filter.OrFilter)
            {
                IQueryable<GeneralCriteriaDAO> queryable = query;
                if (GeneralCriteriaFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, GeneralCriteriaFilter.Id);
                if (GeneralCriteriaFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, GeneralCriteriaFilter.Code);
                if (GeneralCriteriaFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, GeneralCriteriaFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<GeneralCriteriaDAO> DynamicOrder(IQueryable<GeneralCriteriaDAO> query, GeneralCriteriaFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case GeneralCriteriaOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case GeneralCriteriaOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case GeneralCriteriaOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case GeneralCriteriaOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case GeneralCriteriaOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case GeneralCriteriaOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<GeneralCriteria>> DynamicSelect(IQueryable<GeneralCriteriaDAO> query, GeneralCriteriaFilter filter)
        {
            List<GeneralCriteria> GeneralCriterias = await query.Select(q => new GeneralCriteria()
            {
                Id = filter.Selects.Contains(GeneralCriteriaSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(GeneralCriteriaSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(GeneralCriteriaSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return GeneralCriterias;
        }

        public async Task<int> Count(GeneralCriteriaFilter filter)
        {
            IQueryable<GeneralCriteriaDAO> GeneralCriterias = DataContext.GeneralCriteria.AsNoTracking();
            GeneralCriterias = DynamicFilter(GeneralCriterias, filter);
            return await GeneralCriterias.CountAsync();
        }

        public async Task<List<GeneralCriteria>> List(GeneralCriteriaFilter filter)
        {
            if (filter == null) return new List<GeneralCriteria>();
            IQueryable<GeneralCriteriaDAO> GeneralCriteriaDAOs = DataContext.GeneralCriteria.AsNoTracking();
            GeneralCriteriaDAOs = DynamicFilter(GeneralCriteriaDAOs, filter);
            GeneralCriteriaDAOs = DynamicOrder(GeneralCriteriaDAOs, filter);
            List<GeneralCriteria> GeneralCriterias = await DynamicSelect(GeneralCriteriaDAOs, filter);
            return GeneralCriterias;
        }

        public async Task<GeneralCriteria> Get(long Id)
        {
            GeneralCriteria GeneralCriteria = await DataContext.GeneralCriteria.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new GeneralCriteria()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (GeneralCriteria == null)
                return null;
            GeneralCriteria.GeneralKpiCriteriaMappings = await DataContext.GeneralKpiCriteriaMapping.AsNoTracking()
                .Where(x => x.GeneralCriteriaId == GeneralCriteria.Id)
                .Where(x => x.GeneralKpi.DeletedAt == null)
                .Select(x => new GeneralKpiCriteriaMapping
                {
                    GeneralKpiId = x.GeneralKpiId,
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
                    StatusId = x.StatusId,
                    GeneralKpi = new GeneralKpi
                    {
                        Id = x.GeneralKpi.Id,
                        OrganizationId = x.GeneralKpi.OrganizationId,
                        EmployeeId = x.GeneralKpi.EmployeeId,
                        KpiPeriodId = x.GeneralKpi.KpiPeriodId,
                        StatusId = x.GeneralKpi.StatusId,
                        CreatorId = x.GeneralKpi.CreatorId,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToListAsync();

            return GeneralCriteria;
        }
        public async Task<bool> Create(GeneralCriteria GeneralCriteria)
        {
            GeneralCriteriaDAO GeneralCriteriaDAO = new GeneralCriteriaDAO();
            GeneralCriteriaDAO.Id = GeneralCriteria.Id;
            GeneralCriteriaDAO.Code = GeneralCriteria.Code;
            GeneralCriteriaDAO.Name = GeneralCriteria.Name;
            DataContext.GeneralCriteria.Add(GeneralCriteriaDAO);
            await DataContext.SaveChangesAsync();
            GeneralCriteria.Id = GeneralCriteriaDAO.Id;
            await SaveReference(GeneralCriteria);
            return true;
        }

        public async Task<bool> Update(GeneralCriteria GeneralCriteria)
        {
            GeneralCriteriaDAO GeneralCriteriaDAO = DataContext.GeneralCriteria.Where(x => x.Id == GeneralCriteria.Id).FirstOrDefault();
            if (GeneralCriteriaDAO == null)
                return false;
            GeneralCriteriaDAO.Id = GeneralCriteria.Id;
            GeneralCriteriaDAO.Code = GeneralCriteria.Code;
            GeneralCriteriaDAO.Name = GeneralCriteria.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(GeneralCriteria);
            return true;
        }

        public async Task<bool> Delete(GeneralCriteria GeneralCriteria)
        {
            await DataContext.GeneralCriteria.Where(x => x.Id == GeneralCriteria.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<GeneralCriteria> GeneralCriterias)
        {
            List<GeneralCriteriaDAO> GeneralCriteriaDAOs = new List<GeneralCriteriaDAO>();
            foreach (GeneralCriteria GeneralCriteria in GeneralCriterias)
            {
                GeneralCriteriaDAO GeneralCriteriaDAO = new GeneralCriteriaDAO();
                GeneralCriteriaDAO.Id = GeneralCriteria.Id;
                GeneralCriteriaDAO.Code = GeneralCriteria.Code;
                GeneralCriteriaDAO.Name = GeneralCriteria.Name;
                GeneralCriteriaDAOs.Add(GeneralCriteriaDAO);
            }
            await DataContext.BulkMergeAsync(GeneralCriteriaDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<GeneralCriteria> GeneralCriterias)
        {
            List<long> Ids = GeneralCriterias.Select(x => x.Id).ToList();
            await DataContext.GeneralCriteria
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(GeneralCriteria GeneralCriteria)
        {
            await DataContext.GeneralKpiCriteriaMapping
                .Where(x => x.GeneralCriteriaId == GeneralCriteria.Id)
                .DeleteFromQueryAsync();
            List<GeneralKpiCriteriaMappingDAO> GeneralKpiCriteriaMappingDAOs = new List<GeneralKpiCriteriaMappingDAO>();
            if (GeneralCriteria.GeneralKpiCriteriaMappings != null)
            {
                foreach (GeneralKpiCriteriaMapping GeneralKpiCriteriaMapping in GeneralCriteria.GeneralKpiCriteriaMappings)
                {
                    GeneralKpiCriteriaMappingDAO GeneralKpiCriteriaMappingDAO = new GeneralKpiCriteriaMappingDAO();
                    GeneralKpiCriteriaMappingDAO.GeneralKpiId = GeneralKpiCriteriaMapping.GeneralKpiId;
                    GeneralKpiCriteriaMappingDAO.GeneralCriteriaId = GeneralCriteria.Id;
                    GeneralKpiCriteriaMappingDAO.M01 = GeneralKpiCriteriaMapping.M01;
                    GeneralKpiCriteriaMappingDAO.M02 = GeneralKpiCriteriaMapping.M02;
                    GeneralKpiCriteriaMappingDAO.M03 = GeneralKpiCriteriaMapping.M03;
                    GeneralKpiCriteriaMappingDAO.M04 = GeneralKpiCriteriaMapping.M04;
                    GeneralKpiCriteriaMappingDAO.M05 = GeneralKpiCriteriaMapping.M05;
                    GeneralKpiCriteriaMappingDAO.M06 = GeneralKpiCriteriaMapping.M06;
                    GeneralKpiCriteriaMappingDAO.M07 = GeneralKpiCriteriaMapping.M07;
                    GeneralKpiCriteriaMappingDAO.M08 = GeneralKpiCriteriaMapping.M08;
                    GeneralKpiCriteriaMappingDAO.M09 = GeneralKpiCriteriaMapping.M09;
                    GeneralKpiCriteriaMappingDAO.M10 = GeneralKpiCriteriaMapping.M10;
                    GeneralKpiCriteriaMappingDAO.M11 = GeneralKpiCriteriaMapping.M11;
                    GeneralKpiCriteriaMappingDAO.M12 = GeneralKpiCriteriaMapping.M12;
                    GeneralKpiCriteriaMappingDAO.Q01 = GeneralKpiCriteriaMapping.Q01;
                    GeneralKpiCriteriaMappingDAO.Q02 = GeneralKpiCriteriaMapping.Q02;
                    GeneralKpiCriteriaMappingDAO.Q03 = GeneralKpiCriteriaMapping.Q03;
                    GeneralKpiCriteriaMappingDAO.Q04 = GeneralKpiCriteriaMapping.Q04;
                    GeneralKpiCriteriaMappingDAO.Y01 = GeneralKpiCriteriaMapping.Y01;
                    GeneralKpiCriteriaMappingDAO.StatusId = GeneralKpiCriteriaMapping.StatusId;
                    GeneralKpiCriteriaMappingDAOs.Add(GeneralKpiCriteriaMappingDAO);
                }
                await DataContext.GeneralKpiCriteriaMapping.BulkMergeAsync(GeneralKpiCriteriaMappingDAOs);
            }
        }
        
    }
}
