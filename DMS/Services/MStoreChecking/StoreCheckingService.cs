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
        Task<long> CountStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<List<Store>> ListStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<long> CountStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId);
        Task<List<Store>> ListStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                ERoutePerformance ERoutePerformance = new ERoutePerformance
                {
                    SaleEmployeeId = CurrentContext.UserId,
                    Date = StaticParams.DateTimeNow,
                    PlanCounter = await CountStorePlanned(new StoreFilter { }, null)
                };
                await UOW.Begin();
                await UOW.ERoutePerformanceRepository.Save(ERoutePerformance);
                await UOW.StoreCheckingRepository.Create(StoreChecking);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreChecking, new { }, nameof(StoreCheckingService));
                return await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }


        public async Task<StoreChecking> CheckOut(StoreChecking StoreChecking)
        {
            if (!await StoreCheckingValidator.CheckOut(StoreChecking))
                return StoreChecking;
            try
            {
                var oldData = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
                StoreChecking.CheckOutAt = StaticParams.DateTimeNow;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreCheckingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<long> CountStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            List<long> StoreIds = await ListStoreIds(ERouteId, true);
            StoreFilter.Id = new IdFilter { In = StoreIds };
            int count = await UOW.StoreRepository.Count(StoreFilter);
            return count;
        }

        public async Task<List<Store>> ListStorePlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            List<long> StoreIds = await ListStoreIds(ERouteId, true);

            StoreFilter.Id = new IdFilter { In = StoreIds };
            List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
            return Stores;
        }

        public async Task<long> CountStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            List<long> StoreIds = await ListStoreIds(ERouteId, false);

            return StoreIds == null ? 0 : StoreIds.Count();
        }

        public async Task<List<Store>> ListStoreUnPlanned(StoreFilter StoreFilter, IdFilter ERouteId)
        {
            List<long> StoreIds = await ListStoreIds(ERouteId, false);

            StoreFilter.Id = new IdFilter { In = StoreIds };
            List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
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

        private async Task<List<long>> ListStoreIds(IdFilter ERouteId, bool Planned)
        {
            DateTime Now = StaticParams.DateTimeNow;
            List<long> ERouteIds = (await UOW.ERouteRepository.List(new ERouteFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                StartDate = new DateFilter { LessEqual = Now },
                EndDate = new DateFilter { GreaterEqual = Now },
                Id = ERouteId,
                SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
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
                var index = ((DateTime.Now - ERouteContent.ERoute.RealStartDate).Days + 1) % 28;
                if (ERouteContent.ERouteContentDays[index].Planned == Planned)
                    StoreIds.Add(ERouteContent.StoreId);
            }

            return StoreIds;
        }
    }
}
