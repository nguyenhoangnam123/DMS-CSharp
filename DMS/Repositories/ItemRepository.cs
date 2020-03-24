using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace DMS.Repositories
{
    public interface IItemRepository
    {
        Task<int> Count(ItemFilter ItemFilter);
        Task<List<Item>> List(ItemFilter ItemFilter);
        Task<Item> Get(long Id);
        Task<bool> Create(Item Item);
        Task<bool> Update(Item Item);
        Task<bool> Delete(Item Item);
        Task<bool> BulkMerge(List<Item> Items);
        Task<bool> BulkDelete(List<Item> Items);
    }
    public class ItemRepository : IItemRepository
    {
        private DataContext DataContext;
        public ItemRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ItemDAO> DynamicFilter(IQueryable<ItemDAO> query, ItemFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.ProductId != null)
                query = query.Where(q => q.ProductId, filter.ProductId);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.ScanCode != null)
                query = query.Where(q => q.ScanCode, filter.ScanCode);
            if (filter.SalePrice != null)
                query = query.Where(q => q.SalePrice, filter.SalePrice);
            if (filter.RetailPrice != null)
                query = query.Where(q => q.RetailPrice, filter.RetailPrice);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ItemDAO> OrFilter(IQueryable<ItemDAO> query, ItemFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ItemDAO> initQuery = query.Where(q => false);
            foreach (ItemFilter ItemFilter in filter.OrFilter)
            {
                IQueryable<ItemDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.ProductId != null)
                    queryable = queryable.Where(q => q.ProductId, filter.ProductId);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.ScanCode != null)
                    queryable = queryable.Where(q => q.ScanCode, filter.ScanCode);
                if (filter.SalePrice != null)
                    queryable = queryable.Where(q => q.SalePrice, filter.SalePrice);
                if (filter.RetailPrice != null)
                    queryable = queryable.Where(q => q.RetailPrice, filter.RetailPrice);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ItemDAO> DynamicOrder(IQueryable<ItemDAO> query, ItemFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ItemOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ItemOrder.Product:
                            query = query.OrderBy(q => q.ProductId);
                            break;
                        case ItemOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ItemOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ItemOrder.ScanCode:
                            query = query.OrderBy(q => q.ScanCode);
                            break;
                        case ItemOrder.SalePrice:
                            query = query.OrderBy(q => q.SalePrice);
                            break;
                        case ItemOrder.RetailPrice:
                            query = query.OrderBy(q => q.RetailPrice);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ItemOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ItemOrder.Product:
                            query = query.OrderByDescending(q => q.ProductId);
                            break;
                        case ItemOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ItemOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ItemOrder.ScanCode:
                            query = query.OrderByDescending(q => q.ScanCode);
                            break;
                        case ItemOrder.SalePrice:
                            query = query.OrderByDescending(q => q.SalePrice);
                            break;
                        case ItemOrder.RetailPrice:
                            query = query.OrderByDescending(q => q.RetailPrice);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Item>> DynamicSelect(IQueryable<ItemDAO> query, ItemFilter filter)
        {
            List<Item> Items = await query.Select(q => new Item()
            {
                Id = filter.Selects.Contains(ItemSelect.Id) ? q.Id : default(long),
                ProductId = filter.Selects.Contains(ItemSelect.Product) ? q.ProductId : default(long),
                Code = filter.Selects.Contains(ItemSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ItemSelect.Name) ? q.Name : default(string),
                ScanCode = filter.Selects.Contains(ItemSelect.ScanCode) ? q.ScanCode : default(string),
                SalePrice = filter.Selects.Contains(ItemSelect.SalePrice) ? q.SalePrice : default(decimal?),
                RetailPrice = filter.Selects.Contains(ItemSelect.RetailPrice) ? q.RetailPrice : default(decimal?),
                Product = filter.Selects.Contains(ItemSelect.Product) && q.Product != null ? new Product
                {
                    Id = q.Product.Id,
                    Code = q.Product.Code,
                    SupplierCode = q.Product.SupplierCode,
                    Name = q.Product.Name,
                    Description = q.Product.Description,
                    ScanCode = q.Product.ScanCode,
                    ProductTypeId = q.Product.ProductTypeId,
                    SupplierId = q.Product.SupplierId,
                    BrandId = q.Product.BrandId,
                    UnitOfMeasureId = q.Product.UnitOfMeasureId,
                    UnitOfMeasureGroupingId = q.Product.UnitOfMeasureGroupingId,
                    SalePrice = q.Product.SalePrice,
                    RetailPrice = q.Product.RetailPrice,
                    TaxTypeId = q.Product.TaxTypeId,
                    StatusId = q.Product.StatusId,
                } : null,
            }).ToListAsync();
            return Items;
        }

        public async Task<int> Count(ItemFilter filter)
        {
            IQueryable<ItemDAO> Items = DataContext.Item;
            Items = DynamicFilter(Items, filter);
            return await Items.CountAsync();
        }

        public async Task<List<Item>> List(ItemFilter filter)
        {
            if (filter == null) return new List<Item>();
            IQueryable<ItemDAO> ItemDAOs = DataContext.Item;
            ItemDAOs = DynamicFilter(ItemDAOs, filter);
            ItemDAOs = DynamicOrder(ItemDAOs, filter);
            List<Item> Items = await DynamicSelect(ItemDAOs, filter);
            return Items;
        }

        public async Task<Item> Get(long Id)
        {
            Item Item = await DataContext.Item.Where(x => x.Id == Id).Select(x => new Item()
            {
                Id = x.Id,
                ProductId = x.ProductId,
                Code = x.Code,
                Name = x.Name,
                ScanCode = x.ScanCode,
                SalePrice = x.SalePrice,
                RetailPrice = x.RetailPrice,
                Product = x.Product == null ? null : new Product
                {
                    Id = x.Product.Id,
                    Code = x.Product.Code,
                    SupplierCode = x.Product.SupplierCode,
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    ScanCode = x.Product.ScanCode,
                    ProductTypeId = x.Product.ProductTypeId,
                    SupplierId = x.Product.SupplierId,
                    BrandId = x.Product.BrandId,
                    UnitOfMeasureId = x.Product.UnitOfMeasureId,
                    UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                    SalePrice = x.Product.SalePrice,
                    RetailPrice = x.Product.RetailPrice,
                    TaxTypeId = x.Product.TaxTypeId,
                    StatusId = x.Product.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (Item == null)
                return null;

            return Item;
        }
        public async Task<bool> Create(Item Item)
        {
            ItemDAO ItemDAO = new ItemDAO();
            ItemDAO.Id = Item.Id;
            ItemDAO.ProductId = Item.ProductId;
            ItemDAO.Code = Item.Code;
            ItemDAO.Name = Item.Name;
            ItemDAO.ScanCode = Item.ScanCode;
            ItemDAO.SalePrice = Item.SalePrice;
            ItemDAO.RetailPrice = Item.RetailPrice;
            ItemDAO.CreatedAt = StaticParams.DateTimeNow;
            ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Item.Add(ItemDAO);
            await DataContext.SaveChangesAsync();
            Item.Id = ItemDAO.Id;
            await SaveReference(Item);
            return true;
        }

        public async Task<bool> Update(Item Item)
        {
            ItemDAO ItemDAO = DataContext.Item.Where(x => x.Id == Item.Id).FirstOrDefault();
            if (ItemDAO == null)
                return false;
            ItemDAO.Id = Item.Id;
            ItemDAO.ProductId = Item.ProductId;
            ItemDAO.Code = Item.Code;
            ItemDAO.Name = Item.Name;
            ItemDAO.ScanCode = Item.ScanCode;
            ItemDAO.SalePrice = Item.SalePrice;
            ItemDAO.RetailPrice = Item.RetailPrice;
            ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Item);
            return true;
        }

        public async Task<bool> Delete(Item Item)
        {
            await DataContext.Item.Where(x => x.Id == Item.Id).UpdateFromQueryAsync(x => new ItemDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Item> Items)
        {
            List<ItemDAO> ItemDAOs = new List<ItemDAO>();
            foreach (Item Item in Items)
            {
                ItemDAO ItemDAO = new ItemDAO();
                ItemDAO.Id = Item.Id;
                ItemDAO.ProductId = Item.ProductId;
                ItemDAO.Code = Item.Code;
                ItemDAO.Name = Item.Name;
                ItemDAO.ScanCode = Item.ScanCode;
                ItemDAO.SalePrice = Item.SalePrice;
                ItemDAO.RetailPrice = Item.RetailPrice;
                ItemDAO.CreatedAt = StaticParams.DateTimeNow;
                ItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                ItemDAOs.Add(ItemDAO);
            }
            await DataContext.BulkMergeAsync(ItemDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Item> Items)
        {
            List<long> Ids = Items.Select(x => x.Id).ToList();
            await DataContext.Item
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ItemDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Item Item)
        {
        }
        
    }
}
