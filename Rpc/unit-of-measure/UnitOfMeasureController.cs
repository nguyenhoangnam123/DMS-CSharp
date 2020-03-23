using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MStatus;

namespace DMS.Rpc.unit_of_measure
{
    public class UnitOfMeasureRoute : Root
    {
        public const string Master = Module + "/unit-of-measure/unit-of-measure-master";
        public const string Detail = Module + "/unit-of-measure/unit-of-measure-detail";
        private const string Default = Rpc + Module + "/unit-of-measure";
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
            { nameof(UnitOfMeasureFilter.Id), FieldType.ID },
            { nameof(UnitOfMeasureFilter.Code), FieldType.STRING },
            { nameof(UnitOfMeasureFilter.Name), FieldType.STRING },
            { nameof(UnitOfMeasureFilter.Description), FieldType.STRING },
            { nameof(UnitOfMeasureFilter.StatusId), FieldType.ID },
        };
    }

    public class UnitOfMeasureController : RpcController
    {
        private IStatusService StatusService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private ICurrentContext CurrentContext;
        public UnitOfMeasureController(
            IStatusService StatusService,
            IUnitOfMeasureService UnitOfMeasureService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.CurrentContext = CurrentContext;
        }

        [Route(UnitOfMeasureRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] UnitOfMeasure_UnitOfMeasureFilterDTO UnitOfMeasure_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasure_UnitOfMeasureFilterDTO);
            UnitOfMeasureFilter = UnitOfMeasureService.ToFilter(UnitOfMeasureFilter);
            int count = await UnitOfMeasureService.Count(UnitOfMeasureFilter);
            return count;
        }

        [Route(UnitOfMeasureRoute.List), HttpPost]
        public async Task<ActionResult<List<UnitOfMeasure_UnitOfMeasureDTO>>> List([FromBody] UnitOfMeasure_UnitOfMeasureFilterDTO UnitOfMeasure_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasure_UnitOfMeasureFilterDTO);
            UnitOfMeasureFilter = UnitOfMeasureService.ToFilter(UnitOfMeasureFilter);
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<UnitOfMeasure_UnitOfMeasureDTO> UnitOfMeasure_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(c => new UnitOfMeasure_UnitOfMeasureDTO(c)).ToList();
            return UnitOfMeasure_UnitOfMeasureDTOs;
        }

        [Route(UnitOfMeasureRoute.Get), HttpPost]
        public async Task<ActionResult<UnitOfMeasure_UnitOfMeasureDTO>> Get([FromBody]UnitOfMeasure_UnitOfMeasureDTO UnitOfMeasure_UnitOfMeasureDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasure_UnitOfMeasureDTO.Id))
                return Forbid();

            UnitOfMeasure UnitOfMeasure = await UnitOfMeasureService.Get(UnitOfMeasure_UnitOfMeasureDTO.Id);
            return new UnitOfMeasure_UnitOfMeasureDTO(UnitOfMeasure);
        }

        [Route(UnitOfMeasureRoute.Create), HttpPost]
        public async Task<ActionResult<UnitOfMeasure_UnitOfMeasureDTO>> Create([FromBody] UnitOfMeasure_UnitOfMeasureDTO UnitOfMeasure_UnitOfMeasureDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(UnitOfMeasure_UnitOfMeasureDTO.Id))
                return Forbid();

            UnitOfMeasure UnitOfMeasure = ConvertDTOToEntity(UnitOfMeasure_UnitOfMeasureDTO);
            UnitOfMeasure = await UnitOfMeasureService.Create(UnitOfMeasure);
            UnitOfMeasure_UnitOfMeasureDTO = new UnitOfMeasure_UnitOfMeasureDTO(UnitOfMeasure);
            if (UnitOfMeasure.IsValidated)
                return UnitOfMeasure_UnitOfMeasureDTO;
            else
                return BadRequest(UnitOfMeasure_UnitOfMeasureDTO);
        }

        [Route(UnitOfMeasureRoute.Update), HttpPost]
        public async Task<ActionResult<UnitOfMeasure_UnitOfMeasureDTO>> Update([FromBody] UnitOfMeasure_UnitOfMeasureDTO UnitOfMeasure_UnitOfMeasureDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(UnitOfMeasure_UnitOfMeasureDTO.Id))
                return Forbid();

            UnitOfMeasure UnitOfMeasure = ConvertDTOToEntity(UnitOfMeasure_UnitOfMeasureDTO);
            UnitOfMeasure = await UnitOfMeasureService.Update(UnitOfMeasure);
            UnitOfMeasure_UnitOfMeasureDTO = new UnitOfMeasure_UnitOfMeasureDTO(UnitOfMeasure);
            if (UnitOfMeasure.IsValidated)
                return UnitOfMeasure_UnitOfMeasureDTO;
            else
                return BadRequest(UnitOfMeasure_UnitOfMeasureDTO);
        }

        [Route(UnitOfMeasureRoute.Delete), HttpPost]
        public async Task<ActionResult<UnitOfMeasure_UnitOfMeasureDTO>> Delete([FromBody] UnitOfMeasure_UnitOfMeasureDTO UnitOfMeasure_UnitOfMeasureDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasure_UnitOfMeasureDTO.Id))
                return Forbid();

            UnitOfMeasure UnitOfMeasure = ConvertDTOToEntity(UnitOfMeasure_UnitOfMeasureDTO);
            UnitOfMeasure = await UnitOfMeasureService.Delete(UnitOfMeasure);
            UnitOfMeasure_UnitOfMeasureDTO = new UnitOfMeasure_UnitOfMeasureDTO(UnitOfMeasure);
            if (UnitOfMeasure.IsValidated)
                return UnitOfMeasure_UnitOfMeasureDTO;
            else
                return BadRequest(UnitOfMeasure_UnitOfMeasureDTO);
        }

        [Route(UnitOfMeasureRoute.Import), HttpPost]
        public async Task<ActionResult<List<UnitOfMeasure_UnitOfMeasureDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.Import(DataFile);
            List<UnitOfMeasure_UnitOfMeasureDTO> UnitOfMeasure_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(c => new UnitOfMeasure_UnitOfMeasureDTO(c)).ToList();
            return UnitOfMeasure_UnitOfMeasureDTOs;
        }

        [Route(UnitOfMeasureRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] UnitOfMeasure_UnitOfMeasureFilterDTO UnitOfMeasure_UnitOfMeasureFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasure_UnitOfMeasureFilterDTO);
            UnitOfMeasureFilter = UnitOfMeasureService.ToFilter(UnitOfMeasureFilter);
            DataFile DataFile = await UnitOfMeasureService.Export(UnitOfMeasureFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(UnitOfMeasureRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Id.In = Ids;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.Id;
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = int.MaxValue;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            UnitOfMeasures = await UnitOfMeasureService.BulkDelete(UnitOfMeasures);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            if (Id > 0)
                UnitOfMeasureFilter.Id = new IdFilter { Equal = Id };
            UnitOfMeasureFilter = UnitOfMeasureService.ToFilter(UnitOfMeasureFilter);
            int count = await UnitOfMeasureService.Count(UnitOfMeasureFilter);
            if (count == 0)
                return false;
            return true;
        }

        private UnitOfMeasure ConvertDTOToEntity(UnitOfMeasure_UnitOfMeasureDTO UnitOfMeasure_UnitOfMeasureDTO)
        {
            UnitOfMeasure UnitOfMeasure = new UnitOfMeasure();
            UnitOfMeasure.Id = UnitOfMeasure_UnitOfMeasureDTO.Id;
            UnitOfMeasure.Code = UnitOfMeasure_UnitOfMeasureDTO.Code;
            UnitOfMeasure.Name = UnitOfMeasure_UnitOfMeasureDTO.Name;
            UnitOfMeasure.Description = UnitOfMeasure_UnitOfMeasureDTO.Description;
            UnitOfMeasure.StatusId = UnitOfMeasure_UnitOfMeasureDTO.StatusId;
            UnitOfMeasure.Status = UnitOfMeasure_UnitOfMeasureDTO.Status == null ? null : new Status
            {
                Id = UnitOfMeasure_UnitOfMeasureDTO.Status.Id,
                Code = UnitOfMeasure_UnitOfMeasureDTO.Status.Code,
                Name = UnitOfMeasure_UnitOfMeasureDTO.Status.Name,
            };
            UnitOfMeasure.BaseLanguage = CurrentContext.Language;
            return UnitOfMeasure;
        }

        private UnitOfMeasureFilter ConvertFilterDTOToFilterEntity(UnitOfMeasure_UnitOfMeasureFilterDTO UnitOfMeasure_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Skip = UnitOfMeasure_UnitOfMeasureFilterDTO.Skip;
            UnitOfMeasureFilter.Take = UnitOfMeasure_UnitOfMeasureFilterDTO.Take;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasure_UnitOfMeasureFilterDTO.OrderBy;
            UnitOfMeasureFilter.OrderType = UnitOfMeasure_UnitOfMeasureFilterDTO.OrderType;

            UnitOfMeasureFilter.Id = UnitOfMeasure_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = UnitOfMeasure_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = UnitOfMeasure_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = UnitOfMeasure_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = UnitOfMeasure_UnitOfMeasureFilterDTO.StatusId;
            return UnitOfMeasureFilter;
        }

        [Route(UnitOfMeasureRoute.SingleListStatus), HttpPost]
        public async Task<List<UnitOfMeasure_StatusDTO>> SingleListStatus([FromBody] UnitOfMeasure_StatusFilterDTO UnitOfMeasure_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = UnitOfMeasure_StatusFilterDTO.Id;
            StatusFilter.Code = UnitOfMeasure_StatusFilterDTO.Code;
            StatusFilter.Name = UnitOfMeasure_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<UnitOfMeasure_StatusDTO> UnitOfMeasure_StatusDTOs = Statuses
                .Select(x => new UnitOfMeasure_StatusDTO(x)).ToList();
            return UnitOfMeasure_StatusDTOs;
        }

    }
}

