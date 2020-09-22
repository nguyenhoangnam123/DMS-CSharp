using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Rpc.direct_sales_order;
using DMS.Services.MNotification;
using DMS.Services.MOrganization;
using DMS.Services.MWorkflow;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MDirectSalesOrder
{
    public interface IDirectSalesOrderService : IServiceScoped
    {
        Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<DirectSalesOrder> Get(long Id);
        Task<List<Item>> ListItem(ItemFilter ItemFilter, long? StoreId);
        Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Delete(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Approve(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Reject(DirectSalesOrder DirectSalesOrder);
        Task<List<DirectSalesOrder>> BulkDelete(List<DirectSalesOrder> DirectSalesOrders);
        Task<List<DirectSalesOrder>> Import(List<DirectSalesOrder> DirectSalesOrders);
        Task<DirectSalesOrderFilter> ToFilter(DirectSalesOrderFilter DirectSalesOrderFilter);
    }

    public class DirectSalesOrderService : BaseService, IDirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationService NotificationService;
        private IDirectSalesOrderValidator DirectSalesOrderValidator;
        private IRabbitManager RabbitManager;
        private IOrganizationService OrganizationService;
        private IWorkflowService WorkflowService;

        public DirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INotificationService NotificationService,
            IRabbitManager RabbitManager,
            IDirectSalesOrderValidator DirectSalesOrderValidator,
            IOrganizationService OrganizationService,
            IWorkflowService WorkflowService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificationService = NotificationService;
            this.RabbitManager = RabbitManager;
            this.DirectSalesOrderValidator = DirectSalesOrderValidator;
            this.OrganizationService = OrganizationService;
            this.WorkflowService = WorkflowService;
        }
        public async Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                int result = await UOW.DirectSalesOrderRepository.Count(DirectSalesOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                List<DirectSalesOrder> DirectSalesOrders = await UOW.DirectSalesOrderRepository.List(DirectSalesOrderFilter);
                return DirectSalesOrders;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<List<Item>> ListItem(ItemFilter ItemFilter, long? StoreId)
        {
            try
            {
                AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                var Ids = Items.Select(x => x.Id).ToList();

                List<Warehouse> Warehouses = await UOW.WarehouseRepository.List(new WarehouseFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = WarehouseSelect.Id,
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                    OrganizationId = new IdFilter { Equal = AppUser.OrganizationId }
                });
                var WarehouseIds = Warehouses.Select(x => x.Id).ToList();

                InventoryFilter InventoryFilter = new InventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ItemId = new IdFilter { In = Ids },
                    WarehouseId = new IdFilter { In = WarehouseIds },
                    Selects = InventorySelect.SaleStock | InventorySelect.Item
                };

                var inventories = await UOW.InventoryRepository.List(InventoryFilter);
                var list = inventories.GroupBy(x => x.ItemId).Select(x => new { ItemId = x.Key, SaleStock = x.Sum(s => s.SaleStock) }).ToList();

                foreach (var item in Items)
                {
                    item.SaleStock = list.Where(i => i.ItemId == item.Id).Select(i => i.SaleStock).FirstOrDefault();
                    item.HasInventory = item.SaleStock > 0;
                }

                await ApplyPrice(Items, StoreId);
                return Items;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DirectSalesOrder> Get(long Id)
        {
            DirectSalesOrder DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(Id);
            if (DirectSalesOrder == null)
                return null;
            DirectSalesOrder.RequestState = await WorkflowService.GetRequestState(DirectSalesOrder.RowId);
            if (DirectSalesOrder.RequestState == null)
            {
                DirectSalesOrder.RequestWorkflowStepMappings = new List<RequestWorkflowStepMapping>();
            }
            else
            {
                DirectSalesOrder.RequestStateId = DirectSalesOrder.RequestState.Id;
                DirectSalesOrder.RequestWorkflowStepMappings = await WorkflowService.ListRequestWorkflowStepMapping(DirectSalesOrder.RowId);
            }
            return DirectSalesOrder;
        }

        public async Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Create(DirectSalesOrder))
                return DirectSalesOrder;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var SaleEmployee = await UOW.AppUserRepository.Get(DirectSalesOrder.SaleEmployeeId);
                await Calculator(DirectSalesOrder);
                await UOW.Begin();
                DirectSalesOrder.RequestStateId = RequestStateEnum.NEW.Id;
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                DirectSalesOrder.OrganizationId = SaleEmployee.OrganizationId;
                await UOW.DirectSalesOrderRepository.Create(DirectSalesOrder);
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                await WorkflowService.Initialize(DirectSalesOrder.RowId, WorkflowTypeEnum.DIRECT_SALES_ORDER.Id, DirectSalesOrder.OrganizationId, MapParameters(DirectSalesOrder));
                await UOW.Commit();
                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);

                var RecipientIds = await ListReceipientId(SaleEmployee, DirectSalesOrderRoute.Approve);
                RecipientIds.Add(DirectSalesOrder.SaleEmployeeId);
                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đơn hàng {DirectSalesOrder.Code} đã được thêm mới lên hệ thống bởi {CurrentUser.DisplayName}",
                        LinkWebsite = $"{DirectSalesOrderRoute.Master}/?id=*".Replace("*", DirectSalesOrder.Id.ToString()),
                        LinkMobile = $"{DirectSalesOrderRoute.Mobile}".Replace("*", DirectSalesOrder.Id.ToString()),
                        Time = Now,
                        Unread = true,
                        SenderId = CurrentContext.UserId,
                        RecipientId = Id
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                NotifyUsed(DirectSalesOrder);
                await Logging.CreateAuditLog(DirectSalesOrder, new { }, nameof(DirectSalesOrderService));
                return DirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Update(DirectSalesOrder))
                return DirectSalesOrder;
            try
            {
                var oldData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                if (oldData.SaleEmployeeId != DirectSalesOrder.SaleEmployeeId)
                {
                    var SaleEmployee = await UOW.AppUserRepository.Get(DirectSalesOrder.SaleEmployeeId);
                    DirectSalesOrder.OrganizationId = SaleEmployee.OrganizationId;
                }
                await Calculator(DirectSalesOrder);
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);

                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                UserNotification UserNotification = new UserNotification
                {
                    TitleWeb = $"Thông báo từ DMS",
                    ContentWeb = $"Đơn hàng {DirectSalesOrder.Code} đã được cập nhật thông tin bởi {CurrentUser.DisplayName}",
                    LinkWebsite = $"{DirectSalesOrderRoute.Master}/?id=*".Replace("*", DirectSalesOrder.Id.ToString()),
                    LinkMobile = $"{DirectSalesOrderRoute.Mobile}".Replace("*", DirectSalesOrder.Id.ToString()),
                    Time = Now,
                    Unread = true,
                    SenderId = CurrentContext.UserId,
                    RecipientId = DirectSalesOrder.SaleEmployeeId
                };
                UserNotifications.Add(UserNotification);

                await NotificationService.BulkSend(UserNotifications);

                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                NotifyUsed(DirectSalesOrder);
                await Logging.CreateAuditLog(DirectSalesOrder, oldData, nameof(DirectSalesOrderService));
                return DirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DirectSalesOrder> Delete(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Delete(DirectSalesOrder))
                return DirectSalesOrder;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Delete(DirectSalesOrder);
                await UOW.Commit();

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await UOW.PermissionRepository.ListAppUser(DirectSalesOrderRoute.Approve);
                RecipientIds.AddRange(await UOW.PermissionRepository.ListAppUser(DirectSalesOrderRoute.Reject));
                RecipientIds.Add(CurrentContext.UserId);
                RecipientIds = RecipientIds.Distinct().ToList();

                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đơn hàng {DirectSalesOrder.Code} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName}",
                        Time = Now,
                        Unread = true,
                        SenderId = CurrentContext.UserId,
                        RecipientId = Id
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(new { }, DirectSalesOrder, nameof(DirectSalesOrderService));
                return DirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<DirectSalesOrder>> BulkDelete(List<DirectSalesOrder> DirectSalesOrders)
        {
            if (!await DirectSalesOrderValidator.BulkDelete(DirectSalesOrders))
                return DirectSalesOrders;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.BulkDelete(DirectSalesOrders);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, DirectSalesOrders, nameof(DirectSalesOrderService));
                return DirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<DirectSalesOrder>> Import(List<DirectSalesOrder> DirectSalesOrders)
        {
            if (!await DirectSalesOrderValidator.Import(DirectSalesOrders))
                return DirectSalesOrders;
            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.BulkMerge(DirectSalesOrders);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectSalesOrders, new { }, nameof(DirectSalesOrderService));
                return DirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(DirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DirectSalesOrderFilter> ToFilter(DirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DirectSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;

            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });

            foreach (var currentFilter in CurrentContext.Filters)
            {
                DirectSalesOrderFilter subFilter = new DirectSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                    {
                        var organizationIds = FilterOrganization(Organizations, FilterPermissionDefinition.IdFilter);
                        IdFilter IdFilter = new IdFilter { In = organizationIds };
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, IdFilter);
                    }

                    if (FilterPermissionDefinition.Name == nameof(subFilter.BuyerStoreId))
                        subFilter.BuyerStoreId = FilterBuilder.Merge(subFilter.BuyerStoreId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.Total))
                        subFilter.Total = FilterBuilder.Merge(subFilter.Total, FilterPermissionDefinition.DecimalFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderDate))
                        subFilter.OrderDate = FilterBuilder.Merge(subFilter.OrderDate, FilterPermissionDefinition.DateFilter);

                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                            if (subFilter.AppUserId == null) subFilter.AppUserId = new IdFilter { };
                            subFilter.AppUserId.Equal = CurrentContext.UserId;
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                            if (subFilter.AppUserId == null) subFilter.AppUserId = new IdFilter { };
                            subFilter.AppUserId.NotEqual = CurrentContext.UserId;
                        }
                    }
                }
            }
            return filter;
        }

        private async Task<DirectSalesOrder> Calculator(DirectSalesOrder DirectSalesOrder)
        {
            var ProductIds = new List<long>();
            var ItemIds = new List<long>();
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                ProductIds.AddRange(DirectSalesOrder.DirectSalesOrderContents.Select(x => x.Item.ProductId).ToList());
                ItemIds.AddRange(DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).ToList());
            }
            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                ProductIds.AddRange(DirectSalesOrder.DirectSalesOrderPromotions.Select(x => x.Item.ProductId).ToList());
                ItemIds.AddRange(DirectSalesOrder.DirectSalesOrderPromotions.Select(x => x.ItemId).ToList());
            }
            ProductIds = ProductIds.Distinct().ToList();
            ItemIds = ItemIds.Distinct().ToList();

            var Items = await UOW.ItemRepository.List(new ItemFilter
            {
                Id = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.SalePrice | ItemSelect.Id
            });

            var Products = await UOW.ProductRepository.List(new ProductFilter
            {
                Id = new IdFilter { In = ProductIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.UnitOfMeasure | ProductSelect.UnitOfMeasureGrouping | ProductSelect.Id | ProductSelect.TaxType
            });

            var UOMGs = await UOW.UnitOfMeasureGroupingRepository.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.Id | UnitOfMeasureGroupingSelect.UnitOfMeasure | UnitOfMeasureGroupingSelect.UnitOfMeasureGroupingContents
            });

            //sản phẩm bán
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    var Item = Items.Where(x => x.Id == DirectSalesOrderContent.ItemId).FirstOrDefault();
                    var Product = Products.Where(x => DirectSalesOrderContent.Item.ProductId == x.Id).FirstOrDefault();
                    DirectSalesOrderContent.PrimaryUnitOfMeasureId = Product.UnitOfMeasureId;
                    DirectSalesOrderContent.PrimaryPrice = Item.SalePrice;

                    List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
                    if (Product.UnitOfMeasureGroupingId.HasValue)
                    {
                        var UOMG = UOMGs.Where(x => x.Id == Product.UnitOfMeasureGroupingId).FirstOrDefault();
                        UnitOfMeasures = UOMG.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                        {
                            Id = x.UnitOfMeasure.Id,
                            Code = x.UnitOfMeasure.Code,
                            Name = x.UnitOfMeasure.Name,
                            Description = x.UnitOfMeasure.Description,
                            StatusId = x.UnitOfMeasure.StatusId,
                            Factor = x.Factor
                        }).ToList();
                    }

                    UnitOfMeasures.Add(new UnitOfMeasure
                    {
                        Id = Product.UnitOfMeasure.Id,
                        Code = Product.UnitOfMeasure.Code,
                        Name = Product.UnitOfMeasure.Name,
                        Description = Product.UnitOfMeasure.Description,
                        StatusId = Product.UnitOfMeasure.StatusId,
                        Factor = 1
                    });
                    var UOM = UnitOfMeasures.Where(x => DirectSalesOrderContent.UnitOfMeasureId == x.Id).FirstOrDefault();
                    //DirectSalesOrderContent.TaxPercentage = Product.TaxType.Percentage;
                    DirectSalesOrderContent.RequestedQuantity = DirectSalesOrderContent.Quantity * UOM.Factor.Value;

                    //Trường hợp không sửa giá, giá bán = giá bán cơ sở của sản phẩm * hệ số quy đổi của đơn vị tính
                    if (DirectSalesOrder.EditedPriceStatusId == Enums.EditedPriceStatusEnum.INACTIVE.Id)
                    {
                        DirectSalesOrderContent.SalePrice = Item.SalePrice * UOM.Factor.Value;
                    }
                    //giá tiền từng line trước chiết khấu
                    var SubAmount = DirectSalesOrderContent.Quantity * DirectSalesOrderContent.SalePrice;
                    if (DirectSalesOrderContent.DiscountPercentage.HasValue)
                    {
                        DirectSalesOrderContent.DiscountAmount = SubAmount * DirectSalesOrderContent.DiscountPercentage.Value / 100;
                        DirectSalesOrderContent.DiscountAmount = Math.Round(DirectSalesOrderContent.DiscountAmount ?? 0, 0);
                        DirectSalesOrderContent.Amount = SubAmount - DirectSalesOrderContent.DiscountAmount.Value;
                    }
                    else
                    {
                        DirectSalesOrderContent.Amount = SubAmount;
                    }
                }

                //tổng trước chiết khấu
                DirectSalesOrder.SubTotal = DirectSalesOrder.DirectSalesOrderContents.Sum(x => x.Amount);

                //tính tổng chiết khấu theo % chiết khấu chung
                if (DirectSalesOrder.GeneralDiscountPercentage.HasValue && DirectSalesOrder.GeneralDiscountPercentage > 0)
                {
                    DirectSalesOrder.GeneralDiscountAmount = DirectSalesOrder.SubTotal * (DirectSalesOrder.GeneralDiscountPercentage / 100);
                    DirectSalesOrder.GeneralDiscountAmount = Math.Round(DirectSalesOrder.GeneralDiscountAmount.Value, 0);
                }
                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    //phân bổ chiết khấu chung = tổng chiết khấu chung * (tổng từng line/tổng trc chiết khấu)
                    DirectSalesOrderContent.GeneralDiscountPercentage = DirectSalesOrderContent.Amount / DirectSalesOrder.SubTotal * 100;
                    DirectSalesOrderContent.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount * DirectSalesOrderContent.GeneralDiscountPercentage / 100;
                    DirectSalesOrderContent.GeneralDiscountAmount = Math.Round(DirectSalesOrderContent.GeneralDiscountAmount ?? 0, 0);
                    //thuê từng line = (tổng từng line - chiết khấu phân bổ) * % thuế
                    DirectSalesOrderContent.TaxAmount = (DirectSalesOrderContent.Amount - (DirectSalesOrderContent.GeneralDiscountAmount.HasValue ? DirectSalesOrderContent.GeneralDiscountAmount.Value : 0)) * DirectSalesOrderContent.TaxPercentage / 100;
                    DirectSalesOrderContent.TaxAmount = Math.Round(DirectSalesOrderContent.TaxAmount ?? 0, 0);
                }

                DirectSalesOrder.TotalTaxAmount = DirectSalesOrder.DirectSalesOrderContents.Where(x => x.TaxAmount.HasValue).Sum(x => x.TaxAmount.Value);
                DirectSalesOrder.TotalTaxAmount = Math.Round(DirectSalesOrder.TotalTaxAmount, 0);
                //tổng phải thanh toán
                DirectSalesOrder.Total = DirectSalesOrder.SubTotal - (DirectSalesOrder.GeneralDiscountAmount.HasValue ? DirectSalesOrder.GeneralDiscountAmount.Value : 0) + DirectSalesOrder.TotalTaxAmount;
            }
            else
            {
                DirectSalesOrder.SubTotal = 0;
                DirectSalesOrder.GeneralDiscountPercentage = null;
                DirectSalesOrder.GeneralDiscountAmount = null;
                DirectSalesOrder.TotalTaxAmount = 0;
                DirectSalesOrder.Total = 0;
            }

            //sản phẩm khuyến mãi
            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                foreach (var DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    var Product = Products.Where(x => DirectSalesOrderPromotion.Item.ProductId == x.Id).FirstOrDefault();
                    DirectSalesOrderPromotion.PrimaryUnitOfMeasureId = Product.UnitOfMeasureId;

                    List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
                    if (Product.UnitOfMeasureGroupingId.HasValue)
                    {
                        var UOMG = UOMGs.Where(x => x.Id == Product.UnitOfMeasureGroupingId).FirstOrDefault();
                        UnitOfMeasures = UOMG.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                        {
                            Id = x.UnitOfMeasure.Id,
                            Code = x.UnitOfMeasure.Code,
                            Name = x.UnitOfMeasure.Name,
                            Description = x.UnitOfMeasure.Description,
                            StatusId = x.UnitOfMeasure.StatusId,
                            Factor = x.Factor
                        }).ToList();
                    }

                    UnitOfMeasures.Add(new UnitOfMeasure
                    {
                        Id = Product.UnitOfMeasure.Id,
                        Code = Product.UnitOfMeasure.Code,
                        Name = Product.UnitOfMeasure.Name,
                        Description = Product.UnitOfMeasure.Description,
                        StatusId = Product.UnitOfMeasure.StatusId,
                        Factor = 1
                    });
                    var UOM = UnitOfMeasures.Where(x => DirectSalesOrderPromotion.UnitOfMeasureId == x.Id).FirstOrDefault();
                    DirectSalesOrderPromotion.RequestedQuantity = DirectSalesOrderPromotion.Quantity * UOM.Factor.Value;
                }
            }

            return DirectSalesOrder;
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long? StoreId)
        {
            var CurrrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            SystemConfiguration SystemConfiguration = await UOW.SystemConfigurationRepository.Get();
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            var OrganizationIds = Organizations
                .Where(x => x.Path.StartsWith(CurrrentUser.Organization.Path) || CurrrentUser.Organization.Path.StartsWith(x.Path))
                .Select(x => x.Id)
                .ToList();


            var ItemIds = Items.Select(x => x.Id).ToList();
            Dictionary<long, decimal> result = new Dictionary<long, decimal>();
            PriceListItemMappingFilter PriceListItemMappingFilter = new PriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListItemMappingSelect.ALL,
                PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.ALLSTORE.Id },
                SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.DIRECT.Id },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var PriceListItemMappingAllStore = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
            List<PriceListItemMapping> PriceListItemMappings = new List<PriceListItemMapping>();
            PriceListItemMappings.AddRange(PriceListItemMappingAllStore);

            if (StoreId.HasValue)
            {
                var Store = await UOW.StoreRepository.Get(StoreId.Value);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STOREGROUPING.Id },
                    SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.DIRECT.Id },
                    StoreGroupingId = new IdFilter { Equal = Store.StoreGroupingId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreGrouping = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STORETYPE.Id },
                    SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.DIRECT.Id },
                    StoreTypeId = new IdFilter { Equal = Store.StoreTypeId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreType = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.DETAILS.Id },
                    SalesOrderTypeId = new IdFilter { Equal = SalesOrderTypeEnum.DIRECT.Id },
                    StoreId = new IdFilter { Equal = StoreId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreDetail = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreGrouping);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreType);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreDetail);
            }

            foreach (var ItemId in ItemIds)
            {
                result.Add(ItemId, decimal.MaxValue);
            }

            //Áp giá theo cấu hình
            //Ưu tiên lấy giá thấp hơn
            if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 0)
            {
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        long targetPrice = long.MaxValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(long.MaxValue)
                            .Min();
                        if (targetPrice < long.MaxValue)
                        {
                            result[ItemId] = targetPrice;
                            break;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MaxValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice).FirstOrDefault();
                    }
                }
            }
            //Ưu tiên lấy giá cao hơn
            else if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 1)
            {
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        long targetPrice = long.MinValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(long.MinValue)
                            .Max();
                        if (targetPrice > long.MinValue)
                        {
                            result[ItemId] = targetPrice;
                            break;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MinValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice).FirstOrDefault();
                    }
                }
            }

            return Items;
        }

        public async Task<DirectSalesOrder> Approve(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.Id == 0)
                DirectSalesOrder = await Create(DirectSalesOrder);
            else
                DirectSalesOrder = await Update(DirectSalesOrder);
            Dictionary<string, string> Parameters = MapParameters(DirectSalesOrder);
            GenericEnum Action = await WorkflowService.Approve(DirectSalesOrder.RowId, WorkflowTypeEnum.DIRECT_SALES_ORDER.Id, DirectSalesOrder.OrganizationId, Parameters);
            if (Action.Id != WorkflowActionEnum.OK.Id)
                return null;
            return await Get(DirectSalesOrder.Id);
        }

        public async Task<DirectSalesOrder> Reject(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Dictionary<string, string> Parameters = MapParameters(DirectSalesOrder);
            GenericEnum Action = await WorkflowService.Reject(DirectSalesOrder.RowId, WorkflowTypeEnum.DIRECT_SALES_ORDER.Id, DirectSalesOrder.OrganizationId, Parameters);
            if (Action.Id != WorkflowActionEnum.OK.Id)
                return null;
            return await Get(DirectSalesOrder.Id);
        }

        private Dictionary<string, string> MapParameters(DirectSalesOrder DirectSalesOrder)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add(nameof(DirectSalesOrder.Id), DirectSalesOrder.Id.ToString());
            Parameters.Add(nameof(DirectSalesOrder.Code), DirectSalesOrder.Code);
            Parameters.Add("Username", CurrentContext.UserName);
            return Parameters;
        }

        private async Task<List<long>> ListReceipientId(AppUser AppUser, string Path)
        {
            var Ids = await UOW.PermissionRepository.ListAppUser(Path);
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            var OrganizationIds = Organizations
                .Where(x => x.Path.StartsWith(AppUser.Organization.Path) || AppUser.Organization.Path.StartsWith(x.Path))
                .Select(x => x.Id)
                .ToList();

            var AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Organization,
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            });
            var AppUserIds = AppUsers.Where(x => OrganizationIds.Contains(x.OrganizationId)).Select(x => x.Id).ToList();
            AppUserIds = AppUserIds.Intersect(Ids).ToList();
            return AppUserIds;
        }

        private void NotifyUsed(DirectSalesOrder DirectSalesOrder)
        {
            {
                List<long> ItemIds = DirectSalesOrder.DirectSalesOrderContents.Select(i => i.ItemId).ToList();
                List<EventMessage<Item>> itemMessages = ItemIds.Select(i => new EventMessage<Item>
                {
                    Content = new Item { Id = i },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(itemMessages, RoutingKeyEnum.ItemUsed);
            }

            {
                List<long> PrimaryUOMIds = DirectSalesOrder.DirectSalesOrderContents.Select(i => i.PrimaryUnitOfMeasureId).ToList();
                List<long> UOMIds = DirectSalesOrder.DirectSalesOrderContents.Select(i => i.UnitOfMeasureId).ToList();
                UOMIds.AddRange(PrimaryUOMIds);
                List<EventMessage<UnitOfMeasure>> UnitOfMeasureMessages = UOMIds.Select(x => new EventMessage<UnitOfMeasure>
                {
                    Content = new UnitOfMeasure { Id = x },
                    EntityName = nameof(UnitOfMeasure),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(UnitOfMeasureMessages, RoutingKeyEnum.UnitOfMeasureUsed);
            }
            {
                List<EventMessage<Store>> storeMessages = new List<EventMessage<Store>>();
                EventMessage<Store> BuyerStore = new EventMessage<Store>
                {
                    Content = new Store { Id = DirectSalesOrder.BuyerStoreId },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                storeMessages.Add(BuyerStore);
                RabbitManager.PublishList(storeMessages, RoutingKeyEnum.StoreUsed);
            }

            {
                EventMessage<AppUser> AppUserMessage = new EventMessage<AppUser>
                {
                    Content = new AppUser { Id = DirectSalesOrder.SaleEmployeeId },
                    EntityName = nameof(AppUser),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(AppUserMessage, RoutingKeyEnum.AppUserUsed);
            }
        }
    }
}
