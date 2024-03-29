﻿using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Rpc.direct_sales_order;
using DMS.Services.MNotification;
using DMS.Services.MOrganization;
using DMS.Services.MWorkflow;
using DMS.Helpers;
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
        Task<int> CountNew(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> ListNew(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<int> CountPending(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> ListPending(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<int> CountCompleted(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> ListCompleted(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<DirectSalesOrder> Get(long Id);
        Task<DirectSalesOrder> GetDetail(long Id);
        Task<List<Item>> ListItem(ItemFilter ItemFilter, long? SalesEmployeeId, long? StoreId);
        Task<DirectSalesOrder> ApplyPromotionCode(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Delete(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Send(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Approve(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Reject(DirectSalesOrder DirectSalesOrder);
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
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
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
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
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

        public async Task<int> CountNew(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
                int result = await UOW.DirectSalesOrderRepository.CountNew(DirectSalesOrderFilter);
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
        public async Task<List<DirectSalesOrder>> ListNew(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
                List<DirectSalesOrder> DirectSalesOrders = await UOW.DirectSalesOrderRepository.ListNew(DirectSalesOrderFilter);
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

        public async Task<int> CountPending(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
                int result = await UOW.DirectSalesOrderRepository.CountPending(DirectSalesOrderFilter);
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
        public async Task<List<DirectSalesOrder>> ListPending(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
                List<DirectSalesOrder> DirectSalesOrders = await UOW.DirectSalesOrderRepository.ListPending(DirectSalesOrderFilter);
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

        public async Task<int> CountCompleted(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
                int result = await UOW.DirectSalesOrderRepository.CountCompleted(DirectSalesOrderFilter);
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
        public async Task<List<DirectSalesOrder>> ListCompleted(DirectSalesOrderFilter DirectSalesOrderFilter)
        {
            try
            {
                DirectSalesOrderFilter.ApproverId = new IdFilter { Equal = CurrentContext.UserId };
                List<DirectSalesOrder> DirectSalesOrders = await UOW.DirectSalesOrderRepository.ListCompleted(DirectSalesOrderFilter);
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

        public async Task<List<Item>> ListItem(ItemFilter ItemFilter, long? SalesEmployeeId, long? StoreId)
        {
            try
            {
                AppUser AppUser = await UOW.AppUserRepository.Get(SalesEmployeeId.Value);
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                var Ids = Items.Select(x => x.Id).ToList();

                if(AppUser != null)
                {
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

                    await ApplyPrice(Items, SalesEmployeeId, StoreId);
                }
                
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
                RequestWorkflowStepMapping RequestWorkflowStepMapping = new RequestWorkflowStepMapping
                {
                    AppUserId = DirectSalesOrder.SaleEmployeeId,
                    CreatedAt = DirectSalesOrder.CreatedAt,
                    UpdatedAt = DirectSalesOrder.UpdatedAt,
                    RequestId = DirectSalesOrder.RowId,
                    AppUser = DirectSalesOrder.SaleEmployee == null ? null : new AppUser
                    {
                        Id = DirectSalesOrder.SaleEmployee.Id,
                        Username = DirectSalesOrder.SaleEmployee.Username,
                        DisplayName = DirectSalesOrder.SaleEmployee.DisplayName,
                    },
                };
                DirectSalesOrder.RequestWorkflowStepMappings.Add(RequestWorkflowStepMapping);
                RequestWorkflowStepMapping.WorkflowStateId = DirectSalesOrder.RequestStateId;
                DirectSalesOrder.RequestState = WorkflowService.GetRequestState(DirectSalesOrder.RequestStateId);
                RequestWorkflowStepMapping.WorkflowState = WorkflowService.GetWorkflowState(RequestWorkflowStepMapping.WorkflowStateId);
            }
            else
            {
                DirectSalesOrder.RequestStateId = DirectSalesOrder.RequestState.Id;
                DirectSalesOrder.RequestWorkflowStepMappings = await WorkflowService.ListRequestWorkflowStepMapping(DirectSalesOrder.RowId);
            }
            return DirectSalesOrder;
        }

        public async Task<DirectSalesOrder> GetDetail(long Id)
        {
            DirectSalesOrder DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(Id);
            if (DirectSalesOrder == null)
                return null;
            DirectSalesOrder.RequestState = await WorkflowService.GetRequestState(DirectSalesOrder.RowId);
            if (DirectSalesOrder.RequestState == null)
            {
                DirectSalesOrder.RequestWorkflowStepMappings = new List<RequestWorkflowStepMapping>();
                RequestWorkflowStepMapping RequestWorkflowStepMapping = new RequestWorkflowStepMapping
                {
                    AppUserId = DirectSalesOrder.SaleEmployeeId,
                    CreatedAt = DirectSalesOrder.CreatedAt,
                    UpdatedAt = DirectSalesOrder.UpdatedAt,
                    RequestId = DirectSalesOrder.RowId,
                    AppUser = DirectSalesOrder.SaleEmployee == null ? null : new AppUser
                    {
                        Id = DirectSalesOrder.SaleEmployee.Id,
                        Username = DirectSalesOrder.SaleEmployee.Username,
                        DisplayName = DirectSalesOrder.SaleEmployee.DisplayName,
                    },
                };
                DirectSalesOrder.RequestWorkflowStepMappings.Add(RequestWorkflowStepMapping);
                RequestWorkflowStepMapping.WorkflowStateId = DirectSalesOrder.RequestStateId;
                DirectSalesOrder.RequestState = WorkflowService.GetRequestState(DirectSalesOrder.RequestStateId);
                RequestWorkflowStepMapping.WorkflowState = WorkflowService.GetWorkflowState(RequestWorkflowStepMapping.WorkflowStateId);
            }
            else
            {
                DirectSalesOrder.RequestStateId = DirectSalesOrder.RequestState.Id;
                DirectSalesOrder.RequestWorkflowStepMappings = await WorkflowService.ListRequestWorkflowStepMapping(DirectSalesOrder.RowId);
            }
            return DirectSalesOrder;
        }

        public async Task<DirectSalesOrder> ApplyPromotionCode(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.ApplyPromotionCode(DirectSalesOrder))
                return DirectSalesOrder;

            try
            {
                await Calculator(DirectSalesOrder);
                return DirectSalesOrder;
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
                DirectSalesOrder.CreatorId = CurrentContext.UserId;
                DirectSalesOrder.DirectSalesOrderSourceTypeId = DirectSalesOrderSourceTypeEnum.FROM_EMPLOYEE.Id; // create DirectSalesOrder from DMS
                await UOW.DirectSalesOrderRepository.Create(DirectSalesOrder);
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);

                var PromotionCodeId = DirectSalesOrder.PromotionCodeId;
                if (DirectSalesOrder.PromotionCodeId.HasValue)
                {
                    PromotionCodeHistory PromotionCodeHistory = new PromotionCodeHistory()
                    {
                        PromotionCodeId = DirectSalesOrder.PromotionCodeId.Value,
                        AppliedAt = StaticParams.DateTimeNow,
                        RowId = DirectSalesOrder.RowId
                    };
                    await UOW.PromotionCodeHistoryRepository.Create(PromotionCodeHistory);
                }

                await UOW.Commit();
                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                DirectSalesOrder.PromotionCodeId = PromotionCodeId;
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
                        LinkWebsite = $"{DirectSalesOrderRoute.Master}#*".Replace("*", DirectSalesOrder.Id.ToString()),
                        LinkMobile = $"{DirectSalesOrderRoute.Mobile}".Replace("*", DirectSalesOrder.Id.ToString()),
                        Time = Now,
                        Unread = true,
                        SenderId = CurrentContext.UserId,
                        RecipientId = Id
                    };
                    UserNotifications.Add(UserNotification);
                }

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.AppUserNotificationSend);

                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                Sync(new List<DirectSalesOrder> { DirectSalesOrder });
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
                    LinkWebsite = $"{DirectSalesOrderRoute.Master}#*".Replace("*", DirectSalesOrder.Id.ToString()),
                    LinkMobile = $"{DirectSalesOrderRoute.Mobile}".Replace("*", DirectSalesOrder.Id.ToString()),
                    Time = Now,
                    Unread = true,
                    SenderId = CurrentContext.UserId,
                    RecipientId = DirectSalesOrder.SaleEmployeeId
                };
                UserNotifications.Add(UserNotification);

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.AppUserNotificationSend);

                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                Sync(new List<DirectSalesOrder> { DirectSalesOrder });
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
                        RecipientId = Id,
                        RowId = Guid.NewGuid(),
                    };
                    UserNotifications.Add(UserNotification);
                }

                RabbitManager.PublishList(UserNotifications, RoutingKeyEnum.AppUserNotificationSend);

                DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                Sync(new List<DirectSalesOrder> { DirectSalesOrder });
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

            ItemFilter ItemFilter = new ItemFilter
            {
                Skip = 0,
                Take = ItemIds.Count,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.ALL,
            };
            var Items = await ListItem(ItemFilter, DirectSalesOrder.SaleEmployeeId, DirectSalesOrder.BuyerStoreId);

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
                    if (DirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.INACTIVE.Id)
                    {
                        DirectSalesOrderContent.PrimaryPrice = Item.SalePrice.GetValueOrDefault(0);
                        DirectSalesOrderContent.SalePrice = DirectSalesOrderContent.PrimaryPrice * UOM.Factor.Value;
                        DirectSalesOrderContent.EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id;
                    }

                    if (DirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.ACTIVE.Id)
                    {
                        DirectSalesOrderContent.SalePrice = DirectSalesOrderContent.PrimaryPrice * UOM.Factor.Value;
                        if (Item.SalePrice == DirectSalesOrderContent.PrimaryPrice)
                            DirectSalesOrderContent.EditedPriceStatusId = EditedPriceStatusEnum.INACTIVE.Id;
                        else
                            DirectSalesOrderContent.EditedPriceStatusId = EditedPriceStatusEnum.ACTIVE.Id;
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
                DirectSalesOrder.TotalAfterTax = DirectSalesOrder.SubTotal - (DirectSalesOrder.GeneralDiscountAmount.HasValue ? DirectSalesOrder.GeneralDiscountAmount.Value : 0) + DirectSalesOrder.TotalTaxAmount;
                if (!string.IsNullOrWhiteSpace(DirectSalesOrder.PromotionCode))
                {
                    await CalculatePromotionCode(DirectSalesOrder);
                }
                else
                {
                    DirectSalesOrder.Total = DirectSalesOrder.TotalAfterTax;
                }
            }
            else
            {
                DirectSalesOrder.SubTotal = 0;
                DirectSalesOrder.GeneralDiscountPercentage = null;
                DirectSalesOrder.GeneralDiscountAmount = null;
                DirectSalesOrder.TotalTaxAmount = 0;
                DirectSalesOrder.TotalAfterTax = 0;
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

        private async Task CalculatePromotionCode(DirectSalesOrder DirectSalesOrder)
        {
            PromotionCodeFilter PromotionCodeFilter = new PromotionCodeFilter()
            {
                Code = new StringFilter { Equal = DirectSalesOrder.PromotionCode },
                Skip = 0,
                Take = 1,
                Selects = PromotionCodeSelect.Id
            };
            var PromotionCodes = await UOW.PromotionCodeRepository.List(PromotionCodeFilter);
            var PromotionCodeId = PromotionCodes.Select(x => x.Id).FirstOrDefault();
            var PromotionCode = await UOW.PromotionCodeRepository.Get(PromotionCodeId);

            var ProductIds = PromotionCode.PromotionCodeProductMappings.Select(x => x.ProductId).ToList();
            var ProductIdsInOrder = DirectSalesOrder.DirectSalesOrderContents?.Select(x => x.Item.ProductId).Distinct().ToList();

            DirectSalesOrder.PromotionCodeId = PromotionCode.Id;
            if (PromotionCode.PromotionDiscountTypeId == PromotionDiscountTypeEnum.AMOUNT.Id)
            {
                if(PromotionCode.PromotionProductAppliedTypeId == PromotionProductAppliedTypeEnum.ALL.Id)
                {
                    DirectSalesOrder.Total = DirectSalesOrder.TotalAfterTax - PromotionCode.Value;
                    DirectSalesOrder.PromotionValue = PromotionCode.Value;
                }
                else
                {
                    var Intersect = ProductIdsInOrder.Intersect(ProductIds).Count();
                    if(Intersect > 0)
                    {
                        DirectSalesOrder.Total = DirectSalesOrder.TotalAfterTax - PromotionCode.Value;
                        DirectSalesOrder.PromotionValue = PromotionCode.Value;
                    }
                }
            }
            else if (PromotionCode.PromotionDiscountTypeId == PromotionDiscountTypeEnum.PERCENTAGE.Id)
            {
                decimal PromotionValue = 0;
                if (PromotionCode.PromotionProductAppliedTypeId == PromotionProductAppliedTypeEnum.ALL.Id)
                {
                    foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                    {
                        PromotionValue += (DirectSalesOrderContent.Amount - DirectSalesOrderContent.GeneralDiscountAmount.GetValueOrDefault(0) + DirectSalesOrderContent.TaxAmount.GetValueOrDefault(0)) * PromotionCode.Value / 100;
                    }
                }
                else
                {
                    var Intersect = ProductIdsInOrder.Intersect(ProductIds).ToList();
                    foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                    {
                        if(Intersect.Contains(DirectSalesOrderContent.Item.ProductId))
                            PromotionValue += (DirectSalesOrderContent.Amount - DirectSalesOrderContent.GeneralDiscountAmount.GetValueOrDefault(0) + DirectSalesOrderContent.TaxAmount.GetValueOrDefault(0)) * PromotionCode.Value / 100;
                    }
                }

                if(PromotionCode.MaxValue.HasValue && PromotionCode.MaxValue.Value < PromotionValue)
                {
                    PromotionValue = PromotionCode.MaxValue.Value;
                    DirectSalesOrder.Total = DirectSalesOrder.TotalAfterTax - PromotionValue;
                }
                else
                {
                    DirectSalesOrder.Total = DirectSalesOrder.TotalAfterTax - PromotionValue;
                }
                DirectSalesOrder.PromotionValue = PromotionValue;
                if (DirectSalesOrder.Total <= 0)
                    DirectSalesOrder.Total = 0;
            }
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long? SalesEmployeeId, long? StoreId)
        {
            var SalesEmployee = await UOW.AppUserRepository.Get(SalesEmployeeId.Value);
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
                .Where(x => x.Path.StartsWith(SalesEmployee.Organization.Path) || SalesEmployee.Organization.Path.StartsWith(x.Path))
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
                SalesOrderTypeId = new IdFilter { In = new List<long>{ SalesOrderTypeEnum.DIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
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
                    SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.DIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
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
                    SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.DIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
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
                    SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.DIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                    StoreId = new IdFilter { Equal = StoreId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreDetail = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreGrouping);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreType);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreDetail);
            }

            //Áp giá theo cấu hình
            //Ưu tiên lấy giá thấp hơn
            if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 0)
            {
                foreach (var ItemId in ItemIds)
                {
                    result.Add(ItemId, decimal.MaxValue);
                }
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MaxValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(decimal.MaxValue)
                            .Min();
                        if (targetPrice < result[ItemId])
                        {
                            result[ItemId] = targetPrice;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MaxValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice.GetValueOrDefault(0)).FirstOrDefault();
                    }
                }
            }
            //Ưu tiên lấy giá cao hơn
            else if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 1)
            {
                foreach (var ItemId in ItemIds)
                {
                    result.Add(ItemId, decimal.MinValue);
                }
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MinValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(decimal.MinValue)
                            .Max();
                        if (targetPrice > result[ItemId])
                        {
                            result[ItemId] = targetPrice;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MinValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice.GetValueOrDefault(0)).FirstOrDefault();
                    }
                }
            }

            foreach (var item in Items)
            {
                item.SalePrice = result[item.Id];
            }

            return Items;
        }

        public async Task<DirectSalesOrder> Send(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.Id == 0)
                DirectSalesOrder = await Create(DirectSalesOrder);
            else
                DirectSalesOrder = await Update(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated == false)
                return DirectSalesOrder;
            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Dictionary<string, string> Parameters = await MapParameters(DirectSalesOrder);
            GenericEnum RequestState = await WorkflowService.Send(DirectSalesOrder.RowId, WorkflowTypeEnum.DIRECT_SALES_ORDER.Id, DirectSalesOrder.OrganizationId, Parameters);
            DirectSalesOrder.RequestStateId = RequestState.Id;
            if (RequestState.Id == RequestStateEnum.APPROVED.Id)
            {
                DirectSalesOrder.StoreApprovalStateId = StoreApprovalStateEnum.PENDING.Id;
            } // neu phe duyet workflow, don hang co trang thai StoreApprovalState la pending
            await UOW.DirectSalesOrderRepository.UpdateState(DirectSalesOrder);

            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Sync(new List<DirectSalesOrder> { DirectSalesOrder });

            return DirectSalesOrder;
        }

        public async Task<DirectSalesOrder> Approve(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrder = await Update(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated == false)
                return DirectSalesOrder;
            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Dictionary<string, string> Parameters = await MapParameters(DirectSalesOrder);
            await WorkflowService.Approve(DirectSalesOrder.RowId, WorkflowTypeEnum.DIRECT_SALES_ORDER.Id, Parameters);
            RequestState RequestState = await WorkflowService.GetRequestState(DirectSalesOrder.RowId);
            DirectSalesOrder.RequestStateId = RequestState.Id;
            if(RequestState.Id == RequestStateEnum.APPROVED.Id)
            {
                DirectSalesOrder.StoreApprovalStateId = StoreApprovalStateEnum.PENDING.Id;
            } // neu phe duyet workflow, don hang co trang thai StoreApprovalState la pending
            await UOW.DirectSalesOrderRepository.UpdateState(DirectSalesOrder);

            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Sync(new List<DirectSalesOrder> { DirectSalesOrder });

            return DirectSalesOrder;
        }

        public async Task<DirectSalesOrder> Reject(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Dictionary<string, string> Parameters = await MapParameters(DirectSalesOrder);
            GenericEnum Action = await WorkflowService.Reject(DirectSalesOrder.RowId, WorkflowTypeEnum.DIRECT_SALES_ORDER.Id, Parameters);
            RequestState RequestState = await WorkflowService.GetRequestState(DirectSalesOrder.RowId);
            DirectSalesOrder.RequestStateId = RequestState.Id;
            await UOW.DirectSalesOrderRepository.UpdateState(DirectSalesOrder);

            DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            Sync(new List<DirectSalesOrder> { DirectSalesOrder });

            return DirectSalesOrder;
        }

        private async Task<Dictionary<string, string>> MapParameters(DirectSalesOrder DirectSalesOrder)
        {
            var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add(nameof(DirectSalesOrder.Id), DirectSalesOrder.Id.ToString());
            Parameters.Add(nameof(DirectSalesOrder.Code), DirectSalesOrder.Code);
            Parameters.Add(nameof(DirectSalesOrder.SaleEmployeeId), DirectSalesOrder.SaleEmployeeId.ToString());
            Parameters.Add(nameof(DirectSalesOrder.BuyerStoreId), DirectSalesOrder.BuyerStoreId.ToString());
            Parameters.Add(nameof(AppUser.DisplayName), AppUser.DisplayName);
            Parameters.Add(nameof(DirectSalesOrder.RequestStateId), DirectSalesOrder.RequestStateId.ToString());

            Parameters.Add(nameof(DirectSalesOrder.Total), DirectSalesOrder.Total.ToString());
            Parameters.Add(nameof(DirectSalesOrder.TotalDiscountAmount), DirectSalesOrder.TotalDiscountAmount.ToString());
            Parameters.Add(nameof(DirectSalesOrder.TotalRequestedQuantity), DirectSalesOrder.TotalRequestedQuantity.ToString());
            Parameters.Add(nameof(DirectSalesOrder.OrganizationId), DirectSalesOrder.OrganizationId.ToString());

            RequestWorkflowDefinitionMapping RequestWorkflowDefinitionMapping = await UOW.RequestWorkflowDefinitionMappingRepository.Get(DirectSalesOrder.RowId);
            if (RequestWorkflowDefinitionMapping == null)
                Parameters.Add(nameof(RequestState), RequestStateEnum.NEW.Id.ToString());
            else
                Parameters.Add(nameof(RequestState), RequestWorkflowDefinitionMapping.RequestStateId.ToString());
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

        private void Sync(List<DirectSalesOrder> DirectSalesOrders)
        {
            List<AppUser> AppUsers = new List<AppUser>();
            AppUsers.AddRange(DirectSalesOrders.Select(x => new AppUser { Id = x.SaleEmployeeId }));
            AppUsers.AddRange(DirectSalesOrders.Select(x => new AppUser { Id = x.CreatorId.Value }));
            AppUsers = AppUsers.Distinct().ToList();

            List<Organization> Organizations = DirectSalesOrders.Select(x => new Organization { Id = x.OrganizationId }).Distinct().ToList();
            
            string PromotionCode = DirectSalesOrders.Where(x => !string.IsNullOrWhiteSpace(x.PromotionCode)).Select(x => x.PromotionCode).Distinct().FirstOrDefault();
            var PromotionCodeFilter = new PromotionCodeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PromotionCodeSelect.Id,
                Code = new StringFilter { Equal = PromotionCode }
            };
            var PromotionCodes = (UOW.PromotionCodeRepository.List(PromotionCodeFilter)).Result;

            List<Store> Stores = DirectSalesOrders.Select(x => new Store { Id = x.BuyerStoreId }).Distinct().ToList();

            List<Item> Items = new List<Item>();
            Items.AddRange(DirectSalesOrders.Where(x => x.DirectSalesOrderContents != null)
                .SelectMany(x => x.DirectSalesOrderContents).Select(x => new Item { Id = x.ItemId }));
            Items.AddRange(DirectSalesOrders.Where(x => x.DirectSalesOrderPromotions != null)
                .SelectMany(x => x.DirectSalesOrderPromotions).Select(x => new Item { Id = x.ItemId }));
            Items = Items.Distinct().ToList();
            List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
            UnitOfMeasures.AddRange(DirectSalesOrders.Where(x => x.DirectSalesOrderContents != null)
                .SelectMany(x => x.DirectSalesOrderContents).Select(x => new UnitOfMeasure { Id = x.UnitOfMeasureId }));
            UnitOfMeasures.AddRange(DirectSalesOrders.Where(x => x.DirectSalesOrderContents != null)
                .SelectMany(x => x.DirectSalesOrderContents).Select(x => new UnitOfMeasure { Id = x.PrimaryUnitOfMeasureId }));
            UnitOfMeasures.AddRange(DirectSalesOrders.Where(x => x.DirectSalesOrderPromotions != null)
                .SelectMany(x => x.DirectSalesOrderPromotions).Select(x => new UnitOfMeasure { Id = x.UnitOfMeasureId }));
            UnitOfMeasures.AddRange(DirectSalesOrders.Where(x => x.DirectSalesOrderPromotions != null)
                .SelectMany(x => x.DirectSalesOrderPromotions).Select(x => new UnitOfMeasure { Id = x.PrimaryUnitOfMeasureId }));
            UnitOfMeasures = UnitOfMeasures.Distinct().ToList();

            RabbitManager.PublishList(DirectSalesOrders, RoutingKeyEnum.DirectSalesOrderSync);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed);
            RabbitManager.PublishList(Organizations, RoutingKeyEnum.OrganizationUsed);
            RabbitManager.PublishList(Stores, RoutingKeyEnum.StoreUsed);
            RabbitManager.PublishList(PromotionCodes, RoutingKeyEnum.PromotionCodeUsed);
            RabbitManager.PublishList(Items, RoutingKeyEnum.ItemUsed);
            RabbitManager.PublishList(UnitOfMeasures, RoutingKeyEnum.UnitOfMeasureUsed);
        }
    }
}
