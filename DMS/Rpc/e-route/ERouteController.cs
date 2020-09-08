using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MERoute;
using DMS.Services.MERouteType;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MWorkflow;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.e_route
{
    public class ERouteController : RpcController
    {
        private IAppUserService AppUserService;
        private IERouteTypeService ERouteTypeService;
        private IOrganizationService OrganizationService;
        private IRequestStateService RequestStateService;
        private IStatusService StatusService;
        private IERouteService ERouteService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ERouteController(
            IAppUserService AppUserService,
            IERouteTypeService ERouteTypeService,
            IOrganizationService OrganizationService,
            IRequestStateService RequestStateService,
            IStatusService StatusService,
            IERouteService ERouteService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ERouteTypeService = ERouteTypeService;
            this.OrganizationService = OrganizationService;
            this.RequestStateService = RequestStateService;
            this.StatusService = StatusService;
            this.ERouteService = ERouteService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ERouteRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO);
            ERouteFilter = await ERouteService.ToFilter(ERouteFilter);
            int count = await ERouteService.Count(ERouteFilter);
            return count;
        }

        [Route(ERouteRoute.List), HttpPost]
        public async Task<ActionResult<List<ERoute_ERouteDTO>>> List([FromBody] ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO);
            ERouteFilter = await ERouteService.ToFilter(ERouteFilter);
            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            List<ERoute_ERouteDTO> ERoute_ERouteDTOs = ERoutes
                .Select(c => new ERoute_ERouteDTO(c)).ToList();
            return ERoute_ERouteDTOs;
        }

        [Route(ERouteRoute.Get), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Get([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = await ERouteService.Get(ERoute_ERouteDTO.Id);
            return new ERoute_ERouteDTO(ERoute);
        }

        [Route(ERouteRoute.Create), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Create([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = ConvertDTOToEntity(ERoute_ERouteDTO);
            ERoute = await ERouteService.Create(ERoute);
            ERoute_ERouteDTO = new ERoute_ERouteDTO(ERoute);
            if (ERoute.IsValidated)
                return ERoute_ERouteDTO;
            else
                return BadRequest(ERoute_ERouteDTO);
        }

        [Route(ERouteRoute.Update), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Update([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = ConvertDTOToEntity(ERoute_ERouteDTO);
            ERoute = await ERouteService.Update(ERoute);
            ERoute_ERouteDTO = new ERoute_ERouteDTO(ERoute);
            if (ERoute.IsValidated)
                return ERoute_ERouteDTO;
            else
                return BadRequest(ERoute_ERouteDTO);
        }

        [Route(ERouteRoute.Delete), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Delete([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = ConvertDTOToEntity(ERoute_ERouteDTO);
            ERoute = await ERouteService.Delete(ERoute);
            ERoute_ERouteDTO = new ERoute_ERouteDTO(ERoute);
            if (ERoute.IsValidated)
                return ERoute_ERouteDTO;
            else
                return BadRequest(ERoute_ERouteDTO);
        }

        [Route(ERouteRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter = await ERouteService.ToFilter(ERouteFilter);
            ERouteFilter.Id = new IdFilter { In = Ids };
            ERouteFilter.Selects = ERouteSelect.Id | ERouteSelect.RequestState;
            ERouteFilter.Skip = 0;
            ERouteFilter.Take = int.MaxValue;

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            ERoutes = await ERouteService.BulkDelete(ERoutes);
            if (ERoutes.Any(x => !x.IsValidated))
                return BadRequest(ERoutes.Where(x => !x.IsValidated));
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter = await ERouteService.ToFilter(ERouteFilter);
            if (Id == 0)
            {

            }
            else
            {
                ERouteFilter.Id = new IdFilter { Equal = Id };
                int count = await ERouteService.Count(ERouteFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ERoute ConvertDTOToEntity(ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            ERoute ERoute = new ERoute();
            ERoute.Id = ERoute_ERouteDTO.Id;
            ERoute.Code = ERoute_ERouteDTO.Code;
            ERoute.Name = ERoute_ERouteDTO.Name;
            ERoute.SaleEmployeeId = ERoute_ERouteDTO.SaleEmployeeId;
            ERoute.OrganizationId = ERoute_ERouteDTO.OrganizationId;
            ERoute.StartDate = ERoute_ERouteDTO.StartDate;
            ERoute.EndDate = ERoute_ERouteDTO.EndDate;
            ERoute.ERouteTypeId = ERoute_ERouteDTO.ERouteTypeId;
            ERoute.RequestStateId = ERoute_ERouteDTO.RequestStateId;
            ERoute.StatusId = ERoute_ERouteDTO.StatusId;
            ERoute.CreatorId = ERoute_ERouteDTO.CreatorId;
            ERoute.Creator = ERoute_ERouteDTO.Creator == null ? null : new AppUser
            {
                Id = ERoute_ERouteDTO.Creator.Id,
                Username = ERoute_ERouteDTO.Creator.Username,
                DisplayName = ERoute_ERouteDTO.Creator.DisplayName,
                Address = ERoute_ERouteDTO.Creator.Address,
                Email = ERoute_ERouteDTO.Creator.Email,
                Phone = ERoute_ERouteDTO.Creator.Phone,
                PositionId = ERoute_ERouteDTO.Creator.PositionId,
                Department = ERoute_ERouteDTO.Creator.Department,
                OrganizationId = ERoute_ERouteDTO.Creator.OrganizationId,
                SexId = ERoute_ERouteDTO.Creator.SexId,
                StatusId = ERoute_ERouteDTO.Creator.StatusId,
                Avatar = ERoute_ERouteDTO.Creator.Avatar,
                Birthday = ERoute_ERouteDTO.Creator.Birthday,
                ProvinceId = ERoute_ERouteDTO.Creator.ProvinceId,
            };
            ERoute.Organization = ERoute_ERouteDTO.Organization == null ? null : new Organization
            {
                Id = ERoute_ERouteDTO.Organization.Id,
                Code = ERoute_ERouteDTO.Organization.Code,
                Name = ERoute_ERouteDTO.Organization.Name,
                ParentId = ERoute_ERouteDTO.Organization.ParentId,
                Path = ERoute_ERouteDTO.Organization.Path,
                Level = ERoute_ERouteDTO.Organization.Level,
                StatusId = ERoute_ERouteDTO.Organization.StatusId,
                Phone = ERoute_ERouteDTO.Organization.Phone,
                Address = ERoute_ERouteDTO.Organization.Address,
                Email = ERoute_ERouteDTO.Organization.Email,
            };
            ERoute.ERouteType = ERoute_ERouteDTO.ERouteType == null ? null : new ERouteType
            {
                Id = ERoute_ERouteDTO.ERouteType.Id,
                Code = ERoute_ERouteDTO.ERouteType.Code,
                Name = ERoute_ERouteDTO.ERouteType.Name,
            };
            ERoute.RequestState = ERoute_ERouteDTO.RequestState == null ? null : new RequestState
            {
                Id = ERoute_ERouteDTO.RequestState.Id,
                Code = ERoute_ERouteDTO.RequestState.Code,
                Name = ERoute_ERouteDTO.RequestState.Name,
            };
            ERoute.SaleEmployee = ERoute_ERouteDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = ERoute_ERouteDTO.SaleEmployee.Id,
                Username = ERoute_ERouteDTO.SaleEmployee.Username,
                DisplayName = ERoute_ERouteDTO.SaleEmployee.DisplayName,
                Address = ERoute_ERouteDTO.SaleEmployee.Address,
                Email = ERoute_ERouteDTO.SaleEmployee.Email,
                Phone = ERoute_ERouteDTO.SaleEmployee.Phone,
                PositionId = ERoute_ERouteDTO.SaleEmployee.PositionId,
                Department = ERoute_ERouteDTO.SaleEmployee.Department,
                OrganizationId = ERoute_ERouteDTO.SaleEmployee.OrganizationId,
                SexId = ERoute_ERouteDTO.SaleEmployee.SexId,
                StatusId = ERoute_ERouteDTO.SaleEmployee.StatusId,
                Avatar = ERoute_ERouteDTO.SaleEmployee.Avatar,
                Birthday = ERoute_ERouteDTO.SaleEmployee.Birthday,
                ProvinceId = ERoute_ERouteDTO.SaleEmployee.ProvinceId,
            };
            ERoute.Status = ERoute_ERouteDTO.Status == null ? null : new Status
            {
                Id = ERoute_ERouteDTO.Status.Id,
                Code = ERoute_ERouteDTO.Status.Code,
                Name = ERoute_ERouteDTO.Status.Name,
            };
            ERoute.ERouteContents = ERoute_ERouteDTO.ERouteContents?.Select(x => new ERouteContent
            {
                Id = x.Id,
                ERouteId = x.ERouteId,
                StoreId = x.StoreId,
                OrderNumber = x.OrderNumber,
                Monday = x.Monday,
                Tuesday = x.Tuesday,
                Wednesday = x.Wednesday,
                Thursday = x.Thursday,
                Friday = x.Friday,
                Saturday = x.Saturday,
                Sunday = x.Sunday,
                Week1 = x.Week1,
                Week2 = x.Week2,
                Week3 = x.Week3,
                Week4 = x.Week4,
                Store = new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    Name = x.Store.Name,
                    ParentStoreId = x.Store.ParentStoreId,
                    OrganizationId = x.Store.OrganizationId,
                    StoreTypeId = x.Store.StoreTypeId,
                    StoreGroupingId = x.Store.StoreGroupingId,
                    ResellerId = x.Store.ResellerId,
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    DeliveryAddress = x.Store.DeliveryAddress,
                    Latitude = x.Store.Latitude,
                    Longitude = x.Store.Longitude,
                    DeliveryLatitude = x.Store.DeliveryLatitude,
                    DeliveryLongitude = x.Store.DeliveryLongitude,
                    OwnerName = x.Store.OwnerName,
                    OwnerPhone = x.Store.OwnerPhone,
                    OwnerEmail = x.Store.OwnerEmail,
                    TaxCode = x.Store.TaxCode,
                    LegalEntity = x.Store.LegalEntity,
                    StatusId = x.Store.StatusId,
                },
            }).ToList();
            ERoute.BaseLanguage = CurrentContext.Language;
            return ERoute;
        }

        private ERouteFilter ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter.Selects = ERouteSelect.ALL;
            ERouteFilter.Skip = ERoute_ERouteFilterDTO.Skip;
            ERouteFilter.Take = ERoute_ERouteFilterDTO.Take;
            ERouteFilter.OrderBy = ERoute_ERouteFilterDTO.OrderBy;
            ERouteFilter.OrderType = ERoute_ERouteFilterDTO.OrderType;

            ERouteFilter.Id = ERoute_ERouteFilterDTO.Id;
            ERouteFilter.Code = ERoute_ERouteFilterDTO.Code;
            ERouteFilter.Name = ERoute_ERouteFilterDTO.Name;
            ERouteFilter.OrganizationId = ERoute_ERouteFilterDTO.OrganizationId;
            ERouteFilter.AppUserId = ERoute_ERouteFilterDTO.AppUserId;
            ERouteFilter.StartDate = ERoute_ERouteFilterDTO.StartDate;
            ERouteFilter.EndDate = ERoute_ERouteFilterDTO.EndDate;
            ERouteFilter.RequestStateId = ERoute_ERouteFilterDTO.RequestStateId;
            ERouteFilter.ERouteTypeId = ERoute_ERouteFilterDTO.ERouteTypeId;
            ERouteFilter.StatusId = ERoute_ERouteFilterDTO.StatusId;
            ERouteFilter.CreatorId = ERoute_ERouteFilterDTO.CreatorId;
            ERouteFilter.StoreId = ERoute_ERouteFilterDTO.StoreId;
            ERouteFilter.CreatedAt = ERoute_ERouteFilterDTO.CreatedAt;
            ERouteFilter.UpdatedAt = ERoute_ERouteFilterDTO.UpdatedAt;
            return ERouteFilter;
        }

        [Route(ERouteRoute.FilterListAppUser), HttpPost]
        public async Task<List<ERoute_AppUserDTO>> FilterListAppUser([FromBody] ERoute_AppUserFilterDTO ERoute_AppUserFilterDTO)
        {
            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ERoute_AppUserFilterDTO.Id;
            AppUserFilter.Username = ERoute_AppUserFilterDTO.Username;
            AppUserFilter.Password = ERoute_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = ERoute_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ERoute_AppUserFilterDTO.Address;
            AppUserFilter.Email = ERoute_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ERoute_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = ERoute_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = ERoute_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = new IdFilter { Equal = CurrentUser.OrganizationId };
            AppUserFilter.SexId = ERoute_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = ERoute_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = ERoute_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = ERoute_AppUserFilterDTO.ProvinceId;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ERoute_AppUserDTO> ERoute_AppUserDTOs = AppUsers
                .Select(x => new ERoute_AppUserDTO(x)).ToList();
            return ERoute_AppUserDTOs;
        }
        [Route(ERouteRoute.FilterListStore), HttpPost]
        public async Task<List<ERoute_StoreDTO>> FilterListStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = CurrentUser.OrganizationId };
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ERoute_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ERoute_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ERoute_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = ERoute_StoreFilterDTO.StatusId;

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERoute_StoreDTO> ERoute_StoreDTOs = Stores
                .Select(x => new ERoute_StoreDTO(x)).ToList();
            return ERoute_StoreDTOs;
        }
        [Route(ERouteRoute.FilterListERouteType), HttpPost]
        public async Task<List<ERoute_ERouteTypeDTO>> FilterListERouteType([FromBody] ERoute_ERouteTypeFilterDTO ERoute_ERouteTypeFilterDTO)
        {
            ERouteTypeFilter ERouteTypeFilter = new ERouteTypeFilter();
            ERouteTypeFilter.Skip = 0;
            ERouteTypeFilter.Take = 20;
            ERouteTypeFilter.OrderBy = ERouteTypeOrder.Id;
            ERouteTypeFilter.OrderType = OrderType.ASC;
            ERouteTypeFilter.Selects = ERouteTypeSelect.ALL;
            ERouteTypeFilter.Id = ERoute_ERouteTypeFilterDTO.Id;
            ERouteTypeFilter.Code = ERoute_ERouteTypeFilterDTO.Code;
            ERouteTypeFilter.Name = ERoute_ERouteTypeFilterDTO.Name;

            List<ERouteType> ERouteTypes = await ERouteTypeService.List(ERouteTypeFilter);
            List<ERoute_ERouteTypeDTO> ERoute_ERouteTypeDTOs = ERouteTypes
                .Select(x => new ERoute_ERouteTypeDTO(x)).ToList();
            return ERoute_ERouteTypeDTOs;
        }
        [Route(ERouteRoute.FilterListStatus), HttpPost]
        public async Task<List<ERoute_StatusDTO>> FilterListStatus([FromBody] ERoute_StatusFilterDTO ERoute_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ERoute_StatusDTO> ERoute_StatusDTOs = Statuses
                .Select(x => new ERoute_StatusDTO(x)).ToList();
            return ERoute_StatusDTOs;
        }
        [Route(ERouteRoute.FilterListOrganization), HttpPost]
        public async Task<List<ERoute_OrganizationDTO>> FilterListOrganization([FromBody] ERoute_OrganizationFilterDTO ERoute_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ERoute_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ERoute_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ERoute_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ERoute_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ERoute_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ERoute_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = null;
            OrganizationFilter.Phone = ERoute_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = ERoute_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = ERoute_OrganizationFilterDTO.Email;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ERoute_OrganizationDTO> ERoute_OrganizationDTOs = Organizations
                .Select(x => new ERoute_OrganizationDTO(x)).ToList();
            return ERoute_OrganizationDTOs;
        }

        [Route(ERouteRoute.SingleListAppUser), HttpPost]
        public async Task<List<ERoute_AppUserDTO>> SingleListAppUser([FromBody] ERoute_AppUserFilterDTO ERoute_AppUserFilterDTO)
        {
            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ERoute_AppUserFilterDTO.Id;
            AppUserFilter.Username = ERoute_AppUserFilterDTO.Username;
            AppUserFilter.Password = ERoute_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = ERoute_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ERoute_AppUserFilterDTO.Address;
            AppUserFilter.Email = ERoute_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ERoute_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = ERoute_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = ERoute_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = new IdFilter { Equal = CurrentUser.OrganizationId };
            AppUserFilter.SexId = ERoute_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Birthday = ERoute_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = ERoute_AppUserFilterDTO.ProvinceId;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ERoute_AppUserDTO> ERoute_AppUserDTOs = AppUsers
                .Select(x => new ERoute_AppUserDTO(x)).ToList();
            return ERoute_AppUserDTOs;
        }
        [Route(ERouteRoute.SingleListERouteType), HttpPost]
        public async Task<List<ERoute_ERouteTypeDTO>> SingleListERouteType([FromBody] ERoute_ERouteTypeFilterDTO ERoute_ERouteTypeFilterDTO)
        {
            ERouteTypeFilter ERouteTypeFilter = new ERouteTypeFilter();
            ERouteTypeFilter.Skip = 0;
            ERouteTypeFilter.Take = 20;
            ERouteTypeFilter.OrderBy = ERouteTypeOrder.Id;
            ERouteTypeFilter.OrderType = OrderType.ASC;
            ERouteTypeFilter.Selects = ERouteTypeSelect.ALL;
            ERouteTypeFilter.Id = ERoute_ERouteTypeFilterDTO.Id;
            ERouteTypeFilter.Code = ERoute_ERouteTypeFilterDTO.Code;
            ERouteTypeFilter.Name = ERoute_ERouteTypeFilterDTO.Name;

            List<ERouteType> ERouteTypes = await ERouteTypeService.List(ERouteTypeFilter);
            List<ERoute_ERouteTypeDTO> ERoute_ERouteTypeDTOs = ERouteTypes
                .Select(x => new ERoute_ERouteTypeDTO(x)).ToList();
            return ERoute_ERouteTypeDTOs;
        }
        [Route(ERouteRoute.SingleListOrganization), HttpPost]
        public async Task<List<ERoute_OrganizationDTO>> SingleListOrganization([FromBody] ERoute_OrganizationFilterDTO ERoute_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ERoute_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ERoute_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ERoute_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ERoute_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ERoute_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ERoute_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = ERoute_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = ERoute_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = ERoute_OrganizationFilterDTO.Email;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ERoute_OrganizationDTO> ERoute_OrganizationDTOs = Organizations
                .Select(x => new ERoute_OrganizationDTO(x)).ToList();
            return ERoute_OrganizationDTOs;
        }
        [Route(ERouteRoute.SingleListRequestState), HttpPost]
        public async Task<List<ERoute_RequestStateDTO>> SingleListRequestState([FromBody] ERoute_RequestStateFilterDTO ERoute_RequestStateFilterDTO)
        {
            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = ERoute_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = ERoute_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = ERoute_RequestStateFilterDTO.Name;

            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            List<ERoute_RequestStateDTO> ERoute_RequestStateDTOs = RequestStates
                .Select(x => new ERoute_RequestStateDTO(x)).ToList();
            return ERoute_RequestStateDTOs;
        }
        [Route(ERouteRoute.SingleListStore), HttpPost]
        public async Task<List<ERoute_StoreDTO>> SingleListStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = new IdFilter { Equal = CurrentUser.OrganizationId };
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ERoute_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ERoute_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ERoute_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERoute_StoreDTO> ERoute_StoreDTOs = Stores
                .Select(x => new ERoute_StoreDTO(x)).ToList();
            return ERoute_StoreDTOs;

        }
        [Route(ERouteRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<ERoute_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] ERoute_StoreGroupingFilterDTO ERoute_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ERoute_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ERoute_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ERoute_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ERoute_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ERoute_StoreGroupingDTO> ERoute_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ERoute_StoreGroupingDTO(x)).ToList();
            return ERoute_StoreGroupingDTOs;
        }
        [Route(ERouteRoute.SingleListStoreType), HttpPost]
        public async Task<List<ERoute_StoreTypeDTO>> SingleListStoreType([FromBody] ERoute_StoreTypeFilterDTO ERoute_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ERoute_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ERoute_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ERoute_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ERoute_StoreTypeDTO> ERoute_StoreTypeDTOs = StoreTypes
                .Select(x => new ERoute_StoreTypeDTO(x)).ToList();
            return ERoute_StoreTypeDTOs;
        }
        [Route(ERouteRoute.SingleListStatus), HttpPost]
        public async Task<List<ERoute_StatusDTO>> SingleListStatus([FromBody] ERoute_StatusFilterDTO ERoute_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ERoute_StatusDTO> ERoute_StatusDTOs = Statuses
                .Select(x => new ERoute_StatusDTO(x)).ToList();
            return ERoute_StatusDTOs;
        }

        [Route(ERouteRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERoute_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            if (ERoute_StoreFilterDTO.SaleEmployeeId.HasValue && ERoute_StoreFilterDTO.SaleEmployeeId.Equal.HasValue)
            {
                AppUser AppUser = await AppUserService.Get(ERoute_StoreFilterDTO.SaleEmployeeId.Equal.Value);
                var StoreIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
                StoreFilter.Id.In = StoreFilter.Id.In.Intersect(StoreIds).ToList();
            }
            StoreFilter = StoreService.ToFilter(StoreFilter);
            return await StoreService.Count(StoreFilter);
        }

        [Route(ERouteRoute.ListStore), HttpPost]
        public async Task<List<ERoute_StoreDTO>> ListStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            var CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = ERoute_StoreFilterDTO.Skip;
            StoreFilter.Take = ERoute_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERoute_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);

            if (ERoute_StoreFilterDTO.SaleEmployeeId != null && ERoute_StoreFilterDTO.SaleEmployeeId.Equal.HasValue)
            {
                AppUser AppUser = await AppUserService.Get(ERoute_StoreFilterDTO.SaleEmployeeId.Equal.Value);
                var StoreIds = AppUser.AppUserStoreMappings.Select(x => x.StoreId).ToList();
                StoreFilter.Id.In = StoreFilter.Id.In.Intersect(StoreIds).ToList();
            }

            StoreFilter = StoreService.ToFilter(StoreFilter);
            List<Store> Stores = await ERouteService.ListStore(StoreFilter);
            List<ERoute_StoreDTO> ERoute_StoreDTOs = Stores
                .Select(x => new ERoute_StoreDTO(x)).ToList();
            return ERoute_StoreDTOs;
        }
    }
}

