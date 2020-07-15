using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Services.MOrganization;
using Microsoft.AspNetCore.Mvc;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using System;
using Helpers;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MProduct;
using DMS.Services.MAppUser;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_general
{
    public class ReportSalesOrderGeneralController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private ICurrentContext CurrentContext;
        public ReportSalesOrderGeneralController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReportSalesOrderGeneralRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportSalesOrderGeneral_AppUserDTO>> FilterListAppUser([FromBody] ReportSalesOrderGeneral_AppUserFilterDTO ReportSalesOrderGeneral_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportSalesOrderGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportSalesOrderGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportSalesOrderGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportSalesOrderGeneral_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportSalesOrderGeneral_AppUserDTO> StoreCheckerReport_AppUserDTOs = AppUsers
                .Select(x => new ReportSalesOrderGeneral_AppUserDTO(x)).ToList();
            return StoreCheckerReport_AppUserDTOs;
        }

        [Route(ReportSalesOrderGeneralRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportSalesOrderGeneral_OrganizationDTO>> FilterListOrganization([FromBody] ReportSalesOrderGeneral_OrganizationFilterDTO ReportSalesOrderGeneral_OrganizationFilterDTO)
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
            OrganizationFilter.Id = ReportSalesOrderGeneral_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ReportSalesOrderGeneral_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ReportSalesOrderGeneral_OrganizationFilterDTO.Name;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportSalesOrderGeneral_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportSalesOrderGeneral_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportSalesOrderGeneralRoute.FilterListStore), HttpPost]
        public async Task<List<ReportSalesOrderGeneral_StoreDTO>> FilterListStore([FromBody] ReportSalesOrderGeneral_StoreFilterDTO ReportSalesOrderGeneral_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportSalesOrderGeneral_StoreFilterDTO.Id;
            StoreFilter.Code = ReportSalesOrderGeneral_StoreFilterDTO.Code;
            StoreFilter.Name = ReportSalesOrderGeneral_StoreFilterDTO.Name;
            StoreFilter.OrganizationId = ReportSalesOrderGeneral_StoreFilterDTO.OrganizationId;
            StoreFilter.StatusId = ReportSalesOrderGeneral_StoreFilterDTO.StatusId;

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportSalesOrderGeneral_StoreDTO> ReportSalesOrderGeneral_StoreDTOs = Stores
                .Select(x => new ReportSalesOrderGeneral_StoreDTO(x)).ToList();
            return ReportSalesOrderGeneral_StoreDTOs;
        }

        [Route(ReportSalesOrderGeneralRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? SellerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.SellerStoreId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (SellerStoreId.HasValue == false || i.SellerStoreId == SellerStoreId.Value) &&
                        (au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
                        select i;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportSalesOrderGeneralRoute.List), HttpPost]
        public async Task<List<ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO>> List([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.HasValue == false)
                return new List<ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO>();

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? SellerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.SellerStoreId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var AppUsers = await AppUserService.List(new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            });
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (SellerStoreId.HasValue == false || i.SellerStoreId == SellerStoreId.Value) &&
                        (AppUserIds.Contains(i.SaleEmployeeId))
                        select i;

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await query
                .Skip(ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.Skip)
                .Take(ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.Take)
                .ToListAsync();
            var StoreIds = new List<long>();
            StoreIds.AddRange(IndirectSalesOrderDAOs.Select(x => x.SellerStoreId));
            StoreIds.AddRange(IndirectSalesOrderDAOs.Select(x => x.BuyerStoreId));
            StoreIds = StoreIds.Distinct().ToList();
            List<Store> Stores = await StoreService.List(new StoreFilter
            {
                Id = new IdFilter { In = StoreIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Id | StoreSelect.Name
            });

            List<string> OrganizationNames = AppUsers.Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO> ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs = OrganizationNames.Select(on => new ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (var IndirectSalesOrderDAO in IndirectSalesOrderDAOs)
            {
                IndirectSalesOrderDAO.SaleEmployee = AppUsers.Where(x => x.Id == IndirectSalesOrderDAO.SaleEmployeeId)
                    .Select(x => new AppUserDAO
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        Username = x.Username,
                    }).FirstOrDefault();
                IndirectSalesOrderDAO.SellerStore = Stores.Where(x => x.Id == IndirectSalesOrderDAO.SellerStoreId)
                    .Select(x => new StoreDAO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                    }).FirstOrDefault();
                IndirectSalesOrderDAO.BuyerStore = Stores.Where(x => x.Id == IndirectSalesOrderDAO.BuyerStoreId)
                   .Select(x => new StoreDAO
                   {
                       Id = x.Id,
                       Code = x.Code,
                       Name = x.Name,
                   }).FirstOrDefault();
            }
            foreach (ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO in ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs)
            {
                var Org = OrganizationDAOs.Where(x => x.Name == ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO.OrganizationName).FirstOrDefault();
                var SaleEmployeeIds = AppUsers.Where(x => x.OrganizationId.HasValue && x.OrganizationId.Value == Org.Id).Select(x => x.Id).ToList();
                ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO.IndirectSalesOrders = IndirectSalesOrderDAOs
                    .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId))
                    .Select(x => new ReportSalesOrderGeneral_IndirectSalesOrderDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        BuyerStoreName = x.BuyerStore.Name,
                        SellerStoreName = x.SellerStore.Name,
                        SaleEmployeeName = x.SaleEmployee.DisplayName,
                        OrderDate = x.OrderDate,
                        Discount = x.GeneralDiscountAmount ?? 0,
                        TaxValue = x.TotalTaxAmount,
                        Total = x.Total,
                    })
                    .ToList();
            }

            return ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs.Where(x => x.IndirectSalesOrders.Any()).ToList();
        }

        [Route(ReportSalesOrderGeneralRoute.Total), HttpPost]
        public async Task<ReportSalesOrderGeneral_TotalDTO> Total([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.HasValue == false)
                return new ReportSalesOrderGeneral_TotalDTO ();

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? SaleEmployeeId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? SellerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.SellerStoreId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var AppUsers = await AppUserService.List(new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            });
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (SellerStoreId.HasValue == false || i.SellerStoreId == SellerStoreId.Value) &&
                        (AppUserIds.Contains(i.SaleEmployeeId))
                        select i;

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await query.ToListAsync();
            ReportSalesOrderGeneral_TotalDTO ReportSalesOrderGeneral_TotalDTO = new ReportSalesOrderGeneral_TotalDTO
            {
                TotalDiscount = IndirectSalesOrderDAOs.Where(x => x.GeneralDiscountAmount.HasValue)
                .Select(x => x.GeneralDiscountAmount.Value)
                .DefaultIfEmpty(0).Sum(),
                TotalTax = IndirectSalesOrderDAOs.Select(x => x.TotalTaxAmount).DefaultIfEmpty(0).Sum(),
                TotalRevenue = IndirectSalesOrderDAOs.Select(x => x.Total).DefaultIfEmpty(0).Sum(),
            };

            return ReportSalesOrderGeneral_TotalDTO;
        }
    }
}
