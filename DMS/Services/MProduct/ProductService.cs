using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Services.MImage;
using DMS.Handlers;
using DMS.Helpers;
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
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
                    Product.VariationCounter = Items.Where(i => i.ProductId == Product.Id).Count();
                }
                return Products;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }
        public async Task<Product> Get(long Id)
        {
            Product Product = await UOW.ProductRepository.Get(Id);
            if (Product == null)
                return null;
            if (Product.Items != null && Product.Items.Any())
            {
                Product.VariationCounter = Product.Items.Count;
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
                Product = await UOW.ProductRepository.Get(Product.Id);
                RabbitManager.PublishSingle(new EventMessage<Product>(Product, Product.RowId), RoutingKeyEnum.ProductSync);
                await Logging.CreateAuditLog(Product, new { }, nameof(ProductService));
                return Product;
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
                };
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
                            TitleWeb = $"Thông báo từ DMS",
                            ContentWeb = $"Sản phẩm {Product.Code} - {Product.Name} đã được đưa vào danh sách sản phẩm mới bởi {CurrentUser.DisplayName}.",
                            LinkWebsite = $"{ProductRoute.Master}/?id=*".Replace("*", Product.Id.ToString()),
                            LinkMobile = $"{ProductRoute.Mobile}".Replace("*", Product.Id.ToString()),
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProductService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProductService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<Product> Update(Product Product)
        {
            if (!await ProductValidator.Update(Product))
                return Product;
            try
            {
                var oldData = await UOW.ProductRepository.Get(Product.Id);
                Product.IsNew = oldData.IsNew;
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

                Product = await UOW.ProductRepository.Get(Product.Id);
                RabbitManager.PublishSingle(new EventMessage<Product>(Product, Product.RowId), RoutingKeyEnum.ProductSync);
                await Logging.CreateAuditLog(Product, oldData, nameof(ProductService));
                return Product;
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
                };
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
                RabbitManager.PublishSingle(new EventMessage<Product>(Product, Product.RowId), RoutingKeyEnum.ProductSync);
                return Product;
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
                };
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
                List<EventMessage<Product>> eventMessages = Products.Select(x => new EventMessage<Product>(x, x.RowId)).ToList();
                RabbitManager.PublishList(eventMessages, RoutingKeyEnum.ProductSync);
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
                };
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

                NotifyUsed(Products);

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
            string thumbnailPath = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
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
                EventMessage<ProductType> ProductTypeMessage = new EventMessage<ProductType>(
                     new ProductType { Id = Product.ProductTypeId },
                     Guid.NewGuid());
                RabbitManager.PublishSingle(ProductTypeMessage, RoutingKeyEnum.ProductTypeUsed);
            }

            {
                EventMessage<UnitOfMeasure> UnitOfMeasureMessage = new EventMessage<UnitOfMeasure>(
                    new UnitOfMeasure { Id = Product.UnitOfMeasureId }, 
                    Guid.NewGuid());
                RabbitManager.PublishSingle(UnitOfMeasureMessage, RoutingKeyEnum.UnitOfMeasureUsed);
            }

            {
                EventMessage<TaxType> TaxTypeMessage = new EventMessage<TaxType>(
                    new TaxType { Id = Product.TaxTypeId },
                    Guid.NewGuid());
                RabbitManager.PublishSingle(TaxTypeMessage, RoutingKeyEnum.TaxTypeUsed);
            }
        }

        private void NotifyUsed(List<Product> Products)
        {
            {
                List<EventMessage<ProductType>> ProductTypeMessages = new List<EventMessage<ProductType>>();
                foreach (var Product in Products)
                {
                    EventMessage<ProductType> ProductTypeMessage = new EventMessage<ProductType>(
                     new ProductType { Id = Product.ProductTypeId },
                     Guid.NewGuid());
                }
                RabbitManager.PublishList(ProductTypeMessages, RoutingKeyEnum.ProductTypeUsed);
            }

            {
                List<EventMessage<UnitOfMeasure>> UnitOfMeasureMessages = new List<EventMessage<UnitOfMeasure>>();
                foreach (var Product in Products)
                {
                    EventMessage<UnitOfMeasure> UnitOfMeasureMessage = new EventMessage<UnitOfMeasure>(
                    new UnitOfMeasure { Id = Product.UnitOfMeasureId },
                    Guid.NewGuid());
                }
                RabbitManager.PublishList(UnitOfMeasureMessages, RoutingKeyEnum.UnitOfMeasureUsed);
            }

            {
                List<EventMessage<TaxType>> TaxTypeMessages = new List<EventMessage<TaxType>>();
                foreach (var Product in Products)
                {
                    EventMessage<TaxType> TaxTypeMessage = new EventMessage<TaxType>(
                    new TaxType { Id = Product.TaxTypeId },
                    Guid.NewGuid());
                }
                RabbitManager.PublishList(TaxTypeMessages, RoutingKeyEnum.TaxTypeUsed);
            }
        }
    }
}
