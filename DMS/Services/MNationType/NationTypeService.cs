using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MNationType
{
    public interface INationTypeService : IServiceScoped
    {
        Task<int> Count(NationTypeFilter NationTypeFilter);
        Task<List<NationType>> List(NationTypeFilter NationTypeFilter);
    }

    public class SystemConfigurationService : BaseService, INationTypeService
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
        public async Task<int> Count(NationTypeFilter NationTypeFilter)
        {
            try
            {
                int result = await UOW.NationTypeRepository.Count(NationTypeFilter);
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

        public async Task<List<NationType>> List(NationTypeFilter NationTypeFilter)
        {
            try
            {
                List<NationType> NationTypes = await UOW.NationTypeRepository.List(NationTypeFilter);
                return NationTypes;
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
