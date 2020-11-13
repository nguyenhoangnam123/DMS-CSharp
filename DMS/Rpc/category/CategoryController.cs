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
        private IImageService ImageService;
        private IStatusService StatusService;
        private ICategoryService CategoryService;
        private ICurrentContext CurrentContext;
        public CategoryController(
            IImageService ImageService,
            IStatusService StatusService,
            ICategoryService CategoryService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.StatusService = StatusService;
            this.CategoryService = CategoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CategoryRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO);
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            int count = await CategoryService.Count(CategoryFilter);
            return count;
        }

        [Route(CategoryRoute.List), HttpPost]
        public async Task<ActionResult<List<Category_CategoryDTO>>> List([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO);
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Category_CategoryDTO> Category_CategoryDTOs = Categories
                .Select(c => new Category_CategoryDTO(c)).ToList();
            return Category_CategoryDTOs;
        }

        [Route(CategoryRoute.Get), HttpPost]
        public async Task<ActionResult<Category_CategoryDTO>> Get([FromBody]Category_CategoryDTO Category_CategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Category_CategoryDTO.Id))
                return Forbid();

            Category Category = await CategoryService.Get(Category_CategoryDTO.Id);
            return new Category_CategoryDTO(Category);
        }

        private async Task<bool> HasPermission(long Id)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            if (Id == 0)
            {

            }
            else
            {
                CategoryFilter.Id = new IdFilter { Equal = Id };
                int count = await CategoryService.Count(CategoryFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Category ConvertDTOToEntity(Category_CategoryDTO Category_CategoryDTO)
        {
            Category Category = new Category();
            Category.Id = Category_CategoryDTO.Id;
            Category.Code = Category_CategoryDTO.Code;
            Category.Name = Category_CategoryDTO.Name;
            Category.ParentId = Category_CategoryDTO.ParentId;
            Category.Path = Category_CategoryDTO.Path;
            Category.Level = Category_CategoryDTO.Level;
            Category.StatusId = Category_CategoryDTO.StatusId;
            Category.ImageId = Category_CategoryDTO.ImageId;
            Category.RowId = Category_CategoryDTO.RowId;
            Category.Used = Category_CategoryDTO.Used;
            Category.Image = Category_CategoryDTO.Image == null ? null : new Image
            {
                Id = Category_CategoryDTO.Image.Id,
                Name = Category_CategoryDTO.Image.Name,
                Url = Category_CategoryDTO.Image.Url,
                ThumbnailUrl = Category_CategoryDTO.Image.ThumbnailUrl,
            };
            Category.Parent = Category_CategoryDTO.Parent == null ? null : new Category
            {
                Id = Category_CategoryDTO.Parent.Id,
                Code = Category_CategoryDTO.Parent.Code,
                Name = Category_CategoryDTO.Parent.Name,
                ParentId = Category_CategoryDTO.Parent.ParentId,
                Path = Category_CategoryDTO.Parent.Path,
                Level = Category_CategoryDTO.Parent.Level,
                StatusId = Category_CategoryDTO.Parent.StatusId,
                ImageId = Category_CategoryDTO.Parent.ImageId,
                RowId = Category_CategoryDTO.Parent.RowId,
                Used = Category_CategoryDTO.Parent.Used,
            };
            Category.Status = Category_CategoryDTO.Status == null ? null : new Status
            {
                Id = Category_CategoryDTO.Status.Id,
                Code = Category_CategoryDTO.Status.Code,
                Name = Category_CategoryDTO.Status.Name,
            };
            Category.BaseLanguage = CurrentContext.Language;
            return Category;
        }

        private CategoryFilter ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = 99999;
            CategoryFilter.OrderBy = Category_CategoryFilterDTO.OrderBy;
            CategoryFilter.OrderType = Category_CategoryFilterDTO.OrderType;

            CategoryFilter.Id = Category_CategoryFilterDTO.Id;
            CategoryFilter.Code = Category_CategoryFilterDTO.Code;
            CategoryFilter.Name = Category_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Category_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Category_CategoryFilterDTO.Path;
            CategoryFilter.Level = Category_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Category_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Category_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Category_CategoryFilterDTO.RowId;
            CategoryFilter.CreatedAt = Category_CategoryFilterDTO.CreatedAt;
            CategoryFilter.UpdatedAt = Category_CategoryFilterDTO.UpdatedAt;
            return CategoryFilter;
        }
    }
}

