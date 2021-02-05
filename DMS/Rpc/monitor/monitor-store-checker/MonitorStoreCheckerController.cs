using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Hangfire.Annotations;
using DMS.Helpers;
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

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreCheckerController : MonitorController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        public MonitorStoreCheckerController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MonitorStoreCheckerRoute.FilterListAppUser), HttpPost]
        public async Task<List<MonitorStoreChecker_AppUserDTO>> FilterListAppUser([FromBody] StoreCheckerMonitor_AppUserFilterDTO StoreCheckerMonitor_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = StoreCheckerMonitor_AppUserFilterDTO.Id;
            AppUserFilter.Username = StoreCheckerMonitor_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = StoreCheckerMonitor_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = StoreCheckerMonitor_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<MonitorStoreChecker_AppUserDTO> StoreCheckerMonitor_AppUserDTOs = AppUsers
                .Select(x => new MonitorStoreChecker_AppUserDTO(x)).ToList();
            return StoreCheckerMonitor_AppUserDTOs;
        }

        [Route(MonitorStoreCheckerRoute.FilterListOrganization), HttpPost]
        public async Task<List<MonitorStoreChecker_OrganizationDTO>> FilterListOrganization()
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
            List<MonitorStoreChecker_OrganizationDTO> StoreCheckerMonitor_OrganizationDTOs = Organizations
                .Select(x => new MonitorStoreChecker_OrganizationDTO(x)).ToList();
            return StoreCheckerMonitor_OrganizationDTOs;
        }

        [Route(MonitorStoreCheckerRoute.FilterListChecking), HttpPost]
        public List<EnumList> FilterListChecking()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Chưa viếng thăm" });
            EnumList.Add(new EnumList { Id = 1, Name = "Đã viếng thăm" });
            return EnumList;
        }

        [Route(MonitorStoreCheckerRoute.FilterListImage), HttpPost]
        public List<EnumList> FilterListImage()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không hình ảnh" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có hình ảnh" });
            return EnumList;
        }

        [Route(MonitorStoreCheckerRoute.FilterListSalesOrder), HttpPost]
        public List<EnumList> FilterListSalesOrder()
        {
            List<EnumList> EnumList = new List<EnumList>();
            EnumList.Add(new EnumList { Id = 0, Name = "Không có đơn hàng" });
            EnumList.Add(new EnumList { Id = 1, Name = "Có đơn hàng" });
            return EnumList;
        }

        [Route(MonitorStoreCheckerRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
              LocalStartDay(CurrentContext) :
              MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            int count = await DataContext.AppUser.Where(au =>
                AppUserIds.Contains(au.Id) &&
                au.DeletedAt == null &
                (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value))
                .CountAsync();
            return count;
        }

        [Route(MonitorStoreCheckerRoute.List), HttpPost]
        public async Task<ActionResult<List<MonitorStoreChecker_MonitorStoreCheckerDTO>>> List([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            if (End.Subtract(Start).Days > 7)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 7 ngày" });
            List<MonitorStoreChecker_MonitorStoreCheckerDTO> MonitorStoreChecker_MonitorStoreCheckerDTOs = await ListData(MonitorStoreChecker_MonitorStoreCheckerFilterDTO, Start, End);
            return MonitorStoreChecker_MonitorStoreCheckerDTOs;
        }

        [Route(MonitorStoreCheckerRoute.Get), HttpPost]
        public async Task<List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>> Get([FromBody] MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            long SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.SaleEmployeeId;

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            List<long> StoreCheckingStoreIds = await DataContext.StoreChecking
                .Where(sc =>
                    sc.CheckOutAt.HasValue &&
                    sc.SaleEmployeeId == SaleEmployeeId &&
                    Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End)
                .Select(x => x.StoreId)
                .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(o => Start <= o.OrderDate && o.OrderDate <= End &&
                o.SaleEmployeeId == SaleEmployeeId &&
                (o.RequestStateId == RequestStateEnum.APPROVED.Id || o.RequestStateId == RequestStateEnum.PENDING.Id))
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Code = x.Code,
                    BuyerStoreId = x.BuyerStoreId,
                    Total = x.Total,
                })
                .ToListAsync();

            List<ProblemDAO> ProblemDAOs = await DataContext.Problem.Where(p =>
                p.CreatorId == SaleEmployeeId &&
                p.NoteAt >= Start && p.NoteAt <= End
            ).ToListAsync();

            List<StoreImageDAO> StoreImageDAOs = await DataContext.StoreImage
                .Where(x => x.SaleEmployeeId.HasValue && x.SaleEmployeeId == SaleEmployeeId &&
                Start <= x.ShootingAt && x.ShootingAt <= End)
                .ToListAsync();

            List<long> StoreIds = new List<long>();
            StoreIds.AddRange(StoreCheckingStoreIds);
            List<long> SalesOrder_StoreIds = IndirectSalesOrderDAOs.Select(o => o.BuyerStoreId).ToList();
            StoreIds.AddRange(SalesOrder_StoreIds);
            var Problem_StoreIds = ProblemDAOs.Select(x => x.StoreId).Distinct().ToList();
            StoreIds.AddRange(Problem_StoreIds);
            var StoreImage_StoreId = StoreImageDAOs.Select(x => x.StoreId).Distinct().ToList();
            StoreIds.AddRange(StoreImage_StoreId);

            StoreIds = StoreIds.Distinct().ToList();
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                     .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query2 = from s in DataContext.Store
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         select new StoreDAO
                         {
                             Id = s.Id,
                             Code = s.Code,
                             Name = s.Name,
                             Address = s.Address,
                         };
            List<StoreDAO> StoreDAOs = await query2.ToListAsync();

            List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO> MonitorStoreChecker_MonitorStoreCheckerDetailDTOs = new List<MonitorStoreChecker_MonitorStoreCheckerDetailDTO>();
            foreach (long StoreId in StoreIds)
            {
                List<ProblemDAO> Problems = ProblemDAOs.Where(p => p.StoreId == StoreId).ToList();
                List<IndirectSalesOrderDAO> SubIndirectSalesOrderDAOs = IndirectSalesOrderDAOs.Where(i => i.BuyerStoreId == StoreId).ToList();
                List<StoreImageDAO> SubStoreImageDAOs = StoreImageDAOs.Where(x => x.StoreId == StoreId).ToList();

                int Max = 1;
                Max = SubIndirectSalesOrderDAOs.Count > Max ? IndirectSalesOrderDAOs.Count : Max;
                Max = Problems.Count > Max ? Problems.Count : Max;
                StoreDAO storeDAO = StoreDAOs.Where(s => s.Id == StoreId).FirstOrDefault();
                MonitorStoreChecker_MonitorStoreCheckerDetailDTO MonitorStoreChecker_MonitorStoreCheckerDetailDTO = new MonitorStoreChecker_MonitorStoreCheckerDetailDTO
                {
                    StoreId = storeDAO.Id,
                    StoreCode = storeDAO.Code,
                    StoreName = storeDAO.Name,
                    ImageCounter = SubStoreImageDAOs.Count(),
                    Infoes = new List<MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO>(),
                };
                MonitorStoreChecker_MonitorStoreCheckerDetailDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDetailDTO);
                for (int i = 0; i < Max; i++)
                {
                    MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO Info = new MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO();
                    MonitorStoreChecker_MonitorStoreCheckerDetailDTO.Infoes.Add(Info);

                    if (SubStoreImageDAOs.Count > i)
                    {
                        Info.ImagePath = SubStoreImageDAOs[i].Url;
                    }

                    if (SubIndirectSalesOrderDAOs.Count > i)
                    {
                        Info.IndirectSalesOrderCode = SubIndirectSalesOrderDAOs[i].Code;
                        Info.Sales = SubIndirectSalesOrderDAOs[i].Total;
                    }
                    if (Problems.Count > i)
                    {
                        Info.ProblemCode = Problems[i].Code;
                        Info.ProblemId = Problems[i].Id;
                    }
                }
            }
            return MonitorStoreChecker_MonitorStoreCheckerDetailDTOs;
        }

        [Route(MonitorStoreCheckerRoute.ListImage), HttpPost]
        public async Task<List<MonitorStoreChecker_StoreCheckingImageMappingDTO>> ListImage([FromBody] MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = MonitorStoreChecker_StoreCheckingDTO.Date.AddHours(CurrentContext.TimeZone).Date.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddDays(1);

            var query = from si in DataContext.StoreImage
                        where si.StoreId == MonitorStoreChecker_StoreCheckingDTO.StoreId &&
                        si.SaleEmployeeId.HasValue && si.SaleEmployeeId.Value == MonitorStoreChecker_StoreCheckingDTO.SaleEmployeeId &&
                        Start <= si.ShootingAt && si.ShootingAt < End
                        select new MonitorStoreChecker_StoreCheckingImageMappingDTO
                        {
                            AlbumId = si.AlbumId,
                            ImageId = si.ImageId,
                            SaleEmployeeId = si.SaleEmployeeId.Value,
                            ShootingAt = si.ShootingAt,
                            StoreId = si.StoreId,
                            Distance = si.Distance,
                            Album = new MonitorStoreChecker_AlbumDTO
                            {
                                Id = si.AlbumId,
                                Name = si.AlbumName
                            },
                            Image = new MonitorStoreChecker_ImageDTO
                            {
                                Url = si.Url
                            },
                            SaleEmployee = new MonitorStoreChecker_AppUserDTO
                            {
                                Id = si.SaleEmployeeId.Value,
                                DisplayName = si.DisplayName
                            },
                            Store = new MonitorStoreChecker_StoreDTO
                            {
                                Id = si.StoreId,
                                Address = si.StoreAddress,
                                Name = si.StoreName
                            }
                        };

            return await query.ToListAsync();
        }

        [Route(MonitorStoreCheckerRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.GreaterEqual == null ?
               LocalStartDay(CurrentContext) :
               MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    MonitorStoreChecker_MonitorStoreCheckerFilterDTO.CheckIn.LessEqual.Value;

            MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Skip = 0;
            MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Take = int.MaxValue;
            List<MonitorStoreChecker_MonitorStoreCheckerDTO> MonitorStoreChecker_MonitorStoreCheckerDTOs = await ListData(MonitorStoreChecker_MonitorStoreCheckerFilterDTO, Start, End);
            long stt = 1;
            foreach (MonitorStoreChecker_MonitorStoreCheckerDTO MonitorStoreChecker_MonitorStoreCheckerDTO in MonitorStoreChecker_MonitorStoreCheckerDTOs)
            {
                foreach (var SaleEmployee in MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees)
                {
                    foreach (var storeChecking in SaleEmployee.StoreCheckings)
                    {
                        storeChecking.STT = stt;
                        stt++;
                        storeChecking.Date = storeChecking.Date.AddHours(CurrentContext.TimeZone);
                    }
                }
            }

            string path = "Templates/Monitor_Store_Checker_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.MonitorStoreCheckers = MonitorStoreChecker_MonitorStoreCheckerDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "MonitorStoreChecker.xlsx");
        }

        private async Task<List<MonitorStoreChecker_MonitorStoreCheckerDTO>> ListData(
            MonitorStoreChecker_MonitorStoreCheckerFilterDTO MonitorStoreChecker_MonitorStoreCheckerFilterDTO,
            DateTime Start, DateTime End)
        {
            long? SaleEmployeeId = MonitorStoreChecker_MonitorStoreCheckerFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == MonitorStoreChecker_MonitorStoreCheckerFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            var storeCheckingQuery = from sc in DataContext.StoreChecking
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     where AppUserIds.Contains(sc.SaleEmployeeId) &&
                                     OrganizationIds.Contains(sc.OrganizationId) &&
                                     (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                                     au.DeletedAt == null &&
                                     sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt && sc.CheckOutAt <= End
                                     select sc;

            var storeImageQuery = from si in DataContext.StoreImage
                                  join au in DataContext.AppUser on si.SaleEmployeeId equals au.Id
                                  where si.SaleEmployeeId.HasValue && AppUserIds.Contains(si.SaleEmployeeId.Value) &&
                                  OrganizationIds.Contains(si.OrganizationId) &&
                                  (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                                  au.DeletedAt == null &&
                                  Start <= si.ShootingAt && si.ShootingAt <= End
                                  select si;

            var salesOrderQuery = from i in DataContext.IndirectSalesOrder
                                  join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                                  where AppUserIds.Contains(i.SaleEmployeeId) &&
                                  OrganizationIds.Contains(i.OrganizationId) &&
                                  (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value) &&
                                  au.DeletedAt == null &&
                                  Start <= i.OrderDate && i.OrderDate <= End &&
                                  (i.RequestStateId == RequestStateEnum.APPROVED.Id || i.RequestStateId == RequestStateEnum.PENDING.Id)
                                  select new IndirectSalesOrderDAO
                                  {
                                      Id = i.Id,
                                      OrganizationId = i.OrganizationId,
                                      SaleEmployeeId = i.SaleEmployeeId,
                                      BuyerStoreId = i.BuyerStoreId,
                                      OrderDate = i.OrderDate,
                                      Total = i.Total
                                  };

            var storeUncheckingQuery = from su in DataContext.StoreUnchecking
                                       where AppUserIds.Contains(su.AppUserId) &&
                                       OrganizationIds.Contains(su.OrganizationId) &&
                                       (SaleEmployeeId == null || su.AppUserId == SaleEmployeeId.Value) &&
                                       Start <= su.Date && su.Date <= End
                                       select su;

            var Ids1 = await storeCheckingQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.SaleEmployeeId,
            }).ToListAsync();
            var Ids2 = await storeImageQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.SaleEmployeeId.Value,
            }).ToListAsync();
            var Ids3 = await salesOrderQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.SaleEmployeeId,
            }).ToListAsync();
            var Ids4 = await storeUncheckingQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                SalesEmployeeId = x.AppUserId,
            }).ToListAsync();
            var Ids = Ids1;
            Ids.AddRange(Ids2);
            Ids.AddRange(Ids3);
            Ids.AddRange(Ids4);
            Ids = Ids.Distinct()
                .OrderBy(x => x.OrganizationId)
                .Skip(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Skip)
                .Take(MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Take)
                .ToList();

            AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();
            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser.Where(au =>
               AppUserIds.Contains(au.Id) &&
               au.DeletedAt == null &&
               OrganizationIds.Contains(au.OrganizationId) &&
               (SaleEmployeeId == null || au.Id == SaleEmployeeId.Value))
                .Select(x => new AppUserDAO
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    OrganizationId = x.OrganizationId,
                }).ToListAsync();

            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            List<ERouteContentDAO> ERouteContentDAOs = await DataContext.ERouteContent
              .Where(ec => ec.ERoute.RealStartDate <= End &&
                    (ec.ERoute.EndDate == null || ec.ERoute.EndDate.Value >= Start) &&
                    AppUserIds.Contains(ec.ERoute.SaleEmployeeId))
              .Where(x => x.ERoute.StatusId == StatusEnum.ACTIVE.Id &&
              x.ERoute.RequestStateId == RequestStateEnum.APPROVED.Id)
              .Include(ec => ec.ERouteContentDays)
              .Include(ec => ec.ERoute)
              .ToListAsync();

            List<StoreCheckingDAO> StoreCheckingDAOs = await storeCheckingQuery.ToListAsync();
            List<StoreImageDAO> StoreImageDAOs = await storeImageQuery.ToListAsync();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await salesOrderQuery.ToListAsync();
            List<StoreUncheckingDAO> StoreUncheckingDAOs = await storeUncheckingQuery.ToListAsync();

            List<MonitorStoreChecker_MonitorStoreCheckerDTO> MonitorStoreChecker_MonitorStoreCheckerDTOs = new List<MonitorStoreChecker_MonitorStoreCheckerDTO>();
            foreach (var Organization in Organizations)
            {
                MonitorStoreChecker_MonitorStoreCheckerDTO MonitorStoreChecker_MonitorStoreCheckerDTO = new MonitorStoreChecker_MonitorStoreCheckerDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<MonitorStoreChecker_SaleEmployeeDTO>()
                };
                MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees = Ids
                    .Where(x => x.OrganizationId == Organization.Id)
                    .Select(x => new MonitorStoreChecker_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.SalesEmployeeId
                    }).ToList();
                MonitorStoreChecker_MonitorStoreCheckerDTOs.Add(MonitorStoreChecker_MonitorStoreCheckerDTO);
            }

            foreach (var MonitorStoreChecker_MonitorStoreCheckerDTO in MonitorStoreChecker_MonitorStoreCheckerDTOs)
            {
                Parallel.ForEach(MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees, SaleEmployee =>
                {
                    var appUser = AppUserDAOs.Where(x => x.Id == SaleEmployee.SaleEmployeeId).FirstOrDefault();
                    if (appUser != null)
                    {
                        SaleEmployee.SaleEmployeeId = appUser.Id;
                        SaleEmployee.Username = appUser.Username;
                        SaleEmployee.DisplayName = appUser.DisplayName;
                        SaleEmployee.StoreCheckings = new List<MonitorStoreChecker_StoreCheckingDTO>();
                    }
                });

                Parallel.ForEach(MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees, SaleEmployee =>
                {
                    SaleEmployee.StoreCheckings = new List<MonitorStoreChecker_StoreCheckingDTO>();
                    for (DateTime i = Start.AddHours(CurrentContext.TimeZone); i < End.AddHours(CurrentContext.TimeZone); i = i.AddDays(1))
                    {
                        MonitorStoreChecker_StoreCheckingDTO StoreCheckerMonitor_StoreCheckingDTO = new MonitorStoreChecker_StoreCheckingDTO();
                        StoreCheckerMonitor_StoreCheckingDTO.SaleEmployeeId = SaleEmployee.SaleEmployeeId;
                        StoreCheckerMonitor_StoreCheckingDTO.Date = i;
                        StoreCheckerMonitor_StoreCheckingDTO.Image = new HashSet<long>();
                        StoreCheckerMonitor_StoreCheckingDTO.External = new HashSet<long>();
                        StoreCheckerMonitor_StoreCheckingDTO.Internal = new HashSet<long>();
                        SaleEmployee.StoreCheckings.Add(StoreCheckerMonitor_StoreCheckingDTO);
                    }
                });

                Parallel.ForEach(MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees, SaleEmployee =>
                {
                    for (DateTime i = Start.AddHours(CurrentContext.TimeZone); i <= End.AddHours(CurrentContext.TimeZone); i = i.AddDays(1))
                    {
                        MonitorStoreChecker_StoreCheckingDTO MonitorStoreChecker_StoreCheckingDTO = SaleEmployee.StoreCheckings
                            .Where(s => s.Date == i).FirstOrDefault();
                        MonitorStoreChecker_StoreCheckingDTO.SalesOrderCounter = IndirectSalesOrderDAOs
                            .Where(o => i <= o.OrderDate.AddHours(CurrentContext.TimeZone) && o.OrderDate.AddHours(CurrentContext.TimeZone) < i.AddDays(1) &&
                            o.SaleEmployeeId == SaleEmployee.SaleEmployeeId)
                            .Count();
                        MonitorStoreChecker_StoreCheckingDTO.RevenueCounter = IndirectSalesOrderDAOs
                            .Where(o => i <= o.OrderDate.AddHours(CurrentContext.TimeZone) && o.OrderDate.AddHours(CurrentContext.TimeZone) < i.AddDays(1) &&
                            o.SaleEmployeeId == SaleEmployee.SaleEmployeeId)
                            .Select(o => o.Total).DefaultIfEmpty(0).Sum();
                        MonitorStoreChecker_StoreCheckingDTO.Unchecking = StoreUncheckingDAOs
                        .Where(x => i <= x.Date.AddHours(CurrentContext.TimeZone) && x.Date.AddHours(CurrentContext.TimeZone) < i.AddDays(1) &&
                        x.AppUserId == SaleEmployee.SaleEmployeeId)
                        .Count();

                        MonitorStoreChecker_StoreCheckingDTO.PlanCounter = CountPlan(i.AddHours(0 - CurrentContext.TimeZone), SaleEmployee.SaleEmployeeId, ERouteContentDAOs);

                        List<StoreCheckingDAO> ListChecked = StoreCheckingDAOs
                               .Where(s =>
                                   s.SaleEmployeeId == SaleEmployee.SaleEmployeeId &&
                                   i <= s.CheckOutAt.Value.AddHours(CurrentContext.TimeZone) && s.CheckOutAt.Value.AddHours(CurrentContext.TimeZone) < i.AddDays(1)
                               ).ToList();
                        foreach (StoreCheckingDAO Checked in ListChecked)
                        {
                            if (Checked.Planned)
                                MonitorStoreChecker_StoreCheckingDTO.Internal.Add(Checked.StoreId);
                            else
                                MonitorStoreChecker_StoreCheckingDTO.External.Add(Checked.StoreId);
                        }

                        var ImageIds = StoreImageDAOs
                            .Where(x => x.SaleEmployeeId.HasValue && x.SaleEmployeeId == SaleEmployee.SaleEmployeeId)
                            .Where(x => x.ShootingAt.AddHours(CurrentContext.TimeZone).Date == i.Date)
                            .Select(x => x.ImageId)
                            .ToList();
                        ImageIds.ForEach(x => MonitorStoreChecker_StoreCheckingDTO.Image.Add(x));
                    }
                });

                Parallel.ForEach(MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees, SaleEmployee =>
                {
                    foreach (var StoreChecking in SaleEmployee.StoreCheckings)
                    {
                        StoreChecking.Date = StoreChecking.Date.AddHours(CurrentContext.TimeZone);
                    }
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking?.Equal != null)
                    {
                        if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking.Equal.Value == 0)
                            SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(sc => sc.InternalCounter + sc.ExternalCounter == 0).ToList();
                        if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Checking.Equal.Value == 1)
                            SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(sc => sc.InternalCounter + sc.ExternalCounter > 0).ToList();
                    }
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image?.Equal != null)
                    {
                        if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image.Equal.Value == 0)
                            SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(sc => sc.Image.Count == 0).ToList();
                        if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.Image.Equal.Value == 1)
                            SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(sc => sc.Image.Count > 0).ToList();
                    }
                    if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder?.Equal != null)
                    {
                        if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder.Equal.Value == 0)
                            SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(sc => sc.SalesOrderCounter == 0).ToList();
                        if (MonitorStoreChecker_MonitorStoreCheckerFilterDTO.SalesOrder.Equal.Value == 1)
                            SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(sc => sc.SalesOrderCounter > 0).ToList();
                    }
                });

                Parallel.ForEach(MonitorStoreChecker_MonitorStoreCheckerDTO.SaleEmployees, SaleEmployee =>
                {
                    SaleEmployee.StoreCheckings = SaleEmployee.StoreCheckings.Where(x => x.HasValue).ToList();
                });
            }

            return MonitorStoreChecker_MonitorStoreCheckerDTOs.Where(x => x.SaleEmployees.Any()).ToList();
        }

    }
}
