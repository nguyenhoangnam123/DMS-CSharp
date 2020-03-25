using Common;
using DMS.Entities;
using DMS.Services.MProductGrouping;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product_grouping
{
    public class ProductGroupingRoute : Root
    {
        public const string Master = Module + "/product-grouping/product-grouping-master";
        public const string Detail = Module + "/product-grouping/product-grouping-detail";
        private const string Default = Rpc + Module + "/product-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ProductGroupingFilter.Id), FieldType.ID },
            { nameof(ProductGroupingFilter.Code), FieldType.STRING },
            { nameof(ProductGroupingFilter.Name), FieldType.STRING },
            { nameof(ProductGroupingFilter.ParentId), FieldType.ID },
            { nameof(ProductGroupingFilter.Path), FieldType.STRING },
            { nameof(ProductGroupingFilter.Description), FieldType.STRING },
        };
    }

    public class ProductGroupingController : RpcController
    {
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ProductGroupingController(
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext
        )
        {
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


    }
}

