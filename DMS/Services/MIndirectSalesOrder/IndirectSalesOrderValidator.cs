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
            BuyerStoreEmpty,
            SellerStoreEmpty,
            SaleEmployeeNotExisted,
            SaleEmployeeEmpty,
            OrderDateEmpty,
            EditedPriceStatusNotExisted,
            PriceOutOfRange,
            UnitOfMeasureNotExisted,
            PrimaryUnitOfMeasureNotExisted,
            QuantityEmpty,
            ItemNotExisted,
            QuantityInvalid
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
            if(IndirectSalesOrder.BuyerStoreId == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreEmpty);
            else
            {
                StoreFilter StoreFilter = new StoreFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = IndirectSalesOrder.BuyerStoreId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = StoreSelect.Id
                };

                int count = await UOW.StoreRepository.Count(StoreFilter);
                if (count == 0)
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreNotExisted);
            }

            if (IndirectSalesOrder.SellerStoreId == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SellerStore), ErrorCode.SellerStoreEmpty);
            else
            {
                StoreFilter StoreFilter = new StoreFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = IndirectSalesOrder.SellerStoreId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = StoreSelect.Id
                };

                int count = await UOW.StoreRepository.Count(StoreFilter);
                if (count == 0)
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SellerStore), ErrorCode.SellerStoreNotExisted);
            }
            
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEmployee(IndirectSalesOrder IndirectSalesOrder)
        {
            if(IndirectSalesOrder.SaleEmployeeId == 0)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeEmpty);
            else
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = IndirectSalesOrder.SaleEmployeeId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = AppUserSelect.Id
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count == 0)
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeNotExisted);
            }
            
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateOrderDate(IndirectSalesOrder IndirectSalesOrder)
        {
            if(IndirectSalesOrder.OrderDate == default(DateTime))
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.OrderDate), ErrorCode.OrderDateEmpty);
            return IndirectSalesOrder.IsValidated;
        }
        private async Task<bool> ValidateEditedPrice(IndirectSalesOrder IndirectSalesOrder)
        {
            if (EditedPriceStatusEnum.ACTIVE.Id != IndirectSalesOrder.EditedPriceStatusId && EditedPriceStatusEnum.INACTIVE.Id != IndirectSalesOrder.EditedPriceStatusId)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.EditedPriceStatus), ErrorCode.EditedPriceStatusNotExisted);

            else
            {
                var oldData = await UOW.IndirectSalesOrderRepository.Get(IndirectSalesOrder.Id);

                if (IndirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.ACTIVE.Id)
                {
                    if (IndirectSalesOrder.IndirectSalesOrderContents != null)
                    {
                        foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                        {
                            var oldDataContent = oldData.IndirectSalesOrderContents.Where(x => x.Id == IndirectSalesOrderContent.Id).FirstOrDefault();
                            if (oldDataContent != null)
                            {
                                if (IndirectSalesOrderContent.Amount > 1.1 * oldDataContent.Amount || IndirectSalesOrderContent.Amount < 0.9 * oldDataContent.Amount)
                                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Amount), ErrorCode.PriceOutOfRange);
                            }
                        }
                    }
                }
            }

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
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = UnitOfMeasureSelect.Id
                };

                var listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderContent.UnitOfMeasureId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
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
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = UnitOfMeasureSelect.Id
                };

                var listIdsInDB = (await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);

                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderPromotion.UnitOfMeasureId))
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
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
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ItemSelect.Id
                };

                var listIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);
                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if(listIdsNotExisted.Contains(IndirectSalesOrderContent.ItemId))
                        IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Item), ErrorCode.ItemNotExisted);
                    else if(IndirectSalesOrderContent.Quantity <= 0)
                    {
                        IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Quantity), ErrorCode.QuantityInvalid);
                    }
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
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ItemSelect.Id
                };

                var listIdsInDB = (await UOW.ItemRepository.List(ItemFilter)).Select(x => x.Id);
                var listIdsNotExisted = Ids.Except(listIdsInDB);
                foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder.IndirectSalesOrderPromotions)
                {
                    if (listIdsNotExisted.Contains(IndirectSalesOrderPromotion.ItemId))
                        IndirectSalesOrderPromotion.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.Item), ErrorCode.ItemNotExisted);
                    else if (IndirectSalesOrderPromotion.Quantity <= 0)
                    {
                        IndirectSalesOrderPromotion.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.Quantity), ErrorCode.QuantityInvalid);
                    }
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
