using DMS.ABE.Common;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.ABE.Repositories;
using DMS.ABE.Entities;
using DMS.ABE.Enums;

namespace DMS.ABE.Services.MStoreStatus
{
    public interface IStoreStatusService :  IServiceScoped
    {
        Task<int> Count(StoreStatusFilter StoreStatusFilter);
        Task<List<StoreStatus>> List(StoreStatusFilter StoreStatusFilter);
    }

    public class StoreStatusService : BaseService, IStoreStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreStatusValidator StoreStatusValidator;

        public StoreStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreStatusValidator StoreStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreStatusValidator = StoreStatusValidator;
        }
        public async Task<int> Count(StoreStatusFilter StoreStatusFilter)
        {
            try
            {
                int result = await UOW.StoreStatusRepository.Count(StoreStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreStatusService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<StoreStatus>> List(StoreStatusFilter StoreStatusFilter)
        {
            try
            {
                List<StoreStatus> StoreStatuss = await UOW.StoreStatusRepository.List(StoreStatusFilter);
                return StoreStatuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreStatusService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
