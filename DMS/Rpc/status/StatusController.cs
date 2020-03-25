using Common;
using DMS.Entities;
using DMS.Services.MStatus;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.status
{
    public class StatusRoute : Root
    {
        public const string Master = Module + "/status/status-master";
        public const string Detail = Module + "/status/status-detail";
        private const string Default = Rpc + Module + "/status";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(StatusFilter.Id), FieldType.ID },
            { nameof(StatusFilter.Code), FieldType.STRING },
            { nameof(StatusFilter.Name), FieldType.STRING },
        };
    }

    public class StatusController : RpcController
    {
        private IStatusService StatusService;
        private ICurrentContext CurrentContext;
        public StatusController(
            IStatusService StatusService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StatusRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Status_StatusFilterDTO Status_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = ConvertFilterDTOToFilterEntity(Status_StatusFilterDTO);
            int count = await StatusService.Count(StatusFilter);
            return count;
        }

        [Route(StatusRoute.List), HttpPost]
        public async Task<ActionResult<List<Status_StatusDTO>>> List([FromBody] Status_StatusFilterDTO Status_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = ConvertFilterDTOToFilterEntity(Status_StatusFilterDTO);
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Status_StatusDTO> Status_StatusDTOs = Statuses
                .Select(c => new Status_StatusDTO(c)).ToList();
            return Status_StatusDTOs;
        }

        private StatusFilter ConvertFilterDTOToFilterEntity(Status_StatusFilterDTO Status_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Skip = Status_StatusFilterDTO.Skip;
            StatusFilter.Take = Status_StatusFilterDTO.Take;
            StatusFilter.OrderBy = Status_StatusFilterDTO.OrderBy;
            StatusFilter.OrderType = Status_StatusFilterDTO.OrderType;

            StatusFilter.Id = Status_StatusFilterDTO.Id;
            StatusFilter.Code = Status_StatusFilterDTO.Code;
            StatusFilter.Name = Status_StatusFilterDTO.Name;
            return StatusFilter;
        }


    }
}

