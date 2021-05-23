using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.Handlers;
using DMS.Enums;

namespace DMS.Services.MStoreType
{
    public interface IStoreTypeService : IServiceScoped
    {
        Task<int> Count(StoreTypeFilter StoreTypeFilter);
        Task<List<StoreType>> List(StoreTypeFilter StoreTypeFilter);
        Task<StoreType> Get(long Id);
        Task<StoreType> Create(StoreType StoreType);
        Task<StoreType> Update(StoreType StoreType);
        Task<StoreType> Delete(StoreType StoreType);
        Task<List<StoreType>> BulkDelete(List<StoreType> StoreTypes);
    }

    public class StoreTypeService : BaseService, IStoreTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreTypeValidator StoreTypeValidator;
        private IRabbitManager RabbitManager;

        public StoreTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreTypeValidator StoreTypeValidator,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreTypeValidator = StoreTypeValidator;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(StoreTypeFilter StoreTypeFilter)
        {
            try
            {
                int result = await UOW.StoreTypeRepository.Count(StoreTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<StoreType>> List(StoreTypeFilter StoreTypeFilter)
        {
            try
            {
                List<StoreType> StoreTypes = await UOW.StoreTypeRepository.List(StoreTypeFilter);
                return StoreTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<StoreType> Get(long Id)
        {
            StoreType StoreType = await UOW.StoreTypeRepository.Get(Id);
            if (StoreType == null)
                return null;
            return StoreType;
        }

        public async Task<StoreType> Create(StoreType StoreType)
        {
            if (!await StoreTypeValidator.Create(StoreType))
                return StoreType;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.Create(StoreType);
                await UOW.Commit();

                var newData = await UOW.StoreTypeRepository.Get(StoreType.Id);
                Sync(new List<StoreType> { newData });

                await Logging.CreateAuditLog(StoreType, new { }, nameof(StoreTypeService));
                return await UOW.StoreTypeRepository.Get(StoreType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreType> Update(StoreType StoreType)
        {
            if (!await StoreTypeValidator.Update(StoreType))
                return StoreType;
            try
            {
                var oldData = await UOW.StoreTypeRepository.Get(StoreType.Id);

                await UOW.Begin();
                await UOW.StoreTypeRepository.Update(StoreType);
                await UOW.Commit();

                var newData = await UOW.StoreTypeRepository.Get(StoreType.Id);
                Sync(new List<StoreType> { newData });

                await Logging.CreateAuditLog(newData, oldData, nameof(StoreTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreType> Delete(StoreType StoreType)
        {
            if (!await StoreTypeValidator.Delete(StoreType))
                return StoreType;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.Delete(StoreType);
                await UOW.Commit();

                var newData = await UOW.StoreTypeRepository.Get(StoreType.Id);
                Sync(new List<StoreType> { newData });

                await Logging.CreateAuditLog(new { }, StoreType, nameof(StoreTypeService));
                return StoreType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<StoreType>> BulkDelete(List<StoreType> StoreTypes)
        {
            if (!await StoreTypeValidator.BulkDelete(StoreTypes))
                return StoreTypes;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.BulkDelete(StoreTypes);
                await UOW.Commit();

                var Ids = StoreTypes.Select(x => x.Id).ToList();
                StoreTypes = await UOW.StoreTypeRepository.List(Ids);
                Sync(StoreTypes);

                await Logging.CreateAuditLog(new { }, StoreTypes, nameof(StoreTypeService));
                return StoreTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        private void Sync(List<StoreType> StoreTypes)
        {
            RabbitManager.PublishList(StoreTypes, RoutingKeyEnum.StoreTypeSync);
        }
    }
}
