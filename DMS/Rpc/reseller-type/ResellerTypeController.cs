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
using DMS.Services.MResellerType;
using DMS.Services.MStatus;

namespace DMS.Rpc.reseller_type
{
    public class ResellerTypeRoute : Root
    {
        public const string Master = Module + "/reseller-type/reseller-type-master";
        public const string Detail = Module + "/reseller-type/reseller-type-detail";
        private const string Default = Rpc + Module + "/reseller-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ResellerTypeFilter.Id), FieldType.ID },
            { nameof(ResellerTypeFilter.Code), FieldType.STRING },
            { nameof(ResellerTypeFilter.Name), FieldType.STRING },
            { nameof(ResellerTypeFilter.StatusId), FieldType.ID },
        };
    }

    public class ResellerTypeController : RpcController
    {
        private IResellerTypeService ResellerTypeService; 
        private IStatusService StatusService;
        private ICurrentContext CurrentContext;
        public ResellerTypeController(
            IResellerTypeService ResellerTypeService,
            IStatusService StatusService,
            ICurrentContext CurrentContext
        )
        {
            this.ResellerTypeService = ResellerTypeService;
            this.StatusService = StatusService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ResellerTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ResellerType_ResellerTypeFilterDTO ResellerType_ResellerTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ResellerTypeFilter ResellerTypeFilter = ConvertFilterDTOToFilterEntity(ResellerType_ResellerTypeFilterDTO);
            ResellerTypeFilter = ResellerTypeService.ToFilter(ResellerTypeFilter);
            int count = await ResellerTypeService.Count(ResellerTypeFilter);
            return count;
        }

        [Route(ResellerTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<ResellerType_ResellerTypeDTO>>> List([FromBody] ResellerType_ResellerTypeFilterDTO ResellerType_ResellerTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ResellerTypeFilter ResellerTypeFilter = ConvertFilterDTOToFilterEntity(ResellerType_ResellerTypeFilterDTO);
            ResellerTypeFilter = ResellerTypeService.ToFilter(ResellerTypeFilter);
            List<ResellerType> ResellerTypes = await ResellerTypeService.List(ResellerTypeFilter);
            List<ResellerType_ResellerTypeDTO> ResellerType_ResellerTypeDTOs = ResellerTypes
                .Select(c => new ResellerType_ResellerTypeDTO(c)).ToList();
            return ResellerType_ResellerTypeDTOs;
        }

        [Route(ResellerTypeRoute.Get), HttpPost]
        public async Task<ActionResult<ResellerType_ResellerTypeDTO>> Get([FromBody]ResellerType_ResellerTypeDTO ResellerType_ResellerTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ResellerType_ResellerTypeDTO.Id))
                return Forbid();

            ResellerType ResellerType = await ResellerTypeService.Get(ResellerType_ResellerTypeDTO.Id);
            return new ResellerType_ResellerTypeDTO(ResellerType);
        }

        [Route(ResellerTypeRoute.Create), HttpPost]
        public async Task<ActionResult<ResellerType_ResellerTypeDTO>> Create([FromBody] ResellerType_ResellerTypeDTO ResellerType_ResellerTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ResellerType_ResellerTypeDTO.Id))
                return Forbid();

            ResellerType ResellerType = ConvertDTOToEntity(ResellerType_ResellerTypeDTO);
            ResellerType = await ResellerTypeService.Create(ResellerType);
            ResellerType_ResellerTypeDTO = new ResellerType_ResellerTypeDTO(ResellerType);
            if (ResellerType.IsValidated)
                return ResellerType_ResellerTypeDTO;
            else
                return BadRequest(ResellerType_ResellerTypeDTO);
        }

        [Route(ResellerTypeRoute.Update), HttpPost]
        public async Task<ActionResult<ResellerType_ResellerTypeDTO>> Update([FromBody] ResellerType_ResellerTypeDTO ResellerType_ResellerTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ResellerType_ResellerTypeDTO.Id))
                return Forbid();

            ResellerType ResellerType = ConvertDTOToEntity(ResellerType_ResellerTypeDTO);
            ResellerType = await ResellerTypeService.Update(ResellerType);
            ResellerType_ResellerTypeDTO = new ResellerType_ResellerTypeDTO(ResellerType);
            if (ResellerType.IsValidated)
                return ResellerType_ResellerTypeDTO;
            else
                return BadRequest(ResellerType_ResellerTypeDTO);
        }

        [Route(ResellerTypeRoute.Delete), HttpPost]
        public async Task<ActionResult<ResellerType_ResellerTypeDTO>> Delete([FromBody] ResellerType_ResellerTypeDTO ResellerType_ResellerTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ResellerType_ResellerTypeDTO.Id))
                return Forbid();

            ResellerType ResellerType = ConvertDTOToEntity(ResellerType_ResellerTypeDTO);
            ResellerType = await ResellerTypeService.Delete(ResellerType);
            ResellerType_ResellerTypeDTO = new ResellerType_ResellerTypeDTO(ResellerType);
            if (ResellerType.IsValidated)
                return ResellerType_ResellerTypeDTO;
            else
                return BadRequest(ResellerType_ResellerTypeDTO);
        }
        
        [Route(ResellerTypeRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter();
            ResellerTypeFilter = ResellerTypeService.ToFilter(ResellerTypeFilter);
            ResellerTypeFilter.Id = new IdFilter { In = Ids };
            ResellerTypeFilter.Selects = ResellerTypeSelect.Id;
            ResellerTypeFilter.Skip = 0;
            ResellerTypeFilter.Take = int.MaxValue;

            List<ResellerType> ResellerTypes = await ResellerTypeService.List(ResellerTypeFilter);
            ResellerTypes = await ResellerTypeService.BulkDelete(ResellerTypes);
            return true;
        }
        
        [Route(ResellerTypeRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<ResellerType> ResellerTypes = new List<ResellerType>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ResellerTypes);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                int DeteledAtColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string DeteledAtValue = worksheet.Cells[i + StartRow, DeteledAtColumn].Value?.ToString();
                    
                    ResellerType ResellerType = new ResellerType();
                    ResellerType.Code = CodeValue;
                    ResellerType.Name = NameValue;
                    ResellerTypes.Add(ResellerType);
                }
            }
            ResellerTypes = await ResellerTypeService.Import(ResellerTypes);
            if (ResellerTypes.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ResellerTypes.Count; i++)
                {
                    ResellerType ResellerType = ResellerTypes[i];
                    if (!ResellerType.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ResellerType.Errors.ContainsKey(nameof(ResellerType.Id)))
                            Error += ResellerType.Errors[nameof(ResellerType.Id)];
                        if (ResellerType.Errors.ContainsKey(nameof(ResellerType.Code)))
                            Error += ResellerType.Errors[nameof(ResellerType.Code)];
                        if (ResellerType.Errors.ContainsKey(nameof(ResellerType.Name)))
                            Error += ResellerType.Errors[nameof(ResellerType.Name)];
                        if (ResellerType.Errors.ContainsKey(nameof(ResellerType.StatusId)))
                            Error += ResellerType.Errors[nameof(ResellerType.StatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ResellerTypeRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] ResellerType_ResellerTypeFilterDTO ResellerType_ResellerTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var ResellerTypeFilter = ConvertFilterDTOToFilterEntity(ResellerType_ResellerTypeFilterDTO);
            ResellerTypeFilter.Skip = 0;
            ResellerTypeFilter.Take = int.MaxValue;
            ResellerTypeFilter = ResellerTypeService.ToFilter(ResellerTypeFilter);

            List<ResellerType> ResellerTypes = await ResellerTypeService.List(ResellerTypeFilter);
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var ResellerTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                        "DeteledAt",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < ResellerTypes.Count; i++)
                {
                    var ResellerType = ResellerTypes[i];
                    data.Add(new Object[]
                    {
                        ResellerType.Id,
                        ResellerType.Code,
                        ResellerType.Name,
                        ResellerType.StatusId,
                    });
                    excel.GenerateWorksheet("ResellerType", ResellerTypeHeaders, data);
                }
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ResellerType.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter();
            ResellerTypeFilter = ResellerTypeService.ToFilter(ResellerTypeFilter);
            if (Id == 0)
            {

            }
            else
            {
                ResellerTypeFilter.Id = new IdFilter { Equal = Id };
                int count = await ResellerTypeService.Count(ResellerTypeFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ResellerType ConvertDTOToEntity(ResellerType_ResellerTypeDTO ResellerType_ResellerTypeDTO)
        {
            ResellerType ResellerType = new ResellerType();
            ResellerType.Id = ResellerType_ResellerTypeDTO.Id;
            ResellerType.Code = ResellerType_ResellerTypeDTO.Code;
            ResellerType.Name = ResellerType_ResellerTypeDTO.Name;
            ResellerType.StatusId = ResellerType_ResellerTypeDTO.StatusId;
            ResellerType.BaseLanguage = CurrentContext.Language;
            return ResellerType;
        }

        private ResellerTypeFilter ConvertFilterDTOToFilterEntity(ResellerType_ResellerTypeFilterDTO ResellerType_ResellerTypeFilterDTO)
        {
            ResellerTypeFilter ResellerTypeFilter = new ResellerTypeFilter();
            ResellerTypeFilter.Selects = ResellerTypeSelect.ALL;
            ResellerTypeFilter.Skip = ResellerType_ResellerTypeFilterDTO.Skip;
            ResellerTypeFilter.Take = ResellerType_ResellerTypeFilterDTO.Take;
            ResellerTypeFilter.OrderBy = ResellerType_ResellerTypeFilterDTO.OrderBy;
            ResellerTypeFilter.OrderType = ResellerType_ResellerTypeFilterDTO.OrderType;

            ResellerTypeFilter.Id = ResellerType_ResellerTypeFilterDTO.Id;
            ResellerTypeFilter.Code = ResellerType_ResellerTypeFilterDTO.Code;
            ResellerTypeFilter.Name = ResellerType_ResellerTypeFilterDTO.Name;
            ResellerTypeFilter.StatusId = ResellerType_ResellerTypeFilterDTO.StatusId;
            return ResellerTypeFilter;
        }

        public async Task<List<ResellerType_StatusDTO>> SingleListStatus([FromBody] ResellerType_StatusFilterDTO ResellerType_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = ResellerType_StatusFilterDTO.Id;
            StatusFilter.Code = ResellerType_StatusFilterDTO.Code;
            StatusFilter.Name = ResellerType_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ResellerType_StatusDTO> ResellerType_StatusDTOs = Statuses
                .Select(x => new ResellerType_StatusDTO(x)).ToList();
            return ResellerType_StatusDTOs;
        }
    }
}

