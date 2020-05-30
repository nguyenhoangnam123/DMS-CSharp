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
using DMS.Services.MItemSpecificCriteria;
using DMS.Services.MItemSpecificKpiContent;
using DMS.Services.MItem;
using DMS.Services.MItemSpecificKpi;

namespace DMS.Rpc.item_specific_criteria
{
    public class ItemSpecificCriteriaController : RpcController
    {
        private IItemSpecificKpiContentService ItemSpecificKpiContentService;
        private IItemService ItemService;
        private IItemSpecificKpiService ItemSpecificKpiService;
        private IItemSpecificCriteriaService ItemSpecificCriteriaService;
        private ICurrentContext CurrentContext;
        public ItemSpecificCriteriaController(
            IItemSpecificKpiContentService ItemSpecificKpiContentService,
            IItemService ItemService,
            IItemSpecificKpiService ItemSpecificKpiService,
            IItemSpecificCriteriaService ItemSpecificCriteriaService,
            ICurrentContext CurrentContext
        )
        {
            this.ItemSpecificKpiContentService = ItemSpecificKpiContentService;
            this.ItemService = ItemService;
            this.ItemSpecificKpiService = ItemSpecificKpiService;
            this.ItemSpecificCriteriaService = ItemSpecificCriteriaService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ItemSpecificCriteriaRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = ConvertFilterDTOToFilterEntity(ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO);
            ItemSpecificCriteriaFilter = ItemSpecificCriteriaService.ToFilter(ItemSpecificCriteriaFilter);
            int count = await ItemSpecificCriteriaService.Count(ItemSpecificCriteriaFilter);
            return count;
        }

        [Route(ItemSpecificCriteriaRoute.List), HttpPost]
        public async Task<ActionResult<List<ItemSpecificCriteria_ItemSpecificCriteriaDTO>>> List([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = ConvertFilterDTOToFilterEntity(ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO);
            ItemSpecificCriteriaFilter = ItemSpecificCriteriaService.ToFilter(ItemSpecificCriteriaFilter);
            List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);
            List<ItemSpecificCriteria_ItemSpecificCriteriaDTO> ItemSpecificCriteria_ItemSpecificCriteriaDTOs = ItemSpecificCriterias
                .Select(c => new ItemSpecificCriteria_ItemSpecificCriteriaDTO(c)).ToList();
            return ItemSpecificCriteria_ItemSpecificCriteriaDTOs;
        }

        [Route(ItemSpecificCriteriaRoute.Get), HttpPost]
        public async Task<ActionResult<ItemSpecificCriteria_ItemSpecificCriteriaDTO>> Get([FromBody]ItemSpecificCriteria_ItemSpecificCriteriaDTO ItemSpecificCriteria_ItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificCriteria_ItemSpecificCriteriaDTO.Id))
                return Forbid();

