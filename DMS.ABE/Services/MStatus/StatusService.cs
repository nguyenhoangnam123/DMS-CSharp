using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MStatus
{
    public interface IStatusService : IServiceScoped
    {
        Task<int> Count(StatusFilter StatusFilter);
        Task<List<Status>> List(StatusFilter StatusFilter);
    }

    public class SystemConfigurationService : BaseService, IStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public SystemConfigurationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(StatusFilter StatusFilter)
        {
            try
            {
                int result = await UOW.StatusRepository.Count(StatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SystemConfigurationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SystemConfigurationService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Status>> List(StatusFilter StatusFilter)
        {
            try
            {
                List<Status> Statuss = await UOW.StatusRepository.List(StatusFilter);
                return Statuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(SystemConfigurationService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(SystemConfigurationService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
    }
}
