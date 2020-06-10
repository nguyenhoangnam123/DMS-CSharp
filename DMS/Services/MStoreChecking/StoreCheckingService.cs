using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MImage;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DMS.Services.MStoreChecking
{
    public interface IStoreCheckingService : IServiceScoped
    {
        Task<int> Count(StoreCheckingFilter StoreCheckingFilter);
        Task<List<StoreChecking>> List(StoreCheckingFilter StoreCheckingFilter);
        Task<StoreChecking> Get(long Id);
        Task<StoreChecking> Create(StoreChecking StoreChecking);
        Task<StoreChecking> Update(StoreChecking StoreChecking);
        Task<Image> SaveImage(Image Image);
        StoreCheckingFilter ToFilter(StoreCheckingFilter StoreCheckingFilter);
    }

    public class StoreCheckingService : BaseService, IStoreCheckingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageService ImageService;
        private IStoreCheckingValidator StoreCheckingValidator;

        public StoreCheckingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IStoreCheckingValidator StoreCheckingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
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

        public async Task<StoreChecking> Create(StoreChecking StoreChecking)
        {
            if (!await StoreCheckingValidator.Create(StoreChecking))
                return StoreChecking;

            try
            {
                DateTime Now = StaticParams.DateTimeNow;
                StoreChecking.CheckInAt = StaticParams.DateTimeNow;
                StoreChecking.SaleEmployeeId = CurrentContext.UserId;
                ERouteFilter ERouteFilter = new ERouteFilter
                {
                    SaleEmployeeId = new IdFilter { Equal = CurrentContext.UserId },
                    StartDate = new DateFilter { LessEqual = Now },
                    EndDate = new DateFilter { GreaterEqual = Now },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ERouteSelect.ALL,
                };
                List<ERoute> ERoutes = await UOW.ERouteRepository.List(ERouteFilter);
                //TODO
                StoreChecking.Planned = true;
                await UOW.Begin();
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
                StoreChecking.CheckOutAt = StaticParams.DateTimeNow;
                await UOW.Begin();
                await UOW.StoreCheckingRepository.Update(StoreChecking);
                await UOW.Commit();

                var newData = await UOW.StoreCheckingRepository.Get(StoreChecking.Id);
                //if (StoreChecking.StoreCheckingImageMappings != null)
                //{
                //    foreach
                //}
                
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
    }
}
