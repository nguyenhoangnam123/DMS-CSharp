using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MItem
{
    public interface IItemService : IServiceScoped
    {
        Task<int> Count(ItemFilter ItemFilter);
        Task<List<Item>> List(ItemFilter ItemFilter);
        Task<Item> Get(long Id);
        Task<Item> Create(Item Item);
        Task<Item> Update(Item Item);
        Task<Item> Delete(Item Item);
        Task<List<Item>> BulkDelete(List<Item> Items);
        Task<List<Item>> Import(List<Item> Items);
        Task<DataFile> Export(ItemFilter ItemFilter);
        ItemFilter ToFilter(ItemFilter ItemFilter);
    }

    public class ItemService : BaseService, IItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IItemValidator ItemValidator;

        public ItemService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IItemValidator ItemValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Item>> List(ItemFilter ItemFilter)
        {
            try
            {
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                return Items;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Item> Get(long Id)
        {
            Item Item = await UOW.ItemRepository.Get(Id);
            if (Item == null)
                return null;
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Item>> Import(List<Item> Items)
        {  
            if (!await ItemValidator.Import(Items))
                return Items;

            try
            {
                await UOW.Begin();
                await UOW.ItemRepository.BulkMerge(Items);
                await UOW.Commit();

                await Logging.CreateAuditLog(Items, new { }, nameof(ItemService));
                return Items;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ItemService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(ItemFilter ItemFilter)
        {
            List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Item);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int ProductIdColumn = 1 + StartColumn;
                int CodeColumn = 2 + StartColumn;
                int NameColumn = 3 + StartColumn;
                int ScanCodeColumn = 4 + StartColumn;
                int SalePriceColumn = 5 + StartColumn;
                int RetailPriceColumn = 6 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Item.Id);
                worksheet.Cells[1, ProductIdColumn].Value = nameof(Item.ProductId);
                worksheet.Cells[1, CodeColumn].Value = nameof(Item.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Item.Name);
                worksheet.Cells[1, ScanCodeColumn].Value = nameof(Item.ScanCode);
                worksheet.Cells[1, SalePriceColumn].Value = nameof(Item.SalePrice);
                worksheet.Cells[1, RetailPriceColumn].Value = nameof(Item.RetailPrice);

                for (int i = 0; i < Items.Count; i++)
                {
                    Item Item = Items[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Item.Id;
                    worksheet.Cells[i + StartRow, ProductIdColumn].Value = Item.ProductId;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Item.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Item.Name;
                    worksheet.Cells[i + StartRow, ScanCodeColumn].Value = Item.ScanCode;
                    worksheet.Cells[i + StartRow, SalePriceColumn].Value = Item.SalePrice;
                    worksheet.Cells[i + StartRow, RetailPriceColumn].Value = Item.RetailPrice;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Item),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public ItemFilter ToFilter(ItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ItemFilter subFilter = new ItemFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ProductId))
                    subFilter.ProductId = Map(subFilter.ProductId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ScanCode))
                    subFilter.ScanCode = Map(subFilter.ScanCode, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.SalePrice))
                    subFilter.SalePrice = Map(subFilter.SalePrice, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.RetailPrice))
                    subFilter.RetailPrice = Map(subFilter.RetailPrice, currentFilter.Value);
            }
            return filter;
        }
    }
}
