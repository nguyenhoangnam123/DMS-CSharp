﻿using DMS.Common;
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
using DMS.DWEntities;

namespace DMS.Services.MShowingOrderWithDraw
{
    public interface IShowingOrderWithDrawService : IServiceScoped
    {
        Task<int> Count(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter);
        Task<List<ShowingOrderWithDraw>> List(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter);
        Task<List<ShowingItem>> ListShowingItem(ShowingItemFilter ShowingItemFilter);
        Task<ShowingOrderWithDraw> Get(long Id);
        Task<ShowingOrderWithDraw> Create(ShowingOrderWithDraw ShowingOrderWithDraw);
        Task<ShowingOrderWithDraw> Update(ShowingOrderWithDraw ShowingOrderWithDraw);
        Task<ShowingOrderWithDraw> Delete(ShowingOrderWithDraw ShowingOrderWithDraw);
        Task<List<ShowingOrderWithDraw>> BulkDelete(List<ShowingOrderWithDraw> ShowingOrderWithDraws);
        Task<List<ShowingOrderWithDraw>> Import(List<ShowingOrderWithDraw> ShowingOrderWithDraws);
        Task<ShowingOrderWithDrawFilter> ToFilter(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter);
    }

    public class ShowingOrderWithDrawService : BaseService, IShowingOrderWithDrawService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IShowingOrderWithDrawValidator ShowingOrderWithDrawValidator;
        private IRabbitManager RabbitManager;

