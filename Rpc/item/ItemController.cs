using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DMS.Entities;
using DMS.Services.MItem;
using DMS.Services.MProduct;

namespace DMS.Rpc.item
{
    public class ItemRoute : Root
    {
        public const string Master = Module + "/item/item-master";
        public const string Detail = Module + "/item/item-detail";
        private const string Default = Rpc + Module + "/item";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListProduct = Default + "/single-list-product";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ItemFilter.Id), FieldType.ID },
            { nameof(ItemFilter.ProductId), FieldType.ID },
            { nameof(ItemFilter.Code), FieldType.STRING },
            { nameof(ItemFilter.Name), FieldType.STRING },
            { nameof(ItemFilter.ScanCode), FieldType.STRING },
            { nameof(ItemFilter.SalePrice), FieldType.DECIMAL },
            { nameof(ItemFilter.RetailPrice), FieldType.DECIMAL },
        };
    }

    public class ItemController : RpcController
    {
        private IProductService ProductService;
        private IItemService ItemService;
        private ICurrentContext CurrentContext;
        public ItemController(
            IProductService ProductService,
            IItemService ItemService,
            ICurrentContext CurrentContext
        )
        {
            this.ProductService = ProductService;
            this.ItemService = ItemService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ItemRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Item_ItemFilterDTO Item_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = ConvertFilterDTOToFilterEntity(Item_ItemFilterDTO);
            ItemFilter = ItemService.ToFilter(ItemFilter);
            int count = await ItemService.Count(ItemFilter);
            return count;
        }

        [Route(ItemRoute.List), HttpPost]
        public async Task<ActionResult<List<Item_ItemDTO>>> List([FromBody] Item_ItemFilterDTO Item_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = ConvertFilterDTOToFilterEntity(Item_ItemFilterDTO);
            ItemFilter = ItemService.ToFilter(ItemFilter);
            List<Item> Items = await ItemService.List(ItemFilter);
            List<Item_ItemDTO> Item_ItemDTOs = Items
                .Select(c => new Item_ItemDTO(c)).ToList();
            return Item_ItemDTOs;
        }

        [Route(ItemRoute.Get), HttpPost]
        public async Task<ActionResult<Item_ItemDTO>> Get([FromBody]Item_ItemDTO Item_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Item_ItemDTO.Id))
                return Forbid();

            Item Item = await ItemService.Get(Item_ItemDTO.Id);
            return new Item_ItemDTO(Item);
        }

        [Route(ItemRoute.Create), HttpPost]
        public async Task<ActionResult<Item_ItemDTO>> Create([FromBody] Item_ItemDTO Item_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Item_ItemDTO.Id))
                return Forbid();

            Item Item = ConvertDTOToEntity(Item_ItemDTO);
            Item = await ItemService.Create(Item);
            Item_ItemDTO = new Item_ItemDTO(Item);
            if (Item.IsValidated)
                return Item_ItemDTO;
            else
                return BadRequest(Item_ItemDTO);
        }

        [Route(ItemRoute.Update), HttpPost]
        public async Task<ActionResult<Item_ItemDTO>> Update([FromBody] Item_ItemDTO Item_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Item_ItemDTO.Id))
                return Forbid();

            Item Item = ConvertDTOToEntity(Item_ItemDTO);
            Item = await ItemService.Update(Item);
            Item_ItemDTO = new Item_ItemDTO(Item);
            if (Item.IsValidated)
                return Item_ItemDTO;
            else
                return BadRequest(Item_ItemDTO);
        }

        [Route(ItemRoute.Delete), HttpPost]
        public async Task<ActionResult<Item_ItemDTO>> Delete([FromBody] Item_ItemDTO Item_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Item_ItemDTO.Id))
                return Forbid();

            Item Item = ConvertDTOToEntity(Item_ItemDTO);
            Item = await ItemService.Delete(Item);
            Item_ItemDTO = new Item_ItemDTO(Item);
            if (Item.IsValidated)
                return Item_ItemDTO;
            else
                return BadRequest(Item_ItemDTO);
        }

        [Route(ItemRoute.Import), HttpPost]
        public async Task<ActionResult<List<Item_ItemDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Item> Items = await ItemService.Import(DataFile);
            List<Item_ItemDTO> Item_ItemDTOs = Items
                .Select(c => new Item_ItemDTO(c)).ToList();
            return Item_ItemDTOs;
        }

        [Route(ItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Item_ItemFilterDTO Item_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = ConvertFilterDTOToFilterEntity(Item_ItemFilterDTO);
            ItemFilter = ItemService.ToFilter(ItemFilter);
            DataFile DataFile = await ItemService.Export(ItemFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(ItemRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id.In = Ids;
            ItemFilter.Selects = ItemSelect.Id;
            ItemFilter.Skip = 0;
            ItemFilter.Take = int.MaxValue;

            List<Item> Items = await ItemService.List(ItemFilter);
            Items = await ItemService.BulkDelete(Items);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ItemFilter ItemFilter = new ItemFilter();
            if (Id > 0)
                ItemFilter.Id = new IdFilter { Equal = Id };
            ItemFilter = ItemService.ToFilter(ItemFilter);
            int count = await ItemService.Count(ItemFilter);
            if (count == 0)
                return false;
            return true;
        }

        private Item ConvertDTOToEntity(Item_ItemDTO Item_ItemDTO)
        {
            Item Item = new Item();
            Item.Id = Item_ItemDTO.Id;
            Item.ProductId = Item_ItemDTO.ProductId;
            Item.Code = Item_ItemDTO.Code;
            Item.Name = Item_ItemDTO.Name;
            Item.ScanCode = Item_ItemDTO.ScanCode;
            Item.SalePrice = Item_ItemDTO.SalePrice;
            Item.RetailPrice = Item_ItemDTO.RetailPrice;
            Item.Product = Item_ItemDTO.Product == null ? null : new Product
            {
                Id = Item_ItemDTO.Product.Id,
                Code = Item_ItemDTO.Product.Code,
                SupplierCode = Item_ItemDTO.Product.SupplierCode,
                Name = Item_ItemDTO.Product.Name,
                Description = Item_ItemDTO.Product.Description,
                ScanCode = Item_ItemDTO.Product.ScanCode,
                ProductTypeId = Item_ItemDTO.Product.ProductTypeId,
                SupplierId = Item_ItemDTO.Product.SupplierId,
                BrandId = Item_ItemDTO.Product.BrandId,
                UnitOfMeasureId = Item_ItemDTO.Product.UnitOfMeasureId,
                UnitOfMeasureGroupingId = Item_ItemDTO.Product.UnitOfMeasureGroupingId,
                SalePrice = Item_ItemDTO.Product.SalePrice,
                RetailPrice = Item_ItemDTO.Product.RetailPrice,
                TaxTypeId = Item_ItemDTO.Product.TaxTypeId,
                StatusId = Item_ItemDTO.Product.StatusId,
            };
            Item.BaseLanguage = CurrentContext.Language;
            return Item;
        }

        private ItemFilter ConvertFilterDTOToFilterEntity(Item_ItemFilterDTO Item_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Skip = Item_ItemFilterDTO.Skip;
            ItemFilter.Take = Item_ItemFilterDTO.Take;
            ItemFilter.OrderBy = Item_ItemFilterDTO.OrderBy;
            ItemFilter.OrderType = Item_ItemFilterDTO.OrderType;

            ItemFilter.Id = Item_ItemFilterDTO.Id;
            ItemFilter.ProductId = Item_ItemFilterDTO.ProductId;
            ItemFilter.Code = Item_ItemFilterDTO.Code;
            ItemFilter.Name = Item_ItemFilterDTO.Name;
            ItemFilter.ScanCode = Item_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = Item_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = Item_ItemFilterDTO.RetailPrice;
            return ItemFilter;
        }

        [Route(ItemRoute.SingleListProduct), HttpPost]
        public async Task<List<Item_ProductDTO>> SingleListProduct([FromBody] Item_ProductFilterDTO Item_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = 0;
            ProductFilter.Take = 20;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = Item_ProductFilterDTO.Id;
            ProductFilter.Code = Item_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = Item_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = Item_ProductFilterDTO.Name;
            ProductFilter.Description = Item_ProductFilterDTO.Description;
            ProductFilter.ScanCode = Item_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = Item_ProductFilterDTO.ProductTypeId;
            ProductFilter.SupplierId = Item_ProductFilterDTO.SupplierId;
            ProductFilter.BrandId = Item_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = Item_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = Item_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = Item_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = Item_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = Item_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = Item_ProductFilterDTO.StatusId;

            List<Product> Products = await ProductService.List(ProductFilter);
            List<Item_ProductDTO> Item_ProductDTOs = Products
                .Select(x => new Item_ProductDTO(x)).ToList();
            return Item_ProductDTOs;
        }

    }
}

