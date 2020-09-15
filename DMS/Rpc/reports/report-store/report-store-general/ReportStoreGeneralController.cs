using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

        [Route(ReportStoreGeneralRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<ReportStoreGeneral_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] ReportStoreGeneral_StoreGroupingFilterDTO ReportStoreGeneral_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ReportStoreGeneral_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ReportStoreGeneral_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ReportStoreGeneral_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ReportStoreGeneral_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreGroupingFilter.Id == null) StoreGroupingFilter.Id = new IdFilter();
            StoreGroupingFilter.Id.In = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ReportStoreGeneral_StoreGroupingDTO> ReportStoreGeneral_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ReportStoreGeneral_StoreGroupingDTO(x)).ToList();
            return ReportStoreGeneral_StoreGroupingDTOs;
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

            if (StoreTypeFilter.Id == null) StoreTypeFilter.Id = new IdFilter();
            StoreTypeFilter.Id.In = await FilterStoreType(StoreTypeService, CurrentContext);
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
                    LocalStartDay(CurrentContext) :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? StoreId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }
            var query = from s in DataContext.Store
                        where StoreIds.Contains(s.Id) &&
                        StoreTypeIds.Contains(s.StoreTypeId) &&
                        (
                            (
                                StoreGroupingId.HasValue == false && 
                                (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue && 
                                StoreGroupingId.Value == s.StoreGroupingId.Value
                            )
                        ) &&
                        OrganizationIds.Contains(s.OrganizationId)
                        select s;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportStoreGeneralRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportStoreGeneral_ReportStoreGeneralDTO>>> List([FromBody] ReportStoreGeneral_ReportStoreGeneralFilterDTO ReportStoreGeneral_ReportStoreGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.HasValue == false)
                return new List<ReportStoreGeneral_ReportStoreGeneralDTO>();

            DateTime Start = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? StoreId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }

            var query = from s in DataContext.Store
                        where StoreIds.Contains(s.Id) &&
                        StoreTypeIds.Contains(s.StoreTypeId) &&
                        (
                            (
                                StoreGroupingId.HasValue == false &&
                                (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue &&
                                StoreGroupingId.Value == s.StoreGroupingId.Value
                            )
                        ) &&
                        OrganizationIds.Contains(s.OrganizationId)
                        select s;

            StoreIds = await query.Select(x => x.Id).Distinct().ToListAsync();

            List<StoreDAO> StoreDAOs = await DataContext.Store.Include(x => x.Organization)
                .Where(x => OrganizationIds.Contains(x.OrganizationId) &&
                StoreIds.Contains(x.Id))
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.Name)
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

            StoreIds = ReportStoreGeneral_StoreDTOs.Select(x => x.Id).ToList();
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
                .Include(x => x.SaleEmployee)
                .Where(sc => sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End &&
                StoreIds.Contains(sc.StoreId) && 
                AppUserIds.Contains(sc.SaleEmployeeId))
                .ToListAsync();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => StoreIds.Contains(x.BuyerStoreId) &&
                x.OrderDate >= Start && x.OrderDate <= End &&
                AppUserIds.Contains(x.SaleEmployeeId))
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
                    if (Store == null)
                        continue;
                    if (StoreCheckingDAO.Planned)
                    {
                        if (Store.StoreCheckingPlannedIds == null)
                            Store.StoreCheckingPlannedIds = new HashSet<long>();
                        Store.StoreCheckingPlannedIds.Add(StoreCheckingDAO.Id);
                    }
                    else
                    {
                        if (Store.StoreCheckingUnPlannedIds == null)
                            Store.StoreCheckingUnPlannedIds = new HashSet<long>();
                        Store.StoreCheckingUnPlannedIds.Add(StoreCheckingDAO.Id);
                    }
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
                    Store.EmployeeLastChecking = StoreCheckings.OrderByDescending(x => x.CheckOutAt)
                        .Select(x => x.SaleEmployee.DisplayName)
                        .FirstOrDefault();
                    //tổng thời gian viếng thăm
                    var TotalMinuteChecking = StoreCheckings.Sum(x => (x.CheckOutAt.Value.Subtract(x.CheckInAt.Value)).TotalSeconds);
                    TimeSpan timeSpan = TimeSpan.FromSeconds(TotalMinuteChecking);
                    Store.TotalCheckingTime = $"{timeSpan.Hours.ToString().PadLeft(2, '0')} : {timeSpan.Minutes.ToString().PadLeft(2, '0')} : {timeSpan.Seconds.ToString().PadLeft(2, '0')}";

                    //tổng doanh thu
                    Store.TotalRevenue = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id).Sum(x => x.Total);
                    //ngày đặt hàng gần nhất
                    Store.LastOrder = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id)
                        .OrderByDescending(x => x.OrderDate)
                        .Select(x => x.OrderDate.AddHours(CurrentContext.TimeZone).Date)
                        .FirstOrDefault();
                }

                foreach (var IndirectSalesOrderDAO in IndirectSalesOrderDAOs)
                {
                    var Store = ReportStoreGeneral_ReportStoreGeneralDTO.Stores.Where(x => x.Id == IndirectSalesOrderDAO.BuyerStoreId).FirstOrDefault();
                    if (Store == null)
                        continue;
                    if (Store.IndirectSalesOrderIds == null)
                        Store.IndirectSalesOrderIds = new HashSet<long>();
                    Store.IndirectSalesOrderIds.Add(IndirectSalesOrderDAO.Id);

                    var IndirectSalesOrderContents = IndirectSalesOrderContentDAOs.Where(x => x.IndirectSalesOrderId == IndirectSalesOrderDAO.Id).ToList();
                    foreach (var IndirectSalesOrderContent in IndirectSalesOrderContents)
                    {
                        if (Store.SKUItemIds == null)
                            Store.SKUItemIds = new HashSet<long>();
                        Store.SKUItemIds.Add(IndirectSalesOrderContent.ItemId);
                    }
                }
            }
            return ReportStoreGeneral_ReportStoreGeneralDTOs;
        }

        [Route(ReportStoreGeneralRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportStoreGeneral_ReportStoreGeneralFilterDTO ReportStoreGeneral_ReportStoreGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.GreaterEqual.Value;

            DateTime End = ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStoreGeneral_ReportStoreGeneralFilterDTO.CheckIn.LessEqual.Value;

            ReportStoreGeneral_ReportStoreGeneralFilterDTO.Skip = 0;
            ReportStoreGeneral_ReportStoreGeneralFilterDTO.Take = int.MaxValue;
            List<ReportStoreGeneral_ReportStoreGeneralDTO> ReportStoreGeneral_ReportStoreGeneralDTOs = (await List(ReportStoreGeneral_ReportStoreGeneralFilterDTO)).Value;

            long stt = 1;
            foreach (ReportStoreGeneral_ReportStoreGeneralDTO ReportStoreGeneral_ReportStoreGeneralDTO in ReportStoreGeneral_ReportStoreGeneralDTOs)
            {
                foreach (var Store in ReportStoreGeneral_ReportStoreGeneralDTO.Stores)
                {
                    Store.STT = stt;
                    stt++;
                }
            }

            string path = "Templates/Report_Store_General.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportStoreGenerals = ReportStoreGeneral_ReportStoreGeneralDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportStoreGeneral.xlsx");
        }
    }
}
