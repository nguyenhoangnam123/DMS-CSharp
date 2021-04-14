using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MSystemConfiguration
{
    public interface ISystemConfigurationService : IServiceScoped
    {
        Task<SystemConfiguration> Get();
    }

    public class SystemConfigurationService : BaseService, ISystemConfigurationService
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
        public async Task<SystemConfiguration> Get()
        {
            try
            {
                var result = await UOW.SystemConfigurationRepository.Get();
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
    }
}
