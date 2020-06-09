using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IRequestWorkflowParameterMappingRepository
    {
        Task<List<RequestWorkflowParameterMapping>> List(Guid RequestId);
        Task<bool> BulkMerge(Guid RequestId, List<RequestWorkflowParameterMapping> RequestWorkflowParameterMappings);
    }
    public class RequestWorkflowParameterMappingRepository : IRequestWorkflowParameterMappingRepository
    {
        private DataContext DataContext;
        public RequestWorkflowParameterMappingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task<List<RequestWorkflowParameterMapping>> List(Guid RequestId)
        {
            List<RequestWorkflowParameterMapping> RequestWorkflowParameterMappings = await DataContext.RequestWorkflowParameterMapping
                .Where(r => r.RequestId == RequestId).AsNoTracking()
                .Select(r => new RequestWorkflowParameterMapping
                {
                    RequestId = r.RequestId,
                    Value = r.Value,
                    WorkflowParameterId = r.WorkflowParameterId,
                }).ToListAsync();
            return RequestWorkflowParameterMappings;
        }
        public async Task<bool> BulkMerge(Guid RequestId, List<RequestWorkflowParameterMapping> RequestWorkflowParameterMappings)
        {
            List<RequestWorkflowParameterMappingDAO> RequestWorkflowParameterMappingDAOs = DataContext.RequestWorkflowParameterMapping
                .Where(r => r.RequestId == RequestId).ToList();
            RequestWorkflowParameterMappingDAOs.ForEach(r => r.DeletedAt = StaticParams.DateTimeNow);

            foreach (RequestWorkflowParameterMapping RequestWorkflowParameterMapping in RequestWorkflowParameterMappings)
            {
                RequestWorkflowParameterMappingDAO RequestWorkflowParameterMappingDAO = RequestWorkflowParameterMappingDAOs
                    .Where(r => r.WorkflowParameterId == RequestWorkflowParameterMapping.WorkflowParameterId).FirstOrDefault();
                if (RequestWorkflowParameterMappingDAO == null)
                {
                    RequestWorkflowParameterMappingDAO = new RequestWorkflowParameterMappingDAO();
                    RequestWorkflowParameterMappingDAO.RequestId = RequestId;
                    RequestWorkflowParameterMappingDAO.WorkflowParameterId = RequestWorkflowParameterMapping.WorkflowParameterId;
                    DataContext.RequestWorkflowParameterMapping.Add(RequestWorkflowParameterMappingDAO);
                }
                RequestWorkflowParameterMappingDAO.Value = RequestWorkflowParameterMapping.Value;
                RequestWorkflowParameterMappingDAO.DeletedAt = null;
            }
            var Deleted = RequestWorkflowParameterMappingDAOs.Where(r => r.DeletedAt.HasValue).ToList();
            DataContext.RequestWorkflowParameterMapping.RemoveRange(Deleted);
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
