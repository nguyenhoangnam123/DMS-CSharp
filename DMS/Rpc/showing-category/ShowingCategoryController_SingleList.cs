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
using DMS.Services.MShowingCategory;
using DMS.Services.MImage;
using DMS.Services.MCategory;
using DMS.Services.MStatus;

namespace DMS.Rpc.showing_category
{
    public partial class ShowingCategoryController : RpcController
    {
        [Route(ShowingCategoryRoute.SingleListCategory), HttpPost]
        public async Task<List<ShowingCategory_CategoryDTO>> SingleListCategory([FromBody] ShowingCategory_CategoryFilterDTO ShowingCategory_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = ShowingCategory_CategoryFilterDTO.Id;
            CategoryFilter.Code = ShowingCategory_CategoryFilterDTO.Code;
            CategoryFilter.Name = ShowingCategory_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = ShowingCategory_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = ShowingCategory_CategoryFilterDTO.Path;
            CategoryFilter.Level = ShowingCategory_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = ShowingCategory_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = ShowingCategory_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = ShowingCategory_CategoryFilterDTO.RowId;
            CategoryFilter.StatusId = new IdFilter{ Equal = 1 };
            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<ShowingCategory_CategoryDTO> ShowingCategory_CategoryDTOs = Categories
                .Select(x => new ShowingCategory_CategoryDTO(x)).ToList();
            return ShowingCategory_CategoryDTOs;
        }
        [Route(ShowingCategoryRoute.SingleListStatus), HttpPost]
        public async Task<List<ShowingCategory_StatusDTO>> SingleListStatus([FromBody] ShowingCategory_StatusFilterDTO ShowingCategory_StatusFilterDTO)
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
            List<ShowingCategory_StatusDTO> ShowingCategory_StatusDTOs = Statuses
                .Select(x => new ShowingCategory_StatusDTO(x)).ToList();
            return ShowingCategory_StatusDTOs;
        }
    }
}

