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

namespace DMS.Services.MWarehouse
{
    public interface IWarehouseService : IServiceScoped
    {
        Task<int> Count(WarehouseFilter WarehouseFilter);
        Task<List<Warehouse>> List(WarehouseFilter WarehouseFilter);
        Task<Warehouse> Get(long Id);
        Task<Warehouse> Create(Warehouse Warehouse);
        Task<Warehouse> Update(Warehouse Warehouse);
        Task<Warehouse> Delete(Warehouse Warehouse);
        Task<List<Warehouse>> BulkDelete(List<Warehouse> Warehouses);
        Task<List<Warehouse>> Import(List<Warehouse> Warehouses);
        WarehouseFilter ToFilter(WarehouseFilter WarehouseFilter);
    }

    public class WarehouseService : BaseService, IWarehouseService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWarehouseValidator WarehouseValidator;

        public WarehouseService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWarehouseValidator WarehouseValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WarehouseValidator = WarehouseValidator;
        }
        public async Task<int> Count(WarehouseFilter WarehouseFilter)
        {
            try
            {
                int result = await UOW.WarehouseRepository.Count(WarehouseFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Warehouse>> List(WarehouseFilter WarehouseFilter)
        {
            try
            {
                List<Warehouse> Warehouses = await UOW.WarehouseRepository.List(WarehouseFilter);
                return Warehouses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Warehouse> Get(long Id)
        {
            Warehouse Warehouse = await UOW.WarehouseRepository.Get(Id);
            if (Warehouse == null)
                return null;
            return Warehouse;
        }

        public async Task<Warehouse> Create(Warehouse Warehouse)
        {
            if (!await WarehouseValidator.Create(Warehouse))
                return Warehouse;

            try
            {
                await BuildData(Warehouse);
                await UOW.Begin();
                await UOW.WarehouseRepository.Create(Warehouse);
                await UOW.Commit();

                await Logging.CreateAuditLog(Warehouse, new { }, nameof(WarehouseService));
                return await UOW.WarehouseRepository.Get(Warehouse.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Warehouse> Update(Warehouse Warehouse)
        {
            if (!await WarehouseValidator.Update(Warehouse))
                return Warehouse;
            try
            {
                var oldData = await UOW.WarehouseRepository.Get(Warehouse.Id);
                await BuildData(Warehouse);
                await UOW.Begin();
                await UOW.WarehouseRepository.Update(Warehouse);
                await UOW.Commit();

                var newData = await UOW.WarehouseRepository.Get(Warehouse.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(WarehouseService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Warehouse> Delete(Warehouse Warehouse)
        {
            if (!await WarehouseValidator.Delete(Warehouse))
                return Warehouse;

            try
            {
                await UOW.Begin();
                await UOW.WarehouseRepository.Delete(Warehouse);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Warehouse, nameof(WarehouseService));
                return Warehouse;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Warehouse>> BulkDelete(List<Warehouse> Warehouses)
        {
            if (!await WarehouseValidator.BulkDelete(Warehouses))
                return Warehouses;

            try
            {
                await UOW.Begin();
                await UOW.WarehouseRepository.BulkDelete(Warehouses);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Warehouses, nameof(WarehouseService));
                return Warehouses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Warehouse>> Import(List<Warehouse> Warehouses)
        {
            if (!await WarehouseValidator.Import(Warehouses))
                return Warehouses;
            try
            {
                await UOW.Begin();
                await UOW.WarehouseRepository.BulkMerge(Warehouses);
                await UOW.Commit();

                await Logging.CreateAuditLog(Warehouses, new { }, nameof(WarehouseService));
                return Warehouses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WarehouseService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public WarehouseFilter ToFilter(WarehouseFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<WarehouseFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                WarehouseFilter subFilter = new WarehouseFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Address))
                    subFilter.Address = Map(subFilter.Address, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.OrganizationId))
                    subFilter.OrganizationId = Map(subFilter.OrganizationId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ProvinceId))
                    subFilter.ProvinceId = Map(subFilter.ProvinceId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.DistrictId))
                    subFilter.DistrictId = Map(subFilter.DistrictId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.WardId))
                    subFilter.WardId = Map(subFilter.WardId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }

        private async Task BuildData(Warehouse Warehouse)
        {
            List<Item> items = await UOW.ItemRepository.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.ALL
            });
            if (Warehouse.Inventories == null)
                Warehouse.Inventories = new List<Inventory>();
            foreach (Item item in items)
            {
                Inventory Inventory = Warehouse.Inventories.Where(i => i.ItemId == item.Id).FirstOrDefault();
                if (Inventory == null)
                {
                    Inventory = new Inventory();
                    Inventory.Id = 0;
                    Inventory.WarehouseId = Warehouse.Id;
                    Inventory.ItemId = item.Id;
                    Inventory.SaleStock = 0;
                    Inventory.AccountingStock = 0;
                    Warehouse.Inventories.Add(Inventory);
                }
            }
            Warehouse ExistedWarehouse = await UOW.WarehouseRepository.Get(Warehouse.Id);
            if (ExistedWarehouse != null)
            {
                foreach (Inventory inventory in Warehouse.Inventories)
                {
                    if (inventory.InventoryHistories == null ) inventory.InventoryHistories = new List<InventoryHistory>();
                    Inventory ExistedInventory = ExistedWarehouse.Inventories.Where(i => i.ItemId == inventory.Id).FirstOrDefault();
                    if (ExistedInventory == null)
                    {
                        InventoryHistory InventoryHistory = new InventoryHistory();
                        InventoryHistory.SaleStock = inventory.SaleStock;
                        InventoryHistory.AccountingStock = inventory.AccountingStock;
                        InventoryHistory.AppUserId = CurrentContext.UserId;
                        inventory.InventoryHistories.Add(InventoryHistory);
                    }
                    else
                    {
                        inventory.Id = ExistedInventory.Id;
                        if (inventory.SaleStock != ExistedInventory.SaleStock || inventory.AccountingStock != ExistedInventory.AccountingStock)
                        {
                            InventoryHistory InventoryHistory = new InventoryHistory();
                            InventoryHistory.SaleStock = inventory.SaleStock;
                            InventoryHistory.AccountingStock = inventory.AccountingStock;
                            InventoryHistory.AppUserId = CurrentContext.UserId;
                            inventory.InventoryHistories.Add(InventoryHistory);
                        }    
                    }
                }
            }
        }
    }
}
