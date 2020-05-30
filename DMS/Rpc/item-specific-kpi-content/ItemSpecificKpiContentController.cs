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
using DMS.Services.MItemSpecificKpiContent;
using DMS.Services.MItem;
using DMS.Services.MItemSpecificCriteria;
using DMS.Services.MItemSpecificKpi;

namespace DMS.Rpc.item_specific_kpi_content
{
    public class ItemSpecificKpiContentController : RpcController
    {
        private IItemService ItemService;
        private IItemSpecificCriteriaService ItemSpecificCriteriaService;
        private IItemSpecificKpiService ItemSpecificKpiService;
        private IItemSpecificKpiContentService ItemSpecificKpiContentService;
        private ICurrentContext CurrentContext;
        public ItemSpecificKpiContentController(
            IItemService ItemService,
            IItemSpecificCriteriaService ItemSpecificCriteriaService,
            IItemSpecificKpiService ItemSpecificKpiService,
            IItemSpecificKpiContentService ItemSpecificKpiContentService,
            ICurrentContext CurrentContext
        )
        {
            this.ItemService = ItemService;
            this.ItemSpecificCriteriaService = ItemSpecificCriteriaService;
            this.ItemSpecificKpiService = ItemSpecificKpiService;
            this.ItemSpecificKpiContentService = ItemSpecificKpiContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ItemSpecificKpiContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = ConvertFilterDTOToFilterEntity(ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO);
            ItemSpecificKpiContentFilter = ItemSpecificKpiContentService.ToFilter(ItemSpecificKpiContentFilter);
            int count = await ItemSpecificKpiContentService.Count(ItemSpecificKpiContentFilter);
            return count;
        }

        [Route(ItemSpecificKpiContentRoute.List), HttpPost]
        public async Task<ActionResult<List<ItemSpecificKpiContent_ItemSpecificKpiContentDTO>>> List([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = ConvertFilterDTOToFilterEntity(ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO);
            ItemSpecificKpiContentFilter = ItemSpecificKpiContentService.ToFilter(ItemSpecificKpiContentFilter);
            List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);
            List<ItemSpecificKpiContent_ItemSpecificKpiContentDTO> ItemSpecificKpiContent_ItemSpecificKpiContentDTOs = ItemSpecificKpiContents
                .Select(c => new ItemSpecificKpiContent_ItemSpecificKpiContentDTO(c)).ToList();
            return ItemSpecificKpiContent_ItemSpecificKpiContentDTOs;
        }

        [Route(ItemSpecificKpiContentRoute.Get), HttpPost]
        public async Task<ActionResult<ItemSpecificKpiContent_ItemSpecificKpiContentDTO>> Get([FromBody]ItemSpecificKpiContent_ItemSpecificKpiContentDTO ItemSpecificKpiContent_ItemSpecificKpiContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Id))
                return Forbid();

