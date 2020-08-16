using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Services.MImage;
using Helpers;
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
                StoreChecking.CheckInAt = StaticParams.DateTimeNow;
                StoreChecking.SaleEmployeeId = CurrentContext.UserId;

                List<long> StorePlannedIds = await ListStoreIds(null, true);
                if (StorePlannedIds.Contains(StoreChecking.StoreId))
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

                var newData = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
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
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreCheckingService));
                return newData;
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

                var newData = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
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
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreCheckingService));
                return newData;
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

        public async Task<long> CountStore(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            List<long> StoreIds = new List<long>();
            if (ERouteId != null && ERouteId.HasValue)
            {
                StoreIds = await ListStoreIds(ERouteId, true);
                if (AppUser.AppUserStoreMappings.Any())
                {
                    var StoreInScopeIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
                    StoreIds = StoreIds.Intersect(StoreInScopeIds).ToList();
                }
                StoreFilter.Id = new IdFilter { In = StoreIds };
            }

            int count = await UOW.StoreRepository.Count(StoreFilter);
            return count;
        }

        public async Task<List<Store>> ListStore(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);

            List<long> StoreIds = new List<long>();
            if (ERouteId != null && ERouteId.HasValue)
            {
                StoreIds = await ListStoreIds(ERouteId, true);
                if (AppUser.AppUserStoreMappings.Any())
                {
                    var StoreInScopeIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
                    StoreIds = StoreIds.Intersect(StoreInScopeIds).ToList();
                }
                StoreFilter.Id = new IdFilter { In = StoreIds };
            }

            List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
            Stores = await CheckStoreChecking(Stores);
            return Stores;
        }

        public async Task<long> CountStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                List<long> StoreIds = await ListStoreIds(ERouteId, true);
                StoreFilter.Id = new IdFilter { In = StoreIds };
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
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

        public async Task<List<Store>> ListStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                // lấy danh sách tất cả các đại lý trong kế hoạch
                List<long> StoreIds = await ListStoreIds(ERouteId, true);
                StoreFilter.Id = new IdFilter { In = StoreIds };
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
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

        public async Task<long> CountStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<long> StoreIds = await ListStoreIds(ERouteId, false);
                StoreFilter.Id.In = StoreIds;
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
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

        public async Task<List<Store>> ListStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                DateTime Now = StaticParams.DateTimeNow.Date;
                var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<long> StoreIds = await ListStoreIds(ERouteId, false);
                StoreFilter.Id.In = StoreIds;
                StoreFilter.SalesEmployeeId = new IdFilter { Equal = CurrentContext.UserId };
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
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

        public async Task<long> CountStoreInScope(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                var count = await UOW.StoreRepository.Count(StoreFilter);
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

        public async Task<List<Store>> ListStoreInScope(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            try
            {
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
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

        private async  Task<List<Store>> CheckStoreChecking(List<Store> Stores)
        {
            List<long> StoreIds = Stores.Select(x => x.Id).ToList();
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreCheckingSelect.ALL,
                StoreId = new IdFilter { In = StoreIds },
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
        private async Task<List<long>> ListStoreIds(IdFilter ERouteId, bool Planned)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
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

            List<long> StoreIds = new List<long>();
            foreach (var ERouteContent in ERouteContents)
            {
                var index = (Now - ERouteContent.ERoute.RealStartDate).Days % 28;
                if (ERouteContent.ERouteContentDays[index].Planned == Planned)
                    StoreIds.Add(ERouteContent.StoreId);
            }

            return StoreIds;
        }
    }
}
