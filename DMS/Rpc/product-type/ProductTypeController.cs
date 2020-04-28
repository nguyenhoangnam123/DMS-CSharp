using Common;
using DMS.Entities;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product_type
{
    public class ProductTypeRoute : Root
    {
        public const string Master = Module + "/product-type/product-type-master";
        public const string Detail = Module + "/product-type/product-type-detail";
        private const string Default = Rpc + Module + "/product-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ProductTypeFilter.Id), FieldType.ID },
            { nameof(ProductTypeFilter.Code), FieldType.STRING },
            { nameof(ProductTypeFilter.Name), FieldType.STRING },
            { nameof(ProductTypeFilter.Description), FieldType.STRING },
            { nameof(ProductTypeFilter.StatusId), FieldType.ID },
            { nameof(ProductTypeFilter.UpdatedTime), FieldType.DATE },
        };
    }

    public class ProductTypeController : RpcController
    {
        private IStatusService StatusService;
        private IProductTypeService ProductTypeService;
        private ICurrentContext CurrentContext;
        public ProductTypeController(
            IStatusService StatusService,
            IProductTypeService ProductTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.StatusService = StatusService;
            this.ProductTypeService = ProductTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProductTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO);
            int count = await ProductTypeService.Count(ProductTypeFilter);
            return count;
        }

        [Route(ProductTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<ProductType_ProductTypeDTO>>> List([FromBody] ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO);
            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<ProductType_ProductTypeDTO> ProductType_ProductTypeDTOs = ProductTypes
                .Select(c => new ProductType_ProductTypeDTO(c)).ToList();
            return ProductType_ProductTypeDTOs;
        }

        [Route(ProductTypeRoute.Get), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Get([FromBody]ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = await ProductTypeService.Get(ProductType_ProductTypeDTO.Id);
            return new ProductType_ProductTypeDTO(ProductType);
        }

        [Route(ProductTypeRoute.Create), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Create([FromBody] ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = ConvertDTOToEntity(ProductType_ProductTypeDTO);
            ProductType = await ProductTypeService.Create(ProductType);
            ProductType_ProductTypeDTO = new ProductType_ProductTypeDTO(ProductType);
            if (ProductType.IsValidated)
                return ProductType_ProductTypeDTO;
            else
                return BadRequest(ProductType_ProductTypeDTO);
        }

        [Route(ProductTypeRoute.Update), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Update([FromBody] ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = ConvertDTOToEntity(ProductType_ProductTypeDTO);
            ProductType = await ProductTypeService.Update(ProductType);
            ProductType_ProductTypeDTO = new ProductType_ProductTypeDTO(ProductType);
            if (ProductType.IsValidated)
                return ProductType_ProductTypeDTO;
            else
                return BadRequest(ProductType_ProductTypeDTO);
        }

        [Route(ProductTypeRoute.Delete), HttpPost]
        public async Task<ActionResult<ProductType_ProductTypeDTO>> Delete([FromBody] ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ProductType_ProductTypeDTO.Id))
                return Forbid();

            ProductType ProductType = ConvertDTOToEntity(ProductType_ProductTypeDTO);
            ProductType = await ProductTypeService.Delete(ProductType);
            ProductType_ProductTypeDTO = new ProductType_ProductTypeDTO(ProductType);
            if (ProductType.IsValidated)
                return ProductType_ProductTypeDTO;
            else
                return BadRequest(ProductType_ProductTypeDTO);
        }

        [Route(ProductTypeRoute.Import), HttpPost]
        public async Task<ActionResult<List<ProductType_ProductTypeDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<ProductType> ProductTypes = await ProductTypeService.Import(DataFile);
            List<ProductType_ProductTypeDTO> ProductType_ProductTypeDTOs = ProductTypes
                .Select(c => new ProductType_ProductTypeDTO(c)).ToList();
            return ProductType_ProductTypeDTOs;
        }

        [Route(ProductTypeRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO);
            DataFile DataFile = await ProductTypeService.Export(ProductTypeFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(ProductTypeRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Id = new IdFilter { In = Ids };
            ProductTypeFilter.Selects = ProductTypeSelect.Id;
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = int.MaxValue;

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            ProductTypes = await ProductTypeService.BulkDelete(ProductTypes);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            if (Id == 0)
            {

            }
            else
            {
                ProductTypeFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductTypeService.Count(ProductTypeFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ProductType ConvertDTOToEntity(ProductType_ProductTypeDTO ProductType_ProductTypeDTO)
        {
            ProductType ProductType = new ProductType();
            ProductType.Id = ProductType_ProductTypeDTO.Id;
            ProductType.Code = ProductType_ProductTypeDTO.Code;
            ProductType.Name = ProductType_ProductTypeDTO.Name;
            ProductType.Description = ProductType_ProductTypeDTO.Description;
            ProductType.StatusId = ProductType_ProductTypeDTO.StatusId;
            ProductType.UpdatedTime = ProductType_ProductTypeDTO.UpdateTime;
            ProductType.Status = ProductType_ProductTypeDTO.Status == null ? null : new Status
            {
                Id = ProductType_ProductTypeDTO.Status.Id,
                Code = ProductType_ProductTypeDTO.Status.Code,
                Name = ProductType_ProductTypeDTO.Status.Name,
            };
            ProductType.BaseLanguage = CurrentContext.Language;
            return ProductType;
        }

        private ProductTypeFilter ConvertFilterDTOToFilterEntity(ProductType_ProductTypeFilterDTO ProductType_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Skip = ProductType_ProductTypeFilterDTO.Skip;
            ProductTypeFilter.Take = ProductType_ProductTypeFilterDTO.Take;
            ProductTypeFilter.OrderBy = ProductType_ProductTypeFilterDTO.OrderBy;
            ProductTypeFilter.OrderType = ProductType_ProductTypeFilterDTO.OrderType;

            ProductTypeFilter.Id = ProductType_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = ProductType_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = ProductType_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = ProductType_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = ProductType_ProductTypeFilterDTO.StatusId;
            ProductTypeFilter.UpdatedTime = ProductType_ProductTypeFilterDTO.UpdatedTime;
            return ProductTypeFilter;
        }

        [Route(ProductTypeRoute.SingleListStatus), HttpPost]
        public async Task<List<ProductType_StatusDTO>> SingleListStatus([FromBody] ProductType_StatusFilterDTO ProductType_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ProductType_StatusDTO> ProductType_StatusDTOs = Statuses
                .Select(x => new ProductType_StatusDTO(x)).ToList();
            return ProductType_StatusDTOs;
        }

    }
}

