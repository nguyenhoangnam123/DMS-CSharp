using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MImage;
using DMS.Handlers;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.Rpc.product;
using DMS.Services.MNotification;

namespace DMS.Services.MProduct
{
    public interface IProductService : IServiceScoped
    {
        Task<int> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
        Task<Product> Create(Product Product);
        Task<List<Product>> BulkInsertNewProduct(List<Product> Products);
        Task<Product> Update(Product Product);
        Task<Product> Delete(Product Product);
        Task<List<Product>> BulkDeleteNewProduct(List<Product> Products);
        Task<List<Product>> BulkDelete(List<Product> Products);
        Task<List<Product>> Import(List<Product> Products);
        Task<DataFile> Export(ProductFilter ProductFilter);
        ProductFilter ToFilter(ProductFilter ProductFilter);

        Task<Image> SaveImage(Image Image);
    }

    public class ProductService : BaseService, IProductService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationService NotificationService;
        private IProductValidator ProductValidator;
        private IImageService ImageService;
        private IRabbitManager RabbitManager;

        public ProductService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INotificationService NotificationService,
            IProductValidator ProductValidator,
            IImageService ImageService,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificationService = NotificationService;
            this.ProductValidator = ProductValidator;
            this.ImageService = ImageService;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ProductFilter ProductFilter)
        {
            try
            {
                int result = await UOW.ProductRepository.Count(ProductFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> List(ProductFilter ProductFilter)
        {
            try
            {
                List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
                List<long> ProductIds = Products.Select(p => p.Id).ToList();
                ItemFilter ItemFilter = new ItemFilter
                {
                    ProductId = new IdFilter { In = ProductIds },
                    StatusId = null,
                    Selects = ItemSelect.Id | ItemSelect.ProductId,
                    Skip = 0,
                    Take = int.MaxValue,
                };
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                foreach (Product Product in Products)
                {
                    Product.CanDelete = true;
                    Product.VariationCounter = Items.Where(i => i.ProductId == Product.Id).Count();
                    List<long> ItemIds = Items.Where(i => i.ProductId == Product.Id).Select(i => i.Id).ToList();
                    IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter()
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };

                    int count = await UOW.IndirectSalesOrderContentRepository.Count(IndirectSalesOrderContentFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }

                    IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };
                    count = await UOW.IndirectSalesOrderPromotionRepository.Count(IndirectSalesOrderPromotionFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }

                    DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter()
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };

                    count = await UOW.DirectSalesOrderContentRepository.Count(DirectSalesOrderContentFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }

                    DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter
                    {
                        ItemId = new IdFilter { In = ItemIds }
                    };
                    count = await UOW.DirectSalesOrderPromotionRepository.Count(DirectSalesOrderPromotionFilter);
                    if (count != 0)
                    {
                        Product.CanDelete = false;
                        continue;
                    }
                }
                return Products;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Product> Get(long Id)
        {
            Product Product = await UOW.ProductRepository.Get(Id);
            if (Product == null)
                return null;
            Product.CanDelete = true;
            if (Product.Items != null && Product.Items.Any())
            {
                Product.VariationCounter = Product.Items.Count;
                Product.Items.ForEach(x => x.CanDelete = true);

                foreach (var Item in Product.Items)
                {
                    Item.CanDelete = await CanDelete(Item);
                    if (Item.CanDelete == false)
                        Product.CanDelete = false;
                }
            }
            return Product;
        }

        public async Task<Product> Create(Product Product)
        {
            if (!await ProductValidator.Create(Product))
                return Product;

            try
            {
                if (Product.UsedVariationId == UsedVariationEnum.NOTUSED.Id)
                {
                    Product.Items = new List<Item>();
                    Product.Items.Add(new Item
                    {
                        Code = Product.Code,
                        Name = Product.Name,
                        ScanCode = Product.ScanCode,
                        RetailPrice = Product.RetailPrice,
                        SalePrice = Product.SalePrice,
                        ProductId = Product.Id,
                        StatusId = StatusEnum.ACTIVE.Id
                    });
                }
                await UOW.Begin();
                await UOW.ProductRepository.Create(Product);
                await UOW.Commit();

                await Logging.CreateAuditLog(Product, new { }, nameof(ProductService));
                return await UOW.ProductRepository.Get(Product.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> BulkInsertNewProduct(List<Product> Products)
        {
            if (!await ProductValidator.BulkMergeNewProduct(Products))
                return Products;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                Products.ForEach(x => x.IsNew = true);
                await UOW.Begin();
                await UOW.ProductRepository.BulkInsertNewProduct(Products);
                await UOW.Commit();
                List<UserNotification> UserNotifications = new List<UserNotification>();
                var RecipientIds = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = AppUserSelect.Id,
                    OrganizationId = new IdFilter { }
                })).Select(x => x.Id).ToList();
                foreach (var Product in Products)
                {
                    foreach (var Id in RecipientIds)
                    {
                        UserNotification UserNotification = new UserNotification
                        {
                            Content = $"Sản phẩm {Product.Code} - {Product.Name} đã được đưa vào danh sách sản phẩm mới bởi {CurrentUser.DisplayName} vào lúc {StaticParams.DateTimeNow}",
                            LinkWebsite = $"{ProductRoute.Master}/{Product.Id}",
                            LinkMobile = $"{ProductRoute.Mobile}/{Product.Id}",
                            RecipientId = Id,
                            SenderId = CurrentContext.UserId,
                            Time = StaticParams.DateTimeNow,
                            Unread = false
                        };
                        UserNotifications.Add(UserNotification);
                    }
                }

                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> BulkDeleteNewProduct(List<Product> Products)
        {
            if (!await ProductValidator.BulkMergeNewProduct(Products))
                return Products;

            try
            {
                Products.ForEach(x => x.IsNew = false);
                await UOW.Begin();
                await UOW.ProductRepository.BulkDeleteNewProduct(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Product> Update(Product Product)
        {
            if (!await ProductValidator.Update(Product))
                return Product;
            try
            {
                var oldData = await UOW.ProductRepository.Get(Product.Id);
                var Items = Product.Items;
                var OldItems = oldData.Items;
                var OldItemIds = OldItems.Select(x => x.Id).ToList();
                var ItemHistories = await UOW.ItemHistoryRepository.List(new ItemHistoryFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ItemHistorySelect.ALL,
                    ItemId = new IdFilter { In = OldItemIds }
                });
                foreach (var item in Items)
                {
                    item.ItemHistories = ItemHistories.Where(x => x.ItemId == item.Id).ToList();
                    var oldItem = OldItems.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (oldItem != null)
                    {
                        if (item.SalePrice != oldItem.SalePrice)
                        {
                            ItemHistory ItemHistory = new ItemHistory
                            {
                                ItemId = item.Id,
                                ModifierId = CurrentContext.UserId,
                                Time = StaticParams.DateTimeNow,
                                OldPrice = oldItem.SalePrice,
                                NewPrice = item.SalePrice,
                            };
                            if (item.ItemHistories == null || !item.ItemHistories.Any())
                            {
                                item.ItemHistories = new List<ItemHistory>();
                                item.ItemHistories.Add(ItemHistory);
                            }
                            else
                            {
                                item.ItemHistories.Add(ItemHistory);
                            }
                        }
                    }
                }
                if (oldData.UsedVariationId == Enums.UsedVariationEnum.NOTUSED.Id)
                {
                    if (Product.StatusId == StatusEnum.ACTIVE.Id)
                        Product.Items.ForEach(x => x.StatusId = StatusEnum.ACTIVE.Id);
                    else
                        Product.Items.ForEach(x => x.StatusId = StatusEnum.INACTIVE.Id);
                    if (!Product.Used)
                    {
                        foreach (var item in Product.Items)
                        {
                            item.Code = Product.Code;
                            item.Name = Product.Name;
                            item.ScanCode = Product.ScanCode;
                            item.RetailPrice = Product.RetailPrice;
                            item.SalePrice = Product.SalePrice;
                        }
                    }

                }
                else
                {
                    if (Product.StatusId == StatusEnum.INACTIVE.Id)
                        Product.Items.ForEach(x => x.StatusId = StatusEnum.INACTIVE.Id);
                }
                await UOW.Begin();
                await UOW.ProductRepository.Update(Product);
                await UOW.Commit();

                var newData = await UOW.ProductRepository.Get(Product.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProductService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Product> Delete(Product Product)
        {
            if (!await ProductValidator.Delete(Product))
                return Product;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.Delete(Product);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Product, nameof(ProductService));
                return Product;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> BulkDelete(List<Product> Products)
        {
            if (!await ProductValidator.BulkDelete(Products))
                return Products;

            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.BulkDelete(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Products, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Product>> Import(List<Product> Products)
        {
            var ProductProductGroupingMappings = new List<ProductProductGroupingMapping>();
            if (!await ProductValidator.Import(Products))
                return Products;
            try
            {
                await UOW.Begin();
                await UOW.ProductRepository.BulkMerge(Products);
                await UOW.Commit();
                await Logging.CreateAuditLog(Products, new { }, nameof(ProductService));
                return Products;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DataFile> Export(ProductFilter ProductFilter)
        {
            List<Product> Products = await UOW.ProductRepository.List(ProductFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Product);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int SupplierCodeColumn = 2 + StartColumn;
                int NameColumn = 3 + StartColumn;
                int DescriptionColumn = 4 + StartColumn;
                int ScanCodeColumn = 5 + StartColumn;
                int ProductTypeIdColumn = 6 + StartColumn;
                int SupplierIdColumn = 7 + StartColumn;
                int BrandIdColumn = 8 + StartColumn;
                int UnitOfMeasureIdColumn = 9 + StartColumn;
                int UnitOfMeasureGroupingIdColumn = 10 + StartColumn;
                int SalePriceColumn = 11 + StartColumn;
                int RetailPriceColumn = 12 + StartColumn;
                int TaxTypeIdColumn = 13 + StartColumn;
                int StatusIdColumn = 14 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Product.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Product.Code);
                worksheet.Cells[1, SupplierCodeColumn].Value = nameof(Product.SupplierCode);
                worksheet.Cells[1, NameColumn].Value = nameof(Product.Name);
                worksheet.Cells[1, DescriptionColumn].Value = nameof(Product.Description);
                worksheet.Cells[1, ScanCodeColumn].Value = nameof(Product.ScanCode);
                worksheet.Cells[1, ProductTypeIdColumn].Value = nameof(Product.ProductTypeId);
                worksheet.Cells[1, SupplierIdColumn].Value = nameof(Product.SupplierId);
                worksheet.Cells[1, BrandIdColumn].Value = nameof(Product.BrandId);
                worksheet.Cells[1, UnitOfMeasureIdColumn].Value = nameof(Product.UnitOfMeasureId);
                worksheet.Cells[1, UnitOfMeasureGroupingIdColumn].Value = nameof(Product.UnitOfMeasureGroupingId);
                worksheet.Cells[1, SalePriceColumn].Value = nameof(Product.SalePrice);
                worksheet.Cells[1, RetailPriceColumn].Value = nameof(Product.RetailPrice);
                worksheet.Cells[1, TaxTypeIdColumn].Value = nameof(Product.TaxTypeId);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Product.StatusId);

                for (int i = 0; i < Products.Count; i++)
                {
                    Product Product = Products[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Product.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Product.Code;
                    worksheet.Cells[i + StartRow, SupplierCodeColumn].Value = Product.SupplierCode;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Product.Name;
                    worksheet.Cells[i + StartRow, DescriptionColumn].Value = Product.Description;
                    worksheet.Cells[i + StartRow, ScanCodeColumn].Value = Product.ScanCode;
                    worksheet.Cells[i + StartRow, ProductTypeIdColumn].Value = Product.ProductTypeId;
                    worksheet.Cells[i + StartRow, SupplierIdColumn].Value = Product.SupplierId;
                    worksheet.Cells[i + StartRow, BrandIdColumn].Value = Product.BrandId;
                    worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value = Product.UnitOfMeasureId;
                    worksheet.Cells[i + StartRow, UnitOfMeasureGroupingIdColumn].Value = Product.UnitOfMeasureGroupingId;
                    worksheet.Cells[i + StartRow, SalePriceColumn].Value = Product.SalePrice;
                    worksheet.Cells[i + StartRow, RetailPriceColumn].Value = Product.RetailPrice;
                    worksheet.Cells[i + StartRow, TaxTypeIdColumn].Value = Product.TaxTypeId;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Product.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Product),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public ProductFilter ToFilter(ProductFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProductFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProductFilter subFilter = new ProductFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductTypeId))
                        subFilter.ProductTypeId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ProductGroupingId))
                        subFilter.ProductGroupingId = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
        }

        private async Task<bool> CanDelete(Item Item)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter()
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };

            int count = await UOW.IndirectSalesOrderContentRepository.Count(IndirectSalesOrderContentFilter);
            if (count != 0)
                return false;

            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };
            count = await UOW.IndirectSalesOrderPromotionRepository.Count(IndirectSalesOrderPromotionFilter);
            if (count != 0)
                return false;

            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter()
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };

            count = await UOW.DirectSalesOrderContentRepository.Count(DirectSalesOrderContentFilter);
            if (count != 0)
                return false;

            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter
            {
                ItemId = new IdFilter { Equal = Item.Id }
            };
            count = await UOW.DirectSalesOrderPromotionRepository.Count(DirectSalesOrderPromotionFilter);
            if (count != 0)
                return false;
            return true;
        }

        private void NotifyUsed(Product Product)
        {
            {
                EventMessage<ProductType> ProductTypeMessage = new EventMessage<ProductType>
                {
                    Content = new ProductType { Id = Product.ProductTypeId },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(ProductTypeMessage, RoutingKeyEnum.ProductTypeUsed);
            }

            {
                EventMessage<UnitOfMeasure> UnitOfMeasureMessage = new EventMessage<UnitOfMeasure>
                {
                    Content = new UnitOfMeasure { Id = Product.UnitOfMeasureId },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(UnitOfMeasureMessage, RoutingKeyEnum.UnitOfMeasureUsed);
            }

            {
                EventMessage<TaxType> TaxTypeMessage = new EventMessage<TaxType>
                {
                    Content = new TaxType { Id = Product.TaxTypeId },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(TaxTypeMessage, RoutingKeyEnum.TaxTypeUsed);
            }
        }
    }
}
