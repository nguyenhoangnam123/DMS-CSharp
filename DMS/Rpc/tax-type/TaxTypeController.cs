using Common;
using DMS.Entities;
using DMS.Services.MStatus;
using DMS.Services.MTaxType;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.tax_type
{
    public class TaxTypeRoute : Root
    {
        public const string Master = Module + "/tax-type/tax-type-master";
        public const string Detail = Module + "/tax-type/tax-type-detail";
        private const string Default = Rpc + Module + "/tax-type";
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
            { nameof(TaxTypeFilter.Id), FieldType.ID },
            { nameof(TaxTypeFilter.Code), FieldType.STRING },
            { nameof(TaxTypeFilter.Name), FieldType.STRING },
            { nameof(TaxTypeFilter.Percentage), FieldType.DECIMAL },
            { nameof(TaxTypeFilter.StatusId), FieldType.ID },
        };
    }

    public class TaxTypeController : RpcController
    {
        private IStatusService StatusService;
        private ITaxTypeService TaxTypeService;
        private ICurrentContext CurrentContext;
        public TaxTypeController(
            IStatusService StatusService,
            ITaxTypeService TaxTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.TaxTypeService = TaxTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(TaxTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] TaxType_TaxTypeFilterDTO TaxType_TaxTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TaxTypeFilter TaxTypeFilter = ConvertFilterDTOToFilterEntity(TaxType_TaxTypeFilterDTO);
            int count = await TaxTypeService.Count(TaxTypeFilter);
            return count;
        }

        [Route(TaxTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<TaxType_TaxTypeDTO>>> List([FromBody] TaxType_TaxTypeFilterDTO TaxType_TaxTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TaxTypeFilter TaxTypeFilter = ConvertFilterDTOToFilterEntity(TaxType_TaxTypeFilterDTO);
            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<TaxType_TaxTypeDTO> TaxType_TaxTypeDTOs = TaxTypes
                .Select(c => new TaxType_TaxTypeDTO(c)).ToList();
            return TaxType_TaxTypeDTOs;
        }

        [Route(TaxTypeRoute.Get), HttpPost]
        public async Task<ActionResult<TaxType_TaxTypeDTO>> Get([FromBody]TaxType_TaxTypeDTO TaxType_TaxTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(TaxType_TaxTypeDTO.Id))
                return Forbid();

            TaxType TaxType = await TaxTypeService.Get(TaxType_TaxTypeDTO.Id);
            return new TaxType_TaxTypeDTO(TaxType);
        }

        [Route(TaxTypeRoute.Create), HttpPost]
        public async Task<ActionResult<TaxType_TaxTypeDTO>> Create([FromBody] TaxType_TaxTypeDTO TaxType_TaxTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(TaxType_TaxTypeDTO.Id))
                return Forbid();

            TaxType TaxType = ConvertDTOToEntity(TaxType_TaxTypeDTO);
            TaxType = await TaxTypeService.Create(TaxType);
            TaxType_TaxTypeDTO = new TaxType_TaxTypeDTO(TaxType);
            if (TaxType.IsValidated)
                return TaxType_TaxTypeDTO;
            else
                return BadRequest(TaxType_TaxTypeDTO);
        }

        [Route(TaxTypeRoute.Update), HttpPost]
        public async Task<ActionResult<TaxType_TaxTypeDTO>> Update([FromBody] TaxType_TaxTypeDTO TaxType_TaxTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(TaxType_TaxTypeDTO.Id))
                return Forbid();

            TaxType TaxType = ConvertDTOToEntity(TaxType_TaxTypeDTO);
            TaxType = await TaxTypeService.Update(TaxType);
            TaxType_TaxTypeDTO = new TaxType_TaxTypeDTO(TaxType);
            if (TaxType.IsValidated)
                return TaxType_TaxTypeDTO;
            else
                return BadRequest(TaxType_TaxTypeDTO);
        }

        [Route(TaxTypeRoute.Delete), HttpPost]
        public async Task<ActionResult<TaxType_TaxTypeDTO>> Delete([FromBody] TaxType_TaxTypeDTO TaxType_TaxTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(TaxType_TaxTypeDTO.Id))
                return Forbid();

            TaxType TaxType = ConvertDTOToEntity(TaxType_TaxTypeDTO);
            TaxType = await TaxTypeService.Delete(TaxType);
            TaxType_TaxTypeDTO = new TaxType_TaxTypeDTO(TaxType);
            if (TaxType.IsValidated)
                return TaxType_TaxTypeDTO;
            else
                return BadRequest(TaxType_TaxTypeDTO);
        }

        [Route(TaxTypeRoute.Import), HttpPost]
        public async Task<ActionResult<List<TaxType_TaxTypeDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<TaxType> TaxTypes = await TaxTypeService.Import(DataFile);
            List<TaxType_TaxTypeDTO> TaxType_TaxTypeDTOs = TaxTypes
                .Select(c => new TaxType_TaxTypeDTO(c)).ToList();
            return TaxType_TaxTypeDTOs;
        }

        [Route(TaxTypeRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] TaxType_TaxTypeFilterDTO TaxType_TaxTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TaxTypeFilter TaxTypeFilter = ConvertFilterDTOToFilterEntity(TaxType_TaxTypeFilterDTO);
            DataFile DataFile = await TaxTypeService.Export(TaxTypeFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(TaxTypeRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Id = new IdFilter { In = Ids };
            TaxTypeFilter.Selects = TaxTypeSelect.Id;
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = int.MaxValue;

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            TaxTypes = await TaxTypeService.BulkDelete(TaxTypes);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            if (Id == 0)
            {

            }
            else
            {
                TaxTypeFilter.Id = new IdFilter { Equal = Id };
                int count = await TaxTypeService.Count(TaxTypeFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private TaxType ConvertDTOToEntity(TaxType_TaxTypeDTO TaxType_TaxTypeDTO)
        {
            TaxType TaxType = new TaxType();
            TaxType.Id = TaxType_TaxTypeDTO.Id;
            TaxType.Code = TaxType_TaxTypeDTO.Code;
            TaxType.Name = TaxType_TaxTypeDTO.Name;
            TaxType.Percentage = TaxType_TaxTypeDTO.Percentage;
            TaxType.StatusId = TaxType_TaxTypeDTO.StatusId;
            TaxType.Status = TaxType_TaxTypeDTO.Status == null ? null : new Status
            {
                Id = TaxType_TaxTypeDTO.Status.Id,
                Code = TaxType_TaxTypeDTO.Status.Code,
                Name = TaxType_TaxTypeDTO.Status.Name,
            };
            TaxType.BaseLanguage = CurrentContext.Language;
            return TaxType;
        }

        private TaxTypeFilter ConvertFilterDTOToFilterEntity(TaxType_TaxTypeFilterDTO TaxType_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Skip = TaxType_TaxTypeFilterDTO.Skip;
            TaxTypeFilter.Take = TaxType_TaxTypeFilterDTO.Take;
            TaxTypeFilter.OrderBy = TaxType_TaxTypeFilterDTO.OrderBy;
            TaxTypeFilter.OrderType = TaxType_TaxTypeFilterDTO.OrderType;

            TaxTypeFilter.Id = TaxType_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = TaxType_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = TaxType_TaxTypeFilterDTO.Name;
            TaxTypeFilter.Percentage = TaxType_TaxTypeFilterDTO.Percentage;
            TaxTypeFilter.StatusId = TaxType_TaxTypeFilterDTO.StatusId;
            return TaxTypeFilter;
        }

        [Route(TaxTypeRoute.SingleListStatus), HttpPost]
        public async Task<List<TaxType_StatusDTO>> SingleListStatus([FromBody] TaxType_StatusFilterDTO TaxType_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<TaxType_StatusDTO> TaxType_StatusDTOs = Statuses
                .Select(x => new TaxType_StatusDTO(x)).ToList();
            return TaxType_StatusDTOs;
        }
    }
}

