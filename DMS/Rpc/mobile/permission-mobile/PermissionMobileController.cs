using DMS.Common;
using DMS.Enums;
using DMS.Helpers;
using DMS.Models;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public partial class PermissionMobileController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private ICurrentContext CurrentContext;
        private DataContext DataContext;
        public PermissionMobileController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext,
            DataContext DataContext)
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
            this.DataContext = DataContext;
        }
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

                var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
                x.CheckOutAt.HasValue && x.CheckOutAt.Value >= Start && x.CheckOutAt.Value <= End)
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
                    Id= x.Id,
                    EmployeeId= x.EmployeeId,
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

                        if(subResults.Count > 0)
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

        private decimal CalculatePercentage(decimal PlannedValue, decimal CurrentValue)
        {
            if (PlannedValue > 0) return CurrentValue / PlannedValue * 100;
            return 0;
        } // trả về phần trăm thực hiện kế hoạch
    }
}
