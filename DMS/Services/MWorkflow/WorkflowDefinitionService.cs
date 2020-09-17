using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MWorkflow
{
    public interface IWorkflowDefinitionService : IServiceScoped
    {
        Task<int> Count(WorkflowDefinitionFilter WorkflowDefinitionFilter);
        Task<List<WorkflowDefinition>> List(WorkflowDefinitionFilter WorkflowDefinitionFilter);
        Task<WorkflowDefinition> Get(long Id);
        Task<WorkflowDefinition> Create(WorkflowDefinition WorkflowDefinition);
        Task<WorkflowDefinition> Update(WorkflowDefinition WorkflowDefinition);
        Task<WorkflowDefinition> Delete(WorkflowDefinition WorkflowDefinition);
        Task<List<WorkflowDefinition>> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions);
        Task<List<WorkflowDefinition>> Import(List<WorkflowDefinition> WorkflowDefinitions);
        WorkflowDefinitionFilter ToFilter(WorkflowDefinitionFilter WorkflowDefinitionFilter);
    }

    public class WorkflowDefinitionService : BaseService, IWorkflowDefinitionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWorkflowDefinitionValidator WorkflowDefinitionValidator;
        public WorkflowDefinitionService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWorkflowDefinitionValidator WorkflowDefinitionValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WorkflowDefinitionValidator = WorkflowDefinitionValidator;
        }
        public async Task<int> Count(WorkflowDefinitionFilter WorkflowDefinitionFilter)
        {
            try
            {
                int result = await UOW.WorkflowDefinitionRepository.Count(WorkflowDefinitionFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<WorkflowDefinition>> List(WorkflowDefinitionFilter WorkflowDefinitionFilter)
        {
            try
            {
                List<WorkflowDefinition> WorkflowDefinitions = await UOW.WorkflowDefinitionRepository.List(WorkflowDefinitionFilter);
                return WorkflowDefinitions;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<WorkflowDefinition> Get(long Id)
        {
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(Id);
            if (WorkflowDefinition == null)
                return null;
            return WorkflowDefinition;
        }

        public async Task<WorkflowDefinition> Create(WorkflowDefinition WorkflowDefinition)
        {
            if (!await WorkflowDefinitionValidator.Create(WorkflowDefinition))
                return WorkflowDefinition;

            try
            {
                await UOW.Begin();
                WorkflowDefinition.CreatorId = CurrentContext.UserId;
                await UOW.WorkflowDefinitionRepository.Create(WorkflowDefinition);
                await UOW.Commit();

                await Logging.CreateAuditLog(WorkflowDefinition, new { }, nameof(WorkflowDefinitionService));
                return await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinition.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<WorkflowDefinition> Update(WorkflowDefinition WorkflowDefinition)
        {
            if (!await WorkflowDefinitionValidator.Update(WorkflowDefinition))
                return WorkflowDefinition;
            try
            {
                var oldData = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinition.Id);
                await UOW.Begin();
                await UOW.WorkflowDefinitionRepository.Update(WorkflowDefinition);
                await UOW.Commit();

                var newData = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinition.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(WorkflowDefinitionService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<WorkflowDefinition> Delete(WorkflowDefinition WorkflowDefinition)
        {
            if (!await WorkflowDefinitionValidator.Delete(WorkflowDefinition))
                return WorkflowDefinition;

            try
            {
                await UOW.Begin();
                await UOW.WorkflowDefinitionRepository.Delete(WorkflowDefinition);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, WorkflowDefinition, nameof(WorkflowDefinitionService));
                return WorkflowDefinition;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<WorkflowDefinition>> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions)
        {
            if (!await WorkflowDefinitionValidator.BulkDelete(WorkflowDefinitions))
                return WorkflowDefinitions;

            try
            {
                await UOW.Begin();
                await UOW.WorkflowDefinitionRepository.BulkDelete(WorkflowDefinitions);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, WorkflowDefinitions, nameof(WorkflowDefinitionService));
                return WorkflowDefinitions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<WorkflowDefinition>> Import(List<WorkflowDefinition> WorkflowDefinitions)
        {
            if (!await WorkflowDefinitionValidator.Import(WorkflowDefinitions))
                return WorkflowDefinitions;
            try
            {
                await UOW.Begin();
                await UOW.WorkflowDefinitionRepository.BulkMerge(WorkflowDefinitions);
                await UOW.Commit();

                await Logging.CreateAuditLog(WorkflowDefinitions, new { }, nameof(WorkflowDefinitionService));
                return WorkflowDefinitions;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public WorkflowDefinitionFilter ToFilter(WorkflowDefinitionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<WorkflowDefinitionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                WorkflowDefinitionFilter subFilter = new WorkflowDefinitionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
