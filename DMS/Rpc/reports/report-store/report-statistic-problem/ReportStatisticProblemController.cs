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
using System.IO;
using System.Dynamic;
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

namespace DMS.Rpc.reports.report_store.report_statistic_problem
{
    public class ReportStatisticProblemController : RpcController
    {
        private DataContext DataContext;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportStatisticProblemController(
            DataContext DataContext,
            IItemService ItemService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.ItemService = ItemService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReportStatisticProblemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportStatisticProblem_OrganizationDTO>> FilterListOrganization()
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
            List<ReportStatisticProblem_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportStatisticProblem_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportStatisticProblemRoute.FilterListStore), HttpPost]
        public async Task<List<ReportStatisticProblem_StoreDTO>> FilterListStore([FromBody] ReportStatisticProblem_StoreFilterDTO ReportStatisticProblem_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportStatisticProblem_StoreFilterDTO.Id;
            StoreFilter.Code = ReportStatisticProblem_StoreFilterDTO.Code;
            StoreFilter.Name = ReportStatisticProblem_StoreFilterDTO.Name;
            StoreFilter.OrganizationId = ReportStatisticProblem_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportStatisticProblem_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportStatisticProblem_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportStatisticProblem_StoreDTO> ReportStatisticProblem_StoreDTOs = Stores
                .Select(x => new ReportStatisticProblem_StoreDTO(x)).ToList();
            return ReportStatisticProblem_StoreDTOs;
        }

