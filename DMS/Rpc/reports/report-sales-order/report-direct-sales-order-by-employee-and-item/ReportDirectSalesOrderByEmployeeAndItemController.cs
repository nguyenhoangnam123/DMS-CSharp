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
using DMS.Services.MAppUser;
using System.IO;
using System.Dynamic;
using NGS.Templater;
using System.Diagnostics;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_by_employee_and_item
{
    public class ReportDirectSalesOrderByEmployeeAndItemController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportDirectSalesOrderByEmployeeAndItemController(
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
        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportDirectSalesOrderByEmployeeAndItem_AppUserDTO>> FilterListAppUser([FromBody] ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportDirectSalesOrderByEmployeeAndItem_AppUserDTO> ReportDirectSalesOrderByEmployeeAndItem_AppUserDTOs = AppUsers
                .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_AppUserDTO(x)).ToList();
            return ReportDirectSalesOrderByEmployeeAndItem_AppUserDTOs;
        }


        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTO>> FilterListOrganization([FromBody] ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO)
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
            OrganizationFilter.Id = ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Name;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTO> ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTOs = Organizations
                .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTO(x)).ToList();
            return ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTOs;
        }
        #endregion

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            var SaleEmployeeId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.DirectSalesOrder
                        join au in DataContext.AppUser on i.SaleEmployeeId equals au.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                        OrganizationIds.Contains(au.OrganizationId)
                        select au;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>>> List([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>();

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? SaleEmployeeId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.DirectSalesOrder
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
                Skip = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Skip,
                Take = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Take,
                OrderBy = AppUserOrder.DisplayName,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<string> OrganizationNames = AppUsers.OrderBy(x => x.OrganizationId).Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = OrganizationNames
                .Select(on => new ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO
                {
                    OrganizationName = on,
                }).ToList();
            foreach (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees = AppUsers
                    .Where(x => x.Organization.Name == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.OrganizationName)
                    .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.Id,
                        Username = x.Username,
                        DisplayName = x.DisplayName,
                    })
                    .ToList();
            }
            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs.Where(x => x.SaleEmployees.Any()).ToList();

            List<long> SaleEmployeeIds = AppUsers.Select(s => s.Id).ToList();
            List<DirectSalesOrderDAO> DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
                .Where(x => SaleEmployeeIds.Contains(x.SaleEmployeeId) && Start <= x.OrderDate && x.OrderDate <= End)
                .ToListAsync();
            List<long> DirectSalesOrderIds = DirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<DirectSalesOrderContentDAO> DirectSalesOrderContentDAOs = await DataContext.DirectSalesOrderContent
                .Where(x => SaleEmployeeIds.Contains(x.DirectSalesOrder.SaleEmployeeId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End)
                .Select(x => new DirectSalesOrderContentDAO
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    PrimaryPrice = x.PrimaryPrice,
                    RequestedQuantity = x.RequestedQuantity,
                    SalePrice = x.SalePrice,
                    TaxAmount = x.TaxAmount,
                    DirectSalesOrderId = x.DirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    DirectSalesOrder = x.DirectSalesOrder == null ? null : new DirectSalesOrderDAO
                    {
                        BuyerStoreId = x.DirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            List<DirectSalesOrderPromotionDAO> DirectSalesOrderPromotionDAOs = await DataContext.DirectSalesOrderPromotion
                .Where(x => SaleEmployeeIds.Contains(x.DirectSalesOrder.SaleEmployeeId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End)
                .Select(x => new DirectSalesOrderPromotionDAO
                {
                    Id = x.Id,
                    RequestedQuantity = x.RequestedQuantity,
                    DirectSalesOrderId = x.DirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    DirectSalesOrder = x.DirectSalesOrder == null ? null : new DirectSalesOrderDAO
                    {
                        BuyerStoreId = x.DirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            List<long> ItemIds = new List<long>();
            ItemIds.AddRange(DirectSalesOrderContentDAOs.Select(x => x.ItemId));
            ItemIds.AddRange(DirectSalesOrderPromotionDAOs.Select(x => x.ItemId));
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
            foreach (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                    var DirectSalesOrders = DirectSalesOrderDAOs.Where(x => x.SaleEmployeeId == SaleEmployee.SaleEmployeeId).ToList();
                    var SalesOrderIds = DirectSalesOrders.Select(x => x.Id).ToList();

                    SaleEmployee.Items = new List<ReportDirectSalesOrderByEmployeeAndItem_ItemDTO>();
                    foreach (DirectSalesOrderContentDAO DirectSalesOrderContentDAO in DirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(DirectSalesOrderContentDAO.DirectSalesOrderId))
                        {
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = SaleEmployee.Items.Where(i => i.Id == DirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                            if (ReportDirectSalesOrderByEmployeeAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == DirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == DirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = new ReportDirectSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    StoreIds = new HashSet<long>(),
                                    DirectSalesOrderIds = new HashSet<long>(),
                                };
                                SaleEmployee.Items.Add(ReportDirectSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = DirectSalesOrderContentDAO.DirectSalesOrder.BuyerStoreId;
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.DirectSalesOrderIds.Add(DirectSalesOrderContentDAO.DirectSalesOrderId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.SaleStock += DirectSalesOrderContentDAO.RequestedQuantity;
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.SalePriceAverage += (DirectSalesOrderContentDAO.SalePrice * DirectSalesOrderContentDAO.RequestedQuantity);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.Revenue += (DirectSalesOrderContentDAO.Amount - (DirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0) + (DirectSalesOrderContentDAO.TaxAmount ?? 0));
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.Discount += ((DirectSalesOrderContentDAO.DiscountAmount ?? 0) + (DirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0));
                        }
                    }

                    foreach (var item in SaleEmployee.Items)
                    {
                        item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
                    }

                    foreach (DirectSalesOrderPromotionDAO DirectSalesOrderPromotionDAO in DirectSalesOrderPromotionDAOs)
                    {
                        if (SalesOrderIds.Contains(DirectSalesOrderPromotionDAO.DirectSalesOrderId))
                        {
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = SaleEmployee.Items.Where(i => i.Id == DirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                            if (ReportDirectSalesOrderByEmployeeAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == DirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == DirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = new ReportDirectSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    StoreIds = new HashSet<long>(),
                                    DirectSalesOrderIds = new HashSet<long>(),
                                };
                                SaleEmployee.Items.Add(ReportDirectSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = DirectSalesOrderPromotionDAO.DirectSalesOrder.BuyerStoreId;
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.DirectSalesOrderIds.Add(DirectSalesOrderPromotionDAO.DirectSalesOrderId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.PromotionStock += DirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            }

            foreach (var ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees.Where(x => x.Items.Any()).ToList();
            }

            //làm tròn số
            foreach (var ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    foreach (var item in SaleEmployee.Items)
                    {
                        item.Discount = Math.Round(item.Discount, 0);
                        item.Revenue = Math.Round(item.Revenue, 0);
                        item.SalePriceAverage = Math.Round(item.SalePriceAverage, 0);
                    }
                }
            }

            return ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs.Where(x => x.SaleEmployees.Any()).ToList();
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.Total), HttpPost]
        public async Task<ReportDirectSalesOrderByEmployeeAndItem_TotalDTO> Total([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new ReportDirectSalesOrderByEmployeeAndItem_TotalDTO();

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportDirectSalesOrderByEmployeeAndItem_TotalDTO();

            long? SaleEmployeeId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO ReportDirectSalesOrderByEmployeeAndItem_TotalDTO = new ReportDirectSalesOrderByEmployeeAndItem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.DirectSalesOrder
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
                Skip = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Skip,
                Take = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Take,
                OrderBy = AppUserOrder.DisplayName,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<string> OrganizationNames = AppUsers.Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = OrganizationNames.Select(on => new ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees = AppUsers
                    .Where(x => x.Organization.Name == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.OrganizationName)
                    .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                    {
                        SaleEmployeeId = x.Id,
                        Username = x.Username,
                        DisplayName = x.DisplayName,
                    })
                    .ToList();
            }

            List<long> SaleEmployeeIds = AppUsers.Select(s => s.Id).ToList();

            var DirectSalesOrderContentQuery = DataContext.DirectSalesOrderContent
                .Where(x => SaleEmployeeIds.Contains(x.DirectSalesOrder.SaleEmployeeId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End);
            var DirectSalesOrderPromotionQuery = DataContext.DirectSalesOrderPromotion
                .Where(x => SaleEmployeeIds.Contains(x.DirectSalesOrder.SaleEmployeeId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End);

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalSalesStock = DirectSalesOrderContentQuery.Select(x => x.RequestedQuantity).Sum();

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalPromotionStock = DirectSalesOrderPromotionQuery.Select(x => x.RequestedQuantity).Sum();

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalRevenue = Math.Round(DirectSalesOrderContentQuery.Select(x => x.Amount).Sum()
                - DirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + DirectSalesOrderContentQuery.Where(x => x.TaxAmount.HasValue).Select(x => x.TaxAmount.Value).Sum(), 0);

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalDiscount = Math.Round(DirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + DirectSalesOrderContentQuery.Where(x => x.DiscountAmount.HasValue).Select(x => x.DiscountAmount.Value).Sum(), 0);

            return ReportDirectSalesOrderByEmployeeAndItem_TotalDTO;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Skip = 0;
            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Take = int.MaxValue;
            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = (await List(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)).Value;

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO ReportDirectSalesOrderByEmployeeAndItem_TotalDTO = await Total(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO);
            long stt = 1;
            foreach (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees)
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
            Data.ReportSalesOrderByEmployeeAndItems = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs;
            Data.Total = ReportDirectSalesOrderByEmployeeAndItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportDirectSalesOrderByEmployeeAndItem.xlsx");
        }
    }
}