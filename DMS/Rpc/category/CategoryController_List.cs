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
using DMS.Services.MCategory;
using DMS.Services.MImage;
using DMS.Services.MStatus;

namespace DMS.Rpc.category
{
    public partial class CategoryController : RpcController
    {
        [Route(CategoryRoute.FilterListImage), HttpPost]
        public async Task<List<Category_ImageDTO>> FilterListImage([FromBody] Category_ImageFilterDTO Category_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Skip = 0;
            ImageFilter.Take = 20;
            ImageFilter.OrderBy = ImageOrder.Id;
            ImageFilter.OrderType = OrderType.ASC;
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Id = Category_ImageFilterDTO.Id;
            ImageFilter.Name = Category_ImageFilterDTO.Name;
            ImageFilter.Url = Category_ImageFilterDTO.Url;
            ImageFilter.ThumbnailUrl = Category_ImageFilterDTO.ThumbnailUrl;

            List<Image> Images = await ImageService.List(ImageFilter);
            List<Category_ImageDTO> Category_ImageDTOs = Images
                .Select(x => new Category_ImageDTO(x)).ToList();
            return Category_ImageDTOs;
        }
        [Route(CategoryRoute.FilterListCategory), HttpPost]
        public async Task<List<Category_CategoryDTO>> FilterListCategory([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = Category_CategoryFilterDTO.Id;
            CategoryFilter.Code = Category_CategoryFilterDTO.Code;
            CategoryFilter.Name = Category_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Category_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Category_CategoryFilterDTO.Path;
            CategoryFilter.Level = Category_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Category_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Category_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Category_CategoryFilterDTO.RowId;

            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Category_CategoryDTO> Category_CategoryDTOs = Categories
                .Select(x => new Category_CategoryDTO(x)).ToList();
            return Category_CategoryDTOs;
        }
        [Route(CategoryRoute.FilterListStatus), HttpPost]
        public async Task<List<Category_StatusDTO>> FilterListStatus([FromBody] Category_StatusFilterDTO Category_StatusFilterDTO)
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
            List<Category_StatusDTO> Category_StatusDTOs = Statuses
                .Select(x => new Category_StatusDTO(x)).ToList();
            return Category_StatusDTOs;
        }
    }
}

