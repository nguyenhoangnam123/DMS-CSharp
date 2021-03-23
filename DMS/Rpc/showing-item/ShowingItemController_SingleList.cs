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

namespace DMS.Rpc.showing_item
{
    public partial class ShowingItemController : RpcController
    {
        [Route(ShowingItemRoute.SingleListCategory), HttpPost]
        public async Task<List<ShowingItem_CategoryDTO>> SingleListCategory([FromBody] ShowingItem_CategoryFilterDTO ShowingItem_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = ShowingItem_CategoryFilterDTO.Id;
            CategoryFilter.Code = ShowingItem_CategoryFilterDTO.Code;
            CategoryFilter.Name = ShowingItem_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = ShowingItem_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = ShowingItem_CategoryFilterDTO.Path;
            CategoryFilter.Level = ShowingItem_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = ShowingItem_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = ShowingItem_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = ShowingItem_CategoryFilterDTO.RowId;
            CategoryFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<ShowingItem_CategoryDTO> ShowingItem_CategoryDTOs = Categories
                .Select(x => new ShowingItem_CategoryDTO(x)).ToList();
            return ShowingItem_CategoryDTOs;
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

