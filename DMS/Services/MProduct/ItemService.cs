using Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Services.MImage;
using Helpers;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProduct
{
    public interface IItemService : IServiceScoped
    {
        Task<int> Count(ItemFilter ItemFilter);
        Task<List<Item>> List(ItemFilter ItemFilter);
        Task<Item> Get(long Id);
        Task<Item> Create(Item Item);
        Task<Item> Update(Item Item);
        Task<Item> Delete(Item Item);
        Task<Image> SaveImage(Image Image);
        Task<List<Item>> BulkDelete(List<Item> Items);
        ItemFilter ToFilter(ItemFilter ItemFilter);
    }

    public class ItemService : BaseService, IItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageService ImageService;
        private IItemValidator ItemValidator;

        public ItemService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IItemValidator ItemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ImageService = ImageService;
            this.ItemValidator = ItemValidator;
        }
        public async Task<int> Count(ItemFilter ItemFilter)
        {
            try
            {
                int result = await UOW.ItemRepository.Count(ItemFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Item>> List(ItemFilter ItemFilter)
        {
            try
            {
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);

                var Ids = Items.Select(x => x.Id).ToList();
                InventoryFilter InventoryFilter = new InventoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    ItemId = new IdFilter { In = Ids },
                    Selects = InventorySelect.SaleStock | InventorySelect.Item
                };

                var inventories = await UOW.InventoryRepository.List(InventoryFilter);
                var list = inventories.GroupBy(x => x.ItemId).Select(x => new { ItemId = x.Key, SaleStock = x.Sum(s => s.SaleStock) }).ToList();

                foreach (var item in Items)
                {
                    item.SaleStock = list.Where(i => i.ItemId == item.Id).Select(i => i.SaleStock).FirstOrDefault();
                    item.HasInventory = item.SaleStock > 0;
                }
                return Items;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<Item> Get(long Id)
        {
            var appUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            Item Item = await UOW.ItemRepository.Get(Id);
            if (Item == null)
                return null;
            
            WarehouseFilter warehouseFilter = new WarehouseFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                OrderBy = WarehouseOrder.Code,
                OrderType = OrderType.ASC,
                Selects = WarehouseSelect.Id | WarehouseSelect.Code | WarehouseSelect.Name,
                OrganizationId = new IdFilter { Equal = appUser.OrganizationId }
            };
            List<Warehouse> Warehouses = await UOW.WarehouseRepository.List(warehouseFilter);
            var WarehouseIds = Warehouses.Select(x => x.Id).ToList();
            InventoryFilter InventoryFilter = new InventoryFilter
            {
                ItemId = new IdFilter { Equal = Item.Id },
                Skip = 0,
                Take = int.MaxValue,
                Selects = InventorySelect.ALL,
                WarehouseId = new IdFilter { In = WarehouseIds }
            };
            List<Inventory> Inventories = await UOW.InventoryRepository.List(InventoryFilter);
            var InventoryIds = Inventories.Select(x => x.Id).ToList();

            #region lấy thời gian cập nhật tồn kho gần nhất
            InventoryHistoryFilter InventoryHistoryFilter = new InventoryHistoryFilter
            {
                InventoryId = new IdFilter { In = InventoryIds },
                OrderBy = InventoryHistoryOrder.UpdateTime,
                OrderType = OrderType.DESC,
                Skip = 0,
                Take = int.MaxValue,
                Selects = InventoryHistorySelect.UpdateTime,
            };
            List<InventoryHistory> InventoryHistories = await UOW.InventoryHistoryRepository.List(InventoryHistoryFilter);
            InventoryHistory InventoryHistory = InventoryHistories.FirstOrDefault();
            if(InventoryHistory != null)
            {
                Item.LastUpdateInventory = InventoryHistory.UpdateTime;
            }
            #endregion

            Item.Inventories = new List<Inventory>();
            foreach (Warehouse Warehouse in Warehouses)
            {
                Inventory Inventory = Inventories.Where(i => i.WarehouseId == Warehouse.Id && i.ItemId == Id).FirstOrDefault();
                if (Inventory == null)
                {
                    Inventory = new Inventory
                    {
                        Warehouse = Warehouse,
                        ItemId = Id,
                        SaleStock = 0,
                        AccountingStock = 0,
                        WarehouseId = Warehouse.Id,
                    };
                }
                Item.Inventories.Add(Inventory);
            }
            Item.HasInventory = Item.Inventories.Select(i => i.SaleStock).Sum() > 0;
            return Item;
        }

        public async Task<Item> Create(Item Item)
        {
            if (!await ItemValidator.Create(Item))
                return Item;

            try
            {
                await UOW.Begin();
                await UOW.ItemRepository.Create(Item);
                await UOW.Commit();

                await Logging.CreateAuditLog(Item, new { }, nameof(ItemService));
                return await UOW.ItemRepository.Get(Item.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Item> Update(Item Item)
        {
            if (!await ItemValidator.Update(Item))
                return Item;
            try
            {
                var oldData = await UOW.ItemRepository.Get(Item.Id);

                await UOW.Begin();
                await UOW.ItemRepository.Update(Item);
                await UOW.Commit();

                var newData = await UOW.ItemRepository.Get(Item.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ItemService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Item> Delete(Item Item)
        {
            if (!await ItemValidator.Delete(Item))
                return Item;

            try
            {
                await UOW.Begin();
                await UOW.ItemRepository.Delete(Item);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Item, nameof(ItemService));
                return Item;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<Item>> BulkDelete(List<Item> Items)
        {
            if (!await ItemValidator.BulkDelete(Items))
                return Items;

            try
            {
                await UOW.Begin();
                await UOW.ItemRepository.BulkDelete(Items);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Items, nameof(ItemService));
                return Items;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ItemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public ItemFilter ToFilter(ItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemFilter subFilter = new ItemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductTypeId))
                        subFilter.ProductTypeId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductGroupingId))
                        subFilter.ProductGroupingId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalePrice))
                        subFilter.SalePrice = FilterPermissionDefinition.DecimalFilter;
                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/item/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
        }
    }
}
