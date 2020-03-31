using Common;
using DMS.Entities;
using DMS.Enums;
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

        public const string SingleListParentStoreGrouping = Default + "/single-list-parent-store-store";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(StoreGroupingFilter.Id), FieldType.ID },
            { nameof(StoreGroupingFilter.Code), FieldType.STRING },
            { nameof(StoreGroupingFilter.Name), FieldType.STRING },
            { nameof(StoreGroupingFilter.ParentId), FieldType.ID },
            { nameof(StoreGroupingFilter.Path), FieldType.STRING },
            { nameof(StoreGroupingFilter.Level), FieldType.LONG },
            { nameof(StoreGroupingFilter.StatusId), FieldType.LONG },
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
            StoreGrouping.StatusId = StoreGrouping_StoreGroupingDTO.StatusId;
            StoreGrouping.Parent = StoreGrouping_StoreGroupingDTO.Parent == null ? null : new StoreGrouping
            {
                Id = StoreGrouping_StoreGroupingDTO.Parent.Id,
                Code = StoreGrouping_StoreGroupingDTO.Parent.Code,
                Name = StoreGrouping_StoreGroupingDTO.Parent.Name,
                Path = StoreGrouping_StoreGroupingDTO.Parent.Path,
                Level = StoreGrouping_StoreGroupingDTO.Parent.Level,
                ParentId = StoreGrouping_StoreGroupingDTO.Parent.ParentId,
                StatusId = StoreGrouping_StoreGroupingDTO.Parent.StatusId
            };
            StoreGrouping.Status = StoreGrouping_StoreGroupingDTO.Status == null ? null : new Status
            {
                Id = StoreGrouping_StoreGroupingDTO.Status.Id,
                Code = StoreGrouping_StoreGroupingDTO.Status.Code,
                Name = StoreGrouping_StoreGroupingDTO.Status.Name
            };
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
            StoreGroupingFilter.StatusId = StoreGrouping_StoreGroupingFilterDTO.StatusId;
            return StoreGroupingFilter;
        }

        [Route(StoreGroupingRoute.SingleListParentStoreGrouping), HttpPost]
        public async Task<List<StoreGrouping_StoreGroupingDTO>> SingleListParentStoreGrouping([FromBody] StoreGrouping_StoreGroupingFilterDTO StoreGrouping_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = int.MaxValue;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<StoreGrouping_StoreGroupingDTO> StoreGrouping_StoreGroupingDTOs = StoreGroupings
                .Select(x => new StoreGrouping_StoreGroupingDTO(x)).ToList();
            return StoreGrouping_StoreGroupingDTOs;
        }

        [Route(StoreGroupingRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] StoreGrouping_StoreFilterDTO StoreGrouping_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
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
            StoreFilter.Address = StoreGrouping_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreGrouping_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreGrouping_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreGrouping_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = StoreGrouping_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreGrouping_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreGrouping_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await StoreService.Count(StoreFilter);
        }

        [Route(StoreGroupingRoute.ListStore), HttpPost]
        public async Task<List<StoreGrouping_StoreDTO>> ListStore([FromBody] StoreGrouping_StoreFilterDTO StoreGrouping_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = StoreGrouping_StoreFilterDTO.Skip;
            StoreFilter.Take = StoreGrouping_StoreFilterDTO.Take;
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
            StoreFilter.Address = StoreGrouping_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = StoreGrouping_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = StoreGrouping_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = StoreGrouping_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = StoreGrouping_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = StoreGrouping_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = StoreGrouping_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<StoreGrouping_StoreDTO> StoreGrouping_StoreDTOs = Stores
                .Select(x => new StoreGrouping_StoreDTO(x)).ToList();
            return StoreGrouping_StoreDTOs;
        }
    }
}

