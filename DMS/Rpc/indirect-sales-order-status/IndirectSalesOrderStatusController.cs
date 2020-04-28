using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MIndirectSalesOrderStatus;

namespace DMS.Rpc.indirect_sales_order_status
{
    public class IndirectSalesOrderStatusRoute : Root
    {
        public const string Master = Module + "/indirect-sales-order-status/indirect-sales-order-status-master";
        public const string Detail = Module + "/indirect-sales-order-status/indirect-sales-order-status-detail";
        private const string Default = Rpc + Module + "/indirect-sales-order-status";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(IndirectSalesOrderStatusFilter.Id), FieldType.ID },
            { nameof(IndirectSalesOrderStatusFilter.Code), FieldType.STRING },
            { nameof(IndirectSalesOrderStatusFilter.Name), FieldType.STRING },
        };
    }

    public class IndirectSalesOrderStatusController : RpcController
    {
        private IIndirectSalesOrderStatusService IndirectSalesOrderStatusService;
        private ICurrentContext CurrentContext;
        public IndirectSalesOrderStatusController(
            IIndirectSalesOrderStatusService IndirectSalesOrderStatusService,
            ICurrentContext CurrentContext
        )
        {
            this.IndirectSalesOrderStatusService = IndirectSalesOrderStatusService;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectSalesOrderStatusRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO);
            int count = await IndirectSalesOrderStatusService.Count(IndirectSalesOrderStatusFilter);
            return count;
        }

        [Route(IndirectSalesOrderStatusRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrderStatus_IndirectSalesOrderStatusDTO>>> List([FromBody] IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO);
            List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await IndirectSalesOrderStatusService.List(IndirectSalesOrderStatusFilter);
            List<IndirectSalesOrderStatus_IndirectSalesOrderStatusDTO> IndirectSalesOrderStatus_IndirectSalesOrderStatusDTOs = IndirectSalesOrderStatuses
                .Select(c => new IndirectSalesOrderStatus_IndirectSalesOrderStatusDTO(c)).ToList();
            return IndirectSalesOrderStatus_IndirectSalesOrderStatusDTOs;
        }


        private IndirectSalesOrderStatusFilter ConvertFilterDTOToFilterEntity(IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO)
        {
            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter();
            IndirectSalesOrderStatusFilter.Selects = IndirectSalesOrderStatusSelect.ALL;
            IndirectSalesOrderStatusFilter.Skip = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.Skip;
            IndirectSalesOrderStatusFilter.Take = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.Take;
            IndirectSalesOrderStatusFilter.OrderBy = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.OrderBy;
            IndirectSalesOrderStatusFilter.OrderType = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.OrderType;

            IndirectSalesOrderStatusFilter.Id = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.Id;
            IndirectSalesOrderStatusFilter.Code = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.Code;
            IndirectSalesOrderStatusFilter.Name = IndirectSalesOrderStatus_IndirectSalesOrderStatusFilterDTO.Name;
            return IndirectSalesOrderStatusFilter;
        }


    }
}

