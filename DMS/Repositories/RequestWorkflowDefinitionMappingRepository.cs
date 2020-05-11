using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IRequestWorkflowDefinitionMappingRepository 
    {
        Task<List<RequestWorkflowDefinitionMapping>> List(RequestWorkflowDefinitionMappingFilter filter);
        Task<RequestWorkflowDefinitionMapping> Get(Guid RequestId);
        Task<RequestWorkflowDefinitionMapping> Update(RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping);
        Task<bool> Delete(Guid RequestId);
    }
    public class RequestWorkflowDefinitionMappingRepository : IRequestWorkflowDefinitionMappingRepository
    {
        private DataContext DataContext;
        public RequestWorkflowDefinitionMappingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task<RequestWorkflowDefinitionMapping> Get(Guid RequestId)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await DataContext.RequestWorkflowDefinitionMapping.AsNoTracking()
                .Where(q => q.RequestId == RequestId)
                .Select(q => new RequestWorkflowDefinitionMapping
                {
                    RequestId = q.RequestId,
                    RequestStateId = q.RequestStateId,
                    WorkflowDefinitionId = q.WorkflowDefinitionId,
                    RequestState = new RequestState
                    {
                        Id = q.RequestState.Id,
                        Code = q.RequestState.Code,
                        Name = q.RequestState.Name,
                    },
                }).FirstOrDefaultAsync();
            return RequestWorkflowDefinitionMapping;
        }

        public async Task<RequestWorkflowDefinitionMapping> Update(RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping)
        {
            RequestWorkflowDefinitionMappingDAO RequestWorkflowDefinitionMappingDAO = await DataContext.RequestWorkflowDefinitionMapping
                .Where(q => q.RequestId == RequestWorkflowDefinitionMapping.RequestId).FirstOrDefaultAsync();
            if (RequestWorkflowDefinitionMappingDAO == null)
            {
                RequestWorkflowDefinitionMappingDAO = new RequestWorkflowDefinitionMappingDAO
                {
                    RequestId = RequestWorkflowDefinitionMapping.RequestId,
                    RequestStateId = RequestWorkflowDefinitionMapping.RequestStateId,
                    WorkflowDefinitionId = RequestWorkflowDefinitionMapping.WorkflowDefinitionId,
                };
                DataContext.RequestWorkflowDefinitionMapping.Add(RequestWorkflowDefinitionMappingDAO);
            }   
            else
            {
                RequestWorkflowDefinitionMappingDAO.RequestStateId = RequestWorkflowDefinitionMapping.RequestStateId;
                RequestWorkflowDefinitionMappingDAO.WorkflowDefinitionId = RequestWorkflowDefinitionMapping.WorkflowDefinitionId;
            }
            await DataContext.SaveChangesAsync();
            RequestWorkflowDefinitionMapping.RequestId = RequestWorkflowDefinitionMappingDAO.RequestId;
            return RequestWorkflowDefinitionMapping;
        }

        public async Task<List<RequestWorkflowDefinitionMapping>> List(RequestWorkflowDefinitionMappingFilter filter)
        {
            IQueryable<RequestWorkflowDefinitionMappingDAO> query = DataContext.RequestWorkflowDefinitionMapping.AsNoTracking();
            if (filter.RequestId != null)
                query = query.Where(q => q.RequestId, filter.RequestId);
            if (filter.WorkflowDefinitionId != null)
                query = query.Where(q => q.WorkflowDefinitionId, filter.WorkflowDefinitionId);
            query = query.OrderBy(q => q.RequestId).Skip(0).Take(int.MaxValue);
            var result = await query.Select(q => new RequestWorkflowDefinitionMapping
            {
                RequestId = q.RequestId,
                RequestStateId = q.RequestStateId,
                WorkflowDefinitionId = q.WorkflowDefinitionId,
            }).ToListAsync();
            return result;
        }

        public async Task<bool> Delete(Guid RequestId)
        {
            await DataContext.RequestWorkflowStepMapping.Where(r => r.RequestId == RequestId).DeleteFromQueryAsync();
            await DataContext.RequestWorkflowParameterMapping.Where(r => r.RequestId == RequestId).DeleteFromQueryAsync();
            await DataContext.RequestWorkflowDefinitionMapping.Where(r => r.RequestId == RequestId).DeleteFromQueryAsync();
            return true;
        }
    }
}
