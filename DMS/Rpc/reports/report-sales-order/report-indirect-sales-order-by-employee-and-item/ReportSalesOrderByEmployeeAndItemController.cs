﻿using Common;
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
using DMS.Services.MAppUser;
using System.IO;
using System.Dynamic;
using NGS.Templater;
using System.Diagnostics;

namespace DMS.Rpc.reports.report_sales_order.report_indirect_sales_order_by_employee_and_item
{
    public class ReportSalesOrderByEmployeeAndItemController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportSalesOrderByEmployeeAndItemController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IItemService ItemService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.ItemService = ItemService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportSalesOrderByEmployeeAndItemRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportSalesOrderByEmployeeAndItem_AppUserDTO>> FilterListAppUser([FromBody] ReportSalesOrderByEmployeeAndItem_AppUserFilterDTO ReportSalesOrderByEmployeeAndItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportSalesOrderByEmployeeAndItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportSalesOrderByEmployeeAndItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportSalesOrderByEmployeeAndItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportSalesOrderByEmployeeAndItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportSalesOrderByEmployeeAndItem_AppUserDTO> ReportSalesOrderByEmployeeAndItem_AppUserDTOs = AppUsers
                .Select(x => new ReportSalesOrderByEmployeeAndItem_AppUserDTO(x)).ToList();
            return ReportSalesOrderByEmployeeAndItem_AppUserDTOs;
        }


