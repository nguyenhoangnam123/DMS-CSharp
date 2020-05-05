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

namespace DMS.Services.MDirectSalesOrder
{
    public interface IDirectSalesOrderService :  IServiceScoped
    {
        Task<int> Count(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<List<DirectSalesOrder>> List(DirectSalesOrderFilter DirectSalesOrderFilter);
        Task<DirectSalesOrder> Get(long Id);
        Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder);
        Task<DirectSalesOrder> Delete(DirectSalesOrder DirectSalesOrder);
        Task<List<DirectSalesOrder>> BulkDelete(List<DirectSalesOrder> DirectSalesOrders);
        Task<List<DirectSalesOrder>> Import(List<DirectSalesOrder> DirectSalesOrders);
        DirectSalesOrderFilter ToFilter(DirectSalesOrderFilter DirectSalesOrderFilter);
    }

    public class DirectSalesOrderService : BaseService, IDirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDirectSalesOrderValidator DirectSalesOrderValidator;

        public DirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDirectSalesOrderValidator DirectSalesOrderValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DirectSalesOrderValidator = DirectSalesOrderValidator;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<DirectSalesOrder> Get(long Id)
        {
            DirectSalesOrder DirectSalesOrder = await UOW.DirectSalesOrderRepository.Get(Id);
            if (DirectSalesOrder == null)
                return null;
            return DirectSalesOrder;
        }
       
        public async Task<DirectSalesOrder> Create(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Create(DirectSalesOrder))
                return DirectSalesOrder;

            try
            {
                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Create(DirectSalesOrder);
                await UOW.Commit();

                await Logging.CreateAuditLog(DirectSalesOrder, new { }, nameof(DirectSalesOrderService));
                return await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DirectSalesOrder> Update(DirectSalesOrder DirectSalesOrder)
        {
            if (!await DirectSalesOrderValidator.Update(DirectSalesOrder))
                return DirectSalesOrder;
            try
            {
                var oldData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);

                await UOW.Begin();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
                await UOW.Commit();

                var newData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(DirectSalesOrderService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateAuditLog(new { }, DirectSalesOrder, nameof(DirectSalesOrderService));
                return DirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(DirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public DirectSalesOrderFilter ToFilter(DirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DirectSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                DirectSalesOrderFilter subFilter = new DirectSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.BuyerStoreId))
                        subFilter.BuyerStoreId = Map(subFilter.BuyerStoreId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StorePhone))
                        subFilter.StorePhone = Map(subFilter.StorePhone, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreAddress))
                        subFilter.StoreAddress = Map(subFilter.StoreAddress, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreDeliveryAddress))
                        subFilter.StoreDeliveryAddress = Map(subFilter.StoreDeliveryAddress, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TaxCode))
                        subFilter.TaxCode = Map(subFilter.TaxCode, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleEmployeeId))
                        subFilter.SaleEmployeeId = Map(subFilter.SaleEmployeeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderDate))
                        subFilter.OrderDate = Map(subFilter.OrderDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryDate))
                        subFilter.DeliveryDate = Map(subFilter.DeliveryDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EditedPriceStatusId))
                        subFilter.EditedPriceStatusId = Map(subFilter.EditedPriceStatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Note))
                        subFilter.Note = Map(subFilter.Note, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SubTotal))
                        subFilter.SubTotal = Map(subFilter.SubTotal, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountPercentage))
                        subFilter.GeneralDiscountPercentage = Map(subFilter.GeneralDiscountPercentage, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GeneralDiscountAmount))
                        subFilter.GeneralDiscountAmount = Map(subFilter.GeneralDiscountAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TotalTaxAmount))
                        subFilter.TotalTaxAmount = Map(subFilter.TotalTaxAmount, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Total))
                        subFilter.Total = Map(subFilter.Total, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestStateId))
                        subFilter.RequestStateId = Map(subFilter.RequestStateId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
