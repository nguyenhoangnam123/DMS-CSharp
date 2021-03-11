using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class ProductHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Product);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext DataContext, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(DataContext, content);
        }

        private async Task Sync(DataContext DataContext, string json)
        {
            List<EventMessage<Product>> ProductEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Product>>>(json);
            List<Product> Products = ProductEventMessages.Select(x => x.Content).ToList();

            var ProductIds = Products.Select(x => x.Id).ToList();
            var VariationGroupingIds = Products.Where(x => x.VariationGroupings != null).SelectMany(x => x.VariationGroupings).Select(x => x.Id).ToList();
            var ItemIds = Products.Where(x => x.Items != null).SelectMany(x => x.Items).Select(x => x.Id).ToList();
            List<ProductDAO> ProductInDB = await DataContext.Product.Where(x => ProductIds.Contains(x.Id)).ToListAsync();
            List<VariationGroupingDAO> VariationGroupingInDB = await DataContext.VariationGrouping.Where(x => VariationGroupingIds.Contains(x.Id)).ToListAsync();
            List<ItemDAO> ItemInDB = await DataContext.Item.Where(x => ItemIds.Contains(x.Id)).ToListAsync();
            try
            {
                List<ProductDAO> ProductDAOs = new List<ProductDAO>();
                List<VariationGroupingDAO> VariationGroupingDAOs = new List<VariationGroupingDAO>();
                List<VariationDAO> VariationDAOs = new List<VariationDAO>();
                List<ImageDAO> ImageDAOs = new List<ImageDAO>();
                List<ProductImageMappingDAO> ProductImageMappingDAOs = new List<ProductImageMappingDAO>();
                List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
                List<ItemDAO> ItemDAOs = new List<ItemDAO>();
                List<ItemImageMappingDAO> ItemImageMappingDAOs = new List<ItemImageMappingDAO>();
                foreach (var Product in Products)
                {
                    ProductDAO ProductDAO = ProductInDB.Where(x => x.Id == Product.Id).FirstOrDefault();
                    if (ProductDAO == null)
                    {
                        ProductDAO = new ProductDAO();
                    }
                    ProductDAO.Id = Product.Id;
                    ProductDAO.CreatedAt = Product.CreatedAt;
                    ProductDAO.UpdatedAt = Product.UpdatedAt;
                    ProductDAO.DeletedAt = Product.DeletedAt;
                    ProductDAO.Code = Product.Code;
                    ProductDAO.Name = Product.Name;
                    ProductDAO.Description = Product.Description;
                    ProductDAO.ScanCode = Product.ScanCode;
                    ProductDAO.ERPCode = Product.ERPCode;
                    ProductDAO.CategoryId = Product.CategoryId;
                    ProductDAO.ProductTypeId = Product.ProductTypeId;
                    ProductDAO.BrandId = Product.BrandId;
                    ProductDAO.UnitOfMeasureId = Product.UnitOfMeasureId;
                    ProductDAO.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
                    ProductDAO.TaxTypeId = Product.TaxTypeId;
                    ProductDAO.StatusId = Product.StatusId;
                    ProductDAO.OtherName = Product.OtherName;
                    ProductDAO.TechnicalName = Product.TechnicalName;
                    ProductDAO.IsNew = Product.IsNew;
                    ProductDAO.UsedVariationId = Product.UsedVariationId;
                    ProductDAO.RowId = Product.RowId;
                    ProductDAO.Used = Product.Used;
                    ProductDAOs.Add(ProductDAO);

                    foreach (VariationGrouping VariationGrouping in Product.VariationGroupings)
                    {
                        VariationGroupingDAO VariationGroupingDAO = VariationGroupingInDB.Where(x => x.Id == VariationGrouping.Id).FirstOrDefault();
                        if (VariationGroupingDAO == null)
                        {
                            VariationGroupingDAO = new VariationGroupingDAO();
                        }
                        VariationGroupingDAO.Id = VariationGrouping.Id;
                        VariationGroupingDAO.Name = VariationGrouping.Name;
                        VariationGroupingDAO.ProductId = VariationGrouping.ProductId;
                        VariationGroupingDAO.RowId = VariationGrouping.RowId;
                        VariationGroupingDAO.CreatedAt = VariationGrouping.CreatedAt;
                        VariationGroupingDAO.UpdatedAt = VariationGrouping.UpdatedAt;
                        VariationGroupingDAO.DeletedAt = VariationGrouping.DeletedAt;
                        VariationGroupingDAO.Used = VariationGrouping.Used;
                        VariationGroupingDAOs.Add(VariationGroupingDAO);

                        foreach (Variation Variation in VariationGrouping.Variations)
                        {
                            VariationDAO VariationDAO = new VariationDAO
                            {
                                Id = Variation.Id,
                                Code = Variation.Code,
                                Name = Variation.Name,
                                VariationGroupingId = Variation.VariationGroupingId,
                                RowId = Variation.RowId,
                                CreatedAt = Variation.CreatedAt,
                                UpdatedAt = Variation.UpdatedAt,
                                DeletedAt = Variation.DeletedAt,
                                Used = Variation.Used
                            };
                            VariationDAOs.Add(VariationDAO);
                        }
                    }
                    // add item
                    foreach (var Item in Product.Items)
                    {
                        ItemDAO ItemDAO = ItemInDB.Where(x => x.Id == Item.Id).FirstOrDefault();
                        if (ItemDAO == null)
                        {
                            ItemDAO = new ItemDAO();
                        }
                        ItemDAO.Id = Item.Id;
                        ItemDAO.ProductId = Item.ProductId;
                        ItemDAO.Code = Item.Code;
                        ItemDAO.Name = Item.Name;
                        ItemDAO.ScanCode = Item.ScanCode;
                        ItemDAO.SalePrice = Item.SalePrice;
                        ItemDAO.StatusId = Item.StatusId;
                        ItemDAO.Used = Item.Used;
                        ItemDAO.CreatedAt = Item.CreatedAt;
                        ItemDAO.UpdatedAt = Item.UpdatedAt;
                        ItemDAO.DeletedAt = Item.DeletedAt;
                        ItemDAO.RowId = Item.RowId;
                        ItemDAOs.Add(ItemDAO);

                        if (Item.ItemImageMappings != null)
                        {
                            foreach (var ItemImageMapping in Item.ItemImageMappings)
                            {
                                ItemImageMappingDAO ItemImageMappingDAO = new ItemImageMappingDAO
                                {
                                    ItemId = Item.Id,
                                    ImageId = ItemImageMapping.ImageId
                                };
                                ItemImageMappingDAOs.Add(ItemImageMappingDAO);
                                ImageDAOs.Add(new ImageDAO
                                {
                                    Id = ItemImageMapping.Image.Id,
                                    Url = ItemImageMapping.Image.Url,
                                    ThumbnailUrl = ItemImageMapping.Image.ThumbnailUrl,
                                    RowId = ItemImageMapping.Image.RowId,
                                    Name = ItemImageMapping.Image.Name,
                                    CreatedAt = ItemImageMapping.Image.CreatedAt,
                                    UpdatedAt = ItemImageMapping.Image.UpdatedAt,
                                    DeletedAt = ItemImageMapping.Image.DeletedAt,
                                });
                            }
                        }
                    }

                    // add product productgrouping mapping 
                    foreach (var ProductProductGroupingMapping in Product.ProductProductGroupingMappings)
                    {
                        ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO
                        {
                            ProductId = ProductProductGroupingMapping.ProductId,
                            ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId,
                        };
                        ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                    }

                    foreach (var ProductImageMapping in Product.ProductImageMappings)
                    {
                        ProductImageMappingDAO ProductImageMappingDAO = new ProductImageMappingDAO
                        {
                            ProductId = ProductImageMapping.ProductId,
                            ImageId = ProductImageMapping.ImageId,
                        };
                        ProductImageMappingDAOs.Add(ProductImageMappingDAO);
                        ImageDAOs.Add(new ImageDAO
                        {
                            Id = ProductImageMapping.Image.Id,
                            Url = ProductImageMapping.Image.Url,
                            ThumbnailUrl = ProductImageMapping.Image.ThumbnailUrl,
                            RowId = ProductImageMapping.Image.RowId,
                            Name = ProductImageMapping.Image.Name,
                            CreatedAt = ProductImageMapping.Image.CreatedAt,
                            UpdatedAt = ProductImageMapping.Image.UpdatedAt,
                            DeletedAt = ProductImageMapping.Image.DeletedAt,
                        });
                    }
                }

                List<long> Ids = Products.Select(x => x.Id).ToList();

                await DataContext.ItemImageMapping
                  .Where(x => Ids.Contains(x.Item.ProductId))
                  .DeleteFromQueryAsync();

                await DataContext.ProductProductGroupingMapping
                  .Where(x => Ids.Contains(x.ProductId))
                  .DeleteFromQueryAsync();

                await DataContext.ProductImageMapping
                 .Where(x => Ids.Contains(x.ProductId))
                 .DeleteFromQueryAsync();

                await DataContext.Variation
                   .Where(x => Ids.Contains(x.VariationGrouping.ProductId))
                   .DeleteFromQueryAsync();

                await DataContext.BulkMergeAsync(ProductDAOs);
                await DataContext.BulkMergeAsync(ImageDAOs);
                await DataContext.BulkMergeAsync(ProductProductGroupingMappingDAOs);
                await DataContext.BulkMergeAsync(ProductImageMappingDAOs);
                await DataContext.BulkMergeAsync(VariationGroupingDAOs);
                await DataContext.BulkMergeAsync(VariationDAOs);
                await DataContext.BulkMergeAsync(ItemDAOs);
                await DataContext.BulkMergeAsync(ItemImageMappingDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(ProductHandler));
            }
        }
    }
}
