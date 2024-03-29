﻿using DMS.Common;
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
using System.IO;
using System.Dynamic;
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using DMS.Services.MProvince;
using DMS.Services.MDistrict;
using DMS.Services.MWard;
using DMS.Services.MStoreScouting;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;
using System.Diagnostics;

namespace DMS.Rpc.reports.report_store.report_statistic_store_scouting
{
    public class ReportStatisticStoreScoutingController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreScoutingService StoreScoutingService;
        private IProvinceService ProvinceService;
        private IDistrictService DistrictService;
        private IWardService WardService;
        private ICurrentContext CurrentContext;
        public ReportStatisticStoreScoutingController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreScoutingService StoreScoutingService,
            IProvinceService ProvinceService,
            IDistrictService DistrictService,
            IWardService WardService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.ProvinceService = ProvinceService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreScoutingService = StoreScoutingService;
            this.DistrictService = DistrictService;
            this.WardService = WardService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportStatisticStoreScoutingRoute.FilterListProvince), HttpPost]
        public async Task<List<ReportStatisticStoreScouting_ProvinceDTO>> FilterListProvince([FromBody] ReportStatisticStoreScouting_ProvinceFilterDTO ReportStatisticStoreScouting_ProvinceFilterDTO)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter();
            ProvinceFilter.Skip = 0;
            ProvinceFilter.Take = 20;
            ProvinceFilter.OrderBy = ProvinceOrder.Priority;
            ProvinceFilter.OrderType = OrderType.ASC;
            ProvinceFilter.Selects = ProvinceSelect.ALL;
            ProvinceFilter.Id = ReportStatisticStoreScouting_ProvinceFilterDTO.Id;
            ProvinceFilter.Name = ReportStatisticStoreScouting_ProvinceFilterDTO.Name;
            ProvinceFilter.StatusId = ReportStatisticStoreScouting_ProvinceFilterDTO.StatusId;

            List<Province> Provinces = await ProvinceService.List(ProvinceFilter);
            List<ReportStatisticStoreScouting_ProvinceDTO> ReportStatisticStoreScouting_ProvinceDTOs = Provinces
                .Select(x => new ReportStatisticStoreScouting_ProvinceDTO(x)).ToList();
            return ReportStatisticStoreScouting_ProvinceDTOs;
        }

        [Route(ReportStatisticStoreScoutingRoute.FilterListDistrict), HttpPost]
        public async Task<List<ReportStatisticStoreScouting_DistrictDTO>> FilterListDistrict([FromBody] ReportStatisticStoreScouting_DistrictFilterDTO ReportStatisticStoreScouting_DistrictFilterDTO)
        {
            DistrictFilter DistrictFilter = new DistrictFilter();
            DistrictFilter.Skip = 0;
            DistrictFilter.Take = 20;
            DistrictFilter.OrderBy = DistrictOrder.Priority;
            DistrictFilter.OrderType = OrderType.ASC;
            DistrictFilter.Selects = DistrictSelect.ALL;
            DistrictFilter.Id = ReportStatisticStoreScouting_DistrictFilterDTO.Id;
            DistrictFilter.Name = ReportStatisticStoreScouting_DistrictFilterDTO.Name;
            DistrictFilter.Priority = ReportStatisticStoreScouting_DistrictFilterDTO.Priority;
            DistrictFilter.ProvinceId = ReportStatisticStoreScouting_DistrictFilterDTO.ProvinceId;
            DistrictFilter.StatusId = ReportStatisticStoreScouting_DistrictFilterDTO.StatusId;

            List<District> Districts = await DistrictService.List(DistrictFilter);
            List<ReportStatisticStoreScouting_DistrictDTO> ReportStatisticStoreScouting_DistrictDTOs = Districts
                .Select(x => new ReportStatisticStoreScouting_DistrictDTO(x)).ToList();
            return ReportStatisticStoreScouting_DistrictDTOs;
        }

        [Route(ReportStatisticStoreScoutingRoute.FilterListWard), HttpPost]
        public async Task<List<ReportStatisticStoreScouting_WardDTO>> FilterListWard([FromBody] ReportStatisticStoreScouting_WardFilterDTO ReportStatisticStoreScouting_WardFilterDTO)
        {
            WardFilter WardFilter = new WardFilter();
            WardFilter.Skip = 0;
            WardFilter.Take = 20;
            WardFilter.OrderBy = WardOrder.Priority;
            WardFilter.OrderType = OrderType.ASC;
            WardFilter.Selects = WardSelect.ALL;
            WardFilter.Id = ReportStatisticStoreScouting_WardFilterDTO.Id;
            WardFilter.Name = ReportStatisticStoreScouting_WardFilterDTO.Name;
            WardFilter.DistrictId = ReportStatisticStoreScouting_WardFilterDTO.DistrictId;
            WardFilter.StatusId = ReportStatisticStoreScouting_WardFilterDTO.StatusId;
            List<Ward> Wards = await WardService.List(WardFilter);
            List<ReportStatisticStoreScouting_WardDTO> ReportStatisticStoreScouting_WardDTOs = Wards
                .Select(x => new ReportStatisticStoreScouting_WardDTO(x)).ToList();
            return ReportStatisticStoreScouting_WardDTOs;
        }
        #endregion

        [Route(ReportStatisticStoreScoutingRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? ProvinceId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId?.Equal;
            long? DistrictId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId?.Equal;
            long? WardId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            if(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.HasValue == false)
            {
                return await DataContext.Province
                    .Where(x => x.DeletedAt == null)
                    .CountAsync();
            }
            else
            {
                if(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.HasValue &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.HasValue == false &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue == false)
                {
                    return await DataContext.District
                        .Where(x => x.ProvinceId == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.Equal.Value)
                        .Where(x => x.DeletedAt == null)
                        .CountAsync();
                }
                else if(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.HasValue &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue == false)
                {
                    return await DataContext.Ward
                        .Where(x => x.DistrictId == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.Equal.Value)
                        .Where(x => x.DeletedAt == null)
                        .CountAsync();
                }
                else if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue)
                {
                    return await DataContext.Ward
                        .Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.Equal.Value)
                        .Where(x => x.DeletedAt == null)
                        .CountAsync();
                }
            }
            return 0;
        }

        [Route(ReportStatisticStoreScoutingRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO>>> List([FromBody] ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO> ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = await ListData(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO, Start, End);
            return ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs.OrderByDescending(x => x.StoreCoutingRate).ToList();
        }

        [Route(ReportStatisticStoreScoutingRoute.Total), HttpPost]
        public async Task<ReportStatisticStoreScouting_TotalDTO> Total([FromBody] ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportStatisticStoreScouting_TotalDTO();

            ReportStatisticStoreScouting_TotalDTO ReportStatisticStoreScouting_TotalDTO = await TotalData(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO, Start, End);
            return ReportStatisticStoreScouting_TotalDTO;
        }

        [Route(ReportStatisticStoreScoutingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Date.LessEqual.Value;

            ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Skip = 0;
            ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Take = int.MaxValue;
            List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO> ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = await ListData(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO, Start, End);

            ReportStatisticStoreScouting_TotalDTO ReportStatisticStoreScouting_TotalDTO = await TotalData(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO, Start, End);
            long stt = 1;
            foreach (ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO in ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs)
            {
                ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO.STT = stt;
                stt++;
            }
            var StoreScoutingIds = ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs.SelectMany(x => x.StoreScoutingIds).Distinct().ToList();
            Start = await DataContext.StoreScouting.Where(x => StoreScoutingIds.Contains(x.Id)).OrderBy(x => x.CreatedAt).Select(x => x.CreatedAt).FirstOrDefaultAsync();
            string path = "Templates/Report_Statistic_Store_Scouting.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportStatisticStoreScoutings = ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs;
            Data.Total = ReportStatisticStoreScouting_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportStatisticStoreScouting.xlsx");
        }

        private async Task<List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO>> ListData(
            ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO,
            DateTime Start, DateTime End)
        {
            long? ProvinceId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId?.Equal;
            long? DistrictId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId?.Equal;
            long? WardId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query = from s in DataContext.Store
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId.Value == ProvinceId.Value)) &&
                        (DistrictId.HasValue == false || (s.DistrictId.HasValue && s.DistrictId.Value == DistrictId.Value)) &&
                        (WardId.HasValue == false || (s.WardId.HasValue && s.WardId.Value == WardId.Value)) &&
                        s.DeletedAt == null
                        select new StoreDAO
                        {
                            Id = s.Id,
                            ProvinceId = s.ProvinceId,
                            DistrictId = s.DistrictId,
                            WardId = s.WardId,
                            StoreScoutingId = s.StoreScoutingId,
                        };

            var Stores = await query.ToListAsync();
            var StoreScoutings = await DataContext.StoreScouting
                .Where(x => ProvinceId.HasValue == false || (x.ProvinceId.HasValue && x.ProvinceId.Value == ProvinceId.Value))
                .Where(x => DistrictId.HasValue == false || (x.DistrictId.HasValue && x.DistrictId.Value == DistrictId.Value))
                .Where(x => WardId.HasValue == false || (x.WardId.HasValue && x.WardId.Value == WardId.Value))
                .Where(x => x.CreatedAt >= Start && x.CreatedAt <= End)
                .Select(x => new StoreScouting
                {
                    Id = x.Id,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                })
                .ToListAsync();

            List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO>
                ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = new List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO>();
            if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.HasValue == false)
            {
                List<ProvinceDAO> ProvinceDAOs = await DataContext.Province
                .Where(x => x.DeletedAt == null)
                .ToListAsync();

                ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = ProvinceDAOs
                    .Skip(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Skip)
                    .Take(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Take)
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        ProvinceId = x.Id
                    }).ToList();

                Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                {
                    item.StoreScoutingIds = StoreScoutings.Where(x => x.ProvinceId == item.ProvinceId).Select(x => x.Id).ToList();
                    item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                    item.StoreIds = Stores.Where(x => x.ProvinceId == item.ProvinceId).Select(x => x.Id).ToList();
                });
            }
            else
            {
                if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.HasValue &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.HasValue == false &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue == false)
                {
                    List<DistrictDAO> DistrictDAOs = await DataContext.District.Where(x => x.ProvinceId == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.Equal.Value)
                        .Where(x => x.DeletedAt == null)
                        .ToListAsync();

                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = DistrictDAOs
                    .Skip(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Skip)
                    .Take(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Take)
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        DistrictId = x.Id
                    }).ToList();

                    Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                    {
                        item.StoreScoutingIds = StoreScoutings.Where(x => x.DistrictId == item.DistrictId).Select(x => x.Id).ToList();
                        item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                        item.StoreIds = Stores.Where(x => x.DistrictId == item.DistrictId).Select(x => x.Id).ToList();
                    });
                }
                else if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.HasValue &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue == false)
                {
                    List<WardDAO> WardDAOs = await DataContext.Ward.Where(x => x.DistrictId == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.Equal.Value)
                    .Where(x => x.DeletedAt == null)
                    .ToListAsync();

                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = WardDAOs
                    .Skip(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Skip)
                    .Take(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.Take)
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        WardId = x.Id
                    }).ToList();

                    Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                    {
                        item.StoreScoutingIds = StoreScoutings.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                        item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                        item.StoreIds = Stores.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                    });
                }
                else if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue)
                {
                    List<WardDAO> WardDAOs = await DataContext.Ward.Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.Equal.Value)
                    .Where(x => x.DeletedAt == null)
                    .ToListAsync();

                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = WardDAOs
                    .Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.Equal.Value)
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        WardId = x.Id
                    }).ToList();

                    Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                    {
                        item.StoreScoutingIds = StoreScoutings.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                        item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                        item.StoreIds = Stores.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                    });
                }
            }

            return ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs.OrderByDescending(x => x.StoreCoutingRate).ToList();
        }

        private async Task<ReportStatisticStoreScouting_TotalDTO> TotalData(
            ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO,
            DateTime Start, DateTime End)
        {
            long? ProvinceId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId?.Equal;
            long? DistrictId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId?.Equal;
            long? WardId = ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query = from s in DataContext.Store
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (ProvinceId.HasValue == false || (s.ProvinceId.HasValue && s.ProvinceId.Value == ProvinceId.Value)) &&
                        (DistrictId.HasValue == false || (s.DistrictId.HasValue && s.DistrictId.Value == DistrictId.Value)) &&
                        (WardId.HasValue == false || (s.WardId.HasValue && s.WardId.Value == WardId.Value)) &&
                        s.DeletedAt == null
                        select new StoreDAO
                        {
                            Id = s.Id,
                            ProvinceId = s.ProvinceId,
                            DistrictId = s.DistrictId,
                            WardId = s.WardId,
                            StoreScoutingId = s.StoreScoutingId,
                        };

            var Stores = await query.ToListAsync();
            var StoreScoutings = await DataContext.StoreScouting
                .Where(x => ProvinceId.HasValue == false || (x.ProvinceId.HasValue && x.ProvinceId.Value == ProvinceId.Value))
                .Where(x => DistrictId.HasValue == false || (x.DistrictId.HasValue && x.DistrictId.Value == DistrictId.Value))
                .Where(x => WardId.HasValue == false || (x.WardId.HasValue && x.WardId.Value == WardId.Value))
                .Where(x => x.CreatedAt >= Start && x.CreatedAt <= End)
                .Select(x => new StoreScouting
                {
                    Id = x.Id,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                })
                .ToListAsync();

            List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO>
                ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = new List<ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO>();
            ReportStatisticStoreScouting_TotalDTO ReportStatisticStoreScouting_TotalDTO = new ReportStatisticStoreScouting_TotalDTO();
            if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.HasValue == false)
            {
                List<ProvinceDAO> ProvinceDAOs = await DataContext.Province
                .Where(x => x.DeletedAt == null)
                .ToListAsync();

                ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = ProvinceDAOs
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        ProvinceId = x.Id
                    }).ToList();

                Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                {
                    item.StoreScoutingIds = StoreScoutings.Where(x => x.ProvinceId == item.ProvinceId).Select(x => x.Id).ToList();
                    item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                    item.StoreIds = Stores.Where(x => x.ProvinceId == item.ProvinceId).Select(x => x.Id).ToList();
                });
                ReportStatisticStoreScouting_TotalDTO.OfficalName = "Việt Nam";
            }
            else
            {
                if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.HasValue &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.HasValue == false &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue == false)
                {
                    List<DistrictDAO> DistrictDAOs = await DataContext.District.Where(x => x.ProvinceId == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.Equal.Value)
                        .Where(x => x.DeletedAt == null)
                        .ToListAsync();

                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = DistrictDAOs
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        DistrictId = x.Id
                    }).ToList();

                    Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                    {
                        item.StoreScoutingIds = StoreScoutings.Where(x => x.DistrictId == item.DistrictId).Select(x => x.Id).ToList();
                        item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                        item.StoreIds = Stores.Where(x => x.DistrictId == item.DistrictId).Select(x => x.Id).ToList();
                    });
                    ReportStatisticStoreScouting_TotalDTO.OfficalName = await DataContext.Province
                        .Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.ProvinceId.Equal.Value)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();
                }
                else if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.HasValue &&
                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue == false)
                {
                    List<WardDAO> WardDAOs = await DataContext.Ward.Where(x => x.DistrictId == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.Equal.Value)
                    .Where(x => x.DeletedAt == null)
                    .ToListAsync();

                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = WardDAOs
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        WardId = x.Id
                    }).ToList();

                    Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                    {
                        item.StoreScoutingIds = StoreScoutings.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                        item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                        item.StoreIds = Stores.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                    });
                    ReportStatisticStoreScouting_TotalDTO.OfficalName = await DataContext.District
                        .Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.DistrictId.Equal.Value)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();
                }
                else if (ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.HasValue)
                {
                    List<WardDAO> WardDAOs = await DataContext.Ward.Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.Equal.Value)
                    .Where(x => x.DeletedAt == null)
                    .ToListAsync();

                    ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs = WardDAOs
                    .Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.Equal.Value)
                    .Select(x => new ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO
                    {
                        OfficalName = x.Name,
                        WardId = x.Id
                    }).ToList();

                    Parallel.ForEach(ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs, item =>
                    {
                        item.StoreScoutingIds = StoreScoutings.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                        item.StoreOpennedIds = Stores.Where(x => x.StoreScoutingId.HasValue && item.StoreScoutingIds.Contains(x.StoreScoutingId.Value)).Select(x => x.Id).ToList();
                        item.StoreIds = Stores.Where(x => x.WardId == item.WardId).Select(x => x.Id).ToList();
                    });
                    ReportStatisticStoreScouting_TotalDTO.OfficalName = WardDAOs
                        .Where(x => x.Id == ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.WardId.Equal.Value)
                        .Select(x => x.Name)
                        .FirstOrDefault();
                }
            }

            ReportStatisticStoreScouting_TotalDTO.StoreCounter = ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs.Select(x => x.StoreCounter).Sum();
            ReportStatisticStoreScouting_TotalDTO.StoreScoutingCounter = ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs.Select(x => x.StoreScoutingCounter).Sum();
            ReportStatisticStoreScouting_TotalDTO.StoreOpennedCounter = ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTOs.Select(x => x.StoreOpennedCounter).Sum();
            return ReportStatisticStoreScouting_TotalDTO;
        }
    }
}
