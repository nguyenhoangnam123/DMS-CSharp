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

namespace DMS.Services.MWorkflowDefinition
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
        private IMailService MailService;
        public WorkflowDefinitionService(
            IUOW UOW,
            ILogging Logging,
            IMailService MailService,
            ICurrentContext CurrentContext,
            IWorkflowDefinitionValidator WorkflowDefinitionValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.MailService = MailService;
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

        public async Task<bool> Start(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinitionId);

            if (WorkflowDefinition == null)
            {
                return true;
            }
            else
            {
                WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinition.Id);
                long RequestStateId = RequestStateEnum.APPROVING.Id;
                List<RequestWorkflow> RequestWorkflows = new List<RequestWorkflow>();
                List<WorkflowStep> Starts = new List<WorkflowStep>();
                // tìm điểm bắt đầu của workflow
                // nút không có ai trỏ đến là nút bắt đầu
                // trong trường hợp có nhiều nút bắt đầu thì xét có nút nào thuộc các role hiện tại không
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (!WorkflowDefinition.WorkflowDirections.Any(d => d.ToStepId == WorkflowStep.Id) &&
                        CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        Starts.Add(WorkflowStep);
                    }
                }
                // khởi tạo trạng thái cho tất cả các nút
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    RequestWorkflow RequestWorkflow = new RequestWorkflow();
                    RequestWorkflows.Add(RequestWorkflow);
                    RequestWorkflow.WorkflowStepId = WorkflowStep.Id;
                    RequestWorkflow.WorkflowStateId = WorkflowStateEnum.NEW.Id;
                    RequestWorkflow.UpdatedAt = null;
                    RequestWorkflow.AppUserId = null;
                    if (Starts.Any(s => s.Id == WorkflowStep.Id))
                    {
                        RequestWorkflow.WorkflowStateId = WorkflowStateEnum.PENDING.Id;
                    }
                }
                List<RequestWorkflowParameterMapping> RequestWorkflowParameterMappings = new List<RequestWorkflowParameterMapping>();
                foreach (WorkflowParameter WorkflowParameter in WorkflowDefinition.WorkflowParameters)
                {
                    RequestWorkflowParameterMapping RequestWorkflowParameterMapping = new RequestWorkflowParameterMapping();
                    RequestWorkflowParameterMappings.Add(RequestWorkflowParameterMapping);
                    RequestWorkflowParameterMapping.WorkflowParameterId = WorkflowParameter.Id;
                    RequestWorkflowParameterMapping.RequestId = RequestId;
                    RequestWorkflowParameterMapping.Value = null;
                    foreach (var pair in Parameters)
                    {
                        if (WorkflowParameter.Name == pair.Key)
                        {
                            RequestWorkflowParameterMapping.Value = pair.Value;
                        }
                    }
                }

            }
            return true;
        }

        public async Task<bool> Approve(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinitionId);
            // tìm điểm bắt đầu
            // tìm điểm nhảy tiếp theo
            // chuyển trạng thái điểm nhảy
            // gửi mail cho các điểm nhảy có trạng thái thay đổi.
            RequestWorkflowFilter RequestWorkflowFilter = new RequestWorkflowFilter
            {
                RequestId = new GuidFilter { Equal = RequestId }
            };
            List<RequestWorkflow> RequestWorkflows = await UOW.RequestWorkflowRepository.List(RequestWorkflowFilter);
            if (WorkflowDefinition != null && RequestWorkflows.Count > 0)
            {
                List<WorkflowStep> ToSteps = new List<WorkflowStep>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        var StartNode = RequestWorkflows
                            .Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id)
                            .Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        StartNode.WorkflowStateId = WorkflowStateEnum.APPROVED.Id;
                        StartNode.UpdatedAt = StaticParams.DateTimeNow;
                        StartNode.AppUserId = CurrentContext.UserId;

                        var NextSteps = WorkflowDefinition.WorkflowDirections.Where(d => d.FromStepId == WorkflowStep.Id)
                            .Select(x => x.ToStep) ?? new List<WorkflowStep>();
                        ToSteps.AddRange(NextSteps);
                    }
                }

                ToSteps = ToSteps.Distinct().ToList();
                List<Mail> Mails = new List<Mail>();
                var NextStepIds = new List<long>();
                foreach (WorkflowStep WorkflowStep in ToSteps)
                {
                    var FromSteps = WorkflowDefinition.WorkflowDirections.Where(d => d.ToStepId == WorkflowStep.Id).Select(x => x.FromStep) ?? new List<WorkflowStep>();
                    var ApprovedFromSteps = new List<RequestWorkflow>();
                    foreach (var FromStep in FromSteps)
                    {
                        var FromNode = RequestWorkflows.Where(x => x.WorkflowStepId == FromStep.Id).FirstOrDefault();
                        ApprovedFromSteps.Add(FromNode);
                    }

                    if (ApprovedFromSteps.All(x => x.WorkflowStateId == WorkflowStateEnum.APPROVED.Id))
                    {
                        var RequestWorkflow = RequestWorkflows.Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        RequestWorkflow.WorkflowStateId = WorkflowStateEnum.PENDING.Id;
                        RequestWorkflow.UpdatedAt = null;
                        RequestWorkflow.AppUserId = null;
                        NextStepIds.Add(WorkflowStep.Id);
                    }
                }

                List<WorkflowDirection> WorkflowDirections = WorkflowDefinition.WorkflowDirections.Where(x => NextStepIds.Contains(x.ToStepId)).ToList();
                foreach (var WorkflowDirection in WorkflowDirections)
                {
                    //gửi mail
                    AppUserFilter AppUserFilter = new AppUserFilter
                    {
                        RoleId = new IdFilter { Equal = WorkflowDirection.ToStep.RoleId },
                        Skip = 0,
                        Take = int.MaxValue,
                        Selects = AppUserSelect.Email
                    };
                    List<AppUser> appUsers = await UOW.AppUserRepository.List(AppUserFilter);
                    List<string> recipients = appUsers.Select(au => au.Email).Distinct().ToList();

                    Mail MailForCreator = new Mail
                    {
                        Recipients = recipients,
                        Subject = WorkflowDirection.SubjectMailForCreator,
                        Body = WorkflowDirection.BodyMailForCreator
                    };

                    Mail MailForNextStep = new Mail
                    {
                        Recipients = recipients,
                        Subject = WorkflowDirection.SubjectMailForNextStep,
                        Body = WorkflowDirection.BodyMailForNextStep
                    };
                    Mails.Add(MailForCreator);
                    Mails.Add(MailForNextStep);
                }
                Mails.Distinct();
                Mails.ForEach(x => MailService.Send(x));
                return true;
            }

            return false;
        }

        public async Task<bool> Reject(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await Get(WorkflowDefinitionId);
            RequestWorkflowFilter RequestWorkflowFilter = new RequestWorkflowFilter
            {
                RequestId = new GuidFilter { Equal = RequestId }
            };
            List<RequestWorkflow> RequestWorkflows = await UOW.RequestWorkflowRepository.List(RequestWorkflowFilter);
            if (WorkflowDefinition != null && RequestWorkflows.Count > 0)
            {
                List<WorkflowStep> ToSteps = new List<WorkflowStep>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        var StartNode = RequestWorkflows
                            .Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id)
                            .Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        StartNode.WorkflowStateId = WorkflowStateEnum.REJECTED.Id;
                        StartNode.UpdatedAt = StaticParams.DateTimeNow;
                        StartNode.AppUserId = CurrentContext.UserId;
                    }
                }

                WorkflowStep StartStep = null;
                // tìm điểm bắt đầu của workflow
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (!WorkflowDefinition.WorkflowDirections.Any(d => d.ToStepId == WorkflowStep.Id) &&
                        CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        StartStep = WorkflowStep;
                        break;
                    }
                }

                //tìm node đầu tiên để gửi mail thông báo yêu cầu đã bị từ chối
                if (StartStep != null)
                {
                    RequestWorkflow StartNode = RequestWorkflows.Where(x => x.WorkflowStepId == StartStep.Id).FirstOrDefault();
                    if (StartNode != null)
                    {
                        //gửi mail
                    }
                }
            }

            return true;
        }

        public async Task<GenericEnum> End(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await Get(WorkflowDefinitionId);
            RequestWorkflowFilter RequestWorkflowFilter = new RequestWorkflowFilter
            {
                RequestId = new GuidFilter { Equal = RequestId }
            };
            List<RequestWorkflow> RequestWorkflows = await UOW.RequestWorkflowRepository.List(RequestWorkflowFilter);
            if (WorkflowDefinition != null && RequestWorkflows != null)
            {
                if (RequestWorkflows.Any(x => x.WorkflowStateId == WorkflowStateEnum.REJECTED.Id))
                {
                    return RequestStateEnum.REJECTED;
                }
                else if (RequestWorkflows.All(x => x.WorkflowStateId == WorkflowStateEnum.APPROVED.Id))
                {
                    return RequestStateEnum.APPROVED;
                }
            }

            return null;
        }
    }
}
