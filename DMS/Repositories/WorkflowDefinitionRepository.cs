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
    public interface IWorkflowDefinitionRepository
    {
        Task<int> Count(WorkflowDefinitionFilter WorkflowDefinitionFilter);
        Task<List<WorkflowDefinition>> List(WorkflowDefinitionFilter WorkflowDefinitionFilter);
        Task<WorkflowDefinition> Get(long Id);
        Task<bool> Create(WorkflowDefinition WorkflowDefinition);
        Task<bool> Update(WorkflowDefinition WorkflowDefinition);
        Task<bool> Delete(WorkflowDefinition WorkflowDefinition);
        Task<bool> BulkMerge(List<WorkflowDefinition> WorkflowDefinitions);
        Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions);
    }
    public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private DataContext DataContext;
        public WorkflowDefinitionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowDefinitionDAO> DynamicFilter(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.WorkflowTypeId != null)
                query = query.Where(q => q.WorkflowTypeId, filter.WorkflowTypeId);
            if (filter.StartDate != null)
                query = query.Where(q => q.StartDate, filter.StartDate);
            if (filter.EndDate != null)
                query = query.Where(q => q.EndDate, filter.EndDate);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WorkflowDefinitionDAO> OrFilter(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowDefinitionDAO> initQuery = query.Where(q => false);
            foreach (WorkflowDefinitionFilter WorkflowDefinitionFilter in filter.OrFilter)
            {
                IQueryable<WorkflowDefinitionDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.WorkflowTypeId != null)
                    queryable = queryable.Where(q => q.WorkflowTypeId, filter.WorkflowTypeId);
                if (filter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, filter.StartDate);
                if (filter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, filter.EndDate);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WorkflowDefinitionDAO> DynamicOrder(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowDefinitionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowDefinitionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WorkflowDefinitionOrder.WorkflowType:
                            query = query.OrderBy(q => q.WorkflowTypeId);
                            break;
                        case WorkflowDefinitionOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case WorkflowDefinitionOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case WorkflowDefinitionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowDefinitionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowDefinitionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WorkflowDefinitionOrder.WorkflowType:
                            query = query.OrderByDescending(q => q.WorkflowTypeId);
                            break;
                        case WorkflowDefinitionOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case WorkflowDefinitionOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case WorkflowDefinitionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowDefinition>> DynamicSelect(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            List<WorkflowDefinition> WorkflowDefinitions = await query.Select(q => new WorkflowDefinition()
            {
                Id = filter.Selects.Contains(WorkflowDefinitionSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(WorkflowDefinitionSelect.Name) ? q.Name : default(string),
                WorkflowTypeId = filter.Selects.Contains(WorkflowDefinitionSelect.WorkflowType) ? q.WorkflowTypeId : default(long),
                StartDate = filter.Selects.Contains(WorkflowDefinitionSelect.StartDate) ? q.StartDate : default(DateTime?),
                EndDate = filter.Selects.Contains(WorkflowDefinitionSelect.EndDate) ? q.EndDate : default(DateTime?),
                StatusId = filter.Selects.Contains(WorkflowDefinitionSelect.Status) ? q.StatusId : default(long),
                WorkflowType = filter.Selects.Contains(WorkflowDefinitionSelect.WorkflowType) && q.WorkflowType != null ? new WorkflowType
                {
                    Id = q.WorkflowType.Id,
                    Code = q.WorkflowType.Code,
                    Name = q.WorkflowType.Name,
                } : null,
            }).AsNoTracking().ToListAsync();
            return WorkflowDefinitions;
        }

        public async Task<int> Count(WorkflowDefinitionFilter filter)
        {
            IQueryable<WorkflowDefinitionDAO> WorkflowDefinitions = DataContext.WorkflowDefinition;
            WorkflowDefinitions = DynamicFilter(WorkflowDefinitions, filter);
            return await WorkflowDefinitions.CountAsync();
        }

        public async Task<List<WorkflowDefinition>> List(WorkflowDefinitionFilter filter)
        {
            if (filter == null) return new List<WorkflowDefinition>();
            IQueryable<WorkflowDefinitionDAO> WorkflowDefinitionDAOs = DataContext.WorkflowDefinition;
            WorkflowDefinitionDAOs = DynamicFilter(WorkflowDefinitionDAOs, filter);
            WorkflowDefinitionDAOs = DynamicOrder(WorkflowDefinitionDAOs, filter);
            List<WorkflowDefinition> WorkflowDefinitions = await DynamicSelect(WorkflowDefinitionDAOs, filter);
            return WorkflowDefinitions;
        }

        public async Task<WorkflowDefinition> Get(long Id)
        {
            WorkflowDefinition WorkflowDefinition = await DataContext.WorkflowDefinition.Where(x => x.Id == Id).Select(x => new WorkflowDefinition()
            {
                Id = x.Id,
                Name = x.Name,
                WorkflowTypeId = x.WorkflowTypeId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StatusId = x.StatusId,
                WorkflowType = x.WorkflowType == null ? null : new WorkflowType
                {
                    Id = x.WorkflowType.Id,
                    Code = x.WorkflowType.Code,
                    Name = x.WorkflowType.Name,
                },
            }).AsNoTracking().FirstOrDefaultAsync();

            if (WorkflowDefinition == null)
                return null;

            return WorkflowDefinition;
        }
        public async Task<bool> Create(WorkflowDefinition WorkflowDefinition)
        {
            WorkflowDefinitionDAO WorkflowDefinitionDAO = new WorkflowDefinitionDAO();
            WorkflowDefinitionDAO.Id = WorkflowDefinition.Id;
            WorkflowDefinitionDAO.Name = WorkflowDefinition.Name;
            WorkflowDefinitionDAO.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
            WorkflowDefinitionDAO.StartDate = WorkflowDefinition.StartDate;
            WorkflowDefinitionDAO.EndDate = WorkflowDefinition.EndDate;
            WorkflowDefinitionDAO.StatusId = WorkflowDefinition.StatusId;
            WorkflowDefinitionDAO.CreatedAt = StaticParams.DateTimeNow;
            WorkflowDefinitionDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.WorkflowDefinition.Add(WorkflowDefinitionDAO);
            await DataContext.SaveChangesAsync();
            WorkflowDefinition.Id = WorkflowDefinitionDAO.Id;
            await SaveReference(WorkflowDefinition);
            return true;
        }

        public async Task<bool> Update(WorkflowDefinition WorkflowDefinition)
        {
            WorkflowDefinitionDAO WorkflowDefinitionDAO = DataContext.WorkflowDefinition.Where(x => x.Id == WorkflowDefinition.Id).FirstOrDefault();
            if (WorkflowDefinitionDAO == null)
                return false;
            WorkflowDefinitionDAO.Id = WorkflowDefinition.Id;
            WorkflowDefinitionDAO.Name = WorkflowDefinition.Name;
            WorkflowDefinitionDAO.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
            WorkflowDefinitionDAO.StartDate = WorkflowDefinition.StartDate;
            WorkflowDefinitionDAO.EndDate = WorkflowDefinition.EndDate;
            WorkflowDefinitionDAO.StatusId = WorkflowDefinition.StatusId;
            WorkflowDefinitionDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(WorkflowDefinition);
            return true;
        }

        public async Task<bool> Delete(WorkflowDefinition WorkflowDefinition)
        {
            await DataContext.WorkflowDefinition.Where(x => x.Id == WorkflowDefinition.Id).UpdateFromQueryAsync(x => new WorkflowDefinitionDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<WorkflowDefinition> WorkflowDefinitions)
        {
            List<WorkflowDefinitionDAO> WorkflowDefinitionDAOs = new List<WorkflowDefinitionDAO>();
            foreach (WorkflowDefinition WorkflowDefinition in WorkflowDefinitions)
            {
                WorkflowDefinitionDAO WorkflowDefinitionDAO = new WorkflowDefinitionDAO();
                WorkflowDefinitionDAO.Id = WorkflowDefinition.Id;
                WorkflowDefinitionDAO.Name = WorkflowDefinition.Name;
                WorkflowDefinitionDAO.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
                WorkflowDefinitionDAO.StartDate = WorkflowDefinition.StartDate;
                WorkflowDefinitionDAO.EndDate = WorkflowDefinition.EndDate;
                WorkflowDefinitionDAO.StatusId = WorkflowDefinition.StatusId;
                WorkflowDefinitionDAO.CreatedAt = StaticParams.DateTimeNow;
                WorkflowDefinitionDAO.UpdatedAt = StaticParams.DateTimeNow;
                WorkflowDefinitionDAOs.Add(WorkflowDefinitionDAO);
            }
            await DataContext.BulkMergeAsync(WorkflowDefinitionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions)
        {
            List<long> Ids = WorkflowDefinitions.Select(x => x.Id).ToList();
            await DataContext.WorkflowDefinition
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new WorkflowDefinitionDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(WorkflowDefinition WorkflowDefinition)
        {
            await DataContext.WorkflowStep.Where(s => s.WorkflowDefinitionId == WorkflowDefinition.Id).DeleteFromQueryAsync();
            if (WorkflowDefinition.WorkflowSteps != null)
            {
                List<WorkflowStepDAO> WorkflowStepDAOs = new List<WorkflowStepDAO>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    WorkflowStepDAO WorkflowStepDAO = new WorkflowStepDAO();
                    WorkflowStepDAO.WorkflowDefinitionId = WorkflowDefinition.Id;
                    WorkflowStepDAO.Name = WorkflowStep.Name;
                    WorkflowStepDAO.RoleId = WorkflowStep.RoleId;
                    WorkflowStepDAOs.Add(WorkflowStepDAO);
                }
                await DataContext.WorkflowStep.BulkMergeAsync(WorkflowStepDAOs);

                await DataContext.WorkflowDirection.Where(d => d.WorkflowDefinitionId == WorkflowDefinition.Id).DeleteFromQueryAsync();
                if (WorkflowDefinition.WorkflowDirections != null)
                {
                    List<WorkflowDirectionDAO> WorkflowDirectionDAOs = new List<WorkflowDirectionDAO>();
                    foreach (WorkflowDirection WorkflowDirection in WorkflowDefinition.WorkflowDirections)
                    {
                        WorkflowDirectionDAO WorkflowDirectionDAO = new WorkflowDirectionDAO();
                        WorkflowDirectionDAO.WorkflowDefinitionId = WorkflowDefinition.Id;
                        WorkflowDirectionDAO.FromStepId = WorkflowStepDAOs.Where(s => s.Name == WorkflowDirection.FromStep.Name).Select(s => s.Id).FirstOrDefault();
                        WorkflowDirectionDAO.ToStepId = WorkflowStepDAOs.Where(s => s.Name == WorkflowDirection.ToStep.Name).Select(s => s.Id).FirstOrDefault();
                        WorkflowDirectionDAOs.Add(WorkflowDirectionDAO);
                    }
                    await DataContext.WorkflowDirection.BulkMergeAsync(WorkflowDirectionDAOs);
                }
            }
        }
    }
}
