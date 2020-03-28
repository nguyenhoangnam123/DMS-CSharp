using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MDistrict;
using DMS.Services.MStatus;
using DMS.Services.MWard;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.ward
{
    public class WardRoute : Root
    {
        public const string Master = Module + "/ward/ward-master";
        public const string Detail = Module + "/ward/ward-detail";
        private const string Default = Rpc + Module + "/ward";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(WardFilter.Id), FieldType.ID },
            { nameof(WardFilter.Name), FieldType.STRING },
            { nameof(WardFilter.Priority), FieldType.LONG },
            { nameof(WardFilter.DistrictId), FieldType.ID },
            { nameof(WardFilter.StatusId), FieldType.ID },
        };
    }

    public class WardController : RpcController
    {
        private IDistrictService DistrictService;
        private IStatusService StatusService;
        private IWardService WardService;
        private ICurrentContext CurrentContext;
        public WardController(
            IDistrictService DistrictService,
            IStatusService StatusService,
            IWardService WardService,
            ICurrentContext CurrentContext
        )
        {
            this.DistrictService = DistrictService;
            this.StatusService = StatusService;
            this.WardService = WardService;
            this.CurrentContext = CurrentContext;
        }

        [Route(WardRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Ward_WardFilterDTO Ward_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = ConvertFilterDTOToFilterEntity(Ward_WardFilterDTO);
            WardFilter = WardService.ToFilter(WardFilter);
            int count = await WardService.Count(WardFilter);
            return count;
        }

        [Route(WardRoute.List), HttpPost]
        public async Task<ActionResult<List<Ward_WardDTO>>> List([FromBody] Ward_WardFilterDTO Ward_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = ConvertFilterDTOToFilterEntity(Ward_WardFilterDTO);
            WardFilter = WardService.ToFilter(WardFilter);
            List<Ward> Wards = await WardService.List(WardFilter);
            List<Ward_WardDTO> Ward_WardDTOs = Wards
                .Select(c => new Ward_WardDTO(c)).ToList();
            return Ward_WardDTOs;
        }

        [Route(WardRoute.Get), HttpPost]
        public async Task<ActionResult<Ward_WardDTO>> Get([FromBody]Ward_WardDTO Ward_WardDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Ward_WardDTO.Id))
                return Forbid();

            Ward Ward = await WardService.Get(Ward_WardDTO.Id);
            return new Ward_WardDTO(Ward);
        }

        [Route(WardRoute.Create), HttpPost]
        public async Task<ActionResult<Ward_WardDTO>> Create([FromBody] Ward_WardDTO Ward_WardDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Ward_WardDTO.Id))
                return Forbid();

            Ward Ward = ConvertDTOToEntity(Ward_WardDTO);
            Ward = await WardService.Create(Ward);
            Ward_WardDTO = new Ward_WardDTO(Ward);
            if (Ward.IsValidated)
                return Ward_WardDTO;
            else
                return BadRequest(Ward_WardDTO);
        }

        [Route(WardRoute.Update), HttpPost]
        public async Task<ActionResult<Ward_WardDTO>> Update([FromBody] Ward_WardDTO Ward_WardDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Ward_WardDTO.Id))
                return Forbid();

            Ward Ward = ConvertDTOToEntity(Ward_WardDTO);
            Ward = await WardService.Update(Ward);
            Ward_WardDTO = new Ward_WardDTO(Ward);
            if (Ward.IsValidated)
                return Ward_WardDTO;
            else
                return BadRequest(Ward_WardDTO);
        }

        [Route(WardRoute.Delete), HttpPost]
        public async Task<ActionResult<Ward_WardDTO>> Delete([FromBody] Ward_WardDTO Ward_WardDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Ward_WardDTO.Id))
                return Forbid();

            Ward Ward = ConvertDTOToEntity(Ward_WardDTO);
            Ward = await WardService.Delete(Ward);
            Ward_WardDTO = new Ward_WardDTO(Ward);
            if (Ward.IsValidated)
                return Ward_WardDTO;
            else
                return BadRequest(Ward_WardDTO);
        }

        [Route(WardRoute.Import), HttpPost]
        public async Task<ActionResult<List<Ward_WardDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Ward> Wards = await WardService.Import(DataFile);
            List<Ward_WardDTO> Ward_WardDTOs = Wards
                .Select(c => new Ward_WardDTO(c)).ToList();
            return Ward_WardDTOs;
        }

        [Route(WardRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Ward_WardFilterDTO Ward_WardFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = ConvertFilterDTOToFilterEntity(Ward_WardFilterDTO);
            WardFilter = WardService.ToFilter(WardFilter);
            DataFile DataFile = await WardService.Export(WardFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(WardRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WardFilter WardFilter = new WardFilter();
            WardFilter.Id = new IdFilter { In = Ids };
            WardFilter.Selects = WardSelect.Id;
            WardFilter.Skip = 0;
            WardFilter.Take = int.MaxValue;

            List<Ward> Wards = await WardService.List(WardFilter);
            Wards = await WardService.BulkDelete(Wards);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter = WardService.ToFilter(WardFilter);
            if (Id == 0)
            {

            }
            else
            {
                WardFilter.Id = new IdFilter { Equal = Id };
                int count = await WardService.Count(WardFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Ward ConvertDTOToEntity(Ward_WardDTO Ward_WardDTO)
        {
            Ward Ward = new Ward();
            Ward.Id = Ward_WardDTO.Id;
            Ward.Code = Ward_WardDTO.Code;
            Ward.Name = Ward_WardDTO.Name;
            Ward.Priority = Ward_WardDTO.Priority;
            Ward.DistrictId = Ward_WardDTO.DistrictId;
            Ward.StatusId = Ward_WardDTO.StatusId;
            Ward.District = Ward_WardDTO.District == null ? null : new District
            {
                Id = Ward_WardDTO.District.Id,
                Name = Ward_WardDTO.District.Name,
                Priority = Ward_WardDTO.District.Priority,
                ProvinceId = Ward_WardDTO.District.ProvinceId,
                StatusId = Ward_WardDTO.District.StatusId,
            };
            Ward.Status = Ward_WardDTO.Status == null ? null : new Status
            {
                Id = Ward_WardDTO.Status.Id,
                Code = Ward_WardDTO.Status.Code,
                Name = Ward_WardDTO.Status.Name,
            };
            Ward.BaseLanguage = CurrentContext.Language;
            return Ward;
        }

        private WardFilter ConvertFilterDTOToFilterEntity(Ward_WardFilterDTO Ward_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Skip = Ward_WardFilterDTO.Skip;
            WardFilter.Take = Ward_WardFilterDTO.Take;
            WardFilter.OrderBy = Ward_WardFilterDTO.OrderBy;
            WardFilter.OrderType = Ward_WardFilterDTO.OrderType;

            WardFilter.Id = Ward_WardFilterDTO.Id;
            WardFilter.Code = Ward_WardFilterDTO.Code;
            WardFilter.Name = Ward_WardFilterDTO.Name;
            WardFilter.Priority = Ward_WardFilterDTO.Priority;
            WardFilter.DistrictId = Ward_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Ward_WardFilterDTO.StatusId;
            return WardFilter;
        }

        [Route(WardRoute.SingleListDistrict), HttpPost]
        public async Task<List<Ward_DistrictDTO>> SingleListDistrict([FromBody] Ward_DistrictFilterDTO Ward_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Ward_DistrictFilterDTO.Id;
            DistrictFilter.Name = Ward_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Ward_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Ward_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Ward_DistrictDTO> Ward_DistrictDTOs = Districts
                .Select(x => new Ward_DistrictDTO(x)).ToList();
            return Ward_DistrictDTOs;
        }
        [Route(WardRoute.SingleListStatus), HttpPost]
        public async Task<List<Ward_StatusDTO>> SingleListStatus([FromBody] Ward_StatusFilterDTO Ward_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Ward_StatusDTO> Ward_StatusDTOs = Statuses
                .Select(x => new Ward_StatusDTO(x)).ToList();
            return Ward_StatusDTOs;
        }
    }
}

