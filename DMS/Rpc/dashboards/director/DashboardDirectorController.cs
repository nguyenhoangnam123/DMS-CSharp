using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MOrganization;
using DMS.Services.MStoreChecking;
using DMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Enums;
using DMS.Services.MStore;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;
using DMS.Services.MProvince;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirectorController : RpcController
    {
        private const long TODAY = 0;
        private const long THIS_WEEK = 1;
        private const long THIS_MONTH = 2;
        private const long LAST_MONTH = 3;
        private const long THIS_QUARTER = 4;
        private const long LAST_QUATER = 5;
        private const long YEAR = 6;

        private DataContext DataContext;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        private IOrganizationService OrganizationService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IProvinceService ProvinceService;
        private IStoreCheckingService StoreCheckingService;
        private IStoreService StoreService;
        public DashboardDirectorController(
            DataContext DataContext,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext,
            IOrganizationService OrganizationService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IProvinceService ProvinceService,
            IStoreCheckingService StoreCheckingService,
            IStoreService StoreService)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
            this.OrganizationService = OrganizationService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ProvinceService = ProvinceService;
            this.StoreCheckingService = StoreCheckingService;
            this.StoreService = StoreService;
        }

        #region Filter List
        [Route(DashboardDirectorRoute.FilterListTime), HttpPost]
        public List<DashboardDirector_EnumList> FilterListTime()
        {
            List<DashboardDirector_EnumList> Dashborad_EnumLists = new List<DashboardDirector_EnumList>();
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = TODAY, Name = "Hôm nay" });
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = THIS_QUARTER, Name = "Quý này" });
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = LAST_QUATER, Name = "Quý trước" });
            Dashborad_EnumLists.Add(new DashboardDirector_EnumList { Id = YEAR, Name = "Năm" });
            return Dashborad_EnumLists;
        }

        [Route(DashboardDirectorRoute.FilterListAppUser), HttpPost]
        public async Task<List<DashboardDirector_AppUserDTO>> FilterListAppUser([FromBody] DashboardDirector_AppUserFilterDTO DashboardDirector_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = DashboardDirector_AppUserFilterDTO.Id;
            AppUserFilter.Username = DashboardDirector_AppUserFilterDTO.Username;
            AppUserFilter.Password = DashboardDirector_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = DashboardDirector_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = DashboardDirector_AppUserFilterDTO.Address;
            AppUserFilter.Email = DashboardDirector_AppUserFilterDTO.Email;
            AppUserFilter.Phone = DashboardDirector_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = DashboardDirector_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = DashboardDirector_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = DashboardDirector_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = DashboardDirector_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = DashboardDirector_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = DashboardDirector_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = DashboardDirector_AppUserFilterDTO.ProvinceId;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<DashboardDirector_AppUserDTO> DashboardDirector_AppUserDTOs = AppUsers
                .Select(x => new DashboardDirector_AppUserDTO(x)).ToList();
            return DashboardDirector_AppUserDTOs;
        }

        [Route(DashboardDirectorRoute.FilterListOrganization), HttpPost]
        public async Task<List<DashboardDirector_OrganizationDTO>> FilterListOrganization([FromBody] DashboardDirector_OrganizationFilterDTO DashboardDirector_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 20;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = DashboardDirector_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = DashboardDirector_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DashboardDirector_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = DashboardDirector_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = DashboardDirector_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = DashboardDirector_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = DashboardDirector_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = DashboardDirector_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = DashboardDirector_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = DashboardDirector_OrganizationFilterDTO.Email;
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationFilter.Id.In.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_OrganizationFilterDTO.Id?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_OrganizationFilterDTO.Id.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationFilter.Id.In = OrganizationDAOs.Select(o => o.Id).ToList();

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DashboardDirector_OrganizationDTO> DashboardDirector_OrganizationDTOs = Organizations
                .Select(x => new DashboardDirector_OrganizationDTO(x)).ToList();
            return DashboardDirector_OrganizationDTOs;
        }

        [Route(DashboardDirectorRoute.FilterListProvince), HttpPost]
        public async Task<List<DashboardDirector_ProvinceDTO>> FilterListProvince([FromBody] DashboardDirector_ProvinceFilterDTO DashboardDirector_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = DashboardDirector_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = DashboardDirector_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = DashboardDirector_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<DashboardDirector_ProvinceDTO> DashboardDirector_ProvinceDTOs = Provinces
                .Select(x => new DashboardDirector_ProvinceDTO(x)).ToList();
            return DashboardDirector_ProvinceDTOs;
        }
        #endregion

        [Route(DashboardDirectorRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] DashboardDirector_StoreFilterDTO DashboardDirector_StoreFilterDTO)
        {
            var ProvinceId = DashboardDirector_StoreFilterDTO.ProvinceId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_StoreFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_StoreFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query = from s in DataContext.Store
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        s.StatusId == StatusEnum.ACTIVE.Id && 
                        s.DeletedAt == null
                        select s;

            var StoreCounter = query.Count();
            return StoreCounter;
        }

        [Route(DashboardDirectorRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<long> CountIndirectSalesOrder([FromBody] DashboardDirector_IndirectSalesOrderFluctuationFilterDTO DashboardDirector_IndirectSalesOrderFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);
            (Start, End) = ConvertTime(DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time);

            var ProvinceId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        au.DeletedAt == null && o.DeletedAt == null
                        select i;

            return query.Count();
        }

        [Route(DashboardDirectorRoute.RevenueTotal), HttpPost]
        public async Task<decimal> RevenueTotal([FromBody] DashboardDirector_SaledItemFluctuationFilterDTO DashboardDirector_SaledItemFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);
            (Start, End) = ConvertTime(DashboardDirector_SaledItemFluctuationFilterDTO.Time);

            var ProvinceId = DashboardDirector_SaledItemFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_SaledItemFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_SaledItemFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_SaledItemFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        au.DeletedAt == null && o.DeletedAt == null
                        select i;

            var RevenueTotal = query.Select(x => x.Total).Sum();
            return RevenueTotal;
        }

        [Route(DashboardDirectorRoute.StoreHasCheckedCounter), HttpPost]
        public async Task<long> StoreHasCheckedCounter([FromBody] DashboardDirector_SaledItemFluctuationFilterDTO DashboardDirector_SaledItemFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);
            (Start, End) = ConvertTime(DashboardDirector_SaledItemFluctuationFilterDTO.Time);

            var ProvinceId = DashboardDirector_SaledItemFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_SaledItemFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_SaledItemFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_SaledItemFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var queryStoreHasCheckedCounter = from sc in DataContext.StoreChecking
                                              join s in DataContext.Store on sc.StoreId equals s.Id
                                              join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                              join o in DataContext.Organization on au.OrganizationId equals o.Id
                                              join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                              where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                                              AppUserIds.Contains(au.Id) &&
                                              (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                              (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                              OrganizationIds.Contains(s.OrganizationId) &&
                                              au.DeletedAt == null && o.DeletedAt == null && s.DeletedAt == null
                                              select s;

            var StoreHasCheckedCounter = queryStoreHasCheckedCounter.Select(x => x.Id).Distinct().Count();
            return StoreHasCheckedCounter;
        }

        [Route(DashboardDirectorRoute.CountStoreChecking), HttpPost]
        public async Task<long> CountStoreChecking([FromBody] DashboardDirector_StoreFilterDTO DashboardDirector_StoreFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = Start.AddMonths(1).AddSeconds(-1);
            (Start, End) = ConvertTime(DashboardDirector_StoreFilterDTO.Time);

            var ProvinceId = DashboardDirector_StoreFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_StoreFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_StoreFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_StoreFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from sc in DataContext.StoreChecking
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        OrganizationIds.Contains(s.OrganizationId) &&
                        au.DeletedAt == null && o.DeletedAt == null && s.DeletedAt == null
                        select sc;

            var StoreCheckingCounter = query.Count();
            return StoreCheckingCounter;
        }

        [Route(DashboardDirectorRoute.StatisticToday), HttpPost]
        public async Task<DashboardDirector_StatisticDailyDTO> StatisticToday([FromBody] DashboardDirector_StoreFilterDTO DashboardDirector_StoreFilterDTO)
        {
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            var ProvinceId = DashboardDirector_StoreFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_StoreFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_StoreFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_StoreFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var queryRevenue = from i in DataContext.IndirectSalesOrder
                               join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                               join o in DataContext.Organization on au.OrganizationId equals o.Id
                               join s in DataContext.Store on i.BuyerStoreId equals s.Id
                               join tt in tempTableQuery.Query on s.Id equals tt.Column1
                               where i.OrderDate >= Start && i.OrderDate <= End &&
                               AppUserIds.Contains(au.Id) &&
                               (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                               (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                               OrganizationIds.Contains(i.OrganizationId) &&
                               i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                               au.DeletedAt == null && o.DeletedAt == null
                               select i;

            var queryIndirectSalesOrder = from i in DataContext.IndirectSalesOrder
                                          join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                                          join o in DataContext.Organization on au.OrganizationId equals o.Id
                                          join s in DataContext.Store on i.BuyerStoreId equals s.Id
                                          join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                          where i.OrderDate >= Start && i.OrderDate <= End &&
                                          AppUserIds.Contains(au.Id) &&
                                          (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                          (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                          OrganizationIds.Contains(i.OrganizationId) &&
                                          i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                                          au.DeletedAt == null && o.DeletedAt == null
                                          select i;

            var queryStoreHasCheckedCounter = from sc in DataContext.StoreChecking
                                        join s in DataContext.Store on sc.StoreId equals s.Id
                                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                                        AppUserIds.Contains(au.Id) &&
                                        (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                        OrganizationIds.Contains(s.OrganizationId) &&
                                        au.DeletedAt == null && o.DeletedAt == null && s.DeletedAt == null
                                        select s;

            var queryStoreChecking = from sc in DataContext.StoreChecking
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     join o in DataContext.Organization on au.OrganizationId equals o.Id
                                     where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                                     AppUserIds.Contains(au.Id) &&
                                     (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                     (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                     OrganizationIds.Contains(s.OrganizationId) &&
                                     au.DeletedAt == null && o.DeletedAt == null && s.DeletedAt == null
                                     select sc;

            var RevenueTotal = queryRevenue.Select(x => x.Total).Sum();
            var IndirectSalesOrderCounter = queryIndirectSalesOrder.Count();
            var StoreHasCheckedCounter = queryStoreHasCheckedCounter.Select(x => x.Id).Distinct().Count();
            var StoreCheckingCounter = queryStoreChecking.Count();

            DashboardDirector_StatisticDailyDTO DashboardDirector_StatisticDailyDTO = new DashboardDirector_StatisticDailyDTO()
            {
                Revenue = RevenueTotal,
                IndirectSalesOrderCounter = IndirectSalesOrderCounter,
                StoreHasCheckedCounter = StoreHasCheckedCounter,
                StoreCheckingCounter = StoreCheckingCounter
            };
            return DashboardDirector_StatisticDailyDTO;
        }

        [Route(DashboardDirectorRoute.StatisticYesterday), HttpPost]
        public async Task<DashboardDirector_StatisticDailyDTO> StatisticYesterday([FromBody] DashboardDirector_IndirectSalesOrderFluctuationFilterDTO DashboardDirector_IndirectSalesOrderFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = LocalStartDay(CurrentContext).AddDays(-1);
            DateTime End = Start.AddDays(1).AddSeconds(-1);

            var ProvinceId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var queryRevenue = from i in DataContext.IndirectSalesOrder
                               join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                               join o in DataContext.Organization on au.OrganizationId equals o.Id
                               join s in DataContext.Store on i.BuyerStoreId equals s.Id
                               join tt in tempTableQuery.Query on s.Id equals tt.Column1
                               where i.OrderDate >= Start && i.OrderDate <= End &&
                               AppUserIds.Contains(au.Id) &&
                               (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                               (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                               OrganizationIds.Contains(i.OrganizationId) &&
                               i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                               au.DeletedAt == null && o.DeletedAt == null
                               select i;

            var queryIndirectSalesOrder = from i in DataContext.IndirectSalesOrder
                                          join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                                          join o in DataContext.Organization on au.OrganizationId equals o.Id
                                          join s in DataContext.Store on i.BuyerStoreId equals s.Id
                                          join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                          where i.OrderDate >= Start && i.OrderDate <= End &&
                                          AppUserIds.Contains(au.Id) &&
                                          (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                          (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                          OrganizationIds.Contains(i.OrganizationId) &&
                                          i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                                          au.DeletedAt == null && o.DeletedAt == null
                                          select i;

            var queryStoreHasCheckedCounter = from sc in DataContext.StoreChecking
                                              join s in DataContext.Store on sc.StoreId equals s.Id
                                              join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                              join o in DataContext.Organization on au.OrganizationId equals o.Id
                                              where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                                              AppUserIds.Contains(au.Id) &&
                                              (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                              (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                              OrganizationIds.Contains(s.OrganizationId) &&
                                              au.DeletedAt == null && o.DeletedAt == null && s.DeletedAt == null
                                              select s;

            var queryStoreChecking = from sc in DataContext.StoreChecking
                                     join s in DataContext.Store on sc.StoreId equals s.Id
                                     join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                                     join o in DataContext.Organization on au.OrganizationId equals o.Id
                                     where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                                     AppUserIds.Contains(au.Id) &&
                                     (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                                     (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                                     OrganizationIds.Contains(s.OrganizationId) &&
                                     au.DeletedAt == null && o.DeletedAt == null && s.DeletedAt == null
                                     select sc;

            var RevenueTotal = queryRevenue.Select(x => x.Total).Sum();
            var IndirectSalesOrderCounter = queryIndirectSalesOrder.Count();
            var StoreHasCheckedCounter = queryStoreHasCheckedCounter.Select(x => x.Id).Distinct().Count();
            var StoreCheckingCounter = queryStoreChecking.Count();

            DashboardDirector_StatisticDailyDTO DashboardDirector_StatisticDailyDTO = new DashboardDirector_StatisticDailyDTO()
            {
                Revenue = RevenueTotal,
                IndirectSalesOrderCounter = IndirectSalesOrderCounter,
                StoreHasCheckedCounter = StoreHasCheckedCounter,
                StoreCheckingCounter = StoreCheckingCounter
            };
            return DashboardDirector_StatisticDailyDTO;
        }

        [Route(DashboardDirectorRoute.StoreCoverage), HttpPost]
        public async Task<List<DashboardDirector_StoreDTO>> StoreCoverage([FromBody] DashboardDirector_StoreFilterDTO DashboardDirector_StoreFilterDTO)
        {
            var ProvinceId = DashboardDirector_StoreFilterDTO.ProvinceId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_StoreFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_StoreFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);

            long? SaleEmployeeId = DashboardDirector_StoreFilterDTO.AppUserId?.Equal;
            if (SaleEmployeeId.HasValue)
            {
                var AppUserStoreMappings = await DataContext.AppUserStoreMapping.Where(x => x.AppUserId == SaleEmployeeId.Value).ToListAsync();
                if(AppUserStoreMappings.Count > 0)
                {
                    var subStoreIds = AppUserStoreMappings.Select(x => x.StoreId).ToList();
                    StoreIds = StoreIds.Intersect(subStoreIds).ToList();
                }
                else
                {
                    StoreFilter StoreFilter = new StoreFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        Selects = StoreSelect.Id,
                        StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                    };
                    List<Store> Stores = await StoreService.ListInScoped(StoreFilter, SaleEmployeeId.Value);
                    var subStoreIds = Stores.Select(x => x.Id).ToList();
                    StoreIds = StoreIds.Intersect(subStoreIds).ToList();
                }
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from s in DataContext.Store
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value))
                        && s.StatusId == StatusEnum.ACTIVE.Id
                        && s.DeletedAt == null
                        select new DashboardDirector_StoreDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Address = s.Address,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Telephone = s.OwnerPhone,
                            StoreStatusId = s.StoreStatusId,
                            IsScouting = false
                        };
            List<DashboardDirector_StoreDTO> DashboardMonitor_StoreDTOs = await query.Distinct().ToListAsync();

            var query_Scouting = from ss in DataContext.StoreScouting
                                 join au in DataContext.AppUser on ss.CreatorId equals au.Id
                                 where (OrganizationIds.Contains(au.OrganizationId))
                                 && ss.StoreScoutingStatusId == StoreScoutingStatusEnum.NOTOPEN.Id // chi lay cua hang du thao chua chinh thuc
                                 && ss.DeletedAt == null
                                 select new DashboardDirector_StoreDTO
                                 {
                                     Id = ss.Id,
                                     Name = ss.Name,
                                     Address = ss.Address,
                                     Latitude = ss.Latitude,
                                     Longitude = ss.Longitude,
                                     Telephone = ss.OwnerPhone,
                                     IsScouting = true
                                 };
            List<DashboardDirector_StoreDTO> DashboardMonitor_StoreScotingDTOs = await query_Scouting.Distinct().ToListAsync();
            DashboardMonitor_StoreDTOs.AddRange(DashboardMonitor_StoreScotingDTOs);
            return DashboardMonitor_StoreDTOs;
        }

        [Route(DashboardDirectorRoute.SaleEmployeeLocation), HttpPost]
        public async Task<List<DashboardDirector_AppUserDTO>> SaleEmployeeLocation([FromBody] DashboardDirector_StoreFilterDTO DashboardDirector_StoreFilterDTO)
        {
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(DashboardDirector_StoreFilterDTO.Time);

            long? SaleEmployeeId = DashboardDirector_StoreFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_StoreFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_StoreFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from au in DataContext.AppUser
                        where au.StatusId == Enums.StatusEnum.ACTIVE.Id &&
                        AppUserIds.Contains(au.Id) &&
                        (SaleEmployeeId.HasValue == false || au.Id == SaleEmployeeId.Value) &&
                        OrganizationIds.Contains(au.OrganizationId) &&
                        au.DeletedAt == null &&
                        Start <= au.GPSUpdatedAt && au.GPSUpdatedAt <= End
                        select new DashboardDirector_AppUserDTO
                        {
                            Id = au.Id,
                            DisplayName = au.DisplayName,
                            Username = au.Username,
                            Phone = au.Phone,
                            Longitude = au.Longitude,
                            Latitude = au.Latitude,
                        };

            List<DashboardDirector_AppUserDTO> DashboardDirector_AppUserDTOs = await query.Distinct().ToListAsync();
            return DashboardDirector_AppUserDTOs;
        }

        [Route(DashboardDirectorRoute.ListIndirectSalesOrder), HttpPost]
        public async Task<List<DashboardDirector_IndirectSalesOrderDTO>> ListIndirectSalesOrder([FromBody] DashboardDirector_IndirectSalesOrderFluctuationFilterDTO DashboardDirector_IndirectSalesOrderFluctuationFilterDTO)
        {
            var ProvinceId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var appUser = await AppUserService.Get(CurrentContext.UserId);
            var query = from i in DataContext.IndirectSalesOrder
                        join r in DataContext.RequestState on i.RequestStateId equals r.Id
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join o in DataContext.Organization on au.OrganizationId equals o.Id
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where AppUserIds.Contains(i.SaleEmployeeId) &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId != RequestStateEnum.NEW.Id &&
                        au.DeletedAt == null && o.DeletedAt == null
                        orderby i.OrderDate descending
                        select new DashboardDirector_IndirectSalesOrderDTO
                        {
                            Id = i.Id,
                            Code = i.Code,
                            OrderDate = i.OrderDate,
                            RequestStateId = r.Id,
                            SaleEmployeeId = i.SaleEmployeeId,
                            Total = i.Total,
                            SaleEmployee = i.SaleEmployee == null ? null : new DashboardDirector_AppUserDTO
                            {
                                Id = i.SaleEmployee.Id,
                                DisplayName = i.SaleEmployee.DisplayName,
                                Username = i.SaleEmployee.Username,
                            },
                            RequestState = new DashboardDirector_RequestStateDTO
                            {
                                Id = r.Id,
                                Code = r.Code,
                                Name = r.Name,
                            }
                        };

            return await query.Skip(0).Take(5).ToListAsync();
        }

        [Route(DashboardDirectorRoute.Top5RevenueByProduct), HttpPost]
        public async Task<List<DashboardDirector_Top5RevenueByProductDTO>> Top5RevenueByProduct([FromBody] DashboardDirector_Top5RevenueByProductFilterDTO DashboardDirector_Top5RevenueByProductFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);
            (Start, End) = ConvertTime(DashboardDirector_Top5RevenueByProductFilterDTO.Time);

            var ProvinceId = DashboardDirector_Top5RevenueByProductFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_Top5RevenueByProductFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_Top5RevenueByProductFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_Top5RevenueByProductFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var query = from t in DataContext.IndirectSalesOrderTransaction
                        join o in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals o.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join au in DataContext.AppUser on o.SaleEmployeeId equals au.Id
                        join s in DataContext.Store on t.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where t.OrderDate >= Start && t.OrderDate <= End &&
                        AppUserIds.Contains(t.SalesEmployeeId) &&
                        (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        OrganizationIds.Contains(t.OrganizationId) &&
                        o.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        au.DeletedAt == null && o.DeletedAt == null && i.DeletedAt == null && p.DeletedAt == null
                        group t by p.Name into x
                        select new DashboardDirector_Top5RevenueByProductDTO
                        {
                            ProductName = x.Key,
                            Revenue = x.Sum(y => y.Revenue ?? 0)
                        };

            List<DashboardDirector_Top5RevenueByProductDTO> DashboardDirector_Top5RevenueByProductDTOs = await query.OrderByDescending(x => x.Revenue)
                .Skip(0).Take(5).ToListAsync();
            return DashboardDirector_Top5RevenueByProductDTOs;
        }

        [Route(DashboardDirectorRoute.Top5RevenueByEmployee), HttpPost]
        public async Task<List<DashboardDirector_Top5RevenueByEmployeeDTO>> Top5RevenueByEmployee([FromBody] DashboardDirector_Top5RevenueByEmployeeFilterDTO DashboardDirector_Top5RevenueByEmployeeFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = new DateTime(Now.Year, Now.Month, Now.Day);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);
            (Start, End) = ConvertTime(DashboardDirector_Top5RevenueByEmployeeFilterDTO.Time);

            var ProvinceId = DashboardDirector_Top5RevenueByEmployeeFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_Top5RevenueByEmployeeFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_Top5RevenueByEmployeeFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_Top5RevenueByEmployeeFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            var query = from t in DataContext.IndirectSalesOrderTransaction
                        join o in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals o.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join au in DataContext.AppUser on o.SaleEmployeeId equals au.Id
                        join s in DataContext.Store on t.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where t.OrderDate >= Start && t.OrderDate <= End &&
                        AppUserIds.Contains(t.SalesEmployeeId) &&
                        (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                        OrganizationIds.Contains(t.OrganizationId) &&
                        o.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        au.DeletedAt == null && o.DeletedAt == null && i.DeletedAt == null && p.DeletedAt == null
                        group t by au.DisplayName into x
                        select new DashboardDirector_Top5RevenueByEmployeeDTO
                        {
                            EmployeeName = x.Key,
                            Revenue = x.Sum(y => y.Revenue ?? 0)
                        };

            List<DashboardDirector_Top5RevenueByEmployeeDTO>
                DashboardDirector_Top5RevenueByEmployeeDTOs = await query.OrderByDescending(x => x.Revenue)
                .Skip(0).Take(5).ToListAsync();
            return DashboardDirector_Top5RevenueByEmployeeDTOs;
        }

        [Route(DashboardDirectorRoute.RevenueFluctuation), HttpPost]
        public async Task<DashboardDirector_RevenueFluctuationDTO> RevenueFluctuation([FromBody] DashboardDirector_RevenueFluctuationFilterDTO DashboardDirector_RevenueFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);
            (Start, End) = ConvertTime(DashboardDirector_RevenueFluctuationFilterDTO.Time);

            var ProvinceId = DashboardDirector_RevenueFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_RevenueFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_RevenueFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_RevenueFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            if (DashboardDirector_RevenueFluctuationFilterDTO.Time.Equal.HasValue == false
                || DashboardDirector_RevenueFluctuationFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                            join s in DataContext.Store on t.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where t.OrderDate >= Start && t.OrderDate <= End &&
                            AppUserIds.Contains(t.SalesEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(t.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardDirector_RevenueFluctuationDTO DashboardDirector_RevenueFluctuationDTO = new DashboardDirector_RevenueFluctuationDTO();
                DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByMonths = new List<DashboardDirector_RevenueFluctuationByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.Year, Start.Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardDirector_RevenueFluctuationByMonthDTO RevenueFluctuationByMonth = new DashboardDirector_RevenueFluctuationByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByMonths.Add(RevenueFluctuationByMonth);
                }

                foreach (var RevenueFluctuationByMonth in DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.Year, Now.Month, (int)RevenueFluctuationByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddDays(1).AddSeconds(-1);
                    RevenueFluctuationByMonth.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardDirector_RevenueFluctuationDTO;
            }
            else if (DashboardDirector_RevenueFluctuationFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                            join s in DataContext.Store on t.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where t.OrderDate >= Start && t.OrderDate <= End &&
                            AppUserIds.Contains(t.SalesEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(t.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardDirector_RevenueFluctuationDTO DashboardDirector_RevenueFluctuationDTO = new DashboardDirector_RevenueFluctuationDTO();
                DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByMonths = new List<DashboardDirector_RevenueFluctuationByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.Year, Start.Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardDirector_RevenueFluctuationByMonthDTO RevenueFluctuationByMonth = new DashboardDirector_RevenueFluctuationByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByMonths.Add(RevenueFluctuationByMonth);
                }

                foreach (var RevenueFluctuationByMonth in DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.Year, Now.Month, (int)RevenueFluctuationByMonth.Day).AddMonths(-1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddDays(1).AddSeconds(-1);
                    RevenueFluctuationByMonth.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardDirector_RevenueFluctuationDTO;
            }
            else if (DashboardDirector_RevenueFluctuationFilterDTO.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                            join s in DataContext.Store on t.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where t.OrderDate >= Start && t.OrderDate <= End &&
                            AppUserIds.Contains(t.SalesEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(t.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardDirector_RevenueFluctuationDTO DashboardDirector_RevenueFluctuationDTO = new DashboardDirector_RevenueFluctuationDTO();
                DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByQuaters = new List<DashboardDirector_RevenueFluctuationByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardDirector_RevenueFluctuationByQuarterDTO RevenueFluctuationByQuarter = new DashboardDirector_RevenueFluctuationByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByQuaters.Add(RevenueFluctuationByQuarter);
                }

                foreach (var RevenueFluctuationByQuarter in DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.Year, (int)RevenueFluctuationByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddMonths(1).AddSeconds(-1);
                    RevenueFluctuationByQuarter.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardDirector_RevenueFluctuationDTO;
            }
            else if (DashboardDirector_RevenueFluctuationFilterDTO.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));

                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                            join s in DataContext.Store on t.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where t.OrderDate >= Start && t.OrderDate <= End &&
                            AppUserIds.Contains(t.SalesEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(t.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardDirector_RevenueFluctuationDTO DashboardDirector_RevenueFluctuationDTO = new DashboardDirector_RevenueFluctuationDTO();
                DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByQuaters = new List<DashboardDirector_RevenueFluctuationByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardDirector_RevenueFluctuationByQuarterDTO RevenueFluctuationByQuarter = new DashboardDirector_RevenueFluctuationByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByQuaters.Add(RevenueFluctuationByQuarter);
                }

                foreach (var RevenueFluctuationByQuarter in DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.Year, (int)RevenueFluctuationByQuarter.Month, 1).AddMonths(-3).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddMonths(1).AddSeconds(-1);
                    RevenueFluctuationByQuarter.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardDirector_RevenueFluctuationDTO;
            }
            else if (DashboardDirector_RevenueFluctuationFilterDTO.Time.Equal.Value == YEAR)
            {
                var query = from t in DataContext.IndirectSalesOrderTransaction
                            join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                            join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                            join s in DataContext.Store on t.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where t.OrderDate >= Start && t.OrderDate <= End &&
                            AppUserIds.Contains(t.SalesEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(t.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null
                            select new IndirectSalesOrderTransactionDAO
                            {
                                OrderDate = t.OrderDate,
                                Revenue = t.Revenue
                            };

                var IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                DashboardDirector_RevenueFluctuationDTO DashboardDirector_RevenueFluctuationDTO = new DashboardDirector_RevenueFluctuationDTO();
                DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByYears = new List<DashboardDirector_RevenueFluctuationByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    DashboardDirector_RevenueFluctuationByYearDTO RevenueFluctuationByYear = new DashboardDirector_RevenueFluctuationByYearDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByYears.Add(RevenueFluctuationByYear);
                }

                foreach (var RevenueFluctuationByYear in DashboardDirector_RevenueFluctuationDTO.RevenueFluctuationByYears)
                {
                    DateTime LocalStart = new DateTime(Now.Year, (int)RevenueFluctuationByYear.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddMonths(1).AddSeconds(-1);
                    RevenueFluctuationByYear.Revenue = IndirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return DashboardDirector_RevenueFluctuationDTO;
            }
            return new DashboardDirector_RevenueFluctuationDTO();
        }

        [Route(DashboardDirectorRoute.IndirectSalesOrderFluctuation), HttpPost]
        public async Task<DashboardDirector_IndirectSalesOrderFluctuationDTO> IndirectSalesOrderFluctuation([FromBody] DashboardDirector_IndirectSalesOrderFluctuationFilterDTO DashboardDirector_IndirectSalesOrderFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);
            (Start, End) = ConvertTime(DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time);

            var ProvinceId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time.Equal.HasValue == false
                || DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group i by i.OrderDate.Day into x
                            select new DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO
                            {
                                Day = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardDirector_IndirectSalesOrderFluctuationByMonthDTOs = await query.ToListAsync();
                DashboardDirector_IndirectSalesOrderFluctuationDTO DashboardDirector_IndirectSalesOrderFluctuationDTO = new DashboardDirector_IndirectSalesOrderFluctuationDTO();
                DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByMonths = new List<DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.Year, Start.Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO IndirectSalesOrderFluctuationByMonth = new DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO
                    {
                        Day = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByMonths.Add(IndirectSalesOrderFluctuationByMonth);
                }

                foreach (var IndirectSalesOrderFluctuationByMonth in DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByMonths)
                {
                    var data = DashboardDirector_IndirectSalesOrderFluctuationByMonthDTOs.Where(x => x.Day == IndirectSalesOrderFluctuationByMonth.Day).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderFluctuationByMonth.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardDirector_IndirectSalesOrderFluctuationDTO;
            }
            else if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group i by i.OrderDate.Day into x
                            select new DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO
                            {
                                Day = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardDirector_IndirectSalesOrderFluctuationByMonthDTOs = await query.ToListAsync();
                DashboardDirector_IndirectSalesOrderFluctuationDTO DashboardDirector_IndirectSalesOrderFluctuationDTO = new DashboardDirector_IndirectSalesOrderFluctuationDTO();
                DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByMonths = new List<DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.Year, Start.Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO IndirectSalesOrderFluctuationByMonth = new DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO
                    {
                        Day = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByMonths.Add(IndirectSalesOrderFluctuationByMonth);
                }

                foreach (var IndirectSalesOrderFluctuationByMonth in DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByMonths)
                {
                    var data = DashboardDirector_IndirectSalesOrderFluctuationByMonthDTOs.Where(x => x.Day == IndirectSalesOrderFluctuationByMonth.Day).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderFluctuationByMonth.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardDirector_IndirectSalesOrderFluctuationDTO;
            }
            else if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));

                var query = from i in DataContext.IndirectSalesOrder
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group i by i.OrderDate.Month into x
                            select new DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTOs = await query.ToListAsync();
                DashboardDirector_IndirectSalesOrderFluctuationDTO DashboardDirector_IndirectSalesOrderFluctuationDTO = new DashboardDirector_IndirectSalesOrderFluctuationDTO();
                DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByQuaters = new List<DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO IndirectSalesOrderFluctuationByQuarter = new DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByQuaters.Add(IndirectSalesOrderFluctuationByQuarter);
                }

                foreach (var IndirectSalesOrderFluctuationByQuater in DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByQuaters)
                {
                    var data = DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTOs.Where(x => x.Month == IndirectSalesOrderFluctuationByQuater.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderFluctuationByQuater.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardDirector_IndirectSalesOrderFluctuationDTO;
            }
            else if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));

                var query = from i in DataContext.IndirectSalesOrder
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group i by i.OrderDate.Month into x
                            select new DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTOs = await query.ToListAsync();
                DashboardDirector_IndirectSalesOrderFluctuationDTO DashboardDirector_IndirectSalesOrderFluctuationDTO = new DashboardDirector_IndirectSalesOrderFluctuationDTO();
                DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByQuaters = new List<DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO IndirectSalesOrderFluctuationByQuarter = new DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByQuaters.Add(IndirectSalesOrderFluctuationByQuarter);
                }

                foreach (var IndirectSalesOrderFluctuationByQuater in DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByQuaters)
                {
                    var data = DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTOs.Where(x => x.Month == IndirectSalesOrderFluctuationByQuater.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderFluctuationByQuater.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardDirector_IndirectSalesOrderFluctuationDTO;
            }
            else if (DashboardDirector_IndirectSalesOrderFluctuationFilterDTO.Time.Equal.Value == YEAR)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group i by i.OrderDate.Month into x
                            select new DashboardDirector_IndirectSalesOrderFluctuationByYearDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var DashboardDirector_IndirectSalesOrderFluctuationByYearDTO = await query.ToListAsync();
                DashboardDirector_IndirectSalesOrderFluctuationDTO DashboardDirector_IndirectSalesOrderFluctuationDTO = new DashboardDirector_IndirectSalesOrderFluctuationDTO();
                DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByYears = new List<DashboardDirector_IndirectSalesOrderFluctuationByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    DashboardDirector_IndirectSalesOrderFluctuationByYearDTO IndirectSalesOrderFluctuationByYear = new DashboardDirector_IndirectSalesOrderFluctuationByYearDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByYears.Add(IndirectSalesOrderFluctuationByYear);
                }

                foreach (var IndirectSalesOrderFluctuationByYear in DashboardDirector_IndirectSalesOrderFluctuationDTO.IndirectSalesOrderFluctuationByYears)
                {
                    var data = DashboardDirector_IndirectSalesOrderFluctuationByYearDTO.Where(x => x.Month == IndirectSalesOrderFluctuationByYear.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderFluctuationByYear.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return DashboardDirector_IndirectSalesOrderFluctuationDTO;
            }
            return new DashboardDirector_IndirectSalesOrderFluctuationDTO();
        }

        [Route(DashboardDirectorRoute.SaledItemFluctuation), HttpPost]
        public async Task<DashboardDirector_SaledItemFluctuationDTO> SaledItemFluctuation([FromBody] DashboardDirector_SaledItemFluctuationFilterDTO DashboardDirector_SaledItemFluctuationFilterDTO)
        {
            DateTime Now = StaticParams.DateTimeNow.Date;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = new DateTime(Now.Year, Now.Month, Now.Day);
            (Start, End) = ConvertTime(DashboardDirector_SaledItemFluctuationFilterDTO.Time);

            var ProvinceId = DashboardDirector_SaledItemFluctuationFilterDTO.ProvinceId?.Equal;
            long? SaleEmployeeId = DashboardDirector_SaledItemFluctuationFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardDirector_SaledItemFluctuationFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardDirector_SaledItemFluctuationFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

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

            if (DashboardDirector_SaledItemFluctuationFilterDTO.Time.Equal.HasValue == false
                || DashboardDirector_SaledItemFluctuationFilterDTO.Time.Equal.Value == THIS_MONTH)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group ic by i.OrderDate.Day into x
                            select new DashboardDirector_SaledItemFluctuationByMonthDTO
                            {
                                Day = x.Key,
                                SaledItemCounter = x.Sum(x => x.RequestedQuantity)
                            };

                var DashboardDirector_SaledItemFluctuationByMonthDTOs = await query.ToListAsync();
                DashboardDirector_SaledItemFluctuationDTO DashboardDirector_SaledItemFluctuationDTO = new DashboardDirector_SaledItemFluctuationDTO();
                DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByMonths = new List<DashboardDirector_SaledItemFluctuationByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.Year, Start.Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardDirector_SaledItemFluctuationByMonthDTO SaledItemFluctuationByMonth = new DashboardDirector_SaledItemFluctuationByMonthDTO
                    {
                        Day = i,
                        SaledItemCounter = 0
                    };
                    DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByMonths.Add(SaledItemFluctuationByMonth);
                }

                foreach (var SaledItemFluctuationByMonth in DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByMonths)
                {
                    var data = DashboardDirector_SaledItemFluctuationByMonthDTOs.Where(x => x.Day == SaledItemFluctuationByMonth.Day).FirstOrDefault();
                    if (data != null)
                        SaledItemFluctuationByMonth.SaledItemCounter = data.SaledItemCounter;
                }

                return DashboardDirector_SaledItemFluctuationDTO;
            }
            else if (DashboardDirector_SaledItemFluctuationFilterDTO.Time.Equal.Value == LAST_MONTH)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group ic by i.OrderDate.Day into x
                            select new DashboardDirector_SaledItemFluctuationByMonthDTO
                            {
                                Day = x.Key,
                                SaledItemCounter = x.Sum(x => x.RequestedQuantity)
                            };

                var DashboardDirector_SaledItemFluctuationByMonthDTOs = await query.ToListAsync();
                DashboardDirector_SaledItemFluctuationDTO DashboardDirector_SaledItemFluctuationDTO = new DashboardDirector_SaledItemFluctuationDTO();
                DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByMonths = new List<DashboardDirector_SaledItemFluctuationByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.Year, Start.Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    DashboardDirector_SaledItemFluctuationByMonthDTO SaledItemFluctuationByMonth = new DashboardDirector_SaledItemFluctuationByMonthDTO
                    {
                        Day = i,
                        SaledItemCounter = 0
                    };
                    DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByMonths.Add(SaledItemFluctuationByMonth);
                }

                foreach (var SaledItemFluctuationByMonth in DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByMonths)
                {
                    var data = DashboardDirector_SaledItemFluctuationByMonthDTOs.Where(x => x.Day == SaledItemFluctuationByMonth.Day).FirstOrDefault();
                    if (data != null)
                        SaledItemFluctuationByMonth.SaledItemCounter = data.SaledItemCounter;
                }

                return DashboardDirector_SaledItemFluctuationDTO;
            }
            else if (DashboardDirector_SaledItemFluctuationFilterDTO.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));

                var query = from i in DataContext.IndirectSalesOrder
                            join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group ic by i.OrderDate.Month into x
                            select new DashboardDirector_SaledItemFluctuationByQuarterDTO
                            {
                                Month = x.Key,
                                SaledItemCounter = x.Sum(x => x.RequestedQuantity)
                            };

                var DashboardDirector_SaledItemFluctuationByQuarterDTOs = await query.ToListAsync();
                DashboardDirector_SaledItemFluctuationDTO DashboardDirector_SaledItemFluctuationDTO = new DashboardDirector_SaledItemFluctuationDTO();
                DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByQuaters = new List<DashboardDirector_SaledItemFluctuationByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardDirector_SaledItemFluctuationByQuarterDTO SaledItemFluctuationByQuarter = new DashboardDirector_SaledItemFluctuationByQuarterDTO
                    {
                        Month = i,
                        SaledItemCounter = 0
                    };
                    DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByQuaters.Add(SaledItemFluctuationByQuarter);
                }

                foreach (var SaledItemFluctuationByQuater in DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByQuaters)
                {
                    var data = DashboardDirector_SaledItemFluctuationByQuarterDTOs.Where(x => x.Month == SaledItemFluctuationByQuater.Month).FirstOrDefault();
                    if (data != null)
                        SaledItemFluctuationByQuater.SaledItemCounter = data.SaledItemCounter;
                }

                return DashboardDirector_SaledItemFluctuationDTO;
            }
            else if (DashboardDirector_SaledItemFluctuationFilterDTO.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));

                var query = from i in DataContext.IndirectSalesOrder
                            join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group ic by i.OrderDate.Month into x
                            select new DashboardDirector_SaledItemFluctuationByQuarterDTO
                            {
                                Month = x.Key,
                                SaledItemCounter = x.Sum(x => x.RequestedQuantity)
                            };

                var DashboardDirector_SaledItemFluctuationByQuarterDTOs = await query.ToListAsync();
                DashboardDirector_SaledItemFluctuationDTO DashboardDirector_SaledItemFluctuationDTO = new DashboardDirector_SaledItemFluctuationDTO();
                DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByQuaters = new List<DashboardDirector_SaledItemFluctuationByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    DashboardDirector_SaledItemFluctuationByQuarterDTO SaledItemFluctuationByQuarter = new DashboardDirector_SaledItemFluctuationByQuarterDTO
                    {
                        Month = i,
                        SaledItemCounter = 0
                    };
                    DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByQuaters.Add(SaledItemFluctuationByQuarter);
                }

                foreach (var SaledItemFluctuationByQuater in DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByQuaters)
                {
                    var data = DashboardDirector_SaledItemFluctuationByQuarterDTOs.Where(x => x.Month == SaledItemFluctuationByQuater.Month).FirstOrDefault();
                    if (data != null)
                        SaledItemFluctuationByQuater.SaledItemCounter = data.SaledItemCounter;
                }

                return DashboardDirector_SaledItemFluctuationDTO;
            }
            else if (DashboardDirector_SaledItemFluctuationFilterDTO.Time.Equal.Value == YEAR)
            {
                var query = from i in DataContext.IndirectSalesOrder
                            join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                            join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                            join o in DataContext.Organization on au.OrganizationId equals o.Id
                            join s in DataContext.Store on i.BuyerStoreId equals s.Id
                            join tt in tempTableQuery.Query on s.Id equals tt.Column1
                            where i.OrderDate >= Start && i.OrderDate <= End &&
                            AppUserIds.Contains(i.SaleEmployeeId) &&
                            (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                            (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId == ProvinceId.Value)) &&
                            OrganizationIds.Contains(i.OrganizationId) &&
                            i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                            au.DeletedAt == null && o.DeletedAt == null
                            group ic by i.OrderDate.Month into x
                            select new DashboardDirector_SaledItemFluctuationByYearDTO
                            {
                                Month = x.Key,
                                SaledItemCounter = x.Sum(x => x.RequestedQuantity)
                            };

                var DashboardDirector_SaledItemFluctuationByYearDTO = await query.ToListAsync();
                DashboardDirector_SaledItemFluctuationDTO DashboardDirector_SaledItemFluctuationDTO = new DashboardDirector_SaledItemFluctuationDTO();
                DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByYears = new List<DashboardDirector_SaledItemFluctuationByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    DashboardDirector_SaledItemFluctuationByYearDTO SaledItemFluctuationByYear = new DashboardDirector_SaledItemFluctuationByYearDTO
                    {
                        Month = i,
                        SaledItemCounter = 0
                    };
                    DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByYears.Add(SaledItemFluctuationByYear);
                }

                foreach (var SaledItemFluctuationByYear in DashboardDirector_SaledItemFluctuationDTO.SaledItemFluctuationByYears)
                {
                    var data = DashboardDirector_SaledItemFluctuationByYearDTO.Where(x => x.Month == SaledItemFluctuationByYear.Month).FirstOrDefault();
                    if (data != null)
                        SaledItemFluctuationByYear.SaledItemCounter = data.SaledItemCounter;
                }

                return DashboardDirector_SaledItemFluctuationDTO;
            }
            return new DashboardDirector_SaledItemFluctuationDTO();
        }

        private Tuple<DateTime, DateTime> ConvertTime(IdFilter Time)
        {
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = Start.AddDays(1).AddSeconds(-1);
            DateTime Now = StaticParams.DateTimeNow.Date;
            if (Time.Equal.HasValue == false)
            {
                Time.Equal = 0;
                Start = LocalStartDay(CurrentContext);
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (Time.Equal.Value == TODAY)
            {
                Start = LocalStartDay(CurrentContext);
                End = Start.AddDays(1).AddSeconds(-1);
            }
            else if (Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = LocalStartDay(CurrentContext).AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.Year, Now.Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.Year, Now.Month, 1).AddMonths(-1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.Year, Now.Month, 1).AddMonths(-1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));
                Start = new DateTime(Now.Year, (this_quarter - 1) * 3 + 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.Month / 3m));
                Start = new DateTime(Now.Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(-3).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(-3).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == YEAR)
            {
                Start = new DateTime(Now.Year, 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.Year, 1, 1).AddYears(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            return Tuple.Create(Start, End);
        }
    }
}
