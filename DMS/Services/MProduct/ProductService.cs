using DMS.Common;
using DMS.Entities;
using DMS.Handlers;
using DMS.Helpers;
using DMS.Repositories;
using DMS.Services.MImage;
using DMS.Services.MNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProduct
{
    public interface IProductService : IServiceScoped
    {
        Task<int> Count(ProductFilter ProductFilter);
        Task<List<Product>> List(ProductFilter ProductFilter);
        Task<Product> Get(long Id);
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
