using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Enums;
using DMS.ABE.Repositories;
using DMS.ABE.Services.MImage;
using DMS.ABE.Handlers;
using DMS.ABE.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.ABE.Services.MNotification;
using DMS.ABE.Models;

namespace DMS.ABE.Services.MProduct
{
    public interface IProductService : IServiceScoped
    {
        Task<int> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
        Task<Image> SaveImage(Image Image);
        ProductFilter ToFilter(ProductFilter ProductFilter);
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
                List<StoreUserFavoriteProductMapping> StoreUserFavoriteProductMappings = await UOW.StoreUserFavoriteProductMappingRepository.List(
                        new StoreUserFavoriteProductMappingFilter
                        {
                            FavoriteProductId = new IdFilter { In = ProductIds },
                            StoreUserId = new IdFilter { Equal = CurrentContext.StoreUserId },
                            Selects = StoreUserFavoriteProductMappingSelect.FavoriteProduct | StoreUserFavoriteProductMappingSelect.StoreUser
                        }
                    );
                ItemFilter ItemFilter = new ItemFilter
                {
                    ProductId = new IdFilter { In = ProductIds },
                    StatusId = null,
                    Selects = ItemSelect.Id | ItemSelect.ProductId | ItemSelect.SalePrice | ItemSelect.Image,
                    OrderBy = ItemOrder.Id,
                    OrderType = OrderType.ASC,
                    Skip = 0,
                    Take = int.MaxValue,
                };
                List<Item> Items = await UOW.ItemRepository.List(ItemFilter);
                foreach (Product Product in Products)
                {
                    Product.Items = Items.Where(x => x.ProductId == Product.Id).ToList();
                    Product.VariationCounter = Items.Where(i => i.ProductId == Product.Id).Count();
                    Product.IsFavorite = false;
                    int LikeCount = StoreUserFavoriteProductMappings.Where(x => x.FavoriteProductId == Product.Id).Count();
                    if(LikeCount > 0) Product.IsFavorite = true;
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
            Product.IsFavorite = false;
            int LikeCount = await UOW.StoreUserFavoriteProductMappingRepository.Count(new StoreUserFavoriteProductMappingFilter
            {
                FavoriteProductId = new IdFilter { Equal = Id},
                StoreUserId = new IdFilter { Equal = CurrentContext.StoreUserId }
            });
            if (LikeCount > 0)
            {
                Product.IsFavorite = true;
            }
            if (Product == null)
                return null;
            if (Product.Items != null && Product.Items.Any())
            {
                Product.VariationCounter = Product.Items.Count;
            }
            return Product;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            string thumbnailPath = $"/product/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
            return Image;
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
    }
}
