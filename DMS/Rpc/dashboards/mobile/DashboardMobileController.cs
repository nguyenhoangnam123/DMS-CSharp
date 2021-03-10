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

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobileController : SimpleController
    {
        private const long MONTH = 1;
        private const long QUARTER = 2;
        private const long YEAR = 3;

        private DataContext DataContext;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        private IOrganizationService OrganizationService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IStoreCheckingService StoreCheckingService;
        public DashboardMobileController(
            DataContext DataContext,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext,
            IOrganizationService OrganizationService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IStoreCheckingService StoreCheckingService)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
            this.OrganizationService = OrganizationService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.StoreCheckingService = StoreCheckingService;
        }

        [Route(DashboardMobileRoute.SingleListPeriod), HttpPost]
        public async Task<List<GenericEnum>> SingleListPeriod()
        {
            List<GenericEnum> Periods = new List<GenericEnum>
            {
                new GenericEnum
                {
                    Id = MONTH,
                    Code = "MONTH",
                    Name = "Tháng"
                },
                new GenericEnum
                {
                    Id = QUARTER,
                    Code = "QUARTER",
                    Name = "Quý"
                },
                new GenericEnum
                {
                    Id = YEAR,
                    Code = "YEAR",
                    Name = "Năm"
                },
            };
            return Periods;
        }

        [Route(DashboardMobileRoute.CountIndirectSalesOrder), HttpPost]
        public async Task<long> CountIndirectSalesOrder()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId != RequestStateEnum.NEW.Id
                        select i;

            return await query.CountAsync();
        }

        [Route(DashboardMobileRoute.CountStoreChecking), HttpPost]
        public async Task<long> CountStoreChecking()
        {
            var query = from sc in DataContext.StoreChecking
                        where sc.SaleEmployeeId == CurrentContext.UserId &&
                        sc.CheckOutAt.HasValue
                        select sc;

            var count = await query.CountAsync();
            return count;
        }

        [Route(DashboardMobileRoute.Revenue), HttpPost]
        public async Task<decimal> Revenue()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select i;

            var results = await query.ToListAsync();
            return results.Select(x => x.Total).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardMobileRoute.SalesQuantity), HttpPost]
        public async Task<long> SalesQuantity()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var query = from i in DataContext.IndirectSalesOrder
                        join ic in DataContext.IndirectSalesOrderContent on i.Id equals ic.IndirectSalesOrderId
                        where i.SaleEmployeeId == CurrentContext.UserId &&
                        i.RequestStateId == RequestStateEnum.APPROVED.Id
                        select ic;

            var results = await query.ToListAsync();
            return results.Select(x => x.RequestedQuantity).DefaultIfEmpty(0).Sum();
        }

        [Route(DashboardMobileRoute.KpiGeneral), HttpPost]
        public async Task<List<DashboardMobile_KpiGeneralCriterialDTO>> KpiGeneral([FromBody] DashboardMobile_KpiGeneralCriterialFilterDTO DashboardMobile_KpiGeneralCriterialFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long SaleEmployeeId = CurrentContext.UserId;
            long KpiPeriodId = DashboardMobile_KpiGeneralCriterialFilterDTO.Period?.Equal ?? 100 + StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Month;
            long KpiYearId = DashboardMobile_KpiGeneralCriterialFilterDTO.Period?.Equal ?? StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;

            if(DashboardMobile_KpiGeneralCriterialFilterDTO.Period.HasValue == false)
            {
                KpiPeriodId = 100 + StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Month;
                KpiYearId = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;
            }
            else if(DashboardMobile_KpiGeneralCriterialFilterDTO.Period.Equal == MONTH) 
            {
                KpiPeriodId = 100 + StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Month;
                KpiYearId = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;
            }
            else if (DashboardMobile_KpiGeneralCriterialFilterDTO.Period.Equal == QUARTER)
            {
                var Month = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Month;
                if(Month < 4)
                {
                    KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER01.Id;
                }
                else if( Month < 7)
                {
                    KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER02.Id;
                }
                else if(Month < 10)
                {
                    KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER03.Id;
                }
                else
                {
                    KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER04.Id;
                }
                KpiYearId = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;
            }
            else
            {
                KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH01.Id;
                KpiYearId = StaticParams.DateTimeNow.AddHours(CurrentContext.TimeZone).Year;
            }

            DateTime StartDate, EndDate;
            (StartDate, EndDate) = DateTimeConvert(KpiPeriodId, KpiYearId);
            if(DashboardMobile_KpiGeneralCriterialFilterDTO.Period.Equal == YEAR)
            {
                EndDate = EndDate.AddYears(1).AddSeconds(-1);
            }

            var query_detail = from kcm in DataContext.KpiGeneralContentKpiPeriodMapping
                               join kc in DataContext.KpiGeneralContent on kcm.KpiGeneralContentId equals kc.Id
                               join k in DataContext.KpiGeneral on kc.KpiGeneralId equals k.Id
                               where k.EmployeeId == SaleEmployeeId &&
                               k.KpiYearId == KpiYearId &&
                               (DashboardMobile_KpiGeneralCriterialFilterDTO.Period.Equal == YEAR || kcm.KpiPeriodId == KpiPeriodId) &&
                               kcm.Value.HasValue &&
                               k.StatusId == StatusEnum.ACTIVE.Id &&
                               k.DeletedAt == null
                               select new
                               {
                                   SaleEmployeeId = k.EmployeeId,
                                   KpiCriteriaGeneralId = kc.KpiCriteriaGeneralId,
                                   Value = kcm.Value,
                               };
            List<DashboardMobile_KpiGeneralCriterialDTO> DashboardMobile_KpiGeneralCriterialDTOs = (await query_detail
                .Distinct()
                .ToListAsync())
                .Select(x => new DashboardMobile_KpiGeneralCriterialDTO
                {
                    KpiCriterialId = x.KpiCriteriaGeneralId,
                    Plan = x.Value,
                }).ToList();

            var IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
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

            //var DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
            //   .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
            //   x.OrderDate >= StartDate && x.OrderDate <= EndDate &&
            //   x.RequestStateId == RequestStateEnum.APPROVED.Id)
            //   .Select(x => new DirectSalesOrderDAO
            //   {
            //       Id = x.Id,
            //       Total = x.Total,
            //       SaleEmployeeId = x.SaleEmployeeId,
            //       OrderDate = x.OrderDate,
            //       DirectSalesOrderContents = x.DirectSalesOrderContents.Select(c => new DirectSalesOrderContentDAO
            //       {
            //           RequestedQuantity = c.RequestedQuantity,
            //           ItemId = c.ItemId
            //       }).ToList(),
            //       DirectSalesOrderPromotions = x.DirectSalesOrderPromotions.Select(x => new DirectSalesOrderPromotionDAO
            //       {
            //           RequestedQuantity = x.RequestedQuantity,
            //           ItemId = x.ItemId
            //       }).ToList()
            //   })
            //   .ToListAsync();
            //var DirectSalesOrderContents = DirectSalesOrderDAOs
            //            .SelectMany(x => x.DirectSalesOrderContents)
            //            .ToList();
            //var DirectSalesOrderPromotions = DirectSalesOrderDAOs
            //    .SelectMany(x => x.DirectSalesOrderPromotions)
            //    .ToList();

            var StoreCheckingDAOs = await DataContext.StoreChecking
                .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                x.CheckOutAt.HasValue && x.CheckOutAt.Value >= StartDate && x.CheckOutAt.Value <= EndDate)
                .Select(x => new StoreCheckingDAO
                {
                    SaleEmployeeId = x.SaleEmployeeId,
                    Id = x.Id,
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt,
                    StoreId = x.StoreId
                })
                .ToListAsync();

            var StoreScoutingDAOs = await DataContext.StoreScouting
                .Where(x => x.CreatorId == SaleEmployeeId &&
                x.CreatedAt >= StartDate && x.CreatedAt <= EndDate)
                .Select(x => new StoreScoutingDAO
                {
                    CreatorId = x.CreatorId,
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Stores = x.Stores.Select(c => new StoreDAO
                    {
                        StoreScoutingId = c.StoreScoutingId,
                        StoreType = c.StoreType == null ? null : new StoreTypeDAO
                        {
                            Code = c.StoreType.Code
                        }
                    }).ToList()
                })
                .ToListAsync();

            var ProblemDAOs = await DataContext.Problem
                 .Where(x => x.CreatorId == SaleEmployeeId &&
                x.NoteAt >= StartDate && x.NoteAt <= EndDate)
                 .Select(x => new ProblemDAO
                 {
                     Id = x.Id
                 }).ToListAsync();
            var StoreImages = await DataContext.StoreImage
                .Where(x => x.SaleEmployeeId == SaleEmployeeId &&
                x.ShootingAt >= StartDate && x.ShootingAt <= EndDate)
                .ToListAsync();
            foreach (var DashboardMobile_KpiGeneralCriterialDTO in DashboardMobile_KpiGeneralCriterialDTOs)
            {
                #region các chỉ tiêu tạm ẩn
                //#region Số đơn hàng gián tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs.Count();
                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Name;
                //}
                //#endregion

                //#region Tổng sản lượng theo đơn gián tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs
                //        .SelectMany(c => c.IndirectSalesOrderContents)
                //        .Select(q => q.RequestedQuantity)
                //        .DefaultIfEmpty(0).Sum();
                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Name;
                //}
                //#endregion

                //#region SKU/Đơn hàng gián tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    var TotalIndirectOrders = IndirectSalesOrderDAOs.Count();
                //    HashSet<long> SKUIndirectItems = new HashSet<long>();
                //    foreach (var content in IndirectSalesOrderContents)
                //    {
                //        SKUIndirectItems.Add(content.ItemId);
                //    }
                //    foreach (var content in IndirectSalesOrderPromotions)
                //    {
                //        SKUIndirectItems.Add(content.ItemId);
                //    }
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = TotalIndirectOrders == 0 ? 
                //        0 : SKUIndirectItems.Count();

                //    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs
                //        .Sum(iso => iso.Total);
                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Name;
                //}
                //#endregion

                //#region Số đơn hàng trực tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = DirectSalesOrderDAOs.Count();
                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Name;
                //}
                //#endregion

                //#region Tổng sản lượng theo đơn trực tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = DirectSalesOrderDAOs
                //        .SelectMany(c => c.DirectSalesOrderContents)
                //        .Select(q => q.RequestedQuantity)
                //        .DefaultIfEmpty(0).Sum();
                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Name;
                //}
                //#endregion

                //#region Doanh thu theo đơn hàng trực tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = DirectSalesOrderDAOs
                //        .Sum(iso => iso.Total);
                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Name;
                //}
                //#endregion

                //#region SKU/Đơn hàng trực tiếp
                //if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                //{
                //    var TotalDirectOrders = DirectSalesOrderDAOs.Count();
                //    HashSet<long> SKUDirectItems = new HashSet<long>();
                //    foreach (var content in DirectSalesOrderContents)
                //    {
                //        SKUDirectItems.Add(content.ItemId);
                //    }
                //    foreach (var content in DirectSalesOrderPromotions)
                //    {
                //        SKUDirectItems.Add(content.ItemId);
                //    }
                //    DashboardMobile_KpiGeneralCriterialDTO.Value = TotalDirectOrders == 0 ?
                //        0 : SKUDirectItems.Count();

                //    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Name;
                //}
                //#endregion
                #endregion

                #region Tổng doanh thu đơn hàng
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs
                        .Sum(iso => iso.Total);
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Name;
                }
                #endregion

                #region Số đại lý ghé thăm
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.STORE_VISITED.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = StoreCheckingDAOs.Select(x => x.StoreId).Distinct().Count() == 0 ? 
                        0 : StoreCheckingDAOs.Select(x => x.StoreId).Distinct().Count();
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.STORE_VISITED.Name;
                }
                #endregion

                #region Tổng số đại lý mở mới
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = StoreScoutingDAOs
                        .SelectMany(sc => sc.Stores)
                        .Where(x => x.StoreScoutingId.HasValue)
                        .Select(z => z.StoreScoutingId.Value)
                        .Count() == 0 ?
                        0 : StoreScoutingDAOs.SelectMany(sc => sc.Stores)
                        .Where(x => x.StoreScoutingId.HasValue)
                        .Select(z => z.StoreScoutingId.Value)
                        .Count();
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Name;
                }
                #endregion

                #region Tổng số lượt ghé thăm
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = StoreCheckingDAOs
                        .Count() == 0 ?
                        0 : StoreScoutingDAOs.Count();
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Name;
                }
                #endregion

                #region Doanh thu C2 Trọng điểm
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs
                        .Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2TD)
                        .Sum(iso => iso.Total);
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Name;
                }
                #endregion

                #region Doanh thu C2 Siêu lớn
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs
                        .Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2SL)
                        .Sum(iso => iso.Total);
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Name;
                }
                #endregion

                #region Doanh thu C2
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = IndirectSalesOrderDAOs
                        .Where(x => x.BuyerStore.StoreType.Code == StaticParams.C2)
                        .Sum(iso => iso.Total);
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Name;
                }
                #endregion

                #region Số đại lý trọng điểm mở mới
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.NEW_STORE_C2_CREATED.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = StoreScoutingDAOs
                        .SelectMany(sc => sc.Stores)
                        .Where(x => x.StoreScoutingId.HasValue)
                        .Where(x => x.StoreType.Code == StaticParams.C2TD)
                        .Select(z => z.StoreScoutingId.Value)
                        .Count() == 0 ?
                        0 : StoreScoutingDAOs.SelectMany(sc => sc.Stores)
                        .Where(x => x.StoreScoutingId.HasValue)
                        .Where(x => x.StoreType.Code == StaticParams.C2TD)
                        .Select(z => z.StoreScoutingId.Value)
                        .Count();
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.NEW_STORE_C2_CREATED.Name;
                }
                #endregion

                #region Số thông tin phản ánh
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_PROBLEM.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = ProblemDAOs
                        .Count();
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_PROBLEM.Name;
                }
                #endregion

                #region Số hình ảnh chụp
                if (DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialId == KpiCriteriaGeneralEnum.TOTAL_IMAGE.Id || DashboardMobile_KpiGeneralCriterialDTO.Plan.HasValue)
                {
                    DashboardMobile_KpiGeneralCriterialDTO.Value = StoreImages
                        .Count();
                    DashboardMobile_KpiGeneralCriterialDTO.KpiCriterialName = KpiCriteriaGeneralEnum.TOTAL_IMAGE.Name;
                }
                #endregion
            }
            return DashboardMobile_KpiGeneralCriterialDTOs;
        }

        private Tuple<DateTime, DateTime> DateTimeConvert(long KpiPeriodId, long KpiYearId)
        {
            DateTime startDate = StaticParams.DateTimeNow;
            DateTime endDate = StaticParams.DateTimeNow;
            if (KpiPeriodId <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
            {
                startDate = new DateTime((int)KpiYearId, (int)(KpiPeriodId % 100), 1);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
            }
            else
            {
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER01.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 1, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER02.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 4, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER03.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 7, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_QUATER04.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 10, 1);
                    endDate = startDate.AddMonths(3).AddSeconds(-1);
                }
                if (KpiPeriodId == Enums.KpiPeriodEnum.PERIOD_YEAR01.Id)
                {
                    startDate = new DateTime((int)KpiYearId, 1, 1);
                    endDate = startDate.AddYears(1).AddSeconds(-1);
                }
            }

            return Tuple.Create(startDate, endDate);
        }
    }
}
