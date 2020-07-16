﻿using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Rpc.store;
using DMS.Rpc.store_scouting;
using DMS.Services.MImage;
using DMS.Services.MNotification;
using DMS.Services.MWorkflow;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
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
        Task<Store> Approve(Store Store);
        Task<Store> Reject(Store Store);
        Task<List<Store>> BulkDelete(List<Store> Stores);
        Task<List<Store>> Import(List<Store> Stores);
        StoreFilter ToFilter(StoreFilter StoreFilter);
        Task<Image> SaveImage(Image Image);
    }

    public class StoreService : BaseService, IStoreService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationService NotificationService;
        private IStoreValidator StoreValidator;
        private IWorkflowService WorkflowService;
        private IImageService ImageService;
        private IRabbitManager RabbitManager;
        public StoreService(
            IUOW UOW,
            ILogging Logging,
            INotificationService NotificationService,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IWorkflowService WorkflowService,
            IStoreValidator StoreValidator,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificationService = NotificationService;
            this.WorkflowService = WorkflowService;
            this.StoreValidator = StoreValidator;
            this.ImageService = ImageService;
            this.RabbitManager = RabbitManager;
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Store>> List(StoreFilter StoreFilter)
        {
            try
            {
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
                List<long> Ids = Stores.Select(x => x.Id).ToList();
                StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = StoreCheckingSelect.ALL,
                    StoreId = new IdFilter { In = Ids },
                    SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                    CheckOutAt = new DateFilter { GreaterEqual = StaticParams.DateTimeNow.Date, Less = StaticParams.DateTimeNow.Date.AddDays(1) }
                };
                List<StoreChecking> StoreCheckings = await UOW.StoreCheckingRepository.List(StoreCheckingFilter);
                foreach (var Store in Stores)
                {
                    var count = StoreCheckings.Where(x => x.StoreId == Store.Id).Count();
                    Store.HasChecking = count != 0 ? true : false;
                }
                return Stores;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Store> Get(long Id)
        {
            Store Store = await UOW.StoreRepository.Get(Id);
            if (Store == null)
                return null;

            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = 1,
                Selects = StoreCheckingSelect.ALL,
                StoreId = new IdFilter { Equal = Id },
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                CheckOutAt = new DateFilter { GreaterEqual = StaticParams.DateTimeNow.Date, Less = StaticParams.DateTimeNow.Date.AddDays(1) }
            };
            int count = await UOW.StoreCheckingRepository.Count(StoreCheckingFilter);
            Store.HasChecking = count != 0 ? true : false;
            Store.RequestState = await WorkflowService.GetRequestState(Store.RowId);
            if (Store.RequestState == null)
            {
                Store.RequestWorkflowStepMappings = new List<RequestWorkflowStepMapping>();
            }
            else
            {
                Store.RequestStateId = Store.RequestState.Id;
                Store.RequestWorkflowStepMappings = await WorkflowService.ListRequestWorkflowState(Store.RowId);
            }
            return Store;
        }

        public async Task<Store> Create(Store Store)
        {
            if (!await StoreValidator.Create(Store))
                return Store;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                await UOW.Begin();
                await UOW.StoreRepository.Create(Store);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                if (Store.StoreScoutingId.HasValue)
                {
                    StoreScouting StoreScouting = await UOW.StoreScoutingRepository.Get(Store.StoreScoutingId.Value);
                    if(StoreScouting != null)
                    {
                        StoreScouting.StoreScoutingStatusId = Enums.StoreScoutingStatusEnum.OPENED.Id;
                    }
                    await UOW.StoreScoutingRepository.Update(StoreScouting);

                    UserNotification Notification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Cửa hàng cắm cờ {StoreScouting.Code} - {StoreScouting.Name} đã được mở cửa hàng bởi {CurrentUser.DisplayName} vào lúc {StaticParams.DateTimeNow}",
                        LinkWebsite = $"{StoreScoutingRoute.Master}/?id=*".Replace("*", StoreScouting.Id.ToString()),
                        LinkMobile = $"{StoreScoutingRoute.Mobile}".Replace("*", StoreScouting.Id.ToString()),
                        RecipientId = StoreScouting.CreatorId,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false
                    };
                    UserNotifications.Add(Notification);
                }
                await WorkflowService.Initialize(Store.RowId, WorkflowTypeEnum.STORE.Id, MapParameters(Store));
                await UOW.Commit();

                NotifyUsed(Store);

                var RecipientIds = await ListAppUserInOrgs(Store);
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Cửa hàng {Store.Code} - {Store.Name} vừa được thêm mới vào hệ thống bởi {CurrentUser.DisplayName} vào lúc {StaticParams.DateTimeNow}",
                        LinkWebsite = $"{StoreRoute.Detail}".Replace("*", Store.Id.ToString()),
                        LinkMobile = $"{StoreRoute.Mobile}".Replace("*", Store.Id.ToString()),
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false
                    };
                    UserNotifications.Add(UserNotification);
                }
                
                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(Store, new { }, nameof(StoreService));
                return await UOW.StoreRepository.Get(Store.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
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

                NotifyUsed(Store);

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await ListAppUserInOrgs(Store);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Cửa hàng {Store.Code} - {Store.Name} vừa được cập nhật thông tin bởi {CurrentUser.DisplayName} vào lúc {StaticParams.DateTimeNow}",
                        LinkWebsite = $"{StoreRoute.Detail}".Replace("*", Store.Id.ToString()),
                        LinkMobile = $"{StoreRoute.Mobile}".Replace("*", Store.Id.ToString()),
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                var newData = await UOW.StoreRepository.Get(Store.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
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

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await ListAppUserInOrgs(Store);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Cửa hàng {Store.Code} - {Store.Name} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName} vào lúc {StaticParams.DateTimeNow}",
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(new { }, Store, nameof(StoreService));
                return Store;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
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

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Store in Stores)
                {
                    var RecipientIds = await ListAppUserInOrgs(Store);
                    foreach (var Id in RecipientIds)
                    {
                        UserNotification UserNotification = new UserNotification
                        {
                            TitleWeb = $"Thông báo từ DMS",
                            ContentWeb = $"Cửa hàng {Store.Code} - {Store.Name} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName} vào lúc {StaticParams.DateTimeNow}",
                            RecipientId = Id,
                            SenderId = CurrentContext.UserId,
                            Time = StaticParams.DateTimeNow,
                            Unread = false
                        };
                        UserNotifications.Add(UserNotification);
                    }
                }

                await NotificationService.BulkSend(UserNotifications);
                await Logging.CreateAuditLog(new { }, Stores, nameof(StoreService));
                return Stores;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
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
                        item.RowId = Store.RowId;
                        item.StatusId = StatusEnum.ACTIVE.Id;
                    }
                    else
                    {
                        item.Id = 0;
                        item.RowId = Guid.NewGuid();
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
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
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreGroupingId))
                        subFilter.StoreGroupingId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreTypeId))
                        subFilter.StoreTypeId = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }


        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/store/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
        }

        public async Task<Store> Approve(Store Store)
        {
            if (Store.Id == 0)
                Store = await Create(Store);
            else
                Store = await Update(Store);
            Dictionary<string, string> Parameters = MapParameters(Store);
            bool Approved = await WorkflowService.Approve(Store.RowId, WorkflowTypeEnum.STORE.Id, Parameters);
            if (Approved == false)
                return null;
            return await Get(Store.Id);
        }

        public async Task<Store> Reject(Store Store)
        {
            Store = await UOW.StoreRepository.Get(Store.Id);
            Dictionary<string, string> Parameters = MapParameters(Store);
            bool Rejected = await WorkflowService.Reject(Store.RowId, WorkflowTypeEnum.STORE.Id, Parameters);
            if (Rejected == false)
                return null;
            return await Get(Store.Id);
        }

        private Dictionary<string, string> MapParameters(Store Store)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add(nameof(Store.Id), Store.Id.ToString());
            Parameters.Add(nameof(Store.Code), Store.Code);
            Parameters.Add(nameof(Store.Name), Store.Name);
            Parameters.Add(nameof(Store.OwnerName), Store.OwnerName);
            Parameters.Add("Username", CurrentContext.UserName);
            return Parameters;
        }

        private async Task<List<long>> ListAppUserInOrgs(Store Store)
        {
            var Org = await UOW.OrganizationRepository.Get(Store.OrganizationId);
            List<long> Ids = new List<long>();
            if (Org != null)
            {
                var OrganizationIds = (await UOW.OrganizationRepository.List(new OrganizationFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = OrganizationSelect.Id,
                    Path = new StringFilter { StartWith = Org.Path }
                })).Select(x => x.Id).ToList();
                Ids = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id,
                    OrganizationId = new IdFilter { In = OrganizationIds }
                })).Select(x => x.Id).Distinct().ToList();
            }
            
            return Ids;
        }

        private void NotifyUsed(Store Store)
        {
            {
                EventMessage<StoreType> StoreTypeMessage = new EventMessage<StoreType>
                {
                    Content = new StoreType { Id = Store.StoreTypeId },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(StoreTypeMessage, RoutingKeyEnum.StoreTypeUsed);
            }
        }
    }
}
