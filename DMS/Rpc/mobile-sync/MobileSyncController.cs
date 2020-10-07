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
                .Where(x => x.Product.UpdatedAt >= Timestamp && x.Product.StatusId ==StatusEnum.ACTIVE.Id)
                .Include(x => x.Image)
                .ToListAsync();

            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappings = await DataContext.ProductProductGroupingMapping
                .Where(x => x.Product.UpdatedAt >= Timestamp && x.Product.StatusId ==StatusEnum.ACTIVE.Id)
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
                MobileSync_ProductDTO MobileSync_ProductDTO = new MobileSync_ProductDTO
                {

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
