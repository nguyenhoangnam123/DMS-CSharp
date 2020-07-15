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

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_employee_and_item
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
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

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
                        au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value)
                        select au;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportSalesOrderByEmployeeAndItemRoute.List), HttpPost]
        public async Task<List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>> List([FromBody] ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>();

            DateTime Start = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<AppUser> AppUsers = await AppUserService.List(new AppUserFilter
            {
                Id = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.AppUserId,
                OrganizationId = new IdFilter { In = OrganizationIds },
                Skip = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Skip,
                Take = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Take,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization
            });

            List<string> OrganizationNames = AppUsers.Select(s => s.Organization.Name).Distinct().ToList();
            OrganizationNames = OrganizationNames.OrderBy(x => x).ToList();
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
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .ToListAsync();
            List<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionDAOs = await DataContext.IndirectSalesOrderPromotion
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
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
                    var SalesOrderIds = IndirectSalesOrderDAOs.Where(x => x.SaleEmployeeId == SaleEmployee.SaleEmployeeId).Select(x => x.Id).ToList();
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
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                SaleEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderContentDAO.IndirectSalesOrderId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.SalePriceAverage += IndirectSalesOrderContentDAO.SalePrice * IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.Revenue += IndirectSalesOrderContentDAO.Amount;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.Discount += (IndirectSalesOrderContentDAO.DiscountAmount ?? 0 + IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0);
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
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                SaleEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
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
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            ReportSalesOrderByEmployeeAndItem_TotalDTO ReportSalesOrderByEmployeeAndItem_TotalDTO = new ReportSalesOrderByEmployeeAndItem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<AppUser> AppUsers = await AppUserService.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.AppUserId,
                OrganizationId = new IdFilter { In = OrganizationIds },
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization
            });

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
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) && Start <= x.OrderDate && x.OrderDate <= End)
                .ToListAsync();
            List<long> IndirectSalesOrderIds = IndirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .ToListAsync();
            List<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionDAOs = await DataContext.IndirectSalesOrderPromotion
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
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

            // khởi tạo khung dữ liệu
            foreach (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    var SalesOrderIds = IndirectSalesOrderDAOs.Where(x => x.SaleEmployeeId == SaleEmployee.SaleEmployeeId).Select(x => x.Id).ToList();
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
                                ReportSalesOrderByEmployeeAndItem_ItemDTO = new ReportSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Code = item.Code,
                                    Name = item.Name,
                                };
                                SaleEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.Revenue += IndirectSalesOrderContentDAO.Amount;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.Discount += (IndirectSalesOrderContentDAO.DiscountAmount ?? 0 + IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0);
                        }
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
                                ReportSalesOrderByEmployeeAndItem_ItemDTO = new ReportSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Code = item.Code,
                                    Name = item.Name,
                                };
                                SaleEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.PromotionStock += IndirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            }
            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalDiscount = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.SelectMany(x => x.SaleEmployees)
                .SelectMany(x => x.Items).Sum(x => x.Discount);
            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalRevenue = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.SelectMany(x => x.SaleEmployees)
                .SelectMany(x => x.Items).Sum(x => x.Revenue);
            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalPromotionStock = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.SelectMany(x => x.SaleEmployees)
                .SelectMany(x => x.Items).Sum(x => x.PromotionStock);
            ReportSalesOrderByEmployeeAndItem_TotalDTO.TotalSalesStock = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.SelectMany(x => x.SaleEmployees)
                .SelectMany(x => x.Items).Sum(x => x.SaleStock);
            return ReportSalesOrderByEmployeeAndItem_TotalDTO;
        }
    }
}