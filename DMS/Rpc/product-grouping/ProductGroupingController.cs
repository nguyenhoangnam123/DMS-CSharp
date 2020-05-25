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
using DMS.Services.MProductGrouping;
using DMS.Services.MProduct;
using DMS.Enums;

namespace DMS.Rpc.product_grouping
{
    

    public class ProductGroupingController : RpcController
    {
        private IProductService ProductService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ProductGroupingController(
            IProductService ProductService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.ProductService = ProductService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProductGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO);
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            int count = await ProductGroupingService.Count(ProductGroupingFilter);
            return count;
        }

        [Route(ProductGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<ProductGrouping_ProductGroupingDTO>>> List([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO);
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ProductGrouping_ProductGroupingDTO> ProductGrouping_ProductGroupingDTOs = ProductGroupings
                .Select(c => new ProductGrouping_ProductGroupingDTO(c)).ToList();
            return ProductGrouping_ProductGroupingDTOs;
        }

        [Route(ProductGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Get([FromBody]ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = await ProductGroupingService.Get(ProductGrouping_ProductGroupingDTO.Id);
            return new ProductGrouping_ProductGroupingDTO(ProductGrouping);
        }

        [Route(ProductGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Create([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO);
            ProductGrouping = await ProductGroupingService.Create(ProductGrouping);
            ProductGrouping_ProductGroupingDTO = new ProductGrouping_ProductGroupingDTO(ProductGrouping);
            if (ProductGrouping.IsValidated)
                return ProductGrouping_ProductGroupingDTO;
            else
                return BadRequest(ProductGrouping_ProductGroupingDTO);
        }

        [Route(ProductGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Update([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO);
            ProductGrouping = await ProductGroupingService.Update(ProductGrouping);
            ProductGrouping_ProductGroupingDTO = new ProductGrouping_ProductGroupingDTO(ProductGrouping);
            if (ProductGrouping.IsValidated)
                return ProductGrouping_ProductGroupingDTO;
            else
                return BadRequest(ProductGrouping_ProductGroupingDTO);
        }

        [Route(ProductGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<ProductGrouping_ProductGroupingDTO>> Delete([FromBody] ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductGrouping_ProductGroupingDTO.Id))
                return Forbid();

            ProductGrouping ProductGrouping = ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO);
            ProductGrouping = await ProductGroupingService.Delete(ProductGrouping);
            ProductGrouping_ProductGroupingDTO = new ProductGrouping_ProductGroupingDTO(ProductGrouping);
            if (ProductGrouping.IsValidated)
                return ProductGrouping_ProductGroupingDTO;
            else
                return BadRequest(ProductGrouping_ProductGroupingDTO);
        }

        [Route(ProductGroupingRoute.Import), HttpPost]
        public async Task<ActionResult<List<ProductGrouping_ProductGroupingDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.Import(DataFile);
            List<ProductGrouping_ProductGroupingDTO> ProductGrouping_ProductGroupingDTOs = ProductGroupings
                .Select(c => new ProductGrouping_ProductGroupingDTO(c)).ToList();
            return ProductGrouping_ProductGroupingDTOs;
        }

        [Route(ProductGroupingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO);
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            DataFile DataFile = await ProductGroupingService.Export(ProductGroupingFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(ProductGroupingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            ProductGroupingFilter.Id = new IdFilter { In = Ids };
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id;
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            ProductGroupings = await ProductGroupingService.BulkDelete(ProductGroupings);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter = ProductGroupingService.ToFilter(ProductGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProductGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductGroupingService.Count(ProductGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ProductGrouping ConvertDTOToEntity(ProductGrouping_ProductGroupingDTO ProductGrouping_ProductGroupingDTO)
        {
            ProductGrouping ProductGrouping = new ProductGrouping();
            ProductGrouping.Id = ProductGrouping_ProductGroupingDTO.Id;
            ProductGrouping.Code = ProductGrouping_ProductGroupingDTO.Code;
            ProductGrouping.Name = ProductGrouping_ProductGroupingDTO.Name;
            ProductGrouping.ParentId = ProductGrouping_ProductGroupingDTO.ParentId;
            ProductGrouping.Path = ProductGrouping_ProductGroupingDTO.Path;
            ProductGrouping.Description = ProductGrouping_ProductGroupingDTO.Description;
            ProductGrouping.Parent = ProductGrouping_ProductGroupingDTO.Parent == null ? null : new ProductGrouping
            {
                Id = ProductGrouping_ProductGroupingDTO.Parent.Id,
                Code = ProductGrouping_ProductGroupingDTO.Parent.Code,
                Name = ProductGrouping_ProductGroupingDTO.Parent.Name,
                ParentId = ProductGrouping_ProductGroupingDTO.Parent.ParentId,
                Path = ProductGrouping_ProductGroupingDTO.Parent.Path,
                Description = ProductGrouping_ProductGroupingDTO.Parent.Description,
            };
            ProductGrouping.ProductProductGroupingMappings = ProductGrouping_ProductGroupingDTO.ProductProductGroupingMappings?
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductId = x.ProductId,
                    Product = new Product
                    {
                        Id = x.Product.Id,
                        Code = x.Product.Code,
                        SupplierCode = x.Product.SupplierCode,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ScanCode = x.Product.ScanCode,
                        ProductTypeId = x.Product.ProductTypeId,
                        SupplierId = x.Product.SupplierId,
                        BrandId = x.Product.BrandId,
                        UnitOfMeasureId = x.Product.UnitOfMeasureId,
                        UnitOfMeasureGroupingId = x.Product.UnitOfMeasureGroupingId,
                        SalePrice = x.Product.SalePrice,
                        RetailPrice = x.Product.RetailPrice,
                        TaxTypeId = x.Product.TaxTypeId,
                        StatusId = x.Product.StatusId,
                        OtherName = x.Product.OtherName,
                        TechnicalName = x.Product.TechnicalName,
                        Note = x.Product.Note,
                    },
                }).ToList();
            ProductGrouping.BaseLanguage = CurrentContext.Language;
            return ProductGrouping;
        }

        private ProductGroupingFilter ConvertFilterDTOToFilterEntity(ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = 99999;
            ProductGroupingFilter.OrderBy = ProductGrouping_ProductGroupingFilterDTO.OrderBy;
            ProductGroupingFilter.OrderType = ProductGrouping_ProductGroupingFilterDTO.OrderType;

            ProductGroupingFilter.Id = ProductGrouping_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = ProductGrouping_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = ProductGrouping_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = ProductGrouping_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = ProductGrouping_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Description = ProductGrouping_ProductGroupingFilterDTO.Description;
            return ProductGroupingFilter;
        }

        [Route(ProductGroupingRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<ProductGrouping_ProductGroupingDTO>> SingleListProductGrouping([FromBody] ProductGrouping_ProductGroupingFilterDTO ProductGrouping_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = 99999;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code 
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;
 
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ProductGrouping_ProductGroupingDTO> ProductGrouping_ProductGroupingDTOs = ProductGroupings
                .Select(x => new ProductGrouping_ProductGroupingDTO(x)).ToList();
            return ProductGrouping_ProductGroupingDTOs;
        }
        [Route(ProductGroupingRoute.SingleListProduct), HttpPost]
        public async Task<List<ProductGrouping_ProductDTO>> SingleListProduct([FromBody] ProductGrouping_ProductFilterDTO ProductGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = 0;
            ProductFilter.Take = 20;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = ProductGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = ProductGrouping_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = ProductGrouping_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = ProductGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = ProductGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = ProductGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            List<Product> Products = await ProductService.List(ProductFilter);
            List<ProductGrouping_ProductDTO> ProductGrouping_ProductDTOs = Products
                .Select(x => new ProductGrouping_ProductDTO(x)).ToList();
            return ProductGrouping_ProductDTOs;
        }

        [Route(ProductGroupingRoute.CountProduct), HttpPost]
        public async Task<long> CountProduct([FromBody] ProductGrouping_ProductFilterDTO ProductGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = ProductGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = ProductGrouping_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = ProductGrouping_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = ProductGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = ProductGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = ProductGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = ProductGrouping_ProductFilterDTO.ProductTypeId;
            ProductFilter.SupplierId = ProductGrouping_ProductFilterDTO.SupplierId;
            ProductFilter.BrandId = ProductGrouping_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = ProductGrouping_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = ProductGrouping_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = ProductGrouping_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = ProductGrouping_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = ProductGrouping_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = ProductGrouping_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = ProductGrouping_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = ProductGrouping_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = ProductGrouping_ProductFilterDTO.Note;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await ProductService.Count(ProductFilter);
        }

        [Route(ProductGroupingRoute.ListProduct), HttpPost]
        public async Task<List<ProductGrouping_ProductDTO>> ListProduct([FromBody] ProductGrouping_ProductFilterDTO ProductGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = ProductGrouping_ProductFilterDTO.Skip;
            ProductFilter.Take = ProductGrouping_ProductFilterDTO.Take;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = ProductGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = ProductGrouping_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = ProductGrouping_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = ProductGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = ProductGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = ProductGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = ProductGrouping_ProductFilterDTO.ProductTypeId;
            ProductFilter.SupplierId = ProductGrouping_ProductFilterDTO.SupplierId;
            ProductFilter.BrandId = ProductGrouping_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = ProductGrouping_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = ProductGrouping_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = ProductGrouping_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = ProductGrouping_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = ProductGrouping_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = ProductGrouping_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = ProductGrouping_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = ProductGrouping_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = ProductGrouping_ProductFilterDTO.Note;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Product> Products = await ProductService.List(ProductFilter);
            List<ProductGrouping_ProductDTO> ProductGrouping_ProductDTOs = Products
                .Select(x => new ProductGrouping_ProductDTO(x)).ToList();
            return ProductGrouping_ProductDTOs;
        }
    }
}

