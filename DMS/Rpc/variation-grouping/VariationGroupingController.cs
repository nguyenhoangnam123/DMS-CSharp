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
using DMS.Services.MVariationGrouping;
using DMS.Services.MProduct;

namespace DMS.Rpc.variation_grouping
{
    public class VariationGroupingRoute : Root
    {
        public const string Master = Module + "/variation-grouping/variation-grouping-master";
        public const string Detail = Module + "/variation-grouping/variation-grouping-detail";
        private const string Default = Rpc + Module + "/variation-grouping";
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
            { nameof(VariationGroupingFilter.Id), FieldType.ID },
            { nameof(VariationGroupingFilter.Name), FieldType.STRING },
            { nameof(VariationGroupingFilter.ProductId), FieldType.ID },
        };
    }

    public class VariationGroupingController : RpcController
    {
        private IProductService ProductService;
        private IVariationGroupingService VariationGroupingService;
        private ICurrentContext CurrentContext;
        public VariationGroupingController(
            IProductService ProductService,
            IVariationGroupingService VariationGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.ProductService = ProductService;
            this.VariationGroupingService = VariationGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(VariationGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] VariationGrouping_VariationGroupingFilterDTO VariationGrouping_VariationGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationGroupingFilter VariationGroupingFilter = ConvertFilterDTOToFilterEntity(VariationGrouping_VariationGroupingFilterDTO);
            VariationGroupingFilter = VariationGroupingService.ToFilter(VariationGroupingFilter);
            int count = await VariationGroupingService.Count(VariationGroupingFilter);
            return count;
        }

        [Route(VariationGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<VariationGrouping_VariationGroupingDTO>>> List([FromBody] VariationGrouping_VariationGroupingFilterDTO VariationGrouping_VariationGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationGroupingFilter VariationGroupingFilter = ConvertFilterDTOToFilterEntity(VariationGrouping_VariationGroupingFilterDTO);
            VariationGroupingFilter = VariationGroupingService.ToFilter(VariationGroupingFilter);
            List<VariationGrouping> VariationGroupings = await VariationGroupingService.List(VariationGroupingFilter);
            List<VariationGrouping_VariationGroupingDTO> VariationGrouping_VariationGroupingDTOs = VariationGroupings
                .Select(c => new VariationGrouping_VariationGroupingDTO(c)).ToList();
            return VariationGrouping_VariationGroupingDTOs;
        }

        [Route(VariationGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<VariationGrouping_VariationGroupingDTO>> Get([FromBody]VariationGrouping_VariationGroupingDTO VariationGrouping_VariationGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(VariationGrouping_VariationGroupingDTO.Id))
                return Forbid();

            VariationGrouping VariationGrouping = await VariationGroupingService.Get(VariationGrouping_VariationGroupingDTO.Id);
            return new VariationGrouping_VariationGroupingDTO(VariationGrouping);
        }

        [Route(VariationGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<VariationGrouping_VariationGroupingDTO>> Create([FromBody] VariationGrouping_VariationGroupingDTO VariationGrouping_VariationGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(VariationGrouping_VariationGroupingDTO.Id))
                return Forbid();

            VariationGrouping VariationGrouping = ConvertDTOToEntity(VariationGrouping_VariationGroupingDTO);
            VariationGrouping = await VariationGroupingService.Create(VariationGrouping);
            VariationGrouping_VariationGroupingDTO = new VariationGrouping_VariationGroupingDTO(VariationGrouping);
            if (VariationGrouping.IsValidated)
                return VariationGrouping_VariationGroupingDTO;
            else
                return BadRequest(VariationGrouping_VariationGroupingDTO);
        }

        [Route(VariationGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<VariationGrouping_VariationGroupingDTO>> Update([FromBody] VariationGrouping_VariationGroupingDTO VariationGrouping_VariationGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(VariationGrouping_VariationGroupingDTO.Id))
                return Forbid();

            VariationGrouping VariationGrouping = ConvertDTOToEntity(VariationGrouping_VariationGroupingDTO);
            VariationGrouping = await VariationGroupingService.Update(VariationGrouping);
            VariationGrouping_VariationGroupingDTO = new VariationGrouping_VariationGroupingDTO(VariationGrouping);
            if (VariationGrouping.IsValidated)
                return VariationGrouping_VariationGroupingDTO;
            else
                return BadRequest(VariationGrouping_VariationGroupingDTO);
        }

        [Route(VariationGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<VariationGrouping_VariationGroupingDTO>> Delete([FromBody] VariationGrouping_VariationGroupingDTO VariationGrouping_VariationGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(VariationGrouping_VariationGroupingDTO.Id))
                return Forbid();

            VariationGrouping VariationGrouping = ConvertDTOToEntity(VariationGrouping_VariationGroupingDTO);
            VariationGrouping = await VariationGroupingService.Delete(VariationGrouping);
            VariationGrouping_VariationGroupingDTO = new VariationGrouping_VariationGroupingDTO(VariationGrouping);
            if (VariationGrouping.IsValidated)
                return VariationGrouping_VariationGroupingDTO;
            else
                return BadRequest(VariationGrouping_VariationGroupingDTO);
        }

        [Route(VariationGroupingRoute.Import), HttpPost]
        public async Task<ActionResult<List<VariationGrouping_VariationGroupingDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<VariationGrouping> VariationGroupings = await VariationGroupingService.Import(DataFile);
            List<VariationGrouping_VariationGroupingDTO> VariationGrouping_VariationGroupingDTOs = VariationGroupings
                .Select(c => new VariationGrouping_VariationGroupingDTO(c)).ToList();
            return VariationGrouping_VariationGroupingDTOs;
        }

        [Route(VariationGroupingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] VariationGrouping_VariationGroupingFilterDTO VariationGrouping_VariationGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationGroupingFilter VariationGroupingFilter = ConvertFilterDTOToFilterEntity(VariationGrouping_VariationGroupingFilterDTO);
            VariationGroupingFilter = VariationGroupingService.ToFilter(VariationGroupingFilter);
            DataFile DataFile = await VariationGroupingService.Export(VariationGroupingFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }
        
        [Route(VariationGroupingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            VariationGroupingFilter VariationGroupingFilter = new VariationGroupingFilter();
            VariationGroupingFilter.Id = new IdFilter { In = Ids };
            VariationGroupingFilter.Selects = VariationGroupingSelect.Id;
            VariationGroupingFilter.Skip = 0;
            VariationGroupingFilter.Take = int.MaxValue;

            List<VariationGrouping> VariationGroupings = await VariationGroupingService.List(VariationGroupingFilter);
            VariationGroupings = await VariationGroupingService.BulkDelete(VariationGroupings);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            VariationGroupingFilter VariationGroupingFilter = new VariationGroupingFilter();
            VariationGroupingFilter = VariationGroupingService.ToFilter(VariationGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                VariationGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await VariationGroupingService.Count(VariationGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private VariationGrouping ConvertDTOToEntity(VariationGrouping_VariationGroupingDTO VariationGrouping_VariationGroupingDTO)
        {
            VariationGrouping VariationGrouping = new VariationGrouping();
            VariationGrouping.Id = VariationGrouping_VariationGroupingDTO.Id;
            VariationGrouping.Name = VariationGrouping_VariationGroupingDTO.Name;
            VariationGrouping.ProductId = VariationGrouping_VariationGroupingDTO.ProductId;
            VariationGrouping.Product = VariationGrouping_VariationGroupingDTO.Product == null ? null : new Product
            {
                Id = VariationGrouping_VariationGroupingDTO.Product.Id,
                Code = VariationGrouping_VariationGroupingDTO.Product.Code,
                SupplierCode = VariationGrouping_VariationGroupingDTO.Product.SupplierCode,
                Name = VariationGrouping_VariationGroupingDTO.Product.Name,
                Description = VariationGrouping_VariationGroupingDTO.Product.Description,
                ScanCode = VariationGrouping_VariationGroupingDTO.Product.ScanCode,
                ProductTypeId = VariationGrouping_VariationGroupingDTO.Product.ProductTypeId,
                SupplierId = VariationGrouping_VariationGroupingDTO.Product.SupplierId,
                BrandId = VariationGrouping_VariationGroupingDTO.Product.BrandId,
                UnitOfMeasureId = VariationGrouping_VariationGroupingDTO.Product.UnitOfMeasureId,
                UnitOfMeasureGroupingId = VariationGrouping_VariationGroupingDTO.Product.UnitOfMeasureGroupingId,
                SalePrice = VariationGrouping_VariationGroupingDTO.Product.SalePrice,
                RetailPrice = VariationGrouping_VariationGroupingDTO.Product.RetailPrice,
                TaxTypeId = VariationGrouping_VariationGroupingDTO.Product.TaxTypeId,
                StatusId = VariationGrouping_VariationGroupingDTO.Product.StatusId,
            };
            VariationGrouping.BaseLanguage = CurrentContext.Language;
            return VariationGrouping;
        }

        private VariationGroupingFilter ConvertFilterDTOToFilterEntity(VariationGrouping_VariationGroupingFilterDTO VariationGrouping_VariationGroupingFilterDTO)
        {
            VariationGroupingFilter VariationGroupingFilter = new VariationGroupingFilter();
            VariationGroupingFilter.Selects = VariationGroupingSelect.ALL;
            VariationGroupingFilter.Skip = VariationGrouping_VariationGroupingFilterDTO.Skip;
            VariationGroupingFilter.Take = VariationGrouping_VariationGroupingFilterDTO.Take;
            VariationGroupingFilter.OrderBy = VariationGrouping_VariationGroupingFilterDTO.OrderBy;
            VariationGroupingFilter.OrderType = VariationGrouping_VariationGroupingFilterDTO.OrderType;

            VariationGroupingFilter.Id = VariationGrouping_VariationGroupingFilterDTO.Id;
            VariationGroupingFilter.Name = VariationGrouping_VariationGroupingFilterDTO.Name;
            VariationGroupingFilter.ProductId = VariationGrouping_VariationGroupingFilterDTO.ProductId;
            return VariationGroupingFilter;
        }

        [Route(VariationGroupingRoute.SingleListProduct), HttpPost]
        public async Task<List<VariationGrouping_ProductDTO>> SingleListProduct([FromBody] VariationGrouping_ProductFilterDTO VariationGrouping_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Skip = 0;
            ProductFilter.Take = 20;
            ProductFilter.OrderBy = ProductOrder.Id;
            ProductFilter.OrderType = OrderType.ASC;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter.Id = VariationGrouping_ProductFilterDTO.Id;
            ProductFilter.Code = VariationGrouping_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = VariationGrouping_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = VariationGrouping_ProductFilterDTO.Name;
            ProductFilter.Description = VariationGrouping_ProductFilterDTO.Description;
            ProductFilter.ScanCode = VariationGrouping_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = VariationGrouping_ProductFilterDTO.ProductTypeId;
            ProductFilter.SupplierId = VariationGrouping_ProductFilterDTO.SupplierId;
            ProductFilter.BrandId = VariationGrouping_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = VariationGrouping_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = VariationGrouping_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = VariationGrouping_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = VariationGrouping_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = VariationGrouping_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = VariationGrouping_ProductFilterDTO.StatusId;

            List<Product> Products = await ProductService.List(ProductFilter);
            List<VariationGrouping_ProductDTO> VariationGrouping_ProductDTOs = Products
                .Select(x => new VariationGrouping_ProductDTO(x)).ToList();
            return VariationGrouping_ProductDTOs;
        }

    }
}

