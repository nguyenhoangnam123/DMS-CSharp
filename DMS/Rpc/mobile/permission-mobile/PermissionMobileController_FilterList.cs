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
        [Route(PermissionMobileRoute.CountAppUser), HttpPost]
        public async Task<int> CountAppUser([FromBody] PermissionMobile_AppUserFilterDTO PermissionMobile_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = PermissionMobile_AppUserFilterDTO.Id;
            AppUserFilter.Username = PermissionMobile_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = PermissionMobile_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = PermissionMobile_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(PermissionMobileRoute.ListAppUser), HttpPost]
        public async Task<List<PermissionMobile_AppUserDTO>> ListAppUser([FromBody] PermissionMobile_AppUserFilterDTO PermissionMobile_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = PermissionMobile_AppUserFilterDTO.Skip;
            AppUserFilter.Take = PermissionMobile_AppUserFilterDTO.Take;
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
    }
}
