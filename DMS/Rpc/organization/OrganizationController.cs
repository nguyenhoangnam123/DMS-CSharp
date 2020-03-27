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

namespace DMS.Rpc.organization
{
    public class OrganizationRoute : Root
    {
        public const string Master = Module + "/organization/organization-master";
        public const string Detail = Module + "/organization/organization-detail";
        private const string Default = Rpc + Module + "/organization";
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
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListWard = Default + "/single-list-ward";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(OrganizationFilter.Id), FieldType.ID },
            { nameof(OrganizationFilter.Code), FieldType.STRING },
            { nameof(OrganizationFilter.Name), FieldType.STRING },
            { nameof(OrganizationFilter.ParentId), FieldType.ID },
            { nameof(OrganizationFilter.Path), FieldType.STRING },
            { nameof(OrganizationFilter.Level), FieldType.LONG },
            { nameof(OrganizationFilter.StatusId), FieldType.ID },
            { nameof(OrganizationFilter.Phone), FieldType.STRING },
            { nameof(OrganizationFilter.Address), FieldType.STRING },
            { nameof(OrganizationFilter.Latitude), FieldType.DECIMAL },
            { nameof(OrganizationFilter.Longitude), FieldType.DECIMAL },
        };
    }

    public class OrganizationController : RpcController
    {
        private IStatusService StatusService;
        private IStoreService StoreService;
        private IDistrictService DistrictService;
        private IProvinceService ProvinceService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IWardService WardService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public OrganizationController(
            IStatusService StatusService,
            IStoreService StoreService,
            IDistrictService DistrictService,
            IProvinceService ProvinceService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IWardService WardService,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.StoreService = StoreService;
            this.DistrictService = DistrictService;
            this.ProvinceService = ProvinceService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.WardService = WardService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(OrganizationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO);
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            int count = await OrganizationService.Count(OrganizationFilter);
            return count;
        }

        [Route(OrganizationRoute.List), HttpPost]
        public async Task<ActionResult<List<Organization_OrganizationDTO>>> List([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO);
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<Organization_OrganizationDTO> Organization_OrganizationDTOs = Organizations
                .Select(c => new Organization_OrganizationDTO(c)).ToList();
            return Organization_OrganizationDTOs;
        }

        [Route(OrganizationRoute.Get), HttpPost]
        public async Task<ActionResult<Organization_OrganizationDTO>> Get([FromBody]Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Organization_OrganizationDTO.Id))
                return Forbid();

            Organization Organization = await OrganizationService.Get(Organization_OrganizationDTO.Id);
            return new Organization_OrganizationDTO(Organization);
        }

        [Route(OrganizationRoute.Create), HttpPost]
        public async Task<ActionResult<Organization_OrganizationDTO>> Create([FromBody] Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Organization_OrganizationDTO.Id))
                return Forbid();

            Organization Organization = ConvertDTOToEntity(Organization_OrganizationDTO);
            Organization = await OrganizationService.Create(Organization);
            Organization_OrganizationDTO = new Organization_OrganizationDTO(Organization);
            if (Organization.IsValidated)
                return Organization_OrganizationDTO;
            else
                return BadRequest(Organization_OrganizationDTO);
        }

        [Route(OrganizationRoute.Update), HttpPost]
        public async Task<ActionResult<Organization_OrganizationDTO>> Update([FromBody] Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Organization_OrganizationDTO.Id))
                return Forbid();

            Organization Organization = ConvertDTOToEntity(Organization_OrganizationDTO);
            Organization = await OrganizationService.Update(Organization);
            Organization_OrganizationDTO = new Organization_OrganizationDTO(Organization);
            if (Organization.IsValidated)
                return Organization_OrganizationDTO;
            else
                return BadRequest(Organization_OrganizationDTO);
        }

        [Route(OrganizationRoute.Delete), HttpPost]
        public async Task<ActionResult<Organization_OrganizationDTO>> Delete([FromBody] Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Organization_OrganizationDTO.Id))
                return Forbid();

            Organization Organization = ConvertDTOToEntity(Organization_OrganizationDTO);
            Organization = await OrganizationService.Delete(Organization);
            Organization_OrganizationDTO = new Organization_OrganizationDTO(Organization);
            if (Organization.IsValidated)
                return Organization_OrganizationDTO;
            else
                return BadRequest(Organization_OrganizationDTO);
        }

        [Route(OrganizationRoute.Import), HttpPost]
        public async Task<ActionResult<List<Organization_OrganizationDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Organization> Organizations = await OrganizationService.Import(DataFile);
            List<Organization_OrganizationDTO> Organization_OrganizationDTOs = Organizations
                .Select(c => new Organization_OrganizationDTO(c)).ToList();
            return Organization_OrganizationDTOs;
        }

        [Route(OrganizationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO);
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            DataFile DataFile = await OrganizationService.Export(OrganizationFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(OrganizationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Id = new IdFilter { In = Ids };
            OrganizationFilter.Selects = OrganizationSelect.Id;
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            Organizations = await OrganizationService.BulkDelete(Organizations);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter = OrganizationService.ToFilter(OrganizationFilter);
            if (Id == 0)
            {

            }
            else
            {
                OrganizationFilter.Id = new IdFilter { Equal = Id };
                int count = await OrganizationService.Count(OrganizationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Organization ConvertDTOToEntity(Organization_OrganizationDTO Organization_OrganizationDTO)
        {
            Organization Organization = new Organization();
            Organization.Id = Organization_OrganizationDTO.Id;
            Organization.Code = Organization_OrganizationDTO.Code;
            Organization.Name = Organization_OrganizationDTO.Name;
            Organization.ParentId = Organization_OrganizationDTO.ParentId;
            Organization.Path = Organization_OrganizationDTO.Path;
            Organization.Level = Organization_OrganizationDTO.Level;
            Organization.StatusId = Organization_OrganizationDTO.StatusId;
            Organization.Phone = Organization_OrganizationDTO.Phone;
            Organization.Address = Organization_OrganizationDTO.Address;
            Organization.Latitude = Organization_OrganizationDTO.Latitude;
            Organization.Longitude = Organization_OrganizationDTO.Longitude;
            Organization.Parent = Organization_OrganizationDTO.Parent == null ? null : new Organization
            {
                Id = Organization_OrganizationDTO.Parent.Id,
                Code = Organization_OrganizationDTO.Parent.Code,
                Name = Organization_OrganizationDTO.Parent.Name,
                ParentId = Organization_OrganizationDTO.Parent.ParentId,
                Path = Organization_OrganizationDTO.Parent.Path,
                Level = Organization_OrganizationDTO.Parent.Level,
                StatusId = Organization_OrganizationDTO.Parent.StatusId,
                Phone = Organization_OrganizationDTO.Parent.Phone,
                Address = Organization_OrganizationDTO.Parent.Address,
                Latitude = Organization_OrganizationDTO.Parent.Latitude,
                Longitude = Organization_OrganizationDTO.Parent.Longitude,
            };
            Organization.Status = Organization_OrganizationDTO.Status == null ? null : new Status
            {
                Id = Organization_OrganizationDTO.Status.Id,
                Code = Organization_OrganizationDTO.Status.Code,
                Name = Organization_OrganizationDTO.Status.Name,
            };
            Organization.Stores = Organization_OrganizationDTO.Stores?
                .Select(x => new Store
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ParentStoreId = x.ParentStoreId,
                    StoreTypeId = x.StoreTypeId,
                    StoreGroupingId = x.StoreGroupingId,
                    Telephone = x.Telephone,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                    Address = x.Address,
                    DeliveryAddress = x.DeliveryAddress,
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
                        Address = x.ParentStore.Address,
                        DeliveryAddress = x.ParentStore.DeliveryAddress,
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
                    StoreGrouping = new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                        IsActive = x.StoreGrouping.IsActive,
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
            Organization.BaseLanguage = CurrentContext.Language;
            return Organization;
        }

        private OrganizationFilter ConvertFilterDTOToFilterEntity(Organization_OrganizationFilterDTO Organization_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = Organization_OrganizationFilterDTO.OrderBy;
            OrganizationFilter.OrderType = Organization_OrganizationFilterDTO.OrderType;

            OrganizationFilter.Id = Organization_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = Organization_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = Organization_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = Organization_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = Organization_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = Organization_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = Organization_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = Organization_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = Organization_OrganizationFilterDTO.Address;
            OrganizationFilter.Latitude = Organization_OrganizationFilterDTO.Latitude;
            OrganizationFilter.Longitude = Organization_OrganizationFilterDTO.Longitude;
            return OrganizationFilter;
        }

        [Route(OrganizationRoute.SingleListStatus), HttpPost]
        public async Task<List<Organization_StatusDTO>> SingleListStatus([FromBody] Organization_StatusFilterDTO Organization_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Organization_StatusFilterDTO.Id;
            StatusFilter.Code = Organization_StatusFilterDTO.Code;
            StatusFilter.Name = Organization_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Organization_StatusDTO> Organization_StatusDTOs = Statuses
                .Select(x => new Organization_StatusDTO(x)).ToList();
            return Organization_StatusDTOs;
        }
        [Route(OrganizationRoute.SingleListStore), HttpPost]
        public async Task<List<Organization_StoreDTO>> SingleListStore([FromBody] Organization_StoreFilterDTO Organization_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = Organization_StoreFilterDTO.Id;
            StoreFilter.Code = Organization_StoreFilterDTO.Code;
            StoreFilter.Name = Organization_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = Organization_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = Organization_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = Organization_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = Organization_StoreFilterDTO.StoreGroupingId;
            StoreFilter.Telephone = Organization_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = Organization_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = Organization_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = Organization_StoreFilterDTO.WardId;
            StoreFilter.Address = Organization_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = Organization_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = Organization_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = Organization_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = Organization_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = Organization_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = Organization_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = Organization_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<Organization_StoreDTO> Organization_StoreDTOs = Stores
                .Select(x => new Organization_StoreDTO(x)).ToList();
            return Organization_StoreDTOs;
        }
        [Route(OrganizationRoute.SingleListDistrict), HttpPost]
        public async Task<List<Organization_DistrictDTO>> SingleListDistrict([FromBody] Organization_DistrictFilterDTO Organization_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Id;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = Organization_DistrictFilterDTO.Id;
            DistrictFilter.Name = Organization_DistrictFilterDTO.Name;
            DistrictFilter.Priority = Organization_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = Organization_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = Organization_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<Organization_DistrictDTO> Organization_DistrictDTOs = Districts
                .Select(x => new Organization_DistrictDTO(x)).ToList();
            return Organization_DistrictDTOs;
        }
        [Route(OrganizationRoute.SingleListProvince), HttpPost]
        public async Task<List<Organization_ProvinceDTO>> SingleListProvince([FromBody] Organization_ProvinceFilterDTO Organization_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Id;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = Organization_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = Organization_ProvinceFilterDTO.Name;
            ProvinceFilter.Priority = Organization_ProvinceFilterDTO.Priority;
            ProvinceFilter.StatusId = Organization_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<Organization_ProvinceDTO> Organization_ProvinceDTOs = Provinces
                .Select(x => new Organization_ProvinceDTO(x)).ToList();
            return Organization_ProvinceDTOs;
        }
        [Route(OrganizationRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<Organization_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] Organization_StoreGroupingFilterDTO Organization_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Id = Organization_StoreGroupingFilterDTO.Id;
            StoreGroupingFilter.Code = Organization_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = Organization_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.ParentId = Organization_StoreGroupingFilterDTO.ParentId;
            StoreGroupingFilter.Path = Organization_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.Level = Organization_StoreGroupingFilterDTO.Level;

            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<Organization_StoreGroupingDTO> Organization_StoreGroupingDTOs = StoreGroupings
                .Select(x => new Organization_StoreGroupingDTO(x)).ToList();
            return Organization_StoreGroupingDTOs;
        }
        [Route(OrganizationRoute.SingleListStoreType), HttpPost]
        public async Task<List<Organization_StoreTypeDTO>> SingleListStoreType([FromBody] Organization_StoreTypeFilterDTO Organization_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = Organization_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = Organization_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = Organization_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = Organization_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<Organization_StoreTypeDTO> Organization_StoreTypeDTOs = StoreTypes
                .Select(x => new Organization_StoreTypeDTO(x)).ToList();
            return Organization_StoreTypeDTOs;
        }
        [Route(OrganizationRoute.SingleListWard), HttpPost]
        public async Task<List<Organization_WardDTO>> SingleListWard([FromBody] Organization_WardFilterDTO Organization_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Id;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = Organization_WardFilterDTO.Id;
            WardFilter.Name = Organization_WardFilterDTO.Name;
            WardFilter.Priority = Organization_WardFilterDTO.Priority;
            WardFilter.DistrictId = Organization_WardFilterDTO.DistrictId;
            WardFilter.StatusId = Organization_WardFilterDTO.StatusId;

            List<Ward> Wards = await WardService.List(WardFilter);
            List<Organization_WardDTO> Organization_WardDTOs = Wards
                .Select(x => new Organization_WardDTO(x)).ToList();
            return Organization_WardDTOs;
        }

    }
}

