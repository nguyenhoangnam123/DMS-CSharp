using DMS.Common;
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
            SellerStoreEqualBuyerStore,
            ContentEmpty,
            DeliveryDateInvalid,
            BuyerStoreNotInERouteScope,
            SaleEmployeeNotInOrg,
            DiscountOverPrice
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
                    Id = new IdFilter { In = new List<long> { IndirectSalesOrder.BuyerStoreId } },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = StoreSelect.Id
                };

                int count = await UOW.StoreRepository.CountInScoped(StoreFilter, IndirectSalesOrder.SaleEmployeeId);
                if (count == 0)
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.BuyerStore), ErrorCode.BuyerStoreNotInERouteScope);
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
                    Selects = AppUserSelect.Id | AppUserSelect.Organization
                };

                var AppUser = (await UOW.AppUserRepository.List(AppUserFilter)).FirstOrDefault();
                if (AppUser == null)
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeNotExisted);
                else
                {
                    var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                    if (!AppUser.Organization.Path.StartsWith(CurrentUser.Organization.Path)) 
                    {
                        IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.SaleEmployee), ErrorCode.SaleEmployeeNotInOrg);
                    }
                }
            }

            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateOrderDate(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.OrderDate == default(DateTime))
                IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.OrderDate), ErrorCode.OrderDateEmpty);
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateDeliveryDate (IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.DeliveryDate.HasValue)
            {
                if(IndirectSalesOrder.DeliveryDate.Value < IndirectSalesOrder.OrderDate)
                {
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.DeliveryDate), ErrorCode.DeliveryDateInvalid);
                }
            }
            return IndirectSalesOrder.IsValidated;
        }

        private async Task<bool> ValidateContent(IndirectSalesOrder IndirectSalesOrder)
        {
            if (IndirectSalesOrder.IndirectSalesOrderContents != null && IndirectSalesOrder.IndirectSalesOrderContents.Any())
            {
                var ItemIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.ItemId).ToList();
                var ProductIds = IndirectSalesOrder.IndirectSalesOrderContents.Select(x => x.Item.ProductId).ToList();
                ItemFilter ItemFilter = new ItemFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = ItemIds },
                    Selects = ItemSelect.Id | ItemSelect.SalePrice | ItemSelect.Product | ItemSelect.ProductId
                };
                var Items = await ListItem(ItemFilter, IndirectSalesOrder.SaleEmployeeId, IndirectSalesOrder.BuyerStoreId);

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

                decimal SubTotal = 0;
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
                                if (IndirectSalesOrderContent.Quantity <= 0)
                                    IndirectSalesOrderContent.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrderContent.Quantity), ErrorCode.QuantityEmpty);
                                else
                                {
                                    decimal SalePrice = 0;
                                    if (IndirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.INACTIVE.Id)
                                    {
                                        SalePrice = Item.SalePrice.GetValueOrDefault(0) * UOM.Factor.Value;
                                    }

                                    if (IndirectSalesOrder.EditedPriceStatusId == EditedPriceStatusEnum.ACTIVE.Id)
                                    {
                                        SalePrice = IndirectSalesOrderContent.PrimaryPrice * UOM.Factor.Value;
                                    }

                                    var SubTotalContent = SalePrice * IndirectSalesOrderContent.Quantity;
                                    SubTotal += SubTotalContent;
                                }
                            }
                        }
                    }
                }

                decimal GeneralDiscountAmount = 0;
                if (IndirectSalesOrder.GeneralDiscountPercentage.HasValue && IndirectSalesOrder.GeneralDiscountPercentage.Value > 0)
                    GeneralDiscountAmount = SubTotal * IndirectSalesOrder.GeneralDiscountPercentage.Value / 100;
                if (IndirectSalesOrder.GeneralDiscountAmount.HasValue)
                    GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount.Value;
                if (GeneralDiscountAmount > 0 && GeneralDiscountAmount > SubTotal)
                {
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.GeneralDiscountAmount), ErrorCode.DiscountOverPrice);
                }
            }
            else
            {
                if(IndirectSalesOrder.IndirectSalesOrderPromotions == null || !IndirectSalesOrder.IndirectSalesOrderPromotions.Any())
                {
                    IndirectSalesOrder.AddError(nameof(IndirectSalesOrderValidator), nameof(IndirectSalesOrder.Id), ErrorCode.ContentEmpty);
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
            await ValidateDeliveryDate(IndirectSalesOrder);
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
                await ValidateDeliveryDate(IndirectSalesOrder);
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

        private async Task<List<Item>> ListItem(ItemFilter ItemFilter, long? SalesEmployeeId, long? StoreId)
        {
            try
            {
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                var Ids = Items.Select(x => x.Id).ToList();
                AppUser AppUser = await UOW.AppUserRepository.Get(SalesEmployeeId.Value);
                if (AppUser != null)
                {
                    List<Warehouse> Warehouses = await UOW.WarehouseRepository.List(new WarehouseFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        Selects = WarehouseSelect.Id,
                        StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                        OrganizationId = new IdFilter { Equal = AppUser.OrganizationId }
                    });
                    var WarehouseIds = Warehouses.Select(x => x.Id).ToList();

                    InventoryFilter InventoryFilter = new InventoryFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        ItemId = new IdFilter { In = Ids },
                        WarehouseId = new IdFilter { In = WarehouseIds },
                        Selects = InventorySelect.SaleStock | InventorySelect.Item
                    };

                    var inventories = await UOW.InventoryRepository.List(InventoryFilter);
                    var list = inventories.GroupBy(x => x.ItemId).Select(x => new { ItemId = x.Key, SaleStock = x.Sum(s => s.SaleStock) }).ToList();

                    foreach (var item in Items)
                    {
                        item.SaleStock = list.Where(i => i.ItemId == item.Id).Select(i => i.SaleStock).FirstOrDefault();
                        item.HasInventory = item.SaleStock > 0;
                    }

                    await ApplyPrice(Items, SalesEmployeeId, StoreId);
                }
                return Items;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw new MessageException(ex);
                }
                else
                {
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        private async Task<List<Item>> ApplyPrice(List<Item> Items, long? SalesEmployeeId, long? StoreId)
        {
            var SalesEmployee = await UOW.AppUserRepository.Get(SalesEmployeeId.Value);
            SystemConfiguration SystemConfiguration = await UOW.SystemConfigurationRepository.Get();
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            var OrganizationIds = Organizations
                .Where(x => x.Path.StartsWith(SalesEmployee.Organization.Path) || SalesEmployee.Organization.Path.StartsWith(x.Path))
                .Select(x => x.Id)
                .ToList();

            var ItemIds = Items.Select(x => x.Id).Distinct().ToList();
            Dictionary<long, decimal> result = new Dictionary<long, decimal>();
            PriceListItemMappingFilter PriceListItemMappingFilter = new PriceListItemMappingFilter
            {
                ItemId = new IdFilter { In = ItemIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = PriceListItemMappingSelect.ALL,
                PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.ALLSTORE.Id },
                SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };
            var PriceListItemMappingAllStore = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
            List<PriceListItemMapping> PriceListItemMappings = new List<PriceListItemMapping>();
            PriceListItemMappings.AddRange(PriceListItemMappingAllStore);

            if (StoreId.HasValue)
            {
                var Store = await UOW.StoreRepository.Get(StoreId.Value);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STOREGROUPING.Id },
                    SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                    StoreGroupingId = new IdFilter { Equal = Store.StoreGroupingId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreGrouping = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.STORETYPE.Id },
                    SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                    StoreTypeId = new IdFilter { Equal = Store.StoreTypeId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreType = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);

                PriceListItemMappingFilter = new PriceListItemMappingFilter
                {
                    ItemId = new IdFilter { In = ItemIds },
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = PriceListItemMappingSelect.ALL,
                    PriceListTypeId = new IdFilter { Equal = PriceListTypeEnum.DETAILS.Id },
                    SalesOrderTypeId = new IdFilter { In = new List<long> { SalesOrderTypeEnum.INDIRECT.Id, SalesOrderTypeEnum.ALL.Id } },
                    StoreId = new IdFilter { Equal = StoreId },
                    OrganizationId = new IdFilter { In = OrganizationIds },
                    StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
                };
                var PriceListItemMappingStoreDetail = await UOW.PriceListItemMappingItemMappingRepository.List(PriceListItemMappingFilter);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreGrouping);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreType);
                PriceListItemMappings.AddRange(PriceListItemMappingStoreDetail);
            }

            //Áp giá theo cấu hình
            //Ưu tiên lấy giá thấp hơn
            if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 0)
            {
                foreach (var ItemId in ItemIds)
                {
                    result.Add(ItemId, decimal.MaxValue);
                }
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MaxValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(decimal.MaxValue)
                            .Min();
                        if (targetPrice < result[ItemId])
                        {
                            result[ItemId] = targetPrice;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MaxValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice.GetValueOrDefault(0)).FirstOrDefault();
                    }
                }
            }
            //Ưu tiên lấy giá cao hơn
            else if (SystemConfiguration.PRIORITY_USE_PRICE_LIST == 1)
            {
                foreach (var ItemId in ItemIds)
                {
                    result.Add(ItemId, decimal.MinValue);
                }
                foreach (var ItemId in ItemIds)
                {
                    foreach (var OrganizationId in OrganizationIds)
                    {
                        decimal targetPrice = decimal.MinValue;
                        targetPrice = PriceListItemMappings.Where(x => x.ItemId == ItemId && x.PriceList.OrganizationId == OrganizationId)
                            .Select(x => x.Price)
                            .DefaultIfEmpty(decimal.MinValue)
                            .Max();
                        if (targetPrice > result[ItemId])
                        {
                            result[ItemId] = targetPrice;
                        }
                    }
                }

                foreach (var ItemId in ItemIds)
                {
                    if (result[ItemId] == decimal.MinValue)
                    {
                        result[ItemId] = Items.Where(x => x.Id == ItemId).Select(x => x.SalePrice.GetValueOrDefault(0)).FirstOrDefault();
                    }
                }
            }

            //nhân giá với thuế
            foreach (var item in Items)
            {
                item.SalePrice = result[item.Id] * (1 + item.Product.TaxType.Percentage / 100);
            }
            return Items;
        }
    }
}
