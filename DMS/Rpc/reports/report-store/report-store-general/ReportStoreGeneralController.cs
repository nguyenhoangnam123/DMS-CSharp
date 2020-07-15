using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using Hangfire.Annotations;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneralController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportStoreGeneralController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportStoreGeneralRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStoreGeneral_OrganizationDTO>> FilterListOrganization()
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
            List<ReportStoreGeneral_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportStoreGeneral_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportStoreGeneralRoute.FilterListStore), HttpPost]
        public async Task<List<ReportStoreGeneral_StoreDTO>> FilterListStore([FromBody] ReportStoreGeneral_StoreFilterDTO ReportStoreGeneral_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportStoreGeneral_StoreFilterDTO.Id;
            StoreFilter.Code = ReportStoreGeneral_StoreFilterDTO.Code;
            StoreFilter.Name = ReportStoreGeneral_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ReportStoreGeneral_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ReportStoreGeneral_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportStoreGeneral_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportStoreGeneral_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportStoreGeneral_StoreDTO> ReportStoreGeneral_StoreDTOs = Stores
                .Select(x => new ReportStoreGeneral_StoreDTO(x)).ToList();
            return ReportStoreGeneral_StoreDTOs;
        }

        [Route(ReportStoreGeneralRoute.FilterListStoreType), HttpPost]
        public async Task<List<ReportStoreGeneral_StoreTypeDTO>> FilterListStoreType([FromBody] ReportStoreGeneral_StoreTypeFilterDTO ReportStoreGeneral_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ReportStoreGeneral_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ReportStoreGeneral_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ReportStoreGeneral_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportStoreGeneral_StoreTypeDTO> ReportStoreGeneral_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportStoreGeneral_StoreTypeDTO(x)).ToList();
            return ReportStoreGeneral_StoreTypeDTOs;
        }
        #endregion

        [Route(ReportStoreGeneralRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStoreGeneral_ReportStoreGeneralFilterDTO ReportStoreGeneral_ReportStoreGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? StoreId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreTypeId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            
            var query = from sc in DataContext.StoreChecking
                        join s in DataContext.Store on sc.StoreId equals s.Id
                        join o in DataContext.Organization on s.OrganizationId equals o.Id
                        join au in DataContext.AppUser on sc.SaleEmployeeId equals au.Id
                        where sc.CheckOutAt.HasValue && sc.CheckOutAt >= Start && sc.CheckOutAt <= End &&
                        (StoreId.HasValue == false || sc.StoreId == StoreId.Value) &&
                        (StoreTypeId.HasValue == false || s.StoreTypeId == StoreTypeId.Value) &&
                        OrganizationIds.Contains(o.Id)
                        select s;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportStoreGeneralRoute.List), HttpPost]
        public async Task<List<ReportStoreGeneral_ReportStoreGeneralDTO>> List([FromBody] ReportStoreGeneral_ReportStoreGeneralFilterDTO ReportStoreGeneral_ReportStoreGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.HasValue == false)
                return new List<ReportStoreGeneral_ReportStoreGeneralDTO>();

            DateTime Start = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? StoreId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreTypeId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<StoreDAO> StoreDAOs = await DataContext.Store.Include(x => x.Organization)
                .Where(x => OrganizationIds.Contains(x.OrganizationId) &&
                (StoreId == null || x.Id == StoreId.Value) &&
                (StoreTypeId == null || x.StoreTypeId == StoreTypeId.Value))
                .Skip(ReportStoreGeneral_ReportStoreGeneralFilterDTO.Skip)
                .Take(ReportStoreGeneral_ReportStoreGeneralFilterDTO.Take)
                .ToListAsync();

            List<ReportStoreGeneral_StoreDTO> ReportStoreGeneral_StoreDTOs = StoreDAOs.Select(s => new ReportStoreGeneral_StoreDTO
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Address = s.Address,
                Phone = s.Telephone,
                OrganizationId = s.OrganizationId,
                Organization = s.Organization == null ? null : new ReportStoreGeneral_OrganizationDTO
                {
                    Id = s.Organization.Id,
                    Code = s.Organization.Code,
                    Name = s.Organization.Name
                }
            }).ToList();

            var StoreIds = ReportStoreGeneral_StoreDTOs.Select(x => x.Id).ToList();
            List<string> OrganizationNames = ReportStoreGeneral_StoreDTOs.Select(s => s.Organization.Name).Distinct().ToList();
            OrganizationNames = OrganizationNames.OrderBy(x => x).ToList();
            List<ReportStoreGeneral_ReportStoreGeneralDTO> ReportStoreGeneral_ReportStoreGeneralDTOs = OrganizationNames.Select(on => new ReportStoreGeneral_ReportStoreGeneralDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportStoreGeneral_ReportStoreGeneralDTO ReportStoreGeneral_ReportStoreGeneralDTO in ReportStoreGeneral_ReportStoreGeneralDTOs)
            {
                ReportStoreGeneral_ReportStoreGeneralDTO.Stores = ReportStoreGeneral_StoreDTOs
                    .Where(x => x.Organization.Name == ReportStoreGeneral_ReportStoreGeneralDTO.OrganizationName)
                    .Select(x => new ReportStoreGeneral_StoreDetailDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Address = x.Address,
                        Phone = x.Phone,
                    }).ToList();
            }
            ReportStoreGeneral_ReportStoreGeneralDTOs = ReportStoreGeneral_ReportStoreGeneralDTOs.Where(x => x.Stores.Any()).ToList();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End && 
                StoreIds.Contains(sc.StoreId))
                .ToListAsync();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => StoreIds.Contains(x.BuyerStoreId) &&
                x.OrderDate >= Start && x.OrderDate <= End)
                .ToListAsync();
            var IndirectSalesOrderIds = IndirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .ToListAsync();
            // khởi tạo khung dữ liệu
            foreach (ReportStoreGeneral_ReportStoreGeneralDTO ReportStoreGeneral_ReportStoreGeneralDTO in ReportStoreGeneral_ReportStoreGeneralDTOs)
            {
                foreach (var StoreCheckingDAO in StoreCheckingDAOs)
                {
                    var Store = ReportStoreGeneral_ReportStoreGeneralDTO.Stores.Where(x => x.Id == StoreCheckingDAO.StoreId).FirstOrDefault();
                    if (StoreCheckingDAO.Planned)
                        Store.StoreCheckingPlannedIds.Add(StoreCheckingDAO.Id);
                    else
                        Store.StoreCheckingUnPlannedIds.Add(StoreCheckingDAO.Id);
                }

                foreach (var Store in ReportStoreGeneral_ReportStoreGeneralDTO.Stores)
                {
                    var StoreCheckings = StoreCheckingDAOs.Where(x => x.StoreId == Store.Id).ToList();

                    //lượt viếng thăm đầu tiên
                    Store.FirstChecking = StoreCheckings.OrderBy(x => x.CheckOutAt)
                        .Select(x => x.CheckOutAt.Value.Date)
                        .FirstOrDefault();
                    //lượt viếng thăm gần nhất
                    Store.LastChecking = StoreCheckings.OrderByDescending(x => x.CheckOutAt)
                        .Select(x => x.CheckOutAt.Value.Date)
                        .FirstOrDefault();
                    //tổng thời gian viếng thăm
                    var TotalMinuteChecking = StoreCheckings.Sum(x => (x.CheckOutAt.Value.Subtract(x.CheckInAt.Value)).Minutes);
                    Store.TotalCheckingTime = $"{TotalMinuteChecking / 60} : {TotalMinuteChecking % 60}";

                    //tổng doanh thu
                    Store.TotalRevenue = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id).Sum(x => x.Total);
                    //ngày đặt hàng gần nhất
                    Store.LastOrder = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id)
                        .OrderByDescending(x => x.OrderDate)
                        .Select(x => x.OrderDate.Date)
                        .FirstOrDefault();
                }

                foreach (var IndirectSalesOrderDAO in IndirectSalesOrderDAOs)
                {
                    var Store = ReportStoreGeneral_ReportStoreGeneralDTO.Stores.Where(x => x.Id == IndirectSalesOrderDAO.BuyerStoreId).FirstOrDefault();
                    Store.IndirectSalesOrderIds.Add(IndirectSalesOrderDAO.Id);

                    var IndirectSalesOrderContents = IndirectSalesOrderContentDAOs.Where(x => x.IndirectSalesOrderId == IndirectSalesOrderDAO.Id).ToList();
                    foreach (var IndirectSalesOrderContent in IndirectSalesOrderContents)
                    {
                        Store.SKUItemIds.Add(IndirectSalesOrderContent.Id);
                    }
                }
            }
            return ReportStoreGeneral_ReportStoreGeneralDTOs;
        }
    }
}
