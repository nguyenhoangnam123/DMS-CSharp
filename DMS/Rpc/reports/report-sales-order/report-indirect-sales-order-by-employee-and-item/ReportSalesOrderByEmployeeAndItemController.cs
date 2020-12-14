using DMS.Common;
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

        [Route(ReportSalesOrderByEmployeeAndItemRoute.FilterListItem), HttpPost]
        public async Task<List<ReportSalesOrderByEmployeeAndItem_ItemDTO>> FilterListItem([FromBody] ReportSalesOrderByEmployeeAndItem_ItemFilterDTO ReportSalesOrderByEmployeeAndItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ReportSalesOrderByEmployeeAndItem_ItemFilterDTO.Id;
            ItemFilter.Code = ReportSalesOrderByEmployeeAndItem_ItemFilterDTO.Code;
            ItemFilter.Name = ReportSalesOrderByEmployeeAndItem_ItemFilterDTO.Name;
            ItemFilter.StatusId = ReportSalesOrderByEmployeeAndItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ReportSalesOrderByEmployeeAndItem_ItemDTO> ReportSalesOrderByEmployeeAndItem_ItemDTOs = Items
                .Select(x => new ReportSalesOrderByEmployeeAndItem_ItemDTO(x)).ToList();
            return ReportSalesOrderByEmployeeAndItem_ItemDTOs;
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
            var ItemId = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.ItemId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            var orderQuery = from i in DataContext.IndirectSalesOrder
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(i.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id
                             select i.Id;

            var Ids = await orderQuery.ToListAsync();

            var transactionQuery = from t in DataContext.IndirectSalesOrderTransaction
                                   where Ids.Contains(t.IndirectSalesOrderId) &&
                                   (ItemId.HasValue == false || t.ItemId == ItemId.Value)
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       SalesEmployeeId = t.SalesEmployeeId
                                   };
            int count = await transactionQuery.Distinct().CountAsync();
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
            var ItemId = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.ItemId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            var orderQuery = from i in DataContext.IndirectSalesOrder
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(i.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id
                             select i.Id;

            var IndirectSalesOrderIds = await orderQuery.ToListAsync();

            var transactionQuery = from t in DataContext.IndirectSalesOrderTransaction
                                   where IndirectSalesOrderIds.Contains(t.IndirectSalesOrderId) &&
                                   (ItemId.HasValue == false || t.ItemId == ItemId.Value)
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       SalesEmployeeId = t.SalesEmployeeId
                                   };

            var Ids = await transactionQuery
                .Distinct()
                .OrderBy(x => x.OrganizationId)
                .Skip(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Skip)
                .Take(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.Take)
                .ToListAsync();
            AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(au => AppUserIds.Contains(au.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .ToListAsync();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) && Start <= x.OrderDate && x.OrderDate <= End &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .ToListAsync();
            IndirectSalesOrderIds = IndirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                .Where(x => AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End)
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
                .Where(x => AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End)
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
            List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO> ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs = new List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>();
            foreach (var Organization in Organizations)
            {
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO = new ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO>()
                };
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees = Ids
                        .Where(x => x.OrganizationId == Organization.Id)
                        .Select(x => new ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                        {
                            SaleEmployeeId = x.SalesEmployeeId
                        }).ToList();
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.Add(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO);
            }
            // khởi tạo khung dữ liệu
            Parallel.ForEach(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs, ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO =>
            {
                foreach (var SalesEmployee in ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    var Employee = AppUserDAOs.Where(x => x.Id == SalesEmployee.SaleEmployeeId).FirstOrDefault();
                    if (Employee != null)
                    {
                        SalesEmployee.Username = Employee.Username;
                        SalesEmployee.DisplayName = Employee.DisplayName;
                    }

                    //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                    var IndirectSalesOrders = IndirectSalesOrderDAOs.Where(x => x.SaleEmployeeId == SalesEmployee.SaleEmployeeId).ToList();
                    var SalesOrderIds = IndirectSalesOrders.Select(x => x.Id).ToList();

                    SalesEmployee.Items = new List<ReportSalesOrderByEmployeeAndItem_ItemDTO>();
                    foreach (IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO in IndirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderContentDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByEmployeeAndItem_ItemDTO ReportSalesOrderByEmployeeAndItem_ItemDTO = SalesEmployee.Items.Where(i => i.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
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
                                SalesEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
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

                    foreach (var item in SalesEmployee.Items)
                    {
                        item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
                    }

                    foreach (IndirectSalesOrderPromotionDAO IndirectSalesOrderPromotionDAO in IndirectSalesOrderPromotionDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByEmployeeAndItem_ItemDTO ReportSalesOrderByEmployeeAndItem_ItemDTO = SalesEmployee.Items.Where(i => i.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
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
                                SalesEmployee.Items.Add(ReportSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = IndirectSalesOrderPromotionDAO.IndirectSalesOrder.BuyerStoreId;
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId);
                            ReportSalesOrderByEmployeeAndItem_ItemDTO.PromotionStock += IndirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            });

            //làm tròn số
            Parallel.ForEach(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs, ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO =>
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
            });

            return ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs;
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
            var ItemId = ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.ItemId?.Equal;

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

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            var orderQuery = from i in DataContext.DirectSalesOrder
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(i.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id
                             select i.Id;

            var DirectSalesOrderIds = await orderQuery.ToListAsync();

            var transactionQuery = from t in DataContext.DirectSalesOrderTransaction
                                   where DirectSalesOrderIds.Contains(t.DirectSalesOrderId) &&
                                   (ItemId.HasValue == false || t.ItemId == ItemId.Value)
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       SalesEmployeeId = t.SalesEmployeeId
                                   };

            var Ids = await transactionQuery
                .OrderBy(x => x.OrganizationId)
                .Distinct().ToListAsync();
            AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(au => AppUserIds.Contains(au.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .ToListAsync();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO> ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs = new List<ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO>();
            foreach (var Organization in Organizations)
            {
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO = new ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO>()
                };
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO.SaleEmployees = Ids
                        .Where(x => x.OrganizationId == Organization.Id)
                        .Select(x => new ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                        {
                            SaleEmployeeId = x.SalesEmployeeId
                        }).ToList();
                ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTOs.Add(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO);
            }

            var IndirectSalesOrderContentQuery = DataContext.IndirectSalesOrderContent
                .Where(x => AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && 
                Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End &&
                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id);
            var IndirectSalesOrderPromotionQuery = DataContext.IndirectSalesOrderPromotion
                .Where(x => AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) && 
                Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End &&
                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id);

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