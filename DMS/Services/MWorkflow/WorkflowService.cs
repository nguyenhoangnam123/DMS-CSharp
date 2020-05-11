using Common;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Entities;
using DMS.Enums;
using Helpers;
using HandlebarsDotNet;

namespace DMS.Services.MWorkflow
{
    public interface IWorkflowService : IServiceScoped
    {
        Task<RequestState> GetRequestState(Guid RequestId);
        Task<List<RequestWorkflowStepMapping>> ListRequestWorkflowState(Guid RequestId);
        Task<bool> Init(Guid RequestId, long WorkflowTypeId, Dictionary<string, string> Parameters);
        Task<bool> IsInitialized(Guid RequestId);
        Task<bool> IsStarted(Guid RequestId);
        Task<bool> Start(Guid RequestId, Dictionary<string, string> Parameters);
        Task<bool> Approve(Guid RequestId, Dictionary<string, string> Parameters);
        Task<bool> Reject(Guid RequestId, Dictionary<string, string> Parameters);
        Task<GenericEnum> End(Guid RequestId, Dictionary<string, string> Parameters);
    }
    public class WorkflowService : IWorkflowService
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private IMailService MailService;
        public WorkflowService(IUOW UOW,
            IMailService MailService,
            ICurrentContext CurrentContext
            )
        {
            this.UOW = UOW;
            this.MailService = MailService;
            this.CurrentContext = CurrentContext;
        }

