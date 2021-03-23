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
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MWard;
using DMS.Services.MShowingInventory;
using DMS.Services.MAppUser;
using DMS.Services.MShowingItem;

namespace DMS.Rpc.showing_warehouse
{
    public partial class ShowingWarehouseController : RpcController
    {
        [Route(ShowingWarehouseRoute.CountHistory), HttpPost]
        public async Task<int> CountHistory([FromBody] ShowingWarehouse_ShowingInventoryHistoryFilterDTO ShowingWarehouse_ShowingInventoryHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter = new ShowingInventoryHistoryFilter
            {
                Id = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.Id,
                ShowingInventoryId = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.ShowingInventoryId,
                OldSaleStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OldSaleStock,
                OldAccountingStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OldAccountingStock,
                SaleStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.SaleStock,
                AccountingStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.AccountingStock,
                AppUserId = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.AppUserId,
                UpdatedAt = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.UpdatedAt,

                Selects = ShowingInventoryHistorySelect.ALL,
                Skip = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.Skip,
                Take = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.Take,
                OrderBy = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OrderBy,
                OrderType = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OrderType,
            };
            ShowingInventoryHistoryFilter = ShowingInventoryHistoryService.ToFilter(ShowingInventoryHistoryFilter);
            int count = await ShowingInventoryHistoryService.Count(ShowingInventoryHistoryFilter);
            return count;
        }

        [Route(ShowingWarehouseRoute.ListHistory), HttpPost]
        public async Task<List<ShowingWarehouse_ShowingInventoryHistoryDTO>> ListHistory([FromBody] ShowingWarehouse_ShowingInventoryHistoryFilterDTO ShowingWarehouse_ShowingInventoryHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter = new ShowingInventoryHistoryFilter
            {
                Id = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.Id,
                ShowingInventoryId = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.ShowingInventoryId,
                OldSaleStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OldSaleStock,
                OldAccountingStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OldAccountingStock,
                SaleStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.SaleStock,
                AccountingStock = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.AccountingStock,
                AppUserId = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.AppUserId,
                UpdatedAt = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.UpdatedAt,

                Selects = ShowingInventoryHistorySelect.ALL,
                Skip = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.Skip,
                Take = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.Take,
                OrderBy = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OrderBy,
                OrderType = ShowingWarehouse_ShowingInventoryHistoryFilterDTO.OrderType,
            };

            ShowingInventoryHistoryFilter = ShowingInventoryHistoryService.ToFilter(ShowingInventoryHistoryFilter);
            List<ShowingInventoryHistory> ShowingInventoryHistories = await ShowingInventoryHistoryService.List(ShowingInventoryHistoryFilter);
            List<ShowingWarehouse_ShowingInventoryHistoryDTO> ShowingWarehouse_ShowingInventoryHistoryDTOs = ShowingInventoryHistories
                .Select(c => new ShowingWarehouse_ShowingInventoryHistoryDTO(c)).ToList();
            return ShowingWarehouse_ShowingInventoryHistoryDTOs;
        }
    }
}

