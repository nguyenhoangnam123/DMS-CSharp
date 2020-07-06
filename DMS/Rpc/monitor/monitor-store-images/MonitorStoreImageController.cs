using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_images
{
    public class MonitorStoreImageController : MonitorController
    {
        private DataContext DataContext;
        private IStoreService StoreService;
        private IStoreCheckingService StoreCheckingService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public MonitorStoreImageController
            (DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreCheckingService StoreCheckingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.StoreService = StoreService;
            this.StoreCheckingService = StoreCheckingService;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MonitorStoreImageRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorStoreImage_AppUserDTO>> FilterListAppUser([FromBody] MonitorStoreImage_AppUserFilterDTO MonitorStoreImage_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = MonitorStoreImage_AppUserFilterDTO.Id;
            AppUserFilter.Username = MonitorStoreImage_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = MonitorStoreImage_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService,OrganizationService,CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreImage_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreImage_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreImageRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreImage_OrganizationDTO>> FilterListOrganization([FromBody] MonitorStoreImage_OrganizationFilterDTO MonitorStoreImage_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<MonitorStoreImage_OrganizationDTO> StoreCheckerMonitor_OrganizationDTOs = Organizations
                .Select(x => new MonitorStoreImage_OrganizationDTO(x)).ToList();
            return StoreCheckerMonitor_OrganizationDTOs;
        }

        [Route(MonitorStoreImageRoute.FilterListStore), HttpPost]
        public async Task<List<MonitorStoreImage_StoreDTO>> FilterListStore([FromBody] MonitorStoreImage_StoreFilterDTO MonitorStoreImage_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = MonitorStoreImage_StoreFilterDTO.Id;
            StoreFilter.Code = MonitorStoreImage_StoreFilterDTO.Code;
            StoreFilter.Name = MonitorStoreImage_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = MonitorStoreImage_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = MonitorStoreImage_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = MonitorStoreImage_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = MonitorStoreImage_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = MonitorStoreImage_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = MonitorStoreImage_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = MonitorStoreImage_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = MonitorStoreImage_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = MonitorStoreImage_StoreFilterDTO.WardId;
            StoreFilter.Address = MonitorStoreImage_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = MonitorStoreImage_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = MonitorStoreImage_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = MonitorStoreImage_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = MonitorStoreImage_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = MonitorStoreImage_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = MonitorStoreImage_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = MonitorStoreImage_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = MonitorStoreImage_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<MonitorStoreImage_StoreDTO> MonitorStoreImage_StoreDTOs = Stores
                .Select(x => new MonitorStoreImage_StoreDTO(x)).ToList();
            return MonitorStoreImage_StoreDTOs;
        }

        [Route(MonitorStoreImageRoute.FilterListHasImage), HttpPost]
        public List<EnumList> FilterListHasImage()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không có ảnh" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có ảnh" });
            return EnumList;
        }

        [Route(MonitorStoreImageRoute.FilterListHasOrder), HttpPost]
        public List<EnumList> FilterListHasOrder()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không có đơn hàng" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có đơn hàng" });
            return EnumList;
        }

        [Route(MonitorStoreImageRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] MonitorStoreImage_MonitorStoreImageFilterDTO MonitorStoreImage_MonitorStoreImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? OrganizationId = MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreImage_MonitorStoreImageFilterDTO.AppUserId?.Equal;
            long? StoreId = MonitorStoreImage_MonitorStoreImageFilterDTO.StoreId?.Equal;
            long? HasImage = MonitorStoreImage_MonitorStoreImageFilterDTO.HasImage?.Equal;
            long? HasOrder = MonitorStoreImage_MonitorStoreImageFilterDTO.HasOrder?.Equal;

            DateTime Start = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow.Date :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            var query = from au in DataContext.AppUser
                        join sc in DataContext.StoreChecking on au.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationId.HasValue == false || au.OrganizationId == OrganizationId.Value) &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (
                            HasImage == null ||
                            (HasImage.Value == 0 && sc.ImageCounter == 0) ||
                            (HasImage.Value == 1 && sc.ImageCounter > 0)
                        ) &&
                        (
                            HasOrder == null ||
                            (HasOrder.Value == 0 && sc.IndirectSalesOrderCounter == 0) ||
                            (HasOrder.Value == 1 && sc.IndirectSalesOrderCounter > 0)
                        )
                        select au.Id;
            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(MonitorStoreImageRoute.List), HttpPost]
        public async Task<List<MonitorStoreImage_MonitorStoreImageDTO>> List([FromBody] MonitorStoreImage_MonitorStoreImageFilterDTO MonitorStoreImage_MonitorStoreImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? OrganizationId = MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreImage_MonitorStoreImageFilterDTO.AppUserId?.Equal;
            long? StoreId = MonitorStoreImage_MonitorStoreImageFilterDTO.StoreId?.Equal;
            long? HasImage = MonitorStoreImage_MonitorStoreImageFilterDTO.HasImage?.Equal;
            long? HasOrder = MonitorStoreImage_MonitorStoreImageFilterDTO.HasOrder?.Equal;

            DateTime Start = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow.Date :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value.Date;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);

            var query = from au in DataContext.AppUser
                        join sc in DataContext.StoreChecking on au.Id equals sc.SaleEmployeeId
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationId.HasValue == false || au.OrganizationId == OrganizationId.Value) &&
                        (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (
                            HasImage == null ||
                            (HasImage.Value == 0 && sc.ImageCounter == 0) ||
                            (HasImage.Value == 1 && sc.ImageCounter > 0)
                        ) &&
                        (
                            HasOrder == null ||
                            (HasOrder.Value == 0 && sc.IndirectSalesOrderCounter == 0) ||
                            (HasOrder.Value == 1 && sc.IndirectSalesOrderCounter > 0)
                        )
                        select au;
            List<AppUserDAO> SalesEmployees = await query.Distinct().OrderBy(q => q.DisplayName)
                .Skip(MonitorStoreImage_MonitorStoreImageFilterDTO.Skip)
                .Take(MonitorStoreImage_MonitorStoreImageFilterDTO.Take).ToListAsync();

            List<long> SaleEmployeeIds = SalesEmployees.Select(x => x.Id).ToList();

            var StoreCheckingQuery = from sc in DataContext.StoreChecking
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join o in DataContext.Organization on s.OrganizationId equals o.Id
                                     where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                                     (OrganizationId.HasValue == false || s.OrganizationId == OrganizationId.Value) &&
                                     (SaleEmployeeIds.Contains(sc.SaleEmployeeId)) &&
                                     (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                                     (
                                         HasImage == null ||
                                         (HasImage.Value == 0 && sc.ImageCounter == 0) ||
                                         (HasImage.Value == 1 && sc.ImageCounter > 0)
                                     ) &&
                                     (
                                         HasOrder == null ||
                                         (HasOrder.Value == 0 && sc.IndirectSalesOrderCounter == 0) ||
                                         (HasOrder.Value == 1 && sc.IndirectSalesOrderCounter > 0)
                                     )
                                     select sc;

            List<StoreCheckingDAO> StoreCheckingDAOs = await StoreCheckingQuery.Include(s => s.Store).ToListAsync();

            List<long> OrganizationIds = SalesEmployees.Where(x => x.OrganizationId.HasValue).Select(x => x.OrganizationId.Value).ToList();
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Name,
                Id = new IdFilter { In = OrganizationIds }
            });
            //build
            List<MonitorStoreImage_SaleEmployeeDTO> MonitorStoreImage_SaleEmployeeDTOs = new List<MonitorStoreImage_SaleEmployeeDTO>();
            foreach (var SalesEmployee in SalesEmployees)
            {
                MonitorStoreImage_SaleEmployeeDTO MonitorStoreImage_SaleEmployeeDTO = new MonitorStoreImage_SaleEmployeeDTO
                {
                    DisplayName = SalesEmployee.DisplayName,
                    Username = SalesEmployee.Username,
                    SaleEmployeeId = SalesEmployee.Id,
                    OrganizationName = SalesEmployee.Organization.Name
                };

                MonitorStoreImage_SaleEmployeeDTO.StoreCheckings = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == SalesEmployee.Id)
                    .GroupBy(x => new { x.CheckOutAt.Value.Date, x.StoreId, x.Store.Name })
                    .Select(y => new MonitorStoreImage_StoreCheckingDTO
                    {
                        StoreId = y.Key.StoreId,
                        Date = y.Key.Date,
                        StoreName = y.Key.Name,
                        ImageCounter = y.Select(i => i.ImageCounter ?? 0).DefaultIfEmpty().Sum()
                    }).ToList();

                MonitorStoreImage_SaleEmployeeDTOs.Add(MonitorStoreImage_SaleEmployeeDTO);
            }

            List<MonitorStoreImage_MonitorStoreImageDTO> MonitorStoreImage_MonitorStoreImageDTOs = new List<MonitorStoreImage_MonitorStoreImageDTO>();
            foreach (Organization Organization in Organizations)
            {
                MonitorStoreImage_MonitorStoreImageDTO MonitorStoreImage_MonitorStoreImageDTO = new MonitorStoreImage_MonitorStoreImageDTO()
                {
                    OrganizationName = Organization.Name,
                    SaleEmployees = MonitorStoreImage_SaleEmployeeDTOs.Where(x => x.OrganizationName.Equals(Organization.Name)).ToList()
                };
                MonitorStoreImage_MonitorStoreImageDTOs.Add(MonitorStoreImage_MonitorStoreImageDTO);
            }

            MonitorStoreImage_MonitorStoreImageDTOs = MonitorStoreImage_MonitorStoreImageDTOs.Where(si => si.SaleEmployees.Count > 0).ToList();
            return MonitorStoreImage_MonitorStoreImageDTOs;
        }

        [Route(MonitorStoreImageRoute.Get), HttpPost]
        public async Task<List<MonitorStoreImage_StoreCheckingImageMappingDTO>> Get([FromBody] MonitorStoreImage_StoreCheckingDTO MonitorStoreImage_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from scim in DataContext.StoreCheckingImageMapping
                        join sc in DataContext.StoreChecking on scim.StoreCheckingId equals sc.Id
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join a in DataContext.Album on scim.AlbumId equals a.Id
                        join i in DataContext.Image on scim.ImageId equals i.Id
                        join au in DataContext.AppUser on scim.SaleEmployeeId equals au.Id
                        where scim.StoreId == MonitorStoreImage_StoreCheckingDTO.StoreId &&
                        scim.SaleEmployeeId == MonitorStoreImage_StoreCheckingDTO.SaleEmployeeId &&
                        sc.CheckOutAt.Value.Date == MonitorStoreImage_StoreCheckingDTO.Date.Date
                        select new MonitorStoreImage_StoreCheckingImageMappingDTO
                        {
                            AlbumId = scim.AlbumId,
                            ImageId = scim.ImageId,
                            SaleEmployeeId = scim.SaleEmployeeId,
                            ShootingAt = scim.ShootingAt,
                            StoreCheckingId = scim.StoreCheckingId,
                            StoreId = scim.StoreId,
                            Album = new MonitorStoreImage_AlbumDTO
                            {
                                Name = a.Name
                            },
                            Image = new MonitorStoreImage_ImageDTO
                            {
                                Url = i.Url
                            },
                            SaleEmployee = new MonitorStoreImage_AppUserDTO
                            {
                                DisplayName = au.DisplayName
                            },
                            Store = new MonitorStoreImage_StoreDTO
                            {
                                Name = s.Name
                            }
                        };

            return await query.ToListAsync();
        }
    }
}
