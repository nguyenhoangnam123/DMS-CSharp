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
    public interface IWorkflowStepRepository
    {
        Task<int> Count(WorkflowStepFilter WorkflowStepFilter);
        Task<List<WorkflowStep>> List(WorkflowStepFilter WorkflowStepFilter);
        Task<WorkflowStep> Get(long Id);
        Task<bool> Create(WorkflowStep WorkflowStep);
        Task<bool> Update(WorkflowStep WorkflowStep);
        Task<bool> Delete(WorkflowStep WorkflowStep);
        Task<bool> BulkMerge(List<WorkflowStep> WorkflowSteps);
        Task<bool> BulkDelete(List<WorkflowStep> WorkflowSteps);
    }
    public class WorkflowStepRepository : IWorkflowStepRepository
    {
        private DataContext DataContext;
        public WorkflowStepRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowStepDAO> DynamicFilter(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.WorkflowDefinitionId != null)
                query = query.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.RoleId != null)
                query = query.Where(q => q.RoleId, filter.RoleId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<WorkflowStepDAO> OrFilter(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowStepDAO> initQuery = query.Where(q => false);
            foreach (WorkflowStepFilter WorkflowStepFilter in filter.OrFilter)
            {
                IQueryable<WorkflowStepDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.WorkflowDefinitionId != null)
                    queryable = queryable.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.RoleId != null)
                    queryable = queryable.Where(q => q.RoleId, filter.RoleId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<WorkflowStepDAO> DynamicOrder(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowStepOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowStepOrder.WorkflowDefinition:
                            query = query.OrderBy(q => q.WorkflowDefinitionId);
                            break;
                        case WorkflowStepOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WorkflowStepOrder.Role:
                            query = query.OrderBy(q => q.RoleId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowStepOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowStepOrder.WorkflowDefinition:
                            query = query.OrderByDescending(q => q.WorkflowDefinitionId);
                            break;
                        case WorkflowStepOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WorkflowStepOrder.Role:
                            query = query.OrderByDescending(q => q.RoleId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowStep>> DynamicSelect(IQueryable<WorkflowStepDAO> query, WorkflowStepFilter filter)
        {
            List<WorkflowStep> WorkflowSteps = await query.Select(q => new WorkflowStep()
            {
                Id = filter.Selects.Contains(WorkflowStepSelect.Id) ? q.Id : default(long),
                WorkflowDefinitionId = filter.Selects.Contains(WorkflowStepSelect.WorkflowDefinition) ? q.WorkflowDefinitionId : default(long),
                Name = filter.Selects.Contains(WorkflowStepSelect.Name) ? q.Name : default(string),
                RoleId = filter.Selects.Contains(WorkflowStepSelect.Role) ? q.RoleId : default(long),
                Role = filter.Selects.Contains(WorkflowStepSelect.Role) && q.Role != null ? new Role
                {
                    Id = q.Role.Id,
                    Code = q.Role.Code,
                    Name = q.Role.Name,
                    StatusId = q.Role.StatusId,
                } : null,
                WorkflowDefinition = filter.Selects.Contains(WorkflowStepSelect.WorkflowDefinition) && q.WorkflowDefinition != null ? new WorkflowDefinition
                {
                    Id = q.WorkflowDefinition.Id,
                    Name = q.WorkflowDefinition.Name,
                    WorkflowTypeId = q.WorkflowDefinition.WorkflowTypeId,
                    StartDate = q.WorkflowDefinition.StartDate,
                    EndDate = q.WorkflowDefinition.EndDate,
                    StatusId = q.WorkflowDefinition.StatusId,
                } : null,
            }).ToListAsync();
            return WorkflowSteps;
        }

        public async Task<int> Count(WorkflowStepFilter filter)
        {
            IQueryable<WorkflowStepDAO> WorkflowSteps = DataContext.WorkflowStep.AsNoTracking();
            WorkflowSteps = DynamicFilter(WorkflowSteps, filter);
            return await WorkflowSteps.CountAsync();
        }

        public async Task<List<WorkflowStep>> List(WorkflowStepFilter filter)
        {
            if (filter == null) return new List<WorkflowStep>();
            IQueryable<WorkflowStepDAO> WorkflowStepDAOs = DataContext.WorkflowStep.AsNoTracking();
            WorkflowStepDAOs = DynamicFilter(WorkflowStepDAOs, filter);
            WorkflowStepDAOs = DynamicOrder(WorkflowStepDAOs, filter);
            List<WorkflowStep> WorkflowSteps = await DynamicSelect(WorkflowStepDAOs, filter);
            return WorkflowSteps;
        }

        public async Task<WorkflowStep> Get(long Id)
        {
            WorkflowStep WorkflowStep = await DataContext.WorkflowStep.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new WorkflowStep()
            {
                Id = x.Id,
                WorkflowDefinitionId = x.WorkflowDefinitionId,
                Name = x.Name,
                RoleId = x.RoleId,
                SubjectMailForReject = x.SubjectMailForReject,
                BodyMailForReject = x.BodyMailForReject,
                Role = x.Role == null ? null : new Role
                {
                    Id = x.Role.Id,
                    Code = x.Role.Code,
                    Name = x.Role.Name,
                    StatusId = x.Role.StatusId,
                },
                WorkflowDefinition = x.WorkflowDefinition == null ? null : new WorkflowDefinition
                {
                    Id = x.WorkflowDefinition.Id,
                    Name = x.WorkflowDefinition.Name,
                    WorkflowTypeId = x.WorkflowDefinition.WorkflowTypeId,
                    StartDate = x.WorkflowDefinition.StartDate,
                    EndDate = x.WorkflowDefinition.EndDate,
                    StatusId = x.WorkflowDefinition.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (WorkflowStep == null)
                return null;

            return WorkflowStep;
        }
        public async Task<bool> Create(WorkflowStep WorkflowStep)
        {
            WorkflowStepDAO WorkflowStepDAO = new WorkflowStepDAO();
            WorkflowStepDAO.Id = WorkflowStep.Id;
            WorkflowStepDAO.WorkflowDefinitionId = WorkflowStep.WorkflowDefinitionId;
            WorkflowStepDAO.Name = WorkflowStep.Name;
            WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
            WorkflowStepDAO.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
            WorkflowStepDAO.BodyMailForReject = WorkflowStep.BodyMailForReject;
            DataContext.WorkflowStep.Add(WorkflowStepDAO);
            await DataContext.SaveChangesAsync();
            WorkflowStep.Id = WorkflowStepDAO.Id;
            await SaveReference(WorkflowStep);
            return true;
        }

        public async Task<bool> Update(WorkflowStep WorkflowStep)
        {
            WorkflowStepDAO WorkflowStepDAO = DataContext.WorkflowStep.Where(x => x.Id == WorkflowStep.Id).FirstOrDefault();
            if (WorkflowStepDAO == null)
                return false;
            WorkflowStepDAO.Id = WorkflowStep.Id;
            WorkflowStepDAO.WorkflowDefinitionId = WorkflowStep.WorkflowDefinitionId;
            WorkflowStepDAO.Name = WorkflowStep.Name;
            WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
            WorkflowStepDAO.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
            WorkflowStepDAO.BodyMailForReject = WorkflowStep.BodyMailForReject;
            await DataContext.SaveChangesAsync();
            await SaveReference(WorkflowStep);
            return true;
        }

        public async Task<bool> Delete(WorkflowStep WorkflowStep)
        {
            await DataContext.WorkflowStep.Where(x => x.Id == WorkflowStep.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<WorkflowStep> WorkflowSteps)
        {
            List<WorkflowStepDAO> WorkflowStepDAOs = new List<WorkflowStepDAO>();
            foreach (WorkflowStep WorkflowStep in WorkflowSteps)
            {
                WorkflowStepDAO WorkflowStepDAO = new WorkflowStepDAO();
                WorkflowStepDAO.Id = WorkflowStep.Id;
                WorkflowStepDAO.WorkflowDefinitionId = WorkflowStep.WorkflowDefinitionId;
                WorkflowStepDAO.Name = WorkflowStep.Name;
                WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
                WorkflowStepDAO.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
                WorkflowStepDAO.BodyMailForReject = WorkflowStep.BodyMailForReject;
                WorkflowStepDAOs.Add(WorkflowStepDAO);
            }
            await DataContext.BulkMergeAsync(WorkflowStepDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<WorkflowStep> WorkflowSteps)
        {
            List<long> Ids = WorkflowSteps.Select(x => x.Id).ToList();
            await DataContext.WorkflowStep
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(WorkflowStep WorkflowStep)
        {
        }
        
    }
}
