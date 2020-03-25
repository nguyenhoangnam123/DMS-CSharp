using Common;
using DMS.Entities;
using DMS.Services.MBrand;
using DMS.Services.MImage;
using DMS.Services.MItem;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using DMS.Services.MTaxType;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MVariationGrouping;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product
{
    public class ProductRoute : Root
    {
        public const string Master = Module + "/product/product-master";
        public const string Detail = Module + "/product/product-detail";
        private const string Default = Rpc + Module + "/product";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListUnitOfMeasureGrouping = Default + "/single-list-unit-of-measure-grouping";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListVariationGrouping = Default + "/single-list-variation-grouping";
        public const string CountImage = Default + "/count-image";
        public const string ListImage = Default + "/list-image";
        public const string CountProductGrouping = Default + "/count-product-grouping";
        public const string ListProductGrouping = Default + "/list-product-grouping";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ProductFilter.Id), FieldType.ID },
            { nameof(ProductFilter.Code), FieldType.STRING },
            { nameof(ProductFilter.SupplierCode), FieldType.STRING },
            { nameof(ProductFilter.Name), FieldType.STRING },
            { nameof(ProductFilter.Description), FieldType.STRING },
            { nameof(ProductFilter.ScanCode), FieldType.STRING },
            { nameof(ProductFilter.ProductTypeId), FieldType.ID },
            { nameof(ProductFilter.SupplierId), FieldType.ID },
            { nameof(ProductFilter.BrandId), FieldType.ID },
            { nameof(ProductFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(ProductFilter.UnitOfMeasureGroupingId), FieldType.ID },
            { nameof(ProductFilter.SalePrice), FieldType.DECIMAL },
            { nameof(ProductFilter.RetailPrice), FieldType.DECIMAL },
            { nameof(ProductFilter.TaxTypeId), FieldType.ID },
            { nameof(ProductFilter.StatusId), FieldType.ID },
        };
    }

    public class ProductController : RpcController
    {
        private IBrandService BrandService;
        private IProductTypeService ProductTypeService;
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private ITaxTypeService TaxTypeService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IItemService ItemService;
        private IImageService ImageService;
        private IProductGroupingService ProductGroupingService;
        private IVariationGroupingService VariationGroupingService;
        private IProductService ProductService;
        private ICurrentContext CurrentContext;
        public ProductController(
            IBrandService BrandService,
            IProductTypeService ProductTypeService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            ITaxTypeService TaxTypeService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IItemService ItemService,
            IImageService ImageService,
            IProductGroupingService ProductGroupingService,
            IVariationGroupingService VariationGroupingService,
            IProductService ProductService,
            ICurrentContext CurrentContext
        )
        {
            this.BrandService = BrandService;
            this.ProductTypeService = ProductTypeService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.TaxTypeService = TaxTypeService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.ItemService = ItemService;
            this.ImageService = ImageService;
            this.ProductGroupingService = ProductGroupingService;
            this.VariationGroupingService = VariationGroupingService;
            this.ProductService = ProductService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProductRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            int count = await ProductService.Count(ProductFilter);
            return count;
        }

        [Route(ProductRoute.List), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> List([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Get), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Get([FromBody]Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = await ProductService.Get(Product_ProductDTO.Id);
            return new Product_ProductDTO(Product);
        }

        [Route(ProductRoute.Create), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Create([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Create(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Update), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Update([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Update(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Delete), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Delete([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Delete(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Import), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Product> Products = await ProductService.Import(DataFile);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            DataFile DataFile = await ProductService.Export(ProductFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(ProductRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = new IdFilter { In = Ids };
            ProductFilter.Selects = ProductSelect.Id;
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;

            List<Product> Products = await ProductService.List(ProductFilter);
            Products = await ProductService.BulkDelete(Products);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter = ProductService.ToFilter(ProductFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProductFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductService.Count(ProductFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Product ConvertDTOToEntity(Product_ProductDTO Product_ProductDTO)
        {
            Product Product = new Product();
            Product.Id = Product_ProductDTO.Id;
            Product.Code = Product_ProductDTO.Code;
            Product.SupplierCode = Product_ProductDTO.SupplierCode;
            Product.Name = Product_ProductDTO.Name;
            Product.Description = Product_ProductDTO.Description;
            Product.ScanCode = Product_ProductDTO.ScanCode;
            Product.ProductTypeId = Product_ProductDTO.ProductTypeId;
            Product.SupplierId = Product_ProductDTO.SupplierId;
            Product.BrandId = Product_ProductDTO.BrandId;
            Product.UnitOfMeasureId = Product_ProductDTO.UnitOfMeasureId;
            Product.UnitOfMeasureGroupingId = Product_ProductDTO.UnitOfMeasureGroupingId;
            Product.SalePrice = Product_ProductDTO.SalePrice;
            Product.RetailPrice = Product_ProductDTO.RetailPrice;
            Product.TaxTypeId = Product_ProductDTO.TaxTypeId;
            Product.StatusId = Product_ProductDTO.StatusId;
            Product.OtherName = Product_ProductDTO.OtherName;
            Product.TechnicalName = Product_ProductDTO.TechnicalName;
            Product.Note = Product_ProductDTO.Note;
            Product.Brand = Product_ProductDTO.Brand == null ? null : new Brand
            {
                Id = Product_ProductDTO.Brand.Id,
                Code = Product_ProductDTO.Brand.Code,
                Name = Product_ProductDTO.Brand.Name,
                StatusId = Product_ProductDTO.Brand.StatusId,
            };
            Product.ProductType = Product_ProductDTO.ProductType == null ? null : new ProductType
            {
                Id = Product_ProductDTO.ProductType.Id,
                Code = Product_ProductDTO.ProductType.Code,
                Name = Product_ProductDTO.ProductType.Name,
                Description = Product_ProductDTO.ProductType.Description,
                StatusId = Product_ProductDTO.ProductType.StatusId,
            };
            Product.Status = Product_ProductDTO.Status == null ? null : new Status
            {
                Id = Product_ProductDTO.Status.Id,
                Code = Product_ProductDTO.Status.Code,
                Name = Product_ProductDTO.Status.Name,
            };
            Product.Supplier = Product_ProductDTO.Supplier == null ? null : new Supplier
            {
                Id = Product_ProductDTO.Supplier.Id,
                Code = Product_ProductDTO.Supplier.Code,
                Name = Product_ProductDTO.Supplier.Name,
                TaxCode = Product_ProductDTO.Supplier.TaxCode,
                StatusId = Product_ProductDTO.Supplier.StatusId,
            };
            Product.TaxType = Product_ProductDTO.TaxType == null ? null : new TaxType
            {
                Id = Product_ProductDTO.TaxType.Id,
                Code = Product_ProductDTO.TaxType.Code,
                Name = Product_ProductDTO.TaxType.Name,
                Percentage = Product_ProductDTO.TaxType.Percentage,
                StatusId = Product_ProductDTO.TaxType.StatusId,
            };
            Product.UnitOfMeasure = Product_ProductDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = Product_ProductDTO.UnitOfMeasure.Id,
                Code = Product_ProductDTO.UnitOfMeasure.Code,
                Name = Product_ProductDTO.UnitOfMeasure.Name,
                Description = Product_ProductDTO.UnitOfMeasure.Description,
                StatusId = Product_ProductDTO.UnitOfMeasure.StatusId,
            };
            Product.UnitOfMeasureGrouping = Product_ProductDTO.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
            {
                Id = Product_ProductDTO.UnitOfMeasureGrouping.Id,
                Name = Product_ProductDTO.UnitOfMeasureGrouping.Name,
                UnitOfMeasureId = Product_ProductDTO.UnitOfMeasureGrouping.UnitOfMeasureId,
                StatusId = Product_ProductDTO.UnitOfMeasureGrouping.StatusId,
            };
            Product.Items = Product_ProductDTO.Items?
                .Select(x => new Item
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
                }).ToList();
            Product.ProductImageMappings = Product_ProductDTO.ProductImageMappings?
                .Select(x => new ProductImageMapping
                {
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                }).ToList();
            Product.ProductProductGroupingMappings = Product_ProductDTO.ProductProductGroupingMappings?
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductGroupingId = x.ProductGroupingId,
                    ProductGrouping = x.ProductGrouping == null ? null : new ProductGrouping
                    {
                        Id = x.ProductGrouping.Id,
                        Code = x.ProductGrouping.Code,
                        Name = x.ProductGrouping.Name,
                        ParentId = x.ProductGrouping.ParentId,
                        Path = x.ProductGrouping.Path,
                        Description = x.ProductGrouping.Description,
                    },
                }).ToList();
            Product.VariationGroupings = Product_ProductDTO.VariationGroupings?
                .Select(x => new VariationGrouping
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
            Product.BaseLanguage = CurrentContext.Language;
            return Product;
        }

        private ProductFilter ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Skip = Product_ProductFilterDTO.Skip;
            ProductFilter.Take = Product_ProductFilterDTO.Take;
            ProductFilter.OrderBy = Product_ProductFilterDTO.OrderBy;
            ProductFilter.OrderType = Product_ProductFilterDTO.OrderType;

            ProductFilter.Id = Product_ProductFilterDTO.Id;
            ProductFilter.Code = Product_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = Product_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = Product_ProductFilterDTO.Name;
            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.ScanCode = Product_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = Product_ProductFilterDTO.ProductTypeId;
            ProductFilter.SupplierId = Product_ProductFilterDTO.SupplierId;
            ProductFilter.BrandId = Product_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = Product_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = Product_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = Product_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = Product_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = Product_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = Product_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = Product_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = Product_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = Product_ProductFilterDTO.Note;
            return ProductFilter;
        }

        [Route(ProductRoute.SingleListBrand), HttpPost]
        public async Task<List<Product_BrandDTO>> SingleListBrand([FromBody] Product_BrandFilterDTO Product_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Id = Product_BrandFilterDTO.Id;
            BrandFilter.Code = Product_BrandFilterDTO.Code;
            BrandFilter.Name = Product_BrandFilterDTO.Name;
            BrandFilter.StatusId = Product_BrandFilterDTO.StatusId;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Product_BrandDTO> Product_BrandDTOs = Brands
                .Select(x => new Product_BrandDTO(x)).ToList();
            return Product_BrandDTOs;
        }
        [Route(ProductRoute.SingleListProductType), HttpPost]
        public async Task<List<Product_ProductTypeDTO>> SingleListProductType([FromBody] Product_ProductTypeFilterDTO Product_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = Product_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = Product_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = Product_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = Product_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = Product_ProductTypeFilterDTO.StatusId;

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<Product_ProductTypeDTO> Product_ProductTypeDTOs = ProductTypes
                .Select(x => new Product_ProductTypeDTO(x)).ToList();
            return Product_ProductTypeDTOs;
        }
        [Route(ProductRoute.SingleListStatus), HttpPost]
        public async Task<List<Product_StatusDTO>> SingleListStatus([FromBody] Product_StatusFilterDTO Product_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;
            StatusFilter.Id = Product_StatusFilterDTO.Id;
            StatusFilter.Code = Product_StatusFilterDTO.Code;
            StatusFilter.Name = Product_StatusFilterDTO.Name;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Product_StatusDTO> Product_StatusDTOs = Statuses
                .Select(x => new Product_StatusDTO(x)).ToList();
            return Product_StatusDTOs;
        }
        [Route(ProductRoute.SingleListSupplier), HttpPost]
        public async Task<List<Product_SupplierDTO>> SingleListSupplier([FromBody] Product_SupplierFilterDTO Product_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = Product_SupplierFilterDTO.Id;
            SupplierFilter.Code = Product_SupplierFilterDTO.Code;
            SupplierFilter.Name = Product_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Product_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = Product_SupplierFilterDTO.StatusId;

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Product_SupplierDTO> Product_SupplierDTOs = Suppliers
                .Select(x => new Product_SupplierDTO(x)).ToList();
            return Product_SupplierDTOs;
        }
        [Route(ProductRoute.SingleListTaxType), HttpPost]
        public async Task<List<Product_TaxTypeDTO>> SingleListTaxType([FromBody] Product_TaxTypeFilterDTO Product_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = Product_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = Product_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = Product_TaxTypeFilterDTO.Name;
            TaxTypeFilter.Percentage = Product_TaxTypeFilterDTO.Percentage;
            TaxTypeFilter.StatusId = Product_TaxTypeFilterDTO.StatusId;

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<Product_TaxTypeDTO> Product_TaxTypeDTOs = TaxTypes
                .Select(x => new Product_TaxTypeDTO(x)).ToList();
            return Product_TaxTypeDTOs;
        }
        [Route(ProductRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<Product_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] Product_UnitOfMeasureFilterDTO Product_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = Product_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = Product_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = Product_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = Product_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = Product_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<Product_UnitOfMeasureDTO> Product_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new Product_UnitOfMeasureDTO(x)).ToList();
            return Product_UnitOfMeasureDTOs;
        }
        [Route(ProductRoute.SingleListUnitOfMeasureGrouping), HttpPost]
        public async Task<List<Product_UnitOfMeasureGroupingDTO>> SingleListUnitOfMeasureGrouping([FromBody] Product_UnitOfMeasureGroupingFilterDTO Product_UnitOfMeasureGroupingFilterDTO)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter.Skip = 0;
            UnitOfMeasureGroupingFilter.Take = 20;
            UnitOfMeasureGroupingFilter.OrderBy = UnitOfMeasureGroupingOrder.Id;
            UnitOfMeasureGroupingFilter.OrderType = OrderType.ASC;
            UnitOfMeasureGroupingFilter.Selects = UnitOfMeasureGroupingSelect.ALL;
            UnitOfMeasureGroupingFilter.Id = Product_UnitOfMeasureGroupingFilterDTO.Id;
            UnitOfMeasureGroupingFilter.Name = Product_UnitOfMeasureGroupingFilterDTO.Name;
            UnitOfMeasureGroupingFilter.UnitOfMeasureId = Product_UnitOfMeasureGroupingFilterDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingFilter.StatusId = Product_UnitOfMeasureGroupingFilterDTO.StatusId;

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            List<Product_UnitOfMeasureGroupingDTO> Product_UnitOfMeasureGroupingDTOs = UnitOfMeasureGroupings
                .Select(x => new Product_UnitOfMeasureGroupingDTO(x)).ToList();
            return Product_UnitOfMeasureGroupingDTOs;
        }
        [Route(ProductRoute.SingleListItem), HttpPost]
        public async Task<List<Product_ItemDTO>> SingleListItem([FromBody] Product_ItemFilterDTO Product_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Product_ItemFilterDTO.Id;
            ItemFilter.ProductId = Product_ItemFilterDTO.ProductId;
            ItemFilter.Code = Product_ItemFilterDTO.Code;
            ItemFilter.Name = Product_ItemFilterDTO.Name;
            ItemFilter.ScanCode = Product_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = Product_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = Product_ItemFilterDTO.RetailPrice;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Product_ItemDTO> Product_ItemDTOs = Items
                .Select(x => new Product_ItemDTO(x)).ToList();
            return Product_ItemDTOs;
        }
        [Route(ProductRoute.SingleListImage), HttpPost]
        public async Task<List<Product_ImageDTO>> SingleListImage([FromBody] Product_ImageFilterDTO Product_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Skip = 0;
            ImageFilter.Take = 20;
            ImageFilter.OrderBy = ImageOrder.Id;
            ImageFilter.OrderType = OrderType.ASC;
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Id = Product_ImageFilterDTO.Id;
            ImageFilter.Name = Product_ImageFilterDTO.Name;
            ImageFilter.Url = Product_ImageFilterDTO.Url;

            List<Image> Images = await ImageService.List(ImageFilter);
            List<Product_ImageDTO> Product_ImageDTOs = Images
                .Select(x => new Product_ImageDTO(x)).ToList();
            return Product_ImageDTOs;
        }
        [Route(ProductRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> SingleListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = 99999;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Description = Product_ProductGroupingFilterDTO.Description;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }
        [Route(ProductRoute.SingleListVariationGrouping), HttpPost]
        public async Task<List<Product_VariationGroupingDTO>> SingleListVariationGrouping([FromBody] Product_VariationGroupingFilterDTO Product_VariationGroupingFilterDTO)
        {
            VariationGroupingFilter VariationGroupingFilter = new VariationGroupingFilter();
            VariationGroupingFilter.Skip = 0;
            VariationGroupingFilter.Take = 20;
            VariationGroupingFilter.OrderBy = VariationGroupingOrder.Id;
            VariationGroupingFilter.OrderType = OrderType.ASC;
            VariationGroupingFilter.Selects = VariationGroupingSelect.ALL;
            VariationGroupingFilter.Id = Product_VariationGroupingFilterDTO.Id;
            VariationGroupingFilter.Name = Product_VariationGroupingFilterDTO.Name;
            VariationGroupingFilter.ProductId = Product_VariationGroupingFilterDTO.ProductId;

            List<VariationGrouping> VariationGroupings = await VariationGroupingService.List(VariationGroupingFilter);
            List<Product_VariationGroupingDTO> Product_VariationGroupingDTOs = VariationGroupings
                .Select(x => new Product_VariationGroupingDTO(x)).ToList();
            return Product_VariationGroupingDTOs;
        }

        [Route(ProductRoute.CountImage), HttpPost]
        public async Task<long> CountImage([FromBody] Product_ImageFilterDTO Product_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Id = Product_ImageFilterDTO.Id;
            ImageFilter.Name = Product_ImageFilterDTO.Name;
            ImageFilter.Url = Product_ImageFilterDTO.Url;

            return await ImageService.Count(ImageFilter);
        }

        [Route(ProductRoute.ListImage), HttpPost]
        public async Task<List<Product_ImageDTO>> ListImage([FromBody] Product_ImageFilterDTO Product_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Skip = Product_ImageFilterDTO.Skip;
            ImageFilter.Take = Product_ImageFilterDTO.Take;
            ImageFilter.OrderBy = ImageOrder.Id;
            ImageFilter.OrderType = OrderType.ASC;
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Id = Product_ImageFilterDTO.Id;
            ImageFilter.Name = Product_ImageFilterDTO.Name;
            ImageFilter.Url = Product_ImageFilterDTO.Url;

            List<Image> Images = await ImageService.List(ImageFilter);
            List<Product_ImageDTO> Product_ImageDTOs = Images
                .Select(x => new Product_ImageDTO(x)).ToList();
            return Product_ImageDTOs;
        }
        [Route(ProductRoute.CountProductGrouping), HttpPost]
        public async Task<long> CountProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Description = Product_ProductGroupingFilterDTO.Description;

            return await ProductGroupingService.Count(ProductGroupingFilter);
        }

        [Route(ProductRoute.ListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> ListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = Product_ProductGroupingFilterDTO.Skip;
            ProductGroupingFilter.Take = Product_ProductGroupingFilterDTO.Take;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Description = Product_ProductGroupingFilterDTO.Description;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }
    }
}