        [Route(ReportStatisticProblemRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<ReportStatisticProblem_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] ReportStatisticProblem_StoreGroupingFilterDTO ReportStatisticProblem_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ReportStatisticProblem_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ReportStatisticProblem_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ReportStatisticProblem_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ReportStatisticProblem_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreGroupingFilter.Id == null) StoreGroupingFilter.Id = new IdFilter();
            StoreGroupingFilter.Id.In = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ReportStatisticProblem_StoreGroupingDTO> ReportStatisticProblem_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ReportStatisticProblem_StoreGroupingDTO(x)).ToList();
            return ReportStatisticProblem_StoreGroupingDTOs;
        }
        [Route(ReportStatisticProblemRoute.FilterListStoreType), HttpPost]
        public async Task<List<ReportStatisticProblem_StoreTypeDTO>> FilterListStoreType([FromBody] ReportStatisticProblem_StoreTypeFilterDTO ReportStatisticProblem_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ReportStatisticProblem_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ReportStatisticProblem_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ReportStatisticProblem_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreTypeFilter.Id == null) StoreTypeFilter.Id = new IdFilter();
            StoreTypeFilter.Id.In = await FilterStoreType(StoreTypeService, CurrentContext);
            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportStatisticProblem_StoreTypeDTO> ReportStatisticProblem_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportStatisticProblem_StoreTypeDTO(x)).ToList();
            return ReportStatisticProblem_StoreTypeDTOs;
        }

        [Route(ReportStatisticProblemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStatisticProblem_ReportStatisticProblemFilterDTO ReportStatisticProblem_ReportStatisticProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStatisticProblem_ReportStatisticProblemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? StoreId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStatisticProblem_ReportStatisticProblemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStatisticProblem_ReportStatisticProblemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from p in DataContext.Problem
                        join s in DataContext.Store on p.StoreId equals s.Id
                        where p.NoteAt >= Start && p.NoteAt <= End &&
                        (StoreIds.Contains(p.StoreId)) &&
                        (StoreTypeIds.Contains(s.StoreTypeId)) &&
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

        [Route(ReportStatisticProblemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportStatisticProblem_ReportStatisticProblemDTO>>> List([FromBody] ReportStatisticProblem_ReportStatisticProblemFilterDTO ReportStatisticProblem_ReportStatisticProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStatisticProblem_ReportStatisticProblemFilterDTO.HasValue == false)
                return new List<ReportStatisticProblem_ReportStatisticProblemDTO>();

            DateTime Start = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? StoreId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStatisticProblem_ReportStatisticProblemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStatisticProblem_ReportStatisticProblemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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
            var query = from p in DataContext.Problem
                        join s in DataContext.Store on p.StoreId equals s.Id
                        join o in DataContext.Organization on s.OrganizationId equals o.Id
                        where p.NoteAt >= Start && p.NoteAt <= End &&
                        (StoreIds.Contains(p.StoreId)) &&
                        (StoreTypeIds.Contains(s.StoreTypeId)) &&
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
                        (OrganizationIds.Contains(s.OrganizationId))
                        select new Store
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Address = s.Address,
                            Telephone = s.Telephone,
                            OrganizationId = s.OrganizationId,
                            Organization = new Organization
                            {
                                Id = o.Id,
                                Code = o.Code,
                                Name = o.Name,
                            }
                        };

            List<Store> Stores = await query.Distinct().ToListAsync();

            Stores = Stores.OrderBy(x => x.OrganizationId).ThenBy(x => x.Name)
                .Skip(ReportStatisticProblem_ReportStatisticProblemFilterDTO.Skip)
                .Take(ReportStatisticProblem_ReportStatisticProblemFilterDTO.Take)
                .ToList();
            List<string> OrganizationNames = Stores.Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportStatisticProblem_ReportStatisticProblemDTO> ReportStatisticProblem_ReportStatisticProblemDTOs = OrganizationNames.Select(on => new ReportStatisticProblem_ReportStatisticProblemDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportStatisticProblem_ReportStatisticProblemDTO ReportStatisticProblem_ReportStatisticProblemDTO in ReportStatisticProblem_ReportStatisticProblemDTOs)
            {
                ReportStatisticProblem_ReportStatisticProblemDTO.Stores = Stores
                    .Where(x => x.Organization.Name == ReportStatisticProblem_ReportStatisticProblemDTO.OrganizationName)
                    .Select(x => new ReportStatisticProblem_StoreDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Address = x.Address,
                        Phone = x.Telephone,
                        OrganizationId = x.OrganizationId,
                        StoreGroupingId = x.StoreGroupingId,
                        StoreTypeId = x.StoreTypeId,
                    })
                    .ToList();
            }

            StoreIds = Stores.Select(s => s.Id).ToList();
            List<ProblemDAO> ProblemDAOs = await DataContext.Problem
                .Where(x => StoreIds.Contains(x.StoreId) && Start <= x.NoteAt && x.NoteAt <= End)
                .ToListAsync();
            List<ProblemTypeDAO> ProblemTypeDAOs = await DataContext.ProblemType
                .Where(x => x.DeletedAt == null)
                .ToListAsync();
            // khởi tạo khung dữ liệu
            foreach (ReportStatisticProblem_ReportStatisticProblemDTO ReportStatisticProblem_ReportStatisticProblemDTO in ReportStatisticProblem_ReportStatisticProblemDTOs)
            {
                foreach (var Store in ReportStatisticProblem_ReportStatisticProblemDTO.Stores)
                {
                    var Problems = ProblemDAOs.Where(x => x.StoreId == Store.Id).ToList();
                    var ProblemTypeIds = Problems.Select(x => x.ProblemTypeId).Distinct();
                    if (Store.Contents == null)
                        Store.Contents = new List<ReportStatisticProblem_ContentDTO>();
                    
                    foreach (var ProblemTypeId in ProblemTypeIds)
                    {
                        var ProblemType = ProblemTypeDAOs.Where(x => x.Id == ProblemTypeId).FirstOrDefault();
                        if (ProblemType == null)
                            continue;

                        ReportStatisticProblem_ContentDTO ReportStatisticProblem_ContentDTO = new ReportStatisticProblem_ContentDTO();
                        ReportStatisticProblem_ContentDTO.ProblemTypeId = ProblemTypeId;
                        ReportStatisticProblem_ContentDTO.ProblemTypeName = ProblemType.Name;
                        ReportStatisticProblem_ContentDTO.WaitingCounter = Problems
                            .Where(x => x.ProblemTypeId == ProblemTypeId)
                            .Where(x => x.ProblemStatusId == ProblemStatusEnum.WAITING.Id)
                            .Count();
                        ReportStatisticProblem_ContentDTO.ProcessCounter = Problems
                            .Where(x => x.ProblemTypeId == ProblemTypeId)
                            .Where(x => x.ProblemStatusId == ProblemStatusEnum.PROCESSING.Id)
                            .Count();
                        ReportStatisticProblem_ContentDTO.CompletedCounter = Problems
                            .Where(x => x.ProblemTypeId == ProblemTypeId)
                            .Where(x => x.ProblemStatusId == ProblemStatusEnum.DONE.Id)
                            .Count();
                        Store.Contents.Add(ReportStatisticProblem_ContentDTO);
                    }
                }
            }

            return ReportStatisticProblem_ReportStatisticProblemDTOs;
        }

        [Route(ReportStatisticProblemRoute.Total), HttpPost]
        public async Task<ReportStatisticProblem_TotalDTO> Total([FromBody] ReportStatisticProblem_ReportStatisticProblemFilterDTO ReportStatisticProblem_ReportStatisticProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportStatisticProblem_ReportStatisticProblemFilterDTO.HasValue == false)
                return new ReportStatisticProblem_TotalDTO();

            DateTime Start = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.GreaterEqual == null ?
                   StaticParams.DateTimeNow :
                   ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportStatisticProblem_TotalDTO();

            long? StoreId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportStatisticProblem_ReportStatisticProblemFilterDTO.StoreGroupingId?.Equal;

            ReportStatisticProblem_TotalDTO ReportStatisticProblem_TotalDTO = new ReportStatisticProblem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStatisticProblem_ReportStatisticProblemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStatisticProblem_ReportStatisticProblemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var query = from p in DataContext.Problem
                        join s in DataContext.Store on p.StoreId equals s.Id
                        join o in DataContext.Organization on s.OrganizationId equals o.Id
                        where p.NoteAt >= Start && p.NoteAt <= End &&
                        (StoreIds.Contains(p.StoreId)) &&
                        (StoreTypeIds.Contains(s.StoreTypeId)) &&
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
                        (OrganizationIds.Contains(s.OrganizationId))
                        select new Store
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Address = s.Address,
                            OrganizationId = s.OrganizationId,
                            Organization = new Organization
                            {
                                Id = o.Id,
                                Code = o.Code,
                                Name = o.Name,
                            }
                        };

            List<Store> Stores = await query.Distinct().ToListAsync();

            Stores = Stores.ToList();
            List<string> OrganizationNames = Stores.Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportStatisticProblem_ReportStatisticProblemDTO> ReportStatisticProblem_ReportStatisticProblemDTOs = OrganizationNames.Select(on => new ReportStatisticProblem_ReportStatisticProblemDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportStatisticProblem_ReportStatisticProblemDTO ReportStatisticProblem_ReportStatisticProblemDTO in ReportStatisticProblem_ReportStatisticProblemDTOs)
            {
                ReportStatisticProblem_ReportStatisticProblemDTO.Stores = Stores
                    .Where(x => x.Organization.Name == ReportStatisticProblem_ReportStatisticProblemDTO.OrganizationName)
                    .Select(x => new ReportStatisticProblem_StoreDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Address = x.Address,
                        OrganizationId = x.OrganizationId,
                        StoreGroupingId = x.StoreGroupingId,
                        StoreTypeId = x.StoreTypeId,
                    })
                    .ToList();
            }

            StoreIds = Stores.Select(s => s.Id).ToList();
            List<ProblemDAO> ProblemDAOs = await DataContext.Problem
                .Where(x => StoreIds.Contains(x.StoreId) && Start <= x.NoteAt && x.NoteAt <= End)
                .ToListAsync();
            List<ProblemTypeDAO> ProblemTypeDAOs = await DataContext.ProblemType
                .Where(x => x.DeletedAt == null)
                .ToListAsync();

            // khởi tạo khung dữ liệu
            foreach (ReportStatisticProblem_ReportStatisticProblemDTO ReportStatisticProblem_ReportStatisticProblemDTO in ReportStatisticProblem_ReportStatisticProblemDTOs)
            {
                foreach (var Store in ReportStatisticProblem_ReportStatisticProblemDTO.Stores)
                {
                    var Problems = ProblemDAOs.Where(x => x.StoreId == Store.Id).ToList();
                    var ProblemTypeIds = Problems.Select(x => x.ProblemTypeId).Distinct();
                    if (Store.Contents == null)
                        Store.Contents = new List<ReportStatisticProblem_ContentDTO>();

                    foreach (var ProblemTypeId in ProblemTypeIds)
                    {
                        var ProblemType = ProblemTypeDAOs.Where(x => x.Id == ProblemTypeId).FirstOrDefault();
                        if (ProblemType == null)
                            continue;

                        ReportStatisticProblem_ContentDTO ReportStatisticProblem_ContentDTO = new ReportStatisticProblem_ContentDTO();
                        ReportStatisticProblem_ContentDTO.ProblemTypeId = ProblemTypeId;
                        ReportStatisticProblem_ContentDTO.ProblemTypeName = ProblemType.Name;
                        ReportStatisticProblem_ContentDTO.WaitingCounter = Problems
                            .Where(x => x.ProblemTypeId == ProblemTypeId)
                            .Where(x => x.ProblemStatusId == ProblemStatusEnum.WAITING.Id)
                            .Count();
                        ReportStatisticProblem_ContentDTO.ProcessCounter = Problems
                            .Where(x => x.ProblemTypeId == ProblemTypeId)
                            .Where(x => x.ProblemStatusId == ProblemStatusEnum.PROCESSING.Id)
                            .Count();
                        ReportStatisticProblem_ContentDTO.CompletedCounter = Problems
                            .Where(x => x.ProblemTypeId == ProblemTypeId)
                            .Where(x => x.ProblemStatusId == ProblemStatusEnum.DONE.Id)
                            .Count();
                        Store.Contents.Add(ReportStatisticProblem_ContentDTO);
                    }
                }
            }

            ReportStatisticProblem_TotalDTO.WaitingCounter = ReportStatisticProblem_ReportStatisticProblemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Contents).Sum(x => x.WaitingCounter);
            ReportStatisticProblem_TotalDTO.ProcessCounter = ReportStatisticProblem_ReportStatisticProblemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Contents).Sum(x => x.ProcessCounter);
            ReportStatisticProblem_TotalDTO.CompletedCounter = ReportStatisticProblem_ReportStatisticProblemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Contents).Sum(x => x.CompletedCounter);
            return ReportStatisticProblem_TotalDTO;
        }

        [Route(ReportStatisticProblemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportStatisticProblem_ReportStatisticProblemFilterDTO ReportStatisticProblem_ReportStatisticProblemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.GreaterEqual == null ?
               StaticParams.DateTimeNow.Date :
               ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.GreaterEqual.Value.Date;

            DateTime End = ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportStatisticProblem_ReportStatisticProblemFilterDTO.Date.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            ReportStatisticProblem_ReportStatisticProblemFilterDTO.Skip = 0;
            ReportStatisticProblem_ReportStatisticProblemFilterDTO.Take = int.MaxValue;
            List<ReportStatisticProblem_ReportStatisticProblemDTO> ReportStatisticProblem_ReportStatisticProblemDTOs = (await List(ReportStatisticProblem_ReportStatisticProblemFilterDTO)).Value;

            ReportStatisticProblem_TotalDTO ReportStatisticProblem_TotalDTO = await Total(ReportStatisticProblem_ReportStatisticProblemFilterDTO);
            long stt = 1;
            foreach (ReportStatisticProblem_ReportStatisticProblemDTO ReportStatisticProblem_ReportStatisticProblemDTO in ReportStatisticProblem_ReportStatisticProblemDTOs)
            {
                foreach (var Store in ReportStatisticProblem_ReportStatisticProblemDTO.Stores)
                {
                    Store.STT = stt;
                    stt++;
                }
            }

            string path = "Templates/Report_Statistic_Problem.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportStatisticProblems = ReportStatisticProblem_ReportStatisticProblemDTOs;
            Data.Total = ReportStatisticProblem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportStatisticProblem.xlsx");
        }
    }
}
