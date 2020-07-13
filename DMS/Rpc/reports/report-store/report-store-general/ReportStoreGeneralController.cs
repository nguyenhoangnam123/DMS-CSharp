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

        [Route(ReportStoreGeneralRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStoreGeneral_OrganizationDTO>> FilterListOrganization([FromBody] ReportStoreGeneral_OrganizationFilterDTO ReportStoreGeneral_OrganizationFilterDTO)
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
            OrganizationFilter.Id = ReportStoreGeneral_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ReportStoreGeneral_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ReportStoreGeneral_OrganizationFilterDTO.Name;

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
            StoreFilter.StatusId = ReportStoreGeneral_StoreFilterDTO.StatusId;

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
            StoreTypeFilter.StatusId = ReportStoreGeneral_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportStoreGeneral_StoreTypeDTO> ReportStoreGeneral_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportStoreGeneral_StoreTypeDTO(x)).ToList();
            return ReportStoreGeneral_StoreTypeDTOs;
        }

        [Route(ReportStoreGeneralRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStoreGeneral_ReportStoreGeneralFilterDTO ReportStoreGeneral_ReportStoreGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

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
            List<ReportStoreGeneral_ReportStoreGeneralDTO> ReportStoreGeneral_ReportStoreGeneralDTOs = OrganizationNames.Select(on => new ReportStoreGeneral_ReportStoreGeneralDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportStoreGeneral_ReportStoreGeneralDTO ReportStoreGeneral_ReportStoreGeneralDTO in ReportStoreGeneral_ReportStoreGeneralDTOs)
            {
                ReportStoreGeneral_ReportStoreGeneralDTO.Stores = ReportStoreGeneral_StoreDTOs
                    .Where(se => se.Organization.Name == ReportStoreGeneral_ReportStoreGeneralDTO.OrganizationName)
                    .ToList();
            }
            ReportStoreGeneral_ReportStoreGeneralDTOs = ReportStoreGeneral_ReportStoreGeneralDTOs.Where(x => x.Stores.Any()).ToList();
            List<StoreCheckingDAO> StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(sc => sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End && StoreIds.Contains(sc.StoreId))
                .ToListAsync();
            // khởi tạo khung dữ liệu
            //foreach (ReportStoreGeneral_SaleEmployeeDTO ReportStoreGeneral_SaleEmployeeDTO in ReportStoreGeneral_SaleEmployeeDTOs)
            //{
            //    ReportStoreGeneral_SaleEmployeeDTO.StoreCheckingGroupByDates = new List<ReportStoreGeneral_StoreCheckingGroupByDateDTO>();
            //    for (DateTime i = Start; i < End; i = i.AddDays(1))
            //    {
            //        ReportStoreGeneral_StoreCheckingGroupByDateDTO ReportStoreGeneral_StoreCheckingGroupByDateDTO = new ReportStoreGeneral_StoreCheckingGroupByDateDTO();
            //        ReportStoreGeneral_StoreCheckingGroupByDateDTO.Date = i;
            //        ReportStoreGeneral_StoreCheckingGroupByDateDTO.StoreCheckings = StoreCheckingDAOs.Where(x => x.SaleEmployeeId == ReportStoreGeneral_SaleEmployeeDTO.SaleEmployeeId)
            //            .Where(x => x.CheckOutAt.Value.Date == i)
            //            .Select(x => new ReportStoreGeneral_StoreCheckingDTO
            //            {
            //                CheckIn = x.CheckInAt.Value,
            //                CheckOut = x.CheckOutAt.Value,
            //                Duaration = x.CheckOutAt.Value.AddSeconds(x.CheckInAt.Value.Second * -1),
            //                DeviceName = x.DeviceName,
            //                ImageCounter = x.ImageCounter ?? 0,
            //                Planned = x.Planned,
            //                SalesOrder = x.IndirectSalesOrderCounter > 0 ? true : false,
            //                SaleEmployeeId = x.SaleEmployeeId,
            //                StoreName = x.Store.Name,
            //                StoreCode = x.Store.Code,
            //                StoreAddress = x.Store.Address
            //            }).ToList();
            //        ReportStoreGeneral_SaleEmployeeDTO.StoreCheckingGroupByDates.Add(ReportStoreGeneral_StoreCheckingGroupByDateDTO);
            //    }
            //}
            return ReportStoreGeneral_ReportStoreGeneralDTOs;
        }
    }
}
