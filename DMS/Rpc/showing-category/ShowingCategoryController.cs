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
using System.Dynamic;
using DMS.Entities;
using DMS.Services.MShowingCategory;
using DMS.Services.MImage;
using DMS.Services.MCategory;
using DMS.Services.MStatus;

namespace DMS.Rpc.showing_category
{
    public partial class ShowingCategoryController : RpcController
    {
        private IImageService ImageService;
        private ICategoryService CategoryService;
        private IStatusService StatusService;
        private IShowingCategoryService ShowingCategoryService;
        private ICurrentContext CurrentContext;
        public ShowingCategoryController(
            IImageService ImageService,
            ICategoryService CategoryService,
            IStatusService StatusService,
            IShowingCategoryService ShowingCategoryService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.CategoryService = CategoryService;
            this.StatusService = StatusService;
            this.ShowingCategoryService = ShowingCategoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ShowingCategoryRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ShowingCategory_ShowingCategoryFilterDTO ShowingCategory_ShowingCategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingCategoryFilter ShowingCategoryFilter = ConvertFilterDTOToFilterEntity(ShowingCategory_ShowingCategoryFilterDTO);
            ShowingCategoryFilter = await ShowingCategoryService.ToFilter(ShowingCategoryFilter);
            int count = await ShowingCategoryService.Count(ShowingCategoryFilter);
            return count;
        }

        [Route(ShowingCategoryRoute.List), HttpPost]
        public async Task<ActionResult<List<ShowingCategory_ShowingCategoryDTO>>> List([FromBody] ShowingCategory_ShowingCategoryFilterDTO ShowingCategory_ShowingCategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingCategoryFilter ShowingCategoryFilter = ConvertFilterDTOToFilterEntity(ShowingCategory_ShowingCategoryFilterDTO);
            ShowingCategoryFilter = await ShowingCategoryService.ToFilter(ShowingCategoryFilter);
            List<ShowingCategory> ShowingCategories = await ShowingCategoryService.List(ShowingCategoryFilter);
            List<ShowingCategory_ShowingCategoryDTO> ShowingCategory_ShowingCategoryDTOs = ShowingCategories
                .Select(c => new ShowingCategory_ShowingCategoryDTO(c)).ToList();
            return ShowingCategory_ShowingCategoryDTOs;
        }

        [Route(ShowingCategoryRoute.Get), HttpPost]
        public async Task<ActionResult<ShowingCategory_ShowingCategoryDTO>> Get([FromBody]ShowingCategory_ShowingCategoryDTO ShowingCategory_ShowingCategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingCategory_ShowingCategoryDTO.Id))
                return Forbid();

