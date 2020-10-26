using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MResellerStatus
{
    public interface IResellerStatusService : IServiceScoped
    {
        Task<int> Count(ResellerStatusFilter ResellerStatusFilter);
        Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter);
        Task<ResellerStatus> Get(long Id);
    }

    public class ResellerStatusService : BaseService, IResellerStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IResellerStatusValidator ResellerStatusValidator;

        public ResellerStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IResellerStatusValidator ResellerStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ResellerStatusValidator = ResellerStatusValidator;
        }
        public async Task<int> Count(ResellerStatusFilter ResellerStatusFilter)
        {
            try
            {
                int result = await UOW.ResellerStatusRepository.Count(ResellerStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter)
        {
            try
            {
                List<ResellerStatus> ResellerStatuss = await UOW.ResellerStatusRepository.List(ResellerStatusFilter);
                return ResellerStatuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ResellerStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<ResellerStatus> Get(long Id)
        {
            ResellerStatus ResellerStatus = await UOW.ResellerStatusRepository.Get(Id);
            if (ResellerStatus == null)
                return null;
            return ResellerStatus;
        }
    }
}
