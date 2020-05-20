using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MWorkflowParameter
{
    public interface IWorkflowParameterService :  IServiceScoped
    {
        Task<int> Count(WorkflowParameterFilter WorkflowParameterFilter);
        Task<List<WorkflowParameter>> List(WorkflowParameterFilter WorkflowParameterFilter);
        Task<WorkflowParameter> Get(long Id);
        Task<WorkflowParameter> Create(WorkflowParameter WorkflowParameter);
        Task<WorkflowParameter> Update(WorkflowParameter WorkflowParameter);
        Task<WorkflowParameter> Delete(WorkflowParameter WorkflowParameter);
        Task<List<WorkflowParameter>> BulkDelete(List<WorkflowParameter> WorkflowParameters);
        Task<List<WorkflowParameter>> Import(List<WorkflowParameter> WorkflowParameters);
        WorkflowParameterFilter ToFilter(WorkflowParameterFilter WorkflowParameterFilter);
    }

    public class WorkflowParameterService : BaseService, IWorkflowParameterService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWorkflowParameterValidator WorkflowParameterValidator;

        public WorkflowParameterService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWorkflowParameterValidator WorkflowParameterValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WorkflowParameterValidator = WorkflowParameterValidator;
        }
        public async Task<int> Count(WorkflowParameterFilter WorkflowParameterFilter)
        {
            try
            {
                int result = await UOW.WorkflowParameterRepository.Count(WorkflowParameterFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<WorkflowParameter>> List(WorkflowParameterFilter WorkflowParameterFilter)
        {
            try
            {
                List<WorkflowParameter> WorkflowParameters = await UOW.WorkflowParameterRepository.List(WorkflowParameterFilter);
                return WorkflowParameters;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<WorkflowParameter> Get(long Id)
        {
            WorkflowParameter WorkflowParameter = await UOW.WorkflowParameterRepository.Get(Id);
            if (WorkflowParameter == null)
                return null;
            return WorkflowParameter;
        }
       
        public async Task<WorkflowParameter> Create(WorkflowParameter WorkflowParameter)
        {
            if (!await WorkflowParameterValidator.Create(WorkflowParameter))
                return WorkflowParameter;

            try
            {
                await UOW.Begin();
                await UOW.WorkflowParameterRepository.Create(WorkflowParameter);
                await UOW.Commit();

                await Logging.CreateAuditLog(WorkflowParameter, new { }, nameof(WorkflowParameterService));
                return await UOW.WorkflowParameterRepository.Get(WorkflowParameter.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<WorkflowParameter> Update(WorkflowParameter WorkflowParameter)
        {
            if (!await WorkflowParameterValidator.Update(WorkflowParameter))
                return WorkflowParameter;
            try
            {
                var oldData = await UOW.WorkflowParameterRepository.Get(WorkflowParameter.Id);

                await UOW.Begin();
                await UOW.WorkflowParameterRepository.Update(WorkflowParameter);
                await UOW.Commit();

                var newData = await UOW.WorkflowParameterRepository.Get(WorkflowParameter.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(WorkflowParameterService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<WorkflowParameter> Delete(WorkflowParameter WorkflowParameter)
        {
            if (!await WorkflowParameterValidator.Delete(WorkflowParameter))
                return WorkflowParameter;

            try
            {
                await UOW.Begin();
                await UOW.WorkflowParameterRepository.Delete(WorkflowParameter);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, WorkflowParameter, nameof(WorkflowParameterService));
                return WorkflowParameter;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<WorkflowParameter>> BulkDelete(List<WorkflowParameter> WorkflowParameters)
        {
            if (!await WorkflowParameterValidator.BulkDelete(WorkflowParameters))
                return WorkflowParameters;

            try
            {
                await UOW.Begin();
                await UOW.WorkflowParameterRepository.BulkDelete(WorkflowParameters);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, WorkflowParameters, nameof(WorkflowParameterService));
                return WorkflowParameters;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<WorkflowParameter>> Import(List<WorkflowParameter> WorkflowParameters)
        {
            if (!await WorkflowParameterValidator.Import(WorkflowParameters))
                return WorkflowParameters;
            try
            {
                await UOW.Begin();
                await UOW.WorkflowParameterRepository.BulkMerge(WorkflowParameters);
                await UOW.Commit();

                await Logging.CreateAuditLog(WorkflowParameters, new { }, nameof(WorkflowParameterService));
                return WorkflowParameters;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowParameterService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public WorkflowParameterFilter ToFilter(WorkflowParameterFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<WorkflowParameterFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                WorkflowParameterFilter subFilter = new WorkflowParameterFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WorkflowDefinitionId))
                        subFilter.WorkflowDefinitionId = Map(subFilter.WorkflowDefinitionId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
