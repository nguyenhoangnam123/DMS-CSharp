using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Rpc.store;
using DMS.Rpc.store_scouting;
using DMS.Services.MImage;
using DMS.Services.MNotification;
using DMS.Services.MWorkflow;
using DMS.Helpers;
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
        Task<int> CountInScoped(StoreFilter StoreFilter, long AppUserId);
        Task<List<Store>> ListInScoped(StoreFilter StoreFilter, long AppUserId);
        Task<Store> Get(long Id);
        Task<Store> Create(Store Store);
        Task<Store> Update(Store Store);
        Task<Store> Delete(Store Store);
        //Task<Store> Send(Store Store);
        //Task<Store> Approve(Store Store);
        //Task<Store> Reject(Store Store);
        Task<List<Store>> BulkDelete(List<Store> Stores);
        Task<List<Store>> Import(List<Store> Stores);
        Task<List<Store>> Export(StoreFilter StoreFilter);
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

        public async Task<int> CountInScoped(StoreFilter StoreFilter, long AppUserId)
        {
            try
            {
                int result = await UOW.StoreRepository.CountInScoped(StoreFilter, AppUserId);
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

        public async Task<List<Store>> ListInScoped(StoreFilter StoreFilter, long AppUserId)
        {
            try
            {
                List<Store> Stores = await UOW.StoreRepository.ListInScoped(StoreFilter, AppUserId);
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

            List<StoreUser> StoreUsers = await UOW.StoreUserRepository.List(
                new StoreUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    StoreId = new IdFilter { Equal = Id },
                    Selects = StoreUserSelect.Id | StoreUserSelect.Username | StoreUserSelect.Status,
                });

            if(StoreUsers.FirstOrDefault() != null)
            {
                Store.StoreUser = StoreUsers.FirstOrDefault();
                Store.StoreUserId = Store.StoreUser.Id;
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
                Store.CreatorId = CurrentUser.Id;
                Store.UnsignName = Store.Name.ChangeToEnglishChar();
                Store.UnsignAddress = Store.Address.ChangeToEnglishChar();
                var Counter = await UOW.IdGenerateRepository.GetCounter();
                StoreCodeGenerate(Store, Counter);

                if (Store.BrandInStores != null)
                {
                    Store.BrandInStores.ForEach(x => x.CreatorId = CurrentContext.UserId);
                }

                await UOW.Begin();
                await UOW.StoreRepository.Create(Store);

                StoreStatusHistory StoreStatusHistory = new StoreStatusHistory
                {
                    StoreId = Store.Id,
                    AppUserId = CurrentContext.UserId,
                    CreatedAt = DateTime.Now,
                };
                if (Store.StoreScoutingId.HasValue)
                    StoreStatusHistory.PreviousStoreStatusId = StoreStatusHistoryTypeEnum.STORE_SCOUTING.Id;
                else
                    StoreStatusHistory.PreviousStoreStatusId = StoreStatusHistoryTypeEnum.NEW.Id;

                if (Store.StoreStatusId == StoreStatusEnum.OFFICIAL.Id)
                    StoreStatusHistory.StoreStatusId = StoreStatusHistoryTypeEnum.OFFICIAL.Id;
                if (Store.StoreStatusId == StoreStatusEnum.DRAFT.Id)
                    StoreStatusHistory.StoreStatusId = StoreStatusHistoryTypeEnum.DRAFT.Id;

                await UOW.StoreStatusHistoryRepository.Create(StoreStatusHistory);

                List<UserNotification> UserNotifications = new List<UserNotification>();
                if (Store.StoreScoutingId.HasValue)
                {
                    StoreScouting StoreScouting = await UOW.StoreScoutingRepository.Get(Store.StoreScoutingId.Value);
                    if (StoreScouting != null)
                    {
                        StoreScouting.StoreScoutingStatusId = Enums.StoreScoutingStatusEnum.OPENED.Id;
                    }
                    await UOW.StoreScoutingRepository.Update(StoreScouting);

                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đại lý cắm cờ {StoreScouting.Code} - {StoreScouting.Name} đã được mở đại lý bởi {CurrentUser.DisplayName}.",
                        LinkWebsite = $"{StoreRoute.Master}/#*".Replace("*", Store.Id.ToString()),
                        LinkMobile = $"{StoreRoute.Mobile}".Replace("*", Store.Id.ToString()),
                        RecipientId = StoreScouting.CreatorId,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false
                    };
                    UserNotifications.Add(UserNotification);

                    Store.StoreImageMappings = StoreScouting.StoreScoutingImageMappings?.Select(x => new StoreImageMapping
                    {
                        StoreId = Store.Id,
                        ImageId = x.ImageId,
                        Image = new Image
                        {
                            Id = x.Image.Id,
                            Name = x.Image.Name,
                            Url = x.Image.Url,
                        }
                    }).ToList();
                    await UOW.StoreRepository.Update(Store);
                }
                await UOW.Commit();

                Store = await UOW.StoreRepository.Get(Store.Id);
                Sync(new List<Store> { Store });

                var RecipientIds = await ListAppUserInOrgs(Store);
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đại lý {Store.Code} - {Store.Name} vừa được thêm mới vào hệ thống bởi {CurrentUser.DisplayName}.",
                        LinkWebsite = $"{StoreRoute.Master}/#*".Replace("*", Store.Id.ToString()),
                        LinkMobile = $"{StoreRoute.Mobile}".Replace("*", Store.Id.ToString()),
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false,
                        RowId = Guid.NewGuid(),
                    };
                    UserNotifications.Add(UserNotification);
                }

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.UserNotificationSend);

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

                Store.UnsignName = Store.Name.ChangeToEnglishChar();
                Store.UnsignAddress = Store.Address.ChangeToEnglishChar();
                StoreCodeGenerate(Store);

                if (Store.BrandInStores != null)
                {
                    foreach (var BrandInStore in Store.BrandInStores)
                    {
                        if (BrandInStore.Id == 0)
                            BrandInStore.CreatorId = CurrentContext.UserId;
                    }
                }

                if (Store.StoreStatusId != oldData.StoreStatusId)
                {
                    StoreStatusHistory StoreStatusHistory = new StoreStatusHistory
                    {
                        StoreId = Store.Id,
                        AppUserId = CurrentContext.UserId,
                        CreatedAt = DateTime.Now,
                    };
                    if (oldData.StoreStatusId == StoreStatusEnum.OFFICIAL.Id)
                        StoreStatusHistory.PreviousStoreStatusId = StoreStatusHistoryTypeEnum.OFFICIAL.Id;
                    if (oldData.StoreStatusId == StoreStatusEnum.DRAFT.Id)
                        StoreStatusHistory.PreviousStoreStatusId = StoreStatusHistoryTypeEnum.DRAFT.Id;

                    if (Store.StoreStatusId == StoreStatusEnum.OFFICIAL.Id)
                        StoreStatusHistory.StoreStatusId = StoreStatusHistoryTypeEnum.OFFICIAL.Id;
                    if (Store.StoreStatusId == StoreStatusEnum.DRAFT.Id)
                        StoreStatusHistory.StoreStatusId = StoreStatusHistoryTypeEnum.DRAFT.Id;

                    await UOW.StoreStatusHistoryRepository.Create(StoreStatusHistory);
                }

                await UOW.Begin();
                await UOW.StoreRepository.Update(Store);
                if (Store.StatusId == StatusEnum.INACTIVE.Id)
                {
                    await UOW.AppUserStoreMappingRepository.Delete(null, oldData.Id);
                }
                if (Store.StoreStatusId == StoreStatusEnum.OFFICIAL.Id && oldData.StoreStatusId != StoreStatusEnum.OFFICIAL.Id)
                {
                    if (oldData.AppUserId.HasValue)
                        await UOW.AppUserStoreMappingRepository.Delete(oldData.AppUserId.Value, oldData.Id);
                    if (Store.AppUserId.HasValue)
                        await UOW.AppUserStoreMappingRepository.Update(Store.AppUserId.Value, Store.Id);
                }
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await ListAppUserInOrgs(Store);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đại lý {Store.Code} - {Store.Name} vừa được cập nhật thông tin bởi {CurrentUser.DisplayName}.",
                        LinkWebsite = $"{StoreRoute.Master}/#*".Replace("*", Store.Id.ToString()),
                        LinkMobile = $"{StoreRoute.Mobile}".Replace("*", Store.Id.ToString()),
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false
                    };
                    UserNotifications.Add(UserNotification);
                }
                if (Store.StoreStatusId == StoreStatusEnum.OFFICIAL.Id && oldData.StoreStatusId != Store.StoreStatusId)
                {
                    foreach (var Id in RecipientIds)
                    {
                        UserNotification UserNotification = new UserNotification
                        {
                            TitleWeb = $"Thông báo từ DMS",
                            ContentWeb = $"Đại lý dự thảo {Store.Code} - {Store.Name} vừa được phê duyệt thành đại lý chính thức bởi {CurrentUser.DisplayName}.",
                            LinkWebsite = $"{StoreRoute.Master}/#*".Replace("*", Store.Id.ToString()),
                            LinkMobile = $"{StoreRoute.Mobile}".Replace("*", Store.Id.ToString()),
                            RecipientId = Id,
                            SenderId = CurrentContext.UserId,
                            Time = StaticParams.DateTimeNow,
                            Unread = false,
                            RowId = Guid.NewGuid(),
                        };
                        UserNotifications.Add(UserNotification);
                    }
                }

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.UserNotificationSend);

                var newData = await UOW.StoreRepository.Get(Store.Id);
                Sync(new List<Store> { newData });
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
                var oldData = await UOW.StoreRepository.Get(Store.Id);
                await UOW.Begin();
                if (oldData.StoreStatusId == StoreStatusEnum.OFFICIAL.Id)
                {
                    if (oldData.AppUserId.HasValue)
                        await UOW.AppUserStoreMappingRepository.Update(oldData.AppUserId.Value, oldData.Id);
                }

                await UOW.StoreRepository.Delete(Store);
                await UOW.Commit();

                Store = await UOW.StoreRepository.Get(Store.Id);
                Sync(new List<Store> { Store });

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await ListAppUserInOrgs(Store);
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đại lý {Store.Code} - {Store.Name} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName}.",
                        RecipientId = Id,
                        SenderId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                        Unread = false,
                        RowId = Guid.NewGuid(),
                    };
                    UserNotifications.Add(UserNotification);
                }

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.UserNotificationSend);

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
                            ContentWeb = $"Đại lý {Store.Code} - {Store.Name} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName}.",
                            RecipientId = Id,
                            SenderId = CurrentContext.UserId,
                            Time = StaticParams.DateTimeNow,
                            Unread = false,
                            RowId = Guid.NewGuid(),
                        };
                        UserNotifications.Add(UserNotification);
                    }
                }

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.UserNotificationSend);

                var Ids = Stores.Select(x => x.Id).ToList();
                Stores = await UOW.StoreRepository.List(Ids);
                Sync(Stores);

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
                    Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name | StoreSelect.ParentStore,
                };
                List<Store> dbStores = await UOW.StoreRepository.List(StoreFilter);
                var createCounter = Stores.Where(x => x.Id == 0).Count();
                var ListCounter = await UOW.IdGenerateRepository.ListCounter(createCounter);
                for (int i = 0; i < ListCounter.Count; i++)
                {
                    var newStores = Stores.Where(x => x.Id == 0).ToList();
                    var counter = ListCounter[i];
                    StoreCodeGenerate(newStores[i], counter);
                }
                foreach (var Store in Stores)
                {
                    Store.UnsignName = Store.Name.ChangeToEnglishChar();
                    Store.UnsignAddress = Store.Address.ChangeToEnglishChar();

                    var oldData = dbStores.Where(x => x.Id == Store.Id)
                                .FirstOrDefault();
                    if (oldData != null)
                    {
                        Store.RowId = oldData.RowId;
                        Store.Used = oldData.Used;
                    }
                    else
                    {
                        Store.Used = false;
                        Store.RowId = Guid.NewGuid();
                    }

                    StoreCodeGenerate(Store);
                }
                Stores = Stores.Distinct().ToList();
                await UOW.StoreRepository.BulkMerge(Stores);

                dbStores = await UOW.StoreRepository.List(StoreFilter);
                foreach (var store in Stores)
                {
                    long StoreId = dbStores.Where(p => p.Code == store.Code)
                                .Select(x => x.Id)
                                .FirstOrDefault();
                    store.Id = StoreId;
                    if (store.ParentStore != null)
                    {
                        long ParentStoreId = dbStores.Where(p => p.Code == store.ParentStore.Code)
                                    .Select(x => x.Id)
                                    .FirstOrDefault();
                        if (ParentStoreId != 0)
                            store.ParentStoreId = ParentStoreId;
                    }
                }
                await UOW.StoreRepository.BulkMerge(Stores);
                await UOW.Commit();

                var Ids = Stores.Select(x => x.Id).ToList();
                Stores = await UOW.StoreRepository.List(Ids);
                Sync(Stores);

                await Logging.CreateAuditLog(Stores, new { }, nameof(StoreService));
                return null;
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

        public async Task<List<Store>> Export(StoreFilter StoreFilter)
        {
            try
            {
                StoreFilter.Selects = StoreSelect.Id;
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
                var Ids = Stores.Select(x => x.Id).ToList();
                Stores = await UOW.StoreRepository.List(Ids);
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
            string thumbnailPath = $"/store/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
            return Image;
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

        private void Sync(List<Store> Stores)
        {
            List<Brand> Brands = new List<Brand>();
            List<Store> SyncStores = new List<Store>();
            List<AppUser> AppUsers = new List<AppUser>();
            List<Store> Storess = new List<Store>();
            List<StoreType> StoreTypes = new List<StoreType>();
            List<StoreGrouping> StoreGroupings = new List<StoreGrouping>();
            List<Province> Provinces = new List<Province>();
            List<District> Districts = new List<District>();
            List<Ward> Wards = new List<Ward>();
            List<Organization> Organizations = new List<Organization>();
            foreach (var Store in Stores)
            {
                SyncStores.Add(Store);

                StoreTypes.Add(Store.StoreType);
                if (Store.AppUserId.HasValue)
                {
                    AppUsers.Add(Store.AppUser);
                }
                if (Store.ParentStoreId.HasValue)
                {
                    Storess.Add(Store.ParentStore);
                }
                if (Store.StoreGroupingId.HasValue)
                {
                    StoreGroupings.Add(Store.StoreGrouping);
                }
                if (Store.ProvinceId.HasValue)
                {
                    Provinces.Add(Store.Province);
                }
                if (Store.DistrictId.HasValue)
                {
                    Districts.Add(Store.District);
                }
                if (Store.WardId.HasValue)
                {
                    Wards.Add(Store.Ward);
                }

                if (Store.BrandInStores != null)
                {
                    foreach (var BrandInStore in Store.BrandInStores)
                    {
                        Brands.Add(BrandInStore.Brand);
                    }
                }
            }
            RabbitManager.PublishList(SyncStores, RoutingKeyEnum.StoreSync);
            AppUsers = AppUsers.Distinct().ToList();
            Stores = Stores.Distinct().ToList();
            StoreTypes = StoreTypes.Distinct().ToList();
            StoreGroupings = StoreGroupings.Distinct().ToList();
            Provinces = Provinces.Distinct().ToList();
            Districts = Districts.Distinct().ToList();
            Wards = Wards.Distinct().ToList();
            Organizations = Organizations.Distinct().ToList();
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed);
            RabbitManager.PublishList(Brands, RoutingKeyEnum.BrandUsed);
            RabbitManager.PublishList(Stores, RoutingKeyEnum.StoreUsed);
            RabbitManager.PublishList(StoreTypes, RoutingKeyEnum.StoreTypeUsed);
            RabbitManager.PublishList(StoreGroupings, RoutingKeyEnum.StoreGroupingUsed);
            RabbitManager.PublishList(Provinces, RoutingKeyEnum.ProvinceUsed);
            RabbitManager.PublishList(Districts, RoutingKeyEnum.DistrictUsed);
            RabbitManager.PublishList(Wards, RoutingKeyEnum.WardUsed);
            RabbitManager.PublishList(Organizations, RoutingKeyEnum.OrganizationUsed);
        }

        private void StoreCodeGenerate(Store Store, long? Counter = null)
        {
            if (Counter == null)
            {
                var Codes = Store.Code.Split('.');
                Store.Code = $"{Store.Organization.Code}.{Store.StoreType.Code}.{Codes[2]}";
            }
            else
            {
                Store.Code = $"{Store.Organization.Code}.{Store.StoreType.Code}.{(10000000 + Counter).ToString().Substring(1)}";
            }
        }
    }
}
