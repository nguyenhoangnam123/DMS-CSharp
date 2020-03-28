using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MDistrict;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.district
{
    public class DistrictRoute : Root
    {
        public const string Master = Module + "/district/district-master";
        public const string Detail = Module + "/district/district-detail";
        private const string Default = Rpc + Module + "/district";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(DistrictFilter.Id), FieldType.ID },
            { nameof(DistrictFilter.Name), FieldType.STRING },
            { nameof(DistrictFilter.Priority), FieldType.LONG },
            { nameof(DistrictFilter.ProvinceId), FieldType.ID },
            { nameof(DistrictFilter.StatusId), FieldType.ID },
        };
    }

    public class DistrictController : RpcController
    {
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private IDistrictService DistrictService;
        private ICurrentContext CurrentContext;
        public DistrictController(
            IProvinceService ProvinceService,
            IStatusService StatusService,
            IDistrictService DistrictService,
            ICurrentContext CurrentContext
        )
        {
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.DistrictService = DistrictService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DistrictRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] District_DistrictFilterDTO District_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = ConvertFilterDTOToFilterEntity(District_DistrictFilterDTO);
            DistrictFilter = DistrictService.ToFilter(DistrictFilter);
            int count = await DistrictService.Count(DistrictFilter);
            return count;
        }

        [Route(DistrictRoute.List), HttpPost]
        public async Task<ActionResult<List<District_DistrictDTO>>> List([FromBody] District_DistrictFilterDTO District_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = ConvertFilterDTOToFilterEntity(District_DistrictFilterDTO);
            DistrictFilter = DistrictService.ToFilter(DistrictFilter);
            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<District_DistrictDTO> District_DistrictDTOs = Districts
                .Select(c => new District_DistrictDTO(c)).ToList();
            return District_DistrictDTOs;
        }

        [Route(DistrictRoute.Get), HttpPost]
        public async Task<ActionResult<District_DistrictDTO>> Get([FromBody]District_DistrictDTO District_DistrictDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(District_DistrictDTO.Id))
                return Forbid();

            District District = await DistrictService.Get(District_DistrictDTO.Id);
            return new District_DistrictDTO(District);
        }

        [Route(DistrictRoute.Create), HttpPost]
        public async Task<ActionResult<District_DistrictDTO>> Create([FromBody] District_DistrictDTO District_DistrictDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(District_DistrictDTO.Id))
                return Forbid();

            District District = ConvertDTOToEntity(District_DistrictDTO);
            District = await DistrictService.Create(District);
            District_DistrictDTO = new District_DistrictDTO(District);
            if (District.IsValidated)
                return District_DistrictDTO;
            else
                return BadRequest(District_DistrictDTO);
        }

        [Route(DistrictRoute.Update), HttpPost]
        public async Task<ActionResult<District_DistrictDTO>> Update([FromBody] District_DistrictDTO District_DistrictDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(District_DistrictDTO.Id))
                return Forbid();

            District District = ConvertDTOToEntity(District_DistrictDTO);
            District = await DistrictService.Update(District);
            District_DistrictDTO = new District_DistrictDTO(District);
            if (District.IsValidated)
                return District_DistrictDTO;
            else
                return BadRequest(District_DistrictDTO);
        }

        [Route(DistrictRoute.Delete), HttpPost]
        public async Task<ActionResult<District_DistrictDTO>> Delete([FromBody] District_DistrictDTO District_DistrictDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(District_DistrictDTO.Id))
                return Forbid();

            District District = ConvertDTOToEntity(District_DistrictDTO);
            District = await DistrictService.Delete(District);
            District_DistrictDTO = new District_DistrictDTO(District);
            if (District.IsValidated)
                return District_DistrictDTO;
            else
                return BadRequest(District_DistrictDTO);
        }

        [Route(DistrictRoute.Import), HttpPost]
        public async Task<ActionResult<List<District_DistrictDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<District> Districts = await DistrictService.Import(DataFile);
            List<District_DistrictDTO> District_DistrictDTOs = Districts
                .Select(c => new District_DistrictDTO(c)).ToList();
            return District_DistrictDTOs;
        }

        [Route(DistrictRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] District_DistrictFilterDTO District_DistrictFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = ConvertFilterDTOToFilterEntity(District_DistrictFilterDTO);
            DistrictFilter = DistrictService.ToFilter(DistrictFilter);
            DataFile DataFile = await DistrictService.Export(DistrictFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(DistrictRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Id = new IdFilter { In = Ids };
            DistrictFilter.Selects = DistrictSelect.Id;
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = int.MaxValue;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            Districts = await DistrictService.BulkDelete(Districts);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter = DistrictService.ToFilter(DistrictFilter);
            if (Id == 0)
            {

            }
            else
            {
                DistrictFilter.Id = new IdFilter { Equal = Id };
                int count = await DistrictService.Count(DistrictFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private District ConvertDTOToEntity(District_DistrictDTO District_DistrictDTO)
        {
            District District = new District();
            District.Id = District_DistrictDTO.Id;
            District.Code = District_DistrictDTO.Code;
            District.Name = District_DistrictDTO.Name;
            District.Priority = District_DistrictDTO.Priority;
            District.ProvinceId = District_DistrictDTO.ProvinceId;
            District.StatusId = District_DistrictDTO.StatusId;
            District.Province = District_DistrictDTO.Province == null ? null : new Province
            {
                Id = District_DistrictDTO.Province.Id,
                Name = District_DistrictDTO.Province.Name,
                Priority = District_DistrictDTO.Province.Priority,
                StatusId = District_DistrictDTO.Province.StatusId,
            };
            District.Status = District_DistrictDTO.Status == null ? null : new Status
            {
                Id = District_DistrictDTO.Status.Id,
                Code = District_DistrictDTO.Status.Code,
                Name = District_DistrictDTO.Status.Name,
            };
            District.BaseLanguage = CurrentContext.Language;
            return District;
        }

        private DistrictFilter ConvertFilterDTOToFilterEntity(District_DistrictFilterDTO District_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Skip = District_DistrictFilterDTO.Skip;
            DistrictFilter.Take = District_DistrictFilterDTO.Take;
            DistrictFilter.OrderBy = District_DistrictFilterDTO.OrderBy;
            DistrictFilter.OrderType = District_DistrictFilterDTO.OrderType;

            DistrictFilter.Id = District_DistrictFilterDTO.Id;
            DistrictFilter.Code = District_DistrictFilterDTO.Code;
            DistrictFilter.Name = District_DistrictFilterDTO.Name;
            DistrictFilter.Priority = District_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = District_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = District_DistrictFilterDTO.StatusId;
            return DistrictFilter;
        }

        [Route(DistrictRoute.SingleListProvince), HttpPost]
        public async Task<List<District_ProvinceDTO>> SingleListProvince([FromBody] District_ProvinceFilterDTO District_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.Id | ProvinceSelect.Name;
            ProvinceFilter.Id = District_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = District_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<District_ProvinceDTO> District_ProvinceDTOs = Provinces
                .Select(x => new District_ProvinceDTO(x)).ToList();
            return District_ProvinceDTOs;
        }
        [Route(DistrictRoute.SingleListStatus), HttpPost]
        public async Task<List<District_StatusDTO>> SingleListStatus([FromBody] District_StatusFilterDTO District_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<District_StatusDTO> District_StatusDTOs = Statuses
                .Select(x => new District_StatusDTO(x)).ToList();
            return District_StatusDTOs;
        }

    }
}

