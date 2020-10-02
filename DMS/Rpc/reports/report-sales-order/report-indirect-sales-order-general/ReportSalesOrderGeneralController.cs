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
using System.IO;
using System.Dynamic;
using NGS.Templater;
using DMS.Services.MStoreStatus;

namespace DMS.Rpc.reports.report_sales_order.report_indirect_sales_order_general
{
    public class ReportSalesOrderGeneralController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreStatusService StoreStatusService;
        private ICurrentContext CurrentContext;
        public ReportSalesOrderGeneralController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreStatusService StoreStatusService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreStatusService = StoreStatusService;
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
        public async Task<List<ReportSalesOrderGeneral_OrganizationDTO>> FilterListOrganization()
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
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportSalesOrderGeneral_StoreDTO> ReportSalesOrderGeneral_StoreDTOs = Stores
                .Select(x => new ReportSalesOrderGeneral_StoreDTO(x)).ToList();
            return ReportSalesOrderGeneral_StoreDTOs;
        }

        [Route(ReportSalesOrderGeneralRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportSalesOrderGeneral_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportSalesOrderGeneral_StoreStatusFilterDTO ReportSalesOrderGeneral_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportSalesOrderGeneral_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportSalesOrderGeneral_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportSalesOrderGeneral_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportSalesOrderGeneral_StoreStatusDTO> ReportSalesOrderGeneral_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportSalesOrderGeneral_StoreStatusDTO(x)).ToList();
            return ReportSalesOrderGeneral_StoreStatusDTOs;
        }

        [Route(ReportSalesOrderGeneralRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? SaleEmployeeId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? SellerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.SellerStoreId?.Equal;
            long? StoreStatusId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.StoreStatusId?.Equal;

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
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (SellerStoreId.HasValue == false || i.SellerStoreId == SellerStoreId.Value) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportSalesOrderGeneralRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO>>> List([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.HasValue == false)
                return new List<ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO>();

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? SaleEmployeeId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? SellerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.SellerStoreId?.Equal;
            long? StoreStatusId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from i in DataContext.IndirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (SellerStoreId.HasValue == false || i.SellerStoreId == SellerStoreId.Value) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await query
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.OrderDate)
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
                Selects = StoreSelect.Id | StoreSelect.Name | StoreSelect.StoreStatus
            });
            var OrgIds = IndirectSalesOrderDAOs.Select(x => x.OrganizationId).Distinct().ToList();
            var Orgs = OrganizationDAOs.Where(x => OrgIds.Contains(x.Id)).ToList();
            List<string> OrganizationNames = Orgs.Select(o => o.Name).Distinct().ToList();
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
                       StoreStatus = x.StoreStatus == null ? null : new StoreStatusDAO
                       {
                           Name = x.StoreStatus.Name
                       }
                   }).FirstOrDefault();
            }
            foreach (ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO in ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs)
            {
                var Org = OrganizationDAOs.Where(x => x.Name == ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO.OrganizationName).FirstOrDefault();
                ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO.SalesOrders = IndirectSalesOrderDAOs
                    .Where(x => x.OrganizationId==Org.Id)
                    .Select(x => new ReportSalesOrderGeneral_IndirectSalesOrderDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        BuyerStoreName = x.BuyerStore.Name,
                        BuyerStoreStatusName = x.BuyerStore.StoreStatus.Name,
                        SellerStoreName = x.SellerStore.Name,
                        SaleEmployeeName = x.SaleEmployee.DisplayName,
                        OrderDate = x.OrderDate,
                        Discount = x.GeneralDiscountAmount ?? 0,
                        TaxValue = x.TotalTaxAmount,
                        SubTotal = x.SubTotal,
                        Total = x.Total,
                    })
                    .ToList();
            }

            return ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs.Where(x => x.SalesOrders.Any()).ToList();
        }

        [Route(ReportSalesOrderGeneralRoute.Total), HttpPost]
        public async Task<ReportSalesOrderGeneral_TotalDTO> Total([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.HasValue == false)
                return new ReportSalesOrderGeneral_TotalDTO();

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportSalesOrderGeneral_TotalDTO();

            long? SaleEmployeeId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? SellerStoreId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.SellerStoreId?.Equal;
            long? StoreStatusId = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from i in DataContext.IndirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (SellerStoreId.HasValue == false || i.SellerStoreId == SellerStoreId.Value) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        (AppUserIds.Contains(i.SaleEmployeeId)) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await query.ToListAsync();
            ReportSalesOrderGeneral_TotalDTO ReportSalesOrderGeneral_TotalDTO = new ReportSalesOrderGeneral_TotalDTO
            {
                TotalDiscount = IndirectSalesOrderDAOs.Where(x => x.GeneralDiscountAmount.HasValue)
                .Select(x => x.GeneralDiscountAmount.Value)
                .DefaultIfEmpty(0).Sum(),
                TotalTax = IndirectSalesOrderDAOs.Select(x => x.TotalTaxAmount).DefaultIfEmpty(0).Sum(),
                SubTotal = IndirectSalesOrderDAOs.Select(x => x.SubTotal).DefaultIfEmpty(0).Sum(),
                TotalRevenue = IndirectSalesOrderDAOs.Select(x => x.Total).DefaultIfEmpty(0).Sum(),
            };

            return ReportSalesOrderGeneral_TotalDTO;
        }

        [Route(ReportSalesOrderGeneralRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.Skip = 0;
            ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO.Take = int.MaxValue;
            List<ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO> ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs = (await List(ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO)).Value;

            ReportSalesOrderGeneral_TotalDTO ReportSalesOrderGeneral_TotalDTO = await Total(ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO);
            long stt = 1;
            foreach (ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO in ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs)
            {
                foreach (var IndirectSalesOrder in ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO.SalesOrders)
                {
                    IndirectSalesOrder.STT = stt;
                    stt++;
                    IndirectSalesOrder.eOrderDate = IndirectSalesOrder.OrderDate.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
                }
            }

            string path = "Templates/Report_Sales_Order_General.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderGenerals = ReportSalesOrderGeneral_ReportSalesOrderGeneralDTOs;
            Data.Total = ReportSalesOrderGeneral_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportSalesOrderGeneral.xlsx");
        }
    }
}
