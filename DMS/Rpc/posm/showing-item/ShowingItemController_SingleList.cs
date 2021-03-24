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
using DMS.Services.MShowingItem;
using DMS.Services.MCategory;
using DMS.Services.MStatus;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.posm.showing_item
{
    public partial class ShowingItemController : RpcController
    {
        [Route(ShowingItemRoute.SingleListShowingCategory), HttpPost]
        public async Task<List<ShowingItem_ShowingCategoryDTO>> SingleListShowingCategory([FromBody] ShowingItem_ShowingCategoryFilterDTO ShowingItem_ShowingCategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingCategoryFilter ShowingCategoryFilter = new ShowingCategoryFilter();
            ShowingCategoryFilter.Skip = 0;
            ShowingCategoryFilter.Take = int.MaxValue;
            ShowingCategoryFilter.OrderBy = ShowingCategoryOrder.Id;
            ShowingCategoryFilter.OrderType = OrderType.ASC;
            ShowingCategoryFilter.Selects = ShowingCategorySelect.ALL;
            ShowingCategoryFilter.Id = ShowingItem_ShowingCategoryFilterDTO.Id;
            ShowingCategoryFilter.Code = ShowingItem_ShowingCategoryFilterDTO.Code;
            ShowingCategoryFilter.Name = ShowingItem_ShowingCategoryFilterDTO.Name;
            ShowingCategoryFilter.ParentId = ShowingItem_ShowingCategoryFilterDTO.ParentId;
            ShowingCategoryFilter.Path = ShowingItem_ShowingCategoryFilterDTO.Path;
            ShowingCategoryFilter.Level = ShowingItem_ShowingCategoryFilterDTO.Level;
            ShowingCategoryFilter.StatusId = ShowingItem_ShowingCategoryFilterDTO.StatusId;
            ShowingCategoryFilter.ImageId = ShowingItem_ShowingCategoryFilterDTO.ImageId;
            ShowingCategoryFilter.RowId = ShowingItem_ShowingCategoryFilterDTO.RowId;
            ShowingCategoryFilter.StatusId = new IdFilter{ Equal = 1 };
            List<ShowingCategory> Categories = await ShowingCategoryService.List(ShowingCategoryFilter);
            List<ShowingItem_ShowingCategoryDTO> ShowingItem_ShowingCategoryDTOs = Categories
                .Select(x => new ShowingItem_ShowingCategoryDTO(x)).ToList();
            return ShowingItem_ShowingCategoryDTOs;
        }
        [Route(ShowingItemRoute.SingleListStatus), HttpPost]
        public async Task<List<ShowingItem_StatusDTO>> SingleListStatus([FromBody] ShowingItem_StatusFilterDTO ShowingItem_StatusFilterDTO)
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
            List<ShowingItem_StatusDTO> ShowingItem_StatusDTOs = Statuses
                .Select(x => new ShowingItem_StatusDTO(x)).ToList();
            return ShowingItem_StatusDTOs;
        }
        [Route(ShowingItemRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<ShowingItem_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] ShowingItem_UnitOfMeasureFilterDTO ShowingItem_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = ShowingItem_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = ShowingItem_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = ShowingItem_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = ShowingItem_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = ShowingItem_UnitOfMeasureFilterDTO.StatusId;
            UnitOfMeasureFilter.StatusId = new IdFilter{ Equal = 1 };
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<ShowingItem_UnitOfMeasureDTO> ShowingItem_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new ShowingItem_UnitOfMeasureDTO(x)).ToList();
            return ShowingItem_UnitOfMeasureDTOs;
        }
    }
}