            ItemSpecificKpiContent ItemSpecificKpiContent = await ItemSpecificKpiContentService.Get(ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Id);
            return new ItemSpecificKpiContent_ItemSpecificKpiContentDTO(ItemSpecificKpiContent);
        }

        [Route(ItemSpecificKpiContentRoute.Create), HttpPost]
        public async Task<ActionResult<ItemSpecificKpiContent_ItemSpecificKpiContentDTO>> Create([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentDTO ItemSpecificKpiContent_ItemSpecificKpiContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Id))
                return Forbid();

            ItemSpecificKpiContent ItemSpecificKpiContent = ConvertDTOToEntity(ItemSpecificKpiContent_ItemSpecificKpiContentDTO);
            ItemSpecificKpiContent = await ItemSpecificKpiContentService.Create(ItemSpecificKpiContent);
            ItemSpecificKpiContent_ItemSpecificKpiContentDTO = new ItemSpecificKpiContent_ItemSpecificKpiContentDTO(ItemSpecificKpiContent);
            if (ItemSpecificKpiContent.IsValidated)
                return ItemSpecificKpiContent_ItemSpecificKpiContentDTO;
            else
                return BadRequest(ItemSpecificKpiContent_ItemSpecificKpiContentDTO);
        }

        [Route(ItemSpecificKpiContentRoute.Update), HttpPost]
        public async Task<ActionResult<ItemSpecificKpiContent_ItemSpecificKpiContentDTO>> Update([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentDTO ItemSpecificKpiContent_ItemSpecificKpiContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Id))
                return Forbid();

            ItemSpecificKpiContent ItemSpecificKpiContent = ConvertDTOToEntity(ItemSpecificKpiContent_ItemSpecificKpiContentDTO);
            ItemSpecificKpiContent = await ItemSpecificKpiContentService.Update(ItemSpecificKpiContent);
            ItemSpecificKpiContent_ItemSpecificKpiContentDTO = new ItemSpecificKpiContent_ItemSpecificKpiContentDTO(ItemSpecificKpiContent);
            if (ItemSpecificKpiContent.IsValidated)
                return ItemSpecificKpiContent_ItemSpecificKpiContentDTO;
            else
                return BadRequest(ItemSpecificKpiContent_ItemSpecificKpiContentDTO);
        }

        [Route(ItemSpecificKpiContentRoute.Delete), HttpPost]
        public async Task<ActionResult<ItemSpecificKpiContent_ItemSpecificKpiContentDTO>> Delete([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentDTO ItemSpecificKpiContent_ItemSpecificKpiContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Id))
                return Forbid();

            ItemSpecificKpiContent ItemSpecificKpiContent = ConvertDTOToEntity(ItemSpecificKpiContent_ItemSpecificKpiContentDTO);
            ItemSpecificKpiContent = await ItemSpecificKpiContentService.Delete(ItemSpecificKpiContent);
            ItemSpecificKpiContent_ItemSpecificKpiContentDTO = new ItemSpecificKpiContent_ItemSpecificKpiContentDTO(ItemSpecificKpiContent);
            if (ItemSpecificKpiContent.IsValidated)
                return ItemSpecificKpiContent_ItemSpecificKpiContentDTO;
            else
                return BadRequest(ItemSpecificKpiContent_ItemSpecificKpiContentDTO);
        }
        
        [Route(ItemSpecificKpiContentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
            ItemSpecificKpiContentFilter = ItemSpecificKpiContentService.ToFilter(ItemSpecificKpiContentFilter);
            ItemSpecificKpiContentFilter.Id = new IdFilter { In = Ids };
            ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.Id;
            ItemSpecificKpiContentFilter.Skip = 0;
            ItemSpecificKpiContentFilter.Take = int.MaxValue;

            List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);
            ItemSpecificKpiContents = await ItemSpecificKpiContentService.BulkDelete(ItemSpecificKpiContents);
            return true;
        }
        
        [Route(ItemSpecificKpiContentRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ItemFilter ItemFilter = new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.ALL
            };
            List<Item> Items = await ItemService.List(ItemFilter);
            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSpecificCriteriaSelect.ALL
            };
            List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);
            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSpecificKpiSelect.ALL
            };
            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<ItemSpecificKpiContent> ItemSpecificKpiContents = new List<ItemSpecificKpiContent>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ItemSpecificKpiContents);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int ItemSpecificKpiIdColumn = 1 + StartColumn;
                int ItemSpecificCriteriaIdColumn = 2 + StartColumn;
                int ItemIdColumn = 3 + StartColumn;
                int ValueColumn = 4 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string ItemSpecificKpiIdValue = worksheet.Cells[i + StartRow, ItemSpecificKpiIdColumn].Value?.ToString();
                    string ItemSpecificCriteriaIdValue = worksheet.Cells[i + StartRow, ItemSpecificCriteriaIdColumn].Value?.ToString();
                    string ItemIdValue = worksheet.Cells[i + StartRow, ItemIdColumn].Value?.ToString();
                    string ValueValue = worksheet.Cells[i + StartRow, ValueColumn].Value?.ToString();
                    
                    ItemSpecificKpiContent ItemSpecificKpiContent = new ItemSpecificKpiContent();
                    ItemSpecificKpiContent.Value = long.TryParse(ValueValue, out long Value) ? Value : 0;
                    Item Item = Items.Where(x => x.Id.ToString() == ItemIdValue).FirstOrDefault();
                    ItemSpecificKpiContent.ItemId = Item == null ? 0 : Item.Id;
                    ItemSpecificKpiContent.Item = Item;
                    ItemSpecificCriteria ItemSpecificCriteria = ItemSpecificCriterias.Where(x => x.Id.ToString() == ItemSpecificCriteriaIdValue).FirstOrDefault();
                    ItemSpecificKpiContent.ItemSpecificCriteriaId = ItemSpecificCriteria == null ? 0 : ItemSpecificCriteria.Id;
                    ItemSpecificKpiContent.ItemSpecificCriteria = ItemSpecificCriteria;
                    ItemSpecificKpi ItemSpecificKpi = ItemSpecificKpis.Where(x => x.Id.ToString() == ItemSpecificKpiIdValue).FirstOrDefault();
                    ItemSpecificKpiContent.ItemSpecificKpiId = ItemSpecificKpi == null ? 0 : ItemSpecificKpi.Id;
                    ItemSpecificKpiContent.ItemSpecificKpi = ItemSpecificKpi;
                    
                    ItemSpecificKpiContents.Add(ItemSpecificKpiContent);
                }
            }
            ItemSpecificKpiContents = await ItemSpecificKpiContentService.Import(ItemSpecificKpiContents);
            if (ItemSpecificKpiContents.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ItemSpecificKpiContents.Count; i++)
                {
                    ItemSpecificKpiContent ItemSpecificKpiContent = ItemSpecificKpiContents[i];
                    if (!ItemSpecificKpiContent.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ItemSpecificKpiContent.Errors.ContainsKey(nameof(ItemSpecificKpiContent.Id)))
                            Error += ItemSpecificKpiContent.Errors[nameof(ItemSpecificKpiContent.Id)];
                        if (ItemSpecificKpiContent.Errors.ContainsKey(nameof(ItemSpecificKpiContent.ItemSpecificKpiId)))
                            Error += ItemSpecificKpiContent.Errors[nameof(ItemSpecificKpiContent.ItemSpecificKpiId)];
                        if (ItemSpecificKpiContent.Errors.ContainsKey(nameof(ItemSpecificKpiContent.ItemSpecificCriteriaId)))
                            Error += ItemSpecificKpiContent.Errors[nameof(ItemSpecificKpiContent.ItemSpecificCriteriaId)];
                        if (ItemSpecificKpiContent.Errors.ContainsKey(nameof(ItemSpecificKpiContent.ItemId)))
                            Error += ItemSpecificKpiContent.Errors[nameof(ItemSpecificKpiContent.ItemId)];
                        if (ItemSpecificKpiContent.Errors.ContainsKey(nameof(ItemSpecificKpiContent.Value)))
                            Error += ItemSpecificKpiContent.Errors[nameof(ItemSpecificKpiContent.Value)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ItemSpecificKpiContentRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ItemSpecificKpiContent
                var ItemSpecificKpiContentFilter = ConvertFilterDTOToFilterEntity(ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO);
                ItemSpecificKpiContentFilter.Skip = 0;
                ItemSpecificKpiContentFilter.Take = int.MaxValue;
                ItemSpecificKpiContentFilter = ItemSpecificKpiContentService.ToFilter(ItemSpecificKpiContentFilter);
                List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);

                var ItemSpecificKpiContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ItemSpecificKpiId",
                        "ItemSpecificCriteriaId",
                        "ItemId",
                        "Value",
                    }
                };
                List<object[]> ItemSpecificKpiContentData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpiContents.Count; i++)
                {
                    var ItemSpecificKpiContent = ItemSpecificKpiContents[i];
                    ItemSpecificKpiContentData.Add(new Object[]
                    {
                        ItemSpecificKpiContent.Id,
                        ItemSpecificKpiContent.ItemSpecificKpiId,
                        ItemSpecificKpiContent.ItemSpecificCriteriaId,
                        ItemSpecificKpiContent.ItemId,
                        ItemSpecificKpiContent.Value,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpiContent", ItemSpecificKpiContentHeaders, ItemSpecificKpiContentData);
                #endregion
                
                #region Item
                var ItemFilter = new ItemFilter();
                ItemFilter.Selects = ItemSelect.ALL;
                ItemFilter.OrderBy = ItemOrder.Id;
                ItemFilter.OrderType = OrderType.ASC;
                ItemFilter.Skip = 0;
                ItemFilter.Take = int.MaxValue;
                List<Item> Items = await ItemService.List(ItemFilter);

                var ItemHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ProductId",
                        "Code",
                        "Name",
                        "ScanCode",
                        "SalePrice",
                        "RetailPrice",
                        "StatusId",
                    }
                };
                List<object[]> ItemData = new List<object[]>();
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    ItemData.Add(new Object[]
                    {
                        Item.Id,
                        Item.ProductId,
                        Item.Code,
                        Item.Name,
                        Item.ScanCode,
                        Item.SalePrice,
                        Item.RetailPrice,
                        Item.StatusId,
                    });
                }
                excel.GenerateWorksheet("Item", ItemHeaders, ItemData);
                #endregion
                #region ItemSpecificCriteria
                var ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
                ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
                ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteriaOrder.Id;
                ItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
                ItemSpecificCriteriaFilter.Skip = 0;
                ItemSpecificCriteriaFilter.Take = int.MaxValue;
                List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);

                var ItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < ItemSpecificCriterias.Count; i++)
                {
                    var ItemSpecificCriteria = ItemSpecificCriterias[i];
                    ItemSpecificCriteriaData.Add(new Object[]
                    {
                        ItemSpecificCriteria.Id,
                        ItemSpecificCriteria.Code,
                        ItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificCriteria", ItemSpecificCriteriaHeaders, ItemSpecificCriteriaData);
                #endregion
                #region ItemSpecificKpi
                var ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
                ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
                ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
                ItemSpecificKpiFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiFilter.Skip = 0;
                ItemSpecificKpiFilter.Take = int.MaxValue;
                List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);

                var ItemSpecificKpiHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "KpiPeriodId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                    }
                };
                List<object[]> ItemSpecificKpiData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpis.Count; i++)
                {
                    var ItemSpecificKpi = ItemSpecificKpis[i];
                    ItemSpecificKpiData.Add(new Object[]
                    {
                        ItemSpecificKpi.Id,
                        ItemSpecificKpi.OrganizationId,
                        ItemSpecificKpi.KpiPeriodId,
                        ItemSpecificKpi.StatusId,
                        ItemSpecificKpi.EmployeeId,
                        ItemSpecificKpi.CreatorId,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpi", ItemSpecificKpiHeaders, ItemSpecificKpiData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ItemSpecificKpiContent.xlsx");
        }

        [Route(ItemSpecificKpiContentRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ItemSpecificKpiContent
                var ItemSpecificKpiContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ItemSpecificKpiId",
                        "ItemSpecificCriteriaId",
                        "ItemId",
                        "Value",
                    }
                };
                List<object[]> ItemSpecificKpiContentData = new List<object[]>();
                excel.GenerateWorksheet("ItemSpecificKpiContent", ItemSpecificKpiContentHeaders, ItemSpecificKpiContentData);
                #endregion
                
                #region Item
                var ItemFilter = new ItemFilter();
                ItemFilter.Selects = ItemSelect.ALL;
                ItemFilter.OrderBy = ItemOrder.Id;
                ItemFilter.OrderType = OrderType.ASC;
                ItemFilter.Skip = 0;
                ItemFilter.Take = int.MaxValue;
                List<Item> Items = await ItemService.List(ItemFilter);

                var ItemHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ProductId",
                        "Code",
                        "Name",
                        "ScanCode",
                        "SalePrice",
                        "RetailPrice",
                        "StatusId",
                    }
                };
                List<object[]> ItemData = new List<object[]>();
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    ItemData.Add(new Object[]
                    {
                        Item.Id,
                        Item.ProductId,
                        Item.Code,
                        Item.Name,
                        Item.ScanCode,
                        Item.SalePrice,
                        Item.RetailPrice,
                        Item.StatusId,
                    });
                }
                excel.GenerateWorksheet("Item", ItemHeaders, ItemData);
                #endregion
                #region ItemSpecificCriteria
                var ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
                ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
                ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteriaOrder.Id;
                ItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
                ItemSpecificCriteriaFilter.Skip = 0;
                ItemSpecificCriteriaFilter.Take = int.MaxValue;
                List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);

                var ItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < ItemSpecificCriterias.Count; i++)
                {
                    var ItemSpecificCriteria = ItemSpecificCriterias[i];
                    ItemSpecificCriteriaData.Add(new Object[]
                    {
                        ItemSpecificCriteria.Id,
                        ItemSpecificCriteria.Code,
                        ItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificCriteria", ItemSpecificCriteriaHeaders, ItemSpecificCriteriaData);
                #endregion
                #region ItemSpecificKpi
                var ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
                ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
                ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
                ItemSpecificKpiFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiFilter.Skip = 0;
                ItemSpecificKpiFilter.Take = int.MaxValue;
                List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);

                var ItemSpecificKpiHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "KpiPeriodId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                    }
                };
                List<object[]> ItemSpecificKpiData = new List<object[]>();
                for (int i = 0; i < ItemSpecificKpis.Count; i++)
                {
                    var ItemSpecificKpi = ItemSpecificKpis[i];
                    ItemSpecificKpiData.Add(new Object[]
                    {
                        ItemSpecificKpi.Id,
                        ItemSpecificKpi.OrganizationId,
                        ItemSpecificKpi.KpiPeriodId,
                        ItemSpecificKpi.StatusId,
                        ItemSpecificKpi.EmployeeId,
                        ItemSpecificKpi.CreatorId,
                    });
                }
                excel.GenerateWorksheet("ItemSpecificKpi", ItemSpecificKpiHeaders, ItemSpecificKpiData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ItemSpecificKpiContent.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
            ItemSpecificKpiContentFilter = ItemSpecificKpiContentService.ToFilter(ItemSpecificKpiContentFilter);
            if (Id == 0)
            {

            }
            else
            {
                ItemSpecificKpiContentFilter.Id = new IdFilter { Equal = Id };
                int count = await ItemSpecificKpiContentService.Count(ItemSpecificKpiContentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ItemSpecificKpiContent ConvertDTOToEntity(ItemSpecificKpiContent_ItemSpecificKpiContentDTO ItemSpecificKpiContent_ItemSpecificKpiContentDTO)
        {
            ItemSpecificKpiContent ItemSpecificKpiContent = new ItemSpecificKpiContent();
            ItemSpecificKpiContent.Id = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Id;
            ItemSpecificKpiContent.ItemSpecificKpiId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpiId;
            ItemSpecificKpiContent.ItemSpecificCriteriaId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificCriteriaId;
            ItemSpecificKpiContent.ItemId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemId;
            ItemSpecificKpiContent.Value = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Value;
            ItemSpecificKpiContent.Item = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item == null ? null : new Item
            {
                Id = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.Id,
                ProductId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.ProductId,
                Code = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.Code,
                Name = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.Name,
                ScanCode = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.ScanCode,
                SalePrice = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.SalePrice,
                RetailPrice = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.RetailPrice,
                StatusId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.Item.StatusId,
            };
            ItemSpecificKpiContent.ItemSpecificCriteria = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificCriteria == null ? null : new ItemSpecificCriteria
            {
                Id = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificCriteria.Id,
                Code = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificCriteria.Code,
                Name = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificCriteria.Name,
            };
            ItemSpecificKpiContent.ItemSpecificKpi = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi == null ? null : new ItemSpecificKpi
            {
                Id = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi.Id,
                OrganizationId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi.OrganizationId,
                KpiPeriodId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi.KpiPeriodId,
                StatusId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi.StatusId,
                EmployeeId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi.EmployeeId,
                CreatorId = ItemSpecificKpiContent_ItemSpecificKpiContentDTO.ItemSpecificKpi.CreatorId,
            };
            ItemSpecificKpiContent.BaseLanguage = CurrentContext.Language;
            return ItemSpecificKpiContent;
        }

        private ItemSpecificKpiContentFilter ConvertFilterDTOToFilterEntity(ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO)
        {
            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
            ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
            ItemSpecificKpiContentFilter.Skip = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.Skip;
            ItemSpecificKpiContentFilter.Take = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.Take;
            ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.OrderBy;
            ItemSpecificKpiContentFilter.OrderType = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.OrderType;

            ItemSpecificKpiContentFilter.Id = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.Id;
            ItemSpecificKpiContentFilter.ItemSpecificKpiId = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.ItemSpecificKpiId;
            ItemSpecificKpiContentFilter.ItemSpecificCriteriaId = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.ItemSpecificCriteriaId;
            ItemSpecificKpiContentFilter.ItemId = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.ItemId;
            ItemSpecificKpiContentFilter.Value = ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO.Value;
            return ItemSpecificKpiContentFilter;
        }

        [Route(ItemSpecificKpiContentRoute.FilterListItem), HttpPost]
        public async Task<List<ItemSpecificKpiContent_ItemDTO>> FilterListItem([FromBody] ItemSpecificKpiContent_ItemFilterDTO ItemSpecificKpiContent_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ItemSpecificKpiContent_ItemFilterDTO.Id;
            ItemFilter.ProductId = ItemSpecificKpiContent_ItemFilterDTO.ProductId;
            ItemFilter.Code = ItemSpecificKpiContent_ItemFilterDTO.Code;
            ItemFilter.Name = ItemSpecificKpiContent_ItemFilterDTO.Name;
            ItemFilter.ScanCode = ItemSpecificKpiContent_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = ItemSpecificKpiContent_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = ItemSpecificKpiContent_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = ItemSpecificKpiContent_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ItemSpecificKpiContent_ItemDTO> ItemSpecificKpiContent_ItemDTOs = Items
                .Select(x => new ItemSpecificKpiContent_ItemDTO(x)).ToList();
            return ItemSpecificKpiContent_ItemDTOs;
        }
        [Route(ItemSpecificKpiContentRoute.FilterListItemSpecificCriteria), HttpPost]
        public async Task<List<ItemSpecificKpiContent_ItemSpecificCriteriaDTO>> FilterListItemSpecificCriteria([FromBody] ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
            ItemSpecificCriteriaFilter.Skip = 0;
            ItemSpecificCriteriaFilter.Take = 20;
            ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteriaOrder.Id;
            ItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
            ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
            ItemSpecificCriteriaFilter.Id = ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO.Id;
            ItemSpecificCriteriaFilter.Code = ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO.Code;
            ItemSpecificCriteriaFilter.Name = ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO.Name;

            List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);
            List<ItemSpecificKpiContent_ItemSpecificCriteriaDTO> ItemSpecificKpiContent_ItemSpecificCriteriaDTOs = ItemSpecificCriterias
                .Select(x => new ItemSpecificKpiContent_ItemSpecificCriteriaDTO(x)).ToList();
            return ItemSpecificKpiContent_ItemSpecificCriteriaDTOs;
        }
        [Route(ItemSpecificKpiContentRoute.FilterListItemSpecificKpi), HttpPost]
        public async Task<List<ItemSpecificKpiContent_ItemSpecificKpiDTO>> FilterListItemSpecificKpi([FromBody] ItemSpecificKpiContent_ItemSpecificKpiFilterDTO ItemSpecificKpiContent_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<ItemSpecificKpiContent_ItemSpecificKpiDTO> ItemSpecificKpiContent_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new ItemSpecificKpiContent_ItemSpecificKpiDTO(x)).ToList();
            return ItemSpecificKpiContent_ItemSpecificKpiDTOs;
        }

        [Route(ItemSpecificKpiContentRoute.SingleListItem), HttpPost]
        public async Task<List<ItemSpecificKpiContent_ItemDTO>> SingleListItem([FromBody] ItemSpecificKpiContent_ItemFilterDTO ItemSpecificKpiContent_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ItemSpecificKpiContent_ItemFilterDTO.Id;
            ItemFilter.ProductId = ItemSpecificKpiContent_ItemFilterDTO.ProductId;
            ItemFilter.Code = ItemSpecificKpiContent_ItemFilterDTO.Code;
            ItemFilter.Name = ItemSpecificKpiContent_ItemFilterDTO.Name;
            ItemFilter.ScanCode = ItemSpecificKpiContent_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = ItemSpecificKpiContent_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = ItemSpecificKpiContent_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = ItemSpecificKpiContent_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ItemSpecificKpiContent_ItemDTO> ItemSpecificKpiContent_ItemDTOs = Items
                .Select(x => new ItemSpecificKpiContent_ItemDTO(x)).ToList();
            return ItemSpecificKpiContent_ItemDTOs;
        }
        [Route(ItemSpecificKpiContentRoute.SingleListItemSpecificCriteria), HttpPost]
        public async Task<List<ItemSpecificKpiContent_ItemSpecificCriteriaDTO>> SingleListItemSpecificCriteria([FromBody] ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
            ItemSpecificCriteriaFilter.Skip = 0;
            ItemSpecificCriteriaFilter.Take = 20;
            ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteriaOrder.Id;
            ItemSpecificCriteriaFilter.OrderType = OrderType.ASC;
            ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
            ItemSpecificCriteriaFilter.Id = ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO.Id;
            ItemSpecificCriteriaFilter.Code = ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO.Code;
            ItemSpecificCriteriaFilter.Name = ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO.Name;

            List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);
            List<ItemSpecificKpiContent_ItemSpecificCriteriaDTO> ItemSpecificKpiContent_ItemSpecificCriteriaDTOs = ItemSpecificCriterias
                .Select(x => new ItemSpecificKpiContent_ItemSpecificCriteriaDTO(x)).ToList();
            return ItemSpecificKpiContent_ItemSpecificCriteriaDTOs;
        }
        [Route(ItemSpecificKpiContentRoute.SingleListItemSpecificKpi), HttpPost]
        public async Task<List<ItemSpecificKpiContent_ItemSpecificKpiDTO>> SingleListItemSpecificKpi([FromBody] ItemSpecificKpiContent_ItemSpecificKpiFilterDTO ItemSpecificKpiContent_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = ItemSpecificKpiContent_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<ItemSpecificKpiContent_ItemSpecificKpiDTO> ItemSpecificKpiContent_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new ItemSpecificKpiContent_ItemSpecificKpiDTO(x)).ToList();
            return ItemSpecificKpiContent_ItemSpecificKpiDTOs;
        }

    }
}

