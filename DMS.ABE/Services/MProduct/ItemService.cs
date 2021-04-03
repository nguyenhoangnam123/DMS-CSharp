using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Repositories;
using DMS.ABE.Services.MImage;
using DMS.ABE.Helpers;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MProduct
{
    public interface IItemService : IServiceScoped
    {
        Task<int> Count(ItemFilter ItemFilter);
        Task<List<Item>> List(ItemFilter ItemFilter);
        Task<Item> Get(long Id);
        Task<Item> GetItemByVariation(long Id, List<long> VariationIds);
        ItemFilter ToFilter(ItemFilter ItemFilter);
        Task<Image> SaveImage(Image Image);
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
                //if (Items.Count() > 0)
                //{
                //    var Ids = Items.Select(x => x.Id).ToList();
                //    ItemBasePriceFilter ItemBasePriceFilter = new ItemBasePriceFilter
                //    {
                //        Skip = 0,
                //        Take = int.MaxValue,
                //        Selects = ItemBasePriceSelect.ALL,
                //        ItemId = new IdFilter { In = Ids }
                //    };

                //    List<ItemBasePrice> ItemBasePrices = await UOW.ItemBasePriceRepository.List(ItemBasePriceFilter);
                //    foreach (var Item in Items)
                //    {
                //        var ItemBasePrice = ItemBasePrices.Where(x => x.ItemId == Item.Id).FirstOrDefault();
                //        if (ItemBasePrice != null)
                //            Item.RetailPrice = ItemBasePrice.BasePrice;
                //        else
                //            Item.RetailPrice = Item.SalePrice;
                //    }
                //}

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
            var StoreUserId = CurrentContext.StoreUserId;
            Item Item = await UOW.ItemRepository.Get(Id);
            if (Item == null)
                return null;
            Product Product = Item.Product;
            Product.IsFavorite = false;
            int LikeCount = await UOW.StoreUserFavoriteProductMappingRepository.Count(new StoreUserFavoriteProductMappingFilter
            {
                FavoriteProductId = new IdFilter { Equal = Product.Id },
                StoreUserId = new IdFilter { Equal = StoreUserId }
            });
            if (LikeCount > 0)
            {
                Product.IsFavorite = true;
                Item.Product = Product;
            }
            //ItemBasePriceFilter ItemBasePriceFilter = new ItemBasePriceFilter
            //{
            //    Skip = 0,
            //    Take = 1,
            //    Selects = ItemBasePriceSelect.ALL,
            //    ItemId = new IdFilter { Equal = Item.Id }
            //};

            //List<ItemBasePrice> ItemBasePrices = await UOW.ItemBasePriceRepository.List(ItemBasePriceFilter);
            //var ItemBasePrice = ItemBasePrices.FirstOrDefault();
            //if (ItemBasePrice != null)
            //{
            //    Item.RetailPrice = ItemBasePrice.BasePrice;
            //}
            return Item;
        }

        public async Task<Item> GetItemByVariation(long ProductId, List<long> VariationIds)
        {
            Product Product = await UOW.ProductRepository.Get(ProductId);
            if (Product == null)
                return null;
            Product.IsFavorite = false;
            int LikeCount = await UOW.StoreUserFavoriteProductMappingRepository.Count(new StoreUserFavoriteProductMappingFilter
            {
                FavoriteProductId = new IdFilter { Equal = Product.Id },
                StoreUserId = new IdFilter { Equal = CurrentContext.StoreUserId }
            });
            if (LikeCount > 0)
            {
                Product.IsFavorite = true;
            }
            List<Item> Items = await UOW.ItemRepository.List(new ItemFilter
            {
                ProductId = new IdFilter
                {
                    Equal = ProductId
                },
                Selects = ItemSelect.ALL,
                Skip = 0,
                Take = int.MaxValue,
            });// list ra toàn bộ item theo product Id
            if (VariationIds.Count > 0)
            {
                List<VariationGrouping> variationGroupings = await UOW.VariationGroupingRepository.List(
                    new VariationGroupingFilter()
                    {
                        ProductId = new IdFilter
                        {
                            Equal = ProductId
                        },
                        Selects = VariationGroupingSelect.ALL,
                        Skip = 0,
                        Take = int.MaxValue,
                    }); // lấy ra VariationGrouping theo productId
                List<long> VariationGroupingIds = variationGroupings.Select(x => x.Id).ToList(); // lấy ra Ids của VariationGrouping
                List<Variation> Variations = await UOW.VariationRepository.List(
                     new VariationFilter()
                     {
                         Id = new IdFilter
                         {
                             In = VariationIds
                         },
                         VariationGroupingId = new IdFilter
                         {
                             In = VariationGroupingIds
                         },
                         Selects = VariationSelect.Code,
                         Skip = 0,
                         Take = int.MaxValue,
                     }
                ); // lấy ra toàn bộ variation theo variationIds và VariationGroupingIds
                List<string> VariationCodes = Variations.Select(x => x.Code).ToList();
                if (Items != null && Items.Any() && VariationCodes != null && VariationCodes.Any())
                {
                    foreach (string VariationCode in VariationCodes)
                    {
                        Items = Items.Where(x => x.Code.Contains(VariationCode)).ToList();
                    }
                };
            } // nếu chọn variation để filter item
            Item Result = Items.FirstOrDefault();
            if (Result == null)
            {
                return null;
            } // ko tìm thấy Item nào
            Result.Product = Product;
            return Result;
        } // lấy ra product và một item theo ItemFilter

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/item/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            string thumbnailPath = $"/item/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path, thumbnailPath, 128, 128);
            return Image;
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

    }
}
