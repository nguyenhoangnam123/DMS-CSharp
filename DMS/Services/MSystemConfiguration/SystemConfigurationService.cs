using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MSystemConfiguration
{
    public interface ISystemConfigurationService : IServiceScoped
    {
        Task<SystemConfiguration> Get();
        Task<bool> Update(SystemConfiguration SystemConfiguration);
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

        public async Task<bool> Update(SystemConfiguration SystemConfiguration)
        {
            try
            {
                return await UOW.SystemConfigurationRepository.Update(SystemConfiguration);
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
