using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MKpiProductGrouping;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiProductGroupingType;
using DMS.Services.MStatus;

namespace DMS.Rpc.kpi_product_grouping
{
    public partial class KpiProductGroupingController : RpcController
    {
        [Route(KpiProductGroupingRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiProductGrouping_AppUserDTO>> FilterListAppUser([FromBody] KpiProductGrouping_AppUserFilterDTO KpiProductGrouping_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiProductGrouping_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiProductGrouping_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiProductGrouping_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiProductGrouping_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiProductGrouping_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiProductGrouping_AppUserFilterDTO.Phone;
            AppUserFilter.SexId = KpiProductGrouping_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = KpiProductGrouping_AppUserFilterDTO.Birthday;
            AppUserFilter.PositionId = KpiProductGrouping_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = KpiProductGrouping_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = KpiProductGrouping_AppUserFilterDTO.OrganizationId;
            AppUserFilter.ProvinceId = KpiProductGrouping_AppUserFilterDTO.ProvinceId;
            AppUserFilter.StatusId = KpiProductGrouping_AppUserFilterDTO.StatusId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiProductGrouping_AppUserDTO> KpiProductGrouping_AppUserDTOs = AppUsers
                .Select(x => new KpiProductGrouping_AppUserDTO(x)).ToList();
            return KpiProductGrouping_AppUserDTOs;
        }
        [Route(KpiProductGroupingRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<KpiProductGrouping_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] KpiProductGrouping_KpiPeriodFilterDTO KpiProductGrouping_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = int.MaxValue;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<KpiProductGrouping_KpiPeriodDTO> KpiProductGrouping_KpiPeriodDTOs = KpiPeriods
                .Select(x => new KpiProductGrouping_KpiPeriodDTO(x)).ToList();
            return KpiProductGrouping_KpiPeriodDTOs;
        }
        [Route(KpiProductGroupingRoute.FilterListKpiProductGroupingType), HttpPost]
        public async Task<List<KpiProductGrouping_KpiProductGroupingTypeDTO>> FilterListKpiProductGroupingType([FromBody] KpiProductGrouping_KpiProductGroupingTypeFilterDTO KpiProductGrouping_KpiProductGroupingTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingTypeFilter KpiProductGroupingTypeFilter = new KpiProductGroupingTypeFilter();
            KpiProductGroupingTypeFilter.Skip = 0;
            KpiProductGroupingTypeFilter.Take = 20;
            KpiProductGroupingTypeFilter.OrderBy = KpiProductGroupingTypeOrder.Id;
            KpiProductGroupingTypeFilter.OrderType = OrderType.ASC;
            KpiProductGroupingTypeFilter.Selects = KpiProductGroupingTypeSelect.ALL;
            KpiProductGroupingTypeFilter.Id = KpiProductGrouping_KpiProductGroupingTypeFilterDTO.Id;
            KpiProductGroupingTypeFilter.Code = KpiProductGrouping_KpiProductGroupingTypeFilterDTO.Code;
            KpiProductGroupingTypeFilter.Name = KpiProductGrouping_KpiProductGroupingTypeFilterDTO.Name;

            List<KpiProductGroupingType> KpiProductGroupingTypes = await KpiProductGroupingTypeService.List(KpiProductGroupingTypeFilter);
            List<KpiProductGrouping_KpiProductGroupingTypeDTO> KpiProductGrouping_KpiProductGroupingTypeDTOs = KpiProductGroupingTypes
                .Select(x => new KpiProductGrouping_KpiProductGroupingTypeDTO(x)).ToList();
            return KpiProductGrouping_KpiProductGroupingTypeDTOs;
        }
        [Route(KpiProductGroupingRoute.FilterListStatus), HttpPost]
        public async Task<List<KpiProductGrouping_StatusDTO>> FilterListStatus([FromBody] KpiProductGrouping_StatusFilterDTO KpiProductGrouping_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiProductGrouping_StatusDTO> KpiProductGrouping_StatusDTOs = Statuses
                .Select(x => new KpiProductGrouping_StatusDTO(x)).ToList();
            return KpiProductGrouping_StatusDTOs;
        }
    }
}

