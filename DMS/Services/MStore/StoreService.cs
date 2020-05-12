using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MImage;
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
        private IMailService MailService;
        private IStoreValidator StoreValidator;
        private IWorkflowService WorkflowService;
        private IImageService ImageService;
        public StoreService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IMailService MailService,
            IWorkflowService WorkflowService,
            IStoreValidator StoreValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.MailService = MailService;
            this.WorkflowService = WorkflowService;
            this.StoreValidator = StoreValidator;
            this.ImageService = ImageService;
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
                Store.Id = 0;
                await UOW.Begin();
                await UOW.StoreRepository.Create(Store);
                await WorkflowService.Initialize(Store.RowId, WorkflowTypeEnum.STORE.Id, MapParameters(Store));
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
    }
}
