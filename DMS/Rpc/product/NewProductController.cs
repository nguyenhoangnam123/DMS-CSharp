﻿using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MBrand;
using DMS.Services.MCategory;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product
{


    public class NewProductController : RpcController
    {
        private readonly IWebHostEnvironment _env;

        private IProductTypeService ProductTypeService;
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private IProductGroupingService ProductGroupingService;
        private IProductService ProductService;
        private IBrandService BrandService;
        private ICategoryService CategoryService;
        private ICurrentContext CurrentContext;
        public NewProductController(
            IWebHostEnvironment env,
            IProductTypeService ProductTypeService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            IProductGroupingService ProductGroupingService,
            IProductService ProductService,
            IBrandService BrandService,
            ICategoryService CategoryService,
            ICurrentContext CurrentContext
        )
        {
            _env = env;
            this.ProductTypeService = ProductTypeService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductService = ProductService;
            this.BrandService = BrandService;
            this.CategoryService = CategoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NewProductRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            int count = await ProductService.Count(ProductFilter);
            return count;
        }

        [Route(NewProductRoute.List), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> List([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            Product_ProductDTOs.ForEach(p => p.Used = false);
            return Product_ProductDTOs;
        }

        [Route(NewProductRoute.Get), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Get([FromBody]Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = await ProductService.Get(Product_ProductDTO.Id);
            return new Product_ProductDTO(Product);
        }

        [Route(NewProductRoute.Create), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> Create([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = new IdFilter { In = Ids };
            ProductFilter.Selects = ProductSelect.Id;
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;

            List<Product> Products = await ProductService.List(ProductFilter);

            Products = await ProductService.BulkInsertNewProduct(Products);
            if (Products.Any(x => !x.IsValidated))
                return BadRequest(Products.Where(x => !x.IsValidated));
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(NewProductRoute.Delete), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Delete([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            List<Product> Products = new List<Product> { Product };
            Products = await ProductService.BulkDeleteNewProduct(Products);
            if (Products.Any(x => !x.IsValidated))
                return BadRequest(Products.Where(x => !x.IsValidated));
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs.FirstOrDefault();
        }

        [Route(NewProductRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = new IdFilter { In = Ids };
            ProductFilter.Selects = ProductSelect.Id;
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;

            List<Product> Products = await ProductService.List(ProductFilter);

            Products = await ProductService.BulkDeleteNewProduct(Products);
            if (Products.Any(x => !x.IsValidated))
                return BadRequest(Products.Where(x => !x.IsValidated));
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
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
            Product.ERPCode = Product_ProductDTO.ERPCode;
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
            Product.IsNew = Product_ProductDTO.IsNew;
            Product.Brand = Product_ProductDTO.Brand == null ? null : new Brand
            {
                Id = Product_ProductDTO.Brand.Id,
                Code = Product_ProductDTO.Brand.Code,
                Name = Product_ProductDTO.Brand.Name,
                Description = Product_ProductDTO.Brand.Description,
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
                    StatusId = x.StatusId,
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
                        ThumbnailUrl = x.Image.ThumbnailUrl,
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
                    Variations = x.Variations.Select(v => new Variation
                    {
                        Id = v.Id,
                        Code = v.Code,
                        Name = v.Name,
                        VariationGroupingId = x.Id
                    }).ToList()
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
            ProductFilter.CategoryId = Product_ProductFilterDTO.CategoryId;
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
            ProductFilter.Search = Product_ProductFilterDTO.Search;
            ProductFilter.ProductGroupingId = Product_ProductFilterDTO.ProductGroupingId;
            ProductFilter.IsNew = true;
            return ProductFilter;
        }

        [Route(NewProductRoute.FilterListProductType), HttpPost]
        public async Task<List<Product_ProductTypeDTO>> FilterListProductType([FromBody] Product_ProductTypeFilterDTO Product_ProductTypeFilterDTO)
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
        [Route(NewProductRoute.FilterListStatus), HttpPost]
        public async Task<List<Product_StatusDTO>> FilterListStatus([FromBody] Product_StatusFilterDTO Product_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Product_StatusDTO> Product_StatusDTOs = Statuses
                .Select(x => new Product_StatusDTO(x)).ToList();
            return Product_StatusDTOs;
        }
        [Route(NewProductRoute.FilterListSupplier), HttpPost]
        public async Task<List<Product_SupplierDTO>> FilterListSupplier([FromBody] Product_SupplierFilterDTO Product_SupplierFilterDTO)
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
        [Route(NewProductRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> FilterListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }
        [Route(NewProductRoute.FilterListBrand), HttpPost]
        public async Task<List<Product_BrandDTO>> FilterListBrand([FromBody] Product_BrandFilterDTO Product_BrandFilterDTO)
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
            BrandFilter.Description = Product_BrandFilterDTO.Description;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            BrandFilter.UpdateTime = Product_BrandFilterDTO.UpdateTime;

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Product_BrandDTO> Product_BrandDTOs = Brands
                .Select(x => new Product_BrandDTO(x)).ToList();
            return Product_BrandDTOs;
        }
        [Route(NewProductRoute.FilterListCategory), HttpPost]
        public async Task<List<Product_CategoryDTO>> FilterListCategory([FromBody] Product_CategoryFilterDTO Product_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;
            CategoryFilter.OrderBy = CategoryOrder.Id;
            CategoryFilter.OrderType = OrderType.ASC;
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Id = Product_CategoryFilterDTO.Id;
            CategoryFilter.Code = Product_CategoryFilterDTO.Code;
            CategoryFilter.Name = Product_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Product_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Product_CategoryFilterDTO.Path;
            CategoryFilter.Level = Product_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Product_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Product_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Product_CategoryFilterDTO.RowId;

            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Product_CategoryDTO> Product_CategoryDTOs = Categories
                .Select(x => new Product_CategoryDTO(x)).ToList();
            return Product_CategoryDTOs;
        }


        [Route(NewProductRoute.CountProduct), HttpPost]
        public async Task<long> CountProduct([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = Product_ProductFilterDTO.Id;
            ProductFilter.Code = Product_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = Product_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = Product_ProductFilterDTO.Name;
            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.ScanCode = Product_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = Product_ProductFilterDTO.ProductTypeId;
            ProductFilter.ProductGroupingId = Product_ProductFilterDTO.ProductGroupingId;
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
            ProductFilter.IsNew = false;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await ProductService.Count(ProductFilter);
        }

        [Route(NewProductRoute.ListProduct), HttpPost]
        public async Task<List<Product_ProductDTO>> ListProduct([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = Product_ProductFilterDTO.Skip;
            ProductFilter.Take = Product_ProductFilterDTO.Take;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = Product_ProductFilterDTO.Id;
            ProductFilter.Code = Product_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = Product_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = Product_ProductFilterDTO.Name;
            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.ScanCode = Product_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = Product_ProductFilterDTO.ProductTypeId;
            ProductFilter.ProductGroupingId = Product_ProductFilterDTO.ProductGroupingId;
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
            ProductFilter.IsNew = false;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(x => new Product_ProductDTO(x)).ToList();
            return Product_ProductDTOs;
        }
    }
}

