using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
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
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

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

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var StoreCheckingQuery = from sc in DataContext.StoreChecking
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                     where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                                     OrganizationIds.Contains(s.OrganizationId) &&
                                     (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                                     (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                                     (
                                         HasImage == null ||
                                         (HasImage.Value == 0 && sc.ImageCounter == 0) ||
                                         (HasImage.Value == 1 && sc.ImageCounter > 0)
                                     )
                                     group au by new { au.Id, s.OrganizationId } into x
                                     select new
                                     {
                                         SalesEmployeeId = x.Key.Id,
                                         OrganizationId = x.Key.OrganizationId,
                                     };

            var Ids = await StoreCheckingQuery.ToListAsync();
            var StoreImageQuery = from si in DataContext.StoreImage
                                  join tt in tempTableQuery.Query on si.StoreId equals tt.Column1
                                  where Start <= si.ShootingAt && si.ShootingAt <= End &&
                                 OrganizationIds.Contains(si.OrganizationId) &&
                                 (si.SaleEmployeeId.HasValue && (SaleEmployeeId.HasValue == false || si.SaleEmployeeId == SaleEmployeeId.Value)) &&
                                 (StoreId.HasValue == false || si.StoreId == StoreId.Value)
                                  select si;
            var Ids2 = await StoreImageQuery.Select(x => new {
                SalesEmployeeId = x.SaleEmployeeId.Value,
                OrganizationId = x.OrganizationId,
            }).ToListAsync();
            Ids.AddRange(Ids2);
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
            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var StoreCheckingQuery = from sc in DataContext.StoreChecking
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                     where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                                     OrganizationIds.Contains(s.OrganizationId) &&
                                     (SaleEmployeeId.HasValue == false || sc.SaleEmployeeId == SaleEmployeeId.Value) &&
                                     (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                                     (
                                         HasImage == null ||
                                         (HasImage.Value == 0 && sc.ImageCounter == 0) ||
                                         (HasImage.Value == 1 && sc.ImageCounter > 0)
                                     )
                                     group au by new { au.Id, s.OrganizationId } into x
                                     select new
                                     {
                                         SalesEmployeeId = x.Key.Id,
                                         OrganizationId = x.Key.OrganizationId,
                                     };
            
            var Ids = await StoreCheckingQuery.ToListAsync();
            var StoreImageQuery = from si in DataContext.StoreImage
                                  join tt in tempTableQuery.Query on si.StoreId equals tt.Column1
                                  where Start <= si.ShootingAt && si.ShootingAt <= End &&
                                 OrganizationIds.Contains(si.OrganizationId) &&
                                 (si.SaleEmployeeId.HasValue &&(SaleEmployeeId.HasValue == false || si.SaleEmployeeId == SaleEmployeeId.Value)) &&
                                 (StoreId.HasValue == false || si.StoreId == StoreId.Value)
                                  select si;
            var Ids2 = await StoreImageQuery.Select(x => new {
                SalesEmployeeId = x.SaleEmployeeId.Value,
                OrganizationId = x.OrganizationId,
            }).ToListAsync();
            Ids.AddRange(Ids2);
            Ids = Ids
                .OrderBy(x => x.OrganizationId)
                .Distinct()
                .Skip(MonitorStoreImage_MonitorStoreImageFilterDTO.Skip)
                .Take(MonitorStoreImage_MonitorStoreImageFilterDTO.Take)
                .ToList();
            var AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();
            List<AppUserDAO> SalesEmployees = await DataContext.AppUser
                .Where(x => AppUserIds.Contains(x.Id))
                .OrderBy(q => q.OrganizationId).ThenBy(q => q.DisplayName)
                .Select(x => new AppUserDAO
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Username = x.Username,
                })
                .ToListAsync();

            var CheckingQuery = from sc in DataContext.StoreChecking
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                     where sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                                     AppUserIds.Contains(sc.SaleEmployeeId) &&
                                     (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                                     (
                                         HasImage == null ||
                                         (HasImage.Value == 0 && sc.ImageCounter == 0) ||
                                         (HasImage.Value == 1 && sc.ImageCounter > 0)
                                     )
                                     select new StoreCheckingDAO
                                     {
                                         Id = sc.Id,
                                         StoreId = sc.StoreId,
                                         CheckOutAt = sc.CheckOutAt,
                                         ImageCounter = sc.ImageCounter,
                                         SaleEmployeeId = sc.SaleEmployeeId,
                                         Store = new StoreDAO
                                         {
                                             Id = s.Id,
                                             Name = s.Name,
                                             OrganizationId = s.OrganizationId
                                         }
                                     };

            List<StoreCheckingDAO> StoreCheckingDAOs = await CheckingQuery.ToListAsync();
            var StoreImageDAOs = await StoreImageQuery.ToListAsync();

            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
            //build
            List<MonitorStoreImage_MonitorStoreImageDTO> MonitorStoreImage_MonitorStoreImageDTOs = new List<MonitorStoreImage_MonitorStoreImageDTO>();
            foreach (var Organization in Organizations)
            {
                MonitorStoreImage_MonitorStoreImageDTO MonitorStoreImage_MonitorStoreImageDTO = new MonitorStoreImage_MonitorStoreImageDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<MonitorStoreImage_SaleEmployeeDTO>()
                };
                MonitorStoreImage_MonitorStoreImageDTO.SaleEmployees = Ids.Select(x => new MonitorStoreImage_SaleEmployeeDTO
                {
                    SaleEmployeeId = x.SalesEmployeeId
                }).ToList();
                MonitorStoreImage_MonitorStoreImageDTOs.Add(MonitorStoreImage_MonitorStoreImageDTO);
            }

            foreach (var MonitorStoreImage_MonitorStoreImageDTO in MonitorStoreImage_MonitorStoreImageDTOs)
            {
                Parallel.ForEach(MonitorStoreImage_MonitorStoreImageDTO.SaleEmployees, SaleEmployee =>
                {
                    var appUser = SalesEmployees.Where(x => x.Id == SaleEmployee.SaleEmployeeId).FirstOrDefault();
                    if(appUser != null)
                    {
                        SaleEmployee.Username = appUser.Username;
                        SaleEmployee.DisplayName = appUser.DisplayName;
                        SaleEmployee.StoreCheckings = new List<MonitorStoreImage_DetailDTO>();
                    }
                });

                foreach (var SalesEmployee in MonitorStoreImage_MonitorStoreImageDTO.SaleEmployees)
                {
                    List<Tuple<DateTime, long>> Date1s = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == SalesEmployee.SaleEmployeeId).Select(x => new Tuple<DateTime, long>(x.CheckOutAt.Value.AddHours(CurrentContext.TimeZone).Date, x.StoreId)).ToList();
                    List<Tuple<DateTime, long>> Date2s = StoreImageDAOs.Where(x => x.SaleEmployeeId.HasValue && x.SaleEmployeeId.Value == SalesEmployee.SaleEmployeeId).Select(x => new Tuple<DateTime, long>(x.ShootingAt.AddHours(CurrentContext.TimeZone).Date, x.StoreId)).ToList();

                    List<Tuple<DateTime, long>> Dates = new List<Tuple<DateTime, long>>();
                    Dates.AddRange(Date1s);
                    Dates.AddRange(Date2s);
                    Dates = Dates.Distinct().ToList();

                    Parallel.ForEach(Dates, Date =>
                    {
                        var Checkings = StoreCheckingDAOs
                            .Where(x => x.SaleEmployeeId == SalesEmployee.SaleEmployeeId &&
                            x.CheckOutAt.Value.AddHours(CurrentContext.TimeZone).Date == Date.Item1 &&
                            x.StoreId == Date.Item2)
                            .ToList();
                        var Images = StoreImageDAOs
                            .Where(x => x.SaleEmployeeId == SalesEmployee.SaleEmployeeId &&
                            x.ShootingAt.AddHours(CurrentContext.TimeZone).Date == Date.Item1 &&
                            x.StoreId == Date.Item2)
                            .ToList();

                        MonitorStoreImage_DetailDTO MonitorStoreImage_DetailDTO = new MonitorStoreImage_DetailDTO()
                        {
                            Date = Date.Item1,
                            StoreId = Date.Item2,
                            SaleEmployeeId = SalesEmployee.SaleEmployeeId
                        };

                        if (Checkings.Count() != 0)
                        {
                            MonitorStoreImage_DetailDTO.StoreName = Checkings.Select(x => x.Store.Name).FirstOrDefault();
                            MonitorStoreImage_DetailDTO.ImageCounter += Checkings.Where(x => x.ImageCounter.HasValue).Select(x => x.ImageCounter.Value).Sum();
                        }
                        if (Images.Count != 0)
                        {
                            MonitorStoreImage_DetailDTO.StoreName = Images.Select(x => x.StoreName).FirstOrDefault();
                            MonitorStoreImage_DetailDTO.ImageCounter += Images.Count();
                        }
                        SalesEmployee.StoreCheckings.Add(MonitorStoreImage_DetailDTO);
                    });
                }
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

            var query = from si in DataContext.StoreImage
                        where si.StoreId == MonitorStoreImage_StoreCheckingDTO.StoreId &&
                        (si.SaleEmployeeId.HasValue && si.SaleEmployeeId.Value == MonitorStoreImage_StoreCheckingDTO.SaleEmployeeId) &&
                        Start <= si.ShootingAt && si.ShootingAt <= End
                        select new MonitorStoreImage_StoreCheckingImageMappingDTO
                        {
                            AlbumId = si.AlbumId,
                            ImageId = si.ImageId,
                            SaleEmployeeId = si.SaleEmployeeId.Value,
                            ShootingAt = si.ShootingAt,
                            StoreId = si.StoreId,
                            Distance = si.Distance,
                            Album = new MonitorStoreImage_AlbumDTO
                            {
                                Id = si.AlbumId,
                                Name = si.AlbumName
                            },
                            Image = new MonitorStoreImage_ImageDTO
                            {
                                Id = si.ImageId,
                                Url = si.Url
                            },
                            SaleEmployee = new MonitorStoreImage_AppUserDTO
                            {
                                Id = si.SaleEmployeeId.Value,
                                DisplayName = si.DisplayName
                            },
                            Store = new MonitorStoreImage_StoreDTO
                            {
                                Id = si.StoreId,
                                Address = si.StoreAddress,
                                Name = si.StoreName
                            }
                        };
            return await query.ToListAsync();
        }

        [Route(MonitorStoreImageRoute.UpdateAlbum), HttpPost]
        public async Task<MonitorStoreImage_StoreCheckingImageMappingDTO> UpdateAlbum([FromBody] MonitorStoreImage_StoreCheckingImageMappingDTO MonitorStoreImage_StoreCheckingImageMappingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var AlbumImageMappingDAO = await DataContext
                .AlbumImageMapping.Where(x => x.ImageId == MonitorStoreImage_StoreCheckingImageMappingDTO.ImageId)
                .FirstOrDefaultAsync();
            if (AlbumImageMappingDAO != null)
            {
                var newObj = Utils.Clone(AlbumImageMappingDAO);
                await DataContext.AlbumImageMapping.Where(x => x.ImageId == MonitorStoreImage_StoreCheckingImageMappingDTO.ImageId).DeleteFromQueryAsync();
                newObj.AlbumId = MonitorStoreImage_StoreCheckingImageMappingDTO.AlbumId;
                await DataContext.AlbumImageMapping.AddAsync(newObj);
                await DataContext.SaveChangesAsync();
            }
            else
            {
                var StoreCheckingImageMappingDAO = await DataContext
                .StoreCheckingImageMapping.Where(x => x.ImageId == MonitorStoreImage_StoreCheckingImageMappingDTO.ImageId)
                .FirstOrDefaultAsync();
                if (StoreCheckingImageMappingDAO != null)
                {
                    await DataContext.StoreCheckingImageMapping.Where(x => x.ImageId == MonitorStoreImage_StoreCheckingImageMappingDTO.ImageId).UpdateFromQueryAsync(x => new StoreCheckingImageMappingDAO 
                    {
                        AlbumId = MonitorStoreImage_StoreCheckingImageMappingDTO.AlbumId
                    });
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
