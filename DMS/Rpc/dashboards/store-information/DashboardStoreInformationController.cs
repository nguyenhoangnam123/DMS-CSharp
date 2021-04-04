using DMS.Common;
using DMS.Models;
using DMS.Services.MStoreChecking;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DMS.Services.MIndirectSalesOrder;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using RestSharp;
using DMS.Helpers;
using DMS.Enums;
using DMS.Services.MBrand;
using DMS.Services.MStore;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;

namespace DMS.Rpc.dashboards.store_information
{
    public class DashboardStoreInformationController : RpcController
    {
        private const long TODAY = 0;
        private const long THIS_WEEK = 1;
        private const long THIS_MONTH = 2;
        private const long LAST_MONTH = 3;

        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IBrandService BrandService;
        private ICurrentContext CurrentContext;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        public DashboardStoreInformationController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IBrandService BrandService,
            ICurrentContext CurrentContext,
            IOrganizationService OrganizationService,
            IStoreService StoreService)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.BrandService = BrandService;
            this.CurrentContext = CurrentContext;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
        }

        [Route(DashboardStoreInformationRoute.FilterListBrand), HttpPost]
        public async Task<List<DashboardStoreInformation_BrandDTO>> FilterListBrand([FromBody] DashboardStoreInformation_BrandFilterDTO DashboardStoreInformation_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Id = DashboardStoreInformation_BrandFilterDTO.Id;
            BrandFilter.Code = DashboardStoreInformation_BrandFilterDTO.Code;
            BrandFilter.Name = DashboardStoreInformation_BrandFilterDTO.Name;
            BrandFilter.Description = DashboardStoreInformation_BrandFilterDTO.Description;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            BrandFilter.UpdateTime = DashboardStoreInformation_BrandFilterDTO.UpdateTime;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<DashboardStoreInformation_BrandDTO> DashboardStoreInformation_BrandDTOs = Brands
                .Select(x => new DashboardStoreInformation_BrandDTO(x)).ToList();
            return DashboardStoreInformation_BrandDTOs;
        }

        [Route(DashboardStoreInformationRoute.FilterListOrganization), HttpPost]
        public async Task<List<DashboardStoreInformation_OrganizationDTO>> FilterListOrganization([FromBody] DashboardStoreInformation_OrganizationFilterDTO DashboardStoreInformation_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Code = DashboardStoreInformation_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = DashboardStoreInformation_OrganizationFilterDTO.Name;
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<DashboardStoreInformation_OrganizationDTO> DashboardStoreInformation_OrganizationDTOs = Organizations
                .Select(x => new DashboardStoreInformation_OrganizationDTO(x)).ToList();
            return DashboardStoreInformation_OrganizationDTOs;
        }

        [Route(DashboardStoreInformationRoute.StoreCounter), HttpPost]
        public async Task<DashboardStoreInformation_StoreCounterDTO> StoreCounter([FromBody] DashboardStoreInformation_StoreCounterFilterDTO DashboardStoreInformation_StoreCounterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? BrandId = DashboardStoreInformation_StoreCounterFilterDTO.BrandId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardStoreInformation_StoreCounterFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardStoreInformation_StoreCounterFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from s in DataContext.Store
                        join bs in DataContext.BrandInStore on s.Id equals bs.StoreId
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (BrandId.HasValue == false || bs.BrandId == BrandId) &&
                        s.StatusId == StatusEnum.ACTIVE.Id &&
                        s.DeletedAt == null &&
                        bs.DeletedAt == null
                        select s;
            var SurveyedStoreCounter = query.Select(x => x.Id).Distinct().Count();

            var queryStore = from s in DataContext.Store
                             join tt in tempTableQuery.Query on s.Id equals tt.Column1
                             where OrganizationIds.Contains(s.OrganizationId) &&
                             s.StatusId == StatusEnum.ACTIVE.Id &&
                             s.DeletedAt == null
                             select s;
            var StoreCounter = queryStore.Count();

            var queryScouting = from ss in DataContext.StoreScouting
                                where (OrganizationIds.Contains(ss.OrganizationId))
                                && ss.StoreScoutingStatusId == StoreScoutingStatusEnum.NOTOPEN.Id
                                && ss.DeletedAt == null
                                select ss;
            var StoreScoutingCounter = queryScouting.Count();

            DashboardStoreInformation_StoreCounterDTO DashboardStoreInformation_StoreCounterDTO = new DashboardStoreInformation_StoreCounterDTO();
            DashboardStoreInformation_StoreCounterDTO.SurveyedStoreCounter = SurveyedStoreCounter;
            DashboardStoreInformation_StoreCounterDTO.StoreCounter = StoreCounter + StoreScoutingCounter;
            return DashboardStoreInformation_StoreCounterDTO;
        }

        [Route(DashboardStoreInformationRoute.BrandStatistic), HttpPost]
        public async Task<List<DashboardStoreInformation_BrandStatisticsDTO>> BrandStatistic([FromBody] DashboardStoreInformation_BrandStatisticsFilterDTO DashboardStoreInformation_BrandStatisticsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? BrandId = DashboardStoreInformation_BrandStatisticsFilterDTO.BrandId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardStoreInformation_BrandStatisticsFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardStoreInformation_BrandStatisticsFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query = from s in DataContext.Store
                        join bs in DataContext.BrandInStore on s.Id equals bs.StoreId
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (BrandId.HasValue == false || bs.BrandId == BrandId) &&
                        s.StatusId == StatusEnum.ACTIVE.Id &&
                        s.DeletedAt == null &&
                        bs.DeletedAt == null
                        select s;
            var SurveyedStoreCounter = query.Select(x => x.Id).Distinct().Count();

            var queryBrand = from bs in DataContext.BrandInStore
                             join b in DataContext.Brand on bs.BrandId equals b.Id
                             join s in DataContext.Store on bs.StoreId equals s.Id
                             join tt in tempTableQuery.Query on bs.StoreId equals tt.Column1
                             where OrganizationIds.Contains(s.OrganizationId) &&
                             (BrandId.HasValue == false || bs.BrandId == BrandId) &&
                             s.StatusId == StatusEnum.ACTIVE.Id &&
                             s.DeletedAt == null &&
                             bs.DeletedAt == null
                             group bs by new { b.Id, b.Name } into x
                             select new DashboardStoreInformation_BrandStatisticsDTO
                             {
                                 BrandId = x.Key.Id,
                                 BrandName = x.Key.Name,
                                 Value = x.Count()
                             };
            List<DashboardStoreInformation_BrandStatisticsDTO> DashboardStoreInformation_BrandStatisticsDTOs = await queryBrand.ToListAsync();
            foreach (var DashboardStoreInformation_BrandStatisticsDTO in DashboardStoreInformation_BrandStatisticsDTOs)
            {
                DashboardStoreInformation_BrandStatisticsDTO.Total = SurveyedStoreCounter;
            }
            DashboardStoreInformation_BrandStatisticsDTOs = DashboardStoreInformation_BrandStatisticsDTOs.OrderByDescending(x => x.Value).ToList();
            return DashboardStoreInformation_BrandStatisticsDTOs;
        }

        [Route(DashboardStoreInformationRoute.BrandUnStatistic), HttpPost]
        public async Task<List<DashboardStoreInformation_BrandStatisticsDTO>> BrandUnStatistic([FromBody] DashboardStoreInformation_BrandStatisticsFilterDTO DashboardStoreInformation_BrandStatisticsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? BrandId = DashboardStoreInformation_BrandStatisticsFilterDTO.BrandId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardStoreInformation_BrandStatisticsFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardStoreInformation_BrandStatisticsFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
            var query = from s in DataContext.Store
                        join bs in DataContext.BrandInStore on s.Id equals bs.StoreId
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (BrandId.HasValue == false || bs.BrandId == BrandId) &&
                        s.StatusId == StatusEnum.ACTIVE.Id &&
                        s.DeletedAt == null &&
                        bs.DeletedAt == null
                        select s;
            var SurveyedStoreCounter = query.Select(x => x.Id).Distinct().Count();

            var queryBrand = from bs in DataContext.BrandInStore
                             join b in DataContext.Brand on bs.BrandId equals b.Id
                             join s in DataContext.Store on bs.StoreId equals s.Id
                             join tt in tempTableQuery.Query on bs.StoreId equals tt.Column1
                             where OrganizationIds.Contains(s.OrganizationId) &&
                             (BrandId.HasValue == false || bs.BrandId == BrandId) &&
                             s.StatusId == StatusEnum.ACTIVE.Id &&
                             s.DeletedAt == null &&
                             bs.DeletedAt == null
                             group bs by new { b.Id, b.Name } into x
                             select new DashboardStoreInformation_BrandStatisticsDTO
                             {
                                 BrandId = x.Key.Id,
                                 BrandName = x.Key.Name,
                                 Value = x.Count()
                             };
            List<DashboardStoreInformation_BrandStatisticsDTO> DashboardStoreInformation_BrandStatisticsDTOs = await queryBrand.ToListAsync();
            foreach (var DashboardStoreInformation_BrandStatisticsDTO in DashboardStoreInformation_BrandStatisticsDTOs)
            {
                DashboardStoreInformation_BrandStatisticsDTO.Value = SurveyedStoreCounter - DashboardStoreInformation_BrandStatisticsDTO.Value;
                DashboardStoreInformation_BrandStatisticsDTO.Total = SurveyedStoreCounter;
            }
            DashboardStoreInformation_BrandStatisticsDTOs = DashboardStoreInformation_BrandStatisticsDTOs.OrderByDescending(x => x.Value).ToList();
            return DashboardStoreInformation_BrandStatisticsDTOs;
        }

        [Route(DashboardStoreInformationRoute.StoreCoverage), HttpPost]
        public async Task<List<DashboardStoreInformation_StoreDTO>> StoreCoverage([FromBody] DashboardStoreInformation_StoreFilterDTO DashboardStoreInformation_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var BrandId = DashboardStoreInformation_StoreFilterDTO.BrandId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardStoreInformation_StoreFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardStoreInformation_StoreFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from s in DataContext.Store
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        s.StatusId == StatusEnum.ACTIVE.Id &&
                        s.DeletedAt == null
                        select new DashboardStoreInformation_StoreDTO
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Address = s.Address,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Telephone = s.OwnerPhone,
                            StoreStatusId = s.StoreStatusId,
                            IsScouting = false
                        };
            List<DashboardStoreInformation_StoreDTO> DashboardMonitor_StoreDTOs = await query.Distinct().ToListAsync();
            StoreIds = DashboardMonitor_StoreDTOs.Select(x => x.Id).ToList();

            var queryBrandInStore = from bs in DataContext.BrandInStore
                                    join b in DataContext.Brand on bs.BrandId equals b.Id
                                    join s in DataContext.Store on bs.StoreId equals s.Id
                                    join tt in tempTableQuery.Query on s.Id equals tt.Column1
                                    where OrganizationIds.Contains(s.OrganizationId) &&
                                    (BrandId.HasValue == false || (bs.BrandId == BrandId.Value))
                                    && s.StatusId == StatusEnum.ACTIVE.Id
                                    && s.DeletedAt == null &&
                                    bs.DeletedAt == null &&
                                    bs.Top == 1
                                    select new BrandInStoreDAO
                                    {
                                        Id = bs.Id,
                                        StoreId = bs.StoreId,
                                        BrandId = bs.BrandId,
                                        Brand = new BrandDAO
                                        {
                                            Name = b.Name
                                        }
                                    };

            var BrandInStores = await queryBrandInStore.ToListAsync();
            foreach (var DashboardMonitor_StoreDTO in DashboardMonitor_StoreDTOs)
            {
                DashboardMonitor_StoreDTO.Top1BrandName = BrandInStores.Where(x => x.StoreId == DashboardMonitor_StoreDTO.Id).Select(x => x.Brand.Name).FirstOrDefault();
            }

            var query_Scouting = from ss in DataContext.StoreScouting
                                 join au in DataContext.AppUser on ss.CreatorId equals au.Id
                                 where (OrganizationIds.Contains(au.OrganizationId)) &&
                                 ss.StoreScoutingStatusId == StoreScoutingStatusEnum.NOTOPEN.Id &&
                                 ss.DeletedAt == null
                                 select new DashboardStoreInformation_StoreDTO
                                 {
                                     Id = ss.Id,
                                     Name = ss.Name,
                                     Address = ss.Address,
                                     Latitude = ss.Latitude,
                                     Longitude = ss.Longitude,
                                     Telephone = ss.OwnerPhone,
                                     IsScouting = true
                                 };
            List<DashboardStoreInformation_StoreDTO> DashboardMonitor_StoreScotingDTOs = await query_Scouting.Distinct().ToListAsync();
            DashboardMonitor_StoreDTOs.AddRange(DashboardMonitor_StoreScotingDTOs);
            return DashboardMonitor_StoreDTOs;
        }

        [Route(DashboardStoreInformationRoute.ProductGroupingStatistic), HttpPost]
        public async Task<List<DashboardStoreInformation_ProductGroupingStatisticsDTO>> ProductGroupingStatistic([FromBody] DashboardStoreInformation_ProductGroupingStatisticsFilterDTO DashboardStoreInformation_ProductGroupingStatisticsFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var BrandId = DashboardStoreInformation_ProductGroupingStatisticsFilterDTO.BrandId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardStoreInformation_ProductGroupingStatisticsFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardStoreInformation_ProductGroupingStatisticsFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from bs in DataContext.BrandInStore
                        join b in DataContext.Brand on bs.BrandId equals b.Id
                        join s in DataContext.Store on bs.StoreId equals s.Id
                        join tt in tempTableQuery.Query on s.Id equals tt.Column1
                        where OrganizationIds.Contains(s.OrganizationId) &&
                        (BrandId.HasValue == false || (bs.BrandId == BrandId.Value))
                        && s.StatusId == StatusEnum.ACTIVE.Id
                        && s.DeletedAt == null &&
                        bs.DeletedAt == null
                        group bs by new { b.Id, b.Name } into x
                        select new DashboardStoreInformation_ProductGroupingStatisticsDTO
                        {
                            BrandId = x.Key.Id,
                            BrandName = x.Key.Name,
                            Total = x.Count()
                        };

            var query2 = from bs in DataContext.BrandInStore
                         join b in DataContext.Brand on bs.BrandId equals b.Id
                         join s in DataContext.Store on bs.StoreId equals s.Id
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         where OrganizationIds.Contains(s.OrganizationId) &&
                         (BrandId.HasValue == false || (bs.BrandId == BrandId.Value))
                         && s.StatusId == StatusEnum.ACTIVE.Id
                         && s.DeletedAt == null &&
                         bs.DeletedAt == null
                         select bs.Id;

            var BrandInStoreIds = await query2.Distinct().ToListAsync();
            var BrandInStoreProductGroupingMappings = await DataContext.BrandInStoreProductGroupingMapping.Include(x => x.BrandInStore)
                .Where(x => BrandInStoreIds.Contains(x.BrandInStoreId))
                .ToListAsync();

            List<DashboardStoreInformation_ProductGroupingStatisticsDTO> DashboardStoreInformation_ProductGroupingStatisticsDTOs = await query.ToListAsync();
            foreach (var DashboardStoreInformation_ProductGroupingStatisticsDTO in DashboardStoreInformation_ProductGroupingStatisticsDTOs)
            {
                var subBrandInStoreProductGroupingMappings = BrandInStoreProductGroupingMappings
                    .Where(x => x.BrandInStore.BrandId == DashboardStoreInformation_ProductGroupingStatisticsDTO.BrandId)
                    .ToList();

                var aggregate = subBrandInStoreProductGroupingMappings
                    .GroupBy(x => x.BrandInStore.StoreId)
                    .Select(x => new { StoreId = x.Key, ProductGroupingCounter = x.Count() })
                    .ToList();
                DashboardStoreInformation_ProductGroupingStatisticsDTO.Value3 = aggregate.Where(x => x.ProductGroupingCounter >= 3).Count();
                DashboardStoreInformation_ProductGroupingStatisticsDTO.Value4 = aggregate.Where(x => x.ProductGroupingCounter >= 4).Count();
                DashboardStoreInformation_ProductGroupingStatisticsDTO.Value5 = aggregate.Where(x => x.ProductGroupingCounter >= 5).Count();
                DashboardStoreInformation_ProductGroupingStatisticsDTO.Value6 = aggregate.Where(x => x.ProductGroupingCounter >= 6).Count();
                DashboardStoreInformation_ProductGroupingStatisticsDTO.Value7 = aggregate.Where(x => x.ProductGroupingCounter >= 7).Count();
                DashboardStoreInformation_ProductGroupingStatisticsDTO.Value8 = aggregate.Where(x => x.ProductGroupingCounter >= 8).Count();
            }
            DashboardStoreInformation_ProductGroupingStatisticsDTOs
                 = DashboardStoreInformation_ProductGroupingStatisticsDTOs.OrderByDescending(x => x.Total).ToList();
            return DashboardStoreInformation_ProductGroupingStatisticsDTOs;
        }

        [Route(DashboardStoreInformationRoute.TopBrand), HttpPost]
        public async Task<List<DashboardStoreInformation_TopBrandDTO>> TopBrand([FromBody] DashboardStoreInformation_TopBrandFilterDTO DashboardStoreInformation_TopBrandFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var BrandId = DashboardStoreInformation_TopBrandFilterDTO.BrandId?.Equal;
            var Top = DashboardStoreInformation_TopBrandFilterDTO.Top?.Equal ?? 1; // mặc định ở tab Hạng 1
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (DashboardStoreInformation_TopBrandFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == DashboardStoreInformation_TopBrandFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from bs in DataContext.BrandInStore
                         join b in DataContext.Brand on bs.BrandId equals b.Id
                         join s in DataContext.Store on bs.StoreId equals s.Id
                         join tt in tempTableQuery.Query on s.Id equals tt.Column1
                         where OrganizationIds.Contains(s.OrganizationId) &&
                         (BrandId.HasValue == false || (bs.BrandId == BrandId.Value)) &&
                         bs.Top == Top &&
                         s.StatusId == StatusEnum.ACTIVE.Id &&
                         s.DeletedAt == null &&
                         bs.DeletedAt == null
                         select new BrandInStore
                         {
                             Id = bs.Id,
                             BrandId = b.Id,
                             StoreId = s.Id,
                             Brand = new Brand
                             {
                                 Code = b.Code,
                                 Name = b.Name,
                             }
                         };

            var BrandInStores = await query.ToListAsync();
            List<DashboardStoreInformation_TopBrandDTO> DashboardStoreInformation_TopBrandDTOs = BrandInStores
                .GroupBy(x => new { x.BrandId, x.Brand.Name })
                .Select(x => new DashboardStoreInformation_TopBrandDTO
                {
                    BrandId = x.Key.BrandId,
                    BrandName = x.Key.Name,
                    Value = x.Count()
                }).ToList();
            foreach (var DashboardStoreInformation_TopBrandDTO in DashboardStoreInformation_TopBrandDTOs)
            {
                DashboardStoreInformation_TopBrandDTO.Total = BrandInStores.Count();
            }
            DashboardStoreInformation_TopBrandDTOs
                 = DashboardStoreInformation_TopBrandDTOs.OrderByDescending(x => x.Value).ToList();
            return DashboardStoreInformation_TopBrandDTOs;
        }
    }
}
