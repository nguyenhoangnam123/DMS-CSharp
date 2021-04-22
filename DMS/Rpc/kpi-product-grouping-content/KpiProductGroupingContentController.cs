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
using DMS.Services.MKpiProductGroupingContent;
using DMS.Services.MKpiProductGrouping;

namespace DMS.Rpc.kpi_product_grouping_content
{
    public partial class KpiProductGroupingContentController : RpcController
    {
        private IKpiProductGroupingService KpiProductGroupingService;
        private IKpiProductGroupingContentService KpiProductGroupingContentService;
        private ICurrentContext CurrentContext;
        public KpiProductGroupingContentController(
            IKpiProductGroupingService KpiProductGroupingService,
            IKpiProductGroupingContentService KpiProductGroupingContentService,
            ICurrentContext CurrentContext
        )
        {
            this.KpiProductGroupingService = KpiProductGroupingService;
            this.KpiProductGroupingContentService = KpiProductGroupingContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiProductGroupingContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiProductGroupingContent_KpiProductGroupingContentFilterDTO KpiProductGroupingContent_KpiProductGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingContentFilter KpiProductGroupingContentFilter = ConvertFilterDTOToFilterEntity(KpiProductGroupingContent_KpiProductGroupingContentFilterDTO);
            KpiProductGroupingContentFilter = await KpiProductGroupingContentService.ToFilter(KpiProductGroupingContentFilter);
            int count = await KpiProductGroupingContentService.Count(KpiProductGroupingContentFilter);
            return count;
        }

        [Route(KpiProductGroupingContentRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiProductGroupingContent_KpiProductGroupingContentDTO>>> List([FromBody] KpiProductGroupingContent_KpiProductGroupingContentFilterDTO KpiProductGroupingContent_KpiProductGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingContentFilter KpiProductGroupingContentFilter = ConvertFilterDTOToFilterEntity(KpiProductGroupingContent_KpiProductGroupingContentFilterDTO);
            KpiProductGroupingContentFilter = await KpiProductGroupingContentService.ToFilter(KpiProductGroupingContentFilter);
            List<KpiProductGroupingContent> KpiProductGroupingContents = await KpiProductGroupingContentService.List(KpiProductGroupingContentFilter);
            List<KpiProductGroupingContent_KpiProductGroupingContentDTO> KpiProductGroupingContent_KpiProductGroupingContentDTOs = KpiProductGroupingContents
                .Select(c => new KpiProductGroupingContent_KpiProductGroupingContentDTO(c)).ToList();
            return KpiProductGroupingContent_KpiProductGroupingContentDTOs;
        }

        [Route(KpiProductGroupingContentRoute.Get), HttpPost]
        public async Task<ActionResult<KpiProductGroupingContent_KpiProductGroupingContentDTO>> Get([FromBody]KpiProductGroupingContent_KpiProductGroupingContentDTO KpiProductGroupingContent_KpiProductGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGroupingContent_KpiProductGroupingContentDTO.Id))
                return Forbid();

            KpiProductGroupingContent KpiProductGroupingContent = await KpiProductGroupingContentService.Get(KpiProductGroupingContent_KpiProductGroupingContentDTO.Id);
            return new KpiProductGroupingContent_KpiProductGroupingContentDTO(KpiProductGroupingContent);
        }

        [Route(KpiProductGroupingContentRoute.Create), HttpPost]
        public async Task<ActionResult<KpiProductGroupingContent_KpiProductGroupingContentDTO>> Create([FromBody] KpiProductGroupingContent_KpiProductGroupingContentDTO KpiProductGroupingContent_KpiProductGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(KpiProductGroupingContent_KpiProductGroupingContentDTO.Id))
                return Forbid();

            KpiProductGroupingContent KpiProductGroupingContent = ConvertDTOToEntity(KpiProductGroupingContent_KpiProductGroupingContentDTO);
            KpiProductGroupingContent = await KpiProductGroupingContentService.Create(KpiProductGroupingContent);
            KpiProductGroupingContent_KpiProductGroupingContentDTO = new KpiProductGroupingContent_KpiProductGroupingContentDTO(KpiProductGroupingContent);
            if (KpiProductGroupingContent.IsValidated)
                return KpiProductGroupingContent_KpiProductGroupingContentDTO;
            else
                return BadRequest(KpiProductGroupingContent_KpiProductGroupingContentDTO);
        }