            ItemSpecificCriteria ItemSpecificCriteria = await ItemSpecificCriteriaService.Get(ItemSpecificCriteria_ItemSpecificCriteriaDTO.Id);
            return new ItemSpecificCriteria_ItemSpecificCriteriaDTO(ItemSpecificCriteria);
        }

        [Route(ItemSpecificCriteriaRoute.Create), HttpPost]
        public async Task<ActionResult<ItemSpecificCriteria_ItemSpecificCriteriaDTO>> Create([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaDTO ItemSpecificCriteria_ItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ItemSpecificCriteria_ItemSpecificCriteriaDTO.Id))
                return Forbid();

            ItemSpecificCriteria ItemSpecificCriteria = ConvertDTOToEntity(ItemSpecificCriteria_ItemSpecificCriteriaDTO);
            ItemSpecificCriteria = await ItemSpecificCriteriaService.Create(ItemSpecificCriteria);
            ItemSpecificCriteria_ItemSpecificCriteriaDTO = new ItemSpecificCriteria_ItemSpecificCriteriaDTO(ItemSpecificCriteria);
            if (ItemSpecificCriteria.IsValidated)
                return ItemSpecificCriteria_ItemSpecificCriteriaDTO;
            else
                return BadRequest(ItemSpecificCriteria_ItemSpecificCriteriaDTO);
        }

        [Route(ItemSpecificCriteriaRoute.Update), HttpPost]
        public async Task<ActionResult<ItemSpecificCriteria_ItemSpecificCriteriaDTO>> Update([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaDTO ItemSpecificCriteria_ItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ItemSpecificCriteria_ItemSpecificCriteriaDTO.Id))
                return Forbid();

            ItemSpecificCriteria ItemSpecificCriteria = ConvertDTOToEntity(ItemSpecificCriteria_ItemSpecificCriteriaDTO);
            ItemSpecificCriteria = await ItemSpecificCriteriaService.Update(ItemSpecificCriteria);
            ItemSpecificCriteria_ItemSpecificCriteriaDTO = new ItemSpecificCriteria_ItemSpecificCriteriaDTO(ItemSpecificCriteria);
            if (ItemSpecificCriteria.IsValidated)
                return ItemSpecificCriteria_ItemSpecificCriteriaDTO;
            else
                return BadRequest(ItemSpecificCriteria_ItemSpecificCriteriaDTO);
        }

        [Route(ItemSpecificCriteriaRoute.Delete), HttpPost]
        public async Task<ActionResult<ItemSpecificCriteria_ItemSpecificCriteriaDTO>> Delete([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaDTO ItemSpecificCriteria_ItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ItemSpecificCriteria_ItemSpecificCriteriaDTO.Id))
                return Forbid();

            ItemSpecificCriteria ItemSpecificCriteria = ConvertDTOToEntity(ItemSpecificCriteria_ItemSpecificCriteriaDTO);
            ItemSpecificCriteria = await ItemSpecificCriteriaService.Delete(ItemSpecificCriteria);
            ItemSpecificCriteria_ItemSpecificCriteriaDTO = new ItemSpecificCriteria_ItemSpecificCriteriaDTO(ItemSpecificCriteria);
            if (ItemSpecificCriteria.IsValidated)
                return ItemSpecificCriteria_ItemSpecificCriteriaDTO;
            else
                return BadRequest(ItemSpecificCriteria_ItemSpecificCriteriaDTO);
        }
        
        [Route(ItemSpecificCriteriaRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
            ItemSpecificCriteriaFilter = ItemSpecificCriteriaService.ToFilter(ItemSpecificCriteriaFilter);
            ItemSpecificCriteriaFilter.Id = new IdFilter { In = Ids };
            ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.Id;
            ItemSpecificCriteriaFilter.Skip = 0;
            ItemSpecificCriteriaFilter.Take = int.MaxValue;

            List<ItemSpecificCriteria> ItemSpecificCriterias = await ItemSpecificCriteriaService.List(ItemSpecificCriteriaFilter);
            ItemSpecificCriterias = await ItemSpecificCriteriaService.BulkDelete(ItemSpecificCriterias);
            return true;
        }
        
        [Route(ItemSpecificCriteriaRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<ItemSpecificCriteria> ItemSpecificCriterias = new List<ItemSpecificCriteria>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ItemSpecificCriterias);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    
                    ItemSpecificCriteria ItemSpecificCriteria = new ItemSpecificCriteria();
                    ItemSpecificCriteria.Code = CodeValue;
                    ItemSpecificCriteria.Name = NameValue;
                    
                    ItemSpecificCriterias.Add(ItemSpecificCriteria);
                }
            }
            ItemSpecificCriterias = await ItemSpecificCriteriaService.Import(ItemSpecificCriterias);
            if (ItemSpecificCriterias.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ItemSpecificCriterias.Count; i++)
                {
                    ItemSpecificCriteria ItemSpecificCriteria = ItemSpecificCriterias[i];
                    if (!ItemSpecificCriteria.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ItemSpecificCriteria.Errors.ContainsKey(nameof(ItemSpecificCriteria.Id)))
                            Error += ItemSpecificCriteria.Errors[nameof(ItemSpecificCriteria.Id)];
                        if (ItemSpecificCriteria.Errors.ContainsKey(nameof(ItemSpecificCriteria.Code)))
                            Error += ItemSpecificCriteria.Errors[nameof(ItemSpecificCriteria.Code)];
                        if (ItemSpecificCriteria.Errors.ContainsKey(nameof(ItemSpecificCriteria.Name)))
                            Error += ItemSpecificCriteria.Errors[nameof(ItemSpecificCriteria.Name)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ItemSpecificCriteriaRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ItemSpecificCriteria
                var ItemSpecificCriteriaFilter = ConvertFilterDTOToFilterEntity(ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO);
                ItemSpecificCriteriaFilter.Skip = 0;
                ItemSpecificCriteriaFilter.Take = int.MaxValue;
                ItemSpecificCriteriaFilter = ItemSpecificCriteriaService.ToFilter(ItemSpecificCriteriaFilter);
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
                
                #region ItemSpecificKpiContent
                var ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
                ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
                ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContentOrder.Id;
                ItemSpecificKpiContentFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiContentFilter.Skip = 0;
                ItemSpecificKpiContentFilter.Take = int.MaxValue;
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
            return File(memoryStream.ToArray(), "application/octet-stream", "ItemSpecificCriteria.xlsx");
        }

        [Route(ItemSpecificCriteriaRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ItemSpecificCriteria
                var ItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> ItemSpecificCriteriaData = new List<object[]>();
                excel.GenerateWorksheet("ItemSpecificCriteria", ItemSpecificCriteriaHeaders, ItemSpecificCriteriaData);
                #endregion
                
                #region ItemSpecificKpiContent
                var ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
                ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
                ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContentOrder.Id;
                ItemSpecificKpiContentFilter.OrderType = OrderType.ASC;
                ItemSpecificKpiContentFilter.Skip = 0;
                ItemSpecificKpiContentFilter.Take = int.MaxValue;
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
            return File(memoryStream.ToArray(), "application/octet-stream", "ItemSpecificCriteria.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
            ItemSpecificCriteriaFilter = ItemSpecificCriteriaService.ToFilter(ItemSpecificCriteriaFilter);
            if (Id == 0)
            {

            }
            else
            {
                ItemSpecificCriteriaFilter.Id = new IdFilter { Equal = Id };
                int count = await ItemSpecificCriteriaService.Count(ItemSpecificCriteriaFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ItemSpecificCriteria ConvertDTOToEntity(ItemSpecificCriteria_ItemSpecificCriteriaDTO ItemSpecificCriteria_ItemSpecificCriteriaDTO)
        {
            ItemSpecificCriteria ItemSpecificCriteria = new ItemSpecificCriteria();
            ItemSpecificCriteria.Id = ItemSpecificCriteria_ItemSpecificCriteriaDTO.Id;
            ItemSpecificCriteria.Code = ItemSpecificCriteria_ItemSpecificCriteriaDTO.Code;
            ItemSpecificCriteria.Name = ItemSpecificCriteria_ItemSpecificCriteriaDTO.Name;
            ItemSpecificCriteria.ItemSpecificKpiContents = ItemSpecificCriteria_ItemSpecificCriteriaDTO.ItemSpecificKpiContents?
                .Select(x => new ItemSpecificKpiContent
                {
                    Id = x.Id,
                    ItemSpecificKpiId = x.ItemSpecificKpiId,
                    ItemId = x.ItemId,
                    Value = x.Value,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                    },
                    ItemSpecificKpi = x.ItemSpecificKpi == null ? null : new ItemSpecificKpi
                    {
                        Id = x.ItemSpecificKpi.Id,
                        OrganizationId = x.ItemSpecificKpi.OrganizationId,
                        KpiPeriodId = x.ItemSpecificKpi.KpiPeriodId,
                        StatusId = x.ItemSpecificKpi.StatusId,
                        EmployeeId = x.ItemSpecificKpi.EmployeeId,
                        CreatorId = x.ItemSpecificKpi.CreatorId,
                    },
                }).ToList();
            ItemSpecificCriteria.BaseLanguage = CurrentContext.Language;
            return ItemSpecificCriteria;
        }

        private ItemSpecificCriteriaFilter ConvertFilterDTOToFilterEntity(ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO)
        {
            ItemSpecificCriteriaFilter ItemSpecificCriteriaFilter = new ItemSpecificCriteriaFilter();
            ItemSpecificCriteriaFilter.Selects = ItemSpecificCriteriaSelect.ALL;
            ItemSpecificCriteriaFilter.Skip = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.Skip;
            ItemSpecificCriteriaFilter.Take = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.Take;
            ItemSpecificCriteriaFilter.OrderBy = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.OrderBy;
            ItemSpecificCriteriaFilter.OrderType = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.OrderType;

            ItemSpecificCriteriaFilter.Id = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.Id;
            ItemSpecificCriteriaFilter.Code = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.Code;
            ItemSpecificCriteriaFilter.Name = ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO.Name;
            return ItemSpecificCriteriaFilter;
        }

        [Route(ItemSpecificCriteriaRoute.FilterListItemSpecificKpiContent), HttpPost]
        public async Task<List<ItemSpecificCriteria_ItemSpecificKpiContentDTO>> FilterListItemSpecificKpiContent([FromBody] ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
            ItemSpecificKpiContentFilter.Skip = 0;
            ItemSpecificKpiContentFilter.Take = 20;
            ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContentOrder.Id;
            ItemSpecificKpiContentFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
            ItemSpecificKpiContentFilter.Id = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.Id;
            ItemSpecificKpiContentFilter.ItemSpecificKpiId = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.ItemSpecificKpiId;
            ItemSpecificKpiContentFilter.ItemSpecificCriteriaId = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.ItemSpecificCriteriaId;
            ItemSpecificKpiContentFilter.ItemId = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.ItemId;
            ItemSpecificKpiContentFilter.Value = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.Value;

            List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);
            List<ItemSpecificCriteria_ItemSpecificKpiContentDTO> ItemSpecificCriteria_ItemSpecificKpiContentDTOs = ItemSpecificKpiContents
                .Select(x => new ItemSpecificCriteria_ItemSpecificKpiContentDTO(x)).ToList();
            return ItemSpecificCriteria_ItemSpecificKpiContentDTOs;
        }
        [Route(ItemSpecificCriteriaRoute.FilterListItem), HttpPost]
        public async Task<List<ItemSpecificCriteria_ItemDTO>> FilterListItem([FromBody] ItemSpecificCriteria_ItemFilterDTO ItemSpecificCriteria_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ItemSpecificCriteria_ItemFilterDTO.Id;
            ItemFilter.ProductId = ItemSpecificCriteria_ItemFilterDTO.ProductId;
            ItemFilter.Code = ItemSpecificCriteria_ItemFilterDTO.Code;
            ItemFilter.Name = ItemSpecificCriteria_ItemFilterDTO.Name;
            ItemFilter.ScanCode = ItemSpecificCriteria_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = ItemSpecificCriteria_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = ItemSpecificCriteria_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = ItemSpecificCriteria_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ItemSpecificCriteria_ItemDTO> ItemSpecificCriteria_ItemDTOs = Items
                .Select(x => new ItemSpecificCriteria_ItemDTO(x)).ToList();
            return ItemSpecificCriteria_ItemDTOs;
        }
        [Route(ItemSpecificCriteriaRoute.FilterListItemSpecificKpi), HttpPost]
        public async Task<List<ItemSpecificCriteria_ItemSpecificKpiDTO>> FilterListItemSpecificKpi([FromBody] ItemSpecificCriteria_ItemSpecificKpiFilterDTO ItemSpecificCriteria_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<ItemSpecificCriteria_ItemSpecificKpiDTO> ItemSpecificCriteria_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new ItemSpecificCriteria_ItemSpecificKpiDTO(x)).ToList();
            return ItemSpecificCriteria_ItemSpecificKpiDTOs;
        }

        [Route(ItemSpecificCriteriaRoute.SingleListItemSpecificKpiContent), HttpPost]
        public async Task<List<ItemSpecificCriteria_ItemSpecificKpiContentDTO>> SingleListItemSpecificKpiContent([FromBody] ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiContentFilter ItemSpecificKpiContentFilter = new ItemSpecificKpiContentFilter();
            ItemSpecificKpiContentFilter.Skip = 0;
            ItemSpecificKpiContentFilter.Take = 20;
            ItemSpecificKpiContentFilter.OrderBy = ItemSpecificKpiContentOrder.Id;
            ItemSpecificKpiContentFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiContentFilter.Selects = ItemSpecificKpiContentSelect.ALL;
            ItemSpecificKpiContentFilter.Id = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.Id;
            ItemSpecificKpiContentFilter.ItemSpecificKpiId = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.ItemSpecificKpiId;
            ItemSpecificKpiContentFilter.ItemSpecificCriteriaId = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.ItemSpecificCriteriaId;
            ItemSpecificKpiContentFilter.ItemId = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.ItemId;
            ItemSpecificKpiContentFilter.Value = ItemSpecificCriteria_ItemSpecificKpiContentFilterDTO.Value;

            List<ItemSpecificKpiContent> ItemSpecificKpiContents = await ItemSpecificKpiContentService.List(ItemSpecificKpiContentFilter);
            List<ItemSpecificCriteria_ItemSpecificKpiContentDTO> ItemSpecificCriteria_ItemSpecificKpiContentDTOs = ItemSpecificKpiContents
                .Select(x => new ItemSpecificCriteria_ItemSpecificKpiContentDTO(x)).ToList();
            return ItemSpecificCriteria_ItemSpecificKpiContentDTOs;
        }
        [Route(ItemSpecificCriteriaRoute.SingleListItem), HttpPost]
        public async Task<List<ItemSpecificCriteria_ItemDTO>> SingleListItem([FromBody] ItemSpecificCriteria_ItemFilterDTO ItemSpecificCriteria_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ItemSpecificCriteria_ItemFilterDTO.Id;
            ItemFilter.ProductId = ItemSpecificCriteria_ItemFilterDTO.ProductId;
            ItemFilter.Code = ItemSpecificCriteria_ItemFilterDTO.Code;
            ItemFilter.Name = ItemSpecificCriteria_ItemFilterDTO.Name;
            ItemFilter.ScanCode = ItemSpecificCriteria_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = ItemSpecificCriteria_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = ItemSpecificCriteria_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = ItemSpecificCriteria_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ItemSpecificCriteria_ItemDTO> ItemSpecificCriteria_ItemDTOs = Items
                .Select(x => new ItemSpecificCriteria_ItemDTO(x)).ToList();
            return ItemSpecificCriteria_ItemDTOs;
        }
        [Route(ItemSpecificCriteriaRoute.SingleListItemSpecificKpi), HttpPost]
        public async Task<List<ItemSpecificCriteria_ItemSpecificKpiDTO>> SingleListItemSpecificKpi([FromBody] ItemSpecificCriteria_ItemSpecificKpiFilterDTO ItemSpecificCriteria_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = ItemSpecificCriteria_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<ItemSpecificCriteria_ItemSpecificKpiDTO> ItemSpecificCriteria_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new ItemSpecificCriteria_ItemSpecificKpiDTO(x)).ToList();
            return ItemSpecificCriteria_ItemSpecificKpiDTOs;
        }

    }
}