        public ShowingOrderWithDrawService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingOrderWithDrawValidator ShowingOrderWithDrawValidator,
            ILogging Logging,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingOrderWithDrawValidator = ShowingOrderWithDrawValidator;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter)
        {
            try
            {
                int result = await UOW.ShowingOrderWithDrawRepository.Count(ShowingOrderWithDrawFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return 0;
        }

        public async Task<List<ShowingOrderWithDraw>> List(ShowingOrderWithDrawFilter ShowingOrderWithDrawFilter)
        {
            try
            {
                List<ShowingOrderWithDraw> ShowingOrderWithDraws = await UOW.ShowingOrderWithDrawRepository.List(ShowingOrderWithDrawFilter);
                return ShowingOrderWithDraws;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;
        }

        public async Task<List<ShowingItem>> ListShowingItem(ShowingItemFilter ShowingItemFilter)
        {
            try
            {
                List<ShowingItem> ShowingItems = await UOW.ShowingItemRepository.List(ShowingItemFilter);
                return ShowingItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;
        }

        public async Task<ShowingOrderWithDraw> Get(long Id)
        {
            ShowingOrderWithDraw ShowingOrderWithDraw = await UOW.ShowingOrderWithDrawRepository.Get(Id);
            if (ShowingOrderWithDraw == null)
                return null;
            return ShowingOrderWithDraw;
        }
        public async Task<ShowingOrderWithDraw> Create(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            if (!await ShowingOrderWithDrawValidator.Create(ShowingOrderWithDraw))
                return ShowingOrderWithDraw;

            try
            {
                await Calculator(ShowingOrderWithDraw);
                List<ShowingOrderWithDraw> ShowingOrderWithDraws = new List<ShowingOrderWithDraw>();
                if (ShowingOrderWithDraw.Stores != null && ShowingOrderWithDraw.Stores.Any())
                {
                    foreach (var Store in ShowingOrderWithDraw.Stores)
                    {
                        var newObj = Utils.Clone(ShowingOrderWithDraw);
                        newObj.StoreId = Store.Id;
                        newObj.AppUserId = CurrentContext.UserId;
                        newObj.RowId = Guid.NewGuid();
                        ShowingOrderWithDraws.Add(newObj);
                    }
                }
                await UOW.ShowingOrderWithDrawRepository.BulkMerge(ShowingOrderWithDraws);
                NotifyUsed(ShowingOrderWithDraw);
                Sync(ShowingOrderWithDraws, RoutingKeyEnum.POSMTransactionCreate);
                await Logging.CreateAuditLog(ShowingOrderWithDraw, new { }, nameof(ShowingOrderWithDrawService));
                return ShowingOrderWithDraw;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;
        }

        public async Task<ShowingOrderWithDraw> Update(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            if (!await ShowingOrderWithDrawValidator.Update(ShowingOrderWithDraw))
                return ShowingOrderWithDraw;
            try
            {
                var oldData = await UOW.ShowingOrderWithDrawRepository.Get(ShowingOrderWithDraw.Id);
                await Calculator(ShowingOrderWithDraw);
                await UOW.ShowingOrderWithDrawRepository.Update(ShowingOrderWithDraw);

                NotifyUsed(ShowingOrderWithDraw);
                Sync(new List<ShowingOrderWithDraw> {
                    ShowingOrderWithDraw
                }, RoutingKeyEnum.POSMTransactionUpdate);
                ShowingOrderWithDraw = await UOW.ShowingOrderWithDrawRepository.Get(ShowingOrderWithDraw.Id);
                await Logging.CreateAuditLog(ShowingOrderWithDraw, oldData, nameof(ShowingOrderWithDrawService));
                return ShowingOrderWithDraw;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;
        }

        public async Task<ShowingOrderWithDraw> Delete(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            if (!await ShowingOrderWithDrawValidator.Delete(ShowingOrderWithDraw))
                return ShowingOrderWithDraw;

            try
            {
                await UOW.ShowingOrderWithDrawRepository.Delete(ShowingOrderWithDraw);
                Sync(new List<ShowingOrderWithDraw> {
                    ShowingOrderWithDraw
                }, RoutingKeyEnum.POSMTransactionDelete);
                await Logging.CreateAuditLog(new { }, ShowingOrderWithDraw, nameof(ShowingOrderWithDrawService));
                return ShowingOrderWithDraw;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;
        }

        public async Task<List<ShowingOrderWithDraw>> BulkDelete(List<ShowingOrderWithDraw> ShowingOrderWithDraws)
        {
            if (!await ShowingOrderWithDrawValidator.BulkDelete(ShowingOrderWithDraws))
                return ShowingOrderWithDraws;

            try
            {
                await UOW.ShowingOrderWithDrawRepository.BulkDelete(ShowingOrderWithDraws);
                await Logging.CreateAuditLog(new { }, ShowingOrderWithDraws, nameof(ShowingOrderWithDrawService));
                return ShowingOrderWithDraws;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;

        }

        public async Task<List<ShowingOrderWithDraw>> Import(List<ShowingOrderWithDraw> ShowingOrderWithDraws)
        {
            if (!await ShowingOrderWithDrawValidator.Import(ShowingOrderWithDraws))
                return ShowingOrderWithDraws;
            try
            {
                await UOW.ShowingOrderWithDrawRepository.BulkMerge(ShowingOrderWithDraws);

                await Logging.CreateAuditLog(ShowingOrderWithDraws, new { }, nameof(ShowingOrderWithDrawService));
                return ShowingOrderWithDraws;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingOrderWithDrawService));
            }
            return null;
        }

        public async Task<ShowingOrderWithDrawFilter> ToFilter(ShowingOrderWithDrawFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingOrderWithDrawFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingOrderWithDrawFilter subFilter = new ShowingOrderWithDrawFilter();
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

        private async Task Calculator(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            var ShowingItemIds = ShowingOrderWithDraw.ShowingOrderContentWithDraws.Select(x => x.ShowingItemId).Distinct().ToList();
            var ShowingItems = await UOW.ShowingItemRepository.List(new ShowingItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ShowingItemSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                Id = new IdFilter { In = ShowingItemIds }
            });
            ShowingOrderWithDraw.Total = 0; // tính lại tổng giá trị thu hồi
            foreach (var ShowingOrderWithDrawContent in ShowingOrderWithDraw.ShowingOrderContentWithDraws)
            {
                ShowingItem ShowingItem = ShowingItems.Where(x => x.Id == ShowingOrderWithDrawContent.ShowingItemId).FirstOrDefault();
                if (ShowingItem != null)
                {
                    ShowingOrderWithDrawContent.Amount = ShowingItem.SalePrice * ShowingOrderWithDrawContent.Quantity;
                    ShowingOrderWithDraw.Total += ShowingOrderWithDrawContent.Amount;
                };
            }
        }

        private void NotifyUsed(ShowingOrderWithDraw ShowingOrderWithDraw)
        {
            {
                List<long> ShowingItemIds = ShowingOrderWithDraw.ShowingOrderContentWithDraws.Select(i => i.ShowingItemId).ToList();
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
                List<long> UOMIds = ShowingOrderWithDraw.ShowingOrderContentWithDraws.Select(i => i.UnitOfMeasureId).ToList();
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
                if(ShowingOrderWithDraw.Stores != null)
                {
                    List<long> StoreIds = ShowingOrderWithDraw.Stores.Select(x => x.Id).Distinct().ToList();
                    List<EventMessage<Store>> storeMessages = StoreIds.Select(x => new EventMessage<Store>
                    {
                        Content = new Store { Id = x },
                        EntityName = nameof(Store),
                        RowId = Guid.NewGuid(),
                        Time = StaticParams.DateTimeNow,
                    }).ToList();
                    RabbitManager.PublishList(storeMessages, RoutingKeyEnum.StoreUsed);
                }
            }

            {
                EventMessage<AppUser> AppUserMessage = new EventMessage<AppUser>
                {
                    Content = new AppUser { Id = ShowingOrderWithDraw.AppUserId },
                    EntityName = nameof(AppUser),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(AppUserMessage, RoutingKeyEnum.AppUserUsed);
            }

            {
                EventMessage<Organization> OrganizationMessage = new EventMessage<Organization>
                {
                    Content = new Organization { Id = ShowingOrderWithDraw.OrganizationId },
                    EntityName = nameof(Organization),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(OrganizationMessage, RoutingKeyEnum.OrganizationUsed);
            }
        }

        private void Sync(List<ShowingOrderWithDraw> ShowingOrderWithDraws, GenericEnum routeKey)
        {
            List<Fact_POSMTransaction> Transactions = new List<Fact_POSMTransaction>();
            foreach (ShowingOrderWithDraw Order in ShowingOrderWithDraws)
            {
                foreach (var Content in Order.ShowingOrderContentWithDraws)
                {
                    Fact_POSMTransaction Transaction = new Fact_POSMTransaction
                    {
                        ShowingOrderWithDrawId = Order.Id,
                        OrganizationId = Order.OrganizationId,
                        StoreId = Order.StoreId,
                        ItemId = Content.ShowingItemId,
                        Quantity = Content.Quantity,
                        SalePrice = Content.SalePrice,
                        UnitOfMeasureId = Content.UnitOfMeasureId,
                        Amount = Content.Amount,
                        Date = Order.Date,
                        CreatedAt = StaticParams.DateTimeNow,
                        TransactionTypeId = POSMTransactionTypeEnum.ORDER.Id
                    };
                    Transactions.Add(Transaction);
                }
            }
            List<EventMessage<Fact_POSMTransaction>> TransactionMessages = Transactions.Select(x => new EventMessage<Fact_POSMTransaction>
            {
                Content = x,
                EntityName = nameof(Fact_POSMTransaction),
                RowId = Guid.NewGuid(),
                Time = StaticParams.DateTimeNow,
            }).ToList();
            RabbitManager.PublishList(TransactionMessages, routeKey); // POSMtransaction trên DMS Report
        }
    }
}