        [Route(KpiProductGroupingContentRoute.Update), HttpPost]
        public async Task<ActionResult<KpiProductGroupingContent_KpiProductGroupingContentDTO>> Update([FromBody] KpiProductGroupingContent_KpiProductGroupingContentDTO KpiProductGroupingContent_KpiProductGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(KpiProductGroupingContent_KpiProductGroupingContentDTO.Id))
                return Forbid();

            KpiProductGroupingContent KpiProductGroupingContent = ConvertDTOToEntity(KpiProductGroupingContent_KpiProductGroupingContentDTO);
            KpiProductGroupingContent = await KpiProductGroupingContentService.Update(KpiProductGroupingContent);
            KpiProductGroupingContent_KpiProductGroupingContentDTO = new KpiProductGroupingContent_KpiProductGroupingContentDTO(KpiProductGroupingContent);
            if (KpiProductGroupingContent.IsValidated)
                return KpiProductGroupingContent_KpiProductGroupingContentDTO;
            else
                return BadRequest(KpiProductGroupingContent_KpiProductGroupingContentDTO);
        }

        [Route(KpiProductGroupingContentRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiProductGroupingContent_KpiProductGroupingContentDTO>> Delete([FromBody] KpiProductGroupingContent_KpiProductGroupingContentDTO KpiProductGroupingContent_KpiProductGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGroupingContent_KpiProductGroupingContentDTO.Id))
                return Forbid();

            KpiProductGroupingContent KpiProductGroupingContent = ConvertDTOToEntity(KpiProductGroupingContent_KpiProductGroupingContentDTO);
            KpiProductGroupingContent = await KpiProductGroupingContentService.Delete(KpiProductGroupingContent);
            KpiProductGroupingContent_KpiProductGroupingContentDTO = new KpiProductGroupingContent_KpiProductGroupingContentDTO(KpiProductGroupingContent);
            if (KpiProductGroupingContent.IsValidated)
                return KpiProductGroupingContent_KpiProductGroupingContentDTO;
            else
                return BadRequest(KpiProductGroupingContent_KpiProductGroupingContentDTO);
        }
        
        [Route(KpiProductGroupingContentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingContentFilter KpiProductGroupingContentFilter = new KpiProductGroupingContentFilter();
            KpiProductGroupingContentFilter = await KpiProductGroupingContentService.ToFilter(KpiProductGroupingContentFilter);
            KpiProductGroupingContentFilter.Id = new IdFilter { In = Ids };
            KpiProductGroupingContentFilter.Selects = KpiProductGroupingContentSelect.Id;
            KpiProductGroupingContentFilter.Skip = 0;
            KpiProductGroupingContentFilter.Take = int.MaxValue;

            List<KpiProductGroupingContent> KpiProductGroupingContents = await KpiProductGroupingContentService.List(KpiProductGroupingContentFilter);
            KpiProductGroupingContents = await KpiProductGroupingContentService.BulkDelete(KpiProductGroupingContents);
            if (KpiProductGroupingContents.Any(x => !x.IsValidated))
                return BadRequest(KpiProductGroupingContents.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(KpiProductGroupingContentRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiProductGroupingSelect.ALL
            };
            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);
            List<KpiProductGroupingContent> KpiProductGroupingContents = new List<KpiProductGroupingContent>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(KpiProductGroupingContents);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int KpiProductGroupingIdColumn = 1 + StartColumn;
                int ProductGroupingIdColumn = 2 + StartColumn;
                int RowIdColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string KpiProductGroupingIdValue = worksheet.Cells[i + StartRow, KpiProductGroupingIdColumn].Value?.ToString();
                    string ProductGroupingIdValue = worksheet.Cells[i + StartRow, ProductGroupingIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    
                    KpiProductGroupingContent KpiProductGroupingContent = new KpiProductGroupingContent();
                    KpiProductGrouping KpiProductGrouping = KpiProductGroupings.Where(x => x.Id.ToString() == KpiProductGroupingIdValue).FirstOrDefault();
                    KpiProductGroupingContent.KpiProductGroupingId = KpiProductGrouping == null ? 0 : KpiProductGrouping.Id;
                    KpiProductGroupingContent.KpiProductGrouping = KpiProductGrouping;
                    
                    KpiProductGroupingContents.Add(KpiProductGroupingContent);
                }
            }
            KpiProductGroupingContents = await KpiProductGroupingContentService.Import(KpiProductGroupingContents);
            if (KpiProductGroupingContents.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < KpiProductGroupingContents.Count; i++)
                {
                    KpiProductGroupingContent KpiProductGroupingContent = KpiProductGroupingContents[i];
                    if (!KpiProductGroupingContent.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (KpiProductGroupingContent.Errors.ContainsKey(nameof(KpiProductGroupingContent.Id)))
                            Error += KpiProductGroupingContent.Errors[nameof(KpiProductGroupingContent.Id)];
                        if (KpiProductGroupingContent.Errors.ContainsKey(nameof(KpiProductGroupingContent.KpiProductGroupingId)))
                            Error += KpiProductGroupingContent.Errors[nameof(KpiProductGroupingContent.KpiProductGroupingId)];
                        if (KpiProductGroupingContent.Errors.ContainsKey(nameof(KpiProductGroupingContent.ProductGroupingId)))
                            Error += KpiProductGroupingContent.Errors[nameof(KpiProductGroupingContent.ProductGroupingId)];
                        if (KpiProductGroupingContent.Errors.ContainsKey(nameof(KpiProductGroupingContent.RowId)))
                            Error += KpiProductGroupingContent.Errors[nameof(KpiProductGroupingContent.RowId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(KpiProductGroupingContentRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiProductGroupingContent_KpiProductGroupingContentFilterDTO KpiProductGroupingContent_KpiProductGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiProductGroupingContent
                var KpiProductGroupingContentFilter = ConvertFilterDTOToFilterEntity(KpiProductGroupingContent_KpiProductGroupingContentFilterDTO);
                KpiProductGroupingContentFilter.Skip = 0;
                KpiProductGroupingContentFilter.Take = int.MaxValue;
                KpiProductGroupingContentFilter = await KpiProductGroupingContentService.ToFilter(KpiProductGroupingContentFilter);
                List<KpiProductGroupingContent> KpiProductGroupingContents = await KpiProductGroupingContentService.List(KpiProductGroupingContentFilter);

                var KpiProductGroupingContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "KpiProductGroupingId",
                        "ProductGroupingId",
                        "RowId",
                    }
                };
                List<object[]> KpiProductGroupingContentData = new List<object[]>();
                for (int i = 0; i < KpiProductGroupingContents.Count; i++)
                {
                    var KpiProductGroupingContent = KpiProductGroupingContents[i];
                    KpiProductGroupingContentData.Add(new Object[]
                    {
                        KpiProductGroupingContent.Id,
                        KpiProductGroupingContent.KpiProductGroupingId,
                        KpiProductGroupingContent.ProductGroupingId,
                        KpiProductGroupingContent.RowId,
                    });
                }
                excel.GenerateWorksheet("KpiProductGroupingContent", KpiProductGroupingContentHeaders, KpiProductGroupingContentData);
                #endregion
                
                #region KpiProductGrouping
                var KpiProductGroupingFilter = new KpiProductGroupingFilter();
                KpiProductGroupingFilter.Selects = KpiProductGroupingSelect.ALL;
                KpiProductGroupingFilter.OrderBy = KpiProductGroupingOrder.Id;
                KpiProductGroupingFilter.OrderType = OrderType.ASC;
                KpiProductGroupingFilter.Skip = 0;
                KpiProductGroupingFilter.Take = int.MaxValue;
                List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);

                var KpiProductGroupingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "KpiYearId",
                        "KpiPeriodId",
                        "KpiProductGroupingTypeId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                        "RowId",
                    }
                };
                List<object[]> KpiProductGroupingData = new List<object[]>();
                for (int i = 0; i < KpiProductGroupings.Count; i++)
                {
                    var KpiProductGrouping = KpiProductGroupings[i];
                    KpiProductGroupingData.Add(new Object[]
                    {
                        KpiProductGrouping.Id,
                        KpiProductGrouping.OrganizationId,
                        KpiProductGrouping.KpiYearId,
                        KpiProductGrouping.KpiPeriodId,
                        KpiProductGrouping.KpiProductGroupingTypeId,
                        KpiProductGrouping.StatusId,
                        KpiProductGrouping.EmployeeId,
                        KpiProductGrouping.CreatorId,
                        KpiProductGrouping.RowId,
                    });
                }
                excel.GenerateWorksheet("KpiProductGrouping", KpiProductGroupingHeaders, KpiProductGroupingData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiProductGroupingContent.xlsx");
        }

        [Route(KpiProductGroupingContentRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] KpiProductGroupingContent_KpiProductGroupingContentFilterDTO KpiProductGroupingContent_KpiProductGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/KpiProductGroupingContent_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "KpiProductGroupingContent.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiProductGroupingContentFilter KpiProductGroupingContentFilter = new KpiProductGroupingContentFilter();
            KpiProductGroupingContentFilter = await KpiProductGroupingContentService.ToFilter(KpiProductGroupingContentFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiProductGroupingContentFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiProductGroupingContentService.Count(KpiProductGroupingContentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiProductGroupingContent ConvertDTOToEntity(KpiProductGroupingContent_KpiProductGroupingContentDTO KpiProductGroupingContent_KpiProductGroupingContentDTO)
        {
            KpiProductGroupingContent KpiProductGroupingContent = new KpiProductGroupingContent();
            KpiProductGroupingContent.Id = KpiProductGroupingContent_KpiProductGroupingContentDTO.Id;
            KpiProductGroupingContent.KpiProductGroupingId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGroupingId;
            KpiProductGroupingContent.ProductGroupingId = KpiProductGroupingContent_KpiProductGroupingContentDTO.ProductGroupingId;
            KpiProductGroupingContent.RowId = KpiProductGroupingContent_KpiProductGroupingContentDTO.RowId;
            KpiProductGroupingContent.KpiProductGrouping = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping == null ? null : new KpiProductGrouping
            {
                Id = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.Id,
                OrganizationId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.OrganizationId,
                KpiYearId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.KpiYearId,
                KpiPeriodId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.KpiPeriodId,
                KpiProductGroupingTypeId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.KpiProductGroupingTypeId,
                StatusId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.StatusId,
                EmployeeId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.EmployeeId,
                CreatorId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.CreatorId,
                RowId = KpiProductGroupingContent_KpiProductGroupingContentDTO.KpiProductGrouping.RowId,
            };
            KpiProductGroupingContent.BaseLanguage = CurrentContext.Language;
            return KpiProductGroupingContent;
        }

        private KpiProductGroupingContentFilter ConvertFilterDTOToFilterEntity(KpiProductGroupingContent_KpiProductGroupingContentFilterDTO KpiProductGroupingContent_KpiProductGroupingContentFilterDTO)
        {
            KpiProductGroupingContentFilter KpiProductGroupingContentFilter = new KpiProductGroupingContentFilter();
            KpiProductGroupingContentFilter.Selects = KpiProductGroupingContentSelect.ALL;
            KpiProductGroupingContentFilter.Skip = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.Skip;
            KpiProductGroupingContentFilter.Take = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.Take;
            KpiProductGroupingContentFilter.OrderBy = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.OrderBy;
            KpiProductGroupingContentFilter.OrderType = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.OrderType;

            KpiProductGroupingContentFilter.Id = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.Id;
            KpiProductGroupingContentFilter.KpiProductGroupingId = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.KpiProductGroupingId;
            KpiProductGroupingContentFilter.ProductGroupingId = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.ProductGroupingId;
            KpiProductGroupingContentFilter.RowId = KpiProductGroupingContent_KpiProductGroupingContentFilterDTO.RowId;
            return KpiProductGroupingContentFilter;
        }
    }
}

