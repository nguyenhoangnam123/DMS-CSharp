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
using DMS.Services.MKpiProductGroupingContent;
using DMS.Services.MKpiProductGrouping;

namespace DMS.Rpc.kpi_product_grouping_content
{
    public partial class KpiProductGroupingContentController : RpcController
    {
        [Route(KpiProductGroupingContentRoute.SingleListKpiProductGrouping), HttpPost]
        public async Task<List<KpiProductGroupingContent_KpiProductGroupingDTO>> SingleListKpiProductGrouping([FromBody] KpiProductGroupingContent_KpiProductGroupingFilterDTO KpiProductGroupingContent_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter.Skip = 0;
            KpiProductGroupingFilter.Take = 20;
            KpiProductGroupingFilter.OrderBy = KpiProductGroupingOrder.Id;
            KpiProductGroupingFilter.OrderType = OrderType.ASC;
            KpiProductGroupingFilter.Selects = KpiProductGroupingSelect.ALL;
            KpiProductGroupingFilter.Id = KpiProductGroupingContent_KpiProductGroupingFilterDTO.Id;
            KpiProductGroupingFilter.OrganizationId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.OrganizationId;
            KpiProductGroupingFilter.KpiYearId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.KpiYearId;
            KpiProductGroupingFilter.KpiPeriodId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.KpiPeriodId;
            KpiProductGroupingFilter.KpiProductGroupingTypeId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.KpiProductGroupingTypeId;
            KpiProductGroupingFilter.StatusId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.StatusId;
            KpiProductGroupingFilter.EmployeeId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.EmployeeId;
            KpiProductGroupingFilter.CreatorId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.CreatorId;
            KpiProductGroupingFilter.RowId = KpiProductGroupingContent_KpiProductGroupingFilterDTO.RowId;
            KpiProductGroupingFilter.StatusId = new IdFilter{ Equal = 1 };
            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);
            List<KpiProductGroupingContent_KpiProductGroupingDTO> KpiProductGroupingContent_KpiProductGroupingDTOs = KpiProductGroupings
                .Select(x => new KpiProductGroupingContent_KpiProductGroupingDTO(x)).ToList();
            return KpiProductGroupingContent_KpiProductGroupingDTOs;
        }
    }
}

