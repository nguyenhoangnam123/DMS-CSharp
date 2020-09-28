using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAlbum;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreChecking;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGS.Templater;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_images
{
    public class MonitorStoreImageController : MonitorController
    {
        private DataContext DataContext;
        private IAlbumService AlbumService;
        private IStoreService StoreService;
        private IStoreCheckingService StoreCheckingService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public MonitorStoreImageController
            (DataContext DataContext,
            IAlbumService AlbumService,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreCheckingService StoreCheckingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AlbumService = AlbumService;
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
            AppUserFilter.OrganizationId = MonitorStoreImage_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Username = MonitorStoreImage_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = MonitorStoreImage_AppUserFilterDTO.DisplayName;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreImage_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreImage_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreImageRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreImage_OrganizationDTO>> FilterListOrganization()
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

        [Route(MonitorStoreImageRoute.SingleListAlbum), HttpPost]
        public async Task<List<MonitorStoreImage_AlbumDTO>> SingleListAlbum([FromBody] MonitorStoreImage_AlbumFilterDTO MonitorStoreImage_AlbumFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AlbumFilter AlbumFilter = new AlbumFilter();
            AlbumFilter.Skip = 0;
            AlbumFilter.Take = 20;
            AlbumFilter.OrderBy = AlbumOrder.Id;
            AlbumFilter.OrderType = OrderType.ASC;
            AlbumFilter.Selects = AlbumSelect.ALL;
            AlbumFilter.Id = MonitorStoreImage_AlbumFilterDTO.Id;
            AlbumFilter.Name = MonitorStoreImage_AlbumFilterDTO.Name;
            AlbumFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Album> Albums = await AlbumService.List(AlbumFilter);
            List<MonitorStoreImage_AlbumDTO> MonitorStoreImage_AlbumDTOs = Albums
                .Select(x => new MonitorStoreImage_AlbumDTO(x)).ToList();
            return MonitorStoreImage_AlbumDTOs;
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
                    LocalStartDay(CurrentContext) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var query = from sc in DataContext.StoreChecking
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        FilterAppUserIds.Contains(sc.SaleEmployeeId) &&
                        OrganizationIds.Contains(au.OrganizationId) &&
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
                        select sc.SaleEmployeeId;
            var SaleEmployeeIds = await query.Distinct().ToListAsync();
            var query2 = from aim in DataContext.AlbumImageMapping
                         where Start <= aim.ShootingAt && aim.ShootingAt <= End &&
                        (aim.SaleEmployeeId.HasValue && FilterAppUserIds.Contains(aim.SaleEmployeeId.Value)) &&
                        (SaleEmployeeId.HasValue == false || aim.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || aim.StoreId == StoreId.Value)
                         select aim.SaleEmployeeId.Value;
            var SaleEmployeeIds2 = await query2.Distinct().ToListAsync();
            var Ids = new List<long>();
            Ids.AddRange(SaleEmployeeIds);
            Ids.AddRange(SaleEmployeeIds2);
            Ids = Ids.Distinct().ToList();
            int count = Ids.Count();
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
                    LocalStartDay(CurrentContext) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreImage_MonitorStoreImageFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> FilterAppUserIds = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var query = from au in DataContext.AppUser
                        join sc in DataContext.StoreChecking on au.Id equals sc.SaleEmployeeId
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                        FilterAppUserIds.Contains(au.Id) &&
                        OrganizationIds.Contains(au.OrganizationId) &&
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
            var SaleEmployeeIds = await query.Distinct().ToListAsync();
            var query2 = from aim in DataContext.AlbumImageMapping
                         where Start <= aim.ShootingAt && aim.ShootingAt <= End &&
                        (aim.SaleEmployeeId.HasValue && FilterAppUserIds.Contains(aim.SaleEmployeeId.Value)) &&
                        (SaleEmployeeId.HasValue == false || aim.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (StoreId.HasValue == false || aim.StoreId == StoreId.Value)
                         select aim.SaleEmployeeId.Value;
            var SaleEmployeeIds2 = await query2.Distinct().ToListAsync();
            var Ids = new List<long>();
            Ids.AddRange(SaleEmployeeIds);
            Ids.AddRange(SaleEmployeeIds2);
            Ids = Ids.Distinct().ToList();
            List<AppUserDAO> SalesEmployees = await DataContext.AppUser
                .Where(x => Ids.Contains(x.Id))
                .OrderBy(q => q.OrganizationId).ThenBy(q => q.DisplayName)
                .Skip(MonitorStoreImage_MonitorStoreImageFilterDTO.Skip)
                .Take(MonitorStoreImage_MonitorStoreImageFilterDTO.Take)
                .ToListAsync();

            SaleEmployeeIds = SalesEmployees.Select(x => x.Id).ToList();

            var StoreCheckingQuery = from sc in DataContext.StoreChecking
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                                     OrganizationIds.Contains(au.OrganizationId) &&
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

            var AlbumImageQuery = from aim in DataContext.AlbumImageMapping
                                  join s in DataContext.Store on aim.StoreId equals s.Id
                                  join au in DataContext.AppUser on aim.SaleEmployeeId equals au.Id
                                  where Start <= aim.ShootingAt && aim.ShootingAt <= End &&
                                  OrganizationIds.Contains(au.OrganizationId) &&
                                  (aim.SaleEmployeeId.HasValue && SaleEmployeeIds.Contains(aim.SaleEmployeeId.Value))
                                  select new AlbumImageMapping 
                                  {
                                      AlbumId = aim.AlbumId,
                                      ImageId = aim.ImageId,
                                      SaleEmployeeId = aim.SaleEmployeeId,
                                      ShootingAt = aim.ShootingAt,
                                      StoreId = aim.StoreId,
                                      Store = new Store
                                      {
                                          Name = s.Name
                                      }
                                  };

            var AlbumImageMappingDAOs = await AlbumImageQuery.ToListAsync();

            OrganizationIds = SalesEmployees.Select(x => x.OrganizationId).ToList();
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Id | OrganizationSelect.Name | OrganizationSelect.Path,
                OrderBy = OrganizationOrder.Path,
                OrderType = OrderType.ASC,
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
                    OrganizationName = SalesEmployee.Organization.Name,
                    OrganizationPath = SalesEmployee.Organization.Path
                };

                MonitorStoreImage_SaleEmployeeDTO.StoreCheckings = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == SalesEmployee.Id)
                    .GroupBy(x => new { 
                        x.CheckOutAt.Value.AddHours(CurrentContext.TimeZone).Date, 
                        x.StoreId, 
                        x.Store.Name 
                    })
                    .Select(y => new MonitorStoreImage_DetailDTO
                    {
                        StoreId = y.Key.StoreId,
                        Date = y.Key.Date,
                        StoreName = y.Key.Name,
                        ImageCounter = y.Select(i => i.ImageCounter ?? 0).DefaultIfEmpty().Sum()
                    }).ToList();

                var AlbumImageMappings = AlbumImageMappingDAOs.Where(x => x.SaleEmployeeId == SalesEmployee.Id).ToList();
                var StoreIds = AlbumImageMappings.Select(x => x.StoreId).Distinct().ToList();
                foreach (var storeId in StoreIds)
                {
                    var dates = AlbumImageMappings.OrderByDescending(x => x.ShootingAt).Select(x => x.ShootingAt.Date).Distinct().ToList();
                    foreach (var date in dates)
                    {
                        var row = MonitorStoreImage_SaleEmployeeDTO.StoreCheckings.Where(x => x.StoreId == storeId && x.Date == date.AddHours(CurrentContext.TimeZone).Date).FirstOrDefault();
                        if(row == null)
                        {
                            row = new MonitorStoreImage_DetailDTO();
                            row.Date = date.AddHours(CurrentContext.TimeZone).Date;
                            row.ImageCounter = AlbumImageMappings.Where(x => x.StoreId == storeId && x.ShootingAt.AddHours(CurrentContext.TimeZone).Date == date.AddHours(CurrentContext.TimeZone).Date).Count();
                            row.SaleEmployeeId = SalesEmployee.Id;
                            row.StoreId = storeId;
                            row.StoreName = AlbumImageMappings.Where(x => x.StoreId == storeId).Select(x => x.Store.Name).FirstOrDefault();
                            MonitorStoreImage_SaleEmployeeDTO.StoreCheckings.Add(row);
                        }
                        else
                        {
                            row.ImageCounter += AlbumImageMappings.Where(x => x.StoreId == storeId && x.ShootingAt.AddHours(CurrentContext.TimeZone).Date == date.AddHours(CurrentContext.TimeZone).Date).Count();
                        }
                    }
                }

                MonitorStoreImage_SaleEmployeeDTOs.Add(MonitorStoreImage_SaleEmployeeDTO);
            }

            List<MonitorStoreImage_MonitorStoreImageDTO> MonitorStoreImage_MonitorStoreImageDTOs = new List<MonitorStoreImage_MonitorStoreImageDTO>();
            foreach (Organization Organization in Organizations)
            {
                MonitorStoreImage_MonitorStoreImageDTO MonitorStoreImage_MonitorStoreImageDTO = new MonitorStoreImage_MonitorStoreImageDTO()
                {
                    OrganizationName = Organization.Name,
                    SaleEmployees = MonitorStoreImage_SaleEmployeeDTOs.Where(x => x.OrganizationPath.Equals(Organization.Path)).ToList()
                };
                MonitorStoreImage_MonitorStoreImageDTOs.Add(MonitorStoreImage_MonitorStoreImageDTO);
            }

            MonitorStoreImage_MonitorStoreImageDTOs = MonitorStoreImage_MonitorStoreImageDTOs.Where(si => si.SaleEmployees.Count > 0).ToList();

            return MonitorStoreImage_MonitorStoreImageDTOs;
        }

        [Route(MonitorStoreImageRoute.Get), HttpPost]
        public async Task<List<MonitorStoreImage_StoreCheckingImageMappingDTO>> Get([FromBody] MonitorStoreImage_DetailDTO MonitorStoreImage_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = MonitorStoreImage_StoreCheckingDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            var query = from scim in DataContext.StoreCheckingImageMapping
                        join sc in DataContext.StoreChecking on scim.StoreCheckingId equals sc.Id
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join a in DataContext.Album on scim.AlbumId equals a.Id
                        join i in DataContext.Image on scim.ImageId equals i.Id
                        join au in DataContext.AppUser on scim.SaleEmployeeId equals au.Id
                        where scim.StoreId == MonitorStoreImage_StoreCheckingDTO.StoreId &&
                        scim.SaleEmployeeId == MonitorStoreImage_StoreCheckingDTO.SaleEmployeeId &&
                        Start <= scim.ShootingAt && scim.ShootingAt <= End
                        select new MonitorStoreImage_StoreCheckingImageMappingDTO
                        {
                            AlbumId = scim.AlbumId,
                            ImageId = scim.ImageId,
                            SaleEmployeeId = scim.SaleEmployeeId,
                            ShootingAt = scim.ShootingAt,
                            StoreCheckingId = scim.StoreCheckingId,
                            StoreId = scim.StoreId,
                            Distance = scim.Distance,
                            Album = new MonitorStoreImage_AlbumDTO
                            {
                                Id = a.Id,
                                Name = a.Name
                            },
                            Image = new MonitorStoreImage_ImageDTO
                            {
                                Id = i.Id,
                                Url = i.Url
                            },
                            SaleEmployee = new MonitorStoreImage_AppUserDTO
                            {
                                Id = au.Id,
                                DisplayName = au.DisplayName
                            },
                            Store = new MonitorStoreImage_StoreDTO
                            {
                                Id = s.Id,
                                Address = s.Address,
                                Name = s.Name
                            }
                        };

            var query2 = from aim in DataContext.AlbumImageMapping
                        join s in DataContext.Store on aim.StoreId equals s.Id
                        join a in DataContext.Album on aim.AlbumId equals a.Id
                        join i in DataContext.Image on aim.ImageId equals i.Id
                        join au in DataContext.AppUser on aim.SaleEmployeeId equals au.Id
                        where aim.StoreId == MonitorStoreImage_StoreCheckingDTO.StoreId &&
                        (aim.SaleEmployeeId.HasValue && aim.SaleEmployeeId == MonitorStoreImage_StoreCheckingDTO.SaleEmployeeId) &&
                        Start <= aim.ShootingAt && aim.ShootingAt <= End
                         select new MonitorStoreImage_StoreCheckingImageMappingDTO
                        {
                            AlbumId = aim.AlbumId,
                            ImageId = aim.ImageId,
                            SaleEmployeeId = aim.SaleEmployeeId.Value,
                            ShootingAt = aim.ShootingAt,
                            StoreId = aim.StoreId,
                            Album = new MonitorStoreImage_AlbumDTO
                            {
                                Id = a.Id,
                                Name = a.Name
                            },
                            Image = new MonitorStoreImage_ImageDTO
                            {
                                Id = i.Id,
                                Url = i.Url
                            },
                            SaleEmployee = new MonitorStoreImage_AppUserDTO
                            {
                                Id = au.Id,
                                DisplayName = au.DisplayName
                            },
                            Store = new MonitorStoreImage_StoreDTO
                            {
                                Id = s.Id,
                                Address = s.Address,
                                Name = s.Name
                            }
                        };
            var result = await query.ToListAsync();
            var result2 = await query2.ToListAsync();
            result.AddRange(result2);
            return result;
        }

        [Route(MonitorStoreImageRoute.UpdateAlbum), HttpPost]
        public async Task<MonitorStoreImage_StoreCheckingImageMappingDTO> UpdateAlbum([FromBody] MonitorStoreImage_StoreCheckingImageMappingDTO MonitorStoreImage_StoreCheckingImageMappingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var AlbumImageMappingDAO = await DataContext
                .AlbumImageMapping.Where(x => x.ImageId == MonitorStoreImage_StoreCheckingImageMappingDTO.ImageId)
                .FirstOrDefaultAsync();
            if(AlbumImageMappingDAO != null)
            {
                AlbumImageMappingDAO.AlbumId = MonitorStoreImage_StoreCheckingImageMappingDTO.AlbumId;
                await DataContext.SaveChangesAsync();
            }
            else
            {
                var StoreCheckingImageMappingDAO = await DataContext
                .StoreCheckingImageMapping.Where(x => x.ImageId == MonitorStoreImage_StoreCheckingImageMappingDTO.ImageId &&
                x.StoreCheckingId == MonitorStoreImage_StoreCheckingImageMappingDTO.StoreCheckingId)
                .FirstOrDefaultAsync();
                if (StoreCheckingImageMappingDAO != null)
                {
                    StoreCheckingImageMappingDAO.AlbumId = MonitorStoreImage_StoreCheckingImageMappingDTO.AlbumId;
                    await DataContext.SaveChangesAsync();
                }
            }
            
            return MonitorStoreImage_StoreCheckingImageMappingDTO;
        }

        [Route(MonitorStoreImageRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MonitorStoreImage_MonitorStoreImageFilterDTO MonitorStoreImage_MonitorStoreImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MonitorStoreImage_MonitorStoreImageFilterDTO.Skip = 0;
            MonitorStoreImage_MonitorStoreImageFilterDTO.Take = int.MaxValue;
            List<MonitorStoreImage_MonitorStoreImageDTO> MonitorStoreImage_MonitorStoreImageDTOs = await List(MonitorStoreImage_MonitorStoreImageFilterDTO);
            long stt = 1;
            foreach (MonitorStoreImage_MonitorStoreImageDTO MonitorStoreImage_MonitorStoreImageDTO in MonitorStoreImage_MonitorStoreImageDTOs)
            {
                foreach (var SaleEmployee in MonitorStoreImage_MonitorStoreImageDTO.SaleEmployees)
                {
                    foreach (var StoreChecking in SaleEmployee.StoreCheckings)
                    {
                        StoreChecking.STT = stt;
                        stt++;
                    }
                }
            }

            DateTime Start = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreImage_MonitorStoreImageFilterDTO.CheckIn.LessEqual.Value;

            string path = "Templates/Monitor_Store_Image.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.MonitorStoreImages = MonitorStoreImage_MonitorStoreImageDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "Monitor_Store_Image.xlsx");
        }
    }
}
