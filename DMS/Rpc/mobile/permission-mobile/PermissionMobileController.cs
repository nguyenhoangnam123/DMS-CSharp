using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public partial class PermissionMobileController : RpcController
    {
        private const long TODAY = 1;
        private const long THIS_WEEK = 2;
        private const long THIS_MONTH = 3;
        private const long LAST_MONTH = 4;
        private const long THIS_QUARTER = 5;
        private const long LAST_QUATER = 6;
        private const long YEAR = 7;

        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IItemService ItemService;
        private IProductService ProductService;

        private ICurrentContext CurrentContext;
        private DataContext DataContext;

        public PermissionMobileController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IProductService ProductService,
            ICurrentContext CurrentContext,
            DataContext DataContext)
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.ItemService = ItemService;
            this.ProductService = ProductService;
            this.CurrentContext = CurrentContext;
            this.DataContext = DataContext;
        }

        #region Dashboard KPI
        [Route(PermissionMobileRoute.ListCurrentKpiGeneral), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiGeneralReportDTO>> ListCurrentKpiGeneral([FromBody] PermissionMobile_EmployeeKpiGeneralReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiGenerals = new List<PermissionMobile_EmployeeKpiGeneralReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            Start = Start.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }
            // lấy ra số KpiGeneral theo kế hoạch
            // lấy ra KpiGeneral bằng filter theo AppUserId, trạng thái = true, kpiYearId
            List<long> KpiGeneralIds = await DataContext.KpiGeneral.Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.DeletedAt == null
                ).Select(p => p.Id).ToListAsync();
            if (KpiGeneralIds.Count > 0)
            {
                List<KpiGeneralContentDAO> KpiGeneralContentDAOs = await DataContext.KpiGeneralContent
                    .Where(x => KpiGeneralIds.Contains(x.KpiGeneralId) &&
                        x.StatusId == StatusEnum.ACTIVE.Id)
                    .ToListAsync();
                List<long> KpiGeneralContentIds = KpiGeneralContentDAOs.Select(x => x.Id).ToList();
                // lấy ra toàn bộ KpiGeneralContentKpiPeriodMappings bằng filter theo KpiGeneralContent và theo kì
                List<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappingDAOs = await DataContext.KpiGeneralContentKpiPeriodMapping
                    .Where(x => KpiGeneralContentIds.Contains(x.KpiGeneralContentId) &&
                        x.KpiPeriodId == KpiPeriodId)
                    .Select(x => new KpiGeneralContentKpiPeriodMappingDAO
                    {
                        KpiPeriodId = x.KpiPeriodId,
                        KpiGeneralContentId = x.KpiGeneralContentId,
                        Value = x.Value,
                        KpiGeneralContent = x.KpiGeneralContent == null ? null : new KpiGeneralContentDAO
                        {
                            KpiCriteriaGeneralId = x.KpiGeneralContent.KpiCriteriaGeneralId
                        }
                    }).ToListAsync();
                // lấy ra toàn bộ storeChecking để tính số liệu thực hiện bằng filter SaleEmployeeId

                var query_store_checking = from sc in DataContext.StoreChecking
                                           join s in DataContext.Store on sc.StoreId equals s.Id
                                           where AppUserIds.Contains(sc.SaleEmployeeId) &&
                                           (sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End) &&
                                           s.DeletedAt == null
                                           select sc;

                var StoreCheckingDAOs = await query_store_checking
                    .Select(x => new StoreCheckingDAO
                    {
                        SaleEmployeeId = x.SaleEmployeeId,
                        Id = x.Id,
                        CheckInAt = x.CheckInAt,
                        CheckOutAt = x.CheckOutAt,
                        StoreId = x.StoreId
                    })
                    .ToListAsync();

                List<decimal> IndirectSalesOrderTotals = await DataContext.IndirectSalesOrder
                    .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
                        x.RequestStateId == RequestStateEnum.APPROVED.Id &&
                        x.CreatedAt >= Start &&
                        x.CreatedAt <= End)
                    .Select(x => x.Total).ToListAsync();

                var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
                x.OrderDate >= Start && x.OrderDate <= End &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    Total = x.Total,
                    SaleEmployeeId = x.SaleEmployeeId,
                    OrderDate = x.OrderDate,
                    BuyerStoreId = x.BuyerStoreId,
                    BuyerStore = x.BuyerStore == null ? null : new StoreDAO
                    {
                        StoreType = x.BuyerStore.StoreType == null ? null : new StoreTypeDAO
                        {
                            Code = x.BuyerStore.StoreType.Code
                        }
                    },
                    IndirectSalesOrderContents = x.IndirectSalesOrderContents.Select(c => new IndirectSalesOrderContentDAO
                    {
                        RequestedQuantity = c.RequestedQuantity,
                        ItemId = c.ItemId
                    }).ToList(),
                    IndirectSalesOrderPromotions = x.IndirectSalesOrderPromotions.Select(x => new IndirectSalesOrderPromotionDAO
                    {
                        RequestedQuantity = x.RequestedQuantity,
                        ItemId = x.ItemId
                    }).ToList()
                })
                .ToListAsync();
                var IndirectSalesOrderContents = IndirectSalesOrderDAOs
                            .SelectMany(x => x.IndirectSalesOrderContents)
                            .ToList();
                var IndirectSalesOrderPromotions = IndirectSalesOrderDAOs
                    .SelectMany(x => x.IndirectSalesOrderPromotions)
                    .ToList();

                var StoreDAOs = await DataContext.Store
                    .Where(x => AppUserIds.Contains(x.CreatorId) &&
                    x.CreatedAt >= Start && x.CreatedAt <= End)
                    .Select(x => new StoreDAO
                    {
                        CreatorId = x.CreatorId,
                        Id = x.Id,
                        CreatedAt = x.CreatedAt,
                        StoreType = x.StoreType == null ? null : new StoreTypeDAO
                        {
                            Code = x.StoreType.Code
                        }
                    })
                    .ToListAsync();

                var Problems = await DataContext.Problem
                 .Where(x => AppUserIds.Contains(x.CreatorId) &&
                 x.NoteAt >= Start && x.NoteAt <= End)
                 .ToListAsync();

                var StoreImages = await DataContext.StoreImage
                    .Where(x => x.SaleEmployeeId.HasValue &&
                    AppUserIds.Contains(x.SaleEmployeeId.Value) &&
                    x.ShootingAt >= Start && x.ShootingAt <= End)
                    .ToListAsync();

                List<GenericEnum> KpiCriteriaGenerals = KpiCriteriaGeneralEnum.KpiCriteriaGeneralEnumList;
                foreach (var KpiCriteriaGeneral in KpiCriteriaGenerals)
                {
                    List<KpiGeneralContentKpiPeriodMappingDAO>
                        KpiGeneralContentKpiPeriodMappings = KpiGeneralContentKpiPeriodMappingDAOs
                        .Where(x => x.KpiGeneralContent.KpiCriteriaGeneralId == KpiCriteriaGeneral.Id)
                        .ToList();

                    PermissionMobile_EmployeeKpiGeneralReportDTO PermissionMobile_EmployeeKpiGeneralReportDTO = new PermissionMobile_EmployeeKpiGeneralReportDTO();
                    PermissionMobile_EmployeeKpiGeneralReportDTO.KpiCriteriaGeneralName = KpiCriteriaGeneral.Name;
                    PermissionMobile_EmployeeKpiGeneralReportDTO.PlannedValue = KpiGeneralContentKpiPeriodMappings
                        .Where(x => x.Value.HasValue)
                        .Select(x => x.Value.Value)
                        .DefaultIfEmpty(0)
                        .Sum();

                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = IndirectSalesOrderDAOs.Sum(iso => iso.Total);
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.REVENUE_C2_TD.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = IndirectSalesOrderDAOs
                        .Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2TD)
                        .Sum(iso => iso.Total);
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.REVENUE_C2_SL.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = IndirectSalesOrderDAOs
                        .Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2SL)
                        .Sum(iso => iso.Total);
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.REVENUE_C2.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = IndirectSalesOrderDAOs
                        .Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2)
                        .Sum(iso => iso.Total);
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreDAOs.Count();
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.NEW_STORE_C2_CREATED.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreDAOs
                        .Where(x => x.StoreType.Code == StaticParams.C2TD)
                        .Count();
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreCheckingDAOs
                            .Select(x => x.StoreId)
                            .Distinct()
                            .Count();
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreCheckingDAOs.Count();
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.TOTAL_PROBLEM.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = Problems.Count();
                    }
                    if (KpiCriteriaGeneral.Id == KpiCriteriaGeneralEnum.TOTAL_IMAGE.Id)
                    {
                        PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue = StoreImages.Count();
                    }
                    PermissionMobile_EmployeeKpiGeneralReportDTO.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiGeneralReportDTO.PlannedValue, PermissionMobile_EmployeeKpiGeneralReportDTO.CurrentValue); // tính ra phần trăm thực hiện
                    KpiGenerals.Add(PermissionMobile_EmployeeKpiGeneralReportDTO);
                }
            }
            return KpiGenerals;
        }

        [Route(PermissionMobileRoute.ListCurrentKpiItem), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiItemReportDTO>> ListCurrentKpiItem([FromBody] PermissionMobile_EmployeeKpiItemReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiItemDTOs = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            Start = Start.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }

            List<KpiItemDAO> KpiItems = await DataContext.KpiItem.Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.KpiItemTypeId == KpiItemTypeEnum.ALL_PRODUCT.Id &&
                    x.DeletedAt == null)
                .Select(x => new KpiItemDAO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                }).ToListAsync();
            var KpiItemIds = KpiItems.Select(x => x.Id).ToList();
            if (KpiItems.Count > 0)
            {
                List<KpiItemContentDAO> KpiItemContentDAOs = await DataContext.KpiItemContent
                    .Where(x => KpiItemIds.Contains(x.KpiItemId))
                    .Select(x => new KpiItemContentDAO
                    {
                        Id = x.Id,
                        KpiItemId = x.KpiItemId,
                        ItemId = x.ItemId,
                        KpiItem = x.KpiItem == null ? null : new KpiItemDAO
                        {
                            EmployeeId = x.KpiItem.EmployeeId
                        }
                    })
                    .ToListAsync();
                List<long> KpiItemContentIds = KpiItemContentDAOs.Select(x => x.Id).ToList();
                if (KpiItemContentIds.Count > 0)
                {
                    List<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMappingDAOs = await DataContext.KpiItemContentKpiCriteriaItemMapping
                        .Where(x => KpiItemContentIds.Contains(x.KpiItemContentId))
                        .Select(x => new KpiItemContentKpiCriteriaItemMappingDAO
                        {
                            KpiCriteriaItemId = x.KpiCriteriaItemId,
                            KpiItemContentId = x.KpiItemContentId,
                            Value = x.Value,
                            KpiItemContent = x.KpiItemContent == null ? null : new KpiItemContentDAO
                            {
                                ItemId = x.KpiItemContent.ItemId
                            }
                        })
                        .ToListAsync();
                    List<PermissionMobile_EmployeeKpiItem> PermissionMobile_EmployeeKpiItems = new List<PermissionMobile_EmployeeKpiItem>();
                    if (KpiItemContentKpiCriteriaItemMappingDAOs.Count > 0)
                    {
                        List<long> ItemIds = KpiItemContentDAOs.Select(x => x.ItemId).Distinct().ToList(); // lẩy ra list itemId theo chỉ tiêu
                        List<ItemDAO> ItemDAOs = await DataContext.Item.Where(x => ItemIds.Contains(x.Id)).Select(x => new ItemDAO
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                        }).ToListAsync();

                        var query = from t in DataContext.IndirectSalesOrderTransaction
                                    join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                                    join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                                    join it in DataContext.Item on t.ItemId equals it.Id
                                    where Start <= t.OrderDate && t.OrderDate <= End &&
                                    AppUserIds.Contains(t.SalesEmployeeId) &&
                                    ItemIds.Contains(t.ItemId) &&
                                    au.DeletedAt == null && it.DeletedAt == null
                                    select new IndirectSalesOrderTransactionDAO
                                    {
                                        ItemId = t.ItemId,
                                        Revenue = t.Revenue,
                                        BuyerStoreId = t.BuyerStoreId,
                                        SalesEmployeeId = t.SalesEmployeeId,
                                    };

                        List<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                        List<GenericEnum> KpiCriteriaItems = KpiCriteriaItemEnum.KpiCriteriaItemEnumList;
                        var subResults = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
                        foreach (var KpiItem in KpiItems)
                        {
                            var subContents = KpiItemContentDAOs.Where(x => x.KpiItemId == KpiItem.Id).ToList();
                            var subContentIds = subContents.Select(x => x.Id).ToList();
                            var subItemIds = subContents.Select(x => x.ItemId).Distinct().ToList();
                            var subMappings = KpiItemContentKpiCriteriaItemMappingDAOs.Where(x => subContentIds.Contains(x.KpiItemContentId)).ToList();
                            var subOrders = IndirectSalesOrderTransactionDAOs.Where(x => x.SalesEmployeeId == KpiItem.EmployeeId).ToList();

                            foreach (var ItemId in subItemIds)
                            {
                                PermissionMobile_EmployeeKpiItemReportDTO PermissionMobile_EmployeeKpiItemReportDTO = new PermissionMobile_EmployeeKpiItemReportDTO
                                {
                                    ItemId = ItemId,
                                    ItemName = ItemDAOs.Where(x => x.Id == ItemId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiItems = new List<PermissionMobile_EmployeeKpiItem>()
                                };

                                foreach (var KpiCriteriaItem in KpiCriteriaItems)
                                {
                                    PermissionMobile_EmployeeKpiItem PermissionMobile_EmployeeKpiItem = new PermissionMobile_EmployeeKpiItem
                                    {
                                        KpiCriteriaItemName = KpiCriteriaItem.Name,
                                        ItemId = ItemId
                                    };
                                    PermissionMobile_EmployeeKpiItemReportDTO.CurrentKpiItems.Add(PermissionMobile_EmployeeKpiItem);
                                    PermissionMobile_EmployeeKpiItem.PlannedValue = subMappings
                                        .Where(x => x.KpiCriteriaItemId == KpiCriteriaItem.Id && x.KpiItemContent.ItemId == ItemId)
                                        .Where(x => x.Value.HasValue)
                                        .Select(x => (decimal)x.Value.Value)
                                        .Sum();
                                    if (KpiCriteriaItem.Id == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiItem.CurrentValue = subOrders
                                            .Where(x => x.ItemId == ItemId)
                                            .Select(x => x.Revenue ?? 0).Sum();
                                    }
                                    if (KpiCriteriaItem.Id == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiItem.CurrentValue = subOrders
                                            .Where(x => x.ItemId == ItemId)
                                            .Select(x => x.BuyerStoreId).Distinct().Count();
                                    }
                                    PermissionMobile_EmployeeKpiItem.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiItem.PlannedValue, PermissionMobile_EmployeeKpiItem.CurrentValue);
                                }
                                subResults.Add(PermissionMobile_EmployeeKpiItemReportDTO);
                            }
                        }

                        if (subResults.Count > 0)
                        {
                            foreach (var ItemId in ItemIds)
                            {
                                PermissionMobile_EmployeeKpiItemReportDTO PermissionMobile_EmployeeKpiItemReportDTO = new PermissionMobile_EmployeeKpiItemReportDTO
                                {
                                    ItemId = ItemId,
                                    ItemName = ItemDAOs.Where(x => x.Id == ItemId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiItems = new List<PermissionMobile_EmployeeKpiItem>()
                                };
                                KpiItemDTOs.Add(PermissionMobile_EmployeeKpiItemReportDTO);

                                var resultGroupByItems = subResults.Where(x => x.ItemId == ItemId).ToList();
                                foreach (var KpiCriteriaItem in KpiCriteriaItems)
                                {
                                    PermissionMobile_EmployeeKpiItem PermissionMobile_EmployeeKpiItem = new PermissionMobile_EmployeeKpiItem
                                    {
                                        KpiCriteriaItemName = KpiCriteriaItem.Name,
                                        ItemId = ItemId
                                    };
                                    PermissionMobile_EmployeeKpiItemReportDTO.CurrentKpiItems.Add(PermissionMobile_EmployeeKpiItem);
                                    PermissionMobile_EmployeeKpiItem.PlannedValue = resultGroupByItems
                                        .SelectMany(x => x.CurrentKpiItems)
                                        .Where(x => x.KpiCriteriaItemName == KpiCriteriaItem.Name)
                                        .Select(x => x.PlannedValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiItem.CurrentValue = resultGroupByItems
                                        .SelectMany(x => x.CurrentKpiItems)
                                        .Where(x => x.KpiCriteriaItemName == KpiCriteriaItem.Name)
                                        .Select(x => x.CurrentValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiItem.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiItem.PlannedValue, PermissionMobile_EmployeeKpiItem.CurrentValue);
                                }
                            }
                        }
                    }
                }
            }

            return KpiItemDTOs;
        }

        [Route(PermissionMobileRoute.ListCurrentKpiNewItem), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiItemReportDTO>> ListCurrentKpiNewItem([FromBody] PermissionMobile_EmployeeKpiItemReportFilterDTO PermissionMobile_EmployeeKpiFilterDTO)
        {
            var KpiItemDTOs = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            Start = Start.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }

            List<KpiItemDAO> KpiItems = await DataContext.KpiItem.Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.KpiItemTypeId == KpiItemTypeEnum.NEW_PRODUCT.Id &&
                    x.DeletedAt == null)
                .Select(x => new KpiItemDAO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                }).ToListAsync();
            var KpiItemIds = KpiItems.Select(x => x.Id).ToList();
            if (KpiItems.Count > 0)
            {
                List<KpiItemContentDAO> KpiItemContentDAOs = await DataContext.KpiItemContent
                    .Where(x => KpiItemIds.Contains(x.KpiItemId))
                    .Select(x => new KpiItemContentDAO
                    {
                        Id = x.Id,
                        KpiItemId = x.KpiItemId,
                        ItemId = x.ItemId,
                        KpiItem = x.KpiItem == null ? null : new KpiItemDAO
                        {
                            EmployeeId = x.KpiItem.EmployeeId
                        }
                    })
                    .ToListAsync();
                List<long> KpiItemContentIds = KpiItemContentDAOs.Select(x => x.Id).ToList();
                if (KpiItemContentIds.Count > 0)
                {
                    List<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMappingDAOs = await DataContext.KpiItemContentKpiCriteriaItemMapping
                        .Where(x => KpiItemContentIds.Contains(x.KpiItemContentId))
                        .Select(x => new KpiItemContentKpiCriteriaItemMappingDAO
                        {
                            KpiCriteriaItemId = x.KpiCriteriaItemId,
                            KpiItemContentId = x.KpiItemContentId,
                            Value = x.Value,
                            KpiItemContent = x.KpiItemContent == null ? null : new KpiItemContentDAO
                            {
                                ItemId = x.KpiItemContent.ItemId
                            }
                        })
                        .ToListAsync();
                    List<PermissionMobile_EmployeeKpiItem> PermissionMobile_EmployeeKpiItems = new List<PermissionMobile_EmployeeKpiItem>();
                    if (KpiItemContentKpiCriteriaItemMappingDAOs.Count > 0)
                    {
                        List<long> ItemIds = KpiItemContentDAOs.Select(x => x.ItemId).Distinct().ToList(); // lẩy ra list itemId theo chỉ tiêu
                        List<ItemDAO> ItemDAOs = await DataContext.Item.Where(x => ItemIds.Contains(x.Id)).Select(x => new ItemDAO
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                        }).ToListAsync();

                        var query = from t in DataContext.IndirectSalesOrderTransaction
                                    join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                                    join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                                    join it in DataContext.Item on t.ItemId equals it.Id
                                    where Start <= t.OrderDate && t.OrderDate <= End &&
                                    AppUserIds.Contains(t.SalesEmployeeId) &&
                                    ItemIds.Contains(t.ItemId) &&
                                    au.DeletedAt == null && it.DeletedAt == null
                                    select new IndirectSalesOrderTransactionDAO
                                    {
                                        ItemId = t.ItemId,
                                        Revenue = t.Revenue,
                                        BuyerStoreId = t.BuyerStoreId,
                                        SalesEmployeeId = t.SalesEmployeeId
                                    };

                        List<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactionDAOs = await query.ToListAsync();
                        List<GenericEnum> KpiCriteriaItems = KpiCriteriaItemEnum.KpiCriteriaItemEnumList;
                        var subResults = new List<PermissionMobile_EmployeeKpiItemReportDTO>();
                        foreach (var KpiItem in KpiItems)
                        {
                            var subContents = KpiItemContentDAOs.Where(x => x.KpiItemId == KpiItem.Id).ToList();
                            var subContentIds = subContents.Select(x => x.Id).ToList();
                            var subItemIds = subContents.Select(x => x.ItemId).Distinct().ToList();
                            var subMappings = KpiItemContentKpiCriteriaItemMappingDAOs.Where(x => subContentIds.Contains(x.KpiItemContentId)).ToList();
                            var subOrders = IndirectSalesOrderTransactionDAOs.Where(x => x.SalesEmployeeId == KpiItem.EmployeeId).ToList();

                            foreach (var ItemId in subItemIds)
                            {
                                PermissionMobile_EmployeeKpiItemReportDTO PermissionMobile_EmployeeKpiItemReportDTO = new PermissionMobile_EmployeeKpiItemReportDTO
                                {
                                    ItemId = ItemId,
                                    ItemName = ItemDAOs.Where(x => x.Id == ItemId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiItems = new List<PermissionMobile_EmployeeKpiItem>()
                                };

                                foreach (var KpiCriteriaItem in KpiCriteriaItems)
                                {
                                    PermissionMobile_EmployeeKpiItem PermissionMobile_EmployeeKpiItem = new PermissionMobile_EmployeeKpiItem
                                    {
                                        KpiCriteriaItemName = KpiCriteriaItem.Name,
                                        ItemId = ItemId
                                    };
                                    PermissionMobile_EmployeeKpiItemReportDTO.CurrentKpiItems.Add(PermissionMobile_EmployeeKpiItem);
                                    PermissionMobile_EmployeeKpiItem.PlannedValue = subMappings
                                        .Where(x => x.KpiCriteriaItemId == KpiCriteriaItem.Id && x.KpiItemContent.ItemId == ItemId)
                                        .Where(x => x.Value.HasValue)
                                        .Select(x => (decimal)x.Value.Value)
                                        .Sum();
                                    if (KpiCriteriaItem.Id == KpiCriteriaItemEnum.INDIRECT_REVENUE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiItem.CurrentValue = subOrders
                                            .Where(x => x.ItemId == ItemId)
                                            .Select(x => x.Revenue ?? 0).Sum();
                                    }
                                    if (KpiCriteriaItem.Id == KpiCriteriaItemEnum.INDIRECT_STORE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiItem.CurrentValue = subOrders
                                            .Where(x => x.ItemId == ItemId)
                                            .Select(x => x.BuyerStoreId).Distinct().Count();
                                    }
                                    PermissionMobile_EmployeeKpiItem.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiItem.PlannedValue, PermissionMobile_EmployeeKpiItem.CurrentValue);
                                }
                                subResults.Add(PermissionMobile_EmployeeKpiItemReportDTO);
                            }
                        }

                        if (subResults.Count > 0)
                        {
                            foreach (var ItemId in ItemIds)
                            {
                                PermissionMobile_EmployeeKpiItemReportDTO PermissionMobile_EmployeeKpiItemReportDTO = new PermissionMobile_EmployeeKpiItemReportDTO
                                {
                                    ItemId = ItemId,
                                    ItemName = ItemDAOs.Where(x => x.Id == ItemId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiItems = new List<PermissionMobile_EmployeeKpiItem>()
                                };
                                KpiItemDTOs.Add(PermissionMobile_EmployeeKpiItemReportDTO);

                                var resultGroupByItems = subResults.Where(x => x.ItemId == ItemId).ToList();
                                foreach (var KpiCriteriaItem in KpiCriteriaItems)
                                {
                                    PermissionMobile_EmployeeKpiItem PermissionMobile_EmployeeKpiItem = new PermissionMobile_EmployeeKpiItem
                                    {
                                        KpiCriteriaItemName = KpiCriteriaItem.Name,
                                        ItemId = ItemId
                                    };
                                    PermissionMobile_EmployeeKpiItemReportDTO.CurrentKpiItems.Add(PermissionMobile_EmployeeKpiItem);
                                    PermissionMobile_EmployeeKpiItem.PlannedValue = resultGroupByItems
                                        .SelectMany(x => x.CurrentKpiItems)
                                        .Where(x => x.KpiCriteriaItemName == KpiCriteriaItem.Name)
                                        .Select(x => x.PlannedValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiItem.CurrentValue = resultGroupByItems
                                        .SelectMany(x => x.CurrentKpiItems)
                                        .Where(x => x.KpiCriteriaItemName == KpiCriteriaItem.Name)
                                        .Select(x => x.CurrentValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiItem.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiItem.PlannedValue, PermissionMobile_EmployeeKpiItem.CurrentValue);
                                }
                            }
                        }
                    }
                }
            }

            return KpiItemDTOs;
        }

        [Route(PermissionMobileRoute.ListCurrentKpiProductGrouping), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiProductGroupingReportDTO>> ListCurrentKpiProductGrouping([FromBody] PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO)
        {
            var KpiProductGroupingDTOs = new List<PermissionMobile_EmployeeKpiProductGroupingReportDTO>();

            #region chuẩn bị dữ liệu filter: thời gian tính kpi, AppUserIds
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            Start = Start.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }
            #endregion

            #region lấy giữ liệu kpi
            List<KpiProductGroupingDAO> KpiProductGroupings = await DataContext.KpiProductGrouping.AsNoTracking().Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.KpiProductGroupingTypeId == KpiProductGroupingTypeEnum.ALL_NEW_PRODUCT.Id &&
                    x.DeletedAt == null)
                .Select(x => new KpiProductGroupingDAO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId
                }).ToListAsync();
            #endregion

            #region tổng hợp dữ liệu
            if (KpiProductGroupings.Count > 0)
            {
                List<long> KpiProductGroupingIds = KpiProductGroupings.Select(x => x.Id).ToList();
                List<KpiProductGroupingContentDAO> KpiProductGroupingContentDAOs = await DataContext.KpiProductGroupingContent.AsNoTracking()
                    .Where(x => KpiProductGroupingIds.Contains(x.KpiProductGroupingId))
                    .Select(c => new KpiProductGroupingContentDAO
                    {
                        Id = c.Id,
                        ProductGroupingId = c.ProductGroupingId,
                        KpiProductGroupingId = c.KpiProductGroupingId,
                        KpiProductGrouping = c.KpiProductGrouping == null ? null : new KpiProductGroupingDAO
                        {
                            EmployeeId = c.KpiProductGrouping.EmployeeId
                        }
                    }).ToListAsync(); // lấy ra toàn bộ content của kpi
                if (KpiProductGroupingContentDAOs.Count > 0)
                {
                    List<long> KpiProductGroupingContentIds = KpiProductGroupingContentDAOs.Select(x => x.Id).ToList();
                    List<KpiProductGroupingContentCriteriaMappingDAO> KpiProductGroupingContentCriteriaMappingDAOs = await DataContext.KpiProductGroupingContentCriteriaMapping.AsNoTracking()
                        .Where(x => KpiProductGroupingContentIds.Contains(x.KpiProductGroupingContentId))
                        .Select(c => new KpiProductGroupingContentCriteriaMappingDAO
                        {
                            KpiProductGroupingContentId = c.KpiProductGroupingContentId,
                            KpiProductGroupingCriteriaId = c.KpiProductGroupingCriteriaId,
                            Value = c.Value,
                            KpiProductGroupingContent = c.KpiProductGroupingContent == null ? null : new KpiProductGroupingContentDAO
                            {
                                KpiProductGroupingId = c.KpiProductGroupingContent.KpiProductGroupingId,
                                ProductGroupingId = c.KpiProductGroupingContent.ProductGroupingId
                            }
                        }).ToListAsync(); // lấy ra toàn bộ mapping content với chỉ tiêu
                    List<KpiProductGroupingContentItemMappingDAO> KpiProductGroupingContentItemMappingDAOs = await DataContext.KpiProductGroupingContentItemMapping.AsNoTracking()
                        .Where(x => KpiProductGroupingContentIds.Contains(x.KpiProductGroupingContentId))
                        .Select(c => new KpiProductGroupingContentItemMappingDAO
                        {
                            ItemId = c.ItemId,
                            KpiProductGroupingContentId = c.KpiProductGroupingContentId,
                        }).ToListAsync(); // lấy ra toàn bộ mapping content với Item
                    if (KpiProductGroupingContentCriteriaMappingDAOs.Count > 0 && KpiProductGroupingContentItemMappingDAOs.Count > 0)
                    {
                        List<long> ProductGroupingIds = KpiProductGroupingContentDAOs.Select(x => x.ProductGroupingId)
                            .Distinct()
                            .ToList();
                        List<long> ItemIds = KpiProductGroupingContentItemMappingDAOs.Select(x => x.ItemId)
                            .Distinct()
                            .ToList();
                        List<ProductGroupingDAO> ProductGroupingDAOs = await DataContext.ProductGrouping.AsNoTracking()
                            .Where(x => ProductGroupingIds.Contains(x.Id))
                            .ToListAsync();

                        var query = from t in DataContext.IndirectSalesOrderTransaction
                                    join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                                    join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                                    join it in DataContext.Item on t.ItemId equals it.Id
                                    where Start <= t.OrderDate && t.OrderDate <= End &&
                                    AppUserIds.Contains(t.SalesEmployeeId) &&
                                    ItemIds.Contains(t.ItemId) &&
                                    au.DeletedAt == null && it.DeletedAt == null
                                    select new IndirectSalesOrderTransactionDAO
                                    {
                                        ItemId = t.ItemId,
                                        Revenue = t.Revenue,
                                        BuyerStoreId = t.BuyerStoreId,
                                        SalesEmployeeId = t.SalesEmployeeId
                                    };

                        List<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactionDAOs = await query.ToListAsync(); // lấy ra toàn bộ đơn hàng thỏa mãn điều kiện thời gian, nhân viên, Item
                        List<GenericEnum> KpiProductGroupingCriterias = KpiProductGroupingCriteriaEnum.KpiProductGroupingCriteriaEnumList; // lấy ra toàn bộ chỉ tiêu kpi
                        List<PermissionMobile_EmployeeKpiProductGroupingReportDTO> SubResults = new List<PermissionMobile_EmployeeKpiProductGroupingReportDTO>(); // lấy ra kết quả chưa group theo ProductGrouping
                        foreach (var KpiProductGrouping in KpiProductGroupings)
                        {
                            List<KpiProductGroupingContentDAO> Contents = KpiProductGroupingContentDAOs
                                .Where(x => x.KpiProductGroupingId == KpiProductGrouping.Id)
                                .ToList(); // lấy ra Content của Kpi
                            List<long> ContentIds = Contents.Select(x => x.Id).ToList();
                            List<long> ContentProductGroupingIds = Contents.Select(x => x.ProductGroupingId)
                                .Distinct()
                                .ToList();

                            foreach (var ProductGroupingId in ContentProductGroupingIds)
                            {
                                List<KpiProductGroupingContentDAO> SubContents = KpiProductGroupingContentDAOs
                                    .Where(x => x.ProductGroupingId == ProductGroupingId)
                                    .ToList(); // lấy ra Content của từng nhóm sp
                                List<long> SubContentIds = SubContents.Select(x => x.Id).ToList();
                                List<KpiProductGroupingContentCriteriaMappingDAO> SubContentCriteriaMappings = KpiProductGroupingContentCriteriaMappingDAOs
                                    .Where(x => SubContentIds.Contains(x.KpiProductGroupingContentId))
                                    .ToList(); // lay ra mapping item cua content phu
                                List<KpiProductGroupingContentItemMappingDAO> SubContentItemMappings = KpiProductGroupingContentItemMappingDAOs
                                    .Where(x => SubContentIds.Contains(x.KpiProductGroupingContentId))
                                    .ToList(); // lay ra mapping item cua content phu
                                List<long> SubItemIds = SubContentItemMappings.Select(x => x.ItemId).ToList();
                                List<IndirectSalesOrderTransactionDAO> SubOrders = IndirectSalesOrderTransactionDAOs
                                    .Where(x => x.SalesEmployeeId == KpiProductGrouping.EmployeeId && SubItemIds.Contains(x.ItemId))
                                    .ToList();

                                PermissionMobile_EmployeeKpiProductGroupingReportDTO PermissionMobile_EmployeeKpiProductGroupingReportDTO = new PermissionMobile_EmployeeKpiProductGroupingReportDTO
                                {
                                    ProductGroupingId = ProductGroupingId,
                                    ProductGroupingName = ProductGroupingDAOs.Where(x => x.Id == ProductGroupingId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiProductGroupings = new List<PermissionMobile_EmployeeKpiProductGrouping>()
                                };
                                foreach (var KpiProductGroupingCriteria in KpiProductGroupingCriterias)
                                {
                                    PermissionMobile_EmployeeKpiProductGrouping PermissionMobile_EmployeeKpiProductGrouping = new PermissionMobile_EmployeeKpiProductGrouping
                                    {
                                        ProductGroupingId = ProductGroupingId,
                                        KpiProductGroupingCriteriaName = KpiProductGroupingCriteria.Name
                                    };
                                    PermissionMobile_EmployeeKpiProductGroupingReportDTO.CurrentKpiProductGroupings
                                        .Add(PermissionMobile_EmployeeKpiProductGrouping);
                                    PermissionMobile_EmployeeKpiProductGrouping.PlannedValue = SubContentCriteriaMappings
                                        .Where(x => x.KpiProductGroupingCriteriaId == KpiProductGroupingCriteria.Id)
                                        .Where(x => x.KpiProductGroupingContent.ProductGroupingId == ProductGroupingId)
                                        .Where(x => x.KpiProductGroupingContent.KpiProductGroupingId == KpiProductGrouping.Id)
                                        .Where(x => x.Value.HasValue)
                                        .Select(x => (decimal)x.Value.Value)
                                        .Sum(); // lấy ra số kế hoạch
                                    if (KpiProductGroupingCriteria.Id == KpiProductGroupingCriteriaEnum.INDIRECT_REVENUE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiProductGrouping.CurrentValue = SubOrders
                                            .Select(x => x.Revenue ?? 0)
                                            .Sum();
                                    }
                                    if (KpiProductGroupingCriteria.Id == KpiProductGroupingCriteriaEnum.INDIRECT_STORE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiProductGrouping.CurrentValue = SubOrders
                                            .Select(x => x.BuyerStoreId)
                                            .Distinct()
                                            .Count();
                                    }
                                }
                                SubResults.Add(PermissionMobile_EmployeeKpiProductGroupingReportDTO);
                            } // lấy ra đơn hàng thỏa mãn cùng employeeId và ItemId  
                        }
                        if (SubResults.Count > 0)
                        {
                            foreach (var ProductGroupingId in ProductGroupingIds)
                            {
                                PermissionMobile_EmployeeKpiProductGroupingReportDTO PermissionMobile_EmployeeKpiProductGroupingReportDTO = new PermissionMobile_EmployeeKpiProductGroupingReportDTO
                                {
                                    ProductGroupingId = ProductGroupingId,
                                    ProductGroupingName = ProductGroupingDAOs.Where(x => x.Id == ProductGroupingId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiProductGroupings = new List<PermissionMobile_EmployeeKpiProductGrouping>()
                                };
                                KpiProductGroupingDTOs.Add(PermissionMobile_EmployeeKpiProductGroupingReportDTO);
                                var ResultGroupByProductGrouping = SubResults
                                    .Where(x => x.ProductGroupingId == PermissionMobile_EmployeeKpiProductGroupingReportDTO.ProductGroupingId)
                                    .ToList();
                                foreach (var KpiProductGroupingCriteria in KpiProductGroupingCriterias)
                                {
                                    PermissionMobile_EmployeeKpiProductGrouping PermissionMobile_EmployeeKpiProductGrouping = new PermissionMobile_EmployeeKpiProductGrouping
                                    {
                                        ProductGroupingId = ProductGroupingId,
                                        KpiProductGroupingCriteriaName = KpiProductGroupingCriteria.Name
                                    };
                                    PermissionMobile_EmployeeKpiProductGroupingReportDTO.CurrentKpiProductGroupings
                                            .Add(PermissionMobile_EmployeeKpiProductGrouping);
                                    List<PermissionMobile_EmployeeKpiProductGrouping> CurrentGroupByProductGrouping = ResultGroupByProductGrouping
                                        .SelectMany(x => x.CurrentKpiProductGroupings)
                                        .ToList();
                                    PermissionMobile_EmployeeKpiProductGrouping.PlannedValue = CurrentGroupByProductGrouping
                                         .Where(x => x.KpiProductGroupingCriteriaName == KpiProductGroupingCriteria.Name)
                                        .Select(x => x.PlannedValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiProductGrouping.CurrentValue = CurrentGroupByProductGrouping
                                        .Where(x => x.KpiProductGroupingCriteriaName == KpiProductGroupingCriteria.Name)
                                        .Select(x => x.CurrentValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiProductGrouping.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiProductGrouping.PlannedValue, PermissionMobile_EmployeeKpiProductGrouping.CurrentValue);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return KpiProductGroupingDTOs;
        }

        [Route(PermissionMobileRoute.ListCurrentKpiNewProductGrouping), HttpPost]
        public async Task<List<PermissionMobile_EmployeeKpiProductGroupingReportDTO>> ListCurrentKpiNewProductGrouping([FromBody] PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO)
        {
            var KpiNewProductGroupingDTOs = new List<PermissionMobile_EmployeeKpiProductGroupingReportDTO>();

            #region chuẩn bị dữ liệu filter: thời gian tính kpi, AppUserIds
            // lây ra tháng hiện tại + năm hiện tại
            long KpiYearId = StaticParams.DateTimeNow.Year;
            long KpiPeriodId = StaticParams.DateTimeNow.Month + 100;
            GenericEnum CurrentMonth;
            GenericEnum CurrentQuarter;
            GenericEnum CurrentYear;
            (CurrentMonth, CurrentQuarter, CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            DateTime Start = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1);
            Start = Start.AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(StaticParams.DateTimeNow.Year, StaticParams.DateTimeNow.Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);

            List<long> AppUserIds = new List<long>();
            if (PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO.EmployeeId.Equal.HasValue)
            {
                AppUserIds.Add(PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO.EmployeeId.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }
            #endregion

            #region lấy giữ liệu kpi
            List<KpiProductGroupingDAO> KpiProductGroupings = await DataContext.KpiProductGrouping.AsNoTracking().Where(x =>
                    AppUserIds.Contains(x.EmployeeId) &&
                    x.StatusId == StatusEnum.ACTIVE.Id &&
                    x.KpiPeriodId == KpiPeriodId &&
                    x.KpiYearId == CurrentYear.Id &&
                    x.KpiProductGroupingTypeId == KpiProductGroupingTypeEnum.NEW_PRODUCT_GROUPING.Id &&
                    x.DeletedAt == null)
                .Select(x => new KpiProductGroupingDAO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId
                }).ToListAsync();
            #endregion

            #region tổng hợp dữ liệu
            if (KpiProductGroupings.Count > 0)
            {
                List<long> KpiProductGroupingIds = KpiProductGroupings.Select(x => x.Id).ToList();
                List<KpiProductGroupingContentDAO> KpiProductGroupingContentDAOs = await DataContext.KpiProductGroupingContent.AsNoTracking()
                    .Where(x => KpiProductGroupingIds.Contains(x.KpiProductGroupingId))
                    .Select(c => new KpiProductGroupingContentDAO
                    {
                        Id = c.Id,
                        ProductGroupingId = c.ProductGroupingId,
                        KpiProductGroupingId = c.KpiProductGroupingId,
                        KpiProductGrouping = c.KpiProductGrouping == null ? null : new KpiProductGroupingDAO
                        {
                            EmployeeId = c.KpiProductGrouping.EmployeeId
                        }
                    }).ToListAsync(); // lấy ra toàn bộ content của kpi
                if (KpiProductGroupingContentDAOs.Count > 0)
                {
                    List<long> KpiProductGroupingContentIds = KpiProductGroupingContentDAOs.Select(x => x.Id).ToList();
                    List<KpiProductGroupingContentCriteriaMappingDAO> KpiProductGroupingContentCriteriaMappingDAOs = await DataContext.KpiProductGroupingContentCriteriaMapping.AsNoTracking()
                        .Where(x => KpiProductGroupingContentIds.Contains(x.KpiProductGroupingContentId))
                        .Select(c => new KpiProductGroupingContentCriteriaMappingDAO
                        {
                            KpiProductGroupingContentId = c.KpiProductGroupingContentId,
                            KpiProductGroupingCriteriaId = c.KpiProductGroupingCriteriaId,
                            Value = c.Value,
                            KpiProductGroupingContent = c.KpiProductGroupingContent == null ? null : new KpiProductGroupingContentDAO
                            {
                                KpiProductGroupingId = c.KpiProductGroupingContent.KpiProductGroupingId,
                                ProductGroupingId = c.KpiProductGroupingContent.ProductGroupingId
                            }
                        }).ToListAsync(); // lấy ra toàn bộ mapping content với chỉ tiêu
                    List<KpiProductGroupingContentItemMappingDAO> KpiProductGroupingContentItemMappingDAOs = await DataContext.KpiProductGroupingContentItemMapping.AsNoTracking()
                        .Where(x => KpiProductGroupingContentIds.Contains(x.KpiProductGroupingContentId))
                        .Select(c => new KpiProductGroupingContentItemMappingDAO
                        {
                            ItemId = c.ItemId,
                            KpiProductGroupingContentId = c.KpiProductGroupingContentId,
                        }).ToListAsync(); // lấy ra toàn bộ mapping content với Item
                    if (KpiProductGroupingContentCriteriaMappingDAOs.Count > 0 && KpiProductGroupingContentItemMappingDAOs.Count > 0)
                    {
                        List<long> ProductGroupingIds = KpiProductGroupingContentDAOs.Select(x => x.ProductGroupingId)
                            .Distinct()
                            .ToList();
                        List<long> ItemIds = KpiProductGroupingContentItemMappingDAOs.Select(x => x.ItemId)
                            .Distinct()
                            .ToList();
                        List<ProductGroupingDAO> ProductGroupingDAOs = await DataContext.ProductGrouping.AsNoTracking()
                            .Where(x => ProductGroupingIds.Contains(x.Id))
                            .ToListAsync();

                        var query = from t in DataContext.IndirectSalesOrderTransaction
                                    join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                                    join au in DataContext.AppUser on t.SalesEmployeeId equals au.Id
                                    join it in DataContext.Item on t.ItemId equals it.Id
                                    where Start <= t.OrderDate && t.OrderDate <= End &&
                                    AppUserIds.Contains(t.SalesEmployeeId) &&
                                    ItemIds.Contains(t.ItemId) &&
                                    au.DeletedAt == null && it.DeletedAt == null
                                    select new IndirectSalesOrderTransactionDAO
                                    {
                                        ItemId = t.ItemId,
                                        Revenue = t.Revenue,
                                        BuyerStoreId = t.BuyerStoreId,
                                        SalesEmployeeId = t.SalesEmployeeId
                                    };

                        List<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactionDAOs = await query.ToListAsync(); // lấy ra toàn bộ đơn hàng thỏa mãn điều kiện thời gian, nhân viên, Item
                        List<GenericEnum> KpiProductGroupingCriterias = KpiProductGroupingCriteriaEnum.KpiProductGroupingCriteriaEnumList; // lấy ra toàn bộ chỉ tiêu kpi
                        List<PermissionMobile_EmployeeKpiProductGroupingReportDTO> SubResults = new List<PermissionMobile_EmployeeKpiProductGroupingReportDTO>(); // lấy ra kết quả chưa group theo ProductGrouping
                        foreach (var KpiProductGrouping in KpiProductGroupings)
                        {
                            List<KpiProductGroupingContentDAO> Contents = KpiProductGroupingContentDAOs
                                .Where(x => x.KpiProductGroupingId == KpiProductGrouping.Id)
                                .ToList(); // lấy ra Content của Kpi
                            List<long> ContentIds = Contents.Select(x => x.Id).ToList();
                            List<long> ContentProductGroupingIds = Contents.Select(x => x.ProductGroupingId)
                                .Distinct()
                                .ToList();

                            foreach (var ProductGroupingId in ContentProductGroupingIds)
                            {
                                List<KpiProductGroupingContentDAO> SubContents = KpiProductGroupingContentDAOs
                                    .Where(x => x.ProductGroupingId == ProductGroupingId)
                                    .ToList(); // lấy ra Content của từng nhóm sp
                                List<long> SubContentIds = SubContents.Select(x => x.Id).ToList();
                                List<KpiProductGroupingContentCriteriaMappingDAO> SubContentCriteriaMappings = KpiProductGroupingContentCriteriaMappingDAOs
                                    .Where(x => SubContentIds.Contains(x.KpiProductGroupingContentId))
                                    .ToList(); // lay ra mapping item cua content phu
                                List<KpiProductGroupingContentItemMappingDAO> SubContentItemMappings = KpiProductGroupingContentItemMappingDAOs
                                    .Where(x => SubContentIds.Contains(x.KpiProductGroupingContentId))
                                    .ToList(); // lay ra mapping item cua content phu
                                List<long> SubItemIds = SubContentItemMappings.Select(x => x.ItemId).ToList();
                                List<IndirectSalesOrderTransactionDAO> SubOrders = IndirectSalesOrderTransactionDAOs
                                    .Where(x => x.SalesEmployeeId == KpiProductGrouping.EmployeeId && SubItemIds.Contains(x.ItemId))
                                    .ToList();
                                PermissionMobile_EmployeeKpiProductGroupingReportDTO PermissionMobile_EmployeeKpiProductGroupingReportDTO = new PermissionMobile_EmployeeKpiProductGroupingReportDTO
                                {
                                    ProductGroupingId = ProductGroupingId,
                                    ProductGroupingName = ProductGroupingDAOs.Where(x => x.Id == ProductGroupingId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiProductGroupings = new List<PermissionMobile_EmployeeKpiProductGrouping>()
                                };
                                foreach (var KpiProductGroupingCriteria in KpiProductGroupingCriterias)
                                {
                                    PermissionMobile_EmployeeKpiProductGrouping PermissionMobile_EmployeeKpiProductGrouping = new PermissionMobile_EmployeeKpiProductGrouping
                                    {
                                        ProductGroupingId = ProductGroupingId,
                                        KpiProductGroupingCriteriaName = KpiProductGroupingCriteria.Name
                                    };
                                    PermissionMobile_EmployeeKpiProductGroupingReportDTO.CurrentKpiProductGroupings
                                        .Add(PermissionMobile_EmployeeKpiProductGrouping);
                                    PermissionMobile_EmployeeKpiProductGrouping.PlannedValue = SubContentCriteriaMappings
                                        .Where(x => x.KpiProductGroupingCriteriaId == KpiProductGroupingCriteria.Id)
                                        .Where(x => x.KpiProductGroupingContent.ProductGroupingId == ProductGroupingId)
                                        .Where(x => x.KpiProductGroupingContent.KpiProductGroupingId == KpiProductGrouping.Id)
                                        .Where(x => x.Value.HasValue)
                                        .Select(x => (decimal)x.Value.Value)
                                        .Sum(); // lấy ra số kế hoạch
                                    if (KpiProductGroupingCriteria.Id == KpiProductGroupingCriteriaEnum.INDIRECT_REVENUE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiProductGrouping.CurrentValue = SubOrders
                                            .Select(x => x.Revenue ?? 0)
                                            .Sum();
                                    }
                                    if (KpiProductGroupingCriteria.Id == KpiProductGroupingCriteriaEnum.INDIRECT_STORE.Id)
                                    {
                                        PermissionMobile_EmployeeKpiProductGrouping.CurrentValue = SubOrders
                                            .Select(x => x.BuyerStoreId)
                                            .Distinct()
                                            .Count();
                                    }
                                }
                                SubResults.Add(PermissionMobile_EmployeeKpiProductGroupingReportDTO);
                            } // lấy ra đơn hàng thỏa mãn cùng employeeId và ItemId  
                        }
                        if (SubResults.Count > 0)
                        {
                            foreach (var ProductGroupingId in ProductGroupingIds)
                            {
                                PermissionMobile_EmployeeKpiProductGroupingReportDTO PermissionMobile_EmployeeKpiProductGroupingReportDTO = new PermissionMobile_EmployeeKpiProductGroupingReportDTO
                                {
                                    ProductGroupingId = ProductGroupingId,
                                    ProductGroupingName = ProductGroupingDAOs.Where(x => x.Id == ProductGroupingId).Select(x => x.Name).FirstOrDefault(),
                                    CurrentKpiProductGroupings = new List<PermissionMobile_EmployeeKpiProductGrouping>()
                                };
                                KpiNewProductGroupingDTOs.Add(PermissionMobile_EmployeeKpiProductGroupingReportDTO);
                                var ResultGroupByProductGrouping = SubResults
                                    .Where(x => x.ProductGroupingId == PermissionMobile_EmployeeKpiProductGroupingReportDTO.ProductGroupingId)
                                    .ToList();
                                foreach (var KpiProductGroupingCriteria in KpiProductGroupingCriterias)
                                {
                                    PermissionMobile_EmployeeKpiProductGrouping PermissionMobile_EmployeeKpiProductGrouping = new PermissionMobile_EmployeeKpiProductGrouping
                                    {
                                        ProductGroupingId = ProductGroupingId,
                                        KpiProductGroupingCriteriaName = KpiProductGroupingCriteria.Name
                                    };
                                    PermissionMobile_EmployeeKpiProductGroupingReportDTO.CurrentKpiProductGroupings
                                            .Add(PermissionMobile_EmployeeKpiProductGrouping);
                                    List<PermissionMobile_EmployeeKpiProductGrouping> CurrentGroupByProductGrouping = ResultGroupByProductGrouping
                                        .SelectMany(x => x.CurrentKpiProductGroupings)
                                        .ToList();
                                    PermissionMobile_EmployeeKpiProductGrouping.PlannedValue = CurrentGroupByProductGrouping
                                         .Where(x => x.KpiProductGroupingCriteriaName == KpiProductGroupingCriteria.Name)
                                        .Select(x => x.PlannedValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiProductGrouping.CurrentValue = CurrentGroupByProductGrouping
                                        .Where(x => x.KpiProductGroupingCriteriaName == KpiProductGroupingCriteria.Name)
                                        .Select(x => x.CurrentValue)
                                        .Sum();
                                    PermissionMobile_EmployeeKpiProductGrouping.Percentage = CalculatePercentage(PermissionMobile_EmployeeKpiProductGrouping.PlannedValue, PermissionMobile_EmployeeKpiProductGrouping.CurrentValue);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return KpiNewProductGroupingDTOs;
        }
        #endregion

        #region Dashboard Order: Count Store and StoreChecking
        [Route(PermissionMobileRoute.SingleListPeriod), HttpPost]
        public async Task<List<PermissionMobile_EnumList>> SingleListPeriod()
        {
            List<PermissionMobile_EnumList> Periods = new List<PermissionMobile_EnumList>();
            Periods.Add(new PermissionMobile_EnumList { Id = TODAY, Name = "Hôm nay" });
            Periods.Add(new PermissionMobile_EnumList { Id = THIS_WEEK, Name = "Tuần này" });
            Periods.Add(new PermissionMobile_EnumList { Id = THIS_MONTH, Name = "Tháng này" });
            //Periods.Add(new PermissionMobile_EnumList { Id = LAST_MONTH, Name = "Tháng trước" });
            Periods.Add(new PermissionMobile_EnumList { Id = THIS_QUARTER, Name = "Quý này" });
            //Periods.Add(new PermissionMobile_EnumList { Id = LAST_QUATER, Name = "Quý trước" });
            Periods.Add(new PermissionMobile_EnumList { Id = YEAR, Name = "Năm" });
            return Periods;
        }
        [Route(PermissionMobileRoute.CountStoreChecking), HttpPost]
        public async Task<long> CountStoreChecking([FromBody] PermissionMobile_FilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from sc in DataContext.StoreChecking
                        where AppUserIds.Contains(sc.SaleEmployeeId)
                        && (sc.CheckOutAt.HasValue && sc.CheckOutAt.Value >= Start && sc.CheckOutAt.Value <= End)
                        select sc;

            var count = await query.CountAsync();
            return count;
        } // số lượt viếng thăm theo nhân viên được phân quyền

        [Route(PermissionMobileRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] PermissionMobile_FilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from sc in DataContext.StoreChecking
                        where AppUserIds.Contains(sc.SaleEmployeeId)
                        && (sc.CheckOutAt.HasValue && sc.CheckOutAt.Value >= Start && sc.CheckOutAt.Value <= End)
                        select new
                        {
                            Id = sc.StoreId
                        };

            var count = await query
                .Distinct()
                .CountAsync();
            return count;
        } // số dai  viếng thăm
        #endregion

        #region Dashboard Order: IndirectSalesOrder
        [Route(PermissionMobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<long> CountIndirectSalesOrder([FromBody] PermissionMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from ind in DataContext.IndirectSalesOrder
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(ind.SaleEmployeeId)
                        && ind.OrderDate >= Start
                        && ind.OrderDate <= End
                        select ind; // group transaction theo id don hang
            return await query.CountAsync();
        }

        [Route(PermissionMobileRoute.IndirectSalesOrderRevenue), HttpPost]
        public async Task<decimal> IndirectSalesOrderRevenue([FromBody] PermissionMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from ind in DataContext.IndirectSalesOrder
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(ind.SaleEmployeeId)
                        && ind.OrderDate >= Start
                        && ind.OrderDate <= End
                        select ind;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total)
                .DefaultIfEmpty(0)
                .Sum();
        }

        [Route(PermissionMobileRoute.TopIndirectSaleEmployeeRevenue), HttpPost]
        public async Task<List<PermissionMobile_TopRevenueBySalesEmployeeDTO>> TopIndirectSaleEmployeeRevenue([FromBody] PermissionMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from transaction in DataContext.IndirectSalesOrderTransaction
                        join ind in DataContext.IndirectSalesOrder on transaction.IndirectSalesOrderId equals ind.Id
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(ind.SaleEmployeeId)
                        && transaction.OrderDate >= Start
                        && transaction.OrderDate <= End
                        group transaction by transaction.SalesEmployeeId into transGroup
                        select transGroup; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<PermissionMobile_TopRevenueBySalesEmployeeDTO> Result = new List<PermissionMobile_TopRevenueBySalesEmployeeDTO>();
            var transactionGroups = await query
                .Select(x => new DirectSalesOrderTransactionDAO
                {
                    SalesEmployeeId = x.Key,
                    Revenue = x.Sum(x => x.Revenue)
                })
                .ToListAsync();
            List<long> UserIds = transactionGroups
                .Select(x => x.SalesEmployeeId)
                .ToList();
            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => UserIds.Contains(x.Id))
                .ToListAsync();

            foreach (var groupItem in transactionGroups)
            {
                long SaleEmployeeId = groupItem.SalesEmployeeId;
                AppUserDAO SaleEmpolyee = AppUserDAOs
                    .Where(x => x.Id == SaleEmployeeId)
                    .FirstOrDefault();
                PermissionMobile_TopRevenueBySalesEmployeeDTO Item = new PermissionMobile_TopRevenueBySalesEmployeeDTO();
                Item.SaleEmployeeId = SaleEmployeeId;
                Item.SaleEmployeeName = SaleEmpolyee.DisplayName;
                Item.Revenue = groupItem.Revenue.HasValue ? groupItem.Revenue.Value : 0;
                Result.Add(Item);
            }
            Result = Result
                .Where(x => x.Revenue > 0)
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            return Result;
        } // top 5 doanh thu đơn gián tiếp theo nhân viên

        [Route(PermissionMobileRoute.TopIndirecProductRevenue), HttpPost]
        public async Task<List<PermissionMobile_TopRevenueByItemDTO>> TopIndirecProductRevenue([FromBody] PermissionMobile_TopRevenueByItemFilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from transaction in DataContext.IndirectSalesOrderTransaction
                        join ind in DataContext.IndirectSalesOrder on transaction.IndirectSalesOrderId equals ind.Id
                        where ind.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(ind.SaleEmployeeId)
                        && transaction.OrderDate >= Start
                        && transaction.OrderDate <= End
                        group transaction by transaction.ItemId into transGroup
                        select transGroup; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<PermissionMobile_TopRevenueByItemDTO> Result = new List<PermissionMobile_TopRevenueByItemDTO>();
            var transactionGroups = await query
                .Select(x => new DirectSalesOrderTransactionDAO
                {
                    ItemId = x.Key,
                    Revenue = x.Sum(x => x.Revenue)
                })
                .ToListAsync();

            List<long> ItemIds = transactionGroups.Select(x => x.ItemId).ToList();
            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Id = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.Id | ItemSelect.Product
            });
            List<long> ProductIds = Items.Select(x => x.Product.Id).Distinct().ToList();
            List<Product> Products = await ProductService.List(new ProductFilter
            {
                Id = new IdFilter { In = ProductIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.Id | ProductSelect.Name
            });

            foreach (var groupItem in transactionGroups)
            {
                long ItemId = groupItem.ItemId;
                Item Item = Items
                    .Where(x => x.Id == ItemId)
                    .FirstOrDefault();
                if (Item != null)
                {
                    PermissionMobile_TopRevenueByItemDTO ResultItem = new PermissionMobile_TopRevenueByItemDTO();
                    ResultItem.ProductId = Item.Product.Id;
                    ResultItem.Revenue = groupItem.Revenue.HasValue ? groupItem.Revenue.Value : 0;
                    Result.Add(ResultItem);
                }
            }
            Result = Result
                .GroupBy(x => x.ProductId)
                .Select(y => new PermissionMobile_TopRevenueByItemDTO
                {
                    ProductId = y.Key,
                    Revenue = y.Sum(r => r.Revenue)
                })
                .ToList(); // group doanh thu theo id san pham
            foreach (var ResultItem in Result)
            {
                Product Product = Products.Where(x => x.Id == ResultItem.ProductId).FirstOrDefault();
                ResultItem.ProductName = Product.Name;
            }

            Result = Result
                .Where(x => x.Revenue > 0)
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            return Result;
        } // top 5 doanh thu đơn gián tiếp theo item

        [Route(PermissionMobileRoute.IndirectRevenueGrowth), HttpPost]
        public async Task<PermissionMobile_RevenueGrowthDTO> IndirectRevenueGrowth([FromBody] PermissionMobile_RevenueGrowthFilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(filter.Time);

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds
            var query = FilterIndirectTransaction(AppUserIds, Start, End);

            if (filter.Time.Equal.HasValue == false
                || filter.Time.Equal.Value == THIS_MONTH)
            {
                var Transactions = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                RevenueGrowthDTO.IndirectRevenueGrowthByMonths = new List<PermissionMobile_RevenueGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_RevenueGrowthByMonthDTO RevenueGrowthByMonth = new PermissionMobile_RevenueGrowthByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    RevenueGrowthDTO.IndirectRevenueGrowthByMonths.Add(RevenueGrowthByMonth);
                }

                foreach (var RevenueGrowthByMonth in RevenueGrowthDTO.IndirectRevenueGrowthByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, (int)RevenueGrowthByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByMonth.Revenue = Transactions.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_MONTH)
            {
                var Transactions = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                RevenueGrowthDTO.IndirectRevenueGrowthByMonths = new List<PermissionMobile_RevenueGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_RevenueGrowthByMonthDTO RevenueGrowthByMonth = new PermissionMobile_RevenueGrowthByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    RevenueGrowthDTO.IndirectRevenueGrowthByMonths.Add(RevenueGrowthByMonth);
                }

                foreach (var RevenueGrowthByMonth in RevenueGrowthDTO.IndirectRevenueGrowthByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).AddMonths(-1).Month, (int)RevenueGrowthByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByMonth.Revenue = Transactions.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var Transactions = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                RevenueGrowthDTO.IndirectRevenueGrowthByQuaters = new List<PermissionMobile_RevenueGrowthByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_RevenueGrowthByQuarterDTO RevenueGrowthByQuarter = new PermissionMobile_RevenueGrowthByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    RevenueGrowthDTO.IndirectRevenueGrowthByQuaters.Add(RevenueGrowthByQuarter);
                }

                foreach (var RevenueGrowthByQuarter in RevenueGrowthDTO.IndirectRevenueGrowthByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByQuarter.Revenue = Transactions.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                var Transactions = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                RevenueGrowthDTO.IndirectRevenueGrowthByQuaters = new List<PermissionMobile_RevenueGrowthByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_RevenueGrowthByQuarterDTO RevenueGrowthByQuarter = new PermissionMobile_RevenueGrowthByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    RevenueGrowthDTO.IndirectRevenueGrowthByQuaters.Add(RevenueGrowthByQuarter);
                }

                foreach (var RevenueGrowthByQuarter in RevenueGrowthDTO.IndirectRevenueGrowthByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByQuarter.Revenue = Transactions.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == YEAR)
            {
                var Transactions = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                RevenueGrowthDTO.IndirectRevenueGrowthByYears = new List<PermissionMobile_RevenueGrowthByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    PermissionMobile_RevenueGrowthByYearDTO RevenueGrowthByYear = new PermissionMobile_RevenueGrowthByYearDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    RevenueGrowthDTO.IndirectRevenueGrowthByYears.Add(RevenueGrowthByYear);
                }

                foreach (var RevenueGrowthByYear in RevenueGrowthDTO.IndirectRevenueGrowthByYears)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByYear.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByYear.Revenue = Transactions.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return RevenueGrowthDTO;
            }
            return new PermissionMobile_RevenueGrowthDTO();
        } // tăng trưởng doanh thu gián tiếp

        [Route(PermissionMobileRoute.IndirectQuantityGrowth), HttpPost]
        public async Task<PermissionMobile_QuantityGrowthDTO> IndirectSalesOrderGrowth([FromBody] PermissionMobile_QuantityGrowthFilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(filter.Time);

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            if (filter.Time.Equal.HasValue == false
                || filter.Time.Equal.Value == THIS_MONTH)
            {
                var query = from i in FilterIndirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Day into x
                            select new PermissionMobile_QuantityGrowthByMonthDTO
                            {
                                Day = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var OrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths = new List<PermissionMobile_QuantityGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_QuantityGrowthByMonthDTO IndirectSalesOrderQuantityGrowthByMonth = new PermissionMobile_QuantityGrowthByMonthDTO
                    {
                        Day = i,
                        IndirectSalesOrderCounter = 0
                    };
                    QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths.Add(IndirectSalesOrderQuantityGrowthByMonth);
                }

                foreach (var IndirectSalesOrderGrowthByMonth in QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths)
                {
                    var data = OrderGrowthByMonthDTOs.Where(x => x.Day == IndirectSalesOrderGrowthByMonth.Day).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByMonth.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_MONTH)
            {
                var query = from i in FilterIndirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Day into x
                            select new PermissionMobile_QuantityGrowthByMonthDTO
                            {
                                Day = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var OrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths = new List<PermissionMobile_QuantityGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_QuantityGrowthByMonthDTO IndirectSalesOrderQuantityGrowthByMonth = new PermissionMobile_QuantityGrowthByMonthDTO
                    {
                        Day = i,
                        IndirectSalesOrderCounter = 0
                    };
                    QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths.Add(IndirectSalesOrderQuantityGrowthByMonth);
                }

                foreach (var IndirectSalesOrderGrowthByMonth in QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByMonths)
                {
                    var data = OrderGrowthByMonthDTOs.Where(x => x.Day == IndirectSalesOrderGrowthByMonth.Day).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByMonth.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));

                var query = from i in FilterIndirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Month into x
                            select new PermissionMobile_QuantityGrowthByQuarterDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var OrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters = new List<PermissionMobile_QuantityGrowthByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_QuantityGrowthByQuarterDTO IndirectSalesOrderQuantityGrowthByQuarter = new PermissionMobile_QuantityGrowthByQuarterDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters.Add(IndirectSalesOrderQuantityGrowthByQuarter);
                }

                foreach (var IndirectSalesOrderGrowthByQuater in QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters)
                {
                    var data = OrderGrowthByMonthDTOs.Where(x => x.Month == IndirectSalesOrderGrowthByQuater.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByQuater.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                var query = from i in FilterIndirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Month into x
                            select new PermissionMobile_QuantityGrowthByQuarterDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var OrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters = new List<PermissionMobile_QuantityGrowthByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_QuantityGrowthByQuarterDTO IndirectSalesOrderGrowthByQuarter = new PermissionMobile_QuantityGrowthByQuarterDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters.Add(IndirectSalesOrderGrowthByQuarter);
                }

                foreach (var IndirectSalesOrderGrowthByQuater in QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByQuaters)
                {
                    var data = OrderGrowthByMonthDTOs.Where(x => x.Month == IndirectSalesOrderGrowthByQuater.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByQuater.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == YEAR)
            {
                var query = from i in FilterIndirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Month into x
                            select new PermissionMobile_QuantityGrowthByYearDTO
                            {
                                Month = x.Key,
                                IndirectSalesOrderCounter = x.Count()
                            };

                var OrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByYears = new List<PermissionMobile_QuantityGrowthByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    PermissionMobile_QuantityGrowthByYearDTO IndirectSalesOrderGrowthByYear = new PermissionMobile_QuantityGrowthByYearDTO
                    {
                        Month = i,
                        IndirectSalesOrderCounter = 0
                    };
                    QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByYears.Add(IndirectSalesOrderGrowthByYear);
                }

                foreach (var IndirectSalesOrderGrowthByYear in QuantityGrowthDTO.IndirectSalesOrderQuantityGrowthByYears)
                {
                    var data = OrderGrowthByMonthDTOs.Where(x => x.Month == IndirectSalesOrderGrowthByYear.Month).FirstOrDefault();
                    if (data != null)
                        IndirectSalesOrderGrowthByYear.IndirectSalesOrderCounter = data.IndirectSalesOrderCounter;
                }

                return QuantityGrowthDTO;
            }
            return new PermissionMobile_QuantityGrowthDTO();
        } //  tăng trưởng số lượng đơn gián tiếp
        #endregion

        #region Dashboard Order: DirectSalesOrder
        [Route(PermissionMobileRoute.CountDirectSalesOrder), HttpPost]
        public async Task<long> DirectSalesOrder([FromBody] PermissionMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from di in DataContext.DirectSalesOrder
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(di.SaleEmployeeId)
                        && di.OrderDate >= Start
                        && di.OrderDate <= End
                        select di;
            return await query.CountAsync();
        }

        [Route(PermissionMobileRoute.DirectSalesOrderRevenue), HttpPost]
        public async Task<decimal> DirectSalesOrderRevenue([FromBody] PermissionMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from di in DataContext.DirectSalesOrder
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(di.SaleEmployeeId)
                        && di.OrderDate >= Start
                        && di.OrderDate <= End
                        select di;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total)
                .DefaultIfEmpty(0)
                .Sum();
        }

        [Route(PermissionMobileRoute.TopDirectSaleEmployeeRevenue), HttpPost]
        public async Task<List<PermissionMobile_TopRevenueBySalesEmployeeDTO>> TopDirectSaleEmployeeRevenue([FromBody] PermissionMobile_FilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from transaction in DataContext.DirectSalesOrderTransaction
                        join di in DataContext.DirectSalesOrder on transaction.DirectSalesOrderId equals di.Id
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(di.SaleEmployeeId)
                        && transaction.OrderDate >= Start
                        && transaction.OrderDate <= End
                        group transaction by transaction.SalesEmployeeId into transGroup
                        select transGroup; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<PermissionMobile_TopRevenueBySalesEmployeeDTO> Result = new List<PermissionMobile_TopRevenueBySalesEmployeeDTO>();
            var transactionGroups = await query
                .Select(x => new DirectSalesOrderTransactionDAO
                {
                    SalesEmployeeId = x.Key,
                    Revenue = x.Sum(x => x.Revenue)
                })
                .ToListAsync();
            List<long> UserIds = transactionGroups
                .Select(x => x.SalesEmployeeId)
                .ToList();
            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => UserIds.Contains(x.Id))
                .ToListAsync();

            foreach (var groupItem in transactionGroups)
            {
                long SaleEmployeeId = groupItem.SalesEmployeeId;
                AppUserDAO SaleEmpolyee = AppUserDAOs
                    .Where(x => x.Id == SaleEmployeeId)
                    .FirstOrDefault();
                PermissionMobile_TopRevenueBySalesEmployeeDTO Item = new PermissionMobile_TopRevenueBySalesEmployeeDTO();
                Item.SaleEmployeeId = SaleEmployeeId;
                Item.SaleEmployeeName = SaleEmpolyee.DisplayName;
                Item.Revenue = groupItem.Revenue.HasValue ? groupItem.Revenue.Value : 0;
                Result.Add(Item);
            }
            Result = Result
                .Where(x => x.Revenue > 0)
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            return Result;
        } // top 5 doanh thu đơn trực tiếp theo nhân viên

        [Route(PermissionMobileRoute.TopDirecProductRevenue), HttpPost]
        public async Task<List<PermissionMobile_TopRevenueByItemDTO>> TopDirecProductRevenue([FromBody] PermissionMobile_TopRevenueByItemFilterDTO filter)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = new DateTime(Now.Year, Now.Month, 1).AddHours(0 - CurrentContext.TimeZone);
            DateTime End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
            (Start, End) = ConvertTime(filter.Time); // lấy ra startDate và EndDate theo filter time

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            var query = from transaction in DataContext.DirectSalesOrderTransaction
                        join di in DataContext.DirectSalesOrder on transaction.DirectSalesOrderId equals di.Id
                        where di.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(di.SaleEmployeeId)
                        && transaction.OrderDate >= Start
                        && transaction.OrderDate <= End
                        group transaction by transaction.ItemId into transGroup
                        select transGroup; // query tu transaction don hang gian tiep co trang thai phe duyet hoan thanh

            List<PermissionMobile_TopRevenueByItemDTO> Result = new List<PermissionMobile_TopRevenueByItemDTO>();
            var transactionGroups = await query
                .Select(x => new DirectSalesOrderTransactionDAO
                {
                    ItemId = x.Key,
                    Revenue = x.Sum(x => x.Revenue)
                })
                .ToListAsync();

            List<long> ItemIds = transactionGroups.Select(x => x.ItemId).ToList();
            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Id = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.Id | ItemSelect.Product
            });

            List<long> ProductIds = Items.Select(x => x.Product.Id).Distinct().ToList();
            List<Product> Products = await ProductService.List(new ProductFilter
            {
                Id = new IdFilter { In = ProductIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.Id | ProductSelect.Name
            });

            foreach (var groupItem in transactionGroups)
            {
                long ItemId = groupItem.ItemId;
                Item Item = Items
                    .Where(x => x.Id == ItemId)
                    .FirstOrDefault();
                if (Item != null)
                {
                    PermissionMobile_TopRevenueByItemDTO ResultItem = new PermissionMobile_TopRevenueByItemDTO();
                    ResultItem.ProductId = Item.Product.Id;
                    ResultItem.Revenue = groupItem.Revenue.HasValue ? groupItem.Revenue.Value : 0;
                    Result.Add(ResultItem);
                }
            }
            Result = Result
                .GroupBy(x => x.ProductId)
                .Select(y => new PermissionMobile_TopRevenueByItemDTO
                {
                    ProductId = y.Key,
                    Revenue = y.Sum(r => r.Revenue)
                })
                .ToList(); // group doanh thu theo id san pham
            foreach (var ResultItem in Result)
            {
                Product Product = Products.Where(x => x.Id == ResultItem.ProductId).FirstOrDefault();
                ResultItem.ProductName = Product.Name;
            }

            Result = Result
                .Where(x => x.Revenue > 0)
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            return Result;
        } // top 5 doanh thu đơn gián tiếp theo item

        [Route(PermissionMobileRoute.DirectRevenueGrowth), HttpPost]
        public async Task<PermissionMobile_RevenueGrowthDTO> DirectRevenueGrowth([FromBody] PermissionMobile_RevenueGrowthFilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(filter.Time);

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds
            var query = FilterDirectTransaction(AppUserIds, Start, End);

            if (filter.Time.Equal.HasValue == false
                || filter.Time.Equal.Value == THIS_MONTH)
            {
                var DirectSalesOrderTransactionDAOs = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO PermissionMobile_RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByMonths = new List<PermissionMobile_RevenueGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_RevenueGrowthByMonthDTO RevenueGrowthByMonth = new PermissionMobile_RevenueGrowthByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByMonths.Add(RevenueGrowthByMonth);
                }

                foreach (var RevenueGrowthByMonth in PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, (int)RevenueGrowthByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByMonth.Revenue = DirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return PermissionMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_MONTH)
            {
                var DirectSalesOrderTransactionDAOs = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO PermissionMobile_RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByMonths = new List<PermissionMobile_RevenueGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_RevenueGrowthByMonthDTO RevenueGrowthByMonth = new PermissionMobile_RevenueGrowthByMonthDTO
                    {
                        Day = i,
                        Revenue = 0
                    };
                    PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByMonths.Add(RevenueGrowthByMonth);
                }

                foreach (var RevenueGrowthByMonth in PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByMonths)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).AddMonths(-1).Month, (int)RevenueGrowthByMonth.Day).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddDays(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByMonth.Revenue = DirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return PermissionMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var DirectSalesOrderTransactionDAOs = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO PermissionMobile_RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByQuaters = new List<PermissionMobile_RevenueGrowthByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_RevenueGrowthByQuarterDTO RevenueGrowthByQuarter = new PermissionMobile_RevenueGrowthByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByQuaters.Add(RevenueGrowthByQuarter);
                }

                foreach (var RevenueGrowthByQuarter in PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByQuarter.Revenue = DirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return PermissionMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                var DirectSalesOrderTransactionDAOs = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO PermissionMobile_RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByQuaters = new List<PermissionMobile_RevenueGrowthByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_RevenueGrowthByQuarterDTO RevenueGrowthByQuarter = new PermissionMobile_RevenueGrowthByQuarterDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByQuaters.Add(RevenueGrowthByQuarter);
                }

                foreach (var RevenueGrowthByQuarter in PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByQuaters)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByQuarter.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByQuarter.Revenue = DirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return PermissionMobile_RevenueGrowthDTO;
            }
            else if (filter.Time.Equal.Value == YEAR)
            {
                var DirectSalesOrderTransactionDAOs = await query.ToListAsync();
                PermissionMobile_RevenueGrowthDTO PermissionMobile_RevenueGrowthDTO = new PermissionMobile_RevenueGrowthDTO();
                PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByYears = new List<PermissionMobile_RevenueGrowthByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    PermissionMobile_RevenueGrowthByYearDTO RevenueGrowthByYear = new PermissionMobile_RevenueGrowthByYearDTO
                    {
                        Month = i,
                        Revenue = 0
                    };
                    PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByYears.Add(RevenueGrowthByYear);
                }

                foreach (var RevenueGrowthByYear in PermissionMobile_RevenueGrowthDTO.DirectRevenueGrowthByYears)
                {
                    DateTime LocalStart = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (int)RevenueGrowthByYear.Month, 1).AddHours(0 - CurrentContext.TimeZone);
                    DateTime LocalEnd = LocalStart.AddHours(CurrentContext.TimeZone).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
                    RevenueGrowthByYear.Revenue = DirectSalesOrderTransactionDAOs.Where(x => LocalStart <= x.OrderDate && x.OrderDate <= LocalEnd)
                        .Where(x => x.Revenue.HasValue)
                        .Select(x => x.Revenue.Value)
                        .DefaultIfEmpty(0)
                        .Sum();
                }

                return PermissionMobile_RevenueGrowthDTO;
            }
            return new PermissionMobile_RevenueGrowthDTO();
        } // tăng trưởng doanh thu truc tiep

        [Route(PermissionMobileRoute.DirectQuantityGrowth), HttpPost]
        public async Task<PermissionMobile_QuantityGrowthDTO> DirectQuantityGrowth([FromBody] PermissionMobile_QuantityGrowthFilterDTO filter)
        {
            DateTime Now = StaticParams.DateTimeNow;
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            (Start, End) = ConvertTime(filter.Time);

            List<long> AppUserIds = await ListAppUserId(filter.EmployeeId); // lấy ra appUserIds

            if (filter.Time.Equal.HasValue == false
                || filter.Time.Equal.Value == THIS_MONTH)
            {
                var query = from i in FilterDirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Day into x
                            select new PermissionMobile_QuantityGrowthByMonthDTO
                            {
                                Day = x.Key,
                                DirectSalesOrderCounter = x.Count()
                            };

                var PermissionMobile_DirectSalesOrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO PermissionMobile_QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByMonths = new List<PermissionMobile_QuantityGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_QuantityGrowthByMonthDTO DirectSalesOrderQuantityGrowthByMonth = new PermissionMobile_QuantityGrowthByMonthDTO
                    {
                        Day = i,
                        DirectSalesOrderCounter = 0
                    };
                    PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByMonths.Add(DirectSalesOrderQuantityGrowthByMonth);
                }

                foreach (var DirectSalesOrderGrowthByMonth in PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByMonths)
                {
                    var data = PermissionMobile_DirectSalesOrderGrowthByMonthDTOs.Where(x => x.Day == DirectSalesOrderGrowthByMonth.Day).FirstOrDefault();
                    if (data != null)
                        DirectSalesOrderGrowthByMonth.DirectSalesOrderCounter = data.DirectSalesOrderCounter;
                }

                return PermissionMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_MONTH)
            {
                var query = from i in FilterDirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Day into x
                            select new PermissionMobile_QuantityGrowthByMonthDTO
                            {
                                Day = x.Key,
                                DirectSalesOrderCounter = x.Count()
                            };

                var PermissionMobile_DirectSalesOrderGrowthByMonthDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO PermissionMobile_QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByMonths = new List<PermissionMobile_QuantityGrowthByMonthDTO>();
                var number_of_day_in_this_month = DateTime.DaysInMonth(Start.AddHours(CurrentContext.TimeZone).Year, Start.AddHours(CurrentContext.TimeZone).Month);
                for (int i = 1; i < number_of_day_in_this_month + 1; i++)
                {
                    PermissionMobile_QuantityGrowthByMonthDTO DirectSalesOrderQuantityGrowthByMonth = new PermissionMobile_QuantityGrowthByMonthDTO
                    {
                        Day = i,
                        DirectSalesOrderCounter = 0
                    };
                    PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByMonths.Add(DirectSalesOrderQuantityGrowthByMonth);
                }

                foreach (var DirectSalesOrderGrowthByMonth in PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByMonths)
                {
                    var data = PermissionMobile_DirectSalesOrderGrowthByMonthDTOs.Where(x => x.Day == DirectSalesOrderGrowthByMonth.Day).FirstOrDefault();
                    if (data != null)
                        DirectSalesOrderGrowthByMonth.DirectSalesOrderCounter = data.DirectSalesOrderCounter;
                }

                return PermissionMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));

                var query = from i in FilterDirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Month into x
                            select new PermissionMobile_QuantityGrowthByQuarterDTO
                            {
                                Month = x.Key,
                                DirectSalesOrderCounter = x.Count()
                            };

                var PermissionMobile_DirectSalesOrderGrowthByQuarterDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO PermissionMobile_QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByQuaters = new List<PermissionMobile_QuantityGrowthByQuarterDTO>();
                int start = 3 * (this_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_QuantityGrowthByQuarterDTO DirectSalesOrderQuantityGrowthByQuarter = new PermissionMobile_QuantityGrowthByQuarterDTO
                    {
                        Month = i,
                        DirectSalesOrderCounter = 0
                    };
                    PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByQuaters.Add(DirectSalesOrderQuantityGrowthByQuarter);
                }

                foreach (var DirectSalesOrderGrowthByQuater in PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByQuaters)
                {
                    var data = PermissionMobile_DirectSalesOrderGrowthByQuarterDTOs.Where(x => x.Month == DirectSalesOrderGrowthByQuater.Month).FirstOrDefault();
                    if (data != null)
                        DirectSalesOrderGrowthByQuater.DirectSalesOrderCounter = data.DirectSalesOrderCounter;
                }

                return PermissionMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                var last_quarter = (this_quarter + 3) % 4;
                var query = from i in FilterDirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Month into x
                            select new PermissionMobile_QuantityGrowthByQuarterDTO
                            {
                                Month = x.Key,
                                DirectSalesOrderCounter = x.Count()
                            };

                var PermissionMobile_DirectSalesOrderGrowthByQuarterDTOs = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO PermissionMobile_QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByQuaters = new List<PermissionMobile_QuantityGrowthByQuarterDTO>();
                int start = 3 * (last_quarter - 1) + 1;
                int end = start + 3;
                for (int i = start; i < end; i++)
                {
                    PermissionMobile_QuantityGrowthByQuarterDTO DirectSalesOrderGrowthByQuarter = new PermissionMobile_QuantityGrowthByQuarterDTO
                    {
                        Month = i,
                        DirectSalesOrderCounter = 0
                    };
                    PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByQuaters.Add(DirectSalesOrderGrowthByQuarter);
                }

                foreach (var DirectSalesOrderGrowthByQuater in PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByQuaters)
                {
                    var data = PermissionMobile_DirectSalesOrderGrowthByQuarterDTOs.Where(x => x.Month == DirectSalesOrderGrowthByQuater.Month).FirstOrDefault();
                    if (data != null)
                        DirectSalesOrderGrowthByQuater.DirectSalesOrderCounter = data.DirectSalesOrderCounter;
                }

                return PermissionMobile_QuantityGrowthDTO;
            }
            else if (filter.Time.Equal.Value == YEAR)
            {
                var query = from i in FilterDirectSalesOrder(AppUserIds, Start, End)
                            group i by i.OrderDate.Month into x
                            select new PermissionMobile_QuantityGrowthByYearDTO
                            {
                                Month = x.Key,
                                DirectSalesOrderCounter = x.Count()
                            };

                var PermissionMobile_DirectSalesOrderGrowthByYearDTO = await query.ToListAsync();
                PermissionMobile_QuantityGrowthDTO PermissionMobile_QuantityGrowthDTO = new PermissionMobile_QuantityGrowthDTO();
                PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByYears = new List<PermissionMobile_QuantityGrowthByYearDTO>();
                for (int i = 1; i <= 12; i++)
                {
                    PermissionMobile_QuantityGrowthByYearDTO DirectSalesOrderGrowthByYear = new PermissionMobile_QuantityGrowthByYearDTO
                    {
                        Month = i,
                        DirectSalesOrderCounter = 0
                    };
                    PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByYears.Add(DirectSalesOrderGrowthByYear);
                }

                foreach (var DirectSalesOrderGrowthByYear in PermissionMobile_QuantityGrowthDTO.DirectSalesOrderQuantityGrowthByYears)
                {
                    var data = PermissionMobile_DirectSalesOrderGrowthByYearDTO.Where(x => x.Month == DirectSalesOrderGrowthByYear.Month).FirstOrDefault();
                    if (data != null)
                        DirectSalesOrderGrowthByYear.DirectSalesOrderCounter = data.DirectSalesOrderCounter;
                }

                return PermissionMobile_QuantityGrowthDTO;
            }
            return new PermissionMobile_QuantityGrowthDTO();
        } //  tăng trưởng số lượng đơn truc tiếp
        #endregion

        private Tuple<GenericEnum, GenericEnum, GenericEnum> ConvertDateTime(DateTime date)
        {
            GenericEnum monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum quarterName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum yearName = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == StaticParams.DateTimeNow.Year).FirstOrDefault();
            switch (date.Month)
            {
                case 1:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 2:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH02;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 3:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH03;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 4:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH04;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 5:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH05;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 6:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH06;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 7:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH07;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 8:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH08;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 9:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH09;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 10:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH10;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 11:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH11;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 12:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH12;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
            }
            return Tuple.Create(monthName, quarterName, yearName);
        }

        private Tuple<DateTime, DateTime> ConvertTime(IdFilter Time)
        {
            DateTime Start = LocalStartDay(CurrentContext);
            DateTime End = LocalEndDay(CurrentContext);
            DateTime Now = StaticParams.DateTimeNow;
            if (Time.Equal.HasValue == false)
            {
                Time.Equal = 0;
                Start = LocalStartDay(CurrentContext);
                End = LocalEndDay(CurrentContext);
            }
            else if (Time.Equal.Value == TODAY)
            {
                Start = LocalStartDay(CurrentContext);
                End = LocalEndDay(CurrentContext);
            }
            else if (Time.Equal.Value == THIS_WEEK)
            {
                int diff = (7 + (Now.AddHours(CurrentContext.TimeZone).DayOfWeek - DayOfWeek.Monday)) % 7;
                Start = LocalStartDay(CurrentContext).AddDays(-1 * diff);
                End = Start.AddDays(7).AddSeconds(-1);
            }
            else if (Time.Equal.Value == THIS_MONTH)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddMonths(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_MONTH)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddMonths(-1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, Now.AddHours(CurrentContext.TimeZone).Month, 1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == THIS_QUARTER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(3).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == LAST_QUATER)
            {
                var this_quarter = Convert.ToInt32(Math.Ceiling(Now.AddHours(CurrentContext.TimeZone).Month / 3m));
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddMonths(-3).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, (this_quarter - 1) * 3 + 1, 1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            else if (Time.Equal.Value == YEAR)
            {
                Start = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, 1, 1).AddHours(0 - CurrentContext.TimeZone);
                End = new DateTime(Now.AddHours(CurrentContext.TimeZone).Year, 1, 1).AddYears(1).AddSeconds(-1).AddHours(0 - CurrentContext.TimeZone);
            }
            return Tuple.Create(Start, End);
        } // lấy ra thời điểm bắt đầu và kết thúc của ngày, tuần, tháng, quý năm từ một thời điểm cho trước

        private decimal CalculatePercentage(decimal PlannedValue, decimal CurrentValue)
        {
            if (PlannedValue > 0) return CurrentValue / PlannedValue * 100;
            return 0;
        } // trả về phần trăm thực hiện kế hoạch

        private async Task<List<long>> ListAppUserId(IdFilter EmpployeeFilter)
        {
            List<long> AppUserIds = new List<long>();
            if (EmpployeeFilter.Equal != null)
            {
                AppUserIds.Add(EmpployeeFilter.Equal.Value);
            }
            else
            {
                List<long> Ids = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
                AppUserIds.AddRange(Ids);
            }
            return AppUserIds;
        } // lấy ra list AppUserId theo filter hoặc currentContext

        private IQueryable<IndirectSalesOrderTransactionDAO> FilterIndirectTransaction(List<long> AppUserIds, DateTime Start, DateTime End)
        {
            var query = from t in DataContext.IndirectSalesOrderTransaction
                        join i in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals i.Id
                        where i.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(i.SaleEmployeeId)
                        && t.OrderDate >= Start
                        && t.OrderDate <= End
                        select new IndirectSalesOrderTransactionDAO
                        {
                            OrderDate = t.OrderDate,
                            Revenue = t.Revenue
                        };
            return query;
        }

        private IQueryable<DirectSalesOrderTransactionDAO> FilterDirectTransaction(List<long> AppUserIds, DateTime Start, DateTime End)
        {
            var query = from t in DataContext.DirectSalesOrderTransaction
                        join i in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals i.Id
                        where i.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(i.SaleEmployeeId)
                        && t.OrderDate >= Start
                        && t.OrderDate <= End
                        select new DirectSalesOrderTransactionDAO
                        {
                            OrderDate = t.OrderDate,
                            Revenue = t.Revenue
                        };
            return query;
        }

        private IQueryable<IndirectSalesOrderDAO> FilterIndirectSalesOrder(List<long> AppUserIds, DateTime Start, DateTime End)
        {
            var query = from i in DataContext.IndirectSalesOrder
                        where i.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(i.SaleEmployeeId)
                        && i.OrderDate >= Start
                        && i.OrderDate <= End
                        select i;
            return query;
        }

        private IQueryable<DirectSalesOrderDAO> FilterDirectSalesOrder(List<long> AppUserIds, DateTime Start, DateTime End)
        {
            var query = from i in DataContext.DirectSalesOrder
                        where i.RequestStateId == RequestStateEnum.APPROVED.Id
                        && AppUserIds.Contains(i.SaleEmployeeId)
                        && i.OrderDate >= Start
                        && i.OrderDate <= End
                        select i;
            return query;
        }
    }
}
