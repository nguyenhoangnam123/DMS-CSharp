using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MBanner;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.RpcPublic
{
    [AllowAnonymous]
    public class PublicController : ControllerBase
    {
        private IBannerService BannerService;
        private IItemService ItemService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        public PublicController(
            IBannerService BannerService,
            IItemService ItemService,
            IIndirectSalesOrderService IndirectSalesOrderService
            )
        {
            this.BannerService = BannerService;
            this.ItemService = ItemService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
        }

        [Route(PublicRoute.CountBanner), HttpPost]
        public async Task<ActionResult<int>> CountBanner([FromBody] Public_BannerFilterDTO Public_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = Public_BannerFilterDTO.Skip;
            BannerFilter.Take = Public_BannerFilterDTO.Take;
            BannerFilter.OrderBy = Public_BannerFilterDTO.OrderBy;
            BannerFilter.OrderType = Public_BannerFilterDTO.OrderType;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            int count = await BannerService.Count(BannerFilter);
            return count;
        }

        [Route(PublicRoute.ListBanner), HttpPost]
        public async Task<ActionResult<List<Public_BannerDTO>>> ListBanner([FromBody] Public_BannerFilterDTO Public_BannerFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            BannerFilter BannerFilter = new BannerFilter();
            BannerFilter.Selects = BannerSelect.ALL;
            BannerFilter.Skip = Public_BannerFilterDTO.Skip;
            BannerFilter.Take = Public_BannerFilterDTO.Take;
            BannerFilter.OrderBy = BannerOrder.Priority;
            BannerFilter.OrderType = OrderType.ASC;
            BannerFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Banner> Banners = await BannerService.List(BannerFilter);
            List<Public_BannerDTO> Public_BannerDTOs = Banners
                .Select(c => new Public_BannerDTO(c)).ToList();
            return Public_BannerDTOs;
        }

        [Route(PublicRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] Public_ItemFilterDTO Public_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Search = Public_ItemFilterDTO.Search;
            ItemFilter.Id = Public_ItemFilterDTO.Id;
            ItemFilter.Code = Public_ItemFilterDTO.Code;
            ItemFilter.Name = Public_ItemFilterDTO.Name;
            ItemFilter.OtherName = Public_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = Public_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = Public_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = Public_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = Public_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = Public_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Public_ItemFilterDTO.ScanCode;
            ItemFilter.IsNew = Public_ItemFilterDTO.IsNew;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = Public_ItemFilterDTO.SupplierId;
            return await ItemService.Count(ItemFilter);
        }

        [Route(PublicRoute.ListItem), HttpPost]
        public async Task<List<Public_ItemDTO>> ListItem([FromBody] Public_ItemFilterDTO Public_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Search = Public_ItemFilterDTO.Search;
            ItemFilter.Skip = Public_ItemFilterDTO.Skip;
            ItemFilter.Take = Public_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.DESC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Public_ItemFilterDTO.Id;
            ItemFilter.Code = Public_ItemFilterDTO.Code;
            ItemFilter.Name = Public_ItemFilterDTO.Name;
            ItemFilter.OtherName = Public_ItemFilterDTO.OtherName;
            ItemFilter.ProductGroupingId = Public_ItemFilterDTO.ProductGroupingId;
            ItemFilter.ProductId = Public_ItemFilterDTO.ProductId;
            ItemFilter.ProductTypeId = Public_ItemFilterDTO.ProductTypeId;
            ItemFilter.RetailPrice = Public_ItemFilterDTO.RetailPrice;
            ItemFilter.SalePrice = Public_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Public_ItemFilterDTO.ScanCode;
            ItemFilter.IsNew = Public_ItemFilterDTO.IsNew;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ItemFilter.SupplierId = Public_ItemFilterDTO.SupplierId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Public_ItemDTO> Public_ItemDTOs = Items
                .Select(x => new Public_ItemDTO(x)).ToList();
            return Public_ItemDTOs;
        }

        [Route(PublicRoute.GetItem), HttpPost]
        public async Task<Public_ItemDTO> GetItem([FromBody] Public_ItemDTO Public_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Item Item = await ItemService.Get(Public_ItemDTO.Id);
            if (Item == null)
                return null;
            Public_ItemDTO = new Public_ItemDTO(Item);
            return Public_ItemDTO;
        }

    }
}
