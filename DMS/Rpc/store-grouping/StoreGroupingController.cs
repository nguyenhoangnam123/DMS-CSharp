using Common;
using DMS.Entities;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MWard;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_grouping
{
    public class StoreGroupingRoute : Root
    {
        public const string Master = Module + "/store-grouping/store-grouping-master";
        public const string Detail = Module + "/store-grouping/store-grouping-detail";
        private const string Default = Rpc + Module + "/store-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListWard = Default + "/single-list-ward";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(StoreGroupingFilter.Id), FieldType.ID },
            { nameof(StoreGroupingFilter.Code), FieldType.STRING },
            { nameof(StoreGroupingFilter.Name), FieldType.STRING },
            { nameof(StoreGroupingFilter.ParentId), FieldType.ID },
            { nameof(StoreGroupingFilter.Path), FieldType.STRING },
            { nameof(StoreGroupingFilter.Level), FieldType.LONG },
        };
    }

    public class StoreGroupingController : RpcController
    {
        private IStoreService StoreService;
        private IDistrictService DistrictService;
        private IOrganizationService OrganizationService;
        private IProvinceService ProvinceService;
        private IStatusService StatusService;
        private IStoreTypeService StoreTypeService;
        private IWardService WardService;
        private IStoreGroupingService StoreGroupingService;
        private ICurrentContext CurrentContext;
        public StoreGroupingController(
            IStoreService StoreService,
            IDistrictService DistrictService,
            IOrganizationService OrganizationService,
            IProvinceService ProvinceService,
            IStatusService StatusService,
            IStoreTypeService StoreTypeService,
            IWardService WardService,
            IStoreGroupingService StoreGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.StoreService = StoreService;
            this.DistrictService = DistrictService;
            this.OrganizationService = OrganizationService;
            this.ProvinceService = ProvinceService;
            this.StatusService = StatusService;
            this.StoreTypeService = StoreTypeService;
            this.WardService = WardService;
            this.StoreGroupingService = StoreGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(StoreGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] StoreGrouping_StoreGroupingFilterDTO StoreGrouping_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = ConvertFilterDTOToFilterEntity(StoreGrouping_StoreGroupingFilterDTO);
            StoreGroupingFilter = StoreGroupingService.ToFilter(StoreGroupingFilter);
            int count = await StoreGroupingService.Count(StoreGroupingFilter);
            return count;
        }

        [Route(StoreGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<StoreGrouping_StoreGroupingDTO>>> List([FromBody] StoreGrouping_StoreGroupingFilterDTO StoreGrouping_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = ConvertFilterDTOToFilterEntity(StoreGrouping_StoreGroupingFilterDTO);
            StoreGroupingFilter = StoreGroupingService.ToFilter(StoreGroupingFilter);
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<StoreGrouping_StoreGroupingDTO> StoreGrouping_StoreGroupingDTOs = StoreGroupings
                .Select(c => new StoreGrouping_StoreGroupingDTO(c)).ToList();
            return StoreGrouping_StoreGroupingDTOs;
        }

        [Route(StoreGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<StoreGrouping_StoreGroupingDTO>> Get([FromBody]StoreGrouping_StoreGroupingDTO StoreGrouping_StoreGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreGrouping_StoreGroupingDTO.Id))
                return Forbid();

            StoreGrouping StoreGrouping = await StoreGroupingService.Get(StoreGrouping_StoreGroupingDTO.Id);
            return new StoreGrouping_StoreGroupingDTO(StoreGrouping);
        }

        [Route(StoreGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<StoreGrouping_StoreGroupingDTO>> Create([FromBody] StoreGrouping_StoreGroupingDTO StoreGrouping_StoreGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreGrouping_StoreGroupingDTO.Id))
                return Forbid();

            StoreGrouping StoreGrouping = ConvertDTOToEntity(StoreGrouping_StoreGroupingDTO);
            StoreGrouping = await StoreGroupingService.Create(StoreGrouping);
            StoreGrouping_StoreGroupingDTO = new StoreGrouping_StoreGroupingDTO(StoreGrouping);
            if (StoreGrouping.IsValidated)
                return StoreGrouping_StoreGroupingDTO;
            else
                return BadRequest(StoreGrouping_StoreGroupingDTO);
        }

        [Route(StoreGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<StoreGrouping_StoreGroupingDTO>> Update([FromBody] StoreGrouping_StoreGroupingDTO StoreGrouping_StoreGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreGrouping_StoreGroupingDTO.Id))
                return Forbid();

            StoreGrouping StoreGrouping = ConvertDTOToEntity(StoreGrouping_StoreGroupingDTO);
            StoreGrouping = await StoreGroupingService.Update(StoreGrouping);
            StoreGrouping_StoreGroupingDTO = new StoreGrouping_StoreGroupingDTO(StoreGrouping);
            if (StoreGrouping.IsValidated)
                return StoreGrouping_StoreGroupingDTO;
            else
                return BadRequest(StoreGrouping_StoreGroupingDTO);
        }

        [Route(StoreGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<StoreGrouping_StoreGroupingDTO>> Delete([FromBody] StoreGrouping_StoreGroupingDTO StoreGrouping_StoreGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(StoreGrouping_StoreGroupingDTO.Id))
                return Forbid();

            StoreGrouping StoreGrouping = ConvertDTOToEntity(StoreGrouping_StoreGroupingDTO);
            StoreGrouping = await StoreGroupingService.Delete(StoreGrouping);
            StoreGrouping_StoreGroupingDTO = new StoreGrouping_StoreGroupingDTO(StoreGrouping);
            if (StoreGrouping.IsValidated)
                return StoreGrouping_StoreGroupingDTO;
            else
                return BadRequest(StoreGrouping_StoreGroupingDTO);
        }

        [Route(StoreGroupingRoute.Import), HttpPost]
        public async Task<ActionResult<List<StoreGrouping_StoreGroupingDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.Import(DataFile);
            List<StoreGrouping_StoreGroupingDTO> StoreGrouping_StoreGroupingDTOs = StoreGroupings
                .Select(c => new StoreGrouping_StoreGroupingDTO(c)).ToList();
            return StoreGrouping_StoreGroupingDTOs;
        }

        [Route(StoreGroupingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] StoreGrouping_StoreGroupingFilterDTO StoreGrouping_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = ConvertFilterDTOToFilterEntity(StoreGrouping_StoreGroupingFilterDTO);
            StoreGroupingFilter = StoreGroupingService.ToFilter(StoreGroupingFilter);
            DataFile DataFile = await StoreGroupingService.Export(StoreGroupingFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(StoreGroupingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Id = new IdFilter { In = Ids };
            StoreGroupingFilter.Selects = StoreGroupingSelect.Id;
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            StoreGroupings = await StoreGroupingService.BulkDelete(StoreGroupings);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter = StoreGroupingService.ToFilter(StoreGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                StoreGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await StoreGroupingService.Count(StoreGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private StoreGrouping ConvertDTOToEntity(StoreGrouping_StoreGroupingDTO StoreGrouping_StoreGroupingDTO)
        {
            StoreGrouping StoreGrouping = new StoreGrouping();
            StoreGrouping.Id = StoreGrouping_StoreGroupingDTO.Id;
            StoreGrouping.Code = StoreGrouping_StoreGroupingDTO.Code;
            StoreGrouping.Name = StoreGrouping_StoreGroupingDTO.Name;
            StoreGrouping.ParentId = StoreGrouping_StoreGroupingDTO.ParentId;
            StoreGrouping.Path = StoreGrouping_StoreGroupingDTO.Path;
            StoreGrouping.Level = StoreGrouping_StoreGroupingDTO.Level;
            StoreGrouping.IsActive = StoreGrouping_StoreGroupingDTO.IsActive;
            StoreGrouping.Stores = StoreGrouping_StoreGroupingDTO.Stores?
                .Select(x => new Store
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentStoreId = x.ParentStoreId,
                    OrganizationId = x.OrganizationId,
                    StoreTypeId = x.StoreTypeId,
                    Telephone = x.Telephone,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                    Address = x.Address1,
                    DeliveryAddress = x.Address2,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    OwnerName = x.OwnerName,
                    OwnerPhone = x.OwnerPhone,
                    OwnerEmail = x.OwnerEmail,
                    StatusId = x.StatusId,
                    District = new District
                    {
                        Id = x.District.Id,
                        Name = x.District.Name,
                        Priority = x.District.Priority,
                        ProvinceId = x.District.ProvinceId,
                        StatusId = x.District.StatusId,
                    },
                    Organization = new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        ParentId = x.Organization.ParentId,
                        Path = x.Organization.Path,
                        Level = x.Organization.Level,
                        StatusId = x.Organization.StatusId,
                        Phone = x.Organization.Phone,
                        Address = x.Organization.Address,
                        Latitude = x.Organization.Latitude,
                        Longitude = x.Organization.Longitude,
                    },
                    ParentStore = new Store
                    {
                        Id = x.ParentStore.Id,
                        Code = x.ParentStore.Code,
                        Name = x.ParentStore.Name,
                        ParentStoreId = x.ParentStore.ParentStoreId,
                        OrganizationId = x.ParentStore.OrganizationId,
                        StoreTypeId = x.ParentStore.StoreTypeId,
                        StoreGroupingId = x.ParentStore.StoreGroupingId,
                        Telephone = x.ParentStore.Telephone,
                        ProvinceId = x.ParentStore.ProvinceId,
                        DistrictId = x.ParentStore.DistrictId,
                        WardId = x.ParentStore.WardId,
                        Address = x.ParentStore.Address1,
                        DeliveryAddress = x.ParentStore.Address2,
                        Latitude = x.ParentStore.Latitude,
                        Longitude = x.ParentStore.Longitude,
                        OwnerName = x.ParentStore.OwnerName,
                        OwnerPhone = x.ParentStore.OwnerPhone,
                        OwnerEmail = x.ParentStore.OwnerEmail,
                        StatusId = x.ParentStore.StatusId,
                    },
                    Province = new Province
                    {
                        Id = x.Province.Id,
                        Name = x.Province.Name,
                        Priority = x.Province.Priority,
                        StatusId = x.Province.StatusId,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                    Ward = new Ward
                    {
                        Id = x.Ward.Id,
                        Name = x.Ward.Name,
                        Priority = x.Ward.Priority,
                        DistrictId = x.Ward.DistrictId,
                        StatusId = x.Ward.StatusId,
                    },
                }).ToList();
            StoreGrouping.BaseLanguage = CurrentContext.Language;
            return StoreGrouping;
        }

        private StoreGroupingFilter ConvertFilterDTOToFilterEntity(StoreGrouping_StoreGroupingFilterDTO StoreGrouping_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGrouping_StoreGroupingFilterDTO.OrderBy;
            StoreGroupingFilter.OrderType = StoreGrouping_StoreGroupingFilterDTO.OrderType;

            StoreGroupingFilter.Id = StoreGrouping_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = StoreGrouping_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = StoreGrouping_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = StoreGrouping_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = StoreGrouping_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = StoreGrouping_StoreGroupingFilterDTO.Level;
            return StoreGroupingFilter;
        }

        [Route(StoreGroupingRoute.SingleListStore), HttpPost]
        public async Task<List<StoreGrouping_StoreDTO>> SingleListStore([FromBody] StoreGrouping_StoreFilterDTO StoreGrouping_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = StoreGrouping_StoreFilterDTO.Id;
            StoreFilter.Code = StoreGrouping_StoreFilterDTO.Code;
            StoreFilter.Name = StoreGrouping_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = StoreGrouping_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = StoreGrouping_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = StoreGrouping_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = StoreGrouping_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = StoreGrouping_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = StoreGrouping_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = StoreGrouping_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = StoreGrouping_StoreFilterDTO.WardId;
            StoreFilter.Address = StoreGrouping_StoreFilterDTO.Address1;
            StoreFilter.DeliveryAddress = StoreGrouping_StoreFilterDTO.Address2;
            StoreFilter.Latitude = StoreGrouping_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreGrouping_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = StoreGrouping_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreGrouping_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreGrouping_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = StoreGrouping_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreGrouping_StoreDTO> StoreGrouping_StoreDTOs = Stores
                .Select(x => new StoreGrouping_StoreDTO(x)).ToList();
            return StoreGrouping_StoreDTOs;
        }
        [Route(StoreGroupingRoute.SingleListDistrict), HttpPost]
        public async Task<List<StoreGrouping_DistrictDTO>> SingleListDistrict([FromBody] StoreGrouping_DistrictFilterDTO StoreGrouping_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = StoreGrouping_DistrictFilterDTO.Id;
            DistrictFilter.Name = StoreGrouping_DistrictFilterDTO.Name;
            DistrictFilter.Priority = StoreGrouping_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = StoreGrouping_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = StoreGrouping_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<StoreGrouping_DistrictDTO> StoreGrouping_DistrictDTOs = Districts
                .Select(x => new StoreGrouping_DistrictDTO(x)).ToList();
            return StoreGrouping_DistrictDTOs;
        }
        [Route(StoreGroupingRoute.SingleListOrganization), HttpPost]
        public async Task<List<StoreGrouping_OrganizationDTO>> SingleListOrganization([FromBody] StoreGrouping_OrganizationFilterDTO StoreGrouping_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = StoreGrouping_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = StoreGrouping_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = StoreGrouping_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = StoreGrouping_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = StoreGrouping_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = StoreGrouping_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = StoreGrouping_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = StoreGrouping_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = StoreGrouping_OrganizationFilterDTO.Address;
            OrganizationFilter.Latitude = StoreGrouping_OrganizationFilterDTO.Latitude;
            OrganizationFilter.Longitude = StoreGrouping_OrganizationFilterDTO.Longitude;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<StoreGrouping_OrganizationDTO> StoreGrouping_OrganizationDTOs = Organizations
                .Select(x => new StoreGrouping_OrganizationDTO(x)).ToList();
            return StoreGrouping_OrganizationDTOs;
        }
        [Route(StoreGroupingRoute.SingleListProvince), HttpPost]
        public async Task<List<StoreGrouping_ProvinceDTO>> SingleListProvince([FromBody] StoreGrouping_ProvinceFilterDTO StoreGrouping_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = StoreGrouping_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = StoreGrouping_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = StoreGrouping_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = StoreGrouping_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<StoreGrouping_ProvinceDTO> StoreGrouping_ProvinceDTOs = Provinces
                .Select(x => new StoreGrouping_ProvinceDTO(x)).ToList();
            return StoreGrouping_ProvinceDTOs;
        }
        [Route(StoreGroupingRoute.SingleListStatus), HttpPost]
        public async Task<List<StoreGrouping_StatusDTO>> SingleListStatus([FromBody] StoreGrouping_StatusFilterDTO StoreGrouping_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = StoreGrouping_StatusFilterDTO.Id;
            StatusFilter.Code = StoreGrouping_StatusFilterDTO.Code;
            StatusFilter.Name = StoreGrouping_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<StoreGrouping_StatusDTO> StoreGrouping_StatusDTOs = Statuses
                .Select(x => new StoreGrouping_StatusDTO(x)).ToList();
            return StoreGrouping_StatusDTOs;
        }
        [Route(StoreGroupingRoute.SingleListStoreType), HttpPost]
        public async Task<List<StoreGrouping_StoreTypeDTO>> SingleListStoreType([FromBody] StoreGrouping_StoreTypeFilterDTO StoreGrouping_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = StoreGrouping_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = StoreGrouping_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = StoreGrouping_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = StoreGrouping_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<StoreGrouping_StoreTypeDTO> StoreGrouping_StoreTypeDTOs = StoreTypes
                .Select(x => new StoreGrouping_StoreTypeDTO(x)).ToList();
            return StoreGrouping_StoreTypeDTOs;
        }
        [Route(StoreGroupingRoute.SingleListWard), HttpPost]
        public async Task<List<StoreGrouping_WardDTO>> SingleListWard([FromBody] StoreGrouping_WardFilterDTO StoreGrouping_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = StoreGrouping_WardFilterDTO.Id;
            WardFilter.Name = StoreGrouping_WardFilterDTO.Name;
            WardFilter.Priority = StoreGrouping_WardFilterDTO.Priority;
            WardFilter.DistrictId = StoreGrouping_WardFilterDTO.DistrictId;
            WardFilter.StatusId = StoreGrouping_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<StoreGrouping_WardDTO> StoreGrouping_WardDTOs = Wards
                .Select(x => new StoreGrouping_WardDTO(x)).ToList();
            return StoreGrouping_WardDTOs;
        }

    }
}