        [Route(ReportSalesOrderByEmployeeAndItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportSalesOrderByEmployeeAndItem_OrganizationDTO>> FilterListOrganization([FromBody] ReportSalesOrderByEmployeeAndItem_OrganizationFilterDTO ReportSalesOrderByEmployeeAndItem_OrganizationFilterDTO)
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
            OrganizationFilter.Id = ReportSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ReportSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ReportSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Name;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportSalesOrderByEmployeeAndItem_OrganizationDTO> ReportSalesOrderByEmployeeAndItem_OrganizationDTOs = Organizations
                .Select(x => new ReportSalesOrderByEmployeeAndItem_OrganizationDTO(x)).ToList();
            return ReportSalesOrderByEmployeeAndItem_OrganizationDTOs;
        }
        #endregion

        [Route(ReportSalesOrderByEmployeeAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            var SaleEmployeeId = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        OrganizationIds.Contains(au.OrganizationId)
                        select au;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportSalesOrderByEmployeeAndItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>>> List([FromBody] ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>();

            DateTime Start = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? SaleEmployeeId = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        OrganizationIds.Contains(au.OrganizationId)
                        select au;

            var AppUserIds = await query.Select(x => x.Id).Distinct().ToListAsync();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Id = new IdFilter { In = AppUserIds },
                OrganizationId = new IdFilter { In = OrganizationIds },
                Skip = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Skip,
                Take = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Take,
                OrderBy = AppUserOrder.DisplayName,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<string> OrganizationNames = AppUsers.OrderBy(x => x.OrganizationId).Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs = OrganizationNames
                .Select(on => new ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO
                {
                    OrganizationName = on,
                }).ToList();
            foreach (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees = AppUsers
                    .Where(x => x.Organization.Name == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.OrganizationName)
                    .Select(x => new ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.Id,
                        Username = x.Username,
                        DisplayName = x.DisplayName,
                    })
                    .ToList();
            }
            ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.Where(x => x.SaleEmployees.Any()).ToList();

            List<long> SaleEmployeeIds = AppUsers.Select(s => s.Id).ToList();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) && Start <= x.OrderDate && x.OrderDate <= End)
                .ToListAsync();
            List<long> IndirectSalesOrderIds = IndirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                .Where(x => SaleEmployeeIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End)
                .Select(x => new IndirectSalesOrderContentDAO
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    PrimaryPrice = x.PrimaryPrice,
                    RequestedQuantity = x.RequestedQuantity,
                    SalePrice = x.SalePrice,
                    TaxAmount = x.TaxAmount,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    IndirectSalesOrder = x.IndirectSalesOrder == null ? null : new IndirectSalesOrderDAO
                    {
                        BuyerStoreId = x.IndirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            List<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionDAOs = await DataContext.IndirectSalesOrderPromotion
                .Where(x => SaleEmployeeIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End)
                .Select(x => new IndirectSalesOrderPromotionDAO
                {
                    Id = x.Id,
                    RequestedQuantity = x.RequestedQuantity,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    IndirectSalesOrder = x.IndirectSalesOrder == null ? null : new IndirectSalesOrderDAO
                    {
                        BuyerStoreId = x.IndirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            List<long> ItemIds = new List<long>();
            ItemIds.AddRange(IndirectSalesOrderContentDAOs.Select(x => x.ItemId));
            ItemIds.AddRange(IndirectSalesOrderPromotionDAOs.Select(x => x.ItemId));
            ItemIds = ItemIds.Distinct().ToList();

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name | ItemSelect.SalePrice
            });

            List<UnitOfMeasureDAO> UnitOfMeasureDAOs = await DataContext.UnitOfMeasure.ToListAsync();
            // khởi tạo khung dữ liệu
            foreach (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                    var IndirectSalesOrders = IndirectSalesOrderDAOs.Where(x => x.SaleEmployeeId == SaleEmployee.SaleEmployeeId).ToList();
                    var SalesOrderIds = IndirectSalesOrders.Select(x => x.Id).ToList();

                    SaleEmployee.Items = new List<ReportSalesOrderByEmployeeAndItem_ItemDTO>();
                    foreach (IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO in IndirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderContentDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByEmployeeAndItem_ItemDTO ReportSalesOrderByEmployeeAndItem_ItemDTO = SaleEmployee.Items.Where(i => i.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                            if (ReportSalesOrderByEmployeeAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportSalesOrderByEmployeeAndItem_ItemDTO = new ReportSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    StoreIds = new HashSet<long>(),
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                SaleEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = IndirectSalesOrderContentDAO.IndirectSalesOrder.BuyerStoreId;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderContentDAO.IndirectSalesOrderId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.SalePriceAverage += (IndirectSalesOrderContentDAO.SalePrice * IndirectSalesOrderContentDAO.RequestedQuantity);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.Revenue += (IndirectSalesOrderContentDAO.Amount - (IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0) + (IndirectSalesOrderContentDAO.TaxAmount ?? 0));
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.Discount += ((IndirectSalesOrderContentDAO.DiscountAmount ?? 0) + (IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0));
                        }
                    }

                    foreach (var item in SaleEmployee.Items)
                    {
                        item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
                    }

                    foreach (IndirectSalesOrderPromotionDAO IndirectSalesOrderPromotionDAO in IndirectSalesOrderPromotionDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByEmployeeAndItem_ItemDTO ReportSalesOrderByEmployeeAndItem_ItemDTO = SaleEmployee.Items.Where(i => i.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                            if (ReportSalesOrderByEmployeeAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportSalesOrderByEmployeeAndItem_ItemDTO = new ReportSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    StoreIds = new HashSet<long>(),
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                SaleEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = IndirectSalesOrderPromotionDAO.IndirectSalesOrder.BuyerStoreId;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.PromotionStock += IndirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            }

            foreach (var ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees.Where(x => x.Items.Any()).ToList();
            }

            //làm tròn số
            foreach (var ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    foreach (var item in SaleEmployee.Items)
                    {
                        item.Discount = Math.Round(item.Discount, 0);
                        item.Revenue = Math.Round(item.Revenue, 0);
                        item.SalePriceAverage = Math.Round(item.SalePriceAverage, 0);
                    }
                }
            }

            return ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.Where(x => x.SaleEmployees.Any()).ToList();
        }

        [Route(ReportSalesOrderByEmployeeAndItemRoute.Total), HttpPost]
        public async Task<ReportSalesOrderByEmployeeAndItem_TotalDTO> Total([FromBody] ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new ReportSalesOrderByEmployeeAndItem_TotalDTO();

            DateTime Start = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportSalesOrderByEmployeeAndItem_TotalDTO();

            long? SaleEmployeeId = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;

            ReportSalesOrderByEmployeeAndItem_TotalDTO ReportSalesOrderByEmployeeAndItem_TotalDTO = new ReportSalesOrderByEmployeeAndItem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        OrganizationIds.Contains(au.OrganizationId)
                        select au;

            var AppUserIds = await query.Select(x => x.Id).Distinct().ToListAsync();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Id = new IdFilter { In = AppUserIds },
                OrganizationId = new IdFilter { In = OrganizationIds },
                Skip = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Skip,
                Take = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Take,
                OrderBy = AppUserOrder.DisplayName,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<string> OrganizationNames = AppUsers.Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO> ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs = OrganizationNames.Select(on => new ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees = AppUsers
                    .Where(x => x.Organization.Name == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.OrganizationName)
                    .Select(x => new ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.Id,
                        Username = x.Username,
                        DisplayName = x.DisplayName,
                    })
                    .ToList();
            }

            List<long> SaleEmployeeIds = AppUsers.Select(s => s.Id).ToList();

            var IndirectSalesOrderContentQuery = DataContext.IndirectSalesOrderContent
                .Where(x => SaleEmployeeIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End);
            var IndirectSalesOrderPromotionQuery = DataContext.IndirectSalesOrderPromotion
                .Where(x => SaleEmployeeIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End);

            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalSalesStock = IndirectSalesOrderContentQuery.Select(x => x.RequestedQuantity).Sum();

            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalPromotionStock = IndirectSalesOrderPromotionQuery.Select(x => x.RequestedQuantity).Sum();

            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalRevenue = Math.Round(IndirectSalesOrderContentQuery.Select(x => x.Amount).Sum()
                - IndirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + IndirectSalesOrderContentQuery.Where(x => x.TaxAmount.HasValue).Select(x => x.TaxAmount.Value).Sum(), 0);

            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalDiscount = Math.Round(IndirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + IndirectSalesOrderContentQuery.Where(x => x.DiscountAmount.HasValue).Select(x => x.DiscountAmount.Value).Sum(), 0);

            return ReportSalesOrderByEmployeeAndItem_TotalDTO;
        }

        [Route(ReportSalesOrderByEmployeeAndItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Skip = 0;
            ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Take = int.MaxValue;
            List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO> ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs = (await List(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO)).Value;

            ReportSalesOrderByEmployeeAndItem_TotalDTO ReportSalesOrderByEmployeeAndItem_TotalDTO = await Total(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO);
            long stt = 1;
            foreach (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    SaleEmployee.STT = stt;
                    stt++;
                }
            }


            string path = "Templates/Report_Sales_Order_By_Employee_And_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByEmployeeAndItems = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs;
            Data.Total = ReportSalesOrderByEmployeeAndItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportSalesOrderByEmployeeAndItem.xlsx");
        }
    }
}