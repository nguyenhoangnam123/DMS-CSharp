using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MWorkflowDefinition;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MStore
{
    public interface IStoreService : IServiceScoped
    {
        Task<int> Count(StoreFilter StoreFilter);
        Task<List<Store>> List(StoreFilter StoreFilter);
        Task<Store> Get(long Id);
        Task<Store> Create(Store Store);
        Task<Store> Update(Store Store);
        Task<Store> Delete(Store Store);
        Task<Store> Start(Store Store);
        Task<Store> End(Store Store);
        Task<Store> Approve(Store Store);
        Task<Store> Reject(Store Store);
        Task<List<Store>> BulkDelete(List<Store> Stores);
        Task<List<Store>> Import(List<Store> Stores);
        StoreFilter ToFilter(StoreFilter StoreFilter);
    }

    public class StoreService : BaseService, IStoreService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IMailService MailService;
        private IStoreValidator StoreValidator;
        private IWorkflowDefinitionService WorkflowDefinitionService;

        public StoreService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IMailService MailService,
            IWorkflowDefinitionService WorkflowDefinitionService,
            IStoreValidator StoreValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.MailService = MailService;
            this.WorkflowDefinitionService = WorkflowDefinitionService;
            this.StoreValidator = StoreValidator;
        }
        public async Task<int> Count(StoreFilter StoreFilter)
        {
            try
            {
                int result = await UOW.StoreRepository.Count(StoreFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Store>> List(StoreFilter StoreFilter)
        {
            try
            {
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
                return Stores;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Store> Get(long Id)
        {
            Store Store = await UOW.StoreRepository.Get(Id);
            if (Store == null)
                return null;
            return Store;
        }

        public async Task<Store> Create(Store Store)
        {
            if (!await StoreValidator.Create(Store))
                return Store;

            try
            {
                Store.Id = 0;
                await UOW.Begin();
                await UOW.StoreRepository.Create(Store);
                await UOW.Commit();

                await Logging.CreateAuditLog(Store, new { }, nameof(StoreService));
                return await UOW.StoreRepository.Get(Store.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Store> Update(Store Store)
        {
            if (!await StoreValidator.Update(Store))
                return Store;
            try
            {
                var oldData = await UOW.StoreRepository.Get(Store.Id);

                await UOW.Begin();
                await UOW.StoreRepository.Update(Store);
                await UOW.Commit();

                var newData = await UOW.StoreRepository.Get(Store.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Store> Delete(Store Store)
        {
            if (!await StoreValidator.Delete(Store))
                return Store;

            try
            {
                await UOW.Begin();
                await UOW.StoreRepository.Delete(Store);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Store, nameof(StoreService));
                return Store;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Store>> BulkDelete(List<Store> Stores)
        {
            if (!await StoreValidator.BulkDelete(Stores))
                return Stores;

            try
            {
                await UOW.Begin();
                await UOW.StoreRepository.BulkDelete(Stores);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Stores, nameof(StoreService));
                return Stores;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Store>> Import(List<Store> Stores)
        {
            if (!await StoreValidator.Import(Stores))
                return Stores;

            try
            {
                await UOW.Begin();
                StoreFilter StoreFilter = new StoreFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = StoreSelect.ALL,
                };
                List<Store> dbStores = await UOW.StoreRepository.List(StoreFilter);
                foreach (var item in Stores)
                {
                    Store Store = dbStores.Where(p => p.Code == item.Code)
                                .FirstOrDefault();
                    if (Store != null)
                    {
                        item.Id = Store.Id;
                        item.StatusId = StatusEnum.ACTIVE.Id;
                    }
                    else
                    {
                        item.Id = 0;
                        item.StatusId = StatusEnum.ACTIVE.Id;
                    }
                }
                await UOW.StoreRepository.BulkMerge(Stores);

                dbStores = await UOW.StoreRepository.List(StoreFilter);
                foreach (var item in Stores)
                {
                    long StoreId = dbStores.Where(p => p.Code == item.Code)
                                .Select(x => x.Id)
                                .FirstOrDefault();
                    item.Id = StoreId;
                    if (item.ParentStore != null)
                    {
                        long ParentStoreId = dbStores.Where(p => p.Code == item.ParentStore.Code)
                                    .Select(x => x.Id)
                                    .FirstOrDefault();
                        item.ParentStoreId = ParentStoreId;
                    }
                }
                await UOW.StoreRepository.BulkMerge(Stores);
                await UOW.Commit();

                await Logging.CreateAuditLog(Stores, new { }, nameof(StoreService));
                return Stores;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public StoreFilter ToFilter(StoreFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreFilter subFilter = new StoreFilter();
                filter.OrFilter.Add(subFilter);

            }
            return filter;
        }

        public async Task<Store> Start(Store Store)
        {
            WorkflowDefinition WorkflowDefinition = (await WorkflowDefinitionService.List(new WorkflowDefinitionFilter
            {
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                OrderBy = WorkflowDefinitionOrder.StartDate,
                OrderType = OrderType.DESC,
                Skip = 0,
                Take = 1,
                Selects = WorkflowDefinitionSelect.Id,
                WorkflowTypeId = new IdFilter { Equal = WorkflowTypeEnum.STORE.Id }
            })).FirstOrDefault();

            if (WorkflowDefinition == null)
            {

            }
            else
            {
                WorkflowDefinition = await WorkflowDefinitionService.Get(WorkflowDefinition.Id);
                Store.WorkflowDefinitionId = WorkflowDefinition.Id;
                Store.RequestStateId = RequestStateEnum.APPROVING.Id;
                Store.StoreWorkflows = new List<StoreWorkflow>();
                WorkflowStep Start = null;
                // tìm điểm bắt đầu của workflow
                // nút không có ai trỏ đến là nút bắt đầu
                // trong trường hợp có nhiều nút bắt đầu thì xét có nút nào thuộc các role hiện tại không
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (!WorkflowDefinition.WorkflowDirections.Any(d => d.ToStepId == WorkflowStep.Id) &&
                        CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        Start = WorkflowStep;
                        break;
                    }
                }
                // khởi tạo trạng thái cho tất cả các nút
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    StoreWorkflow StoreWorkflow = new StoreWorkflow();
                    Store.StoreWorkflows.Add(StoreWorkflow);
                    StoreWorkflow.WorkflowStepId = WorkflowStep.Id;
                    StoreWorkflow.WorkflowStateId = WorkflowStateEnum.NEW.Id;
                    StoreWorkflow.UpdatedAt = null;
                    StoreWorkflow.AppUserId = null;
                    if (Start.Id == WorkflowStep.Id)
                    {
                        StoreWorkflow.WorkflowStateId = WorkflowStateEnum.PENDING.Id;
                    }
                }
                Store.StoreWorkflowParameterMappings = new List<StoreWorkflowParameterMapping>();
                foreach (WorkflowParameter WorkflowParameter in WorkflowDefinition.WorkflowParameters)
                {
                    StoreWorkflowParameterMapping StoreWorkflowParameterMapping = new StoreWorkflowParameterMapping();
                    Store.StoreWorkflowParameterMappings.Add(StoreWorkflowParameterMapping);
                    StoreWorkflowParameterMapping.WorkflowParameterId = WorkflowParameter.Id;
                    StoreWorkflowParameterMapping.StoreId = Store.Id;
                    StoreWorkflowParameterMapping.Value = null;
                    switch (WorkflowParameter.Name)
                    {
                        case nameof(Store.Code):
                            StoreWorkflowParameterMapping.Value = Store.Code;
                            break;
                        case nameof(Store.Name):
                            StoreWorkflowParameterMapping.Value = Store.Name;
                            break;
                        case nameof(Store.StoreTypeId):
                            StoreWorkflowParameterMapping.Value = Store.StoreTypeId.ToString();
                            break;
                    }
                }

            }
            return Store;
        }

        public async Task<Store> Approve(Store Store)
        {
            Store = await UOW.StoreRepository.Get(Store.Id);
            WorkflowDefinition WorkflowDefinition = await WorkflowDefinitionService.Get(Store.WorkflowDefinitionId.Value);
            // tìm điểm bắt đầu
            // tìm điểm nhảy tiếp theo
            // chuyển trạng thái điểm nhảy
            // gửi mail cho các điểm nhảy có trạng thái thay đổi.

            if (WorkflowDefinition != null && Store.StoreWorkflows != null)
            {
                List<WorkflowStep> ToSteps = new List<WorkflowStep>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        var StartNode = Store.StoreWorkflows
                            .Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id)
                            .Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        StartNode.WorkflowStateId = WorkflowStateEnum.APPROVED.Id;
                        StartNode.UpdatedAt = StaticParams.DateTimeNow;
                        StartNode.AppUserId = CurrentContext.UserId;

                        var NextSteps = WorkflowDefinition.WorkflowDirections.Where(d => d.FromStepId == WorkflowStep.Id).Select(x => x.ToStep) ?? new List<WorkflowStep>();
                        ToSteps.AddRange(NextSteps);
                    }
                }

                ToSteps = ToSteps.Distinct().ToList();
                List<Mail> Mails = new List<Mail>();
                var NextStepIds = new List<long>();
                foreach (WorkflowStep WorkflowStep in ToSteps)
                {
                    var FromSteps = WorkflowDefinition.WorkflowDirections.Where(d => d.ToStepId == WorkflowStep.Id).Select(x => x.FromStep) ?? new List<WorkflowStep>();
                    var FromNodes = new List<StoreWorkflow>();
                    foreach (var FromStep in FromSteps)
                    {
                        var FromNode = Store.StoreWorkflows.Where(x => x.WorkflowStepId == FromStep.Id).FirstOrDefault();
                        FromNodes.Add(FromNode);
                    }

                    if (FromNodes.All(x => x.WorkflowStateId == WorkflowStateEnum.APPROVED.Id))
                    {
                        var StoreWorkflow = Store.StoreWorkflows.Where(x => x.WorkflowStepId == WorkflowStep.Id).FirstOrDefault();
                        StoreWorkflow.WorkflowStateId = WorkflowStateEnum.PENDING.Id;
                        StoreWorkflow.UpdatedAt = null;
                        StoreWorkflow.AppUserId = null;
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
            }

            return Store;
        }

        public async Task<Store> Reject(Store Store)
        {
            Store = await UOW.StoreRepository.Get(Store.Id);
            WorkflowDefinition WorkflowDefinition = await WorkflowDefinitionService.Get(Store.WorkflowDefinitionId.Value);

            if (WorkflowDefinition != null && Store.StoreWorkflows != null)
            {
                List<WorkflowStep> ToSteps = new List<WorkflowStep>();
                foreach (WorkflowStep WorkflowStep in WorkflowDefinition.WorkflowSteps)
                {
                    if (CurrentContext.RoleIds.Contains(WorkflowStep.RoleId))
                    {
                        var StartNode = Store.StoreWorkflows
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
                    StoreWorkflow StartNode = Store.StoreWorkflows.Where(x => x.WorkflowStepId == StartStep.Id).FirstOrDefault();
                    if (StartNode != null)
                    {
                        //gửi mail
                    }
                }
            }

            return Store;
        }

        public async Task<Store> End(Store Store)
        {
            Store = await UOW.StoreRepository.Get(Store.Id);
            WorkflowDefinition WorkflowDefinition = await WorkflowDefinitionService.Get(Store.WorkflowDefinitionId.Value);

            if (WorkflowDefinition != null && Store.StoreWorkflows != null)
            {
                if (Store.StoreWorkflows.Any(x => x.WorkflowStateId == WorkflowStateEnum.REJECTED.Id))
                {
                    Store.RequestStateId = RequestStateEnum.REJECTED.Id;
                }
                else if (Store.StoreWorkflows.All(x => x.WorkflowStateId == WorkflowStateEnum.APPROVED.Id))
                {
                    Store.RequestStateId = RequestStateEnum.APPROVED.Id;
                }
            }

            return Store;
        }
    }
}
