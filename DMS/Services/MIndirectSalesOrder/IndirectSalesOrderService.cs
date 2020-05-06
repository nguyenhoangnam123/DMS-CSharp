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

namespace DMS.Services.MIndirectSalesOrder
{
    public interface IIndirectSalesOrderService :  IServiceScoped
    {
        Task<int> Count(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter IndirectSalesOrderFilter);
        Task<IndirectSalesOrder> Get(long Id);
        Task<IndirectSalesOrder> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<IndirectSalesOrder> Delete(IndirectSalesOrder IndirectSalesOrder);
        Task<List<IndirectSalesOrder>> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<List<IndirectSalesOrder>> Import(List<IndirectSalesOrder> IndirectSalesOrders);
        IndirectSalesOrderFilter ToFilter(IndirectSalesOrderFilter IndirectSalesOrderFilter);
    }

    public class IndirectSalesOrderService : BaseService, IIndirectSalesOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IIndirectSalesOrderValidator IndirectSalesOrderValidator;

        public IndirectSalesOrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IIndirectSalesOrderValidator IndirectSalesOrderValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.IndirectSalesOrderValidator = IndirectSalesOrderValidator;
        }
        public async Task<int> Count(IndirectSalesOrderFilter IndirectSalesOrderFilter)
        {
            try
            {
                int result = await UOW.IndirectSalesOrderRepository.Count(IndirectSalesOrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrder>> List(IndirectSalesOrderFilter IndirectSalesOrderFilter)
        {
            try
            {
                List<IndirectSalesOrder> IndirectSalesOrders = await UOW.IndirectSalesOrderRepository.List(IndirectSalesOrderFilter);
                return IndirectSalesOrders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<IndirectSalesOrder> Get(long Id)
        {
            IndirectSalesOrder IndirectSalesOrder = await UOW.IndirectSalesOrderRepository.Get(Id);
            if (IndirectSalesOrder == null)
                return null;
            return IndirectSalesOrder;
        }
       
        public async Task<IndirectSalesOrder> Create(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Create(IndirectSalesOrder))
                return IndirectSalesOrder;

            try
            {
                await Calculator(IndirectSalesOrder);
                await UOW.Begin();
                IndirectSalesOrder.Code = IndirectSalesOrder.Id.ToString();
                await UOW.IndirectSalesOrderRepository.Create(IndirectSalesOrder);
                IndirectSalesOrder.Code = IndirectSalesOrder.Id.ToString();
                await UOW.IndirectSalesOrderRepository.Update(IndirectSalesOrder);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrder, new { }, nameof(IndirectSalesOrderService));
                return await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectSalesOrder> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Update(IndirectSalesOrder))
                return IndirectSalesOrder;
            try
            {
                var oldData = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
                await Calculator(IndirectSalesOrder);
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.Update(IndirectSalesOrder);
                await UOW.Commit();

                var newData = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(IndirectSalesOrderService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<IndirectSalesOrder> Delete(IndirectSalesOrder IndirectSalesOrder)
        {
            if (!await IndirectSalesOrderValidator.Delete(IndirectSalesOrder))
                return IndirectSalesOrder;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.Delete(IndirectSalesOrder);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrder, nameof(IndirectSalesOrderService));
                return IndirectSalesOrder;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<IndirectSalesOrder>> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            if (!await IndirectSalesOrderValidator.BulkDelete(IndirectSalesOrders))
                return IndirectSalesOrders;

            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.BulkDelete(IndirectSalesOrders);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, IndirectSalesOrders, nameof(IndirectSalesOrderService));
                return IndirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<IndirectSalesOrder>> Import(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            if (!await IndirectSalesOrderValidator.Import(IndirectSalesOrders))
                return IndirectSalesOrders;
            try
            {
                await UOW.Begin();
                await UOW.IndirectSalesOrderRepository.BulkMerge(IndirectSalesOrders);
                await UOW.Commit();

                await Logging.CreateAuditLog(IndirectSalesOrders, new { }, nameof(IndirectSalesOrderService));
                return IndirectSalesOrders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(IndirectSalesOrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public IndirectSalesOrderFilter ToFilter(IndirectSalesOrderFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<IndirectSalesOrderFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                IndirectSalesOrderFilter subFilter = new IndirectSalesOrderFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.BuyerStoreId))
                        subFilter.BuyerStoreId = Map(subFilter.BuyerStoreId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PhoneNumber))
                        subFilter.PhoneNumber = Map(subFilter.PhoneNumber, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreAddress))
                        subFilter.StoreAddress = Map(subFilter.StoreAddress, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryAddress))
                        subFilter.DeliveryAddress = Map(subFilter.DeliveryAddress, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SellerStoreId))
                        subFilter.SellerStoreId = Map(subFilter.SellerStoreId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleEmployeeId))
                        subFilter.SaleEmployeeId = Map(subFilter.SaleEmployeeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrderDate))
                        subFilter.OrderDate = Map(subFilter.OrderDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DeliveryDate))
                        subFilter.DeliveryDate = Map(subFilter.DeliveryDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.IndirectSalesOrderStatusId))
                        subFilter.IndirectSalesOrderStatusId = Map(subFilter.IndirectSalesOrderStatusId, FilterPermissionDefinition);
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
                }
            }
            return filter;
        }

        private async Task<IndirectSalesOrder> Calculator(IndirectSalesOrder IndirectSalesOrder)
        {
            //sản phẩm bán
            if(IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    var item = await UOW.ItemRepository.Get(IndirectSalesOrderContent.ItemId);
                    IndirectSalesOrderContent.PrimaryUnitOfMeasureId = item.Product.UnitOfMeasureId;

                    var UOMG = await UOW.UnitOfMeasureGroupingRepository.Get(item.Product.UnitOfMeasureGroupingId.Value);
                    var UOMGC = UOMG.UnitOfMeasureGroupingContents.Where(x => x.UnitOfMeasureId == IndirectSalesOrderContent.UnitOfMeasureId).FirstOrDefault();
                    IndirectSalesOrderContent.RequestedQuantity = IndirectSalesOrderContent.Quantity * UOMGC.Factor.Value;

                    //giá tiền từng line = số lượng yc*đơn giá*(100-%chiết khấu)
                    IndirectSalesOrderContent.Amount = Convert.ToInt64((IndirectSalesOrderContent.RequestedQuantity * IndirectSalesOrderContent.SalePrice) * ((100 - IndirectSalesOrderContent.DiscountPercentage.Value) / 100));
                }

                //tổng trước chiết khấu
                IndirectSalesOrder.SubTotal = IndirectSalesOrder.IndirectSalesOrderContents.Sum(x => x.Amount);

                //tính tổng chiết khấu theo % chiết khấu chung
                if (IndirectSalesOrder.GeneralDiscountPercentage.HasValue)
                {
                    IndirectSalesOrder.GeneralDiscountAmount = Convert.ToInt64(IndirectSalesOrder.SubTotal * IndirectSalesOrder.GeneralDiscountPercentage);
                }

                if (IndirectSalesOrder.GeneralDiscountAmount.HasValue && IndirectSalesOrder.GeneralDiscountAmount > 0)
                {
                    foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                    {
                        //phân bổ chiết khấu chung = tổng chiết khấu chung * (tổng từng line/tổng trc chiết khấu)
                        IndirectSalesOrderContent.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount * (IndirectSalesOrderContent.Amount / IndirectSalesOrder.SubTotal);
                        //thuê từng line = (tổng từng line - chiết khấu phân bổ) * % thuế
                        IndirectSalesOrderContent.TaxAmount = Convert.ToInt64((IndirectSalesOrderContent.Amount - IndirectSalesOrderContent.GeneralDiscountAmount) * IndirectSalesOrderContent.TaxPercentage);
                    }
                }
                IndirectSalesOrder.TotalTaxAmount = IndirectSalesOrder.IndirectSalesOrderContents.Sum(x => x.TaxAmount.Value);
                IndirectSalesOrder.Total = IndirectSalesOrder.SubTotal - IndirectSalesOrder.GeneralDiscountAmount ?? 0 + IndirectSalesOrder.TotalTaxAmount;
            }

            //sản phẩm khuyến mãi
            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    var item = await UOW.ItemRepository.Get(IndirectSalesOrderPromotion.ItemId);
                    IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId = item.Product.UnitOfMeasureId;

                    var UOMG = await UOW.UnitOfMeasureGroupingRepository.Get(item.Product.UnitOfMeasureGroupingId.Value);
                    var UOMGC = UOMG.UnitOfMeasureGroupingContents.Where(x => x.UnitOfMeasureId == IndirectSalesOrderPromotion.UnitOfMeasureId).FirstOrDefault();
                    IndirectSalesOrderPromotion.RequestedQuantity = IndirectSalesOrderPromotion.Quantity * UOMGC.Factor.Value;
                }
            }
            
            return IndirectSalesOrder;
        }
    }
}
