using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Services.MImage;
using GeoCoordinatePortable;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace DMS.Services.MStoreChecking
{
    public interface IStoreCheckingService : IServiceScoped
    {
        Task<int> Count(StoreCheckingFilter StoreCheckingFilter);
        Task<List<StoreChecking>> List(StoreCheckingFilter StoreCheckingFilter);

        Task<StoreChecking> Get(long Id);
        Task<StoreChecking> CheckIn(StoreChecking StoreChecking);
        Task<StoreChecking> Update(StoreChecking StoreChecking);
        Task<StoreChecking> CheckOut(StoreChecking StoreChecking);
        Task<Image> SaveImage(Image Image);
        Task<long> CountStore(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<List<Store>> ListStore(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<long> CountStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<List<Store>> ListStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<long> CountStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<List<Store>> ListStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<long> CountStoreInScope(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<List<Store>> ListStoreInScope(StoreFilter StoreFilter, IdFilter ERouteId);
        StoreCheckingFilter ToFilter(StoreCheckingFilter StoreCheckingFilter);
    }

    public class StoreCheckingService : BaseService, IStoreCheckingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IRabbitManager RabbitManager;
        private IImageService ImageService;

        private IStoreCheckingValidator StoreCheckingValidator;

        public StoreCheckingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IImageService ImageService,
            IStoreCheckingValidator StoreCheckingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.RabbitManager = RabbitManager;
            this.ImageService = ImageService;
            this.StoreCheckingValidator = StoreCheckingValidator;
        }
        public async Task<int> Count(StoreCheckingFilter StoreCheckingFilter)
        {
            try
            {
                int result = await UOW.StoreCheckingRepository.Count(StoreCheckingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<StoreChecking>> List(StoreCheckingFilter StoreCheckingFilter)
        {
            try
            {
                List<StoreChecking> StoreCheckings = await UOW.StoreCheckingRepository.List(StoreCheckingFilter);
                return StoreCheckings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<StoreChecking> Get(long Id)
        {
            StoreChecking StoreChecking = await UOW.StoreCheckingRepository.Get(Id);
            if (StoreChecking == null)
                return null;
            return StoreChecking;
        }

        public async Task<StoreChecking> CheckIn(StoreChecking StoreChecking)
        {
            if (!await StoreCheckingValidator.CheckIn(StoreChecking))
                return StoreChecking;

            try
            {
                var currentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                StoreChecking.CheckInAt = StaticParams.DateTimeNow;
                StoreChecking.SaleEmployeeId = CurrentContext.UserId;
                StoreChecking.OrganizationId = currentUser.OrganizationId;

                Dictionary<long, long> StorePlannedIds = await ListOnlineStoreIds(null);
                if (StorePlannedIds.Any(x => x.Key == StoreChecking.StoreId))
                {
                    StoreChecking.Planned = true;
                }
                else
                {
                    StoreChecking.Planned = false;
                }

                await UOW.Begin();
                await UOW.StoreCheckingRepository.Create(StoreChecking);
                await UOW.Commit();
                NotifyUsed(StoreChecking);
                await Logging.CreateAuditLog(StoreChecking, new { }, nameof(StoreCheckingService));
                return await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));

                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));

                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreChecking> Update(StoreChecking StoreChecking)
        {
            if (!await StoreCheckingValidator.Update(StoreChecking))
                return StoreChecking;
            try
            {
                var oldData = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
                StoreChecking.CheckOutAt = oldData.CheckOutAt;
                StoreChecking.ImageCounter = StoreChecking.StoreCheckingImageMappings?.Count() ?? 0;
                await UOW.Begin();
                await UOW.StoreCheckingRepository.Update(StoreChecking);
                await UOW.Commit();

                StoreChecking = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
                NotifyUsed(StoreChecking);
                await Logging.CreateAuditLog(StoreChecking, oldData, nameof(StoreCheckingService));
                return StoreChecking;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));

                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));

                    throw new MessageException(ex.InnerException);
                }
            }
        }


        public async Task<StoreChecking> CheckOut(StoreChecking StoreChecking)
        {
            if (!await StoreCheckingValidator.CheckOut(StoreChecking))
                return StoreChecking;
            try
            {
                var oldData = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
                StoreChecking.CheckInAt = oldData.CheckInAt;
                StoreChecking.CheckOutAt = StaticParams.DateTimeNow;
                StoreChecking.ImageCounter = StoreChecking.StoreCheckingImageMappings?.Count() ?? 0;
                await UOW.Begin();
                await UOW.StoreCheckingRepository.Update(StoreChecking);
                await UOW.Commit();

                StoreChecking = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
                NotifyUsed(StoreChecking);

                await Logging.CreateAuditLog(StoreChecking, oldData, nameof(StoreCheckingService));
                return StoreChecking;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));

                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));

                    throw new MessageException(ex.InnerException);
                }
            }
        }

        /// <summary>
        /// Danh sách store chung
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<long> CountStore(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            StoreFilter.OrganizationId = new IdFilter { Equal = AppUser.OrganizationId };
            StoreFilter.TimeZone = CurrentContext.TimeZone;
            int count = await UOW.StoreRepository.Count(StoreFilter);
            return count;
        }

        /// <summary>
        /// Danh sách store chung
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<List<Store>> ListStore(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            List<Store> Stores;
            // Lấy danh sách tất cả các cửa hàng ra
            // Tính khoảng cách
            // sắp xếp theo khoảng cách
            int skip = StoreFilter.Skip;
            int take = StoreFilter.Take;
            StoreFilter.Skip = 0;
            StoreFilter.Take = int.MaxValue;
            StoreFilter.Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name |
                StoreSelect.Address | StoreSelect.Telephone | StoreSelect.Latitude |
                StoreSelect.Longitude | StoreSelect.HasChecking | StoreSelect.OwnerPhone | StoreSelect.StoreType;
            var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            StoreFilter.OrganizationId = new IdFilter { Equal = AppUser.OrganizationId };
            StoreFilter.TimeZone = CurrentContext.TimeZone;
            Stores = await UOW.StoreRepository.List(StoreFilter);
            if (CurrentContext.Latitude.HasValue && CurrentContext.Longitude.HasValue)
            {
                Stores = await ListRecentStore(Stores, CurrentContext.Latitude.Value, CurrentContext.Longitude.Value);
            }
            Stores = Stores.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Distance).Skip(skip).Take(take).ToList();
            Stores = await CheckStoreChecking(Stores);
            return Stores;
        }

        /// <summary>
        /// Danh sách store theo tuyến trong ngày
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<long> CountStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                Dictionary<long, long> StoreIds = await ListOnlineStoreIds(ERouteId);
                StoreFilter.Id = new IdFilter { In = StoreIds.Select(x => x.Key).ToList() };
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                StoreFilter.TimeZone = CurrentContext.TimeZone;
                int count = await UOW.StoreRepository.Count(StoreFilter);
                return count;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        /// <summary>
        /// Danh sách store theo tuyến trong ngày
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<List<Store>> ListStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                List<Store> Stores;
                int skip = StoreFilter.Skip;
                int take = StoreFilter.Take;
                // Lấy danh sách tất cả các cửa hàng trong tuyến ra
                // Tính khoảng cách
                // sắp xếp theo thứ tự ưu tiên trước rồi đến khoảng cách
                Dictionary<long, long> StoreIds = await ListOnlineStoreIds(ERouteId);
                StoreFilter.Id = new IdFilter { In = StoreIds.Select(x => x.Key).ToList() };
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                StoreFilter.Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name |
                StoreSelect.Address | StoreSelect.Telephone | StoreSelect.Latitude |
                StoreSelect.Longitude | StoreSelect.HasChecking | StoreSelect.OwnerPhone | StoreSelect.StoreType;
                StoreFilter.TimeZone = CurrentContext.TimeZone;
                Stores = await UOW.StoreRepository.List(StoreFilter);
                if (CurrentContext.Latitude.HasValue && CurrentContext.Longitude.HasValue)
                {
                    Stores = await ListRecentStore(Stores, CurrentContext.Latitude.Value, CurrentContext.Longitude.Value);
                }
                Stores = (from s in Stores
                          join id in StoreIds on s.Id equals id.Key
                          orderby id.Value, s.Distance
                          select s)
                           .Skip(skip).Take(take).ToList();

                Stores = await CheckStoreChecking(Stores);
                return Stores;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        /// <summary>
        /// Danh sách store theo tuyến nhưng không trong ngày
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<long> CountStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                Dictionary<long, long> StoreIds = await ListOfflineStoreIds(ERouteId);
                StoreFilter.Id.In = StoreIds.Select(x => x.Key).ToList();
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                StoreFilter.TimeZone = CurrentContext.TimeZone;
                int count = await UOW.StoreRepository.Count(StoreFilter);
                return count;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        /// <summary>
        /// Danh sách store theo tuyến nhưng không trong ngày
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<List<Store>> ListStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                List<Store> Stores;

                // Lấy danh sách tất cả các cửa hàng ngoại tuyến ra
                // Tính khoảng cách
                // sắp xếp theo thứ tự ưu tiên trước rồi đến khoảng cách
                int skip = StoreFilter.Skip;
                int take = StoreFilter.Take;
                Dictionary<long, long> StoreIds = await ListOfflineStoreIds(ERouteId);
                StoreFilter.Id.In = StoreIds.Select(x => x.Key).ToList();
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                StoreFilter.Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name |
                StoreSelect.Address | StoreSelect.Telephone | StoreSelect.Latitude |
                StoreSelect.Longitude | StoreSelect.HasChecking | StoreSelect.OwnerPhone | StoreSelect.StoreType;
                StoreFilter.TimeZone = CurrentContext.TimeZone;
                Stores = await UOW.StoreRepository.List(StoreFilter);
                if (CurrentContext.Latitude.HasValue && CurrentContext.Longitude.HasValue)
                {
                    Stores = await ListRecentStore(Stores, CurrentContext.Latitude.Value, CurrentContext.Longitude.Value);
                }
                Stores = (from s in Stores
                          join id in StoreIds on s.Id equals id.Key
                          orderby id.Value, s.Distance
                          select s)
                          .Skip(skip).Take(take).ToList();
                Stores = await CheckStoreChecking(Stores);
                return Stores;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        /// <summary>
        /// Danh sách store theo phạm vi
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<long> CountStoreInScope(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                StoreFilter.TimeZone = CurrentContext.TimeZone;
                var count = await UOW.StoreRepository.CountInScoped(StoreFilter, CurrentContext.UserId);
                return count;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        /// <summary>
        /// Danh sách store theo phạm vi
        /// </summary>
        /// <param name="StoreFilter"></param>
        /// <param name="ERouteId"></param>
        /// <returns></returns>
        public async Task<List<Store>> ListStoreInScope(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<Store> Stores;
                int skip = StoreFilter.Skip;
                int take = StoreFilter.Take;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                StoreFilter.Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name |
                StoreSelect.Address | StoreSelect.Telephone | StoreSelect.Latitude |
                StoreSelect.Longitude | StoreSelect.HasChecking | StoreSelect.OwnerPhone | StoreSelect.StoreType;
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                StoreFilter.TimeZone = CurrentContext.TimeZone;
                Stores = await UOW.StoreRepository.ListInScoped(StoreFilter, CurrentContext.UserId);
                if (CurrentContext.Latitude.HasValue && CurrentContext.Longitude.HasValue)
                {
                    Stores = await ListRecentStore(Stores, CurrentContext.Latitude.Value, CurrentContext.Longitude.Value);
                }
                Stores = Stores.OrderBy(s => s.Distance).Skip(skip).Take(take).ToList();
                Stores = await CheckStoreChecking(Stores);
                return Stores;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreCheckingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        private async Task<List<Store>> CheckStoreChecking(List<Store> Stores)
        {
            List<long> StoreIds = Stores.Select(x => x.Id).ToList();
            DateTime StartToday = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime EndToday = StartToday.AddDays(1);
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.ALL,
                StoreId = new IdFilter { In = StoreIds },
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                CheckOutAt = new DateFilter { GreaterEqual = StartToday, Less = EndToday }
            };
            List<StoreChecking> StoreCheckings = await UOW.StoreCheckingRepository.List(StoreCheckingFilter);
            foreach (var Store in Stores)
            {
                var count = StoreCheckings.Where(x => x.StoreId == Store.Id).Count();
                Store.HasChecking = count != 0 ? true : false;
            }
            return Stores;
        }

        public StoreCheckingFilter ToFilter(StoreCheckingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreCheckingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreCheckingFilter subFilter = new StoreCheckingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/store-checking/images/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
        }

        // Lấy danh sách tất cả các đại lý theo kế hoạch
        private async Task<Dictionary<long, long>> ListOnlineStoreIds(IdFilter ERouteId)
        {
            DateTime Now = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            List<long> ERouteIds = (await UOW.ERouteRepository.List(new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                StartDate = new DateFilter { LessEqual = Now },
                EndDate = new DateFilter { GreaterEqual = Now },
                Id = ERouteId,
                AppUserId = new IdFilter { Equal = CurrentContext.UserId },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                Selects = ERouteSelect.Id
            })).Select(x => x.Id).ToList();

            List<ERouteContent> ERouteContents = await UOW.ERouteContentRepository.List(new ERouteContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                ERouteId = new IdFilter { In = ERouteIds },
                Selects = ERouteContentSelect.Id | ERouteContentSelect.Store | ERouteContentSelect.ERoute
            });

            Dictionary<long, long> StoreIds = new Dictionary<long, long>();
            foreach (var ERouteContent in ERouteContents)
            {
                var index = (Now - ERouteContent.ERoute.RealStartDate).Days % 28;
                if (ERouteContent.ERouteContentDays[index].Planned == true)
                {
                    long StoreId = StoreIds.Where(x => x.Key == ERouteContent.StoreId)
                        .Select(x => x.Key)
                        .FirstOrDefault();
                    if (StoreId == 0)
                    {
                        long value = ERouteContent.OrderNumber ?? int.MaxValue;
                        StoreIds.Add(ERouteContent.StoreId, value);
                    }
                    else
                    {
                        long value = ERouteContent.OrderNumber ?? int.MaxValue;
                        long oldValue = StoreIds[StoreId];
                        if (oldValue > value)
                            StoreIds[StoreId] = value;
                    }
                }
            }

            return StoreIds;
        }

        private async Task<Dictionary<long, long>> ListOfflineStoreIds(IdFilter ERouteId)
        {
            DateTime Now = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            List<long> ERouteIds = (await UOW.ERouteRepository.List(new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                StartDate = new DateFilter { LessEqual = Now },
                EndDate = new DateFilter { GreaterEqual = Now },
                Id = ERouteId,
                AppUserId = new IdFilter { Equal = CurrentContext.UserId },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                Selects = ERouteSelect.Id
            })).Select(x => x.Id).ToList();

            List<ERouteContent> ERouteContents = await UOW.ERouteContentRepository.List(new ERouteContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                ERouteId = new IdFilter { In = ERouteIds },
                Selects = ERouteContentSelect.Id | ERouteContentSelect.Store | ERouteContentSelect.ERoute
            });

            Dictionary<long, long> StoreIds = new Dictionary<long, long>();
            foreach (var ERouteContent in ERouteContents)
            {
                var index = (Now - ERouteContent.ERoute.RealStartDate).Days % 28;
                if (ERouteContent.ERouteContentDays[index].Planned == false)
                {
                    long StoreId = StoreIds.Where(x => x.Key == ERouteContent.StoreId)
                        .Select(x => x.Key)
                        .FirstOrDefault();
                    if (StoreId == 0)
                    {
                        long value = ERouteContent.OrderNumber ?? int.MaxValue;
                        StoreIds.Add(ERouteContent.StoreId, value);
                    }
                    else
                    {
                        long value = ERouteContent.OrderNumber ?? int.MaxValue;
                        long oldValue = StoreIds[StoreId];
                        if (oldValue > value)
                            StoreIds[StoreId] = value;
                    }
                }
            }

            return StoreIds;
        }

        private async Task<List<Store>> ListRecentStore(List<Store> Stores, decimal CurrentLatitude, decimal CurrentLongitude)
        {
            GeoCoordinate sCoord = new GeoCoordinate((double)CurrentLatitude, (double)CurrentLongitude);
            foreach (Store Store in Stores)
            {
                GeoCoordinate eCoord = new GeoCoordinate((double)Store.Latitude, (double)Store.Longitude);
                Store.Distance = sCoord.GetDistanceTo(eCoord);
            }
            return Stores;
        }

        private void NotifyUsed(StoreChecking StoreChecking)
        {
            {
                EventMessage<Store> StoreMessage = new EventMessage<Store>
                {
                    Content = new Store { Id = StoreChecking.StoreId },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(StoreMessage, RoutingKeyEnum.StoreUsed);
            }
            {
                List<long> AlbumIds = new List<long>();
                if (StoreChecking.StoreCheckingImageMappings != null)
                {
                    foreach (StoreCheckingImageMapping StoreCheckingImageMapping in StoreChecking.StoreCheckingImageMappings)
                    {
                        AlbumIds.Add(StoreCheckingImageMapping.AlbumId);
                    }
                }
                List<EventMessage<Album>> messages = AlbumIds.Select(a => new EventMessage<Album>
                {
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                    EntityName = nameof(Album),
                    Content = new Album { Id = a },
                }).ToList();
                // phai check quan he lien ket 1:1, hay 1:n hay n:m de publishSingle, hay publishList
                RabbitManager.PublishList(messages, RoutingKeyEnum.AlbumUsed);
            }
        }
    }
}
