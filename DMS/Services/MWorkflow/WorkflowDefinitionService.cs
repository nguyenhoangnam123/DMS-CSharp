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
using DMS.Enums;
using HandlebarsDotNet;

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
        private List<string> StoreParameters;
        private List<string> ROUTER;
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

            StoreParameters = new List<string> { nameof(Store.Code), nameof(Store.Name), nameof(Store.StoreTypeId) };
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                InitParameter(WorkflowDefinition);
                WorkflowDefinition.CreatorId = CurrentContext.UserId;
                await UOW.WorkflowDefinitionRepository.Create(WorkflowDefinition);
                await UOW.Commit();

                await Logging.CreateAuditLog(WorkflowDefinition, new { }, nameof(WorkflowDefinitionService));
                return await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinition.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<WorkflowDefinition> Update(WorkflowDefinition WorkflowDefinition)
        {
            if (!await WorkflowDefinitionValidator.Update(WorkflowDefinition))
                return WorkflowDefinition;
            try
            {
                var oldData = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinition.Id);
                InitParameter(WorkflowDefinition);
                WorkflowDefinition.ModifierId = CurrentContext.UserId;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(WorkflowDefinitionService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = Map(subFilter.CreatorId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ModifierId))
                        subFilter.ModifierId = Map(subFilter.ModifierId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.WorkflowTypeId))
                        subFilter.WorkflowTypeId = Map(subFilter.WorkflowTypeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartDate))
                        subFilter.StartDate = Map(subFilter.StartDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndDate))
                        subFilter.EndDate = Map(subFilter.EndDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                }
            }
            return filter;
        }

        private void InitParameter(WorkflowDefinition WorkflowDefinition)
        {
            if (WorkflowDefinition.WorkflowTypeId == WorkflowTypeEnum.STORE.Id)
            {
                WorkflowDefinition.WorkflowParameters = StoreParameters.Select(x => new WorkflowParameter
                {
                    Name = x
                }).ToList();
            }
        }
    }
}
