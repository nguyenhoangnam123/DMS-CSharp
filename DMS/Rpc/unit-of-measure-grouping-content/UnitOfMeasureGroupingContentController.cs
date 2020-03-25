using Common;
using DMS.Entities;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MUnitOfMeasureGroupingContent;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.unit_of_measure_grouping_content
{
    public class UnitOfMeasureGroupingContentRoute : Root
    {
        public const string Master = Module + "/unit-of-measure-grouping-content/unit-of-measure-grouping-content-master";
        public const string Detail = Module + "/unit-of-measure-grouping-content/unit-of-measure-grouping-content-detail";
        private const string Default = Rpc + Module + "/unit-of-measure-grouping-content";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListUnitOfMeasureGrouping = Default + "/single-list-unit-of-measure-grouping";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(UnitOfMeasureGroupingContentFilter.Id), FieldType.ID },
            { nameof(UnitOfMeasureGroupingContentFilter.UnitOfMeasureGroupingId), FieldType.ID },
            { nameof(UnitOfMeasureGroupingContentFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(UnitOfMeasureGroupingContentFilter.Factor), FieldType.LONG },
        };
    }

    public class UnitOfMeasureGroupingContentController : RpcController
    {
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IUnitOfMeasureGroupingContentService UnitOfMeasureGroupingContentService;
        private ICurrentContext CurrentContext;
        public UnitOfMeasureGroupingContentController(
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IUnitOfMeasureGroupingContentService UnitOfMeasureGroupingContentService,
            ICurrentContext CurrentContext
        )
        {
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.UnitOfMeasureGroupingContentService = UnitOfMeasureGroupingContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(UnitOfMeasureGroupingContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO);
            UnitOfMeasureGroupingContentFilter = UnitOfMeasureGroupingContentService.ToFilter(UnitOfMeasureGroupingContentFilter);
            int count = await UnitOfMeasureGroupingContentService.Count(UnitOfMeasureGroupingContentFilter);
            return count;
        }

        [Route(UnitOfMeasureGroupingContentRoute.List), HttpPost]
        public async Task<ActionResult<List<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO>>> List([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO);
            UnitOfMeasureGroupingContentFilter = UnitOfMeasureGroupingContentService.ToFilter(UnitOfMeasureGroupingContentFilter);
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await UnitOfMeasureGroupingContentService.List(UnitOfMeasureGroupingContentFilter);
            List<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO> UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTOs = UnitOfMeasureGroupingContents
                .Select(c => new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO(c)).ToList();
            return UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTOs;
        }

        [Route(UnitOfMeasureGroupingContentRoute.Get), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO>> Get([FromBody]UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Id))
                return Forbid();

            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = await UnitOfMeasureGroupingContentService.Get(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Id);
            return new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO(UnitOfMeasureGroupingContent);
        }

        [Route(UnitOfMeasureGroupingContentRoute.Create), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO>> Create([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Id))
                return Forbid();

            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = ConvertDTOToEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO);
            UnitOfMeasureGroupingContent = await UnitOfMeasureGroupingContentService.Create(UnitOfMeasureGroupingContent);
            UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO = new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO(UnitOfMeasureGroupingContent);
            if (UnitOfMeasureGroupingContent.IsValidated)
                return UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO;
            else
                return BadRequest(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO);
        }

        [Route(UnitOfMeasureGroupingContentRoute.Update), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO>> Update([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Id))
                return Forbid();

            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = ConvertDTOToEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO);
            UnitOfMeasureGroupingContent = await UnitOfMeasureGroupingContentService.Update(UnitOfMeasureGroupingContent);
            UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO = new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO(UnitOfMeasureGroupingContent);
            if (UnitOfMeasureGroupingContent.IsValidated)
                return UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO;
            else
                return BadRequest(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO);
        }

        [Route(UnitOfMeasureGroupingContentRoute.Delete), HttpPost]
        public async Task<ActionResult<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO>> Delete([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Id))
                return Forbid();

            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = ConvertDTOToEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO);
            UnitOfMeasureGroupingContent = await UnitOfMeasureGroupingContentService.Delete(UnitOfMeasureGroupingContent);
            UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO = new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO(UnitOfMeasureGroupingContent);
            if (UnitOfMeasureGroupingContent.IsValidated)
                return UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO;
            else
                return BadRequest(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO);
        }

        [Route(UnitOfMeasureGroupingContentRoute.Import), HttpPost]
        public async Task<ActionResult<List<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await UnitOfMeasureGroupingContentService.Import(DataFile);
            List<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO> UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTOs = UnitOfMeasureGroupingContents
                .Select(c => new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO(c)).ToList();
            return UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTOs;
        }

        [Route(UnitOfMeasureGroupingContentRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = ConvertFilterDTOToFilterEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO);
            UnitOfMeasureGroupingContentFilter = UnitOfMeasureGroupingContentService.ToFilter(UnitOfMeasureGroupingContentFilter);
            DataFile DataFile = await UnitOfMeasureGroupingContentService.Export(UnitOfMeasureGroupingContentFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(UnitOfMeasureGroupingContentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = new UnitOfMeasureGroupingContentFilter();
            UnitOfMeasureGroupingContentFilter.Id = new IdFilter { In = Ids };
            UnitOfMeasureGroupingContentFilter.Selects = UnitOfMeasureGroupingContentSelect.Id;
            UnitOfMeasureGroupingContentFilter.Skip = 0;
            UnitOfMeasureGroupingContentFilter.Take = int.MaxValue;

            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await UnitOfMeasureGroupingContentService.List(UnitOfMeasureGroupingContentFilter);
            UnitOfMeasureGroupingContents = await UnitOfMeasureGroupingContentService.BulkDelete(UnitOfMeasureGroupingContents);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = new UnitOfMeasureGroupingContentFilter();
            UnitOfMeasureGroupingContentFilter = UnitOfMeasureGroupingContentService.ToFilter(UnitOfMeasureGroupingContentFilter);
            if (Id == 0)
            {

            }
            else
            {
                UnitOfMeasureGroupingContentFilter.Id = new IdFilter { Equal = Id };
                int count = await UnitOfMeasureGroupingContentService.Count(UnitOfMeasureGroupingContentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private UnitOfMeasureGroupingContent ConvertDTOToEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO)
        {
            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = new UnitOfMeasureGroupingContent();
            UnitOfMeasureGroupingContent.Id = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Id;
            UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureGroupingId;
            UnitOfMeasureGroupingContent.UnitOfMeasureId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingContent.Factor = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.Factor;
            UnitOfMeasureGroupingContent.UnitOfMeasure = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasure.Id,
                Code = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasure.Code,
                Name = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasure.Name,
                Description = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasure.Description,
                StatusId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasure.StatusId,
            };
            UnitOfMeasureGroupingContent.UnitOfMeasureGrouping = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
            {
                Id = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureGrouping.Id,
                Name = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureGrouping.Name,
                UnitOfMeasureId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureGrouping.UnitOfMeasureId,
                StatusId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentDTO.UnitOfMeasureGrouping.StatusId,
            };
            UnitOfMeasureGroupingContent.BaseLanguage = CurrentContext.Language;
            return UnitOfMeasureGroupingContent;
        }

        private UnitOfMeasureGroupingContentFilter ConvertFilterDTOToFilterEntity(UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO)
        {
            UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter = new UnitOfMeasureGroupingContentFilter();
            UnitOfMeasureGroupingContentFilter.Selects = UnitOfMeasureGroupingContentSelect.ALL;
            UnitOfMeasureGroupingContentFilter.Skip = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.Skip;
            UnitOfMeasureGroupingContentFilter.Take = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.Take;
            UnitOfMeasureGroupingContentFilter.OrderBy = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.OrderBy;
            UnitOfMeasureGroupingContentFilter.OrderType = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.OrderType;

            UnitOfMeasureGroupingContentFilter.Id = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.Id;
            UnitOfMeasureGroupingContentFilter.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.UnitOfMeasureGroupingId;
            UnitOfMeasureGroupingContentFilter.UnitOfMeasureId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingContentFilter.Factor = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingContentFilterDTO.Factor;
            return UnitOfMeasureGroupingContentFilter;
        }

        [Route(UnitOfMeasureGroupingContentRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<UnitOfMeasureGroupingContent_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = UnitOfMeasureGroupingContent_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<UnitOfMeasureGroupingContent_UnitOfMeasureDTO> UnitOfMeasureGroupingContent_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new UnitOfMeasureGroupingContent_UnitOfMeasureDTO(x)).ToList();
            return UnitOfMeasureGroupingContent_UnitOfMeasureDTOs;
        }
        [Route(UnitOfMeasureGroupingContentRoute.SingleListUnitOfMeasureGrouping), HttpPost]
        public async Task<List<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingDTO>> SingleListUnitOfMeasureGrouping([FromBody] UnitOfMeasureGroupingContent_UnitOfMeasureGroupingFilterDTO UnitOfMeasureGroupingContent_UnitOfMeasureGroupingFilterDTO)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter.Skip = 0;
            UnitOfMeasureGroupingFilter.Take = 20;
            UnitOfMeasureGroupingFilter.OrderBy = UnitOfMeasureGroupingOrder.Id;
            UnitOfMeasureGroupingFilter.OrderType = OrderType.ASC;
            UnitOfMeasureGroupingFilter.Selects = UnitOfMeasureGroupingSelect.ALL;
            UnitOfMeasureGroupingFilter.Id = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingFilterDTO.Id;
            UnitOfMeasureGroupingFilter.Name = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingFilterDTO.Name;
            UnitOfMeasureGroupingFilter.UnitOfMeasureId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingFilterDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingFilter.StatusId = UnitOfMeasureGroupingContent_UnitOfMeasureGroupingFilterDTO.StatusId;

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            List<UnitOfMeasureGroupingContent_UnitOfMeasureGroupingDTO> UnitOfMeasureGroupingContent_UnitOfMeasureGroupingDTOs = UnitOfMeasureGroupings
                .Select(x => new UnitOfMeasureGroupingContent_UnitOfMeasureGroupingDTO(x)).ToList();
            return UnitOfMeasureGroupingContent_UnitOfMeasureGroupingDTOs;
        }

    }
}

