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
    public interface IRequestWorkflowRepository
    {
        Task<int> Count(RequestWorkflowFilter RequestWorkflowFilter);
        Task<List<RequestWorkflow>> List(RequestWorkflowFilter RequestWorkflowFilter);
        Task<RequestWorkflow> Get(long Id);
        Task<bool> Create(RequestWorkflow RequestWorkflow);
        Task<bool> Update(RequestWorkflow RequestWorkflow);
        Task<bool> Delete(RequestWorkflow RequestWorkflow);
        Task<bool> BulkMerge(List<RequestWorkflow> RequestWorkflows);
        Task<bool> BulkDelete(List<RequestWorkflow> RequestWorkflows);
    }
    public class RequestWorkflowRepository : IRequestWorkflowRepository
    {
        private DataContext DataContext;
        public RequestWorkflowRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<RequestWorkflowDAO> DynamicFilter(IQueryable<RequestWorkflowDAO> query, RequestWorkflowFilter filter)
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
            if (filter.RequestId != null)
                query = query.Where(q => q.RequestId, filter.RequestId);
            if (filter.WorkflowStepId != null)
                query = query.Where(q => q.WorkflowStepId, filter.WorkflowStepId);
            if (filter.WorkflowStateId != null)
                query = query.Where(q => q.WorkflowStateId, filter.WorkflowStateId);
            if (filter.AppUserId != null)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<RequestWorkflowDAO> OrFilter(IQueryable<RequestWorkflowDAO> query, RequestWorkflowFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<RequestWorkflowDAO> initQuery = query.Where(q => false);
            foreach (RequestWorkflowFilter RequestWorkflowFilter in filter.OrFilter)
            {
                IQueryable<RequestWorkflowDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.RequestId != null)
                    queryable = queryable.Where(q => q.RequestId, filter.RequestId);
                if (filter.WorkflowStepId != null)
                    queryable = queryable.Where(q => q.WorkflowStepId, filter.WorkflowStepId);
                if (filter.WorkflowStateId != null)
                    queryable = queryable.Where(q => q.WorkflowStateId, filter.WorkflowStateId);
                if (filter.AppUserId != null)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<RequestWorkflowDAO> DynamicOrder(IQueryable<RequestWorkflowDAO> query, RequestWorkflowFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case RequestWorkflowOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case RequestWorkflowOrder.WorkflowStep:
                            query = query.OrderBy(q => q.WorkflowStepId);
                            break;
                        case RequestWorkflowOrder.WorkflowState:
                            query = query.OrderBy(q => q.WorkflowStateId);
                            break;
                        case RequestWorkflowOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case RequestWorkflowOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case RequestWorkflowOrder.WorkflowStep:
                            query = query.OrderByDescending(q => q.WorkflowStepId);
                            break;
                        case RequestWorkflowOrder.WorkflowState:
                            query = query.OrderByDescending(q => q.WorkflowStateId);
                            break;
                        case RequestWorkflowOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<RequestWorkflow>> DynamicSelect(IQueryable<RequestWorkflowDAO> query, RequestWorkflowFilter filter)
        {
            List<RequestWorkflow> RequestWorkflows = await query.Select(q => new RequestWorkflow()
            {
                Id = filter.Selects.Contains(RequestWorkflowSelect.Id) ? q.Id : default(long),
                RequestId = filter.Selects.Contains(RequestWorkflowSelect.RequestId) ? q.RequestId : Guid.Empty, 
                WorkflowStepId = filter.Selects.Contains(RequestWorkflowSelect.WorkflowStep) ? q.WorkflowStepId : default(long),
                WorkflowStateId = filter.Selects.Contains(RequestWorkflowSelect.WorkflowState) ? q.WorkflowStateId : default(long),
                AppUserId = filter.Selects.Contains(RequestWorkflowSelect.AppUser) ? q.AppUserId : default(long?),
                AppUser = filter.Selects.Contains(RequestWorkflowSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    Password = q.AppUser.Password,
                    DisplayName = q.AppUser.DisplayName,
                    Address = q.AppUser.Address,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                    Position = q.AppUser.Position,
                    Department = q.AppUser.Department,
                    OrganizationId = q.AppUser.OrganizationId,
                    SexId = q.AppUser.SexId,
                    StatusId = q.AppUser.StatusId,
                    Avatar = q.AppUser.Avatar,
                    Birthday = q.AppUser.Birthday,
                    RowId = q.AppUser.RowId,
                    ProvinceId = q.AppUser.ProvinceId,
                } : null,
                WorkflowState = filter.Selects.Contains(RequestWorkflowSelect.WorkflowState) && q.WorkflowState != null ? new WorkflowState
                {
                    Id = q.WorkflowState.Id,
                    Code = q.WorkflowState.Code,
                    Name = q.WorkflowState.Name,
                } : null,
                WorkflowStep = filter.Selects.Contains(RequestWorkflowSelect.WorkflowStep) && q.WorkflowStep != null ? new WorkflowStep
                {
                    Id = q.WorkflowStep.Id,
                    WorkflowDefinitionId = q.WorkflowStep.WorkflowDefinitionId,
                    Name = q.WorkflowStep.Name,
                    RoleId = q.WorkflowStep.RoleId,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return RequestWorkflows;
        }

        public async Task<int> Count(RequestWorkflowFilter filter)
        {
            IQueryable<RequestWorkflowDAO> RequestWorkflows = DataContext.RequestWorkflow.AsNoTracking();
            RequestWorkflows = DynamicFilter(RequestWorkflows, filter);
            return await RequestWorkflows.CountAsync();
        }

        public async Task<List<RequestWorkflow>> List(RequestWorkflowFilter filter)
        {
            if (filter == null) return new List<RequestWorkflow>();
            IQueryable<RequestWorkflowDAO> RequestWorkflowDAOs = DataContext.RequestWorkflow.AsNoTracking();
            RequestWorkflowDAOs = DynamicFilter(RequestWorkflowDAOs, filter);
            RequestWorkflowDAOs = DynamicOrder(RequestWorkflowDAOs, filter);
            List<RequestWorkflow> RequestWorkflows = await DynamicSelect(RequestWorkflowDAOs, filter);
            return RequestWorkflows;
        }

        public async Task<RequestWorkflow> Get(long Id)
        {
            RequestWorkflow RequestWorkflow = await DataContext.RequestWorkflow.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new RequestWorkflow()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                RequestId = x.RequestId,
                WorkflowStepId = x.WorkflowStepId,
                WorkflowStateId = x.WorkflowStateId,
                AppUserId = x.AppUserId,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    Password = x.AppUser.Password,
                    DisplayName = x.AppUser.DisplayName,
                    Address = x.AppUser.Address,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                    Position = x.AppUser.Position,
                    Department = x.AppUser.Department,
                    OrganizationId = x.AppUser.OrganizationId,
                    SexId = x.AppUser.SexId,
                    StatusId = x.AppUser.StatusId,
                    Avatar = x.AppUser.Avatar,
                    Birthday = x.AppUser.Birthday,
                    RowId = x.AppUser.RowId,
                    ProvinceId = x.AppUser.ProvinceId,
                },
                WorkflowState = x.WorkflowState == null ? null : new WorkflowState
                {
                    Id = x.WorkflowState.Id,
                    Code = x.WorkflowState.Code,
                    Name = x.WorkflowState.Name,
                },
                WorkflowStep = x.WorkflowStep == null ? null : new WorkflowStep
                {
                    Id = x.WorkflowStep.Id,
                    WorkflowDefinitionId = x.WorkflowStep.WorkflowDefinitionId,
                    Name = x.WorkflowStep.Name,
                    RoleId = x.WorkflowStep.RoleId,
                },
            }).FirstOrDefaultAsync();

            if (RequestWorkflow == null)
                return null;

            return RequestWorkflow;
        }
        public async Task<bool> Create(RequestWorkflow RequestWorkflow)
        {
            RequestWorkflowDAO RequestWorkflowDAO = new RequestWorkflowDAO();
            RequestWorkflowDAO.Id = RequestWorkflow.Id;
            RequestWorkflowDAO.WorkflowStepId = RequestWorkflow.WorkflowStepId;
            RequestWorkflowDAO.WorkflowStateId = RequestWorkflow.WorkflowStateId;
            RequestWorkflowDAO.AppUserId = RequestWorkflow.AppUserId;
            RequestWorkflowDAO.CreatedAt = StaticParams.DateTimeNow;
            RequestWorkflowDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.RequestWorkflow.Add(RequestWorkflowDAO);
            await DataContext.SaveChangesAsync();
            RequestWorkflow.Id = RequestWorkflowDAO.Id;
            await SaveReference(RequestWorkflow);
            return true;
        }

        public async Task<bool> Update(RequestWorkflow RequestWorkflow)
        {
            RequestWorkflowDAO RequestWorkflowDAO = DataContext.RequestWorkflow.Where(x => x.Id == RequestWorkflow.Id).FirstOrDefault();
            if (RequestWorkflowDAO == null)
                return false;
            RequestWorkflowDAO.Id = RequestWorkflow.Id;
            RequestWorkflowDAO.WorkflowStepId = RequestWorkflow.WorkflowStepId;
            RequestWorkflowDAO.WorkflowStateId = RequestWorkflow.WorkflowStateId;
            RequestWorkflowDAO.AppUserId = RequestWorkflow.AppUserId;
            RequestWorkflowDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(RequestWorkflow);
            return true;
        }

        public async Task<bool> Delete(RequestWorkflow RequestWorkflow)
        {
            await DataContext.RequestWorkflow.Where(x => x.Id == RequestWorkflow.Id).UpdateFromQueryAsync(x => new RequestWorkflowDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<RequestWorkflow> RequestWorkflows)
        {
            List<RequestWorkflowDAO> RequestWorkflowDAOs = new List<RequestWorkflowDAO>();
            foreach (RequestWorkflow RequestWorkflow in RequestWorkflows)
            {
                RequestWorkflowDAO RequestWorkflowDAO = new RequestWorkflowDAO();
                RequestWorkflowDAO.Id = RequestWorkflow.Id;
                RequestWorkflowDAO.WorkflowStepId = RequestWorkflow.WorkflowStepId;
                RequestWorkflowDAO.WorkflowStateId = RequestWorkflow.WorkflowStateId;
                RequestWorkflowDAO.AppUserId = RequestWorkflow.AppUserId;
                RequestWorkflowDAO.CreatedAt = StaticParams.DateTimeNow;
                RequestWorkflowDAO.UpdatedAt = StaticParams.DateTimeNow;
                RequestWorkflowDAOs.Add(RequestWorkflowDAO);
            }
            await DataContext.BulkMergeAsync(RequestWorkflowDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<RequestWorkflow> RequestWorkflows)
        {
            List<long> Ids = RequestWorkflows.Select(x => x.Id).ToList();
            await DataContext.RequestWorkflow
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new RequestWorkflowDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(RequestWorkflow RequestWorkflow)
        {
        }
        
    }
}
