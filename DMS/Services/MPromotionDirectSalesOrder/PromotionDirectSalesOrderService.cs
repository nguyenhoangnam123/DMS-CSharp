using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MPromotionDirectSalesOrder
{
    public interface IPromotionDirectSalesOrderService :  IServiceScoped
    {
        Task<int> Count(PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter);
        Task<List<PromotionDirectSalesOrder>> List(PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter);
        Task<PromotionDirectSalesOrder> Get(long Id);
        Task<PromotionDirectSalesOrder> Create(PromotionDirectSalesOrder PromotionDirectSalesOrder);
        Task<PromotionDirectSalesOrder> Update(PromotionDirectSalesOrder PromotionDirectSalesOrder);
        Task<PromotionDirectSalesOrder> Delete(PromotionDirectSalesOrder PromotionDirectSalesOrder);
        Task<List<PromotionDirectSalesOrder>> BulkDelete(List<PromotionDirectSalesOrder> PromotionDirectSalesOrders);
        Task<List<PromotionDirectSalesOrder>> Import(List<PromotionDirectSalesOrder> PromotionDirectSalesOrders);
        Task<PromotionDirectSalesOrderFilter> ToFilter(PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter);
    }

    public class PromotionDirectSalesOrderService : BaseService, IPromotionDirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPromotionDirectSalesOrderValidator PromotionDirectSalesOrderValidator;

        public PromotionDirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IPromotionDirectSalesOrderValidator PromotionDirectSalesOrderValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PromotionDirectSalesOrderValidator = PromotionDirectSalesOrderValidator;
        }
        public async Task<int> Count(PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter)
        {
            try
            {
                int result = await UOW.PromotionDirectSalesOrderRepository.Count(PromotionDirectSalesOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionDirectSalesOrder>> List(PromotionDirectSalesOrderFilter PromotionDirectSalesOrderFilter)
        {
            try
            {
                List<PromotionDirectSalesOrder> PromotionDirectSalesOrders = await UOW.PromotionDirectSalesOrderRepository.List(PromotionDirectSalesOrderFilter);
                return PromotionDirectSalesOrders;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<PromotionDirectSalesOrder> Get(long Id)
        {
            PromotionDirectSalesOrder PromotionDirectSalesOrder = await UOW.PromotionDirectSalesOrderRepository.Get(Id);
            if (PromotionDirectSalesOrder == null)
                return null;
            return PromotionDirectSalesOrder;
        }
       
        public async Task<PromotionDirectSalesOrder> Create(PromotionDirectSalesOrder PromotionDirectSalesOrder)
        {
            if (!await PromotionDirectSalesOrderValidator.Create(PromotionDirectSalesOrder))
                return PromotionDirectSalesOrder;

            try
            {
                await UOW.Begin();
                await UOW.PromotionDirectSalesOrderRepository.Create(PromotionDirectSalesOrder);
                await UOW.Commit();
                PromotionDirectSalesOrder = await UOW.PromotionDirectSalesOrderRepository.Get(PromotionDirectSalesOrder.Id);
                await Logging.CreateAuditLog(PromotionDirectSalesOrder, new { }, nameof(PromotionDirectSalesOrderService));
                return PromotionDirectSalesOrder;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionDirectSalesOrder> Update(PromotionDirectSalesOrder PromotionDirectSalesOrder)
        {
            if (!await PromotionDirectSalesOrderValidator.Update(PromotionDirectSalesOrder))
                return PromotionDirectSalesOrder;
            try
            {
                var oldData = await UOW.PromotionDirectSalesOrderRepository.Get(PromotionDirectSalesOrder.Id);

                await UOW.Begin();
                await UOW.PromotionDirectSalesOrderRepository.Update(PromotionDirectSalesOrder);
                await UOW.Commit();

                PromotionDirectSalesOrder = await UOW.PromotionDirectSalesOrderRepository.Get(PromotionDirectSalesOrder.Id);
                await Logging.CreateAuditLog(PromotionDirectSalesOrder, oldData, nameof(PromotionDirectSalesOrderService));
                return PromotionDirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<PromotionDirectSalesOrder> Delete(PromotionDirectSalesOrder PromotionDirectSalesOrder)
        {
            if (!await PromotionDirectSalesOrderValidator.Delete(PromotionDirectSalesOrder))
                return PromotionDirectSalesOrder;

            try
            {
                await UOW.Begin();
                await UOW.PromotionDirectSalesOrderRepository.Delete(PromotionDirectSalesOrder);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionDirectSalesOrder, nameof(PromotionDirectSalesOrderService));
                return PromotionDirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<PromotionDirectSalesOrder>> BulkDelete(List<PromotionDirectSalesOrder> PromotionDirectSalesOrders)
        {
            if (!await PromotionDirectSalesOrderValidator.BulkDelete(PromotionDirectSalesOrders))
                return PromotionDirectSalesOrders;

            try
            {
                await UOW.Begin();
                await UOW.PromotionDirectSalesOrderRepository.BulkDelete(PromotionDirectSalesOrders);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, PromotionDirectSalesOrders, nameof(PromotionDirectSalesOrderService));
                return PromotionDirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        
        public async Task<List<PromotionDirectSalesOrder>> Import(List<PromotionDirectSalesOrder> PromotionDirectSalesOrders)
        {
            if (!await PromotionDirectSalesOrderValidator.Import(PromotionDirectSalesOrders))
                return PromotionDirectSalesOrders;
            try
            {
                await UOW.Begin();
                await UOW.PromotionDirectSalesOrderRepository.BulkMerge(PromotionDirectSalesOrders);
                await UOW.Commit();

                await Logging.CreateAuditLog(PromotionDirectSalesOrders, new { }, nameof(PromotionDirectSalesOrderService));
                return PromotionDirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(PromotionDirectSalesOrderService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }     
        
        public async Task<PromotionDirectSalesOrderFilter> ToFilter(PromotionDirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PromotionDirectSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PromotionDirectSalesOrderFilter subFilter = new PromotionDirectSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        
                        
                        
                        
                        
                        
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionId))
                        subFilter.PromotionId = FilterBuilder.Merge(subFilter.PromotionId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        
                        
                        
                        
                        
                        
                        subFilter.Note = FilterBuilder.Merge(subFilter.Note, FilterPermissionDefinition.StringFilter);
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.FromValue))
                        
                        
                        subFilter.FromValue = FilterBuilder.Merge(subFilter.FromValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ToValue))
                        
                        
                        subFilter.ToValue = FilterBuilder.Merge(subFilter.ToValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PromotionDiscountTypeId))
                        subFilter.PromotionDiscountTypeId = FilterBuilder.Merge(subFilter.PromotionDiscountTypeId, FilterPermissionDefinition.IdFilter);                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountPercentage))
                        
                        
                        subFilter.DiscountPercentage = FilterBuilder.Merge(subFilter.DiscountPercentage, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DiscountValue))
                        
                        
                        subFilter.DiscountValue = FilterBuilder.Merge(subFilter.DiscountValue, FilterPermissionDefinition.DecimalFilter);
                        
                        
                        
                        
                        
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
