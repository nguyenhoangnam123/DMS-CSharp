using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            foreach(BannerDAO BannerDAO in BannerDAOs)
            {
                MobileSync_BannerDTO MobileSync_BannerDTO = new MobileSync_BannerDTO(BannerDAO);
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
            List<ProductDAO> ProductDAOs = await DataContext.Product.Where(x => x.UpdatedAt >= Timestamp).ToListAsync();
            foreach (ProductDAO ProductDAO in ProductDAOs)
            {
                MobileSync_ProductDTO MobileSync_ProductDTO = new MobileSync_ProductDTO(ProductDAO);
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
