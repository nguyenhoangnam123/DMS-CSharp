using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;
using DMS.Enums;

namespace DMS.Services.MIndirectSalesOrder
{
    public interface IIndirectSalesOrderValidator : IServiceScoped
    {
        Task<bool> Create(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Update(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder);
        Task<bool> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders);
        Task<bool> Import(List<IndirectSalesOrder> IndirectSalesOrders);
    }

    public class IndirectSalesOrderValidator : IIndirectSalesOrderValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            BuyerStoreNotExisted,
            SellerStoreNotExisted,
            SaleEmployeeNotExisted,
            OrderDateEmpty,
            EditedPriceStatusNotExisted,
            PriceOutOfRange,
            UnitOfMeasureNotExisted,
            PrimaryUnitOfMeasureNotExisted,
            QuantityEmpty,
            ItemNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public IndirectSalesOrderValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(IndirectSalesOrder IndirectSalesOrder)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrder.Id },
                Selects = IndirectSalesOrderSelect.Id
            };

            int count = await UOW.IndirectSalesOrderRepository.Count(IndirectSalesOrderFilter);
            if (count == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateStore(IndirectSalesOrder IndirectSalesOrder)
        {
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrder.BuyerStoreId } ,
                Selects = StoreSelect.Id
            };

            int count = await UOW.StoreRepository.Count(StoreFilter);
            if(count == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreNotExisted);

            StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrder.SellerStoreId } ,
                Selects = StoreSelect.Id
            };

            count = await UOW.StoreRepository.Count(StoreFilter);
            if (count == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SellerStore), ErrorCode.SellerStoreNotExisted);
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEmployee(IndirectSalesOrder IndirectSalesOrder)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = IndirectSalesOrder.SaleEmployeeId },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if(count == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeNotExisted);
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateOrderDate(IndirectSalesOrder IndirectSalesOrder)
        {
            if(IndirectSalesOrder.OrderDate == default(DateTime))
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.OrderDate), ErrorCode.OrderDateEmpty);
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateUOM(IndirectSalesOrder IndirectSalesOrder)
        {
            if(IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                //validate đơn vị tính sản phẩm bán
                var Ids = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.UnitOfMeasureId).ToList();
                UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = UnitOfMeasureSelect.Id
                };

                var listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderContent.UnitOfMeasureId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                }

                //validate đơn vị lưu kho sản phẩm bán
                Ids = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.PrimaryUnitOfMeasureId).ToList();
                UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = UnitOfMeasureSelect.Id
                };

                listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderContent.PrimaryUnitOfMeasureId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.PrimaryUnitOfMeasure), ErrorCode.PrimaryUnitOfMeasureNotExisted);
                }
            }
            
            if(IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                //validate đơn vị tính sản phẩm khuyến mãi
                var Ids = IndirectSalesOrder.IndirectSalesOrderPromotions.Select(x => x.UnitOfMeasureId).ToList();
                var UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = UnitOfMeasureSelect.Id
                };

                var listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderPromotion.UnitOfMeasureId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                }

                //validate đơn vị lưu kho sản phẩm khuyến mãi
                Ids = IndirectSalesOrder.IndirectSalesOrderPromotions.Select(x => x.PrimaryUnitOfMeasureId).ToList();
                UnitOfMeasureFilter = new UnitOfMeasureFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = UnitOfMeasureSelect.Id
                };

                listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.PrimaryUnitOfMeasure), ErrorCode.PrimaryUnitOfMeasureNotExisted);
                }
            }

            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateQuantity(IndirectSalesOrder IndirectSalesOrder)
        {
            if(IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if(IndirectSalesOrderContent.Quantity <= 0)
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Quantity), ErrorCode.QuantityEmpty);
                }
            }

            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    if (IndirectSalesOrderPromotion.Quantity <= 0)
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.Quantity), ErrorCode.QuantityEmpty);
                }
            }
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateItem(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                var Ids = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).ToList();
                var ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = ItemSelect.Id
                };

                var listIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if(listIdsNotExisted.Contains(IndirectSalesOrderContent.ItemId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Item), ErrorCode.ItemNotExisted);
                }
            }

            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                var Ids = IndirectSalesOrder.IndirectSalesOrderPromotions.Select(x => x.ItemId).ToList();
                var ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids },
                    Selects = ItemSelect.Id
                };

                var listIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);
                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderPromotion.ItemId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.Item), ErrorCode.ItemNotExisted);
                }
            }
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool>Create(IndirectSalesOrder IndirectSalesOrder)
        {
            await ValidateStore(IndirectSalesOrder);
            await ValidateEmployee(IndirectSalesOrder);
            await ValidateOrderDate(IndirectSalesOrder);
            await ValidateUOM(IndirectSalesOrder);
            await ValidateQuantity(IndirectSalesOrder);
            await ValidateItem(IndirectSalesOrder);
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            if (await ValidateId(IndirectSalesOrder))
            {
                await ValidateStore(IndirectSalesOrder);
                await ValidateEmployee(IndirectSalesOrder);
                await ValidateOrderDate(IndirectSalesOrder);
                await ValidateUOM(IndirectSalesOrder);
                await ValidateQuantity(IndirectSalesOrder);
                await ValidateItem(IndirectSalesOrder);
            }
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool> Delete(IndirectSalesOrder IndirectSalesOrder)
        {
            if (await ValidateId(IndirectSalesOrder))
            {
            }
            return IndirectSalesOrder.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            foreach (IndirectSalesOrder IndirectSalesOrder in IndirectSalesOrders)
            {
                await Delete(IndirectSalesOrder);
            }
            return IndirectSalesOrders.All(st => st.IsValidated);
        }
        
        public async Task<bool> Import(List<IndirectSalesOrder> IndirectSalesOrders)
        {
            return true;
        }
    }
}
