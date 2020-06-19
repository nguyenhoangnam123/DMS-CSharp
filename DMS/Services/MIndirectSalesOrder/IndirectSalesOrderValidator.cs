using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            UnitOfMeasureEmpty,
            UnitOfMeasureNotExisted,
            QuantityEmpty,
            ItemNotExisted,
            QuantityInvalid,
            SellerStoreEqualBuyerStore
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
            if (IndirectSalesOrder.BuyerStoreId == 0)
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

            if(IndirectSalesOrder.SellerStoreId != 0 && IndirectSalesOrder.SellerStoreId == IndirectSalesOrder.BuyerStoreId)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SellerStore), ErrorCode.SellerStoreEqualBuyerStore);
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEmployee(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.SaleEmployeeId == 0)
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
            if (IndirectSalesOrder.OrderDate == default(DateTime))
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.OrderDate), ErrorCode.OrderDateEmpty);
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateContent(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.IndirectSalesOrderContents != null)
            {
                var ItemIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).ToList();
                var ProductIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.Item.ProductId).ToList();
                List<Item> Items = await UOW.ItemRepository.List(new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = ItemIds },
                    Selects = ItemSelect.Id | ItemSelect.SalePrice | ItemSelect.ProductId
                });

                var Products = await UOW.ProductRepository.List(new ProductFilter
                {
                    Id = new IdFilter { In = ProductIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ProductSelect.UnitOfMeasure | ProductSelect.UnitOfMeasureGrouping | ProductSelect.Id | ProductSelect.TaxType
                });

                var UOMGs = await UOW.UnitOfMeasureGroupingRepository.List(new UnitOfMeasureGroupingFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = UnitOfMeasureGroupingSelect.Id | UnitOfMeasureGroupingSelect.UnitOfMeasure | UnitOfMeasureGroupingSelect.UnitOfMeasureGroupingContents
                });

                foreach (var IndirectSalesOrderContent in IndirectSalesOrder.IndirectSalesOrderContents)
                {
                    if (IndirectSalesOrderContent.UnitOfMeasureId == 0)
                        IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.UnitOfMeasure), ErrorCode.UnitOfMeasureEmpty);
                    else
                    {
                        var Item = Items.Where(x => x.Id == IndirectSalesOrderContent.ItemId).FirstOrDefault();
                        if (Item == null)
                        {
                            IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Item), ErrorCode.ItemNotExisted);
                        }
                        else
                        {
                            var Product = Products.Where(x => Item.ProductId == x.Id).FirstOrDefault();
                            List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
                            if (Product.UnitOfMeasureGroupingId.HasValue)
                            {
                                var UOMG = UOMGs.Where(x => x.Id == Product.UnitOfMeasureGroupingId).FirstOrDefault();
                                UnitOfMeasures = UOMG.UnitOfMeasureGroupingContents.Select(x => new UnitOfMeasure
                                {
                                    Id = x.UnitOfMeasure.Id,
                                    Code = x.UnitOfMeasure.Code,
                                    Name = x.UnitOfMeasure.Name,
                                    Description = x.UnitOfMeasure.Description,
                                    StatusId = x.UnitOfMeasure.StatusId,
                                    Factor = x.Factor
                                }).ToList();
                            }

                            UnitOfMeasures.Add(new UnitOfMeasure
                            {
                                Id = Product.UnitOfMeasure.Id,
                                Code = Product.UnitOfMeasure.Code,
                                Name = Product.UnitOfMeasure.Name,
                                Description = Product.UnitOfMeasure.Description,
                                StatusId = Product.UnitOfMeasure.StatusId,
                                Factor = 1
                            });

                            var UOM = UnitOfMeasures.Where(x => IndirectSalesOrderContent.UnitOfMeasureId == x.Id).FirstOrDefault();
                            if (UOM == null)
                                IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.UnitOfMeasure), ErrorCode.UnitOfMeasureNotExisted);
                            else
                            {
                                if(IndirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.ACTIVE.Id)
                                {
                                    if(IndirectSalesOrderContent.SalePrice < (Item.SalePrice * UOM.Factor.Value) * 0.9m
                                        || IndirectSalesOrderContent.SalePrice > (Item.SalePrice * UOM.Factor.Value) * 1.1m)
                                        IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.SalePrice), ErrorCode.PriceOutOfRange);
                                }
                            }
                        }

                        if (IndirectSalesOrderContent.Quantity <= 0)
                            IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Quantity), ErrorCode.QuantityEmpty);

                    }
                    
                }
            }

            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidatePromotion(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.IndirectSalesOrderPromotions != null)
            {
                await ValidateItem(IndirectSalesOrder);
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
                        IndirectSalesOrderPromotion.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.UnitOfMeasure), ErrorCode.UnitOfMeasureEmpty);
                    //validate số lượng
                    if (IndirectSalesOrderPromotion.Quantity <= 0)
                        IndirectSalesOrderPromotion.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderPromotion.Quantity), ErrorCode.QuantityEmpty);
                }
                
            }
            return IndirectSalesOrder.IsValidated;
        }


        private async Task<bool> ValidateItem(IndirectSalesOrder IndirectSalesOrder)
        {
            
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
                }
            }
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateEditedPrice(IndirectSalesOrder IndirectSalesOrder)
        {
            if (EditedPriceStatusEnum.ACTIVE.Id != IndirectSalesOrder.EditedPriceStatusId && EditedPriceStatusEnum.INACTIVE.Id != IndirectSalesOrder.EditedPriceStatusId)
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.EditedPriceStatus), ErrorCode.EditedPriceStatusNotExisted);
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool> Create(IndirectSalesOrder IndirectSalesOrder)
        {
            await ValidateStore(IndirectSalesOrder);
            await ValidateEmployee(IndirectSalesOrder);
            await ValidateOrderDate(IndirectSalesOrder);
            await ValidateContent(IndirectSalesOrder);
            await ValidatePromotion(IndirectSalesOrder);
            await ValidateEditedPrice(IndirectSalesOrder);
            return IndirectSalesOrder.IsValidated;
        }

        public async Task<bool> Update(IndirectSalesOrder IndirectSalesOrder)
        {
            if (await ValidateId(IndirectSalesOrder))
            {
                await ValidateStore(IndirectSalesOrder);
                await ValidateEmployee(IndirectSalesOrder);
                await ValidateOrderDate(IndirectSalesOrder);
                await ValidateContent(IndirectSalesOrder);
                await ValidatePromotion(IndirectSalesOrder);
                await ValidateEditedPrice(IndirectSalesOrder);
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