            ShowingCategory ShowingCategory = await ShowingCategoryService.Get(ShowingCategory_ShowingCategoryDTO.Id);
            return new ShowingCategory_ShowingCategoryDTO(ShowingCategory);
        }

        [Route(ShowingCategoryRoute.Create), HttpPost]
        public async Task<ActionResult<ShowingCategory_ShowingCategoryDTO>> Create([FromBody] ShowingCategory_ShowingCategoryDTO ShowingCategory_ShowingCategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ShowingCategory_ShowingCategoryDTO.Id))
                return Forbid();

            ShowingCategory ShowingCategory = ConvertDTOToEntity(ShowingCategory_ShowingCategoryDTO);
            ShowingCategory = await ShowingCategoryService.Create(ShowingCategory);
            ShowingCategory_ShowingCategoryDTO = new ShowingCategory_ShowingCategoryDTO(ShowingCategory);
            if (ShowingCategory.IsValidated)
                return ShowingCategory_ShowingCategoryDTO;
            else
                return BadRequest(ShowingCategory_ShowingCategoryDTO);
        }

        [Route(ShowingCategoryRoute.Update), HttpPost]
        public async Task<ActionResult<ShowingCategory_ShowingCategoryDTO>> Update([FromBody] ShowingCategory_ShowingCategoryDTO ShowingCategory_ShowingCategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ShowingCategory_ShowingCategoryDTO.Id))
                return Forbid();

            ShowingCategory ShowingCategory = ConvertDTOToEntity(ShowingCategory_ShowingCategoryDTO);
            ShowingCategory = await ShowingCategoryService.Update(ShowingCategory);
            ShowingCategory_ShowingCategoryDTO = new ShowingCategory_ShowingCategoryDTO(ShowingCategory);
            if (ShowingCategory.IsValidated)
                return ShowingCategory_ShowingCategoryDTO;
            else
                return BadRequest(ShowingCategory_ShowingCategoryDTO);
        }

        [Route(ShowingCategoryRoute.Delete), HttpPost]
        public async Task<ActionResult<ShowingCategory_ShowingCategoryDTO>> Delete([FromBody] ShowingCategory_ShowingCategoryDTO ShowingCategory_ShowingCategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingCategory_ShowingCategoryDTO.Id))
                return Forbid();

            ShowingCategory ShowingCategory = ConvertDTOToEntity(ShowingCategory_ShowingCategoryDTO);
            ShowingCategory = await ShowingCategoryService.Delete(ShowingCategory);
            ShowingCategory_ShowingCategoryDTO = new ShowingCategory_ShowingCategoryDTO(ShowingCategory);
            if (ShowingCategory.IsValidated)
                return ShowingCategory_ShowingCategoryDTO;
            else
                return BadRequest(ShowingCategory_ShowingCategoryDTO);
        }
        
        [Route(ShowingCategoryRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingCategoryFilter ShowingCategoryFilter = new ShowingCategoryFilter();
            ShowingCategoryFilter = await ShowingCategoryService.ToFilter(ShowingCategoryFilter);
            ShowingCategoryFilter.Id = new IdFilter { In = Ids };
            ShowingCategoryFilter.Selects = ShowingCategorySelect.Id;
            ShowingCategoryFilter.Skip = 0;
            ShowingCategoryFilter.Take = int.MaxValue;

            List<ShowingCategory> ShowingCategories = await ShowingCategoryService.List(ShowingCategoryFilter);
            ShowingCategories = await ShowingCategoryService.BulkDelete(ShowingCategories);
            if (ShowingCategories.Any(x => !x.IsValidated))
                return BadRequest(ShowingCategories.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ShowingCategoryRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ImageFilter ImageFilter = new ImageFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ImageSelect.ALL
            };
            List<Image> Images = await ImageService.List(ImageFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ShowingCategory> ShowingCategories = new List<ShowingCategory>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ShowingCategories);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentIdColumn = 3 + StartColumn;
                int PathColumn = 4 + StartColumn;
                int LevelColumn = 5 + StartColumn;
                int StatusIdColumn = 6 + StartColumn;
                int ImageIdColumn = 7 + StartColumn;
                int RowIdColumn = 11 + StartColumn;
                int UsedColumn = 12 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ParentIdValue = worksheet.Cells[i + StartRow, ParentIdColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string LevelValue = worksheet.Cells[i + StartRow, LevelColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string ImageIdValue = worksheet.Cells[i + StartRow, ImageIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    ShowingCategory ShowingCategory = new ShowingCategory();
                    ShowingCategory.Code = CodeValue;
                    ShowingCategory.Name = NameValue;
                    ShowingCategory.Path = PathValue;
                    ShowingCategory.Level = long.TryParse(LevelValue, out long Level) ? Level : 0;
                    Image Image = Images.Where(x => x.Id.ToString() == ImageIdValue).FirstOrDefault();
                    ShowingCategory.ImageId = Image == null ? 0 : Image.Id;
                    ShowingCategory.Image = Image;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    ShowingCategory.StatusId = Status == null ? 0 : Status.Id;
                    ShowingCategory.Status = Status;
                    
                    ShowingCategories.Add(ShowingCategory);
                }
            }
            ShowingCategories = await ShowingCategoryService.Import(ShowingCategories);
            if (ShowingCategories.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ShowingCategories.Count; i++)
                {
                    ShowingCategory ShowingCategory = ShowingCategories[i];
                    if (!ShowingCategory.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.Id)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.Id)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.Code)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.Code)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.Name)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.Name)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.ParentId)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.ParentId)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.Path)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.Path)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.Level)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.Level)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.StatusId)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.StatusId)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.ImageId)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.ImageId)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.RowId)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.RowId)];
                        if (ShowingCategory.Errors.ContainsKey(nameof(ShowingCategory.Used)))
                            Error += ShowingCategory.Errors[nameof(ShowingCategory.Used)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ShowingCategoryRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ShowingCategory_ShowingCategoryFilterDTO ShowingCategory_ShowingCategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ShowingCategory
                var ShowingCategoryFilter = ConvertFilterDTOToFilterEntity(ShowingCategory_ShowingCategoryFilterDTO);
                ShowingCategoryFilter.Skip = 0;
                ShowingCategoryFilter.Take = int.MaxValue;
                ShowingCategoryFilter = await ShowingCategoryService.ToFilter(ShowingCategoryFilter);
                List<ShowingCategory> ShowingCategories = await ShowingCategoryService.List(ShowingCategoryFilter);

                var ShowingCategoryHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "ImageId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> ShowingCategoryData = new List<object[]>();
                for (int i = 0; i < ShowingCategories.Count; i++)
                {
                    var ShowingCategory = ShowingCategories[i];
                    ShowingCategoryData.Add(new Object[]
                    {
                        ShowingCategory.Id,
                        ShowingCategory.Code,
                        ShowingCategory.Name,
                        ShowingCategory.ParentId,
                        ShowingCategory.Path,
                        ShowingCategory.Level,
                        ShowingCategory.StatusId,
                        ShowingCategory.ImageId,
                        ShowingCategory.RowId,
                        ShowingCategory.Used,
                    });
                }
                excel.GenerateWorksheet("ShowingCategory", ShowingCategoryHeaders, ShowingCategoryData);
                #endregion
                
                #region Image
                var ImageFilter = new ImageFilter();
                ImageFilter.Selects = ImageSelect.ALL;
                ImageFilter.OrderBy = ImageOrder.Id;
                ImageFilter.OrderType = OrderType.ASC;
                ImageFilter.Skip = 0;
                ImageFilter.Take = int.MaxValue;
                List<Image> Images = await ImageService.List(ImageFilter);

                var ImageHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "Url",
                        "ThumbnailUrl",
                        "RowId",
                    }
                };
                List<object[]> ImageData = new List<object[]>();
                for (int i = 0; i < Images.Count; i++)
                {
                    var Image = Images[i];
                    ImageData.Add(new Object[]
                    {
                        Image.Id,
                        Image.Name,
                        Image.Url,
                        Image.ThumbnailUrl,
                        Image.RowId,
                    });
                }
                excel.GenerateWorksheet("Image", ImageHeaders, ImageData);
                #endregion
                #region Category
                var CategoryFilter = new CategoryFilter();
                CategoryFilter.Selects = CategorySelect.ALL;
                CategoryFilter.OrderBy = CategoryOrder.Id;
                CategoryFilter.OrderType = OrderType.ASC;
                CategoryFilter.Skip = 0;
                CategoryFilter.Take = int.MaxValue;
                List<Category> Categories = await CategoryService.List(CategoryFilter);

                var CategoryHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "ImageId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> CategoryData = new List<object[]>();
                for (int i = 0; i < Categories.Count; i++)
                {
                    var Category = Categories[i];
                    CategoryData.Add(new Object[]
                    {
                        Category.Id,
                        Category.Code,
                        Category.Name,
                        Category.ParentId,
                        Category.Path,
                        Category.Level,
                        Category.StatusId,
                        Category.ImageId,
                        Category.RowId,
                        Category.Used,
                    });
                }
                excel.GenerateWorksheet("Category", CategoryHeaders, CategoryData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ShowingCategory.xlsx");
        }

        [Route(ShowingCategoryRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ShowingCategory_ShowingCategoryFilterDTO ShowingCategory_ShowingCategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/ShowingCategory_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "ShowingCategory.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ShowingCategoryFilter ShowingCategoryFilter = new ShowingCategoryFilter();
            ShowingCategoryFilter = await ShowingCategoryService.ToFilter(ShowingCategoryFilter);
            if (Id == 0)
            {

            }
            else
            {
                ShowingCategoryFilter.Id = new IdFilter { Equal = Id };
                int count = await ShowingCategoryService.Count(ShowingCategoryFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ShowingCategory ConvertDTOToEntity(ShowingCategory_ShowingCategoryDTO ShowingCategory_ShowingCategoryDTO)
        {
            ShowingCategory ShowingCategory = new ShowingCategory();
            ShowingCategory.Id = ShowingCategory_ShowingCategoryDTO.Id;
            ShowingCategory.Code = ShowingCategory_ShowingCategoryDTO.Code;
            ShowingCategory.Name = ShowingCategory_ShowingCategoryDTO.Name;
            ShowingCategory.ParentId = ShowingCategory_ShowingCategoryDTO.ParentId;
            ShowingCategory.Path = ShowingCategory_ShowingCategoryDTO.Path;
            ShowingCategory.Level = ShowingCategory_ShowingCategoryDTO.Level;
            ShowingCategory.StatusId = ShowingCategory_ShowingCategoryDTO.StatusId;
            ShowingCategory.ImageId = ShowingCategory_ShowingCategoryDTO.ImageId;
            ShowingCategory.RowId = ShowingCategory_ShowingCategoryDTO.RowId;
            ShowingCategory.Used = ShowingCategory_ShowingCategoryDTO.Used;
            ShowingCategory.Image = ShowingCategory_ShowingCategoryDTO.Image == null ? null : new Image
            {
                Id = ShowingCategory_ShowingCategoryDTO.Image.Id,
                Name = ShowingCategory_ShowingCategoryDTO.Image.Name,
                Url = ShowingCategory_ShowingCategoryDTO.Image.Url,
                ThumbnailUrl = ShowingCategory_ShowingCategoryDTO.Image.ThumbnailUrl,
                RowId = ShowingCategory_ShowingCategoryDTO.Image.RowId,
            };
            ShowingCategory.Parent = ShowingCategory_ShowingCategoryDTO.Parent == null ? null : new Category
            {
                Id = ShowingCategory_ShowingCategoryDTO.Parent.Id,
                Code = ShowingCategory_ShowingCategoryDTO.Parent.Code,
                Name = ShowingCategory_ShowingCategoryDTO.Parent.Name,
                ParentId = ShowingCategory_ShowingCategoryDTO.Parent.ParentId,
                Path = ShowingCategory_ShowingCategoryDTO.Parent.Path,
                Level = ShowingCategory_ShowingCategoryDTO.Parent.Level,
                StatusId = ShowingCategory_ShowingCategoryDTO.Parent.StatusId,
                ImageId = ShowingCategory_ShowingCategoryDTO.Parent.ImageId,
                RowId = ShowingCategory_ShowingCategoryDTO.Parent.RowId,
                Used = ShowingCategory_ShowingCategoryDTO.Parent.Used,
            };
            ShowingCategory.Status = ShowingCategory_ShowingCategoryDTO.Status == null ? null : new Status
            {
                Id = ShowingCategory_ShowingCategoryDTO.Status.Id,
                Code = ShowingCategory_ShowingCategoryDTO.Status.Code,
                Name = ShowingCategory_ShowingCategoryDTO.Status.Name,
            };
            ShowingCategory.BaseLanguage = CurrentContext.Language;
            return ShowingCategory;
        }

        private ShowingCategoryFilter ConvertFilterDTOToFilterEntity(ShowingCategory_ShowingCategoryFilterDTO ShowingCategory_ShowingCategoryFilterDTO)
        {
            ShowingCategoryFilter ShowingCategoryFilter = new ShowingCategoryFilter();
            ShowingCategoryFilter.Selects = ShowingCategorySelect.ALL;
            ShowingCategoryFilter.Skip = 0;
            ShowingCategoryFilter.Take = 99999;
            ShowingCategoryFilter.OrderBy = ShowingCategory_ShowingCategoryFilterDTO.OrderBy;
            ShowingCategoryFilter.OrderType = ShowingCategory_ShowingCategoryFilterDTO.OrderType;

            ShowingCategoryFilter.Id = ShowingCategory_ShowingCategoryFilterDTO.Id;
            ShowingCategoryFilter.Code = ShowingCategory_ShowingCategoryFilterDTO.Code;
            ShowingCategoryFilter.Name = ShowingCategory_ShowingCategoryFilterDTO.Name;
            ShowingCategoryFilter.ParentId = ShowingCategory_ShowingCategoryFilterDTO.ParentId;
            ShowingCategoryFilter.Path = ShowingCategory_ShowingCategoryFilterDTO.Path;
            ShowingCategoryFilter.Level = ShowingCategory_ShowingCategoryFilterDTO.Level;
            ShowingCategoryFilter.StatusId = ShowingCategory_ShowingCategoryFilterDTO.StatusId;
            ShowingCategoryFilter.ImageId = ShowingCategory_ShowingCategoryFilterDTO.ImageId;
            ShowingCategoryFilter.RowId = ShowingCategory_ShowingCategoryFilterDTO.RowId;
            ShowingCategoryFilter.CreatedAt = ShowingCategory_ShowingCategoryFilterDTO.CreatedAt;
            ShowingCategoryFilter.UpdatedAt = ShowingCategory_ShowingCategoryFilterDTO.UpdatedAt;
            return ShowingCategoryFilter;
        }
    }
}

