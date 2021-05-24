using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MStoreType
{
    public interface IStoreTypeService : IServiceScoped
    {
        Task<int> Count(StoreTypeFilter StoreTypeFilter);
        Task<List<StoreType>> List(StoreTypeFilter StoreTypeFilter);
        Task<StoreType> Get(long Id);
    }

    public class StoreTypeService : BaseService, IStoreTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public StoreTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
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

    }
}
