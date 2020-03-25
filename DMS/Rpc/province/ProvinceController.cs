using Common;
using DMS.Entities;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.province
{
    public class ProvinceRoute : Root
    {
        public const string Master = Module + "/province/province-master";
        public const string Detail = Module + "/province/province-detail";
        private const string Default = Rpc + Module + "/province";
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
            { nameof(ProvinceFilter.Id), FieldType.ID },
            { nameof(ProvinceFilter.Name), FieldType.STRING },
            { nameof(ProvinceFilter.Priority), FieldType.LONG },
            { nameof(ProvinceFilter.StatusId), FieldType.ID },
        };
    }

    public class ProvinceController : RpcController
    {
        private IStatusService StatusService;
        private IProvinceService ProvinceService;
        private ICurrentContext CurrentContext;
        public ProvinceController(
            IStatusService StatusService,
            IProvinceService ProvinceService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.ProvinceService = ProvinceService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProvinceRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Province_ProvinceFilterDTO Province_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = ConvertFilterDTOToFilterEntity(Province_ProvinceFilterDTO);
            ProvinceFilter = ProvinceService.ToFilter(ProvinceFilter);
            int count = await ProvinceService.Count(ProvinceFilter);
            return count;
        }

        [Route(ProvinceRoute.List), HttpPost]
        public async Task<ActionResult<List<Province_ProvinceDTO>>> List([FromBody] Province_ProvinceFilterDTO Province_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = ConvertFilterDTOToFilterEntity(Province_ProvinceFilterDTO);
            ProvinceFilter = ProvinceService.ToFilter(ProvinceFilter);
            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Province_ProvinceDTO> Province_ProvinceDTOs = Provinces
                .Select(c => new Province_ProvinceDTO(c)).ToList();
            return Province_ProvinceDTOs;
        }

        [Route(ProvinceRoute.Get), HttpPost]
        public async Task<ActionResult<Province_ProvinceDTO>> Get([FromBody]Province_ProvinceDTO Province_ProvinceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Province_ProvinceDTO.Id))
                return Forbid();

            Province Province = await ProvinceService.Get(Province_ProvinceDTO.Id);
            return new Province_ProvinceDTO(Province);
        }

        [Route(ProvinceRoute.Create), HttpPost]
        public async Task<ActionResult<Province_ProvinceDTO>> Create([FromBody] Province_ProvinceDTO Province_ProvinceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Province_ProvinceDTO.Id))
                return Forbid();

            Province Province = ConvertDTOToEntity(Province_ProvinceDTO);
            Province = await ProvinceService.Create(Province);
            Province_ProvinceDTO = new Province_ProvinceDTO(Province);
            if (Province.IsValidated)
                return Province_ProvinceDTO;
            else
                return BadRequest(Province_ProvinceDTO);
        }

        [Route(ProvinceRoute.Update), HttpPost]
        public async Task<ActionResult<Province_ProvinceDTO>> Update([FromBody] Province_ProvinceDTO Province_ProvinceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Province_ProvinceDTO.Id))
                return Forbid();

            Province Province = ConvertDTOToEntity(Province_ProvinceDTO);
            Province = await ProvinceService.Update(Province);
            Province_ProvinceDTO = new Province_ProvinceDTO(Province);
            if (Province.IsValidated)
                return Province_ProvinceDTO;
            else
                return BadRequest(Province_ProvinceDTO);
        }

        [Route(ProvinceRoute.Delete), HttpPost]
        public async Task<ActionResult<Province_ProvinceDTO>> Delete([FromBody] Province_ProvinceDTO Province_ProvinceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Province_ProvinceDTO.Id))
                return Forbid();

            Province Province = ConvertDTOToEntity(Province_ProvinceDTO);
            Province = await ProvinceService.Delete(Province);
            Province_ProvinceDTO = new Province_ProvinceDTO(Province);
            if (Province.IsValidated)
                return Province_ProvinceDTO;
            else
                return BadRequest(Province_ProvinceDTO);
        }

        [Route(ProvinceRoute.Import), HttpPost]
        public async Task<ActionResult<List<Province_ProvinceDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Province> Provinces = await ProvinceService.Import(DataFile);
            List<Province_ProvinceDTO> Province_ProvinceDTOs = Provinces
                .Select(c => new Province_ProvinceDTO(c)).ToList();
            return Province_ProvinceDTOs;
        }

        [Route(ProvinceRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Province_ProvinceFilterDTO Province_ProvinceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = ConvertFilterDTOToFilterEntity(Province_ProvinceFilterDTO);
            ProvinceFilter = ProvinceService.ToFilter(ProvinceFilter);
            DataFile DataFile = await ProvinceService.Export(ProvinceFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(ProvinceRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter = ProvinceService.ToFilter(ProvinceFilter);
            ProvinceFilter.Id = new IdFilter { In = Ids };
            ProvinceFilter.Selects = ProvinceSelect.Id;
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = int.MaxValue;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            Provinces = await ProvinceService.BulkDelete(Provinces);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter = ProvinceService.ToFilter(ProvinceFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProvinceFilter.Id = new IdFilter { Equal = Id };
                int count = await ProvinceService.Count(ProvinceFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Province ConvertDTOToEntity(Province_ProvinceDTO Province_ProvinceDTO)
        {
            Province Province = new Province();
            Province.Id = Province_ProvinceDTO.Id;
            Province.Name = Province_ProvinceDTO.Name;
            Province.Priority = Province_ProvinceDTO.Priority;
            Province.StatusId = Province_ProvinceDTO.StatusId;
            Province.Status = Province_ProvinceDTO.Status == null ? null : new Status
            {
                Id = Province_ProvinceDTO.Status.Id,
                Code = Province_ProvinceDTO.Status.Code,
                Name = Province_ProvinceDTO.Status.Name,
            };
            Province.BaseLanguage = CurrentContext.Language;
            return Province;
        }

        private ProvinceFilter ConvertFilterDTOToFilterEntity(Province_ProvinceFilterDTO Province_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Skip = Province_ProvinceFilterDTO.Skip;
            ProvinceFilter.Take = Province_ProvinceFilterDTO.Take;
            ProvinceFilter.OrderBy = Province_ProvinceFilterDTO.OrderBy;
            ProvinceFilter.OrderType = Province_ProvinceFilterDTO.OrderType;

            ProvinceFilter.Id = Province_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Province_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = Province_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = Province_ProvinceFilterDTO.StatusId;
            return ProvinceFilter;
        }

        [Route(ProvinceRoute.SingleListStatus), HttpPost]
        public async Task<List<Province_StatusDTO>> SingleListStatus([FromBody] Province_StatusFilterDTO Province_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Province_StatusFilterDTO.Id;
            StatusFilter.Code = Province_StatusFilterDTO.Code;
            StatusFilter.Name = Province_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Province_StatusDTO> Province_StatusDTOs = Statuses
                .Select(x => new Province_StatusDTO(x)).ToList();
            return Province_StatusDTOs;
        }

    }
}

