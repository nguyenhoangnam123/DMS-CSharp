using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MDirectSalesOrder
{
    public interface IDirectSalesOrderValidator : IServiceScoped
    {
        Task<bool> Create(DirectSalesOrder DirectSalesOrder);
        Task<bool> Update(DirectSalesOrder DirectSalesOrder);
        Task<bool> Delete(DirectSalesOrder DirectSalesOrder);
        Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders);
        Task<bool> Import(List<DirectSalesOrder> DirectSalesOrders);
    }

    public class DirectSalesOrderValidator : IDirectSalesOrderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            BuyerStoreNotExisted,
            SaleEmployeeNotExisted,
            OrderDateEmpty,
            EditedPriceStatusNotExisted,
            PriceOutOfRange,
            UnitOfMeasureNotExisted,
            PrimaryUnitOfMeasureNotExisted,
            QuantityEmpty,
            QuantityInvalid,
            ItemNotExisted,
            InventoryHasntEnough
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public DirectSalesOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(DirectSalesOrder DirectSalesOrder)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrder.Id },
                Selects = DirectSalesOrderSelect.Id
            };

            int count = await UOW.DirectSalesOrderRepository.Count(DirectSalesOrderFilter);
            if (count == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateStore(DirectSalesOrder DirectSalesOrder)
        {
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrder.BuyerStoreId },
                Selects = StoreSelect.Id
            };

            int count = await UOW.StoreRepository.Count(StoreFilter);
            if (count == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreNotExisted);
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEmployee(DirectSalesOrder DirectSalesOrder)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = DirectSalesOrder.SaleEmployeeId },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeNotExisted);
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateOrderDate(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.OrderDate == default(DateTime))
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.OrderDate), ErrorCode.OrderDateEmpty);
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEditedPrice(DirectSalesOrder DirectSalesOrder)
        {
            if (EditedPriceStatusEnum.ACTIVE.Id != DirectSalesOrder.EditedPriceStatusId && EditedPriceStatusEnum.INACTIVE.Id != DirectSalesOrder.EditedPriceStatusId)
                DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrder.EditedPriceStatus), ErrorCode.EditedPriceStatusNotExisted);

            else
            {
                var oldData = await UOW.DirectSalesOrderRepository.Get(DirectSalesOrder.Id);

                if (DirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.ACTIVE.Id)
                {
                    if (DirectSalesOrder.DirectSalesOrderContents != null)
                    {
                        foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                        {
                            var oldDataContent = oldData.DirectSalesOrderContents.Where(x => x.Id == DirectSalesOrderContent.Id).FirstOrDefault();
                            if (oldDataContent != null)
                            {
                                if (DirectSalesOrderContent.Amount > 1.1 * oldDataContent.Amount || DirectSalesOrderContent.Amount < 0.9 * oldDataContent.Amount)
                                    DirectSalesOrderContent.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Amount), ErrorCode.PriceOutOfRange);
                            }
                        }
                    }
                }
            }

            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateUOM(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                //validate đơn vị tính sản phẩm bán
                var Ids = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.UnitOfMeasureId).ToList();
                UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = UnitOfMeasureSelect.Id
                };

                var listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    if (listIdsNotExisted.Contains(DirectSalesOrderContent.UnitOfMeasureId))
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                }

            }

            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                //validate đơn vị tính sản phẩm khuyến mãi
                var Ids = DirectSalesOrder.DirectSalesOrderPromotions.Select(x => x.UnitOfMeasureId).ToList();
                var UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = UnitOfMeasureSelect.Id
                };

                var listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(DirectSalesOrderPromotion.UnitOfMeasureId))
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderPromotion.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                }

            }

            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateQuantity(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                var IdsItem = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).ToList();

                InventoryFilter InventoryFilter = new InventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ItemId = new IdFilter { In = IdsItem },
                    Selects = InventorySelect.SaleStock | InventorySelect.Item
                };

                var inventories = await UOW.InventoryRepository.List(InventoryFilter);
                var SaleStockOfItems = inventories.GroupBy(x => x.ItemId).Select(x => new { ItemId = x.Key, SaleStock = x.Sum(s => s.SaleStock) }).ToList();
                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    if (DirectSalesOrderContent.Quantity == 0)
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Quantity), ErrorCode.QuantityEmpty);
                    else if (DirectSalesOrderContent.Quantity < 0)
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Quantity), ErrorCode.QuantityInvalid);
                    var saleStockOfItem = SaleStockOfItems.Where(x => x.ItemId == DirectSalesOrderContent.ItemId).Select(x => x.SaleStock).FirstOrDefault();
                    if (DirectSalesOrderContent.Quantity > saleStockOfItem)
                    {
                        DirectSalesOrderContent.Item.HasInventory = false;
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Quantity), ErrorCode.InventoryHasntEnough);
                    }
                }
            }

            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                var IdsItem = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).ToList();

                InventoryFilter InventoryFilter = new InventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ItemId = new IdFilter { In = IdsItem },
                    Selects = InventorySelect.SaleStock | InventorySelect.Item
                };

                var inventories = await UOW.InventoryRepository.List(InventoryFilter);
                var SaleStockOfItems = inventories.GroupBy(x => x.ItemId).Select(x => new { ItemId = x.Key, SaleStock = x.Sum(s => s.SaleStock) }).ToList();

                foreach (var DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    if (DirectSalesOrderPromotion.Quantity == 0)
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderPromotion.Quantity), ErrorCode.QuantityEmpty);
                    else if (DirectSalesOrderPromotion.Quantity < 0)
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderPromotion.Quantity), ErrorCode.QuantityInvalid);
                    var saleStockOfItem = SaleStockOfItems.Where(x => x.ItemId == DirectSalesOrderPromotion.ItemId).Select(x => x.SaleStock).FirstOrDefault();
                    if (DirectSalesOrderPromotion.Quantity > saleStockOfItem)
                    {
                        DirectSalesOrderPromotion.Item.HasInventory = false;
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderPromotion.Quantity), ErrorCode.InventoryHasntEnough);
                    }
                }
            }
            return DirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateItem(DirectSalesOrder DirectSalesOrder)
        {
            if (DirectSalesOrder.DirectSalesOrderContents != null)
            {
                var Ids = DirectSalesOrder.DirectSalesOrderContents.Select(x => x.ItemId).ToList();
                var ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = ItemSelect.Id
                };

                var listIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);
                foreach (var DirectSalesOrderContent in DirectSalesOrder.DirectSalesOrderContents)
                {
                    if (listIdsNotExisted.Contains(DirectSalesOrderContent.ItemId))
                        DirectSalesOrderContent.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderContent.Item), ErrorCode.ItemNotExisted);
                }
            }

            if (DirectSalesOrder.DirectSalesOrderPromotions != null)
            {
                var Ids = DirectSalesOrder.DirectSalesOrderPromotions.Select(x => x.ItemId).ToList();
                var ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = ItemSelect.Id
                };

                var listIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);
                foreach (var DirectSalesOrderPromotion in DirectSalesOrder.DirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(DirectSalesOrderPromotion.ItemId))
                        DirectSalesOrder.AddError(nameof(DirectSalesOrderValidator), nameof(DirectSalesOrderPromotion.Item), ErrorCode.ItemNotExisted);
                }
            }
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Create(DirectSalesOrder DirectSalesOrder)
        {
            await ValidateStore(DirectSalesOrder);
            await ValidateEmployee(DirectSalesOrder);
            await ValidateOrderDate(DirectSalesOrder);
            await ValidateUOM(DirectSalesOrder);
            await ValidateQuantity(DirectSalesOrder);
            await ValidateItem(DirectSalesOrder);
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Update(DirectSalesOrder DirectSalesOrder)
        {
            if (await ValidateId(DirectSalesOrder))
            {
                await ValidateStore(DirectSalesOrder);
                await ValidateEmployee(DirectSalesOrder);
                await ValidateOrderDate(DirectSalesOrder);
                await ValidateEditedPrice(DirectSalesOrder);
                await ValidateUOM(DirectSalesOrder);
                await ValidateQuantity(DirectSalesOrder);
                await ValidateItem(DirectSalesOrder);
            }
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> Delete(DirectSalesOrder DirectSalesOrder)
        {
            if (await ValidateId(DirectSalesOrder))
            {
            }
            return DirectSalesOrder.IsValidated;
        }

        public async Task<bool> BulkDelete(List<DirectSalesOrder> DirectSalesOrders)
        {
            foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
            {
                await Delete(DirectSalesOrder);
            }
            return DirectSalesOrders.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<DirectSalesOrder> DirectSalesOrders)
        {
            return true;
        }
    }
}
