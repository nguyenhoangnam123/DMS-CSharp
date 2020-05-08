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
        Task<List<Item>> ListItem(ItemFilter ItemFilter, long StoreId);
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

        public async Task<List<Item>> ListItem(ItemFilter ItemFilter, long StoreId)
        {
            try
            {
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                await ApplyPrice(Items, StoreId);
                return Items;
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
                await Calculator(DirectSalesOrder);
                await UOW.Begin();
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                await UOW.DirectSalesOrderRepository.Create(DirectSalesOrder);
                DirectSalesOrder.Code = DirectSalesOrder.Id.ToString();
                await UOW.DirectSalesOrderRepository.Update(DirectSalesOrder);
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
                await Calculator(DirectSalesOrder);
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

        private async Task<DirectSalesOrder> Calculator(DirectSalesOrder DirectSalesOrder)
        {
            //sản phẩm bán
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    var item = await UOW.ItemRepository.Get(DirectSalesOrderContent.ItemId);
                    DirectSalesOrderContent.PrimaryUnitOfMeasureId = item.Product.UnitOfMeasureId;

                    var UOMG = await UOW.UnitOfMeasureGroupingRepository.Get(item.Product.UnitOfMeasureGroupingId.Value);
                    var UOMGC = UOMG.UnitOfMeasureGroupingContents.Where(x => x.UnitOfMeasureId == DirectSalesOrderContent.UnitOfMeasureId).FirstOrDefault();
                    DirectSalesOrderContent.RequestedQuantity = DirectSalesOrderContent.Quantity * UOMGC.Factor.Value;

                    //giá tiền từng line = số lượng yc*đơn giá*(100-%chiết khấu)
                    DirectSalesOrderContent.Amount = Convert.ToInt64((DirectSalesOrderContent.RequestedQuantity * DirectSalesOrderContent.Price) * ((100 - DirectSalesOrderContent.DiscountPercentage.Value) / 100));
                }

                //tổng trước chiết khấu
                DirectSalesOrder.SubTotal = DirectSalesOrder.DirectSalesOrderContents.Sum(x => x.Amount);

                //tính tổng chiết khấu theo % chiết khấu chung
                if (DirectSalesOrder.GeneralDiscountPercentage.HasValue)
                {
                    DirectSalesOrder.GeneralDiscountAmount = Convert.ToInt64(DirectSalesOrder.SubTotal * DirectSalesOrder.GeneralDiscountPercentage);
                }

                if (DirectSalesOrder.GeneralDiscountAmount.HasValue && DirectSalesOrder.GeneralDiscountAmount > 0)
                {
                    foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                    {
                        //phân bổ chiết khấu chung = tổng chiết khấu chung * (tổng từng line/tổng trc chiết khấu)
                        DirectSalesOrderContent.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount * (DirectSalesOrderContent.Amount / DirectSalesOrder.SubTotal);
                        //thuê từng line = (tổng từng line - chiết khấu phân bổ) * % thuế
                        DirectSalesOrderContent.TaxAmount = Convert.ToInt64((DirectSalesOrderContent.Amount - DirectSalesOrderContent.GeneralDiscountAmount) * DirectSalesOrderContent.TaxPercentage);
                    }
                }
                DirectSalesOrder.TotalTaxAmount = DirectSalesOrder.DirectSalesOrderContents.Sum(x => x.TaxAmount.Value);
                DirectSalesOrder.Total = DirectSalesOrder.SubTotal - DirectSalesOrder.GeneralDiscountAmount ?? 0 + DirectSalesOrder.TotalTaxAmount;
            }

            //sản phẩm khuyến mãi
            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                foreach (var DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    var item = await UOW.ItemRepository.Get(DirectSalesOrderPromotion.ItemId);
                    DirectSalesOrderPromotion.PrimaryUnitOfMeasureId = item.Product.UnitOfMeasureId;

                    var UOMG = await UOW.UnitOfMeasureGroupingRepository.Get(item.Product.UnitOfMeasureGroupingId.Value);
                    var UOMGC = UOMG.UnitOfMeasureGroupingContents.Where(x => x.UnitOfMeasureId == DirectSalesOrderPromotion.UnitOfMeasureId).FirstOrDefault();
                    DirectSalesOrderPromotion.RequestedQuantity = DirectSalesOrderPromotion.Quantity * UOMGC.Factor.Value;
                }

            }
            
            return DirectSalesOrder;
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long StoreId)
        {
            var CurrrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            var OrganizationIds = CurrrentUser.Organization.Path.Split('.').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => long.Parse(x)).OrderByDescending(x => x).ToList();
            var Store = await UOW.StoreRepository.Get(StoreId);
            var ItemIds = Items.Select(x => x.Id).ToList();
            Dictionary<long, long> result = new Dictionary<long, long>();
            DirectPriceListItemMappingFilter DirectPriceListItemMappingFilter = new DirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = DirectPriceListItemMappingSelect.ALL,
                DirectPriceListTypeId = new IdFilter { Equal = Enums.DirectPriceListTypeEnum.ALLSTORE.Id },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var DirectPriceListItemMappingAllStore = await UOW.DirectPriceListItemMappingItemMappingRepository.List(DirectPriceListItemMappingFilter);

            DirectPriceListItemMappingFilter = new DirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = DirectPriceListItemMappingSelect.ALL,
                DirectPriceListTypeId = new IdFilter { Equal = Enums.DirectPriceListTypeEnum.STOREGROUPING.Id },
                StoreGroupingId = new IdFilter { Equal = Store.StoreGroupingId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var DirectPriceListItemMappingStoreGrouping = await UOW.DirectPriceListItemMappingItemMappingRepository.List(DirectPriceListItemMappingFilter);

            DirectPriceListItemMappingFilter = new DirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = DirectPriceListItemMappingSelect.ALL,
                DirectPriceListTypeId = new IdFilter { Equal = Enums.DirectPriceListTypeEnum.STORETYPE.Id },
                StoreGroupingId = new IdFilter { Equal = Store.StoreTypeId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var DirectPriceListItemMappingStoreType = await UOW.DirectPriceListItemMappingItemMappingRepository.List(DirectPriceListItemMappingFilter);

            DirectPriceListItemMappingFilter = new DirectPriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = DirectPriceListItemMappingSelect.ALL,
                DirectPriceListTypeId = new IdFilter { Equal = Enums.DirectPriceListTypeEnum.DETAILS.Id },
                StoreId = new IdFilter { Equal = StoreId },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            };
            var DirectPriceListItemMappingStoreDetail = await UOW.DirectPriceListItemMappingItemMappingRepository.List(DirectPriceListItemMappingFilter);

            List<DirectPriceListItemMapping> DirectPriceListItemMappings = new List<DirectPriceListItemMapping>();
            DirectPriceListItemMappings.AddRange(DirectPriceListItemMappingAllStore);
            DirectPriceListItemMappings.AddRange(DirectPriceListItemMappingStoreGrouping);
            DirectPriceListItemMappings.AddRange(DirectPriceListItemMappingStoreType);
            DirectPriceListItemMappings.AddRange(DirectPriceListItemMappingStoreDetail);
            foreach (var ItemId in ItemIds)
            {
                result.Add(ItemId, long.MaxValue);
            }

            foreach (var ItemId in ItemIds)
            {
                foreach (var OrganizationId in OrganizationIds)
                {
                    long targetPrice = long.MaxValue;
                    targetPrice = DirectPriceListItemMappings.Where(x => x.ItemId == ItemId && x.DirectPriceList.OrganizationId == OrganizationId).Select(x => x.Price).DefaultIfEmpty(long.MaxValue).Min();
                    if (targetPrice < long.MaxValue)
                    {
                        result[ItemId] = targetPrice;
                        break;
                    }
                }
            }

            foreach (var ItemId in ItemIds)
            {
                if (result[ItemId] == long.MaxValue)
                {
                    result[ItemId] = Convert.ToInt64(Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice).FirstOrDefault());
                }
            }
            return Items;
        }
    }
}
