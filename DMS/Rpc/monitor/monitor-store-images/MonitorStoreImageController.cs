using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_images
{
    public class MonitorStoreImageController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;
        private IStoreService StoreService;

        public MonitorStoreImageController
            (DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
            this.StoreService = StoreService;
        }

        [Route(MonitorStoreImageRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorStoreImage_AppUserDTO>> FilterListAppUser([FromBody] MonitorStoreImage_AppUserFilterDTO MonitorStoreImage_AppUserFilterDTO)
        {
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

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreImage_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreImage_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreImageRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreImage_OrganizationDTO>> FilterListOrganization([FromBody] MonitorStoreImage_OrganizationFilterDTO MonitorStoreImage_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

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
            long? OrganizationId = MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreImage_MonitorStoreImageFilterDTO.SaleEmployeeId?.Equal;
            long? StoreId = MonitorStoreImage_MonitorStoreImageFilterDTO.StoreId?.Equal;
            long? HasImage = MonitorStoreImage_MonitorStoreImageFilterDTO.HasImage?.Equal;
            long? HasOrder = MonitorStoreImage_MonitorStoreImageFilterDTO.HasOrder?.Equal;
            AppUserDAO AppUserDAO = null;
            if (SaleEmployeeId != null)
            {
                AppUserDAO = await DataContext.AppUser.Where(x => x.Id == SaleEmployeeId.Value).FirstOrDefaultAsync();
            }
            OrganizationDAO OrganizationDAO = null;
            if (OrganizationId != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == OrganizationId.Value).FirstOrDefaultAsync();
            }
            StoreDAO StoreDAO = null;
            if (StoreId != null)
            {
                StoreDAO = await DataContext.Store.Where(o => o.Id == StoreId.Value).FirstOrDefaultAsync();
            }
            DateTime Start = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationDAO == null || ap.OrganizationId == OrganizationDAO.Id) &&
                        (AppUserDAO == null || ap.Id == SaleEmployeeId.Value) &&
                        (StoreDAO == null || sc.StoreId == StoreId.Value) &&
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
                        select ap.Id;
            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(MonitorStoreImageRoute.List), HttpPost]
        public async Task<List<MonitorStoreImage_MonitorStoreImageDTO>> List(MonitorStoreImage_MonitorStoreImageFilterDTO MonitorStoreImage_MonitorStoreImageFilterDTO)
        {
            long? OrganizationId = MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId?.Equal;
            long? SaleEmployeeId = MonitorStoreImage_MonitorStoreImageFilterDTO.SaleEmployeeId?.Equal;
            long? StoreId = MonitorStoreImage_MonitorStoreImageFilterDTO.StoreId?.Equal;
            long? HasImage = MonitorStoreImage_MonitorStoreImageFilterDTO.HasImage?.Equal;
            long? HasOrder = MonitorStoreImage_MonitorStoreImageFilterDTO.HasOrder?.Equal;
            AppUserDAO AppUserDAO = null;
            if(SaleEmployeeId != null)
            {
                AppUserDAO = await DataContext.AppUser.Where(x => x.Id == SaleEmployeeId.Value).FirstOrDefaultAsync();
            }
            OrganizationDAO OrganizationDAO = null;
            if (OrganizationId != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == OrganizationId.Value).FirstOrDefaultAsync();
            }
            StoreDAO StoreDAO = null;
            if (StoreId != null)
            {
                StoreDAO = await DataContext.Store.Where(o => o.Id == StoreId.Value).FirstOrDefaultAsync();
            }
            DateTime Start = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            var query = from ap in DataContext.AppUser
                        join sc in DataContext.StoreChecking on ap.Id equals sc.SaleEmployeeId
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        (OrganizationDAO == null || ap.OrganizationId == OrganizationDAO.Id) &&
                        (AppUserDAO == null || ap.Id == SaleEmployeeId.Value) &&
                        (StoreDAO == null || sc.StoreId == StoreId.Value)
                        select new MonitorStoreImage_SaleEmployeeDTO
                        {
                            SaleEmployeeId = ap.Id,
                            Username = ap.Username,
                            DisplayName = ap.DisplayName,
                            OrganizationName = ap.Organization == null ? null : ap.Organization.Name,
                        };
            List<MonitorStoreImage_SaleEmployeeDTO> MonitorStoreImage_SaleEmployeeDTOs = await query.Distinct().OrderBy(q => q.DisplayName)
                .Skip(MonitorStoreImage_MonitorStoreImageFilterDTO.Skip)
                .Take(MonitorStoreImage_MonitorStoreImageFilterDTO.Take)
                .ToListAsync();
            List<string> OrganizationNames = MonitorStoreImage_SaleEmployeeDTOs.Select(se => se.OrganizationName).Distinct().ToList();
            List<MonitorStoreImage_MonitorStoreImageDTO> MonitorStoreImage_MonitorStoreImageDTOs = OrganizationNames.Select(on => new MonitorStoreImage_MonitorStoreImageDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (MonitorStoreImage_MonitorStoreImageDTO MonitorStoreImage_MonitorStoreImageDTO in MonitorStoreImage_MonitorStoreImageDTOs)
            {
                MonitorStoreImage_MonitorStoreImageDTO.SaleEmployees = MonitorStoreImage_SaleEmployeeDTOs
                    .Where(se => se.OrganizationName == MonitorStoreImage_MonitorStoreImageDTO.OrganizationName)
                    .ToList();
            }
            List<long> AppUserIds = MonitorStoreImage_SaleEmployeeDTOs.Select(s => s.SaleEmployeeId).ToList();

            List<StoreCheckingDAO> StoreCheckingDAOs = new List<StoreCheckingDAO>();
            var StoreCheckingQuery = DataContext.StoreChecking
                .Where(sc => AppUserIds.Contains(sc.SaleEmployeeId) && 
                sc.StoreId == StoreId.Value &&
                sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End);
            if (MonitorStoreImage_MonitorStoreImageFilterDTO.HasImage?.Equal != null)
                if (MonitorStoreImage_MonitorStoreImageFilterDTO.HasImage.Equal == 0)
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.ImageCounter == 0);
                }
                else
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.ImageCounter > 0);
                }

            if (MonitorStoreImage_MonitorStoreImageFilterDTO.HasOrder?.Equal != null)
                if (MonitorStoreImage_MonitorStoreImageFilterDTO.HasOrder.Equal == 0)
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.IndirectSalesOrderCounter == 0);
                }
                else
                {
                    StoreCheckingQuery = StoreCheckingQuery.Where(sc => sc.IndirectSalesOrderCounter > 0);
                }
            StoreCheckingDAOs = await StoreCheckingQuery.ToListAsync();

            foreach (MonitorStoreImage_SaleEmployeeDTO MonitorStoreImage_SaleEmployeeDTO in MonitorStoreImage_SaleEmployeeDTOs)
            {
                MonitorStoreImage_SaleEmployeeDTO.StoreCheckings = new List<MonitorStoreImage_StoreCheckingDTO>();
                for (DateTime i = Start; i < End; i = i.AddDays(1))
                {
                    var StoreCheckingDAO = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == MonitorStoreImage_SaleEmployeeDTO.SaleEmployeeId &&
                    x.CheckOutAt.HasValue && i <= x.CheckOutAt.Value && x.CheckOutAt.Value <= i.AddDays(1)).FirstOrDefault();
                    MonitorStoreImage_StoreCheckingDTO MonitorStoreImage_StoreCheckingDTO = new MonitorStoreImage_StoreCheckingDTO();
                    MonitorStoreImage_StoreCheckingDTO.Date = i;
                    MonitorStoreImage_StoreCheckingDTO.StoreName = StoreCheckingDAO.Store.Name;
                    MonitorStoreImage_StoreCheckingDTO.Image = new HashSet<long>();
                    MonitorStoreImage_SaleEmployeeDTO.StoreCheckings.Add(MonitorStoreImage_StoreCheckingDTO);
                }
            }

            return MonitorStoreImage_MonitorStoreImageDTOs;
        }
    }
}
