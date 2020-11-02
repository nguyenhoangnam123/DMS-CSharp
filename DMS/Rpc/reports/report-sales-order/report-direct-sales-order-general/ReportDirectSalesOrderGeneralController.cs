using DMS.Common;
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
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MProduct;
using DMS.Services.MAppUser;
using System.IO;
using System.Dynamic;
using NGS.Templater;
using DMS.Services.MStoreStatus;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_general
{
    public class ReportDirectSalesOrderGeneralController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreStatusService StoreStatusService;
        private ICurrentContext CurrentContext;
        public ReportDirectSalesOrderGeneralController(
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

        [Route(ReportDirectSalesOrderGeneralRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportDirectSalesOrderGeneral_AppUserDTO>> FilterListAppUser([FromBody] ReportDirectSalesOrderGeneral_AppUserFilterDTO ReportDirectSalesOrderGeneral_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportDirectSalesOrderGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportDirectSalesOrderGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportDirectSalesOrderGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportDirectSalesOrderGeneral_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportDirectSalesOrderGeneral_AppUserDTO> StoreCheckerReport_AppUserDTOs = AppUsers
                .Select(x => new ReportDirectSalesOrderGeneral_AppUserDTO(x)).ToList();
            return StoreCheckerReport_AppUserDTOs;
        }

        [Route(ReportDirectSalesOrderGeneralRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportDirectSalesOrderGeneral_OrganizationDTO>> FilterListOrganization()
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
            List<ReportDirectSalesOrderGeneral_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportDirectSalesOrderGeneral_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportDirectSalesOrderGeneralRoute.FilterListStore), HttpPost]
        public async Task<List<ReportDirectSalesOrderGeneral_StoreDTO>> FilterListStore([FromBody] ReportDirectSalesOrderGeneral_StoreFilterDTO ReportDirectSalesOrderGeneral_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportDirectSalesOrderGeneral_StoreFilterDTO.Id;
            StoreFilter.Code = ReportDirectSalesOrderGeneral_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ReportDirectSalesOrderGeneral_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ReportDirectSalesOrderGeneral_StoreFilterDTO.Name;
            StoreFilter.OrganizationId = ReportDirectSalesOrderGeneral_StoreFilterDTO.OrganizationId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportDirectSalesOrderGeneral_StoreDTO> ReportDirectSalesOrderGeneral_StoreDTOs = Stores
                .Select(x => new ReportDirectSalesOrderGeneral_StoreDTO(x)).ToList();
            return ReportDirectSalesOrderGeneral_StoreDTOs;
        }

        [Route(ReportDirectSalesOrderGeneralRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportDirectSalesOrderGeneral_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportDirectSalesOrderGeneral_StoreStatusFilterDTO ReportDirectSalesOrderGeneral_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportDirectSalesOrderGeneral_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportDirectSalesOrderGeneral_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportDirectSalesOrderGeneral_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportDirectSalesOrderGeneral_StoreStatusDTO> ReportDirectSalesOrderGeneral_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportDirectSalesOrderGeneral_StoreStatusDTO(x)).ToList();
            return ReportDirectSalesOrderGeneral_StoreStatusDTOs;
        }

        [Route(ReportDirectSalesOrderGeneralRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? SaleEmployeeId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? StoreStatusId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            
            var query = from i in DataContext.DirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        s.DeletedAt == null
                        select i;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportDirectSalesOrderGeneralRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO>>> List([FromBody] ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.HasValue == false)
                return new List<ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO>();

            DateTime Start = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? SaleEmployeeId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? StoreStatusId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from i in DataContext.DirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        s.DeletedAt == null
                        select i;

            List<DirectSalesOrderDAO> DirectSalesOrderDAOs = await query
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.OrderDate)
                .ToListAsync();
            var StoreIds = DirectSalesOrderDAOs.Select(x => x.BuyerStoreId).Distinct().ToList();
            List<Store> Stores = await StoreService.List(new StoreFilter
            {
                Id = new IdFilter { In = StoreIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Id | StoreSelect.Name | StoreSelect.StoreStatus
            });
            var OrgIds = DirectSalesOrderDAOs.Select(x => x.OrganizationId).Distinct().ToList();
            var Orgs = OrganizationDAOs.Where(x => OrgIds.Contains(x.Id)).ToList();
            List<string> OrganizationNames = Orgs.Select(o => o.Name).Distinct().ToList();
            List<ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO> ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTOs = OrganizationNames.Select(on => new ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (var DirectSalesOrderDAO in DirectSalesOrderDAOs)
            {
                DirectSalesOrderDAO.SaleEmployee = AppUsers.Where(x => x.Id == DirectSalesOrderDAO.SaleEmployeeId)
                    .Select(x => new AppUserDAO
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        Username = x.Username,
                    }).FirstOrDefault();
                DirectSalesOrderDAO.BuyerStore = Stores.Where(x => x.Id == DirectSalesOrderDAO.BuyerStoreId)
                   .Select(x => new StoreDAO
                   {
                       Id = x.Id,
                       Code = x.Code,
                       CodeDraft = x.CodeDraft,
                       Name = x.Name,
                       StoreStatus = x.StoreStatus == null ? null : new StoreStatusDAO
                       {
                           Name = x.StoreStatus.Name
                       }
                   }).FirstOrDefault();
            }
            foreach (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO in ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTOs)
            {
                var Org = OrganizationDAOs.Where(x => x.Name == ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO.OrganizationName).FirstOrDefault();
                ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO.SalesOrders = DirectSalesOrderDAOs
                    .Where(x => x.OrganizationId==Org.Id)
                    .Select(x => new ReportDirectSalesOrderGeneral_DirectSalesOrderDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        BuyerStoreName = x.BuyerStore.Name,
                        BuyerStoreStatusName = x.BuyerStore.StoreStatus.Name,
                        SaleEmployeeName = x.SaleEmployee.DisplayName,
                        OrderDate = x.OrderDate,
                        Discount = x.GeneralDiscountAmount ?? 0,
                        TaxValue = x.TotalTaxAmount,
                        SubTotal = x.SubTotal,
                        TotalAfterTax = x.TotalAfterTax,
                        PromotionValue = x.PromotionValue.GetValueOrDefault(0),
                        Total = x.Total
                    })
                    .ToList();
            }

            return ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTOs.Where(x => x.SalesOrders.Any()).ToList();
        }

        [Route(ReportDirectSalesOrderGeneralRoute.Total), HttpPost]
        public async Task<ReportDirectSalesOrderGeneral_TotalDTO> Total([FromBody] ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.HasValue == false)
                return new ReportDirectSalesOrderGeneral_TotalDTO();

            DateTime Start = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportDirectSalesOrderGeneral_TotalDTO();

            long? SaleEmployeeId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.AppUserId?.Equal;
            long? BuyerStoreId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.BuyerStoreId?.Equal;
            long? StoreStatusId = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from i in DataContext.DirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        (BuyerStoreId.HasValue == false || i.BuyerStoreId == BuyerStoreId.Value) &&
                        (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                        (AppUserIds.Contains(i.SaleEmployeeId)) &&
                        OrganizationIds.Contains(i.OrganizationId) &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        s.DeletedAt == null
                        select i;

            List<DirectSalesOrderDAO> DirectSalesOrderDAOs = await query.ToListAsync();
            ReportDirectSalesOrderGeneral_TotalDTO ReportDirectSalesOrderGeneral_TotalDTO = new ReportDirectSalesOrderGeneral_TotalDTO
            {
                TotalDiscount = DirectSalesOrderDAOs.Where(x => x.GeneralDiscountAmount.HasValue)
                .Select(x => x.GeneralDiscountAmount.Value)
                .DefaultIfEmpty(0).Sum(),
                TotalTax = DirectSalesOrderDAOs.Select(x => x.TotalTaxAmount).DefaultIfEmpty(0).Sum(),
                SubTotal = DirectSalesOrderDAOs.Select(x => x.SubTotal).DefaultIfEmpty(0).Sum(),
                TotalAfterTax = DirectSalesOrderDAOs.Select(x => x.TotalAfterTax).DefaultIfEmpty(0).Sum(),
                PromotionValue = DirectSalesOrderDAOs.Select(x => x.PromotionValue.GetValueOrDefault(0)).DefaultIfEmpty(0).Sum(),
                Total = DirectSalesOrderDAOs.Select(x => x.Total).DefaultIfEmpty(0).Sum(),
            };

            return ReportDirectSalesOrderGeneral_TotalDTO;
        }

        [Route(ReportDirectSalesOrderGeneralRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.OrderDate.LessEqual.Value;

            ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.Skip = 0;
            ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO.Take = int.MaxValue;
            List<ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO> ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTOs = (await List(ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO)).Value;

            ReportDirectSalesOrderGeneral_TotalDTO ReportDirectSalesOrderGeneral_TotalDTO = await Total(ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO);
            long stt = 1;
            foreach (ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO in ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTOs)
            {
                foreach (var DirectSalesOrder in ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO.SalesOrders)
                {
                    DirectSalesOrder.STT = stt;
                    stt++;
                    DirectSalesOrder.eOrderDate = DirectSalesOrder.OrderDate.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
                }
            }

            string path = "Templates/Report_Direct_Sales_Order_General.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderGenerals = ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTOs;
            Data.Total = ReportDirectSalesOrderGeneral_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportDirectSalesOrderGeneral.xlsx");
        }
    }
}
