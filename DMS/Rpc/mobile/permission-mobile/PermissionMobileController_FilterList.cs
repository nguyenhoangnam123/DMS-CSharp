using DMS.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Rpc.mobile.permission_mobile
{
    public partial class PermissionMobileController
    {
        [Route(PermissionMobileRoute.FilterListAppUser), HttpPost]
        public async Task<List<PermissionMobile_AppUserDTO>> FilterListAppUser([FromBody] PermissionMobile_AppUserFilterDTO PermissionMobile_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = PermissionMobile_AppUserFilterDTO.Id;
            AppUserFilter.Username = PermissionMobile_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = PermissionMobile_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = PermissionMobile_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<PermissionMobile_AppUserDTO> PermissionMobile_AppUserDTOs = AppUsers
                .Select(x => new PermissionMobile_AppUserDTO(x)).ToList();
            return PermissionMobile_AppUserDTOs;
        }

        [Route(PermissionMobileRoute.FilterListOrganization), HttpPost]
        public async Task<List<PermissionMobile_OrganizationDTO>> FilterListOrganization()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<PermissionMobile_OrganizationDTO> PermissionMobile_OrganizationDTOs = Organizations
                .Select(x => new PermissionMobile_OrganizationDTO(x)).ToList();
            return PermissionMobile_OrganizationDTOs;
        }
    }
}
