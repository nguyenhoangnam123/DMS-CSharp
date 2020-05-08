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
        Task<bool> Start(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters);
        Task<bool> Approce(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters);
        Task<bool> Reject(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters);
        Task<bool> End(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters);
    }
    public class WorkflowService
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

        public async Task<long> IsStarted(Guid RequestId)
        {
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
            if (RequestWorkflowDefinitionMapping == null)
                return 0;
            else
                return RequestWorkflowDefinitionMapping.WorkflowDefinitionId;
        }
        public async Task<bool> Start(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinitionId);

            if (WorkflowDefinition == null)
            {
                return false;
            }
            else
            {
                // khởi tạo workflow
                RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = new RequestWorkflowDefinitionMapping
                {
                    RequestId = RequestId,
                    WorkflowDefinitionId = WorkflowDefinitionId,
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
            return true;
        }

        public async Task<bool> Approve(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinitionId);
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

        public async Task<bool> Reject(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
        {
            WorkflowDefinition WorkflowDefinition = await UOW.WorkflowDefinitionRepository.Get(WorkflowDefinitionId);
            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(RequestId);
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

        public async Task<GenericEnum> End(Guid RequestId, long WorkflowDefinitionId, Dictionary<string, string> Parameters)
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
                WorkflowDefinitionId = WorkflowDefinitionId,
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
    }
}
