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
using DMS.Services.MTotalItemSpecificCriteria;
using DMS.Services.MItemSpecificKpi;

namespace DMS.Rpc.total_item_specific_criteria
{
    public class TotalItemSpecificCriteriaController : RpcController
    {
        private IItemSpecificKpiService ItemSpecificKpiService;
        private ITotalItemSpecificCriteriaService TotalItemSpecificCriteriaService;
        private ICurrentContext CurrentContext;
        public TotalItemSpecificCriteriaController(
            IItemSpecificKpiService ItemSpecificKpiService,
            ITotalItemSpecificCriteriaService TotalItemSpecificCriteriaService,
            ICurrentContext CurrentContext
        )
        {
            this.ItemSpecificKpiService = ItemSpecificKpiService;
            this.TotalItemSpecificCriteriaService = TotalItemSpecificCriteriaService;
            this.CurrentContext = CurrentContext;
        }

        [Route(TotalItemSpecificCriteriaRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter = ConvertFilterDTOToFilterEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO);
            TotalItemSpecificCriteriaFilter = TotalItemSpecificCriteriaService.ToFilter(TotalItemSpecificCriteriaFilter);
            int count = await TotalItemSpecificCriteriaService.Count(TotalItemSpecificCriteriaFilter);
            return count;
        }

        [Route(TotalItemSpecificCriteriaRoute.List), HttpPost]
        public async Task<ActionResult<List<TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO>>> List([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter = ConvertFilterDTOToFilterEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO);
            TotalItemSpecificCriteriaFilter = TotalItemSpecificCriteriaService.ToFilter(TotalItemSpecificCriteriaFilter);
            List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.List(TotalItemSpecificCriteriaFilter);
            List<TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO> TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTOs = TotalItemSpecificCriterias
                .Select(c => new TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO(c)).ToList();
            return TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTOs;
        }

        [Route(TotalItemSpecificCriteriaRoute.Get), HttpPost]
        public async Task<ActionResult<TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO>> Get([FromBody]TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Id))
                return Forbid();