        public async Task<RequestState> GetRequestState(Guid RequestId)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            if (RequestWorkflowDefinitionMapping == null)
                return null;
            return RequestWorkflowDefinitionMapping.RequestState;
        }

        public async Task<List<RequestWorkflowStepMapping>> ListRequestWorkflowState(Guid RequestId)
        {
            List<RequestWorkflowStepMapping> RequestWorkflowStepMappings = await UOW.RequestWorkflowStepMappingRepository.List(RequestId);
            return RequestWorkflowStepMappings;
        }

        public async Task<bool> IsInitialized(Guid RequestId)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            if (RequestWorkflowDefinitionMapping == null)
                return false;
            else
                return true;
        }
        public async Task<bool> IsStarted(Guid RequestId)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            if (RequestWorkflowDefinitionMapping == null)
                return false;
            if (RequestWorkflowDefinitionMapping.RequestStateId == RequestStateEnum.APPROVING.Id)
                return true;
            return false;
        }
        public async Task<bool> Init(Guid RequestId, long WorkflowTypeId, Dictionary<string, string> Parameters)
        {
            List<WorkflowDefinition> WorkflowDefinitions = await UOW.WorkflowDefinitionRepository.List(new WorkflowDefinitionFilter
            {
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                WorkflowTypeId = new IdFilter { Equal = WorkflowTypeId },
                Selects = WorkflowDefinitionSelect.Id,
            });
            WorkflowDefinition WorkflowDefinition = WorkflowDefinitions.FirstOrDefault();
            if (WorkflowDefinition == null)
                return false;

            // khởi tạo workflow
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = new RequestWorkflowDefinitionMapping
            {
                RequestId = RequestId,
                WorkflowDefinitionId = WorkflowDefinition.Id,
                RequestStateId = RequestStateEnum.NEW.Id,
                CreatorId = CurrentContext.UserId,
            };
            await UOW.RequestWorkflowDefinitionMappingRepository.Update(RequestWorkflowDefinitionMapping);

            // khởi tạo trạng thái cho tất cả các nút
            List<RequestWorkflowStepMapping> RequestWorkflowStepMappings = new List<RequestWorkflowStepMapping>();
            foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
            {
                RequestWorkflowStepMapping RequestWorkflowStepMapping = new RequestWorkflowStepMapping();
                RequestWorkflowStepMappings.Add(RequestWorkflowStepMapping);
                RequestWorkflowStepMapping.RequestId = RequestId;
                RequestWorkflowStepMapping.WorkflowStepId = WorkflowStep.Id;
                RequestWorkflowStepMapping.WorkflowStateId = WorkflowStateEnum.NEW.Id;
                RequestWorkflowStepMapping.CreatedAt = StaticParams.DateTimeNow;
                RequestWorkflowStepMapping.UpdatedAt = null;
                RequestWorkflowStepMapping.AppUserId = null;
            }
            await UOW.RequestWorkflowStepMappingRepository.BulkMerge(RequestId, RequestWorkflowStepMappings);

            await UpdateParameters(RequestId, WorkflowDefinition, Parameters);
            return true;
        }

        public async Task<bool> Start(Guid RequestId, Dictionary<string, string> Parameters)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(RequestWorkflowDefinitionMapping.WorkflowDefinitionId);

            if (WorkflowDefinition == null)
            {
                return false;
            }
            else
            {
                // khởi tạo workflow
                RequestWorkflowDefinitionMapping = new RequestWorkflowDefinitionMapping
                {
                    RequestId = RequestId,
                    WorkflowDefinitionId = WorkflowDefinition.Id,
                    RequestStateId = RequestStateEnum.APPROVING.Id,
                    CreatorId = CurrentContext.UserId,
                };
                await UOW.RequestWorkflowDefinitionMappingRepository.Update(RequestWorkflowDefinitionMapping);
                List<RequestWorkflowStepMapping> RequestWorkflowStepMappings = new List<RequestWorkflowStepMapping>();
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
                    RequestWorkflowStepMapping RequestWorkflowStepMapping = new RequestWorkflowStepMapping();
                    RequestWorkflowStepMappings.Add(RequestWorkflowStepMapping);
                    RequestWorkflowStepMapping.RequestId = RequestId;
                    RequestWorkflowStepMapping.WorkflowStepId = WorkflowStep.Id;
                    RequestWorkflowStepMapping.WorkflowStateId = WorkflowStateEnum.NEW.Id;
                    RequestWorkflowStepMapping.CreatedAt = StaticParams.DateTimeNow;
                    RequestWorkflowStepMapping.UpdatedAt = null;
                    RequestWorkflowStepMapping.AppUserId = null;
                    if (Starts.Any(s => s.Id == WorkflowStep.Id))
                    {
                        RequestWorkflowStepMapping.WorkflowStateId = WorkflowStateEnum.PENDING.Id;
                    }
                }
                await UOW.RequestWorkflowStepMappingRepository.BulkMerge(RequestId, RequestWorkflowStepMappings);

                await UpdateParameters(RequestId, WorkflowDefinition, Parameters);
            }
            return true;
        }

        public async Task<bool> Approve(Guid RequestId, Dictionary<string, string> Parameters)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(RequestWorkflowDefinitionMapping.WorkflowDefinitionId);
            // tìm điểm bắt đầu
            // tìm điểm nhảy tiếp theo
            // chuyển trạng thái điểm nhảy
            // gửi mail cho các điểm nhảy có trạng thái thay đổi.
            List<RequestWorkflowStepMapping> RequestWorkflowStepMappings = await UOW.RequestWorkflowStepMappingRepository.List(RequestId);
            if (WorkflowDefinition != null && RequestWorkflowStepMappings.Count > 0)
            {
                // Tìm điểm đích
                List<WorkflowStep> ToSteps = new List<WorkflowStep>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        var StartRequestStep = RequestWorkflowStepMappings
                            .Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id)
                            .Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        if (StartRequestStep != null)
                        {
                            StartRequestStep.WorkflowStateId = WorkflowStateEnum.APPROVED.Id;
                            StartRequestStep.UpdatedAt = StaticParams.DateTimeNow;
                            StartRequestStep.AppUserId = CurrentContext.UserId;

                            var NextSteps = WorkflowDefinition.WorkflowDirections.Where(d => d.FromStepId == WorkflowStep.Id)
                                .Select(x => x.ToStep) ?? new List<WorkflowStep>();
                            ToSteps.AddRange(NextSteps);
                        }
                    }
                }

                ToSteps = ToSteps.Distinct().ToList();
                List<Mail> Mails = new List<Mail>();
                var NextStepIds = new List<long>();
                var PreviousStepIds = new List<long>();
                // tìm điểm bắt đầu cho những điểm đích tìm được
                foreach (WorkflowStep WorkflowStep in ToSteps)
                {
                    var FromSteps = WorkflowDefinition.WorkflowDirections.Where(d => d.ToStepId == WorkflowStep.Id).Select(x => x.FromStep) ?? new List<WorkflowStep>();
                    var ApprovedFromRequestSteps = new List<RequestWorkflowStepMapping>();
                    foreach (var FromStep in FromSteps)
                    {
                        var FromRequestStep = RequestWorkflowStepMappings.Where(x => x.WorkflowStepId == FromStep.Id).FirstOrDefault();
                        ApprovedFromRequestSteps.Add(FromRequestStep);
                        PreviousStepIds.Add(FromStep.Id);
                    }

                    // Nếu tất cả các điểm bắt đầu của điểm đích đang xét đều là APPROVED thì điểm đích sẽ là PENDING
                    if (ApprovedFromRequestSteps.All(x => x.WorkflowStateId == WorkflowStateEnum.APPROVED.Id))
                    {
                        var RequestWorkflowStepMapping = RequestWorkflowStepMappings.Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        RequestWorkflowStepMapping.WorkflowStateId = WorkflowStateEnum.PENDING.Id;
                        RequestWorkflowStepMapping.UpdatedAt = null;
                        RequestWorkflowStepMapping.AppUserId = null;
                        NextStepIds.Add(WorkflowStep.Id);
                    }
                }
                await UOW.RequestWorkflowStepMappingRepository.BulkMerge(RequestId, RequestWorkflowStepMappings);

                // Gửi mail cho những direction mà điểm nguồn và đích được tìm ra bên trên
                List<WorkflowDirection> WorkflowDirections = WorkflowDefinition.WorkflowDirections
                    .Where(x => PreviousStepIds.Contains(x.FromStepId) && NextStepIds.Contains(x.ToStepId)).ToList();
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
                        Subject = CreateMailContent(WorkflowDirection.SubjectMailForCreator, Parameters),
                        Body = CreateMailContent(WorkflowDirection.BodyMailForCreator, Parameters),
                    };

                    Mail MailForNextStep = new Mail
                    {
                        Recipients = recipients,
                        Subject = CreateMailContent(WorkflowDirection.SubjectMailForNextStep, Parameters),
                        Body = CreateMailContent(WorkflowDirection.BodyMailForNextStep, Parameters),
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

        public async Task<bool> Reject(Guid RequestId, Dictionary<string, string> Parameters)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(RequestWorkflowDefinitionMapping.WorkflowDefinitionId);
            List<RequestWorkflowStepMapping> RequestWorkflowStepMappings = await UOW.RequestWorkflowStepMappingRepository.List(RequestId);
            if (WorkflowDefinition != null && RequestWorkflowStepMappings.Count > 0)
            {
                List<WorkflowStep> ToSteps = new List<WorkflowStep>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        var StartRequestStep = RequestWorkflowStepMappings
                            .Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id)
                            .Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        StartRequestStep.WorkflowStateId = WorkflowStateEnum.REJECTED.Id;
                        StartRequestStep.UpdatedAt = StaticParams.DateTimeNow;
                        StartRequestStep.AppUserId = CurrentContext.UserId;
                        AppUserFilter AppUserFilter = new AppUserFilter
                        {
                            Id = new IdFilter { Equal = RequestWorkflowDefinitionMapping.CreatorId },
                            Skip = 0,
                            Take = int.MaxValue,
                            Selects = AppUserSelect.Email
                        };
                        List<AppUser> appUsers = await UOW.AppUserRepository.List(AppUserFilter);
                        List<string> recipients = appUsers.Select(au => au.Email).Distinct().ToList();
                        Mail MailForReject = new Mail
                        {
                            Recipients = recipients,
                            Subject = CreateMailContent(WorkflowStep.SubjectMailForReject, Parameters),
                            Body = CreateMailContent(WorkflowStep.BodyMailForReject, Parameters),
                        };
                        await MailService.Send(MailForReject);
                    }
                }
            }

            return true;
        }

        public async Task<GenericEnum> End(Guid RequestId, Dictionary<string, string> Parameters)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            if (RequestWorkflowDefinitionMapping == null)
                return null;
            List<RequestWorkflowStepMapping> RequestWorkflowStepMapping = await UOW.RequestWorkflowStepMappingRepository.List(RequestId);

            GenericEnum RequestState = RequestStateEnum.APPROVING;
            if (RequestWorkflowStepMapping != null)
            {
                if (RequestWorkflowStepMapping.Any(x => x.WorkflowStateId == WorkflowStateEnum.REJECTED.Id))
                {
                    RequestState = RequestStateEnum.REJECTED;
                }
                else if (RequestWorkflowStepMapping.All(x => x.WorkflowStateId == WorkflowStateEnum.APPROVED.Id))
                {
                    RequestState = RequestStateEnum.APPROVED;
                }
            }

            if (RequestState == null)
                return null;
            RequestWorkflowDefinitionMapping = new RequestWorkflowDefinitionMapping
            {
                RequestId = RequestId,
                WorkflowDefinitionId = RequestWorkflowDefinitionMapping.WorkflowDefinitionId,
                RequestStateId = RequestState.Id,
            };
            await UOW.RequestWorkflowDefinitionMappingRepository.Update(RequestWorkflowDefinitionMapping);

            return RequestState;
        }

        private string CreateMailContent(string templateContent, Dictionary<string, string> Parameters)
        {
            var template = Handlebars.Compile(templateContent);
            var result = template(Parameters);
            return result;
        }

        public async Task UpdateParameters(Guid RequestId, WorkflowDefinition WorkflowDefinition, Dictionary<string, string> Parameters)
        {
            // khởi tạo các parameter cho workflow
            List<RequestWorkflowParameterMapping> RequestWorkflowParameterMappings = new List<RequestWorkflowParameterMapping>();
            foreach (WorkflowParameter WorkflowParameter in WorkflowDefinition.WorkflowParameters)
            {
                RequestWorkflowParameterMapping RequestWorkflowParameterMapping = new RequestWorkflowParameterMapping();
                RequestWorkflowParameterMappings.Add(RequestWorkflowParameterMapping);
                RequestWorkflowParameterMapping.RequestId = RequestId;
                RequestWorkflowParameterMapping.WorkflowParameterId = WorkflowParameter.Id;
                RequestWorkflowParameterMapping.Value = null;
                foreach (var pair in Parameters)
                {
                    if (WorkflowParameter.Name == pair.Key)
                    {
                        RequestWorkflowParameterMapping.Value = pair.Value;
                    }
                }
            }
            await UOW.RequestWorkflowParameterMappingRepository.BulkMerge(RequestId, RequestWorkflowParameterMappings);
        }

       
    }
}
