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

namespace DMS.Services.MShowingOrder
{
    public interface IShowingOrderService :  IServiceScoped
    {
        Task<int> Count(ShowingOrderFilter ShowingOrderFilter);
        Task<List<ShowingOrder>> List(ShowingOrderFilter ShowingOrderFilter);
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

        public ShowingOrderService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingOrderValidator ShowingOrderValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingOrderValidator = ShowingOrderValidator;
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
                await UOW.ShowingOrderRepository.Create(ShowingOrder);
                ShowingOrder = await UOW.ShowingOrderRepository.Get(ShowingOrder.Id);
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

                await UOW.ShowingOrderRepository.Update(ShowingOrder);

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
    }
}
