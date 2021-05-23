using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Services.MSystemConfiguration;
using DMS.ABE.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DMS.ABE.Rpc.system_configuration
{
    public class SystemConfigurationController : SimpleController
    {
        private ISystemConfigurationService SystemConfigurationService;
        public SystemConfigurationController(
            ISystemConfigurationService SystemConfigurationService,
            ICurrentContext CurrentContext
        )
        {
            this.SystemConfigurationService = SystemConfigurationService;
        }
        [AllowAnonymous]
        [Route(SystemConfigurationRoute.Get), HttpPost]
        public async Task<ActionResult<SystemConfiguration_SystemConfigurationDTO>> Get()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SystemConfiguration SystemConfiguration = await SystemConfigurationService.Get();
            return new SystemConfiguration_SystemConfigurationDTO(SystemConfiguration);
        }
    }
}

