using Common;
using DMS.Entities;
using DMS.Services.MStatus;
using DMS.Services.MSystemConfiguration;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.system_configuration
{
    public class SystemConfigurationController : RpcController
    {
        private ISystemConfigurationService SystemConfigurationService;
        private ICurrentContext CurrentContext;
        public SystemConfigurationController(
            ISystemConfigurationService SystemConfigurationService,
            ICurrentContext CurrentContext
        )
        {
            this.SystemConfigurationService = SystemConfigurationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SystemConfigurationRoute.Get), HttpPost]
        public async Task<ActionResult<SystemConfiguration_SystemConfigurationDTO>> Get()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SystemConfiguration SystemConfiguration = await SystemConfigurationService.Get();
            return new SystemConfiguration_SystemConfigurationDTO(SystemConfiguration);
        }


        [Route(SystemConfigurationRoute.Update), HttpPost]
        public async Task<SystemConfiguration_SystemConfigurationDTO> Update([FromBody] SystemConfiguration_SystemConfigurationDTO SystemConfiguration_SystemConfigurationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SystemConfiguration SystemConfiguration = new SystemConfiguration
            {
                STORE_CHECKING_DISTANCE = SystemConfiguration_SystemConfigurationDTO.STORE_CHECKING_DISTANCE,
                STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE = SystemConfiguration_SystemConfigurationDTO.STORE_CHECKING_OFFLINE_CONSTRAINT_DISTANCE,
                USE_DIRECT_SALES_ORDER = SystemConfiguration_SystemConfigurationDTO.USE_DIRECT_SALES_ORDER,
                USE_INDIRECT_SALES_ORDER = SystemConfiguration_SystemConfigurationDTO.USE_INDIRECT_SALES_ORDER,
                ALLOW_EDIT_KPI_IN_PERIOD = SystemConfiguration_SystemConfigurationDTO.ALLOW_EDIT_KPI_IN_PERIOD,
                PRIORITY_USE_PRICE_LIST = SystemConfiguration_SystemConfigurationDTO.PRIORITY_USE_PRICE_LIST,
                PRIORITY_USE_PROMOTION = SystemConfiguration_SystemConfigurationDTO.PRIORITY_USE_PROMOTION,
                STORE_CHECKING_MINIMUM_TIME = SystemConfiguration_SystemConfigurationDTO.STORE_CHECKING_MINIMUM_TIME,
                DASH_BOARD_REFRESH_TIME = SystemConfiguration_SystemConfigurationDTO.DASH_BOARD_REFRESH_TIME,
                AMPLITUDE_PRICE_IN_DIRECT = SystemConfiguration_SystemConfigurationDTO.AMPLITUDE_PRICE_IN_DIRECT,
                AMPLITUDE_PRICE_IN_INDIRECT = SystemConfiguration_SystemConfigurationDTO.AMPLITUDE_PRICE_IN_INDIRECT,
            };
            await SystemConfigurationService.Update(SystemConfiguration);
            SystemConfiguration = await SystemConfigurationService.Get();
            return new SystemConfiguration_SystemConfigurationDTO(SystemConfiguration);
        }
    }
}

