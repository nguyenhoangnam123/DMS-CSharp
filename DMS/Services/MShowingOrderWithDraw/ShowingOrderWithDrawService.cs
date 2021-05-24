using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Helpers;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                Sync(ShowingOrderWithDraws);
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
                });
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
                });
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
                List<ShowingItem> ShowingItemMessages = ShowingItemIds.Select(i => new ShowingItem { Id = i }).ToList();
                RabbitManager.PublishList(ShowingItemMessages, RoutingKeyEnum.ShowingItemUsed);
            }

            {
                List<long> UOMIds = ShowingOrderWithDraw.ShowingOrderContentWithDraws.Select(i => i.UnitOfMeasureId).ToList();
                List<UnitOfMeasure> UnitOfMeasureMessages = UOMIds.Select(x => new UnitOfMeasure{ Id = x }).ToList();
                RabbitManager.PublishList(UnitOfMeasureMessages, RoutingKeyEnum.UnitOfMeasureUsed);
            }

            {
                if (ShowingOrderWithDraw.Stores != null)
                {
                    List<long> StoreIds = ShowingOrderWithDraw.Stores.Select(x => x.Id).Distinct().ToList();
                    List<Store> storeMessages = StoreIds.Select(x => new Store { Id = x }).ToList();
                    RabbitManager.PublishList(storeMessages, RoutingKeyEnum.StoreUsed);
                }
            }

            {
                AppUser AppUserMessage = new AppUser { Id = ShowingOrderWithDraw.AppUserId };
                RabbitManager.PublishSingle(AppUserMessage, RoutingKeyEnum.AppUserUsed);
            }

            {
                Organization OrganizationMessage = new Organization { Id = ShowingOrderWithDraw.OrganizationId };
                RabbitManager.PublishSingle(OrganizationMessage, RoutingKeyEnum.OrganizationUsed);
            }
        }

        private void Sync(List<ShowingOrderWithDraw> ShowingOrderWithDraws)
        {
            foreach (var ShowingOrderWithDraw in ShowingOrderWithDraws)
            {
                ShowingOrderWithDraw.CreatedAt = StaticParams.DateTimeNow;
                ShowingOrderWithDraw.UpdatedAt = StaticParams.DateTimeNow;
            }
            RabbitManager.PublishList(ShowingOrderWithDraws, RoutingKeyEnum.ShowingOrderWithDrawSync); // sync sang report
        }
    }
}
