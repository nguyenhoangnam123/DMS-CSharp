using Common;
using DMS.Entities;
using DMS.Services.MStatus;
using DMS.Services.MStoreType;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_type
{
    public class StoreTypeRoute : Root
    {
        public const string Master = Module + "/store-type/store-type-master";
        public const string Detail = Module + "/store-type/store-type-detail";
        private const string Default = Rpc + Module + "/store-type";
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
            { nameof(StoreTypeFilter.Code), FieldType.STRING },
            { nameof(StoreTypeFilter.Name), FieldType.STRING },
            { nameof(StoreTypeFilter.StatusId), FieldType.ID },
        };
    }

    public class StoreTypeController : RpcController
    {
        private IStatusService StatusService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public StoreTypeController(
            IStatusService StatusService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreType_StoreTypeFilterDTO StoreType_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = ConvertFilterDTOToFilterEntity(StoreType_StoreTypeFilterDTO);
            int count = await StoreTypeService.Count(StoreTypeFilter);
            return count;
        }

        [Route(StoreTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<StoreType_StoreTypeDTO>>> List([FromBody] StoreType_StoreTypeFilterDTO StoreType_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = ConvertFilterDTOToFilterEntity(StoreType_StoreTypeFilterDTO);
            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<StoreType_StoreTypeDTO> StoreType_StoreTypeDTOs = StoreTypes
                .Select(c => new StoreType_StoreTypeDTO(c)).ToList();
            return StoreType_StoreTypeDTOs;
        }

        [Route(StoreTypeRoute.Get), HttpPost]
        public async Task<ActionResult<StoreType_StoreTypeDTO>> Get([FromBody]StoreType_StoreTypeDTO StoreType_StoreTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreType_StoreTypeDTO.Id))
                return Forbid();

            StoreType StoreType = await StoreTypeService.Get(StoreType_StoreTypeDTO.Id);
            return new StoreType_StoreTypeDTO(StoreType);
        }

        [Route(StoreTypeRoute.Create), HttpPost]
        public async Task<ActionResult<StoreType_StoreTypeDTO>> Create([FromBody] StoreType_StoreTypeDTO StoreType_StoreTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreType_StoreTypeDTO.Id))
                return Forbid();

            StoreType StoreType = ConvertDTOToEntity(StoreType_StoreTypeDTO);
            StoreType = await StoreTypeService.Create(StoreType);
            StoreType_StoreTypeDTO = new StoreType_StoreTypeDTO(StoreType);
            if (StoreType.IsValidated)
                return StoreType_StoreTypeDTO;
            else
                return BadRequest(StoreType_StoreTypeDTO);
        }

        [Route(StoreTypeRoute.Update), HttpPost]
        public async Task<ActionResult<StoreType_StoreTypeDTO>> Update([FromBody] StoreType_StoreTypeDTO StoreType_StoreTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreType_StoreTypeDTO.Id))
                return Forbid();

            StoreType StoreType = ConvertDTOToEntity(StoreType_StoreTypeDTO);
            StoreType = await StoreTypeService.Update(StoreType);
            StoreType_StoreTypeDTO = new StoreType_StoreTypeDTO(StoreType);
            if (StoreType.IsValidated)
                return StoreType_StoreTypeDTO;
            else
                return BadRequest(StoreType_StoreTypeDTO);
        }

        [Route(StoreTypeRoute.Delete), HttpPost]
        public async Task<ActionResult<StoreType_StoreTypeDTO>> Delete([FromBody] StoreType_StoreTypeDTO StoreType_StoreTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreType_StoreTypeDTO.Id))
                return Forbid();

            StoreType StoreType = ConvertDTOToEntity(StoreType_StoreTypeDTO);
            StoreType = await StoreTypeService.Delete(StoreType);
            StoreType_StoreTypeDTO = new StoreType_StoreTypeDTO(StoreType);
            if (StoreType.IsValidated)
                return StoreType_StoreTypeDTO;
            else
                return BadRequest(StoreType_StoreTypeDTO);
        }

        [Route(StoreTypeRoute.Import), HttpPost]
        public async Task<ActionResult<List<StoreType_StoreTypeDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<StoreType> StoreTypes = await StoreTypeService.Import(DataFile);
            List<StoreType_StoreTypeDTO> StoreType_StoreTypeDTOs = StoreTypes
                .Select(c => new StoreType_StoreTypeDTO(c)).ToList();
            return StoreType_StoreTypeDTOs;
        }

        [Route(StoreTypeRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] StoreType_StoreTypeFilterDTO StoreType_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = ConvertFilterDTOToFilterEntity(StoreType_StoreTypeFilterDTO);
            DataFile DataFile = await StoreTypeService.Export(StoreTypeFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(StoreTypeRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Id = new IdFilter { In = Ids };
            StoreTypeFilter.Selects = StoreTypeSelect.Id;
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = int.MaxValue;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            StoreTypes = await StoreTypeService.BulkDelete(StoreTypes);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            if (Id == 0)
            {

            }
            else
            {
                StoreTypeFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreTypeService.Count(StoreTypeFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private StoreType ConvertDTOToEntity(StoreType_StoreTypeDTO StoreType_StoreTypeDTO)
        {
            StoreType StoreType = new StoreType();
            StoreType.Id = StoreType_StoreTypeDTO.Id;
            StoreType.Code = StoreType_StoreTypeDTO.Code;
            StoreType.Name = StoreType_StoreTypeDTO.Name;
            StoreType.StatusId = StoreType_StoreTypeDTO.StatusId;
            StoreType.Status = StoreType_StoreTypeDTO.Status == null ? null : new Status
            {
                Id = StoreType_StoreTypeDTO.Status.Id,
                Code = StoreType_StoreTypeDTO.Status.Code,
                Name = StoreType_StoreTypeDTO.Status.Name,
            };
            StoreType.BaseLanguage = CurrentContext.Language;
            return StoreType;
        }

        private StoreTypeFilter ConvertFilterDTOToFilterEntity(StoreType_StoreTypeFilterDTO StoreType_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Skip = StoreType_StoreTypeFilterDTO.Skip;
            StoreTypeFilter.Take = StoreType_StoreTypeFilterDTO.Take;
            StoreTypeFilter.OrderBy = StoreType_StoreTypeFilterDTO.OrderBy;
            StoreTypeFilter.OrderType = StoreType_StoreTypeFilterDTO.OrderType;

            StoreTypeFilter.Id = StoreType_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = StoreType_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = StoreType_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = StoreType_StoreTypeFilterDTO.StatusId;
            return StoreTypeFilter;
        }

        [Route(StoreTypeRoute.SingleListStatus), HttpPost]
        public async Task<List<StoreType_StatusDTO>> SingleListStatus([FromBody] StoreType_StatusFilterDTO StoreType_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<StoreType_StatusDTO> StoreType_StatusDTOs = Statuses
                .Select(x => new StoreType_StatusDTO(x)).ToList();
            return StoreType_StatusDTOs;
        }

    }
}

