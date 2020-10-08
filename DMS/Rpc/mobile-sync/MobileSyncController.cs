using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Enums;
using DMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMS.Rpc.mobile_sync
{

    public class MobileSyncController : SimpleController
    {
        private DataContext DataContext;
        public MobileSyncController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        [Route("rpc/dms/mobile-sync/pull"), HttpPost]
        public async Task<MobileSync_ChangeDTO> Pull([FromBody] MobileSync_MobileSyncDTO MobileSync_MobileSyncDTO)
        {
            MobileSync_ChangeDTO MobileSync_ChangeDTO = new MobileSync_ChangeDTO();
            MobileSync_ChangeDTO.Banner = await BuildBanner(MobileSync_MobileSyncDTO.Timestamp);
            MobileSync_ChangeDTO.Product = await BuildProduct(MobileSync_MobileSyncDTO.Timestamp);
            return MobileSync_ChangeDTO;
        }

        private async Task<MobileSync_BannerSyncDTO> BuildBanner(DateTime Timestamp)
        {
            MobileSync_BannerSyncDTO MobileSync_BannerSyncDTO = new MobileSync_BannerSyncDTO()
            {
                Created = new List<MobileSync_BannerDTO>(),
                Updated = new List<MobileSync_BannerDTO>(),
                Deleted = new List<MobileSync_BannerDTO>()
            };
            List<BannerDAO> BannerDAOs = await DataContext.Banner.Where(x => x.UpdatedAt >= Timestamp).ToListAsync();
            foreach (BannerDAO BannerDAO in BannerDAOs)
            {
                MobileSync_BannerDTO MobileSync_BannerDTO = new MobileSync_BannerDTO
                {
                    Id = BannerDAO.Id,
                    Code = BannerDAO.Code,
                    Content = BannerDAO.Content,
                    Priority = BannerDAO.Priority,
                    StatusId = BannerDAO.StatusId,
                    UpdatedAt = BannerDAO.UpdatedAt,
                    CreatedAt = BannerDAO.CreatedAt,
                    ImageId = BannerDAO.ImageId,
                    Title = BannerDAO.Title,
                };
                MobileSync_BannerDTO.Images = new List<MobileSync_ImageDTO>();
                MobileSync_BannerDTO.Images.Add(new MobileSync_ImageDTO
                {
                    Id = BannerDAO.Image.Id,
                    Name = BannerDAO.Image.Name,
                    Url = BannerDAO.Image.Url,
                });

                if (BannerDAO.CreatedAt >= Timestamp)
                    MobileSync_BannerSyncDTO.Created.Add(MobileSync_BannerDTO);
                else if (BannerDAO.DeletedAt >= Timestamp)
                    MobileSync_BannerSyncDTO.Deleted.Add(MobileSync_BannerDTO);
                else
                    MobileSync_BannerSyncDTO.Updated.Add(MobileSync_BannerDTO);
            }
            return MobileSync_BannerSyncDTO;
        }

        private async Task<MobileSync_ProductSyncDTO> BuildProduct(DateTime Timestamp)
        {
            MobileSync_ProductSyncDTO MobileSync_ProductSyncDTO = new MobileSync_ProductSyncDTO()
            {
                Created = new List<MobileSync_ProductDTO>(),
                Updated = new List<MobileSync_ProductDTO>(),
                Deleted = new List<MobileSync_ProductDTO>()
            };
            List<ProductDAO> ProductDAOs = await DataContext.Product
                .Where(x => x.UpdatedAt >= Timestamp && x.StatusId == StatusEnum.ACTIVE.Id)
                .Include(x => x.ProductType)
                .Include(x => x.Brand)
                .Include(x => x.Supplier)
                .Include(x => x.TaxType)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.UsedVariation)
                .ToListAsync();

            List<ProductImageMappingDAO> ProductImageMappings = await DataContext.ProductImageMapping
                .Where(x => x.Product.UpdatedAt >= Timestamp && x.Product.StatusId == StatusEnum.ACTIVE.Id)
                .Include(x => x.Image)
                .ToListAsync();

            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping
                .Where(x => x.Product.UpdatedAt >= Timestamp && x.Product.StatusId == StatusEnum.ACTIVE.Id)
                .Include(x => x.ProductGrouping)
                .ToListAsync();

            List<ItemDAO> ItemDAOs = await DataContext.Item
                .Where(x => x.Product.UpdatedAt >= Timestamp && x.StatusId == StatusEnum.ACTIVE.Id)
                .ToListAsync();
            List<ItemImageMappingDAO> ItemImageMappingDAOs = await DataContext.ItemImageMapping
                .Where(x => x.Item.Product.UpdatedAt >= Timestamp && x.Item.StatusId == StatusEnum.ACTIVE.Id)
                .ToListAsync();

            List<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings =
                await (from uomg in DataContext.UnitOfMeasureGrouping
                       join p in DataContext.Product on uomg.Id equals p.UnitOfMeasureGroupingId
                       where p.UpdatedAt >= Timestamp
                       select uomg)
                .Include(x => x.UnitOfMeasureGroupingContents)
                .ToListAsync();
            foreach (ProductDAO ProductDAO in ProductDAOs)
            {
                ProductDAO.ProductImageMappings = ProductImageMappings.Where(x => x.ProductId == ProductDAO.Id).ToList();
                ProductDAO.ProductProductGroupingMappings = ProductProductGroupingMappings.Where(x => x.ProductId == ProductDAO.Id).ToList();
                ProductDAO.Items = ItemDAOs.Where(x => x.ProductId == x.ProductId).ToList();
                foreach (ItemDAO ItemDAO in ProductDAO.Items)
                {
                    ItemDAO.ItemImageMappings = ItemImageMappingDAOs.Where(x => x.ItemId == ItemDAO.Id).ToList();
                }
                ProductDAO.UnitOfMeasureGrouping = UnitOfMeasureGroupings.Where(x =>
                    ProductDAO.UnitOfMeasureGroupingId.HasValue &&
                    x.Id == ProductDAO.UnitOfMeasureGroupingId.Value)
                    .FirstOrDefault();

            }

            foreach (ProductDAO ProductDAO in ProductDAOs)
            {
                MobileSync_ProductDTO MobileSync_ProductDTO = new MobileSync_ProductDTO
                {
                    BrandId = ProductDAO.BrandId,
                    Code = ProductDAO.Code,
                    Description = ProductDAO.Description,
                    ERPCode = ProductDAO.ERPCode,
                    Id = ProductDAO.Id,
                    Name = ProductDAO.Name,
                    Note = ProductDAO.Note,
                    OtherName = ProductDAO.OtherName,
                    ProductTypeId = ProductDAO.ProductTypeId,
                    RetailPrice = ProductDAO.RetailPrice,
                    SalePrice = ProductDAO.SalePrice,
                    ScanCode = ProductDAO.ScanCode,
                    StatusId = ProductDAO.StatusId,
                    SupplierCode = ProductDAO.SupplierCode,
                    SupplierId = ProductDAO.SupplierId,
                    TaxTypeId = ProductDAO.TaxTypeId,
                    TechnicalName = ProductDAO.TechnicalName,
                    UnitOfMeasureGroupingId = ProductDAO.UnitOfMeasureGroupingId,
                    UnitOfMeasureId = ProductDAO.UnitOfMeasureId,
                    ProductProductGroupingMappings = ProductDAO.ProductProductGroupingMappings?.Select(x => new MobileSync_ProductProductGroupingMappingDTO
                    {
                        ProductGroupingId = x.ProductGroupingId,
                        ProductId = x.ProductId,
                        ProductGrouping = x.ProductGrouping == null ? null : new MobileSync_ProductGroupingDTO
                        {
                            Code = x.ProductGrouping.Code,
                            Description = x.ProductGrouping.Description,
                            Id = x.ProductGrouping.Id,
                            Name = x.ProductGrouping.Name,
                            ParentId = x.ProductGrouping.ParentId,
                            Path = x.ProductGrouping.Path,
                        },
                    }).ToList(),
                    ProductType = ProductDAO.ProductType == null ? null : new MobileSync_ProductTypeDTO
                    {
                        Code = ProductDAO.ProductType.Code,
                        Description = ProductDAO.ProductType.Description,
                        Id = ProductDAO.ProductType.Id,
                        Name = ProductDAO.ProductType.Name,
                        StatusId = ProductDAO.ProductType.StatusId,
                        UpdatedAt = ProductDAO.ProductType.UpdatedAt,
                    },
                    Supplier = ProductDAO.Supplier == null ? null : new MobileSync_SupplierDTO
                    {
                        Id = ProductDAO.Supplier.Id,
                        Code = ProductDAO.Supplier.Code,
                        Name = ProductDAO.Supplier.Name,
                    },
                    TaxType = ProductDAO.TaxType == null ? null : new MobileSync_TaxTypeDTO
                    {
                        Id = ProductDAO.TaxType.Id,
                        Code = ProductDAO.TaxType.Code,
                        Name = ProductDAO.TaxType.Name,
                    },
                    UnitOfMeasure = ProductDAO.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO
                    {
                        Id = ProductDAO.UnitOfMeasure.Id,
                        Code = ProductDAO.UnitOfMeasure.Code,
                        Name = ProductDAO.UnitOfMeasure.Name,
                    },
                    UnitOfMeasureGrouping = ProductDAO.UnitOfMeasureGrouping == null ? null : new MobileSync_UnitOfMeasureGroupingDTO
                    {
                        Id = ProductDAO.UnitOfMeasureGrouping.Id,
                        Code = ProductDAO.UnitOfMeasureGrouping.Code,
                        Name = ProductDAO.UnitOfMeasureGrouping.Name,
                        UnitOfMeasureGroupingContents = ProductDAO.UnitOfMeasureGrouping.UnitOfMeasureGroupingContents?.Select(x => new MobileSync_UnitOfMeasureGroupingContentDTO
                        {
                            Factor = x.Factor,
                            Id = x.Id,
                            UnitOfMeasureGroupingId = x.UnitOfMeasureGroupingId,
                            UnitOfMeasureId = x.UnitOfMeasureId,
                            UnitOfMeasure = x.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO
                            {
                                Id = x.UnitOfMeasure.Id,
                                Code = x.UnitOfMeasure.Code,
                                Name = x.UnitOfMeasure.Name,
                            }
                        }).ToList(),
                    },
                };
                if (ProductDAO.CreatedAt >= Timestamp)
                    MobileSync_ProductSyncDTO.Created.Add(MobileSync_ProductDTO);
                else if (ProductDAO.DeletedAt >= Timestamp)
                    MobileSync_ProductSyncDTO.Deleted.Add(MobileSync_ProductDTO);
                else
                    MobileSync_ProductSyncDTO.Updated.Add(MobileSync_ProductDTO);
            }
            return MobileSync_ProductSyncDTO;
        }
    }
}
