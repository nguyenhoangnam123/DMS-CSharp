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
    public interface IWorkflowDirectionRepository
    {
        Task<int> Count(WorkflowDirectionFilter WorkflowDirectionFilter);
        Task<List<WorkflowDirection>> List(WorkflowDirectionFilter WorkflowDirectionFilter);
        Task<WorkflowDirection> Get(long Id);
        Task<bool> Create(WorkflowDirection WorkflowDirection);
        Task<bool> Update(WorkflowDirection WorkflowDirection);
        Task<bool> Delete(WorkflowDirection WorkflowDirection);
        Task<bool> BulkMerge(List<WorkflowDirection> WorkflowDirections);
        Task<bool> BulkDelete(List<WorkflowDirection> WorkflowDirections);
    }
    public class WorkflowDirectionRepository : IWorkflowDirectionRepository
    {
        private DataContext DataContext;
        public WorkflowDirectionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowDirectionDAO> DynamicFilter(IQueryable<WorkflowDirectionDAO> query, WorkflowDirectionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.WorkflowDefinitionId != null)
                query = query.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
            if (filter.FromStepId != null)
                query = query.Where(q => q.FromStepId, filter.FromStepId);
            if (filter.ToStepId != null)
                query = query.Where(q => q.ToStepId, filter.ToStepId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<WorkflowDirectionDAO> OrFilter(IQueryable<WorkflowDirectionDAO> query, WorkflowDirectionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowDirectionDAO> initQuery = query.Where(q => false);
            foreach (WorkflowDirectionFilter WorkflowDirectionFilter in filter.OrFilter)
            {
                IQueryable<WorkflowDirectionDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.WorkflowDefinitionId != null)
                    queryable = queryable.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
                if (filter.FromStepId != null)
                    queryable = queryable.Where(q => q.FromStepId, filter.FromStepId);
                if (filter.ToStepId != null)
                    queryable = queryable.Where(q => q.ToStepId, filter.ToStepId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<WorkflowDirectionDAO> DynamicOrder(IQueryable<WorkflowDirectionDAO> query, WorkflowDirectionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowDirectionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowDirectionOrder.WorkflowDefinition:
                            query = query.OrderBy(q => q.WorkflowDefinitionId);
                            break;
                        case WorkflowDirectionOrder.FromStep:
                            query = query.OrderBy(q => q.FromStepId);
                            break;
                        case WorkflowDirectionOrder.ToStep:
                            query = query.OrderBy(q => q.ToStepId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowDirectionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowDirectionOrder.WorkflowDefinition:
                            query = query.OrderByDescending(q => q.WorkflowDefinitionId);
                            break;
                        case WorkflowDirectionOrder.FromStep:
                            query = query.OrderByDescending(q => q.FromStepId);
                            break;
                        case WorkflowDirectionOrder.ToStep:
                            query = query.OrderByDescending(q => q.ToStepId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowDirection>> DynamicSelect(IQueryable<WorkflowDirectionDAO> query, WorkflowDirectionFilter filter)
        {
            List<WorkflowDirection> WorkflowDirections = await query.Select(q => new WorkflowDirection()
            {
                Id = filter.Selects.Contains(WorkflowDirectionSelect.Id) ? q.Id : default(long),
                WorkflowDefinitionId = filter.Selects.Contains(WorkflowDirectionSelect.WorkflowDefinition) ? q.WorkflowDefinitionId : default(long),
                FromStepId = filter.Selects.Contains(WorkflowDirectionSelect.FromStep) ? q.FromStepId : default(long),
                ToStepId = filter.Selects.Contains(WorkflowDirectionSelect.ToStep) ? q.ToStepId : default(long),
                FromStep = filter.Selects.Contains(WorkflowDirectionSelect.FromStep) && q.FromStep != null ? new WorkflowStep
                {
                    Id = q.FromStep.Id,
                    WorkflowDefinitionId = q.FromStep.WorkflowDefinitionId,
                    Name = q.FromStep.Name,
                    RoleId = q.FromStep.RoleId,
                    SubjectMailForReject = q.FromStep.SubjectMailForReject,
                    BodyMailForReject = q.FromStep.BodyMailForReject,
                } : null,
                ToStep = filter.Selects.Contains(WorkflowDirectionSelect.ToStep) && q.ToStep != null ? new WorkflowStep
                {
                    Id = q.ToStep.Id,
                    WorkflowDefinitionId = q.ToStep.WorkflowDefinitionId,
                    Name = q.ToStep.Name,
                    RoleId = q.ToStep.RoleId,
                    SubjectMailForReject = q.ToStep.SubjectMailForReject,
                    BodyMailForReject = q.ToStep.BodyMailForReject,
                } : null,
                WorkflowDefinition = filter.Selects.Contains(WorkflowDirectionSelect.WorkflowDefinition) && q.WorkflowDefinition != null ? new WorkflowDefinition
                {
                    Id = q.WorkflowDefinition.Id,
                    Name = q.WorkflowDefinition.Name,
                    WorkflowTypeId = q.WorkflowDefinition.WorkflowTypeId,
                    StartDate = q.WorkflowDefinition.StartDate,
                    EndDate = q.WorkflowDefinition.EndDate,
                    StatusId = q.WorkflowDefinition.StatusId,
                } : null,
            }).ToListAsync();
            return WorkflowDirections;
        }

        public async Task<int> Count(WorkflowDirectionFilter filter)
        {
            IQueryable<WorkflowDirectionDAO> WorkflowDirections = DataContext.WorkflowDirection.AsNoTracking();
            WorkflowDirections = DynamicFilter(WorkflowDirections, filter);
            return await WorkflowDirections.CountAsync();
        }

        public async Task<List<WorkflowDirection>> List(WorkflowDirectionFilter filter)
        {
            if (filter == null) return new List<WorkflowDirection>();
            IQueryable<WorkflowDirectionDAO> WorkflowDirectionDAOs = DataContext.WorkflowDirection.AsNoTracking();
            WorkflowDirectionDAOs = DynamicFilter(WorkflowDirectionDAOs, filter);
            WorkflowDirectionDAOs = DynamicOrder(WorkflowDirectionDAOs, filter);
            List<WorkflowDirection> WorkflowDirections = await DynamicSelect(WorkflowDirectionDAOs, filter);
            return WorkflowDirections;
        }

        public async Task<WorkflowDirection> Get(long Id)
        {
            WorkflowDirection WorkflowDirection = await DataContext.WorkflowDirection.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new WorkflowDirection()
            {
                Id = x.Id,
                WorkflowDefinitionId = x.WorkflowDefinitionId,
                FromStepId = x.FromStepId,
                ToStepId = x.ToStepId,
                SubjectMailForCreator = x.SubjectMailForCreator,
                SubjectMailForNextStep = x.SubjectMailForNextStep,
                BodyMailForCreator = x.BodyMailForCreator,
                BodyMailForNextStep = x.BodyMailForNextStep,
                FromStep = x.FromStep == null ? null : new WorkflowStep
                {
                    Id = x.FromStep.Id,
                    WorkflowDefinitionId = x.FromStep.WorkflowDefinitionId,
                    Name = x.FromStep.Name,
                    RoleId = x.FromStep.RoleId,
                    SubjectMailForReject = x.FromStep.SubjectMailForReject,
                    BodyMailForReject = x.FromStep.BodyMailForReject,
                },
                ToStep = x.ToStep == null ? null : new WorkflowStep
                {
                    Id = x.ToStep.Id,
                    WorkflowDefinitionId = x.ToStep.WorkflowDefinitionId,
                    Name = x.ToStep.Name,
                    RoleId = x.ToStep.RoleId,
                    SubjectMailForReject = x.ToStep.SubjectMailForReject,
                    BodyMailForReject = x.ToStep.BodyMailForReject,
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

            if (WorkflowDirection == null)
                return null;

            return WorkflowDirection;
        }
        public async Task<bool> Create(WorkflowDirection WorkflowDirection)
        {
            WorkflowDirectionDAO WorkflowDirectionDAO = new WorkflowDirectionDAO();
            WorkflowDirectionDAO.Id = WorkflowDirection.Id;
            WorkflowDirectionDAO.WorkflowDefinitionId = WorkflowDirection.WorkflowDefinitionId;
            WorkflowDirectionDAO.FromStepId = WorkflowDirection.FromStepId;
            WorkflowDirectionDAO.ToStepId = WorkflowDirection.ToStepId;
            WorkflowDirectionDAO.SubjectMailForCreator = WorkflowDirection.SubjectMailForCreator;
            WorkflowDirectionDAO.SubjectMailForNextStep = WorkflowDirection.SubjectMailForNextStep;
            WorkflowDirectionDAO.BodyMailForCreator = WorkflowDirection.BodyMailForCreator;
            WorkflowDirectionDAO.BodyMailForNextStep = WorkflowDirection.BodyMailForNextStep;
            DataContext.WorkflowDirection.Add(WorkflowDirectionDAO);
            await DataContext.SaveChangesAsync();
            WorkflowDirection.Id = WorkflowDirectionDAO.Id;
            await SaveReference(WorkflowDirection);
            return true;
        }

        public async Task<bool> Update(WorkflowDirection WorkflowDirection)
        {
            WorkflowDirectionDAO WorkflowDirectionDAO = DataContext.WorkflowDirection.Where(x => x.Id == WorkflowDirection.Id).FirstOrDefault();
            if (WorkflowDirectionDAO == null)
                return false;
            WorkflowDirectionDAO.Id = WorkflowDirection.Id;
            WorkflowDirectionDAO.WorkflowDefinitionId = WorkflowDirection.WorkflowDefinitionId;
            WorkflowDirectionDAO.FromStepId = WorkflowDirection.FromStepId;
            WorkflowDirectionDAO.ToStepId = WorkflowDirection.ToStepId;
            WorkflowDirectionDAO.SubjectMailForCreator = WorkflowDirection.SubjectMailForCreator;
            WorkflowDirectionDAO.SubjectMailForNextStep = WorkflowDirection.SubjectMailForNextStep;
            WorkflowDirectionDAO.BodyMailForCreator = WorkflowDirection.BodyMailForCreator;
            WorkflowDirectionDAO.BodyMailForNextStep = WorkflowDirection.BodyMailForNextStep;
            await DataContext.SaveChangesAsync();
            await SaveReference(WorkflowDirection);
            return true;
        }

        public async Task<bool> Delete(WorkflowDirection WorkflowDirection)
        {
            await DataContext.WorkflowDirection.Where(x => x.Id == WorkflowDirection.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<WorkflowDirection> WorkflowDirections)
        {
            List<WorkflowDirectionDAO> WorkflowDirectionDAOs = new List<WorkflowDirectionDAO>();
            foreach (WorkflowDirection WorkflowDirection in WorkflowDirections)
            {
                WorkflowDirectionDAO WorkflowDirectionDAO = new WorkflowDirectionDAO();
                WorkflowDirectionDAO.Id = WorkflowDirection.Id;
                WorkflowDirectionDAO.WorkflowDefinitionId = WorkflowDirection.WorkflowDefinitionId;
                WorkflowDirectionDAO.FromStepId = WorkflowDirection.FromStepId;
                WorkflowDirectionDAO.ToStepId = WorkflowDirection.ToStepId;
                WorkflowDirectionDAO.SubjectMailForCreator = WorkflowDirection.SubjectMailForCreator;
                WorkflowDirectionDAO.SubjectMailForNextStep = WorkflowDirection.SubjectMailForNextStep;
                WorkflowDirectionDAO.BodyMailForCreator = WorkflowDirection.BodyMailForCreator;
                WorkflowDirectionDAO.BodyMailForNextStep = WorkflowDirection.BodyMailForNextStep;
                WorkflowDirectionDAOs.Add(WorkflowDirectionDAO);
            }
            await DataContext.BulkMergeAsync(WorkflowDirectionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<WorkflowDirection> WorkflowDirections)
        {
            List<long> Ids = WorkflowDirections.Select(x => x.Id).ToList();
            await DataContext.WorkflowDirection
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(WorkflowDirection WorkflowDirection)
        {
        }
        
    }
}
