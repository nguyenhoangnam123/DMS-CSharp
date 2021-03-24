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
using DMS.Services.MShowingItem;
using DMS.Services.MCategory;
using DMS.Services.MStatus;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.posm.showing_item
{
    public partial class ShowingItemController : RpcController
    {
        private ICategoryService CategoryService;
        private IStatusService StatusService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IShowingItemService ShowingItemService;
        private ICurrentContext CurrentContext;
        public ShowingItemController(
            ICategoryService CategoryService,
            IStatusService StatusService,
            IUnitOfMeasureService UnitOfMeasureService,
            IShowingItemService ShowingItemService,
            ICurrentContext CurrentContext
        )
        {
            this.CategoryService = CategoryService;
            this.StatusService = StatusService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.ShowingItemService = ShowingItemService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ShowingItemRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ShowingItem_ShowingItemFilterDTO ShowingItem_ShowingItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingItemFilter ShowingItemFilter = ConvertFilterDTOToFilterEntity(ShowingItem_ShowingItemFilterDTO);
            ShowingItemFilter = await ShowingItemService.ToFilter(ShowingItemFilter);
            int count = await ShowingItemService.Count(ShowingItemFilter);
            return count;
        }

        [Route(ShowingItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ShowingItem_ShowingItemDTO>>> List([FromBody] ShowingItem_ShowingItemFilterDTO ShowingItem_ShowingItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingItemFilter ShowingItemFilter = ConvertFilterDTOToFilterEntity(ShowingItem_ShowingItemFilterDTO);
            ShowingItemFilter = await ShowingItemService.ToFilter(ShowingItemFilter);
            List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);
            List<ShowingItem_ShowingItemDTO> ShowingItem_ShowingItemDTOs = ShowingItems
                .Select(c => new ShowingItem_ShowingItemDTO(c)).ToList();
            return ShowingItem_ShowingItemDTOs;
        }

        [Route(ShowingItemRoute.Get), HttpPost]
        public async Task<ActionResult<ShowingItem_ShowingItemDTO>> Get([FromBody]ShowingItem_ShowingItemDTO ShowingItem_ShowingItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingItem_ShowingItemDTO.Id))
                return Forbid();

            ShowingItem ShowingItem = await ShowingItemService.Get(ShowingItem_ShowingItemDTO.Id);
            return new ShowingItem_ShowingItemDTO(ShowingItem);
        }

        [Route(ShowingItemRoute.Create), HttpPost]
        public async Task<ActionResult<ShowingItem_ShowingItemDTO>> Create([FromBody] ShowingItem_ShowingItemDTO ShowingItem_ShowingItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ShowingItem_ShowingItemDTO.Id))
                return Forbid();

            ShowingItem ShowingItem = ConvertDTOToEntity(ShowingItem_ShowingItemDTO);
            ShowingItem = await ShowingItemService.Create(ShowingItem);
            ShowingItem_ShowingItemDTO = new ShowingItem_ShowingItemDTO(ShowingItem);
            if (ShowingItem.IsValidated)
                return ShowingItem_ShowingItemDTO;
            else
                return BadRequest(ShowingItem_ShowingItemDTO);
        }

        [Route(ShowingItemRoute.Update), HttpPost]
        public async Task<ActionResult<ShowingItem_ShowingItemDTO>> Update([FromBody] ShowingItem_ShowingItemDTO ShowingItem_ShowingItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ShowingItem_ShowingItemDTO.Id))
                return Forbid();

            ShowingItem ShowingItem = ConvertDTOToEntity(ShowingItem_ShowingItemDTO);
            ShowingItem = await ShowingItemService.Update(ShowingItem);
            ShowingItem_ShowingItemDTO = new ShowingItem_ShowingItemDTO(ShowingItem);
            if (ShowingItem.IsValidated)
                return ShowingItem_ShowingItemDTO;
            else
                return BadRequest(ShowingItem_ShowingItemDTO);
        }

        [Route(ShowingItemRoute.Delete), HttpPost]
        public async Task<ActionResult<ShowingItem_ShowingItemDTO>> Delete([FromBody] ShowingItem_ShowingItemDTO ShowingItem_ShowingItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ShowingItem_ShowingItemDTO.Id))
                return Forbid();

            ShowingItem ShowingItem = ConvertDTOToEntity(ShowingItem_ShowingItemDTO);
            ShowingItem = await ShowingItemService.Delete(ShowingItem);
            ShowingItem_ShowingItemDTO = new ShowingItem_ShowingItemDTO(ShowingItem);
            if (ShowingItem.IsValidated)
                return ShowingItem_ShowingItemDTO;
            else
                return BadRequest(ShowingItem_ShowingItemDTO);
        }
        
        [Route(ShowingItemRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter();
            ShowingItemFilter = await ShowingItemService.ToFilter(ShowingItemFilter);
            ShowingItemFilter.Id = new IdFilter { In = Ids };
            ShowingItemFilter.Selects = ShowingItemSelect.Id;
            ShowingItemFilter.Skip = 0;
            ShowingItemFilter.Take = int.MaxValue;

            List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);
            ShowingItems = await ShowingItemService.BulkDelete(ShowingItems);
            if (ShowingItems.Any(x => !x.IsValidated))
                return BadRequest(ShowingItems.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ShowingItemRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL
            };
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<ShowingItem> ShowingItems = new List<ShowingItem>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ShowingItems);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int CategoryIdColumn = 3 + StartColumn;
                int UnitOfMeasureIdColumn = 4 + StartColumn;
                int SalePriceColumn = 5 + StartColumn;
                int DesceptionColumn = 6 + StartColumn;
                int StatusIdColumn = 7 + StartColumn;
                int UsedColumn = 11 + StartColumn;
                int RowIdColumn = 12 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string CategoryIdValue = worksheet.Cells[i + StartRow, CategoryIdColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string SalePriceValue = worksheet.Cells[i + StartRow, SalePriceColumn].Value?.ToString();
                    string DesceptionValue = worksheet.Cells[i + StartRow, DesceptionColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    
                    ShowingItem ShowingItem = new ShowingItem();
                    ShowingItem.Code = CodeValue;
                    ShowingItem.Name = NameValue;
                    ShowingItem.SalePrice = decimal.TryParse(SalePriceValue, out decimal SalePrice) ? SalePrice : 0;
                    ShowingItem.Desception = DesceptionValue;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    ShowingItem.StatusId = Status == null ? 0 : Status.Id;
                    ShowingItem.Status = Status;
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures.Where(x => x.Id.ToString() == UnitOfMeasureIdValue).FirstOrDefault();
                    ShowingItem.UnitOfMeasureId = UnitOfMeasure == null ? 0 : UnitOfMeasure.Id;
                    ShowingItem.UnitOfMeasure = UnitOfMeasure;
                    
                    ShowingItems.Add(ShowingItem);
                }
            }
            ShowingItems = await ShowingItemService.Import(ShowingItems);
            if (ShowingItems.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ShowingItems.Count; i++)
                {
                    ShowingItem ShowingItem = ShowingItems[i];
                    if (!ShowingItem.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.Id)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.Id)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.Code)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.Code)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.Name)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.Name)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.CategoryId)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.CategoryId)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.UnitOfMeasureId)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.UnitOfMeasureId)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.SalePrice)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.SalePrice)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.Desception)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.Desception)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.StatusId)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.StatusId)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.Used)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.Used)];
                        if (ShowingItem.Errors.ContainsKey(nameof(ShowingItem.RowId)))
                            Error += ShowingItem.Errors[nameof(ShowingItem.RowId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ShowingItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ShowingItem_ShowingItemFilterDTO ShowingItem_ShowingItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ShowingItem
                var ShowingItemFilter = ConvertFilterDTOToFilterEntity(ShowingItem_ShowingItemFilterDTO);
                ShowingItemFilter.Skip = 0;
                ShowingItemFilter.Take = int.MaxValue;
                ShowingItemFilter = await ShowingItemService.ToFilter(ShowingItemFilter);
                List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);

                var ShowingItemHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "CategoryId",
                        "UnitOfMeasureId",
                        "SalePrice",
                        "Desception",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> ShowingItemData = new List<object[]>();
                for (int i = 0; i < ShowingItems.Count; i++)
                {
                    var ShowingItem = ShowingItems[i];
                    ShowingItemData.Add(new Object[]
                    {
                        ShowingItem.Id,
                        ShowingItem.Code,
                        ShowingItem.Name,
                        ShowingItem.CategoryId,
                        ShowingItem.UnitOfMeasureId,
                        ShowingItem.SalePrice,
                        ShowingItem.Desception,
                        ShowingItem.StatusId,
                        ShowingItem.Used,
                        ShowingItem.RowId,
                    });
                }
                excel.GenerateWorksheet("ShowingItem", ShowingItemHeaders, ShowingItemData);
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
                #region UnitOfMeasure
                var UnitOfMeasureFilter = new UnitOfMeasureFilter();
                UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
                UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
                UnitOfMeasureFilter.OrderType = OrderType.ASC;
                UnitOfMeasureFilter.Skip = 0;
                UnitOfMeasureFilter.Take = int.MaxValue;
                List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);

                var UnitOfMeasureHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Description",
                        "StatusId",
                        "Used",
                        "RowId",
                    }
                };
                List<object[]> UnitOfMeasureData = new List<object[]>();
                for (int i = 0; i < UnitOfMeasures.Count; i++)
                {
                    var UnitOfMeasure = UnitOfMeasures[i];
                    UnitOfMeasureData.Add(new Object[]
                    {
                        UnitOfMeasure.Id,
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name,
                        UnitOfMeasure.Description,
                        UnitOfMeasure.StatusId,
                        UnitOfMeasure.Used,
                        UnitOfMeasure.RowId,
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasure", UnitOfMeasureHeaders, UnitOfMeasureData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ShowingItem.xlsx");
        }

        [Route(ShowingItemRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ShowingItem_ShowingItemFilterDTO ShowingItem_ShowingItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/ShowingItem_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "ShowingItem.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter();
            ShowingItemFilter = await ShowingItemService.ToFilter(ShowingItemFilter);
            if (Id == 0)
            {

            }
            else
            {
                ShowingItemFilter.Id = new IdFilter { Equal = Id };
                int count = await ShowingItemService.Count(ShowingItemFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ShowingItem ConvertDTOToEntity(ShowingItem_ShowingItemDTO ShowingItem_ShowingItemDTO)
        {
            ShowingItem ShowingItem = new ShowingItem();
            ShowingItem.Id = ShowingItem_ShowingItemDTO.Id;
            ShowingItem.Code = ShowingItem_ShowingItemDTO.Code;
            ShowingItem.Name = ShowingItem_ShowingItemDTO.Name;
            ShowingItem.CategoryId = ShowingItem_ShowingItemDTO.CategoryId;
            ShowingItem.UnitOfMeasureId = ShowingItem_ShowingItemDTO.UnitOfMeasureId;
            ShowingItem.SalePrice = ShowingItem_ShowingItemDTO.SalePrice;
            ShowingItem.Desception = ShowingItem_ShowingItemDTO.Desception;
            ShowingItem.StatusId = ShowingItem_ShowingItemDTO.StatusId;
            ShowingItem.Used = ShowingItem_ShowingItemDTO.Used;
            ShowingItem.RowId = ShowingItem_ShowingItemDTO.RowId;
            ShowingItem.Category = ShowingItem_ShowingItemDTO.Category == null ? null : new Category
            {
                Id = ShowingItem_ShowingItemDTO.Category.Id,
                Code = ShowingItem_ShowingItemDTO.Category.Code,
                Name = ShowingItem_ShowingItemDTO.Category.Name,
                ParentId = ShowingItem_ShowingItemDTO.Category.ParentId,
                Path = ShowingItem_ShowingItemDTO.Category.Path,
                Level = ShowingItem_ShowingItemDTO.Category.Level,
                StatusId = ShowingItem_ShowingItemDTO.Category.StatusId,
                ImageId = ShowingItem_ShowingItemDTO.Category.ImageId,
                RowId = ShowingItem_ShowingItemDTO.Category.RowId,
                Used = ShowingItem_ShowingItemDTO.Category.Used,
            };
            ShowingItem.Status = ShowingItem_ShowingItemDTO.Status == null ? null : new Status
            {
                Id = ShowingItem_ShowingItemDTO.Status.Id,
                Code = ShowingItem_ShowingItemDTO.Status.Code,
                Name = ShowingItem_ShowingItemDTO.Status.Name,
            };
            ShowingItem.UnitOfMeasure = ShowingItem_ShowingItemDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = ShowingItem_ShowingItemDTO.UnitOfMeasure.Id,
                Code = ShowingItem_ShowingItemDTO.UnitOfMeasure.Code,
                Name = ShowingItem_ShowingItemDTO.UnitOfMeasure.Name,
                Description = ShowingItem_ShowingItemDTO.UnitOfMeasure.Description,
                StatusId = ShowingItem_ShowingItemDTO.UnitOfMeasure.StatusId,
                Used = ShowingItem_ShowingItemDTO.UnitOfMeasure.Used,
                RowId = ShowingItem_ShowingItemDTO.UnitOfMeasure.RowId,
            };
            ShowingItem.BaseLanguage = CurrentContext.Language;
            return ShowingItem;
        }

        private ShowingItemFilter ConvertFilterDTOToFilterEntity(ShowingItem_ShowingItemFilterDTO ShowingItem_ShowingItemFilterDTO)
        {
            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter();
            ShowingItemFilter.Selects = ShowingItemSelect.ALL;
            ShowingItemFilter.Skip = ShowingItem_ShowingItemFilterDTO.Skip;
            ShowingItemFilter.Take = ShowingItem_ShowingItemFilterDTO.Take;
            ShowingItemFilter.OrderBy = ShowingItem_ShowingItemFilterDTO.OrderBy;
            ShowingItemFilter.OrderType = ShowingItem_ShowingItemFilterDTO.OrderType;

            ShowingItemFilter.Id = ShowingItem_ShowingItemFilterDTO.Id;
            ShowingItemFilter.Code = ShowingItem_ShowingItemFilterDTO.Code;
            ShowingItemFilter.Name = ShowingItem_ShowingItemFilterDTO.Name;
            ShowingItemFilter.CategoryId = ShowingItem_ShowingItemFilterDTO.CategoryId;
            ShowingItemFilter.UnitOfMeasureId = ShowingItem_ShowingItemFilterDTO.UnitOfMeasureId;
            ShowingItemFilter.SalePrice = ShowingItem_ShowingItemFilterDTO.SalePrice;
            ShowingItemFilter.Desception = ShowingItem_ShowingItemFilterDTO.Desception;
            ShowingItemFilter.StatusId = ShowingItem_ShowingItemFilterDTO.StatusId;
            ShowingItemFilter.RowId = ShowingItem_ShowingItemFilterDTO.RowId;
            ShowingItemFilter.CreatedAt = ShowingItem_ShowingItemFilterDTO.CreatedAt;
            ShowingItemFilter.UpdatedAt = ShowingItem_ShowingItemFilterDTO.UpdatedAt;
            return ShowingItemFilter;
        }
    }
}