            TotalItemSpecificCriteria TotalItemSpecificCriteria = await TotalItemSpecificCriteriaService.Get(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Id);
            return new TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteria);
        }

        [Route(TotalItemSpecificCriteriaRoute.Create), HttpPost]
        public async Task<ActionResult<TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO>> Create([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Id))
                return Forbid();

            TotalItemSpecificCriteria TotalItemSpecificCriteria = ConvertDTOToEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO);
            TotalItemSpecificCriteria = await TotalItemSpecificCriteriaService.Create(TotalItemSpecificCriteria);
            TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO = new TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteria);
            if (TotalItemSpecificCriteria.IsValidated)
                return TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO;
            else
                return BadRequest(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO);
        }

        [Route(TotalItemSpecificCriteriaRoute.Update), HttpPost]
        public async Task<ActionResult<TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO>> Update([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Id))
                return Forbid();

            TotalItemSpecificCriteria TotalItemSpecificCriteria = ConvertDTOToEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO);
            TotalItemSpecificCriteria = await TotalItemSpecificCriteriaService.Update(TotalItemSpecificCriteria);
            TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO = new TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteria);
            if (TotalItemSpecificCriteria.IsValidated)
                return TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO;
            else
                return BadRequest(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO);
        }

        [Route(TotalItemSpecificCriteriaRoute.Delete), HttpPost]
        public async Task<ActionResult<TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO>> Delete([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Id))
                return Forbid();

            TotalItemSpecificCriteria TotalItemSpecificCriteria = ConvertDTOToEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO);
            TotalItemSpecificCriteria = await TotalItemSpecificCriteriaService.Delete(TotalItemSpecificCriteria);
            TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO = new TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteria);
            if (TotalItemSpecificCriteria.IsValidated)
                return TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO;
            else
                return BadRequest(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO);
        }
        
        [Route(TotalItemSpecificCriteriaRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter = new TotalItemSpecificCriteriaFilter();
            TotalItemSpecificCriteriaFilter = TotalItemSpecificCriteriaService.ToFilter(TotalItemSpecificCriteriaFilter);
            TotalItemSpecificCriteriaFilter.Id = new IdFilter { In = Ids };
            TotalItemSpecificCriteriaFilter.Selects = TotalItemSpecificCriteriaSelect.Id;
            TotalItemSpecificCriteriaFilter.Skip = 0;
            TotalItemSpecificCriteriaFilter.Take = int.MaxValue;

            List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.List(TotalItemSpecificCriteriaFilter);
            TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.BulkDelete(TotalItemSpecificCriterias);
            return true;
        }
        
        [Route(TotalItemSpecificCriteriaRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = new List<TotalItemSpecificCriteria>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(TotalItemSpecificCriterias);
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
                    
                    TotalItemSpecificCriteria TotalItemSpecificCriteria = new TotalItemSpecificCriteria();
                    TotalItemSpecificCriteria.Code = CodeValue;
                    TotalItemSpecificCriteria.Name = NameValue;
                    
                    TotalItemSpecificCriterias.Add(TotalItemSpecificCriteria);
                }
            }
            TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.Import(TotalItemSpecificCriterias);
            if (TotalItemSpecificCriterias.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < TotalItemSpecificCriterias.Count; i++)
                {
                    TotalItemSpecificCriteria TotalItemSpecificCriteria = TotalItemSpecificCriterias[i];
                    if (!TotalItemSpecificCriteria.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (TotalItemSpecificCriteria.Errors.ContainsKey(nameof(TotalItemSpecificCriteria.Id)))
                            Error += TotalItemSpecificCriteria.Errors[nameof(TotalItemSpecificCriteria.Id)];
                        if (TotalItemSpecificCriteria.Errors.ContainsKey(nameof(TotalItemSpecificCriteria.Code)))
                            Error += TotalItemSpecificCriteria.Errors[nameof(TotalItemSpecificCriteria.Code)];
                        if (TotalItemSpecificCriteria.Errors.ContainsKey(nameof(TotalItemSpecificCriteria.Name)))
                            Error += TotalItemSpecificCriteria.Errors[nameof(TotalItemSpecificCriteria.Name)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(TotalItemSpecificCriteriaRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region TotalItemSpecificCriteria
                var TotalItemSpecificCriteriaFilter = ConvertFilterDTOToFilterEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO);
                TotalItemSpecificCriteriaFilter.Skip = 0;
                TotalItemSpecificCriteriaFilter.Take = int.MaxValue;
                TotalItemSpecificCriteriaFilter = TotalItemSpecificCriteriaService.ToFilter(TotalItemSpecificCriteriaFilter);
                List<TotalItemSpecificCriteria> TotalItemSpecificCriterias = await TotalItemSpecificCriteriaService.List(TotalItemSpecificCriteriaFilter);

                var TotalItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> TotalItemSpecificCriteriaData = new List<object[]>();
                for (int i = 0; i < TotalItemSpecificCriterias.Count; i++)
                {
                    var TotalItemSpecificCriteria = TotalItemSpecificCriterias[i];
                    TotalItemSpecificCriteriaData.Add(new Object[]
                    {
                        TotalItemSpecificCriteria.Id,
                        TotalItemSpecificCriteria.Code,
                        TotalItemSpecificCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("TotalItemSpecificCriteria", TotalItemSpecificCriteriaHeaders, TotalItemSpecificCriteriaData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "TotalItemSpecificCriteria.xlsx");
        }

        [Route(TotalItemSpecificCriteriaRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region TotalItemSpecificCriteria
                var TotalItemSpecificCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> TotalItemSpecificCriteriaData = new List<object[]>();
                excel.GenerateWorksheet("TotalItemSpecificCriteria", TotalItemSpecificCriteriaHeaders, TotalItemSpecificCriteriaData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "TotalItemSpecificCriteria.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter = new TotalItemSpecificCriteriaFilter();
            TotalItemSpecificCriteriaFilter = TotalItemSpecificCriteriaService.ToFilter(TotalItemSpecificCriteriaFilter);
            if (Id == 0)
            {

            }
            else
            {
                TotalItemSpecificCriteriaFilter.Id = new IdFilter { Equal = Id };
                int count = await TotalItemSpecificCriteriaService.Count(TotalItemSpecificCriteriaFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private TotalItemSpecificCriteria ConvertDTOToEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO)
        {
            TotalItemSpecificCriteria TotalItemSpecificCriteria = new TotalItemSpecificCriteria();
            TotalItemSpecificCriteria.Id = TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Id;
            TotalItemSpecificCriteria.Code = TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Code;
            TotalItemSpecificCriteria.Name = TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.Name;
            TotalItemSpecificCriteria.ItemSpecificKpiTotalItemSpecificCriteriaMappings = TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO.ItemSpecificKpiTotalItemSpecificCriteriaMappings?
                .Select(x => new ItemSpecificKpiTotalItemSpecificCriteriaMapping
                {
                    ItemSpecificKpiId = x.ItemSpecificKpiId,
                    Value = x.Value,
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
            TotalItemSpecificCriteria.BaseLanguage = CurrentContext.Language;
            return TotalItemSpecificCriteria;
        }

        private TotalItemSpecificCriteriaFilter ConvertFilterDTOToFilterEntity(TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO)
        {
            TotalItemSpecificCriteriaFilter TotalItemSpecificCriteriaFilter = new TotalItemSpecificCriteriaFilter();
            TotalItemSpecificCriteriaFilter.Selects = TotalItemSpecificCriteriaSelect.ALL;
            TotalItemSpecificCriteriaFilter.Skip = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.Skip;
            TotalItemSpecificCriteriaFilter.Take = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.Take;
            TotalItemSpecificCriteriaFilter.OrderBy = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.OrderBy;
            TotalItemSpecificCriteriaFilter.OrderType = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.OrderType;

            TotalItemSpecificCriteriaFilter.Id = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.Id;
            TotalItemSpecificCriteriaFilter.Code = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.Code;
            TotalItemSpecificCriteriaFilter.Name = TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO.Name;
            return TotalItemSpecificCriteriaFilter;
        }

        [Route(TotalItemSpecificCriteriaRoute.FilterListItemSpecificKpi), HttpPost]
        public async Task<List<TotalItemSpecificCriteria_ItemSpecificKpiDTO>> FilterListItemSpecificKpi([FromBody] TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<TotalItemSpecificCriteria_ItemSpecificKpiDTO> TotalItemSpecificCriteria_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new TotalItemSpecificCriteria_ItemSpecificKpiDTO(x)).ToList();
            return TotalItemSpecificCriteria_ItemSpecificKpiDTOs;
        }

        [Route(TotalItemSpecificCriteriaRoute.SingleListItemSpecificKpi), HttpPost]
        public async Task<List<TotalItemSpecificCriteria_ItemSpecificKpiDTO>> SingleListItemSpecificKpi([FromBody] TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = 0;
            ItemSpecificKpiFilter.Take = 20;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<TotalItemSpecificCriteria_ItemSpecificKpiDTO> TotalItemSpecificCriteria_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new TotalItemSpecificCriteria_ItemSpecificKpiDTO(x)).ToList();
            return TotalItemSpecificCriteria_ItemSpecificKpiDTOs;
        }

        [Route(TotalItemSpecificCriteriaRoute.CountItemSpecificKpi), HttpPost]
        public async Task<long> CountItemSpecificKpi([FromBody] TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Id = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.CreatorId;

            return await ItemSpecificKpiService.Count(ItemSpecificKpiFilter);
        }

        [Route(TotalItemSpecificCriteriaRoute.ListItemSpecificKpi), HttpPost]
        public async Task<List<TotalItemSpecificCriteria_ItemSpecificKpiDTO>> ListItemSpecificKpi([FromBody] TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemSpecificKpiFilter ItemSpecificKpiFilter = new ItemSpecificKpiFilter();
            ItemSpecificKpiFilter.Skip = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.Skip;
            ItemSpecificKpiFilter.Take = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.Take;
            ItemSpecificKpiFilter.OrderBy = ItemSpecificKpiOrder.Id;
            ItemSpecificKpiFilter.OrderType = OrderType.ASC;
            ItemSpecificKpiFilter.Selects = ItemSpecificKpiSelect.ALL;
            ItemSpecificKpiFilter.Id = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.Id;
            ItemSpecificKpiFilter.OrganizationId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.OrganizationId;
            ItemSpecificKpiFilter.KpiPeriodId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.KpiPeriodId;
            ItemSpecificKpiFilter.StatusId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.StatusId;
            ItemSpecificKpiFilter.EmployeeId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.EmployeeId;
            ItemSpecificKpiFilter.CreatorId = TotalItemSpecificCriteria_ItemSpecificKpiFilterDTO.CreatorId;

            List<ItemSpecificKpi> ItemSpecificKpis = await ItemSpecificKpiService.List(ItemSpecificKpiFilter);
            List<TotalItemSpecificCriteria_ItemSpecificKpiDTO> TotalItemSpecificCriteria_ItemSpecificKpiDTOs = ItemSpecificKpis
                .Select(x => new TotalItemSpecificCriteria_ItemSpecificKpiDTO(x)).ToList();
            return TotalItemSpecificCriteria_ItemSpecificKpiDTOs;
        }
    }
}

