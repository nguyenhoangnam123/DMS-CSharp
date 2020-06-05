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
    public interface IWorkflowParameterRepository
    {
        Task<int> Count(WorkflowParameterFilter WorkflowParameterFilter);
        Task<List<WorkflowParameter>> List(WorkflowParameterFilter WorkflowParameterFilter);
        Task<WorkflowParameter> Get(long Id);
        Task<bool> Create(WorkflowParameter WorkflowParameter);
        Task<bool> Update(WorkflowParameter WorkflowParameter);
        Task<bool> Delete(WorkflowParameter WorkflowParameter);
        Task<bool> BulkMerge(List<WorkflowParameter> WorkflowParameters);
        Task<bool> BulkDelete(List<WorkflowParameter> WorkflowParameters);
    }
    public class WorkflowParameterRepository : IWorkflowParameterRepository
    {
        private DataContext DataContext;
        public WorkflowParameterRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowParameterDAO> DynamicFilter(IQueryable<WorkflowParameterDAO> query, WorkflowParameterFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.WorkflowDefinitionId != null)
                query = query.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<WorkflowParameterDAO> OrFilter(IQueryable<WorkflowParameterDAO> query, WorkflowParameterFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowParameterDAO> initQuery = query.Where(q => false);
            foreach (WorkflowParameterFilter WorkflowParameterFilter in filter.OrFilter)
            {
                IQueryable<WorkflowParameterDAO> queryable = query;
                if (WorkflowParameterFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WorkflowParameterFilter.Id);
                if (WorkflowParameterFilter.WorkflowDefinitionId != null)
                    queryable = queryable.Where(q => q.WorkflowDefinitionId, WorkflowParameterFilter.WorkflowDefinitionId);
                if (WorkflowParameterFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WorkflowParameterFilter.Code);
                if (WorkflowParameterFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WorkflowParameterFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<WorkflowParameterDAO> DynamicOrder(IQueryable<WorkflowParameterDAO> query, WorkflowParameterFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowParameterOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowParameterOrder.WorkflowDefinition:
                            query = query.OrderBy(q => q.WorkflowDefinitionId);
                            break;
                        case WorkflowParameterOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WorkflowParameterOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowParameterOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowParameterOrder.WorkflowDefinition:
                            query = query.OrderByDescending(q => q.WorkflowDefinitionId);
                            break;
                        case WorkflowParameterOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WorkflowParameterOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowParameter>> DynamicSelect(IQueryable<WorkflowParameterDAO> query, WorkflowParameterFilter filter)
        {
            List<WorkflowParameter> WorkflowParameters = await query.Select(q => new WorkflowParameter()
            {
                Id = filter.Selects.Contains(WorkflowParameterSelect.Id) ? q.Id : default(long),
                WorkflowDefinitionId = filter.Selects.Contains(WorkflowParameterSelect.WorkflowDefinition) ? q.WorkflowDefinitionId : default(long),
                Code = filter.Selects.Contains(WorkflowParameterSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WorkflowParameterSelect.Name) ? q.Name : default(string),
                WorkflowDefinition = filter.Selects.Contains(WorkflowParameterSelect.WorkflowDefinition) && q.WorkflowDefinition != null ? new WorkflowDefinition
                {
                    Id = q.WorkflowDefinition.Id,
                    Code = q.WorkflowDefinition.Code,
                    Name = q.WorkflowDefinition.Name,
                    WorkflowTypeId = q.WorkflowDefinition.WorkflowTypeId,
                    StartDate = q.WorkflowDefinition.StartDate,
                    EndDate = q.WorkflowDefinition.EndDate,
                    StatusId = q.WorkflowDefinition.StatusId,
                    UpdatedAt = q.WorkflowDefinition.UpdatedAt,
                } : null,
            }).ToListAsync();
            return WorkflowParameters;
        }

        public async Task<int> Count(WorkflowParameterFilter filter)
        {
            IQueryable<WorkflowParameterDAO> WorkflowParameters = DataContext.WorkflowParameter.AsNoTracking();
            WorkflowParameters = DynamicFilter(WorkflowParameters, filter);
            return await WorkflowParameters.CountAsync();
        }

        public async Task<List<WorkflowParameter>> List(WorkflowParameterFilter filter)
        {
            if (filter == null) return new List<WorkflowParameter>();
            IQueryable<WorkflowParameterDAO> WorkflowParameterDAOs = DataContext.WorkflowParameter.AsNoTracking();
            WorkflowParameterDAOs = DynamicFilter(WorkflowParameterDAOs, filter);
            WorkflowParameterDAOs = DynamicOrder(WorkflowParameterDAOs, filter);
            List<WorkflowParameter> WorkflowParameters = await DynamicSelect(WorkflowParameterDAOs, filter);
            return WorkflowParameters;
        }

        public async Task<WorkflowParameter> Get(long Id)
        {
            WorkflowParameter WorkflowParameter = await DataContext.WorkflowParameter.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new WorkflowParameter()
            {
                Id = x.Id,
                WorkflowDefinitionId = x.WorkflowDefinitionId,
                Code = x.Code,
                Name = x.Name,
                WorkflowDefinition = x.WorkflowDefinition == null ? null : new WorkflowDefinition
                {
                    Id = x.WorkflowDefinition.Id,
                    Code = x.WorkflowDefinition.Code,
                    Name = x.WorkflowDefinition.Name,
                    WorkflowTypeId = x.WorkflowDefinition.WorkflowTypeId,
                    StartDate = x.WorkflowDefinition.StartDate,
                    EndDate = x.WorkflowDefinition.EndDate,
                    StatusId = x.WorkflowDefinition.StatusId,
                    UpdatedAt = x.WorkflowDefinition.UpdatedAt,
                },
            }).FirstOrDefaultAsync();

            if (WorkflowParameter == null)
                return null;

            return WorkflowParameter;
        }
        public async Task<bool> Create(WorkflowParameter WorkflowParameter)
        {
            WorkflowParameterDAO WorkflowParameterDAO = new WorkflowParameterDAO();
            WorkflowParameterDAO.Id = WorkflowParameter.Id;
            WorkflowParameterDAO.WorkflowDefinitionId = WorkflowParameter.WorkflowDefinitionId;
            WorkflowParameterDAO.Code = WorkflowParameter.Code;
            WorkflowParameterDAO.Name = WorkflowParameter.Name;
            DataContext.WorkflowParameter.Add(WorkflowParameterDAO);
            await DataContext.SaveChangesAsync();
            WorkflowParameter.Id = WorkflowParameterDAO.Id;
            await SaveReference(WorkflowParameter);
            return true;
        }

        public async Task<bool> Update(WorkflowParameter WorkflowParameter)
        {
            WorkflowParameterDAO WorkflowParameterDAO = DataContext.WorkflowParameter.Where(x => x.Id == WorkflowParameter.Id).FirstOrDefault();
            if (WorkflowParameterDAO == null)
                return false;
            WorkflowParameterDAO.Id = WorkflowParameter.Id;
            WorkflowParameterDAO.WorkflowDefinitionId = WorkflowParameter.WorkflowDefinitionId;
            WorkflowParameterDAO.Code = WorkflowParameter.Code;
            WorkflowParameterDAO.Name = WorkflowParameter.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(WorkflowParameter);
            return true;
        }

        public async Task<bool> Delete(WorkflowParameter WorkflowParameter)
        {
            await DataContext.WorkflowParameter.Where(x => x.Id == WorkflowParameter.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<WorkflowParameter> WorkflowParameters)
        {
            List<WorkflowParameterDAO> WorkflowParameterDAOs = new List<WorkflowParameterDAO>();
            foreach (WorkflowParameter WorkflowParameter in WorkflowParameters)
            {
                WorkflowParameterDAO WorkflowParameterDAO = new WorkflowParameterDAO();
                WorkflowParameterDAO.Id = WorkflowParameter.Id;
                WorkflowParameterDAO.WorkflowDefinitionId = WorkflowParameter.WorkflowDefinitionId;
                WorkflowParameterDAO.Code = WorkflowParameter.Code;
                WorkflowParameterDAO.Name = WorkflowParameter.Name;
                WorkflowParameterDAOs.Add(WorkflowParameterDAO);
            }
            await DataContext.BulkMergeAsync(WorkflowParameterDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<WorkflowParameter> WorkflowParameters)
        {
            List<long> Ids = WorkflowParameters.Select(x => x.Id).ToList();
            await DataContext.WorkflowParameter
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(WorkflowParameter WorkflowParameter)
        {
        }
        
    }
}
