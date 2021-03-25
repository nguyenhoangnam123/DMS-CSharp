using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;

namespace DMS.Services.MShowingOrder
{
    public interface IShowingOrderService : IServiceScoped
    {
        Task<int> Count(ShowingOrderFilter ShowingOrderFilter);
        Task<List<ShowingOrder>> List(ShowingOrderFilter ShowingOrderFilter);
        Task<List<ShowingItem>> ListShowingItem(ShowingItemFilter ShowingItemFilter);
        Task<ShowingOrder> Get(long Id);
        Task<ShowingOrder> Create(ShowingOrder ShowingOrder);
        Task<ShowingOrder> Update(ShowingOrder ShowingOrder);
        Task<ShowingOrder> Delete(ShowingOrder ShowingOrder);
        Task<List<ShowingOrder>> BulkDelete(List<ShowingOrder> ShowingOrders);
        Task<List<ShowingOrder>> Import(List<ShowingOrder> ShowingOrders);
        Task<ShowingOrderFilter> ToFilter(ShowingOrderFilter ShowingOrderFilter);
    }

    public class ShowingOrderService : BaseService, IShowingOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IShowingOrderValidator ShowingOrderValidator;
        private IRabbitManager RabbitManager;

        public ShowingOrderService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingOrderValidator ShowingOrderValidator,
            ILogging Logging,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingOrderValidator = ShowingOrderValidator;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ShowingOrderFilter ShowingOrderFilter)
        {
            try
            {
                int result = await UOW.ShowingOrderRepository.Count(ShowingOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return 0;
        }

        public async Task<List<ShowingOrder>> List(ShowingOrderFilter ShowingOrderFilter)
        {
            try
            {
                List<ShowingOrder> ShowingOrders = await UOW.ShowingOrderRepository.List(ShowingOrderFilter);
                return ShowingOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;
        }

        public async Task<List<ShowingItem>> ListShowingItem(ShowingItemFilter ShowingItemFilter)
        {
            try
            {
                List<ShowingItem> ShowingItems = await UOW.ShowingItemRepository.List(ShowingItemFilter);
                var Ids = ShowingItems.Select(x => x.Id).ToList();

                ShowingInventoryFilter ShowingInventoryFilter = new ShowingInventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ShowingItemId = new IdFilter { In = Ids },
                    ShowingWarehouseId = ShowingItemFilter.ShowingWarehouseId,
                    Selects = ShowingInventorySelect.SaleStock | ShowingInventorySelect.ShowingItem
                };

                var ShowingInventories = await UOW.ShowingInventoryRepository.List(ShowingInventoryFilter);
                var list = ShowingInventories.GroupBy(x => x.ShowingItemId).Select(x => new { ShowingItemId = x.Key, SaleStock = x.Sum(s => s.SaleStock) }).ToList();

                foreach (var ShowingItem in ShowingItems)
                {
                    ShowingItem.SaleStock = list.Where(i => i.ShowingItemId == ShowingItem.Id).Select(i => i.SaleStock).FirstOrDefault();
                    ShowingItem.HasInventory = ShowingItem.SaleStock > 0;
                }
                return ShowingItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;
        }

        public async Task<ShowingOrder> Get(long Id)
        {
            ShowingOrder ShowingOrder = await UOW.ShowingOrderRepository.Get(Id);
            if (ShowingOrder == null)
                return null;
            return ShowingOrder;
        }
        public async Task<ShowingOrder> Create(ShowingOrder ShowingOrder)
        {
            if (!await ShowingOrderValidator.Create(ShowingOrder))
                return ShowingOrder;

            try
            {
                await Calculator(ShowingOrder);
                List<ShowingOrder> ShowingOrders = new List<ShowingOrder>();
                if (ShowingOrder.Stores != null && ShowingOrder.Stores.Any())
                {
                    foreach (var Store in ShowingOrder.Stores)
                    {
                        var newObj = Utils.Clone(ShowingOrder);
                        newObj.StoreId = Store.Id;
                        newObj.AppUserId = CurrentContext.UserId;
                        newObj.RowId = Guid.NewGuid();
                        ShowingOrders.Add(newObj);
                    }
                }
                await UOW.ShowingOrderRepository.BulkMerge(ShowingOrders);

                #region cập nhật lại tồn kho
                var ShowingItemIds = ShowingOrder.ShowingOrderContents.Select(x => x.ShowingItemId).ToList();
                var ShowingInventories = await UOW.ShowingInventoryRepository.List(new ShowingInventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ShowingItemId = new IdFilter { In = ShowingItemIds },
                    ShowingWarehouseId = new IdFilter { Equal = ShowingOrder.ShowingWarehouse.Id },
                    Selects = ShowingInventorySelect.SaleStock | ShowingInventorySelect.ShowingItem
                });

                var StoreCounter = ShowingOrder.Stores.Count();
                foreach (var ShowingOrderContent in ShowingOrder.ShowingOrderContents)
                {
                    ShowingInventory ShowingInventory = ShowingInventories.Where(x => x.ShowingItemId == ShowingOrderContent.ShowingItemId).FirstOrDefault();
                    if (ShowingInventory != null)
                    {
                        ShowingInventory.SaleStock -= (ShowingOrderContent.Quantity * StoreCounter);
                        if (ShowingInventory.AccountingStock.HasValue)
                            ShowingInventory.AccountingStock -= (ShowingOrderContent.Quantity * StoreCounter);
                    }
                }
                await UOW.ShowingInventoryRepository.BulkMerge(ShowingInventories);
                #endregion

                NotifyUsed(ShowingOrder);
                await Logging.CreateAuditLog(ShowingOrder, new { }, nameof(ShowingOrderService));
                return ShowingOrder;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;
        }

        public async Task<ShowingOrder> Update(ShowingOrder ShowingOrder)
        {
            if (!await ShowingOrderValidator.Update(ShowingOrder))
                return ShowingOrder;
            try
            {
                var oldData = await UOW.ShowingOrderRepository.Get(ShowingOrder.Id);
                await Calculator(ShowingOrder);
                await UOW.ShowingOrderRepository.Update(ShowingOrder);

                #region cập nhật lại tồn kho
                var OldShowingItemIds = oldData.ShowingOrderContents.Select(x => x.ShowingItemId).ToList();
                var NewShowingItemIds = ShowingOrder.ShowingOrderContents.Select(x => x.ShowingItemId).ToList();
                var ShowingItemIds = new List<long>();
                ShowingItemIds.AddRange(NewShowingItemIds);
                ShowingItemIds.AddRange(OldShowingItemIds);
                ShowingItemIds = ShowingItemIds.Distinct().ToList();
                var ShowingInventories = await UOW.ShowingInventoryRepository.List(new ShowingInventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ShowingItemId = new IdFilter { In = ShowingItemIds },
                    ShowingWarehouseId = new IdFilter { Equal = ShowingOrder.ShowingWarehouse.Id },
                    Selects = ShowingInventorySelect.SaleStock | ShowingInventorySelect.ShowingItem
                });

                foreach (var ShowingOrderContent in oldData.ShowingOrderContents)
                {
                    //cộng trả lại tồn kho của các các item cũ
                    ShowingInventory ShowingInventory = ShowingInventories.Where(x => x.ShowingItemId == ShowingOrderContent.ShowingItemId).FirstOrDefault();
                    if (ShowingInventory != null)
                    {
                        ShowingInventory.SaleStock += ShowingOrderContent.Quantity;
                        if (ShowingInventory.AccountingStock.HasValue)
                            ShowingInventory.AccountingStock += ShowingOrderContent.Quantity;
                    }
                }
                foreach (var ShowingOrderContent in ShowingOrder.ShowingOrderContents)
                {
                    //trừ lại tồn kho
                    ShowingInventory ShowingInventory = ShowingInventories.Where(x => x.ShowingItemId == ShowingOrderContent.ShowingItemId).FirstOrDefault();
                    if (ShowingInventory != null)
                    {
                        ShowingInventory.SaleStock -= ShowingOrderContent.Quantity;
                        if (ShowingInventory.AccountingStock.HasValue)
                            ShowingInventory.AccountingStock -= ShowingOrderContent.Quantity;
                    }
                }
                await UOW.ShowingInventoryRepository.BulkMerge(ShowingInventories);
                #endregion

                NotifyUsed(ShowingOrder);
                ShowingOrder = await UOW.ShowingOrderRepository.Get(ShowingOrder.Id);
                await Logging.CreateAuditLog(ShowingOrder, oldData, nameof(ShowingOrderService));
                return ShowingOrder;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;
        }

        public async Task<ShowingOrder> Delete(ShowingOrder ShowingOrder)
        {
            if (!await ShowingOrderValidator.Delete(ShowingOrder))
                return ShowingOrder;

            try
            {
                await UOW.ShowingOrderRepository.Delete(ShowingOrder);
                await Logging.CreateAuditLog(new { }, ShowingOrder, nameof(ShowingOrderService));
                return ShowingOrder;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;
        }

        public async Task<List<ShowingOrder>> BulkDelete(List<ShowingOrder> ShowingOrders)
        {
            if (!await ShowingOrderValidator.BulkDelete(ShowingOrders))
                return ShowingOrders;

            try
            {
                await UOW.ShowingOrderRepository.BulkDelete(ShowingOrders);
                await Logging.CreateAuditLog(new { }, ShowingOrders, nameof(ShowingOrderService));
                return ShowingOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;

        }

        public async Task<List<ShowingOrder>> Import(List<ShowingOrder> ShowingOrders)
        {
            if (!await ShowingOrderValidator.Import(ShowingOrders))
                return ShowingOrders;
            try
            {
                await UOW.ShowingOrderRepository.BulkMerge(ShowingOrders);

                await Logging.CreateAuditLog(ShowingOrders, new { }, nameof(ShowingOrderService));
                return ShowingOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderService));
            }
            return null;
        }

        public async Task<ShowingOrderFilter> ToFilter(ShowingOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingOrderFilter subFilter = new ShowingOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Date))
                        subFilter.Date = FilterBuilder.Merge(subFilter.Date, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ShowingWarehouseId))
                        subFilter.ShowingWarehouseId = FilterBuilder.Merge(subFilter.ShowingWarehouseId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Total))
                        subFilter.Total = FilterBuilder.Merge(subFilter.Total, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private async Task Calculator(ShowingOrder ShowingOrder)
        {
            var ShowingItemIds = ShowingOrder.ShowingOrderContents.Select(x => x.ShowingItemId).Distinct().ToList();
            var ShowingItems = await UOW.ShowingItemRepository.List(new ShowingItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ShowingItemSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                Id = new IdFilter { In = ShowingItemIds }
            });
            foreach (var ShowingOrderContent in ShowingOrder.ShowingOrderContents)
            {
                ShowingItem ShowingItem = ShowingItems.Where(x => x.Id == ShowingOrderContent.ShowingItemId).FirstOrDefault();
                if (ShowingItem != null)
                {
                    ShowingOrderContent.Amount = ShowingItem.SalePrice * ShowingOrderContent.Quantity;
                    ShowingOrder.Total += ShowingOrderContent.Amount;
                };
            }
        }

        private void NotifyUsed(ShowingOrder ShowingOrder)
        {
            {
                List<long> ShowingItemIds = ShowingOrder.ShowingOrderContents.Select(i => i.ShowingItemId).ToList();
                List<EventMessage<ShowingItem>> ShowingItemMessages = ShowingItemIds.Select(i => new EventMessage<ShowingItem>
                {
                    Content = new ShowingItem { Id = i },
                    EntityName = nameof(ShowingItem),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(ShowingItemMessages, RoutingKeyEnum.ShowingItemUsed);
            }

            {
                List<long> UOMIds = ShowingOrder.ShowingOrderContents.Select(i => i.UnitOfMeasureId).ToList();
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
                List<long> StoreIds = ShowingOrder.Stores.Select(x => x.Id).Distinct().ToList();
                List<EventMessage<Store>> storeMessages = StoreIds.Select(x => new EventMessage<Store>
                {
                    Content = new Store { Id = x },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                }).ToList();
                RabbitManager.PublishList(storeMessages, RoutingKeyEnum.StoreUsed);
            }

            {
                EventMessage<AppUser> AppUserMessage = new EventMessage<AppUser>
                {
                    Content = new AppUser { Id = ShowingOrder.AppUserId },
                    EntityName = nameof(AppUser),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(AppUserMessage, RoutingKeyEnum.AppUserUsed);
            }

            {
                EventMessage<Organization> OrganizationMessage = new EventMessage<Organization>
                {
                    Content = new Organization { Id = ShowingOrder.OrganizationId },
                    EntityName = nameof(Organization),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(OrganizationMessage, RoutingKeyEnum.OrganizationUsed);
            }
        }
    }
}
